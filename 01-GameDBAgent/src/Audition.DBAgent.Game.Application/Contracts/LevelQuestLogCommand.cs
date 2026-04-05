namespace Audition.DBAgent.Game.Application.Contracts;

public sealed record LevelQuestLogCommand(
    uint UserSN,
    short ProcLevel,
    int BeforeMoney,
    int AfterMoney,
    int BeforeExp,
    int AfterExp,
    byte Pass);
