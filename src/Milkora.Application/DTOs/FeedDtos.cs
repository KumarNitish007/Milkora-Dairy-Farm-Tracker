using Milkora.Domain.Entities;

namespace Milkora.Application.DTOs;

public class FeedItemDto
{
    public Guid InventoryId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal QuantityInStock { get; set; }
    public string? Unit { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? Cost { get; set; }
    public decimal? DailyUsage { get; set; }
    public decimal MinimumLevel { get; set; }
    public bool IsLowStock { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateFeedItemRequest
{
    public Guid? InventoryId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal QuantityInStock { get; set; }
    public string? Unit { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? Cost { get; set; }
    public decimal? DailyUsage { get; set; }
    public decimal MinimumLevel { get; set; }
    public string? Notes { get; set; }
}

public class UpdateStockRequest
{
    public decimal QuantityInStock { get; set; }
}

public static class FeedMappings
{
    public static FeedItemDto ToDto(this FeedItem f) => new()
    {
        InventoryId = f.InventoryId, ItemName = f.ItemName, QuantityInStock = f.QuantityInStock, Unit = f.Unit,
        PurchaseDate = f.PurchaseDate, Cost = f.Cost, DailyUsage = f.DailyUsage, MinimumLevel = f.MinimumLevel,
        IsLowStock = f.IsLowStock, Notes = f.Notes, CreatedAt = f.CreatedAt, UpdatedAt = f.UpdatedAt,
    };

    public static FeedItem ToEntity(this CreateFeedItemRequest r) => new()
    {
        InventoryId = r.InventoryId ?? Guid.Empty, ItemName = r.ItemName, QuantityInStock = r.QuantityInStock,
        Unit = r.Unit, PurchaseDate = r.PurchaseDate, Cost = r.Cost, DailyUsage = r.DailyUsage,
        MinimumLevel = r.MinimumLevel, Notes = r.Notes,
    };
}
