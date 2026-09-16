/* =============================================================================
   Milkora Dairy Farm Tracker
   01_Tables.sql  --  All 8 data tables.

   Design rules (per project spec):
     * Every primary key is UNIQUEIDENTIFIER (GUID) to prevent offline-created
       PK collisions when Dotmim.Sync merges devices.
     * Server-side inserts default the GUID via NEWID(); offline clients supply
       their own GUID (SQLite) so both paths are collision-free.
     * Every table carries CreatedAt (and UpdatedAt where the row is mutable),
       stored in UTC via SYSUTCDATETIME() so sync ordering is timezone-safe.
     * Idempotent: each table is created only if it does not already exist.
   ============================================================================= */

USE [MilkoraDB];
GO

-- Required so tables with PERSISTED computed columns and their indexes can be
-- created regardless of client default (sqlcmd/ODBC defaults QUOTED_IDENTIFIER OFF).
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* ---------------------------------------------------------------------------
   1. Animals  --  animal master records
   --------------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Animals', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Animals
    (
        AnimalId       UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Animals PRIMARY KEY DEFAULT NEWID(),
        TagNumber      NVARCHAR(50)     NOT NULL,
        Name           NVARCHAR(100)    NULL,
        Type           NVARCHAR(50)     NULL,          -- Cow / Buffalo / Goat ...
        Breed          NVARCHAR(100)    NULL,
        Gender         NVARCHAR(10)     NULL,          -- Female / Male
        DateOfBirth    DATE             NULL,
        PurchaseDate   DATE             NULL,
        PurchasePrice  DECIMAL(18,2)    NULL,
        Status         NVARCHAR(20)     NOT NULL DEFAULT N'Milking',  -- Milking/Pregnant/Dry/Sick/Sold/Dead
        DailyMilkYield DECIMAL(10,2)    NULL,
        PhotoPath      NVARCHAR(500)    NULL,
        Notes          NVARCHAR(1000)   NULL,
        CreatedAt      DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt      DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME()
    );
    CREATE UNIQUE INDEX UX_Animals_TagNumber ON dbo.Animals(TagNumber);
    CREATE INDEX IX_Animals_Status ON dbo.Animals(Status);
    PRINT 'Created table dbo.Animals';
END
GO

