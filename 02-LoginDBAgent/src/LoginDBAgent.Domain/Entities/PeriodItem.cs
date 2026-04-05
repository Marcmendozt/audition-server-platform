namespace LoginDBAgent.Domain.Entities;

public sealed record PeriodItem(
    uint UserSN,
    int ItemId,
    int RemainingHours);
