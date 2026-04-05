namespace AccountServer.Host.Contracts;

public sealed class BinaryPacketRequest
{
    public BinaryPacketRequest(ushort length, byte[] buffer)
    {
        if (buffer.Length == 0)
        {
            throw new ArgumentException("El paquete binario debe contener al menos un opcode.", nameof(buffer));
        }

        Length = length;
        Buffer = buffer;
    }

    public ushort Length { get; }

    public byte Opcode => Buffer[0];

    public byte[] Buffer { get; }

    public ReadOnlyMemory<byte> Payload => Buffer.AsMemory(1);

    public byte? SubOpcode => Buffer.Length > 1 ? Buffer[1] : null;
}