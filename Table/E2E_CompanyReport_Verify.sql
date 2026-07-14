USE [Dev.BamboeHR]
GO
SET NOCOUNT ON;

PRINT '=== E2E Company Report Verification (Phase 4.9) ==='

-- P1: Active companies
PRINT ''
PRINT '-- P1: Active companies --'
SELECT CompanyId, CompanyName, StatusId
FROM [app].[Company]
WHERE StatusId > 0
ORDER BY CompanyId;

DECLARE @CompanyCount INT = (SELECT COUNT(*) FROM [app].[Company] WHERE StatusId > 0);
PRINT 'Active company count: ' + CAST(@CompanyCount AS NVARCHAR(10));

-- P2: Reports menu
PRINT ''
PRINT '-- P2: Reports menu 96.00.00 --'
SELECT ProgramId, ProgramCode, ProgramName
FROM [core].[Programs]
WHERE ProgramCode = N'96.00.00';

-- 4.1 Metadata
PRINT ''
PRINT '-- 4.1 Program + ReportDefinition --'
SELECT p.ProgramCode, p.ProgramName, p.ProgramType, p.StoreProcedureName,
       rd.DefinitionKey, rd.RendererType, rd.FilePath,
       rd.IsTracked, rd.RequiresPrintId, rd.PrintIdPolicy, rd.PrintIdPrefix
FROM [core].[Programs] p
INNER JOIN [core].[ReportDefinition] rd ON rd.ProgramId = p.ProgramId
WHERE p.ProgramCode = N'RPT.02.09.01';

PRINT ''
PRINT '-- 4.1 ReportParameter --'
SELECT rp.ParameterName, rp.DisplayLabel, rp.ControlType, rp.ColumnGroup, rp.SortOrder
FROM [core].[ReportParameter] rp
INNER JOIN [core].[ReportDefinition] rd ON rd.ReportDefinitionId = rp.ReportDefinitionId
INNER JOIN [core].[Programs] p ON p.ProgramId = rd.ProgramId
WHERE p.ProgramCode = N'RPT.02.09.01'
  AND rp.StatusId > 0
ORDER BY rp.SortOrder;

-- 4.2 SP definition checks
PRINT ''
PRINT '-- 4.2 SP filter clause present? --'
DECLARE @SpDef NVARCHAR(MAX) = OBJECT_DEFINITION(OBJECT_ID(N'[app].[rpt_Company]'));
IF @SpDef IS NULL
    PRINT 'FAIL: app.rpt_Company not found';
ELSE
BEGIN
    IF @SpDef LIKE '%AND (@CompanyId IS NULL OR CAST(t.CompanyId AS NVARCHAR(50)) = @CompanyId)%'
        PRINT 'PASS: CompanyId filter clause found';
    ELSE
        PRINT 'FAIL: CompanyId filter clause missing';

    IF @SpDef LIKE '%TODO: apply @CompanyId%'
        PRINT 'FAIL: Old TODO comment still present';
    ELSE
        PRINT 'PASS: No TODO scope comment';

    IF @SpDef LIKE '%@CompanyOfficeId scope filter skipped%'
        PRINT 'PASS: CompanyOfficeId skip note found';
    ELSE
        PRINT 'WARN: CompanyOfficeId skip note not found';
END

-- 4.3 SP data filter test
PRINT ''
PRINT '-- 4.3 EXEC app.rpt_Company (no filter) --'
EXEC [app].[rpt_Company];

DECLARE @IdA BIGINT = (SELECT MIN(CompanyId) FROM [app].[Company] WHERE StatusId > 0);
DECLARE @IdB BIGINT = (
    SELECT MIN(CompanyId) FROM [app].[Company]
    WHERE StatusId > 0 AND CompanyId > @IdA
);
DECLARE @CompanyIdA NVARCHAR(50);
DECLARE @CompanyIdB NVARCHAR(50);

IF @IdA IS NOT NULL
BEGIN
    SET @CompanyIdA = CAST(@IdA AS NVARCHAR(50));
    PRINT ''
    PRINT '-- 4.3 EXEC with CompanyId A = ' + @CompanyIdA + ' --'
    EXEC [app].[rpt_Company] @CompanyId = @CompanyIdA;
END

IF @IdB IS NOT NULL
BEGIN
    SET @CompanyIdB = CAST(@IdB AS NVARCHAR(50));
    PRINT ''
    PRINT '-- 4.3 EXEC with CompanyId B = ' + @CompanyIdB + ' --'
    EXEC [app].[rpt_Company] @CompanyId = @CompanyIdB;
END
ELSE
    PRINT 'WARN: Only one active company — cannot test filter B';

GO
