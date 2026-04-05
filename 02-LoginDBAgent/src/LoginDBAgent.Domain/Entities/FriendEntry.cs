namespace LoginDBAgent.Domain.Entities;

public sealed record FriendEntry(
    string UserNick,
    string FriendNick,
    string State);
