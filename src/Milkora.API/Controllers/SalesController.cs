using Microsoft.AspNetCore.Mvc;
using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;

namespace Milkora.API.Controllers;

public sealed class SalesController : ApiControllerBase
{
    private readonly ISaleService _service;
    public SalesController(ISaleService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] DateTime start, [FromQuery] DateTime end,
        [FromQuery] string? paymentStatus, [FromQuery] string? buyer, CancellationToken ct)
        => OkData(await _service.GetByDateRangeAsync(start, end, paymentStatus, buyer, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleRequest request, CancellationToken ct)
        => Created(new { SaleId = await _service.CreateAsync(request, ct) }, "Sale created.");

    [HttpPut("{id:guid}/payment")]
    public async Task<IActionResult> UpdatePayment(Guid id, [FromBody] UpdatePaymentStatusRequest request, CancellationToken ct)
    {
        await _service.UpdatePaymentStatusAsync(id, request, ct);
        return OkMessage("Payment status updated.");
    }
}
