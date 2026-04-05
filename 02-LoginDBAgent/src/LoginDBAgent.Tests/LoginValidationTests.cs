using LoginDBAgent.Application.Abstractions;
using LoginDBAgent.Application.Contracts;
using LoginDBAgent.Application.Services;
using LoginDBAgent.Domain.Entities;
using NSubstitute;
using Xunit;

namespace LoginDBAgent.Tests;

public class LoginValidationTests
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

    public LoginValidationTests()
    {
        _sut = new LoginDbAgentService(
            _userAccountRepo, _userInfoRepo, _levelQuestRepo, _avatarItemRepo,
            _presentRepo, _rankRepo, _friendRepo, _statsRepo);
    }

    [Fact]
    public async Task Login_Fails_WhenEmptyCredentials()
    {
        var result = await _sut.ValidateLoginAsync(new LoginCommand("", ""), CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Empty credentials", result.ErrorReason);
    }

    [Fact]
    public async Task Login_Fails_WhenUserNotFound()
    {
        _userAccountRepo.GetByUserIdAsync("nouser", Arg.Any<CancellationToken>()).Returns((UserAccount?)null);

        var result = await _sut.ValidateLoginAsync(new LoginCommand("nouser", "pass"), CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User not found", result.ErrorReason);
    }

    [Fact]
    public async Task Login_Fails_WhenPasswordIncorrect()
    {
        var account = new UserAccount(1001, "testuser", "correct", "TestNick", 1);
        _userAccountRepo.GetByUserIdAsync("testuser", Arg.Any<CancellationToken>()).Returns(account);

        var result = await _sut.ValidateLoginAsync(new LoginCommand("testuser", "wrong"), CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Invalid password", result.ErrorReason);
    }

    [Fact]
    public async Task Login_Succeeds_WithCorrectCredentials()
    {
        var account = new UserAccount(1001, "testuser", "pass123", "TestNick", 1);
        var userInfo = new UserInfo(1001, 5000, 2000, 500, 5, true);
        _userAccountRepo.GetByUserIdAsync("testuser", Arg.Any<CancellationToken>()).Returns(account);
        _userInfoRepo.GetByUserSNAsync(1001, Arg.Any<CancellationToken>()).Returns(userInfo);

        var result = await _sut.ValidateLoginAsync(new LoginCommand("testuser", "pass123"), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(1001u, result.UserSN);
        Assert.Equal("TestNick", result.UserNick);
        Assert.Equal(500, result.Cash);
        Assert.Null(result.ErrorReason);
    }

    [Fact]
    public async Task Login_Succeeds_WithZeroCash_WhenNoUserInfo()
    {
        var account = new UserAccount(1001, "testuser", "pass", "Nick", 0);
        _userAccountRepo.GetByUserIdAsync("testuser", Arg.Any<CancellationToken>()).Returns(account);
        _userInfoRepo.GetByUserSNAsync(1001, Arg.Any<CancellationToken>()).Returns((UserInfo?)null);

        var result = await _sut.ValidateLoginAsync(new LoginCommand("testuser", "pass"), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(0, result.Cash);
    }
}
