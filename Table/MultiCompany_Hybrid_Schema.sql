/* =========================================================
   HYBRID SCHEMA — Multi Company + Multi Office + RBAC
   Database  : Dev.BamboeHR
   Author    : BamboeUp
   Created   : 2026-04-20
   
   EXECUTION ORDER:
     1. core.Users              (base, no deps)
     2. app.Company             (no deps)
     3. app.CompanyOffice       (depends on Company)
     4. core.Programs           (no deps)
     5. core.UserGroup          (no deps)
     6. core.UserGroupProgram   (depends on UserGroup, Programs)
     7. core.UserCompanyScope   (depends on Users, Company, CompanyOffice)  [NEW]
     8. core.UserGroupScope     (depends on Users, UserGroup, Company, CompanyOffice) [NEW]
     Section A: Indexes
     Section B: Data Migration (from old tables)

   CHANGES vs. EXISTING:
     - UserGroupProgram  : +UNIQUE(UserGroupId,ProgramsId), +IsUserGroupViewAble
     - UserCompanyScope  : BARU — menggabungkan UsersCompanies + UsersCompanyOffices
     - UserGroupScope    : BARU — menggabungkan UserGroupUsers + tambah context Company/Office
     - Programs          : +UNIQUE(ProgramCode), bit nullable → NOT NULL DEFAULT 0
   ========================================================= */

USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

PRINT '=== Starting Hybrid Schema Migration ==='
PRINT ''

