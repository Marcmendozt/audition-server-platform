namespace LoginDBAgent.Domain.Entities;

public sealed record LevelQuest(
    int Level,
    int RequiredExp,
    int Score,
    int Perfect,
    int ConsecutivePerfect,
    int GameMode,
    int MusicCode,
    int StageCode,
    int Fee,
    int WinDen,
    int WinExp);
