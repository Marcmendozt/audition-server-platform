using Audition.DBAgent.Game.Application.Abstractions;
using Audition.DBAgent.Game.Infrastructure.Database;
using MySqlConnector;

namespace Audition.DBAgent.Game.Infrastructure.Repositories;

public sealed class MySqlAvatarItemRepository(MySqlConnectionFactory connectionFactory) : IAvatarItemRepository
{
    public async Task InsertAsync(uint userSN, int itemId, int days, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);

        const string sql = """
            INSERT INTO AvatarItemList (UserSN, AvatarItem, RegDate, ExpireDate) 
            VALUES (@sn, @item, NOW(), DATE_ADD(NOW(), INTERVAL @d DAY))
            """;

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);
        cmd.Parameters.AddWithValue("@item", itemId);
        cmd.Parameters.AddWithValue("@d", days);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<bool> HasActiveItemAsync(uint userSN, int itemId, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);

        const string sql = """
            SELECT COUNT(1) FROM AvatarItemList 
            WHERE UserSN = @sn AND AvatarItem = @item AND ExpireDate > NOW()
            """;

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);
        cmd.Parameters.AddWithValue("@item", itemId);

        var count = Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
        return count > 0;
    }

    public async Task<bool> HasUnlimitedItemAsync(uint userSN, int itemId, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);

        const string sql = """
            SELECT COUNT(1) FROM AvatarItemList 
            WHERE UserSN = @sn AND AvatarItem = @item AND ExpireDate >= '2090-01-01 00:00:00'
            """;

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);
        cmd.Parameters.AddWithValue("@item", itemId);

        var count = Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
        return count > 0;
    }
}
