using LoginDBAgent.Host.Configuration;
using LoginDBAgent.Host.Network;
using LoginDBAgent.Infrastructure.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<LoginDbAgentOptions>(
    builder.Configuration.GetSection(LoginDbAgentOptions.SectionName));

builder.Services.AddLoginDbAgentInfrastructure(builder.Configuration);

builder.Services.AddSingleton<PacketDispatcher>();
builder.Services.AddHostedService<LoginDbAgentTcpWorker>();

var host = builder.Build();
host.Run();
