USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    AUTO-GENERATED - pivot (PVT) aggregate stub for SalesTransaction
    DataSource: Table [app].[SalesTransaction]
    ReportDefinition StoreProcedureName: app.pvt_SalesTransaction
    ParameterProfile: period

    Returns a pre-aggregated grain, not detail rows. Dimensions: 8,
    measures: 5.
        Skipped (grain cap / unique code): TransactionCode, ProductCode, TransactionDate, CompanyOfficeId, StatusId
*/

IF OBJECT_ID(N'[app].[pvt_SalesTransaction]', N'P') IS NOT NULL
    DROP PROCEDURE [app].[pvt_SalesTransaction];
GO

CREATE PROCEDURE [app].[pvt_SalesTransaction]
    @CompanyId NVARCHAR(50) = NULL,
    @CompanyOfficeId NVARCHAR(50) = NULL,
    @DateFrom DATETIME = NULL,
    @DateTo DATETIME = NULL
            AS
BEGIN
    SET NOCOUNT ON;

    -- Server-side guard: the client must never be the one deciding what to discard.
    DECLARE @PivotMaxRows INT = 50000;

    SELECT TOP (@PivotMaxRows)
        t.ProductCategory AS ProductCategory,
        t.SalesChannel AS SalesChannel,
        t.CustomerSegment AS CustomerSegment,
        t.RegionCode AS RegionCode,
        t.PeriodYear AS PeriodYear,
        t.PeriodMonth AS PeriodMonth,
        t.ProductName AS ProductName,
        t.CompanyId AS CompanyId,
        COUNT_BIG(*) AS SalesTransactionCount,
        SUM(t.Quantity) AS Quantity,
        SUM(t.UnitPrice) AS UnitPrice,
        SUM(t.DiscountAmount) AS DiscountAmount,
        SUM(t.NetAmount) AS NetAmount,
        SUM(t.CostAmount) AS CostAmount
    FROM [app].[SalesTransaction] t
    WHERE 1 = 1
      AND t.StatusId > 0
      AND t.DeletedTime IS NULL
      AND (@CompanyId IS NULL OR CAST(t.CompanyId AS NVARCHAR(50)) = @CompanyId)
      AND (@CompanyOfficeId IS NULL OR CAST(t.CompanyOfficeId AS NVARCHAR(50)) = @CompanyOfficeId)
      AND (@DateFrom IS NULL OR t.CreatedTime >= @DateFrom)
      AND (@DateTo IS NULL OR t.CreatedTime < DATEADD(day, 1, CAST(CAST(@DateTo AS DATE) AS DATETIME)))
    GROUP BY
        t.ProductCategory,
        t.SalesChannel,
        t.CustomerSegment,
        t.RegionCode,
        t.PeriodYear,
        t.PeriodMonth,
        t.ProductName,
        t.CompanyId
    ORDER BY Quantity DESC;
END
GO

PRINT 'app.pvt_SalesTransaction created.';
GO
