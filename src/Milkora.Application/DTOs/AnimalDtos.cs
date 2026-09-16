using Milkora.Domain.Entities;

namespace Milkora.Application.DTOs;

public class AnimalDto
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

public class CreateAnimalRequest
{
    /// <summary>Optional client-generated GUID (offline-first). Omit to let the server generate one.</summary>
    public Guid? AnimalId { get; set; }
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
}

public class UpdateAnimalRequest
{
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
}

/// <summary>Hand-written mappers (no reflection => fast).</summary>
public static class AnimalMappings
{
    public static AnimalDto ToDto(this Animal a) => new()
    {
        AnimalId = a.AnimalId, TagNumber = a.TagNumber, Name = a.Name, Type = a.Type,
        Breed = a.Breed, Gender = a.Gender, DateOfBirth = a.DateOfBirth, PurchaseDate = a.PurchaseDate,
        PurchasePrice = a.PurchasePrice, Status = a.Status, DailyMilkYield = a.DailyMilkYield,
        PhotoPath = a.PhotoPath, Notes = a.Notes, CreatedAt = a.CreatedAt, UpdatedAt = a.UpdatedAt,
    };

    public static Animal ToEntity(this CreateAnimalRequest r) => new()
    {
        AnimalId = r.AnimalId ?? Guid.Empty, TagNumber = r.TagNumber, Name = r.Name, Type = r.Type,
        Breed = r.Breed, Gender = r.Gender, DateOfBirth = r.DateOfBirth, PurchaseDate = r.PurchaseDate,
        PurchasePrice = r.PurchasePrice, Status = r.Status, DailyMilkYield = r.DailyMilkYield,
        PhotoPath = r.PhotoPath, Notes = r.Notes,
    };

    public static Animal ToEntity(this UpdateAnimalRequest r, Guid id) => new()
    {
        AnimalId = id, TagNumber = r.TagNumber, Name = r.Name, Type = r.Type, Breed = r.Breed,
        Gender = r.Gender, DateOfBirth = r.DateOfBirth, PurchaseDate = r.PurchaseDate,
        PurchasePrice = r.PurchasePrice, Status = r.Status, DailyMilkYield = r.DailyMilkYield,
        PhotoPath = r.PhotoPath, Notes = r.Notes,
    };
}
