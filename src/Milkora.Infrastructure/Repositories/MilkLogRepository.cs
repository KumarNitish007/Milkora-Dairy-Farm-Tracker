using Microsoft.Data.SqlClient;
using Milkora.Domain.Entities;
using Milkora.Domain.Interfaces;
using Milkora.Infrastructure.Persistence;

namespace Milkora.Infrastructure.Repositories;

public sealed class MilkLogRepository : RepositoryBase, IMilkLogRepository
{
    public MilkLogRepository(IDbConnectionFactory factory) : base(factory) { }

    private static MilkLog Map(SqlDataReader r) => new()
    {
        LogId        = r.GetGuid("LogId"),
        AnimalId     = r.GetNullableGuid("AnimalId"),
        TagNumber    = r.GetNullableString("TagNumber"),
        AnimalName   = r.GetNullableString("AnimalName"),
        LogDate      = r.GetDateTime("LogDate"),
        MorningMilk  = r.GetDecimal("MorningMilk"),
        EveningMilk  = r.GetDecimal("EveningMilk"),
        FatPercent   = r.GetNullableDecimal("FatPercent"),
        RatePerLitre = r.GetNullableDecimal("RatePerLitre"),
        TotalMilk    = r.GetDecimal("TotalMilk"),
        TotalValue   = r.GetDecimal("TotalValue"),
        Notes        = r.GetNullableString("Notes"),
        CreatedAt    = r.GetDateTime("CreatedAt"),
        UpdatedAt    = r.GetDateTime("UpdatedAt"),
    };

    public Task<IReadOnlyList<MilkLog>> GetByDateAsync(DateTime date, CancellationToken ct = default)
        => QueryListAsync("dbo.sp_GetMilkLogsByDate", cmd => AddParam(cmd, "@LogDate", date.Date), Map, ct);

    public Task<IReadOnlyList<MilkLog>> GetByDateRangeAsync(DateTime start, DateTime end, Guid? animalId, CancellationToken ct = default)
        => QueryListAsync("dbo.sp_GetMilkLogsByDateRange", cmd =>
        {
            AddParam(cmd, "@StartDate", start.Date);
            AddParam(cmd, "@EndDate", end.Date);
            AddParam(cmd, "@AnimalId", animalId);
        }, Map, ct);

    public Task<Guid> InsertAsync(MilkLog m, CancellationToken ct = default)
        => ExecuteInsertAsync("dbo.sp_InsertMilkLog", cmd =>
        {
            AddParam(cmd, "@LogId", m.LogId == Guid.Empty ? null : m.LogId);
            AddParam(cmd, "@AnimalId", m.AnimalId);
            AddParam(cmd, "@LogDate", m.LogDate.Date);
            AddParam(cmd, "@MorningMilk", m.MorningMilk);
            AddParam(cmd, "@EveningMilk", m.EveningMilk);
            AddParam(cmd, "@FatPercent", m.FatPercent);
            AddParam(cmd, "@RatePerLitre", m.RatePerLitre);
            AddParam(cmd, "@Notes", m.Notes);
        }, ct);

    public Task<int> UpdateAsync(MilkLog m, CancellationToken ct = default)
        => ExecuteRowCountAsync("dbo.sp_UpdateMilkLog", cmd =>
        {
            AddParam(cmd, "@LogId", m.LogId);
            AddParam(cmd, "@AnimalId", m.AnimalId);
            AddParam(cmd, "@LogDate", m.LogDate.Date);
            AddParam(cmd, "@MorningMilk", m.MorningMilk);
            AddParam(cmd, "@EveningMilk", m.EveningMilk);
            AddParam(cmd, "@FatPercent", m.FatPercent);
            AddParam(cmd, "@RatePerLitre", m.RatePerLitre);
            AddParam(cmd, "@Notes", m.Notes);
        }, ct);
}
