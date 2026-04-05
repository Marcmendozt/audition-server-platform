using GatewayAudition.Domain.Entities;
using GatewayAudition.Domain.Interfaces;

namespace GatewayAudition.Infrastructure.Repositories;

public sealed class InMemoryPlayerManager : IPlayerManager
{
    private readonly object _lock = new();
    private readonly Dictionary<int, User> _players = new();

    public int PlayerCount
    {
        get { lock (_lock) return _players.Count; }
    }

    public void AddPlayer(int uniqueIndex, User user)
    {
        lock (_lock)
        {
            _players[uniqueIndex] = user;
        }
    }

    public void RemovePlayer(int uniqueIndex)
    {
        lock (_lock)
        {
            _players.Remove(uniqueIndex);
        }
    }

    public User? GetPlayer(int uniqueIndex)
    {
        lock (_lock)
        {
            return _players.GetValueOrDefault(uniqueIndex);
        }
    }
}
