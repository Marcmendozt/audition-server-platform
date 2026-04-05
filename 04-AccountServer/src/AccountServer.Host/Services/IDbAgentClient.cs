using AccountServer.Host.Contracts;

namespace AccountServer.Host.Services;

public interface IDbAgentClient
{
    DbAgentStatusSnapshot GetStatus();

    Task<DbAgentLoginResult> RequestLoginChinaAsync(DbAgentLoginRequest request, CancellationToken cancellationToken);
}