using GatewayAudition.Application.DTOs;
using GatewayAudition.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GatewayAudition.Application.Services;

public sealed class ServerListService
{
    private readonly IServerDirectory _serverDirectory;
    private readonly ILogger<ServerListService> _logger;

    public ServerListService(
        IServerDirectory serverDirectory,
        ILogger<ServerListService> logger)
    {
        _serverDirectory = serverDirectory;
        _logger = logger;
    }

    public ServerListResponse GetServerClusterList()
    {
        var clusters = _serverDirectory.GetClusters();

        return new ServerListResponse
        {
            Clusters = clusters.Select(c => new ServerClusterDto
            {
                ClusterId = c.ClusterId,
                ClusterName = c.ClusterName,
                ServerCount = c.ServerCount
            }).ToList()
        };
    }

    public IReadOnlyList<ServerInClusterDto> GetServersInCluster(uint clusterId)
    {
        var servers = _serverDirectory.GetServersInCluster(clusterId);

        return servers.Select(s => new ServerInClusterDto
        {
            ServerId = s.ServerId,
            ServerName = s.ServerName,
            CurrentUserCount = s.CurrentUserCount,
            MaxUserCount = s.MaxUserCount,
            ServerIp = s.ServerIp,
            ServerPort = s.ServerPort,
            ServerStatus = s.ServerStatus
        }).ToList();
    }
}
