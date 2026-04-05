namespace Audition.GameServer.Domain.Models;

public sealed record BattleStageConfiguration(
    int Index,
    int DanceMaster,
    int GameMode,
    string MusicCode,
    int MapCode,
    int EntryFee,
    int Prize,
    int Perfect,
    int Great,
    int Cool,
    int Bad,
    int StraightPerfect,
    int StartPerfectGage);