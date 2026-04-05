namespace AuditionSyntheticPartnerManager;

public sealed class SyntheticPartnerProfile
{
    public string Nickname { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public byte Gender { get; set; } = 1;

    public uint Experience { get; set; } = 1;

    public byte Power { get; set; }

    public bool Ready { get; set; } = true;

    public byte Team { get; set; }

    public SyntheticPartnerProfile Clone()
    {
        return new SyntheticPartnerProfile
        {
            Nickname = Nickname,
            UserId = UserId,
            Gender = Gender,
            Experience = Experience,
            Power = Power,
            Ready = Ready,
            Team = Team
        };
    }

    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(Nickname) ? "Nuevo partner" : Nickname;
    }
}