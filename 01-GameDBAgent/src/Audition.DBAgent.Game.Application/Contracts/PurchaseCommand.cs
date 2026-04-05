namespace Audition.DBAgent.Game.Application.Contracts;

public sealed record PurchaseCommand(
    uint UserSN,
    int ItemId,
    int Days,
    int Cost);
