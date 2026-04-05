namespace Audition.GameServer.Host.Configuration;

public sealed record RuntimePaths(
    string RootPath,
    string DataPath,
    string LogPath,
    string ReportPath,
    string ScriptPath,
    string SoundPath,
    string SessionLogFilePath);