using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;
using Milkora.Domain.Exceptions;
using Milkora.Domain.Interfaces;

namespace Milkora.Application.Services;

public sealed class ReportService : IReportService
{
    private readonly IReportRepository _repo;
    public ReportService(IReportRepository repo) => _repo = repo;

    public async Task<DashboardSummaryDto> GetDashboardAsync(DateTime? date, CancellationToken ct = default)
    {
        var summary = await _repo.GetDashboardSummaryAsync(date, ct)
            ?? throw new NotFoundException("Dashboard summary could not be produced.");
        return summary.ToDto();
    }

    public async Task<MonthlyReportDto> GetMonthlyAsync(int year, int month, CancellationToken ct = default)
    {
        var report = await _repo.GetMonthlyReportAsync(year, month, ct)
            ?? throw new NotFoundException("Monthly report could not be produced.");
        return report.ToDto();
    }

    public async Task<ProfitLossReportDto> GetProfitLossAsync(DateTime start, DateTime end, CancellationToken ct = default)
    {
        var report = await _repo.GetProfitLossReportAsync(start, end, ct)
            ?? throw new NotFoundException("Profit & loss report could not be produced.");
        return report.ToDto();
    }

    public async Task<IReadOnlyList<PerAnimalYieldDto>> GetPerAnimalYieldAsync(DateTime start, DateTime end, CancellationToken ct = default)
        => (await _repo.GetPerAnimalYieldAsync(start, end, ct)).Select(y => y.ToDto()).ToList();
}
