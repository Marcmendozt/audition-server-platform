namespace AuditionSyntheticPartnerManager;

public sealed class SyntheticPartnerSettings
{
    public static SyntheticPartnerSettings Default => new();

    public bool Enabled { get; set; }

    public string SelectedProfileUserId { get; set; } = string.Empty;
}