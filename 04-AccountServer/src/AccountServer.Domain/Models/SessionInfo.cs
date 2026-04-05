namespace AccountServer.Domain.Models;

public sealed record SessionInfo(
    long SocketHandle,
    string IpAddress,
    DateTime LastAccessUtc);