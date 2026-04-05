using AccountServer.Domain.Models;

namespace AccountServer.Application.Contracts;

public sealed record RegisterGatewayServerCommand(
    ushort ServerId,
    uint ClusterId,
    string Name,
    byte Level,
    string IpAddress,
    ushort Port,
    uint MaxUserCount,
    uint Version,
    ServerStatus Status = ServerStatus.Online);