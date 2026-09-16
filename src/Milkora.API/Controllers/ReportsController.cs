using Microsoft.AspNetCore.Mvc;
using Milkora.Application.Common;
using Milkora.Application.Interfaces;

namespace Milkora.API.Controllers;

public sealed class ReportsController : ApiControllerBase
{
    private readonly IReportService _service;
    public ReportsController(IReportService service) => _service = service;

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard([FromQuery] DateTime? date, CancellationToken ct)
        => OkData(await _service.GetDashboardAsync(date, ct));

    [HttpGet("monthly")]
    public async Task<IActionResult> Monthly([FromQuery] int year, [FromQuery] int month, CancellationToken ct)
    {
        if (month is < 1 or > 12)
            return BadRequest(ApiResponse.Fail("month must be between 1 and 12."));
        return OkData(await _service.GetMonthlyAsync(year, month, ct));
    }

    [HttpGet("profitloss")]
    public async Task<IActionResult> ProfitLoss([FromQuery] DateTime start, [FromQuery] DateTime end, CancellationToken ct)
        => OkData(await _service.GetProfitLossAsync(start, end, ct));

    [HttpGet("peranimalyield")]
    public async Task<IActionResult> PerAnimalYield([FromQuery] DateTime start, [FromQuery] DateTime end, CancellationToken ct)
        => OkData(await _service.GetPerAnimalYieldAsync(start, end, ct));
}
