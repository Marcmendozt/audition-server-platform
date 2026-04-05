using System.Text;
using LoginDBAgent.Application.Contracts;
using LoginDBAgent.Application.Services;
using Microsoft.Extensions.Logging;

namespace LoginDBAgent.Host.Network;

public sealed class PacketDispatcher(
    ILoginDbAgentService service,
    ILogger<PacketDispatcher> logger)
{
    public async Task<byte[]?> DispatchAsync(byte[] data, CancellationToken ct)
    {
        if (data.Length < 4) return null;

        short size = BitConverter.ToInt16(data, 0);
        byte opCode = data[2];
        byte version = data[3];
        short packetHeader = (short)(opCode | (version << 8));

        logger.LogDebug("OpCode: {OpCode} | Version: {Version} | Size: {Size} bytes", opCode, version, size);

        try
        {
            return opCode switch
            {
                95 => await HandleLogoutNotificationAsync(data, version, ct),
                96 => await HandleLoginRequestAsync(data, packetHeader, ct),
                97 => await HandleUserProfileRequestAsync(data, ct),
                98 => await HandleRankRequestAsync(data, ct),
                99 => await HandleFriendListRequestAsync(data, ct),
                100 => await HandleAvatarInfoRequestAsync(data, ct),
                101 => await HandleItemCheckRequestAsync(data, ct),
                102 => await HandleQuestRequestAsync(data, ct),
                103 => await HandleGameResultRequestAsync(data, ct),
                104 => await HandleEquipRequestAsync(data, ct),
                105 => await HandleCashQueryAsync(data, ct),
                110 => await HandleDayUniqueCountAsync(ct),
                _ => HandleUnknownOpCode(opCode, version),
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing OpCode {OpCode}.{Version}", opCode, version);
            return null;
        }
    }

    private async Task<byte[]?> HandleLoginRequestAsync(byte[] data, short opCode, CancellationToken ct)
    {
        // Extract 12-byte relay context (offset 4..16) — must be echoed back exactly.
        // The AccountServer uses these 12 bytes as pointers (pClient, hInstance, GatewayServer*).
        byte[] relayContext = new byte[12];
        if (data.Length >= 16)
            Buffer.BlockCopy(data, 4, relayContext, 0, 12);

        int currentPos = 16;
        if (currentPos >= data.Length) return BuildLoginResponse(0, 0, opCode, relayContext, "", "");

        byte userLen = data[currentPos++];
        if (currentPos + userLen > data.Length) return BuildLoginResponse(0, 0, opCode, relayContext, "", "");
        string userId = Encoding.ASCII.GetString(data, currentPos, userLen);
        currentPos += userLen;

        if (currentPos >= data.Length) return BuildLoginResponse(0, 0, opCode, relayContext, "", "");
        byte passLen = data[currentPos++];
        if (currentPos + passLen > data.Length) return BuildLoginResponse(0, 0, opCode, relayContext, "", "");
        string password = Encoding.ASCII.GetString(data, currentPos, passLen);

        logger.LogInformation("[LOGIN] Request: {UserId}", userId);

        var result = await service.ValidateLoginAsync(new LoginCommand(userId, password), ct);

        if (result.Success)
        {
            logger.LogInformation("[LOGIN] Success: {Nick} (SN: {UserSN}) | Cash: {Cash}",
                result.UserNick, result.UserSN, result.Cash);

            await service.RecordLoginAsync(result.UserSN, result.UserNick, ct);

            return BuildLoginResponse(1, (int)result.UserSN, opCode, relayContext, userId, result.UserNick);
        }

        logger.LogWarning("[LOGIN] Failed for {UserId}: {Reason}", userId, result.ErrorReason);
        return BuildLoginResponse(0, 0, opCode, relayContext, "", "");
    }

    private async Task<byte[]?> HandleLogoutNotificationAsync(byte[] data, byte version, CancellationToken ct)
    {
        if (data.Length < 4)
        {
            logger.LogWarning("[LOGOUT] Invalid packet length: {Length}", data.Length);
            return null;
        }

        if (version == 0)
        {
            int currentPos = 4;
            if (currentPos + 4 > data.Length)
            {
                logger.LogWarning("[LOGOUT] Missing UserSN payload");
                return null;
            }

            uint userSN = BitConverter.ToUInt32(data, currentPos);
            currentPos += 4;

            if (currentPos + 2 > data.Length)
            {
                logger.LogWarning("[LOGOUT] Missing reserved field for UserSN {UserSN}", userSN);
                return null;
            }

            ushort reserved = BitConverter.ToUInt16(data, currentPos);
            currentPos += 2;

            if (currentPos >= data.Length)
            {
                logger.LogWarning("[LOGOUT] Missing state flag for UserSN {UserSN}", userSN);
                return null;
            }

            byte state = data[currentPos++];

            if (currentPos >= data.Length)
            {
                logger.LogWarning("[LOGOUT] Missing source IP string for UserSN {UserSN}", userSN);
                return null;
            }

            byte sourceIpLength = data[currentPos++];
            if (currentPos + sourceIpLength > data.Length)
            {
                logger.LogWarning("[LOGOUT] Invalid source IP length {Length} for UserSN {UserSN}", sourceIpLength, userSN);
                return null;
            }

            string sourceIp = Encoding.ASCII.GetString(data, currentPos, sourceIpLength);
            await service.RecordLogoutAsync(userSN, sourceIp, ct);

            logger.LogInformation(
                "[SESSION RELEASE] UserSN {UserSN} | SourceIP: {SourceIP} | Reserved: {Reserved} | State: {State}",
                userSN,
                sourceIp,
                reserved,
                state);

            return null;
        }

        if (version == 1)
        {
            int currentPos = 4;
            if (currentPos >= data.Length)
            {
                logger.LogWarning("[LOGOUT] Missing message payload for version 1");
                return null;
            }

            byte messageLength = data[currentPos++];
            if (currentPos + messageLength > data.Length)
            {
                logger.LogWarning("[LOGOUT] Invalid message length {Length} for version 1", messageLength);
                return null;
            }

            string message = Encoding.ASCII.GetString(data, currentPos, messageLength);
            logger.LogInformation("[LOGOUT] Version 1 message: {Message}", message);
            return null;
        }

        logger.LogWarning("[LOGOUT] Unsupported version {Version}", version);
        return null;
    }

    private async Task<byte[]?> HandleUserProfileRequestAsync(byte[] data, CancellationToken ct)
    {
        if (data.Length < 8) return null;
        uint userSN = BitConverter.ToUInt32(data, 4);

        var profile = await service.GetUserProfileAsync(userSN, ct);
        if (profile is null)
        {
            logger.LogWarning("[PROFILE] User {UserSN} not found", userSN);
            return null;
        }

        logger.LogInformation("[PROFILE] User {UserSN} | Exp: {Exp} | Money: {Money} | Cash: {Cash} | Level: {Level}",
            userSN, profile.Exp, profile.Money, profile.Cash, profile.Level);
        return null;
    }

    private async Task<byte[]?> HandleRankRequestAsync(byte[] data, CancellationToken ct)
    {
        if (data.Length < 12) return null;
        int offset = BitConverter.ToInt32(data, 4);
        int limit = BitConverter.ToInt32(data, 8);

        var rankings = await service.GetTopRankingsAsync(offset, limit, ct);
        logger.LogInformation("[RANK] Retrieved {Count} rankings (offset: {Offset})", rankings.Count, offset);
        return null;
    }

    private async Task<byte[]?> HandleFriendListRequestAsync(byte[] data, CancellationToken ct)
    {
        if (data.Length < 5) return null;
        byte nickLen = data[4];
        if (data.Length < 5 + nickLen) return null;
        string userNick = Encoding.ASCII.GetString(data, 5, nickLen);

        var friends = await service.GetFriendListAsync(userNick, ct);
        logger.LogInformation("[FRIENDS] {UserNick} has {Count} friends", userNick, friends.Count);
        return null;
    }

    private async Task<byte[]?> HandleAvatarInfoRequestAsync(byte[] data, CancellationToken ct)
    {
        if (data.Length < 8) return null;
        uint userSN = BitConverter.ToUInt32(data, 4);

        var equipped = await service.GetEquippedItemsAsync(userSN, ct);
        var inventory = await service.GetInventoryItemsAsync(userSN, ct);
        logger.LogInformation("[AVATAR] User {UserSN}: {Equipped} equipped, {Inventory} in inventory",
            userSN, equipped.Count, inventory.Count);
        return null;
    }

    private async Task<byte[]?> HandleItemCheckRequestAsync(byte[] data, CancellationToken ct)
    {
        if (data.Length < 12) return null;
        uint userSN = BitConverter.ToUInt32(data, 4);
        int itemId = BitConverter.ToInt32(data, 8);

        bool active = await service.HasActiveItemAsync(userSN, itemId, ct);
        bool unlimited = await service.HasUnlimitedItemAsync(userSN, itemId, ct);
        bool pending = await service.HasPendingPresentAsync(userSN, itemId, ct);

        logger.LogInformation("[ITEM] User {UserSN} Item {ItemId}: Active={Active}, Unlimited={Unlimited}, Pending={Pending}",
            userSN, itemId, active, unlimited, pending);
        return null;
    }

    private async Task<byte[]?> HandleQuestRequestAsync(byte[] data, CancellationToken ct)
    {
        if (data.Length < 24) return null;
        uint userSN = BitConverter.ToUInt32(data, 4);
        int questSN = BitConverter.ToInt32(data, 8);
        int score = BitConverter.ToInt32(data, 12);
        int perfect = BitConverter.ToInt32(data, 16);
        int mode = BitConverter.ToInt32(data, 20);

        var result = await service.ValidateAndCompleteQuestAsync(
            new QuestAttemptCommand(userSN, questSN, score, perfect, mode), ct);

        if (result.Success)
            logger.LogInformation("[QUEST] User {UserSN} completed quest {QuestSN}", userSN, questSN);
        else
            logger.LogWarning("[QUEST] User {UserSN} quest {QuestSN} failed: {Reason}", userSN, questSN, result.ErrorReason);
        return null;
    }

    private async Task<byte[]?> HandleGameResultRequestAsync(byte[] data, CancellationToken ct)
    {
        if (data.Length < 16) return null;
        uint userSN = BitConverter.ToUInt32(data, 4);
        int expGain = BitConverter.ToInt32(data, 8);
        int moneyGain = BitConverter.ToInt32(data, 12);

        await service.UpdateGameResultsAsync(new GameResultCommand(userSN, expGain, moneyGain), ct);
        logger.LogInformation("[GAME] User {UserSN} gained {Exp} EXP, {Money} Money", userSN, expGain, moneyGain);
        return null;
    }

    private async Task<byte[]?> HandleEquipRequestAsync(byte[] data, CancellationToken ct)
    {
        if (data.Length < 13) return null;
        uint userSN = BitConverter.ToUInt32(data, 4);
        int itemId = BitConverter.ToInt32(data, 8);
        byte stateFlag = data[12];
        string state = stateFlag == 1 ? "EQUIP" : "INV";

        await service.EquipItemAsync(new AvatarEquipCommand(userSN, itemId, state), ct);
        logger.LogInformation("[EQUIP] User {UserSN} Item {ItemId} -> {State}", userSN, itemId, state);
        return null;
    }

    private async Task<byte[]?> HandleCashQueryAsync(byte[] data, CancellationToken ct)
    {
        if (data.Length < 8) return null;
        uint userSN = BitConverter.ToUInt32(data, 4);

        var result = await service.GetCashAndGenderAsync(userSN, ct);
        if (result is null)
        {
            logger.LogWarning("[CASH] User {UserSN} not found", userSN);
            return null;
        }

        logger.LogInformation("[CASH] User {UserSN} Cash: {Cash}, Gender: {Gender}",
            userSN, result.Value.Cash, result.Value.Gender);
        return null;
    }

    private async Task<byte[]?> HandleDayUniqueCountAsync(CancellationToken ct)
    {
        await service.SaveDayUniqueCountAsync(ct);
        logger.LogInformation("[STATS] Daily unique user count saved");
        return null;
    }

    private byte[]? HandleUnknownOpCode(byte opCode, byte version)
    {
        logger.LogWarning("Unknown OpCode {OpCode}.{Version}", opCode, version);
        return null;
    }

    /// <summary>
    /// Builds login response matching the native AccountServer's packetProcedure96 + RequestLoginChina format.
    ///
    /// The native Packet class uses [u16 LE total_length][body...] framing.
    /// The dispatcher reads body[0] as opcode (0x60=96), packetProcedure96 reads body[1] as version.
    /// Since we write opCode as short LE, bytes are [0x60, 0x00] = opcode + China version in one write.
    ///
    /// Native RequestLoginChina parsing sequence (body[2+]):
    ///   1. getString(12)      → 12-byte relay context
    ///   2. operator>>(byte)   → result byte: 0x00 = success, non-zero = failure
    ///   3. operator>>(String) → userId  (1-byte length prefix + ASCII data)
    ///   [if result == 0x00 (success):]
    ///   4. operator>>(String) → userNick (1-byte length prefix + ASCII data)
    ///   5. operator>>(ulong)  → userExp  (uint32 LE)
    ///   6. operator>>(ulong)  → userSN   (uint32 LE)
    ///   7. operator>>(byte)   → flag1 (login flag, forwarded to Gateway)
    ///   8. operator>>(byte)   → flag2
    /// </summary>
    private static byte[] BuildLoginResponse(byte result, int userSN, short opCode, byte[] relayContext, string userId, string nick)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        // Placeholder for packet length (2 bytes) — filled at end
        writer.Write((short)0);
        // OpCode as short LE: low byte = 0x60 (opcode), high byte = 0x00 (China version)
        writer.Write(opCode);
        // 12-byte relay context — echoed back from request (contains GatewayServer pointers)
        writer.Write(relayContext);

        if (result == 1)
        {
            // Result byte: 0x00 = success
            writer.Write((byte)0x00);
            // UserId (1-byte length prefix + ASCII)
            WritePrefixedString(writer, userId);
            // UserNick (1-byte length prefix + ASCII)
            WritePrefixedString(writer, nick);
            // UserExp (uint32 LE)
            writer.Write((uint)0);
            // UserSN (uint32 LE)
            writer.Write((uint)userSN);
            // Flag1: login flag (0x01 = success, forwarded to Gateway)
            writer.Write((byte)0x01);
            // Flag2: reserved
            writer.Write((byte)0x00);
        }
        else
        {
            // Result byte: non-zero = failure
            writer.Write((byte)0x01);
            // UserId (empty)
            WritePrefixedString(writer, string.Empty);
        }

        // Patch packet length at offset 0 (total size including the 2-byte length field)
        var data = ms.ToArray();
        var length = (short)data.Length;
        data[0] = (byte)(length & 0xFF);
        data[1] = (byte)((length >> 8) & 0xFF);

        return data;
    }

    private static void WritePrefixedString(BinaryWriter writer, string value)
    {
        var bytes = Encoding.ASCII.GetBytes(value ?? string.Empty);
        writer.Write((byte)Math.Min(bytes.Length, 255));
        writer.Write(bytes);
    }
}
