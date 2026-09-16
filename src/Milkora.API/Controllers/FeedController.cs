using Microsoft.AspNetCore.Mvc;
using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;

namespace Milkora.API.Controllers;

public sealed class FeedController : ApiControllerBase
{
    private readonly IFeedService _service;
    public FeedController(IFeedService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? search, CancellationToken ct)
        => OkData(await _service.GetAllAsync(search, ct));

    [HttpGet("lowstock")]
    public async Task<IActionResult> GetLowStock(CancellationToken ct)
        => OkData(await _service.GetLowStockAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFeedItemRequest request, CancellationToken ct)
        => Created(new { InventoryId = await _service.CreateAsync(request, ct) }, "Feed item created.");

    [HttpPut("{id:guid}/stock")]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockRequest request, CancellationToken ct)
    {
        await _service.UpdateStockAsync(id, request, ct);
        return OkMessage("Stock updated.");
    }
}
