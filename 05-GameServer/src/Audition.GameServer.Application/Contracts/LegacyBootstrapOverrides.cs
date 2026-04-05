namespace Audition.GameServer.Application.Contracts;

public sealed record LegacyBootstrapOverrides(
    string? AccountServerHost,
    string? DbAgentHost,
    ushort? ServerId);