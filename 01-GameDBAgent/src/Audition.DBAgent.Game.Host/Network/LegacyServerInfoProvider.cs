using Audition.DBAgent.Game.Infrastructure.Database;
using MySqlConnector;

namespace Audition.DBAgent.Game.Host.Network;

public interface ILegacyServerInfoProvider
{
    Task<LegacyServerInfoData?> GetServerInfoAsync(ushort serverNumber, CancellationToken ct);
    Task<IReadOnlyList<LegacyChannelInfoData>> GetChannelInfosAsync(ushort serverNumber, CancellationToken ct);
}

public sealed record LegacyServerInfoData(
    ushort ServerNumber,
    string Name,
    string IpAddress,
    ushort Port,
    ushort CurrentUsers,
    ushort MaxUsers,
    ushort Grade,
    ushort IpRestriction,
    ushort DoubleDen);

public sealed record LegacyChannelInfoData(
    ushort Number,
    string Name,
    ushort MaxUsers,
    ushort MaxRooms,
    ushort MinLevel,
    ushort MaxLevel,
    byte EventNumber);

public sealed class MySqlLegacyServerInfoProvider(MySqlConnectionFactory connectionFactory) : ILegacyServerInfoProvider
{
    public async Task<LegacyServerInfoData?> GetServerInfoAsync(ushort serverNumber, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);

        const string sql = """
            SELECT Num, Name, IPAddr, Port, CurUser, MaxUser, Grade, IPRestriction, DoubleDen
            FROM serverlist
            WHERE Num = @serverNumber
            LIMIT 1
            """;

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@serverNumber", serverNumber);

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
        {
            return null;
        }

        return new LegacyServerInfoData(
            Convert.ToUInt16(reader["Num"]),
            reader.GetString("Name"),
            reader.GetString("IPAddr"),
            Convert.ToUInt16(reader["Port"]),
            Convert.ToUInt16(reader["CurUser"]),
            Convert.ToUInt16(reader["MaxUser"]),
            Convert.ToUInt16(reader["Grade"]),
            Convert.ToUInt16(reader["IPRestriction"]),
            Convert.ToUInt16(reader["DoubleDen"]));
    }

    public async Task<IReadOnlyList<LegacyChannelInfoData>> GetChannelInfosAsync(ushort serverNumber, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);

        const string sql = """
            SELECT Num, Name, UserCount, RoomCount, MinLevel, MaxLevel, EventNum
            FROM channellist
            WHERE ServerNum = @serverNumber
            ORDER BY Num
            """;

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@serverNumber", serverNumber);

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        var channels = new List<LegacyChannelInfoData>();

        while (await reader.ReadAsync(ct))
        {
            channels.Add(new LegacyChannelInfoData(
                Convert.ToUInt16(reader["Num"]),
                reader.GetString("Name"),
                Convert.ToUInt16(reader["UserCount"]),
                Convert.ToUInt16(reader["RoomCount"]),
                Convert.ToUInt16(reader["MinLevel"]),
                Convert.ToUInt16(reader["MaxLevel"]),
                Convert.ToByte(reader["EventNum"])));
        }

        return channels;
    }
}