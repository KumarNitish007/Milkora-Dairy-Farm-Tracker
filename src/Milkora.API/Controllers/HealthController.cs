using Microsoft.AspNetCore.Mvc;
using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;

namespace Milkora.API.Controllers;

public sealed class HealthController : ApiControllerBase
{
    private readonly IHealthService _service;
    public HealthController(IHealthService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? animalId, [FromQuery] string? type, CancellationToken ct)
        => OkData(await _service.GetByAnimalAsync(animalId, type, ct));

    [HttpGet("reminders")]
    public async Task<IActionResult> GetReminders([FromQuery] DateTime? asOf, [FromQuery] int daysAhead = 7, CancellationToken ct = default)
        => OkData(await _service.GetDueRemindersAsync(asOf, daysAhead, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHealthRecordRequest request, CancellationToken ct)
        => Created(new { HealthId = await _service.CreateAsync(request, ct) }, "Health record created.");
}
