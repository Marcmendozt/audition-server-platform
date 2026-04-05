using GatewayAudition.Infrastructure.Networking;

namespace GatewayAudition.Host;

public sealed class SessionCollectionWorker : BackgroundService
{
    private readonly SessionCollectionService _collectionService;
    private readonly ILogger<SessionCollectionWorker> _logger;

    public SessionCollectionWorker(
        SessionCollectionService collectionService,
        ILogger<SessionCollectionWorker> logger)
    {
        _collectionService = collectionService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Session collection service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(60_000, stoppingToken); // Every 60 seconds
                _collectionService.CollectStaleSessions();
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during session collection");
            }
        }
    }
}
