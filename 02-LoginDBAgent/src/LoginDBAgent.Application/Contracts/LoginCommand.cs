namespace LoginDBAgent.Application.Contracts;

public sealed record LoginCommand(
    string UserId,
    string Password);
