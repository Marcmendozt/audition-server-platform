using AccountServer.Host.Contracts;

namespace AccountServer.Host.Services;

public interface IDbAgentPayClient
{
    DbAgentStatusSnapshot GetStatus();

    Task<DbAgentStatusSnapshot> ProbeAsync(CancellationToken cancellationToken);

    Task<DbAgentPayOperationResult> SendHeartbeatAsync(CancellationToken cancellationToken);

    Task<DbAgentPayOperationResult> SendAccountInfoAsync(DbAgentPayAccountInfoRequest request, CancellationToken cancellationToken);

    Task<DbAgentPayOperationResult> SendPurchaseAsync(DbAgentPayPurchaseRequest request, CancellationToken cancellationToken);

    Task<DbAgentPayOperationResult> SendGameResultsAsync(DbAgentPayGameResultsRequest request, CancellationToken cancellationToken);

    Task<DbAgentPayOperationResult> SendLevelQuestLogAsync(DbAgentPayLevelQuestLogRequest request, CancellationToken cancellationToken);
}