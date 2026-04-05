namespace Audition.GameServer.Domain.Models;

public sealed record MissionConfiguration(
    int MissionCount,
    int OccurRate,
    int PeakTime1,
    int PeakTime2,
    int PeakRate,
    IReadOnlyList<MissionRule> Rules);