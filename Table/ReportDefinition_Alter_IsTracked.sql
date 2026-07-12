USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[core].[ReportDefinition]') AND name = N'IsTracked'
)
BEGIN
    ALTER TABLE [core].[ReportDefinition]
        ADD [IsTracked] BIT NOT NULL CONSTRAINT [DF_ReportDefinition_IsTracked] DEFAULT (0);
END
GO

-- PrintId requires tracked
IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = N'CK_ReportDefinition_PrintIdRequiresTracked'
)
BEGIN
    ALTER TABLE [core].[ReportDefinition]
        ADD CONSTRAINT [CK_ReportDefinition_PrintIdRequiresTracked]
        CHECK ([RequiresPrintId] = 0 OR [IsTracked] = 1);
END
GO

-- Existing rows with PrintId must be tracked
UPDATE [core].[ReportDefinition]
SET [IsTracked] = 1
WHERE [RequiresPrintId] = 1 AND [IsTracked] = 0;
GO

-- Sample policy: employee list = tracked without print id; salary slip = tracked + print id; pivot = not tracked
UPDATE rd
SET [IsTracked] = 1, [RequiresPrintId] = 0
FROM [core].[ReportDefinition] rd
INNER JOIN [core].[Programs] p ON p.ProgramId = rd.ProgramId
WHERE p.ProgramCode = N'96.01.01' AND rd.ReportKind = N'RPT';

UPDATE rd
SET [IsTracked] = 1, [RequiresPrintId] = 1
FROM [core].[ReportDefinition] rd
INNER JOIN [core].[Programs] p ON p.ProgramId = rd.ProgramId
WHERE p.ProgramCode = N'96.01.02' AND rd.ReportKind = N'RPT';

UPDATE rd
SET [IsTracked] = 0, [RequiresPrintId] = 0
FROM [core].[ReportDefinition] rd
INNER JOIN [core].[Programs] p ON p.ProgramId = rd.ProgramId
WHERE p.ProgramCode = N'96.02.01' AND rd.ReportKind = N'PVT';
GO

PRINT 'ReportDefinition.IsTracked ready.';
GO
