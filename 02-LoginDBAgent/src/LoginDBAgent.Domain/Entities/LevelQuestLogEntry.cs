namespace LoginDBAgent.Domain.Entities;

public sealed record LevelQuestLogEntry(
    uint UserSN,
    int LevelQuestSN,
    int Score,
    int Perfect,
    int GameMode,
    int Reward,
    int Pass);
