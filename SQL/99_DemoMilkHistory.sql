/* =============================================================================
   Milkora — 99_DemoMilkHistory.sql   (DEV ONLY, optional)
   Generates ~60 days of daily milk logs for the 3 seed animals so the ML
   features (forecast + anomaly detection) have real history to work with.
   Yamuna (COW-002) gets a deliberate ~55% drop 8-12 days ago = the anomaly.

   Idempotent: clears prior demo rows (Notes = 'DEMO') then regenerates.
   Remove these rows anytime with:  DELETE FROM dbo.MilkProductionLog WHERE Notes = 'DEMO';
   ============================================================================= */
USE [MilkoraDB];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

DELETE FROM dbo.MilkProductionLog WHERE Notes = 'DEMO';

DECLARE @Ganga   UNIQUEIDENTIFIER = 'A1111111-1111-1111-1111-111111111111';
DECLARE @Yamuna  UNIQUEIDENTIFIER = 'A2222222-2222-2222-2222-222222222222';
DECLARE @Lakshmi UNIQUEIDENTIFIER = 'A3333333-3333-3333-3333-333333333333';

;WITH days AS (
    SELECT TOP (60) n = ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1
    FROM sys.all_objects
)
INSERT INTO dbo.MilkProductionLog (AnimalId, LogDate, MorningMilk, EveningMilk, FatPercent, RatePerLitre, Notes)
SELECT a.AnimalId,
       CAST(DATEADD(DAY, -d.n, SYSUTCDATETIME()) AS DATE),
       ROUND(t.total * 0.55, 2),
       ROUND(t.total * 0.45, 2),
       a.fat,
       55,
       'DEMO'
FROM days d
CROSS APPLY (VALUES
    (@Ganga,   13.0, 4.1),
    (@Yamuna,   9.0, 4.5),
    (@Lakshmi,  8.2, 6.8)
) a(AnimalId, base, fat)
CROSS APPLY (VALUES (
    CASE
        WHEN a.AnimalId = @Yamuna AND d.n BETWEEN 0 AND 3 THEN 4.0       -- recent ongoing drop (last 4 days)
        ELSE a.base + (ABS(CHECKSUM(NEWID())) % 20) / 10.0 - 1.0          -- base +/- 1L noise
    END
)) t(total);

SELECT CONCAT('Demo rows inserted: ', (SELECT COUNT(*) FROM dbo.MilkProductionLog WHERE Notes = 'DEMO')) AS Result;
GO
