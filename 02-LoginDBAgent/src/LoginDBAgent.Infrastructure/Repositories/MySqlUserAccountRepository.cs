using LoginDBAgent.Application.Abstractions;
using LoginDBAgent.Domain.Entities;
using LoginDBAgent.Infrastructure.Database;
using MySqlConnector;

namespace LoginDBAgent.Infrastructure.Repositories;

public sealed class MySqlUserAccountRepository(MySqlConnectionFactory connectionFactory) : IUserAccountRepository
{
    public async Task<UserAccount?> GetByUserIdAsync(string userId, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);

        const string sql = """
            SELECT u.UserSN, u.UserID, ui.Password, ui.UserNick, u.UserGender
            FROM Users u
            INNER JOIN UserInfo ui ON u.UserSN = ui.UserSN
            WHERE u.UserID = @uid
            LIMIT 1
            """;

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@uid", userId);

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;

        return new UserAccount(
            reader.GetUInt32("UserSN"),
            reader.GetString("UserID"),
            reader.GetString("Password"),
            reader.GetString("UserNick"),
            ParseGender(reader, "UserGender"));
    }

    public async Task<UserAccount?> GetByUserSNAsync(uint userSN, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);

        const string sql = "SELECT * FROM Users WHERE UserSN = @sn";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;

        return new UserAccount(
            reader.GetUInt32("UserSN"),
            reader.GetString("UserID"),
            "",
            "",
            ParseGender(reader, "UserGender"));
    }

    private static byte ParseGender(MySqlDataReader reader, string column)
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
            string s when s.Length == 1 => (byte)s[0],
            _ => 0
        };
    }
}
