namespace Audition.DBAgent.Game.Domain.Entities;

public sealed record AvatarItem(
    uint UserSN,
    int ItemId,
    DateTime RegDate,
    DateTime ExpireDate);
