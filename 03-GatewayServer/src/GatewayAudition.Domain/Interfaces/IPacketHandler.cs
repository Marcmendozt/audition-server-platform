using System.Net.Sockets;
using GatewayAudition.Domain.Entities;

namespace GatewayAudition.Domain.Interfaces;

public interface IPacketHandler
{
    Task<bool> HandlePacketAsync(User user, Session session, Packet packet, NetworkStream stream, CancellationToken cancellationToken);
}
