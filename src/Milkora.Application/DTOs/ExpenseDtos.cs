using Milkora.Domain.Entities;

namespace Milkora.Application.DTOs;

public class ExpenseDto
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

public class CreateExpenseRequest
{
    public Guid? ExpenseId { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public string? PaymentMode { get; set; }
    public string? Notes { get; set; }
}

public class UpdateExpenseRequest
{
    public DateTime ExpenseDate { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public string? PaymentMode { get; set; }
    public string? Notes { get; set; }
}

public static class ExpenseMappings
{
    public static ExpenseDto ToDto(this Expense e) => new()
    {
        ExpenseId = e.ExpenseId, ExpenseDate = e.ExpenseDate, Category = e.Category, Description = e.Description,
        Amount = e.Amount, PaymentMode = e.PaymentMode, Notes = e.Notes, CreatedAt = e.CreatedAt, UpdatedAt = e.UpdatedAt,
    };

    public static Expense ToEntity(this CreateExpenseRequest r) => new()
    {
        ExpenseId = r.ExpenseId ?? Guid.Empty, ExpenseDate = r.ExpenseDate, Category = r.Category,
        Description = r.Description, Amount = r.Amount, PaymentMode = r.PaymentMode, Notes = r.Notes,
    };

    public static Expense ToEntity(this UpdateExpenseRequest r, Guid id) => new()
    {
        ExpenseId = id, ExpenseDate = r.ExpenseDate, Category = r.Category, Description = r.Description,
        Amount = r.Amount, PaymentMode = r.PaymentMode, Notes = r.Notes,
    };
}
