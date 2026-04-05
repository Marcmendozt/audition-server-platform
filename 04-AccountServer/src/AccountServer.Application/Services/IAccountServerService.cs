using AccountServer.Application.Contracts;
using AccountServer.Domain.Models;

namespace AccountServer.Application.Services;

public interface IAccountServerService
{
    Task<BoardItem> UpsertBoardItemAsync(UpsertBoardItemCommand command, CancellationToken cancellationToken);

    Task<PlayerSession?> CloseSessionAsync(ClosePlayerSessionCommand command, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<BoardItem>> GetBoardItemsAsync(CancellationToken cancellationToken);

    Task<ServerDirectorySnapshot> GetServerDirectoryAsync(CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PlayerSession>> GetPlayersAsync(CancellationToken cancellationToken);

    Task<AccountServerSnapshot> GetSnapshotAsync(CancellationToken cancellationToken);

    Task<ServerInfo> RegisterCommunityServerAsync(RegisterCommunityServerCommand command, CancellationToken cancellationToken);

    Task<GameServer> RegisterGameServerAsync(RegisterGameServerCommand command, CancellationToken cancellationToken);

    Task<GatewayServer> RegisterGatewayServerAsync(RegisterGatewayServerCommand command, CancellationToken cancellationToken);

    Task<PlayerSession> OpenSessionAsync(OpenPlayerSessionCommand command, CancellationToken cancellationToken);
}