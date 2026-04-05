using GatewayAudition.Domain.ValueObjects;

using System.Buffers.Binary;

namespace GatewayAudition.Domain.Entities;

public class Packet
{
    private readonly PacketInfo _packetInfo = new();
    private const int DefaultBufferSize = 4096;
    private const byte MinNativeKeyLength = 2;
    private const byte MaxNativeKeyLength = 9;
    private const uint NativeHeaderSize = 2;

    public PacketInfo Info => _packetInfo;

    public void Initialize()
    {
        _packetInfo.Buffer = new byte[DefaultBufferSize];
        _packetInfo.TotalBufferSize = DefaultBufferSize;
        _packetInfo.DataPosition = NativeHeaderSize;
        _packetInfo.PacketSize = NativeHeaderSize;
        _packetInfo.IsEncrypted = false;
    }

    public void Initialize(uint bufferSize)
    {
        _packetInfo.Buffer = new byte[bufferSize];
        _packetInfo.TotalBufferSize = bufferSize;
        _packetInfo.DataPosition = NativeHeaderSize;
        _packetInfo.PacketSize = NativeHeaderSize;
        _packetInfo.IsEncrypted = false;
    }

    public byte GetByte()
    {
        if (_packetInfo.DataPosition >= _packetInfo.PacketSize)
            return 0;

        byte value = _packetInfo.Buffer[_packetInfo.DataPosition];
        _packetInfo.DataPosition++;
        return value;
    }

    public ushort GetUInt16()
    {
        if (_packetInfo.DataPosition + 2 > _packetInfo.PacketSize)
            return 0;

        ushort value = BitConverter.ToUInt16(_packetInfo.Buffer, (int)_packetInfo.DataPosition);
        _packetInfo.DataPosition += 2;
        return value;
    }

    public uint GetUInt32()
    {
        if (_packetInfo.DataPosition + 4 > _packetInfo.PacketSize)
            return 0;

        uint value = BitConverter.ToUInt32(_packetInfo.Buffer, (int)_packetInfo.DataPosition);
        _packetInfo.DataPosition += 4;
        return value;
    }

    public ReadOnlySpan<byte> GetString(uint length)
    {
        if (_packetInfo.DataPosition + length > _packetInfo.PacketSize)
            return ReadOnlySpan<byte>.Empty;

        var span = new ReadOnlySpan<byte>(_packetInfo.Buffer, (int)_packetInfo.DataPosition, (int)length);
        _packetInfo.DataPosition += length;
        return span;
    }

    public void PutByte(byte value)
    {
        EnsureCapacity(1);
        _packetInfo.Buffer[_packetInfo.PacketSize] = value;
        _packetInfo.PacketSize++;
    }

    public void PutUInt16(ushort value)
    {
        EnsureCapacity(2);
        BitConverter.TryWriteBytes(
            _packetInfo.Buffer.AsSpan((int)_packetInfo.PacketSize), value);
        _packetInfo.PacketSize += 2;
    }

    public void PutUInt32(uint value)
    {
        EnsureCapacity(4);
        BitConverter.TryWriteBytes(
            _packetInfo.Buffer.AsSpan((int)_packetInfo.PacketSize), value);
        _packetInfo.PacketSize += 4;
    }

    public void PutBytes(ReadOnlySpan<byte> data)
    {
        EnsureCapacity((uint)data.Length);
        data.CopyTo(_packetInfo.Buffer.AsSpan((int)_packetInfo.PacketSize));
        _packetInfo.PacketSize += (uint)data.Length;
    }

    public void Reset()
    {
        if (_packetInfo.Buffer.Length >= NativeHeaderSize)
        {
            _packetInfo.Buffer[0] = 0;
            _packetInfo.Buffer[1] = 0;
        }

        _packetInfo.DataPosition = NativeHeaderSize;
        _packetInfo.PacketSize = NativeHeaderSize;
        _packetInfo.IsEncrypted = false;
    }

    public bool TrySetPlainPacket(ReadOnlySpan<byte> plainPacket)
    {
        if (plainPacket.Length < NativeHeaderSize)
            return false;

        ushort declaredLength = BinaryPrimitives.ReadUInt16LittleEndian(plainPacket);
        if (declaredLength != plainPacket.Length)
            return false;

        if (plainPacket.Length > _packetInfo.Buffer.Length)
            Initialize((uint)plainPacket.Length);

        plainPacket.CopyTo(_packetInfo.Buffer);
        _packetInfo.PacketSize = (uint)plainPacket.Length;
        _packetInfo.DataPosition = NativeHeaderSize;
        return true;
    }

