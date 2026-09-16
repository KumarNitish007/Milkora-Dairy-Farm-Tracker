using Microsoft.Data.SqlClient;

namespace Milkora.Infrastructure.Persistence;

/// Safe, null-aware column readers. Reading by name keeps mappers resilient
/// to SELECT column ordering.
internal static class SqlReaderExtensions
{
    public static Guid GetGuid(this SqlDataReader r, string col) => r.GetGuid(r.GetOrdinal(col));

    public static Guid? GetNullableGuid(this SqlDataReader r, string col)
    { int i = r.GetOrdinal(col); return r.IsDBNull(i) ? null : r.GetGuid(i); }

    public static string GetString(this SqlDataReader r, string col)
    { int i = r.GetOrdinal(col); return r.IsDBNull(i) ? string.Empty : r.GetString(i); }

    public static string? GetNullableString(this SqlDataReader r, string col)
    { int i = r.GetOrdinal(col); return r.IsDBNull(i) ? null : r.GetString(i); }

    public static decimal GetDecimal(this SqlDataReader r, string col)
    { int i = r.GetOrdinal(col); return r.IsDBNull(i) ? 0m : r.GetDecimal(i); }

    public static decimal? GetNullableDecimal(this SqlDataReader r, string col)
    { int i = r.GetOrdinal(col); return r.IsDBNull(i) ? null : r.GetDecimal(i); }

    public static DateTime GetDateTime(this SqlDataReader r, string col)
    { int i = r.GetOrdinal(col); return r.IsDBNull(i) ? default : r.GetDateTime(i); }

    public static DateTime? GetNullableDateTime(this SqlDataReader r, string col)
    { int i = r.GetOrdinal(col); return r.IsDBNull(i) ? null : r.GetDateTime(i); }

    public static int GetInt(this SqlDataReader r, string col)
    { int i = r.GetOrdinal(col); return r.IsDBNull(i) ? 0 : r.GetInt32(i); }

    public static bool GetBool(this SqlDataReader r, string col)
    { int i = r.GetOrdinal(col); return !r.IsDBNull(i) && r.GetBoolean(i); }
}
