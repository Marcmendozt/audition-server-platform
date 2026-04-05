using AccountServer.Application.Abstractions;
using AccountServer.Application.Contracts;
using AccountServer.Domain.Models;

namespace AccountServer.Application.Services;

public sealed class AccountServerService(
    IGatewayServerRepository gatewayServerRepository,
    IGameServerRepository gameServerRepository,
    IPlayerSessionRepository playerSessionRepository,
    IBoardItemRepository boardItemRepository,
    ICommunityServerRepository communityServerRepository) : IAccountServerService
{
    private readonly SemaphoreSlim synchronization = new(1, 1);

    public async Task<ServerInfo> RegisterCommunityServerAsync(RegisterCommunityServerCommand command, CancellationToken cancellationToken)
    {
        ValidateName(command.Name, nameof(command.Name));
        ValidateName(command.IpAddress, nameof(command.IpAddress));

        var server = new ServerInfo(
            command.ServerId,
            command.ClusterId,
            command.Name.Trim(),
            command.Level,
            0,
            command.MaxUserCount,
            command.IpAddress.Trim(),
            command.Port,
            command.Version,
            command.Status);

        await communityServerRepository.UpsertAsync(server, cancellationToken);
        return server;
    }

    public async Task<GatewayServer> RegisterGatewayServerAsync(RegisterGatewayServerCommand command, CancellationToken cancellationToken)
    {
        ValidateName(command.Name, nameof(command.Name));
        ValidateName(command.IpAddress, nameof(command.IpAddress));

        await synchronization.WaitAsync(cancellationToken);
        try
        {
            var existing = await gatewayServerRepository.GetByIdAsync(command.ServerId, cancellationToken);
            var userCount = existing?.UserCount ?? 0;
            var gateway = new GatewayServer(
                command.ServerId,
                true,
                userCount,
                new ServerInfo(
                    command.ServerId,
                    command.ClusterId,
                    command.Name.Trim(),
                    command.Level,
                    userCount,
                    command.MaxUserCount,
                    command.IpAddress.Trim(),
                    command.Port,
                    command.Version,
                    command.Status));

            await gatewayServerRepository.UpsertAsync(gateway, cancellationToken);
            return gateway;
        }
        finally
        {
            synchronization.Release();
        }
    }

    public async Task<GameServer> RegisterGameServerAsync(RegisterGameServerCommand command, CancellationToken cancellationToken)
    {
        ValidateName(command.Name, nameof(command.Name));
        ValidateName(command.IpAddress, nameof(command.IpAddress));

        await synchronization.WaitAsync(cancellationToken);
        try
        {
            var existing = await gameServerRepository.GetByIdAsync(command.ServerId, cancellationToken);
            var userCount = existing?.CurrentUserCount ?? 0;
            var gameServer = new GameServer(
                command.ServerId,
                true,
                command.Name.Trim(),
                command.IpAddress.Trim(),
                command.Port,
                command.Grade,
                userCount,
                command.MaxChannelCount,
                command.MaxUserCountPerChannel,
                command.MaxUserCount,
                command.Status);

            await gameServerRepository.UpsertAsync(gameServer, cancellationToken);
            return gameServer;
        }
        finally
        {
            synchronization.Release();
        }
    }

    public async Task<PlayerSession> OpenSessionAsync(OpenPlayerSessionCommand command, CancellationToken cancellationToken)
    {
        ValidateName(command.UserId, nameof(command.UserId));
        ValidateName(command.IpAddress, nameof(command.IpAddress));

        await synchronization.WaitAsync(cancellationToken);
        try
        {
            var gateway = await gatewayServerRepository.GetByIdAsync(command.GatewayServerId, cancellationToken)
                ?? throw new InvalidOperationException($"Gateway {command.GatewayServerId} no existe.");

            if (!gateway.IsActive)
            {
                throw new InvalidOperationException($"Gateway {command.GatewayServerId} no esta activo.");
            }

            var gameServer = await gameServerRepository.GetByIdAsync(command.ServerId, cancellationToken)
                ?? throw new InvalidOperationException($"Game server {command.ServerId} no existe.");

            if (!gameServer.IsActive)
            {
                throw new InvalidOperationException($"Game server {command.ServerId} no esta activo.");
            }

            if (gameServer.CurrentUserCount >= gameServer.MaxUserCount)
            {
                throw new InvalidOperationException($"Game server {command.ServerId} alcanzo su capacidad maxima.");
            }

            var existingSessions = await playerSessionRepository.ListAsync(cancellationToken);
            if (existingSessions.Any(item =>
                    item.State == SessionState.Active &&
                    (string.Equals(item.UserId, command.UserId.Trim(), StringComparison.OrdinalIgnoreCase) ||
                     item.UserSerial == command.UserSerial)))
            {
                throw new InvalidOperationException($"El jugador {command.UserId} ya tiene una sesion activa.");
            }

            var session = new PlayerSession(
                Guid.NewGuid(),
                command.UserSerial,
                command.UserExperience,
                command.UserId.Trim(),
                command.ClusterId,
                command.ServerId,
                command.GatewayServerId,
                new SessionInfo(command.SocketHandle, command.IpAddress.Trim(), DateTime.UtcNow),
                DateTime.UtcNow,
                SessionState.Active);

            var updatedGateway = gateway.WithUserCount(checked((ushort)(gateway.UserCount + 1)));
            var updatedGameServer = gameServer.WithUserCount(checked((ushort)(gameServer.CurrentUserCount + 1)));

            await playerSessionRepository.UpsertAsync(session, cancellationToken);
            await gatewayServerRepository.UpsertAsync(updatedGateway, cancellationToken);
            await gameServerRepository.UpsertAsync(updatedGameServer, cancellationToken);

            return session;
        }
        finally
        {
            synchronization.Release();
        }
    }

    public async Task<PlayerSession?> CloseSessionAsync(ClosePlayerSessionCommand command, CancellationToken cancellationToken)
    {
        await synchronization.WaitAsync(cancellationToken);
        try
        {
            var session = await playerSessionRepository.GetByIdAsync(command.SessionId, cancellationToken);
            if (session is null)
            {
                return null;
            }

            var gateway = await gatewayServerRepository.GetByIdAsync(session.GatewayServerId, cancellationToken);
            if (gateway is not null)
            {
                var updatedGateway = gateway.WithUserCount((ushort)Math.Max(0, gateway.UserCount - 1));
                await gatewayServerRepository.UpsertAsync(updatedGateway, cancellationToken);
            }

            var gameServer = await gameServerRepository.GetByIdAsync(session.ServerId, cancellationToken);
            if (gameServer is not null)
            {
                var updatedGameServer = gameServer.WithUserCount((ushort)Math.Max(0, gameServer.CurrentUserCount - 1));
                await gameServerRepository.UpsertAsync(updatedGameServer, cancellationToken);
            }

            await playerSessionRepository.RemoveAsync(command.SessionId, cancellationToken);
            return session;
        }
        finally
        {
            synchronization.Release();
        }
    }

    public async Task<AccountServerSnapshot> GetSnapshotAsync(CancellationToken cancellationToken)
    {
        var gateways = await gatewayServerRepository.ListAsync(cancellationToken);
        var gameServers = await gameServerRepository.ListAsync(cancellationToken);
        var sessions = await playerSessionRepository.ListAsync(cancellationToken);

        return new AccountServerSnapshot(
            gateways.OrderBy(item => item.ServerId).ToArray(),
            gameServers.OrderBy(item => item.ServerId).ToArray(),
            sessions.OrderBy(item => item.UserId).ToArray());
    }

    public async Task<ServerDirectorySnapshot> GetServerDirectoryAsync(CancellationToken cancellationToken)
    {
        var communityServer = await communityServerRepository.GetAsync(cancellationToken);
        var gateways = await gatewayServerRepository.ListAsync(cancellationToken);
        var gameServers = await gameServerRepository.ListAsync(cancellationToken);

        return new ServerDirectorySnapshot(
            communityServer,
            gateways.OrderBy(item => item.ServerId).ToArray(),
            gameServers.OrderBy(item => item.ServerId).ToArray());
    }

    public async Task<BoardItem> UpsertBoardItemAsync(UpsertBoardItemCommand command, CancellationToken cancellationToken)
    {
        ValidateName(command.UserNickname, nameof(command.UserNickname));
        ValidateName(command.Title, nameof(command.Title));

        var now = DateTime.UtcNow;
        var item = new BoardItem(
            command.BoardSerial,
            command.UserSerial,
            command.UserNickname.Trim(),
            command.DestinationServer,
            command.Title.Trim(),
            command.Payload ?? string.Empty,
            now,
            now);

        await boardItemRepository.UpsertAsync(item, cancellationToken);
        return item;
    }

    public async Task<IReadOnlyCollection<BoardItem>> GetBoardItemsAsync(CancellationToken cancellationToken)
    {
        var items = await boardItemRepository.ListAsync(cancellationToken);
        return items.OrderByDescending(item => item.UpdatedAtUtc).ToArray();
    }

    public async Task<IReadOnlyCollection<PlayerSession>> GetPlayersAsync(CancellationToken cancellationToken)
    {
        var players = await playerSessionRepository.ListAsync(cancellationToken);
        return players.OrderBy(item => item.UserId).ToArray();
    }

    private static void ValidateName(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{paramName} es obligatorio.");
        }
    }
}