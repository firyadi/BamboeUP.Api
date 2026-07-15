USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    AUTO-GENERATED — Phase 4.1 Report seed for Hospital
    Table: [app].[Hospital]
    ProgramCode: RPT.02.09.20
    ParameterProfile: none
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

PRINT '=== Seed: Hospital (Hospital Report) ==='

SELECT @ReportsMenuId = ProgramId FROM [core].[Programs] WHERE ProgramCode = N'96.00.00';

IF @ReportsMenuId IS NULL
BEGIN
    RAISERROR('Reports menu 96.00.00 not found. Run ReportMenuSeed.sql first.', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM [core].[Programs] WHERE ProgramCode = N'RPT.02.09.20')
BEGIN
    INSERT INTO [core].[Programs]
    (ProgramGuid, ProgramCode, ProgramName, ParentId, RootLevel, RowIndex, ProgramType,
     IsProgramViewAble, IsProgramPrintAble, IsVisible, IsActive, StoreProcedureName,
     CreatedById, CreatedTime, StatusId)
    VALUES
    (NEWID(), N'RPT.02.09.20', N'Hospital Report', @ReportsMenuId, 2,
     (SELECT ISNULL(MAX(RowIndex), 0) + 1 FROM [core].[Programs] WHERE ParentId = @ReportsMenuId),
     N'RPT', 1, 1, 1, 1, N'app.rpt_Hospital', @CreatedById, SYSDATETIME(), 1);
END

SELECT @ProgramId = ProgramId FROM [core].[Programs] WHERE ProgramCode = N'RPT.02.09.20';

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
               WHERE ProgramId = @ProgramId AND DefinitionKey = N'Hospital' AND CompanyId IS NULL)
BEGIN
    INSERT INTO [core].[ReportDefinition]
    (ReportDefinitionGuid, ProgramId, ReportScope, CompanyId, ReportKind, DefinitionKey, RendererType,
     FilePath, StoreProcedureName, LayoutJson, IsTracked, RequiresPrintId, PrintIdPolicy, PrintIdPrefix,
     [Version], IsActive, StatusId, CreatedById, CreatedTime)
    VALUES
    (NEWID(), @ProgramId, N'Standard', NULL, N'RPT', N'Hospital', N'FastReport',
     N'Rpt/Standard/Hospital.frx', N'app.rpt_Hospital', N'{"paper":"A4","orientation":"Portrait","marginPreset":"Normal","header":{"profile":"Minimal","showCompanyLogo":false,"showCompanyName":false,"showOfficeName":false,"showPrintedAt":false}}', 0, 0, N'PerRun', N'HO',
     1, 1, 1, @CreatedById, SYSDATETIME());
END
ELSE
BEGIN
    UPDATE [core].[ReportDefinition]
    SET RendererType = N'FastReport',
        FilePath = N'Rpt/Standard/Hospital.frx',
        StoreProcedureName = N'app.rpt_Hospital',
        LayoutJson = N'{"paper":"A4","orientation":"Portrait","marginPreset":"Normal","header":{"profile":"Minimal","showCompanyLogo":false,"showCompanyName":false,"showOfficeName":false,"showPrintedAt":false}}',
        IsTracked = 0,
        RequiresPrintId = 0,
        PrintIdPolicy = N'PerRun',
        PrintIdPrefix = N'HO',
        IsActive = 1,
        StatusId = 1
    WHERE ProgramId = @ProgramId
      AND DefinitionKey = N'Hospital'
      AND CompanyId IS NULL;
END

SELECT @ReportDefinitionId = ReportDefinitionId
FROM [core].[ReportDefinition]
WHERE ProgramId = @ProgramId AND DefinitionKey = N'Hospital' AND CompanyId IS NULL;

DELETE FROM [core].[ReportParameter] WHERE ReportDefinitionId = @ReportDefinitionId;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[core].[ReportParameter]') AND name = N'ColumnGroup')
BEGIN
    -- No ReportParameter rows (profile: None)
END
ELSE
BEGIN
    -- No ReportParameter rows (profile: None)
    PRINT '  -> WARNING: Run ReportParameter_Alter_Phase3.sql for ColumnGroup/ControlType columns.';
END

INSERT INTO [core].[UserGroupProgram]
(UserGroupProgramGuid, UserGroupId, ProgramsId, IsUserGroupViewAble, IsUserGroupAddAble,
 IsUserGroupEditAble, IsUserGroupDeleteAble, IsUserGroupApprovalAble, IsUserGroupUnApprovalAble,
 IsUserGroupVoidAble, IsUserGroupUnVoidAble, IsUserGroupExportAble, StatusId, CreatedById, CreatedTime)
SELECT NEWID(), 1, @ProgramId, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, @CreatedById, SYSDATETIME()
WHERE NOT EXISTS (SELECT 1 FROM [core].[UserGroupProgram] WHERE UserGroupId = 1 AND ProgramsId = @ProgramId);

PRINT 'Hospital report + parameter mapping seeded.';
GO