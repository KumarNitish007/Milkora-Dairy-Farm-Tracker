namespace Milkora.Domain.Entities;

/// <summary>Milk sold to a buyer. Mirrors dbo.MilkSales.</summary>
public class Sale
{
    public Guid SaleId { get; set; }
    public DateTime SaleDate { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public decimal QuantityLitres { get; set; }
    public decimal RatePerLitre { get; set; }
    public decimal TotalAmount { get; set; }   // computed in DB
    public string PaymentStatus { get; set; } = "Pending";
    public string? PaymentMode { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
