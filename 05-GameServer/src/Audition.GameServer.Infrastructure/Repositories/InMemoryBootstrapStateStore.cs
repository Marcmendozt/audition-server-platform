using Audition.GameServer.Application.Abstractions;
using Audition.GameServer.Application.Contracts;

namespace Audition.GameServer.Infrastructure.Repositories;

public sealed class InMemoryBootstrapStateStore : IBootstrapStateStore
{
    private BootstrapExecutionResult _current = new(new([]), Domain.Models.BootstrapRuntimeState.Empty);

    public BootstrapExecutionResult Current => _current;

    public Task UpdateAsync(BootstrapExecutionResult result, CancellationToken ct)
    {
        _current = result;
        return Task.CompletedTask;
    }
}