namespace Audition.DBAgent.Game.Application.Abstractions;

public interface IPresentRepository
{
    Task<bool> HasPendingPresentAsync(uint userSN, int itemId, CancellationToken ct);
}
