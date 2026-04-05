using GatewayAudition.Domain.Entities;
using GatewayAudition.Domain.ValueObjects;

namespace GatewayAudition.Domain.Interfaces;

public interface IAccountServerManager
{
    bool IsServerStarted { get; }
    int ActiveConnectionCount { get; }
    Task ConnectAsync(CancellationToken cancellationToken);
    Task<AccountServerLoginResult> RequestLoginAsync(User user, string username, string password, string clientVersion, ushort requestedGameServerId, CancellationToken cancellationToken);
    Task CloseSessionAsync(Guid sessionId, CancellationToken cancellationToken);
    void Initialize(string serverIp, ushort serverPort);
}
