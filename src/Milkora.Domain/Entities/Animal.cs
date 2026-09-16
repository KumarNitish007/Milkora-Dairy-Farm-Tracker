namespace Milkora.Domain.Entities;

/// <summary>Animal master record. Mirrors dbo.Animals.</summary>
public class Animal
{
    public Guid AnimalId { get; set; }
    public string TagNumber { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Breed { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string Status { get; set; } = "Milking";
    public decimal? DailyMilkYield { get; set; }
    public string? PhotoPath { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
