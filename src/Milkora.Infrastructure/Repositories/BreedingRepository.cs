using Microsoft.Data.SqlClient;
using Milkora.Domain.Entities;
using Milkora.Domain.Interfaces;
using Milkora.Infrastructure.Persistence;

namespace Milkora.Infrastructure.Repositories;

public sealed class BreedingRepository : RepositoryBase, IBreedingRepository
{
    public BreedingRepository(IDbConnectionFactory factory) : base(factory) { }

    private static BreedingRecord Map(SqlDataReader r) => new()
    {
        BreedingId           = r.GetGuid("BreedingId"),
        AnimalId             = r.GetNullableGuid("AnimalId"),
        TagNumber            = r.GetNullableString("TagNumber"),
        AnimalName           = r.GetNullableString("AnimalName"),
        InseminationDate     = r.GetDateTime("InseminationDate"),
        BullDetails          = r.GetNullableString("BullDetails"),
        PregnancyStatus      = r.GetString("PregnancyStatus"),
        ExpectedDeliveryDate = r.GetNullableDateTime("ExpectedDeliveryDate"),
        CalvingDate          = r.GetNullableDateTime("CalvingDate"),
        CalfDetails          = r.GetNullableString("CalfDetails"),
        Notes                = r.GetNullableString("Notes"),
        CreatedAt            = r.GetDateTime("CreatedAt"),
        UpdatedAt            = r.GetDateTime("UpdatedAt"),
    };

    public Task<IReadOnlyList<BreedingRecord>> GetByAnimalAsync(Guid? animalId, CancellationToken ct = default)
        => QueryListAsync("dbo.sp_GetBreedingRecordsByAnimal", cmd => AddParam(cmd, "@AnimalId", animalId), Map, ct);

    public Task<Guid> InsertAsync(BreedingRecord b, CancellationToken ct = default)
        => ExecuteInsertAsync("dbo.sp_InsertBreedingRecord", cmd =>
        {
            AddParam(cmd, "@BreedingId", b.BreedingId == Guid.Empty ? null : b.BreedingId);
            AddParam(cmd, "@AnimalId", b.AnimalId);
            AddParam(cmd, "@InseminationDate", b.InseminationDate.Date);
            AddParam(cmd, "@BullDetails", b.BullDetails);
            AddParam(cmd, "@PregnancyStatus", b.PregnancyStatus);
            AddParam(cmd, "@Notes", b.Notes);
        }, ct);

    public Task<int> UpdateCalvingAsync(Guid breedingId, DateTime calvingDate, string? calfDetails, string pregnancyStatus, CancellationToken ct = default)
        => ExecuteRowCountAsync("dbo.sp_UpdateCalving", cmd =>
        {
            AddParam(cmd, "@BreedingId", breedingId);
            AddParam(cmd, "@CalvingDate", calvingDate.Date);
            AddParam(cmd, "@CalfDetails", calfDetails);
            AddParam(cmd, "@PregnancyStatus", pregnancyStatus);
        }, ct);
}
