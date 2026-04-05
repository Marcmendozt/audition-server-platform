namespace AccountServer.Host.Configuration;

public sealed class AccountServerOptions
{
    public const string SectionName = "AccountServer";

    public bool BoardItemMode { get; init; } = true;

    public int BoardItemRetentionMinutes { get; init; } = 1440;

    public int BoardItemSweepIntervalSeconds { get; init; } = 60;

    public string BuildVersion { get; init; } = "Apr 20 2006 10:41:35";

    public string Host { get; init; } = "127.0.0.1";

    public ListenerOptions Account { get; init; } = new();

    public DbAgentOptions DbAgent { get; init; } = new();

    public DbAgentOptions DbAgentPay { get; init; } = new();

    public ListenerOptions Gateway { get; init; } = new()
    {
        Name = "Gateway",
        Port = 4501
    };

    public ListenerOptions Game { get; init; } = new()
    {
        Name = "Game",
        Port = 4502
    };

    public ListenerOptions Admin { get; init; } = new()
    {
        Name = "Admin",
        Port = 4503
    };

    public int PacketVersion { get; init; } = 0x0125;

    public int SessionBufferSize { get; init; } = 8192;

    public string ServerVersion { get; init; } = "1.25";

    public ServerDirectoryOptions ServerDirectory { get; init; } = new();

    public IReadOnlyList<ListenerOptions> GetEnabledListeners()
    {
        return new[] { Account, Gateway, Game, Admin }
            .Where(listener => listener.Enabled)
            .ToArray();
    }
}

public sealed class ListenerOptions
{
    public bool Enabled { get; init; } = true;

    public string Name { get; init; } = "Account";

    public int Port { get; init; } = 4500;
}

public sealed class DbAgentOptions
{
    public int ConnectTimeoutMs { get; init; } = 2000;

    public bool Enabled { get; init; }

    public string Host { get; init; } = "127.0.0.1";

    public int Port { get; init; } = 5500;
}

public sealed class ServerDirectoryOptions
{
    public List<ClusterEntry> Clusters { get; init; } = [];

    public List<GameServerEntry> GameServers { get; init; } = [];

    public string? CommunityServerIp { get; init; }
}

public sealed class ClusterEntry
{
    public int ClusterId { get; init; }

    public string Name { get; init; } = string.Empty;
}

public sealed class GameServerEntry
{
    public ushort ServerId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string IpAddress { get; init; } = "127.0.0.1";

    public ushort Port { get; init; } = 25511;

    public ushort Grade { get; init; } = 1;

    public ushort MaxUserCount { get; init; } = 5000;
}