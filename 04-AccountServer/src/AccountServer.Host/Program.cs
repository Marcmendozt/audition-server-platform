using AccountServer.Application.Abstractions;
using AccountServer.Application.Services;
using AccountServer.Domain.Models;
using AccountServer.Host.Configuration;
using AccountServer.Host.Logging;
using AccountServer.Host.Services;
using AccountServer.Infrastructure.Repositories;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options => options.FormatterName = LegacyConsoleFormatter.FormatterName);
builder.Logging.AddConsoleFormatter<LegacyConsoleFormatter, LegacyConsoleFormatterOptions>(options =>
{
	options.LegacyTimestampFormat = "yyyy-MM-dd HH:mm:ss";
});
builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);

builder.Services.Configure<AccountServerOptions>(
	builder.Configuration.GetSection(AccountServerOptions.SectionName));

builder.Services.AddSingleton<IBoardItemRepository, InMemoryBoardItemRepository>();
builder.Services.AddSingleton<ICommunityServerRepository, InMemoryCommunityServerRepository>();
builder.Services.AddSingleton<IGatewayServerRepository, InMemoryGatewayServerRepository>();
builder.Services.AddSingleton<IGameServerRepository, InMemoryGameServerRepository>();
builder.Services.AddSingleton<IPlayerSessionRepository, InMemoryPlayerSessionRepository>();
builder.Services.AddSingleton<IAccountServerService, AccountServerService>();
builder.Services.AddSingleton<PacketBufferManager>();
builder.Services.AddSingleton<SessionRuntimeManager>();
builder.Services.AddSingleton<BinaryPacketCodec>();
builder.Services.AddSingleton<DbAgentClientService>();
builder.Services.AddSingleton<IDbAgentClient>(serviceProvider => serviceProvider.GetRequiredService<DbAgentClientService>());
builder.Services.AddSingleton<IDbAgentPayClient, DbAgentPayClientService>();
builder.Services.AddSingleton<AccountRequestProcessor>();
builder.Services.AddHostedService(serviceProvider => serviceProvider.GetRequiredService<DbAgentClientService>());
builder.Services.AddHostedService<BoardItemMaintenanceService>();
builder.Services.AddHostedService<AccountServerTcpWorker>();

var host = builder.Build();

// Seed pre-configured server directory (TServerDirectoryDB::LoadConfig equivalent)
var options = host.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<AccountServerOptions>>().Value;
var directory = options.ServerDirectory;
if (directory.GameServers.Count > 0)
{
    var gameServerRepo = host.Services.GetRequiredService<IGameServerRepository>();
    foreach (var entry in directory.GameServers)
    {
        var gameServer = new GameServer(
            entry.ServerId,
            IsActive: false,
            entry.Name,
            entry.IpAddress,
            entry.Port,
            entry.Grade,
            CurrentUserCount: 0,
            MaxChannelCount: 1,
            MaxUserCountPerChannel: entry.MaxUserCount,
            entry.MaxUserCount,
            ServerStatus.Offline);
        await gameServerRepo.UpsertAsync(gameServer, CancellationToken.None);
    }
}

host.Run();
