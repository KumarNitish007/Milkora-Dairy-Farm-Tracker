namespace Milkora.Domain.Entities;

/// <summary>Daily morning/evening milk entry. Mirrors dbo.MilkProductionLog.</summary>
public class MilkLog
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
    public decimal TotalMilk { get; set; }   // computed in DB
    public decimal TotalValue { get; set; }   // computed in DB
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