-- =========================================================
-- 1. core.Users
--    CHANGE: Tidak ada perubahan struktur, hanya tambah index
-- =========================================================
PRINT '--- [1/8] Checking core.Users ...'

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[Users]'))
BEGIN
    CREATE TABLE [core].[Users](
        [UserId]       BIGINT           IDENTITY(1,1) NOT NULL,
        [UserGuid]     UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT [DF_Users_UserGuid]    DEFAULT (NEWID()),
        [UserName]     NVARCHAR(100)    NOT NULL,
        [PasswordHash] NVARCHAR(MAX)    NOT NULL,
        [PasswordSalt] NVARCHAR(128)    NOT NULL
            CONSTRAINT [DF_Users_PasswordSalt]  DEFAULT (''),
        [FullName]     NVARCHAR(200)    NULL,
        [Email]        NVARCHAR(200)    NULL,
        [IsAdmin]      BIT              NOT NULL
            CONSTRAINT [DF_Users_IsAdmin]     DEFAULT (0),
        [StatusId]     INT              NOT NULL
            CONSTRAINT [DF_Users_StatusId]    DEFAULT (1),
        [RowVersion]   TIMESTAMP        NOT NULL,
        [CreatedById]  BIGINT           NOT NULL,
        [CreatedTime]  DATETIME2(7)     NOT NULL
            CONSTRAINT [DF_Users_CreatedTime] DEFAULT (SYSDATETIME()),
        [UpdatedById]  BIGINT           NULL,
        [UpdatedTime]  DATETIME2(7)     NULL,
        [DeletedById]  BIGINT           NULL,
        [DeletedTime]  DATETIME2(7)     NULL,

        CONSTRAINT [PK_Users]
            PRIMARY KEY CLUSTERED ([UserId] ASC),
        CONSTRAINT [UQ_Users_Guid]
            UNIQUE NONCLUSTERED ([UserGuid]),
        CONSTRAINT [UQ_Users_UserName]
            UNIQUE NONCLUSTERED ([UserName]),
        CONSTRAINT [CHK_Users_Status]
            CHECK ([StatusId] IN (0, 1, 2))
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

    PRINT '  -> core.Users created.'
END
ELSE
    PRINT '  -> core.Users already exists, skipped.'
GO

-- =========================================================
-- 2. app.Company
--    CHANGE: Tidak ada perubahan struktur
-- =========================================================
PRINT '--- [2/8] Checking app.Company ...'

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[app].[Company]'))
BEGIN
    CREATE TABLE [app].[Company](
        [CompanyId]       BIGINT           IDENTITY(1,1) NOT NULL,
        [CompanyGuid]     UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT [DF_Company_Guid]        DEFAULT (NEWID()),
        [CompanyName]     NVARCHAR(200)    NOT NULL,
        [InitialName]     VARCHAR(20)      NULL,
        [TaxCompulsionNo] VARCHAR(30)      NULL,
        [RegistrationNo]  VARCHAR(30)      NULL,
        [ParentCompanyId] BIGINT           NULL,
        [DefaultCurrency] CHAR(3)          NULL,
        [CompanyLogo]     VARBINARY(MAX)   NULL,
        [LeaderId]        BIGINT           NULL,
        [StatusId]        INT              NOT NULL
            CONSTRAINT [DF_Company_StatusId]    DEFAULT (1),
        [RowVersion]      TIMESTAMP        NOT NULL,
        [CreatedById]     BIGINT           NOT NULL,
        [CreatedTime]     DATETIME2(7)     NOT NULL
            CONSTRAINT [DF_Company_CreatedTime] DEFAULT (SYSDATETIME()),
        [UpdatedById]     BIGINT           NULL,
        [UpdatedTime]     DATETIME2(7)     NULL,
        [DeletedById]     BIGINT           NULL,
        [DeletedTime]     DATETIME2(7)     NULL,

        CONSTRAINT [PK_Company]
            PRIMARY KEY CLUSTERED ([CompanyId] ASC),
        CONSTRAINT [UQ_Company_Guid]
            UNIQUE NONCLUSTERED ([CompanyGuid]),
        CONSTRAINT [UQ_Company_Name]
            UNIQUE NONCLUSTERED ([CompanyName]),
        CONSTRAINT [CHK_Company_Status]
            CHECK ([StatusId] IN (0, 1, 2))
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

    PRINT '  -> app.Company created.'
END
ELSE
    PRINT '  -> app.Company already exists, skipped.'
GO

-- =========================================================
-- 3. app.CompanyOffice
--    CHANGE: Tidak ada perubahan struktur
-- =========================================================
PRINT '--- [3/8] Checking app.CompanyOffice ...'

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[app].[CompanyOffice]'))
BEGIN
    CREATE TABLE [app].[CompanyOffice](
        [CompanyOfficeId]   BIGINT           IDENTITY(1,1) NOT NULL,
        [CompanyOfficeGuid] UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT [DF_CompanyOffice_Guid]        DEFAULT (NEWID()),
        [CompanyId]         BIGINT           NOT NULL,
        [CompanyGuid]       UNIQUEIDENTIFIER NOT NULL,
        [CompanyOfficeName] VARCHAR(150)     NOT NULL,
        [SrAddressType]     BIGINT           NOT NULL,
        [CountryId]         BIGINT           NOT NULL,
        [StateId]           BIGINT           NOT NULL,
        [CityId]            BIGINT           NOT NULL
            CONSTRAINT [DF_CompanyOffice_CityId]      DEFAULT (0),
        [PostalCodeId]      VARCHAR(8)       NOT NULL
            CONSTRAINT [DF_CompanyOffice_PostalCode]  DEFAULT (''),
        [Address]           VARCHAR(500)     NOT NULL,
        [StatusId]          INT              NOT NULL
            CONSTRAINT [DF_CompanyOffice_StatusId]    DEFAULT (1),
        [RowVersion]        TIMESTAMP        NOT NULL,
        [CreatedById]       BIGINT           NOT NULL,
        [CreatedTime]       DATETIME2(7)     NOT NULL
            CONSTRAINT [DF_CompanyOffice_CreatedTime] DEFAULT (SYSDATETIME()),
        [UpdatedById]       BIGINT           NULL,
        [UpdatedTime]       DATETIME2(7)     NULL,
        [DeletedById]       BIGINT           NULL,
        [DeletedTime]       DATETIME2(7)     NULL,

        CONSTRAINT [PK_CompanyOffice]
            PRIMARY KEY CLUSTERED ([CompanyOfficeId] ASC),
        CONSTRAINT [UQ_CompanyOffice_Guid]
            UNIQUE NONCLUSTERED ([CompanyOfficeGuid]),
        CONSTRAINT [CHK_CompanyOffice_Status]
            CHECK ([StatusId] IN (0, 1, 2)),
        CONSTRAINT [FK_CompanyOffice_Company]
            FOREIGN KEY ([CompanyId]) REFERENCES [app].[Company]([CompanyId])
    ) ON [PRIMARY]

    PRINT '  -> app.CompanyOffice created.'
END
ELSE
    PRINT '  -> app.CompanyOffice already exists, skipped.'
GO

-- =========================================================
-- 4. core.Programs
--    CHANGE: +UNIQUE(ProgramCode), bit columns NOT NULL DEFAULT 0
--            ProgramGuid dipertahankan (penting untuk API)
-- =========================================================
PRINT '--- [4/8] Checking core.Programs ...'

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[Programs]'))
BEGIN
    CREATE TABLE [core].[Programs](
        [ProgramId]              BIGINT           IDENTITY(1,1) NOT NULL,
        [ProgramGuid]            UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT [DF_Programs_Guid]             DEFAULT (NEWID()),
        [ProgramCode]            NVARCHAR(30)     NOT NULL,
        [ParentId]               BIGINT           NULL,
        [IconCode]               VARCHAR(50)      NULL,
        [ProgramName]            NVARCHAR(100)    NOT NULL,
        [TopLevelProgramId]      BIGINT           NULL,
        [RootLevel]              TINYINT          NOT NULL,
        [RowIndex]               TINYINT          NOT NULL,
        [Note]                   NVARCHAR(1000)   NULL,
        [IsParentProgram]        BIT              NOT NULL CONSTRAINT [DF_Programs_IsParentProgram]    DEFAULT (0),
        [IsProgram]              BIT              NOT NULL CONSTRAINT [DF_Programs_IsProgram]          DEFAULT (0),
        [IsBeginGroup]           BIT              NOT NULL CONSTRAINT [DF_Programs_IsBeginGroup]       DEFAULT (0),
        [ProgramType]            NVARCHAR(5)      NULL,
        [IsProgramAddAble]       BIT              NOT NULL CONSTRAINT [DF_Programs_AddAble]            DEFAULT (0),
        [IsProgramEditAble]      BIT              NOT NULL CONSTRAINT [DF_Programs_EditAble]           DEFAULT (0),
        [IsProgramDeleteAble]    BIT              NOT NULL CONSTRAINT [DF_Programs_DeleteAble]         DEFAULT (0),
        [IsProgramViewAble]      BIT              NOT NULL CONSTRAINT [DF_Programs_ViewAble]           DEFAULT (1),
        [IsProgramApprovalAble]  BIT              NOT NULL CONSTRAINT [DF_Programs_ApprovalAble]       DEFAULT (0),
        [IsProgramUnApprovalAble]BIT              NOT NULL CONSTRAINT [DF_Programs_UnApprovalAble]     DEFAULT (0),
        [IsProgramVoidAble]      BIT              NOT NULL CONSTRAINT [DF_Programs_VoidAble]           DEFAULT (0),
        [IsProgramUnVoidAble]    BIT              NOT NULL CONSTRAINT [DF_Programs_UnVoidAble]         DEFAULT (0),
        [IsProgramDirectVoid]    BIT              NOT NULL CONSTRAINT [DF_Programs_DirectVoid]         DEFAULT (0),
        [IsProgramPrintAble]     BIT              NOT NULL CONSTRAINT [DF_Programs_PrintAble]          DEFAULT (0),
        [IsMenuAddVisible]       BIT              NOT NULL CONSTRAINT [DF_Programs_MenuAddVisible]      DEFAULT (1),
        [IsMenuHomeVisible]      BIT              NOT NULL CONSTRAINT [DF_Programs_MenuHomeVisible]     DEFAULT (1),
        [IsVisible]              BIT              NOT NULL CONSTRAINT [DF_Programs_IsVisible]           DEFAULT (1),
        [NavigateUrl]            NVARCHAR(1000)   NULL,
        [HelpLinkId]             NVARCHAR(255)    NULL,
        [AssemblyName]           NVARCHAR(50)     NULL,
        [AssemblyClassName]      NVARCHAR(200)    NULL,
        [StoreProcedureName]     NVARCHAR(200)    NULL,
        [AccessKey]              NCHAR(1)         NULL,
        [IsActive]               BIT              NOT NULL CONSTRAINT [DF_Programs_IsActive]           DEFAULT (1),
        [StatusId]               INT              NOT NULL CONSTRAINT [DF_Programs_StatusId]           DEFAULT (1),
        [RowVersion]             TIMESTAMP        NOT NULL,
        [CreatedById]            BIGINT           NOT NULL,
        [CreatedTime]            DATETIME2(7)     NOT NULL
            CONSTRAINT [DF_Programs_CreatedTime] DEFAULT (SYSDATETIME()),
        [UpdatedById]            BIGINT           NULL,
        [UpdatedTime]            DATETIME2(7)     NULL,
        [DeletedById]            BIGINT           NULL,
        [DeletedTime]            DATETIME2(7)     NULL,

        CONSTRAINT [PK_Programs]
            PRIMARY KEY CLUSTERED ([ProgramId] ASC),
        CONSTRAINT [UQ_Programs_Guid]
            UNIQUE NONCLUSTERED ([ProgramGuid]),
        CONSTRAINT [UQ_Programs_ProgramCode]          -- BARU: mencegah duplikasi kode
            UNIQUE NONCLUSTERED ([ProgramCode]),
        CONSTRAINT [CHK_Programs_Status]
            CHECK ([StatusId] IN (0, 1, 2))
    ) ON [PRIMARY]

    PRINT '  -> core.Programs created.'
END
ELSE
BEGIN
    -- Jika tabel sudah ada, tambah UQ constraint ProgramCode jika belum ada
    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE name = 'UQ_Programs_ProgramCode'
          AND object_id = OBJECT_ID(N'[core].[Programs]')
    )
    BEGIN
        ALTER TABLE [core].[Programs]
            ADD CONSTRAINT [UQ_Programs_ProgramCode]
                UNIQUE NONCLUSTERED ([ProgramCode])
        PRINT '  -> Added UQ_Programs_ProgramCode on existing table.'
    END
    ELSE
        PRINT '  -> core.Programs already exists, skipped.'
END
GO

-- =========================================================
-- 5. core.UserGroup
--    CHANGE: Tidak ada perubahan struktur
-- =========================================================
PRINT '--- [5/8] Checking core.UserGroup ...'

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UserGroup]'))
BEGIN
    CREATE TABLE [core].[UserGroup](
        [UserGroupId]   BIGINT           IDENTITY(1,1) NOT NULL,
        [UserGroupGuid] UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT [DF_UserGroup_Guid]        DEFAULT (NEWID()),
        [UserGroupName] NVARCHAR(100)    NOT NULL,
        [IsEditAble]    BIT              NOT NULL,
        [StatusId]      INT              NOT NULL
            CONSTRAINT [DF_UserGroup_StatusId]    DEFAULT (1),
        [RowVersion]    TIMESTAMP        NOT NULL,
        [CreatedById]   BIGINT           NOT NULL,
        [CreatedTime]   DATETIME2(7)     NOT NULL
            CONSTRAINT [DF_UserGroup_CreatedTime] DEFAULT (SYSDATETIME()),
        [UpdatedById]   BIGINT           NULL,
        [UpdatedTime]   DATETIME2(7)     NULL,
        [DeletedById]   BIGINT           NULL,
        [DeletedTime]   DATETIME2(7)     NULL,

        CONSTRAINT [PK_UserGroup]
            PRIMARY KEY CLUSTERED ([UserGroupId] ASC),
        CONSTRAINT [UQ_UserGroup_Guid]
            UNIQUE NONCLUSTERED ([UserGroupGuid]),
        CONSTRAINT [CHK_UserGroup_Status]
            CHECK ([StatusId] IN (0, 1, 2))
    ) ON [PRIMARY]

    PRINT '  -> core.UserGroup created.'
