using Audition.GameServer.Host.Configuration;
using Audition.GameServer.Domain.Models;
using Microsoft.Extensions.Options;
using MySqlConnector;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Audition.GameServer.Host.Services;

public sealed class GameServerListenerService(
    ILogger<GameServerListenerService> logger,
    IOptions<GameServerOptions> options,
    IConfiguration configuration,
    RuntimePaths runtimePaths) : BackgroundService
{
    private static readonly ushort[] LegacyDefaultAvatarItems = [6, 106, 140, 187, 217];
    private static readonly JsonSerializerOptions SyntheticPartnerJsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };
    private TcpListener? _listener;
    private readonly ConcurrentDictionary<int, Task> _sessions = new();
    private int _nextSessionId;
    private int _nextSyntheticRoomNumber;
    private int _nextSyntheticPartnerIndex;
    private readonly string? _databaseConnectionString = BuildDatabaseConnectionString(configuration);
    private readonly RuntimePaths _runtimePaths = runtimePaths;
    private readonly string _syntheticPartnerSettingsPath = Path.Combine(runtimePaths.DataPath, "SyntheticPartnerSettings.json");
    private readonly IReadOnlyList<SyntheticPartnerProfile> _syntheticPartnerProfiles = LoadSyntheticPartnerProfiles(
        Path.Combine(runtimePaths.DataPath, "SyntheticPartners.json"),
        logger);
    private readonly Lazy<Dictionary<string, ushort>> _musicTotalMeasures = new(
        () => LoadMusicTotalMeasures(Path.Combine(runtimePaths.ScriptPath, "music.slk")),
        true);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ListenerOptions listenerOptions = options.Value.Listener;
        IPAddress listenAddress = ResolveAddress(listenerOptions.Host);

        try
        {
            _listener = new TcpListener(listenAddress, listenerOptions.Port);
            _listener.Start();
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
        {
            logger.LogWarning(
                "Bootstrap listener skipped because {Host}:{Port} is already in use. The host will continue without accepting client connections yet.",
                listenerOptions.Host,
                listenerOptions.Port);
            _listener = null;
            return;
        }

        logger.LogInformation(
            "Bootstrap listener active on {Host}:{Port}",
            listenerOptions.Host,
            listenerOptions.Port);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync(stoppingToken);
                int sessionId = Interlocked.Increment(ref _nextSessionId);
                logger.LogInformation(
                    "Bootstrap listener accepted connection from {RemoteEndPoint} as session {SessionId}",
                    client.Client.RemoteEndPoint?.ToString() ?? "unknown",
                    sessionId);
                logger.LogInformation(
                    "[AUDITION FLOW] ENTRAR_SALA_CONNECT | SessionId={SessionId} | RemoteEndPoint={RemoteEndPoint}",
                    sessionId,
                    client.Client.RemoteEndPoint?.ToString() ?? "unknown");

                Task sessionTask = HandleClientAsync(sessionId, client, stoppingToken);
                _sessions[sessionId] = sessionTask;
                _ = sessionTask.ContinueWith(
                    _ => _sessions.TryRemove(sessionId, out Task? _),
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        finally
        {
            _listener?.Stop();
            _listener = null;
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _listener?.Stop();
        return base.StopAsync(cancellationToken);
    }

    private async Task HandleClientAsync(int sessionId, TcpClient client, CancellationToken stoppingToken)
    {
        using var _ = client;
        client.NoDelay = true;
        var sessionState = new GameClientSessionState();

        try
        {
            using NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4096];
            List<byte> pendingBytes = new();

            while (!stoppingToken.IsCancellationRequested)
            {
                int read = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), stoppingToken);
                if (read == 0)
                {
                    logger.LogInformation(
                        "Game client session {SessionId} disconnected from {RemoteEndPoint}",
                        sessionId,
                        client.Client.RemoteEndPoint?.ToString() ?? "unknown");
                    logger.LogInformation(
                        "[AUDITION FLOW] ENTRAR_SALA_DISCONNECT | SessionId={SessionId} | RemoteEndPoint={RemoteEndPoint} | RawLoginHandled={RawLoginHandled} | KeyExchangeHandled={KeyExchangeHandled}",
                        sessionId,
                        client.Client.RemoteEndPoint?.ToString() ?? "unknown",
                        sessionState.RawLoginHandled,
                        sessionState.KeyExchangeHandled);
                    break;
                }

                pendingBytes.AddRange(buffer.AsSpan(0, read).ToArray());

                while (TryDequeueFrame(pendingBytes, out byte[] frame))
                {
                    logger.LogInformation(
                        "Game client session {SessionId} received frame {Length} bytes from {RemoteEndPoint} | Hex={PayloadHex}",
                        sessionId,
                        frame.Length,
                        client.Client.RemoteEndPoint?.ToString() ?? "unknown",
                        Convert.ToHexString(frame));

                    if (!sessionState.RawLoginHandled)
                    {
                        if (TryDecryptTafFrame(sessionState.ReceiveCipher, frame, out byte[] plainTafFrame))
                        {
                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_BOOTSTRAP_TAF_FRAME | SessionId={SessionId} | PlainHex={PayloadHex}",
                                sessionId,
                                Convert.ToHexString(plainTafFrame));

                            if (TryParseLoginHandshakeRequest(plainTafFrame))
                            {
                                logger.LogInformation(
                                    "[AUDITION FLOW] ENTRAR_SALA_REQUEST_01_05 | SessionId={SessionId} | PlainHex={PayloadHex}",
                                    sessionId,
                                    Convert.ToHexString(plainTafFrame));

                                byte[] responseFrame = BuildLoginHandshakeResponse(sessionState);
                                LoginHandshakeState handshake = sessionState.Handshake!;
                                await stream.WriteAsync(responseFrame, stoppingToken);
                                await stream.FlushAsync(stoppingToken);
                                logger.LogInformation(
                                    "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_01_05 | SessionId={SessionId} | Prime={Prime} | Generator={Generator} | PublicKey={PublicKey} | Hex={PayloadHex}",
                                    sessionId,
                                    handshake.Prime,
                                    handshake.Generator,
                                    handshake.PublicKey,
                                    Convert.ToHexString(responseFrame));
                                sessionState.RawLoginHandled = true;
                                continue;
                            }
                        }

                        if (!TryDecryptRawFrame(frame, out byte[] plainFrame))
                        {
                            logger.LogWarning(
                                "[AUDITION FLOW] ENTRAR_SALA_BOOTSTRAP_DECRYPT_FAIL | SessionId={SessionId} | Hex={PayloadHex}",
                                sessionId,
                                Convert.ToHexString(frame));
                            continue;
                        }

                        if (TryParseInitialLoginRequest(plainFrame, out InitialLoginRequest loginRequest))
                        {
                            logger.LogInformation(
                                "Game client session {SessionId} bootstrap login | Header={HeaderA:X2}/{HeaderB:X2} | User={UserId} | Version={Version}",
                                sessionId,
                                loginRequest.HeaderA,
                                loginRequest.HeaderB,
                                loginRequest.UserId,
                                loginRequest.Version);
                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_REQUEST | SessionId={SessionId} | User={UserId} | Version={Version} | PlainHex={PayloadHex}",
                                sessionId,
                                loginRequest.UserId,
                                loginRequest.Version,
                                Convert.ToHexString(plainFrame));

                            byte[] responseFrame = BuildLoginHandshakeResponse(sessionState);
                            LoginHandshakeState handshake = sessionState.Handshake!;
                            await stream.WriteAsync(responseFrame, stoppingToken);
                            await stream.FlushAsync(stoppingToken);
                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_01_05_RAW | SessionId={SessionId} | Prime={Prime} | Generator={Generator} | PublicKey={PublicKey} | Hex={PayloadHex}",
                                sessionId,
                                handshake.Prime,
                                handshake.Generator,
                                handshake.PublicKey,
                                Convert.ToHexString(responseFrame));
                            sessionState.RawLoginHandled = true;
                        }
                        else
                        {
                            logger.LogWarning(
                                "[AUDITION FLOW] ENTRAR_SALA_REQUEST_UNKNOWN | SessionId={SessionId} | PlainHex={PayloadHex}",
                                sessionId,
                                Convert.ToHexString(plainFrame));
                        }

                        continue;
                    }

                    if (!TryDecryptTafFrame(sessionState.ReceiveCipher, frame, out byte[] plainEncryptedFrame))
                    {
                        logger.LogWarning(
                            "[AUDITION FLOW] ENTRAR_SALA_TAF_DECRYPT_FAIL | SessionId={SessionId} | Hex={PayloadHex}",
                            sessionId,
                            Convert.ToHexString(frame));
                        continue;
                    }

                    logger.LogInformation(
                        "[AUDITION FLOW] ENTRAR_SALA_TAF_FRAME | SessionId={SessionId} | PlainHex={PayloadHex}",
                        sessionId,
                        Convert.ToHexString(plainEncryptedFrame));

                    if (!sessionState.KeyExchangeHandled &&
                        TryParseKeyExchangeRequest(plainEncryptedFrame, out KeyExchangeRequest keyExchangeRequest))
                    {
                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_REQUEST_01_06 | SessionId={SessionId} | ClientPublicKey={ClientPublicKey} | ClientHash={ClientHash}",
                            sessionId,
                            keyExchangeRequest.ClientPublicKey,
                            keyExchangeRequest.ClientHashHex);
                        bool accepted = TryHandleKeyExchange(
                            sessionState,
                            keyExchangeRequest,
                            out byte[] responseFrame,
                            out string sharedKeyHex,
                            out string cryptKey,
                            out string expectedMd5);
                        await stream.WriteAsync(responseFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "Game client session {SessionId} key exchange | Accepted={Accepted} | SharedKey={SharedKey} | CryptKey={CryptKey} | ExpectedHash={ExpectedHash}",
                            sessionId,
                            accepted,
                            sharedKeyHex,
                            cryptKey,
                            expectedMd5);
                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_01_06 | SessionId={SessionId} | Accepted={Accepted} | SharedKey={SharedKey} | CryptKey={CryptKey} | ExpectedHash={ExpectedHash} | Hex={PayloadHex}",
                            sessionId,
                            accepted,
                            sharedKeyHex,
                            cryptKey,
                            expectedMd5,
                            Convert.ToHexString(responseFrame));

                        sessionState.KeyExchangeHandled = accepted;
                        continue;
                    }

                    if (sessionState.KeyExchangeHandled &&
                        !sessionState.ApplicationLoginHandled &&
                        TryParseGameLoginRequest(plainEncryptedFrame, out GameLoginRequest gameLoginRequest))
                    {
                        sessionState.UserSerial = gameLoginRequest.UserSerial;
                        sessionState.ClientVersion = gameLoginRequest.ClientVersion;
                        await PopulateSessionProfileAsync(sessionState, stoppingToken);
                        EnsureDefaultAvatarItems(sessionState);
                        sessionState.ApplicationLoginHandled = true;

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_REQUEST_LOGIN | SessionId={SessionId} | LoginMode={LoginMode} | UserSN={UserSN} | ClientVersion={ClientVersion} | Nickname={Nickname}",
                            sessionId,
                            gameLoginRequest.LoginMode,
                            gameLoginRequest.UserSerial,
                            gameLoginRequest.ClientVersion,
                            sessionState.UserNickname);

                        byte[] responseFrame = BuildGameLoginResponse(sessionState.UserNickname, sessionState.SendCipher);
                        await stream.WriteAsync(responseFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_LOGIN | SessionId={SessionId} | UserSN={UserSN} | Nickname={Nickname} | Hex={PayloadHex}",
                            sessionId,
                            gameLoginRequest.UserSerial,
                            sessionState.UserNickname,
                            Convert.ToHexString(responseFrame));

                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseSelfUserInfoRequest(plainEncryptedFrame))
                    {
                        if (!sessionState.AvatarItemListsLoaded)
                        {
                            byte[] avatarItemsStartFrame = BuildAvatarItemListsStartResponse(0, sessionState.SendCipher);
                            byte[] avatarItemsEndFrame = BuildAvatarItemListsEndResponse(sessionState.SendCipher);
                            await stream.WriteAsync(avatarItemsStartFrame, stoppingToken);
                            await stream.WriteAsync(avatarItemsEndFrame, stoppingToken);
                            sessionState.AvatarItemListsLoaded = true;

                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_AVATAR_ITEMS | SessionId={SessionId} | StartHex={StartHex} | EndHex={EndHex}",
                                sessionId,
                                Convert.ToHexString(avatarItemsStartFrame),
                                Convert.ToHexString(avatarItemsEndFrame));
                        }

                        byte[] responseFrame = BuildSelfUserInfoResponse(sessionState, sessionState.SendCipher);
                        await stream.WriteAsync(responseFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_USERINFO_SELF | SessionId={SessionId} | UserSN={UserSN} | Nickname={Nickname} | Hex={PayloadHex}",
                            sessionId,
                            sessionState.UserSerial,
                            sessionState.UserNickname,
                            Convert.ToHexString(responseFrame));

                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseRequestedUserInfoRequest(plainEncryptedFrame, out byte requestedUserInfoType, out string requestedUserInfoNickname))
                    {
                        if (!TryResolveUserProfile(sessionState, requestedUserInfoNickname, out UserProfileSnapshot profileSnapshot))
                        {
                            byte[] errorFrame = BuildUserInfoErrorResponse(sessionState.SendCipher);
                            await stream.WriteAsync(errorFrame, stoppingToken);
                            await stream.FlushAsync(stoppingToken);

                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_USERINFO_ERROR | SessionId={SessionId} | RequestType={RequestType} | RequestedNickname={RequestedNickname}",
                                sessionId,
                                requestedUserInfoType,
                                requestedUserInfoNickname);
                            continue;
                        }

                        byte[] responseFrame = requestedUserInfoType == 0x04
                            ? BuildRoomUserInfoResponse(profileSnapshot, sessionState.SendCipher)
                            : BuildRequestedUserInfoResponse(profileSnapshot, sessionState.SendCipher);
                        await stream.WriteAsync(responseFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_USERINFO_REQUESTED | SessionId={SessionId} | RequestType={RequestType} | RequestedNickname={RequestedNickname} | Hex={PayloadHex}",
                            sessionId,
                            requestedUserInfoType,
                            requestedUserInfoNickname,
                            Convert.ToHexString(responseFrame));
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseUserAvatarInfoRequest(plainEncryptedFrame, out string requestedNickname))
                    {
                        EnsureDefaultAvatarItems(sessionState);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_REQUEST_AVATARINFO | SessionId={SessionId} | RequestedNickname={RequestedNickname}",
                            sessionId,
                            requestedNickname);

                        byte[] responseFrame = BuildUserAvatarInfoResponse(sessionState, requestedNickname, sessionState.SendCipher);
                        await stream.WriteAsync(responseFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_AVATARINFO | SessionId={SessionId} | RequestedNickname={RequestedNickname} | DefaultItems={DefaultItems} | EquippedItems={EquippedItems} | Hex={PayloadHex}",
                            sessionId,
                            requestedNickname,
                            string.Join(',', ResolveAvatarItems(sessionState, requestedNickname)),
                            string.Join(',', ResolveEquippedAvatarItems(sessionState, requestedNickname)),
                            Convert.ToHexString(responseFrame));

                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseAvatarInventoryRequest(plainEncryptedFrame))
                    {
                        byte[] responseFrame = BuildAvatarInventoryResponse(sessionState, sessionState.SendCipher);
                        await stream.WriteAsync(responseFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_AVATAR_INVENTORY | SessionId={SessionId} | GameMoney={GameMoney} | Cash={Cash} | Hex={PayloadHex}",
                            sessionId,
                            sessionState.UserGameMoney,
                            sessionState.UserGameCash,
                            Convert.ToHexString(responseFrame));

                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseDefaultAvatarSelectionRequest(plainEncryptedFrame, out ushort[] avatarDefaults))
                    {
                        Array.Copy(avatarDefaults, sessionState.DefaultAvatarItems, avatarDefaults.Length);
                        SyncEquippedAvatarItemsFromDefaults(sessionState);
                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_REQUEST_AVATAR_DEFAULTS | SessionId={SessionId} | AvatarDefaults={AvatarDefaults}",
                            sessionId,
                            string.Join(',', avatarDefaults));
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseAvatarEquipChangeRequest(plainEncryptedFrame, out byte avatarEquipSubOpcode, out ushort avatarItemCode))
                    {
                        ApplyAvatarEquipChange(sessionState, avatarEquipSubOpcode, avatarItemCode);

                        byte[] responseFrame = BuildAvatarEquipChangeResponse(avatarEquipSubOpcode, avatarItemCode, sessionState.SendCipher);
                        await stream.WriteAsync(responseFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_AVATAR_EQUIP | SessionId={SessionId} | SubOpcode={SubOpcode} | ItemCode={ItemCode} | EquippedItems={EquippedItems}",
                            sessionId,
                            avatarEquipSubOpcode,
                            avatarItemCode,
                            string.Join(',', sessionState.EquippedAvatarItems));
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseChannelListRequest(plainEncryptedFrame))
                    {
                        LegacyChannelInfo[] channels = BuildDefaultChannels();
                        byte[] startFrame = BuildChannelListStartResponse((ushort)channels.Length, sessionState.SendCipher);
                        await stream.WriteAsync(startFrame, stoppingToken);

                        foreach (LegacyChannelInfo channel in channels)
                        {
                            byte[] itemFrame = BuildChannelListItemResponse(channel, sessionState.SendCipher);
                            await stream.WriteAsync(itemFrame, stoppingToken);
                        }

                        byte[] endFrame = BuildChannelListEndResponse(sessionState.SendCipher);
                        await stream.WriteAsync(endFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_CHANNEL_LIST | SessionId={SessionId} | ChannelCount={ChannelCount}",
                            sessionId,
                            channels.Length);
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseEnterChannelRequest(plainEncryptedFrame, out byte channelNumber))
                    {
                        sessionState.CurrentChannelNumber = channelNumber;
                        sessionState.CurrentLocation = 0x00;
                        sessionState.LastSuccessfulLeaveRoomTick = 0;

                        byte[] enterChannelFrame = BuildEnterChannelSuccessResponse(channelNumber, sessionState.SendCipher);
                        await stream.WriteAsync(enterChannelFrame, stoppingToken);

                        byte[] channelPresenceFrame = BuildChannelPresenceResponse(sessionState, sessionState.SendCipher);
                        await stream.WriteAsync(channelPresenceFrame, stoppingToken);

                        if (!sessionState.CashPresentLoaded)
                        {
                            byte[] saveCashPresentFrame = BuildSaveCashPresentResponse(sessionState, sessionState.SendCipher);
                            await stream.WriteAsync(saveCashPresentFrame, stoppingToken);
                            sessionState.CashPresentLoaded = true;
                        }

                        byte[] roomListStartFrame = BuildRoomListStartResponse(0, sessionState.SendCipher);
                        await stream.WriteAsync(roomListStartFrame, stoppingToken);

                        byte[] roomListEndFrame = BuildRoomListEndResponse(sessionState.SendCipher);
                        await stream.WriteAsync(roomListEndFrame, stoppingToken);

                        byte[] userListStartFrame = BuildUserListStartResponse(0, sessionState.SendCipher);
                        await stream.WriteAsync(userListStartFrame, stoppingToken);

                        byte[] userListEndFrame = BuildUserListEndResponse(sessionState.SendCipher);
                        await stream.WriteAsync(userListEndFrame, stoppingToken);

                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_ENTER_CHANNEL | SessionId={SessionId} | ChannelNumber={ChannelNumber}",
                            sessionId,
                            channelNumber);
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseTournamentScrollRequest(plainEncryptedFrame))
                    {
                        byte[] responseFrame = BuildTournamentScrollResponse(sessionState.SendCipher);
                        await stream.WriteAsync(responseFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_TOURNAMENT_SCROLL | SessionId={SessionId}",
                            sessionId);
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseLobbyListRequest(plainEncryptedFrame, out byte lobbyRequestType))
                    {
                        bool sentLobbyFrame = false;

                        if (lobbyRequestType == 0x00)
                        {
                            byte[] roomListStartFrame = BuildRoomListStartResponse(0, sessionState.SendCipher);
                            byte[] roomListEndFrame = BuildRoomListEndResponse(sessionState.SendCipher);
                            await stream.WriteAsync(roomListStartFrame, stoppingToken);
                            await stream.WriteAsync(roomListEndFrame, stoppingToken);
                            sentLobbyFrame = true;
                        }
                        else if (lobbyRequestType == 0x01)
                        {
                            byte[] userListStartFrame = BuildUserListStartResponse(1, sessionState.SendCipher);
                            byte[] userListItemFrame = BuildUserListItemResponse(sessionState, sessionState.SendCipher);
                            byte[] userListEndFrame = BuildUserListEndResponse(sessionState.SendCipher);
                            await stream.WriteAsync(userListStartFrame, stoppingToken);
                            await stream.WriteAsync(userListItemFrame, stoppingToken);
                            await stream.WriteAsync(userListEndFrame, stoppingToken);
                            sentLobbyFrame = true;
                        }
                        else
                        {
                            if (sessionState.CurrentRoomNumber == 0 &&
                                sessionState.CurrentLocation == 0x01)
                            {
                                byte[] nonLobbyEnterFrame = BuildNonLobbyEnterResponse(sessionState, sessionState.SendCipher);
                                await stream.WriteAsync(nonLobbyEnterFrame, stoppingToken);
                                sentLobbyFrame = true;
                            }
                            else
                            {
                                logger.LogInformation(
                                    "[AUDITION FLOW] ENTRAR_SALA_IGNORE_LOBBY_REFRESH | SessionId={SessionId} | RequestType={RequestType} | ChannelNumber={ChannelNumber} | Location={Location} | RoomNumber={RoomNumber}",
                                    sessionId,
                                    lobbyRequestType,
                                    sessionState.CurrentChannelNumber,
                                    sessionState.CurrentLocation,
                                    sessionState.CurrentRoomNumber);
                            }
                        }

                        if (sentLobbyFrame)
                        {
                            await stream.FlushAsync(stoppingToken);
                        }

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_LOBBY_LIST | SessionId={SessionId} | RequestType={RequestType} | SentFrame={SentFrame}",
                            sessionId,
                            lobbyRequestType,
                            sentLobbyFrame);
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseMusicMallRequest(plainEncryptedFrame, out byte musicMallSubOpcode))
                    {
                        byte[] responseFrame = musicMallSubOpcode == 0x01
                            ? BuildMusicTicketRemainResponse(sessionState.SendCipher)
                            : BuildMusicMallHeaderErrorResponse(sessionState.SendCipher);
                        await stream.WriteAsync(responseFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_MUSIC_MALL | SessionId={SessionId} | RequestType={RequestType}",
                            sessionId,
                            musicMallSubOpcode);
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseAvatarBasketRequest(plainEncryptedFrame))
                    {
                        byte[] plainCashFrame = BuildPlainSaveCashPresentFrame(sessionState);
                        byte[] saveCashPresentFrame = BuildSaveCashPresentResponse(sessionState, sessionState.SendCipher);
                        await stream.WriteAsync(saveCashPresentFrame, stoppingToken);

                        byte[] plainBasketFrame = BuildPlainAvatarBasketFrame();
                        byte[] responseFrame = BuildAvatarBasketResponse(sessionState.SendCipher);
                        await stream.WriteAsync(responseFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_AVATAR_BASKET | SessionId={SessionId} | CashHex={CashHex} | CashPlainHex={CashPlainHex} | BasketHex={BasketHex} | BasketPlainHex={BasketPlainHex}",
                            sessionId,
                            Convert.ToHexString(saveCashPresentFrame),
                            Convert.ToHexString(plainCashFrame),
                            Convert.ToHexString(responseFrame),
                            Convert.ToHexString(plainBasketFrame));
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseCreateRoomMetaRequest(plainEncryptedFrame, out byte createRoomType))
                    {
                        byte[] responseFrame = BuildCreateRoomMetaResponse(createRoomType, sessionState.SendCipher);
                        await stream.WriteAsync(responseFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_CREATE_ROOM_META | SessionId={SessionId} | RequestType={RequestType}",
                            sessionId,
                            createRoomType);
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseCreateRoomRequest(plainEncryptedFrame, out CreateRoomRequest createRoomRequest))
                    {
                        SyntheticPartnerProfile? syntheticPartnerProfile = GetConfiguredSyntheticPartnerProfile(
                            LoadSyntheticPartnerSettings(_syntheticPartnerSettingsPath, logger));
                        ushort roomNumber = checked((ushort)Math.Max(1, Interlocked.Increment(ref _nextSyntheticRoomNumber)));
                        sessionState.CurrentRoomNumber = roomNumber;
                        sessionState.CurrentRoomSlotIndex = 0;
                        sessionState.CurrentLocation = 0x02;
                        sessionState.LastSuccessfulLeaveRoomTick = 0;
                        sessionState.RoomReady = false;
                        sessionState.CurrentRoomMusicCode = 0;
                        sessionState.CurrentRoomStageCode = 0;
                        sessionState.CurrentRoomStageVariant = 0;
                        sessionState.CurrentRoomGameMode = 0;
                        sessionState.CurrentRoomRandomMusic = 0;
                        sessionState.CurrentRoomRandomMode = 0;
                        sessionState.UserTeam = 0;
                        ClearSyntheticRoomGuest(sessionState);
                        if (syntheticPartnerProfile is SyntheticPartnerProfile configuredSyntheticPartner)
                        {
                            ConfigureSyntheticRoomGuest(sessionState, configuredSyntheticPartner);
                        }
                        sessionState.GameStartActive = false;
                        sessionState.GameMusicReady = false;
                        sessionState.SyntheticRoomGuestMusicReady = false;
                        sessionState.GameStartSync = false;
                        sessionState.SyntheticRoomGuestStartSync = false;
                        sessionState.GameStartBroadcastSent = false;
                        sessionState.GameStartTickCount = 0;
                        sessionState.CurrentRoomMusicName = string.Empty;
                        sessionState.CurrentRoomMusicChecksum = 0;
                        sessionState.CurrentGameScore = 0;
                        sessionState.CurrentGamePerfectCount = 0;

                        byte[] createRoomAckFrame = BuildCreateRoomAcceptedResponse(roomNumber, sessionState.SendCipher);
                        await stream.WriteAsync(createRoomAckFrame, stoppingToken);

                        byte[] roomCreatedFrame = BuildRoomCreatedBroadcastResponse(createRoomRequest, roomNumber, sessionState.SendCipher);
                        await stream.WriteAsync(roomCreatedFrame, stoppingToken);

                        byte[] enterRoomFrame = BuildEnterRoomResponse(sessionState, createRoomRequest, roomNumber, sessionState.SendCipher);
                        await stream.WriteAsync(enterRoomFrame, stoppingToken);

                        byte[] masterFrame = BuildRoomMasterResponse(
                            sessionState.CurrentRoomSlotIndex,
                            sessionState.UserNickname,
                            sessionState.SendCipher);
                        await stream.WriteAsync(masterFrame, stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_ROOM_MASTER | SessionId={SessionId} | SlotIndex={SlotIndex} | Nickname={Nickname}",
                            sessionId,
                            sessionState.CurrentRoomSlotIndex,
                            sessionState.UserNickname);

                        if (sessionState.HasSyntheticRoomGuest)
                        {
                            byte[] syntheticGuestPresenceFrame = BuildRoomPresenceResponse(
                                sessionState.SyntheticRoomGuestSlotIndex,
                                sessionState.SyntheticRoomGuestNickname,
                                sessionState.SyntheticRoomGuestUserId,
                                sessionState.SyntheticRoomGuestGender,
                                sessionState.SyntheticRoomGuestExperience,
                                isMaster: false,
                                isReady: sessionState.SyntheticRoomGuestReady,
                                team: sessionState.SyntheticRoomGuestTeam,
                                power: sessionState.SyntheticRoomGuestPower,
                                sessionState.SendCipher);
                            await stream.WriteAsync(syntheticGuestPresenceFrame, stoppingToken);

                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_ROOM_PRESENCE | SessionId={SessionId} | SlotIndex={SlotIndex} | Nickname={Nickname} | Ready={Ready} | Team={Team}",
                                sessionId,
                                sessionState.SyntheticRoomGuestSlotIndex,
                                sessionState.SyntheticRoomGuestNickname,
                                sessionState.SyntheticRoomGuestReady,
                                sessionState.SyntheticRoomGuestTeam);

                            byte[] syntheticGuestReadyFrame = BuildRoomReadyStateResponse(
                                sessionState.SyntheticRoomGuestSlotIndex,
                                sessionState.SyntheticRoomGuestReady,
                                sessionState.SendCipher);
                            await stream.WriteAsync(syntheticGuestReadyFrame, stoppingToken);

                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_ROOM_READY_SYNTHETIC | SessionId={SessionId} | SlotIndex={SlotIndex} | Ready={Ready}",
                                sessionId,
                                sessionState.SyntheticRoomGuestSlotIndex,
                                sessionState.SyntheticRoomGuestReady);
                        }

                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_CREATE_ROOM | SessionId={SessionId} | RoomNumber={RoomNumber} | MaxUsers={MaxUsers} | RoomKind={RoomKind} | RoomNameHex={RoomNameHex}",
                            sessionId,
                            roomNumber,
                            createRoomRequest.MaxUsers,
                            createRoomRequest.RoomKind,
                            Convert.ToHexString(createRoomRequest.RoomNameBytes));
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        sessionState.CurrentRoomNumber != 0 &&
                        TryParseRoomSlotRequest(plainEncryptedFrame))
                    {
                        byte[] roomSlotsStartFrame = BuildRoomSlotListStartResponse(sessionState.SendCipher);
                        await stream.WriteAsync(roomSlotsStartFrame, stoppingToken);

                        for (byte slotIndex = 0; slotIndex < 0x11; slotIndex++)
                        {
                            byte[] roomSlotItemFrame = BuildRoomSlotListItemResponse(sessionState, slotIndex, sessionState.SendCipher);
                            await stream.WriteAsync(roomSlotItemFrame, stoppingToken);
                        }

                        byte[] roomSlotsEndFrame = BuildRoomSlotListEndResponse(sessionState.SendCipher);
                        await stream.WriteAsync(roomSlotsEndFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_ROOM_SLOTS | SessionId={SessionId} | RoomNumber={RoomNumber} | SlotIndex={SlotIndex}",
                            sessionId,
                            sessionState.CurrentRoomNumber,
                            sessionState.CurrentRoomSlotIndex);
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseInviteListRequest(plainEncryptedFrame))
                    {
                        byte[] responseFrame = sessionState.CurrentRoomNumber != 0
                            ? BuildInviteListResponse([], sessionState.SendCipher)
                            : BuildInviteListErrorResponse(0x01, sessionState.SendCipher);
                        await stream.WriteAsync(responseFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_INVITE_LIST | SessionId={SessionId} | InRoom={InRoom}",
                            sessionId,
                            sessionState.CurrentRoomNumber != 0);
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseRoomHostChangeRequest(plainEncryptedFrame, out RoomHostChangeRequest roomHostChangeRequest))
                    {
                        if (sessionState.CurrentRoomNumber == 0)
                        {
                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_IGNORE_ROOM_HOST | SessionId={SessionId} | SubOpcode={SubOpcode} | Value16={Value16} | Value8={Value8}",
                                sessionId,
                                roomHostChangeRequest.SubOpcode,
                                roomHostChangeRequest.Value16,
                                roomHostChangeRequest.Value8);
                            continue;
                        }

                        switch (roomHostChangeRequest.SubOpcode)
                        {
                            case 0x02:
                                sessionState.CurrentRoomMusicCode = roomHostChangeRequest.Value16;
                                sessionState.CurrentRoomRandomMusic = roomHostChangeRequest.Value8;

                                byte[] musicChangedFrame = BuildRoomMusicChangedResponse(sessionState, sessionState.SendCipher);
                                await stream.WriteAsync(musicChangedFrame, stoppingToken);

                                byte[] musicAckFrame = BuildRoomMusicChangeAcceptedResponse(sessionState.SendCipher);
                                await stream.WriteAsync(musicAckFrame, stoppingToken);
                                break;

                            case 0x03:
                                sessionState.CurrentRoomStageCode = roomHostChangeRequest.Value16;
                                sessionState.CurrentRoomStageVariant = roomHostChangeRequest.Value8;

                                byte[] stageChangedFrame = BuildRoomStageChangedResponse(sessionState, sessionState.SendCipher);
                                await stream.WriteAsync(stageChangedFrame, stoppingToken);
                                break;

                            case 0x04:
                                sessionState.CurrentRoomGameMode = roomHostChangeRequest.Value8;

                                byte[] gameModeChangedFrame = BuildRoomGameModeChangedResponse(sessionState, sessionState.SendCipher);
                                await stream.WriteAsync(gameModeChangedFrame, stoppingToken);
                                break;

                            case 0x05:
                                sessionState.CurrentRoomRandomMode = roomHostChangeRequest.Value8;

                                byte[] randomModeChangedFrame = BuildRoomRandomModeChangedResponse(sessionState, sessionState.SendCipher);
                                await stream.WriteAsync(randomModeChangedFrame, stoppingToken);
                                break;
                        }

                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_ROOM_HOST | SessionId={SessionId} | SubOpcode={SubOpcode} | Value16={Value16} | Value8={Value8}",
                            sessionId,
                            roomHostChangeRequest.SubOpcode,
                            roomHostChangeRequest.Value16,
                            roomHostChangeRequest.Value8);
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseRoomReadyRequest(plainEncryptedFrame, out bool isReady))
                    {
                        if (sessionState.CurrentRoomNumber == 0)
                        {
                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_IGNORE_ROOM_READY | SessionId={SessionId} | Ready={Ready}",
                                sessionId,
                                isReady);
                            continue;
                        }

                        sessionState.RoomReady = isReady;

                        byte[] readyFrame = BuildRoomReadyResponse(sessionState, sessionState.SendCipher);
                        await stream.WriteAsync(readyFrame, stoppingToken);

                        if (ShouldAutoStartRoomGame(sessionState))
                        {
                            uint randomSeed = BeginRoomGame(sessionState);

                            byte[] hackListFrame = BuildRoomGameHackListResponse(sessionState.SendCipher);
                            await stream.WriteAsync(hackListFrame, stoppingToken);

                            byte[] gameStartFrame = BuildRoomGameStartSuccessResponse(randomSeed, sessionState.SendCipher);
                            await stream.WriteAsync(gameStartFrame, stoppingToken);

                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_GAME_START_AUTO | SessionId={SessionId} | RoomNumber={RoomNumber} | RandomSeed={RandomSeed}",
                                sessionId,
                                sessionState.CurrentRoomNumber,
                                randomSeed);
                        }

                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_ROOM_READY | SessionId={SessionId} | SlotIndex={SlotIndex} | Ready={Ready}",
                            sessionId,
                            sessionState.CurrentRoomSlotIndex,
                            sessionState.RoomReady);
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseRoomAudienceRequest(plainEncryptedFrame))
                    {
                        byte[] responseFrame = BuildRoomAudienceErrorResponse(sessionState.SendCipher);
                        await stream.WriteAsync(responseFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_ROOM_AUDIENCE_ERROR | SessionId={SessionId} | InRoom={InRoom}",
                            sessionId,
                            sessionState.CurrentRoomNumber != 0);
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseRoomTeamChangeRequest(plainEncryptedFrame, out byte newTeam))
                    {
                        if (sessionState.CurrentRoomNumber == 0)
                        {
                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_IGNORE_ROOM_TEAM | SessionId={SessionId} | Team={Team}",
                                sessionId,
                                newTeam);
                            continue;
                        }

                        if (sessionState.RoomReady)
                        {
                            byte[] errorFrame = BuildRoomTeamChangeErrorResponse(sessionState.SendCipher);
                            await stream.WriteAsync(errorFrame, stoppingToken);
                            await stream.FlushAsync(stoppingToken);

                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_ROOM_TEAM_ERROR | SessionId={SessionId} | Team={Team}",
                                sessionId,
                                newTeam);
                            continue;
                        }

                        sessionState.UserTeam = newTeam;

                        byte[] teamFrame = BuildRoomTeamChangeResponse(sessionState, sessionState.SendCipher);
                        await stream.WriteAsync(teamFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_ROOM_TEAM | SessionId={SessionId} | SlotIndex={SlotIndex} | Team={Team}",
                            sessionId,
                            sessionState.CurrentRoomSlotIndex,
                            sessionState.UserTeam);
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseRoomGameStartRequest(plainEncryptedFrame))
                    {
                        if (sessionState.CurrentRoomNumber == 0)
                        {
                            byte[] errorFrame = BuildRoomGameStartErrorResponse(sessionState.SendCipher);
                            await stream.WriteAsync(errorFrame, stoppingToken);
                            await stream.FlushAsync(stoppingToken);

                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_GAME_START_ERROR | SessionId={SessionId} | Reason=NotInRoom",
                                sessionId);
                            continue;
                        }

                        uint randomSeed = BeginRoomGame(sessionState);

                        byte[] hackListFrame = BuildRoomGameHackListResponse(sessionState.SendCipher);
                        await stream.WriteAsync(hackListFrame, stoppingToken);

                        byte[] gameStartFrame = BuildRoomGameStartSuccessResponse(randomSeed, sessionState.SendCipher);
                        await stream.WriteAsync(gameStartFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_GAME_START | SessionId={SessionId} | RoomNumber={RoomNumber} | RandomSeed={RandomSeed}",
                            sessionId,
                            sessionState.CurrentRoomNumber,
                            randomSeed);
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseLeaveRoomRequest(plainEncryptedFrame))
                    {
                        bool wasInRoom = sessionState.CurrentRoomNumber != 0;

                        if (!wasInRoom)
                        {
                            if (sessionState.CurrentLocation == 0x01)
                            {
                                sessionState.LastSuccessfulLeaveRoomTick = Environment.TickCount64;
                                byte[] duplicateLobbyEnterFrame = BuildNonLobbyEnterResponse(sessionState, sessionState.SendCipher);
                                await stream.WriteAsync(duplicateLobbyEnterFrame, stoppingToken);
                                await stream.FlushAsync(stoppingToken);

                                logger.LogInformation(
                                    "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_LEAVE_ROOM_DUPLICATE | SessionId={SessionId} | ChannelNumber={ChannelNumber} | Location={Location}",
                                    sessionId,
                                    sessionState.CurrentChannelNumber,
                                    sessionState.CurrentLocation);
                                continue;
                            }

                            byte[] errorFrame = BuildLeaveRoomErrorResponse(sessionState.SendCipher);
                            await stream.WriteAsync(errorFrame, stoppingToken);
                            await stream.FlushAsync(stoppingToken);

                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_LEAVE_ROOM_ERROR | SessionId={SessionId} | Reason=NotInRoom | ChannelNumber={ChannelNumber}",
                                sessionId,
                                sessionState.CurrentChannelNumber);
                            continue;
                        }

                        sessionState.CurrentRoomNumber = 0;
                        sessionState.CurrentRoomSlotIndex = 0;
                        sessionState.CurrentLocation = 0x01;
                        sessionState.RoomReady = false;
                        sessionState.CurrentRoomMusicCode = 0;
                        sessionState.CurrentRoomStageCode = 0;
                        sessionState.CurrentRoomStageVariant = 0;
                        sessionState.CurrentRoomGameMode = 0;
                        sessionState.CurrentRoomRandomMusic = 0;
                        sessionState.CurrentRoomRandomMode = 0;
                        sessionState.UserTeam = 0;
                        sessionState.HasSyntheticRoomGuest = false;
                        sessionState.SyntheticRoomGuestSlotIndex = 0;
                        sessionState.SyntheticRoomGuestReady = false;
                        sessionState.SyntheticRoomGuestTeam = 0;
                        sessionState.GameStartActive = false;
                        sessionState.GameMusicReady = false;
                        sessionState.SyntheticRoomGuestMusicReady = false;
                        sessionState.GameStartSync = false;
                        sessionState.SyntheticRoomGuestStartSync = false;
                        sessionState.GameStartBroadcastSent = false;
                        sessionState.GameStartTickCount = 0;
                        sessionState.CurrentRoomMusicName = string.Empty;
                        sessionState.CurrentRoomMusicChecksum = 0;
                        sessionState.CurrentGameScore = 0;
                        sessionState.CurrentGamePerfectCount = 0;
                        sessionState.LastSuccessfulLeaveRoomTick = Environment.TickCount64;

                        byte[] lobbyEnterFrame = BuildNonLobbyEnterResponse(sessionState, sessionState.SendCipher);
                        await stream.WriteAsync(lobbyEnterFrame, stoppingToken);

                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_LEAVE_ROOM | SessionId={SessionId} | ChannelNumber={ChannelNumber}",
                            sessionId,
                            sessionState.CurrentChannelNumber);
                        continue;
                    }

                    if (TryParseKeepAlivePing(plainEncryptedFrame))
                    {
                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_KEEPALIVE | SessionId={SessionId}",
                            sessionId);
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseGameTimePingRequest(plainEncryptedFrame))
                    {
                        uint tickCount = unchecked((uint)Environment.TickCount64);
                        byte[] pingFrame = BuildGameTimePingResponse(tickCount, sessionState.SendCipher);
                        await stream.WriteAsync(pingFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_GAME_TIME_PING | SessionId={SessionId} | TickCount={TickCount}",
                            sessionId,
                            tickCount);
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseMusicReadyRequest(plainEncryptedFrame, out string musicName, out uint musicChecksum, out bool reloadMusic))
                    {
                        sessionState.GameMusicReady = true;
                        sessionState.CurrentRoomMusicName = musicName;
                        sessionState.CurrentRoomMusicChecksum = musicChecksum;

                        if (sessionState.HasSyntheticRoomGuest)
                        {
                            sessionState.SyntheticRoomGuestMusicReady = true;
                        }

                        ServerMusicReadyPayload musicReadyPayload = ResolveMusicReadyPayload(musicName, musicChecksum);
                        byte[] musicReadyFrame = musicReadyPayload.LoadResult == 0
                            ? BuildMusicReadyDataResponse(
                                musicName,
                                musicReadyPayload.ServerChecksum,
                                musicChecksum,
                                musicReadyPayload.MusicBuffer,
                                musicReadyPayload.TotalMeasure,
                                musicReadyPayload.MusicBody,
                                sessionState.SendCipher)
                            : BuildMusicReadyAcceptedResponse(musicReadyPayload.LoadResult, sessionState.SendCipher);
                        await stream.WriteAsync(musicReadyFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_MUSIC_READY | SessionId={SessionId} | MusicName={MusicName} | Checksum={Checksum} | Reload={Reload} | AllMusicReady={AllMusicReady} | LoadResult={LoadResult} | ServerChecksum={ServerChecksum} | TotalMeasure={TotalMeasure} | ResponseKind={ResponseKind}",
                            sessionId,
                            musicName,
                            musicChecksum,
                            reloadMusic,
                            sessionState.GameMusicReady && (!sessionState.HasSyntheticRoomGuest || sessionState.SyntheticRoomGuestMusicReady),
                            musicReadyPayload.LoadResult,
                            musicReadyPayload.ServerChecksum,
                            musicReadyPayload.TotalMeasure,
                            musicReadyPayload.LoadResult == 0 ? "FullData" : "ShortResult");
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseInGameFrame(plainEncryptedFrame, out byte inGameSubOpcode))
                    {
                        if (inGameSubOpcode == 0x00 &&
                            TryParseInGameDanceStepRequest(plainEncryptedFrame, out InGameDanceStepRequest danceStepRequest))
                        {
                            sessionState.CurrentGameScore = AddClampedUInt32(
                                sessionState.CurrentGameScore,
                                danceStepRequest.ScoreDelta > 0 ? (uint)danceStepRequest.ScoreDelta : 0u);

                            if (danceStepRequest.Judge == 0x00)
                            {
                                sessionState.CurrentGamePerfectCount++;
                            }

                            uint eventTickCount = unchecked((uint)Environment.TickCount64);
                            byte[] danceStepFrame = BuildInGameDanceStepResponse(
                                sessionState.CurrentRoomSlotIndex,
                                danceStepRequest,
                                eventTickCount,
                                sessionState.SendCipher);
                            await stream.WriteAsync(danceStepFrame, stoppingToken);
                            await stream.FlushAsync(stoppingToken);

                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_INGAME_STEP | SessionId={SessionId} | SlotIndex={SlotIndex} | PacketCount={PacketCount} | Judge={Judge} | ScoreDelta={ScoreDelta} | TickCount={TickCount}",
                                sessionId,
                                sessionState.CurrentRoomSlotIndex,
                                danceStepRequest.PacketCount,
                                danceStepRequest.Judge,
                                danceStepRequest.ScoreDelta,
                                eventTickCount);

                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_INGAME_PACKET | SessionId={SessionId} | SubOpcode={SubOpcode} | PlainHex={PayloadHex}",
                                sessionId,
                                inGameSubOpcode,
                                Convert.ToHexString(plainEncryptedFrame));
                        }
                        else if (inGameSubOpcode == 0x01)
                        {
                            sessionState.GameStartSync = true;

                            if (sessionState.HasSyntheticRoomGuest)
                            {
                                sessionState.SyntheticRoomGuestStartSync = true;
                            }

                            bool allStartSync = sessionState.GameStartSync &&
                                (!sessionState.HasSyntheticRoomGuest || sessionState.SyntheticRoomGuestStartSync);

                            if (allStartSync && !sessionState.GameStartBroadcastSent)
                            {
                                uint startTickCount = unchecked((uint)Environment.TickCount64) + 2000u;
                                sessionState.GameStartTickCount = startTickCount;
                                sessionState.GameStartBroadcastSent = true;

                                byte[] startSyncFrame = BuildInGameStartSyncResponse(startTickCount, teamMode: 0, sessionState.SendCipher);
                                await stream.WriteAsync(startSyncFrame, stoppingToken);
                                await stream.FlushAsync(stoppingToken);

                                logger.LogInformation(
                                    "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_INGAME_START_SYNC | SessionId={SessionId} | StartTickCount={StartTickCount} | TeamMode={TeamMode}",
                                    sessionId,
                                    startTickCount,
                                    0);
                            }

                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_INGAME_START_SYNC | SessionId={SessionId} | AllStartSync={AllStartSync} | PlainHex={PayloadHex}",
                                sessionId,
                                allStartSync,
                                Convert.ToHexString(plainEncryptedFrame));
                        }
                        else
                        {
                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_INGAME_PACKET | SessionId={SessionId} | SubOpcode={SubOpcode} | PlainHex={PayloadHex}",
                                sessionId,
                                inGameSubOpcode,
                                Convert.ToHexString(plainEncryptedFrame));
                        }

                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseGameEndRequest(plainEncryptedFrame, sessionState.UserLevel, out GameEndRequest gameEndRequest))
                    {
                        if (sessionState.CurrentRoomNumber == 0)
                        {
                            logger.LogInformation(
                                "[AUDITION FLOW] ENTRAR_SALA_IGNORE_GAME_END | SessionId={SessionId} | Reason=NotInRoom | PlainHex={PayloadHex}",
                                sessionId,
                                Convert.ToHexString(plainEncryptedFrame));
                            continue;
                        }

                        GameEndResponseState gameEndState = BuildGameEndResponseState(sessionState, gameEndRequest);
                        sessionState.UserExperience = gameEndState.SelfEntry.AfterExperience;
                        sessionState.UserGameMoney = AddClampedInt32(sessionState.UserGameMoney, gameEndState.SelfEntry.MoneyEarned);

                        if (sessionState.HasSyntheticRoomGuest)
                        {
                            sessionState.SyntheticRoomGuestExperience = gameEndState.PartnerEntry.AfterExperience;
                        }

                        sessionState.CurrentLocation = 0x02;
                        sessionState.RoomReady = false;
                        sessionState.GameStartActive = false;
                        sessionState.GameMusicReady = false;
                        sessionState.SyntheticRoomGuestMusicReady = false;
                        sessionState.GameStartSync = false;
                        sessionState.SyntheticRoomGuestStartSync = false;
                        sessionState.GameStartBroadcastSent = false;
                        sessionState.GameStartTickCount = 0;
                        sessionState.CurrentRoomMusicName = string.Empty;
                        sessionState.CurrentRoomMusicChecksum = 0;
                        sessionState.CurrentGameScore = 0;
                        sessionState.CurrentGamePerfectCount = 0;

                        byte[] resultFrame = BuildGameEndResultsResponse(gameEndState, sessionState.SendCipher);
                        await stream.WriteAsync(resultFrame, stoppingToken);

                        byte[] roomReturnFrame = BuildGameEndRoomReturnResponse(sessionState.CurrentRoomNumber, sessionState.SendCipher);
                        await stream.WriteAsync(roomReturnFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_GAME_END | SessionId={SessionId} | RoomNumber={RoomNumber} | Score={Score} | BeforeExperience={BeforeExperience} | BaseExperience={BaseExperience} | BonusExperience={BonusExperience} | Money={Money} | PlayerCount={PlayerCount}",
                            sessionId,
                            sessionState.CurrentRoomNumber,
                            gameEndState.SelfEntry.TotalScore,
                            gameEndState.SelfEntry.BeforeExperience,
                            gameEndState.SelfEntry.BaseExperience,
                            gameEndState.SelfEntry.BonusExperience,
                            gameEndState.SelfEntry.MoneyEarned,
                            gameEndState.Entries.Count);
                        continue;
                    }

                    if (sessionState.ApplicationLoginHandled &&
                        TryParseChatRequest(plainEncryptedFrame, out byte chatMode, out string chatMessage))
                    {
                        byte[] chatFrame = BuildChatResponse(sessionState.UserNickname, chatMessage, sessionState.SendCipher);
                        await stream.WriteAsync(chatFrame, stoppingToken);
                        await stream.FlushAsync(stoppingToken);

                        logger.LogInformation(
                            "[AUDITION FLOW] ENTRAR_SALA_RESPONSE_CHAT | SessionId={SessionId} | ChatMode={ChatMode} | Nickname={Nickname} | Message={Message}",
                            sessionId,
                            chatMode,
                            sessionState.UserNickname,
                            chatMessage);
                        continue;
                    }

                    logger.LogInformation(
                        "[AUDITION FLOW] ENTRAR_SALA_UNHANDLED_APP_PACKET | SessionId={SessionId} | PlainHex={PayloadHex}",
                        sessionId,
                        Convert.ToHexString(plainEncryptedFrame));
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (IOException ex)
        {
            logger.LogWarning(
                ex,
                "Game client session {SessionId} closed with IO error from {RemoteEndPoint}",
                sessionId,
                client.Client.RemoteEndPoint?.ToString() ?? "unknown");
        }
        catch (SocketException ex)
        {
            logger.LogWarning(
                ex,
                "Game client session {SessionId} closed with socket error from {RemoteEndPoint}",
                sessionId,
                client.Client.RemoteEndPoint?.ToString() ?? "unknown");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Game client session {SessionId} crashed for {RemoteEndPoint}",
                sessionId,
                client.Client.RemoteEndPoint?.ToString() ?? "unknown");
        }
    }

    private static IPAddress ResolveAddress(string host)
    {
        if (string.Equals(host, "0.0.0.0", StringComparison.OrdinalIgnoreCase))
        {
            return IPAddress.Any;
        }

        if (IPAddress.TryParse(host, out IPAddress? parsedAddress))
        {
            return parsedAddress;
        }

        IPAddress[] addresses = Dns.GetHostAddresses(host);
        return addresses.FirstOrDefault() ?? IPAddress.Loopback;
    }

    private static bool TryDequeueFrame(List<byte> pendingBytes, out byte[] frame)
    {
        frame = Array.Empty<byte>();
        if (pendingBytes.Count < 2)
        {
            return false;
        }

        ushort frameLength = BinaryPrimitives.ReadUInt16LittleEndian([pendingBytes[0], pendingBytes[1]]);
        if (frameLength < 4 || frameLength > 0x5000)
        {
            pendingBytes.Clear();
            return false;
        }

        if (pendingBytes.Count < frameLength)
        {
            return false;
        }

        frame = pendingBytes.GetRange(0, frameLength).ToArray();
        pendingBytes.RemoveRange(0, frameLength);
        return true;
    }

    private static bool TryDecryptRawFrame(byte[] frame, out byte[] plainFrame)
    {
        plainFrame = Array.Empty<byte>();
        if (frame.Length < 4)
        {
            return false;
        }

        byte[] payload = frame[2..];
        byte encodedKey = payload[0];
        byte key = (byte)((((~encodedKey) & 0xFF) >> 7 | (((~encodedKey) & 0xFF) << 1)) & 0xFF);
        if (key == 0 || payload.Length <= key + 1)
        {
            return false;
        }

        int plainLength = payload.Length - (key + 1);
        byte[] plainPayload = new byte[plainLength];

        for (int index = 0; index < plainLength; index++)
        {
            int keyIndex = (index % key) + 1;
            plainPayload[index] = (byte)(payload[key + index + 1] ^ payload[keyIndex] ^ (byte)(plainLength - index));
        }

        plainFrame = new byte[plainLength + 2];
        BinaryPrimitives.WriteUInt16LittleEndian(plainFrame.AsSpan(0, 2), (ushort)(plainLength + 2));
        plainPayload.CopyTo(plainFrame, 2);
        return BinaryPrimitives.ReadUInt16LittleEndian(plainFrame.AsSpan(0, 2)) == plainFrame.Length;
    }

    private static bool TryDecryptTafFrame(TafCipher cipher, byte[] frame, out byte[] plainFrame)
    {
        plainFrame = Array.Empty<byte>();
        if (frame.Length < 4)
        {
            return false;
        }

        byte[] encryptedPayload = frame[2..];
        byte[] plainPayload = cipher.Decrypt(encryptedPayload);
        if (plainPayload.Length < 2)
        {
            return false;
        }

        plainFrame = plainPayload;
        return BinaryPrimitives.ReadUInt16LittleEndian(plainFrame.AsSpan(0, 2)) == plainFrame.Length;
    }

    private static bool TryParseLoginHandshakeRequest(byte[] plainFrame)
    {
        if (plainFrame.Length != 4)
        {
            return false;
        }

        return plainFrame[2] == 0x01 && plainFrame[3] == 0x05;
    }

    private static bool TryParseGameLoginRequest(byte[] plainFrame, out GameLoginRequest request)
    {
        request = default;
        if (plainFrame.Length < 9)
        {
            return false;
        }

        ReadOnlySpan<byte> body = plainFrame.AsSpan(2);
        if (body.Length < 7 || body[0] != 0x01)
        {
            return false;
        }

        byte loginMode = body[1];
        if (loginMode > 0x02)
        {
            return false;
        }

        uint userSerial = BinaryPrimitives.ReadUInt32LittleEndian(body.Slice(2, 4));
        if (!TryReadLengthPrefixedString(body, 6, out string clientVersion, out _))
        {
            return false;
        }

        request = new GameLoginRequest(loginMode, userSerial, clientVersion);
        return true;
    }

    private static bool TryParseSelfUserInfoRequest(byte[] plainFrame)
    {
        if (plainFrame.Length != 4)
        {
            return false;
        }

        return plainFrame[2] == 0x0A && plainFrame[3] == 0x00;
    }

    private static bool TryParseRequestedUserInfoRequest(byte[] plainFrame, out byte requestType, out string nickname)
    {
        requestType = 0;
        nickname = string.Empty;
        if (plainFrame.Length < 6 || plainFrame[2] != 0x0A)
        {
            return false;
        }

        requestType = plainFrame[3];
        if (requestType is not 0x02 and not 0x04)
        {
            return false;
        }

        if (!TryReadLengthPrefixedString(plainFrame.AsSpan(2), 2, out nickname, out int nextOffset))
        {
            return false;
        }

        return nextOffset == plainFrame.Length - 2;
    }

    private static bool TryParseUserAvatarInfoRequest(byte[] plainFrame, out string nickname)
    {
        nickname = string.Empty;
        if (plainFrame.Length < 6)
        {
            return false;
        }

        ReadOnlySpan<byte> body = plainFrame.AsSpan(2);
        if (body[0] != 0x0B || body[1] != 0x00)
        {
            return false;
        }

        if (!TryReadLengthPrefixedString(body, 2, out nickname, out int nextOffset))
        {
            return false;
        }

        return nextOffset == body.Length;
    }

    private static bool TryParseDefaultAvatarSelectionRequest(byte[] plainFrame, out ushort[] avatarDefaults)
    {
        avatarDefaults = [];
        if (plainFrame.Length != 14)
        {
            return false;
        }

        ReadOnlySpan<byte> body = plainFrame.AsSpan(2);
        if (body[0] != 0x0B || body[1] != 0x02)
        {
            return false;
        }

        avatarDefaults = new ushort[5];
        for (int index = 0; index < avatarDefaults.Length; index++)
        {
            avatarDefaults[index] = BinaryPrimitives.ReadUInt16LittleEndian(body.Slice(2 + (index * 2), 2));
        }

        return true;
    }

    private static bool TryParseAvatarInventoryRequest(byte[] plainFrame)
    {
        return plainFrame.Length == 4 && plainFrame[2] == 0x0B && plainFrame[3] == 0x01;
    }

    private static bool TryParseAvatarEquipChangeRequest(byte[] plainFrame, out byte subOpcode, out ushort itemCode)
    {
        subOpcode = 0;
        itemCode = 0;
        if (plainFrame.Length != 6 || plainFrame[2] != 0x0B)
        {
            return false;
        }

        subOpcode = plainFrame[3];
        if (subOpcode is not 0x05 and not 0x06)
        {
            return false;
        }

        itemCode = BinaryPrimitives.ReadUInt16LittleEndian(plainFrame.AsSpan(4, 2));
        return true;
    }

    private static bool TryParseChannelListRequest(byte[] plainFrame)
    {
        if (plainFrame.Length != 4)
        {
            return false;
        }

        return plainFrame[2] == 0x08 && plainFrame[3] == 0x00;
    }

    private static bool TryParseEnterChannelRequest(byte[] plainFrame, out byte channelNumber)
    {
        channelNumber = 0;
        if (plainFrame.Length != 5)
        {
            return false;
        }

        return plainFrame[2] == 0x09 && plainFrame[3] == 0x00 && (channelNumber = plainFrame[4]) >= 0;
    }

    private static bool TryParseTournamentScrollRequest(byte[] plainFrame)
    {
        return plainFrame.Length == 4 && plainFrame[2] == 0x06 && plainFrame[3] == 0x00;
    }

    private static bool TryParseLobbyListRequest(byte[] plainFrame, out byte requestType)
    {
        requestType = 0;
        if (plainFrame.Length != 4 || plainFrame[2] != 0x14)
        {
            return false;
        }

        requestType = plainFrame[3];
        return requestType <= 0x02;
    }

    private static bool TryParseMusicMallRequest(byte[] plainFrame, out byte requestType)
    {
        requestType = 0;
        if (plainFrame.Length < 4 || plainFrame[2] != 0x0E)
        {
            return false;
        }

        requestType = plainFrame[3];
        return requestType is 0x00 or 0x01;
    }

    private static bool TryParseAvatarBasketRequest(byte[] plainFrame)
    {
        return plainFrame.Length == 4 && plainFrame[2] == 0x0B && plainFrame[3] == 0x09;
    }

    private static bool TryParseCreateRoomMetaRequest(byte[] plainFrame, out byte requestType)
    {
        requestType = 0;
        if (plainFrame.Length != 4 || plainFrame[2] != 0x16)
        {
            return false;
        }

        requestType = plainFrame[3];
        return requestType is 0x01 or 0x03;
    }

    private static bool TryParseCreateRoomRequest(byte[] plainFrame, out CreateRoomRequest request)
    {
        request = default;
        if (plainFrame.Length < 7)
        {
            return false;
        }

        ReadOnlySpan<byte> body = plainFrame.AsSpan(2);
        if (body[0] != 0x16 || body[1] != 0x00)
        {
            return false;
        }

        if (!TryReadLengthPrefixedBytes(body, 2, out byte[] roomNameBytes, out int nextOffset))
        {
            return false;
        }

        if (nextOffset + 2 > body.Length)
        {
            return false;
        }

        byte maxUsers = body[nextOffset];
        byte roomKind = body[nextOffset + 1];
        int tailOffset = nextOffset + 2;

        if ((roomKind & 0x01) != 0)
        {
            if (!TryReadLengthPrefixedBytes(body, tailOffset, out _, out tailOffset))
            {
                return false;
            }
        }

        if (tailOffset != body.Length)
        {
            return false;
        }

        request = new CreateRoomRequest(roomNameBytes, maxUsers, roomKind);
        return true;
    }

    private static bool TryParseRoomSlotRequest(byte[] plainFrame)
    {
        return plainFrame.Length == 4 && plainFrame[2] == 0x18 && plainFrame[3] == 0x00;
    }

    private static bool TryParseRoomHostChangeRequest(byte[] plainFrame, out RoomHostChangeRequest request)
    {
        request = default;
        if (plainFrame.Length < 5 || plainFrame[2] != 0x1A)
        {
            return false;
        }

        byte subOpcode = plainFrame[3];
        if (subOpcode is 0x02 or 0x03)
        {
            if (plainFrame.Length != 7)
            {
                return false;
            }

            ushort value16 = BinaryPrimitives.ReadUInt16LittleEndian(plainFrame.AsSpan(4, 2));
            request = new RoomHostChangeRequest(subOpcode, value16, plainFrame[6]);
            return true;
        }

        if (subOpcode is 0x04 or 0x05)
        {
            if (plainFrame.Length != 5)
            {
                return false;
            }

            request = new RoomHostChangeRequest(subOpcode, 0, plainFrame[4]);
            return true;
        }

        return false;
    }

    private static bool TryParseRoomReadyRequest(byte[] plainFrame, out bool isReady)
    {
        isReady = false;
        if (plainFrame.Length != 5)
        {
            return false;
        }

        if (plainFrame[2] != 0x1B || plainFrame[3] != 0x00)
        {
            return false;
        }

        isReady = plainFrame[4] != 0;
        return true;
    }

    private static bool TryParseRoomTeamChangeRequest(byte[] plainFrame, out byte newTeam)
    {
        newTeam = 0;
        if (plainFrame.Length != 5)
        {
            return false;
        }

        if (plainFrame[2] != 0x1B || plainFrame[3] != 0x01)
        {
            return false;
        }

        newTeam = plainFrame[4];
        return true;
    }

    private static bool TryParseRoomAudienceRequest(byte[] plainFrame)
    {
        return plainFrame.Length == 4 && plainFrame[2] == 0x1B && plainFrame[3] == 0x04;
    }

    private static bool TryParseChatRequest(byte[] plainFrame, out byte chatMode, out string message)
    {
        chatMode = 0;
        message = string.Empty;

        if (plainFrame.Length < 5 || plainFrame[2] != 0x0C)
        {
            return false;
        }

        chatMode = plainFrame[3];
        if (chatMode != 0x00)
        {
            return false;
        }

        ReadOnlySpan<byte> body = plainFrame.AsSpan(2);
        if (!TryReadLengthPrefixedString(body, 2, out message, out int nextOffset))
        {
            message = string.Empty;
            return false;
        }

        return nextOffset == body.Length;
    }

    private static bool TryParseRoomGameStartRequest(byte[] plainFrame)
    {
        return plainFrame.Length == 4 && plainFrame[2] == 0x1B && plainFrame[3] == 0x02;
    }

    private static bool TryParseInviteListRequest(byte[] plainFrame)
    {
        return plainFrame.Length == 4 && plainFrame[2] == 0x05 && plainFrame[3] == 0x00;
    }

    private static bool TryParseLeaveRoomRequest(byte[] plainFrame)
    {
        return plainFrame.Length == 4 && plainFrame[2] == 0x17 && plainFrame[3] == 0x01;
    }

    private static bool TryParseKeepAlivePing(byte[] plainFrame)
    {
        if (plainFrame.Length != 4)
        {
            return false;
        }

        return plainFrame[2] == 0x00 && plainFrame[3] == 0x00;
    }

    private static bool TryParseGameTimePingRequest(byte[] plainFrame)
    {
        return plainFrame.Length == 4 && plainFrame[2] == 0x00 && plainFrame[3] == 0x07;
    }

    private static bool TryParseMusicReadyRequest(byte[] plainFrame, out string musicName, out uint checksum, out bool reload)
    {
        musicName = string.Empty;
        checksum = 0;
        reload = false;

        if (plainFrame.Length < 9 || plainFrame[2] != 0x1B || plainFrame[3] != 0x03)
        {
            return false;
        }

        ReadOnlySpan<byte> body = plainFrame.AsSpan(2);
        if (!TryReadLengthPrefixedString(body, 2, out musicName, out int nextOffset))
        {
            return false;
        }

        if (nextOffset + 5 != body.Length)
        {
            return false;
        }

        checksum = BinaryPrimitives.ReadUInt32LittleEndian(body.Slice(nextOffset, 4));
        reload = body[nextOffset + 4] != 0;
        return true;
    }

    private static bool TryParseInGameFrame(byte[] plainFrame, out byte subOpcode)
    {
        subOpcode = 0;

        if (plainFrame.Length < 4 || plainFrame[2] != 0x1C)
        {
            return false;
        }

        subOpcode = plainFrame[3];
        return true;
    }

    private static bool TryParseInGameDanceStepRequest(byte[] plainFrame, out InGameDanceStepRequest request)
    {
        request = default;

        if (plainFrame.Length != 25 || plainFrame[2] != 0x1C || plainFrame[3] != 0x00)
        {
            return false;
        }

        ReadOnlySpan<byte> body = plainFrame.AsSpan(4);
        request = new InGameDanceStepRequest(
            body[0],
            body[1],
            body[2],
            body[3],
            body[4],
            body[5],
            body[6],
            BinaryPrimitives.ReadUInt16LittleEndian(body.Slice(7, 2)),
            BinaryPrimitives.ReadInt16LittleEndian(body.Slice(9, 2)),
            BinaryPrimitives.ReadInt32LittleEndian(body.Slice(11, 4)),
            body[15],
            body[16],
            BinaryPrimitives.ReadUInt16LittleEndian(body.Slice(17, 2)),
            BinaryPrimitives.ReadUInt16LittleEndian(body.Slice(19, 2)));
        return true;
    }

    private static bool TryParseGameEndRequest(byte[] plainFrame, int defaultLevel, out GameEndRequest request)
    {
        request = default;

        if (plainFrame.Length == 13 && plainFrame[2] == 0x1E && plainFrame[3] == 0x00)
        {
            ReadOnlySpan<byte> body = plainFrame.AsSpan(4);
            request = new GameEndRequest(
                0x00,
                BinaryPrimitives.ReadUInt32LittleEndian(body.Slice(0, 4)),
                body[4],
                BinaryPrimitives.ReadInt32LittleEndian(body.Slice(5, 4)));
            return true;
        }

        if (plainFrame.Length == 4 && plainFrame[2] == 0x1E && plainFrame[3] == 0x01)
        {
            request = new GameEndRequest(0x01, 0, (byte)Math.Clamp(defaultLevel, 0, byte.MaxValue), 0);
            return true;
        }

        return false;
    }

    private static bool TryParseInitialLoginRequest(byte[] plainFrame, out InitialLoginRequest request)
    {
        request = default;
        if (plainFrame.Length < 7)
        {
            return false;
        }

        ReadOnlySpan<byte> body = plainFrame.AsSpan(2);
        if (body.Length >= 4)
        {
            ushort nestedLength = BinaryPrimitives.ReadUInt16LittleEndian(body[..2]);
            if (nestedLength == body.Length)
            {
                body = body[2..];
            }
        }

        if (body.Length < 5 || body[0] != 0x01)
        {
            return false;
        }

        if (!TryReadLengthPrefixedString(body, 2, out string userId, out string password, out string version))
        {
            request = default;
            return false;
        }

        request = new InitialLoginRequest(body[0], body[1], userId, password, version);
        return true;
    }

    private static bool TryParseKeyExchangeRequest(byte[] plainFrame, out KeyExchangeRequest request)
    {
        request = default;
        if (plainFrame.Length < 14)
        {
            return false;
        }

        ReadOnlySpan<byte> body = plainFrame.AsSpan(2);
        if (body[0] != 0x01 || body[1] != 0x06)
        {
            return false;
        }

        ulong clientPublicKey = BinaryPrimitives.ReadUInt64LittleEndian(body.Slice(2, 8));
        string md5Hex;

        if (body.Length >= 11 + 32 && body[10] == 0x20)
        {
            md5Hex = Encoding.ASCII.GetString(body.Slice(11, 32));
        }
        else if (body.Length >= 10 + 32)
        {
            md5Hex = Encoding.ASCII.GetString(body.Slice(10, 32));
        }
        else
        {
            return false;
        }

        request = new KeyExchangeRequest(clientPublicKey, md5Hex.TrimEnd('\0'));
        return true;
    }

    private static bool TryHandleKeyExchange(
        GameClientSessionState sessionState,
        KeyExchangeRequest request,
        out byte[] responseFrame,
        out string sharedKeyHex,
        out string cryptKey,
        out string expectedMd5)
    {
        ulong sharedKey = ModPow(request.ClientPublicKey, sessionState.Handshake!.PrivateKey, sessionState.Handshake.Prime);
        sharedKeyHex = sharedKey.ToString("X16");
        cryptKey = string.Concat(sessionState.Handshake.AddKeySeed, ((uint)sharedKey).ToString("X8"));
        expectedMd5 = Convert.ToHexString(MD5.HashData(Encoding.ASCII.GetBytes(cryptKey)));
        bool accepted = string.Equals(expectedMd5, request.ClientHashHex, StringComparison.OrdinalIgnoreCase);

        byte[] plainFrame = BuildPlainFrame([0x01, 0x06, accepted ? (byte)0x00 : (byte)0x01]);
        responseFrame = EncryptTafFrame(sessionState.SendCipher, plainFrame);

        if (accepted)
        {
            byte[] negotiatedKey = Encoding.ASCII.GetBytes(cryptKey);
            sessionState.ReceiveCipher = TafCipher.FromKey(negotiatedKey);
            sessionState.SendCipher = TafCipher.FromKey(negotiatedKey);
        }

        return accepted;
    }

    private static byte[] BuildLoginHandshakeResponse(GameClientSessionState sessionState)
    {
        LoginHandshakeState handshake = LoginHandshakeState.Create();
        sessionState.Handshake = handshake;

        byte[] addKeyBytes = BuildAddKeyBytes(handshake.AddKeySeed);
        byte[] body = new byte[3 + 8 + 8 + 8 + addKeyBytes.Length];
        body[0] = 0x01;
        body[1] = 0x05;
        body[2] = 0x00;
        BinaryPrimitives.WriteUInt64LittleEndian(body.AsSpan(3, 8), handshake.Generator);
        BinaryPrimitives.WriteUInt64LittleEndian(body.AsSpan(11, 8), handshake.Prime);
        BinaryPrimitives.WriteUInt64LittleEndian(body.AsSpan(19, 8), handshake.PublicKey);
        addKeyBytes.CopyTo(body, 27);

        byte[] plainFrame = BuildPlainFrame(body);
        return EncryptTafFrame(sessionState.SendCipher, plainFrame);
    }

    private static byte[] BuildGameLoginResponse(string nickname, TafCipher cipher)
    {
        byte[] nicknameBytes = Encoding.ASCII.GetBytes(nickname);
        byte[] body = new byte[4 + nicknameBytes.Length];
        body[0] = 0x01;
        body[1] = 0x00;
        body[2] = 0x00;
        body[3] = checked((byte)Math.Min(nicknameBytes.Length, byte.MaxValue));
        nicknameBytes.AsSpan(0, body[3]).CopyTo(body.AsSpan(4));

        byte[] plainFrame = BuildPlainFrame(body);
        return EncryptTafFrame(cipher, plainFrame);
    }

    private static byte[] BuildSelfUserInfoResponse(GameClientSessionState sessionState, TafCipher cipher)
    {
        return BuildRequestedUserInfoResponse(CreateSelfProfileSnapshot(sessionState), cipher);
    }

    private static byte[] BuildRequestedUserInfoResponse(UserProfileSnapshot profileSnapshot, TafCipher cipher)
    {
        List<byte> body = [0x0A, 0x00];
        AddLengthPrefixedString(body, profileSnapshot.Nickname);
        AddLengthPrefixedString(body, profileSnapshot.UserId);
        AddLengthPrefixedString(body, profileSnapshot.Nickname);
        body.Add(profileSnapshot.Gender);
        AddUInt32(body, profileSnapshot.Experience);
        AddLengthPrefixedString(body, profileSnapshot.Nickname);
        AddLengthPrefixedString(body, profileSnapshot.UserId);
        AddUInt32(body, profileSnapshot.Experience);
        AddInt32(body, profileSnapshot.GameMoney);
        AddUInt32(body, profileSnapshot.GameCash);
        body.Add(profileSnapshot.Power);

        byte[] plainFrame = BuildPlainFrame([.. body]);
        return EncryptTafFrame(cipher, plainFrame);
    }

    private static byte[] BuildRoomUserInfoResponse(UserProfileSnapshot profileSnapshot, TafCipher cipher)
    {
        List<byte> body = [0x0A, 0x04];
        AddLengthPrefixedString(body, profileSnapshot.Nickname);
        AddInt32(body, profileSnapshot.GameMoney);
        AddInt32(body, profileSnapshot.ApPoint);
        AddInt32(body, profileSnapshot.TournamentRank);
        AddLengthPrefixedString(body, profileSnapshot.CoupleNickname);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildUserInfoErrorResponse(TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x0A, 0x01]));
    }

    private static byte[] BuildUserAvatarInfoResponse(GameClientSessionState sessionState, string nickname, TafCipher cipher)
    {
        ushort[] defaultAvatarItems = ResolveAvatarItems(sessionState, nickname);
        ushort[] equippedAvatarItems = ResolveEquippedAvatarItems(sessionState, nickname);

        List<byte> body = [0x0B, 0x00, 0x00];
        AddLengthPrefixedString(body, nickname);

        for (int index = 0; index < defaultAvatarItems.Length; index++)
        {
            AddUInt16(body, defaultAvatarItems[index]);
        }

        body.Add(checked((byte)Math.Min(equippedAvatarItems.Length, byte.MaxValue)));
        for (int index = 0; index < equippedAvatarItems.Length; index++)
        {
            AddUInt16(body, equippedAvatarItems[index]);
        }

        byte[] plainFrame = BuildPlainFrame([.. body]);
        return EncryptTafFrame(cipher, plainFrame);
    }

    private static byte[] BuildAvatarEquipChangeResponse(byte subOpcode, ushort itemCode, TafCipher cipher)
    {
        List<byte> body = [0x0B, subOpcode, 0x00];
        AddUInt16(body, itemCode);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildAvatarInventoryResponse(GameClientSessionState sessionState, TafCipher cipher)
    {
        List<byte> body = [0x0B, 0x07, 0x00];
        body.Add(0x00);
        AddUInt16(body, 0);
        AddUInt32(body, sessionState.UserGameCash);
        AddLengthPrefixedString(body, string.Empty);
        body.Add(0x00);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildChannelListStartResponse(ushort channelCount, TafCipher cipher)
    {
        List<byte> body = [0x08, 0x00, 0x00];
        AddUInt16(body, channelCount);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildChannelListItemResponse(LegacyChannelInfo channel, TafCipher cipher)
    {
        List<byte> body = [0x08, 0x00, 0x01, checked((byte)channel.Number)];
        AddLengthPrefixedString(body, channel.Name);
        AddUInt16(body, 0);
        AddUInt16(body, channel.MaxUsers);
        AddUInt16(body, channel.MinLevel);
        AddUInt16(body, channel.MaxLevel);
        body.Add(channel.EventNumber);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildChannelListEndResponse(TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x08, 0x00, 0x02]));
    }

    private static byte[] BuildEnterChannelSuccessResponse(byte channelNumber, TafCipher cipher)
    {
        byte[] plainFrame = BuildPlainFrame([0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]);
        return EncryptTafFrame(cipher, plainFrame);
    }

    private static byte[] BuildTournamentScrollResponse(TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x06, 0x00, 0x00, 0x00]));
    }

    private static byte[] BuildRoomListStartResponse(ushort roomCount, TafCipher cipher)
    {
        List<byte> body = [0x14, 0x00, 0x00];
        AddUInt16(body, roomCount);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildRoomListEndResponse(TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x14, 0x00, 0x02]));
    }

    private static byte[] BuildUserListStartResponse(ushort userCount, TafCipher cipher)
    {
        List<byte> body = [0x14, 0x01, 0x00];
        AddUInt16(body, userCount);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildUserListItemResponse(GameClientSessionState sessionState, TafCipher cipher)
    {
        List<byte> body = [0x14, 0x01, 0x01];
        AddLengthPrefixedString(body, sessionState.UserNickname);
        AddLengthPrefixedString(body, sessionState.UserId);
        body.Add(sessionState.UserGender);
        AddUInt32(body, sessionState.UserExperience);
        body.Add(sessionState.CurrentLocation);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildUserListEndResponse(TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x14, 0x01, 0x02]));
    }

    private static byte[] BuildChannelPresenceResponse(GameClientSessionState sessionState, TafCipher cipher)
    {
        List<byte> body = [0x15, 0x02];
        AddLengthPrefixedString(body, sessionState.UserNickname);
        AddLengthPrefixedString(body, sessionState.UserId);
        body.Add(sessionState.UserGender);
        AddUInt32(body, sessionState.UserExperience);
        body.Add(sessionState.CurrentLocation);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildNonLobbyEnterResponse(GameClientSessionState sessionState, TafCipher cipher)
    {
        List<byte> body = [0x15, 0x07];
        AddLengthPrefixedString(body, sessionState.UserNickname);
        body.Add(sessionState.CurrentLocation);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildMusicTicketRemainResponse(TafCipher cipher)
    {
        List<byte> body = [0x0E, 0x02, 0x00];
        AddInt32(body, 0);
        AddUInt32(body, 0);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildMusicMallHeaderErrorResponse(TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x0E, 0x00, 0x01]));
    }

    private static byte[] BuildAvatarItemListsStartResponse(ushort itemCount, TafCipher cipher)
    {
        List<byte> body = [0x04, 0x04, 0x00, (byte)'A'];
        AddUInt16(body, itemCount);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildAvatarItemListsEndResponse(TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x04, 0x04, 0x02]));
    }

    private static byte[] BuildAvatarBasketResponse(TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainAvatarBasketFrame());
    }

    private static byte[] BuildSaveCashPresentResponse(GameClientSessionState sessionState, TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainSaveCashPresentFrame(sessionState));
    }

    private static byte[] BuildPlainAvatarBasketFrame()
    {
        return BuildPlainFrame([0x0B, 0x09, 0x01, 0x00]);
    }

    private static byte[] BuildPlainSaveCashPresentFrame(GameClientSessionState sessionState)
    {
        List<byte> body = [0x04, 0x01, 0x00, 0x00];
        AddUInt32(body, 0);
        AddUInt32(body, sessionState.UserGameCash);
        AddUInt32(body, 0);
        return BuildPlainFrame([.. body]);
    }

    private static byte[] BuildCreateRoomMetaResponse(byte requestType, TafCipher cipher)
    {
        if (requestType == 0x01)
        {
            return EncryptTafFrame(cipher, BuildPlainFrame([0x16, 0x01, 0x04]));
        }

        List<byte> body = [0x16, 0x03, 0x00];
        AddInt32(body, 0);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildCreateRoomAcceptedResponse(ushort roomNumber, TafCipher cipher)
    {
        List<byte> body = [0x16, 0x00, 0x00];
        AddUInt16(body, roomNumber);
        body.Add(0x00);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static bool ShouldAutoStartRoomGame(GameClientSessionState sessionState)
    {
        return sessionState.RoomReady &&
            sessionState.CurrentRoomNumber != 0 &&
            sessionState.CurrentRoomSlotIndex == 0 &&
            (!sessionState.HasSyntheticRoomGuest || sessionState.SyntheticRoomGuestReady);
    }

    private static uint BeginRoomGame(GameClientSessionState sessionState)
    {
        uint randomSeed = (uint)RandomNumberGenerator.GetInt32(1, int.MaxValue);
        sessionState.CurrentLocation = 0x03;
        sessionState.RoomReady = false;
        sessionState.GameStartActive = true;
        sessionState.GameMusicReady = false;
        sessionState.SyntheticRoomGuestMusicReady = false;
        sessionState.GameStartSync = false;
        sessionState.SyntheticRoomGuestStartSync = false;
        sessionState.GameStartBroadcastSent = false;
        sessionState.GameStartTickCount = 0;
        sessionState.CurrentRoomMusicName = string.Empty;
        sessionState.CurrentRoomMusicChecksum = 0;
        sessionState.CurrentGameScore = 0;
        sessionState.CurrentGamePerfectCount = 0;
        return randomSeed;
    }

    private static byte[] BuildRoomCreatedBroadcastResponse(CreateRoomRequest request, ushort roomNumber, TafCipher cipher)
    {
        List<byte> body = [0x15, 0x00];
        AddUInt16(body, roomNumber);
        AddLengthPrefixedBytes(body, request.RoomNameBytes);
        body.Add(0x01);
        body.Add(request.MaxUsers);
        body.Add(0x00);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildEnterRoomResponse(GameClientSessionState sessionState, CreateRoomRequest request, ushort roomNumber, TafCipher cipher)
    {
        List<byte> body = [0x17, 0x00, 0x00];
        AddUInt16(body, roomNumber);
        AddLengthPrefixedBytes(body, request.RoomNameBytes);
        body.Add((byte)(sessionState.HasSyntheticRoomGuest ? 0x02 : 0x01));
        body.Add(request.MaxUsers);
        AddUInt16(body, sessionState.CurrentRoomMusicCode);
        AddUInt16(body, sessionState.CurrentRoomStageCode);
        body.Add(sessionState.CurrentRoomGameMode);
        body.Add(sessionState.CurrentRoomRandomMode);
        body.Add(request.RoomKind);

        if ((request.RoomKind & 0x08) != 0)
        {
            body.Add(0x00);
            AddInt32(body, 0);
            AddInt32(body, 0);
            body.Add(0x00);
        }

        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildLeaveRoomErrorResponse(TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x17, 0x01, 0x01]));
    }

    private static byte[] BuildRoomPresenceResponse(
        byte slotIndex,
        string nickname,
        string userId,
        byte gender,
        uint experience,
        bool isMaster,
        bool isReady,
        byte team,
        byte power,
        TafCipher cipher)
    {
        List<byte> body = [0x19, 0x00, slotIndex, 0x02];
        AddLengthPrefixedString(body, nickname);
        AddLengthPrefixedString(body, userId);
        body.Add(gender);
        AddUInt32(body, experience);
        body.Add(isMaster ? (byte)0x01 : (byte)0x00);
        body.Add(isReady ? (byte)0x01 : (byte)0x00);
        body.Add(team);
        body.Add(power);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildRoomMasterResponse(byte slotIndex, string nickname, TafCipher cipher)
    {
        List<byte> body = [0x19, 0x01, slotIndex];
        AddLengthPrefixedString(body, nickname);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildRoomMusicChangedResponse(GameClientSessionState sessionState, TafCipher cipher)
    {
        List<byte> body = [0x1A, 0x02, 0x00];
        AddUInt16(body, sessionState.CurrentRoomMusicCode);
        body.Add(sessionState.CurrentRoomRandomMusic);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildRoomMusicChangeAcceptedResponse(TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x1A, 0x02, 0x01]));
    }

    private static byte[] BuildRoomStageChangedResponse(GameClientSessionState sessionState, TafCipher cipher)
    {
        List<byte> body = [0x1A, 0x03, 0x00];
        AddUInt16(body, sessionState.CurrentRoomStageCode);
        body.Add(sessionState.CurrentRoomStageVariant);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildRoomGameModeChangedResponse(GameClientSessionState sessionState, TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x1A, 0x04, 0x00, sessionState.CurrentRoomGameMode]));
    }

    private static byte[] BuildRoomRandomModeChangedResponse(GameClientSessionState sessionState, TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x1A, 0x05, 0x00, sessionState.CurrentRoomRandomMode]));
    }

    private static byte[] BuildRoomReadyResponse(GameClientSessionState sessionState, TafCipher cipher)
    {
        return BuildRoomReadyStateResponse(sessionState.CurrentRoomSlotIndex, sessionState.RoomReady, cipher);
    }

    private static byte[] BuildRoomReadyStateResponse(byte slotIndex, bool isReady, TafCipher cipher)
    {
        return EncryptTafFrame(
            cipher,
            BuildPlainFrame([0x1B, 0x00, 0x00, slotIndex, isReady ? (byte)0x01 : (byte)0x00]));
    }

    private static byte[] BuildChatResponse(string nickname, string message, TafCipher cipher)
    {
        List<byte> body = [0x0C, 0x00];
        AddLengthPrefixedString(body, nickname);
        AddLengthPrefixedString(body, message);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildRoomTeamChangeResponse(GameClientSessionState sessionState, TafCipher cipher)
    {
        return EncryptTafFrame(
            cipher,
            BuildPlainFrame([0x1B, 0x01, 0x00, sessionState.CurrentRoomSlotIndex, sessionState.UserTeam]));
    }

    private static byte[] BuildRoomTeamChangeErrorResponse(TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x1B, 0x01, 0x01]));
    }

    private static byte[] BuildRoomAudienceErrorResponse(TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x1B, 0x04, 0x01]));
    }

    private static byte[] BuildRoomGameHackListResponse(TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x1B, 0x05, 0x00, 0x00]));
    }

    private static byte[] BuildRoomGameStartSuccessResponse(uint randomSeed, TafCipher cipher)
    {
        List<byte> body = [0x1B, 0x02, 0x00];
        AddUInt32(body, randomSeed);
        body.Add(0x01);
        body.Add(0x00);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildMusicReadyAcceptedResponse(byte loadResult, TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x1B, 0x03, 0x01, loadResult]));
    }

    private static byte[] BuildMusicReadyDataResponse(
        string musicName,
        uint serverChecksum,
        uint clientChecksum,
        byte[] musicBuffer,
        ushort totalMeasure,
        byte[] musicBody,
        TafCipher cipher)
    {
        List<byte> body = [0x1B, 0x03, 0x00];
        AddLengthPrefixedString(body, musicName);
        AddUInt32(body, serverChecksum);
        AddUInt32(body, clientChecksum);
        AddUInt16(body, checked((ushort)musicBuffer.Length));
        body.AddRange(musicBuffer);
        AddUInt16(body, totalMeasure);
        body.AddRange(musicBody);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildGameTimePingResponse(uint tickCount, TafCipher cipher)
    {
        List<byte> body = [0x00, 0x07];
        AddUInt32(body, tickCount);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildInGameStartSyncResponse(uint startTickCount, byte teamMode, TafCipher cipher)
    {
        List<byte> body = [0x1C, 0x01];
        AddUInt32(body, startTickCount);
        body.Add(teamMode);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildInGameDanceStepResponse(
        byte slotIndex,
        InGameDanceStepRequest request,
        uint tickCount,
        TafCipher cipher)
    {
        List<byte> body = [0x1C, 0x00, slotIndex];
        body.Add(request.Lane0);
        body.Add(request.Lane1);
        body.Add(request.Lane2);
        body.Add(request.Lane3);
        body.Add(request.Lane4);
        body.Add(request.Lane5);
        AddUInt16(body, request.Measure);
        AddInt16(body, request.Accuracy);
        AddInt32(body, request.ScoreDelta);
        body.Add(request.ComboFlag);
        body.Add(request.Judge);
        AddUInt16(body, request.Value0);
        AddUInt16(body, request.Value1);
        AddUInt32(body, tickCount);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildGameEndResultsResponse(GameEndResponseState state, TafCipher cipher)
    {
        List<byte> body = [0x1E, 0x00, 0x00, (byte)state.Entries.Count];

        foreach (GameEndScoreEntry entry in state.Entries)
        {
            body.Add(entry.SlotIndex);
            AddUInt32(body, entry.BeforeExperience);
            AddUInt32(body, entry.BaseExperience);
            AddUInt32(body, entry.BonusExperience);
            AddInt32(body, entry.MoneyEarned);
            body.Add(entry.MissionSucceeded ? (byte)0x01 : (byte)0x00);
            body.Add(entry.QuestMode);
        }

        body.Add(state.DoubleDenMultiplier);
        body.Add(state.MissionMultiplier);
        body.Add(state.TeamGenderPenaltyIndex);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildGameEndRoomReturnResponse(ushort roomNumber, TafCipher cipher)
    {
        List<byte> body = [0x15, 0x05];
        AddUInt16(body, roomNumber);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static GameEndResponseState BuildGameEndResponseState(GameClientSessionState sessionState, GameEndRequest request)
    {
        uint totalScore = request.TotalScore != 0 ? request.TotalScore : sessionState.CurrentGameScore;
        uint beforeExperience = sessionState.UserExperience;
        uint baseExperience = ResolveGameEndBaseExperience(totalScore, sessionState.CurrentGamePerfectCount);
        uint bonusExperience = ResolveGameEndBonusExperience(baseExperience, sessionState.CurrentGamePerfectCount);
        int moneyEarned = Math.Max(0, request.MoneyEarned);
        uint afterExperience = AddClampedUInt32(beforeExperience, AddClampedUInt32(baseExperience, bonusExperience));

        GameEndScoreEntry selfEntry = new(
            sessionState.CurrentRoomSlotIndex,
            totalScore,
            beforeExperience,
            baseExperience,
            bonusExperience,
            moneyEarned,
            afterExperience,
            MissionSucceeded: false,
            QuestMode: 0x00);

        List<GameEndScoreEntry> entries = [selfEntry];
        GameEndScoreEntry partnerEntry = default;

        if (sessionState.HasSyntheticRoomGuest)
        {
            uint partnerBeforeExperience = sessionState.SyntheticRoomGuestExperience;
            uint partnerScore = totalScore == 0 ? 0 : Math.Max(1u, totalScore * 17u / 20u);
            uint partnerBaseExperience = baseExperience * 4u / 5u;
            uint partnerBonusExperience = bonusExperience / 2u;
            int partnerMoney = moneyEarned * 3 / 4;
            uint partnerAfterExperience = AddClampedUInt32(
                partnerBeforeExperience,
                AddClampedUInt32(partnerBaseExperience, partnerBonusExperience));

            partnerEntry = new GameEndScoreEntry(
                sessionState.SyntheticRoomGuestSlotIndex,
                partnerScore,
                partnerBeforeExperience,
                partnerBaseExperience,
                partnerBonusExperience,
                partnerMoney,
                partnerAfterExperience,
                MissionSucceeded: false,
                QuestMode: 0x00);

            entries.Add(partnerEntry);
        }

        return new GameEndResponseState(entries, selfEntry, partnerEntry, 0x01, 0x01, 0x00);
    }

    private static uint ResolveGameEndBaseExperience(uint totalScore, int perfectCount)
    {
        uint scoreReward = totalScore / 2500u;
        uint perfectReward = (uint)Math.Max(0, perfectCount) * 5u;
        return Math.Max(25u, AddClampedUInt32(scoreReward, perfectReward));
    }

    private static uint ResolveGameEndBonusExperience(uint baseExperience, int perfectCount)
    {
        if (perfectCount < 10)
        {
            return 0;
        }

        return Math.Max(10u, baseExperience / 4u);
    }

    private static uint AddClampedUInt32(uint left, uint right)
    {
        ulong total = (ulong)left + right;
        return total > uint.MaxValue ? uint.MaxValue : (uint)total;
    }

    private static int AddClampedInt32(int left, int right)
    {
        long total = (long)left + right;
        if (total > int.MaxValue)
        {
            return int.MaxValue;
        }

        if (total < int.MinValue)
        {
            return int.MinValue;
        }

        return (int)total;
    }

    private static byte[] BuildRoomGameStartErrorResponse(TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x1B, 0x02, 0x01]));
    }

    private static byte[] BuildInviteListResponse(IReadOnlyList<InviteListEntry> entries, TafCipher cipher)
    {
        List<byte> body = [0x05, 0x00, 0x00, (byte)Math.Min(entries.Count, 20)];
        for (int index = 0; index < entries.Count && index < 20; index++)
        {
            InviteListEntry entry = entries[index];
            body.Add(entry.Level);
            body.Add(entry.Gender);
            AddLengthPrefixedString(body, entry.Nickname);
        }

        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private static byte[] BuildInviteListErrorResponse(byte errorCode, TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x05, 0x00, errorCode]));
    }

    private static byte[] BuildRoomSlotListStartResponse(TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x18, 0x00, 0x00, 0x11]));
    }

    private static byte[] BuildRoomSlotListItemResponse(GameClientSessionState sessionState, byte slotIndex, TafCipher cipher)
    {
        List<byte> body = [0x18, 0x00, 0x01, slotIndex];
        if (slotIndex == sessionState.CurrentRoomSlotIndex)
        {
            body.Add(0x02);
            AddLengthPrefixedString(body, sessionState.UserNickname);
            AddLengthPrefixedString(body, sessionState.UserId);
            body.Add(sessionState.UserGender);
            AddUInt32(body, sessionState.UserExperience);
            body.Add(0x01);
            body.Add(sessionState.RoomReady ? (byte)0x01 : (byte)0x00);
            body.Add(sessionState.UserTeam);
            body.Add(sessionState.UserPower);
            return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
        }

        if (sessionState.HasSyntheticRoomGuest && slotIndex == sessionState.SyntheticRoomGuestSlotIndex)
        {
            body.Add(0x02);
            AddLengthPrefixedString(body, sessionState.SyntheticRoomGuestNickname);
            AddLengthPrefixedString(body, sessionState.SyntheticRoomGuestUserId);
            body.Add(sessionState.SyntheticRoomGuestGender);
            AddUInt32(body, sessionState.SyntheticRoomGuestExperience);
            body.Add(0x00);
            body.Add(sessionState.SyntheticRoomGuestReady ? (byte)0x01 : (byte)0x00);
            body.Add(sessionState.SyntheticRoomGuestTeam);
            body.Add(sessionState.SyntheticRoomGuestPower);
            return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
        }

        body.Add(0x00);
        return EncryptTafFrame(cipher, BuildPlainFrame([.. body]));
    }

    private ServerMusicReadyPayload ResolveMusicReadyPayload(string musicName, uint clientChecksum)
    {
        if (string.IsNullOrWhiteSpace(musicName))
        {
            return new ServerMusicReadyPayload(3, 0, [], [], 0);
        }

        string baseName = Path.GetFileNameWithoutExtension(musicName);
        if (string.IsNullOrWhiteSpace(baseName))
        {
            return new ServerMusicReadyPayload(3, 0, [], [], 0);
        }

        string headerPath = Path.Combine(_runtimePaths.SoundPath, $"{baseName}.thd");
        string bodyPath = Path.Combine(_runtimePaths.SoundPath, "body", $"{baseName}.bdy");
        if (!File.Exists(headerPath) || !File.Exists(bodyPath))
        {
            return new ServerMusicReadyPayload(1, 0, [], [], 0);
        }

        byte[] headerBytes = File.ReadAllBytes(headerPath);
        byte[] bodyBytes = File.ReadAllBytes(bodyPath);
        if (headerBytes.Length < 4 || bodyBytes.Length < 0x21)
        {
            return new ServerMusicReadyPayload(1, 0, [], [], 0);
        }

        uint serverChecksum = BinaryPrimitives.ReadUInt32LittleEndian(headerBytes.AsSpan(0, 4));
        ushort totalMeasure = ResolveMusicTotalMeasure(musicName);

        if (clientChecksum != serverChecksum)
        {
            return new ServerMusicReadyPayload(2, serverChecksum, [], [], totalMeasure);
        }

        byte[] musicBuffer = headerBytes[4..];
        byte[] musicBody = bodyBytes[..0x21];
        return new ServerMusicReadyPayload(0, serverChecksum, musicBuffer, musicBody, totalMeasure);
    }

    private ushort ResolveMusicTotalMeasure(string musicName)
    {
        string fileName = Path.GetFileName(musicName);
        return _musicTotalMeasures.Value.TryGetValue(fileName, out ushort totalMeasure)
            ? totalMeasure
            : (ushort)0;
    }

    private static Dictionary<string, ushort> LoadMusicTotalMeasures(string slkPath)
    {
        var totals = new Dictionary<string, ushort>(StringComparer.OrdinalIgnoreCase);
        if (!File.Exists(slkPath))
        {
            return totals;
        }

        int currentRow = -1;
        string? currentFileName = null;
        ushort currentTotalMeasure = 0;

        foreach (string rawLine in File.ReadLines(slkPath))
        {
            if (!TryParseMusicSlkCell(rawLine, out int? row, out int column, out string value))
            {
                continue;
            }

            if (row.HasValue && row.Value != currentRow)
            {
                if (!string.IsNullOrEmpty(currentFileName))
                {
                    totals[currentFileName] = currentTotalMeasure;
                }

                currentRow = row.Value;
                currentFileName = null;
                currentTotalMeasure = 0;
            }

            if (currentRow <= 1)
            {
                continue;
            }

            if (column == 3)
            {
                currentFileName = value;
                continue;
            }

            if (column == 14 && ushort.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out ushort parsedTotalMeasure))
            {
                currentTotalMeasure = parsedTotalMeasure;
            }
        }

        if (!string.IsNullOrEmpty(currentFileName))
        {
            totals[currentFileName] = currentTotalMeasure;
        }

        return totals;
    }

    private static bool TryParseMusicSlkCell(string line, out int? row, out int column, out string value)
    {
        row = null;
        column = 0;
        value = string.Empty;

        if (!line.StartsWith("C;", StringComparison.Ordinal) || !line.Contains(";K", StringComparison.Ordinal))
        {
            return false;
        }

        string[] parts = line.Split(';');
        foreach (string part in parts)
        {
            if (part.StartsWith("Y", StringComparison.Ordinal) && int.TryParse(part[1..], NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedRow))
            {
                row = parsedRow;
                continue;
            }

            if (part.StartsWith("X", StringComparison.Ordinal) && int.TryParse(part[1..], NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedColumn))
            {
                column = parsedColumn;
                continue;
            }

            if (part.StartsWith("K", StringComparison.Ordinal))
            {
                value = part[1..].Trim('"');
            }
        }

        return column != 0 && value.Length != 0;
    }

    private static byte[] BuildRoomSlotListEndResponse(TafCipher cipher)
    {
        return EncryptTafFrame(cipher, BuildPlainFrame([0x18, 0x00, 0x02]));
    }

    private static LegacyChannelInfo[] BuildDefaultChannels()
    {
        return
        [
            new LegacyChannelInfo(0, "Channel 1", 200, 100, 0, 61, 0),
            new LegacyChannelInfo(1, "Channel 2", 200, 100, 0, 61, 0),
            new LegacyChannelInfo(2, "Channel 3", 200, 100, 0, 61, 0),
            new LegacyChannelInfo(3, "Channel 4", 200, 100, 0, 61, 0),
            new LegacyChannelInfo(4, "Channel 5", 200, 100, 0, 61, 0),
            new LegacyChannelInfo(5, "Channel 6", 200, 100, 0, 61, 0),
            new LegacyChannelInfo(6, "Channel 7", 200, 100, 0, 61, 0),
            new LegacyChannelInfo(7, "Channel 8", 200, 100, 0, 61, 0),
            new LegacyChannelInfo(8, "Channel 9", 200, 100, 0, 61, 0),
            new LegacyChannelInfo(9, "Channel 10", 200, 100, 0, 61, 0),
        ];
    }

    private static string ResolveLoginNickname(uint userSerial)
    {
        return userSerial == 0 ? "guest" : $"user{userSerial}";
    }

    private static string ResolveLoginUserId(uint userSerial)
    {
        return userSerial == 0 ? "guest" : $"user{userSerial}";
    }

    private static string? BuildDatabaseConnectionString(IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection("Database");
        string? server = section["Server"];
        string? database = section["Database"];
        string? user = section["User"];
        string? password = section["Password"];

        if (string.IsNullOrWhiteSpace(server) ||
            string.IsNullOrWhiteSpace(database) ||
            string.IsNullOrWhiteSpace(user))
        {
            return null;
        }

        bool sslEnabled = bool.TryParse(section["SslEnabled"], out bool parsedSslEnabled) && parsedSslEnabled;
        string sslMode = sslEnabled ? "Required" : "Disabled";
        return $"Server={server};Database={database};Uid={user};Pwd={password};SslMode={sslMode};";
    }

    private async Task PopulateSessionProfileAsync(GameClientSessionState sessionState, CancellationToken cancellationToken)
    {
        sessionState.UserNickname = ResolveLoginNickname(sessionState.UserSerial);
        sessionState.UserId = ResolveLoginUserId(sessionState.UserSerial);

        if (string.IsNullOrWhiteSpace(_databaseConnectionString))
        {
            return;
        }

        try
        {
            await using var connection = new MySqlConnection(_databaseConnectionString);
            await connection.OpenAsync(cancellationToken);

            const string sql = """
                SELECT ui.UserNick, ui.UserID, ui.Exp, ui.Level, ui.Money, ui.Cash, u.UserGender, COALESCE(li.Exp, 0) AS LevelExp
                FROM UserInfo ui
                INNER JOIN Users u ON u.UserSN = ui.UserSN
                LEFT JOIN levelinfo li ON li.Level = ui.Level
                WHERE ui.UserSN = @sn
                LIMIT 1
                """;

            await using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@sn", sessionState.UserSerial);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                return;
            }

            string databaseNickname = ReadDbString(reader, "UserNick");
            string databaseUserId = ReadDbString(reader, "UserID");

            if (!string.IsNullOrWhiteSpace(databaseNickname) && databaseNickname != "0")
            {
                sessionState.UserNickname = databaseNickname;
            }

            if (!string.IsNullOrWhiteSpace(databaseUserId) && databaseUserId != "0")
            {
                sessionState.UserId = databaseUserId;
            }

            sessionState.UserLevel = ReadDbInt32(reader, "Level");
            uint rawExperience = ReadDbUInt32(reader, "Exp");
            uint levelExperience = ReadDbUInt32(reader, "LevelExp");
            sessionState.UserExperience = ResolveProtocolExperience(rawExperience, sessionState.UserLevel, levelExperience);
            sessionState.UserGameMoney = ReadDbInt32(reader, "Money");
            sessionState.UserGameCash = ReadDbUInt32(reader, "Cash");
            sessionState.UserGender = ParseProtocolGender(ReadDbString(reader, "UserGender"));

            logger.LogInformation(
                "[AUDITION FLOW] ENTRAR_SALA_PROFILE_DB | UserSN={UserSN} | UserId={UserId} | Nickname={Nickname} | Gender={Gender} | Level={Level} | RawExperience={RawExperience} | LevelExperience={LevelExperience} | ProtocolExperience={ProtocolExperience} | Cash={Cash}",
                sessionState.UserSerial,
                sessionState.UserId,
                sessionState.UserNickname,
                sessionState.UserGender,
                sessionState.UserLevel,
                rawExperience,
                levelExperience,
                sessionState.UserExperience,
                sessionState.UserGameCash);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "[AUDITION FLOW] ENTRAR_SALA_PROFILE_DB_FAIL | UserSN={UserSN}",
                sessionState.UserSerial);
        }
    }

    private static string ReadDbString(MySqlDataReader reader, string column)
    {
        int ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? string.Empty : Convert.ToString(reader.GetValue(ordinal)) ?? string.Empty;
    }

    private static uint ReadDbUInt32(MySqlDataReader reader, string column)
    {
        int ordinal = reader.GetOrdinal(column);
        if (reader.IsDBNull(ordinal))
        {
            return 0;
        }

        return TryConvertDbUInt32(reader.GetValue(ordinal), out uint parsed) ? parsed : 0;
    }

    private static int ReadDbInt32(MySqlDataReader reader, string column)
    {
        int ordinal = reader.GetOrdinal(column);
        if (reader.IsDBNull(ordinal))
        {
            return 0;
        }

        return TryConvertDbInt32(reader.GetValue(ordinal), out int parsed) ? parsed : 0;
    }

    private static bool TryConvertDbUInt32(object value, out uint parsed)
    {
        try
        {
            parsed = value switch
            {
                byte byteValue => byteValue,
                sbyte sbyteValue when sbyteValue >= 0 => (uint)sbyteValue,
                short shortValue when shortValue >= 0 => (uint)shortValue,
                ushort ushortValue => ushortValue,
                int intValue when intValue >= 0 => (uint)intValue,
                uint uintValue => uintValue,
                long longValue when longValue >= 0 && longValue <= uint.MaxValue => (uint)longValue,
                ulong ulongValue when ulongValue <= uint.MaxValue => (uint)ulongValue,
                decimal decimalValue when decimalValue >= 0 && decimalValue <= uint.MaxValue => decimal.ToUInt32(decimal.Truncate(decimalValue)),
                string stringValue => uint.Parse(stringValue, CultureInfo.InvariantCulture),
                IConvertible convertibleValue => Convert.ToUInt32(convertibleValue, CultureInfo.InvariantCulture),
                _ => throw new InvalidCastException()
            };

            return true;
        }
        catch
        {
            parsed = 0;
            return false;
        }
    }

    private static bool TryConvertDbInt32(object value, out int parsed)
    {
        try
        {
            parsed = value switch
            {
                byte byteValue => byteValue,
                sbyte sbyteValue => sbyteValue,
                short shortValue => shortValue,
                ushort ushortValue => ushortValue,
                int intValue => intValue,
                uint uintValue when uintValue <= int.MaxValue => (int)uintValue,
                long longValue when longValue is >= int.MinValue and <= int.MaxValue => (int)longValue,
                ulong ulongValue when ulongValue <= int.MaxValue => (int)ulongValue,
                decimal decimalValue when decimalValue is >= int.MinValue and <= int.MaxValue => decimal.ToInt32(decimal.Truncate(decimalValue)),
                string stringValue => int.Parse(stringValue, CultureInfo.InvariantCulture),
                IConvertible convertibleValue => Convert.ToInt32(convertibleValue, CultureInfo.InvariantCulture),
                _ => throw new InvalidCastException()
            };

            return true;
        }
        catch
        {
            parsed = 0;
            return false;
        }
    }

    private static byte ParseProtocolGender(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0;
        }

        return value.Trim().ToUpperInvariant() switch
        {
            "M" or "1" => 1,
            "F" or "0" => 0,
            _ => 0
        };
    }

    private static uint ResolveProtocolExperience(uint rawExperience, int level, uint levelExperience)
    {
        if (level > 1 && levelExperience > rawExperience)
        {
            return levelExperience;
        }

        return rawExperience;
    }

    private static UserProfileSnapshot CreateSelfProfileSnapshot(GameClientSessionState sessionState)
    {
        return new UserProfileSnapshot(
            sessionState.UserNickname,
            sessionState.UserId,
            sessionState.UserGender,
            sessionState.UserExperience,
            sessionState.UserGameMoney,
            sessionState.UserGameCash,
            sessionState.UserPower,
            0,
            0,
            string.Empty);
    }

    private static bool TryResolveUserProfile(GameClientSessionState sessionState, string requestedNickname, out UserProfileSnapshot profileSnapshot)
    {
        if (string.Equals(requestedNickname, sessionState.UserNickname, StringComparison.OrdinalIgnoreCase))
        {
            profileSnapshot = CreateSelfProfileSnapshot(sessionState);
            return true;
        }

        if (sessionState.HasSyntheticRoomGuest &&
            string.Equals(requestedNickname, sessionState.SyntheticRoomGuestNickname, StringComparison.OrdinalIgnoreCase))
        {
            profileSnapshot = new UserProfileSnapshot(
                sessionState.SyntheticRoomGuestNickname,
                sessionState.SyntheticRoomGuestUserId,
                sessionState.SyntheticRoomGuestGender,
                sessionState.SyntheticRoomGuestExperience,
                0,
                0,
                sessionState.SyntheticRoomGuestPower,
                0,
                0,
                string.Empty);
            return true;
        }

        profileSnapshot = default;
        return false;
    }

    private static ushort[] ResolveAvatarItems(GameClientSessionState sessionState, string nickname)
    {
        if (string.Equals(nickname, sessionState.SyntheticRoomGuestNickname, StringComparison.OrdinalIgnoreCase))
        {
            return [.. LegacyDefaultAvatarItems];
        }

        EnsureDefaultAvatarItems(sessionState);
        return [.. sessionState.DefaultAvatarItems];
    }

    private static ushort[] ResolveEquippedAvatarItems(GameClientSessionState sessionState, string nickname)
    {
        if (string.Equals(nickname, sessionState.SyntheticRoomGuestNickname, StringComparison.OrdinalIgnoreCase))
        {
            return [.. LegacyDefaultAvatarItems];
        }

        if (!string.Equals(nickname, sessionState.UserNickname, StringComparison.OrdinalIgnoreCase))
        {
            return [];
        }

        if (sessionState.HasExplicitEquippedAvatarItems)
        {
            return [.. sessionState.EquippedAvatarItems];
        }

        ushort[] defaultAvatarItems = ResolveAvatarItems(sessionState, nickname);
        List<ushort> fallbackEquippedItems = [];
        for (int index = 0; index < defaultAvatarItems.Length; index++)
        {
            ushort avatarItem = defaultAvatarItems[index];
            if (avatarItem != 0)
            {
                fallbackEquippedItems.Add(avatarItem);
            }
        }

        return [.. fallbackEquippedItems];
    }

    private static void ApplyAvatarEquipChange(GameClientSessionState sessionState, byte subOpcode, ushort itemCode)
    {
        EnsureExplicitEquippedAvatarState(sessionState);
        _ = subOpcode;
        _ = itemCode;
    }

    private static void AddLengthPrefixedString(List<byte> buffer, string value)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(value ?? string.Empty);
        int length = Math.Min(bytes.Length, byte.MaxValue);
        buffer.Add((byte)length);
        buffer.AddRange(bytes.AsSpan(0, length).ToArray());
    }

    private static void AddLengthPrefixedBytes(List<byte> buffer, ReadOnlySpan<byte> value)
    {
        int length = Math.Min(value.Length, byte.MaxValue);
        buffer.Add((byte)length);
        buffer.AddRange(value.Slice(0, length).ToArray());
    }

    private static void AddUInt32(List<byte> buffer, uint value)
    {
        byte[] bytes = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(bytes, value);
        buffer.AddRange(bytes);
    }

    private static void AddUInt64(List<byte> buffer, ulong value)
    {
        byte[] bytes = new byte[8];
        BinaryPrimitives.WriteUInt64LittleEndian(bytes, value);
        buffer.AddRange(bytes);
    }

    private static void AddUInt16(List<byte> buffer, ushort value)
    {
        byte[] bytes = new byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(bytes, value);
        buffer.AddRange(bytes);
    }

    private static void AddInt16(List<byte> buffer, short value)
    {
        byte[] bytes = new byte[2];
        BinaryPrimitives.WriteInt16LittleEndian(bytes, value);
        buffer.AddRange(bytes);
    }

    private static void AddInt32(List<byte> buffer, int value)
    {
        byte[] bytes = new byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(bytes, value);
        buffer.AddRange(bytes);
    }

    private static byte[] BuildPlainFrame(byte[] body)
    {
        byte[] frame = new byte[body.Length + 2];
        BinaryPrimitives.WriteUInt16LittleEndian(frame.AsSpan(0, 2), (ushort)frame.Length);
        body.CopyTo(frame, 2);
        return frame;
    }

    private static byte[] EncryptTafFrame(TafCipher cipher, byte[] plainFrame)
    {
        byte[] encryptedPayload = cipher.Encrypt(plainFrame);
        byte[] frame = new byte[encryptedPayload.Length + 2];
        BinaryPrimitives.WriteUInt16LittleEndian(frame.AsSpan(0, 2), (ushort)frame.Length);
        encryptedPayload.CopyTo(frame, 2);
        return frame;
    }

    private static byte[] BuildAddKeyBytes(string addKeySeed)
    {
        int padLength = RandomNumberGenerator.GetInt32(8, 24);
        byte[] result = new byte[padLength + 9];
        result[0] = (byte)padLength;
        RandomNumberGenerator.Fill(result.AsSpan(1, padLength));
        Encoding.ASCII.GetBytes(addKeySeed).CopyTo(result, padLength + 1);
        return result;
    }

    private static ulong ModPow(ulong value, ulong exponent, ulong modulus)
    {
        return (ulong)BigInteger.ModPow(new BigInteger(value), new BigInteger(exponent), new BigInteger(modulus));
    }

    private static void EnsureDefaultAvatarItems(GameClientSessionState sessionState)
    {
        if (Array.Exists(sessionState.DefaultAvatarItems, item => item != 0))
        {
            return;
        }

        Array.Copy(LegacyDefaultAvatarItems, sessionState.DefaultAvatarItems, LegacyDefaultAvatarItems.Length);
    }

    private static void EnsureExplicitEquippedAvatarState(GameClientSessionState sessionState)
    {
        if (sessionState.HasExplicitEquippedAvatarItems)
        {
            return;
        }

        SyncEquippedAvatarItemsFromDefaults(sessionState);
    }

    private static void SyncEquippedAvatarItemsFromDefaults(GameClientSessionState sessionState)
    {
        EnsureDefaultAvatarItems(sessionState);

        sessionState.EquippedAvatarItems.Clear();
        for (int index = 0; index < sessionState.DefaultAvatarItems.Length; index++)
        {
            ushort avatarItem = sessionState.DefaultAvatarItems[index];
            if (avatarItem != 0)
            {
                sessionState.EquippedAvatarItems.Add(avatarItem);
            }
        }

        sessionState.HasExplicitEquippedAvatarItems = true;
    }

    private static bool TryReadLengthPrefixedString(ReadOnlySpan<byte> body, int offset, out string value, out int nextOffset)
    {
        value = string.Empty;
        nextOffset = offset;
        if (offset >= body.Length)
        {
            return false;
        }

        int length = body[offset];
        int start = offset + 1;
        int end = start + length;
        if (end > body.Length)
        {
            return false;
        }

        value = Encoding.ASCII.GetString(body.Slice(start, length));
        nextOffset = end;
        return true;
    }

    private static bool TryReadLengthPrefixedBytes(ReadOnlySpan<byte> body, int offset, out byte[] value, out int nextOffset)
    {
        value = [];
        nextOffset = offset;
        if (offset >= body.Length)
        {
            return false;
        }

        int length = body[offset];
        int start = offset + 1;
        int end = start + length;
        if (end > body.Length)
        {
            return false;
        }

        value = body.Slice(start, length).ToArray();
        nextOffset = end;
        return true;
    }

    private static bool TryReadLengthPrefixedString(ReadOnlySpan<byte> body, int offset, out string first, out string second, out string third)
    {
        first = string.Empty;
        second = string.Empty;
        third = string.Empty;

        if (!TryReadLengthPrefixedString(body, offset, out first, out int nextOffset))
        {
            return false;
        }

        if (!TryReadLengthPrefixedString(body, nextOffset, out second, out nextOffset))
        {
            return false;
        }

        return TryReadLengthPrefixedString(body, nextOffset, out third, out _);
    }

    private SyntheticPartnerProfile GetNextSyntheticPartnerProfile()
    {
        if (_syntheticPartnerProfiles.Count == 0)
        {
            return SyntheticPartnerProfile.Default;
        }

        int index = (Interlocked.Increment(ref _nextSyntheticPartnerIndex) - 1) % _syntheticPartnerProfiles.Count;
        if (index < 0)
        {
            index += _syntheticPartnerProfiles.Count;
        }

        return _syntheticPartnerProfiles[index];
    }

    private SyntheticPartnerProfile? GetConfiguredSyntheticPartnerProfile(SyntheticPartnerSettings settings)
    {
        if (!settings.Enabled)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(settings.SelectedProfileUserId))
        {
            foreach (SyntheticPartnerProfile profile in _syntheticPartnerProfiles)
            {
                if (string.Equals(profile.UserId, settings.SelectedProfileUserId, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(profile.Nickname, settings.SelectedProfileUserId, StringComparison.OrdinalIgnoreCase))
                {
                    return profile;
                }
            }
        }

        return GetNextSyntheticPartnerProfile();
    }

    private static void ClearSyntheticRoomGuest(GameClientSessionState sessionState)
    {
        sessionState.HasSyntheticRoomGuest = false;
        sessionState.SyntheticRoomGuestSlotIndex = 0;
        sessionState.SyntheticRoomGuestNickname = SyntheticPartnerProfile.Default.Nickname;
        sessionState.SyntheticRoomGuestUserId = SyntheticPartnerProfile.Default.UserId;
        sessionState.SyntheticRoomGuestGender = SyntheticPartnerProfile.Default.Gender;
        sessionState.SyntheticRoomGuestExperience = SyntheticPartnerProfile.Default.Experience;
        sessionState.SyntheticRoomGuestReady = false;
        sessionState.SyntheticRoomGuestTeam = 0;
        sessionState.SyntheticRoomGuestPower = 0;
    }

    private static void ConfigureSyntheticRoomGuest(GameClientSessionState sessionState, SyntheticPartnerProfile profile)
    {
        sessionState.HasSyntheticRoomGuest = true;
        sessionState.SyntheticRoomGuestSlotIndex = 1;
        sessionState.SyntheticRoomGuestNickname = profile.Nickname;
        sessionState.SyntheticRoomGuestUserId = profile.UserId;
        sessionState.SyntheticRoomGuestGender = profile.Gender;
        sessionState.SyntheticRoomGuestExperience = profile.Experience;
        sessionState.SyntheticRoomGuestReady = profile.Ready;
        sessionState.SyntheticRoomGuestTeam = profile.Team;
        sessionState.SyntheticRoomGuestPower = profile.Power;
    }

    private static IReadOnlyList<SyntheticPartnerProfile> LoadSyntheticPartnerProfiles(string filePath, ILogger logger)
    {
        try
        {
            if (File.Exists(filePath))
            {
                using FileStream stream = File.OpenRead(filePath);
                List<SyntheticPartnerProfile>? profiles = JsonSerializer.Deserialize<List<SyntheticPartnerProfile>>(stream, SyntheticPartnerJsonOptions);
                IReadOnlyList<SyntheticPartnerProfile> normalizedProfiles = NormalizeSyntheticPartnerProfiles(profiles);

                logger.LogInformation(
                    "Loaded {ProfileCount} synthetic partner profiles from {FilePath}",
                    normalizedProfiles.Count,
                    filePath);

                return normalizedProfiles;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to load synthetic partner profiles from {FilePath}. Falling back to defaults.", filePath);
        }

        IReadOnlyList<SyntheticPartnerProfile> defaults = NormalizeSyntheticPartnerProfiles(null);
        logger.LogInformation(
            "Using {ProfileCount} default synthetic partner profiles because {FilePath} was not available.",
            defaults.Count,
            filePath);
        return defaults;
    }

    private static SyntheticPartnerSettings LoadSyntheticPartnerSettings(string filePath, ILogger logger)
    {
        try
        {
            if (File.Exists(filePath))
            {
                using FileStream stream = File.OpenRead(filePath);
                SyntheticPartnerSettings? settings = JsonSerializer.Deserialize<SyntheticPartnerSettings>(stream, SyntheticPartnerJsonOptions);
                return NormalizeSyntheticPartnerSettings(settings);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to load synthetic partner settings from {FilePath}. Using defaults.", filePath);
        }

        return SyntheticPartnerSettings.Default;
    }

    private static SyntheticPartnerSettings NormalizeSyntheticPartnerSettings(SyntheticPartnerSettings? settings)
    {
        if (settings is null)
        {
            return SyntheticPartnerSettings.Default;
        }

        return settings.Value with
        {
            SelectedProfileUserId = settings.Value.SelectedProfileUserId?.Trim() ?? string.Empty
        };
    }

    private static IReadOnlyList<SyntheticPartnerProfile> NormalizeSyntheticPartnerProfiles(IEnumerable<SyntheticPartnerProfile>? profiles)
    {
        List<SyntheticPartnerProfile> normalized = [];
        int index = 1;

        foreach (SyntheticPartnerProfile profile in profiles ?? [])
        {
            string nickname = string.IsNullOrWhiteSpace(profile.Nickname) ? $"partner{index}" : profile.Nickname.Trim();
            string userId = string.IsNullOrWhiteSpace(profile.UserId) ? nickname : profile.UserId.Trim();

            normalized.Add(new SyntheticPartnerProfile(
                nickname,
                userId,
                profile.Gender,
                profile.Experience == 0 ? 1u : profile.Experience,
                profile.Power,
                profile.Ready,
                profile.Team));

            index++;
        }

        if (normalized.Count == 0)
        {
            normalized.Add(SyntheticPartnerProfile.Default);
            normalized.Add(new SyntheticPartnerProfile("stardust", "stardust", 1, 540000, 7, true, 0));
            normalized.Add(new SyntheticPartnerProfile("rhythmfox", "rhythmfox", 0, 920000, 9, true, 0));
        }

        return normalized;
    }
}

internal readonly record struct InitialLoginRequest(byte HeaderA, byte HeaderB, string UserId, string Password, string Version);

internal readonly record struct KeyExchangeRequest(ulong ClientPublicKey, string ClientHashHex);

internal readonly record struct GameLoginRequest(byte LoginMode, uint UserSerial, string ClientVersion);

internal readonly record struct RoomHostChangeRequest(byte SubOpcode, ushort Value16, byte Value8);

internal readonly record struct InviteListEntry(string Nickname, byte Gender, byte Level);

internal readonly record struct UserProfileSnapshot(
    string Nickname,
    string UserId,
    byte Gender,
    uint Experience,
    int GameMoney,
    uint GameCash,
    byte Power,
    int TournamentRank,
    int ApPoint,
    string CoupleNickname);

internal readonly record struct SyntheticPartnerProfile(
    string Nickname,
    string UserId,
    byte Gender,
    uint Experience,
    byte Power,
    bool Ready,
    byte Team)
{
    public static SyntheticPartnerProfile Default => new("partner", "partner", 1, 1, 0, true, 0);
}

internal readonly record struct SyntheticPartnerSettings(
    bool Enabled,
    string SelectedProfileUserId)
{
    public static SyntheticPartnerSettings Default => new(false, string.Empty);
}

internal sealed class GameClientSessionState
{
    public TafCipher ReceiveCipher { get; set; } = TafCipher.CreateDefault();

    public TafCipher SendCipher { get; set; } = TafCipher.CreateDefault();

    public LoginHandshakeState? Handshake { get; set; }

    public bool RawLoginHandled { get; set; }

    public bool KeyExchangeHandled { get; set; }

    public bool ApplicationLoginHandled { get; set; }

    public uint UserSerial { get; set; }

    public string ClientVersion { get; set; } = string.Empty;

    public string UserNickname { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public byte UserGender { get; set; }

    public int UserLevel { get; set; }

    public uint UserExperience { get; set; }

    public int UserGameMoney { get; set; }

    public uint UserGameCash { get; set; }

    public byte UserPower { get; set; }

    public byte UserTeam { get; set; }

    public ushort[] DefaultAvatarItems { get; } = new ushort[5];

    public List<ushort> EquippedAvatarItems { get; } = [];

    public bool HasExplicitEquippedAvatarItems { get; set; }

    public byte CurrentChannelNumber { get; set; }

    public byte CurrentLocation { get; set; }

    public long LastSuccessfulLeaveRoomTick { get; set; }

    public ushort CurrentRoomNumber { get; set; }

    public byte CurrentRoomSlotIndex { get; set; }

    public bool RoomReady { get; set; }

    public ushort CurrentRoomMusicCode { get; set; }

    public ushort CurrentRoomStageCode { get; set; }

    public byte CurrentRoomStageVariant { get; set; }

    public byte CurrentRoomGameMode { get; set; }

    public byte CurrentRoomRandomMusic { get; set; }

    public byte CurrentRoomRandomMode { get; set; }

    public bool GameStartActive { get; set; }

    public bool GameMusicReady { get; set; }

    public bool SyntheticRoomGuestMusicReady { get; set; }

    public bool GameStartSync { get; set; }

    public bool SyntheticRoomGuestStartSync { get; set; }

    public bool GameStartBroadcastSent { get; set; }

    public uint GameStartTickCount { get; set; }

    public string CurrentRoomMusicName { get; set; } = string.Empty;

    public uint CurrentRoomMusicChecksum { get; set; }

    public bool HasSyntheticRoomGuest { get; set; }

    public byte SyntheticRoomGuestSlotIndex { get; set; }

    public string SyntheticRoomGuestNickname { get; set; } = "partner";

    public string SyntheticRoomGuestUserId { get; set; } = "partner";

    public byte SyntheticRoomGuestGender { get; set; } = 1;

    public uint SyntheticRoomGuestExperience { get; set; } = 1;

    public bool SyntheticRoomGuestReady { get; set; }

    public byte SyntheticRoomGuestTeam { get; set; }

    public byte SyntheticRoomGuestPower { get; set; }

    public bool CashPresentLoaded { get; set; }

    public bool AvatarItemListsLoaded { get; set; }

    public uint CurrentGameScore { get; set; }

    public int CurrentGamePerfectCount { get; set; }
}

internal readonly record struct CreateRoomRequest(byte[] RoomNameBytes, byte MaxUsers, byte RoomKind);

internal readonly record struct ServerMusicReadyPayload(
    byte LoadResult,
    uint ServerChecksum,
    byte[] MusicBuffer,
    byte[] MusicBody,
    ushort TotalMeasure);

internal readonly record struct InGameDanceStepRequest(
    byte PacketCount,
    byte Lane0,
    byte Lane1,
    byte Lane2,
    byte Lane3,
    byte Lane4,
    byte Lane5,
    ushort Measure,
    short Accuracy,
    int ScoreDelta,
    byte ComboFlag,
    byte Judge,
    ushort Value0,
    ushort Value1);

internal readonly record struct GameEndRequest(
    byte SubOpcode,
    uint TotalScore,
    byte ReportedLevel,
    int MoneyEarned);

internal readonly record struct GameEndScoreEntry(
    byte SlotIndex,
    uint TotalScore,
    uint BeforeExperience,
    uint BaseExperience,
    uint BonusExperience,
    int MoneyEarned,
    uint AfterExperience,
    bool MissionSucceeded,
    byte QuestMode);

internal readonly record struct GameEndResponseState(
    IReadOnlyList<GameEndScoreEntry> Entries,
    GameEndScoreEntry SelfEntry,
    GameEndScoreEntry PartnerEntry,
    byte DoubleDenMultiplier,
    byte MissionMultiplier,
    byte TeamGenderPenaltyIndex);

internal sealed class LoginHandshakeState
{
    public ulong Generator { get; init; }

    public ulong Prime { get; init; }

    public ulong PrivateKey { get; init; }

    public ulong PublicKey { get; init; }

    public string AddKeySeed { get; init; } = string.Empty;

    public static LoginHandshakeState Create()
    {
        ulong generator = 2147483629;
        ulong prime = 2147483647;
        ulong privateKey = (ulong)RandomNumberGenerator.GetInt32(1, int.MaxValue);
        ulong publicKey = (ulong)BigInteger.ModPow(new BigInteger(generator), new BigInteger(privateKey), new BigInteger(prime));
        DateTime now = DateTime.Now;
        string addKeySeed = string.Create(
            8,
            (now.Hour, now.Minute, now.Second, now.Millisecond & 0xFF),
            static (chars, state) =>
            {
                state.Hour.TryFormat(chars[0..2], out _, "X2");
                state.Minute.TryFormat(chars[2..4], out _, "X2");
                state.Second.TryFormat(chars[4..6], out _, "X2");
                state.Item4.TryFormat(chars[6..8], out _, "X2");
            });

        return new LoginHandshakeState
        {
            Generator = generator,
            Prime = prime,
            PrivateKey = privateKey,
            PublicKey = publicKey,
            AddKeySeed = addKeySeed,
        };
    }
}

internal sealed class TafCipher
{
    private readonly byte[] _state = new byte[256];
    private int _x;
    private int _y;

    private TafCipher(ReadOnlySpan<byte> key)
    {
        Initialize(key);
    }

    public static TafCipher CreateDefault()
    {
        return new TafCipher([0x29, 0x34, 0x59, 0x92, 0xFB, 0x42, 0xBA, 0x5F, 0xC7, 0x12, 0xF7, 0x32, 0xF1, 0xA8, 0x29, 0xC9]);
    }

    public static TafCipher FromKey(ReadOnlySpan<byte> key)
    {
        return new TafCipher(key);
    }

    public byte[] Encrypt(ReadOnlySpan<byte> plainPayload)
    {
        int randomPadLength = RandomNumberGenerator.GetInt32(3, 13);
        byte[] encrypted = new byte[plainPayload.Length + randomPadLength + 2];
        encrypted[0] = 0x01;
        encrypted[1] = (byte)(randomPadLength ^ NextByte());

        for (int index = 0; index < randomPadLength; index++)
        {
            encrypted[index + 2] = (byte)(RandomNumberGenerator.GetInt32(0, 255) ^ NextByte());
        }

        for (int index = 0; index < plainPayload.Length; index++)
        {
            encrypted[randomPadLength + 2 + index] = (byte)(plainPayload[index] ^ NextByte());
        }

        return encrypted;
    }

    public byte[] Decrypt(ReadOnlySpan<byte> encryptedPayload)
    {
        if (encryptedPayload.Length < 3)
        {
            return Array.Empty<byte>();
        }

        byte packetType = encryptedPayload[0];
        if (packetType == 0x00)
        {
            return encryptedPayload[1..].ToArray();
        }

        if (packetType != 0x01)
        {
            return Array.Empty<byte>();
        }

        int randomPadLength = encryptedPayload[1] ^ NextByte();
        int dataLength = encryptedPayload.Length - randomPadLength - 2;
        if (dataLength <= 0)
        {
            return Array.Empty<byte>();
        }

        int cursor = 2;
        for (int index = 0; index < randomPadLength; index++)
        {
            _ = NextByte();
            cursor++;
        }

        byte[] plain = new byte[dataLength];
        for (int index = 0; index < dataLength; index++)
        {
            plain[index] = (byte)(encryptedPayload[cursor + index] ^ NextByte());
        }

        return plain;
    }

    private void Initialize(ReadOnlySpan<byte> key)
    {
        _x = 0;
        _y = 0;

        for (int index = 0; index < _state.Length; index++)
        {
            _state[index] = (byte)index;
        }

        int j = 0;
        for (int index = 0; index < _state.Length; index++)
        {
            j = (j + _state[index] + key[index % key.Length]) & 0xFF;
            (_state[index], _state[j]) = (_state[j], _state[index]);
        }
    }

    private byte NextByte()
    {
        _x = (_x + 1) & 0xFF;
        byte sx = _state[_x];
        _y = (_y + sx) & 0xFF;
        byte sy = _state[_y];
        _state[_y] = sx;
        _state[_x] = sy;
        return _state[(sx + sy) & 0xFF];
    }
}