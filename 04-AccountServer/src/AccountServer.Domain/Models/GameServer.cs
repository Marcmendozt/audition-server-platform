namespace AccountServer.Domain.Models;

public sealed record GameServer(
    ushort ServerId,
    bool IsActive,
    string Name,
    string IpAddress,
    ushort Port,
    ushort Grade,
    ushort CurrentUserCount,
    ushort MaxChannelCount,
    ushort MaxUserCountPerChannel,
    ushort MaxUserCount,
    ServerStatus Status)
{
    public GameServer WithUserCount(ushort userCount)
    {
        return this with { CurrentUserCount = userCount };
    }
}