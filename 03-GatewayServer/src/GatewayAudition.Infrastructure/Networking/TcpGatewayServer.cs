using System.Net;
using System.Net.Sockets;
using System.IO;
using GatewayAudition.Application.DTOs;
using GatewayAudition.Domain.Entities;
using GatewayAudition.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GatewayAudition.Infrastructure.Networking;

public sealed class TcpGatewayServer : IGatewayServer, IDisposable
{
    private readonly ISessionManager _sessionManager;
    private readonly IPlayerManager _playerManager;
    private readonly IAccountServerManager _accountServerManager;
    private readonly IPacketHandler _packetHandler;
    private readonly ILogger<TcpGatewayServer> _logger;
    private readonly GatewaySettings _settings;
    private TcpListener? _listener;
    private CancellationTokenSource? _cts;

    public bool IsRunning { get; private set; }

    public TcpGatewayServer(
        ISessionManager sessionManager,
        IPlayerManager playerManager,
        IAccountServerManager accountServerManager,
        IPacketHandler packetHandler,
        IOptions<GatewaySettings> settings,
        ILogger<TcpGatewayServer> logger)
    {
        _sessionManager = sessionManager;
        _playerManager = playerManager;
        _accountServerManager = accountServerManager;
        _packetHandler = packetHandler;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task StartAsync(ushort port, CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start(backlog: int.MaxValue);
        IsRunning = true;

        _logger.LogInformation("Gateway server started on port {Port}", port);

        _ = Task.Run(() => AcceptClientsAsync(_cts.Token), _cts.Token);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Gateway server stopping...");
        IsRunning = false;
        _cts?.Cancel();
        _listener?.Stop();
        _logger.LogInformation("Gateway server stopped");
        return Task.CompletedTask;
    }

    private async Task AcceptClientsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _listener != null)
        {
            try
            {
                var tcpClient = await _listener.AcceptTcpClientAsync(cancellationToken);
                var session = _sessionManager.GetFreeSession();

                if (session == null)
                {
                    _logger.LogWarning("No free sessions available, rejecting connection");
                    tcpClient.Close();
                    continue;
                }

                var remoteEndpoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;
                string ipAddress = remoteEndpoint?.Address.ToString() ?? "unknown";

                session.SetSocketAndIp((uint)tcpClient.Client.Handle.ToInt32(), ipAddress);

                _logger.LogDebug("Client connected: {Ip} (session {Index})",
                    ipAddress, session.UniqueIndex);

                _ = Task.Run(() => HandleClientAsync(tcpClient, session, cancellationToken), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting client connection");
            }
        }
    }

