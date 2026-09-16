using System.Data;
using Microsoft.Data.SqlClient;
using Milkora.Domain.Exceptions;

namespace Milkora.Infrastructure.Persistence;

/// <summary>
/// Shared ADO.NET plumbing for every repository: connection handling and the
/// small set of generic stored-procedure execution helpers. Keeps each concrete
/// repository focused on parameter binding and row mapping (SRP).
/// </summary>
public abstract class RepositoryBase
{
    private readonly IDbConnectionFactory _factory;

    protected RepositoryBase(IDbConnectionFactory factory) => _factory = factory;

    // CLR null -> DBNull.Value.
    protected static void AddParam(SqlCommand cmd, string name, object? value)
        => cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);

    protected async Task<IReadOnlyList<T>> QueryListAsync<T>(
        string proc, Action<SqlCommand>? addParams, Func<SqlDataReader, T> map, CancellationToken ct)
    {
        var list = new List<T>();
        await using var conn = _factory.Create();
        await using var cmd = new SqlCommand(proc, conn) { CommandType = CommandType.StoredProcedure };
        addParams?.Invoke(cmd);
        await conn.OpenAsync(ct);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct)) list.Add(map(reader));
        return list;
    }

    protected async Task<T?> QuerySingleAsync<T>(
        string proc, Action<SqlCommand>? addParams, Func<SqlDataReader, T> map, CancellationToken ct) where T : class
    {
        await using var conn = _factory.Create();
        await using var cmd = new SqlCommand(proc, conn) { CommandType = CommandType.StoredProcedure };
        addParams?.Invoke(cmd);
        await conn.OpenAsync(ct);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        return await reader.ReadAsync(ct) ? map(reader) : null;
    }

    /// INSERT proc that SELECTs the generated key back.
    protected async Task<Guid> ExecuteInsertAsync(
        string proc, Action<SqlCommand> addParams, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await using var cmd = new SqlCommand(proc, conn) { CommandType = CommandType.StoredProcedure };
        addParams(cmd);
        await conn.OpenAsync(ct);
        try
        {
            var scalar = await cmd.ExecuteScalarAsync(ct);
            return scalar is Guid g ? g : Guid.Parse(scalar!.ToString()!);
        }
        catch (SqlException ex) when (IsUniqueViolation(ex))
        {
            throw new ConflictException("A record with the same unique key already exists.");
        }
    }

    /// UPDATE/DELETE proc that SELECTs @@ROWCOUNT AS RowsAffected.
    protected async Task<int> ExecuteRowCountAsync(
        string proc, Action<SqlCommand> addParams, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await using var cmd = new SqlCommand(proc, conn) { CommandType = CommandType.StoredProcedure };
        addParams(cmd);
        await conn.OpenAsync(ct);
        try
        {
            var scalar = await cmd.ExecuteScalarAsync(ct);
            return scalar is int i ? i : Convert.ToInt32(scalar ?? 0);
        }
        catch (SqlException ex) when (IsUniqueViolation(ex))
        {
            throw new ConflictException("A record with the same unique key already exists.");
        }
    }

    // 2601 = unique index violation, 2627 = unique/PK constraint violation.
    private static bool IsUniqueViolation(SqlException ex)
        => ex.Number is 2601 or 2627;
}
