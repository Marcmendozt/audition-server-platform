using GatewayAudition.Domain.Entities;

namespace GatewayAudition.Domain.Interfaces;

public interface ISessionManager
{
    Session? GetFreeSession();
    void ReturnSession(Session session);
    void Initialize(uint maxSessions, uint bufferSize);
    int ActiveSessionCount { get; }
    IReadOnlyCollection<Session> ActiveSessions { get; }
}
