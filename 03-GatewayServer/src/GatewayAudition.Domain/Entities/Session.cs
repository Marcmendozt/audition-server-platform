using GatewayAudition.Domain.ValueObjects;

namespace GatewayAudition.Domain.Entities;

public class Session
{
    public uint UniqueIndex { get; set; }
    public SessionInfo SessionInfo { get; } = new();
    public int TotalBufferSize { get; set; }
    public byte[] RecvBuffer { get; set; } = Array.Empty<byte>();
    public byte[] SendBuffer { get; set; } = Array.Empty<byte>();
    public int ReceivedSize { get; set; }
    public bool IsAlive { get; private set; }
    public string LastInboundPacketSummary { get; set; } = string.Empty;
    public string LastInboundFrameHex { get; set; } = string.Empty;
    public string LastInboundPlainHex { get; set; } = string.Empty;
    public string LastOutboundPacketLabel { get; set; } = string.Empty;
    public string LastOutboundFrameHex { get; set; } = string.Empty;
    public string LastOutboundPlainHex { get; set; } = string.Empty;

    public void Initialize()
    {
        UniqueIndex = 0;
        SessionInfo.Socket = 0;
        SessionInfo.IpAddress = string.Empty;
        SessionInfo.LastAccessTime = 0;
        ReceivedSize = 0;
        IsAlive = false;
        LastInboundPacketSummary = string.Empty;
        LastInboundFrameHex = string.Empty;
        LastInboundPlainHex = string.Empty;
        LastOutboundPacketLabel = string.Empty;
        LastOutboundFrameHex = string.Empty;
        LastOutboundPlainHex = string.Empty;
    }

    public void Initialize(int bufferSize)
    {
        Initialize();
        TotalBufferSize = bufferSize;
        RecvBuffer = new byte[bufferSize];
        SendBuffer = new byte[bufferSize];
    }

    public void SetSocketAndIp(uint socket, string ipAddress)
    {
        SessionInfo.Socket = socket;
        SessionInfo.IpAddress = ipAddress;
        SessionInfo.UpdateAccessTime();
        IsAlive = true;
    }

    public void Close()
    {
        IsAlive = false;
        SessionInfo.Socket = 0;
        LastInboundPacketSummary = string.Empty;
        LastInboundFrameHex = string.Empty;
        LastInboundPlainHex = string.Empty;
        LastOutboundPacketLabel = string.Empty;
        LastOutboundFrameHex = string.Empty;
        LastOutboundPlainHex = string.Empty;
    }
}
