using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;
using Milkora.Domain.Interfaces;

namespace Milkora.Application.Services;

public sealed class HealthService : IHealthService
{
    private readonly IHealthRepository _repo;
    public HealthService(IHealthRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<HealthRecordDto>> GetByAnimalAsync(Guid? animalId, string? recordType, CancellationToken ct = default)
        => (await _repo.GetByAnimalAsync(animalId, recordType, ct)).Select(h => h.ToDto()).ToList();

    public async Task<IReadOnlyList<DueReminderDto>> GetDueRemindersAsync(DateTime? asOf, int daysAhead, CancellationToken ct = default)
        => (await _repo.GetDueRemindersAsync(asOf, daysAhead, ct)).Select(d => d.ToDto()).ToList();

    public Task<Guid> CreateAsync(CreateHealthRecordRequest request, CancellationToken ct = default)
        => _repo.InsertAsync(request.ToEntity(), ct);
}
