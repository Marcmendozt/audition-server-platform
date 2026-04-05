using LoginDBAgent.Application.Abstractions;
using LoginDBAgent.Infrastructure.Database;
using MySqlConnector;

namespace LoginDBAgent.Infrastructure.Repositories;

public sealed class MySqlPresentRepository(MySqlConnectionFactory connectionFactory) : IPresentRepository
{
    public async Task<bool> HasPendingPresentAsync(uint userSN, int itemId, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = """
            SELECT COUNT(1) FROM Present 
            WHERE RecvSN = @sn AND ItemID = @item AND Period = 365 AND RecvDate = '0000-00-00 00:00:00'
            """;
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);
        cmd.Parameters.AddWithValue("@item", itemId);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct)) > 0;
    }
}
