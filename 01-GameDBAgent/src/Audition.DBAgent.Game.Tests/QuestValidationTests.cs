using Audition.DBAgent.Game.Application.Abstractions;
using Audition.DBAgent.Game.Application.Contracts;
using Audition.DBAgent.Game.Application.Services;
using Audition.DBAgent.Game.Domain.Entities;
using NSubstitute;
using Xunit;

namespace Audition.DBAgent.Game.Tests;

public class QuestValidationTests
{
    private readonly IUserInfoRepository _userInfoRepo = Substitute.For<IUserInfoRepository>();
    private readonly ILevelQuestRepository _levelQuestRepo = Substitute.For<ILevelQuestRepository>();
    private readonly IAvatarItemRepository _avatarItemRepo = Substitute.For<IAvatarItemRepository>();
    private readonly IPresentRepository _presentRepo = Substitute.For<IPresentRepository>();
    private readonly IStatisticsRepository _statsRepo = Substitute.For<IStatisticsRepository>();
    private readonly GameDbAgentService _sut;

    private static readonly List<LevelQuest> SampleQuests =
    [
        //           Level, ReqExp, Score, Perfect, ConsPerfect, Mode, Music, Stage, Fee, WinDen, WinExp
        new LevelQuest(1,   100,    80,    5,       2,           1,    1001,  1,     50,  200,    100),
        new LevelQuest(2,   500,    85,    8,       3,           1,    1002,  2,     100, 400,    200),
        new LevelQuest(3,   1200,   90,    10,      5,           2,    1003,  3,     200, 600,    350),
    ];

