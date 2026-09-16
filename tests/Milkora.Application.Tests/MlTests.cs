using FluentAssertions;
using Milkora.ML;
using Xunit;

namespace Milkora.Application.Tests;

public class MlAnomalyTests
{
    private readonly MilkAnomalyDetector _sut = new();

    [Fact]
    public void Detects_a_sustained_milk_drop()
    {
        // 20 days ~9L, then a 5-day drop to 4L, then recovery.
        var series = Enumerable.Repeat(9.0, 20)
            .Concat(Enumerable.Repeat(4.0, 5))
            .Concat(Enumerable.Repeat(9.0, 5))
            .ToList();

        var result = _sut.Detect(series);

        result.Should().NotBeEmpty();
        result.Should().Contain(a => a.DropPercent >= 40);
    }

    [Fact]
    public void No_anomaly_on_a_steady_series()
    {
        var series = Enumerable.Range(0, 25).Select(i => 9.0 + (i % 2 == 0 ? 0.2 : -0.2)).ToList();

        var result = _sut.Detect(series);

        result.Should().BeEmpty();
    }
}

public class MlForecastTests
{
    [Fact]
    public void Forecast_returns_the_requested_horizon_of_non_negative_points()
    {
        var series = Enumerable.Range(0, 30).Select(i => 10.0 + Math.Sin(i / 3.0)).ToList();

        var forecast = new MilkForecaster().Forecast(series, 7);

        forecast.Should().HaveCount(7);
        forecast.Should().OnlyContain(p => p.Value >= 0);
    }

    [Fact]
    public void Short_series_falls_back_to_an_average()
    {
        var forecast = new MilkForecaster().Forecast(new List<double> { 10, 11, 9 }, 5);

        forecast.Should().HaveCount(5);
        forecast.Should().OnlyContain(p => p.Value > 0);
    }
}
