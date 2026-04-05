using Audition.GameServer.Domain.Models;

namespace Audition.GameServer.Application.Abstractions;

public interface ILegacyGameDataStore
{
    LegacyGameDataSnapshot Current { get; }

    Task UpdateAsync(LegacyGameDataSnapshot snapshot, CancellationToken ct);
}