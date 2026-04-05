using GatewayAudition.Application.DTOs;
using GatewayAudition.Application.Services;
using GatewayAudition.Domain.Interfaces;
using GatewayAudition.Infrastructure.Configuration;
using GatewayAudition.Infrastructure.Encryption;
using GatewayAudition.Infrastructure.Networking;
using GatewayAudition.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GatewayAudition.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGatewayInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        services.Configure<GatewaySettings>(configuration.GetSection("Gateway"));

        // Domain interfaces -> Infrastructure implementations
        services.AddSingleton<ISessionManager, InMemorySessionManager>();
        services.AddSingleton<IPlayerManager, InMemoryPlayerManager>();
        services.AddSingleton<IPacketManager, InMemoryPacketManager>();
        services.AddSingleton<IServerDirectory, ServerDirectoryService>();
        services.AddSingleton<IAccountServerManager, AccountServerConnector>();
        services.AddSingleton<IGatewayServer, TcpGatewayServer>();
        services.AddSingleton<IPacketHandler, PacketDispatchService>();

        // Encryption
        services.AddSingleton<ArcfourCipher>();
        services.AddSingleton<AesCipher>();

        // Application services
        services.AddSingleton<LoginService>();
        services.AddSingleton<ServerListService>();
        services.AddSingleton<PacketDispatchService>();

        // Infrastructure services
        services.AddSingleton<SessionCollectionService>();

        return services;
    }
}
