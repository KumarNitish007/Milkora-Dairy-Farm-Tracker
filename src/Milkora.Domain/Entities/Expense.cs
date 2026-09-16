namespace Milkora.Domain.Entities;

/// <summary>Farm expense. Mirrors dbo.Expenses.</summary>
public class Expense
{
    public Guid ExpenseId { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public string? PaymentMode { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
