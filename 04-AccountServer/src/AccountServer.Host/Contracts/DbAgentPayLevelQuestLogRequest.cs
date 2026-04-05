namespace AccountServer.Host.Contracts;

public sealed record DbAgentPayLevelQuestLogRequest(
    uint UserSerial,
    short ProcLevel,
    int BeforeDen,
    int AfterDen,
    int BeforeExp,
    int AfterExp,
    byte Pass);