using Audition.GameServer.Application.Contracts;

namespace Audition.GameServer.Application.Services;

public interface IGameServerBootstrapOrchestrator
{
    Task<BootstrapExecutionResult> ExecuteAsync(BootstrapConfiguration configuration, CancellationToken ct);
}