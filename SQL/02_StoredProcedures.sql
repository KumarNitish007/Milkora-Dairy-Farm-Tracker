/* =============================================================================
   Milkora Dairy Farm Tracker
   02_StoredProcedures.sql  --  EVERY database operation used by the API.

   Rules (per project spec):
     * No inline SQL in the API - everything goes through these procedures.
     * All parameters are typed; the API binds them with SqlParameter only
       (never string concatenation) => SQL-injection safe.
     * Insert procedures accept an optional @Id (GUID). If NULL the server
       generates one with NEWID(); either way the id is returned via SELECT
       so the caller/ADO.NET can read it back. This lets offline clients pass
       their own GUID and keeps server inserts collision-free.
     * CREATE OR ALTER => safe to re-run.
   ============================================================================= */

USE [MilkoraDB];
GO

-- Procedures are compiled with these SET options captured; required because
-- several reference tables that have PERSISTED computed columns.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* =========================================================================
   ANIMALS
   ========================================================================= */
CREATE OR ALTER PROCEDURE dbo.sp_GetAllAnimals
    @Status NVARCHAR(20) = NULL,     -- optional filter
    @Search NVARCHAR(100) = NULL     -- matches TagNumber / Name / Breed
AS
BEGIN
    SET NOCOUNT ON;
    SELECT AnimalId, TagNumber, Name, Type, Breed, Gender, DateOfBirth,
           PurchaseDate, PurchasePrice, Status, DailyMilkYield, PhotoPath,
           Notes, CreatedAt, UpdatedAt
    FROM dbo.Animals
    WHERE (@Status IS NULL OR Status = @Status)
      AND (@Search IS NULL
           OR TagNumber LIKE '%' + @Search + '%'
           OR Name      LIKE '%' + @Search + '%'
           OR Breed     LIKE '%' + @Search + '%')
    ORDER BY TagNumber;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetAnimalById
    @AnimalId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT AnimalId, TagNumber, Name, Type, Breed, Gender, DateOfBirth,
           PurchaseDate, PurchasePrice, Status, DailyMilkYield, PhotoPath,
           Notes, CreatedAt, UpdatedAt
    FROM dbo.Animals
    WHERE AnimalId = @AnimalId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_InsertAnimal
    @AnimalId       UNIQUEIDENTIFIER = NULL OUTPUT,
    @TagNumber      NVARCHAR(50),
    @Name           NVARCHAR(100)  = NULL,
    @Type           NVARCHAR(50)   = NULL,
    @Breed          NVARCHAR(100)  = NULL,
    @Gender         NVARCHAR(10)   = NULL,
    @DateOfBirth    DATE           = NULL,
    @PurchaseDate   DATE           = NULL,
    @PurchasePrice  DECIMAL(18,2)  = NULL,
    @Status         NVARCHAR(20)   = N'Milking',
    @DailyMilkYield DECIMAL(10,2)  = NULL,
    @PhotoPath      NVARCHAR(500)  = NULL,
    @Notes          NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @AnimalId IS NULL SET @AnimalId = NEWID();
    INSERT INTO dbo.Animals
        (AnimalId, TagNumber, Name, Type, Breed, Gender, DateOfBirth,
         PurchaseDate, PurchasePrice, Status, DailyMilkYield, PhotoPath, Notes)
    VALUES
        (@AnimalId, @TagNumber, @Name, @Type, @Breed, @Gender, @DateOfBirth,
         @PurchaseDate, @PurchasePrice, @Status, @DailyMilkYield, @PhotoPath, @Notes);
    SELECT @AnimalId AS AnimalId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_UpdateAnimal
    @AnimalId       UNIQUEIDENTIFIER,
    @TagNumber      NVARCHAR(50),
    @Name           NVARCHAR(100)  = NULL,
    @Type           NVARCHAR(50)   = NULL,
    @Breed          NVARCHAR(100)  = NULL,
    @Gender         NVARCHAR(10)   = NULL,
    @DateOfBirth    DATE           = NULL,
    @PurchaseDate   DATE           = NULL,
    @PurchasePrice  DECIMAL(18,2)  = NULL,
    @Status         NVARCHAR(20)   = N'Milking',
    @DailyMilkYield DECIMAL(10,2)  = NULL,
    @PhotoPath      NVARCHAR(500)  = NULL,
    @Notes          NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Animals
       SET TagNumber = @TagNumber, Name = @Name, Type = @Type, Breed = @Breed,
           Gender = @Gender, DateOfBirth = @DateOfBirth, PurchaseDate = @PurchaseDate,
           PurchasePrice = @PurchasePrice, Status = @Status,
           DailyMilkYield = @DailyMilkYield, PhotoPath = @PhotoPath,
           Notes = @Notes, UpdatedAt = SYSUTCDATETIME()
     WHERE AnimalId = @AnimalId;
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_DeleteAnimal
    @AnimalId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Animals WHERE AnimalId = @AnimalId;
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