END
ELSE
    PRINT '  -> core.UserGroup already exists, skipped.'
GO

-- =========================================================
-- 6. core.UserGroupProgram
--    CHANGE: +IsUserGroupViewAble (penting!)
--            +UNIQUE(UserGroupId, ProgramsId) (cegah duplikasi)
--            bit columns NOT NULL DEFAULT 0
-- =========================================================
PRINT '--- [6/8] Checking core.UserGroupProgram ...'

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UserGroupProgram]'))
BEGIN
    CREATE TABLE [core].[UserGroupProgram](
        [UserGroupProgramId]       BIGINT           IDENTITY(1,1) NOT NULL,
        [UserGroupProgramGuid]     UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT [DF_UserGroupProgram_Guid]            DEFAULT (NEWID()),
        [UserGroupId]              BIGINT           NOT NULL,
        [ProgramsId]               BIGINT           NOT NULL,
        -- ===== PERMISSION FLAGS =====
        [IsUserGroupViewAble]      BIT              NOT NULL  -- BARU: hak lihat
            CONSTRAINT [DF_UGP_ViewAble]            DEFAULT (1),
        [IsUserGroupAddAble]       BIT              NOT NULL
            CONSTRAINT [DF_UGP_AddAble]             DEFAULT (0),
        [IsUserGroupEditAble]      BIT              NOT NULL
            CONSTRAINT [DF_UGP_EditAble]            DEFAULT (0),
        [IsUserGroupDeleteAble]    BIT              NOT NULL
            CONSTRAINT [DF_UGP_DeleteAble]          DEFAULT (0),
        [IsUserGroupApprovalAble]  BIT              NOT NULL
            CONSTRAINT [DF_UGP_ApprovalAble]        DEFAULT (0),
        [IsUserGroupUnApprovalAble]BIT              NOT NULL
            CONSTRAINT [DF_UGP_UnApprovalAble]      DEFAULT (0),
        [IsUserGroupVoidAble]      BIT              NOT NULL
            CONSTRAINT [DF_UGP_VoidAble]            DEFAULT (0),
        [IsUserGroupUnVoidAble]    BIT              NOT NULL
            CONSTRAINT [DF_UGP_UnVoidAble]          DEFAULT (0),
        [IsUserGroupExportAble]    BIT              NOT NULL
            CONSTRAINT [DF_UGP_ExportAble]          DEFAULT (0),
        -- ===========================
        [StatusId]                 INT              NOT NULL
            CONSTRAINT [DF_UserGroupProgram_StatusId]        DEFAULT (1),
        [RowVersion]               TIMESTAMP        NOT NULL,
        [CreatedById]              BIGINT           NOT NULL,
        [CreatedTime]              DATETIME2(7)     NOT NULL
            CONSTRAINT [DF_UserGroupProgram_CreatedTime]     DEFAULT (SYSDATETIME()),
        [UpdatedById]              BIGINT           NULL,
        [UpdatedTime]              DATETIME2(7)     NULL,
        [DeletedById]              BIGINT           NULL,
        [DeletedTime]              DATETIME2(7)     NULL,

        CONSTRAINT [PK_UserGroupProgram]
            PRIMARY KEY CLUSTERED ([UserGroupProgramId] ASC),
        CONSTRAINT [UQ_UserGroupProgram_Guid]
            UNIQUE NONCLUSTERED ([UserGroupProgramGuid]),
        CONSTRAINT [UQ_UserGroupProgram_Unique]           -- BARU: cegah duplikasi!
            UNIQUE NONCLUSTERED ([UserGroupId], [ProgramsId]),
        CONSTRAINT [CHK_UserGroupProgram_Status]
            CHECK ([StatusId] IN (0, 1, 2)),
        CONSTRAINT [FK_UserGroupProgram_UserGroup]
            FOREIGN KEY ([UserGroupId]) REFERENCES [core].[UserGroup]([UserGroupId]),
        CONSTRAINT [FK_UserGroupProgram_Programs]
            FOREIGN KEY ([ProgramsId]) REFERENCES [core].[Programs]([ProgramId])
    ) ON [PRIMARY]

    PRINT '  -> core.UserGroupProgram created.'
