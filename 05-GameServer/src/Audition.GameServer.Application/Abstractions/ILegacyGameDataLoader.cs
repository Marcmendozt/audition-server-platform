using Audition.GameServer.Domain.Models;

namespace Audition.GameServer.Application.Abstractions;

public interface ILegacyGameDataLoader
{
    Task<LegacyGameDataSnapshot> LoadAsync(string dataDirectory, CancellationToken ct);
}