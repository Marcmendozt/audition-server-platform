using Audition.GameServer.Host.Configuration;
using Xunit;

namespace Audition.GameServer.Tests;

public sealed class RuntimePathLocatorTests
{
    [Fact]
    public void Resolve_CreatesRuntimePaths_UnderProjectRoot()
    {
        var options = new GameServerOptions();

        RuntimePaths runtimePaths = RuntimePathLocator.Resolve(options);

        Assert.True(Directory.Exists(runtimePaths.DataPath));
        Assert.True(Directory.Exists(runtimePaths.LogPath));
        Assert.True(Directory.Exists(runtimePaths.ReportPath));
        Assert.True(Directory.Exists(runtimePaths.ScriptPath));
        Assert.True(Directory.Exists(runtimePaths.SoundPath));
        Assert.StartsWith(runtimePaths.LogPath, runtimePaths.SessionLogFilePath, StringComparison.OrdinalIgnoreCase);
    }
}