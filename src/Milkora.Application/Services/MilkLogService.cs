using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;
using Milkora.Domain.Exceptions;
using Milkora.Domain.Interfaces;

namespace Milkora.Application.Services;

public sealed class MilkLogService : IMilkLogService
{
    private readonly IMilkLogRepository _repo;
    public MilkLogService(IMilkLogRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<MilkLogDto>> GetByDateAsync(DateTime date, CancellationToken ct = default)
        => (await _repo.GetByDateAsync(date, ct)).Select(m => m.ToDto()).ToList();

    public async Task<IReadOnlyList<MilkLogDto>> GetByDateRangeAsync(DateTime start, DateTime end, Guid? animalId, CancellationToken ct = default)
        => (await _repo.GetByDateRangeAsync(start, end, animalId, ct)).Select(m => m.ToDto()).ToList();

    public Task<Guid> CreateAsync(CreateMilkLogRequest request, CancellationToken ct = default)
        => _repo.InsertAsync(request.ToEntity(), ct);

    public async Task UpdateAsync(Guid id, UpdateMilkLogRequest request, CancellationToken ct = default)
    {
        var rows = await _repo.UpdateAsync(request.ToEntity(id), ct);
        if (rows == 0) throw new NotFoundException("MilkLog", id);
    }
}
