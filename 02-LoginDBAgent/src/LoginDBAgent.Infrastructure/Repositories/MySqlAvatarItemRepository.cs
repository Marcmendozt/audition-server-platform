using LoginDBAgent.Application.Abstractions;
using LoginDBAgent.Domain.Entities;
using LoginDBAgent.Infrastructure.Database;
using MySqlConnector;

namespace LoginDBAgent.Infrastructure.Repositories;

public sealed class MySqlAvatarItemRepository(MySqlConnectionFactory connectionFactory) : IAvatarItemRepository
{
    public async Task InsertAsync(uint userSN, int itemId, int days, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = """
            INSERT INTO AvatarItemList (UserSN, AvatarItem, EquipState, RegDate, ExpireDate) 
            VALUES (@sn, @item, 'INV', NOW(), DATE_ADD(NOW(), INTERVAL @d DAY))
            """;
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);
        cmd.Parameters.AddWithValue("@item", itemId);
        cmd.Parameters.AddWithValue("@d", days);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task DeleteAsync(uint userSN, int itemId, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "DELETE FROM AvatarItemList WHERE UserSN = @sn AND AvatarItem = @item";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);
        cmd.Parameters.AddWithValue("@item", itemId);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<bool> HasActiveItemAsync(uint userSN, int itemId, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "SELECT COUNT(1) FROM AvatarItemList WHERE UserSN = @sn AND AvatarItem = @item AND ExpireDate > NOW()";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);
        cmd.Parameters.AddWithValue("@item", itemId);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct)) > 0;
    }

    public async Task<bool> HasUnlimitedItemAsync(uint userSN, int itemId, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "SELECT COUNT(1) FROM AvatarItemList WHERE UserSN = @sn AND AvatarItem = @item AND ExpireDate >= '2090-01-01 00:00:00'";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);
        cmd.Parameters.AddWithValue("@item", itemId);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct)) > 0;
    }

    public async Task SetEquipStateAsync(uint userSN, int itemId, string state, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "UPDATE AvatarItemList SET EquipState = @state WHERE UserSN = @sn AND AvatarItem = @item";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@state", state);
        cmd.Parameters.AddWithValue("@sn", userSN);
        cmd.Parameters.AddWithValue("@item", itemId);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<AvatarItem>> GetEquippedItemsAsync(uint userSN, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "SELECT AvatarItem, EquipState, RegDate, ExpireDate FROM AvatarItemList WHERE UserSN = @sn AND EquipState = 'EQUIP' AND ExpireDate > NOW()";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);
        return await ReadAvatarItemsAsync(userSN, cmd, ct);
    }

    public async Task<IReadOnlyList<AvatarItem>> GetInventoryItemsAsync(uint userSN, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "SELECT AvatarItem, EquipState, RegDate, ExpireDate FROM AvatarItemList WHERE UserSN = @sn AND EquipState = 'INV' AND ExpireDate > NOW()";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);
        return await ReadAvatarItemsAsync(userSN, cmd, ct);
    }

    public async Task<IReadOnlyList<AvatarItem>> GetAllItemsAsync(uint userSN, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);
        const string sql = "SELECT AvatarItem, EquipState, RegDate, ExpireDate FROM AvatarItemList WHERE UserSN = @sn ORDER BY ExpireDate DESC";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);
        return await ReadAvatarItemsAsync(userSN, cmd, ct);
    }

    private static async Task<IReadOnlyList<AvatarItem>> ReadAvatarItemsAsync(uint userSN, MySqlCommand cmd, CancellationToken ct)
    {
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<AvatarItem>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new AvatarItem(
                userSN,
                reader.GetInt32("AvatarItem"),
                reader.GetString("EquipState"),
                reader.GetDateTime("RegDate"),
                reader.GetDateTime("ExpireDate")));
        }
        return items;
    }
}
