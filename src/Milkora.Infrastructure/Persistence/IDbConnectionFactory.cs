using Microsoft.Data.SqlClient;

namespace Milkora.Infrastructure.Persistence;

/// <summary>Creates SqlConnections for the configured Milkora database.
/// Abstracted so repositories don't depend on how the connection string is
/// obtained, and so it can be swapped/mocked in tests.</summary>
public interface IDbConnectionFactory
{
    SqlConnection Create();
}
