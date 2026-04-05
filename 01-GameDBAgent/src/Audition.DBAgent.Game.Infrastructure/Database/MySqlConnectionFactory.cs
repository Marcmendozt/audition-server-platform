using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Audition.DBAgent.Game.Infrastructure.Database;

public sealed class MySqlConnectionFactory(IOptions<DatabaseOptions> options)
{
    private readonly string _connectionString = options.Value.ToConnectionString();

    public async Task<MySqlConnection> CreateOpenConnectionAsync(CancellationToken ct)
    {
        var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        return conn;
    }
}