/* ---------------------------------------------------------------------------
   2. MilkProductionLog  --  daily morning / evening milk entries
   --------------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.MilkProductionLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MilkProductionLog
    (
        LogId        UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_MilkProductionLog PRIMARY KEY DEFAULT NEWID(),
        AnimalId     UNIQUEIDENTIFIER NULL,          -- nullable: allows farm-wide entries
        LogDate      DATE             NOT NULL,
        MorningMilk  DECIMAL(10,2)    NOT NULL DEFAULT 0,
        EveningMilk  DECIMAL(10,2)    NOT NULL DEFAULT 0,
        FatPercent   DECIMAL(5,2)     NULL,
        RatePerLitre DECIMAL(10,2)    NULL,
        -- computed helpers (persisted so reports can index/aggregate quickly)
        TotalMilk    AS (MorningMilk + EveningMilk) PERSISTED,
        TotalValue   AS ((MorningMilk + EveningMilk) * ISNULL(RatePerLitre,0)) PERSISTED,
        Notes        NVARCHAR(1000)   NULL,
        CreatedAt    DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt    DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_MilkLog_Animal FOREIGN KEY (AnimalId) REFERENCES dbo.Animals(AnimalId)
    );
    CREATE INDEX IX_MilkLog_LogDate ON dbo.MilkProductionLog(LogDate);
    CREATE INDEX IX_MilkLog_Animal ON dbo.MilkProductionLog(AnimalId);
    PRINT 'Created table dbo.MilkProductionLog';
END
GO

/* ---------------------------------------------------------------------------
   3. MilkSales  --  milk sold to buyers
   --------------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.MilkSales', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MilkSales
    (
        SaleId         UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_MilkSales PRIMARY KEY DEFAULT NEWID(),
        SaleDate       DATE             NOT NULL,
        BuyerName      NVARCHAR(150)    NOT NULL,
        QuantityLitres DECIMAL(10,2)    NOT NULL DEFAULT 0,
        RatePerLitre   DECIMAL(10,2)    NOT NULL DEFAULT 0,
        TotalAmount    AS (QuantityLitres * RatePerLitre) PERSISTED,
        PaymentStatus  NVARCHAR(20)     NOT NULL DEFAULT N'Pending',  -- Paid / Pending
        PaymentMode    NVARCHAR(20)     NULL,                          -- Cash / UPI / Bank
        Notes          NVARCHAR(1000)   NULL,
        CreatedAt      DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt      DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME()
    );
    CREATE INDEX IX_MilkSales_SaleDate ON dbo.MilkSales(SaleDate);
    CREATE INDEX IX_MilkSales_PaymentStatus ON dbo.MilkSales(PaymentStatus);
    PRINT 'Created table dbo.MilkSales';
END
GO

/* ---------------------------------------------------------------------------
   4. Expenses  --  all farm expenses by category
   --------------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Expenses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Expenses
    (
        ExpenseId   UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Expenses PRIMARY KEY DEFAULT NEWID(),
        ExpenseDate DATE             NOT NULL,
        Category    NVARCHAR(50)     NOT NULL,   -- Feed/Medicine/Labour/Electricity/Equipment/Transport/Animal Purchase/Miscellaneous
        Description NVARCHAR(500)    NULL,
        Amount      DECIMAL(18,2)    NOT NULL DEFAULT 0,
        PaymentMode NVARCHAR(20)     NULL,        -- Cash / UPI / Bank
        Notes       NVARCHAR(1000)   NULL,
        CreatedAt   DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt   DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME()
    );
    CREATE INDEX IX_Expenses_ExpenseDate ON dbo.Expenses(ExpenseDate);
    CREATE INDEX IX_Expenses_Category ON dbo.Expenses(Category);
    PRINT 'Created table dbo.Expenses';
END
GO

/* ---------------------------------------------------------------------------
   5. Income  --  all income sources
   --------------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Income', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Income
    (
        IncomeId    UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Income PRIMARY KEY DEFAULT NEWID(),
        IncomeDate  DATE             NOT NULL,
        Category    NVARCHAR(50)     NOT NULL,   -- Milk Sale / Calf Sale / Manure Sale / Other
        Description NVARCHAR(500)    NULL,
        Amount      DECIMAL(18,2)    NOT NULL DEFAULT 0,
        Notes       NVARCHAR(1000)   NULL,
        CreatedAt   DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt   DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME()
    );
    CREATE INDEX IX_Income_IncomeDate ON dbo.Income(IncomeDate);
    CREATE INDEX IX_Income_Category ON dbo.Income(Category);
    PRINT 'Created table dbo.Income';
END
GO

/* ---------------------------------------------------------------------------
   6. HealthRecords  --  vaccination, deworming, checkup, treatment
   --------------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.HealthRecords', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HealthRecords
    (
        HealthId     UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_HealthRecords PRIMARY KEY DEFAULT NEWID(),
        AnimalId     UNIQUEIDENTIFIER NULL,
        RecordDate   DATE             NOT NULL,
        RecordType   NVARCHAR(30)     NOT NULL,   -- Vaccine / Deworming / Checkup / Treatment
        MedicineName NVARCHAR(200)    NULL,
        VetName      NVARCHAR(150)    NULL,
        VetContact   NVARCHAR(50)     NULL,
        Cost         DECIMAL(18,2)    NULL,
        NextDueDate  DATE             NULL,        -- drives reminders
        Notes        NVARCHAR(1000)   NULL,
        CreatedAt    DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt    DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Health_Animal FOREIGN KEY (AnimalId) REFERENCES dbo.Animals(AnimalId)
    );
    CREATE INDEX IX_Health_Animal ON dbo.HealthRecords(AnimalId);
    CREATE INDEX IX_Health_NextDueDate ON dbo.HealthRecords(NextDueDate);
    PRINT 'Created table dbo.HealthRecords';
END
GO

/* ---------------------------------------------------------------------------
   7. BreedingRecords  --  insemination, pregnancy, calving
       ExpectedDeliveryDate = InseminationDate + 280 days (computed, persisted)
   --------------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.BreedingRecords', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BreedingRecords
    (
        BreedingId           UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_BreedingRecords PRIMARY KEY DEFAULT NEWID(),
        AnimalId             UNIQUEIDENTIFIER NULL,
        InseminationDate     DATE             NOT NULL,
        BullDetails          NVARCHAR(300)    NULL,
        PregnancyStatus      NVARCHAR(30)     NOT NULL DEFAULT N'Inseminated', -- Inseminated/Confirmed/NotPregnant/Delivered
        ExpectedDeliveryDate AS (DATEADD(DAY, 280, InseminationDate)) PERSISTED,
        CalvingDate          DATE             NULL,
        CalfDetails          NVARCHAR(300)    NULL,
        Notes                NVARCHAR(1000)   NULL,
        CreatedAt            DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt            DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Breeding_Animal FOREIGN KEY (AnimalId) REFERENCES dbo.Animals(AnimalId)
    );
    CREATE INDEX IX_Breeding_Animal ON dbo.BreedingRecords(AnimalId);
    CREATE INDEX IX_Breeding_ExpectedDelivery ON dbo.BreedingRecords(ExpectedDeliveryDate);
    PRINT 'Created table dbo.BreedingRecords';
END
GO

/* ---------------------------------------------------------------------------
   8. FeedInventory  --  feed stock, usage, low-stock alerts
   --------------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.FeedInventory', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FeedInventory
    (
        InventoryId     UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_FeedInventory PRIMARY KEY DEFAULT NEWID(),
        ItemName        NVARCHAR(150)    NOT NULL,
        QuantityInStock DECIMAL(12,2)    NOT NULL DEFAULT 0,
        Unit            NVARCHAR(20)     NULL,        -- kg / bag / litre ...
        PurchaseDate    DATE             NULL,
        Cost            DECIMAL(18,2)    NULL,
        DailyUsage      DECIMAL(12,2)    NULL,
        MinimumLevel    DECIMAL(12,2)    NOT NULL DEFAULT 0,   -- low-stock threshold
        Notes           NVARCHAR(1000)   NULL,
        CreatedAt       DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt       DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME()
    );
    CREATE INDEX IX_Feed_ItemName ON dbo.FeedInventory(ItemName);
    PRINT 'Created table dbo.FeedInventory';
END
GO

PRINT '01_Tables.sql complete.';
GO
