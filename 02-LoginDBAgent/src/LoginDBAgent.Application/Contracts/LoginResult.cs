namespace LoginDBAgent.Application.Contracts;

public sealed record LoginResult(
    bool Success,
    uint UserSN,
    string UserNick,
    int Cash,
    string? ErrorReason);
