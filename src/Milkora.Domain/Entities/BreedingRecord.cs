namespace Milkora.Domain.Entities;

/// <summary>Insemination / pregnancy / calving. Mirrors dbo.BreedingRecords.</summary>
public class BreedingRecord
{
    public Guid BreedingId { get; set; }
    public Guid? AnimalId { get; set; }
    public string? TagNumber { get; set; }
    public string? AnimalName { get; set; }
    public DateTime InseminationDate { get; set; }
    public string? BullDetails { get; set; }
    public string PregnancyStatus { get; set; } = "Inseminated";
    public DateTime? ExpectedDeliveryDate { get; set; }  // computed in DB (+280 days)
    public DateTime? CalvingDate { get; set; }
    public string? CalfDetails { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
