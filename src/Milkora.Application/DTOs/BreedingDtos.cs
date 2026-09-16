using Milkora.Domain.Entities;

namespace Milkora.Application.DTOs;

public class BreedingRecordDto
{
    public Guid BreedingId { get; set; }
    public Guid? AnimalId { get; set; }
    public string? TagNumber { get; set; }
    public string? AnimalName { get; set; }
    public DateTime InseminationDate { get; set; }
    public string? BullDetails { get; set; }
    public string PregnancyStatus { get; set; } = "Inseminated";
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? CalvingDate { get; set; }
    public string? CalfDetails { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateBreedingRequest
{
    public Guid? BreedingId { get; set; }
    public Guid? AnimalId { get; set; }
    public DateTime InseminationDate { get; set; }
    public string? BullDetails { get; set; }
    public string PregnancyStatus { get; set; } = "Inseminated";
    public string? Notes { get; set; }
}

public class UpdateCalvingRequest
{
    public DateTime CalvingDate { get; set; }
    public string? CalfDetails { get; set; }
    public string PregnancyStatus { get; set; } = "Delivered";
}

public static class BreedingMappings
{
    public static BreedingRecordDto ToDto(this BreedingRecord b) => new()
    {
        BreedingId = b.BreedingId, AnimalId = b.AnimalId, TagNumber = b.TagNumber, AnimalName = b.AnimalName,
        InseminationDate = b.InseminationDate, BullDetails = b.BullDetails, PregnancyStatus = b.PregnancyStatus,
        ExpectedDeliveryDate = b.ExpectedDeliveryDate, CalvingDate = b.CalvingDate, CalfDetails = b.CalfDetails,
        Notes = b.Notes, CreatedAt = b.CreatedAt, UpdatedAt = b.UpdatedAt,
    };

    public static BreedingRecord ToEntity(this CreateBreedingRequest r) => new()
    {
        BreedingId = r.BreedingId ?? Guid.Empty, AnimalId = r.AnimalId, InseminationDate = r.InseminationDate,
        BullDetails = r.BullDetails, PregnancyStatus = r.PregnancyStatus, Notes = r.Notes,
    };
}
