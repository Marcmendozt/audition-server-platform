using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace AccountServer.Host.Logging;

public sealed class LegacyConsoleFormatterOptions : ConsoleFormatterOptions
{
    public string LegacyTimestampFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
}

public sealed class LegacyConsoleFormatter : ConsoleFormatter, IDisposable
{
    public const string FormatterName = "legacy";

    private readonly IDisposable? reloadToken;
    private LegacyConsoleFormatterOptions options;

    public LegacyConsoleFormatter(IOptionsMonitor<LegacyConsoleFormatterOptions> optionsMonitor)
        : base(FormatterName)
    {
        options = optionsMonitor.CurrentValue;
        reloadToken = optionsMonitor.OnChange(updatedOptions => options = updatedOptions);
    }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        if (string.IsNullOrEmpty(message) && logEntry.Exception is null)
        {
            return;
        }

        var timestamp = DateTime.Now.ToString(options.LegacyTimestampFormat);
        textWriter.Write('[');
        textWriter.Write(timestamp);
        textWriter.Write(']');

        if (!string.IsNullOrEmpty(message))
        {
            textWriter.WriteLine(message);
        }
        else
        {
            textWriter.WriteLine();
        }

        if (logEntry.Exception is not null)
        {
            textWriter.Write('[');
            textWriter.Write(timestamp);
            textWriter.Write(']');
            textWriter.WriteLine(logEntry.Exception.ToString());
        }
    }

    public void Dispose()
    {
        reloadToken?.Dispose();
    }
}