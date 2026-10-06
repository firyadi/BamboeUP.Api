USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    AUTO-GENERATED — Phase 4.2 SP stub for WorkNotice
    DataSource: Table [app].[WorkNotice]
    ReportDefinition StoreProcedureName: app.rpt_WorkNotice
    ParameterProfile: none
    FastReport DataSource: WorkNoticeData
*/

IF OBJECT_ID(N'[app].[rpt_WorkNotice]', N'P') IS NOT NULL
    DROP PROCEDURE [app].[rpt_WorkNotice];
GO

CREATE PROCEDURE [app].[rpt_WorkNotice]            AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.WorkNoticeId,
        t.WorkNoticeGuid,
        t.WorkNoticeCode,
        t.WorkNoticeName,
        t.WorkNoticeDate,
        CONVERT(nvarchar(11), t.WorkNoticeDate, 106) AS WorkNoticeDateText,
        t.IsEditAble,
        CASE WHEN t.IsEditAble = 1 THEN N'Ya' ELSE N'Tidak' END AS EditableText,
        t.Note,
        t.StatusId,
        SYSDATETIME() AS PrintedAt
    FROM [app].[WorkNotice] t
    WHERE 1 = 1
      AND t.StatusId > 0
      -- TODO: add soft-delete filter if applicable
                ;
END
GO

PRINT 'app.rpt_WorkNotice created.';
GO