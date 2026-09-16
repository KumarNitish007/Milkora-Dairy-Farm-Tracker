using Dotmim.Sync;

namespace Milkora.Sync;

/// <summary>
/// Single source of truth for what Dotmim.Sync synchronises between the central
/// MS SQL Server and each device's local SQLite. Shared by the server (API) and
/// the future SQLite client so both agree on the table list and columns.
///
/// Computed columns (TotalMilk/TotalValue, TotalAmount, ExpectedDeliveryDate) are
/// EXCLUDED from the setup: they are derived server-side and cannot be written,
/// so the client recomputes/relies on the server value rather than syncing them.
/// </summary>
public static class SyncTables
{
    // The eight business tables. Tracking/scope tables are created by Dotmim.Sync.
    public static readonly string[] All =
    {
        "Animals",
        "MilkProductionLog",
        "MilkSales",
        "Expenses",
        "Income",
        "HealthRecords",
        "BreedingRecords",
        "FeedInventory",
    };

    /// <summary>Builds the SyncSetup, restricting the three tables that carry
    /// computed columns to their writable columns only.</summary>
    public static SyncSetup CreateSetup()
    {
        var setup = new SyncSetup(All);

        // Empty column list = "all columns". For tables with computed columns we
        // list the writable columns explicitly to omit the computed ones.
        AddColumns(setup, "MilkProductionLog",
            "LogId", "AnimalId", "LogDate", "MorningMilk", "EveningMilk",
            "FatPercent", "RatePerLitre", "Notes", "CreatedAt", "UpdatedAt");

        AddColumns(setup, "MilkSales",
            "SaleId", "SaleDate", "BuyerName", "QuantityLitres", "RatePerLitre",
            "PaymentStatus", "PaymentMode", "Notes", "CreatedAt", "UpdatedAt");

        AddColumns(setup, "BreedingRecords",
            "BreedingId", "AnimalId", "InseminationDate", "BullDetails", "PregnancyStatus",
            "CalvingDate", "CalfDetails", "Notes", "CreatedAt", "UpdatedAt");

        return setup;
    }

    private static void AddColumns(SyncSetup setup, string table, params string[] columns)
    {
        var setupTable = setup.Tables[table];
        if (setupTable is null) return;
        foreach (var col in columns)
            setupTable.Columns.Add(col);
    }
}
