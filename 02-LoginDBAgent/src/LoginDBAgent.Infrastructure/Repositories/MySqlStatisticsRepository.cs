using LoginDBAgent.Application.Abstractions;
using LoginDBAgent.Infrastructure.Database;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace LoginDBAgent.Infrastructure.Repositories;

public sealed class MySqlStatisticsRepository(MySqlConnectionFactory connectionFactory, IOptions<DatabaseOptions> dbOptions) : IStatisticsRepository
{
    private string LoginDb => dbOptions.Value.LoginDatabase;
    private string LogDb => dbOptions.Value.LogDatabase;

    public async Task SaveDayUniqueCountAsync(CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        var sql = $"""
            INSERT INTO {LogDb}.dayusercount (Time, ChannelNum, UserCount, ServerNum) 
            SELECT NOW(), 0, COUNT(DISTINCT U.UserNick), 0
            FROM {LoginDb}.logininfo AS L 
            INNER JOIN userinfo AS U ON L.UserSN = U.UserSN
            WHERE L.Time >= DATE_SUB(NOW(), INTERVAL 1 DAY) 
              AND L.Time < NOW() 
              AND L.Type = 'LOGIN'
            """;
        await using var cmd = new MySqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task SaveUserCountAsync(int count, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        var sql = $"INSERT INTO {LogDb}.dayusercount (Time, ChannelNum, UserCount, ServerNum) VALUES (NOW(), 0, @count, 0)";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@count", count);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task InsertLoginInfoAsync(uint userSN, string userNick, string type, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        var sql = $"INSERT INTO {LoginDb}.logininfo (UserSN, ServerNum, Time, Type, IP) VALUES (@sn, 0, NOW(), @type, '127.0.0.1')";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);
        cmd.Parameters.AddWithValue("@type", type);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
