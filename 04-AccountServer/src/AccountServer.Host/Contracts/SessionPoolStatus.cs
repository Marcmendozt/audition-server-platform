namespace AccountServer.Host.Contracts;

public sealed record SessionPoolStatus(
    int ActiveSessions,
    int FreeSessions,
    int SessionsCreated,
    int MaxConcurrentSessions,
    int BufferSize);