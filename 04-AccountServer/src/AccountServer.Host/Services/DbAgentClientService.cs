using System.Buffers.Binary;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using AccountServer.Host.Configuration;
using AccountServer.Host.Contracts;
using Microsoft.Extensions.Options;

namespace AccountServer.Host.Services;

public sealed class DbAgentClientService(
    ILogger<DbAgentClientService> logger,
    IOptions<AccountServerOptions> options) : BackgroundService, IDbAgentClient
{
    private readonly object sync = new();
    private static readonly Encoding StringEncoding = Encoding.ASCII;
    private DbAgentStatusSnapshot status = new(false, false, "127.0.0.1:5500", null, null);

    public DbAgentStatusSnapshot GetStatus()
    {
        lock (sync)
        {
            return status;
        }
    }

    public async Task<DbAgentLoginResult> RequestLoginChinaAsync(DbAgentLoginRequest request, CancellationToken cancellationToken)
    {
        var settings = options.Value.DbAgent;
        if (!settings.Enabled)
        {
            throw new InvalidOperationException("DBAgent esta deshabilitado en la configuracion.");
        }

        ValidateLoginRequest(request);

        using var tcpClient = new TcpClient();
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(settings.ConnectTimeoutMs);
        var timeoutToken = timeoutCts.Token;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            logger.LogInformation("[DBAGENT LOGIN] Request start | UserId={UserId} | Host={Host}:{Port}",
                request.UserId,
                settings.Host,
                settings.Port);

            await tcpClient.ConnectAsync(settings.Host, settings.Port, timeoutToken);
            await using var stream = tcpClient.GetStream();

            var relayContext = ResolveRelayContext(request);
            var packet = DbAgentProtocol.BuildRequestLoginChina(
                relayContext,
                request.UserId,
                request.Password,
                request.ExtraField3,
                request.ExtraField4);

            await stream.WriteAsync(packet, timeoutToken);
            await stream.FlushAsync(timeoutToken);

            var header = new byte[4];
            await ReadExactAsync(stream, header, timeoutToken);
            var packetLength = BinaryPrimitives.ReadUInt16LittleEndian(header.AsSpan(0, 2));
            var opCode = BinaryPrimitives.ReadUInt16LittleEndian(header.AsSpan(2, 2));

            if (packetLength < 4)
            {
                throw new InvalidOperationException($"Respuesta invalida de DBAgent. Length={packetLength}");
            }

            var responseBuffer = new byte[packetLength];
            Array.Copy(header, responseBuffer, header.Length);
            await ReadExactAsync(stream, responseBuffer.AsMemory(4, packetLength - 4), timeoutToken);

            var region = (byte)(opCode >> 8);
            int offset = 4;

            offset += 12; // relay context echoed from request
            var internalResult = responseBuffer.Length > offset ? responseBuffer[offset++] : (byte)1;
            var userId = ReadPrefixedAscii(responseBuffer, ref offset);

            uint userSerial = 0;
            uint userExperience = 0;
            byte loginFlag = 0;
            string userNickname = string.Empty;

            if (internalResult == 0)
            {
                userNickname = ReadPrefixedAscii(responseBuffer, ref offset);

                if (offset + 4 <= responseBuffer.Length)
                {
                    userExperience = BinaryPrimitives.ReadUInt32LittleEndian(responseBuffer.AsSpan(offset, 4));
                    offset += 4;
                }

                if (offset + 4 <= responseBuffer.Length)
                {
                    userSerial = BinaryPrimitives.ReadUInt32LittleEndian(responseBuffer.AsSpan(offset, 4));
                    offset += 4;
                }

                if (offset < responseBuffer.Length)
                {
                    loginFlag = responseBuffer[offset++];
                }
            }

            var success = internalResult == 0 && userSerial > 0 && loginFlag == 1;

            logger.LogInformation("[DBAGENT LOGIN] Request end | UserId={UserId} | Success={Success} | UserSN={UserSN} | ElapsedMs={ElapsedMs}",
                request.UserId,
                success,
                userSerial,
                stopwatch.ElapsedMilliseconds);

            return new DbAgentLoginResult(success, packetLength, opCode, region, internalResult, userSerial, loginFlag, userId, userNickname, userExperience);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("[DBAGENT LOGIN] Timeout | UserId={UserId} | Host={Host}:{Port} | ElapsedMs={ElapsedMs}",
                request.UserId,
                settings.Host,
                settings.Port,
                stopwatch.ElapsedMilliseconds);
            throw new InvalidOperationException($"DBAgent no respondio a tiempo para {request.UserId}.");
        }
        catch (SocketException exception)
        {
            logger.LogWarning(exception, "[DBAGENT LOGIN] Socket error | UserId={UserId} | Host={Host}:{Port}",
                request.UserId,
                settings.Host,
                settings.Port);
            throw new InvalidOperationException($"No se pudo conectar al DBAgent {settings.Host}:{settings.Port}.");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value.DbAgent;
        var lastProbeConnected = false;
        UpdateStatus(new DbAgentStatusSnapshot(
            settings.Enabled,
            false,
            $"{settings.Host}:{settings.Port}",
            null,
            null));

        if (!settings.Enabled)
        {
            logger.LogInformation("DBAgentClientManager Ready!");
            await Task.Delay(Timeout.Infinite, stoppingToken);
            return;
        }

        logger.LogInformation("DBAgentClientManager Ready!");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var tcpClient = new TcpClient();
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            timeoutCts.CancelAfter(settings.ConnectTimeoutMs);

            try
            {
                await tcpClient.ConnectAsync(settings.Host, settings.Port, timeoutCts.Token);
                UpdateStatus(new DbAgentStatusSnapshot(
                    true,
                    true,
                    $"{settings.Host}:{settings.Port}",
                    DateTime.UtcNow,
                    null));

                if (!lastProbeConnected)
                {
                    logger.LogInformation("DBAgentClientManager Connected to {Host}:{Port}", settings.Host, settings.Port);
                }

                lastProbeConnected = true;
            }
            catch (OperationCanceledException) when (!stoppingToken.IsCancellationRequested)
            {
                UpdateStatus(new DbAgentStatusSnapshot(
                    true,
                    false,
                    $"{settings.Host}:{settings.Port}",
                    status.LastConnectedAtUtc,
                    "timeout"));

                if (lastProbeConnected)
                {
                    logger.LogWarning("DBAgentClientManager timeout connecting to {Host}:{Port}", settings.Host, settings.Port);
                }

                lastProbeConnected = false;
            }
            catch (SocketException exception)
            {
                UpdateStatus(new DbAgentStatusSnapshot(
                    true,
                    false,
                    $"{settings.Host}:{settings.Port}",
                    status.LastConnectedAtUtc,
                    exception.Message));

                if (lastProbeConnected)
                {
                    logger.LogWarning(exception, "DBAgentClientManager failed to connect to {Host}:{Port}", settings.Host, settings.Port);
                }

                lastProbeConnected = false;
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private void UpdateStatus(DbAgentStatusSnapshot snapshot)
    {
        lock (sync)
        {
            status = snapshot;
        }
    }

    private static async Task ReadExactAsync(Stream stream, Memory<byte> buffer, CancellationToken cancellationToken)
    {
        var readTotal = 0;
        while (readTotal < buffer.Length)
        {
            var read = await stream.ReadAsync(buffer[readTotal..], cancellationToken);
            if (read == 0)
            {
                throw new EndOfStreamException("El stream de DBAgent termino antes de completar la respuesta.");
            }

            readTotal += read;
        }
    }

    private static string ReadFixedAscii(ReadOnlySpan<byte> buffer)
    {
        var terminatorIndex = buffer.IndexOf((byte)0);
        var effectiveLength = terminatorIndex >= 0 ? terminatorIndex : buffer.Length;
        return StringEncoding.GetString(buffer.Slice(0, effectiveLength)).Trim();
    }

    private static string ReadPrefixedAscii(ReadOnlySpan<byte> buffer, ref int offset)
    {
        if (offset >= buffer.Length)
        {
            return string.Empty;
        }

        int length = buffer[offset++];
        if (length <= 0 || offset + length > buffer.Length)
        {
            return string.Empty;
        }

        string value = StringEncoding.GetString(buffer.Slice(offset, length));
        offset += length;
        return value;
    }

    private static ReadOnlySpan<byte> ResolveRelayContext(DbAgentLoginRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.RelayContextBase64))
        {
            var relayContext = Convert.FromBase64String(request.RelayContextBase64);
            if (relayContext.Length != 12)
            {
                throw new InvalidOperationException("RelayContextBase64 debe representar exactamente 12 bytes.");
            }

            return relayContext;
        }

        var fallback = new byte[12];
        BinaryPrimitives.WriteInt32LittleEndian(fallback.AsSpan(0, 4), request.SocketHandle);
        return fallback;
    }

    private static void ValidateLoginRequest(DbAgentLoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            throw new InvalidOperationException("UserId es obligatorio para RequestLoginChina.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new InvalidOperationException("Password es obligatorio para RequestLoginChina.");
        }
    }
}