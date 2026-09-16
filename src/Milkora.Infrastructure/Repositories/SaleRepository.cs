using Microsoft.Data.SqlClient;
using Milkora.Domain.Entities;
using Milkora.Domain.Interfaces;
using Milkora.Infrastructure.Persistence;

namespace Milkora.Infrastructure.Repositories;

public sealed class SaleRepository : RepositoryBase, ISaleRepository
{
    public SaleRepository(IDbConnectionFactory factory) : base(factory) { }

    private static Sale Map(SqlDataReader r) => new()
    {
        SaleId         = r.GetGuid("SaleId"),
        SaleDate       = r.GetDateTime("SaleDate"),
        BuyerName      = r.GetString("BuyerName"),
        QuantityLitres = r.GetDecimal("QuantityLitres"),
        RatePerLitre   = r.GetDecimal("RatePerLitre"),
        TotalAmount    = r.GetDecimal("TotalAmount"),
        PaymentStatus  = r.GetString("PaymentStatus"),
        PaymentMode    = r.GetNullableString("PaymentMode"),
        Notes          = r.GetNullableString("Notes"),
        CreatedAt      = r.GetDateTime("CreatedAt"),
        UpdatedAt      = r.GetDateTime("UpdatedAt"),
    };

    public Task<IReadOnlyList<Sale>> GetByDateRangeAsync(DateTime start, DateTime end, string? paymentStatus, string? buyerName, CancellationToken ct = default)
        => QueryListAsync("dbo.sp_GetSalesByDateRange", cmd =>
        {
            AddParam(cmd, "@StartDate", start.Date);
            AddParam(cmd, "@EndDate", end.Date);
            AddParam(cmd, "@PaymentStatus", paymentStatus);
            AddParam(cmd, "@BuyerName", buyerName);
        }, Map, ct);

    public Task<Guid> InsertAsync(Sale s, CancellationToken ct = default)
        => ExecuteInsertAsync("dbo.sp_InsertSale", cmd =>
        {
            AddParam(cmd, "@SaleId", s.SaleId == Guid.Empty ? null : s.SaleId);
            AddParam(cmd, "@SaleDate", s.SaleDate.Date);
            AddParam(cmd, "@BuyerName", s.BuyerName);
            AddParam(cmd, "@QuantityLitres", s.QuantityLitres);
            AddParam(cmd, "@RatePerLitre", s.RatePerLitre);
            AddParam(cmd, "@PaymentStatus", s.PaymentStatus);
            AddParam(cmd, "@PaymentMode", s.PaymentMode);
            AddParam(cmd, "@Notes", s.Notes);
        }, ct);

    public Task<int> UpdatePaymentStatusAsync(Guid saleId, string paymentStatus, string? paymentMode, CancellationToken ct = default)
        => ExecuteRowCountAsync("dbo.sp_UpdatePaymentStatus", cmd =>
        {
            AddParam(cmd, "@SaleId", saleId);
            AddParam(cmd, "@PaymentStatus", paymentStatus);
            AddParam(cmd, "@PaymentMode", paymentMode);
        }, ct);
}
