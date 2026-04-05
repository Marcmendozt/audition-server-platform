namespace Audition.GameServer.Domain.Models;

public sealed record BootstrapRuntimeState(
    bool AccountServerRegistered,
    LegacyServerInfo? ServerInfo,
    IReadOnlyList<LegacyChannelInfo> Channels,
    IReadOnlyList<LevelQuestInfo> LevelQuests,
    LegacyGameDataSnapshot GameData)
{
    public static BootstrapRuntimeState Empty { get; } = new(false, null, [], [], LegacyGameDataSnapshot.Empty);
}