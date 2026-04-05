namespace LoginDBAgent.Application.Contracts;

public sealed record GameResultCommand(
    uint UserSN,
    int ExpGain,
    int MoneyGain);
