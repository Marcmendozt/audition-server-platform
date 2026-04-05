using Audition.GameServer.Application.Abstractions;
using Audition.GameServer.Application.Contracts;

namespace Audition.GameServer.Infrastructure.Configuration;

public sealed class LegacyGameServerIniLoader : ILegacyGameServerConfigLoader
{
    public async Task<LegacyBootstrapOverrides?> LoadAsync(string path, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return null;
        }

        string? accountServerHost = null;
        string? dbAgentHost = null;
        ushort? serverId = null;
        bool inServerConnectionSection = false;

        foreach (string rawLine in await File.ReadAllLinesAsync(path, ct))
        {
            string line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith(";") || line.StartsWith("//"))
            {
                continue;
            }

            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                inServerConnectionSection = line.Equals("[ServerConnection]", StringComparison.OrdinalIgnoreCase);
                continue;
            }

            if (!inServerConnectionSection)
            {
                continue;
            }

            int separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            string key = line[..separatorIndex].Trim();
            string value = line[(separatorIndex + 1)..].Trim().Trim('"');

            switch (key)
            {
                case "AccountServerIP":
                    accountServerHost = value;
                    break;
                case "DBAgentServerIP":
                    dbAgentHost = value;
                    break;
                case "ServerID" when ushort.TryParse(value, out ushort parsedServerId):
                    serverId = parsedServerId;
                    break;
            }
        }

        return new LegacyBootstrapOverrides(accountServerHost, dbAgentHost, serverId);
    }
}