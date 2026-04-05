using System.Collections.Concurrent;
using AccountServer.Application.Abstractions;
using AccountServer.Domain.Models;

namespace AccountServer.Infrastructure.Repositories;

public sealed class InMemoryGatewayServerRepository : IGatewayServerRepository
{
    private readonly ConcurrentDictionary<ushort, GatewayServer> items = new();

    public Task<GatewayServer?> GetByIdAsync(ushort serverId, CancellationToken cancellationToken)
    {
        items.TryGetValue(serverId, out var gatewayServer);
        return Task.FromResult(gatewayServer);
    }

    public Task<IReadOnlyCollection<GatewayServer>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<GatewayServer> snapshot = items.Values.OrderBy(item => item.ServerId).ToArray();
        return Task.FromResult(snapshot);
    }

    public Task UpsertAsync(GatewayServer gatewayServer, CancellationToken cancellationToken)
    {
        items[gatewayServer.ServerId] = gatewayServer;
        return Task.CompletedTask;
    }
}