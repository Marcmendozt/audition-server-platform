using GatewayAudition.Domain.Entities;
using GatewayAudition.Domain.Interfaces;

namespace GatewayAudition.Infrastructure.Repositories;

public sealed class InMemoryPacketManager : IPacketManager
{
    private readonly object _lockFree = new();
    private readonly object _lockUsing = new();
    private readonly Queue<Packet> _freeList = new();
    private readonly List<Packet> _usingList = new();
    private uint _bufferSize;

    public void Initialize(uint maxPackets, uint bufferSize)
    {
        _bufferSize = bufferSize;
        lock (_lockFree)
        {
            _freeList.Clear();
            for (uint i = 0; i < maxPackets; i++)
            {
                var packet = new Packet();
                packet.Initialize(bufferSize);
                _freeList.Enqueue(packet);
            }
        }
    }

    public Packet? AcquirePacket()
    {
        Packet? packet;
        lock (_lockFree)
        {
            if (!_freeList.TryDequeue(out packet))
                return null;
        }

        lock (_lockUsing)
        {
            _usingList.Add(packet);
        }

        packet.Reset();
        return packet;
    }

    public void ReleasePacket(Packet packet)
    {
        lock (_lockUsing)
        {
            _usingList.Remove(packet);
        }

        lock (_lockFree)
        {
            packet.Reset();
            _freeList.Enqueue(packet);
        }
    }
}
