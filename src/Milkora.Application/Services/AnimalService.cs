using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;
using Milkora.Domain.Exceptions;
using Milkora.Domain.Interfaces;

namespace Milkora.Application.Services;

public sealed class AnimalService : IAnimalService
{
    private readonly IAnimalRepository _repo;
    public AnimalService(IAnimalRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<AnimalDto>> GetAllAsync(string? status, string? search, CancellationToken ct = default)
    {
        var animals = await _repo.GetAllAsync(status, search, ct);
        return animals.Select(a => a.ToDto()).ToList();
    }

    public async Task<AnimalDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var animal = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Animal), id);
        return animal.ToDto();
    }

    public Task<Guid> CreateAsync(CreateAnimalRequest request, CancellationToken ct = default)
        => _repo.InsertAsync(request.ToEntity(), ct);

    public async Task UpdateAsync(Guid id, UpdateAnimalRequest request, CancellationToken ct = default)
    {
        var rows = await _repo.UpdateAsync(request.ToEntity(id), ct);
        if (rows == 0) throw new NotFoundException(nameof(Domain.Entities.Animal), id);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var rows = await _repo.DeleteAsync(id, ct);
        if (rows == 0) throw new NotFoundException(nameof(Domain.Entities.Animal), id);
    }
}
