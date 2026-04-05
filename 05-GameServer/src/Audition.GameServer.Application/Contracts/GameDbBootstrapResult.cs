using Audition.GameServer.Domain.Models;

namespace Audition.GameServer.Application.Contracts;

public sealed record GameDbBootstrapResult(
    bool Success,
    string Message,
    LegacyServerInfo? ServerInfo,
    IReadOnlyList<LegacyChannelInfo> Channels,
    IReadOnlyList<LevelQuestInfo> LevelQuests);