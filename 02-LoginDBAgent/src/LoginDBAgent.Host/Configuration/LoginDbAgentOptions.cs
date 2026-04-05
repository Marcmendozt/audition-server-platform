namespace LoginDBAgent.Host.Configuration;

public sealed class LoginDbAgentOptions
{
    public const string SectionName = "LoginDbAgent";

    public int Port { get; set; } = 25527;
}
