using LoginDBAgent.Domain.Entities;

namespace LoginDBAgent.Application.Abstractions;

public interface IFriendRepository
{
    Task<int> GetFriendCountAsync(string userNick, CancellationToken ct);
    Task<IReadOnlyList<FriendEntry>> GetFriendListAsync(string userNick, CancellationToken ct);
}
