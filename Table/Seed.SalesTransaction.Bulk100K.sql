USE [Dev.BamboeHR]
GO

SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/*
    Bulk generator — 100 000 pivot-friendly sales fact rows.
    Prerequisites: run SalesTransaction.sql first.

    Dimensions (low cardinality, good for WebDataRocks slice):
      RegionCode       8  (R01–R08)
      ProductCategory  12 (Electronics, FMCG, …)
      SalesChannel     5  (Online, Retail, Partner, B2B, Direct)
      CustomerSegment  4  (SME, Enterprise, Consumer, Government)
      PeriodYear       3  (2023–2025)
      PeriodMonth      12

    Measures: Quantity, UnitPrice, DiscountAmount, NetAmount, CostAmount

    Idempotent: skips if >= @TargetRows active rows exist.
    Re-run: TRUNCATE TABLE [app].[SalesTransaction]; then execute again.
*/

DECLARE @CreatedById BIGINT = 1;
DECLARE @TargetRows  INT = 100000;
DECLARE @Existing    INT;
DECLARE @Started     DATETIME2(7) = SYSDATETIME();

SELECT @Existing = COUNT(*)
FROM [app].[SalesTransaction]
WHERE [DeletedTime] IS NULL AND [StatusId] > 0;

IF @Existing >= @TargetRows
BEGIN
    PRINT CONCAT('Skip — already ', @Existing, ' active rows (target ', @TargetRows, ').');
    PRINT 'To regenerate: TRUNCATE TABLE [app].[SalesTransaction]; then re-run this script.';
    RETURN;
END

IF OBJECT_ID(N'[app].[SalesTransaction]', N'U') IS NULL
BEGIN
    RAISERROR('Table [app].[SalesTransaction] not found. Run SalesTransaction.sql first.', 16, 1);
    RETURN;
END

PRINT CONCAT('=== Generating ', @TargetRows, ' SalesTransaction rows ===');

;WITH
    E1 AS (SELECT 1 AS n UNION ALL SELECT 1),
    E2 AS (SELECT 1 AS n FROM E1 a CROSS JOIN E1 b),
    E4 AS (SELECT 1 AS n FROM E2 a CROSS JOIN E2 b),
    E8 AS (SELECT 1 AS n FROM E4 a CROSS JOIN E4 b),
    E16 AS (SELECT 1 AS n FROM E8 a CROSS JOIN E8 b),
    E32 AS (SELECT 1 AS n FROM E16 a CROSS JOIN E1 b),
    Nums AS (
        SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
        FROM E32
    ),
    Src AS (
        SELECT
            n,
            CONCAT('TXN-', RIGHT(CONCAT('000000', n), 6)) AS TransactionCode,
            1 + (n % 3) AS CompanyId,
            1 + (n % 5) AS CompanyOfficeId,
            2023 + (n % 3) AS PeriodYear,
            1 + (n % 12) AS PeriodMonth,
            DATEFROMPARTS(2023 + (n % 3), 1 + (n % 12), 1 + (n % 28)) AS TransactionDate,
            CONCAT('R', RIGHT(CONCAT('0', 1 + (n % 8)), 2)) AS RegionCode,
            CASE (n % 12)
                WHEN 0  THEN 'Electronics'
                WHEN 1  THEN 'FMCG'
                WHEN 2  THEN 'Apparel'
                WHEN 3  THEN 'Home & Living'
                WHEN 4  THEN 'Automotive'
                WHEN 5  THEN 'Healthcare'
                WHEN 6  THEN 'Sports'
                WHEN 7  THEN 'Books'
                WHEN 8  THEN 'Food & Beverage'
                WHEN 9  THEN 'Beauty'
                WHEN 10 THEN 'Office Supplies'
                ELSE 'Industrial'
            END AS ProductCategory,
            CONCAT('PRD-', RIGHT(CONCAT('0000', 1 + (n % 120)), 4)) AS ProductCode,
            CASE (n % 12)
                WHEN 0  THEN CONCAT(N'Smart Device ', 1 + (n % 50))
                WHEN 1  THEN CONCAT(N'Packaged Goods ', 1 + (n % 50))
                WHEN 2  THEN CONCAT(N'Fashion Item ', 1 + (n % 50))
                WHEN 3  THEN CONCAT(N'Home Product ', 1 + (n % 50))
                WHEN 4  THEN CONCAT(N'Auto Part ', 1 + (n % 50))
                WHEN 5  THEN CONCAT(N'Medical Supply ', 1 + (n % 50))
                WHEN 6  THEN CONCAT(N'Sport Gear ', 1 + (n % 50))
                WHEN 7  THEN CONCAT(N'Book Title ', 1 + (n % 50))
                WHEN 8  THEN CONCAT(N'Beverage ', 1 + (n % 50))
                WHEN 9  THEN CONCAT(N'Cosmetic ', 1 + (n % 50))
                WHEN 10 THEN CONCAT(N'Office Item ', 1 + (n % 50))
                ELSE CONCAT(N'Industrial Part ', 1 + (n % 50))
            END AS ProductName,
            CASE (n % 5)
                WHEN 0 THEN 'Online'
                WHEN 1 THEN 'Retail'
                WHEN 2 THEN 'Partner'
                WHEN 3 THEN 'B2B'
                ELSE 'Direct'
            END AS SalesChannel,
            CASE (n % 4)
                WHEN 0 THEN 'SME'
                WHEN 1 THEN 'Enterprise'
                WHEN 2 THEN 'Consumer'
                ELSE 'Government'
            END AS CustomerSegment,
            (n % 50) + 1 AS Quantity,
            CAST(((n % 200) + 10) * 1000.00 AS DECIMAL(18, 2)) AS UnitPrice,
            CAST((n % 10) * 500.00 AS DECIMAL(18, 2)) AS DiscountAmount
        FROM Nums
        WHERE n <= @TargetRows
    )
