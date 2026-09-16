using Milkora.Domain.Entities;

namespace Milkora.Application.DTOs;

public class MilkLogDto
{
    public Guid LogId { get; set; }
    public Guid? AnimalId { get; set; }
    public string? TagNumber { get; set; }
    public string? AnimalName { get; set; }
    public DateTime LogDate { get; set; }
    public decimal MorningMilk { get; set; }
    public decimal EveningMilk { get; set; }
    public decimal? FatPercent { get; set; }
    public decimal? RatePerLitre { get; set; }
    public decimal TotalMilk { get; set; }
    public decimal TotalValue { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateMilkLogRequest
{
    public Guid? LogId { get; set; }
    public Guid? AnimalId { get; set; }
    public DateTime LogDate { get; set; }
    public decimal MorningMilk { get; set; }
    public decimal EveningMilk { get; set; }
    public decimal? FatPercent { get; set; }
    public decimal? RatePerLitre { get; set; }
    public string? Notes { get; set; }
}

public class UpdateMilkLogRequest
{
    public Guid? AnimalId { get; set; }
    public DateTime LogDate { get; set; }
    public decimal MorningMilk { get; set; }
    public decimal EveningMilk { get; set; }
    public decimal? FatPercent { get; set; }
    public decimal? RatePerLitre { get; set; }
    public string? Notes { get; set; }
}

public static class MilkLogMappings
{
    public static MilkLogDto ToDto(this MilkLog m) => new()
    {
        LogId = m.LogId, AnimalId = m.AnimalId, TagNumber = m.TagNumber, AnimalName = m.AnimalName,
        LogDate = m.LogDate, MorningMilk = m.MorningMilk, EveningMilk = m.EveningMilk,
        FatPercent = m.FatPercent, RatePerLitre = m.RatePerLitre, TotalMilk = m.TotalMilk,
        TotalValue = m.TotalValue, Notes = m.Notes, CreatedAt = m.CreatedAt, UpdatedAt = m.UpdatedAt,
    };

    public static MilkLog ToEntity(this CreateMilkLogRequest r) => new()
    {
        LogId = r.LogId ?? Guid.Empty, AnimalId = r.AnimalId, LogDate = r.LogDate,
        MorningMilk = r.MorningMilk, EveningMilk = r.EveningMilk, FatPercent = r.FatPercent,
        RatePerLitre = r.RatePerLitre, Notes = r.Notes,
    };

    public static MilkLog ToEntity(this UpdateMilkLogRequest r, Guid id) => new()
    {
        LogId = id, AnimalId = r.AnimalId, LogDate = r.LogDate, MorningMilk = r.MorningMilk,
        EveningMilk = r.EveningMilk, FatPercent = r.FatPercent, RatePerLitre = r.RatePerLitre, Notes = r.Notes,
    };
}
