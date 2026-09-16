using Milkora.Domain.Entities;

namespace Milkora.Application.DTOs;

public class IncomeDto
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

public class CreateIncomeRequest
{
    public Guid? IncomeId { get; set; }
    public DateTime IncomeDate { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
}

public static class IncomeMappings
{
    public static IncomeDto ToDto(this Income i) => new()
    {
        IncomeId = i.IncomeId, IncomeDate = i.IncomeDate, Category = i.Category, Description = i.Description,
        Amount = i.Amount, Notes = i.Notes, CreatedAt = i.CreatedAt, UpdatedAt = i.UpdatedAt,
    };

    public static Income ToEntity(this CreateIncomeRequest r) => new()
    {
        IncomeId = r.IncomeId ?? Guid.Empty, IncomeDate = r.IncomeDate, Category = r.Category,
        Description = r.Description, Amount = r.Amount, Notes = r.Notes,
    };
}
