/*
    Read grain for the sales analytics page.
    One row per dimension combination. The page groups and sums these rows again.
*/
CREATE OR ALTER PROCEDURE [app].[usp_SalesTransactionPie]
    @CompanyId bigint,
    @CompanyOfficeId bigint = NULL,
    @DateFrom datetime,
    @DateTo datetime
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.PeriodYear,
        t.PeriodMonth,
        t.RegionCode,
        t.ProductCategory,
        t.ProductName,
        t.SalesChannel,
        t.CustomerSegment,
        t.Quantity,
        t.NetAmount,
        t.DiscountAmount,
        t.CostAmount
    INTO #src
    FROM [app].[SalesTransaction] t
    WHERE t.StatusId > 0
      AND t.DeletedTime IS NULL
      AND t.CompanyId = @CompanyId
      AND (@CompanyOfficeId IS NULL OR t.CompanyOfficeId = @CompanyOfficeId)
      AND t.TransactionDate >= @DateFrom
      AND t.TransactionDate < DATEADD(day, 1, CAST(@DateTo AS date));

    SELECT
        Box1Caption = 'Total Sales',
        Box2Caption = 'Total Quantity',
        Box3Caption = 'Transactions',
        ISNULL(SUM(NetAmount), 0) AS Box1Value,
        ISNULL(SUM(CAST(Quantity AS bigint)), 0) AS Box2Value,
        COUNT_BIG(*) AS Box3Value
    FROM #src;

    SELECT
        PeriodYear,
        PeriodMonth,
        CONCAT(PeriodYear, '-', RIGHT(CONCAT('0', PeriodMonth), 2)) AS PeriodYearMonth,
        RegionCode,
        ProductCategory,
        ProductName,
        SalesChannel,
        CustomerSegment,
        CAST(SUM(Quantity) AS bigint) AS Quantity,
        SUM(NetAmount) AS NetAmount,
        SUM(DiscountAmount) AS DiscountAmount,
        SUM(CostAmount) AS CostAmount,
        COUNT_BIG(*) AS TransactionCount
    FROM #src
    GROUP BY
        PeriodYear,
        PeriodMonth,
        CONCAT(PeriodYear, '-', RIGHT(CONCAT('0', PeriodMonth), 2)),
        RegionCode,
        ProductCategory,
        ProductName,
        SalesChannel,
        CustomerSegment;
END
GO
