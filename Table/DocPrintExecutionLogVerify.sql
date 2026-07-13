USE [Dev.BamboeHR]
GO

/*
    Phase 3.9.8 / §9B — Verify DOC execution log after toolbar print
    Run after printing Bank engine demo slips (02.09.04.01 / .03 / .04 / .05).
    Expect rows in [rpt].[ReportExecutionLog] when ReportDefinition.IsTracked = 1.
*/

SET NOCOUNT ON;

DECLARE @SlipCodes TABLE (ProgramCode NVARCHAR(32) NOT NULL);
INSERT INTO @SlipCodes (ProgramCode)
VALUES
    (N'02.09.04.01'),  -- QuestPDF
    (N'02.09.04.03'),  -- FastReport
    (N'02.09.04.04'),  -- Telerik
    (N'02.09.04.05');  -- DevExpress (optional)

SELECT
    l.ReportExecutionLogId,
    l.ReportExecutionGuid,
    l.ProgramCode,
    l.ProgramName,
    l.ReportKind,
    l.Status,
    l.OutputFormat,
    l.ReportPrintIdMasked,
    l.DurationMs,
    l.CreatedTime,
    rd.IsTracked,
    rd.RequiresPrintId,
    rd.DefinitionKey,
    rd.RendererType
FROM [rpt].[ReportExecutionLog] l
INNER JOIN [core].[Programs] p ON p.ProgramId = l.ProgramId
INNER JOIN [core].[ReportDefinition] rd ON rd.ReportDefinitionId = l.ReportDefinitionId
WHERE p.ProgramCode IN (SELECT ProgramCode FROM @SlipCodes)
ORDER BY l.CreatedTime DESC;

PRINT 'Latest DOC execution log rows for Bank engine demo slips listed above.';
GO