/* =========================================================================
   MILK PRODUCTION LOG
   ========================================================================= */
CREATE OR ALTER PROCEDURE dbo.sp_GetMilkLogsByDate
    @LogDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT m.LogId, m.AnimalId, a.TagNumber, a.Name AS AnimalName, m.LogDate,
           m.MorningMilk, m.EveningMilk, m.FatPercent, m.RatePerLitre,
           m.TotalMilk, m.TotalValue, m.Notes, m.CreatedAt, m.UpdatedAt
    FROM dbo.MilkProductionLog m
    LEFT JOIN dbo.Animals a ON a.AnimalId = m.AnimalId
    WHERE m.LogDate = @LogDate
    ORDER BY a.TagNumber, m.CreatedAt;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetMilkLogsByDateRange
    @StartDate DATE,
    @EndDate   DATE,
    @AnimalId  UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT m.LogId, m.AnimalId, a.TagNumber, a.Name AS AnimalName, m.LogDate,
           m.MorningMilk, m.EveningMilk, m.FatPercent, m.RatePerLitre,
           m.TotalMilk, m.TotalValue, m.Notes, m.CreatedAt, m.UpdatedAt
    FROM dbo.MilkProductionLog m
    LEFT JOIN dbo.Animals a ON a.AnimalId = m.AnimalId
    WHERE m.LogDate BETWEEN @StartDate AND @EndDate
      AND (@AnimalId IS NULL OR m.AnimalId = @AnimalId)
    ORDER BY m.LogDate DESC, a.TagNumber;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_InsertMilkLog
    @LogId        UNIQUEIDENTIFIER = NULL OUTPUT,
    @AnimalId     UNIQUEIDENTIFIER = NULL,
    @LogDate      DATE,
    @MorningMilk  DECIMAL(10,2) = 0,
    @EveningMilk  DECIMAL(10,2) = 0,
    @FatPercent   DECIMAL(5,2)  = NULL,
    @RatePerLitre DECIMAL(10,2) = NULL,
    @Notes        NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @LogId IS NULL SET @LogId = NEWID();
    INSERT INTO dbo.MilkProductionLog
        (LogId, AnimalId, LogDate, MorningMilk, EveningMilk, FatPercent, RatePerLitre, Notes)
    VALUES
        (@LogId, @AnimalId, @LogDate, @MorningMilk, @EveningMilk, @FatPercent, @RatePerLitre, @Notes);
    SELECT @LogId AS LogId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_UpdateMilkLog
    @LogId        UNIQUEIDENTIFIER,
    @AnimalId     UNIQUEIDENTIFIER = NULL,
    @LogDate      DATE,
    @MorningMilk  DECIMAL(10,2) = 0,
    @EveningMilk  DECIMAL(10,2) = 0,
    @FatPercent   DECIMAL(5,2)  = NULL,
    @RatePerLitre DECIMAL(10,2) = NULL,
    @Notes        NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.MilkProductionLog
       SET AnimalId = @AnimalId, LogDate = @LogDate, MorningMilk = @MorningMilk,
           EveningMilk = @EveningMilk, FatPercent = @FatPercent,
           RatePerLitre = @RatePerLitre, Notes = @Notes, UpdatedAt = SYSUTCDATETIME()
     WHERE LogId = @LogId;
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

