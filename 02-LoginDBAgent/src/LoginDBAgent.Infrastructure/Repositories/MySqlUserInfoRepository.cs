using LoginDBAgent.Application.Abstractions;
using LoginDBAgent.Domain.Entities;
using LoginDBAgent.Infrastructure.Database;
using MySqlConnector;

namespace LoginDBAgent.Infrastructure.Repositories;

public sealed class MySqlUserInfoRepository(MySqlConnectionFactory connectionFactory) : IUserInfoRepository
{
    public async Task<UserInfo?> GetByUserSNAsync(uint userSN, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);

        const string sql = "SELECT Exp, Money, Cash, Level, IsAllowMsg FROM UserInfo WHERE UserSN = @sn";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;

        return new UserInfo(
            userSN,
            reader.GetInt32("Exp"),
            reader.GetInt32("Money"),
            reader.GetInt32("Cash"),
            reader.GetInt32("Level"),
            ParseBool(reader, "IsAllowMsg"));
    }

    public async Task<int> GetCashAsync(uint userSN, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "SELECT Cash FROM UserInfo WHERE UserSN = @sn";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);
        var result = await cmd.ExecuteScalarAsync(ct);
        return result is not null ? Convert.ToInt32(result) : 0;
    }

    public async Task<(int Cash, byte Gender)?> GetCashAndGenderAsync(uint userSN, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = """
            SELECT ui.Cash, u.UserGender
            FROM UserInfo ui
            INNER JOIN Users u ON u.UserSN = ui.UserSN
            WHERE ui.UserSN = @sn
            """;
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;
        return (reader.GetInt32("Cash"), ParseByte(reader, "UserGender"));
    }

    public async Task<int> GetExpAsync(uint userSN, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "SELECT Exp FROM UserInfo WHERE UserSN = @sn";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);
        var result = await cmd.ExecuteScalarAsync(ct);
        return result is not null ? Convert.ToInt32(result) : 0;
    }

    public async Task UpdateExpAndMoneyAsync(uint userSN, int exp, int money, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "UPDATE UserInfo SET Exp = @exp, Money = @money WHERE UserSN = @sn";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@exp", exp);
        cmd.Parameters.AddWithValue("@money", money);
        cmd.Parameters.AddWithValue("@sn", userSN);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task UpdateLevelAsync(uint userSN, int level, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "UPDATE UserInfo SET Level = @level WHERE UserSN = @sn";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@level", level);
        cmd.Parameters.AddWithValue("@sn", userSN);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task DeductMoneyAsync(uint userSN, int amount, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "UPDATE UserInfo SET Money = Money - @amount WHERE UserSN = @sn";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@amount", amount);
        cmd.Parameters.AddWithValue("@sn", userSN);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task SetMoneyToZeroAsync(uint userSN, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "UPDATE UserInfo SET Money = 0 WHERE UserSN = @sn";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task AddExpAndMoneyAsync(uint userSN, int expGain, int moneyGain, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "UPDATE UserInfo SET Exp = Exp + @exp, Money = Money + @money WHERE UserSN = @sn";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@exp", expGain);
        cmd.Parameters.AddWithValue("@money", moneyGain);
        cmd.Parameters.AddWithValue("@sn", userSN);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task AddMoneyAsync(uint userSN, int amount, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "UPDATE UserInfo SET Money = Money + @amount WHERE UserSN = @sn";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@amount", amount);
        cmd.Parameters.AddWithValue("@sn", userSN);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static bool ParseBool(MySqlDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        if (reader.IsDBNull(ordinal)) return false;
        var value = reader.GetValue(ordinal);
        return value switch
        {
            bool b => b,
            sbyte sb => sb != 0,
            byte b => b != 0,
            int i => i != 0,
            long l => l != 0,
            string s => s is "1" or "true" or "True" or "Y" or "y",
            _ => false
        };
    }

    private static byte ParseByte(MySqlDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        if (reader.IsDBNull(ordinal)) return 0;
        var value = reader.GetValue(ordinal);
        return value switch
        {
            byte b => b,
            sbyte sb => (byte)sb,
            int i => (byte)i,
            long l => (byte)l,
            string s when byte.TryParse(s, out var parsed) => parsed,
            _ => 0
        };
    }
}
