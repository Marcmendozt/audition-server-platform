namespace AccountServer.Domain.Models;

public sealed record ServerInfo(
    ushort ServerId,
    uint ClusterId,
    string Name,
    byte Level,
    uint CurrentUserCount,
    uint MaxUserCount,
    string IpAddress,
    ushort Port,
    uint Version,
    ServerStatus Status);