    public QuestValidationTests()
    {
        _sut = new GameDbAgentService(
            _userInfoRepo,
            _levelQuestRepo,
            _avatarItemRepo,
            _presentRepo,
            _statsRepo);

        // Pre-load quests
        _levelQuestRepo.LoadAllAsync(Arg.Any<CancellationToken>()).Returns(SampleQuests);
        _sut.LoadLevelQuestDataAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Quest_Fails_WhenQuestNotFound()
    {
        var command = new QuestAttemptCommand(1001, 99, 80, 5, 1);

        var result = await _sut.ValidateAndCompleteQuestAsync(command, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Quest not found", result.ErrorReason);
    }

    [Fact]
    public async Task Quest_Fails_WhenAlreadyCompleted()
    {
        _levelQuestRepo.HasCompletedQuestAsync(1001, 1, Arg.Any<CancellationToken>()).Returns(true);

        var command = new QuestAttemptCommand(1001, 1, 80, 5, 1);
        var result = await _sut.ValidateAndCompleteQuestAsync(command, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Quest already completed", result.ErrorReason);
    }

    [Fact]
    public async Task Quest_Fails_WhenUserNotFound()
    {
        _levelQuestRepo.HasCompletedQuestAsync(1001, 1, Arg.Any<CancellationToken>()).Returns(false);
        _userInfoRepo.GetByUserSNAsync(1001, Arg.Any<CancellationToken>()).Returns((UserInfo?)null);

        var command = new QuestAttemptCommand(1001, 1, 80, 5, 1);
        var result = await _sut.ValidateAndCompleteQuestAsync(command, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User not found", result.ErrorReason);
    }

    [Fact]
    public async Task Quest_Fails_WhenInsufficientMoney()
    {
        var user = new UserInfo(1001, 200, 30, 100, 1, 1, true); // Money=30, Fee=50
        _levelQuestRepo.HasCompletedQuestAsync(1001, 1, Arg.Any<CancellationToken>()).Returns(false);
        _userInfoRepo.GetByUserSNAsync(1001, Arg.Any<CancellationToken>()).Returns(user);

        var command = new QuestAttemptCommand(1001, 1, 80, 5, 1);
        var result = await _sut.ValidateAndCompleteQuestAsync(command, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Insufficient Money for quest fee", result.ErrorReason);
    }

    [Fact]
    public async Task Quest_Succeeds_WithCorrectRewards()
    {
        // Quest 1: Fee=50, WinDen=200, WinExp=100
        var user = new UserInfo(1001, 50, 500, 100, 1, 1, true); // Exp=50, Money=500
        _levelQuestRepo.HasCompletedQuestAsync(1001, 1, Arg.Any<CancellationToken>()).Returns(false);
        _userInfoRepo.GetByUserSNAsync(1001, Arg.Any<CancellationToken>()).Returns(user);

        var command = new QuestAttemptCommand(1001, 1, 95, 5, 1);
        var result = await _sut.ValidateAndCompleteQuestAsync(command, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Null(result.ErrorReason);
        // newMoney = 500 - 50 (fee) + 200 (WinDen) = 650
        Assert.Equal(650, result.NewMoney);
        // newExp = 50 + 100 (WinExp) = 150
        Assert.Equal(150, result.NewExp);
    }

    [Fact]
    public async Task Quest_TriggersLevelUp_WhenExpThresholdCrossed()
    {
        // Quest 1: Fee=50, WinDen=200, WinExp=100
        // User at Level 1 with Exp=450. After +100 WinExp = 550 >= Level 2 req (500)
        var user = new UserInfo(1001, 450, 1000, 100, 1, 1, true);
        _levelQuestRepo.HasCompletedQuestAsync(1001, 1, Arg.Any<CancellationToken>()).Returns(false);
        _userInfoRepo.GetByUserSNAsync(1001, Arg.Any<CancellationToken>()).Returns(user);

        var command = new QuestAttemptCommand(1001, 1, 95, 5, 1);
        var result = await _sut.ValidateAndCompleteQuestAsync(command, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(2, result.NewLevel);
        // Verify level update was called
        await _userInfoRepo.Received(1).UpdateLevelAsync(1001, 2, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Quest_DoesNotLevelUp_WhenExpBelowThreshold()
    {
        // Quest 1: WinExp=100. User Exp=50, after = 150. Level 2 needs 500.
        var user = new UserInfo(1001, 50, 1000, 100, 1, 1, true);
        _levelQuestRepo.HasCompletedQuestAsync(1001, 1, Arg.Any<CancellationToken>()).Returns(false);
        _userInfoRepo.GetByUserSNAsync(1001, Arg.Any<CancellationToken>()).Returns(user);

        var command = new QuestAttemptCommand(1001, 1, 95, 5, 1);
        var result = await _sut.ValidateAndCompleteQuestAsync(command, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(1, result.NewLevel); // stays at level 1
        await _userInfoRepo.DidNotReceive().UpdateLevelAsync(Arg.Any<uint>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Quest_LogsCompletion_OnSuccess()
    {
        var user = new UserInfo(1001, 50, 500, 100, 1, 1, true);
        _levelQuestRepo.HasCompletedQuestAsync(1001, 1, Arg.Any<CancellationToken>()).Returns(false);
        _userInfoRepo.GetByUserSNAsync(1001, Arg.Any<CancellationToken>()).Returns(user);

        var command = new QuestAttemptCommand(1001, 1, 95, 5, 1);
        await _sut.ValidateAndCompleteQuestAsync(command, CancellationToken.None);

        await _levelQuestRepo.Received(1).InsertQuestLogAsync(
            Arg.Is<LevelQuestLogEntry>(e =>
                e.UserSN == 1001 &&
                e.LevelQuestSN == 1 &&
                e.Score == 95 &&
                e.Perfect == 5 &&
                e.GameMode == 1 &&
                e.Pass == 1),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Quest_DeductsFeeAndAppliesRewards_InOrder()
    {
        var user = new UserInfo(1001, 50, 500, 100, 1, 1, true);
        _levelQuestRepo.HasCompletedQuestAsync(1001, 1, Arg.Any<CancellationToken>()).Returns(false);
        _userInfoRepo.GetByUserSNAsync(1001, Arg.Any<CancellationToken>()).Returns(user);

        var command = new QuestAttemptCommand(1001, 1, 95, 5, 1);
        await _sut.ValidateAndCompleteQuestAsync(command, CancellationToken.None);

        // Fee deduction (50 Money)
        await _userInfoRepo.Received(1).DeductMoneyAsync(1001, 50, Arg.Any<CancellationToken>());
        // Exp and Money update
        await _userInfoRepo.Received(1).UpdateExpAndMoneyAsync(1001, 150, 650, Arg.Any<CancellationToken>());
    }
}
