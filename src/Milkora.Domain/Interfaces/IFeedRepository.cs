using Milkora.Domain.Entities;

namespace Milkora.Domain.Interfaces;

public interface IFeedRepository
{
    Task<IReadOnlyList<FeedItem>> GetAllAsync(string? search, CancellationToken ct = default);
    Task<IReadOnlyList<FeedItem>> GetLowStockAsync(CancellationToken ct = default);
    Task<Guid> InsertAsync(FeedItem item, CancellationToken ct = default);
    Task<int> UpdateStockAsync(Guid inventoryId, decimal quantityInStock, CancellationToken ct = default);
}
