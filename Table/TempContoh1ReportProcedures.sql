USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    AUTO-GENERATED — Phase 4.2 SP stub for TempContoh1
    DataSource: Table [app].[TempContoh1]
    ReportDefinition StoreProcedureName: app.rpt_TempContoh1
    ParameterProfile: standard
    FastReport DataSource: TempContoh1Data
*/

IF OBJECT_ID(N'[app].[rpt_TempContoh1]', N'P') IS NOT NULL
    DROP PROCEDURE [app].[rpt_TempContoh1];
GO

CREATE PROCEDURE [app].[rpt_TempContoh1]
    @CompanyId NVARCHAR(50) = NULL,
    @CompanyOfficeId NVARCHAR(50) = NULL,
    @DateFrom DATETIME = NULL,
    @DateTo DATETIME = NULL
            AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.TempContoh1Id,
        t.TempContoh1Guid,
        t.PersonId,
        t.SrAwardType,
        t.StatusId,
        SYSDATETIME() AS PrintedAt
    FROM [app].[TempContoh1] t
    WHERE 1 = 1
      AND t.StatusId > 0
      -- TODO: add soft-delete filter if applicable
      -- NOTE: @CompanyId scope filter skipped — column CompanyId not found on source.
      -- NOTE: @CompanyOfficeId scope filter skipped — column CompanyOfficeId not found on source.
      AND (@DateFrom IS NULL OR t.CreatedTime >= @DateFrom)
      AND (@DateTo IS NULL OR t.CreatedTime < DATEADD(day, 1, CAST(CAST(@DateTo AS DATE) AS DATETIME)))
                ;
END
GO

PRINT 'app.rpt_TempContoh1 created.';
GO