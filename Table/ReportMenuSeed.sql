USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

SET NOCOUNT ON;

DECLARE @CreatedById BIGINT = 1;
DECLARE @ReportsMenuId BIGINT;
DECLARE @ReportSelectionId BIGINT;

IF NOT EXISTS (SELECT 1 FROM [core].[Programs] WHERE ProgramCode = N'96.00.00')
BEGIN
    INSERT INTO [core].[Programs]
    (ProgramGuid, ProgramCode, ProgramName, RootLevel, RowIndex, ProgramType, IsProgramViewAble, IsProgramPrintAble,
     IsVisible, IsActive, NavigateUrl, IconCode, CreatedById, CreatedTime, StatusId)
    VALUES
    (NEWID(), N'96.00.00', N'Reports', 1, 1, N'MNU', 1, 0, 1, 1, NULL, N'Assessment', @CreatedById, SYSDATETIME(), 1);
END

SELECT @ReportsMenuId = ProgramId FROM [core].[Programs] WHERE ProgramCode = N'96.00.00';

IF NOT EXISTS (SELECT 1 FROM [core].[Programs] WHERE ProgramCode = N'96.00.01')
BEGIN
    INSERT INTO [core].[Programs]
    (ProgramGuid, ProgramCode, ProgramName, ParentId, RootLevel, RowIndex, ProgramType, IsProgramViewAble, IsProgramPrintAble,
     IsVisible, IsActive, NavigateUrl, IconCode, CreatedById, CreatedTime, StatusId)
    VALUES
    (NEWID(), N'96.00.01', N'Report Selection', @ReportsMenuId, 2, 1, N'PRG', 1, 1, 1, 1,
     N'/app/reportselection?tp=rpt', N'ListAlt', @CreatedById, SYSDATETIME(), 1);
END

IF NOT EXISTS (SELECT 1 FROM [core].[Programs] WHERE ProgramCode = N'96.00.02')
BEGIN
    INSERT INTO [core].[Programs]
    (ProgramGuid, ProgramCode, ProgramName, ParentId, RootLevel, RowIndex, ProgramType, IsProgramViewAble, IsProgramPrintAble,
     IsVisible, IsActive, NavigateUrl, IconCode, CreatedById, CreatedTime, StatusId)
    VALUES
    (NEWID(), N'96.00.02', N'Pivot Selection', @ReportsMenuId, 2, 2, N'PRG', 1, 1, 1, 1,
     N'/app/reportselection?tp=pvt', N'PivotTableChart', @CreatedById, SYSDATETIME(), 1);
END

DECLARE @EmployeeListId BIGINT;
IF NOT EXISTS (SELECT 1 FROM [core].[Programs] WHERE ProgramCode = N'96.01.01')
BEGIN
    INSERT INTO [core].[Programs]
    (ProgramGuid, ProgramCode, ProgramName, ParentId, RootLevel, RowIndex, ProgramType, IsProgramViewAble, IsProgramPrintAble,
     IsVisible, IsActive, StoreProcedureName, CreatedById, CreatedTime, StatusId)
    VALUES
    (NEWID(), N'96.01.01', N'Employee List Report', @ReportsMenuId, 2, 3, N'RPT', 1, 1, 1, 1,
     N'app.rpt_EmployeeList', @CreatedById, SYSDATETIME(), 1);
END

SELECT @EmployeeListId = ProgramId FROM [core].[Programs] WHERE ProgramCode = N'96.01.01';

DECLARE @SalarySlipId BIGINT;
IF NOT EXISTS (SELECT 1 FROM [core].[Programs] WHERE ProgramCode = N'96.01.02')
BEGIN
    INSERT INTO [core].[Programs]
    (ProgramGuid, ProgramCode, ProgramName, ParentId, RootLevel, RowIndex, ProgramType, IsProgramViewAble, IsProgramPrintAble,
     IsVisible, IsActive, StoreProcedureName, CreatedById, CreatedTime, StatusId)
    VALUES
    (NEWID(), N'96.01.02', N'Salary Slip', @ReportsMenuId, 2, 4, N'RPT', 1, 1, 1, 1,
     N'app.rpt_SalarySlip', @CreatedById, SYSDATETIME(), 1);
END

SELECT @SalarySlipId = ProgramId FROM [core].[Programs] WHERE ProgramCode = N'96.01.02';

