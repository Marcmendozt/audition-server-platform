using GatewayAudition.Application.DTOs;
using GatewayAudition.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace GatewayAudition.Host;

public sealed class GatewayWorker : BackgroundService
{
    private readonly IGatewayServer _gatewayServer;
    private readonly ISessionManager _sessionManager;
    private readonly IPacketManager _packetManager;
    private readonly IServerDirectory _serverDirectory;
    private readonly GatewaySettings _settings;
    private readonly ILogger<GatewayWorker> _logger;

    public GatewayWorker(
        IGatewayServer gatewayServer,
        ISessionManager sessionManager,
        IPacketManager packetManager,
        IServerDirectory serverDirectory,
        IOptions<GatewaySettings> settings,
        ILogger<GatewayWorker> logger)
    {
        _gatewayServer = gatewayServer;
        _sessionManager = sessionManager;
        _packetManager = packetManager;
        _serverDirectory = serverDirectory;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("=== Audition Gateway Server ===");
        _logger.LogInformation("Initializing...");

        _sessionManager.Initialize(_settings.MaxSessions, _settings.SessionBufferSize);
        _packetManager.Initialize(_settings.MaxPackets, _settings.PacketBufferSize);
        _serverDirectory.LoadConfig();

        _logger.LogInformation("Sessions: {Max}, Buffer: {Size}",
            _settings.MaxSessions, _settings.SessionBufferSize);

        await _gatewayServer.StartAsync(_settings.ListenPort, stoppingToken);

        _logger.LogInformation("Gateway server running on port {Port}", _settings.ListenPort);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Shutting down gateway server...");
        }

        await _gatewayServer.StopAsync(stoppingToken);
    }
}
