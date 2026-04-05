using System.Text.Json;
using System.Text.Json.Serialization;
using AccountServer.Application.Contracts;
using AccountServer.Application.Services;
using AccountServer.Host.Configuration;
using AccountServer.Host.Contracts;
using Microsoft.Extensions.Options;

namespace AccountServer.Host.Services;

public sealed class AccountRequestProcessor
{
    private readonly IAccountServerService accountServerService;
    private readonly BinaryPacketCodec binaryPacketCodec;
    private readonly IDbAgentClient dbAgentClient;
    private readonly IDbAgentPayClient dbAgentPayClient;
    private readonly ILogger<AccountRequestProcessor> logger;
    private readonly PacketBufferManager packetBufferManager;
    private readonly IOptions<AccountServerOptions> options;
    private readonly SessionRuntimeManager sessionRuntimeManager;
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public AccountRequestProcessor(
        IAccountServerService accountServerService,
        BinaryPacketCodec binaryPacketCodec,
        IDbAgentClient dbAgentClient,
        IDbAgentPayClient dbAgentPayClient,
        PacketBufferManager packetBufferManager,
        SessionRuntimeManager sessionRuntimeManager,
        IOptions<AccountServerOptions> options,
        ILogger<AccountRequestProcessor> logger)
    {
        this.accountServerService = accountServerService;
        this.binaryPacketCodec = binaryPacketCodec;
        this.dbAgentClient = dbAgentClient;
        this.dbAgentPayClient = dbAgentPayClient;
        this.logger = logger;
        this.options = options;
        this.packetBufferManager = packetBufferManager;
        this.sessionRuntimeManager = sessionRuntimeManager;
    }

    public async Task<AccountServerResponse> ProcessAsync(
        AccountServerRequest request,
        string endpointName,
        CancellationToken cancellationToken)
    {
        try
        {
            var normalizedCommand = request.Command.Trim().ToLowerInvariant();
            if (!EndpointCommandPolicy.IsAllowed(endpointName, normalizedCommand))
            {
                return new AccountServerResponse(false, $"El comando {normalizedCommand} no esta permitido en el endpoint {endpointName}.");
            }

            return normalizedCommand switch
            {
                "health" => new AccountServerResponse(true, $"{endpointName} endpoint operativo."),
                "register_gateway" => await RegisterGatewayAsync(request.Payload, cancellationToken),
                "register_game_server" => await RegisterGameServerAsync(request.Payload, cancellationToken),
                "register_community_server" => await RegisterCommunityServerAsync(request.Payload, cancellationToken),
                "open_session" => await OpenSessionAsync(request.Payload, cancellationToken),
                "close_session" => await CloseSessionAsync(request.Payload, cancellationToken),
                "request_login_china" => await RequestLoginChinaAsync(request.Payload, cancellationToken),
                "list_servers" => await ListServersAsync(cancellationToken),
                "list_players" => await ListPlayersAsync(cancellationToken),
                "list_directory" => await ListDirectoryAsync(cancellationToken),
                "upsert_board_item" => await UpsertBoardItemAsync(request.Payload, cancellationToken),
                "list_board_items" => await ListBoardItemsAsync(cancellationToken),
                "session_pool_status" => SessionPoolStatusResponse(),
                "packet_manager_status" => PacketManagerStatusResponse(),
                "dbagent_status" => DbAgentStatusResponse(),
                "dbagent_pay_status" => DbAgentPayStatusResponse(),
                "dbagent_pay_probe" => await DbAgentPayProbeAsync(cancellationToken),
                "dbagent_pay_heartbeat" => await DbAgentPayHeartbeatAsync(cancellationToken),
                "dbagent_pay_account_info" => await DbAgentPayAccountInfoAsync(request.Payload, cancellationToken),
                "dbagent_pay_purchase" => await DbAgentPayPurchaseAsync(request.Payload, cancellationToken),
                "dbagent_pay_game_results" => await DbAgentPayGameResultsAsync(request.Payload, cancellationToken),
                "dbagent_pay_level_quest_log" => await DbAgentPayLevelQuestLogAsync(request.Payload, cancellationToken),
                _ => new AccountServerResponse(false, $"Comando desconocido: {request.Command}")
            };
        }
        catch (JsonException exception)
        {
            return new AccountServerResponse(false, $"Payload invalido: {exception.Message}");
        }
        catch (InvalidOperationException exception)
        {
            return new AccountServerResponse(false, exception.Message);
        }
    }

