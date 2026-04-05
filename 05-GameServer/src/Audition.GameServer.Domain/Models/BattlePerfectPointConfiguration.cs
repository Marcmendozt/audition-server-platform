namespace Audition.GameServer.Domain.Models;

public sealed record BattlePerfectPointConfiguration(
    int SyncCount,
    int NormalSoloPerfect,
    int HighSoloPerfect,
    int NormalSoloStraightPerfect,
    int HighSoloStraightPerfect,
    int NormalSyncPerfect,
    int HighSyncPerfect,
    int NormalDanceMasterPerfect,
    int HighDanceMasterPerfect,
    int NormalDanceMasterStraightPerfect,
    int HighDanceMasterStraightPerfect);