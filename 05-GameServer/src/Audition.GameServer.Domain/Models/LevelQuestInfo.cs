namespace Audition.GameServer.Domain.Models;

public sealed record LevelQuestInfo(
    byte Level,
    int RequiredExp,
    int RequiredScore,
    byte PerfectCount,
    byte ConsecutivePerfectCount,
    byte GameMode,
    ushort MusicFileIndex,
    byte Stage,
    int RequiredDen,
    int RewardDen,
    int RewardExp);