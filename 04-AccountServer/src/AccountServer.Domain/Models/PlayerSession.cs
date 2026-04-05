namespace AccountServer.Domain.Models;

public sealed record PlayerSession(
    Guid SessionId,
    uint UserSerial,
    uint UserExperience,
    string UserId,
    uint ClusterId,
    ushort ServerId,
    ushort GatewayServerId,
    SessionInfo SessionInfo,
    DateTime ConnectedAtUtc,
    SessionState State);