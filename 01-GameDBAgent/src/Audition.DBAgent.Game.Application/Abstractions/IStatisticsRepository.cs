namespace Audition.DBAgent.Game.Application.Abstractions;

public interface IStatisticsRepository
{
    Task SaveDayUniqueCountAsync(CancellationToken ct);
}
