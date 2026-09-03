USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    SalesTransaction pivot catalog — 5 renderer engines (app.SalesTransaction).
    Programs: PVT.01.001 .. PVT.01.005
    SP:        app.pvt_SalesTransaction (SalesTransactionPivotProcedures.sql)
    Config:    BamboeUp.Report/Pvt/Standard/SalesTransaction*.json

    Prerequisites:
      - ReportMenuSeed.sql (menu 96.00.00 + Pivot Selection 96.00.02)
      - SalesTransaction.sql + bulk seed
      - SalesTransactionPivotProcedures.sql
*/

SET NOCOUNT ON;

DECLARE @CreatedById BIGINT = 1;
DECLARE @ReportsMenuId BIGINT;
DECLARE @LayoutJson NVARCHAR(MAX) = N'{"paper":"A4","orientation":"Portrait","marginPreset":"Normal","header":{"profile":"Minimal","showCompanyLogo":false,"showCompanyName":false,"showOfficeName":false,"showPrintedAt":false}}';

SELECT @ReportsMenuId = ProgramId FROM [core].[Programs] WHERE ProgramCode = N'96.00.00';

IF @ReportsMenuId IS NULL
BEGIN
    RAISERROR('Reports menu 96.00.00 not found. Run ReportMenuSeed.sql first.', 16, 1);
    RETURN;
END

IF COL_LENGTH(N'[core].[ReportDefinition]', N'RendererType') IS NULL
BEGIN
    ALTER TABLE [core].[ReportDefinition] ADD [RendererType] NVARCHAR(30) NULL;
END

IF COL_LENGTH(N'[core].[ReportDefinition]', N'LayoutJson') IS NULL
BEGIN
    ALTER TABLE [core].[ReportDefinition] ADD [LayoutJson] NVARCHAR(MAX) NULL;
END

-- Retire legacy single-engine code (SchemaStudio generator default).
UPDATE [core].[Programs]
SET IsActive = 0, IsVisible = 0, StatusId = 0
WHERE ProgramCode = N'PVT.01.0001';

PRINT '=== Seed: SalesTransaction pivot engines (PVT.01.001 - PVT.01.005) ===';

DECLARE @Engines TABLE (
    RowIndex INT NOT NULL,
    ProgramCode NVARCHAR(20) NOT NULL,
    ProgramName NVARCHAR(200) NOT NULL,
    DefinitionKey NVARCHAR(100) NOT NULL,
    RendererType NVARCHAR(30) NOT NULL,
    FilePath NVARCHAR(260) NOT NULL
);

INSERT INTO @Engines (RowIndex, ProgramCode, ProgramName, DefinitionKey, RendererType, FilePath)
VALUES
    (11, N'PVT.01.001', N'SalesTransaction Pivot (WebDataRocks)', N'SalesTransaction_WebDataRocks', N'WebDataRocks', N'Pvt/Standard/SalesTransaction.json'),
    (12, N'PVT.01.002', N'SalesTransaction Pivot (VoltCube)',     N'SalesTransaction_VoltCube',     N'VoltCube',     N'Pvt/Standard/SalesTransaction.VoltCube.json'),
    (13, N'PVT.01.003', N'SalesTransaction Pivot (Native)',       N'SalesTransaction_Native',       N'Native',       N'Pvt/Standard/SalesTransaction.Native.json'),
    (14, N'PVT.01.004', N'SalesTransaction Pivot (Telerik)',      N'SalesTransaction_TelerikPivot', N'TelerikPivot', N'Pvt/Standard/SalesTransaction.TelerikPivot.json'),
    (15, N'PVT.01.005', N'SalesTransaction Pivot (DevExpress)', N'SalesTransaction_DevExpressPivot', N'DevExpressPivot', N'Pvt/Standard/SalesTransaction.DevExpressPivot.json');

