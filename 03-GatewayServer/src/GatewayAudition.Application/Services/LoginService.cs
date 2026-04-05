using GatewayAudition.Application.DTOs;
using GatewayAudition.Domain.Entities;
using GatewayAudition.Domain.Interfaces;
using GatewayAudition.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace GatewayAudition.Application.Services;

public sealed class LoginService
{
    private readonly IAccountServerManager _accountServerManager;
    private readonly IPlayerManager _playerManager;
    private readonly ILogger<LoginService> _logger;

    public LoginService(
        IAccountServerManager accountServerManager,
        IPlayerManager playerManager,
        ILogger<LoginService> logger)
    {
        _accountServerManager = accountServerManager;
        _playerManager = playerManager;
        _logger = logger;
    }

    public async Task<LoginResponse> ProcessLoginAsync(
        User user, LoginRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login request from user {Username}", request.Username);

        if (!_accountServerManager.IsServerStarted)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Account server is not available"
            };
        }

        var accountResult = await _accountServerManager.RequestLoginAsync(
            user,
            request.Username,
            request.Password,
            request.ClientVersion,
            request.RequestedGameServerId,
            cancellationToken);

        if (!accountResult.Success)
        {
            return new LoginResponse
            {
                Success = false,
                Message = accountResult.Message,
                UserSerialNumber = accountResult.UserSerial
            };
        }

        user.Player.UserSerialNumber = accountResult.UserSerial;
        user.AccountSessionId = accountResult.SessionId;
        user.CurrentGameServerId = accountResult.GameServerId;

        return new LoginResponse
        {
            Success = true,
            UserSerialNumber = accountResult.UserSerial,
            LoginFlag = accountResult.LoginFlag,
            UserNickname = accountResult.UserNickname,
            UserExperience = accountResult.UserExperience,
            SessionId = accountResult.SessionId,
            GameServerId = accountResult.GameServerId,
            Message = "Login validado por account server"
        };
    }
}