/* =========================================================================
   MILK SALES
   ========================================================================= */
CREATE OR ALTER PROCEDURE dbo.sp_GetSalesByDateRange
    @StartDate     DATE,
    @EndDate       DATE,
    @PaymentStatus NVARCHAR(20) = NULL,
    @BuyerName     NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT SaleId, SaleDate, BuyerName, QuantityLitres, RatePerLitre, TotalAmount,
           PaymentStatus, PaymentMode, Notes, CreatedAt, UpdatedAt
    FROM dbo.MilkSales
    WHERE SaleDate BETWEEN @StartDate AND @EndDate
      AND (@PaymentStatus IS NULL OR PaymentStatus = @PaymentStatus)
      AND (@BuyerName IS NULL OR BuyerName LIKE '%' + @BuyerName + '%')
    ORDER BY SaleDate DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_InsertSale
    @SaleId         UNIQUEIDENTIFIER = NULL OUTPUT,
    @SaleDate       DATE,
    @BuyerName      NVARCHAR(150),
    @QuantityLitres DECIMAL(10,2) = 0,
    @RatePerLitre   DECIMAL(10,2) = 0,
    @PaymentStatus  NVARCHAR(20)  = N'Pending',
    @PaymentMode    NVARCHAR(20)  = NULL,
    @Notes          NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @SaleId IS NULL SET @SaleId = NEWID();
    INSERT INTO dbo.MilkSales
        (SaleId, SaleDate, BuyerName, QuantityLitres, RatePerLitre, PaymentStatus, PaymentMode, Notes)
    VALUES
        (@SaleId, @SaleDate, @BuyerName, @QuantityLitres, @RatePerLitre, @PaymentStatus, @PaymentMode, @Notes);
    SELECT @SaleId AS SaleId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_UpdatePaymentStatus
    @SaleId        UNIQUEIDENTIFIER,
    @PaymentStatus NVARCHAR(20),
    @PaymentMode   NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.MilkSales
       SET PaymentStatus = @PaymentStatus,
           PaymentMode = ISNULL(@PaymentMode, PaymentMode),
           UpdatedAt = SYSUTCDATETIME()
     WHERE SaleId = @SaleId;
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

/* =========================================================================
   EXPENSES
   ========================================================================= */
CREATE OR ALTER PROCEDURE dbo.sp_GetExpensesByDateRange
    @StartDate DATE,
    @EndDate   DATE,
    @Category  NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ExpenseId, ExpenseDate, Category, Description, Amount, PaymentMode,
           Notes, CreatedAt, UpdatedAt
    FROM dbo.Expenses
    WHERE ExpenseDate BETWEEN @StartDate AND @EndDate
      AND (@Category IS NULL OR Category = @Category)
    ORDER BY ExpenseDate DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_InsertExpense
    @ExpenseId   UNIQUEIDENTIFIER = NULL OUTPUT,
    @ExpenseDate DATE,
    @Category    NVARCHAR(50),
    @Description NVARCHAR(500) = NULL,
    @Amount      DECIMAL(18,2) = 0,
    @PaymentMode NVARCHAR(20)  = NULL,
    @Notes       NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @ExpenseId IS NULL SET @ExpenseId = NEWID();
    INSERT INTO dbo.Expenses
        (ExpenseId, ExpenseDate, Category, Description, Amount, PaymentMode, Notes)
    VALUES
        (@ExpenseId, @ExpenseDate, @Category, @Description, @Amount, @PaymentMode, @Notes);
    SELECT @ExpenseId AS ExpenseId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_UpdateExpense
    @ExpenseId   UNIQUEIDENTIFIER,
    @ExpenseDate DATE,
    @Category    NVARCHAR(50),
    @Description NVARCHAR(500) = NULL,
    @Amount      DECIMAL(18,2) = 0,
    @PaymentMode NVARCHAR(20)  = NULL,
    @Notes       NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Expenses
       SET ExpenseDate = @ExpenseDate, Category = @Category, Description = @Description,
           Amount = @Amount, PaymentMode = @PaymentMode, Notes = @Notes,
           UpdatedAt = SYSUTCDATETIME()
     WHERE ExpenseId = @ExpenseId;
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_DeleteExpense
    @ExpenseId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Expenses WHERE ExpenseId = @ExpenseId;
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

