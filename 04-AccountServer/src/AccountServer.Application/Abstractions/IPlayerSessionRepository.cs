using AccountServer.Domain.Models;

namespace AccountServer.Application.Abstractions;

public interface IPlayerSessionRepository
{
    Task<PlayerSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PlayerSession>> ListAsync(CancellationToken cancellationToken);

    Task RemoveAsync(Guid sessionId, CancellationToken cancellationToken);

    Task UpsertAsync(PlayerSession session, CancellationToken cancellationToken);
}