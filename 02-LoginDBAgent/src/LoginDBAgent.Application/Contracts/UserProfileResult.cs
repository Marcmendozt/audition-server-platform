namespace LoginDBAgent.Application.Contracts;

public sealed record UserProfileResult(
    uint UserSN,
    int Exp,
    int Money,
    int Cash,
    int Level,
    bool IsAllowMsg);
