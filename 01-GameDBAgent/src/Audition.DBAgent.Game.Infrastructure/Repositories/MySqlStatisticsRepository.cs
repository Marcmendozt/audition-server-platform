using Audition.DBAgent.Game.Application.Abstractions;
using Audition.DBAgent.Game.Infrastructure.Database;
using MySqlConnector;

namespace Audition.DBAgent.Game.Infrastructure.Repositories;

public sealed class MySqlStatisticsRepository(MySqlConnectionFactory connectionFactory) : IStatisticsRepository
{
    public async Task SaveDayUniqueCountAsync(CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);

        const string sql = """
            INSERT INTO DayUniqueCount (SN, Time, UserCount) 
            SELECT NULL, NOW(), COUNT(DISTINCT U.UserNick) 
            FROM LoginInfo AS L 
            INNER JOIN UserInfo AS U ON L.UserSN = U.UserSN
            WHERE L.Time >= DATE_SUB(NOW(), INTERVAL 1 DAY) 
              AND L.Time < NOW() 
              AND L.Type = 'LOGIN'
            """;

        await using var cmd = new MySqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