END
ELSE
BEGIN
    -- Tambah kolom dan constraint baru jika belum ada di tabel existing
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[core].[UserGroupProgram]') AND name = 'IsUserGroupViewAble')
    BEGIN
        ALTER TABLE [core].[UserGroupProgram]
            ADD [IsUserGroupViewAble] BIT NOT NULL
                CONSTRAINT [DF_UGP_ViewAble] DEFAULT (1)
        PRINT '  -> Added IsUserGroupViewAble column.'
    END

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_UserGroupProgram_Unique' AND object_id = OBJECT_ID(N'[core].[UserGroupProgram]'))
    BEGIN
        ALTER TABLE [core].[UserGroupProgram]
            ADD CONSTRAINT [UQ_UserGroupProgram_Unique]
                UNIQUE NONCLUSTERED ([UserGroupId], [ProgramsId])
        PRINT '  -> Added UQ_UserGroupProgram_Unique constraint.'
    END
    ELSE
        PRINT '  -> core.UserGroupProgram already exists, skipped.'
END
GO

-- =========================================================
-- 7. core.UserCompanyScope  [BARU]
--    Menggantikan: UsersCompanies + UsersCompanyOffices
--    Keunggulan  : 1 tabel, CompanyOfficeId nullable (NULL = akses semua office),
--                  Filtered UNIQUE index untuk handle NULL dengan benar
-- =========================================================
PRINT '--- [7/8] Creating core.UserCompanyScope (NEW) ...'

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UserCompanyScope]'))
BEGIN
    CREATE TABLE [core].[UserCompanyScope](
        [UserCompanyScopeId]   BIGINT           IDENTITY(1,1) NOT NULL,
        [UserCompanyScopeGuid] UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT [DF_UCS_Guid]          DEFAULT (NEWID()),
        [UserId]               BIGINT           NOT NULL,
        [CompanyId]            BIGINT           NOT NULL,
        -- NULL = user punya akses semua office di company tersebut
        -- NOT NULL = user hanya punya akses ke 1 office spesifik
        [CompanyOfficeId]      BIGINT           NULL,
        [IsDefaultCompany]     BIT              NOT NULL
            CONSTRAINT [DF_UCS_DefaultCompany] DEFAULT (0),
        [IsDefaultOffice]      BIT              NOT NULL
            CONSTRAINT [DF_UCS_DefaultOffice]  DEFAULT (0),
        [StatusId]             INT              NOT NULL
            CONSTRAINT [DF_UCS_StatusId]       DEFAULT (1),
        [RowVersion]           TIMESTAMP        NOT NULL,
        [CreatedById]          BIGINT           NOT NULL,
        [CreatedTime]          DATETIME2(7)     NOT NULL
            CONSTRAINT [DF_UCS_CreatedTime]    DEFAULT (SYSDATETIME()),
        [UpdatedById]          BIGINT           NULL,
        [UpdatedTime]          DATETIME2(7)     NULL,
        [DeletedById]          BIGINT           NULL,
        [DeletedTime]          DATETIME2(7)     NULL,

        CONSTRAINT [PK_UserCompanyScope]
            PRIMARY KEY CLUSTERED ([UserCompanyScopeId] ASC),
        CONSTRAINT [UQ_UCS_Guid]
            UNIQUE NONCLUSTERED ([UserCompanyScopeGuid]),
        CONSTRAINT [CHK_UCS_Status]
            CHECK ([StatusId] IN (0, 1, 2)),
        CONSTRAINT [FK_UCS_User]
            FOREIGN KEY ([UserId]) REFERENCES [core].[Users]([UserId]),
        CONSTRAINT [FK_UCS_Company]
            FOREIGN KEY ([CompanyId]) REFERENCES [app].[Company]([CompanyId]),
        CONSTRAINT [FK_UCS_Office]
            FOREIGN KEY ([CompanyOfficeId]) REFERENCES [app].[CompanyOffice]([CompanyOfficeId])
    ) ON [PRIMARY]

    PRINT '  -> core.UserCompanyScope created.'
