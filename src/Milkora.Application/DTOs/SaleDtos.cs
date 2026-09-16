using Milkora.Domain.Entities;

namespace Milkora.Application.DTOs;

public class SaleDto
{
    public Guid SaleId { get; set; }
    public DateTime SaleDate { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public decimal QuantityLitres { get; set; }
    public decimal RatePerLitre { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = "Pending";
    public string? PaymentMode { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateSaleRequest
{
    public Guid? SaleId { get; set; }
    public DateTime SaleDate { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public decimal QuantityLitres { get; set; }
    public decimal RatePerLitre { get; set; }
    public string PaymentStatus { get; set; } = "Pending";
    public string? PaymentMode { get; set; }
    public string? Notes { get; set; }
}

public class UpdatePaymentStatusRequest
{
    public string PaymentStatus { get; set; } = "Pending";
    public string? PaymentMode { get; set; }
}

public static class SaleMappings
{
    public static SaleDto ToDto(this Sale s) => new()
    {
        SaleId = s.SaleId, SaleDate = s.SaleDate, BuyerName = s.BuyerName, QuantityLitres = s.QuantityLitres,
        RatePerLitre = s.RatePerLitre, TotalAmount = s.TotalAmount, PaymentStatus = s.PaymentStatus,
        PaymentMode = s.PaymentMode, Notes = s.Notes, CreatedAt = s.CreatedAt, UpdatedAt = s.UpdatedAt,
    };

    public static Sale ToEntity(this CreateSaleRequest r) => new()
    {
        SaleId = r.SaleId ?? Guid.Empty, SaleDate = r.SaleDate, BuyerName = r.BuyerName,
        QuantityLitres = r.QuantityLitres, RatePerLitre = r.RatePerLitre,
        PaymentStatus = r.PaymentStatus, PaymentMode = r.PaymentMode, Notes = r.Notes,
    };
}
