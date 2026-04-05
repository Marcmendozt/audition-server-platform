using LoginDBAgent.Application.Abstractions;
using LoginDBAgent.Application.Services;
using LoginDBAgent.Infrastructure.Database;
using LoginDBAgent.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LoginDBAgent.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLoginDbAgentInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddSingleton<MySqlConnectionFactory>();

        services.AddSingleton<IUserAccountRepository, MySqlUserAccountRepository>();
        services.AddSingleton<IUserInfoRepository, MySqlUserInfoRepository>();
        services.AddSingleton<IAvatarItemRepository, MySqlAvatarItemRepository>();
        services.AddSingleton<ILevelQuestRepository, MySqlLevelQuestRepository>();
        services.AddSingleton<IRankRepository, MySqlRankRepository>();
        services.AddSingleton<IFriendRepository, MySqlFriendRepository>();
        services.AddSingleton<IPresentRepository, MySqlPresentRepository>();
        services.AddSingleton<IStatisticsRepository, MySqlStatisticsRepository>();

        services.AddSingleton<ILoginDbAgentService, LoginDbAgentService>();

        return services;
    }
}
