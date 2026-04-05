namespace Audition.DBAgent.Game.Host.Network;

public static class PacketFraming
{
    private const int MinimumFramedPacketLength = 4;
    private const int MaximumPacketLength = 8192;

    public static bool UsesLengthPrefix(ReadOnlySpan<byte> chunk)
    {
        if (chunk.Length < 2)
        {
            return false;
        }

        byte firstByte = chunk[0];
        int minimumRawPacketLength = GetMinimumRawPacketLength(firstByte);
        if (minimumRawPacketLength != int.MaxValue && chunk.Length >= minimumRawPacketLength)
        {
            return false;
        }

        ushort packetLength = (ushort)(chunk[0] | (chunk[1] << 8));
        return packetLength >= MinimumFramedPacketLength && packetLength <= MaximumPacketLength;
    }

    public static bool TryExtractPayload(List<byte> buffer, out byte[] payload)
    {
        payload = [];

        if (buffer.Count < 2)
        {
            return false;
        }

        ushort packetLength = (ushort)(buffer[0] | (buffer[1] << 8));
        if (packetLength < MinimumFramedPacketLength || packetLength > MaximumPacketLength)
        {
            throw new InvalidDataException($"Invalid length-prefixed packet size: {packetLength}");
        }

        if (buffer.Count < packetLength)
        {
            return false;
        }

        int payloadLength = packetLength - 2;
        payload = new byte[payloadLength];
        buffer.CopyTo(2, payload, 0, payloadLength);
        buffer.RemoveRange(0, packetLength);
        return true;
    }

    private static int GetMinimumRawPacketLength(byte opCode) => opCode switch
    {
        0 => 13,
        1 => 5,
        2 => 21,
        3 => 13,
        4 => 1,
        5 => 9,
        6 => 23,
        7 => 5,
        8 => 1,
        0x4B => 4,
        0x5D => 2,
        (byte)'[' => 2,
        _ => int.MaxValue,
    };
}