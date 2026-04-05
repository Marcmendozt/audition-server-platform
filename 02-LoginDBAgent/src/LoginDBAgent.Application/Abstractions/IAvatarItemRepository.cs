using LoginDBAgent.Domain.Entities;

namespace LoginDBAgent.Application.Abstractions;

public interface IAvatarItemRepository
{
    Task InsertAsync(uint userSN, int itemId, int days, CancellationToken ct);
    Task DeleteAsync(uint userSN, int itemId, CancellationToken ct);
    Task<bool> HasActiveItemAsync(uint userSN, int itemId, CancellationToken ct);
    Task<bool> HasUnlimitedItemAsync(uint userSN, int itemId, CancellationToken ct);
    Task SetEquipStateAsync(uint userSN, int itemId, string state, CancellationToken ct);
    Task<IReadOnlyList<AvatarItem>> GetEquippedItemsAsync(uint userSN, CancellationToken ct);
    Task<IReadOnlyList<AvatarItem>> GetInventoryItemsAsync(uint userSN, CancellationToken ct);
    Task<IReadOnlyList<AvatarItem>> GetAllItemsAsync(uint userSN, CancellationToken ct);
}
