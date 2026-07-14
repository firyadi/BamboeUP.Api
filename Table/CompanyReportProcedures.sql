USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    AUTO-GENERATED — Phase 4.2 SP stub for Company
    DataSource: Table [app].[Company]
    ReportDefinition StoreProcedureName: app.rpt_Company
    ParameterProfile: scope
    FastReport DataSource: CompanyData
*/

IF OBJECT_ID(N'[app].[rpt_Company]', N'P') IS NOT NULL
    DROP PROCEDURE [app].[rpt_Company];
GO

CREATE PROCEDURE [app].[rpt_Company]
    @CompanyId NVARCHAR(50) = NULL,
    @CompanyOfficeId NVARCHAR(50) = NULL
            AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.CompanyId,
        t.CompanyGuid,
        t.CompanyName,
        t.InitialName,
        SYSDATETIME() AS PrintedAt
    FROM [app].[Company] t
    WHERE 1 = 1
      AND t.StatusId > 0
      -- TODO: add soft-delete filter if applicable
      -- NOTE: @CompanyOfficeId scope filter skipped — column CompanyOfficeId not found on source.
      AND (@CompanyId IS NULL OR CAST(t.CompanyId AS NVARCHAR(50)) = @CompanyId)
                ;
END
GO

PRINT 'app.rpt_Company created.';
GO