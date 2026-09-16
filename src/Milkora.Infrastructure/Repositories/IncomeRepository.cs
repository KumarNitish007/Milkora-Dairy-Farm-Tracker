using Microsoft.Data.SqlClient;
using Milkora.Domain.Entities;
using Milkora.Domain.Interfaces;
using Milkora.Infrastructure.Persistence;

namespace Milkora.Infrastructure.Repositories;

public sealed class IncomeRepository : RepositoryBase, IIncomeRepository
{
    public IncomeRepository(IDbConnectionFactory factory) : base(factory) { }

    private static Income Map(SqlDataReader r) => new()
    {
        IncomeId    = r.GetGuid("IncomeId"),
        IncomeDate  = r.GetDateTime("IncomeDate"),
        Category    = r.GetString("Category"),
        Description = r.GetNullableString("Description"),
        Amount      = r.GetDecimal("Amount"),
        Notes       = r.GetNullableString("Notes"),
        CreatedAt   = r.GetDateTime("CreatedAt"),
        UpdatedAt   = r.GetDateTime("UpdatedAt"),
    };

    public Task<IReadOnlyList<Income>> GetByDateRangeAsync(DateTime start, DateTime end, string? category, CancellationToken ct = default)
        => QueryListAsync("dbo.sp_GetIncomeByDateRange", cmd =>
        {
            AddParam(cmd, "@StartDate", start.Date);
            AddParam(cmd, "@EndDate", end.Date);
            AddParam(cmd, "@Category", category);
        }, Map, ct);

    public Task<Guid> InsertAsync(Income i, CancellationToken ct = default)
        => ExecuteInsertAsync("dbo.sp_InsertIncome", cmd =>
        {
            AddParam(cmd, "@IncomeId", i.IncomeId == Guid.Empty ? null : i.IncomeId);
            AddParam(cmd, "@IncomeDate", i.IncomeDate.Date);
            AddParam(cmd, "@Category", i.Category);
            AddParam(cmd, "@Description", i.Description);
            AddParam(cmd, "@Amount", i.Amount);
            AddParam(cmd, "@Notes", i.Notes);
        }, ct);
}
