using Audition.DBAgent.Game.Host.Configuration;
using Audition.DBAgent.Game.Host.Network;
using Audition.DBAgent.Game.Infrastructure.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<GameDbAgentOptions>(
    builder.Configuration.GetSection(GameDbAgentOptions.SectionName));

builder.Services.AddGameDbAgentInfrastructure(builder.Configuration);

builder.Services.AddSingleton<ILegacyServerInfoProvider, MySqlLegacyServerInfoProvider>();
builder.Services.AddSingleton<PacketDispatcher>();
builder.Services.AddHostedService<GameDbAgentTcpWorker>();

var host = builder.Build();
host.Run();
