namespace LoginDBAgent.Domain.Entities;

public sealed record RankEntry(
    string UserNick,
    int Exp,
    int Position);