END
ELSE
    PRINT '  -> core.UserCompanyScope already exists, skipped.'
GO

-- =========================================================
-- 8. core.UserGroupScope  [BARU - PALING PENTING]
--    Menggantikan: UserGroupUsers
--    Keunggulan  : Role per Context (Company + Office)
--    Contoh      : User A = "Admin" di Company 1, "Viewer" di Company 2
-- =========================================================
PRINT '--- [8/8] Creating core.UserGroupScope (NEW - CRITICAL) ...'

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UserGroupScope]'))
BEGIN
    CREATE TABLE [core].[UserGroupScope](
        [UserGroupScopeId]   BIGINT           IDENTITY(1,1) NOT NULL,
        [UserGroupScopeGuid] UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT [DF_UGS_Guid]          DEFAULT (NEWID()),
        [UserId]             BIGINT           NOT NULL,
        [UserGroupId]        BIGINT           NOT NULL,
        [CompanyId]          BIGINT           NOT NULL,
        -- NULL = group berlaku untuk semua office di company
        -- NOT NULL = group hanya berlaku untuk 1 office spesifik
        [CompanyOfficeId]    BIGINT           NULL,
        [StatusId]           INT              NOT NULL
            CONSTRAINT [DF_UGS_StatusId]      DEFAULT (1),
        [RowVersion]         TIMESTAMP        NOT NULL,
        [CreatedById]        BIGINT           NOT NULL,
        [CreatedTime]        DATETIME2(7)     NOT NULL
            CONSTRAINT [DF_UGS_CreatedTime]   DEFAULT (SYSDATETIME()),
        [UpdatedById]        BIGINT           NULL,
        [UpdatedTime]        DATETIME2(7)     NULL,
        [DeletedById]        BIGINT           NULL,
        [DeletedTime]        DATETIME2(7)     NULL,

        CONSTRAINT [PK_UserGroupScope]
            PRIMARY KEY CLUSTERED ([UserGroupScopeId] ASC),
        CONSTRAINT [UQ_UGS_Guid]
            UNIQUE NONCLUSTERED ([UserGroupScopeGuid]),
        CONSTRAINT [CHK_UGS_Status]
            CHECK ([StatusId] IN (0, 1, 2)),
        CONSTRAINT [FK_UGS_User]
            FOREIGN KEY ([UserId]) REFERENCES [core].[Users]([UserId]),
        CONSTRAINT [FK_UGS_Group]
            FOREIGN KEY ([UserGroupId]) REFERENCES [core].[UserGroup]([UserGroupId]),
        CONSTRAINT [FK_UGS_Company]
            FOREIGN KEY ([CompanyId]) REFERENCES [app].[Company]([CompanyId]),
        CONSTRAINT [FK_UGS_Office]
            FOREIGN KEY ([CompanyOfficeId]) REFERENCES [app].[CompanyOffice]([CompanyOfficeId])
    ) ON [PRIMARY]

    PRINT '  -> core.UserGroupScope created.'
