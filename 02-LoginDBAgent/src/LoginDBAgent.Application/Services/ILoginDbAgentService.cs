using LoginDBAgent.Application.Contracts;
using LoginDBAgent.Domain.Entities;

namespace LoginDBAgent.Application.Services;

public interface ILoginDbAgentService
{
    // Startup
    Task LoadLevelQuestDataAsync(CancellationToken ct);
    IReadOnlyList<LevelQuest> GetCachedLevelQuests();

    // Authentication (AgentLoginDB)
    Task<LoginResult> ValidateLoginAsync(LoginCommand command, CancellationToken ct);

    // User Profile (requestUserInfo, requestGameInfo)
    Task<UserProfileResult?> GetUserProfileAsync(uint userSN, CancellationToken ct);
    Task<int> GetCashAsync(uint userSN, CancellationToken ct);
    Task<(int Cash, byte Gender)?> GetCashAndGenderAsync(uint userSN, CancellationToken ct);
    Task<int> GetExpAsync(uint userSN, CancellationToken ct);

    // Game results (saveUserInfo)
    Task UpdateGameResultsAsync(GameResultCommand command, CancellationToken ct);
    Task AddMoneyAsync(uint userSN, int amount, CancellationToken ct);

    // Avatar/Item system
    Task PurchaseItemAsync(uint userSN, int itemId, int days, CancellationToken ct);
    Task DeleteItemAsync(uint userSN, int itemId, CancellationToken ct);
    Task<bool> HasActiveItemAsync(uint userSN, int itemId, CancellationToken ct);
    Task<bool> HasUnlimitedItemAsync(uint userSN, int itemId, CancellationToken ct);
    Task<bool> HasPendingPresentAsync(uint userSN, int itemId, CancellationToken ct);
    Task EquipItemAsync(AvatarEquipCommand command, CancellationToken ct);
    Task<IReadOnlyList<AvatarItem>> GetEquippedItemsAsync(uint userSN, CancellationToken ct);
    Task<IReadOnlyList<AvatarItem>> GetInventoryItemsAsync(uint userSN, CancellationToken ct);

    // Quest system
    Task<QuestResult> ValidateAndCompleteQuestAsync(QuestAttemptCommand command, CancellationToken ct);

    // Ranking
    Task<IReadOnlyList<RankEntry>> GetTopRankingsAsync(int offset, int limit, CancellationToken ct);
    Task<RankEntry?> GetUserRankAsync(string userNick, CancellationToken ct);

    // Friends
    Task<int> GetFriendCountAsync(string userNick, CancellationToken ct);
    Task<IReadOnlyList<FriendEntry>> GetFriendListAsync(string userNick, CancellationToken ct);

    // Statistics
    Task SaveDayUniqueCountAsync(CancellationToken ct);
    Task RecordLoginAsync(uint userSN, string userNick, CancellationToken ct);
    Task RecordLogoutAsync(uint userSN, string userNick, CancellationToken ct);
}
