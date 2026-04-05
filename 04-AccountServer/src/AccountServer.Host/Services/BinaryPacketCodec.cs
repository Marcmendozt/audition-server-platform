using System.Buffers.Binary;
using System.Text;
using System.Text.Json;
using AccountServer.Application.Contracts;
using AccountServer.Domain.Models;
using AccountServer.Host.Contracts;

namespace AccountServer.Host.Services;

public sealed class BinaryPacketCodec(PacketBufferManager packetBufferManager)
{
    private const int HeaderSize = 2;
    private const int MaxPacketLength = 0x1000;
    private const int MinPacketLength = 3;
    private const int ServerInfoSize = 0x70;
    private static readonly Encoding StringEncoding = Encoding.ASCII;

    public bool LooksLikeBinary(byte firstByte)
    {
        return firstByte != (byte)'{';
    }

    public async Task<BinaryPacketRequest?> ReadRequestAsync(Stream stream, CancellationToken cancellationToken)
    {
        var headerBuffer = new byte[HeaderSize];
        var hasHeader = await ReadExactOrEofAsync(stream, headerBuffer, cancellationToken);
        if (!hasHeader)
        {
            return null;
        }

        var packetLength = BinaryPrimitives.ReadUInt16LittleEndian(headerBuffer);
        if (packetLength < MinPacketLength || packetLength > MaxPacketLength)
        {
            throw new JsonException($"Packet length invalido: {packetLength}");
        }

        var bodyLength = packetLength - HeaderSize;
        var payloadBuffer = packetBufferManager.Rent(bodyLength);

        try
        {
            await ReadExactAsync(stream, payloadBuffer.AsMemory(0, bodyLength), cancellationToken);
            var exactBody = new byte[bodyLength];
            Array.Copy(payloadBuffer, exactBody, bodyLength);
            packetBufferManager.RegisterProcessedPacket();
            return new BinaryPacketRequest(packetLength, exactBody);
        }
        finally
        {
            packetBufferManager.Return(payloadBuffer);
        }
    }

    public bool TryParseGatewayRegistration(BinaryPacketRequest request, out RegisterGatewayServerCommand command)
    {
        command = default!;
        if (!TryParseServerInfoPacket(request, out var serverInfo))
        {
            return false;
        }

        command = new RegisterGatewayServerCommand(
            serverInfo.ServerId,
            serverInfo.ClusterId,
            serverInfo.Name,
            serverInfo.Level,
            serverInfo.IpAddress,
            serverInfo.Port,
            serverInfo.MaxUserCount,
            serverInfo.Version,
            serverInfo.Status);

        return true;
    }

    public bool TryParseGameRegistration(BinaryPacketRequest request, out RegisterGameServerCommand command)
    {
        command = default!;
        if (!TryParseServerInfoPacket(request, out var serverInfo))
        {
            return false;
        }

        command = new RegisterGameServerCommand(
            serverInfo.ServerId,
            serverInfo.Name,
            serverInfo.IpAddress,
            serverInfo.Port,
            serverInfo.Level,
            0,
            checked((ushort)Math.Min(serverInfo.MaxUserCount, ushort.MaxValue)),
            checked((ushort)Math.Min(serverInfo.MaxUserCount, ushort.MaxValue)),
            serverInfo.Status);

        return true;
    }

    private static bool TryParseServerInfoPacket(BinaryPacketRequest request, out LegacyServerInfo serverInfo)
    {
        serverInfo = default;
        if (request.Opcode != 0x65 || request.SubOpcode != 0x00)
        {
            return false;
        }

        if (request.Buffer.Length < 2 + ServerInfoSize)
        {
            return false;
        }

        var payload = request.Buffer.AsSpan(2, ServerInfoSize);
        serverInfo = new LegacyServerInfo(
            BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(0, 2)),
            BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(4, 4)),
            ReadFixedString(payload.Slice(8, 48)),
            payload[56],
            BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(60, 4)),
            BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(64, 4)),
            ReadFixedString(payload.Slice(68, 32)),
            BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(100, 2)),
            BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(104, 4)),
            ParseStatus(payload[108]));

        return true;
    }

    private static ServerStatus ParseStatus(byte rawStatus)
    {
        return Enum.IsDefined(typeof(ServerStatus), (int)rawStatus)
            ? (ServerStatus)rawStatus
            : ServerStatus.Online;
    }

    private static string ReadFixedString(ReadOnlySpan<byte> buffer)
    {
        var terminatorIndex = buffer.IndexOf((byte)0);
        var effectiveLength = terminatorIndex >= 0 ? terminatorIndex : buffer.Length;
        return StringEncoding.GetString(buffer.Slice(0, effectiveLength)).Trim();
    }

    private static async Task<bool> ReadExactOrEofAsync(Stream stream, Memory<byte> buffer, CancellationToken cancellationToken)
    {
        var readTotal = 0;
        while (readTotal < buffer.Length)
        {
            var read = await stream.ReadAsync(buffer[readTotal..], cancellationToken);
            if (read == 0)
            {
                if (readTotal == 0)
                {
                    return false;
                }

                throw new EndOfStreamException("El stream termino antes de completar la cabecera del paquete.");
            }

            readTotal += read;
        }

        return true;
    }

    private static async Task ReadExactAsync(Stream stream, Memory<byte> buffer, CancellationToken cancellationToken)
    {
        var readTotal = 0;
        while (readTotal < buffer.Length)
        {
            var read = await stream.ReadAsync(buffer[readTotal..], cancellationToken);
            if (read == 0)
            {
                throw new EndOfStreamException("El stream termino antes de completar el paquete.");
            }

            readTotal += read;
        }
    }

    private readonly record struct LegacyServerInfo(
        ushort ServerId,
        uint ClusterId,
        string Name,
        byte Level,
        uint CurrentUserCount,
        uint MaxUserCount,
        string IpAddress,
        ushort Port,
        uint Version,
        ServerStatus Status);
}