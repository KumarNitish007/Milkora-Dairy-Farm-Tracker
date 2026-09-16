using Milkora.Domain.Entities;

namespace Milkora.Domain.Interfaces;

public interface ISaleRepository
{
    Task<IReadOnlyList<Sale>> GetByDateRangeAsync(DateTime start, DateTime end, string? paymentStatus, string? buyerName, CancellationToken ct = default);
    Task<Guid> InsertAsync(Sale sale, CancellationToken ct = default);
    Task<int> UpdatePaymentStatusAsync(Guid saleId, string paymentStatus, string? paymentMode, CancellationToken ct = default);
}
