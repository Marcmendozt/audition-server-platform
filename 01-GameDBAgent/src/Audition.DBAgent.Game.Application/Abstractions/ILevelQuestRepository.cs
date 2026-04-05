using Audition.DBAgent.Game.Domain.Entities;

namespace Audition.DBAgent.Game.Application.Abstractions;

public interface ILevelQuestRepository
{
    Task<IReadOnlyList<LevelQuest>> LoadAllAsync(CancellationToken ct);
    Task<bool> HasCompletedQuestAsync(uint userSN, int levelQuestSN, CancellationToken ct);
    Task InsertQuestLogAsync(LevelQuestLogEntry entry, CancellationToken ct);
}
