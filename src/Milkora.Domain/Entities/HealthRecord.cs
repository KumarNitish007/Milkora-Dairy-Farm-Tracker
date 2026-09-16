namespace Milkora.Domain.Entities;

/// <summary>Vaccination / deworming / checkup / treatment. Mirrors dbo.HealthRecords.</summary>
public class HealthRecord
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

/// <summary>Projection returned by sp_GetDueReminders.</summary>
public class DueReminder
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
