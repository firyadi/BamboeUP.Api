USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    AUTO-GENERATED - pivot (PVT) aggregate stub for Position
    DataSource: Table [app].[Position]
    ReportDefinition StoreProcedureName: app.pvt_Position
    ParameterProfile: none

    Returns a pre-aggregated grain, not detail rows. Dimensions: 8,
    measures: 0.
        Skipped (grain cap): ValidtoMonth, PositionDescription, StatusId
*/

IF OBJECT_ID(N'[app].[pvt_Position]', N'P') IS NOT NULL
    DROP PROCEDURE [app].[pvt_Position];
GO

CREATE PROCEDURE [app].[pvt_Position]            AS
BEGIN
    SET NOCOUNT ON;

    -- Server-side guard: the client must never be the one deciding what to discard.
    DECLARE @PivotMaxRows INT = 50000;

    SELECT TOP (@PivotMaxRows)
        t.PositionCode AS PositionCode,
        t.PositionName AS PositionName,
        t.Summary AS Summary,
        t.PositionGradeId AS PositionGradeId,
        t.PositionLevelId AS PositionLevelId,
        YEAR(t.ValidFrom) AS ValidFromYear,
        MONTH(t.ValidFrom) AS ValidFromMonth,
        YEAR(t.Validto) AS ValidtoYear,
        COUNT_BIG(*) AS PositionCount
    FROM [app].[Position] t
    WHERE 1 = 1
      AND t.StatusId > 0
      -- TODO: add soft-delete filter if applicable
    GROUP BY
        t.PositionCode,
        t.PositionName,
        t.Summary,
        t.PositionGradeId,
        t.PositionLevelId,
        YEAR(t.ValidFrom),
        MONTH(t.ValidFrom),
        YEAR(t.Validto)
    ORDER BY PositionCount DESC;
END
GO

PRINT 'app.pvt_Position created.';
GO