/* =========================================================================
   INCOME
   ========================================================================= */
CREATE OR ALTER PROCEDURE dbo.sp_GetIncomeByDateRange
    @StartDate DATE,
    @EndDate   DATE,
    @Category  NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IncomeId, IncomeDate, Category, Description, Amount, Notes, CreatedAt, UpdatedAt
    FROM dbo.Income
    WHERE IncomeDate BETWEEN @StartDate AND @EndDate
      AND (@Category IS NULL OR Category = @Category)
    ORDER BY IncomeDate DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_InsertIncome
    @IncomeId    UNIQUEIDENTIFIER = NULL OUTPUT,
    @IncomeDate  DATE,
    @Category    NVARCHAR(50),
    @Description NVARCHAR(500) = NULL,
    @Amount      DECIMAL(18,2) = 0,
    @Notes       NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @IncomeId IS NULL SET @IncomeId = NEWID();
    INSERT INTO dbo.Income
        (IncomeId, IncomeDate, Category, Description, Amount, Notes)
    VALUES
        (@IncomeId, @IncomeDate, @Category, @Description, @Amount, @Notes);
    SELECT @IncomeId AS IncomeId;
END
GO

/* =========================================================================
   HEALTH RECORDS
   ========================================================================= */
CREATE OR ALTER PROCEDURE dbo.sp_GetHealthRecordsByAnimal
    @AnimalId   UNIQUEIDENTIFIER = NULL,
    @RecordType NVARCHAR(30)     = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT h.HealthId, h.AnimalId, a.TagNumber, a.Name AS AnimalName, h.RecordDate,
           h.RecordType, h.MedicineName, h.VetName, h.VetContact, h.Cost,
           h.NextDueDate, h.Notes, h.CreatedAt, h.UpdatedAt
    FROM dbo.HealthRecords h
    LEFT JOIN dbo.Animals a ON a.AnimalId = h.AnimalId
    WHERE (@AnimalId IS NULL OR h.AnimalId = @AnimalId)
      AND (@RecordType IS NULL OR h.RecordType = @RecordType)
    ORDER BY h.RecordDate DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetDueReminders
    @AsOfDate DATE = NULL,          -- defaults to today
    @DaysAhead INT = 7              -- window of upcoming due dates
AS
BEGIN
    SET NOCOUNT ON;
    IF @AsOfDate IS NULL SET @AsOfDate = CAST(SYSUTCDATETIME() AS DATE);
    SELECT h.HealthId, h.AnimalId, a.TagNumber, a.Name AS AnimalName, h.RecordType,
           h.MedicineName, h.NextDueDate,
           DATEDIFF(DAY, @AsOfDate, h.NextDueDate) AS DaysUntilDue
    FROM dbo.HealthRecords h
    LEFT JOIN dbo.Animals a ON a.AnimalId = h.AnimalId
    WHERE h.NextDueDate IS NOT NULL
      AND h.NextDueDate <= DATEADD(DAY, @DaysAhead, @AsOfDate)
    ORDER BY h.NextDueDate;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_InsertHealthRecord
    @HealthId     UNIQUEIDENTIFIER = NULL OUTPUT,
    @AnimalId     UNIQUEIDENTIFIER = NULL,
    @RecordDate   DATE,
    @RecordType   NVARCHAR(30),
    @MedicineName NVARCHAR(200) = NULL,
    @VetName      NVARCHAR(150) = NULL,
    @VetContact   NVARCHAR(50)  = NULL,
    @Cost         DECIMAL(18,2) = NULL,
    @NextDueDate  DATE          = NULL,
    @Notes        NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @HealthId IS NULL SET @HealthId = NEWID();
    INSERT INTO dbo.HealthRecords
        (HealthId, AnimalId, RecordDate, RecordType, MedicineName, VetName,
         VetContact, Cost, NextDueDate, Notes)
    VALUES
        (@HealthId, @AnimalId, @RecordDate, @RecordType, @MedicineName, @VetName,
         @VetContact, @Cost, @NextDueDate, @Notes);
    SELECT @HealthId AS HealthId;
