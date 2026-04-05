using Audition.DBAgent.Game.Application.Abstractions;
using Audition.DBAgent.Game.Application.Contracts;
using Audition.DBAgent.Game.Application.Services;
using Audition.DBAgent.Game.Domain.Entities;
using NSubstitute;
using Xunit;

namespace Audition.DBAgent.Game.Tests;

public class GameDbAgentServiceTests
{
    private readonly IUserInfoRepository _userInfoRepo = Substitute.For<IUserInfoRepository>();
    private readonly ILevelQuestRepository _levelQuestRepo = Substitute.For<ILevelQuestRepository>();
    private readonly IAvatarItemRepository _avatarItemRepo = Substitute.For<IAvatarItemRepository>();
    private readonly IPresentRepository _presentRepo = Substitute.For<IPresentRepository>();
    private readonly IStatisticsRepository _statsRepo = Substitute.For<IStatisticsRepository>();
    private readonly GameDbAgentService _sut;

    public GameDbAgentServiceTests()
    {
        _sut = new GameDbAgentService(
            _userInfoRepo,
            _levelQuestRepo,
            _avatarItemRepo,
            _presentRepo,
            _statsRepo);
    }

    // --- GetUserProfileAsync ---

    [Fact]
    public async Task GetUserProfileAsync_ReturnsProfile_WhenUserExists()
    {
        var user = new UserInfo(1001, 5000, 2000, 100, 5, 1, true);
        _userInfoRepo.GetByUserSNAsync(1001, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.GetUserProfileAsync(1001, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1001u, result.UserSN);
        Assert.Equal(5000, result.Exp);
        Assert.Equal(2000, result.Money);
        Assert.Equal(100, result.Cash);
        Assert.Equal(5, result.Level);
        Assert.True(result.IsAllowMsg);
    }

    [Fact]
    public async Task GetUserProfileAsync_ReturnsNull_WhenUserNotFound()
    {
        _userInfoRepo.GetByUserSNAsync(9999, Arg.Any<CancellationToken>()).Returns((UserInfo?)null);

        var result = await _sut.GetUserProfileAsync(9999, CancellationToken.None);

        Assert.Null(result);
    }

    // --- PurchaseItemAsync ---

    [Fact]
    public async Task PurchaseItemAsync_InsertsItemAndDeductsCash()
    {
        var command = new PurchaseCommand(1001, 5001, 30, 1000);

        await _sut.PurchaseItemAsync(command, CancellationToken.None);

        await _avatarItemRepo.Received(1).InsertAsync(1001, 5001, 30, Arg.Any<CancellationToken>());
        await _userInfoRepo.Received(1).DeductCashAsync(1001, 1000, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PurchaseItemAsync_SkipsCashDeduction_WhenCostIsZero()
    {
        var command = new PurchaseCommand(1001, 5001, 30, 0);

        await _sut.PurchaseItemAsync(command, CancellationToken.None);

        await _avatarItemRepo.Received(1).InsertAsync(1001, 5001, 30, Arg.Any<CancellationToken>());
        await _userInfoRepo.DidNotReceive().DeductCashAsync(Arg.Any<uint>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    // --- UpdateGameResultsAsync ---

    [Fact]
    public async Task UpdateGameResultsAsync_AddsExpAndMoney()
    {
        var command = new GameResultCommand(1001, 500, 300);

        await _sut.UpdateGameResultsAsync(command, CancellationToken.None);

        await _userInfoRepo.Received(1).AddExpAndMoneyAsync(1001, 500, 300, Arg.Any<CancellationToken>());
    }

    // --- Item checks ---

    [Fact]
    public async Task HasActiveItemAsync_DelegatesToRepository()
    {
        _avatarItemRepo.HasActiveItemAsync(1001, 5001, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _sut.HasActiveItemAsync(1001, 5001, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task HasUnlimitedItemAsync_DelegatesToRepository()
    {
        _avatarItemRepo.HasUnlimitedItemAsync(1001, 5001, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _sut.HasUnlimitedItemAsync(1001, 5001, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task HasPendingPresentAsync_DelegatesToRepository()
    {
        _presentRepo.HasPendingPresentAsync(1001, 5001, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _sut.HasPendingPresentAsync(1001, 5001, CancellationToken.None);

        Assert.True(result);
    }

    // --- LoadLevelQuestDataAsync ---

    [Fact]
    public async Task LoadLevelQuestDataAsync_CachesQuests()
    {
        var quests = new List<LevelQuest>
        {
            new(1, 100, 80, 5, 2, 1, 1001, 1, 50, 200, 100),
            new(2, 500, 85, 8, 3, 1, 1002, 2, 100, 400, 200),
        };
        _levelQuestRepo.LoadAllAsync(Arg.Any<CancellationToken>()).Returns(quests);

        await _sut.LoadLevelQuestDataAsync(CancellationToken.None);
        var cached = _sut.GetCachedLevelQuests();

        Assert.Equal(2, cached.Count);
        Assert.Equal(1, cached[0].Level);
        Assert.Equal(2, cached[1].Level);
    }

    // --- SaveDayUniqueCountAsync ---

    [Fact]
    public async Task SaveDayUniqueCountAsync_DelegatesToRepository()
    {
        await _sut.SaveDayUniqueCountAsync(CancellationToken.None);

        await _statsRepo.Received(1).SaveDayUniqueCountAsync(Arg.Any<CancellationToken>());
    }
}
