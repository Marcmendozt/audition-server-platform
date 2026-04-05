using AccountServer.Domain.Models;

namespace AccountServer.Application.Contracts;

public sealed record RegisterGameServerCommand(
    ushort ServerId,
    string Name,
    string IpAddress,
    ushort Port,
    ushort Grade,
    ushort MaxChannelCount,
    ushort MaxUserCountPerChannel,
    ushort MaxUserCount,
    ServerStatus Status = ServerStatus.Online);