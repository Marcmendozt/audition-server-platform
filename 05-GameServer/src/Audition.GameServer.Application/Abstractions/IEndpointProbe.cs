using Audition.GameServer.Application.Contracts;
using Audition.GameServer.Domain.Models;

namespace Audition.GameServer.Application.Abstractions;

public interface IEndpointProbe
{
    Task<ProbeResult> ProbeAsync(EndpointDefinition endpoint, CancellationToken ct);
}