USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

SET NOCOUNT ON;

DECLARE @CreatedById BIGINT = 1;
DECLARE @ProgramId BIGINT;
DECLARE @ReportDefinitionId BIGINT;
DECLARE @ReportsMenuId BIGINT;

PRINT '=== Seed: RptEng (Engineering CV Report) ==='

SELECT @ReportsMenuId = ProgramId FROM [core].[Programs] WHERE ProgramCode = N'96.00.00';

IF @ReportsMenuId IS NULL
BEGIN
    RAISERROR('Reports menu 96.00.00 not found. Run ReportMenuSeed.sql first.', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM [core].[Programs] WHERE ProgramCode = N'96.01.03')
BEGIN
    INSERT INTO [core].[Programs]
    (ProgramGuid, ProgramCode, ProgramName, ParentId, RootLevel, RowIndex, ProgramType,
     IsProgramViewAble, IsProgramPrintAble, IsVisible, IsActive, StoreProcedureName,
     CreatedById, CreatedTime, StatusId)
    VALUES
    (NEWID(), N'96.01.03', N'Engineering CV Report', @ReportsMenuId, 2, 6, N'RPT',
     1, 1, 1, 1, N'app.rpt_EngineeringCv', @CreatedById, SYSDATETIME(), 1);
END

SELECT @ProgramId = ProgramId FROM [core].[Programs] WHERE ProgramCode = N'96.01.03';

IF NOT EXISTS (SELECT 1 FROM [core].[ReportDefinition]
               WHERE ProgramId = @ProgramId AND DefinitionKey = N'RptEng' AND CompanyId IS NULL)
BEGIN
    INSERT INTO [core].[ReportDefinition]
    (ReportDefinitionGuid, ProgramId, ReportScope, CompanyId, ReportKind, DefinitionKey,
     FilePath, StoreProcedureName, RequiresPrintId, PrintIdPolicy, PrintIdPrefix,
     [Version], IsActive, StatusId, CreatedById, CreatedTime)
    VALUES
    (NEWID(), @ProgramId, N'Standard', NULL, N'RPT', N'RptEng',
     N'Rpt/Standard/RptEng.rdlc', N'app.rpt_EngineeringCv', 0, N'PerRun', N'EN',
     1, 1, 1, @CreatedById, SYSDATETIME());
END

SELECT @ReportDefinitionId = ReportDefinitionId
FROM [core].[ReportDefinition]
WHERE ProgramId = @ProgramId AND DefinitionKey = N'RptEng' AND CompanyId IS NULL;

DELETE FROM [core].[ReportParameter] WHERE ReportDefinitionId = @ReportDefinitionId;

-- Requires Phase 3 ALTER for extended columns; fallback insert uses base columns only if ALTER not run
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[core].[ReportParameter]') AND name = N'ColumnGroup')
BEGIN
    INSERT INTO [core].[ReportParameter]
    (ReportDefinitionId, ParameterName, DisplayLabel, DataType, IsRequired, SortOrder,
     LookupType, IsSensitive, StatusId, CreatedById, CreatedTime,
     FieldKey, ControlType, ColumnGroup, ColumnSpan, RowGroup)
    VALUES
    (@ReportDefinitionId, N'CompanyId', N'Company', N'string', 1, 10, NULL, 0, 1, @CreatedById, SYSDATETIME(),
     N'Company', N'ReadonlyText', 1, 12, N'Scope'),
    (@ReportDefinitionId, N'CompanyOfficeId', N'Office', N'string', 1, 20, NULL, 0, 1, @CreatedById, SYSDATETIME(),
     N'Office', N'ReadonlyText', 1, 12, N'Scope'),
    (@ReportDefinitionId, N'DateFrom', N'Valid From', N'DateTime', 1, 10, NULL, 0, 1, @CreatedById, SYSDATETIME(),
     N'DateFrom', N'DatePicker', 2, 6, N'Period'),
    (@ReportDefinitionId, N'DateTo', N'Valid To', N'DateTime', 1, 20, NULL, 0, 1, @CreatedById, SYSDATETIME(),
     N'DateTo', N'DatePicker', 2, 6, N'Period'),
    (@ReportDefinitionId, N'EmployeeId', N'Employee', N'long', 0, 10, N'Employee', 0, 1, @CreatedById, SYSDATETIME(),
     N'EmployeeId', N'ComboBox', 3, 12, N'Filter'),
    (@ReportDefinitionId, N'DepartmentId', N'Department', N'long', 0, 20, N'OrganizationUnit', 0, 1, @CreatedById, SYSDATETIME(),
     N'DepartmentId', N'ComboBox', 3, 12, N'Filter');
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
    (@ReportDefinitionId, N'DateTo', N'Valid To', N'DateTime', 1, 20, NULL, 0, 1, @CreatedById, SYSDATETIME()),
    (@ReportDefinitionId, N'EmployeeId', N'Employee', N'long', 0, 10, N'Employee', 0, 1, @CreatedById, SYSDATETIME()),
    (@ReportDefinitionId, N'DepartmentId', N'Department', N'long', 0, 20, N'OrganizationUnit', 0, 1, @CreatedById, SYSDATETIME());

    PRINT '  -> WARNING: Run ReportParameter_Alter_Phase3.sql for ColumnGroup/ControlType columns.';
END

INSERT INTO [core].[UserGroupProgram]
(UserGroupProgramGuid, UserGroupId, ProgramsId, IsUserGroupViewAble, IsUserGroupAddAble,
 IsUserGroupEditAble, IsUserGroupDeleteAble, IsUserGroupApprovalAble, IsUserGroupUnApprovalAble,
 IsUserGroupVoidAble, IsUserGroupUnVoidAble, IsUserGroupExportAble, StatusId, CreatedById, CreatedTime)
SELECT NEWID(), 1, @ProgramId, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, @CreatedById, SYSDATETIME()
WHERE NOT EXISTS (SELECT 1 FROM [core].[UserGroupProgram] WHERE UserGroupId = 1 AND ProgramsId = @ProgramId);

PRINT 'RptEng report + parameter mapping seeded.';
GO
