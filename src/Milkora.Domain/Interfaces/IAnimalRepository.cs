using Milkora.Domain.Entities;

namespace Milkora.Domain.Interfaces;

public interface IAnimalRepository
{
    Task<IReadOnlyList<Animal>> GetAllAsync(string? status, string? search, CancellationToken ct = default);
    Task<Animal?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Guid> InsertAsync(Animal animal, CancellationToken ct = default);
    Task<int> UpdateAsync(Animal animal, CancellationToken ct = default);
    Task<int> DeleteAsync(Guid id, CancellationToken ct = default);
}
