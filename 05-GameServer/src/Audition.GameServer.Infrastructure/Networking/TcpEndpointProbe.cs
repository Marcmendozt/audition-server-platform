using Audition.GameServer.Application.Abstractions;
using Audition.GameServer.Application.Contracts;
using Audition.GameServer.Domain.Models;
using System.Net.Sockets;

namespace Audition.GameServer.Infrastructure.Networking;

public sealed class TcpEndpointProbe : IEndpointProbe
{
    public async Task<ProbeResult> ProbeAsync(EndpointDefinition endpoint, CancellationToken ct)
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(endpoint.Host, endpoint.Port, ct);
            return new ProbeResult(true, $"Connected to {endpoint.Host}:{endpoint.Port}");
        }
        catch (Exception ex)
        {
            return new ProbeResult(false, $"Unable to reach {endpoint.Host}:{endpoint.Port} ({ex.Message})");
        }
    }
}