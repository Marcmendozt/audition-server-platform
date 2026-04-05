using AccountServer.Domain.Models;

namespace AccountServer.Application.Abstractions;

public interface ICommunityServerRepository
{
    Task<ServerInfo?> GetAsync(CancellationToken cancellationToken);

    Task UpsertAsync(ServerInfo serverInfo, CancellationToken cancellationToken);
}