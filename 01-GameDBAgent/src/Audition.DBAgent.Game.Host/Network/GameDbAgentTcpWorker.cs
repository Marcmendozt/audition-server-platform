using System.Net;
using System.Net.Sockets;
using Audition.DBAgent.Game.Application.Services;
using Audition.DBAgent.Game.Host.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Audition.DBAgent.Game.Host.Network;

public sealed class GameDbAgentTcpWorker(
    IGameDbAgentService service,
    PacketDispatcher dispatcher,
    IOptions<GameDbAgentOptions> options,
    ILogger<GameDbAgentTcpWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var port = options.Value.Port;

        logger.LogInformation("=== Audition DBAgentGame v2.0 (Clean Architecture) ===");

        // Load level quest data at startup (setLevelQuestInfo from native)
        try
        {
            await service.LoadLevelQuestDataAsync(stoppingToken);
            var quests = service.GetCachedLevelQuests();
            logger.LogInformation("Loaded {Count} level quests from database", quests.Count);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not load level quest data at startup. Quest features will be unavailable");
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
            var buffer = new byte[8192];
            var framedBuffer = new List<byte>();
            bool? usesLengthPrefix = null;

            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, ct)) > 0 && !ct.IsCancellationRequested)
            {
                var chunk = buffer.AsSpan(0, bytesRead).ToArray();

                if (usesLengthPrefix is null)
                {
                    usesLengthPrefix = PacketFraming.UsesLengthPrefix(chunk);
                    logger.LogInformation(
                        "Legacy DBAgent client connected ({Bytes} bytes, protocol: {Protocol})",
                        bytesRead,
                        usesLengthPrefix.Value ? "length-prefixed" : "raw");
                }

                if (!usesLengthPrefix.Value)
                {
                    var responses = await dispatcher.DispatchAsync(chunk, ct);
                    await WriteResponsesAsync(stream, responses, ct);
                    continue;
                }

                framedBuffer.AddRange(chunk);
                while (PacketFraming.TryExtractPayload(framedBuffer, out var payload))
                {
                    var responses = await dispatcher.DispatchAsync(payload, ct);
                    await WriteResponsesAsync(stream, responses, ct);
                }
            }

            if (usesLengthPrefix is null)
            {
                logger.LogDebug("Health probe (0-byte read)");
                return;
            }

            if (usesLengthPrefix.Value && framedBuffer.Count > 0)
            {
                logger.LogWarning("Connection closed with {Bytes} buffered bytes from an incomplete length-prefixed packet", framedBuffer.Count);
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Graceful shutdown
        }
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

    private static async Task WriteResponsesAsync(NetworkStream stream, IReadOnlyList<byte[]> responses, CancellationToken ct)
    {
        foreach (var response in responses)
        {
            await stream.WriteAsync(response, ct);
        }
    }
}
