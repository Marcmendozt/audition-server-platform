using LoginDBAgent.Application.Abstractions;
using LoginDBAgent.Application.Contracts;
using LoginDBAgent.Application.Services;
using LoginDBAgent.Domain.Entities;
using NSubstitute;
using Xunit;

namespace LoginDBAgent.Tests;

public class LoginDbAgentServiceTests
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

    public LoginDbAgentServiceTests()
    {
        _sut = new LoginDbAgentService(
            _userAccountRepo, _userInfoRepo, _levelQuestRepo, _avatarItemRepo,
            _presentRepo, _rankRepo, _friendRepo, _statsRepo);
    }

    // --- User Profile ---

    [Fact]
    public async Task GetUserProfile_ReturnsProfile()
    {
        var user = new UserInfo(1001, 5000, 2000, 500, 5, true);
        _userInfoRepo.GetByUserSNAsync(1001, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.GetUserProfileAsync(1001, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(5000, result.Exp);
        Assert.Equal(2000, result.Money);
    }

    [Fact]
    public async Task GetUserProfile_ReturnsNull_WhenNotFound()
    {
        _userInfoRepo.GetByUserSNAsync(9999, Arg.Any<CancellationToken>()).Returns((UserInfo?)null);

        var result = await _sut.GetUserProfileAsync(9999, CancellationToken.None);

        Assert.Null(result);
    }

    // --- Game Results ---

    [Fact]
    public async Task UpdateGameResults_AddsExpAndMoney()
    {
        await _sut.UpdateGameResultsAsync(new GameResultCommand(1001, 500, 300), CancellationToken.None);

        await _userInfoRepo.Received(1).AddExpAndMoneyAsync(1001, 500, 300, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddMoney_DelegatesToRepository()
    {
        await _sut.AddMoneyAsync(1001, 1000, CancellationToken.None);

        await _userInfoRepo.Received(1).AddMoneyAsync(1001, 1000, Arg.Any<CancellationToken>());
    }

    // --- Avatar/Item ---

    [Fact]
    public async Task PurchaseItem_InsertsItem()
    {
        await _sut.PurchaseItemAsync(1001, 5001, 30, CancellationToken.None);

        await _avatarItemRepo.Received(1).InsertAsync(1001, 5001, 30, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteItem_RemovesItem()
    {
        await _sut.DeleteItemAsync(1001, 5001, CancellationToken.None);

        await _avatarItemRepo.Received(1).DeleteAsync(1001, 5001, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EquipItem_SetsEquipState()
    {
        await _sut.EquipItemAsync(new AvatarEquipCommand(1001, 5001, "EQUIP"), CancellationToken.None);

        await _avatarItemRepo.Received(1).SetEquipStateAsync(1001, 5001, "EQUIP", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HasActiveItem_DelegatesToRepo()
    {
        _avatarItemRepo.HasActiveItemAsync(1001, 5001, Arg.Any<CancellationToken>()).Returns(true);
        Assert.True(await _sut.HasActiveItemAsync(1001, 5001, CancellationToken.None));
    }

    [Fact]
    public async Task HasUnlimitedItem_DelegatesToRepo()
    {
        _avatarItemRepo.HasUnlimitedItemAsync(1001, 5001, Arg.Any<CancellationToken>()).Returns(true);
        Assert.True(await _sut.HasUnlimitedItemAsync(1001, 5001, CancellationToken.None));
    }

    [Fact]
    public async Task HasPendingPresent_DelegatesToRepo()
    {
        _presentRepo.HasPendingPresentAsync(1001, 5001, Arg.Any<CancellationToken>()).Returns(true);
        Assert.True(await _sut.HasPendingPresentAsync(1001, 5001, CancellationToken.None));
    }

    // --- Ranking ---

    [Fact]
    public async Task GetTopRankings_DelegatesToRepo()
    {
        var rankings = new List<RankEntry> { new("Nick1", 5000, 1) };
        _rankRepo.GetTopRankingsAsync(0, 10, Arg.Any<CancellationToken>()).Returns(rankings);

        var result = await _sut.GetTopRankingsAsync(0, 10, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Nick1", result[0].UserNick);
    }

    [Fact]
    public async Task GetUserRank_ReturnsRank()
    {
        _rankRepo.GetUserRankAsync("Nick1", Arg.Any<CancellationToken>()).Returns(new RankEntry("Nick1", 5000, 1));

        var result = await _sut.GetUserRankAsync("Nick1", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(5000, result.Exp);
    }

    // --- Friends ---

    [Fact]
    public async Task GetFriendCount_DelegatesToRepo()
    {
        _friendRepo.GetFriendCountAsync("Nick1", Arg.Any<CancellationToken>()).Returns(5);

        Assert.Equal(5, await _sut.GetFriendCountAsync("Nick1", CancellationToken.None));
    }

    [Fact]
    public async Task GetFriendList_ReturnsFriends()
    {
        var friends = new List<FriendEntry> { new("Nick1", "Friend1", "admit") };
        _friendRepo.GetFriendListAsync("Nick1", Arg.Any<CancellationToken>()).Returns(friends);

        var result = await _sut.GetFriendListAsync("Nick1", CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Friend1", result[0].FriendNick);
    }

    // --- Statistics ---

    [Fact]
    public async Task SaveDayUniqueCount_DelegatesToRepo()
    {
        await _sut.SaveDayUniqueCountAsync(CancellationToken.None);
        await _statsRepo.Received(1).SaveDayUniqueCountAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RecordLogin_InsertsLoginInfo()
    {
        await _sut.RecordLoginAsync(1001, "TestNick", CancellationToken.None);
        await _statsRepo.Received(1).InsertLoginInfoAsync(1001, "TestNick", "LOGIN", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RecordLogout_InsertsLogoutInfo()
    {
        await _sut.RecordLogoutAsync(1001, "TestNick", CancellationToken.None);
        await _statsRepo.Received(1).InsertLoginInfoAsync(1001, "TestNick", "LOGOUT", Arg.Any<CancellationToken>());
    }

    // --- Level Quest Loading ---

    [Fact]
    public async Task LoadLevelQuests_CachesData()
    {
        var quests = new List<LevelQuest>
        {
            new(1, 100, 80, 5, 2, 1, 1001, 1, 50, 200, 100),
        };
        _levelQuestRepo.LoadAllAsync(Arg.Any<CancellationToken>()).Returns(quests);

        await _sut.LoadLevelQuestDataAsync(CancellationToken.None);

        Assert.Single(_sut.GetCachedLevelQuests());
    }
}
