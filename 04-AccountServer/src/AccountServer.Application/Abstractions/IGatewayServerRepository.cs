using AccountServer.Domain.Models;

namespace AccountServer.Application.Abstractions;

public interface IGatewayServerRepository
{
    Task<GatewayServer?> GetByIdAsync(ushort serverId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<GatewayServer>> ListAsync(CancellationToken cancellationToken);

    Task UpsertAsync(GatewayServer gatewayServer, CancellationToken cancellationToken);
}