using Microsoft.AspNetCore.Mvc;
using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;

namespace Milkora.API.Controllers;

public sealed class ExpensesController : ApiControllerBase
{
    private readonly IExpenseService _service;
    public ExpensesController(IExpenseService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] string? category, CancellationToken ct)
        => OkData(await _service.GetByDateRangeAsync(start, end, category, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExpenseRequest request, CancellationToken ct)
        => Created(new { ExpenseId = await _service.CreateAsync(request, ct) }, "Expense created.");

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExpenseRequest request, CancellationToken ct)
    {
        await _service.UpdateAsync(id, request, ct);
        return OkMessage("Expense updated.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return OkMessage("Expense deleted.");
    }
}
