using Microsoft.AspNetCore.Mvc;
using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;

namespace Milkora.API.Controllers;

public sealed class BreedingController : ApiControllerBase
{
    private readonly IBreedingService _service;
    public BreedingController(IBreedingService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? animalId, CancellationToken ct)
        => OkData(await _service.GetByAnimalAsync(animalId, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBreedingRequest request, CancellationToken ct)
        => Created(new { BreedingId = await _service.CreateAsync(request, ct) }, "Breeding record created.");

    [HttpPut("{id:guid}/calving")]
    public async Task<IActionResult> UpdateCalving(Guid id, [FromBody] UpdateCalvingRequest request, CancellationToken ct)
    {
        await _service.UpdateCalvingAsync(id, request, ct);
        return OkMessage("Calving recorded.");
    }
}
