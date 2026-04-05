using Audition.GameServer.Domain.Models;

namespace Audition.GameServer.Application.Contracts;

public sealed record BootstrapExecutionResult(
    BootstrapSnapshot Snapshot,
    BootstrapRuntimeState RuntimeState);