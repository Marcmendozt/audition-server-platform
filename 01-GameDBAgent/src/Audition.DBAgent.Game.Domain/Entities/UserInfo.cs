namespace Audition.DBAgent.Game.Domain.Entities;

public sealed record UserInfo(
    uint UserSN,
    int Exp,
    int Money,
    int Cash,
    int Level,
    byte Gender,
    bool IsAllowMsg);
