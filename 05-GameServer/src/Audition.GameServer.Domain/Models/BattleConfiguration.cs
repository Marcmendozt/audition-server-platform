namespace Audition.GameServer.Domain.Models;

public sealed record BattleConfiguration(
    int BattleCount,
    int BattleEntryFee,
    BattlePerfectPointConfiguration PerfectPoint,
    BattleAttackConfiguration Attack,
    IReadOnlyList<BattleStageConfiguration> Stages);