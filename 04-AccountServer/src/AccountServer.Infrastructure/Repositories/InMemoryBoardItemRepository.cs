using System.Collections.Concurrent;
using AccountServer.Application.Abstractions;
using AccountServer.Domain.Models;

namespace AccountServer.Infrastructure.Repositories;

public sealed class InMemoryBoardItemRepository : IBoardItemRepository
{
    private readonly ConcurrentDictionary<ulong, BoardItem> items = new();

    public Task<IReadOnlyCollection<BoardItem>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<BoardItem> snapshot = items.Values
            .OrderByDescending(item => item.UpdatedAtUtc)
            .ToArray();

        return Task.FromResult(snapshot);
    }

    public Task PruneExpiredAsync(DateTime thresholdUtc, CancellationToken cancellationToken)
    {
        foreach (var item in items.Values.Where(item => item.UpdatedAtUtc < thresholdUtc).ToArray())
        {
            items.TryRemove(item.BoardSerial, out _);
        }

        return Task.CompletedTask;
    }

    public Task UpsertAsync(BoardItem item, CancellationToken cancellationToken)
    {
        items[item.BoardSerial] = item;
        return Task.CompletedTask;
    }
}