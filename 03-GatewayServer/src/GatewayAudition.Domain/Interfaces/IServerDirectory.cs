using GatewayAudition.Domain.Entities;
using GatewayAudition.Domain.ValueObjects;

namespace GatewayAudition.Domain.Interfaces;

public interface IServerDirectory
{
    ServerInfo GetServerInfo();
    IReadOnlyList<ServerCluster> GetClusters();
    IReadOnlyList<ServerInfo> GetServersInCluster(uint clusterId);
    void LoadConfig();
    void ReplaceServersInCluster(uint clusterId, string clusterName, IReadOnlyList<ServerInfo> servers);
    int ClusterCount { get; }
}
