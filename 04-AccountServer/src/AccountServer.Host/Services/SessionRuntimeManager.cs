using System.Collections.Concurrent;
using AccountServer.Host.Contracts;

namespace AccountServer.Host.Services;

public sealed class SessionRuntimeManager(PacketBufferManager packetBufferManager)
{
    private readonly ConcurrentDictionary<int, SessionRuntime> activeSessions = new();
    private readonly ConcurrentQueue<ReusableBuffers> freeBuffers = new();
    private int maxConcurrentSessions;
    private int nextSessionId;
    private int sessionsCreated;

    public SessionLease Attach(string listenerName, string remoteEndPoint, int bufferSize)
    {
        var sessionId = Interlocked.Increment(ref nextSessionId);
        var reusableBuffers = freeBuffers.TryDequeue(out var existing)
            ? existing
            : CreateBuffers(bufferSize);

        if (existing is null)
        {
            Interlocked.Increment(ref sessionsCreated);
        }

        var runtime = new SessionRuntime(
            sessionId,
            listenerName,
            remoteEndPoint,
            reusableBuffers.ReceiveBuffer,
            reusableBuffers.SendBuffer,
            DateTime.UtcNow);

        activeSessions[sessionId] = runtime;
        UpdateMaxConcurrentSessions(activeSessions.Count);

        return new SessionLease(this, runtime);
    }

    public SessionPoolStatus GetStatus(int bufferSize)
    {
        return new SessionPoolStatus(
            activeSessions.Count,
            freeBuffers.Count,
            Volatile.Read(ref sessionsCreated),
            Volatile.Read(ref maxConcurrentSessions),
            bufferSize);
    }

    private ReusableBuffers CreateBuffers(int bufferSize)
    {
        return new ReusableBuffers(
            packetBufferManager.Rent(bufferSize),
            packetBufferManager.Rent(bufferSize));
    }

    private void Release(SessionRuntime runtime)
    {
        activeSessions.TryRemove(runtime.SessionId, out _);
        freeBuffers.Enqueue(new ReusableBuffers(runtime.ReceiveBuffer, runtime.SendBuffer));
    }

    private void UpdateMaxConcurrentSessions(int currentCount)
    {
        while (true)
        {
            var snapshot = Volatile.Read(ref maxConcurrentSessions);
            if (currentCount <= snapshot)
            {
                break;
            }

            if (Interlocked.CompareExchange(ref maxConcurrentSessions, currentCount, snapshot) == snapshot)
            {
                break;
            }
        }
    }

    public readonly record struct SessionLease(SessionRuntimeManager Owner, SessionRuntime Runtime) : IDisposable
    {
        public void Dispose()
        {
            Owner.Release(Runtime);
        }
    }

    private sealed record ReusableBuffers(byte[] ReceiveBuffer, byte[] SendBuffer);

    public sealed record SessionRuntime(
        int SessionId,
        string ListenerName,
        string RemoteEndPoint,
        byte[] ReceiveBuffer,
        byte[] SendBuffer,
        DateTime ConnectedAtUtc);
}