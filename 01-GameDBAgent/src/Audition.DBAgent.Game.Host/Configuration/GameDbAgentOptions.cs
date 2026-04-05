namespace Audition.DBAgent.Game.Host.Configuration;

public sealed class GameDbAgentOptions
{
    public const string SectionName = "GameDbAgent";

    public int Port { get; set; } = 25525;
}