    public void FinalizePlainPacket()
    {
        BinaryPrimitives.WriteUInt16LittleEndian(_packetInfo.Buffer.AsSpan(0, 2), checked((ushort)_packetInfo.PacketSize));
    }

    /// <summary>
    /// Lee un string con prefijo de 1 byte de longitud (Packet::operator>> String).
    /// </summary>
    public string ReadPrefixedString()
    {
        byte length = GetByte();
        if (length == 0 || _packetInfo.DataPosition + length > _packetInfo.PacketSize)
            return string.Empty;
        var str = System.Text.Encoding.ASCII.GetString(
            _packetInfo.Buffer, (int)_packetInfo.DataPosition, length);
        _packetInfo.DataPosition += length;
        return str;
    }

    /// <summary>
    /// Escribe un string con prefijo de 1 byte de longitud (Packet::operator&lt;&lt; String).
    /// </summary>
    public void WritePrefixedString(string value)
    {
        var bytes = System.Text.Encoding.ASCII.GetBytes(value ?? string.Empty);
        var len = (byte)Math.Min(bytes.Length, 255);
        PutByte(len);
        PutBytes(bytes.AsSpan(0, len));
    }

    public static bool TryDecryptFrame(ReadOnlySpan<byte> frame, out byte[] payload)
    {
        payload = Array.Empty<byte>();

        if (frame.Length < 5)
            return false;

        ushort declaredLength = BinaryPrimitives.ReadUInt16LittleEndian(frame);
        if (declaredLength != frame.Length)
            return false;

        var body = frame.Slice(2);
        byte keyLength = DecodeNativeKeyLength(body[0]);
        if (keyLength < MinNativeKeyLength || keyLength > MaxNativeKeyLength)
            return false;

        if (body.Length <= keyLength)
            return false;

        int payloadLength = body.Length - 1 - keyLength;
        if (payloadLength <= 0 || payloadLength > 0x5000)
            return false;

        payload = new byte[payloadLength];

        for (int index = 0; index < payloadLength; index++)
        {
            int keyIndex = 1 + (index % keyLength);
            byte xorMask = unchecked((byte)(payloadLength - index));
            payload[index] = (byte)(body[1 + keyLength + index] ^ body[keyIndex] ^ xorMask);
        }

        return true;
    }

    public static byte[] EncryptFrame(ReadOnlySpan<byte> payload)
    {
        int keyLength = Random.Shared.Next(MinNativeKeyLength, MaxNativeKeyLength + 1);
        int totalLength = 2 + 1 + keyLength + payload.Length;
        var frame = new byte[totalLength];

        BinaryPrimitives.WriteUInt16LittleEndian(frame, checked((ushort)totalLength));
        frame[2] = EncodeNativeKeyLength((byte)keyLength);

        for (int index = 0; index < keyLength; index++)
        {
            frame[3 + index] = (byte)Random.Shared.Next(0, 0x7f);
        }

        for (int index = 0; index < payload.Length; index++)
        {
            byte keyByte = frame[3 + (index % keyLength)];
            byte xorMask = unchecked((byte)(payload.Length - index));
            frame[3 + keyLength + index] = (byte)(payload[index] ^ keyByte ^ xorMask);
        }

        return frame;
    }

    private static byte EncodeNativeKeyLength(byte keyLength)
    {
        byte rotated = (byte)((keyLength << 7) | (keyLength >> 1));
        return (byte)~rotated;
    }

    private static byte DecodeNativeKeyLength(byte encodedKeyLength)
    {
        byte inverted = (byte)~encodedKeyLength;
        return (byte)((inverted >> 7) | (inverted << 1));
    }

    private void EnsureCapacity(uint additionalBytes)
    {
        uint required = _packetInfo.PacketSize + additionalBytes;
        if (required > _packetInfo.TotalBufferSize)
        {
            uint newSize = Math.Max(required, _packetInfo.TotalBufferSize * 2);
            var newBuffer = new byte[newSize];
            Array.Copy(_packetInfo.Buffer, newBuffer, _packetInfo.Buffer.Length);
            _packetInfo.Buffer = newBuffer;
            _packetInfo.TotalBufferSize = newSize;
        }
    }
}
