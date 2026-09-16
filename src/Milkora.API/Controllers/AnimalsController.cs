using Microsoft.AspNetCore.Mvc;
using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;

namespace Milkora.API.Controllers;

public sealed class AnimalsController : ApiControllerBase
{
    private readonly IAnimalService _service;
    public AnimalsController(IAnimalService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] string? search, CancellationToken ct)
        => OkData(await _service.GetAllAsync(status, search, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => OkData(await _service.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAnimalRequest request, CancellationToken ct)
        => Created(new { AnimalId = await _service.CreateAsync(request, ct) }, "Animal created.");

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAnimalRequest request, CancellationToken ct)
    {
        await _service.UpdateAsync(id, request, ct);
        return OkMessage("Animal updated.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return OkMessage("Animal deleted.");
    }
}
