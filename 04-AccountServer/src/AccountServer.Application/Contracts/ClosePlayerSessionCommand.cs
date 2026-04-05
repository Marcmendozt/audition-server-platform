namespace AccountServer.Application.Contracts;

public sealed record ClosePlayerSessionCommand(Guid SessionId);