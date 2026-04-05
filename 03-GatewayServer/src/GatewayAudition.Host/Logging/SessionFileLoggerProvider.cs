using Microsoft.Extensions.Logging;

namespace GatewayAudition.Host.Logging;

public sealed class SessionFileLoggerProvider : ILoggerProvider
{
    private readonly SessionFileLogWriter writer;

    public SessionFileLoggerProvider(string filePath)
    {
        writer = new SessionFileLogWriter(filePath);
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new SessionFileLogger(categoryName, writer);
    }

    public void Dispose()
    {
        writer.Dispose();
    }

    private sealed class SessionFileLogger(string categoryName, SessionFileLogWriter writer) : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string message = formatter(state, exception);
            if (string.IsNullOrWhiteSpace(message) && exception is null)
            {
                return;
            }

            writer.WriteLine(logLevel, categoryName, message, exception);
        }
    }

    private sealed class SessionFileLogWriter : IDisposable
    {
        private readonly object gate = new();
        private readonly StreamWriter writer;

        public SessionFileLogWriter(string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            writer = new StreamWriter(new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                AutoFlush = true,
            };
        }

        public void WriteLine(LogLevel logLevel, string categoryName, string message, Exception? exception)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            lock (gate)
            {
                writer.Write('[');
                writer.Write(timestamp);
                writer.Write("] ");
                writer.Write(logLevel.ToString().ToLowerInvariant());
                writer.Write(": ");
                writer.Write(categoryName);
                writer.Write(' ');
                writer.WriteLine(message);

                if (exception is not null)
                {
                    writer.Write('[');
                    writer.Write(timestamp);
                    writer.Write("] exception: ");
                    writer.WriteLine(exception);
                }
            }
        }

        public void Dispose()
        {
            lock (gate)
            {
                writer.Dispose();
            }
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}