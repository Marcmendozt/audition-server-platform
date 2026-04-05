using System.Net;
using System.Net.Sockets;
using LoginDBAgent.Application.Services;
using LoginDBAgent.Host.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LoginDBAgent.Host.Network;

public sealed class LoginDbAgentTcpWorker(
    ILoginDbAgentService service,
    PacketDispatcher dispatcher,
    IOptions<LoginDbAgentOptions> options,
    ILogger<LoginDbAgentTcpWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var port = options.Value.Port;

        logger.LogInformation("=== Audition LoginDBAgent v2.0 (Clean Architecture) ===");

        try
        {
            await service.LoadLevelQuestDataAsync(stoppingToken);
            var quests = service.GetCachedLevelQuests();
            logger.LogInformation("Loaded {Count} level quests from database", quests.Count);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not load level quest data at startup");
        }

        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        logger.LogInformation("Listening on port {Port}", port);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var client = await listener.AcceptTcpClientAsync(stoppingToken);
                logger.LogDebug("Connection from {RemoteEndPoint}", client.Client.RemoteEndPoint);
                _ = HandleClientAsync(client, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error accepting connection");
            }
        }

        listener.Stop();
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken ct)
    {
        try
        {
            await using var stream = client.GetStream();
            var buffer = new byte[4096];

            int bytesRead = await stream.ReadAsync(buffer, ct);
                if (bytesRead == 0)
                {
                    logger.LogDebug("Health probe (0-byte read) from AccountServer");
                    return;
                }

                logger.LogInformation("AccountServer connected ({Bytes} bytes)", bytesRead);

                do
                {
                    var data = buffer.AsMemory(0, bytesRead).ToArray();
                    var response = await dispatcher.DispatchAsync(data, ct);

                    if (response is not null)
                    {
                        await stream.WriteAsync(response, ct);
                    }

                    bytesRead = await stream.ReadAsync(buffer, ct);
                } while (bytesRead > 0 && !ct.IsCancellationRequested);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling client connection");
        }
        finally
        {
            client.Dispose();
            logger.LogDebug("Connection closed");
        }
    }
}
