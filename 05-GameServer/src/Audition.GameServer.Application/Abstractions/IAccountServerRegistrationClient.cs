using Audition.GameServer.Application.Contracts;

namespace Audition.GameServer.Application.Abstractions;

public interface IAccountServerRegistrationClient
{
    Task<AccountServerRegistrationResult> RegisterAsync(BootstrapConfiguration configuration, CancellationToken ct);
}