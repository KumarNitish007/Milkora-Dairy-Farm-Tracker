using Microsoft.Data.SqlClient;
using Milkora.Domain.Entities;
using Milkora.Domain.Interfaces;
using Milkora.Infrastructure.Persistence;

namespace Milkora.Infrastructure.Repositories;

public sealed class ExpenseRepository : RepositoryBase, IExpenseRepository
{
    public ExpenseRepository(IDbConnectionFactory factory) : base(factory) { }

    private static Expense Map(SqlDataReader r) => new()
    {
        ExpenseId   = r.GetGuid("ExpenseId"),
        ExpenseDate = r.GetDateTime("ExpenseDate"),
        Category    = r.GetString("Category"),
        Description = r.GetNullableString("Description"),
        Amount      = r.GetDecimal("Amount"),
        PaymentMode = r.GetNullableString("PaymentMode"),
        Notes       = r.GetNullableString("Notes"),
        CreatedAt   = r.GetDateTime("CreatedAt"),
        UpdatedAt   = r.GetDateTime("UpdatedAt"),
    };

    public Task<IReadOnlyList<Expense>> GetByDateRangeAsync(DateTime start, DateTime end, string? category, CancellationToken ct = default)
        => QueryListAsync("dbo.sp_GetExpensesByDateRange", cmd =>
        {
            AddParam(cmd, "@StartDate", start.Date);
            AddParam(cmd, "@EndDate", end.Date);
            AddParam(cmd, "@Category", category);
        }, Map, ct);

    public Task<Guid> InsertAsync(Expense e, CancellationToken ct = default)
        => ExecuteInsertAsync("dbo.sp_InsertExpense", cmd =>
        {
            AddParam(cmd, "@ExpenseId", e.ExpenseId == Guid.Empty ? null : e.ExpenseId);
            AddParam(cmd, "@ExpenseDate", e.ExpenseDate.Date);
            AddParam(cmd, "@Category", e.Category);
            AddParam(cmd, "@Description", e.Description);
            AddParam(cmd, "@Amount", e.Amount);
            AddParam(cmd, "@PaymentMode", e.PaymentMode);
            AddParam(cmd, "@Notes", e.Notes);
        }, ct);

    public Task<int> UpdateAsync(Expense e, CancellationToken ct = default)
        => ExecuteRowCountAsync("dbo.sp_UpdateExpense", cmd =>
        {
            AddParam(cmd, "@ExpenseId", e.ExpenseId);
            AddParam(cmd, "@ExpenseDate", e.ExpenseDate.Date);
            AddParam(cmd, "@Category", e.Category);
            AddParam(cmd, "@Description", e.Description);
            AddParam(cmd, "@Amount", e.Amount);
            AddParam(cmd, "@PaymentMode", e.PaymentMode);
            AddParam(cmd, "@Notes", e.Notes);
        }, ct);

    public Task<int> DeleteAsync(Guid id, CancellationToken ct = default)
        => ExecuteRowCountAsync("dbo.sp_DeleteExpense", cmd => AddParam(cmd, "@ExpenseId", id), ct);
}
