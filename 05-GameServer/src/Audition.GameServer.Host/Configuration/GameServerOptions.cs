using Audition.GameServer.Application.Contracts;
using Audition.GameServer.Domain.Models;

namespace Audition.GameServer.Host.Configuration;

public sealed class GameServerOptions
{
    public const string SectionName = "GameServer";

    public ushort ServerId { get; set; } = 101;

    public string ServerName { get; set; } = "RZ Bootstrap";

    public uint ClusterId { get; set; } = 1;

    public ushort Grade { get; set; } = 1;

    public uint MaxUserCount { get; set; } = 5000;

    public uint Version { get; set; } = 1;

    public ServerStatus Status { get; set; } = ServerStatus.Online;

    public ListenerOptions Listener { get; set; } = new();

    public EndpointOptions DbAgent { get; set; } = new()
    {
        Name = "DBAgent",
        Host = "127.0.0.1",
        Port = 25525,
        Enabled = true,
        Required = true,
    };

    public EndpointOptions AccountServer { get; set; } = new()
    {
        Name = "AccountServer",
        Host = "127.0.0.1",
        Port = 4502,
        Enabled = true,
        Required = true,
    };

    public EndpointOptions Certify { get; set; } = new()
    {
        Name = "Certify",
        Host = "127.0.0.1",
        Port = 28888,
        Enabled = true,
        Required = false,
    };

    public LegacyIniOptions LegacyIni { get; set; } = new();

    public LegacyDataOptions LegacyData { get; set; } = new();

    public RuntimePathOptions RuntimePaths { get; set; } = new();

    public BootstrapConfiguration ToBootstrapConfiguration(LegacyBootstrapOverrides? overrides = null)
    {
        return new BootstrapConfiguration(
            overrides?.ServerId ?? ServerId,
            ServerName,
            Listener.Host,
            Listener.Port,
            ClusterId,
            Grade,
            MaxUserCount,
            Version,
            Status,
            DbAgent.ToDefinition(overrides?.DbAgentHost),
            AccountServer.ToDefinition(overrides?.AccountServerHost),
            Certify.ToDefinition());
    }
}

public sealed class ListenerOptions
{
    public string Host { get; set; } = "127.0.0.1";

    public int Port { get; set; } = 25511;
}

public sealed class EndpointOptions
{
    public string Name { get; set; } = string.Empty;

    public string Host { get; set; } = "127.0.0.1";

    public int Port { get; set; }

    public bool Enabled { get; set; } = true;

    public bool Required { get; set; } = true;

    public EndpointDefinition ToDefinition(string? overrideHost = null)
    {
        return new EndpointDefinition(
            string.IsNullOrWhiteSpace(Name) ? Host : Name,
            string.IsNullOrWhiteSpace(overrideHost) ? Host : overrideHost,
            Port,
            Enabled,
            Required);
    }
}

public sealed class LegacyIniOptions
{
    public bool Enabled { get; set; } = true;

    public string Path { get; set; } = "Data/ServerInfo.ini";
}

public sealed class LegacyDataOptions
{
    public bool Enabled { get; set; } = true;

    public string Path { get; set; } = "Data";
}

public sealed class RuntimePathOptions
{
    public string DataPath { get; set; } = "Data";

    public string LogPath { get; set; } = "log";

    public string ReportPath { get; set; } = "Report";

    public string ScriptPath { get; set; } = "script";

    public string SoundPath { get; set; } = "sound";
}