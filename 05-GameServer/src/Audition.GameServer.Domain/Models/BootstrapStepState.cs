namespace Audition.GameServer.Domain.Models;

public enum BootstrapStepState
{
    Pending = 0,
    Succeeded = 1,
    Failed = 2,
    Skipped = 3,
}