using Milkora.Domain.Entities;

namespace Milkora.Domain.Interfaces;

public interface IBreedingRepository
{
    Task<IReadOnlyList<BreedingRecord>> GetByAnimalAsync(Guid? animalId, CancellationToken ct = default);
    Task<Guid> InsertAsync(BreedingRecord record, CancellationToken ct = default);
    Task<int> UpdateCalvingAsync(Guid breedingId, DateTime calvingDate, string? calfDetails, string pregnancyStatus, CancellationToken ct = default);
}
