using System.Collections.Concurrent;
using AccountServer.Application.Abstractions;
using AccountServer.Domain.Models;

namespace AccountServer.Infrastructure.Repositories;

public sealed class InMemoryGameServerRepository : IGameServerRepository
{
    private readonly ConcurrentDictionary<ushort, GameServer> items = new();

    public Task<GameServer?> GetByIdAsync(ushort serverId, CancellationToken cancellationToken)
    {
        items.TryGetValue(serverId, out var gameServer);
        return Task.FromResult(gameServer);
    }

    public Task<IReadOnlyCollection<GameServer>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<GameServer> snapshot = items.Values.OrderBy(item => item.ServerId).ToArray();
        return Task.FromResult(snapshot);
    }

    public Task UpsertAsync(GameServer gameServer, CancellationToken cancellationToken)
    {
        items[gameServer.ServerId] = gameServer;
        return Task.CompletedTask;
    }
}