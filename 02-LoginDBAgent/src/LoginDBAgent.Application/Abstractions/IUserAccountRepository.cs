using LoginDBAgent.Domain.Entities;

namespace LoginDBAgent.Application.Abstractions;

public interface IUserAccountRepository
{
    Task<UserAccount?> GetByUserIdAsync(string userId, CancellationToken ct);
    Task<UserAccount?> GetByUserSNAsync(uint userSN, CancellationToken ct);
}
