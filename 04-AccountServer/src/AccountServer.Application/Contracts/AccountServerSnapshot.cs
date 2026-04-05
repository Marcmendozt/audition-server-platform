using AccountServer.Domain.Models;

namespace AccountServer.Application.Contracts;

public sealed record AccountServerSnapshot(
    IReadOnlyCollection<GatewayServer> Gateways,
    IReadOnlyCollection<GameServer> GameServers,
    IReadOnlyCollection<PlayerSession> Sessions);