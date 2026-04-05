namespace Audition.GameServer.Domain.Models;

public sealed record BootstrapSnapshot(IReadOnlyList<BootstrapStepResult> Steps)
{
    public bool IsHealthy => Steps.Where(step => step.Required)
        .All(step => step.State == BootstrapStepState.Succeeded);
}