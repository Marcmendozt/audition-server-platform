using Audition.DBAgent.Game.Application.Abstractions;
using Audition.DBAgent.Game.Domain.Entities;
using Audition.DBAgent.Game.Infrastructure.Database;
using MySqlConnector;

namespace Audition.DBAgent.Game.Infrastructure.Repositories;

public sealed class MySqlLevelQuestRepository(MySqlConnectionFactory connectionFactory) : ILevelQuestRepository
{
    public async Task<IReadOnlyList<LevelQuest>> LoadAllAsync(CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);

        const string sql = """
            SELECT lq.Level, li.Exp, lq.Score, lq.Perfect, lq.ConsecutivePerfect,
                   lq.GameMode, lq.MusicCode, lq.StageCode, lq.Fee, lq.WinDen, lq.WinExp
            FROM Level_Quest lq
            INNER JOIN LevelInfo li ON lq.Level = li.Level
            """;

        await using var cmd = new MySqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var quests = new List<LevelQuest>();
        while (await reader.ReadAsync(ct))
        {
            quests.Add(new LevelQuest(
                reader.GetInt32("Level"),
                reader.GetInt32("Exp"),
                reader.GetInt32("Score"),
                reader.GetInt32("Perfect"),
                reader.GetInt32("ConsecutivePerfect"),
                reader.GetInt32("GameMode"),
                reader.GetInt32("MusicCode"),
                reader.GetInt32("StageCode"),
                reader.GetInt32("Fee"),
                reader.GetInt32("WinDen"),
                reader.GetInt32("WinExp")));
        }

        return quests;
    }

    public async Task<bool> HasCompletedQuestAsync(uint userSN, int levelQuestSN, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);

        const string sql = "SELECT COUNT(1) FROM LevelQuestLog WHERE UserSN = @sn AND LevelQuestSN = @qsn";
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", userSN);
        cmd.Parameters.AddWithValue("@qsn", levelQuestSN);

        var count = Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
        return count > 0;
    }

    public async Task InsertQuestLogAsync(LevelQuestLogEntry entry, CancellationToken ct)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(ct);

        const string sql = """
            INSERT INTO LevelQuestLog 
            VALUES(NULL, @sn, @qsn, @score, @perfect, @mode, @reward, NULL, @pass)
            """;

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@sn", entry.UserSN);
        cmd.Parameters.AddWithValue("@qsn", entry.LevelQuestSN);
        cmd.Parameters.AddWithValue("@score", entry.Score);
        cmd.Parameters.AddWithValue("@perfect", entry.Perfect);
        cmd.Parameters.AddWithValue("@mode", entry.GameMode);
        cmd.Parameters.AddWithValue("@reward", entry.Reward);
        cmd.Parameters.AddWithValue("@pass", entry.Pass);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