END
GO

/* =========================================================================
   BREEDING RECORDS
   ========================================================================= */
CREATE OR ALTER PROCEDURE dbo.sp_GetBreedingRecordsByAnimal
    @AnimalId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT b.BreedingId, b.AnimalId, a.TagNumber, a.Name AS AnimalName,
           b.InseminationDate, b.BullDetails, b.PregnancyStatus,
           b.ExpectedDeliveryDate, b.CalvingDate, b.CalfDetails, b.Notes,
           b.CreatedAt, b.UpdatedAt
    FROM dbo.BreedingRecords b
    LEFT JOIN dbo.Animals a ON a.AnimalId = b.AnimalId
    WHERE (@AnimalId IS NULL OR b.AnimalId = @AnimalId)
    ORDER BY b.InseminationDate DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_InsertBreedingRecord
    @BreedingId       UNIQUEIDENTIFIER = NULL OUTPUT,
    @AnimalId         UNIQUEIDENTIFIER = NULL,
    @InseminationDate DATE,
    @BullDetails      NVARCHAR(300) = NULL,
    @PregnancyStatus  NVARCHAR(30)  = N'Inseminated',
    @Notes            NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @BreedingId IS NULL SET @BreedingId = NEWID();
    INSERT INTO dbo.BreedingRecords
        (BreedingId, AnimalId, InseminationDate, BullDetails, PregnancyStatus, Notes)
    VALUES
        (@BreedingId, @AnimalId, @InseminationDate, @BullDetails, @PregnancyStatus, @Notes);
    SELECT @BreedingId AS BreedingId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_UpdateCalving
    @BreedingId      UNIQUEIDENTIFIER,
    @CalvingDate     DATE,
    @CalfDetails     NVARCHAR(300) = NULL,
    @PregnancyStatus NVARCHAR(30)  = N'Delivered'
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.BreedingRecords
       SET CalvingDate = @CalvingDate, CalfDetails = @CalfDetails,
           PregnancyStatus = @PregnancyStatus, UpdatedAt = SYSUTCDATETIME()
     WHERE BreedingId = @BreedingId;
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

/* =========================================================================
   FEED INVENTORY
   ========================================================================= */
CREATE OR ALTER PROCEDURE dbo.sp_GetFeedInventory
    @Search NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT InventoryId, ItemName, QuantityInStock, Unit, PurchaseDate, Cost,
           DailyUsage, MinimumLevel,
           CAST(CASE WHEN QuantityInStock <= MinimumLevel THEN 1 ELSE 0 END AS BIT) AS IsLowStock,
           Notes, CreatedAt, UpdatedAt
    FROM dbo.FeedInventory
    WHERE (@Search IS NULL OR ItemName LIKE '%' + @Search + '%')
    ORDER BY ItemName;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_InsertFeedItem
    @InventoryId     UNIQUEIDENTIFIER = NULL OUTPUT,
    @ItemName        NVARCHAR(150),
    @QuantityInStock DECIMAL(12,2) = 0,
    @Unit            NVARCHAR(20)  = NULL,
    @PurchaseDate    DATE          = NULL,
    @Cost            DECIMAL(18,2) = NULL,
    @DailyUsage      DECIMAL(12,2) = NULL,
    @MinimumLevel    DECIMAL(12,2) = 0,
    @Notes           NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @InventoryId IS NULL SET @InventoryId = NEWID();
    INSERT INTO dbo.FeedInventory
        (InventoryId, ItemName, QuantityInStock, Unit, PurchaseDate, Cost,
         DailyUsage, MinimumLevel, Notes)
    VALUES
        (@InventoryId, @ItemName, @QuantityInStock, @Unit, @PurchaseDate, @Cost,
         @DailyUsage, @MinimumLevel, @Notes);
    SELECT @InventoryId AS InventoryId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_UpdateStock
    @InventoryId     UNIQUEIDENTIFIER,
    @QuantityInStock DECIMAL(12,2)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.FeedInventory
       SET QuantityInStock = @QuantityInStock, UpdatedAt = SYSUTCDATETIME()
     WHERE InventoryId = @InventoryId;
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetLowStock
AS
BEGIN
    SET NOCOUNT ON;
    SELECT InventoryId, ItemName, QuantityInStock, Unit, MinimumLevel, DailyUsage, Notes
    FROM dbo.FeedInventory
    WHERE QuantityInStock <= MinimumLevel
    ORDER BY ItemName;
