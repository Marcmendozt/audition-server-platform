namespace GatewayAudition.Application.DTOs;

public sealed class LoginResponse
{
    public bool Success { get; set; }
    public uint UserSerialNumber { get; set; }
    public byte LoginFlag { get; set; }
    public string UserNickname { get; set; } = string.Empty;
    public uint UserExperience { get; set; }
    public Guid? SessionId { get; set; }
    public ushort GameServerId { get; set; }
    public string Message { get; set; } = string.Empty;
}
