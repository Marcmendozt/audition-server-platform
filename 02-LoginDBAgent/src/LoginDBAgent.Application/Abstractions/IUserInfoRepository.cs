using LoginDBAgent.Domain.Entities;

namespace LoginDBAgent.Application.Abstractions;

public interface IUserInfoRepository
{
    Task<UserInfo?> GetByUserSNAsync(uint userSN, CancellationToken ct);
    Task<int> GetCashAsync(uint userSN, CancellationToken ct);
    Task<(int Cash, byte Gender)?> GetCashAndGenderAsync(uint userSN, CancellationToken ct);
    Task<int> GetExpAsync(uint userSN, CancellationToken ct);
    Task UpdateExpAndMoneyAsync(uint userSN, int exp, int money, CancellationToken ct);
    Task UpdateLevelAsync(uint userSN, int level, CancellationToken ct);
    Task DeductMoneyAsync(uint userSN, int amount, CancellationToken ct);
    Task SetMoneyToZeroAsync(uint userSN, CancellationToken ct);
    Task AddExpAndMoneyAsync(uint userSN, int expGain, int moneyGain, CancellationToken ct);
    Task AddMoneyAsync(uint userSN, int amount, CancellationToken ct);
}
