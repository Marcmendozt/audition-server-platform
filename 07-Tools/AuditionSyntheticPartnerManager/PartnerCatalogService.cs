using System.Text.Json;

namespace AuditionSyntheticPartnerManager;

public sealed class PartnerCatalogService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public string CatalogPath { get; }

    public string SettingsPath { get; }

    public PartnerCatalogService()
    {
        CatalogPath = ResolveCatalogPath();
        SettingsPath = Path.Combine(Path.GetDirectoryName(CatalogPath)!, "SyntheticPartnerSettings.json");
    }

    public List<SyntheticPartnerProfile> LoadProfiles()
    {
        if (!File.Exists(CatalogPath))
        {
            List<SyntheticPartnerProfile> defaults = CreateDefaultProfiles();
            SaveProfiles(defaults);
            return defaults;
        }

        using FileStream stream = File.OpenRead(CatalogPath);
        List<SyntheticPartnerProfile>? profiles = JsonSerializer.Deserialize<List<SyntheticPartnerProfile>>(stream, JsonOptions);
        return NormalizeProfiles(profiles);
    }

    public void SaveProfiles(IEnumerable<SyntheticPartnerProfile> profiles)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(CatalogPath)!);
        List<SyntheticPartnerProfile> normalized = NormalizeProfiles(profiles);
        string json = JsonSerializer.Serialize(normalized, JsonOptions);
        File.WriteAllText(CatalogPath, json);
    }

    public SyntheticPartnerSettings LoadSettings()
    {
        if (!File.Exists(SettingsPath))
        {
            SyntheticPartnerSettings defaults = SyntheticPartnerSettings.Default;
            SaveSettings(defaults);
            return defaults;
        }

        using FileStream stream = File.OpenRead(SettingsPath);
        SyntheticPartnerSettings? settings = JsonSerializer.Deserialize<SyntheticPartnerSettings>(stream, JsonOptions);
        return NormalizeSettings(settings);
    }

    public void SaveSettings(SyntheticPartnerSettings settings)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        string json = JsonSerializer.Serialize(NormalizeSettings(settings), JsonOptions);
        File.WriteAllText(SettingsPath, json);
    }

    private static string ResolveCatalogPath()
    {
        foreach (string seed in GetSeedDirectories())
        {
            string? current = seed;
            while (!string.IsNullOrEmpty(current))
            {
                string dataDirectory = Path.Combine(current, "05-GameServer", "Data");
                if (Directory.Exists(dataDirectory))
                {
                    return Path.Combine(dataDirectory, "SyntheticPartners.json");
                }

                current = Directory.GetParent(current)?.FullName;
            }
        }

        string fallbackDirectory = Path.Combine(Environment.CurrentDirectory, "05-GameServer", "Data");
        Directory.CreateDirectory(fallbackDirectory);
        return Path.Combine(fallbackDirectory, "SyntheticPartners.json");
    }

    private static IEnumerable<string> GetSeedDirectories()
    {
        yield return AppContext.BaseDirectory;
        yield return Environment.CurrentDirectory;
    }

    private static List<SyntheticPartnerProfile> NormalizeProfiles(IEnumerable<SyntheticPartnerProfile>? profiles)
    {
        List<SyntheticPartnerProfile> normalized = [];
        int index = 1;

        foreach (SyntheticPartnerProfile profile in profiles ?? [])
        {
            string nickname = string.IsNullOrWhiteSpace(profile.Nickname) ? $"partner{index}" : profile.Nickname.Trim();
            string userId = string.IsNullOrWhiteSpace(profile.UserId) ? nickname : profile.UserId.Trim();

            normalized.Add(new SyntheticPartnerProfile
            {
                Nickname = nickname,
                UserId = userId,
                Gender = profile.Gender,
                Experience = profile.Experience == 0 ? 1u : profile.Experience,
                Power = profile.Power,
                Ready = profile.Ready,
                Team = profile.Team
            });
            index++;
        }

        if (normalized.Count == 0)
        {
            normalized.AddRange(CreateDefaultProfiles());
        }

        return normalized;
    }

    private static List<SyntheticPartnerProfile> CreateDefaultProfiles()
    {
        return
        [
            new SyntheticPartnerProfile
            {
                Nickname = "partner",
                UserId = "partner",
                Gender = 1,
                Experience = 1,
                Power = 0,
                Ready = true,
                Team = 0
            },
            new SyntheticPartnerProfile
            {
                Nickname = "stardust",
                UserId = "stardust",
                Gender = 1,
                Experience = 540000,
                Power = 7,
                Ready = true,
                Team = 0
            },
            new SyntheticPartnerProfile
            {
                Nickname = "rhythmfox",
                UserId = "rhythmfox",
                Gender = 0,
                Experience = 920000,
                Power = 9,
                Ready = true,
                Team = 0
            }
        ];
    }

    private static SyntheticPartnerSettings NormalizeSettings(SyntheticPartnerSettings? settings)
    {
        if (settings is null)
        {
            return SyntheticPartnerSettings.Default;
        }

        return new SyntheticPartnerSettings
        {
            Enabled = settings.Enabled,
            SelectedProfileUserId = settings.SelectedProfileUserId?.Trim() ?? string.Empty
        };
    }
}