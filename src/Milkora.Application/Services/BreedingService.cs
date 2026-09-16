using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;
using Milkora.Domain.Exceptions;
using Milkora.Domain.Interfaces;

namespace Milkora.Application.Services;

public sealed class BreedingService : IBreedingService
{
    private readonly IBreedingRepository _repo;
    public BreedingService(IBreedingRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<BreedingRecordDto>> GetByAnimalAsync(Guid? animalId, CancellationToken ct = default)
        => (await _repo.GetByAnimalAsync(animalId, ct)).Select(b => b.ToDto()).ToList();

    public Task<Guid> CreateAsync(CreateBreedingRequest request, CancellationToken ct = default)
        => _repo.InsertAsync(request.ToEntity(), ct);

    public async Task UpdateCalvingAsync(Guid id, UpdateCalvingRequest request, CancellationToken ct = default)
    {
        var rows = await _repo.UpdateCalvingAsync(id, request.CalvingDate, request.CalfDetails, request.PregnancyStatus, ct);
        if (rows == 0) throw new NotFoundException("BreedingRecord", id);
    }
}
