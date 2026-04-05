namespace GatewayAudition.Domain.Entities;

public class ServerInfo
{
    public uint ServerId { get; set; }
    public uint ClusterId { get; set; }
    public string ServerName { get; set; } = string.Empty;
    public byte ServerLevel { get; set; }
    public uint CurrentUserCount { get; set; }
    public uint MaxUserCount { get; set; }
    public string ServerIp { get; set; } = string.Empty;
    public ushort ServerPort { get; set; }
    public uint ServerVersion { get; set; }
    public byte ServerStatus { get; set; }
}
