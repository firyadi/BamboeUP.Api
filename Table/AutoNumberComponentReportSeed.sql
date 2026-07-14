USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    AUTO-GENERATED — Phase 4.1 Report seed for AutoNumberComponent
    Table: [app].[AutoNumberComponent]
    ProgramCode: 96.01.06
    ParameterProfile: none
    HeaderProfile: letterhead
    LayoutPreset: a4-portrait
    MarginPreset: normal
    GenerateWithPrintId: false
    Run ReportMenuSeed.sql first if menu 96.00.00 is missing.
*/

SET NOCOUNT ON;

DECLARE @CreatedById BIGINT = 1;
DECLARE @ProgramId BIGINT;
DECLARE @ReportDefinitionId BIGINT;
DECLARE @ReportsMenuId BIGINT;

PRINT '=== Seed: AutoNumberComponent (AutoNumberComponent Report) ==='

SELECT @ReportsMenuId = ProgramId FROM [core].[Programs] WHERE ProgramCode = N'96.00.00';

IF @ReportsMenuId IS NULL
BEGIN
    RAISERROR('Reports menu 96.00.00 not found. Run ReportMenuSeed.sql first.', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM [core].[Programs] WHERE ProgramCode = N'96.01.06')
BEGIN
    INSERT INTO [core].[Programs]
    (ProgramGuid, ProgramCode, ProgramName, ParentId, RootLevel, RowIndex, ProgramType,
     IsProgramViewAble, IsProgramPrintAble, IsVisible, IsActive, StoreProcedureName,
     CreatedById, CreatedTime, StatusId)
    VALUES
    (NEWID(), N'96.01.06', N'AutoNumberComponent Report', @ReportsMenuId, 2,
     (SELECT ISNULL(MAX(RowIndex), 0) + 1 FROM [core].[Programs] WHERE ParentId = @ReportsMenuId),
     N'RPT', 1, 1, 1, 1, N'app.rpt_AutoNumberComponent', @CreatedById, SYSDATETIME(), 1);
END

SELECT @ProgramId = ProgramId FROM [core].[Programs] WHERE ProgramCode = N'96.01.06';

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
               WHERE ProgramId = @ProgramId AND DefinitionKey = N'AutoNumberComponent' AND CompanyId IS NULL)
BEGIN
    INSERT INTO [core].[ReportDefinition]
    (ReportDefinitionGuid, ProgramId, ReportScope, CompanyId, ReportKind, DefinitionKey, RendererType,
     FilePath, StoreProcedureName, LayoutJson, IsTracked, RequiresPrintId, PrintIdPolicy, PrintIdPrefix,
     [Version], IsActive, StatusId, CreatedById, CreatedTime)
    VALUES
    (NEWID(), @ProgramId, N'Standard', NULL, N'RPT', N'AutoNumberComponent', N'FastReport',
     N'Rpt/Standard/AutoNumberComponent.frx', N'app.rpt_AutoNumberComponent', N'{"paper":"A4","orientation":"Portrait","marginPreset":"Normal","header":{"profile":"Letterhead","showCompanyLogo":true,"showCompanyName":true,"showOfficeName":true,"showPrintedAt":true}}', 1, 0, N'PerRun', N'AU',
     1, 1, 1, @CreatedById, SYSDATETIME());
END
ELSE
BEGIN
    UPDATE [core].[ReportDefinition]
    SET RendererType = N'FastReport',
        FilePath = N'Rpt/Standard/AutoNumberComponent.frx',
        StoreProcedureName = N'app.rpt_AutoNumberComponent',
        LayoutJson = N'{"paper":"A4","orientation":"Portrait","marginPreset":"Normal","header":{"profile":"Letterhead","showCompanyLogo":true,"showCompanyName":true,"showOfficeName":true,"showPrintedAt":true}}',
        IsTracked = 1,
        RequiresPrintId = 0,
        PrintIdPolicy = N'PerRun',
        PrintIdPrefix = N'AU',
        IsActive = 1,
        StatusId = 1
    WHERE ProgramId = @ProgramId
      AND DefinitionKey = N'AutoNumberComponent'
      AND CompanyId IS NULL;
END

SELECT @ReportDefinitionId = ReportDefinitionId
FROM [core].[ReportDefinition]
WHERE ProgramId = @ProgramId AND DefinitionKey = N'AutoNumberComponent' AND CompanyId IS NULL;

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

PRINT 'AutoNumberComponent report + parameter mapping seeded.';
GO