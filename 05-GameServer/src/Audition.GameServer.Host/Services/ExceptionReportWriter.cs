using System.Text;
using Audition.GameServer.Host.Configuration;

namespace Audition.GameServer.Host.Services;

public sealed class ExceptionReportWriter(RuntimePaths runtimePaths)
{
    public string Write(Exception exception, string source)
    {
        string reportFilePath = Path.Combine(runtimePaths.ReportPath, $"{DateTime.Now:yyyyMMdd_HHmmssfff}.rpt");
        var builder = new StringBuilder();
        builder.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
        builder.AppendLine($"Source: {source}");
        builder.AppendLine($"Machine: {Environment.MachineName}");
        builder.AppendLine($"Process: {Environment.ProcessPath}");
        builder.AppendLine();
        builder.AppendLine(exception.ToString());

        File.WriteAllText(reportFilePath, builder.ToString(), Encoding.UTF8);
        return reportFilePath;
    }
}