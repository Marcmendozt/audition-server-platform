namespace GatewayAudition.Application.DTOs;

public sealed class GatewaySettings
{
    public ushort ListenPort { get; set; } = 10700;
    public uint MaxSessions { get; set; } = 1000;
    public uint SessionBufferSize { get; set; } = 8192;
    public uint MaxPackets { get; set; } = 2000;
    public uint PacketBufferSize { get; set; } = 4096;
    public string AccountServerIp { get; set; } = "127.0.0.1";
    public ushort AccountServerPort { get; set; } = 4501;
    public uint ClusterId { get; set; } = 1;
    public uint ServerId { get; set; } = 11;
    public byte ServerLevel { get; set; } = 1;
    public string ServerName { get; set; } = "GatewayServer1";
    public string PublicIp { get; set; } = "127.0.0.1";
    public uint MaxUserCount { get; set; } = 5000;
    public ushort TargetGameServerId { get; set; }
    public uint ServerVersion { get; set; } = 17;
    public int MaxWorkerThreads { get; set; } = 4;
    public int AccountServerSyncIntervalMs { get; set; } = 5000;
    public int AccountServerTimeoutMs { get; set; } = 5000;
}