END
ELSE
    PRINT '  -> core.UserGroupScope already exists, skipped.'
GO

-- =========================================================
-- SECTION A: INDEXES (Performance)
-- Semua index dibuat dengan IF NOT EXISTS
-- =========================================================
PRINT ''
PRINT '=== Section A: Creating Indexes ==='

-- -------------------------------------------------------
-- A1. core.Users
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Users_Status' AND object_id = OBJECT_ID(N'[core].[Users]'))
    CREATE NONCLUSTERED INDEX [IX_Users_Status]
    ON [core].[Users] ([StatusId])
    INCLUDE ([UserGuid], [UserName], [FullName], [Email], [IsAdmin])
    WHERE [DeletedTime] IS NULL
GO

-- -------------------------------------------------------
-- A2. app.CompanyOffice
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_CompanyOffice_CompanyId' AND object_id = OBJECT_ID(N'[app].[CompanyOffice]'))
    CREATE NONCLUSTERED INDEX [IX_CompanyOffice_CompanyId]
    ON [app].[CompanyOffice] ([CompanyId], [StatusId])
    INCLUDE ([CompanyOfficeGuid], [CompanyOfficeName])
    WHERE [DeletedTime] IS NULL
GO

-- -------------------------------------------------------
-- A3. core.Programs
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Programs_Code_Status' AND object_id = OBJECT_ID(N'[core].[Programs]'))
    CREATE NONCLUSTERED INDEX [IX_Programs_Code_Status]
    ON [core].[Programs] ([ProgramCode], [StatusId])
    INCLUDE ([ProgramGuid], [ProgramName], [ParentId], [RootLevel], [RowIndex], [IsVisible], [IsActive], [NavigateUrl], [IconCode])
    WHERE [DeletedTime] IS NULL
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Programs_ParentId' AND object_id = OBJECT_ID(N'[core].[Programs]'))
    CREATE NONCLUSTERED INDEX [IX_Programs_ParentId]
    ON [core].[Programs] ([ParentId], [StatusId])
    INCLUDE ([ProgramGuid], [ProgramCode], [ProgramName], [RootLevel], [RowIndex], [IconCode])
    WHERE [DeletedTime] IS NULL
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Programs_TopLevel' AND object_id = OBJECT_ID(N'[core].[Programs]'))
    CREATE NONCLUSTERED INDEX [IX_Programs_TopLevel]
    ON [core].[Programs] ([TopLevelProgramId])
    WHERE [DeletedTime] IS NULL AND [StatusId] > 0
GO

-- -------------------------------------------------------
-- A4. core.UserGroup
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UserGroup_Name' AND object_id = OBJECT_ID(N'[core].[UserGroup]'))
    CREATE NONCLUSTERED INDEX [IX_UserGroup_Name]
    ON [core].[UserGroup] ([UserGroupName], [StatusId])
    INCLUDE ([UserGroupGuid], [IsEditAble])
    WHERE [DeletedTime] IS NULL
