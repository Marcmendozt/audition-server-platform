namespace Audition.GameServer.Domain.Models;

public sealed record EndpointDefinition(
    string Name,
    string Host,
    int Port,
    bool Enabled = true,
    bool Required = true);