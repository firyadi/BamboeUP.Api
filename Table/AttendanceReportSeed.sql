USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    AUTO-GENERATED — Phase 4.1 Report seed for Attendance
    Table: [app].[Attendance]
    ProgramCode: 80.01
    ParameterProfile: standard
    HeaderProfile: minimal
    LayoutPreset: a4-portrait
    DataSourceType: table
    IsTracked: false
    GenerateWithPrintId: false
    Run ReportMenuSeed.sql first if menu 96.00.00 is missing.
*/

SET NOCOUNT ON;

DECLARE @CreatedById BIGINT = 1;
DECLARE @ProgramId BIGINT;
DECLARE @ReportDefinitionId BIGINT;
DECLARE @ReportsMenuId BIGINT;

PRINT '=== Seed: Attendance (Attendance Report) ==='

SELECT @ReportsMenuId = ProgramId FROM [core].[Programs] WHERE ProgramCode = N'96.00.00';

IF @ReportsMenuId IS NULL
BEGIN
    RAISERROR('Reports menu 96.00.00 not found. Run ReportMenuSeed.sql first.', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM [core].[Programs] WHERE ProgramCode = N'80.01')
BEGIN
    INSERT INTO [core].[Programs]
    (ProgramGuid, ProgramCode, ProgramName, ParentId, RootLevel, RowIndex, ProgramType,
     IsProgramViewAble, IsProgramPrintAble, IsVisible, IsActive, StoreProcedureName,
     CreatedById, CreatedTime, StatusId)
    VALUES
    (NEWID(), N'80.01', N'Attendance Report', @ReportsMenuId, 2,
     (SELECT ISNULL(MAX(RowIndex), 0) + 1 FROM [core].[Programs] WHERE ParentId = @ReportsMenuId),
     N'RPT', 1, 1, 1, 1, N'app.rpt_Attendance', @CreatedById, SYSDATETIME(), 1);
END

SELECT @ProgramId = ProgramId FROM [core].[Programs] WHERE ProgramCode = N'80.01';

IF COL_LENGTH(N'[core].[ReportDefinition]', N'RendererType') IS NULL
BEGIN
    ALTER TABLE [core].[ReportDefinition]
        ADD [RendererType] NVARCHAR(30) NULL;
END

IF COL_LENGTH(N'[core].[ReportDefinition]', N'LayoutJson') IS NULL
BEGIN
    ALTER TABLE [core].[ReportDefinition]
        ADD [LayoutJson] NVARCHAR(MAX) NULL;
END

IF NOT EXISTS (SELECT 1 FROM [core].[ReportDefinition]
               WHERE ProgramId = @ProgramId AND DefinitionKey = N'Attendance' AND CompanyId IS NULL)
BEGIN
    INSERT INTO [core].[ReportDefinition]
    (ReportDefinitionGuid, ProgramId, ReportScope, CompanyId, ReportKind, DefinitionKey, RendererType,
     FilePath, StoreProcedureName, LayoutJson, IsTracked, RequiresPrintId, PrintIdPolicy, PrintIdPrefix,
     [Version], IsActive, StatusId, CreatedById, CreatedTime)
    VALUES
    (NEWID(), @ProgramId, N'Standard', NULL, N'RPT', N'Attendance', N'FastReport',
     N'Rpt/Standard/Attendance.frx', N'app.rpt_Attendance', N'{"paper":"A4","orientation":"Portrait","marginPreset":"Normal","header":{"profile":"Minimal","showCompanyLogo":false,"showCompanyName":false,"showOfficeName":false,"showPrintedAt":false}}', 0, 0, N'PerRun', N'AT',
     1, 1, 1, @CreatedById, SYSDATETIME());
END
ELSE
BEGIN
    UPDATE [core].[ReportDefinition]
    SET RendererType = N'FastReport',
        FilePath = N'Rpt/Standard/Attendance.frx',
        StoreProcedureName = N'app.rpt_Attendance',
        LayoutJson = N'{"paper":"A4","orientation":"Portrait","marginPreset":"Normal","header":{"profile":"Minimal","showCompanyLogo":false,"showCompanyName":false,"showOfficeName":false,"showPrintedAt":false}}',
        IsTracked = 0,
        RequiresPrintId = 0,
        PrintIdPolicy = N'PerRun',
        PrintIdPrefix = N'AT',
        IsActive = 1,
        StatusId = 1
    WHERE ProgramId = @ProgramId
      AND DefinitionKey = N'Attendance'
      AND CompanyId IS NULL;
END

SELECT @ReportDefinitionId = ReportDefinitionId
FROM [core].[ReportDefinition]
WHERE ProgramId = @ProgramId AND DefinitionKey = N'Attendance' AND CompanyId IS NULL;

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
    PRINT '  -> WARNING: Run ReportParameter_Alter_Phase3.sql for ColumnGroup/ControlType columns.';
END

INSERT INTO [core].[UserGroupProgram]
(UserGroupProgramGuid, UserGroupId, ProgramsId, IsUserGroupViewAble, IsUserGroupAddAble,
 IsUserGroupEditAble, IsUserGroupDeleteAble, IsUserGroupApprovalAble, IsUserGroupUnApprovalAble,
 IsUserGroupVoidAble, IsUserGroupUnVoidAble, IsUserGroupExportAble, StatusId, CreatedById, CreatedTime)
SELECT NEWID(), 1, @ProgramId, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, @CreatedById, SYSDATETIME()
WHERE NOT EXISTS (SELECT 1 FROM [core].[UserGroupProgram] WHERE UserGroupId = 1 AND ProgramsId = @ProgramId);

PRINT 'Attendance report + parameter mapping seeded.';
GO