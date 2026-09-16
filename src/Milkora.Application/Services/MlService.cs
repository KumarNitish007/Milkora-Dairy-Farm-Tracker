using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;
using Milkora.Domain.Entities;
using Milkora.Domain.Interfaces;
using Milkora.ML;

namespace Milkora.Application.Services;

public sealed class MlService : IMlService
{
    private readonly IMilkLogRepository _milk;
    private readonly MilkForecaster _forecaster;
    private readonly MilkAnomalyDetector _detector;
    private readonly MilkHealthAnalyzer _health;

    private const int ForecastHistoryDays = 60;

    public MlService(IMilkLogRepository milk, MilkForecaster forecaster, MilkAnomalyDetector detector, MilkHealthAnalyzer health)
    {
        _milk = milk;
        _forecaster = forecaster;
        _detector = detector;
        _health = health;
    }

    public async Task<MilkForecastDto> GetForecastAsync(int days, CancellationToken ct = default)
    {
        days = Math.Clamp(days, 1, 30);
        var end = DateTime.Today;
        var start = end.AddDays(-ForecastHistoryDays);
        var logs = await _milk.GetByDateRangeAsync(start, end, null, ct);

        // Farm-wide daily totals, in date order.
        var daily = logs
            .GroupBy(l => l.LogDate.Date)
            .OrderBy(g => g.Key)
            .Select(g => (Date: g.Key, Total: (double)g.Sum(x => x.TotalMilk)))
            .ToList();

        if (daily.Count < 4)
            return new MilkForecastDto { Days = days, Basis = "none", Message = "Not enough milk history yet — keep logging daily and the forecast will appear." };

        var series = daily.Select(d => d.Total).ToList();
        var forecast = _forecaster.Forecast(series, days);
        var lastDate = daily[^1].Date;

        var points = forecast.Select(f => new ForecastPointDto
        {
            Step = f.Step,
            Date = lastDate.AddDays(f.Step).ToString("yyyy-MM-dd"),
            Value = f.Value,
            LowerBound = f.LowerBound,
            UpperBound = f.UpperBound,
        }).ToList();

        var total = Math.Round(points.Sum(p => p.Value), 2);
        return new MilkForecastDto
        {
            Days = days,
            ProjectedTotal = total,
            DailyAverage = Math.Round(total / days, 2),
            Basis = series.Count >= 8 ? "ssa" : "average",
            Points = points,
        };
    }

    public async Task<AnomalyReportDto> GetAnomaliesAsync(int lookbackDays, CancellationToken ct = default)
    {
        lookbackDays = Math.Clamp(lookbackDays, 7, 120);
        var end = DateTime.Today;
        var start = end.AddDays(-lookbackDays);
        var logs = await _milk.GetByDateRangeAsync(start, end, null, ct);

        var byAnimal = logs.Where(l => l.AnimalId.HasValue).GroupBy(l => l.AnimalId!.Value);
        var report = new AnomalyReportDto();
        var analysed = 0;

        foreach (var group in byAnimal)
        {
            var daily = group
                .GroupBy(l => l.LogDate.Date)
                .OrderBy(g => g.Key)
                .Select(g => (Date: g.Key, Total: (double)g.Sum(x => x.TotalMilk)))
                .ToList();

            if (daily.Count < 4) continue;
            analysed++;

            var series = daily.Select(d => d.Total).ToList();
            var first = group.First();

            foreach (var a in _detector.Detect(series))
            {
                report.Anomalies.Add(new MilkAnomalyDto
                {
                    AnimalId = group.Key,
                    TagNumber = first.TagNumber,
                    AnimalName = first.AnimalName,
                    Date = daily[a.Index].Date.ToString("yyyy-MM-dd"),
                    Value = a.Value,
                    Expected = a.Expected,
                    DropPercent = a.DropPercent,
                    Severity = a.DropPercent >= 40 ? "High" : a.DropPercent >= 25 ? "Medium" : "Low",
                });
            }
        }

        report.AnimalsAnalysed = analysed;
        report.Anomalies = report.Anomalies.OrderByDescending(a => a.DropPercent).Take(25).ToList();
        report.Message = analysed == 0
            ? "Not enough per-animal history yet to analyse."
            : report.Anomalies.Count == 0 ? "No abnormal milk drops detected." : null;
        return report;
    }

    private static readonly Dictionary<string, int> StatusRank = new()
    {
        ["AtRisk"] = 0, ["Watch"] = 1, ["Unknown"] = 2, ["Healthy"] = 3,
    };

    public async Task<IReadOnlyList<MilkHealthDto>> GetHealthChecksAsync(int lookbackDays, CancellationToken ct = default)
    {
        lookbackDays = Math.Clamp(lookbackDays, 7, 120);
        var end = DateTime.Today;
        var start = end.AddDays(-lookbackDays);
        var logs = await _milk.GetByDateRangeAsync(start, end, null, ct);

        var results = new List<MilkHealthDto>();

        foreach (var group in logs.Where(l => l.AnimalId.HasValue).GroupBy(l => l.AnimalId!.Value))
        {
            var series = group
                .GroupBy(l => l.LogDate.Date)
                .OrderBy(g => g.Key)
                .Select(g => (double)g.Sum(x => x.TotalMilk))
                .ToList();

            var a = _health.Analyze(series);
            var first = group.First();
            results.Add(new MilkHealthDto
            {
                AnimalId = group.Key,
                TagNumber = first.TagNumber,
                AnimalName = first.AnimalName,
                Status = a.Status,
                Score = a.Score,
                Reasons = a.Reasons.ToList(),
                BaselineAvg = a.BaselineAvg,
                RecentAvg = a.RecentAvg,
                TrendPercent = a.TrendPercent,
                DropPercent = a.DropPercent,
            });
        }

        // Most concerning first.
        return results
            .OrderBy(r => StatusRank.GetValueOrDefault(r.Status, 2))
            .ThenBy(r => r.Score)
            .ToList();
    }
}
