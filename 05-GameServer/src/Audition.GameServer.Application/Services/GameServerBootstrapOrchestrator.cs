using Audition.GameServer.Application.Abstractions;
using Audition.GameServer.Application.Contracts;
using Audition.GameServer.Domain.Models;

namespace Audition.GameServer.Application.Services;

public sealed class GameServerBootstrapOrchestrator(
    IAccountServerRegistrationClient accountServerRegistrationClient,
    IGameDbAgentBootstrapClient gameDbAgentBootstrapClient,
    IEndpointProbe endpointProbe,
    IBootstrapStateStore stateStore) : IGameServerBootstrapOrchestrator
{
    public async Task<BootstrapExecutionResult> ExecuteAsync(BootstrapConfiguration configuration, CancellationToken ct)
    {
        var steps = new List<BootstrapStepResult>
        {
            new(
                "ListenerConfig",
                Required: true,
                State: BootstrapStepState.Succeeded,
                Message: $"Configured listener on {configuration.BindAddress}:{configuration.BindPort} for server {configuration.ServerId}")
        };

        var runtimeState = BootstrapRuntimeState.Empty;

        if (configuration.AccountServer.Enabled)
        {
            AccountServerRegistrationResult registration = await accountServerRegistrationClient.RegisterAsync(configuration, ct);
            steps.Add(new BootstrapStepResult(
                configuration.AccountServer.Name,
                configuration.AccountServer.Required,
                registration.Success ? BootstrapStepState.Succeeded : BootstrapStepState.Failed,
                registration.Message));
            runtimeState = runtimeState with { AccountServerRegistered = registration.Success };
        }
        else
        {
            steps.Add(new BootstrapStepResult(
                configuration.AccountServer.Name,
                configuration.AccountServer.Required,
                BootstrapStepState.Skipped,
                "Endpoint disabled in configuration"));
        }

        if (configuration.DbAgent.Enabled)
        {
            GameDbBootstrapResult dbResult = await gameDbAgentBootstrapClient.SynchronizeAsync(configuration, ct);
            steps.Add(new BootstrapStepResult(
                configuration.DbAgent.Name,
                configuration.DbAgent.Required,
                dbResult.Success ? BootstrapStepState.Succeeded : BootstrapStepState.Failed,
                dbResult.Message));
            runtimeState = runtimeState with
            {
                ServerInfo = dbResult.ServerInfo,
                Channels = dbResult.Channels,
                LevelQuests = dbResult.LevelQuests,
            };
        }
        else
        {
            steps.Add(new BootstrapStepResult(
                configuration.DbAgent.Name,
                configuration.DbAgent.Required,
                BootstrapStepState.Skipped,
                "Endpoint disabled in configuration"));
        }

        if (!configuration.Certify.Enabled)
        {
            steps.Add(new BootstrapStepResult(
                configuration.Certify.Name,
                configuration.Certify.Required,
                BootstrapStepState.Skipped,
                "Endpoint disabled in configuration"));
        }
        else
        {
            ProbeResult probe = await endpointProbe.ProbeAsync(configuration.Certify, ct);
            steps.Add(new BootstrapStepResult(
                configuration.Certify.Name,
                configuration.Certify.Required,
                probe.Success ? BootstrapStepState.Succeeded : BootstrapStepState.Failed,
                probe.Message));
        }

        var snapshot = new BootstrapSnapshot(steps);
        var result = new BootstrapExecutionResult(snapshot, runtimeState);
        await stateStore.UpdateAsync(result, ct);
        return result;
    }
}