END
GO

/* =========================================================================
   REPORTS
   ========================================================================= */

-- Dashboard summary for a single day (defaults to today).
-- Income for the day = Income table + milk sales booked that day.
CREATE OR ALTER PROCEDURE dbo.sp_GetDashboardSummary
    @Date DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Date IS NULL SET @Date = CAST(SYSUTCDATETIME() AS DATE);

    DECLARE @TotalMilk    DECIMAL(18,2) = (SELECT ISNULL(SUM(TotalMilk),0) FROM dbo.MilkProductionLog WHERE LogDate = @Date);
    DECLARE @OtherIncome  DECIMAL(18,2) = (SELECT ISNULL(SUM(Amount),0)    FROM dbo.Income            WHERE IncomeDate = @Date);
    DECLARE @SalesIncome  DECIMAL(18,2) = (SELECT ISNULL(SUM(TotalAmount),0) FROM dbo.MilkSales       WHERE SaleDate = @Date);
    DECLARE @TotalIncome  DECIMAL(18,2) = @OtherIncome + @SalesIncome;
    DECLARE @TotalExpense DECIMAL(18,2) = (SELECT ISNULL(SUM(Amount),0)    FROM dbo.Expenses          WHERE ExpenseDate = @Date);

    SELECT
        @Date                                        AS [Date],
        @TotalMilk                                   AS TotalMilkLitres,
        @TotalIncome                                 AS TotalIncome,
        @TotalExpense                                AS TotalExpense,
        (@TotalIncome - @TotalExpense)               AS ProfitOrLoss,
        (SELECT COUNT(*) FROM dbo.Animals WHERE Status NOT IN (N'Sold', N'Dead')) AS ActiveAnimals,
        (SELECT COUNT(*) FROM dbo.Animals WHERE Status = N'Sick')                 AS SickAnimals,
        (SELECT COUNT(*) FROM dbo.HealthRecords WHERE NextDueDate IS NOT NULL
                 AND NextDueDate <= DATEADD(DAY, 7, @Date))                       AS DueReminders,
        (SELECT COUNT(*) FROM dbo.FeedInventory WHERE QuantityInStock <= MinimumLevel) AS LowStockItems,
        (SELECT COUNT(*) FROM dbo.MilkSales WHERE PaymentStatus = N'Pending')     AS PendingPayments;
END
GO

