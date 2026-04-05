namespace Audition.GameServer.Domain.Models;

public sealed record BootstrapStepResult(
    string Name,
    bool Required,
    BootstrapStepState State,
    string Message);