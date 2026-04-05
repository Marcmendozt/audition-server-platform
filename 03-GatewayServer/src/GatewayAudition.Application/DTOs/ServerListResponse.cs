using GatewayAudition.Domain.ValueObjects;

namespace GatewayAudition.Application.DTOs;

public sealed class ServerListResponse
{
    public IReadOnlyList<ServerClusterDto> Clusters { get; set; } = Array.Empty<ServerClusterDto>();
}

public sealed class ServerClusterDto
{
    public uint ClusterId { get; set; }
    public string ClusterName { get; set; } = string.Empty;
    public int ServerCount { get; set; }
}

public sealed class ServerInClusterDto
{
    public uint ServerId { get; set; }
    public string ServerName { get; set; } = string.Empty;
    public uint CurrentUserCount { get; set; }
    public uint MaxUserCount { get; set; }
    public string ServerIp { get; set; } = string.Empty;
    public ushort ServerPort { get; set; }
    public byte ServerStatus { get; set; }
}