GO

-- -------------------------------------------------------
-- A5. core.UserGroupProgram
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UGP_GroupId_Status' AND object_id = OBJECT_ID(N'[core].[UserGroupProgram]'))
    CREATE NONCLUSTERED INDEX [IX_UGP_GroupId_Status]
    ON [core].[UserGroupProgram] ([UserGroupId], [StatusId])
    INCLUDE ([UserGroupProgramGuid], [ProgramsId],
             [IsUserGroupViewAble], [IsUserGroupAddAble], [IsUserGroupEditAble],
             [IsUserGroupDeleteAble], [IsUserGroupApprovalAble], [IsUserGroupExportAble])
    WHERE [DeletedTime] IS NULL
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UGP_ProgramId' AND object_id = OBJECT_ID(N'[core].[UserGroupProgram]'))
    CREATE NONCLUSTERED INDEX [IX_UGP_ProgramId]
    ON [core].[UserGroupProgram] ([ProgramsId])
    WHERE [DeletedTime] IS NULL
GO

-- -------------------------------------------------------
-- A6. core.UserCompanyScope (BARU)
-- -------------------------------------------------------

-- Filtered UNIQUE: satu record per (user, company, office tertentu)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_UCS_WithOffice' AND object_id = OBJECT_ID(N'[core].[UserCompanyScope]'))
    CREATE UNIQUE NONCLUSTERED INDEX [UQ_UCS_WithOffice]
    ON [core].[UserCompanyScope] ([UserId], [CompanyId], [CompanyOfficeId])
    WHERE [CompanyOfficeId] IS NOT NULL AND [DeletedTime] IS NULL
GO

-- Filtered UNIQUE: satu "akses semua office" per (user, company)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_UCS_CompanyOnly' AND object_id = OBJECT_ID(N'[core].[UserCompanyScope]'))
    CREATE UNIQUE NONCLUSTERED INDEX [UQ_UCS_CompanyOnly]
    ON [core].[UserCompanyScope] ([UserId], [CompanyId])
    WHERE [CompanyOfficeId] IS NULL AND [DeletedTime] IS NULL
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UCS_UserId_Status' AND object_id = OBJECT_ID(N'[core].[UserCompanyScope]'))
    CREATE NONCLUSTERED INDEX [IX_UCS_UserId_Status]
    ON [core].[UserCompanyScope] ([UserId], [StatusId])
    INCLUDE ([UserCompanyScopeGuid], [CompanyId], [CompanyOfficeId], [IsDefaultCompany], [IsDefaultOffice])
    WHERE [DeletedTime] IS NULL
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UCS_CompanyId' AND object_id = OBJECT_ID(N'[core].[UserCompanyScope]'))
    CREATE NONCLUSTERED INDEX [IX_UCS_CompanyId]
    ON [core].[UserCompanyScope] ([CompanyId], [StatusId])
    INCLUDE ([UserId], [CompanyOfficeId], [IsDefaultCompany])
    WHERE [DeletedTime] IS NULL
GO

-- -------------------------------------------------------
-- A7. core.UserGroupScope (BARU)
-- -------------------------------------------------------

-- Filtered UNIQUE: user+group hanya 1x per company+office tertentu
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_UGS_WithOffice' AND object_id = OBJECT_ID(N'[core].[UserGroupScope]'))
    CREATE UNIQUE NONCLUSTERED INDEX [UQ_UGS_WithOffice]
    ON [core].[UserGroupScope] ([UserId], [UserGroupId], [CompanyId], [CompanyOfficeId])
    WHERE [CompanyOfficeId] IS NOT NULL AND [DeletedTime] IS NULL
GO

-- Filtered UNIQUE: user+group hanya 1x per company (berlaku semua office)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_UGS_CompanyLevel' AND object_id = OBJECT_ID(N'[core].[UserGroupScope]'))
    CREATE UNIQUE NONCLUSTERED INDEX [UQ_UGS_CompanyLevel]
    ON [core].[UserGroupScope] ([UserId], [UserGroupId], [CompanyId])
    WHERE [CompanyOfficeId] IS NULL AND [DeletedTime] IS NULL
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UGS_UserId_Company' AND object_id = OBJECT_ID(N'[core].[UserGroupScope]'))
    CREATE NONCLUSTERED INDEX [IX_UGS_UserId_Company]
    ON [core].[UserGroupScope] ([UserId], [CompanyId], [StatusId])
    INCLUDE ([UserGroupScopeGuid], [UserGroupId], [CompanyOfficeId])
    WHERE [DeletedTime] IS NULL
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UGS_GroupId' AND object_id = OBJECT_ID(N'[core].[UserGroupScope]'))
    CREATE NONCLUSTERED INDEX [IX_UGS_GroupId]
    ON [core].[UserGroupScope] ([UserGroupId], [StatusId])
    INCLUDE ([UserGroupScopeGuid], [UserId], [CompanyId], [CompanyOfficeId])
    WHERE [DeletedTime] IS NULL
