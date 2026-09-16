using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Milkora.Infrastructure.Persistence;

public sealed class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("MilkoraDb")
            ?? throw new InvalidOperationException("Connection string 'MilkoraDb' is not configured.");
    }

    // Connection pooling (on by default) makes creating a new SqlConnection per
    // call cheap — the physical connection is reused from the pool.
    public SqlConnection Create() => new(_connectionString);
}
