namespace LoginDBAgent.Application.Contracts;

public sealed record QuestResult(
    bool Success,
    string? ErrorReason,
    int NewExp,
    int NewMoney,
    int NewLevel);
