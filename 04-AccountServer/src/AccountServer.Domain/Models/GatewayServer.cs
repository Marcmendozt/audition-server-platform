namespace AccountServer.Domain.Models;

public sealed record GatewayServer(
    ushort ServerId,
    bool IsActive,
    ushort UserCount,
    ServerInfo Info)
{
    public GatewayServer WithUserCount(ushort userCount)
    {
        return this with
        {
            UserCount = userCount,
            Info = Info with { CurrentUserCount = userCount }
        };
    }
}