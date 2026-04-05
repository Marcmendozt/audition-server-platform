namespace LoginDBAgent.Domain.Entities;

public sealed record UserAccount(
    uint UserSN,
    string UserId,
    string Password,
    string UserNick,
    byte Gender);
