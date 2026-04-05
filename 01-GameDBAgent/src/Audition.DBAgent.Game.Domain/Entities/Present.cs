namespace Audition.DBAgent.Game.Domain.Entities;

public sealed record Present(
    uint RecvSN,
    int ItemId,
    int Period,
    DateTime RecvDate);
