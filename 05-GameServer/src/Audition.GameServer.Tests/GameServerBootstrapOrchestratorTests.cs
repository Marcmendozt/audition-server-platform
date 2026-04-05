using Audition.GameServer.Application.Abstractions;
using Audition.GameServer.Application.Contracts;
using Audition.GameServer.Application.Services;
using Audition.GameServer.Domain.Models;
using Xunit;

namespace Audition.GameServer.Tests;

public sealed class GameServerBootstrapOrchestratorTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsHealthy_WhenRequiredDependenciesRespond()
    {
        var accountClient = new FakeAccountServerRegistrationClient(new(true, "registered"));
        var dbAgentClient = new FakeGameDbAgentBootstrapClient(new(
            true,
            "synced",
            new LegacyServerInfo(101, "RZ", "127.0.0.1", 25511, 0, 5000, 1, 0, 1),
            [new LegacyChannelInfo(0, "Channel-0", 200, 100, 0, 61, 0)],
            [new LevelQuestInfo(1, 100, 50, 5, 2, 1, 1001, 1, 10, 20, 30)]));
        var probe = new FakeEndpointProbe(new Dictionary<string, ProbeResult>
        {
            ["Certify"] = new(false, "optional endpoint offline"),
        });
        var store = new FakeBootstrapStateStore();
        var sut = new GameServerBootstrapOrchestrator(accountClient, dbAgentClient, probe, store);

        var execution = await sut.ExecuteAsync(CreateConfiguration(), CancellationToken.None);

        Assert.True(execution.Snapshot.IsHealthy);
        Assert.True(execution.RuntimeState.AccountServerRegistered);
        Assert.Single(execution.RuntimeState.Channels);
        Assert.Single(execution.RuntimeState.LevelQuests);
        Assert.Contains(execution.Snapshot.Steps, step =>
            step.Name == "Certify" &&
            step.State == BootstrapStepState.Failed &&
            !step.Required);
        Assert.Same(execution, store.Current);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUnhealthy_WhenRequiredDependencyFails()
    {
        var accountClient = new FakeAccountServerRegistrationClient(new(false, "refused"));
        var dbAgentClient = new FakeGameDbAgentBootstrapClient(new(
            true,
            "synced",
            new LegacyServerInfo(101, "RZ", "127.0.0.1", 25511, 0, 5000, 1, 0, 1),
            [],
            []));
        var probe = new FakeEndpointProbe(new Dictionary<string, ProbeResult>
        {
            ["Certify"] = new(true, "connected"),
        });
        var store = new FakeBootstrapStateStore();
        var sut = new GameServerBootstrapOrchestrator(accountClient, dbAgentClient, probe, store);

        var execution = await sut.ExecuteAsync(CreateConfiguration(), CancellationToken.None);

        Assert.False(execution.Snapshot.IsHealthy);
        Assert.Contains(execution.Snapshot.Steps, step =>
            step.Name == "AccountServer" &&
            step.State == BootstrapStepState.Failed &&
            step.Required);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUnhealthy_WhenDbAgentSyncFails()
    {
        var accountClient = new FakeAccountServerRegistrationClient(new(true, "registered"));
        var dbAgentClient = new FakeGameDbAgentBootstrapClient(new(false, "dbagent timeout", null, [], []));
        var probe = new FakeEndpointProbe(new Dictionary<string, ProbeResult>
        {
            ["Certify"] = new(true, "connected"),
        });
        var store = new FakeBootstrapStateStore();
        var sut = new GameServerBootstrapOrchestrator(accountClient, dbAgentClient, probe, store);

        var execution = await sut.ExecuteAsync(CreateConfiguration(), CancellationToken.None);

        Assert.False(execution.Snapshot.IsHealthy);
        Assert.Contains(execution.Snapshot.Steps, step =>
            step.Name == "DBAgent" &&
            step.State == BootstrapStepState.Failed &&
            step.Required);
    }

    private static BootstrapConfiguration CreateConfiguration()
    {
        return new BootstrapConfiguration(
            101,
            "RZ Bootstrap",
            "127.0.0.1",
            25511,
            1,
            1,
            5000,
            1,
            ServerStatus.Online,
            new EndpointDefinition("DBAgent", "127.0.0.1", 25525, Enabled: true, Required: true),
            new EndpointDefinition("AccountServer", "127.0.0.1", 4502, Enabled: true, Required: true),
            new EndpointDefinition("Certify", "127.0.0.1", 28888, Enabled: true, Required: false));
    }

    private sealed class FakeAccountServerRegistrationClient(AccountServerRegistrationResult result) : IAccountServerRegistrationClient
    {
        public Task<AccountServerRegistrationResult> RegisterAsync(BootstrapConfiguration configuration, CancellationToken ct)
        {
            return Task.FromResult(result);
        }
    }

    private sealed class FakeGameDbAgentBootstrapClient(GameDbBootstrapResult result) : IGameDbAgentBootstrapClient
    {
        public Task<GameDbBootstrapResult> SynchronizeAsync(BootstrapConfiguration configuration, CancellationToken ct)
        {
            return Task.FromResult(result);
        }
    }

    private sealed class FakeEndpointProbe(Dictionary<string, ProbeResult> results) : IEndpointProbe
    {
        public Task<ProbeResult> ProbeAsync(EndpointDefinition endpoint, CancellationToken ct)
        {
            if (results.TryGetValue(endpoint.Name, out ProbeResult? result))
            {
                return Task.FromResult(result);
            }

            return Task.FromResult(new ProbeResult(false, "missing fake result"));
        }
    }

    private sealed class FakeBootstrapStateStore : IBootstrapStateStore
    {
        public BootstrapExecutionResult Current { get; private set; } = new(new([]), BootstrapRuntimeState.Empty);

        public Task UpdateAsync(BootstrapExecutionResult result, CancellationToken ct)
        {
            Current = result;
            return Task.CompletedTask;
        }
    }
}