INSERT INTO [app].[SalesTransaction] WITH (TABLOCK)
(
    [TransactionCode],
    [CompanyId],
    [CompanyOfficeId],
    [PeriodYear],
    [PeriodMonth],
    [TransactionDate],
    [RegionCode],
    [ProductCategory],
    [ProductCode],
    [ProductName],
    [SalesChannel],
    [CustomerSegment],
    [Quantity],
    [UnitPrice],
    [DiscountAmount],
    [NetAmount],
    [CostAmount],
    [StatusId],
    [CreatedById],
    [CreatedTime]
)
SELECT
    s.TransactionCode,
    s.CompanyId,
    s.CompanyOfficeId,
    s.PeriodYear,
    s.PeriodMonth,
    s.TransactionDate,
    s.RegionCode,
    s.ProductCategory,
    s.ProductCode,
    s.ProductName,
    s.SalesChannel,
    s.CustomerSegment,
    s.Quantity,
    s.UnitPrice,
    s.DiscountAmount,
    CAST(s.Quantity * s.UnitPrice - s.DiscountAmount AS DECIMAL(18, 2)) AS NetAmount,
    CAST((s.Quantity * s.UnitPrice - s.DiscountAmount) * 0.65 AS DECIMAL(18, 2)) AS CostAmount,
    1,
    @CreatedById,
    SYSDATETIME()
FROM Src s;

DECLARE @Inserted INT = @@ROWCOUNT;
DECLARE @ElapsedMs INT = DATEDIFF(MILLISECOND, @Started, SYSDATETIME());

PRINT CONCAT('Inserted ', @Inserted, ' rows in ', @ElapsedMs, ' ms.');

SELECT
    COUNT(*) AS TotalRows,
    MIN([TransactionDate]) AS MinDate,
    MAX([TransactionDate]) AS MaxDate,
    SUM([NetAmount]) AS TotalNetAmount,
    COUNT(DISTINCT [RegionCode]) AS RegionCount,
    COUNT(DISTINCT [ProductCategory]) AS CategoryCount,
    COUNT(DISTINCT [SalesChannel]) AS ChannelCount
FROM [app].[SalesTransaction]
WHERE [DeletedTime] IS NULL AND [StatusId] > 0;

PRINT '=== Sample pivot slice preview (Region × Category × SUM NetAmount) ===';

SELECT TOP (20)
    [RegionCode],
    [ProductCategory],
    SUM([NetAmount]) AS TotalNetAmount,
    SUM([Quantity]) AS TotalQuantity,
    COUNT(*) AS TxnCount
FROM [app].[SalesTransaction]
WHERE [DeletedTime] IS NULL AND [StatusId] > 0
GROUP BY [RegionCode], [ProductCategory]
ORDER BY TotalNetAmount DESC;

GO
