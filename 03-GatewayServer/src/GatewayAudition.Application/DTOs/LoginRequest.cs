namespace GatewayAudition.Application.DTOs;

public sealed class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ClientVersion { get; set; } = string.Empty;
    public uint UserSerialNumber { get; set; }
    public ushort RequestedGameServerId { get; set; }
}
