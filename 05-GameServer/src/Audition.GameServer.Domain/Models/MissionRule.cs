namespace Audition.GameServer.Domain.Models;

public sealed record MissionRule(
    int Index,
    int Rank,
    int Point,
    int Perfect,
    int Great,
    int Cool,
    int Bad,
    int Miss,
    int SuccessivePerfect,
    int Finish,
    int Multiply,
    string Message);