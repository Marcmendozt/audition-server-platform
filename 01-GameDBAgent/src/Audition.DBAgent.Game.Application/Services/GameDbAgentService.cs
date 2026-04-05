using Audition.DBAgent.Game.Application.Abstractions;
using Audition.DBAgent.Game.Application.Contracts;
using Audition.DBAgent.Game.Domain.Entities;

namespace Audition.DBAgent.Game.Application.Services;

public sealed class GameDbAgentService(
    IUserInfoRepository userInfoRepo,
    ILevelQuestRepository levelQuestRepo,
    IAvatarItemRepository avatarItemRepo,
    IPresentRepository presentRepo,
    IStatisticsRepository statisticsRepo) : IGameDbAgentService
{
    private IReadOnlyList<LevelQuest> _levelQuests = [];

    public async Task LoadLevelQuestDataAsync(CancellationToken ct)
    {
        _levelQuests = await levelQuestRepo.LoadAllAsync(ct);
    }

    public IReadOnlyList<LevelQuest> GetCachedLevelQuests() => _levelQuests;

    public async Task<UserProfileResult?> GetUserProfileAsync(uint userSN, CancellationToken ct)
    {
        var user = await userInfoRepo.GetByUserSNAsync(userSN, ct);
        if (user is null) return null;

        return new UserProfileResult(
            user.UserSN,
            user.Exp,
            user.Money,
            user.Cash,
            user.Level,
            user.IsAllowMsg);
    }

    public Task<int> GetCashAsync(uint userSN, CancellationToken ct)
        => userInfoRepo.GetCashAsync(userSN, ct);

    public Task<(int Cash, byte Gender)?> GetCashAndGenderAsync(uint userSN, CancellationToken ct)
        => userInfoRepo.GetCashAndGenderAsync(userSN, ct);

    public Task<bool> HasActiveItemAsync(uint userSN, int itemId, CancellationToken ct)
        => avatarItemRepo.HasActiveItemAsync(userSN, itemId, ct);

    public Task<bool> HasUnlimitedItemAsync(uint userSN, int itemId, CancellationToken ct)
        => avatarItemRepo.HasUnlimitedItemAsync(userSN, itemId, ct);

    public Task<bool> HasPendingPresentAsync(uint userSN, int itemId, CancellationToken ct)
        => presentRepo.HasPendingPresentAsync(userSN, itemId, ct);

    public async Task PurchaseItemAsync(PurchaseCommand command, CancellationToken ct)
    {
        await avatarItemRepo.InsertAsync(command.UserSN, command.ItemId, command.Days, ct);
        if (command.Cost > 0)
        {
            await userInfoRepo.DeductCashAsync(command.UserSN, command.Cost, ct);
        }
    }

    public async Task UpdateGameResultsAsync(GameResultCommand command, CancellationToken ct)
    {
        await userInfoRepo.AddExpAndMoneyAsync(command.UserSN, command.ExpGain, command.MoneyGain, ct);
    }

    /// <summary>
    /// Quest validation and completion following the decompiled native logic:
    /// 1. Check if quest already completed
    /// 2. Verify user has enough Money for the quest Fee
    /// 3. Deduct Fee from Money
    /// 4. Apply rewards (WinExp, WinDen)
    /// 5. Check for level-up based on accumulated Exp
    /// </summary>
    public async Task<QuestResult> ValidateAndCompleteQuestAsync(QuestAttemptCommand command, CancellationToken ct)
    {
        var quest = _levelQuests.FirstOrDefault(q => q.Level == command.LevelQuestSN);
        if (quest is null)
        {
            return new QuestResult(false, "Quest not found", 0, 0, 0);
        }

        bool alreadyDone = await levelQuestRepo.HasCompletedQuestAsync(command.UserSN, command.LevelQuestSN, ct);
        if (alreadyDone)
        {
            return new QuestResult(false, "Quest already completed", 0, 0, 0);
        }

        var user = await userInfoRepo.GetByUserSNAsync(command.UserSN, ct);
        if (user is null)
        {
            return new QuestResult(false, "User not found", 0, 0, 0);
        }

        if (user.Money < quest.Fee)
        {
            return new QuestResult(false, "Insufficient Money for quest fee", user.Exp, user.Money, user.Level);
        }

        int newMoney = user.Money - quest.Fee + quest.WinDen;
        int newExp = user.Exp + quest.WinExp;

        // Determine new level based on accumulated Exp
        int newLevel = user.Level;
        foreach (var lq in _levelQuests.Where(q => q.Level > user.Level).OrderBy(q => q.Level))
        {
            if (newExp >= lq.RequiredExp)
                newLevel = lq.Level;
            else
                break;
        }

        // Deduct fee
        await userInfoRepo.DeductMoneyAsync(command.UserSN, quest.Fee, ct);

        // Apply rewards
        await userInfoRepo.UpdateExpAndMoneyAsync(command.UserSN, newExp, newMoney, ct);

        // Level up if needed
        if (newLevel > user.Level)
        {
            await userInfoRepo.UpdateLevelAsync(command.UserSN, newLevel, ct);
        }

        // Log quest completion
        int reward = quest.WinDen + quest.WinExp;
        await levelQuestRepo.InsertQuestLogAsync(new LevelQuestLogEntry(
            command.UserSN,
            command.LevelQuestSN,
            command.Score,
            command.Perfect,
            command.GameMode,
            reward,
            1), ct);

        return new QuestResult(true, null, newExp, newMoney, newLevel);
    }

    public async Task LogLevelQuestAsync(LevelQuestLogCommand command, CancellationToken ct)
    {
        await levelQuestRepo.InsertQuestLogAsync(new LevelQuestLogEntry(
            command.UserSN,
            command.ProcLevel,
            0,
            0,
            0,
            0,
            command.Pass), ct);
    }

    public Task SaveDayUniqueCountAsync(CancellationToken ct)
        => statisticsRepo.SaveDayUniqueCountAsync(ct);
}
