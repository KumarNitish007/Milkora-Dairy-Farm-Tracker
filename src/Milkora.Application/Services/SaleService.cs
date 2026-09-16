using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;
using Milkora.Domain.Exceptions;
using Milkora.Domain.Interfaces;

namespace Milkora.Application.Services;

public sealed class SaleService : ISaleService
{
    private readonly ISaleRepository _repo;
    public SaleService(ISaleRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<SaleDto>> GetByDateRangeAsync(DateTime start, DateTime end, string? paymentStatus, string? buyer, CancellationToken ct = default)
        => (await _repo.GetByDateRangeAsync(start, end, paymentStatus, buyer, ct)).Select(s => s.ToDto()).ToList();

    public Task<Guid> CreateAsync(CreateSaleRequest request, CancellationToken ct = default)
        => _repo.InsertAsync(request.ToEntity(), ct);

    public async Task UpdatePaymentStatusAsync(Guid id, UpdatePaymentStatusRequest request, CancellationToken ct = default)
    {
        var rows = await _repo.UpdatePaymentStatusAsync(id, request.PaymentStatus, request.PaymentMode, ct);
        if (rows == 0) throw new NotFoundException("Sale", id);
    }
}