GO

PRINT '=== All Indexes Created ==='
PRINT ''

-- =========================================================
-- SECTION B: DATA MIGRATION
-- Migrate dari tabel lama ke tabel baru (opsional, jalankan
-- hanya jika UsersCompanies & UsersCompanyOffices sudah ada datanya)
-- =========================================================
PRINT '=== Section B: Data Migration (from old tables) ==='

-- B1. Migrate UsersCompanies -> UserCompanyScope
IF EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UsersCompanies]'))
   AND EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UserCompanyScope]'))
BEGIN
    IF NOT EXISTS (SELECT TOP 1 1 FROM [core].[UserCompanyScope])
    BEGIN
        INSERT INTO [core].[UserCompanyScope]
            (UserId, CompanyId, CompanyOfficeId, IsDefaultCompany, IsDefaultOffice,
             StatusId, CreatedById, CreatedTime, UpdatedById, UpdatedTime, DeletedById, DeletedTime)
        SELECT
            uc.UserId,
            uc.CompanyId,
            NULL,                 -- akses semua office (belum granular)
            uc.IsDefault,         -- IsDefaultCompany
            0,                    -- IsDefaultOffice
            uc.StatusId,
            uc.CreatedById,
            uc.CreatedTime,
            uc.UpdatedById,
            uc.UpdatedTime,
            uc.DeletedById,
            uc.DeletedTime
        FROM [core].[UsersCompanies] uc
        WHERE uc.StatusId > 0 AND uc.DeletedTime IS NULL

        PRINT '  -> Migrated UsersCompanies -> UserCompanyScope.'
    END
    ELSE
        PRINT '  -> UserCompanyScope already has data, migration skipped.'
END

-- B2. Migrate UserGroupUsers -> UserGroupScope
-- Catatan: CompanyId harus diisi secara manual setelah migrasi
-- karena UserGroupUsers tidak punya context company
IF EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UserGroupUsers]'))
   AND EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UserGroupScope]'))
BEGIN
    IF NOT EXISTS (SELECT TOP 1 1 FROM [core].[UserGroupScope])
    BEGIN
        -- PERHATIAN: CompanyId diisi dengan company default user
        -- Setelah migrasi, perlu review manual untuk set CompanyId yang benar
        INSERT INTO [core].[UserGroupScope]
            (UserId, UserGroupId, CompanyId, CompanyOfficeId, StatusId,
             CreatedById, CreatedTime, UpdatedById, UpdatedTime, DeletedById, DeletedTime)
        SELECT
            ugu.UserId,
            ugu.UserGroupId,
            -- Ambil company default user (IsDefaultCompany=1), jika tidak ada ambil yang pertama
            COALESCE(
                (SELECT TOP 1 CompanyId FROM [core].[UsersCompanies]
                 WHERE UserId = ugu.UserId AND IsDefault = 1 AND StatusId > 0),
                (SELECT TOP 1 CompanyId FROM [core].[UsersCompanies]
                 WHERE UserId = ugu.UserId AND StatusId > 0)
            ),
            NULL,               -- CompanyOfficeId: NULL = semua office
            ugu.StatusId,
            ugu.CreatedById,
            ugu.CreatedTime,
            ugu.UpdatedById,
            ugu.UpdatedTime,
            ugu.DeletedById,
            ugu.DeletedTime
        FROM [core].[UserGroupUsers] ugu
        WHERE ugu.StatusId > 0
          AND ugu.DeletedTime IS NULL
          AND EXISTS (
              SELECT 1 FROM [core].[UsersCompanies]
              WHERE UserId = ugu.UserId AND StatusId > 0
          )

        PRINT '  -> Migrated UserGroupUsers -> UserGroupScope.'
        PRINT '  !! IMPORTANT: Review CompanyId values in UserGroupScope after migration!'
    END
    ELSE
        PRINT '  -> UserGroupScope already has data, migration skipped.'
END

PRINT ''
PRINT '==========================================='
PRINT '  SCHEMA MIGRATION COMPLETED SUCCESSFULLY'
PRINT '==========================================='
PRINT ''
PRINT 'New Tables Created:'
PRINT '  + core.UserCompanyScope (replaces UsersCompanies + UsersCompanyOffices)'
PRINT '  + core.UserGroupScope   (replaces UserGroupUsers + adds Company context)'
PRINT ''
PRINT 'Modified Tables:'
PRINT '  ~ core.UserGroupProgram: +IsUserGroupViewAble, +UQ(GroupId,ProgramsId)'
PRINT '  ~ core.Programs: +UQ(ProgramCode), bit columns NOT NULL DEFAULT 0'
PRINT ''
PRINT 'IMPORTANT NOTES:'
PRINT '  1. Old tables (UsersCompanies, UsersCompanyOffices, UserGroupUsers) '
PRINT '     are NOT dropped - review migration data first before dropping.'
PRINT '  2. Review UserGroupScope.CompanyId values after migration.'
PRINT '  3. Update API code to use new tables.'
GO
