using Audition.DBAgent.Game.Application.Contracts;
using Audition.DBAgent.Game.Application.Services;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Audition.DBAgent.Game.Host.Network;

public sealed class PacketDispatcher(
    IGameDbAgentService service,
    ILogger<PacketDispatcher> logger,
    ILegacyServerInfoProvider? legacyServerInfoProvider = null)
{
    public async Task<IReadOnlyList<byte[]>> DispatchAsync(byte[] data, CancellationToken ct)
    {
        if (data.Length == 0) return [];

        var hex = BitConverter.ToString(data);

        using var ms = new MemoryStream(data);
        using var reader = new BinaryReader(ms);

        try
        {
            byte opCode = reader.ReadByte();
            logger.LogDebug("OpCode: {OpCode} | Size: {Size} bytes | HEX: {Hex}", opCode, data.Length, hex);

            if (opCode == (byte)'[')
            {
                if (ms.Position >= ms.Length)
                {
                    logger.LogWarning("Legacy DBAgentPay packet missing sub-opcode | HEX: {Hex}", hex);
                    return [];
                }

                byte subOpcode = reader.ReadByte();
                return await HandleLegacyPacketAsync(subOpcode, reader, data.Length, hex, ct);
            }

            if (opCode is 0x1F or 0x20 or 0x4B or 0x5D)
            {
                return await HandleLegacyPacketAsync(opCode, reader, data.Length, hex, ct);
            }

            switch (opCode)
            {
                case 0: // Mall purchase
                    if (ms.Position + 12 <= ms.Length)
                        await HandlePurchaseAsync(reader, ct);
                    break;

                case 1: // Account info sync
                    if (ms.Position + 4 <= ms.Length)
                        await HandleAccountInfoAsync(reader, ct);
                    break;

                case 2: // Quest validation + completion
                    if (ms.Position + 20 <= ms.Length)
                        await HandleQuestAttemptAsync(reader, ct);
                    break;

                case 3: // Game results (Exp/Money)
                    if (ms.Position + 12 <= ms.Length)
                        await HandleGameResultsAsync(reader, ct);
                    break;

                case 4: // Heartbeat / KeepAlive
                    logger.LogDebug("Heartbeat received");
                    break;

                case 5: // Item ownership check
                    if (ms.Position + 8 <= ms.Length)
                        await HandleItemCheckAsync(reader, ct);
                    break;

                case 6: // Level quest log
                    if (ms.Position + 22 <= ms.Length)
                        await HandleLevelQuestLogAsync(reader, ct);
                    break;

                case 7: // Cash query
                    if (ms.Position + 4 <= ms.Length)
                        await HandleCashQueryAsync(reader, ct);
                    break;

                case 8: // Daily unique count (triggered by GameServer)
                    await HandleDayUniqueCountAsync(ct);
                    break;

                default:
                    logger.LogWarning("Unknown OpCode {OpCode}", opCode);
                    break;
            }

            return [];
        }
        catch (EndOfStreamException)
        {
            logger.LogWarning("Packet truncated - attempted to read beyond end of stream");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Critical error processing packet");
        }

        return [];
    }

    private Task<IReadOnlyList<byte[]>> HandleLegacyPacketAsync(byte subOpcode, BinaryReader reader, int size, string hex, CancellationToken ct)
    {
        switch (subOpcode)
        {
            case 0x1F:
                logger.LogWarning("Legacy DBAgentPay board-item request [0x1F] received but is not implemented yet | Size: {Size} bytes | HEX: {Hex}", size, hex);
                break;

            case 0x20:
                if (reader.BaseStream.Length - reader.BaseStream.Position >= 8)
                {
                    uint userSN = reader.ReadUInt32();
                    uint itemId = reader.ReadUInt32();
                    logger.LogInformation("[BOARD] Legacy item usage notification | UserSN: {UserSN} | ItemId: {ItemId}", userSN, itemId);
                }
                else
                {
                    logger.LogInformation("[BOARD] Legacy item usage notification [0x20] received | Size: {Size} bytes | HEX: {Hex}", size, hex);
                }

                break;

            case 0x4B:
                return HandleLegacyServerInfoPacketAsync(reader, size, hex, ct);

            case 0x5D:
                return Task.FromResult(HandleLegacyLevelQuestPacket(reader, size, hex));

            default:
                logger.LogWarning("Unknown legacy DBAgentPay sub-opcode {SubOpcode} | Size: {Size} bytes | HEX: {Hex}", subOpcode, size, hex);
                break;
        }

        return Task.FromResult<IReadOnlyList<byte[]>>([]);
    }

    private async Task<IReadOnlyList<byte[]>> HandleLegacyServerInfoPacketAsync(BinaryReader reader, int size, string hex, CancellationToken ct)
    {
        if (reader.BaseStream.Position >= reader.BaseStream.Length)
        {
            logger.LogWarning("Legacy server info packet [0x4B] missing subtype | Size: {Size} bytes | HEX: {Hex}", size, hex);
            return [];
        }

        byte subtype = reader.ReadByte();
        switch (subtype)
        {
            case 0:
                if (reader.BaseStream.Length - reader.BaseStream.Position >= sizeof(ushort))
                {
                    ushort serverNumber = reader.ReadUInt16();
                    if (legacyServerInfoProvider is null)
                    {
                        logger.LogWarning("Legacy server info bootstrap [0x4B.0] cannot be answered because no provider is registered | ServerNum: {ServerNum} | HEX: {Hex}", serverNumber, hex);
                        return [];
                    }

                    var serverInfo = await legacyServerInfoProvider.GetServerInfoAsync(serverNumber, ct);
                    if (serverInfo is null)
                    {
                        logger.LogWarning("Legacy server info bootstrap [0x4B.0] has no serverlist row | ServerNum: {ServerNum} | HEX: {Hex}", serverNumber, hex);
                        return [];
                    }

                    logger.LogInformation("Legacy server info bootstrap [0x4B.0] answered | ServerNum: {ServerNum} | Name: {Name} | IP: {Ip} | Port: {Port}", serverInfo.ServerNumber, serverInfo.Name, serverInfo.IpAddress, serverInfo.Port);
                    return [FrameLegacyPacket(BuildLegacyServerBootstrapPayload(serverInfo))];
                }
                else
                {
                    logger.LogWarning("Legacy server info bootstrap [0x4B.0] truncated | Size: {Size} bytes | HEX: {Hex}", size, hex);
                }

                break;

            case 1:
                if (reader.BaseStream.Length - reader.BaseStream.Position >= sizeof(ushort))
                {
                    ushort serverNumber = reader.ReadUInt16();
                    if (legacyServerInfoProvider is null)
                    {
                        logger.LogWarning("Legacy server info follow-up [0x4B.1] cannot be answered because no provider is registered | ServerNum: {ServerNum} | HEX: {Hex}", serverNumber, hex);
                        return [];
                    }

                    var channels = await legacyServerInfoProvider.GetChannelInfosAsync(serverNumber, ct);
                    logger.LogInformation("Legacy server info follow-up [0x4B.1] answered | ServerNum: {ServerNum} | Channels: {Count}", serverNumber, channels.Count);
                    return [FrameLegacyPacket(BuildLegacyServerChannelPayload(channels))];
                }
                else
                {
                    logger.LogInformation("Legacy server info follow-up [0x4B.1] received | Size: {Size} bytes | HEX: {Hex}", size, hex);
                }

                break;

            default:
                logger.LogWarning("Unknown legacy server info subtype [0x4B.{Subtype}] | Size: {Size} bytes | HEX: {Hex}", subtype, size, hex);
                break;
        }

            return [];
    }

    private IReadOnlyList<byte[]> HandleLegacyLevelQuestPacket(BinaryReader reader, int size, string hex)
    {
        if (reader.BaseStream.Position >= reader.BaseStream.Length)
        {
            logger.LogWarning("Legacy level quest packet [0x5D] missing subtype | Size: {Size} bytes | HEX: {Hex}", size, hex);
            return [];
        }

        byte subtype = reader.ReadByte();
        return subtype switch
        {
            0 => BuildLegacyLevelQuestSyncResponses(),
            1 => HandleLegacyLevelQuestCompletion(reader, size, hex),
            2 => HandleLegacyLevelQuestPrecheck(reader, size, hex),
            _ => LogUnknownLegacyLevelQuestSubtype(subtype, size, hex),
        };
    }

    private IReadOnlyList<byte[]> BuildLegacyLevelQuestSyncResponses()
    {
        var quests = service.GetCachedLevelQuests();
        var responses = new List<byte[]>(quests.Count + 1);

        foreach (var quest in quests)
        {
            var payload = new byte[3 + 32];
            payload[0] = 0x5D;
            payload[1] = 0x00;
            payload[2] = 0x00;

            WriteLegacyLevelQuestRow(payload.AsSpan(3, 32), quest);
            responses.Add(FrameLegacyPacket(payload));
        }

        responses.Add(FrameLegacyPacket([0x5D, 0x00, 0x01, 0x00, 0x00]));

        logger.LogInformation("[LEVEL QUEST] Sent {Count} legacy quest sync packets plus end marker", quests.Count);
        return responses;
    }

    private IReadOnlyList<byte[]> HandleLegacyLevelQuestCompletion(BinaryReader reader, int size, string hex)
    {
        if (reader.BaseStream.Length - reader.BaseStream.Position < sizeof(uint) + sizeof(byte) + sizeof(byte))
        {
            logger.LogWarning("Legacy level quest completion [0x5D.1] truncated | Size: {Size} bytes | HEX: {Hex}", size, hex);
            return [];
        }

        uint userSN = reader.ReadUInt32();
        byte levelQuestSn = reader.ReadByte();
        byte pass = reader.ReadByte();

        logger.LogInformation("[LEVEL QUEST] Legacy completion [0x5D.1] | UserSN: {UserSN} | QuestSN: {QuestSN} | Pass: {Pass} | Response not implemented yet", userSN, levelQuestSn, pass);
        return [];
    }

    private IReadOnlyList<byte[]> HandleLegacyLevelQuestPrecheck(BinaryReader reader, int size, string hex)
    {
        if (reader.BaseStream.Length - reader.BaseStream.Position < sizeof(uint) + sizeof(byte))
        {
            logger.LogWarning("Legacy level quest precheck [0x5D.2] truncated | Size: {Size} bytes | HEX: {Hex}", size, hex);
            return [];
        }

        uint userSN = reader.ReadUInt32();
        byte levelQuestSn = reader.ReadByte();

        logger.LogInformation("[LEVEL QUEST] Legacy precheck [0x5D.2] | UserSN: {UserSN} | QuestSN: {QuestSN} | Response not implemented yet", userSN, levelQuestSn);
        return [];
    }

    private IReadOnlyList<byte[]> LogUnknownLegacyLevelQuestSubtype(byte subtype, int size, string hex)
    {
        logger.LogWarning("Unknown legacy level quest subtype [0x5D.{Subtype}] | Size: {Size} bytes | HEX: {Hex}", subtype, size, hex);
        return [];
    }

    private static byte[] FrameLegacyPacket(ReadOnlySpan<byte> payload)
    {
        ushort packetLength = checked((ushort)(payload.Length + sizeof(ushort)));
        var framed = new byte[packetLength];
        BitConverter.TryWriteBytes(framed.AsSpan(0, sizeof(ushort)), packetLength);
        payload.CopyTo(framed.AsSpan(sizeof(ushort)));
        return framed;
    }

    private static byte[] BuildLegacyServerBootstrapPayload(LegacyServerInfoData serverInfo)
    {
        byte[] nameBytes = EncodeLegacyString(serverInfo.Name, 32);
        byte[] ipBytes = EncodeLegacyString(serverInfo.IpAddress, 32);

        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        writer.Write((byte)0x4B);
        writer.Write((byte)0x00);
        writer.Write((byte)nameBytes.Length);
        writer.Write(nameBytes);
        writer.Write(serverInfo.Port);
        writer.Write(serverInfo.CurrentUsers);
        writer.Write(serverInfo.MaxUsers);
        writer.Write(serverInfo.Grade);
        writer.Write(serverInfo.IpRestriction);
        writer.Write(serverInfo.DoubleDen);
        writer.Write((byte)ipBytes.Length);
        writer.Write(ipBytes);

        return ms.ToArray();
    }

    private static byte[] BuildLegacyServerChannelPayload(IReadOnlyList<LegacyChannelInfoData> channels)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        writer.Write((byte)0x4B);
        writer.Write((byte)0x01);
        writer.Write(checked((ushort)channels.Count));

        foreach (var channel in channels)
        {
            byte[] nameBytes = EncodeLegacyString(channel.Name, 32);
            writer.Write((byte)nameBytes.Length);
            writer.Write(nameBytes);
            writer.Write(channel.Number);
            writer.Write(channel.MaxUsers);
            writer.Write(channel.MaxRooms);
            writer.Write(channel.MinLevel);
            writer.Write(channel.MaxLevel);
            writer.Write(channel.EventNumber);
        }

        return ms.ToArray();
    }

    private static byte[] EncodeLegacyString(string value, int maxLength)
    {
        var bytes = Encoding.ASCII.GetBytes(value ?? string.Empty);
        if (bytes.Length <= maxLength)
        {
            return bytes;
        }

        return bytes[..maxLength];
    }

    private static void WriteLegacyLevelQuestRow(Span<byte> destination, Domain.Entities.LevelQuest quest)
    {
        destination.Clear();
        destination[0] = checked((byte)quest.Level);
        BitConverter.TryWriteBytes(destination.Slice(4, sizeof(int)), quest.RequiredExp);
        BitConverter.TryWriteBytes(destination.Slice(8, sizeof(int)), quest.Score);
        destination[12] = checked((byte)quest.Perfect);
        destination[13] = checked((byte)quest.ConsecutivePerfect);
        destination[14] = checked((byte)quest.GameMode);
        BitConverter.TryWriteBytes(destination.Slice(16, sizeof(ushort)), checked((ushort)quest.MusicCode));
        destination[18] = checked((byte)quest.StageCode);
        BitConverter.TryWriteBytes(destination.Slice(20, sizeof(int)), quest.Fee);
        BitConverter.TryWriteBytes(destination.Slice(24, sizeof(int)), quest.WinDen);
        BitConverter.TryWriteBytes(destination.Slice(28, sizeof(int)), quest.WinExp);
    }

    private async Task HandlePurchaseAsync(BinaryReader reader, CancellationToken ct)
    {
        uint userSN = reader.ReadUInt32();
        int itemId = reader.ReadInt32();
        int days = reader.ReadInt32();

        var command = new PurchaseCommand(userSN, itemId, days, Cost: 1000);
        await service.PurchaseItemAsync(command, ct);

        logger.LogInformation("[MALL] User {UserSN} purchased Item {ItemId} for {Days} days", userSN, itemId, days);
    }

    private async Task HandleAccountInfoAsync(BinaryReader reader, CancellationToken ct)
    {
        uint userSN = reader.ReadUInt32();

        var profile = await service.GetUserProfileAsync(userSN, ct);
        if (profile is null)
        {
            logger.LogWarning("[INFO] User {UserSN} not found", userSN);
            return;
        }

        logger.LogInformation("[INFO] User {UserSN} | Exp: {Exp} | Money: {Money} | Cash: {Cash} | Level: {Level}",
            userSN, profile.Exp, profile.Money, profile.Cash, profile.Level);
    }

    private async Task HandleQuestAttemptAsync(BinaryReader reader, CancellationToken ct)
    {
        uint userSN = reader.ReadUInt32();
        int questSN = reader.ReadInt32();
        int score = reader.ReadInt32();
        int perfect = reader.ReadInt32();
        int mode = reader.ReadInt32();

        var command = new QuestAttemptCommand(userSN, questSN, score, perfect, mode);
        var result = await service.ValidateAndCompleteQuestAsync(command, ct);

        if (result.Success)
        {
            logger.LogInformation("[QUEST] User {UserSN} completed quest {QuestSN} -> Exp: {Exp}, Money: {Money}, Level: {Level}",
                userSN, questSN, result.NewExp, result.NewMoney, result.NewLevel);
        }
        else
        {
            logger.LogWarning("[QUEST] User {UserSN} quest {QuestSN} failed: {Reason}", userSN, questSN, result.ErrorReason);
        }
    }

    private async Task HandleGameResultsAsync(BinaryReader reader, CancellationToken ct)
    {
        uint userSN = reader.ReadUInt32();
        int expGain = reader.ReadInt32();
        int moneyGain = reader.ReadInt32();

        var command = new GameResultCommand(userSN, expGain, moneyGain);
        await service.UpdateGameResultsAsync(command, ct);

        logger.LogInformation("[GAME] User {UserSN} gained {Exp} EXP and {Money} Money", userSN, expGain, moneyGain);
    }

    private async Task HandleItemCheckAsync(BinaryReader reader, CancellationToken ct)
    {
        uint userSN = reader.ReadUInt32();
        int itemId = reader.ReadInt32();

        bool hasActive = await service.HasActiveItemAsync(userSN, itemId, ct);
        bool hasUnlimited = await service.HasUnlimitedItemAsync(userSN, itemId, ct);
        bool hasPending = await service.HasPendingPresentAsync(userSN, itemId, ct);

        logger.LogInformation("[ITEM] User {UserSN} Item {ItemId}: Active={Active}, Unlimited={Unlimited}, PendingGift={Pending}",
            userSN, itemId, hasActive, hasUnlimited, hasPending);
    }

    private async Task HandleLevelQuestLogAsync(BinaryReader reader, CancellationToken ct)
    {
        uint userSN = reader.ReadUInt32();
        short procLevel = reader.ReadInt16();
        int beforeMoney = reader.ReadInt32();
        int afterMoney = reader.ReadInt32();
        int beforeExp = reader.ReadInt32();
        int afterExp = reader.ReadInt32();
        byte pass = reader.ReadByte();

        var command = new LevelQuestLogCommand(userSN, procLevel, beforeMoney, afterMoney, beforeExp, afterExp, pass);
        await service.LogLevelQuestAsync(command, ct);

        logger.LogInformation("[QUEST] Log saved for User {UserSN} (Pass: {Pass})", userSN, pass);
    }

    private async Task HandleCashQueryAsync(BinaryReader reader, CancellationToken ct)
    {
        uint userSN = reader.ReadUInt32();

        var result = await service.GetCashAndGenderAsync(userSN, ct);
        if (result is null)
        {
            logger.LogWarning("[CASH] User {UserSN} not found", userSN);
            return;
        }

        logger.LogInformation("[CASH] User {UserSN} Cash: {Cash}, Gender: {Gender}", userSN, result.Value.Cash, result.Value.Gender);
    }

    private async Task HandleDayUniqueCountAsync(CancellationToken ct)
    {
        await service.SaveDayUniqueCountAsync(ct);
        logger.LogInformation("[STATS] Daily unique user count saved");
    }
}
