using LoginDBAgent.Domain.Entities;

namespace LoginDBAgent.Application.Abstractions;

public interface ILevelQuestRepository
{
    Task<IReadOnlyList<LevelQuest>> LoadAllAsync(CancellationToken ct);
    Task<bool> HasCompletedQuestAsync(uint userSN, int levelQuestSN, CancellationToken ct);
    Task InsertQuestLogAsync(LevelQuestLogEntry entry, CancellationToken ct);
}
