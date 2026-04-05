namespace AccountServer.Domain.Models;

public sealed record ServerDirectorySnapshot(
    ServerInfo? CommunityServer,
    IReadOnlyCollection<GatewayServer> Gateways,
    IReadOnlyCollection<GameServer> GameServers);