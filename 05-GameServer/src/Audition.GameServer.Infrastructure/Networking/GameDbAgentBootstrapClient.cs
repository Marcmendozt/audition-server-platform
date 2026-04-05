using System.Buffers.Binary;
using System.Net.Sockets;
using Audition.GameServer.Application.Abstractions;
using Audition.GameServer.Application.Contracts;
using Audition.GameServer.Domain.Models;

namespace Audition.GameServer.Infrastructure.Networking;

public sealed class GameDbAgentBootstrapClient : IGameDbAgentBootstrapClient
{
    public async Task<GameDbBootstrapResult> SynchronizeAsync(BootstrapConfiguration configuration, CancellationToken ct)
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(configuration.DbAgent.Host, configuration.DbAgent.Port, ct);
            await using NetworkStream stream = client.GetStream();

            IReadOnlyList<LevelQuestInfo> quests = await RequestLevelQuestsAsync(stream, ct);
            LegacyServerInfo serverInfo = await RequestServerInfoAsync(stream, configuration.ServerId, ct);
            IReadOnlyList<LegacyChannelInfo> channels = await RequestChannelsAsync(stream, configuration.ServerId, ct);

            return new GameDbBootstrapResult(
                true,
                $"Synchronized {quests.Count} level quests and {channels.Count} channels from DBAgent",
                serverInfo,
                channels,
                quests);
        }
        catch (Exception ex)
        {
            return new GameDbBootstrapResult(false, ex.Message, null, [], []);
        }
    }

    private static async Task<IReadOnlyList<LevelQuestInfo>> RequestLevelQuestsAsync(NetworkStream stream, CancellationToken ct)
    {
        await SendRawRequestAsync(stream, new byte[] { 0x5D, 0x00 }, ct);

        var quests = new List<LevelQuestInfo>();
        while (true)
        {
            byte[] payload = await ReadPayloadAsync(stream, ct);
            if (payload.Length < 3 || payload[0] != 0x5D || payload[1] != 0x00)
            {
                throw new InvalidDataException("Unexpected DBAgent payload while waiting for level quest sync");
            }

            if (payload[2] == 0x01)
            {
                return quests;
            }

            if (payload[2] != 0x00 || payload.Length < 35)
            {
                throw new InvalidDataException("Invalid level quest sync row received from DBAgent");
            }

            ReadOnlySpan<byte> row = payload.AsSpan(3, 32);
            quests.Add(new LevelQuestInfo(
                row[0],
                BinaryPrimitives.ReadInt32LittleEndian(row.Slice(4, 4)),
                BinaryPrimitives.ReadInt32LittleEndian(row.Slice(8, 4)),
                row[12],
                row[13],
                row[14],
                BinaryPrimitives.ReadUInt16LittleEndian(row.Slice(16, 2)),
                row[18],
                BinaryPrimitives.ReadInt32LittleEndian(row.Slice(20, 4)),
                BinaryPrimitives.ReadInt32LittleEndian(row.Slice(24, 4)),
                BinaryPrimitives.ReadInt32LittleEndian(row.Slice(28, 4))));
        }
    }

    private static async Task<LegacyServerInfo> RequestServerInfoAsync(NetworkStream stream, ushort serverId, CancellationToken ct)
    {
        byte[] request = new byte[4];
        request[0] = 0x4B;
        request[1] = 0x00;
        BinaryPrimitives.WriteUInt16LittleEndian(request.AsSpan(2, 2), serverId);
        await SendRawRequestAsync(stream, request, ct);

        byte[] payload = await ReadPayloadAsync(stream, ct);
        return ParseServerInfo(payload);
    }

    private static async Task<IReadOnlyList<LegacyChannelInfo>> RequestChannelsAsync(NetworkStream stream, ushort serverId, CancellationToken ct)
    {
        byte[] request = new byte[4];
        request[0] = 0x4B;
        request[1] = 0x01;
        BinaryPrimitives.WriteUInt16LittleEndian(request.AsSpan(2, 2), serverId);
        await SendRawRequestAsync(stream, request, ct);

        byte[] payload = await ReadPayloadAsync(stream, ct);
        return ParseChannelInfo(payload);
    }

    public static LegacyServerInfo ParseServerInfo(ReadOnlySpan<byte> payload)
    {
        if (payload.Length < 12 || payload[0] != 0x4B || payload[1] != 0x00)
        {
            throw new InvalidDataException("Invalid legacy server info payload");
        }

        int offset = 2;
        int nameLength = payload[offset++];
        EnsureReadable(payload, offset, nameLength + 11);
        string name = ReadAscii(payload.Slice(offset, nameLength));
        offset += nameLength;

        ushort port = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(offset, 2));
        offset += 2;
        ushort currentUsers = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(offset, 2));
        offset += 2;
        ushort maxUsers = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(offset, 2));
        offset += 2;
        ushort grade = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(offset, 2));
        offset += 2;
        ushort ipRestriction = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(offset, 2));
        offset += 2;
        ushort doubleDen = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(offset, 2));
        offset += 2;
        int ipLength = payload[offset++];
        EnsureReadable(payload, offset, ipLength);
        string ipAddress = ReadAscii(payload.Slice(offset, ipLength));

        return new LegacyServerInfo(0, name, ipAddress, port, currentUsers, maxUsers, grade, ipRestriction, doubleDen);
    }

    public static IReadOnlyList<LegacyChannelInfo> ParseChannelInfo(ReadOnlySpan<byte> payload)
    {
        if (payload.Length < 4 || payload[0] != 0x4B || payload[1] != 0x01)
        {
            throw new InvalidDataException("Invalid legacy channel info payload");
        }

        int offset = 2;
        ushort channelCount = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(offset, 2));
        offset += 2;
        var channels = new List<LegacyChannelInfo>(channelCount);

        for (int index = 0; index < channelCount; index++)
        {
            EnsureReadable(payload, offset, 1);
            int nameLength = payload[offset++];
            EnsureReadable(payload, offset, nameLength + 11);
            string name = ReadAscii(payload.Slice(offset, nameLength));
            offset += nameLength;

            ushort number = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(offset, 2));
            offset += 2;
            ushort maxUsers = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(offset, 2));
            offset += 2;
            ushort maxRooms = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(offset, 2));
            offset += 2;
            ushort minLevel = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(offset, 2));
            offset += 2;
            ushort maxLevel = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(offset, 2));
            offset += 2;
            byte eventNumber = payload[offset++];

            channels.Add(new LegacyChannelInfo(number, name, maxUsers, maxRooms, minLevel, maxLevel, eventNumber));
        }

        return channels;
    }

    private static async Task SendRawRequestAsync(NetworkStream stream, ReadOnlyMemory<byte> payload, CancellationToken ct)
    {
        await stream.WriteAsync(payload, ct);
        await stream.FlushAsync(ct);
    }

    private static async Task<byte[]> ReadPayloadAsync(NetworkStream stream, CancellationToken ct)
    {
        byte[] header = new byte[2];
        await ReadExactAsync(stream, header, ct);
        ushort packetLength = BinaryPrimitives.ReadUInt16LittleEndian(header);
        if (packetLength < 2)
        {
            throw new InvalidDataException($"Invalid DBAgent packet length {packetLength}");
        }

        int payloadLength = packetLength - 2;
        byte[] payload = new byte[payloadLength];
        await ReadExactAsync(stream, payload, ct);
        return payload;
    }

    private static async Task ReadExactAsync(NetworkStream stream, byte[] buffer, CancellationToken ct)
    {
        int readTotal = 0;
        while (readTotal < buffer.Length)
        {
            int read = await stream.ReadAsync(buffer.AsMemory(readTotal, buffer.Length - readTotal), ct);
            if (read == 0)
            {
                throw new EndOfStreamException("DBAgent closed the connection while reading a packet");
            }

            readTotal += read;
        }
    }

    private static void EnsureReadable(ReadOnlySpan<byte> payload, int offset, int count)
    {
        if (offset < 0 || count < 0 || offset + count > payload.Length)
        {
            throw new InvalidDataException("Unexpected end of DBAgent payload");
        }
    }

    private static string ReadAscii(ReadOnlySpan<byte> payload)
    {
        return System.Text.Encoding.ASCII.GetString(payload);
    }
}