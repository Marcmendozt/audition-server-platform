using LoginDBAgent.Application.Abstractions;
using LoginDBAgent.Domain.Entities;
using LoginDBAgent.Infrastructure.Database;
using MySqlConnector;

namespace LoginDBAgent.Infrastructure.Repositories;

public sealed class MySqlFriendRepository(MySqlConnectionFactory connectionFactory) : IFriendRepository
{
    public async Task<int> GetFriendCountAsync(string userNick, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "SELECT COUNT(*) FROM UserFriends WHERE UserNick = @nick AND FriendState = 'admit'";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@nick", userNick);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }

    public async Task<IReadOnlyList<FriendEntry>> GetFriendListAsync(string userNick, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "SELECT FriendNick FROM UserFriends WHERE UserNick = @nick AND FriendState = 'admit'";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@nick", userNick);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var friends = new List<FriendEntry>();
        while (await reader.ReadAsync(ct))
        {
            friends.Add(new FriendEntry(
                userNick,
                reader.GetString("FriendNick"),
                "admit"));
        }
        return friends;
    }
}
