USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    Phase 4.5 — ReportDefinition.LayoutJson
    Stores page/header presets (paper, orientation, company logo flags).
*/

IF COL_LENGTH(N'[core].[ReportDefinition]', N'LayoutJson') IS NULL
BEGIN
    ALTER TABLE [core].[ReportDefinition]
        ADD [LayoutJson] NVARCHAR(MAX) NULL;
END
GO

PRINT 'ReportDefinition.LayoutJson ready.';
GO
