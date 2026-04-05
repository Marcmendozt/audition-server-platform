namespace Audition.GameServer.Host.Configuration;

public static class RuntimePathLocator
{
    public static RuntimePaths Resolve(GameServerOptions options)
    {
        string rootPath = ResolveProjectRoot();
        string dataPath = ResolveDirectoryPath(options.RuntimePaths.DataPath, rootPath);
        string logPath = ResolveDirectoryPath(options.RuntimePaths.LogPath, rootPath);
        string reportPath = ResolveDirectoryPath(options.RuntimePaths.ReportPath, rootPath);
        string scriptPath = ResolveDirectoryPath(options.RuntimePaths.ScriptPath, rootPath);
        string soundPath = ResolveDirectoryPath(options.RuntimePaths.SoundPath, rootPath);
        string logFileName = $"AuditionGameServer_{DateTime.Now:yyyy_MM_dd_HH_mm_ss_fff}.log";
        string sessionLogFilePath = Path.Combine(logPath, logFileName);

        return new RuntimePaths(
            rootPath,
            dataPath,
            logPath,
            reportPath,
            scriptPath,
            soundPath,
            sessionLogFilePath);
    }

    public static string ResolveExistingPath(string configuredPath, bool expectDirectory)
    {
        if (Path.IsPathRooted(configuredPath))
        {
            return configuredPath;
        }

        foreach (string root in EnumerateRoots())
        {
            string candidate = Path.GetFullPath(Path.Combine(root, configuredPath));
            bool exists = expectDirectory ? Directory.Exists(candidate) : File.Exists(candidate);
            if (exists)
            {
                return candidate;
            }
        }

        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configuredPath));
    }

    public static string ResolveDirectoryPath(string configuredPath, string rootPath)
    {
        string path = Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.GetFullPath(Path.Combine(rootPath, configuredPath));

        Directory.CreateDirectory(path);
        return path;
    }

    private static string ResolveProjectRoot()
    {
        string? candidate = EnumerateRoots()
            .FirstOrDefault(root =>
                File.Exists(Path.Combine(root, "README.md")) &&
                Directory.Exists(Path.Combine(root, "src")) &&
                Directory.Exists(Path.Combine(root, "Data")));

        return candidate ?? Directory.GetCurrentDirectory();
    }

    private static IEnumerable<string> EnumerateRoots()
    {
        foreach (string seed in new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() })
        {
            DirectoryInfo? current = new(seed);
            while (current is not null)
            {
                yield return current.FullName;
                current = current.Parent;
            }
        }
    }
}