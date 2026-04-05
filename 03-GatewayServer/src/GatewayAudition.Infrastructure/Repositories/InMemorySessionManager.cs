using GatewayAudition.Domain.Entities;
using GatewayAudition.Domain.Interfaces;

namespace GatewayAudition.Infrastructure.Repositories;

public sealed class InMemorySessionManager : ISessionManager
{
    private readonly object _lockUsing = new();
    private readonly object _lockFree = new();
    private readonly Queue<Session> _freeList = new();
    private readonly List<Session> _usingList = new();

    public int ActiveSessionCount
    {
        get { lock (_lockUsing) return _usingList.Count; }
    }

    public IReadOnlyCollection<Session> ActiveSessions
    {
        get { lock (_lockUsing) return _usingList.ToList().AsReadOnly(); }
    }

    public void Initialize(uint maxSessions, uint bufferSize)
    {
        lock (_lockFree)
        {
            _freeList.Clear();
            for (uint i = 0; i < maxSessions; i++)
            {
                var session = new Session();
                session.Initialize((int)bufferSize);
                session.UniqueIndex = i;
                _freeList.Enqueue(session);
            }
        }
    }

    public Session? GetFreeSession()
    {
        Session? session;
        lock (_lockFree)
        {
            if (!_freeList.TryDequeue(out session))
                return null;
        }

        lock (_lockUsing)
        {
            _usingList.Add(session);
        }

        return session;
    }

    public void ReturnSession(Session session)
    {
        session.Close();

        lock (_lockUsing)
        {
            _usingList.Remove(session);
        }

        lock (_lockFree)
        {
            session.Initialize(session.TotalBufferSize);
            _freeList.Enqueue(session);
        }
    }
}
