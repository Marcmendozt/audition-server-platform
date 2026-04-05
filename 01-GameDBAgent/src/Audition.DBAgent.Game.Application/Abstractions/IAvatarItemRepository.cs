namespace Audition.DBAgent.Game.Application.Abstractions;

public interface IAvatarItemRepository
{
    Task InsertAsync(uint userSN, int itemId, int days, CancellationToken ct);
    Task<bool> HasActiveItemAsync(uint userSN, int itemId, CancellationToken ct);
    Task<bool> HasUnlimitedItemAsync(uint userSN, int itemId, CancellationToken ct);
}
