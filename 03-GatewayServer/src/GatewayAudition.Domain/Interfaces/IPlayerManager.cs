using GatewayAudition.Domain.Entities;

namespace GatewayAudition.Domain.Interfaces;

public interface IPlayerManager
{
    void AddPlayer(int uniqueIndex, User user);
    void RemovePlayer(int uniqueIndex);
    User? GetPlayer(int uniqueIndex);
    int PlayerCount { get; }
}
