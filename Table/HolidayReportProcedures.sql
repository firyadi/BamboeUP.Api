USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    AUTO-GENERATED — Phase 4.2 SP stub for Holiday
    DataSource: Table [app].[Holiday]
    ReportDefinition StoreProcedureName: app.rpt_Holiday
    ParameterProfile: none
    FastReport DataSource: HolidayData
*/

IF OBJECT_ID(N'[app].[rpt_Holiday]', N'P') IS NOT NULL
    DROP PROCEDURE [app].[rpt_Holiday];
GO

CREATE PROCEDURE [app].[rpt_Holiday]            AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.HolidayId,
        t.HolidayGuid,
        t.HolidayName,
        t.HolidayDescription,
        t.HolidayDate,
        t.SrHolidayType,
        t.IsNational,
        t.IsCompanyHoliday,
        SYSDATETIME() AS PrintedAt
    FROM [app].[Holiday] t
    WHERE 1 = 1
      AND t.StatusId > 0
      -- TODO: add soft-delete filter if applicable
                ;
END
GO

PRINT 'app.rpt_Holiday created.';
GO
