using LoginDBAgent.Application.Abstractions;
using LoginDBAgent.Domain.Entities;
using LoginDBAgent.Infrastructure.Database;
using MySqlConnector;

namespace LoginDBAgent.Infrastructure.Repositories;

public sealed class MySqlRankRepository(MySqlConnectionFactory connectionFactory) : IRankRepository
{
    public async Task<IReadOnlyList<RankEntry>> GetTopRankingsAsync(int offset, int limit, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "SELECT UserNick, Exp FROM rank LIMIT @offset, @limit";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@offset", offset);
        cmd.Parameters.AddWithValue("@limit", limit);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var entries = new List<RankEntry>();
        int position = offset + 1;
        while (await reader.ReadAsync(ct))
        {
            entries.Add(new RankEntry(
                reader.GetString("UserNick"),
                reader.GetInt32("Exp"),
                position++));
        }
        return entries;
    }

    public async Task<RankEntry?> GetUserRankAsync(string userNick, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "SELECT UserNick, Exp FROM rank WHERE UserNick = @nick";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@nick", userNick);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        if (!await reader.ReadAsync(ct)) return null;
        return new RankEntry(reader.GetString("UserNick"), reader.GetInt32("Exp"), 0);
    }

    public async Task UpdateRankAsync(CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = """
            REPLACE INTO rank (UserNick, Exp)
            SELECT UserNick, Exp FROM UserInfo ORDER BY Exp DESC LIMIT 100
            """;
        await using var cmd = new MySqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
