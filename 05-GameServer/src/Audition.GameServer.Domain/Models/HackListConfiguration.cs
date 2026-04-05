namespace Audition.GameServer.Domain.Models;

public sealed record HackListConfiguration(int DeclaredCount, IReadOnlyList<string> Entries);