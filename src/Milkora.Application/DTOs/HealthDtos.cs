using Milkora.Domain.Entities;

namespace Milkora.Application.DTOs;

public class HealthRecordDto
{
    public Guid HealthId { get; set; }
    public Guid? AnimalId { get; set; }
    public string? TagNumber { get; set; }
    public string? AnimalName { get; set; }
    public DateTime RecordDate { get; set; }
    public string RecordType { get; set; } = string.Empty;
    public string? MedicineName { get; set; }
    public string? VetName { get; set; }
    public string? VetContact { get; set; }
    public decimal? Cost { get; set; }
    public DateTime? NextDueDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class DueReminderDto
{
    public Guid HealthId { get; set; }
    public Guid? AnimalId { get; set; }
    public string? TagNumber { get; set; }
    public string? AnimalName { get; set; }
    public string RecordType { get; set; } = string.Empty;
    public string? MedicineName { get; set; }
    public DateTime? NextDueDate { get; set; }
    public int DaysUntilDue { get; set; }
}

public class CreateHealthRecordRequest
{
    public Guid? HealthId { get; set; }
    public Guid? AnimalId { get; set; }
    public DateTime RecordDate { get; set; }
    public string RecordType { get; set; } = string.Empty;
    public string? MedicineName { get; set; }
    public string? VetName { get; set; }
    public string? VetContact { get; set; }
    public decimal? Cost { get; set; }
    public DateTime? NextDueDate { get; set; }
    public string? Notes { get; set; }
}

public static class HealthMappings
{
    public static HealthRecordDto ToDto(this HealthRecord h) => new()
    {
        HealthId = h.HealthId, AnimalId = h.AnimalId, TagNumber = h.TagNumber, AnimalName = h.AnimalName,
        RecordDate = h.RecordDate, RecordType = h.RecordType, MedicineName = h.MedicineName,
        VetName = h.VetName, VetContact = h.VetContact, Cost = h.Cost, NextDueDate = h.NextDueDate,
        Notes = h.Notes, CreatedAt = h.CreatedAt, UpdatedAt = h.UpdatedAt,
    };

    public static DueReminderDto ToDto(this DueReminder d) => new()
    {
        HealthId = d.HealthId, AnimalId = d.AnimalId, TagNumber = d.TagNumber, AnimalName = d.AnimalName,
        RecordType = d.RecordType, MedicineName = d.MedicineName, NextDueDate = d.NextDueDate, DaysUntilDue = d.DaysUntilDue,
    };

    public static HealthRecord ToEntity(this CreateHealthRecordRequest r) => new()
    {
        HealthId = r.HealthId ?? Guid.Empty, AnimalId = r.AnimalId, RecordDate = r.RecordDate,
        RecordType = r.RecordType, MedicineName = r.MedicineName, VetName = r.VetName,
        VetContact = r.VetContact, Cost = r.Cost, NextDueDate = r.NextDueDate, Notes = r.Notes,
    };
}
