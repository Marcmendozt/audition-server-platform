using GatewayAudition.Host;
using GatewayAudition.Infrastructure.DependencyInjection;
using GatewayAudition.Host.Logging;

var builder = Host.CreateApplicationBuilder(args);

string sessionLogFilePath = Path.GetFullPath(
	Path.Combine(builder.Environment.ContentRootPath, "..", "..", "log", "gateway-session.log"));

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
	options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
	options.SingleLine = true;
});
builder.Logging.AddProvider(new SessionFileLoggerProvider(sessionLogFilePath));
builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);

builder.Services.AddGatewayInfrastructure(builder.Configuration);
builder.Services.AddHostedService<GatewayWorker>();
builder.Services.AddHostedService<AccountServerWorker>();
builder.Services.AddHostedService<SessionCollectionWorker>();

var host = builder.Build();
host.Run();
