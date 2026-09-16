using Microsoft.Data.SqlClient;
using Milkora.Domain.Entities;
using Milkora.Domain.Interfaces;
using Milkora.Infrastructure.Persistence;

namespace Milkora.Infrastructure.Repositories;

public sealed class HealthRepository : RepositoryBase, IHealthRepository
{
    public HealthRepository(IDbConnectionFactory factory) : base(factory) { }

    private static HealthRecord MapRecord(SqlDataReader r) => new()
    {
        HealthId     = r.GetGuid("HealthId"),
        AnimalId     = r.GetNullableGuid("AnimalId"),
        TagNumber    = r.GetNullableString("TagNumber"),
        AnimalName   = r.GetNullableString("AnimalName"),
        RecordDate   = r.GetDateTime("RecordDate"),
        RecordType   = r.GetString("RecordType"),
        MedicineName = r.GetNullableString("MedicineName"),
        VetName      = r.GetNullableString("VetName"),
        VetContact   = r.GetNullableString("VetContact"),
        Cost         = r.GetNullableDecimal("Cost"),
        NextDueDate  = r.GetNullableDateTime("NextDueDate"),
        Notes        = r.GetNullableString("Notes"),
        CreatedAt    = r.GetDateTime("CreatedAt"),
        UpdatedAt    = r.GetDateTime("UpdatedAt"),
    };

    private static DueReminder MapReminder(SqlDataReader r) => new()
    {
        HealthId     = r.GetGuid("HealthId"),
        AnimalId     = r.GetNullableGuid("AnimalId"),
        TagNumber    = r.GetNullableString("TagNumber"),
        AnimalName   = r.GetNullableString("AnimalName"),
        RecordType   = r.GetString("RecordType"),
        MedicineName = r.GetNullableString("MedicineName"),
        NextDueDate  = r.GetNullableDateTime("NextDueDate"),
        DaysUntilDue = r.GetInt("DaysUntilDue"),
    };

    public Task<IReadOnlyList<HealthRecord>> GetByAnimalAsync(Guid? animalId, string? recordType, CancellationToken ct = default)
        => QueryListAsync("dbo.sp_GetHealthRecordsByAnimal", cmd =>
        {
            AddParam(cmd, "@AnimalId", animalId);
            AddParam(cmd, "@RecordType", recordType);
        }, MapRecord, ct);

    public Task<IReadOnlyList<DueReminder>> GetDueRemindersAsync(DateTime? asOf, int daysAhead, CancellationToken ct = default)
        => QueryListAsync("dbo.sp_GetDueReminders", cmd =>
        {
            AddParam(cmd, "@AsOfDate", asOf?.Date);
            AddParam(cmd, "@DaysAhead", daysAhead);
        }, MapReminder, ct);

    public Task<Guid> InsertAsync(HealthRecord h, CancellationToken ct = default)
        => ExecuteInsertAsync("dbo.sp_InsertHealthRecord", cmd =>
        {
            AddParam(cmd, "@HealthId", h.HealthId == Guid.Empty ? null : h.HealthId);
            AddParam(cmd, "@AnimalId", h.AnimalId);
            AddParam(cmd, "@RecordDate", h.RecordDate.Date);
            AddParam(cmd, "@RecordType", h.RecordType);
            AddParam(cmd, "@MedicineName", h.MedicineName);
            AddParam(cmd, "@VetName", h.VetName);
            AddParam(cmd, "@VetContact", h.VetContact);
            AddParam(cmd, "@Cost", h.Cost);
            AddParam(cmd, "@NextDueDate", h.NextDueDate?.Date);
            AddParam(cmd, "@Notes", h.Notes);
        }, ct);
}
