using System.Buffers.Binary;
using System.Text;

namespace AccountServer.Host.Services;

public sealed class LegacyPacketBuilder(byte opcode)
{
    private readonly List<byte> body = new() { opcode };

    public LegacyPacketBuilder AddByte(byte value)
    {
        body.Add(value);
        return this;
    }

    public LegacyPacketBuilder AddBytes(ReadOnlySpan<byte> value)
    {
        foreach (var item in value)
        {
            body.Add(item);
        }

        return this;
    }

    public LegacyPacketBuilder AddDword(uint value)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
        return AddBytes(buffer);
    }

    public LegacyPacketBuilder AddLengthPrefixedAscii(string value)
    {
        var encoded = Encoding.ASCII.GetBytes(value ?? string.Empty);
        if (encoded.Length > byte.MaxValue)
        {
            throw new InvalidOperationException("La cadena excede el maximo de 255 bytes del protocolo heredado.");
        }

        body.Add((byte)encoded.Length);
        return AddBytes(encoded);
    }

    public LegacyPacketBuilder AddWord(ushort value)
    {
        Span<byte> buffer = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
        return AddBytes(buffer);
    }

    public byte[] Build()
    {
        var totalLength = checked((ushort)(2 + body.Count));
        var packet = new byte[totalLength];
        BinaryPrimitives.WriteUInt16LittleEndian(packet.AsSpan(0, 2), totalLength);
        body.CopyTo(packet, 2);
        return packet;
    }
}