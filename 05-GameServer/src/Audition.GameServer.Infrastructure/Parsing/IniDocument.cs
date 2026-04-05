using System.Diagnostics.CodeAnalysis;

namespace Audition.GameServer.Infrastructure.Parsing;

public sealed class IniDocument
{
    private readonly List<IniSection> sections;

    private IniDocument(List<IniSection> sections)
    {
        this.sections = sections;
    }

    public IReadOnlyList<IniSection> Sections => sections;

    public static async Task<IniDocument> LoadAsync(string path, CancellationToken ct)
    {
        var sections = new List<IniSection>();
        IniSection? current = null;

        foreach (string rawLine in await File.ReadAllLinesAsync(path, ct))
        {
            string line = StripComments(rawLine).Trim();
            if (line.Length == 0)
            {
                continue;
            }

            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                current = new IniSection(line[1..^1].Trim(), new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                sections.Add(current);
                continue;
            }

            if (current is null)
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
            current.Properties[key] = value;
        }

        return new IniDocument(sections);
    }

    public bool TryGetSection(string name, [NotNullWhen(true)] out IniSection? section)
    {
        section = sections.FirstOrDefault(entry => entry.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return section is not null;
    }

    private static string StripComments(string rawLine)
    {
        bool inQuotes = false;

        for (int index = 0; index < rawLine.Length - 1; index++)
        {
            char current = rawLine[index];
            char next = rawLine[index + 1];

            if (current == '"')
            {
                inQuotes = !inQuotes;
            }

            if (!inQuotes && current == '/' && next == '/')
            {
                return rawLine[..index];
            }
        }

        return rawLine;
    }
}

public sealed class IniSection(string name, Dictionary<string, string> properties)
{
    public string Name { get; } = name;

    public Dictionary<string, string> Properties { get; } = properties;

    public int GetInt(string key, int defaultValue = 0)
    {
        return Properties.TryGetValue(key, out string? value) && int.TryParse(value, out int parsed)
            ? parsed
            : defaultValue;
    }

    public string GetString(string key, string defaultValue = "")
    {
        return Properties.TryGetValue(key, out string? value)
            ? value
            : defaultValue;
    }
}