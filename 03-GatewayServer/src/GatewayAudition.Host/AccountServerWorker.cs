using GatewayAudition.Application.DTOs;
using GatewayAudition.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace GatewayAudition.Host;

public sealed class AccountServerWorker : BackgroundService
{
    private readonly IAccountServerManager _accountServerManager;
    private readonly GatewaySettings _settings;
    private readonly ILogger<AccountServerWorker> _logger;

    public AccountServerWorker(
        IAccountServerManager accountServerManager,
        IOptions<GatewaySettings> settings,
        ILogger<AccountServerWorker> logger)
    {
        _accountServerManager = accountServerManager;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Account server connector starting...");

        _accountServerManager.Initialize(
            _settings.AccountServerIp, _settings.AccountServerPort);

        await _accountServerManager.ConnectAsync(stoppingToken);
    }
}
