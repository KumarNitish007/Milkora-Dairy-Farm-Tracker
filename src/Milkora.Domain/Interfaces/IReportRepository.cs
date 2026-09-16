using Milkora.Domain.Entities;

namespace Milkora.Domain.Interfaces;

public interface IReportRepository
{
    Task<DashboardSummary?> GetDashboardSummaryAsync(DateTime? date, CancellationToken ct = default);
    Task<MonthlyReport?> GetMonthlyReportAsync(int year, int month, CancellationToken ct = default);
    Task<ProfitLossReport?> GetProfitLossReportAsync(DateTime start, DateTime end, CancellationToken ct = default);
    Task<IReadOnlyList<PerAnimalYield>> GetPerAnimalYieldAsync(DateTime start, DateTime end, CancellationToken ct = default);
}
