using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;
using Milkora.Domain.Exceptions;
using Milkora.Domain.Interfaces;

namespace Milkora.Application.Services;

public sealed class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _repo;
    public ExpenseService(IExpenseRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<ExpenseDto>> GetByDateRangeAsync(DateTime start, DateTime end, string? category, CancellationToken ct = default)
        => (await _repo.GetByDateRangeAsync(start, end, category, ct)).Select(e => e.ToDto()).ToList();

    public Task<Guid> CreateAsync(CreateExpenseRequest request, CancellationToken ct = default)
        => _repo.InsertAsync(request.ToEntity(), ct);

    public async Task UpdateAsync(Guid id, UpdateExpenseRequest request, CancellationToken ct = default)
    {
        var rows = await _repo.UpdateAsync(request.ToEntity(id), ct);
        if (rows == 0) throw new NotFoundException("Expense", id);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var rows = await _repo.DeleteAsync(id, ct);
        if (rows == 0) throw new NotFoundException("Expense", id);
    }
}
