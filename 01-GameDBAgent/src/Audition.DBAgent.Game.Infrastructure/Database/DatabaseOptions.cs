namespace Audition.DBAgent.Game.Infrastructure.Database;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string Server { get; set; } = "127.0.0.1";
    public string Database { get; set; } = "audition";
    public string User { get; set; } = "ragezone";
    public string Password { get; set; } = "ragezone";
    public bool SslEnabled { get; set; }

    public string ToConnectionString()
    {
        var sslMode = SslEnabled ? "Required" : "Disabled";
        return $"Server={Server};Database={Database};Uid={User};Pwd={Password};SslMode={sslMode};";
    }
}
