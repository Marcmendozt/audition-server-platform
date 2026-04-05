namespace AccountServer.Host.Contracts;

public sealed record DbAgentLoginRequest(
    string UserId,
    string Password,
    string ExtraField3,
    string ExtraField4,
    int SocketHandle,
    ushort? GatewayServerId = null,
    string? RelayContextBase64 = null);