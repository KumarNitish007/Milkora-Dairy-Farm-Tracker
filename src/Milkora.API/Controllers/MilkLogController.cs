using Microsoft.AspNetCore.Mvc;
using Milkora.Application.Common;
using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;

namespace Milkora.API.Controllers;

[Route("api/milklog")]
public sealed class MilkLogController : ApiControllerBase
{
    private readonly IMilkLogService _service;
    public MilkLogController(IMilkLogService service) => _service = service;

    // GET api/milklog?date=..  OR  ?start=..&end=..&animalId=..
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] DateTime? date,
        [FromQuery] DateTime? start,
        [FromQuery] DateTime? end,
        [FromQuery] Guid? animalId,
        CancellationToken ct)
    {
        if (date.HasValue)
            return OkData(await _service.GetByDateAsync(date.Value, ct));
        if (start.HasValue && end.HasValue)
            return OkData(await _service.GetByDateRangeAsync(start.Value, end.Value, animalId, ct));
        return BadRequest(ApiResponse.Fail("Provide either 'date' or both 'start' and 'end'."));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMilkLogRequest request, CancellationToken ct)
        => Created(new { LogId = await _service.CreateAsync(request, ct) }, "Milk log created.");

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMilkLogRequest request, CancellationToken ct)
    {
        await _service.UpdateAsync(id, request, ct);
        return OkMessage("Milk log updated.");
    }
}
