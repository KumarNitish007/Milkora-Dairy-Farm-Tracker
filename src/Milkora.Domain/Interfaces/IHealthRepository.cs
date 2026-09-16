using Milkora.Domain.Entities;

namespace Milkora.Domain.Interfaces;

public interface IHealthRepository
{
    Task<IReadOnlyList<HealthRecord>> GetByAnimalAsync(Guid? animalId, string? recordType, CancellationToken ct = default);
    Task<IReadOnlyList<DueReminder>> GetDueRemindersAsync(DateTime? asOf, int daysAhead, CancellationToken ct = default);
    Task<Guid> InsertAsync(HealthRecord record, CancellationToken ct = default);
}
