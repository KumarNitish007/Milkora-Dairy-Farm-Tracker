using Microsoft.AspNetCore.Mvc;
using Milkora.Application.Interfaces;

namespace Milkora.API.Controllers;

/// <summary>AI/ML insights over the farm's own data (ML.NET — free, offline).</summary>
public sealed class MlController : ApiControllerBase
{
    private readonly IMlService _service;
    public MlController(IMlService service) => _service = service;

    // GET api/ml/forecast?days=7
    [HttpGet("forecast")]
    public async Task<IActionResult> Forecast([FromQuery] int days = 7, CancellationToken ct = default)
        => OkData(await _service.GetForecastAsync(days, ct));

    // GET api/ml/anomalies?lookbackDays=30
    [HttpGet("anomalies")]
    public async Task<IActionResult> Anomalies([FromQuery] int lookbackDays = 30, CancellationToken ct = default)
        => OkData(await _service.GetAnomaliesAsync(lookbackDays, ct));

    // GET api/ml/health-check?lookbackDays=45  — per-animal health inferred from milk
    [HttpGet("health-check")]
    public async Task<IActionResult> HealthCheck([FromQuery] int lookbackDays = 45, CancellationToken ct = default)
        => OkData(await _service.GetHealthChecksAsync(lookbackDays, ct));
}
