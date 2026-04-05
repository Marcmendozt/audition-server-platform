using Audition.GameServer.Host.Configuration;
using Audition.GameServer.Host.Services;
using Xunit;

namespace Audition.GameServer.Tests;

public sealed class ExceptionReportWriterTests
{
    [Fact]
    public void Write_CreatesReportFile()
    {
        string tempRoot = Directory.CreateTempSubdirectory().FullName;

        try
        {
            string reportPath = Path.Combine(tempRoot, "Report");
            Directory.CreateDirectory(reportPath);
            var runtimePaths = new RuntimePaths(
                tempRoot,
                Path.Combine(tempRoot, "Data"),
                Path.Combine(tempRoot, "log"),
                reportPath,
                Path.Combine(tempRoot, "script"),
                Path.Combine(tempRoot, "sound"),
                Path.Combine(tempRoot, "log", "test.log"));
            var writer = new ExceptionReportWriter(runtimePaths);

            string filePath = writer.Write(new InvalidOperationException("boom"), "unit-test");

            Assert.True(File.Exists(filePath));
            string content = File.ReadAllText(filePath);
            Assert.Contains("unit-test", content);
            Assert.Contains("boom", content);
        }
        finally
        {
            Directory.Delete(tempRoot, recursive: true);
        }
    }
}