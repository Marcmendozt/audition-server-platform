namespace LoginDBAgent.Domain.Entities;

public sealed record UserInfo(
    uint UserSN,
    int Exp,
    int Money,
    int Cash,
    int Level,
    bool IsAllowMsg);
