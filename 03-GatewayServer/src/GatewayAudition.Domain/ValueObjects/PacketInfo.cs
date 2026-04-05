namespace GatewayAudition.Domain.ValueObjects;

public sealed class PacketInfo
{
    public uint TotalBufferSize { get; set; }
    public byte[] Buffer { get; set; } = Array.Empty<byte>();
    public uint PacketSize { get; set; }
    public bool IsEncrypted { get; set; }
    public uint DataPosition { get; set; }
}
