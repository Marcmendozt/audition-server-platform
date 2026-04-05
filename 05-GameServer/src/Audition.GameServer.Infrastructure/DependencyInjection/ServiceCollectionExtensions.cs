using Audition.GameServer.Application.Abstractions;
using Audition.GameServer.Infrastructure.Configuration;
using Audition.GameServer.Infrastructure.Networking;
using Audition.GameServer.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Audition.GameServer.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameServerInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IAccountServerRegistrationClient, AccountServerRegistrationClient>();
        services.AddSingleton<IGameDbAgentBootstrapClient, GameDbAgentBootstrapClient>();
        services.AddSingleton<IEndpointProbe, TcpEndpointProbe>();
        services.AddSingleton<IBootstrapStateStore, InMemoryBootstrapStateStore>();
        services.AddSingleton<ILegacyGameDataLoader, LegacyGameDataLoader>();
        services.AddSingleton<ILegacyGameDataStore, InMemoryLegacyGameDataStore>();
        services.AddSingleton<ILegacyGameServerConfigLoader, LegacyGameServerIniLoader>();
        return services;
    }
}