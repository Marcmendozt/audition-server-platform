namespace GatewayAudition.Domain.ValueObjects;

public sealed class AccountServerLoginResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public uint UserSerial { get; set; }
    public byte LoginFlag { get; set; }
    public byte Region { get; set; }
    public string UserNickname { get; set; } = string.Empty;
    public uint UserExperience { get; set; }
    public Guid? SessionId { get; set; }
    public ushort GameServerId { get; set; }
}