    private async Task HandleClientAsync(TcpClient tcpClient, Session session, CancellationToken cancellationToken)
    {
        var user = new User();
        user.Initialize();
        user.UniqueIndex = session.UniqueIndex;
        user.Session.UniqueIndex = session.UniqueIndex;
        user.Session.SetSocketAndIp(session.SessionInfo.Socket, session.SessionInfo.IpAddress);
        bool loggedFirstReceive = false;

        try
        {
            using (tcpClient)
            await using (var stream = tcpClient.GetStream())
            {
                _playerManager.AddPlayer((int)session.UniqueIndex, user);
                _logger.LogInformation(
                    "[AUDITION FLOW] CLIENT_BOOTSTRAP_CONNECT | Ip={Ip} | Session={Index}",
                    session.SessionInfo.IpAddress,
                    session.UniqueIndex);

                while (!cancellationToken.IsCancellationRequested && tcpClient.Connected)
                {
                    int bytesRead = await stream.ReadAsync(
                        session.RecvBuffer.AsMemory(session.ReceivedSize), cancellationToken);

                    if (bytesRead == 0)
                        break;

                    if (!loggedFirstReceive)
                    {
                        loggedFirstReceive = true;
                        _logger.LogInformation(
                            "[AUDITION FLOW] CLIENT_BOOTSTRAP_FIRST_RECV | Ip={Ip} | Session={Index} | Bytes={Bytes} | Hex={Hex}",
                            session.SessionInfo.IpAddress,
                            session.UniqueIndex,
                            bytesRead,
                            Convert.ToHexString(session.RecvBuffer.AsSpan(session.ReceivedSize, bytesRead)));
                    }

                    session.ReceivedSize += bytesRead;
                    session.SessionInfo.UpdateAccessTime();
                    user.Session.SessionInfo.UpdateAccessTime();

                    var keepConnected = await ProcessReceivedDataAsync(user, session, stream, cancellationToken);
                    if (!keepConnected)
                        break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected on shutdown
        }
        catch (IOException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                ex,
                "[AUDITION FLOW] CLIENT_TRANSPORT_ABORT | Ip={Ip} | Session={Index} | UserSN={UserSerial} | Buffered={Buffered} | LastInbound={LastInbound} | LastInboundPlainHex={LastInboundPlainHex} | LastOutbound={LastOutbound} | LastOutboundPlainHex={LastOutboundPlainHex} | LastOutboundFrameHex={LastOutboundFrameHex}",
                session.SessionInfo.IpAddress,
                session.UniqueIndex,
                user.Player.UserSerialNumber,
                session.ReceivedSize,
                session.LastInboundPacketSummary,
                session.LastInboundPlainHex,
                session.LastOutboundPacketLabel,
                session.LastOutboundPlainHex,
                session.LastOutboundFrameHex);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling client {Ip} | Session={Index} | UserSN={UserSerial} | Buffered={Buffered} | LastInbound={LastInbound} | LastOutbound={LastOutbound}",
                session.SessionInfo.IpAddress,
                session.UniqueIndex,
                user.Player.UserSerialNumber,
                session.ReceivedSize,
                session.LastInboundPacketSummary,
                session.LastOutboundPacketLabel);
        }
        finally
        {
            if (user.AccountSessionId.HasValue)
            {
                try
                {
                    await _accountServerManager.CloseSessionAsync(user.AccountSessionId.Value, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to close account session {SessionId}", user.AccountSessionId.Value);
                }
            }

            _playerManager.RemovePlayer((int)session.UniqueIndex);
            _sessionManager.ReturnSession(session);

            _logger.LogInformation("[AUDITION FLOW] CLIENT_BOOTSTRAP_DISCONNECT | Ip={Ip} | Session={Index}",
                session.SessionInfo.IpAddress, session.UniqueIndex);
        }
    }

    private async Task<bool> ProcessReceivedDataAsync(
        User user, Session session, NetworkStream stream, CancellationToken cancellationToken)
    {
        while (session.ReceivedSize >= 2)
        {
            ushort packetSize = BitConverter.ToUInt16(session.RecvBuffer, 0);

            if (packetSize > session.TotalBufferSize || packetSize < 5)
            {
                _logger.LogWarning(
                    "[AUDITION FLOW] CLIENT_BOOTSTRAP_INVALID_FRAME | Ip={Ip} | DeclaredSize={Size} | Buffered={Buffered} | Hex={Hex}",
                    session.SessionInfo.IpAddress,
                    packetSize,
                    session.ReceivedSize,
                    Convert.ToHexString(session.RecvBuffer.AsSpan(0, session.ReceivedSize)));
                session.ReceivedSize = 0;
                return false;
            }

            if (session.ReceivedSize < packetSize)
                return true;

            if (!Packet.TryDecryptFrame(session.RecvBuffer.AsSpan(0, packetSize), out var payload))
            {
                _logger.LogWarning(
                    "[AUDITION FLOW] CLIENT_BOOTSTRAP_INVALID_DECRYPT | Ip={Ip} | DeclaredSize={Size} | Buffered={Buffered} | Hex={Hex}",
                    session.SessionInfo.IpAddress,
                    packetSize,
                    session.ReceivedSize,
                    Convert.ToHexString(session.RecvBuffer.AsSpan(0, packetSize)));
                session.ReceivedSize = 0;
                return false;
            }

            session.LastInboundFrameHex = Convert.ToHexString(session.RecvBuffer.AsSpan(0, packetSize));
            session.LastInboundPlainHex = Convert.ToHexString(payload);
            session.LastInboundPacketSummary = DescribePlainPacket(payload);

            _logger.LogInformation(
                "[AUDITION FLOW] CLIENT_PACKET_IN | Ip={Ip} | Session={Index} | Summary={Summary} | PlainHex={Hex}",
                session.SessionInfo.IpAddress,
                session.UniqueIndex,
                session.LastInboundPacketSummary,
                session.LastInboundPlainHex);

            var packet = new Packet();
            packet.Initialize((uint)Math.Max(payload.Length, 2));
            if (!packet.TrySetPlainPacket(payload))
            {
                _logger.LogWarning(
                    "[AUDITION FLOW] CLIENT_BOOTSTRAP_INVALID_PLAIN | Ip={Ip} | DeclaredSize={Size} | PlainHex={Hex}",
                    session.SessionInfo.IpAddress,
                    packetSize,
                    Convert.ToHexString(payload));
                session.ReceivedSize = 0;
                return false;
            }

            int remaining = session.ReceivedSize - packetSize;
            if (remaining > 0)
                Buffer.BlockCopy(session.RecvBuffer, packetSize, session.RecvBuffer, 0, remaining);
            session.ReceivedSize = remaining;

            if (!await _packetHandler.HandlePacketAsync(user, session, packet, stream, cancellationToken))
                return false;
        }

        return true;
    }

    private static string DescribePlainPacket(ReadOnlySpan<byte> plainPacket)
    {
        if (plainPacket.Length < 3)
        {
            return "PacketTooShort";
        }

        byte command = plainPacket[2];
        return command switch
        {
            0x00 when plainPacket.Length >= 4 => $"Control/SubOpcode=0x{plainPacket[3]:X2}",
            0x01 when plainPacket.Length >= 4 => $"Login/Region=0x{plainPacket[3]:X2}",
            0x03 when plainPacket.Length >= 4 => $"ServerDirectory/SubOpcode=0x{plainPacket[3]:X2}",
            _ => $"Opcode=0x{command:X2}"
        };
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _listener?.Stop();
    }
}
