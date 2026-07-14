USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    AUTO-GENERATED — Phase 4.2 SP stub for Award
    DataSource: Table [app].[Award]
    ReportDefinition StoreProcedureName: app.rpt_Award
    ParameterProfile: none
    FastReport DataSource: AwardData
*/

IF OBJECT_ID(N'[app].[rpt_Award]', N'P') IS NOT NULL
    DROP PROCEDURE [app].[rpt_Award];
GO

CREATE PROCEDURE [app].[rpt_Award]            AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.AwardId,
        t.AwardGuid,
        t.AwardCode,
        t.AwardName,
        SYSDATETIME() AS PrintedAt
    FROM [app].[Award] t
    WHERE 1 = 1
      AND t.StatusId > 0
      -- TODO: add soft-delete filter if applicable
                ;
END
GO

PRINT 'app.rpt_Award created.';
GO