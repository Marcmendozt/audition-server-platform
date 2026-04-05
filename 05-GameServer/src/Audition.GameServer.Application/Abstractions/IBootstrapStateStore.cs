using Audition.GameServer.Application.Contracts;

namespace Audition.GameServer.Application.Abstractions;

public interface IBootstrapStateStore
{
    BootstrapExecutionResult Current { get; }

    Task UpdateAsync(BootstrapExecutionResult result, CancellationToken ct);
}