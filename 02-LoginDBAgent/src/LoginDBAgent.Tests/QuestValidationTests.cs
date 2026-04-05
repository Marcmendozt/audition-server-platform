using LoginDBAgent.Application.Abstractions;
using LoginDBAgent.Application.Contracts;
using LoginDBAgent.Application.Services;
using LoginDBAgent.Domain.Entities;
using NSubstitute;
using Xunit;

namespace LoginDBAgent.Tests;

public class QuestValidationTests
{
    private readonly IUserAccountRepository _userAccountRepo = Substitute.For<IUserAccountRepository>();
    private readonly IUserInfoRepository _userInfoRepo = Substitute.For<IUserInfoRepository>();
    private readonly ILevelQuestRepository _levelQuestRepo = Substitute.For<ILevelQuestRepository>();
    private readonly IAvatarItemRepository _avatarItemRepo = Substitute.For<IAvatarItemRepository>();
    private readonly IPresentRepository _presentRepo = Substitute.For<IPresentRepository>();
    private readonly IRankRepository _rankRepo = Substitute.For<IRankRepository>();
    private readonly IFriendRepository _friendRepo = Substitute.For<IFriendRepository>();
    private readonly IStatisticsRepository _statsRepo = Substitute.For<IStatisticsRepository>();
    private readonly LoginDbAgentService _sut;

    private static readonly List<LevelQuest> SampleQuests =
    [
        new LevelQuest(1, 100, 80, 5, 2, 1, 1001, 1, 50, 200, 100),
        new LevelQuest(2, 500, 85, 8, 3, 1, 1002, 2, 100, 400, 200),
        new LevelQuest(3, 1200, 90, 10, 5, 2, 1003, 3, 200, 600, 350),
    ];

    public QuestValidationTests()
    {
        _sut = new LoginDbAgentService(
            _userAccountRepo, _userInfoRepo, _levelQuestRepo, _avatarItemRepo,
            _presentRepo, _rankRepo, _friendRepo, _statsRepo);

        _levelQuestRepo.LoadAllAsync(Arg.Any<CancellationToken>()).Returns(SampleQuests);
        _sut.LoadLevelQuestDataAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Quest_Fails_WhenNotFound()
    {
        var result = await _sut.ValidateAndCompleteQuestAsync(
            new QuestAttemptCommand(1001, 99, 80, 5, 1), CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Quest not found", result.ErrorReason);
    }

    [Fact]
    public async Task Quest_Fails_WhenAlreadyCompleted()
    {
        _levelQuestRepo.HasCompletedQuestAsync(1001, 1, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _sut.ValidateAndCompleteQuestAsync(
            new QuestAttemptCommand(1001, 1, 80, 5, 1), CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Quest already completed", result.ErrorReason);
    }

    [Fact]
    public async Task Quest_Fails_WhenUserNotFound()
    {
        _levelQuestRepo.HasCompletedQuestAsync(1001, 1, Arg.Any<CancellationToken>()).Returns(false);
        _userInfoRepo.GetByUserSNAsync(1001, Arg.Any<CancellationToken>()).Returns((UserInfo?)null);

        var result = await _sut.ValidateAndCompleteQuestAsync(
            new QuestAttemptCommand(1001, 1, 80, 5, 1), CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User not found", result.ErrorReason);
    }

    [Fact]
    public async Task Quest_Fails_WhenInsufficientMoney()
    {
        var user = new UserInfo(1001, 200, 30, 100, 1, true);
        _levelQuestRepo.HasCompletedQuestAsync(1001, 1, Arg.Any<CancellationToken>()).Returns(false);
        _userInfoRepo.GetByUserSNAsync(1001, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.ValidateAndCompleteQuestAsync(
            new QuestAttemptCommand(1001, 1, 80, 5, 1), CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Insufficient Money for quest fee", result.ErrorReason);
    }

    [Fact]
    public async Task Quest_Succeeds_WithCorrectRewards()
    {
        var user = new UserInfo(1001, 50, 500, 100, 1, true);
        _levelQuestRepo.HasCompletedQuestAsync(1001, 1, Arg.Any<CancellationToken>()).Returns(false);
        _userInfoRepo.GetByUserSNAsync(1001, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.ValidateAndCompleteQuestAsync(
            new QuestAttemptCommand(1001, 1, 95, 5, 1), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(650, result.NewMoney); // 500 - 50 (fee) + 200 (WinDen)
        Assert.Equal(150, result.NewExp);   // 50 + 100 (WinExp)
    }

    [Fact]
    public async Task Quest_TriggersLevelUp()
    {
        var user = new UserInfo(1001, 450, 1000, 100, 1, true);
        _levelQuestRepo.HasCompletedQuestAsync(1001, 1, Arg.Any<CancellationToken>()).Returns(false);
        _userInfoRepo.GetByUserSNAsync(1001, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.ValidateAndCompleteQuestAsync(
            new QuestAttemptCommand(1001, 1, 95, 5, 1), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(2, result.NewLevel); // 550 Exp >= Level 2 req (500)
        await _userInfoRepo.Received(1).UpdateLevelAsync(1001, 2, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Quest_NoLevelUp_WhenExpBelowThreshold()
    {
        var user = new UserInfo(1001, 50, 1000, 100, 1, true);
        _levelQuestRepo.HasCompletedQuestAsync(1001, 1, Arg.Any<CancellationToken>()).Returns(false);
        _userInfoRepo.GetByUserSNAsync(1001, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.ValidateAndCompleteQuestAsync(
            new QuestAttemptCommand(1001, 1, 95, 5, 1), CancellationToken.None);

        Assert.Equal(1, result.NewLevel);
        await _userInfoRepo.DidNotReceive().UpdateLevelAsync(Arg.Any<uint>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Quest_LogsCompletion()
    {
        var user = new UserInfo(1001, 50, 500, 100, 1, true);
        _levelQuestRepo.HasCompletedQuestAsync(1001, 1, Arg.Any<CancellationToken>()).Returns(false);
        _userInfoRepo.GetByUserSNAsync(1001, Arg.Any<CancellationToken>()).Returns(user);

        await _sut.ValidateAndCompleteQuestAsync(
            new QuestAttemptCommand(1001, 1, 95, 5, 1), CancellationToken.None);

        await _levelQuestRepo.Received(1).InsertQuestLogAsync(
            Arg.Is<LevelQuestLogEntry>(e => e.UserSN == 1001 && e.LevelQuestSN == 1 && e.Pass == 1),
            Arg.Any<CancellationToken>());
    }
}
