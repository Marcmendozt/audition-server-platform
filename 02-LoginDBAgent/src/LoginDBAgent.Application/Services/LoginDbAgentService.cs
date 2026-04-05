using LoginDBAgent.Application.Abstractions;
using LoginDBAgent.Application.Contracts;
using LoginDBAgent.Domain.Entities;

namespace LoginDBAgent.Application.Services;

public sealed class LoginDbAgentService(
    IUserAccountRepository userAccountRepo,
    IUserInfoRepository userInfoRepo,
    ILevelQuestRepository levelQuestRepo,
    IAvatarItemRepository avatarItemRepo,
    IPresentRepository presentRepo,
    IRankRepository rankRepo,
    IFriendRepository friendRepo,
    IStatisticsRepository statisticsRepo) : ILoginDbAgentService
{
    private IReadOnlyList<LevelQuest> _levelQuests = [];

    // --- Startup ---

    public async Task LoadLevelQuestDataAsync(CancellationToken ct)
    {
        _levelQuests = await levelQuestRepo.LoadAllAsync(ct);
    }

    public IReadOnlyList<LevelQuest> GetCachedLevelQuests() => _levelQuests;

    // --- Authentication (AgentLoginDB) ---

    public async Task<LoginResult> ValidateLoginAsync(LoginCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.UserId) || string.IsNullOrWhiteSpace(command.Password))
        {
            return new LoginResult(false, 0, "", 0, "Empty credentials");
        }

        var account = await userAccountRepo.GetByUserIdAsync(command.UserId, ct);
        if (account is null)
        {
            return new LoginResult(false, 0, "", 0, "User not found");
        }

        if (account.Password != command.Password)
        {
            return new LoginResult(false, 0, "", 0, "Invalid password");
        }

        var userInfo = await userInfoRepo.GetByUserSNAsync(account.UserSN, ct);
        int cash = userInfo?.Cash ?? 0;

        return new LoginResult(true, account.UserSN, account.UserNick, cash, null);
    }

    // --- User Profile ---

    public async Task<UserProfileResult?> GetUserProfileAsync(uint userSN, CancellationToken ct)
    {
        var user = await userInfoRepo.GetByUserSNAsync(userSN, ct);
        if (user is null) return null;

        return new UserProfileResult(
            user.UserSN, user.Exp, user.Money, user.Cash, user.Level, user.IsAllowMsg);
    }

    public Task<int> GetCashAsync(uint userSN, CancellationToken ct)
        => userInfoRepo.GetCashAsync(userSN, ct);

    public Task<(int Cash, byte Gender)?> GetCashAndGenderAsync(uint userSN, CancellationToken ct)
        => userInfoRepo.GetCashAndGenderAsync(userSN, ct);

    public Task<int> GetExpAsync(uint userSN, CancellationToken ct)
        => userInfoRepo.GetExpAsync(userSN, ct);

    // --- Game Results ---

    public async Task UpdateGameResultsAsync(GameResultCommand command, CancellationToken ct)
    {
        await userInfoRepo.AddExpAndMoneyAsync(command.UserSN, command.ExpGain, command.MoneyGain, ct);
    }

    public Task AddMoneyAsync(uint userSN, int amount, CancellationToken ct)
        => userInfoRepo.AddMoneyAsync(userSN, amount, ct);

    // --- Avatar/Item System ---

    public async Task PurchaseItemAsync(uint userSN, int itemId, int days, CancellationToken ct)
    {
        await avatarItemRepo.InsertAsync(userSN, itemId, days, ct);
    }

    public Task DeleteItemAsync(uint userSN, int itemId, CancellationToken ct)
        => avatarItemRepo.DeleteAsync(userSN, itemId, ct);

    public Task<bool> HasActiveItemAsync(uint userSN, int itemId, CancellationToken ct)
        => avatarItemRepo.HasActiveItemAsync(userSN, itemId, ct);

    public Task<bool> HasUnlimitedItemAsync(uint userSN, int itemId, CancellationToken ct)
        => avatarItemRepo.HasUnlimitedItemAsync(userSN, itemId, ct);

    public Task<bool> HasPendingPresentAsync(uint userSN, int itemId, CancellationToken ct)
        => presentRepo.HasPendingPresentAsync(userSN, itemId, ct);

    public async Task EquipItemAsync(AvatarEquipCommand command, CancellationToken ct)
    {
        await avatarItemRepo.SetEquipStateAsync(command.UserSN, command.ItemId, command.EquipState, ct);
    }

    public Task<IReadOnlyList<AvatarItem>> GetEquippedItemsAsync(uint userSN, CancellationToken ct)
        => avatarItemRepo.GetEquippedItemsAsync(userSN, ct);

    public Task<IReadOnlyList<AvatarItem>> GetInventoryItemsAsync(uint userSN, CancellationToken ct)
        => avatarItemRepo.GetInventoryItemsAsync(userSN, ct);

    // --- Quest System ---

    public async Task<QuestResult> ValidateAndCompleteQuestAsync(QuestAttemptCommand command, CancellationToken ct)
    {
        var quest = _levelQuests.FirstOrDefault(q => q.Level == command.LevelQuestSN);
        if (quest is null)
            return new QuestResult(false, "Quest not found", 0, 0, 0);

        bool alreadyDone = await levelQuestRepo.HasCompletedQuestAsync(command.UserSN, command.LevelQuestSN, ct);
        if (alreadyDone)
            return new QuestResult(false, "Quest already completed", 0, 0, 0);

        var user = await userInfoRepo.GetByUserSNAsync(command.UserSN, ct);
        if (user is null)
            return new QuestResult(false, "User not found", 0, 0, 0);

        if (user.Money < quest.Fee)
            return new QuestResult(false, "Insufficient Money for quest fee", user.Exp, user.Money, user.Level);

        int newMoney = user.Money - quest.Fee + quest.WinDen;
        int newExp = user.Exp + quest.WinExp;

        // Level-up check from LevelInfo RequiredExp
        int newLevel = user.Level;
        foreach (var lq in _levelQuests.Where(q => q.Level > user.Level).OrderBy(q => q.Level))
        {
            if (newExp >= lq.RequiredExp)
                newLevel = lq.Level;
            else
                break;
        }

        await userInfoRepo.DeductMoneyAsync(command.UserSN, quest.Fee, ct);
        await userInfoRepo.UpdateExpAndMoneyAsync(command.UserSN, newExp, newMoney, ct);

        if (newLevel > user.Level)
            await userInfoRepo.UpdateLevelAsync(command.UserSN, newLevel, ct);

        int reward = quest.WinDen + quest.WinExp;
        await levelQuestRepo.InsertQuestLogAsync(new LevelQuestLogEntry(
            command.UserSN, command.LevelQuestSN, command.Score,
            command.Perfect, command.GameMode, reward, 1), ct);

        return new QuestResult(true, null, newExp, newMoney, newLevel);
    }

    // --- Ranking ---

    public Task<IReadOnlyList<RankEntry>> GetTopRankingsAsync(int offset, int limit, CancellationToken ct)
        => rankRepo.GetTopRankingsAsync(offset, limit, ct);

    public Task<RankEntry?> GetUserRankAsync(string userNick, CancellationToken ct)
        => rankRepo.GetUserRankAsync(userNick, ct);

    // --- Friends ---

    public Task<int> GetFriendCountAsync(string userNick, CancellationToken ct)
        => friendRepo.GetFriendCountAsync(userNick, ct);

    public Task<IReadOnlyList<FriendEntry>> GetFriendListAsync(string userNick, CancellationToken ct)
        => friendRepo.GetFriendListAsync(userNick, ct);

    // --- Statistics ---

    public Task SaveDayUniqueCountAsync(CancellationToken ct)
        => statisticsRepo.SaveDayUniqueCountAsync(ct);

    public Task RecordLoginAsync(uint userSN, string userNick, CancellationToken ct)
        => statisticsRepo.InsertLoginInfoAsync(userSN, userNick, "LOGIN", ct);

    public Task RecordLogoutAsync(uint userSN, string userNick, CancellationToken ct)
        => statisticsRepo.InsertLoginInfoAsync(userSN, userNick, "LOGOUT", ct);
}
