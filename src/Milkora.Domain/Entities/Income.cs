namespace Milkora.Domain.Entities;

/// <summary>Income record. Mirrors dbo.Income.</summary>
public class Income
{
    public Guid IncomeId { get; set; }
    public DateTime IncomeDate { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
