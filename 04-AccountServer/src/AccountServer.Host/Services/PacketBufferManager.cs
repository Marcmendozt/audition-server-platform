using System.Buffers;
using AccountServer.Host.Contracts;

namespace AccountServer.Host.Services;

public sealed class PacketBufferManager
{
    private long buffersReturned;
    private long buffersRented;
    private int buffersInUse;
    private long packetsProcessed;

    public byte[] Rent(int minimumLength)
    {
        Interlocked.Increment(ref buffersInUse);
        Interlocked.Increment(ref buffersRented);
        return ArrayPool<byte>.Shared.Rent(Math.Max(1, minimumLength));
    }

    public void Return(byte[]? buffer)
    {
        if (buffer is null)
        {
            return;
        }

        ArrayPool<byte>.Shared.Return(buffer);
        Interlocked.Decrement(ref buffersInUse);
        Interlocked.Increment(ref buffersReturned);
    }

    public void RegisterProcessedPacket()
    {
        Interlocked.Increment(ref packetsProcessed);
    }

    public PacketManagerStatus GetStatus()
    {
        return new PacketManagerStatus(
            Volatile.Read(ref buffersInUse),
            Interlocked.Read(ref buffersRented),
            Interlocked.Read(ref buffersReturned),
            Interlocked.Read(ref packetsProcessed));
    }
}