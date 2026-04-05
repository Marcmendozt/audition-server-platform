using Audition.GameServer.Application.Abstractions;
using Audition.GameServer.Application.Contracts;
using Audition.GameServer.Application.Services;
using Audition.GameServer.Domain.Models;
using Audition.GameServer.Host.Configuration;
using Microsoft.Extensions.Options;

namespace Audition.GameServer.Host.Services;

public sealed class GameServerBootstrapWorker(
    ILogger<GameServerBootstrapWorker> logger,
    RuntimePaths runtimePaths,
    IOptions<GameServerOptions> options,
    ILegacyGameServerConfigLoader legacyConfigLoader,
    ILegacyGameDataLoader legacyGameDataLoader,
    ILegacyGameDataStore legacyGameDataStore,
    IGameServerBootstrapOrchestrator bootstrapOrchestrator) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        GameServerOptions currentOptions = options.Value;
        LegacyBootstrapOverrides? overrides = await LoadOverridesAsync(currentOptions, stoppingToken);
        LegacyGameDataSnapshot gameData = await LoadGameDataAsync(currentOptions, stoppingToken);
        var configuration = currentOptions.ToBootstrapConfiguration(overrides);

        logger.LogInformation("=== Audition GameServer Bootstrap Skeleton ===");
        logger.LogInformation(
            "Server {ServerId} ({ServerName}) configured for {BindAddress}:{BindPort}",
            configuration.ServerId,
            configuration.ServerName,
            configuration.BindAddress,
            configuration.BindPort);
        logger.LogInformation(
            "Runtime paths: Data={DataPath}, Log={LogPath}, Report={ReportPath}, Script={ScriptPath}, Sound={SoundPath}",
            runtimePaths.DataPath,
            runtimePaths.LogPath,
            runtimePaths.ReportPath,
            runtimePaths.ScriptPath,
            runtimePaths.SoundPath);

        BootstrapExecutionResult execution = await bootstrapOrchestrator.ExecuteAsync(configuration, stoppingToken);
        execution = execution with
        {
            RuntimeState = execution.RuntimeState with
            {
                GameData = gameData,
            }
        };
        foreach (BootstrapStepResult step in execution.Snapshot.Steps)
        {
            switch (step.State)
            {
                case BootstrapStepState.Succeeded:
                    logger.LogInformation("Bootstrap step {Step} OK | {Message}", step.Name, step.Message);
                    break;
                case BootstrapStepState.Skipped:
                    logger.LogInformation("Bootstrap step {Step} SKIPPED | {Message}", step.Name, step.Message);
                    break;
                default:
                    logger.LogWarning("Bootstrap step {Step} FAILED | {Message}", step.Name, step.Message);
                    break;
            }
        }

        logger.LogInformation(
            "Bootstrap status: {Status}",
            execution.Snapshot.IsHealthy ? "READY_FOR_PROTOCOL_WORK" : "DEGRADED_NEEDS_IMPLEMENTATION");

        if (execution.RuntimeState.ServerInfo is not null)
        {
            logger.LogInformation(
                "DBAgent server info loaded: Name={Name}, Ip={Ip}, Port={Port}, Channels={Channels}, LevelQuests={LevelQuests}",
                execution.RuntimeState.ServerInfo.Name,
                execution.RuntimeState.ServerInfo.IpAddress,
                execution.RuntimeState.ServerInfo.Port,
                execution.RuntimeState.Channels.Count,
                execution.RuntimeState.LevelQuests.Count);
        }

        logger.LogInformation(
            "Legacy data loaded: Notices={NoticeCount}, Hacks={HackCount}, Missions={MissionCount}, BattleStages={BattleStageCount}",
            execution.RuntimeState.GameData.Notice?.Channels.Count ?? 0,
            execution.RuntimeState.GameData.HackList?.Entries.Count ?? 0,
            execution.RuntimeState.GameData.Mission?.Rules.Count ?? 0,
            execution.RuntimeState.GameData.Battle?.Stages.Count ?? 0);

        await legacyGameDataStore.UpdateAsync(gameData, stoppingToken);
    }

    private async Task<LegacyBootstrapOverrides?> LoadOverridesAsync(GameServerOptions currentOptions, CancellationToken ct)
    {
        if (!currentOptions.LegacyIni.Enabled)
        {
            return null;
        }

        string iniPath = ResolveLegacyIniPath(currentOptions.LegacyIni.Path);
        LegacyBootstrapOverrides? overrides = await legacyConfigLoader.LoadAsync(iniPath, ct);
        if (overrides is not null)
        {
            logger.LogInformation("Loaded legacy bootstrap overrides from {IniPath}", iniPath);
        }

        return overrides;
    }

    private static string ResolveLegacyIniPath(string configuredPath)
    {
        return RuntimePathLocator.ResolveExistingPath(configuredPath, expectDirectory: false);
    }

    private async Task<LegacyGameDataSnapshot> LoadGameDataAsync(GameServerOptions currentOptions, CancellationToken ct)
    {
        if (!currentOptions.LegacyData.Enabled)
        {
            return LegacyGameDataSnapshot.Empty;
        }

        string dataPath = runtimePaths.DataPath;
        LegacyGameDataSnapshot snapshot = await legacyGameDataLoader.LoadAsync(dataPath, ct);
        logger.LogInformation("Loaded legacy game data from {DataPath}", dataPath);
        return snapshot;
    }
}