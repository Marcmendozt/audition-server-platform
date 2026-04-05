namespace Audition.GameServer.Domain.Models;

public sealed record LegacyGameDataSnapshot(
    NoticeConfiguration? Notice,
    HackListConfiguration? HackList,
    MissionConfiguration? Mission,
    BattleConfiguration? Battle)
{
    public static LegacyGameDataSnapshot Empty { get; } = new(null, null, null, null);
}