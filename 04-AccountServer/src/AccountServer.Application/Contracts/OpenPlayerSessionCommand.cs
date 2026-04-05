namespace AccountServer.Application.Contracts;

public sealed record OpenPlayerSessionCommand(
    uint UserSerial,
    uint UserExperience,
    string UserId,
    uint ClusterId,
    ushort ServerId,
    ushort GatewayServerId,
    string IpAddress,
    long SocketHandle = 0);