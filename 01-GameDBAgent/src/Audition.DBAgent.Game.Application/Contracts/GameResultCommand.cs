namespace Audition.DBAgent.Game.Application.Contracts;

public sealed record GameResultCommand(
    uint UserSN,
    int ExpGain,
    int MoneyGain);
