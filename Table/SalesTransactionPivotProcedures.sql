USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    Pivot data source — flat rows for WebDataRocks (ReportKind=PVT).
    Source: [app].[SalesTransaction] (100k bulk seed)
    StoreProcedureName: app.pvt_SalesTransaction

    Suggested WDR slice:
      Rows:    RegionCode, ProductCategory
      Columns: SalesChannel, PeriodYear
      Values:  SUM(NetAmount), SUM(Quantity), SUM(CostAmount)
*/

IF OBJECT_ID(N'[app].[pvt_SalesTransaction]', N'P') IS NOT NULL
    DROP PROCEDURE [app].[pvt_SalesTransaction];
GO

CREATE PROCEDURE [app].[pvt_SalesTransaction]
    @CompanyId BIGINT = NULL,
    @CompanyOfficeId BIGINT = NULL,
    @PeriodYear INT = NULL,
    @PeriodMonth INT = NULL,
    @DateFrom DATETIME = NULL,
    @DateTo DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.SalesTransactionId,
        t.TransactionCode,
        t.CompanyId,
        t.CompanyOfficeId,
        t.PeriodYear,
        t.PeriodMonth,
        t.TransactionDate,
        t.RegionCode,
        t.ProductCategory,
        t.ProductCode,
        t.ProductName,
        t.SalesChannel,
        t.CustomerSegment,
        t.Quantity,
        t.UnitPrice,
        t.DiscountAmount,
        t.NetAmount,
        t.CostAmount,
        t.NetAmount - t.CostAmount AS GrossMargin,
        SYSDATETIME() AS PrintedAt
    FROM [app].[SalesTransaction] t
    WHERE t.StatusId > 0
      AND t.DeletedTime IS NULL
      AND (@CompanyId IS NULL OR t.CompanyId = @CompanyId)
      AND (@CompanyOfficeId IS NULL OR t.CompanyOfficeId = @CompanyOfficeId)
      AND (@PeriodYear IS NULL OR t.PeriodYear = @PeriodYear)
      AND (@PeriodMonth IS NULL OR t.PeriodMonth = @PeriodMonth)
      AND (@DateFrom IS NULL OR t.TransactionDate >= CAST(@DateFrom AS DATE))
      AND (@DateTo IS NULL OR t.TransactionDate <= CAST(@DateTo AS DATE));
END
GO

PRINT 'app.pvt_SalesTransaction created.';
GO

/*
    Quick smoke test (optional — comment out on deploy):

    EXEC [app].[pvt_SalesTransaction]
        @CompanyId = 1,
        @PeriodYear = 2024;

    SELECT RegionCode, ProductCategory, SalesChannel,
           SUM(NetAmount) AS TotalNet, SUM(Quantity) AS TotalQty, COUNT(*) AS Cnt
    FROM (
        EXEC [app].[pvt_SalesTransaction] @CompanyId = 1, @PeriodYear = 2024
    ) x
    GROUP BY RegionCode, ProductCategory, SalesChannel
    ORDER BY TotalNet DESC;
*/

GO
