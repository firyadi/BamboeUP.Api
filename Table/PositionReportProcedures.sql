USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    AUTO-GENERATED — Phase 4.2 SP stub for Position
    DataSource: Table [app].[Position]
    ReportDefinition StoreProcedureName: app.rpt_Position
    ParameterProfile: none
    FastReport DataSource: PositionData
*/

IF OBJECT_ID(N'[app].[rpt_Position]', N'P') IS NOT NULL
    DROP PROCEDURE [app].[rpt_Position];
GO

CREATE PROCEDURE [app].[rpt_Position]            AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.PositionId,
        t.PositionGuid,
        t.PositionCode,
        t.PositionName,
        t.Summary,
        t.PositionGradeId,
        t.PositionLevelId,
        t.ValidFrom,
        t.Validto,
        t.PositionDescription,
        t.StatusId,
        SYSDATETIME() AS PrintedAt
    FROM [app].[Position] t
    WHERE 1 = 1
      AND t.StatusId > 0
      -- TODO: add soft-delete filter if applicable
                ;
END
GO

PRINT 'app.rpt_Position created.';
GO