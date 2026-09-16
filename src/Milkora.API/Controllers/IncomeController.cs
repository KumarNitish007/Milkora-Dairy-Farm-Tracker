using Microsoft.AspNetCore.Mvc;
using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;

namespace Milkora.API.Controllers;

public sealed class IncomeController : ApiControllerBase
{
    private readonly IIncomeService _service;
    public IncomeController(IIncomeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] string? category, CancellationToken ct)
        => OkData(await _service.GetByDateRangeAsync(start, end, category, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIncomeRequest request, CancellationToken ct)
        => Created(new { IncomeId = await _service.CreateAsync(request, ct) }, "Income created.");
}
