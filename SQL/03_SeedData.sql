/* =============================================================================
   Milkora Dairy Farm Tracker
   03_SeedData.sql  --  Sample data for development / first-run demo.

   Uses FIXED GUIDs so animals, milk logs, health and breeding rows link up and
   the script is safe to re-run (each insert is guarded by NOT EXISTS).
   Remove or skip this file for a clean production database.
   ============================================================================= */

USE [MilkoraDB];
GO

-- Required to modify tables that have PERSISTED computed columns.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* ---- Animals ---------------------------------------------------------------- */
DECLARE @Ganga   UNIQUEIDENTIFIER = 'A1111111-1111-1111-1111-111111111111';
DECLARE @Yamuna  UNIQUEIDENTIFIER = 'A2222222-2222-2222-2222-222222222222';
DECLARE @Lakshmi UNIQUEIDENTIFIER = 'A3333333-3333-3333-3333-333333333333';

IF NOT EXISTS (SELECT 1 FROM dbo.Animals WHERE AnimalId = @Ganga)
    INSERT INTO dbo.Animals (AnimalId, TagNumber, Name, Type, Breed, Gender, DateOfBirth, PurchaseDate, PurchasePrice, Status, DailyMilkYield, Notes)
    VALUES (@Ganga, N'COW-001', N'Ganga', N'Cow', N'Gir', N'Female', '2020-03-15', '2021-06-01', 65000, N'Milking', 12.50, N'High yielder');

IF NOT EXISTS (SELECT 1 FROM dbo.Animals WHERE AnimalId = @Yamuna)
    INSERT INTO dbo.Animals (AnimalId, TagNumber, Name, Type, Breed, Gender, DateOfBirth, PurchaseDate, PurchasePrice, Status, DailyMilkYield, Notes)
    VALUES (@Yamuna, N'COW-002', N'Yamuna', N'Cow', N'Sahiwal', N'Female', '2019-11-20', '2021-02-10', 58000, N'Pregnant', 9.00, N'Due for calving soon');

IF NOT EXISTS (SELECT 1 FROM dbo.Animals WHERE AnimalId = @Lakshmi)
    INSERT INTO dbo.Animals (AnimalId, TagNumber, Name, Type, Breed, Gender, DateOfBirth, PurchaseDate, PurchasePrice, Status, DailyMilkYield, Notes)
    VALUES (@Lakshmi, N'BUF-001', N'Lakshmi', N'Buffalo', N'Murrah', N'Female', '2018-08-05', '2020-09-15', 92000, N'Milking', 8.20, N'Buffalo - high fat milk');

/* ---- Milk production log (today) -------------------------------------------- */
DECLARE @Today DATE = CAST(SYSUTCDATETIME() AS DATE);

IF NOT EXISTS (SELECT 1 FROM dbo.MilkProductionLog WHERE AnimalId = @Ganga AND LogDate = @Today)
    INSERT INTO dbo.MilkProductionLog (AnimalId, LogDate, MorningMilk, EveningMilk, FatPercent, RatePerLitre, Notes)
    VALUES (@Ganga, @Today, 7.0, 6.0, 4.2, 55, N'Normal');

IF NOT EXISTS (SELECT 1 FROM dbo.MilkProductionLog WHERE AnimalId = @Lakshmi AND LogDate = @Today)
    INSERT INTO dbo.MilkProductionLog (AnimalId, LogDate, MorningMilk, EveningMilk, FatPercent, RatePerLitre, Notes)
    VALUES (@Lakshmi, @Today, 4.5, 3.8, 6.8, 62, N'Buffalo milk');

/* ---- Milk sales ------------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.MilkSales WHERE SaleDate = @Today AND BuyerName = N'Local Dairy Co-op')
    INSERT INTO dbo.MilkSales (SaleDate, BuyerName, QuantityLitres, RatePerLitre, PaymentStatus, PaymentMode, Notes)
    VALUES (@Today, N'Local Dairy Co-op', 20, 55, N'Pending', N'Bank', N'Bulk morning collection');

/* ---- Expenses --------------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Expenses WHERE ExpenseDate = @Today AND Category = N'Feed')
    INSERT INTO dbo.Expenses (ExpenseDate, Category, Description, Amount, PaymentMode)
    VALUES (@Today, N'Feed', N'Cattle feed 50kg bag', 1350, N'Cash');

/* ---- Income ----------------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Income WHERE IncomeDate = @Today AND Category = N'Manure Sale')
    INSERT INTO dbo.Income (IncomeDate, Category, Description, Amount)
    VALUES (@Today, N'Manure Sale', N'Organic manure - 5 sacks', 500);

/* ---- Health records --------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.HealthRecords WHERE AnimalId = @Ganga AND RecordType = N'Vaccine')
    INSERT INTO dbo.HealthRecords (AnimalId, RecordDate, RecordType, MedicineName, VetName, VetContact, Cost, NextDueDate, Notes)
    VALUES (@Ganga, DATEADD(DAY,-150,@Today), N'Vaccine', N'FMD Vaccine', N'Dr. Sharma', N'9876543210', 250, DATEADD(DAY, 5, @Today), N'Booster due');

/* ---- Breeding records ------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.BreedingRecords WHERE AnimalId = @Yamuna)
    INSERT INTO dbo.BreedingRecords (AnimalId, InseminationDate, BullDetails, PregnancyStatus, Notes)
    VALUES (@Yamuna, DATEADD(DAY,-240,@Today), N'AI - Gir semen batch #GS-204', N'Confirmed', N'Confirmed pregnant at 60 days');

/* ---- Feed inventory --------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.FeedInventory WHERE ItemName = N'Cattle Feed')
    INSERT INTO dbo.FeedInventory (ItemName, QuantityInStock, Unit, PurchaseDate, Cost, DailyUsage, MinimumLevel, Notes)
    VALUES (N'Cattle Feed', 120, N'kg', DATEADD(DAY,-3,@Today), 1350, 15, 50, N'Standard concentrate');

IF NOT EXISTS (SELECT 1 FROM dbo.FeedInventory WHERE ItemName = N'Green Fodder')
    INSERT INTO dbo.FeedInventory (ItemName, QuantityInStock, Unit, PurchaseDate, Cost, DailyUsage, MinimumLevel, Notes)
    VALUES (N'Green Fodder', 30, N'kg', DATEADD(DAY,-1,@Today), 400, 40, 60, N'LOW - reorder needed');

PRINT '03_SeedData.sql complete.';
GO
