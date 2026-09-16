namespace Milkora.Domain.Entities;

/// <summary>Feed stock item. Mirrors dbo.FeedInventory.</summary>
public class FeedItem
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
