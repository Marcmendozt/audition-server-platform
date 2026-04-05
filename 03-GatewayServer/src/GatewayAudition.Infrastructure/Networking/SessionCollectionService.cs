using GatewayAudition.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GatewayAudition.Infrastructure.Networking;

public sealed class SessionCollectionService
{
    private readonly ISessionManager _sessionManager;
    private readonly ILogger<SessionCollectionService> _logger;
    private const uint SessionTimeoutMs = 300_000; // 5 minutes

    public SessionCollectionService(
        ISessionManager sessionManager,
        ILogger<SessionCollectionService> logger)
    {
        _sessionManager = sessionManager;
        _logger = logger;
    }

    public void CollectStaleSessions()
    {
        uint now = (uint)Environment.TickCount;
        var activeSessions = _sessionManager.ActiveSessions;

        foreach (var session in activeSessions)
        {
            if (session.IsAlive && (now - session.SessionInfo.LastAccessTime) > SessionTimeoutMs)
            {
                _logger.LogInformation("Collecting stale session {Index} ({Ip})",
                    session.UniqueIndex, session.SessionInfo.IpAddress);
                _sessionManager.ReturnSession(session);
            }
        }
    }
}
