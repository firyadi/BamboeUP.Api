USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    AUTO-GENERATED — Phase 4.2 SP stub for Hospital
    DataSource: Table [app].[Hospital]
    ReportDefinition StoreProcedureName: app.rpt_Hospital
    ParameterProfile: none
    FastReport DataSource: HospitalData
*/

IF OBJECT_ID(N'[app].[rpt_Hospital]', N'P') IS NOT NULL
    DROP PROCEDURE [app].[rpt_Hospital];
GO

CREATE PROCEDURE [app].[rpt_Hospital]            AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.HospitalId,
        t.HospitalGuid,
        t.HospitalName,
        t.HospitalCode,
        SYSDATETIME() AS PrintedAt
    FROM [app].[Hospital] t
    WHERE 1 = 1
      AND t.StatusId > 0
      -- TODO: add soft-delete filter if applicable
                ;
END
GO

PRINT 'app.rpt_Hospital created.';
GO