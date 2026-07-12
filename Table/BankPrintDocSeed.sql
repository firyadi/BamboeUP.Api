USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

SET NOCOUNT ON;

/*
    Phase 3.9 — Bank toolbar print slips (DOC)
    Parent form: 02.09.04 Bank Detail (PRG)
    Slip 1: 02.09.04.01 — Bank Master Slip
    Slip 2: 02.09.04.02 — Bank Confirmation Letter
*/

DECLARE @CreatedById BIGINT = 1;
DECLARE @BankFormProgramId BIGINT;
DECLARE @Slip1Id BIGINT;
DECLARE @Slip2Id BIGINT;
DECLARE @AdminUserGroupId BIGINT = 1;

SELECT @BankFormProgramId = ProgramId
FROM [core].[Programs]
WHERE ProgramCode = N'02.09.04';

IF @BankFormProgramId IS NULL
BEGIN
    RAISERROR('Parent program 02.09.04 not found. Seed Bank menu/program first.', 16, 1);
    RETURN;
END

-- Ensure parent form allows toolbar print
UPDATE [core].[Programs]
SET IsProgramPrintAble = 1
WHERE ProgramId = @BankFormProgramId;

IF NOT EXISTS (SELECT 1 FROM [core].[Programs] WHERE ProgramCode = N'02.09.04.01')
BEGIN
    INSERT INTO [core].[Programs]
    (ProgramGuid, ProgramCode, ProgramName, ParentId, RootLevel, RowIndex, ProgramType,
     IsProgramViewAble, IsProgramPrintAble, IsVisible, IsActive, CreatedById, CreatedTime, StatusId)
    VALUES
    (NEWID(), N'02.09.04.01', N'Bank Master Slip (QuestPDF)', @BankFormProgramId, 3, 1, N'DOC',
     1, 1, 0, 1, @CreatedById, SYSDATETIME(), 1);
END

IF NOT EXISTS (SELECT 1 FROM [core].[Programs] WHERE ProgramCode = N'02.09.04.02')
BEGIN
    INSERT INTO [core].[Programs]
    (ProgramGuid, ProgramCode, ProgramName, ParentId, RootLevel, RowIndex, ProgramType,
     IsProgramViewAble, IsProgramPrintAble, IsVisible, IsActive, CreatedById, CreatedTime, StatusId)
    VALUES
    (NEWID(), N'02.09.04.02', N'Bank Confirmation Letter', @BankFormProgramId, 3, 2, N'DOC',
     1, 1, 0, 1, @CreatedById, SYSDATETIME(), 1);
END

SELECT @Slip1Id = ProgramId FROM [core].[Programs] WHERE ProgramCode = N'02.09.04.01';
SELECT @Slip2Id = ProgramId FROM [core].[Programs] WHERE ProgramCode = N'02.09.04.02';

IF @Slip1Id IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [core].[ReportDefinition]
    WHERE ProgramId = @Slip1Id AND ReportKind = N'DOC' AND CompanyId IS NULL)
BEGIN
    INSERT INTO [core].[ReportDefinition]
    (ReportDefinitionGuid, ProgramId, ReportScope, CompanyId, ReportKind, DefinitionKey, RendererType, FilePath, StoreProcedureName,
     IsTracked, RequiresPrintId, PrintIdPolicy, PrintIdPrefix, [Version], IsActive, StatusId, CreatedById, CreatedTime)
    VALUES
    (NEWID(), @Slip1Id, N'Standard', NULL, N'DOC', N'BankMasterSlip_QuestPDF', N'QuestPDF', NULL, N'app.doc_BankMasterSlip',
     1, 0, N'PerRun', N'BK', 1, 1, 1, @CreatedById, SYSDATETIME());
END

IF @Slip2Id IS NOT NULL AND NOT EXISTS (
    SELECT 1 FROM [core].[ReportDefinition]
    WHERE ProgramId = @Slip2Id AND ReportKind = N'DOC' AND CompanyId IS NULL)
BEGIN
    INSERT INTO [core].[ReportDefinition]
    (ReportDefinitionGuid, ProgramId, ReportScope, CompanyId, ReportKind, DefinitionKey, RendererType, FilePath, StoreProcedureName,
     IsTracked, RequiresPrintId, PrintIdPolicy, PrintIdPrefix, [Version], IsActive, StatusId, CreatedById, CreatedTime)
    VALUES
    (NEWID(), @Slip2Id, N'Standard', NULL, N'DOC', N'BankConfirmationLetter', N'QuestPDF', N'Doc/Standard/BankConfirmationLetter.frx', N'app.doc_BankConfirmationLetter',
     1, 1, N'PerDocument', N'BC', 1, 1, 1, @CreatedById, SYSDATETIME());
END

INSERT INTO [core].[UserGroupProgram]
(UserGroupProgramGuid, UserGroupId, ProgramsId, IsUserGroupViewAble, IsUserGroupAddAble, IsUserGroupEditAble, IsUserGroupDeleteAble,
 IsUserGroupApprovalAble, IsUserGroupUnApprovalAble, IsUserGroupVoidAble, IsUserGroupUnVoidAble, IsUserGroupExportAble,
 StatusId, CreatedById, CreatedTime)
SELECT NEWID(), @AdminUserGroupId, p.ProgramId, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, @CreatedById, SYSDATETIME()
FROM [core].[Programs] p
WHERE p.ProgramCode IN (N'02.09.04.01', N'02.09.04.02')
  AND NOT EXISTS (
      SELECT 1 FROM [core].[UserGroupProgram] ugp
      WHERE ugp.UserGroupId = @AdminUserGroupId AND ugp.ProgramsId = p.ProgramId
  );

PRINT 'Bank DOC print slips seeded (02.09.04.01, 02.09.04.02).';
GO
