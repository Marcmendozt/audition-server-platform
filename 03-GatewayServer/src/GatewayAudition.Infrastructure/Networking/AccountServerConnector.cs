using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
using GatewayAudition.Application.DTOs;
using GatewayAudition.Domain.Entities;
using GatewayAudition.Domain.Interfaces;
using GatewayAudition.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GatewayAudition.Infrastructure.Networking;

public sealed class AccountServerConnector : IAccountServerManager
{
    private readonly ILogger<AccountServerConnector> _logger;
    private readonly GatewaySettings _settings;
    private readonly IServerDirectory _serverDirectory;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };
    private string _serverIp = string.Empty;
    private ushort _serverPort;

    public bool IsServerStarted { get; private set; }
    public int ActiveConnectionCount => IsServerStarted ? 1 : 0;

    public AccountServerConnector(
        IOptions<GatewaySettings> settings,
        IServerDirectory serverDirectory,
        ILogger<AccountServerConnector> logger)
    {
        _settings = settings.Value;
        _serverDirectory = serverDirectory;
        _logger = logger;
    }

    public void Initialize(string serverIp, ushort serverPort)
    {
        _serverIp = serverIp;
        _serverPort = serverPort;
    }

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var wasStarted = IsServerStarted;
                if (!wasStarted)
                {
                    await RegisterGatewayAsync(cancellationToken);
                    IsServerStarted = true;
                    _logger.LogInformation("Connected to account server {Ip}:{Port}", _serverIp, _serverPort);
                }

                await RefreshDirectoryAsync(cancellationToken);
                IsServerStarted = true;

                await Task.Delay(_settings.AccountServerSyncIntervalMs, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                IsServerStarted = false;
                _logger.LogWarning("Account server synchronization failed: {Message}. Retrying in {Delay}ms...",
                    ex.Message,
                    _settings.AccountServerSyncIntervalMs);

                try
                {
                    await Task.Delay(_settings.AccountServerSyncIntervalMs, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    public async Task<AccountServerLoginResult> RequestLoginAsync(
        User user,
        string username,
        string password,
        string clientVersion,
        ushort requestedGameServerId,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        var payload = new
        {
            userId = username,
            password,
            extraField3 = clientVersion,
            extraField4 = user.Session.SessionInfo.IpAddress,
            socketHandle = unchecked((int)user.Session.SessionInfo.Socket),
            gatewayServerId = _settings.ServerId,
            relayContextBase64 = Convert.ToBase64String(user.RelayContext)
        };

        _logger.LogInformation("[ACCOUNT LOGIN] request_login_china start | UserId={UserId} | Account={Ip}:{Port}",
            username,
            _serverIp,
            _serverPort);

        var response = await SendCommandAsync("request_login_china", payload, cancellationToken);
        var data = response.Data.ValueKind is JsonValueKind.Object
            ? response.Data.Deserialize<AccountServerLoginResponseData>(_serializerOptions)
            : null;

        _logger.LogInformation("[ACCOUNT LOGIN] request_login_china end | UserId={UserId} | EnvelopeSuccess={EnvelopeSuccess} | PayloadSuccess={PayloadSuccess} | UserSN={UserSN} | LoginFlag={LoginFlag} | Nickname={Nickname} | Experience={Experience} | ElapsedMs={ElapsedMs}",
            username,
            response.Success,
            data?.Success,
            data?.UserSerial ?? 0,
            data?.LoginFlag ?? 0,
            data?.UserNickname ?? string.Empty,
            data?.UserExperience ?? 0,
            stopwatch.ElapsedMilliseconds);

        if (!response.Success)
        {
            return new AccountServerLoginResult
            {
                Success = false,
                Message = response.Message
            };
        }

        if (data is null)
        {
            return new AccountServerLoginResult
            {
                Success = false,
                Message = "Account server returned an empty login payload"
            };
        }

        if (!data.Success || data.UserSerial == 0 || data.LoginFlag != 1)
        {
            return new AccountServerLoginResult
            {
                Success = false,
                Message = response.Message,
                UserSerial = data.UserSerial,
                LoginFlag = data.LoginFlag,
                Region = data.Region,
                UserNickname = data.UserNickname ?? string.Empty,
                UserExperience = data.UserExperience
            };
        }

        var targetGameServerId = ResolveTargetGameServerId(requestedGameServerId);
        if (targetGameServerId == 0)
        {
            return new AccountServerLoginResult
            {
                Success = false,
                Message = "No hay GameServer activo para abrir la sesion.",
                UserSerial = data.UserSerial,
                LoginFlag = data.LoginFlag,
                Region = data.Region,
                UserNickname = data.UserNickname ?? string.Empty,
                UserExperience = data.UserExperience
            };
        }

        var openSessionPayload = new
        {
            userSerial = data.UserSerial,
            userExperience = 0,
            userId = username,
            clusterId = _settings.ClusterId,
            serverId = targetGameServerId,
            gatewayServerId = _settings.ServerId,
            ipAddress = user.Session.SessionInfo.IpAddress,
            socketHandle = (long)user.Session.SessionInfo.Socket
        };

        stopwatch.Restart();
        _logger.LogInformation("[ACCOUNT LOGIN] open_session start | UserId={UserId} | UserSN={UserSN} | GameServerId={GameServerId}",
            username,
            data.UserSerial,
            targetGameServerId);

        var openSessionResponse = await SendCommandAsync("open_session", openSessionPayload, cancellationToken);
        _logger.LogInformation("[ACCOUNT LOGIN] open_session end | UserId={UserId} | Success={Success} | ElapsedMs={ElapsedMs}",
            username,
            openSessionResponse.Success,
            stopwatch.ElapsedMilliseconds);

        if (!openSessionResponse.Success)
        {
            return new AccountServerLoginResult
            {
                Success = false,
                Message = openSessionResponse.Message,
                UserSerial = data.UserSerial,
                LoginFlag = data.LoginFlag,
                Region = data.Region,
                UserNickname = data.UserNickname ?? string.Empty,
                UserExperience = data.UserExperience,
                GameServerId = targetGameServerId
            };
        }

        var sessionData = openSessionResponse.Data.Deserialize<AccountServerSessionResponseData>(_serializerOptions);

        return new AccountServerLoginResult
        {
            Success = data.Success,
            Message = openSessionResponse.Message,
            UserSerial = data.UserSerial,
            LoginFlag = data.LoginFlag,
            Region = data.Region,
            UserNickname = data.UserNickname ?? string.Empty,
            UserExperience = data.UserExperience,
            SessionId = sessionData?.SessionId,
            GameServerId = targetGameServerId
        };
    }

    public async Task CloseSessionAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        await SendCommandAsync("close_session", new { sessionId }, cancellationToken);
    }

    private async Task RegisterGatewayAsync(CancellationToken cancellationToken)
    {
        var payload = new
        {
            serverId = _settings.ServerId,
            clusterId = _settings.ClusterId,
            name = _settings.ServerName,
            level = _settings.ServerLevel,
            ipAddress = _settings.PublicIp,
            port = _settings.ListenPort,
            maxUserCount = _settings.MaxUserCount,
            version = _settings.ServerVersion,
            status = "Online"
        };

        var response = await SendCommandAsync("register_gateway", payload, cancellationToken);
        if (!response.Success)
        {
            throw new InvalidOperationException($"Gateway registration failed: {response.Message}");
        }
    }

    private async Task RefreshDirectoryAsync(CancellationToken cancellationToken)
    {
        var response = await SendCommandAsync("list_directory", new { }, cancellationToken);
        if (!response.Success)
        {
            throw new InvalidOperationException($"Directory refresh failed: {response.Message}");
        }

        var snapshot = response.Data.Deserialize<AccountServerDirectorySnapshot>(_serializerOptions);
        if (snapshot is null)
        {
            return;
        }

        var servers = snapshot.GameServers
            .Select(server => new ServerInfo
            {
                ServerId = server.ServerId,
                ClusterId = _settings.ClusterId,
                ServerName = server.Name,
                ServerLevel = checked((byte)Math.Min(server.Grade, byte.MaxValue)),
                CurrentUserCount = server.CurrentUserCount,
                MaxUserCount = server.MaxUserCount,
                ServerIp = server.IpAddress,
                ServerPort = server.Port,
                ServerVersion = 0,
                ServerStatus = ParseServerStatus(server.Status)
            })
            .ToArray();

        _serverDirectory.ReplaceServersInCluster(_settings.ClusterId, _settings.ServerName, servers);
    }

    private ushort ResolveTargetGameServerId(ushort requestedGameServerId)
    {
        if (requestedGameServerId != 0)
        {
            return requestedGameServerId;
        }

        if (_settings.TargetGameServerId != 0)
        {
            return _settings.TargetGameServerId;
        }

        var server = _serverDirectory
            .GetServersInCluster(_settings.ClusterId)
            .FirstOrDefault(item => item.ServerStatus == 1);

        return server is null ? (ushort)0 : checked((ushort)server.ServerId);
    }

    private async Task<AccountServerEnvelope> SendCommandAsync(string command, object payload, CancellationToken cancellationToken)
    {
        using var tcpClient = new TcpClient();
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(_settings.AccountServerTimeoutMs);

        await tcpClient.ConnectAsync(_serverIp, _serverPort, timeoutCts.Token);
        await using var stream = tcpClient.GetStream();

        using var writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: true)
        {
            NewLine = "\n",
            AutoFlush = true
        };
        using var reader = new StreamReader(stream, Encoding.UTF8, false, leaveOpen: true);

        var requestJson = JsonSerializer.Serialize(new { command, payload }, _serializerOptions);
        await writer.WriteLineAsync(requestJson);

        var responseJson = await reader.ReadLineAsync(timeoutCts.Token);
        if (string.IsNullOrWhiteSpace(responseJson))
        {
            throw new InvalidOperationException($"Account server returned an empty response for {command}.");
        }

        return JsonSerializer.Deserialize<AccountServerEnvelope>(responseJson, _serializerOptions)
            ?? throw new InvalidOperationException($"Account server returned an invalid response for {command}.");
    }

    private static byte ParseServerStatus(string? status)
    {
        return string.Equals(status, "Online", StringComparison.OrdinalIgnoreCase) ? (byte)1 : (byte)0;
    }

    private sealed class AccountServerEnvelope
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public JsonElement Data { get; set; }
    }

    private sealed class AccountServerLoginResponseData
    {
        public bool Success { get; set; }
        public uint UserSerial { get; set; }
        public byte LoginFlag { get; set; }
        public byte Region { get; set; }
        public string? UserNickname { get; set; }
        public uint UserExperience { get; set; }
    }

    private sealed class AccountServerSessionResponseData
    {
        public Guid SessionId { get; set; }
    }

    private sealed class AccountServerDirectorySnapshot
    {
        public List<AccountServerGameServer> GameServers { get; set; } = new();
    }

    private sealed class AccountServerGameServer
    {
        public uint ServerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public ushort Port { get; set; }
        public ushort Grade { get; set; }
        public uint CurrentUserCount { get; set; }
        public uint MaxUserCount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
