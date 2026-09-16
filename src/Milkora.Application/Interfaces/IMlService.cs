using Milkora.Application.DTOs;

namespace Milkora.Application.Interfaces;

public interface IMlService
{
    /// <summary>Forecast farm-wide milk output for the next <paramref name="days"/> days.</summary>
    Task<MilkForecastDto> GetForecastAsync(int days, CancellationToken ct = default);

    /// <summary>Detect abnormal milk drops per animal over the recent window.</summary>
    Task<AnomalyReportDto> GetAnomaliesAsync(int lookbackDays, CancellationToken ct = default);

    /// <summary>Screening health assessment per animal, inferred from its milk series.</summary>
    Task<IReadOnlyList<MilkHealthDto>> GetHealthChecksAsync(int lookbackDays, CancellationToken ct = default);
}
