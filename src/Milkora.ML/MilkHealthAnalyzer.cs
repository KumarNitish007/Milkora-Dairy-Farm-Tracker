namespace Milkora.ML;

/// <summary>A screening assessment of an animal's health inferred from its milk
/// series. NOT a diagnosis — an early-warning aid that tells the farmer which
/// animals to inspect.</summary>
public sealed record HealthAssessment(
    string Status,                     // Healthy | Watch | AtRisk | Unknown
    int Score,                         // 0-100 (higher = healthier)
    IReadOnlyList<string> Reasons,
    double BaselineAvg,
    double RecentAvg,
    double TrendPercent,               // last-week vs previous, negative = declining
    double DropPercent);               // recent vs baseline, positive = a drop

/// <summary>Turns a daily milk series into a health signal by combining a
/// recent-drop check, a weekly trend, and output variability.</summary>
public sealed class MilkHealthAnalyzer
{
    private readonly MilkAnomalyDetector _anomaly = new();

    public HealthAssessment Analyze(IReadOnlyList<double> series)
    {
        if (series.Count < 5)
            return new HealthAssessment("Unknown", 0,
                new[] { "Not enough milk history yet to assess health." }, 0, 0, 0, 0);

        var baseline = Median(series);
        var recent = series.TakeLast(Math.Min(3, series.Count)).Average();
        var drop = baseline > 0 ? (baseline - recent) / baseline * 100.0 : 0;

        var last7 = series.TakeLast(7).Average();
        var prior = series.Count > 7 ? series.SkipLast(7).TakeLast(7).Average() : last7;
        var trend = prior > 0 ? (last7 - prior) / prior * 100.0 : 0;

        var mean = series.Average();
        var sd = Math.Sqrt(series.Select(v => (v - mean) * (v - mean)).Average());
        var cv = mean > 0 ? sd / mean : 0;

        // A drop that started within the last week is the strongest warning.
        var recentAnomaly = _anomaly.Detect(series)
            .Where(a => a.Index >= series.Count - 7 && a.DropPercent >= 15)
            .OrderByDescending(a => a.DropPercent)
            .FirstOrDefault();

        var reasons = new List<string>();
        var score = 100;
        string status;

        if (drop >= 30 || (recentAnomaly is not null && recentAnomaly.DropPercent >= 30))
        {
            status = "AtRisk";
            score -= 55;
            var d = Math.Round(Math.Max(drop, recentAnomaly?.DropPercent ?? 0), 0);
            reasons.Add($"Milk is down ~{d}% versus its normal level — a possible sign of illness, mastitis or heat stress. Inspect the animal and consult a vet.");
        }
        else if (drop >= 15 || trend <= -15 || recentAnomaly is not null)
        {
            status = "Watch";
            score -= 28;
            if (drop >= 15) reasons.Add($"Milk is about {Math.Round(drop, 0)}% below its normal level — keep an eye on it.");
            if (trend <= -15) reasons.Add($"Yield has been trending down (~{Math.Round(trend, 0)}%) over the past week.");
            if (recentAnomaly is not null && drop < 15 && trend > -15)
                reasons.Add($"An unusual dip was detected recently (~{recentAnomaly.DropPercent}% below expected).");
        }
        else
        {
            status = "Healthy";
            reasons.Add("Milk output is steady and near its normal level.");
        }

        if (cv > 0.35)
        {
            score -= 12;
            reasons.Add("Daily output is unusually erratic — check the milking routine and feed.");
        }

        score = Math.Clamp(score, 0, 100);
        return new HealthAssessment(status, score, reasons,
            Math.Round(baseline, 2), Math.Round(recent, 2), Math.Round(trend, 1), Math.Round(drop, 1));
    }

    private static double Median(IReadOnlyList<double> values)
    {
        var sorted = values.OrderBy(v => v).ToList();
        var n = sorted.Count;
        return n % 2 == 1 ? sorted[n / 2] : (sorted[n / 2 - 1] + sorted[n / 2]) / 2.0;
    }
}
