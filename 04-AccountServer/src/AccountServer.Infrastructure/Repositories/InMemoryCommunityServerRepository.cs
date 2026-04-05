using AccountServer.Application.Abstractions;
using AccountServer.Domain.Models;

namespace AccountServer.Infrastructure.Repositories;

public sealed class InMemoryCommunityServerRepository : ICommunityServerRepository
{
    private readonly SemaphoreSlim synchronization = new(1, 1);
    private ServerInfo? serverInfo;

    public async Task<ServerInfo?> GetAsync(CancellationToken cancellationToken)
    {
        await synchronization.WaitAsync(cancellationToken);
        try
        {
            return serverInfo;
        }
        finally
        {
            synchronization.Release();
        }
    }

    public async Task UpsertAsync(ServerInfo serverInfo, CancellationToken cancellationToken)
    {
        await synchronization.WaitAsync(cancellationToken);
        try
        {
            this.serverInfo = serverInfo;
        }
        finally
        {
            synchronization.Release();
        }
    }
}