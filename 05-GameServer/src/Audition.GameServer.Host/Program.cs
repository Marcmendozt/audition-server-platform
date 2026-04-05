using Audition.GameServer.Application.Services;
using Audition.GameServer.Host.Configuration;
using Audition.GameServer.Host.Logging;
using Audition.GameServer.Host.Services;
using Audition.GameServer.Infrastructure.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);
var gameServerOptions = new GameServerOptions();
builder.Configuration.GetSection(GameServerOptions.SectionName).Bind(gameServerOptions);
var runtimePaths = RuntimePathLocator.Resolve(gameServerOptions);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
    options.SingleLine = true;
});
builder.Logging.AddProvider(new SessionFileLoggerProvider(runtimePaths.SessionLogFilePath));
builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);

builder.Services.Configure<GameServerOptions>(
    builder.Configuration.GetSection(GameServerOptions.SectionName));
builder.Services.AddSingleton(runtimePaths);
builder.Services.AddSingleton<ExceptionReportWriter>();
builder.Services.AddHostedService<ExceptionReportService>();

builder.Services.AddGameServerInfrastructure();
builder.Services.AddSingleton<IGameServerBootstrapOrchestrator, GameServerBootstrapOrchestrator>();
builder.Services.AddHostedService<GameServerBootstrapWorker>();
builder.Services.AddHostedService<GameServerListenerService>();

var host = builder.Build();
host.Run();