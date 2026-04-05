namespace Audition.GameServer.Domain.Models;

public sealed record NoticeConfiguration(IReadOnlyList<NoticeChannel> Channels);