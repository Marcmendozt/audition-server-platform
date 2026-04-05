namespace Audition.GameServer.Host.Services;

public sealed class ExceptionReportService(
    ILogger<ExceptionReportService> logger,
    ExceptionReportWriter reportWriter) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
        TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
        return Task.CompletedTask;
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        if (args.ExceptionObject is Exception exception)
        {
            WriteReport(exception, "AppDomain.CurrentDomain.UnhandledException");
        }
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        WriteReport(args.Exception, "TaskScheduler.UnobservedTaskException");
    }

    private void WriteReport(Exception exception, string source)
    {
        try
        {
            string path = reportWriter.Write(exception, source);
            logger.LogError(exception, "Unhandled exception report written to {ReportPath}", path);
        }
        catch (Exception reportException)
        {
            logger.LogError(reportException, "Failed to write exception report for source {Source}", source);
        }
    }
}