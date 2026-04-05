using Audition.GameServer.Application.Contracts;

namespace Audition.GameServer.Application.Abstractions;

public interface ILegacyGameServerConfigLoader
{
    Task<LegacyBootstrapOverrides?> LoadAsync(string path, CancellationToken ct);
}