-- Monthly rollup (totals for the given year/month).
CREATE OR ALTER PROCEDURE dbo.sp_GetMonthlyReport
    @Year  INT,
    @Month INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Start DATE = DATEFROMPARTS(@Year, @Month, 1);
    DECLARE @End   DATE = EOMONTH(@Start);

    DECLARE @Income  DECIMAL(18,2) =
        (SELECT ISNULL(SUM(Amount),0)      FROM dbo.Income     WHERE IncomeDate BETWEEN @Start AND @End)
      + (SELECT ISNULL(SUM(TotalAmount),0) FROM dbo.MilkSales  WHERE SaleDate   BETWEEN @Start AND @End);
    DECLARE @Expense DECIMAL(18,2) =
        (SELECT ISNULL(SUM(Amount),0)      FROM dbo.Expenses   WHERE ExpenseDate BETWEEN @Start AND @End);

    SELECT
        @Year  AS [Year],
        @Month AS [Month],
        @Start AS StartDate,
        @End   AS EndDate,
        (SELECT ISNULL(SUM(TotalMilk),0)  FROM dbo.MilkProductionLog WHERE LogDate BETWEEN @Start AND @End) AS TotalMilkLitres,
        (SELECT ISNULL(SUM(TotalValue),0) FROM dbo.MilkProductionLog WHERE LogDate BETWEEN @Start AND @End) AS MilkValue,
        @Income                        AS TotalIncome,
        @Expense                       AS TotalExpense,
        (@Income - @Expense)           AS ProfitOrLoss,
        (SELECT COUNT(*) FROM dbo.MilkSales WHERE SaleDate BETWEEN @Start AND @End) AS SalesCount;
END
GO

-- Profit & loss for an arbitrary date range.
-- Result set 1: totals row.  Result set 2: income by category.  Result set 3: expense by category.
CREATE OR ALTER PROCEDURE dbo.sp_GetProfitLossReport
    @StartDate DATE,
    @EndDate   DATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Income  DECIMAL(18,2) =
        (SELECT ISNULL(SUM(Amount),0)      FROM dbo.Income     WHERE IncomeDate BETWEEN @StartDate AND @EndDate)
      + (SELECT ISNULL(SUM(TotalAmount),0) FROM dbo.MilkSales  WHERE SaleDate   BETWEEN @StartDate AND @EndDate);
    DECLARE @Expense DECIMAL(18,2) =
        (SELECT ISNULL(SUM(Amount),0)      FROM dbo.Expenses   WHERE ExpenseDate BETWEEN @StartDate AND @EndDate);

    -- 1) Totals
    SELECT @StartDate AS StartDate, @EndDate AS EndDate,
           @Income AS TotalIncome, @Expense AS TotalExpense,
           (@Income - @Expense) AS ProfitOrLoss;

    -- 2) Income by category (milk sales folded in as a synthetic category)
    SELECT Category, SUM(Amount) AS Amount FROM
    (
        SELECT Category, Amount FROM dbo.Income WHERE IncomeDate BETWEEN @StartDate AND @EndDate
        UNION ALL
        SELECT N'Milk Sale' AS Category, TotalAmount FROM dbo.MilkSales WHERE SaleDate BETWEEN @StartDate AND @EndDate
    ) x
    GROUP BY Category
    ORDER BY Amount DESC;

    -- 3) Expense by category
    SELECT Category, SUM(Amount) AS Amount
    FROM dbo.Expenses
    WHERE ExpenseDate BETWEEN @StartDate AND @EndDate
    GROUP BY Category
    ORDER BY Amount DESC;
END
GO

-- Per-animal milk yield across a date range.
CREATE OR ALTER PROCEDURE dbo.sp_GetPerAnimalYield
    @StartDate DATE,
    @EndDate   DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.AnimalId, a.TagNumber, a.Name AS AnimalName, a.Status,
           ISNULL(SUM(m.TotalMilk),0)                            AS TotalMilkLitres,
           ISNULL(SUM(m.TotalValue),0)                           AS TotalValue,
           COUNT(DISTINCT m.LogDate)                             AS DaysRecorded,
           CASE WHEN COUNT(DISTINCT m.LogDate) > 0
                THEN ISNULL(SUM(m.TotalMilk),0) / COUNT(DISTINCT m.LogDate)
                ELSE 0 END                                       AS AvgDailyYield
    FROM dbo.Animals a
    LEFT JOIN dbo.MilkProductionLog m
           ON m.AnimalId = a.AnimalId
          AND m.LogDate BETWEEN @StartDate AND @EndDate
    GROUP BY a.AnimalId, a.TagNumber, a.Name, a.Status
    ORDER BY TotalMilkLitres DESC;
END
GO

PRINT '02_StoredProcedures.sql complete.';
GO
