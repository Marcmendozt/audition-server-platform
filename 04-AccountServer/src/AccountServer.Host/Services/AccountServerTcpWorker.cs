using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AccountServer.Host.Configuration;
using AccountServer.Host.Contracts;
using Microsoft.Extensions.Options;

namespace AccountServer.Host.Services;

public sealed class AccountServerTcpWorker(
    ILogger<AccountServerTcpWorker> logger,
    IOptions<AccountServerOptions> options,
    BinaryPacketCodec binaryPacketCodec,
    SessionRuntimeManager sessionRuntimeManager,
    AccountRequestProcessor requestProcessor) : BackgroundService
{
    private readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;
        var ipAddress = IPAddress.TryParse(settings.Host, out var parsedAddress)
            ? parsedAddress
            : IPAddress.Loopback;

        logger.LogInformation("Account Server Build Version[{BuildVersion}]...", settings.BuildVersion);
        logger.LogInformation("[Board Item Mode] {Mode}", settings.BoardItemMode ? "On" : "Off");
        logger.LogInformation("Account Server Ver {Version} Launching.", settings.ServerVersion);

        var listeners = settings.GetEnabledListeners()
            .Select(listener => new ListenerRuntime(listener, new TcpListener(ipAddress, listener.Port)))
            .ToArray();

        foreach (var listener in listeners)
        {
            listener.TcpListener.Start();
            logger.LogInformation(
                "{ListenerName} Server escuchando en {Host}:{Port}",
                listener.Options.Name,
                settings.Host,
                listener.Options.Port);
        }

        try
        {
            var acceptTasks = listeners
                .Select(listener => AcceptLoopAsync(listener, stoppingToken))
                .ToArray();

            await Task.WhenAll(acceptTasks);
        }
        finally
        {
            foreach (var listener in listeners)
            {
                listener.TcpListener.Stop();
            }
        }
    }

    private async Task AcceptLoopAsync(ListenerRuntime listener, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            TcpClient client;

            try
            {
                client = await listener.TcpListener.AcceptTcpClientAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ObjectDisposedException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (SocketException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            _ = HandleClientAsync(listener.Options.Name, client, stoppingToken);
        }
    }

    private async Task HandleClientAsync(string listenerName, TcpClient client, CancellationToken stoppingToken)
    {
        using var tcpClient = client;
        var remoteEndPoint = tcpClient.Client.RemoteEndPoint?.ToString() ?? "desconocido";
        var settings = options.Value;

        try
        {
            await using var stream = tcpClient.GetStream();

            logger.LogInformation(
                "{ListenerName} client connected from {RemoteEndPoint}",
                listenerName,
                remoteEndPoint);

            using var sessionLease = sessionRuntimeManager.Attach(listenerName, remoteEndPoint, settings.SessionBufferSize);

            var firstByteBuffer = new byte[1];
            var firstByteRead = await stream.ReadAsync(firstByteBuffer, stoppingToken);
            if (firstByteRead == 0)
            {
                return;
            }

            var prefixedStream = new PrefixedReadStream(stream, firstByteBuffer.AsMemory(0, 1));
            if (binaryPacketCodec.LooksLikeBinary(firstByteBuffer[0]))
            {
                await ProcessBinaryClientAsync(listenerName, remoteEndPoint, prefixedStream, stoppingToken);
            }
            else
            {
                await ProcessJsonClientAsync(listenerName, remoteEndPoint, prefixedStream, stream, stoppingToken);
            }
        }
        catch (IOException exception)
        {
            logger.LogWarning(
                exception,
                "Conexion cerrada con error en {ListenerName} para {RemoteEndPoint}",
                listenerName,
                remoteEndPoint);
        }
        finally
        {
            logger.LogInformation(
                "{ListenerName} client disconnected {RemoteEndPoint}",
                listenerName,
                remoteEndPoint);
        }
    }

    private async Task ProcessJsonClientAsync(
        string listenerName,
        string remoteEndPoint,
        Stream prefixedStream,
        Stream outputStream,
        CancellationToken stoppingToken)
    {
        using var reader = new StreamReader(prefixedStream, Encoding.UTF8, leaveOpen: true);
        while (!stoppingToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            if (line is null)
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            AccountServerResponse response;

            try
            {
                var request = JsonSerializer.Deserialize<AccountServerRequest>(line, serializerOptions)
                    ?? throw new JsonException("Solicitud vacia.");

                response = await requestProcessor.ProcessAsync(request, listenerName, stoppingToken);
            }
            catch (JsonException exception)
            {
                response = new AccountServerResponse(false, $"Solicitud invalida: {exception.Message}");
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Error procesando solicitud en {ListenerName} para {RemoteEndPoint}",
                    listenerName,
                    remoteEndPoint);
                response = new AccountServerResponse(false, "Error interno del servidor.");
            }

            var json = JsonSerializer.Serialize(response, serializerOptions);
            var responseBytes = Encoding.UTF8.GetBytes(json + Environment.NewLine);
            await outputStream.WriteAsync(responseBytes, stoppingToken);
            await outputStream.FlushAsync(stoppingToken);
        }
    }

    private async Task ProcessBinaryClientAsync(
        string listenerName,
        string remoteEndPoint,
        Stream prefixedStream,
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            BinaryPacketRequest? request;

            try
            {
                request = await binaryPacketCodec.ReadRequestAsync(prefixedStream, stoppingToken);
            }
            catch (IOException)
            {
                break;
            }
            catch (SocketException)
            {
                break;
            }
            catch (JsonException exception)
            {
                logger.LogWarning(
                    exception,
                    "Packet binario invalido en {ListenerName} para {RemoteEndPoint}",
                    listenerName,
                    remoteEndPoint);
                break;
            }

            if (request is null)
            {
                break;
            }

            try
            {
                await requestProcessor.ProcessBinaryAsync(
                    request,
                    listenerName,
                    stoppingToken);
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Error procesando packet en {ListenerName} para {RemoteEndPoint}",
                    listenerName,
                    remoteEndPoint);
                break;
            }
        }
    }

    private sealed record ListenerRuntime(ListenerOptions Options, TcpListener TcpListener);
}