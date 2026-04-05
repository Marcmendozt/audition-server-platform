namespace Audition.GameServer.Domain.Models;

public sealed record LegacyChannelInfo(
    ushort Number,
    string Name,
    ushort MaxUsers,
    ushort MaxRooms,
    ushort MinLevel,
    ushort MaxLevel,
    byte EventNumber);