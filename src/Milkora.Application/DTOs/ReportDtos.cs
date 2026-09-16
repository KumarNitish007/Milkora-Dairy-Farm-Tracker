using Milkora.Domain.Entities;

namespace Milkora.Application.DTOs;

public class DashboardSummaryDto
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

public class MonthlyReportDto
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

public class CategoryAmountDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class ProfitLossReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal ProfitOrLoss { get; set; }
    public List<CategoryAmountDto> IncomeByCategory { get; set; } = new();
    public List<CategoryAmountDto> ExpenseByCategory { get; set; } = new();
}

public class PerAnimalYieldDto
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

public static class ReportMappings
{
    public static DashboardSummaryDto ToDto(this DashboardSummary s) => new()
    {
        Date = s.Date, TotalMilkLitres = s.TotalMilkLitres, TotalIncome = s.TotalIncome,
        TotalExpense = s.TotalExpense, ProfitOrLoss = s.ProfitOrLoss, ActiveAnimals = s.ActiveAnimals,
        SickAnimals = s.SickAnimals, DueReminders = s.DueReminders, LowStockItems = s.LowStockItems,
        PendingPayments = s.PendingPayments,
    };

    public static MonthlyReportDto ToDto(this MonthlyReport m) => new()
    {
        Year = m.Year, Month = m.Month, StartDate = m.StartDate, EndDate = m.EndDate,
        TotalMilkLitres = m.TotalMilkLitres, MilkValue = m.MilkValue, TotalIncome = m.TotalIncome,
        TotalExpense = m.TotalExpense, ProfitOrLoss = m.ProfitOrLoss, SalesCount = m.SalesCount,
    };

    public static ProfitLossReportDto ToDto(this ProfitLossReport p) => new()
    {
        StartDate = p.StartDate, EndDate = p.EndDate, TotalIncome = p.TotalIncome,
        TotalExpense = p.TotalExpense, ProfitOrLoss = p.ProfitOrLoss,
        IncomeByCategory = p.IncomeByCategory.Select(c => new CategoryAmountDto { Category = c.Category, Amount = c.Amount }).ToList(),
        ExpenseByCategory = p.ExpenseByCategory.Select(c => new CategoryAmountDto { Category = c.Category, Amount = c.Amount }).ToList(),
    };

    public static PerAnimalYieldDto ToDto(this PerAnimalYield y) => new()
    {
        AnimalId = y.AnimalId, TagNumber = y.TagNumber, AnimalName = y.AnimalName, Status = y.Status,
        TotalMilkLitres = y.TotalMilkLitres, TotalValue = y.TotalValue, DaysRecorded = y.DaysRecorded,
        AvgDailyYield = y.AvgDailyYield,
    };
}