DECLARE @ProgramCode NVARCHAR(20);
DECLARE @ProgramName NVARCHAR(200);
DECLARE @DefinitionKey NVARCHAR(100);
DECLARE @RendererType NVARCHAR(30);
DECLARE @FilePath NVARCHAR(260);
DECLARE @RowIndex INT;
DECLARE @ProgramId BIGINT;
DECLARE @ReportDefinitionId BIGINT;

DECLARE engine_cursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT RowIndex, ProgramCode, ProgramName, DefinitionKey, RendererType, FilePath
    FROM @Engines
    ORDER BY RowIndex;

OPEN engine_cursor;
FETCH NEXT FROM engine_cursor INTO @RowIndex, @ProgramCode, @ProgramName, @DefinitionKey, @RendererType, @FilePath;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT CONCAT('  -> ', @ProgramCode, ' / ', @RendererType);

    IF NOT EXISTS (SELECT 1 FROM [core].[Programs] WHERE ProgramCode = @ProgramCode)
    BEGIN
        INSERT INTO [core].[Programs]
        (ProgramGuid, ProgramCode, ProgramName, ParentId, RootLevel, RowIndex, ProgramType,
         IsProgramViewAble, IsProgramPrintAble, IsVisible, IsActive, StoreProcedureName,
         CreatedById, CreatedTime, StatusId)
        VALUES
        (NEWID(), @ProgramCode, @ProgramName, @ReportsMenuId, 2, @RowIndex, N'PVT',
         1, 1, 1, 1, N'app.pvt_SalesTransaction', @CreatedById, SYSDATETIME(), 1);
    END
    ELSE
    BEGIN
        UPDATE [core].[Programs]
        SET ProgramName = @ProgramName,
            ParentId = @ReportsMenuId,
            RowIndex = @RowIndex,
            ProgramType = N'PVT',
            IsProgramViewAble = 1,
            IsProgramPrintAble = 1,
            IsVisible = 1,
            IsActive = 1,
            StoreProcedureName = N'app.pvt_SalesTransaction',
            StatusId = 1
        WHERE ProgramCode = @ProgramCode;
    END

    SELECT @ProgramId = ProgramId FROM [core].[Programs] WHERE ProgramCode = @ProgramCode;

    IF NOT EXISTS (
        SELECT 1 FROM [core].[ReportDefinition]
        WHERE ProgramId = @ProgramId AND DefinitionKey = @DefinitionKey AND CompanyId IS NULL)
    BEGIN
        INSERT INTO [core].[ReportDefinition]
        (ReportDefinitionGuid, ProgramId, ReportScope, CompanyId, ReportKind, DefinitionKey, RendererType,
         FilePath, StoreProcedureName, LayoutJson, IsTracked, RequiresPrintId, PrintIdPolicy, PrintIdPrefix,
         [Version], IsActive, StatusId, CreatedById, CreatedTime)
        VALUES
        (NEWID(), @ProgramId, N'Standard', NULL, N'PVT', @DefinitionKey, @RendererType,
         @FilePath, N'app.pvt_SalesTransaction', @LayoutJson, 0, 0, N'PerRun', N'SA',
         1, 1, 1, @CreatedById, SYSDATETIME());
    END
    ELSE
    BEGIN
        UPDATE [core].[ReportDefinition]
        SET RendererType = @RendererType,
            FilePath = @FilePath,
            StoreProcedureName = N'app.pvt_SalesTransaction',
            LayoutJson = @LayoutJson,
            IsTracked = 0,
            RequiresPrintId = 0,
            PrintIdPolicy = N'PerRun',
            PrintIdPrefix = N'SA',
            IsActive = 1,
            StatusId = 1
        WHERE ProgramId = @ProgramId
          AND DefinitionKey = @DefinitionKey
          AND CompanyId IS NULL;
    END

    SELECT @ReportDefinitionId = ReportDefinitionId
    FROM [core].[ReportDefinition]
    WHERE ProgramId = @ProgramId AND DefinitionKey = @DefinitionKey AND CompanyId IS NULL;

    DELETE FROM [core].[ReportParameter] WHERE ReportDefinitionId = @ReportDefinitionId;

    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[core].[ReportParameter]') AND name = N'ColumnGroup')
    BEGIN
        INSERT INTO [core].[ReportParameter]
        (ReportDefinitionId, ParameterName, DisplayLabel, DataType, IsRequired, SortOrder,
         LookupType, IsSensitive, StatusId, CreatedById, CreatedTime,
         FieldKey, ControlType, ColumnGroup, ColumnSpan, RowGroup)
        VALUES
        (@ReportDefinitionId, N'CompanyId', N'Company', N'string', 1, 10, NULL, 0, 1, @CreatedById, SYSDATETIME(), N'Company', N'ReadonlyText', 1, 12, N'Scope'),
        (@ReportDefinitionId, N'CompanyOfficeId', N'Office', N'string', 1, 20, NULL, 0, 1, @CreatedById, SYSDATETIME(), N'Office', N'ReadonlyText', 1, 12, N'Scope'),
        (@ReportDefinitionId, N'DateFrom', N'Valid From', N'DateTime', 1, 10, NULL, 0, 1, @CreatedById, SYSDATETIME(), N'DateFrom', N'DatePicker', 2, 6, N'Period'),
        (@ReportDefinitionId, N'DateTo', N'Valid To', N'DateTime', 1, 20, NULL, 0, 1, @CreatedById, SYSDATETIME(), N'DateTo', N'DatePicker', 2, 6, N'Period');
    END
    ELSE
    BEGIN
        INSERT INTO [core].[ReportParameter]
        (ReportDefinitionId, ParameterName, DisplayLabel, DataType, IsRequired, SortOrder,
         LookupType, IsSensitive, StatusId, CreatedById, CreatedTime)
        VALUES
        (@ReportDefinitionId, N'CompanyId', N'Company', N'string', 1, 10, NULL, 0, 1, @CreatedById, SYSDATETIME()),
        (@ReportDefinitionId, N'CompanyOfficeId', N'Office', N'string', 1, 20, NULL, 0, 1, @CreatedById, SYSDATETIME()),
        (@ReportDefinitionId, N'DateFrom', N'Valid From', N'DateTime', 1, 10, NULL, 0, 1, @CreatedById, SYSDATETIME()),
        (@ReportDefinitionId, N'DateTo', N'Valid To', N'DateTime', 1, 20, NULL, 0, 1, @CreatedById, SYSDATETIME());
    END

    INSERT INTO [core].[UserGroupProgram]
    (UserGroupProgramGuid, UserGroupId, ProgramsId, IsUserGroupViewAble, IsUserGroupAddAble,
     IsUserGroupEditAble, IsUserGroupDeleteAble, IsUserGroupApprovalAble, IsUserGroupUnApprovalAble,
     IsUserGroupVoidAble, IsUserGroupUnVoidAble, IsUserGroupExportAble, StatusId, CreatedById, CreatedTime)
    SELECT NEWID(), 1, @ProgramId, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, @CreatedById, SYSDATETIME()
    WHERE NOT EXISTS (
        SELECT 1 FROM [core].[UserGroupProgram]
        WHERE UserGroupId = 1 AND ProgramsId = @ProgramId);

    FETCH NEXT FROM engine_cursor INTO @RowIndex, @ProgramCode, @ProgramName, @DefinitionKey, @RendererType, @FilePath;
END

CLOSE engine_cursor;
DEALLOCATE engine_cursor;

PRINT 'SalesTransaction pivot engines seeded (PVT.01.001 - PVT.01.005).';
GO

-- Verify registration for Pivot Selection (?tp=pvt)
SELECT p.ProgramCode, p.ProgramName, p.ProgramType, p.IsActive, p.IsVisible,
       rd.DefinitionKey, rd.RendererType, rd.FilePath, rd.StoreProcedureName
FROM [core].[Programs] p
INNER JOIN [core].[ReportDefinition] rd ON rd.ProgramId = p.ProgramId AND rd.ReportKind = N'PVT' AND rd.CompanyId IS NULL
WHERE p.ProgramCode LIKE N'PVT.01.00%'
ORDER BY p.ProgramCode;
GO
