using Audition.GameServer.Application.Contracts;

namespace Audition.GameServer.Application.Abstractions;

public interface IGameDbAgentBootstrapClient
{
    Task<GameDbBootstrapResult> SynchronizeAsync(BootstrapConfiguration configuration, CancellationToken ct);
}