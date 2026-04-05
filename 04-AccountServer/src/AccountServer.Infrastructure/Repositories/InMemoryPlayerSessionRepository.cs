using System.Collections.Concurrent;
using AccountServer.Application.Abstractions;
using AccountServer.Domain.Models;

namespace AccountServer.Infrastructure.Repositories;

public sealed class InMemoryPlayerSessionRepository : IPlayerSessionRepository
{
    private readonly ConcurrentDictionary<Guid, PlayerSession> items = new();

    public Task<PlayerSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        items.TryGetValue(sessionId, out var session);
        return Task.FromResult(session);
    }

    public Task<IReadOnlyCollection<PlayerSession>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<PlayerSession> snapshot = items.Values.OrderBy(item => item.ConnectedAtUtc).ToArray();
        return Task.FromResult(snapshot);
    }

    public Task RemoveAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        items.TryRemove(sessionId, out _);
        return Task.CompletedTask;
    }

    public Task UpsertAsync(PlayerSession session, CancellationToken cancellationToken)
    {
        items[session.SessionId] = session;
        return Task.CompletedTask;
    }
}