using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;
using Milkora.Domain.Interfaces;

namespace Milkora.Application.Services;

public sealed class IncomeService : IIncomeService
{
    private readonly IIncomeRepository _repo;
    public IncomeService(IIncomeRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<IncomeDto>> GetByDateRangeAsync(DateTime start, DateTime end, string? category, CancellationToken ct = default)
        => (await _repo.GetByDateRangeAsync(start, end, category, ct)).Select(i => i.ToDto()).ToList();

    public Task<Guid> CreateAsync(CreateIncomeRequest request, CancellationToken ct = default)
        => _repo.InsertAsync(request.ToEntity(), ct);
}
