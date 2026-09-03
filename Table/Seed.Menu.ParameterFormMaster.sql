USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

SET NOCOUNT ON;

/* =========================================================
   Menu: Parameter Form Master (AppShell preview)
   ProgramCode: 95.03.06  (95.03.04 already used by Transaction Code)
   Route: /app/parameterformmaster
   ========================================================= */

DECLARE @CreatedById BIGINT = 1;
DECLARE @ParentId BIGINT;
DECLARE @RowIndex INT = 6;

-- Prefer sibling of existing Parameter List (95.03.02)
SELECT @ParentId = ParentId
FROM [core].[Programs]
WHERE ProgramCode = N'95.03.02' AND DeletedTime IS NULL;

IF @ParentId IS NULL
    SELECT @ParentId = ProgramId
    FROM [core].[Programs]
    WHERE ProgramCode = N'95.03.00' AND DeletedTime IS NULL;

IF @ParentId IS NULL
    SELECT @ParentId = ProgramId
    FROM [core].[Programs]
    WHERE ProgramCode = N'95.00.00' AND DeletedTime IS NULL;

IF @ParentId IS NULL
BEGIN
    -- Fallback: create a System Parameters folder
    IF NOT EXISTS (SELECT 1 FROM [core].[Programs] WHERE ProgramCode = N'95.03.00' AND DeletedTime IS NULL)
    BEGIN
        INSERT INTO [core].[Programs]
        (ProgramGuid, ProgramCode, ProgramName, RootLevel, RowIndex, ProgramType,
         IsProgramViewAble, IsProgramPrintAble, IsVisible, IsActive,
         NavigateUrl, IconCode, CreatedById, CreatedTime, StatusId)
        VALUES
        (NEWID(), N'95.03.00', N'Parameters', 1, 95, N'MNU',
         1, 0, 1, 1, NULL, N'Tune', @CreatedById, SYSDATETIME(), 1);
    END

    SELECT @ParentId = ProgramId FROM [core].[Programs] WHERE ProgramCode = N'95.03.00' AND DeletedTime IS NULL;
END

IF @ParentId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [core].[Programs] WHERE ProgramCode = N'95.03.06' AND DeletedTime IS NULL)
BEGIN
    INSERT INTO [core].[Programs]
    (ProgramGuid, ProgramCode, ProgramName, ParentId, RootLevel, RowIndex, ProgramType,
     IsProgramViewAble, IsProgramAddAble, IsProgramEditAble, IsProgramDeleteAble, IsProgramPrintAble,
     IsVisible, IsActive, NavigateUrl, IconCode, CreatedById, CreatedTime, StatusId)
    VALUES
    (NEWID(), N'95.03.06', N'Parameter Form Master', @ParentId, 2, @RowIndex, N'PRG',
     1, 0, 0, 0, 0, 1, 1,
     N'/app/parameterformmaster', N'Widgets', @CreatedById, SYSDATETIME(), 1);

    PRINT 'Seeded program 95.03.06 Parameter Form Master.';
END
ELSE IF EXISTS (SELECT 1 FROM [core].[Programs] WHERE ProgramCode = N'95.03.06' AND DeletedTime IS NULL)
    PRINT 'Program 95.03.06 already exists.';
ELSE
    PRINT 'WARN: Could not resolve parent menu for 95.03.06.';

-- Grant to admin user group (id=1) if present
DECLARE @AdminUserGroupId BIGINT = 1;

INSERT INTO [core].[UserGroupProgram]
(UserGroupProgramGuid, UserGroupId, ProgramsId, IsUserGroupViewAble, IsUserGroupAddAble, IsUserGroupEditAble, IsUserGroupDeleteAble,
 IsUserGroupApprovalAble, IsUserGroupUnApprovalAble, IsUserGroupVoidAble, IsUserGroupUnVoidAble, IsUserGroupExportAble,
 StatusId, CreatedById, CreatedTime)
SELECT NEWID(), @AdminUserGroupId, p.ProgramId, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, @CreatedById, SYSDATETIME()
FROM [core].[Programs] p
WHERE p.ProgramCode = N'95.03.06'
  AND p.DeletedTime IS NULL
  AND NOT EXISTS (
      SELECT 1 FROM [core].[UserGroupProgram] ugp
      WHERE ugp.UserGroupId = @AdminUserGroupId AND ugp.ProgramsId = p.ProgramId
  );

PRINT 'Parameter Form Master menu seed done.';
GO
