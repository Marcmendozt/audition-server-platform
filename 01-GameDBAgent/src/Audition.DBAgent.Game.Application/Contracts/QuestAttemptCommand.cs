namespace Audition.DBAgent.Game.Application.Contracts;

public sealed record QuestAttemptCommand(
    uint UserSN,
    int LevelQuestSN,
    int Score,
    int Perfect,
    int GameMode);
