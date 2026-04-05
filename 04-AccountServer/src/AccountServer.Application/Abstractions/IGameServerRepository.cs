using AccountServer.Domain.Models;

namespace AccountServer.Application.Abstractions;

public interface IGameServerRepository
{
    Task<GameServer?> GetByIdAsync(ushort serverId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<GameServer>> ListAsync(CancellationToken cancellationToken);

    Task UpsertAsync(GameServer gameServer, CancellationToken cancellationToken);
}