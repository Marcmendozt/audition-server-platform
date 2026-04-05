using GatewayAudition.Domain.Entities;

namespace GatewayAudition.Domain.Interfaces;

public interface IPacketManager
{
    Packet? AcquirePacket();
    void ReleasePacket(Packet packet);
    void Initialize(uint maxPackets, uint bufferSize);
}
