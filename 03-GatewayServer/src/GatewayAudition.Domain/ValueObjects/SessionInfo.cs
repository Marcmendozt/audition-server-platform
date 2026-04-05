using System.Net;

namespace GatewayAudition.Domain.ValueObjects;

public sealed class SessionInfo
{
    public uint Socket { get; set; }
    public uint LastAccessTime { get; set; }
    public string IpAddress { get; set; } = string.Empty;

    public void UpdateAccessTime()
    {
        LastAccessTime = (uint)Environment.TickCount;
    }
}