    public async Task ProcessBinaryAsync(
        BinaryPacketRequest request,
        string endpointName,
        CancellationToken cancellationToken)
    {
        switch (endpointName)
        {
            case "Gateway" when binaryPacketCodec.TryParseGatewayRegistration(request, out var gatewayCommand):
                await RegisterGatewayBinaryAsync(gatewayCommand, cancellationToken);
                return;
            case "Game" when binaryPacketCodec.TryParseGameRegistration(request, out var gameCommand):
                await RegisterGameBinaryAsync(gameCommand, cancellationToken);
                return;
            default:
                logger.LogWarning(
                    "Packet binario no soportado en {EndpointName}. Opcode=0x{Opcode:X2}, SubOpcode={SubOpcode}",
                    endpointName,
                    request.Opcode,
                    request.SubOpcode.HasValue ? $"0x{request.SubOpcode.Value:X2}" : "n/a");
                return;
        }
    }

    private async Task<AccountServerResponse> RegisterGatewayAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        var command = Deserialize<RegisterGatewayServerCommand>(payload);
        var result = await accountServerService.RegisterGatewayServerAsync(command, cancellationToken);
        logger.LogInformation(
            "Gateway server connected. ServerId={ServerId}, Name={Name}, Ip={IpAddress}, Port={Port}",
            result.ServerId,
            result.Info.Name,
            result.Info.IpAddress,
            result.Info.Port);
        return new AccountServerResponse(true, "Gateway registrado.", result);
    }

    private async Task RegisterGatewayBinaryAsync(RegisterGatewayServerCommand command, CancellationToken cancellationToken)
    {
        var result = await accountServerService.RegisterGatewayServerAsync(command, cancellationToken);
        logger.LogInformation(
            "Gateway server connected. ServerId={ServerId}, Name={Name}, Ip={IpAddress}, Port={Port}",
            result.ServerId,
            result.Info.Name,
            result.Info.IpAddress,
            result.Info.Port);
    }

    private async Task<AccountServerResponse> RegisterGameServerAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        var command = Deserialize<RegisterGameServerCommand>(payload);
        var result = await accountServerService.RegisterGameServerAsync(command, cancellationToken);
        logger.LogInformation(
            "Game server connected. ServerId={ServerId}, Name={Name}, Ip={IpAddress}, Port={Port}",
            result.ServerId,
            result.Name,
            result.IpAddress,
            result.Port);
        return new AccountServerResponse(true, "Game server registrado.", result);
    }

    private async Task RegisterGameBinaryAsync(RegisterGameServerCommand command, CancellationToken cancellationToken)
    {
        var result = await accountServerService.RegisterGameServerAsync(command, cancellationToken);
        logger.LogInformation(
            "Game server connected. ServerId={ServerId}, Name={Name}, Ip={IpAddress}, Port={Port}",
            result.ServerId,
            result.Name,
            result.IpAddress,
            result.Port);
    }

    private async Task<AccountServerResponse> RegisterCommunityServerAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        var command = Deserialize<RegisterCommunityServerCommand>(payload);
        var result = await accountServerService.RegisterCommunityServerAsync(command, cancellationToken);
        logger.LogInformation(
            "Community server connected. ServerId={ServerId}, Name={Name}, Ip={IpAddress}, Port={Port}",
            result.ServerId,
            result.Name,
            result.IpAddress,
            result.Port);
        return new AccountServerResponse(true, "Community server registrado.", result);
    }

    private async Task<AccountServerResponse> OpenSessionAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        var command = Deserialize<OpenPlayerSessionCommand>(payload);
        var result = await accountServerService.OpenSessionAsync(command, cancellationToken);

        logger.LogInformation(
            "UserSN({UserSN}), UserID({UserId}), dwExp({Experience}) login success",
            result.UserSerial,
            result.UserId,
            result.UserExperience);

        return new AccountServerResponse(true, "Sesion abierta.", result);
    }

    private async Task<AccountServerResponse> RequestLoginChinaAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        var command = Deserialize<DbAgentLoginRequest>(payload);

        logger.LogInformation(
            "Server[{ServerId}] [packet1.1] {UserId}, <hidden>, {Version}",
            command.GatewayServerId ?? 0,
            command.UserId,
            command.ExtraField3);

        logger.LogInformation(
            "[96.0.0] UserID({UserId})",
            command.UserId);

        var result = await dbAgentClient.RequestLoginChinaAsync(command, cancellationToken);

        logger.LogInformation(
            "[ACCOUNT LOGIN] request_login_china response | UserId={UserId} | Success={Success} | InternalResult={InternalResult} | UserSN={UserSN} | LoginFlag={LoginFlag} | Nickname={Nickname} | Experience={Experience}",
            command.UserId,
            result.Success,
            result.InternalResult,
            result.UserSerial,
            result.LoginFlag,
            result.UserNickname,
            result.UserExperience);

        return new AccountServerResponse(result.Success, result.Success ? "Login procesado por DBAgent." : "DBAgent rechazo el login.", result);
    }

    private async Task<AccountServerResponse> CloseSessionAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        var command = Deserialize<ClosePlayerSessionCommand>(payload);
        var session = await accountServerService.CloseSessionAsync(command, cancellationToken);

        if (session is not null)
        {
            logger.LogInformation(
                "[{ServerId}.1]delete player OK - ID : {UserId}, PlayerID : {PlayerId}",
                session.ServerId,
                session.UserId,
                session.UserId);
        }

        return new AccountServerResponse(true, "Sesion cerrada.");
    }

    private async Task<AccountServerResponse> ListServersAsync(CancellationToken cancellationToken)
    {
        var result = await accountServerService.GetSnapshotAsync(cancellationToken);
        return new AccountServerResponse(true, "Snapshot generado.", result);
    }

    private async Task<AccountServerResponse> ListPlayersAsync(CancellationToken cancellationToken)
    {
        var result = await accountServerService.GetPlayersAsync(cancellationToken);
        return new AccountServerResponse(true, "Jugadores activos recuperados.", result);
    }

    private async Task<AccountServerResponse> ListDirectoryAsync(CancellationToken cancellationToken)
    {
        var result = await accountServerService.GetServerDirectoryAsync(cancellationToken);
        return new AccountServerResponse(true, "Directorio recuperado.", result);
    }

    private async Task<AccountServerResponse> UpsertBoardItemAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        var command = Deserialize<UpsertBoardItemCommand>(payload);
        var result = await accountServerService.UpsertBoardItemAsync(command, cancellationToken);
        return new AccountServerResponse(true, "Board item actualizado.", result);
    }

    private async Task<AccountServerResponse> ListBoardItemsAsync(CancellationToken cancellationToken)
    {
        var result = await accountServerService.GetBoardItemsAsync(cancellationToken);
        return new AccountServerResponse(true, "Board items recuperados.", result);
    }

    private AccountServerResponse SessionPoolStatusResponse()
    {
        return new AccountServerResponse(true, "Estado del pool de sesiones.", sessionRuntimeManager.GetStatus(options.Value.SessionBufferSize));
    }

    private AccountServerResponse PacketManagerStatusResponse()
    {
        return new AccountServerResponse(true, "Estado del packet manager.", packetBufferManager.GetStatus());
    }

    private AccountServerResponse DbAgentStatusResponse()
    {
        return new AccountServerResponse(true, "Estado de DBAgent.", dbAgentClient.GetStatus());
    }

    private AccountServerResponse DbAgentPayStatusResponse()
    {
        return new AccountServerResponse(true, "Estado de DBAgent.Pay.", dbAgentPayClient.GetStatus());
    }

    private async Task<AccountServerResponse> DbAgentPayProbeAsync(CancellationToken cancellationToken)
    {
        var result = await dbAgentPayClient.ProbeAsync(cancellationToken);
        return new AccountServerResponse(result.Connected, result.Connected ? "DBAgent.Pay accesible." : "DBAgent.Pay no accesible.", result);
    }

    private async Task<AccountServerResponse> DbAgentPayHeartbeatAsync(CancellationToken cancellationToken)
    {
        var result = await dbAgentPayClient.SendHeartbeatAsync(cancellationToken);
        return new AccountServerResponse(true, "Heartbeat enviado a DBAgent.Pay.", result);
    }

    private async Task<AccountServerResponse> DbAgentPayAccountInfoAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        var command = Deserialize<DbAgentPayAccountInfoRequest>(payload);
        var result = await dbAgentPayClient.SendAccountInfoAsync(command, cancellationToken);
        return new AccountServerResponse(true, "Solicitud AccountInfo enviada a DBAgent.Pay.", result);
    }

    private async Task<AccountServerResponse> DbAgentPayPurchaseAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        var command = Deserialize<DbAgentPayPurchaseRequest>(payload);
        var result = await dbAgentPayClient.SendPurchaseAsync(command, cancellationToken);
        return new AccountServerResponse(true, "Solicitud Purchase enviada a DBAgent.Pay.", result);
    }

    private async Task<AccountServerResponse> DbAgentPayGameResultsAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        var command = Deserialize<DbAgentPayGameResultsRequest>(payload);
        var result = await dbAgentPayClient.SendGameResultsAsync(command, cancellationToken);
        return new AccountServerResponse(true, "Solicitud GameResults enviada a DBAgent.Pay.", result);
    }

    private async Task<AccountServerResponse> DbAgentPayLevelQuestLogAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        var command = Deserialize<DbAgentPayLevelQuestLogRequest>(payload);
        var result = await dbAgentPayClient.SendLevelQuestLogAsync(command, cancellationToken);
        return new AccountServerResponse(true, "Solicitud LevelQuestLog enviada a DBAgent.Pay.", result);
    }

    private static T Deserialize<T>(JsonElement payload)
    {
        var rawPayload = payload.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null
            ? "{}"
            : payload.GetRawText();

        var result = JsonSerializer.Deserialize<T>(rawPayload, SerializerOptions);
        return result ?? throw new JsonException($"No se pudo deserializar el payload a {typeof(T).Name}.");
    }
}