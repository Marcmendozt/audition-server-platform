using System.Buffers.Binary;
using System.Net.Sockets;
using AccountServer.Host.Configuration;
using AccountServer.Host.Contracts;
using Microsoft.Extensions.Options;

namespace AccountServer.Host.Services;

public sealed class DbAgentPayClientService(IOptions<AccountServerOptions> options) : IDbAgentPayClient
{
    private readonly object sync = new();
    private DbAgentStatusSnapshot status = new(false, false, "127.0.0.1:25525", null, null);

    public DbAgentStatusSnapshot GetStatus()
    {
        lock (sync)
        {
            return status;
        }
    }

    public async Task<DbAgentStatusSnapshot> ProbeAsync(CancellationToken cancellationToken)
    {
        var settings = options.Value.DbAgentPay;
        if (!settings.Enabled)
        {
            var disabledSnapshot = new DbAgentStatusSnapshot(false, false, $"{settings.Host}:{settings.Port}", null, "disabled");
            UpdateStatus(disabledSnapshot);
            return disabledSnapshot;
        }

        using var tcpClient = new TcpClient();
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(settings.ConnectTimeoutMs);

        try
        {
            await tcpClient.ConnectAsync(settings.Host, settings.Port, timeoutCts.Token);
            var snapshot = new DbAgentStatusSnapshot(true, true, $"{settings.Host}:{settings.Port}", DateTime.UtcNow, null);
            UpdateStatus(snapshot);
            return snapshot;
        }
        catch (Exception exception) when (exception is SocketException or OperationCanceledException)
        {
            var snapshot = new DbAgentStatusSnapshot(true, false, $"{settings.Host}:{settings.Port}", status.LastConnectedAtUtc, exception.Message);
            UpdateStatus(snapshot);
            return snapshot;
        }
    }

    public Task<DbAgentPayOperationResult> SendHeartbeatAsync(CancellationToken cancellationToken)
    {
        return SendAsync("Heartbeat", 4, [4], cancellationToken);
    }

    public Task<DbAgentPayOperationResult> SendAccountInfoAsync(DbAgentPayAccountInfoRequest request, CancellationToken cancellationToken)
    {
        return SendAsync("AccountInfo", 1, BuildOpcodeWithUInt32(1, request.UserSerial), cancellationToken);
    }

    public Task<DbAgentPayOperationResult> SendPurchaseAsync(DbAgentPayPurchaseRequest request, CancellationToken cancellationToken)
    {
        var payload = new byte[13];
        payload[0] = 0;
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(1, 4), request.UserSerial);
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(5, 4), request.ItemId);
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(9, 4), request.Days);
        return SendAsync("Purchase", 0, payload, cancellationToken);
    }

    public Task<DbAgentPayOperationResult> SendGameResultsAsync(DbAgentPayGameResultsRequest request, CancellationToken cancellationToken)
    {
        var payload = new byte[13];
        payload[0] = 3;
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(1, 4), request.UserSerial);
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(5, 4), request.ExperienceGain);
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(9, 4), request.DenGain);
        return SendAsync("GameResults", 3, payload, cancellationToken);
    }

    public Task<DbAgentPayOperationResult> SendLevelQuestLogAsync(DbAgentPayLevelQuestLogRequest request, CancellationToken cancellationToken)
    {
        var payload = new byte[24];
        payload[0] = 6;
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(1, 4), request.UserSerial);
        BinaryPrimitives.WriteInt16LittleEndian(payload.AsSpan(5, 2), request.ProcLevel);
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(7, 4), request.BeforeDen);
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(11, 4), request.AfterDen);
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(15, 4), request.BeforeExp);
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(19, 4), request.AfterExp);
        payload[23] = request.Pass;
        return SendAsync("LevelQuestLog", 6, payload, cancellationToken);
    }

    private async Task<DbAgentPayOperationResult> SendAsync(string operation, int opcode, byte[] payload, CancellationToken cancellationToken)
    {
        var settings = options.Value.DbAgentPay;
        if (!settings.Enabled)
        {
            throw new InvalidOperationException("DBAgent.Pay esta deshabilitado en la configuracion.");
        }

        using var tcpClient = new TcpClient();
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(settings.ConnectTimeoutMs);

        try
        {
            await tcpClient.ConnectAsync(settings.Host, settings.Port, timeoutCts.Token);
            await using var stream = tcpClient.GetStream();
            await stream.WriteAsync(payload, cancellationToken);
            await stream.FlushAsync(cancellationToken);

            var snapshot = new DbAgentStatusSnapshot(true, true, $"{settings.Host}:{settings.Port}", DateTime.UtcNow, null);
            UpdateStatus(snapshot);

            return new DbAgentPayOperationResult(true, operation, opcode, payload.Length, snapshot.Endpoint);
        }
        catch (Exception exception) when (exception is SocketException or OperationCanceledException)
        {
            var snapshot = new DbAgentStatusSnapshot(true, false, $"{settings.Host}:{settings.Port}", status.LastConnectedAtUtc, exception.Message);
            UpdateStatus(snapshot);
            throw new InvalidOperationException($"No se pudo enviar {operation} a DBAgent.Pay: {exception.Message}", exception);
        }
    }

    private static byte[] BuildOpcodeWithUInt32(byte opcode, uint value)
    {
        var payload = new byte[5];
        payload[0] = opcode;
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(1, 4), value);
        return payload;
    }

    private void UpdateStatus(DbAgentStatusSnapshot snapshot)
    {
        lock (sync)
        {
            status = snapshot;
        }
    }
}