using Audition.DBAgent.Game.Application.Abstractions;
using Audition.DBAgent.Game.Application.Services;
using Audition.DBAgent.Game.Infrastructure.Database;
using Audition.DBAgent.Game.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Audition.DBAgent.Game.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameDbAgentInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddSingleton<MySqlConnectionFactory>();

        services.AddSingleton<IUserInfoRepository, MySqlUserInfoRepository>();
        services.AddSingleton<ILevelQuestRepository, MySqlLevelQuestRepository>();
        services.AddSingleton<IAvatarItemRepository, MySqlAvatarItemRepository>();
        services.AddSingleton<IPresentRepository, MySqlPresentRepository>();
        services.AddSingleton<IStatisticsRepository, MySqlStatisticsRepository>();

        services.AddSingleton<IGameDbAgentService, GameDbAgentService>();

        return services;
    }
}
