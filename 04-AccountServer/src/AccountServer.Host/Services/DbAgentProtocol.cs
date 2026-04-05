using System.Buffers.Binary;
using System.Text;

namespace AccountServer.Host.Services;

public static class DbAgentProtocol
{
    private static readonly Encoding StringEncoding = Encoding.ASCII;

    public static byte[] BuildRequestLoginChina(
        ReadOnlySpan<byte> relayContext,
        string field1,
        string field2,
        string field3,
        string field4)
    {
        if (relayContext.Length != 12)
        {
            throw new ArgumentException("El relay context heredado debe medir 12 bytes.", nameof(relayContext));
        }

        var field1Bytes = GetLengthPrefixedAscii(field1);
        var field2Bytes = GetLengthPrefixedAscii(field2);
        var field3Bytes = GetLengthPrefixedAscii(field3);
        var field4Bytes = GetLengthPrefixedAscii(field4);

        var packetLength = checked((ushort)(4 + relayContext.Length + field1Bytes.Length + field2Bytes.Length + field3Bytes.Length + field4Bytes.Length));
        var packet = new byte[packetLength];
        var offset = 0;

        BinaryPrimitives.WriteUInt16LittleEndian(packet.AsSpan(offset, 2), packetLength);
        offset += 2;
        BinaryPrimitives.WriteUInt16LittleEndian(packet.AsSpan(offset, 2), 0x60);
        offset += 2;

        relayContext.CopyTo(packet.AsSpan(offset, relayContext.Length));
        offset += relayContext.Length;

        field1Bytes.CopyTo(packet.AsSpan(offset, field1Bytes.Length));
        offset += field1Bytes.Length;
        field2Bytes.CopyTo(packet.AsSpan(offset, field2Bytes.Length));
        offset += field2Bytes.Length;
        field3Bytes.CopyTo(packet.AsSpan(offset, field3Bytes.Length));
        offset += field3Bytes.Length;
        field4Bytes.CopyTo(packet.AsSpan(offset, field4Bytes.Length));

        return packet;
    }

    public static byte[] BuildSaveUserLogout(uint userSerial, bool isBlocked, string userId)
    {
        return new LegacyPacketBuilder((byte)'_')
            .AddByte(0)
            .AddDword(userSerial)
            .AddWord(0)
            .AddByte(isBlocked ? (byte)1 : (byte)0)
            .AddLengthPrefixedAscii(userId)
            .Build();
    }

    public static byte[] BuildUsedBoardItem(uint sourceSerial, uint targetSerial)
    {
        return new LegacyPacketBuilder((byte)'[')
            .AddByte((byte)' ')
            .AddDword(sourceSerial)
            .AddDword(targetSerial)
            .Build();
    }

    private static byte[] GetLengthPrefixedAscii(string value)
    {
        var encoded = StringEncoding.GetBytes(value ?? string.Empty);
        if (encoded.Length > byte.MaxValue)
        {
            throw new InvalidOperationException("La cadena excede el maximo de 255 bytes del protocolo heredado.");
        }

        var result = new byte[encoded.Length + 1];
        result[0] = (byte)encoded.Length;
        encoded.CopyTo(result.AsSpan(1));
        return result;
    }
}