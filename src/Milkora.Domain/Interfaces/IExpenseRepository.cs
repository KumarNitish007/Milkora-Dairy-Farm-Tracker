using Milkora.Domain.Entities;

namespace Milkora.Domain.Interfaces;

public interface IExpenseRepository
{
    Task<IReadOnlyList<Expense>> GetByDateRangeAsync(DateTime start, DateTime end, string? category, CancellationToken ct = default);
    Task<Guid> InsertAsync(Expense expense, CancellationToken ct = default);
    Task<int> UpdateAsync(Expense expense, CancellationToken ct = default);
    Task<int> DeleteAsync(Guid id, CancellationToken ct = default);
}
