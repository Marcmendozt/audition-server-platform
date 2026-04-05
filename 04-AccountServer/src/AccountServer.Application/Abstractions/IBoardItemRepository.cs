using AccountServer.Domain.Models;

namespace AccountServer.Application.Abstractions;

public interface IBoardItemRepository
{
    Task<IReadOnlyCollection<BoardItem>> ListAsync(CancellationToken cancellationToken);

    Task PruneExpiredAsync(DateTime thresholdUtc, CancellationToken cancellationToken);

    Task UpsertAsync(BoardItem item, CancellationToken cancellationToken);
}