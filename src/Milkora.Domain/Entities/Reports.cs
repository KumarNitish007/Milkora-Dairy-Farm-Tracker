namespace Milkora.Domain.Entities;

/// <summary>Read-model returned by sp_GetDashboardSummary.</summary>
public class DashboardSummary
{
    public DateTime Date { get; set; }
    public decimal TotalMilkLitres { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal ProfitOrLoss { get; set; }
    public int ActiveAnimals { get; set; }
    public int SickAnimals { get; set; }
    public int DueReminders { get; set; }
    public int LowStockItems { get; set; }
    public int PendingPayments { get; set; }
}

/// <summary>Read-model returned by sp_GetMonthlyReport.</summary>
public class MonthlyReport
{
    public int Year { get; set; }
    public int Month { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalMilkLitres { get; set; }
    public decimal MilkValue { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal ProfitOrLoss { get; set; }
    public int SalesCount { get; set; }
}

public class CategoryAmount
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

/// <summary>Read-model composed from the three result sets of sp_GetProfitLossReport.</summary>
public class ProfitLossReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal ProfitOrLoss { get; set; }
    public List<CategoryAmount> IncomeByCategory { get; set; } = new();
    public List<CategoryAmount> ExpenseByCategory { get; set; } = new();
}

/// <summary>Read-model returned by sp_GetPerAnimalYield.</summary>
public class PerAnimalYield
{
    public Guid AnimalId { get; set; }
    public string TagNumber { get; set; } = string.Empty;
    public string? AnimalName { get; set; }
    public string? Status { get; set; }
    public decimal TotalMilkLitres { get; set; }
    public decimal TotalValue { get; set; }
    public int DaysRecorded { get; set; }
    public decimal AvgDailyYield { get; set; }
}
