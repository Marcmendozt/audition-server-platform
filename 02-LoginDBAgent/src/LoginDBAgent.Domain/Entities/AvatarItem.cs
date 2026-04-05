namespace LoginDBAgent.Domain.Entities;

public sealed record AvatarItem(
    uint UserSN,
    int ItemId,
    string EquipState,
    DateTime RegDate,
    DateTime ExpireDate);
