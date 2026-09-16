using Milkora.Application.DTOs;

namespace Milkora.Application.Interfaces;

public interface IAnimalService
{
    Task<IReadOnlyList<AnimalDto>> GetAllAsync(string? status, string? search, CancellationToken ct = default);
    Task<AnimalDto> GetByIdAsync(Guid id, CancellationToken ct = default);       // throws NotFoundException
    Task<Guid> CreateAsync(CreateAnimalRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateAnimalRequest request, CancellationToken ct = default); // throws NotFoundException
    Task DeleteAsync(Guid id, CancellationToken ct = default);                    // throws NotFoundException
}