IF NOT EXISTS (SELECT 1 FROM [core].[Programs] WHERE ProgramCode = N'96.02.01')
BEGIN
    INSERT INTO [core].[Programs]
    (ProgramGuid, ProgramCode, ProgramName, ParentId, RootLevel, RowIndex, ProgramType, IsProgramViewAble, IsProgramPrintAble,
     IsVisible, IsActive, StoreProcedureName, CreatedById, CreatedTime, StatusId)
    VALUES
    (NEWID(), N'96.02.01', N'Payroll Summary Pivot', @ReportsMenuId, 2, 5, N'PVT', 1, 1, 1, 1,
     N'app.pvt_PayrollSummary', @CreatedById, SYSDATETIME(), 1);
END

DECLARE @PayrollPivotId BIGINT = (SELECT ProgramId FROM [core].[Programs] WHERE ProgramCode = N'96.02.01');

IF @EmployeeListId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [core].[ReportDefinition] WHERE ProgramId = @EmployeeListId AND ReportKind = N'RPT' AND CompanyId IS NULL)
BEGIN
    INSERT INTO [core].[ReportDefinition]
    (ReportDefinitionGuid, ProgramId, ReportScope, CompanyId, ReportKind, DefinitionKey, FilePath, StoreProcedureName,
     RequiresPrintId, PrintIdPolicy, PrintIdPrefix, [Version], IsActive, StatusId, CreatedById, CreatedTime)
    VALUES
    (NEWID(), @EmployeeListId, N'Standard', NULL, N'RPT', N'EmployeeList', N'Rpt/Standard/EmployeeList.rdlc', N'app.rpt_EmployeeList',
     0, N'PerRun', N'EL', 1, 1, 1, @CreatedById, SYSDATETIME());
END

IF @SalarySlipId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [core].[ReportDefinition] WHERE ProgramId = @SalarySlipId AND ReportKind = N'RPT' AND CompanyId IS NULL)
BEGIN
    INSERT INTO [core].[ReportDefinition]
    (ReportDefinitionGuid, ProgramId, ReportScope, CompanyId, ReportKind, DefinitionKey, FilePath, StoreProcedureName,
     RequiresPrintId, PrintIdPolicy, PrintIdPrefix, [Version], IsActive, StatusId, CreatedById, CreatedTime)
    VALUES
    (NEWID(), @SalarySlipId, N'Standard', NULL, N'RPT', N'SalarySlip', N'Rpt/Standard/SalarySlip.rdlc', N'app.rpt_SalarySlip',
     1, N'PerEmployeePerPeriod', N'SS', 1, 1, 1, @CreatedById, SYSDATETIME());
END

IF @PayrollPivotId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [core].[ReportDefinition] WHERE ProgramId = @PayrollPivotId AND ReportKind = N'PVT' AND CompanyId IS NULL)
BEGIN
    INSERT INTO [core].[ReportDefinition]
    (ReportDefinitionGuid, ProgramId, ReportScope, CompanyId, ReportKind, DefinitionKey, FilePath, StoreProcedureName,
     RequiresPrintId, PrintIdPolicy, PrintIdPrefix, [Version], IsActive, StatusId, CreatedById, CreatedTime)
    VALUES
    (NEWID(), @PayrollPivotId, N'Standard', NULL, N'PVT', N'PayrollSummary', N'Pvt/Standard/PayrollSummary.pvt', N'app.pvt_PayrollSummary',
     0, N'PerRun', N'PV', 1, 1, 1, @CreatedById, SYSDATETIME());
END

DECLARE @AdminUserGroupId BIGINT = 1;

INSERT INTO [core].[UserGroupProgram]
(UserGroupProgramGuid, UserGroupId, ProgramsId, IsUserGroupViewAble, IsUserGroupAddAble, IsUserGroupEditAble, IsUserGroupDeleteAble,
 IsUserGroupApprovalAble, IsUserGroupUnApprovalAble, IsUserGroupVoidAble, IsUserGroupUnVoidAble, IsUserGroupExportAble,
 StatusId, CreatedById, CreatedTime)
SELECT NEWID(), @AdminUserGroupId, p.ProgramId, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, @CreatedById, SYSDATETIME()
FROM [core].[Programs] p
WHERE p.ProgramCode LIKE N'96.%'
  AND NOT EXISTS (
      SELECT 1 FROM [core].[UserGroupProgram] ugp
      WHERE ugp.UserGroupId = @AdminUserGroupId AND ugp.ProgramsId = p.ProgramId
  );

PRINT 'Report menu and sample programs seeded.';
GO
