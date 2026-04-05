namespace LoginDBAgent.Application.Abstractions;

public interface IStatisticsRepository
{
    Task SaveDayUniqueCountAsync(CancellationToken ct);
    Task SaveUserCountAsync(int count, CancellationToken ct);
    Task InsertLoginInfoAsync(uint userSN, string userNick, string type, CancellationToken ct);
}
