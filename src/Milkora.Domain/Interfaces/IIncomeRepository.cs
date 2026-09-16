using Milkora.Domain.Entities;

namespace Milkora.Domain.Interfaces;

public interface IIncomeRepository
{
    Task<IReadOnlyList<Income>> GetByDateRangeAsync(DateTime start, DateTime end, string? category, CancellationToken ct = default);
    Task<Guid> InsertAsync(Income income, CancellationToken ct = default);
}
