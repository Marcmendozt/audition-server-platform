namespace Audition.GameServer.Domain.Models;

public sealed record LegacyServerInfo(
    ushort ServerNumber,
    string Name,
    string IpAddress,
    ushort Port,
    ushort CurrentUsers,
    ushort MaxUsers,
    ushort Grade,
    ushort IpRestriction,
    ushort DoubleDen);