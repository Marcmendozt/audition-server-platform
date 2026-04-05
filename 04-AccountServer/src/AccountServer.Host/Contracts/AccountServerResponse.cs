namespace AccountServer.Host.Contracts;

public sealed record AccountServerResponse(bool Success, string Message, object? Data = null);