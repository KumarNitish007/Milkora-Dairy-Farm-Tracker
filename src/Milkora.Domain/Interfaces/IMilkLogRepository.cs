using Milkora.Domain.Entities;

namespace Milkora.Domain.Interfaces;

public interface IMilkLogRepository
{
    Task<IReadOnlyList<MilkLog>> GetByDateAsync(DateTime date, CancellationToken ct = default);
    Task<IReadOnlyList<MilkLog>> GetByDateRangeAsync(DateTime start, DateTime end, Guid? animalId, CancellationToken ct = default);
    Task<Guid> InsertAsync(MilkLog log, CancellationToken ct = default);
    Task<int> UpdateAsync(MilkLog log, CancellationToken ct = default);
}
