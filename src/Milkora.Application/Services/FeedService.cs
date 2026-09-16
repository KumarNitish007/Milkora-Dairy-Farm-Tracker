using Milkora.Application.DTOs;
using Milkora.Application.Interfaces;
using Milkora.Domain.Exceptions;
using Milkora.Domain.Interfaces;

namespace Milkora.Application.Services;

public sealed class FeedService : IFeedService
{
    private readonly IFeedRepository _repo;
    public FeedService(IFeedRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<FeedItemDto>> GetAllAsync(string? search, CancellationToken ct = default)
        => (await _repo.GetAllAsync(search, ct)).Select(f => f.ToDto()).ToList();

    public async Task<IReadOnlyList<FeedItemDto>> GetLowStockAsync(CancellationToken ct = default)
        => (await _repo.GetLowStockAsync(ct)).Select(f => f.ToDto()).ToList();

    public Task<Guid> CreateAsync(CreateFeedItemRequest request, CancellationToken ct = default)
        => _repo.InsertAsync(request.ToEntity(), ct);

    public async Task UpdateStockAsync(Guid id, UpdateStockRequest request, CancellationToken ct = default)
    {
        var rows = await _repo.UpdateStockAsync(id, request.QuantityInStock, ct);
        if (rows == 0) throw new NotFoundException("FeedItem", id);
    }
}
