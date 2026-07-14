USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    AUTO-GENERATED — Phase 4.2 SP stub for AutoNumberComponent
    ReportDefinition StoreProcedureName: app.rpt_AutoNumberComponent
    ParameterProfile: none
    FastReport DataSource: AutoNumberComponentData
*/

IF OBJECT_ID(N'[app].[rpt_AutoNumberComponent]', N'P') IS NOT NULL
    DROP PROCEDURE [app].[rpt_AutoNumberComponent];
GO

CREATE PROCEDURE [app].[rpt_AutoNumberComponent]            AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.AutoNumberComponentId,
        t.AutoNumberComponentGuid,
        t.ComponentType,
        t.StaticValue,
        SYSDATETIME() AS PrintedAt
    FROM [app].[AutoNumberComponent] t
    WHERE t.StatusId > 0
      -- TODO: add soft-delete filter if applicable

    ;
END
GO

PRINT 'app.rpt_AutoNumberComponent created.';
GO