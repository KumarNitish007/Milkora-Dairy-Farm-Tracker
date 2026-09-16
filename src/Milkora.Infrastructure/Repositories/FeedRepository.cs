using Microsoft.Data.SqlClient;
using Milkora.Domain.Entities;
using Milkora.Domain.Interfaces;
using Milkora.Infrastructure.Persistence;

namespace Milkora.Infrastructure.Repositories;

public sealed class FeedRepository : RepositoryBase, IFeedRepository
{
    public FeedRepository(IDbConnectionFactory factory) : base(factory) { }

    private static FeedItem MapFull(SqlDataReader r) => new()
    {
        InventoryId     = r.GetGuid("InventoryId"),
        ItemName        = r.GetString("ItemName"),
        QuantityInStock = r.GetDecimal("QuantityInStock"),
        Unit            = r.GetNullableString("Unit"),
        PurchaseDate    = r.GetNullableDateTime("PurchaseDate"),
        Cost            = r.GetNullableDecimal("Cost"),
        DailyUsage      = r.GetNullableDecimal("DailyUsage"),
        MinimumLevel    = r.GetDecimal("MinimumLevel"),
        IsLowStock      = r.GetBool("IsLowStock"),
        Notes           = r.GetNullableString("Notes"),
        CreatedAt       = r.GetDateTime("CreatedAt"),
        UpdatedAt       = r.GetDateTime("UpdatedAt"),
    };

    // sp_GetLowStock returns a reduced column set; everything it returns is low by definition.
    private static FeedItem MapLow(SqlDataReader r) => new()
    {
        InventoryId     = r.GetGuid("InventoryId"),
        ItemName        = r.GetString("ItemName"),
        QuantityInStock = r.GetDecimal("QuantityInStock"),
        Unit            = r.GetNullableString("Unit"),
        MinimumLevel    = r.GetDecimal("MinimumLevel"),
        DailyUsage      = r.GetNullableDecimal("DailyUsage"),
        Notes           = r.GetNullableString("Notes"),
        IsLowStock      = true,
    };

    public Task<IReadOnlyList<FeedItem>> GetAllAsync(string? search, CancellationToken ct = default)
        => QueryListAsync("dbo.sp_GetFeedInventory", cmd => AddParam(cmd, "@Search", search), MapFull, ct);

    public Task<IReadOnlyList<FeedItem>> GetLowStockAsync(CancellationToken ct = default)
        => QueryListAsync("dbo.sp_GetLowStock", null, MapLow, ct);

    public Task<Guid> InsertAsync(FeedItem f, CancellationToken ct = default)
        => ExecuteInsertAsync("dbo.sp_InsertFeedItem", cmd =>
        {
            AddParam(cmd, "@InventoryId", f.InventoryId == Guid.Empty ? null : f.InventoryId);
            AddParam(cmd, "@ItemName", f.ItemName);
            AddParam(cmd, "@QuantityInStock", f.QuantityInStock);
            AddParam(cmd, "@Unit", f.Unit);
            AddParam(cmd, "@PurchaseDate", f.PurchaseDate);
            AddParam(cmd, "@Cost", f.Cost);
            AddParam(cmd, "@DailyUsage", f.DailyUsage);
            AddParam(cmd, "@MinimumLevel", f.MinimumLevel);
            AddParam(cmd, "@Notes", f.Notes);
        }, ct);

    public Task<int> UpdateStockAsync(Guid inventoryId, decimal quantityInStock, CancellationToken ct = default)
        => ExecuteRowCountAsync("dbo.sp_UpdateStock", cmd =>
        {
            AddParam(cmd, "@InventoryId", inventoryId);
            AddParam(cmd, "@QuantityInStock", quantityInStock);
        }, ct);
}
