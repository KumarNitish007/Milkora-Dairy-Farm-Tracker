using System.Data;
using Microsoft.Data.SqlClient;
using Milkora.Domain.Entities;
using Milkora.Domain.Interfaces;
using Milkora.Infrastructure.Persistence;

namespace Milkora.Infrastructure.Repositories;

public sealed class ReportRepository : RepositoryBase, IReportRepository
{
    private readonly IDbConnectionFactory _factory;

    public ReportRepository(IDbConnectionFactory factory) : base(factory) => _factory = factory;

    public Task<DashboardSummary?> GetDashboardSummaryAsync(DateTime? date, CancellationToken ct = default)
        => QuerySingleAsync("dbo.sp_GetDashboardSummary", cmd => AddParam(cmd, "@Date", date?.Date),
            r => new DashboardSummary
            {
                Date            = r.GetDateTime("Date"),
                TotalMilkLitres = r.GetDecimal("TotalMilkLitres"),
                TotalIncome     = r.GetDecimal("TotalIncome"),
                TotalExpense    = r.GetDecimal("TotalExpense"),
                ProfitOrLoss    = r.GetDecimal("ProfitOrLoss"),
                ActiveAnimals   = r.GetInt("ActiveAnimals"),
                SickAnimals     = r.GetInt("SickAnimals"),
                DueReminders    = r.GetInt("DueReminders"),
                LowStockItems   = r.GetInt("LowStockItems"),
                PendingPayments = r.GetInt("PendingPayments"),
            }, ct);

    public Task<MonthlyReport?> GetMonthlyReportAsync(int year, int month, CancellationToken ct = default)
        => QuerySingleAsync("dbo.sp_GetMonthlyReport", cmd =>
            {
                AddParam(cmd, "@Year", year);
                AddParam(cmd, "@Month", month);
            },
            r => new MonthlyReport
            {
                Year            = r.GetInt("Year"),
                Month           = r.GetInt("Month"),
                StartDate       = r.GetDateTime("StartDate"),
                EndDate         = r.GetDateTime("EndDate"),
                TotalMilkLitres = r.GetDecimal("TotalMilkLitres"),
                MilkValue       = r.GetDecimal("MilkValue"),
                TotalIncome     = r.GetDecimal("TotalIncome"),
                TotalExpense    = r.GetDecimal("TotalExpense"),
                ProfitOrLoss    = r.GetDecimal("ProfitOrLoss"),
                SalesCount      = r.GetInt("SalesCount"),
            }, ct);

    public Task<IReadOnlyList<PerAnimalYield>> GetPerAnimalYieldAsync(DateTime start, DateTime end, CancellationToken ct = default)
        => QueryListAsync("dbo.sp_GetPerAnimalYield", cmd =>
            {
                AddParam(cmd, "@StartDate", start.Date);
                AddParam(cmd, "@EndDate", end.Date);
            },
            r => new PerAnimalYield
            {
                AnimalId        = r.GetGuid("AnimalId"),
                TagNumber       = r.GetString("TagNumber"),
                AnimalName      = r.GetNullableString("AnimalName"),
                Status          = r.GetNullableString("Status"),
                TotalMilkLitres = r.GetDecimal("TotalMilkLitres"),
                TotalValue      = r.GetDecimal("TotalValue"),
                DaysRecorded    = r.GetInt("DaysRecorded"),
                AvgDailyYield   = r.GetDecimal("AvgDailyYield"),
            }, ct);

    // Three result sets: totals, income-by-category, expense-by-category.
    public async Task<ProfitLossReport?> GetProfitLossReportAsync(DateTime start, DateTime end, CancellationToken ct = default)
    {
        await using var conn = _factory.Create();
        await using var cmd = new SqlCommand("dbo.sp_GetProfitLossReport", conn) { CommandType = CommandType.StoredProcedure };
        AddParam(cmd, "@StartDate", start.Date);
        AddParam(cmd, "@EndDate", end.Date);

        await conn.OpenAsync(ct);
        await using var r = await cmd.ExecuteReaderAsync(ct);

        if (!await r.ReadAsync(ct)) return null;
        var report = new ProfitLossReport
        {
            StartDate    = r.GetDateTime("StartDate"),
            EndDate      = r.GetDateTime("EndDate"),
            TotalIncome  = r.GetDecimal("TotalIncome"),
            TotalExpense = r.GetDecimal("TotalExpense"),
            ProfitOrLoss = r.GetDecimal("ProfitOrLoss"),
        };

        if (await r.NextResultAsync(ct))
            while (await r.ReadAsync(ct))
                report.IncomeByCategory.Add(new CategoryAmount { Category = r.GetString("Category"), Amount = r.GetDecimal("Amount") });

        if (await r.NextResultAsync(ct))
            while (await r.ReadAsync(ct))
                report.ExpenseByCategory.Add(new CategoryAmount { Category = r.GetString("Category"), Amount = r.GetDecimal("Amount") });

        return report;
    }
}
