using Audition.GameServer.Application.Abstractions;
using Audition.GameServer.Domain.Models;
using Audition.GameServer.Infrastructure.Parsing;

namespace Audition.GameServer.Infrastructure.Configuration;

public sealed class LegacyGameDataLoader : ILegacyGameDataLoader
{
    public async Task<LegacyGameDataSnapshot> LoadAsync(string dataDirectory, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dataDirectory) || !Directory.Exists(dataDirectory))
        {
            return LegacyGameDataSnapshot.Empty;
        }

        NoticeConfiguration? notice = await LoadNoticeAsync(Path.Combine(dataDirectory, "NoticeInfo.ini"), ct);
        HackListConfiguration? hackList = await LoadHackListAsync(Path.Combine(dataDirectory, "HackList.ini"), ct);
        MissionConfiguration? mission = await LoadMissionAsync(Path.Combine(dataDirectory, "MissionInfo.ini"), ct);
        BattleConfiguration? battle = await LoadBattleAsync(Path.Combine(dataDirectory, "BattleInfo.ini"), ct);

        return new LegacyGameDataSnapshot(notice, hackList, mission, battle);
    }

    private static async Task<NoticeConfiguration?> LoadNoticeAsync(string path, CancellationToken ct)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        IniDocument document = await IniDocument.LoadAsync(path, ct);
        if (!document.TryGetSection("Notice", out IniSection? section))
        {
            return null;
        }

        var channels = section.Properties
            .Where(entry => entry.Key.StartsWith("Channel", StringComparison.OrdinalIgnoreCase))
            .Select(entry => new NoticeChannel(ParseTrailingIndex(entry.Key), entry.Value))
            .OrderBy(entry => entry.Index)
            .ToArray();

        return new NoticeConfiguration(channels);
    }

    private static async Task<HackListConfiguration?> LoadHackListAsync(string path, CancellationToken ct)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        IniDocument document = await IniDocument.LoadAsync(path, ct);
        if (!document.TryGetSection("Hack", out IniSection? section))
        {
            return null;
        }

        int declaredCount = section.GetInt("HackCount");
        var entries = section.Properties
            .Where(entry => entry.Key.StartsWith("List", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(entry.Value))
            .OrderBy(entry => ParseTrailingIndex(entry.Key))
            .Select(entry => entry.Value)
            .ToArray();

        return new HackListConfiguration(declaredCount, entries);
    }

    private static async Task<MissionConfiguration?> LoadMissionAsync(string path, CancellationToken ct)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        IniDocument document = await IniDocument.LoadAsync(path, ct);
        if (!document.TryGetSection("MissionInfo", out IniSection? section))
        {
            return null;
        }

        var rules = document.Sections
            .Where(entry => entry.Name.StartsWith("Mission", StringComparison.OrdinalIgnoreCase) && !entry.Name.Equals("MissionInfo", StringComparison.OrdinalIgnoreCase))
            .Select(entry => new MissionRule(
                ParseTrailingIndex(entry.Name),
                entry.GetInt("Rank"),
                entry.GetInt("Point"),
                entry.GetInt("Perpect"),
                entry.GetInt("Great"),
                entry.GetInt("Cool"),
                entry.GetInt("Bad"),
                entry.GetInt("Miss"),
                entry.GetInt("SuccessivePerpect"),
                entry.GetInt("Finish"),
                entry.GetInt("Multiply"),
                entry.GetString("Message")))
            .OrderBy(entry => entry.Index)
            .ToArray();

        return new MissionConfiguration(
            section.GetInt("MissionCount"),
            section.GetInt("OccurRate"),
            section.GetInt("PeakTime1"),
            section.GetInt("PeakTime2"),
            section.GetInt("PeakRate"),
            rules);
    }

    private static async Task<BattleConfiguration?> LoadBattleAsync(string path, CancellationToken ct)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        IniDocument document = await IniDocument.LoadAsync(path, ct);
        if (!document.TryGetSection("BattleInfo", out IniSection? battleInfoSection) ||
            !document.TryGetSection("PerfectPoint", out IniSection? perfectPointSection) ||
            !document.TryGetSection("Attack", out IniSection? attackSection))
        {
            return null;
        }

        var stages = document.Sections
            .Where(entry => entry.Name.StartsWith("BattleInfo", StringComparison.OrdinalIgnoreCase) && !entry.Name.Equals("BattleInfo", StringComparison.OrdinalIgnoreCase))
            .Select(entry => new BattleStageConfiguration(
                ParseTrailingIndex(entry.Name),
                entry.GetInt("DanceMaster"),
                entry.GetInt("GameMode"),
                entry.GetString("MusicCode"),
                entry.GetInt("MapCode"),
                entry.GetInt("EntryFee"),
                entry.GetInt("Prize"),
                entry.GetInt("Perfect"),
                entry.GetInt("Great"),
                entry.GetInt("Cool"),
                entry.GetInt("Bad"),
                entry.GetInt("StraightPerfect"),
                entry.GetInt("StartPerfectGage")))
            .OrderBy(entry => entry.Index)
            .ToArray();

        var perfectPoint = new BattlePerfectPointConfiguration(
            perfectPointSection.GetInt("SyncCount"),
            perfectPointSection.GetInt("NormalSoloPerfect"),
            perfectPointSection.GetInt("HighSoloPerfect"),
            perfectPointSection.GetInt("NormalSoloStraightPerfect"),
            perfectPointSection.GetInt("HighSoloStraightPerfect"),
            perfectPointSection.GetInt("NormalSyncPerfect"),
            perfectPointSection.GetInt("HighSyncPerfect"),
            perfectPointSection.GetInt("NormalDanceMasterPerfect"),
            perfectPointSection.GetInt("HighDanceMasterPerfect"),
            perfectPointSection.GetInt("NormalDanceMasterStraightPerfect"),
            perfectPointSection.GetInt("HighDanceMasterStraightPerfect"));

        var attack = new BattleAttackConfiguration(
            attackSection.GetInt("UserToDanceMaster"),
            attackSection.GetInt("DanceMasterToUser"));

        return new BattleConfiguration(
            battleInfoSection.GetInt("BattleCount"),
            battleInfoSection.GetInt("BattleEntryFee"),
            perfectPoint,
            attack,
            stages);
    }

    private static int ParseTrailingIndex(string value)
    {
        string digits = new string(value.Reverse().TakeWhile(char.IsDigit).Reverse().ToArray());
        return int.TryParse(digits, out int parsed) ? parsed : 0;
    }
}