using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.TimeSeries;

namespace Milkora.ML;

/// <summary>
/// Detects abnormal DROPS in a daily milk series. Combines two signals:
///   1. ML.NET SR-CNN spike detection (good at sharp anomalies), and
///   2. a robust rolling-median baseline (catches the ONSET of a sustained drop,
///      which SR-CNN tends to absorb as a new "normal").
/// Only genuine drops (value materially below the recent baseline) are returned —
/// the early-warning signal for illness or heat stress.
/// </summary>
public sealed class MilkAnomalyDetector
{
    private sealed class Point { public double Value { get; set; } }

    private sealed class SrCnnOutput
    {
        [VectorType(7)] public double[] Prediction { get; set; } = Array.Empty<double>();
    }

    private const double MinDropPercent = 15.0;

    public IReadOnlyList<AnomalyResult> Detect(IReadOnlyList<double> series)
    {
        var map = new SortedDictionary<int, AnomalyResult>();

        // 1) ML.NET SR-CNN (needs enough points).
        if (series.Count >= 12)
        {
            try
            {
                foreach (var a in DetectSrCnn(series)) map[a.Index] = a;
            }
            catch { /* fall back to baseline only */ }
        }

        // 2) Robust rolling baseline — always runs; catches sustained-drop onset.
        foreach (var a in DetectRollingBaseline(series))
            map.TryAdd(a.Index, a);

        return map.Values.ToList();
    }

    private static IReadOnlyList<AnomalyResult> DetectSrCnn(IReadOnlyList<double> series)
    {
        var ml = new MLContext(seed: 0);
        var data = series.Select(v => new Point { Value = v }).ToList();
        var dv = ml.Data.LoadFromEnumerable(data);

        var res = ml.AnomalyDetection.DetectEntireAnomalyBySrCnn(
            dv,
            outputColumnName: nameof(SrCnnOutput.Prediction),
            inputColumnName: nameof(Point.Value),
            threshold: 0.30,
            batchSize: -1,
            sensitivity: 98.0,
            detectMode: SrCnnDetectMode.AnomalyAndMargin);

        var rows = ml.Data.CreateEnumerable<SrCnnOutput>(res, reuseRowObject: false).ToList();
        var list = new List<AnomalyResult>();

        // Vector layout (AnomalyAndMargin): [isAnomaly, score, magnitude, expectedValue, unit, upper, lower]
        for (var i = 0; i < rows.Count; i++)
        {
            var p = rows[i].Prediction;
            var value = series[i];
            var expected = p.Length > 3 ? p[3] : series[i];
            var lower = p.Length > 6 ? p[6] : expected;

            var flagged = (p.Length > 0 && p[0] == 1) || value < lower;
            if (flagged && value < expected)
            {
                var drop = expected > 0 ? (expected - value) / expected * 100.0 : 0;
                if (drop >= MinDropPercent)
                    list.Add(new AnomalyResult(i, Math.Round(value, 2), Math.Round(expected, 2), Math.Round(drop, 1),
                        Math.Round(p.Length > 1 ? p[1] : 0, 3)));
            }
        }
        return list;
    }

    // Compares each day to the median of the preceding window; flags a sharp fall.
    private static IReadOnlyList<AnomalyResult> DetectRollingBaseline(IReadOnlyList<double> series, int window = 7)
    {
        var list = new List<AnomalyResult>();
        for (var i = 1; i < series.Count; i++)
        {
            var start = Math.Max(0, i - window);
            var prior = series.Skip(start).Take(i - start).ToList();
            if (prior.Count < 3) continue;

            var baseline = Median(prior);
            if (baseline <= 0) continue;

            var drop = (baseline - series[i]) / baseline * 100.0;
            if (drop >= MinDropPercent)
                list.Add(new AnomalyResult(i, Math.Round(series[i], 2), Math.Round(baseline, 2), Math.Round(drop, 1),
                    Math.Round(drop / 100.0, 3)));
        }
        return list;
    }

    private static double Median(List<double> values)
    {
        var sorted = values.OrderBy(v => v).ToList();
        var n = sorted.Count;
        return n % 2 == 1 ? sorted[n / 2] : (sorted[n / 2 - 1] + sorted[n / 2]) / 2.0;
    }
}
