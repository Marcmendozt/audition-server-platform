using LoginDBAgent.Domain.Entities;

namespace LoginDBAgent.Application.Abstractions;

public interface IRankRepository
{
    Task<IReadOnlyList<RankEntry>> GetTopRankingsAsync(int offset, int limit, CancellationToken ct);
    Task<RankEntry?> GetUserRankAsync(string userNick, CancellationToken ct);
    Task UpdateRankAsync(CancellationToken ct);
}
