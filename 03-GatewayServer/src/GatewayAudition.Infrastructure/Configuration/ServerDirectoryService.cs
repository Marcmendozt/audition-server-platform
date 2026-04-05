using GatewayAudition.Application.DTOs;
using GatewayAudition.Domain.Entities;
using GatewayAudition.Domain.Interfaces;
using GatewayAudition.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GatewayAudition.Infrastructure.Configuration;

public sealed class ServerDirectoryService : IServerDirectory
{
    private readonly ILogger<ServerDirectoryService> _logger;
    private readonly GatewaySettings _settings;
    private ServerInfo _serverInfo = new();
    private readonly List<ServerCluster> _clusters = new();
    private readonly Dictionary<uint, List<ServerInfo>> _serversPerCluster = new();

    public int ClusterCount => _clusters.Count;

    public ServerDirectoryService(
        IOptions<GatewaySettings> settings,
        ILogger<ServerDirectoryService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public void LoadConfig()
    {
        _serverInfo = new ServerInfo
        {
            ServerName = _settings.ServerName,
            ServerIp = "0.0.0.0",
            ServerPort = _settings.ListenPort,
            ServerVersion = _settings.ServerVersion,
            ServerStatus = 1
        };

        _clusters.Clear();
        _clusters.Add(new ServerCluster
        {
            ClusterId = _settings.ClusterId,
            ClusterName = _settings.ServerName,
            ServerCount = 0
        });

        _logger.LogInformation("Server directory loaded: {ServerName}, {ClusterCount} clusters",
            _serverInfo.ServerName, _clusters.Count);
    }

    public ServerInfo GetServerInfo() => _serverInfo;

    public IReadOnlyList<ServerCluster> GetClusters() => _clusters.AsReadOnly();

    public IReadOnlyList<ServerInfo> GetServersInCluster(uint clusterId)
    {
        if (_serversPerCluster.TryGetValue(clusterId, out var servers))
            return servers.AsReadOnly();

        return Array.Empty<ServerInfo>();
    }

    public void ReplaceServersInCluster(uint clusterId, string clusterName, IReadOnlyList<ServerInfo> servers)
    {
        _serversPerCluster[clusterId] = servers.ToList();

        var existingCluster = _clusters.Find(c => c.ClusterId == clusterId);
        if (existingCluster != null)
        {
            existingCluster.ClusterName = clusterName;
            existingCluster.ServerCount = servers.Count;
        }
        else
        {
            _clusters.Add(new ServerCluster
            {
                ClusterId = clusterId,
                ClusterName = clusterName,
                ServerCount = servers.Count
            });
        }
    }

    public void UpdateClusterInfo(uint clusterId, string clusterName, int serverCount)
    {
        var existing = _clusters.Find(c => c.ClusterId == clusterId);
        if (existing != null)
        {
            existing.ClusterName = clusterName;
            existing.ServerCount = serverCount;
        }
        else
        {
            _clusters.Add(new ServerCluster
            {
                ClusterId = clusterId,
                ClusterName = clusterName,
                ServerCount = serverCount
            });
        }
    }

    public void UpdateServerInCluster(uint clusterId, ServerInfo serverInfo)
    {
        if (!_serversPerCluster.TryGetValue(clusterId, out var servers))
        {
            servers = new List<ServerInfo>();
            _serversPerCluster[clusterId] = servers;
        }

        var existing = servers.Find(s => s.ServerId == serverInfo.ServerId);
        if (existing != null)
        {
            existing.CurrentUserCount = serverInfo.CurrentUserCount;
            existing.MaxUserCount = serverInfo.MaxUserCount;
            existing.ServerStatus = serverInfo.ServerStatus;
        }
        else
        {
            servers.Add(serverInfo);
            var cluster = _clusters.Find(c => c.ClusterId == clusterId);
            if (cluster != null)
                cluster.ServerCount = servers.Count;
        }
    }
}
