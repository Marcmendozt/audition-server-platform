using Audition.GameServer.Application.Abstractions;
using Audition.GameServer.Domain.Models;

namespace Audition.GameServer.Infrastructure.Repositories;

public sealed class InMemoryLegacyGameDataStore : ILegacyGameDataStore
{
    private LegacyGameDataSnapshot current = LegacyGameDataSnapshot.Empty;

    public LegacyGameDataSnapshot Current => current;

    public Task UpdateAsync(LegacyGameDataSnapshot snapshot, CancellationToken ct)
    {
        current = snapshot;
        return Task.CompletedTask;
    }
}