namespace AccountServer.Host.Contracts;

public sealed record DbAgentStatusSnapshot(
    bool Enabled,
    bool Connected,
    string Endpoint,
    DateTime? LastConnectedAtUtc,
    string? LastError);