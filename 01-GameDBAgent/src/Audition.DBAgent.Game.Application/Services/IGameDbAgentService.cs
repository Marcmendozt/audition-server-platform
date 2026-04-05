using Audition.DBAgent.Game.Application.Contracts;
using Audition.DBAgent.Game.Domain.Entities;

namespace Audition.DBAgent.Game.Application.Services;

public interface IGameDbAgentService
{
    Task LoadLevelQuestDataAsync(CancellationToken ct);
    IReadOnlyList<LevelQuest> GetCachedLevelQuests();

    Task<UserProfileResult?> GetUserProfileAsync(uint userSN, CancellationToken ct);
    Task<int> GetCashAsync(uint userSN, CancellationToken ct);
    Task<(int Cash, byte Gender)?> GetCashAndGenderAsync(uint userSN, CancellationToken ct);

    Task<bool> HasActiveItemAsync(uint userSN, int itemId, CancellationToken ct);
    Task<bool> HasUnlimitedItemAsync(uint userSN, int itemId, CancellationToken ct);
    Task<bool> HasPendingPresentAsync(uint userSN, int itemId, CancellationToken ct);

    Task PurchaseItemAsync(PurchaseCommand command, CancellationToken ct);
    Task UpdateGameResultsAsync(GameResultCommand command, CancellationToken ct);
    Task<QuestResult> ValidateAndCompleteQuestAsync(QuestAttemptCommand command, CancellationToken ct);
    Task LogLevelQuestAsync(LevelQuestLogCommand command, CancellationToken ct);

    Task SaveDayUniqueCountAsync(CancellationToken ct);
}
