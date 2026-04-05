namespace AccountServer.Host.Contracts;

public sealed record DbAgentLoginResult(
    bool Success,
    ushort PacketLength,
    ushort OpCode,
    byte Region,
    byte InternalResult,
    uint UserSerial,
    byte LoginFlag,
    string UserId,
    string UserNickname,
    uint UserExperience);