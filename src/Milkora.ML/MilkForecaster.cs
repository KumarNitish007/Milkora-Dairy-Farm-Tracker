using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace Milkora.ML;

/// <summary>Forecasts future milk output from a daily series using ML.NET's
/// Singular Spectrum Analysis (SSA). Falls back to a moving average when there
/// isn't enough history for SSA to be meaningful.</summary>
public sealed class MilkForecaster
{
    private sealed class Point { public float Value { get; set; } }

    private sealed class Prediction
    {
        public float[] Forecast { get; set; } = Array.Empty<float>();
        public float[] LowerBound { get; set; } = Array.Empty<float>();
        public float[] UpperBound { get; set; } = Array.Empty<float>();
    }

    public IReadOnlyList<ForecastPoint> Forecast(IReadOnlyList<double> series, int horizon)
    {
        horizon = Math.Max(1, horizon);

        // SSA needs a reasonable amount of history; otherwise use a simple average.
        if (series.Count < 8)
            return MovingAverageFallback(series, horizon);

        try
        {
            var ml = new MLContext(seed: 0);
            var data = series.Select(v => new Point { Value = (float)v }).ToList();
            var dv = ml.Data.LoadFromEnumerable(data);
            var window = Math.Clamp(series.Count / 3, 2, 30);

            var pipeline = ml.Forecasting.ForecastBySsa(
                outputColumnName: nameof(Prediction.Forecast),
                inputColumnName: nameof(Point.Value),
                windowSize: window,
                seriesLength: series.Count,
                trainSize: series.Count,
                horizon: horizon,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: nameof(Prediction.LowerBound),
                confidenceUpperBoundColumn: nameof(Prediction.UpperBound));

            var model = pipeline.Fit(dv);
            var engine = model.CreateTimeSeriesEngine<Point, Prediction>(ml);
            var p = engine.Predict();

            var list = new List<ForecastPoint>(horizon);
            for (var i = 0; i < horizon; i++)
                list.Add(new ForecastPoint(
                    i + 1,
                    Math.Max(0, Math.Round(p.Forecast[i], 2)),
                    Math.Max(0, Math.Round(p.LowerBound[i], 2)),
                    Math.Round(p.UpperBound[i], 2)));
            return list;
        }
        catch
        {
            return MovingAverageFallback(series, horizon);
        }
    }

    private static IReadOnlyList<ForecastPoint> MovingAverageFallback(IReadOnlyList<double> series, int horizon)
    {
        var avg = series.Count == 0 ? 0 : Math.Round(series.TakeLast(Math.Min(7, series.Count)).Average(), 2);
        var list = new List<ForecastPoint>(horizon);
        for (var i = 0; i < horizon; i++)
            list.Add(new ForecastPoint(i + 1, avg, Math.Max(0, Math.Round(avg * 0.85, 2)), Math.Round(avg * 1.15, 2)));
        return list;
    }
}
