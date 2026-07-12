USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

SET NOCOUNT ON;

/*
    §9B — Bank Master Slip engine demo (4 renderers)
    Parent form: 02.09.04 Bank Detail (PRG)

    | ProgramCode  | ProgramName                         | RendererType | DefinitionKey              |
    |--------------|-------------------------------------|--------------|----------------------------|
    | 02.09.04.01  | Bank Master Slip (QuestPDF)         | QuestPDF     | BankMasterSlip_QuestPDF    |
    | 02.09.04.03  | Bank Master Slip (FastReport)       | FastReport   | BankMasterSlip_FastReport  |
    | 02.09.04.04  | Bank Master Slip (Telerik)          | Telerik      | BankMasterSlip_Telerik     |
    | 02.09.04.05  | Bank Master Slip (DevExpress)       | DevExpress   | BankMasterSlip_DevExpress  |

    All share SP: app.doc_BankMasterSlip
    Templates: BamboeUp.Report/Doc/Standard/EngineDemo/
*/

DECLARE @CreatedById BIGINT = 1;
DECLARE @BankFormProgramId BIGINT;
DECLARE @AdminUserGroupId BIGINT = 1;

SELECT @BankFormProgramId = ProgramId
FROM [core].[Programs]
WHERE ProgramCode = N'02.09.04';

IF @BankFormProgramId IS NULL
BEGIN
    RAISERROR('Parent program 02.09.04 not found. Seed Bank menu/program first.', 16, 1);
    RETURN;
END

UPDATE [core].[Programs]
SET IsProgramPrintAble = 1
WHERE ProgramId = @BankFormProgramId;

-- Ensure RendererType column exists (idempotent)
IF COL_LENGTH(N'[core].[ReportDefinition]', N'RendererType') IS NULL
BEGIN
    ALTER TABLE [core].[ReportDefinition]
        ADD [RendererType] NVARCHAR(30) NULL;
END

-- Upsert DOC programs (engine demo)
MERGE [core].[Programs] AS target
USING (VALUES
    (N'02.09.04.01', N'Bank Master Slip (QuestPDF)',   1),
    (N'02.09.04.03', N'Bank Master Slip (FastReport)', 3),
    (N'02.09.04.04', N'Bank Master Slip (Telerik)',    4),
    (N'02.09.04.05', N'Bank Master Slip (DevExpress)', 5)
) AS src (ProgramCode, ProgramName, RowIndex)
ON target.ProgramCode = src.ProgramCode
WHEN MATCHED THEN
    UPDATE SET
        ProgramName = src.ProgramName,
        ParentId = @BankFormProgramId,
        ProgramType = N'DOC',
        IsProgramPrintAble = 1,
        IsVisible = 0,
        IsActive = 1,
        RowIndex = src.RowIndex
WHEN NOT MATCHED THEN
    INSERT (ProgramGuid, ProgramCode, ProgramName, ParentId, RootLevel, RowIndex, ProgramType,
            IsProgramViewAble, IsProgramPrintAble, IsVisible, IsActive, CreatedById, CreatedTime, StatusId)
    VALUES (NEWID(), src.ProgramCode, src.ProgramName, @BankFormProgramId, 3, src.RowIndex, N'DOC',
            1, 1, 0, 1, @CreatedById, SYSDATETIME(), 1);

-- Upsert ReportDefinition per engine
MERGE [core].[ReportDefinition] AS target
USING (
    SELECT p.ProgramId, v.DefinitionKey, v.RendererType, v.FilePath
    FROM (VALUES
        (N'02.09.04.01', N'BankMasterSlip_QuestPDF',   N'QuestPDF',   NULL),
        (N'02.09.04.03', N'BankMasterSlip_FastReport', N'FastReport', N'Doc/Standard/EngineDemo/BankMasterSlip_FastReport.frx'),
        (N'02.09.04.04', N'BankMasterSlip_Telerik',    N'Telerik',    N'Doc/Standard/EngineDemo/BankMasterSlip_Telerik.trdx'),
        (N'02.09.04.05', N'BankMasterSlip_DevExpress', N'DevExpress', N'Doc/Standard/EngineDemo/BankMasterSlip_DevExpress.repx')
    ) AS v (ProgramCode, DefinitionKey, RendererType, FilePath)
    INNER JOIN [core].[Programs] p ON p.ProgramCode = v.ProgramCode
) AS src
ON target.ProgramId = src.ProgramId
   AND target.ReportKind = N'DOC'
   AND target.CompanyId IS NULL
   AND target.ReportScope = N'Standard'
WHEN MATCHED THEN
    UPDATE SET
        DefinitionKey = src.DefinitionKey,
        RendererType = src.RendererType,
        FilePath = src.FilePath,
        StoreProcedureName = N'app.doc_BankMasterSlip',
        IsTracked = 1,
        RequiresPrintId = 0,
        PrintIdPolicy = N'PerRun',
        PrintIdPrefix = N'BK',
        [Version] = target.[Version] + 1,
        IsActive = 1,
        UpdatedById = @CreatedById,
        UpdatedTime = SYSDATETIME()
WHEN NOT MATCHED THEN
    INSERT (ReportDefinitionGuid, ProgramId, ReportScope, CompanyId, ReportKind, DefinitionKey, RendererType, FilePath,
            StoreProcedureName, IsTracked, RequiresPrintId, PrintIdPolicy, PrintIdPrefix,
            [Version], IsActive, StatusId, CreatedById, CreatedTime)
    VALUES (NEWID(), src.ProgramId, N'Standard', NULL, N'DOC', src.DefinitionKey, src.RendererType, src.FilePath,
            N'app.doc_BankMasterSlip', 1, 0, N'PerRun', N'BK', 1, 1, 1, @CreatedById, SYSDATETIME());

-- ACL for admin user group
INSERT INTO [core].[UserGroupProgram]
(UserGroupProgramGuid, UserGroupId, ProgramsId, IsUserGroupViewAble, IsUserGroupAddAble, IsUserGroupEditAble, IsUserGroupDeleteAble,
 IsUserGroupApprovalAble, IsUserGroupUnApprovalAble, IsUserGroupVoidAble, IsUserGroupUnVoidAble, IsUserGroupExportAble,
 StatusId, CreatedById, CreatedTime)
SELECT NEWID(), @AdminUserGroupId, p.ProgramId, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, @CreatedById, SYSDATETIME()
FROM [core].[Programs] p
WHERE p.ProgramCode IN (N'02.09.04.01', N'02.09.04.03', N'02.09.04.04', N'02.09.04.05')
  AND NOT EXISTS (
      SELECT 1 FROM [core].[UserGroupProgram] ugp
      WHERE ugp.UserGroupId = @AdminUserGroupId AND ugp.ProgramsId = p.ProgramId
  );

PRINT 'Bank engine demo slips seeded: QuestPDF, FastReport, Telerik, DevExpress (02.09.04.01, .03-.05).';
GO
