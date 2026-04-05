using Audition.GameServer.Domain.Models;

namespace Audition.GameServer.Application.Contracts;

public sealed record BootstrapConfiguration(
    ushort ServerId,
    string ServerName,
    string BindAddress,
    int BindPort,
    uint ClusterId,
    ushort Grade,
    uint MaxUserCount,
    uint Version,
    ServerStatus Status,
    EndpointDefinition DbAgent,
    EndpointDefinition AccountServer,
    EndpointDefinition Certify)
{
    public IEnumerable<EndpointDefinition> EnumerateDependencies()
    {
        yield return DbAgent;
        yield return AccountServer;
        yield return Certify;
    }
}