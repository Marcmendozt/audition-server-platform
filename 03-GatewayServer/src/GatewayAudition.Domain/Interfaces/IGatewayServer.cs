using GatewayAudition.Domain.Entities;

namespace GatewayAudition.Domain.Interfaces;

public interface IGatewayServer
{
    Task StartAsync(ushort port, CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
    bool IsRunning { get; }
}
