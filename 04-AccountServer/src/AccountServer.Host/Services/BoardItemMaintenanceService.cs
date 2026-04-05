using AccountServer.Application.Abstractions;
using AccountServer.Host.Configuration;
using Microsoft.Extensions.Options;

namespace AccountServer.Host.Services;

public sealed class BoardItemMaintenanceService(
    IBoardItemRepository boardItemRepository,
    IOptions<AccountServerOptions> options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;

        while (!stoppingToken.IsCancellationRequested)
        {
            if (settings.BoardItemMode)
            {
                var threshold = DateTime.UtcNow.AddMinutes(-settings.BoardItemRetentionMinutes);
                await boardItemRepository.PruneExpiredAsync(threshold, stoppingToken);
            }

            await Task.Delay(TimeSpan.FromSeconds(settings.BoardItemSweepIntervalSeconds), stoppingToken);
        }
    }
}