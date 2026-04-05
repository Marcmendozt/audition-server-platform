namespace GatewayAudition.Domain.ValueObjects;

public sealed class ServerCluster
{
    public uint ClusterId { get; set; }
    public string ClusterName { get; set; } = string.Empty;
    public int ServerCount { get; set; }
}
