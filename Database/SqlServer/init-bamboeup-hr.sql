-- BamboeUP.HR - SQL Server production schema (minimal runtime set)
-- Dialect twin of Database/PostgreSQL/init-bamboeup-hr.sql
-- Same tables/columns: brackets, IDENTITY, datetime2, uniqueidentifier, ROWVERSION.
-- Create the database yourself first. This script does not DROP DATABASE.
-- Physical FKs omitted on business tables (indexes on *Id / Sr* instead).
-- Script Date: 2026-09-11

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Drop views first so DROP TABLE succeeds on re-run
DROP VIEW IF EXISTS [app].[ManagerEmployee];
DROP VIEW IF EXISTS [app].[SupervisorEmployee];
DROP VIEW IF EXISTS [app].[FromCompany];
DROP VIEW IF EXISTS [app].[ToCompany];
DROP VIEW IF EXISTS [app].[FromOrganizationUnit];
DROP VIEW IF EXISTS [app].[ToOrganizationUnit];
DROP VIEW IF EXISTS [app].[FromManagerEmployee];
DROP VIEW IF EXISTS [app].[ToManagerEmployee];
DROP VIEW IF EXISTS [app].[FromJobGrade];
DROP VIEW IF EXISTS [app].[ToJobGrade];
DROP VIEW IF EXISTS [app].[FromJobLevel];
DROP VIEW IF EXISTS [app].[ToJobLevel];
DROP VIEW IF EXISTS [app].[FromJobTitle];
DROP VIEW IF EXISTS [app].[ToJobTitle];
DROP VIEW IF EXISTS [app].[FromPosition];
DROP VIEW IF EXISTS [app].[ToPosition];
DROP VIEW IF EXISTS [app].[vw_StandardReference_Display];
DROP VIEW IF EXISTS [app].[vw_StandardReferenceItem];
DROP VIEW IF EXISTS [core].[vw_AdministrativeArea];

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'app')
    EXEC(N'CREATE SCHEMA [app]');

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'core')
    EXEC(N'CREATE SCHEMA [core]');

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'apv')
    EXEC(N'CREATE SCHEMA [apv]');

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'log')
    EXEC(N'CREATE SCHEMA [log]');

-- ---------------------------------------------------------------------------
-- core.Users
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [core].[Users];
CREATE TABLE [core].[Users]
(
    [UserId] BIGINT IDENTITY(1,1) NOT NULL,
    [UserGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [UserName] NVARCHAR(100) NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [PasswordSalt] NVARCHAR(MAX) NOT NULL,
    [FullName] NVARCHAR(200),
    [Email] NVARCHAR(200),
    [IsAdmin] BIT NOT NULL DEFAULT 0,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_User_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_User_Guid] ON [core].[Users] ([UserGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_User_UserName' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_User_UserName] ON [core].[Users] ([UserName]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);

-- ---------------------------------------------------------------------------
-- core.UserGroup
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [core].[UserGroup];
CREATE TABLE [core].[UserGroup]
(
    [UserGroupId] BIGINT IDENTITY(1,1) NOT NULL,
    [UserGroupGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [UserGroupName] NVARCHAR(100) NOT NULL,
    [IsEditAble] BIT NOT NULL DEFAULT 0,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_UserGroup] PRIMARY KEY ([UserGroupId])
);

-- ---------------------------------------------------------------------------
-- core.Programs
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [core].[Programs];
CREATE TABLE [core].[Programs]
(
    [ProgramId] BIGINT IDENTITY(1,1) NOT NULL,
    [ProgramGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ProgramCode] NVARCHAR(30) NOT NULL,
    [ParentId] BIGINT,
    [IconCode] NVARCHAR(50),
    [ProgramName] NVARCHAR(100) NOT NULL,
    [TopLevelProgramId] BIGINT,
    [RootLevel] SMALLINT NOT NULL,
    [RowIndex] SMALLINT NOT NULL,
    [Note] NVARCHAR(1000),
    [IsParentProgram] BIT DEFAULT 0,
    [IsProgram] BIT DEFAULT 0,
    [IsBeginGroup] BIT DEFAULT 0,
    [ProgramType] NVARCHAR(5),
    [IsProgramAddAble] BIT DEFAULT 0,
    [IsProgramEditAble] BIT DEFAULT 0,
    [IsProgramDeleteAble] BIT DEFAULT 0,
    [IsProgramViewAble] BIT DEFAULT 0,
    [IsProgramApprovalAble] BIT DEFAULT 0,
    [IsProgramUnApprovalAble] BIT DEFAULT 0,
    [IsProgramVoidAble] BIT DEFAULT 0,
    [IsProgramUnVoidAble] BIT DEFAULT 0,
    [IsProgramDirectVoid] BIT DEFAULT 0,
    [IsProgramPrintAble] BIT DEFAULT 0,
    [IsMenuAddVisible] BIT DEFAULT 0,
    [IsMenuHomeVisible] BIT DEFAULT 0,
    [IsVisible] BIT DEFAULT 0,
    [NavigateUrl] NVARCHAR(1000),
    [HelpLinkId] NVARCHAR(255),
    [AssemblyName] NVARCHAR(50),
    [AssemblyClassName] NVARCHAR(200),
    [StoreProcedureName] NVARCHAR(200),
    [AccessKey] NCHAR(1),
    [IsActive] BIT DEFAULT 0,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_Programs] PRIMARY KEY ([ProgramId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Programs_ProgramCode' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Programs_ProgramCode] ON [core].[Programs] ([ProgramCode]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Programs_ParentId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Programs_ParentId] ON [core].[Programs] ([ParentId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Programs_TopLevelProgramId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Programs_TopLevelProgramId] ON [core].[Programs] ([TopLevelProgramId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Programs_IsProgramDirectVoid' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Programs_IsProgramDirectVoid] ON [core].[Programs] ([IsProgramDirectVoid]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Programs_HelpLinkId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Programs_HelpLinkId] ON [core].[Programs] ([HelpLinkId]);

-- ---------------------------------------------------------------------------
-- app.Company
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[Company];
CREATE TABLE [app].[Company]
(
    [CompanyId] BIGINT IDENTITY(1,1) NOT NULL,
    [CompanyGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CompanyName] NVARCHAR(200) NOT NULL,
    [InitialName] NVARCHAR(20),
    [TaxCompulsionNo] NVARCHAR(30),
    [RegistrationNo] NVARCHAR(30),
    [ParentCompanyId] BIGINT,
    [DefaultCurrency] NCHAR(3),
    [CompanyLogo] VARBINARY(MAX),
    [LeaderId] BIGINT,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_Company] PRIMARY KEY ([CompanyId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Company_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Company_Guid] ON [app].[Company] ([CompanyGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Company_Name' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Company_Name] ON [app].[Company] ([CompanyName]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Company_ParentCompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Company_ParentCompanyId] ON [app].[Company] ([ParentCompanyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Company_LeaderId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Company_LeaderId] ON [app].[Company] ([LeaderId]);

-- ---------------------------------------------------------------------------
-- core.Country
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [core].[Country];
CREATE TABLE [core].[Country]
(
    [CountryId] BIGINT IDENTITY(1,1) NOT NULL,
    [CountryGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CountryIso] NCHAR(2) NOT NULL,
    [CountryName] NVARCHAR(200) NOT NULL,
    [CountryIso3] NCHAR(3),
    [PhoneCode] INT NOT NULL,
    [CurrencyCode] NCHAR(3) NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_Country] PRIMARY KEY ([CountryId])
);

-- ---------------------------------------------------------------------------
-- core.Province
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [core].[Province];
CREATE TABLE [core].[Province]
(
    [ProvinceId] BIGINT IDENTITY(1,1) NOT NULL,
    [ProvinceGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ProvinceName] NVARCHAR(255) NOT NULL,
    [CountryId] BIGINT NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_Province] PRIMARY KEY ([ProvinceId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Province_CountryId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Province_CountryId] ON [core].[Province] ([CountryId]);

-- ---------------------------------------------------------------------------
-- core.City
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [core].[City];
CREATE TABLE [core].[City]
(
    [CityId] BIGINT IDENTITY(1,1) NOT NULL,
    [CityGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CityName] NVARCHAR(255) NOT NULL,
    [ProvinceId] BIGINT NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_City] PRIMARY KEY ([CityId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_City_ProvinceId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_City_ProvinceId] ON [core].[City] ([ProvinceId]);

-- ---------------------------------------------------------------------------
-- core.District
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [core].[District];
CREATE TABLE [core].[District]
(
    [DistrictId] BIGINT IDENTITY(1,1) NOT NULL,
    [DistrictGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [DistrictName] NVARCHAR(255) NOT NULL,
    [CityId] BIGINT NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_District] PRIMARY KEY ([DistrictId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_District_CityId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_District_CityId] ON [core].[District] ([CityId]);

-- ---------------------------------------------------------------------------
-- core.SubDistrict
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [core].[SubDistrict];
CREATE TABLE [core].[SubDistrict]
(
    [SubDistrictId] BIGINT IDENTITY(1,1) NOT NULL,
    [SubDistrictGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [SubDistrictName] NVARCHAR(255) NOT NULL,
    [DistrictId] BIGINT NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_SubDistrict] PRIMARY KEY ([SubDistrictId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SubDistrict_DistrictId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_SubDistrict_DistrictId] ON [core].[SubDistrict] ([DistrictId]);

-- ---------------------------------------------------------------------------
-- core.PostalCode
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [core].[PostalCode];
CREATE TABLE [core].[PostalCode]
(
    [PostalCodeId] BIGINT IDENTITY(1,1) NOT NULL,
    [PostalCodeGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [SubDistrictId] BIGINT NOT NULL,
    [PostalCode] NVARCHAR(10) NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_PostalCode] PRIMARY KEY ([PostalCodeId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PostalCode_SubDistrictId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PostalCode_SubDistrictId] ON [core].[PostalCode] ([SubDistrictId]);

-- ---------------------------------------------------------------------------
-- core.Bank
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [core].[Bank];
CREATE TABLE [core].[Bank]
(
    [BankId] BIGINT IDENTITY(1,1) NOT NULL,
    [BankGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [BankName] NVARCHAR(200) NOT NULL,
    [BankInitial] NVARCHAR(15) NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_Bank] PRIMARY KEY ([BankId])
);

-- ---------------------------------------------------------------------------
-- app.CompanyOffice
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[CompanyOffice];
CREATE TABLE [app].[CompanyOffice]
(
    [CompanyOfficeId] BIGINT IDENTITY(1,1) NOT NULL,
    [CompanyOfficeGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CompanyId] BIGINT NOT NULL,
    [CompanyGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CompanyOfficeName] NVARCHAR(150) NOT NULL,
    [SrAddressType] BIGINT NOT NULL,
    [CountryId] BIGINT NOT NULL,
    [StateId] BIGINT NOT NULL,
    [CityId] BIGINT NOT NULL,
    [PostalCodeId] NVARCHAR(8) NOT NULL,
    [Address] NVARCHAR(500) NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    CONSTRAINT [PK_CompanyOffice] PRIMARY KEY ([CompanyOfficeId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_CompanyOffice_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_CompanyOffice_Guid] ON [app].[CompanyOffice] ([CompanyOfficeGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CompanyOffice_CompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_CompanyOffice_CompanyId] ON [app].[CompanyOffice] ([CompanyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CompanyOffice_SrAddressType' AND object_id IS NOT NULL)
    CREATE INDEX [IX_CompanyOffice_SrAddressType] ON [app].[CompanyOffice] ([SrAddressType]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CompanyOffice_CountryId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_CompanyOffice_CountryId] ON [app].[CompanyOffice] ([CountryId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CompanyOffice_StateId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_CompanyOffice_StateId] ON [app].[CompanyOffice] ([StateId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CompanyOffice_CityId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_CompanyOffice_CityId] ON [app].[CompanyOffice] ([CityId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CompanyOffice_PostalCodeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_CompanyOffice_PostalCodeId] ON [app].[CompanyOffice] ([PostalCodeId]);

-- ---------------------------------------------------------------------------
-- core.UserGroupProgram
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [core].[UserGroupProgram];
CREATE TABLE [core].[UserGroupProgram]
(
    [UserGroupProgramId] BIGINT IDENTITY(1,1) NOT NULL,
    [UserGroupProgramGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [UserGroupId] BIGINT NOT NULL,
    [ProgramsId] BIGINT NOT NULL,
    [IsUserGroupAddAble] BIT DEFAULT 0,
    [IsUserGroupEditAble] BIT DEFAULT 0,
    [IsUserGroupDeleteAble] BIT DEFAULT 0,
    [IsUserGroupApprovalAble] BIT DEFAULT 0,
    [IsUserGroupUnApprovalAble] BIT DEFAULT 0,
    [IsUserGroupVoidAble] BIT DEFAULT 0,
    [IsUserGroupUnVoidAble] BIT DEFAULT 0,
    [IsUserGroupExportAble] BIT DEFAULT 0,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    [IsUserGroupViewAble] BIT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_UserGroupProgram] PRIMARY KEY ([UserGroupProgramId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_UserGroupProgram_Unique' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_UserGroupProgram_Unique] ON [core].[UserGroupProgram] ([UserGroupId], [ProgramsId]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_UserGroupProgram_UserGroupId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_UserGroupProgram_UserGroupId] ON [core].[UserGroupProgram] ([UserGroupId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_UserGroupProgram_ProgramsId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_UserGroupProgram_ProgramsId] ON [core].[UserGroupProgram] ([ProgramsId]);

-- ---------------------------------------------------------------------------
-- core.UserCompanyScope
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [core].[UserCompanyScope];
CREATE TABLE [core].[UserCompanyScope]
(
    [UserCompanyScopeId] BIGINT IDENTITY(1,1) NOT NULL,
    [UserCompanyScopeGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [UserId] BIGINT NOT NULL,
    [CompanyId] BIGINT NOT NULL,
    [CompanyOfficeId] BIGINT,
    [IsDefaultCompany] BIT NOT NULL DEFAULT 0,
    [IsDefaultOffice] BIT NOT NULL DEFAULT 0,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_UserCompanyScope] PRIMARY KEY ([UserCompanyScopeId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_UCS_CompanyOnly' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_UCS_CompanyOnly] ON [core].[UserCompanyScope] ([UserId], [CompanyId]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_UCS_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_UCS_Guid] ON [core].[UserCompanyScope] ([UserCompanyScopeGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_UCS_WithOffice' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_UCS_WithOffice] ON [core].[UserCompanyScope] ([UserId], [CompanyId], [CompanyOfficeId]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_UserCompanyScope_UserId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_UserCompanyScope_UserId] ON [core].[UserCompanyScope] ([UserId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_UserCompanyScope_CompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_UserCompanyScope_CompanyId] ON [core].[UserCompanyScope] ([CompanyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_UserCompanyScope_CompanyOfficeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_UserCompanyScope_CompanyOfficeId] ON [core].[UserCompanyScope] ([CompanyOfficeId]);

-- ---------------------------------------------------------------------------
-- core.UserGroupScope
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [core].[UserGroupScope];
CREATE TABLE [core].[UserGroupScope]
(
    [UserGroupScopeId] BIGINT IDENTITY(1,1) NOT NULL,
    [UserGroupScopeGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [UserId] BIGINT NOT NULL,
    [UserGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [UserGroupId] BIGINT NOT NULL,
    [UserGroupGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CompanyId] BIGINT NOT NULL,
    [CompanyGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CompanyOfficeId] BIGINT,
    [CompanyOfficeGuid] UNIQUEIDENTIFIER,
    [PersonId] BIGINT,
    [IsDefault] BIT DEFAULT 0,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_UserGroupScope] PRIMARY KEY ([UserGroupScopeId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_UGS_CompanyLevel' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_UGS_CompanyLevel] ON [core].[UserGroupScope] ([UserId], [UserGroupId], [CompanyId]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_UGS_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_UGS_Guid] ON [core].[UserGroupScope] ([UserGroupScopeGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_UGS_WithOffice' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_UGS_WithOffice] ON [core].[UserGroupScope] ([UserId], [UserGroupId], [CompanyId], [CompanyOfficeId]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_UserGroupScope_UserId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_UserGroupScope_UserId] ON [core].[UserGroupScope] ([UserId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_UserGroupScope_UserGroupId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_UserGroupScope_UserGroupId] ON [core].[UserGroupScope] ([UserGroupId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_UserGroupScope_CompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_UserGroupScope_CompanyId] ON [core].[UserGroupScope] ([CompanyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_UserGroupScope_CompanyOfficeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_UserGroupScope_CompanyOfficeId] ON [core].[UserGroupScope] ([CompanyOfficeId]);

-- ---------------------------------------------------------------------------
-- app.StandardReference
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[StandardReference];
CREATE TABLE [app].[StandardReference]
(
    [StandardReferenceId] BIGINT IDENTITY(1,1) NOT NULL,
    [StandardReferenceGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [StandardReferenceName] NVARCHAR(100) NOT NULL,
    [StandardReferenceInitial] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(500),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_StandardReference] PRIMARY KEY ([StandardReferenceId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_StandardReference_StandardReferenceInitial' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_StandardReference_StandardReferenceInitial] ON [app].[StandardReference] ([StandardReferenceInitial]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);

-- ---------------------------------------------------------------------------
-- app.StandardReferenceItem
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[StandardReferenceItem];
CREATE TABLE [app].[StandardReferenceItem]
(
    [StandardReferenceItemId] BIGINT IDENTITY(1,1) NOT NULL,
    [StandardReferenceItemGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [StandardReferenceId] BIGINT NOT NULL,
    [StandardReferenceGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [StandardReferenceItemInitial] NVARCHAR(50) NOT NULL,
    [StandardReferenceItemName] NVARCHAR(100) NOT NULL,
    [StandardReferenceItemValue] NVARCHAR(100),
    [DisplayOrder] INT NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_StandardReferenceItem] PRIMARY KEY ([StandardReferenceItemId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StandardReferenceItem_StandardReferenceId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_StandardReferenceItem_StandardReferenceId] ON [app].[StandardReferenceItem] ([StandardReferenceId]);

-- ---------------------------------------------------------------------------
-- app.StandardReferenceScope
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[StandardReferenceScope];
CREATE TABLE [app].[StandardReferenceScope]
(
    [StandardReferenceScopeId] BIGINT IDENTITY(1,1) NOT NULL,
    [StandardReferenceScopeGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [StandardReferenceId] BIGINT NOT NULL,
    [StandardReferenceGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CompanyId] BIGINT,
    [CompanyGuid] UNIQUEIDENTIFIER,
    [CompanyOfficeId] BIGINT,
    [CompanyOfficeGuid] UNIQUEIDENTIFIER,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_StandardReferenceScope] PRIMARY KEY ([StandardReferenceScopeId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StandardReferenceScope_StandardReferenceId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_StandardReferenceScope_StandardReferenceId] ON [app].[StandardReferenceScope] ([StandardReferenceId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StandardReferenceScope_CompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_StandardReferenceScope_CompanyId] ON [app].[StandardReferenceScope] ([CompanyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StandardReferenceScope_CompanyOfficeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_StandardReferenceScope_CompanyOfficeId] ON [app].[StandardReferenceScope] ([CompanyOfficeId]);

-- ---------------------------------------------------------------------------
-- app.StandardReferenceScopeItem
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[StandardReferenceScopeItem];
CREATE TABLE [app].[StandardReferenceScopeItem]
(
    [StandardReferenceScopeItemId] BIGINT IDENTITY(1,1) NOT NULL,
    [StandardReferenceScopeItemGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [StandardReferenceScopeId] BIGINT NOT NULL,
    [StandardReferenceScopeGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [StandardReferenceScopeItemInitial] NVARCHAR(50) NOT NULL,
    [StandardReferenceScopeItemName] NVARCHAR(100) NOT NULL,
    [StandardReferenceScopeItemValue] NVARCHAR(100),
    [DisplayOrder] INT NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_StandardReferenceScopeItem] PRIMARY KEY ([StandardReferenceScopeItemId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StandardReferenceScopeItem_StandardReferenceScopeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_StandardReferenceScopeItem_StandardReferenceScopeId] ON [app].[StandardReferenceScopeItem] ([StandardReferenceScopeId]);

-- ---------------------------------------------------------------------------
-- app.Parameter
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[Parameter];
CREATE TABLE [app].[Parameter]
(
    [ParameterId] BIGINT IDENTITY(1,1) NOT NULL,
    [ParameterGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ParameterName] NVARCHAR(50) NOT NULL,
    [ParameterValue] NVARCHAR(100) NOT NULL,
    [ParameterType] NVARCHAR(10),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_Parameter] PRIMARY KEY ([ParameterId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Parameter_ParameterName' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Parameter_ParameterName] ON [app].[Parameter] ([ParameterName]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);

-- ---------------------------------------------------------------------------
-- app.ParameterScope
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[ParameterScope];
CREATE TABLE [app].[ParameterScope]
(
    [ParameterScopeId] BIGINT IDENTITY(1,1) NOT NULL,
    [ParameterScopeGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ParameterId] BIGINT NOT NULL,
    [CompanyId] BIGINT,
    [CompanyGuid] UNIQUEIDENTIFIER,
    [CompanyOfficeId] BIGINT,
    [CompanyOfficeGuid] UNIQUEIDENTIFIER,
    [OverrideValue] NVARCHAR(100) NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    [ParameterGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    CONSTRAINT [PK_ParameterScope] PRIMARY KEY ([ParameterScopeId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_ParameterScope_Company' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_ParameterScope_Company] ON [app].[ParameterScope] ([ParameterId], [CompanyId]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_ParameterScope_Office' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_ParameterScope_Office] ON [app].[ParameterScope] ([ParameterId], [CompanyId], [CompanyOfficeId]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ParameterScope_ParameterId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ParameterScope_ParameterId] ON [app].[ParameterScope] ([ParameterId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ParameterScope_CompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ParameterScope_CompanyId] ON [app].[ParameterScope] ([CompanyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ParameterScope_CompanyOfficeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ParameterScope_CompanyOfficeId] ON [app].[ParameterScope] ([CompanyOfficeId]);

-- ---------------------------------------------------------------------------
-- app.OrganizationUnit
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[OrganizationUnit];
CREATE TABLE [app].[OrganizationUnit]
(
    [OrganizationUnitId] BIGINT IDENTITY(1,1) NOT NULL,
    [OrganizationUnitGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [OrganizationUnitCode] NVARCHAR(20) NOT NULL,
    [OrganizationUnitName] NVARCHAR(200) NOT NULL,
    [ParentOrganizationUnitId] BIGINT,
    [SrOrganizationLevel] BIGINT NOT NULL,
    [LevelDepth] INT NOT NULL,
    [HierarchyPath] NVARCHAR(500),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_OrganizationUnit] PRIMARY KEY ([OrganizationUnitId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_OrganizationUnit_Code' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_OrganizationUnit_Code] ON [app].[OrganizationUnit] ([OrganizationUnitCode]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_OrganizationUnit_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_OrganizationUnit_Guid] ON [app].[OrganizationUnit] ([OrganizationUnitGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OrganizationUnit_ParentOrganizationUnitId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_OrganizationUnit_ParentOrganizationUnitId] ON [app].[OrganizationUnit] ([ParentOrganizationUnitId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OrganizationUnit_SrOrganizationLevel' AND object_id IS NOT NULL)
    CREATE INDEX [IX_OrganizationUnit_SrOrganizationLevel] ON [app].[OrganizationUnit] ([SrOrganizationLevel]);

-- ---------------------------------------------------------------------------
-- app.OrganizationUnitScope
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[OrganizationUnitScope];
CREATE TABLE [app].[OrganizationUnitScope]
(
    [OrganizationUnitScopeId] BIGINT IDENTITY(1,1) NOT NULL,
    [OrganizationUnitScopeGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [OrganizationUnitId] BIGINT NOT NULL,
    [CompanyId] BIGINT NOT NULL,
    [CompanyOfficeId] BIGINT,
    [ScopeType] NVARCHAR(20) NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_OrganizationUnitScope] PRIMARY KEY ([OrganizationUnitScopeId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_OrganizationUnitScope_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_OrganizationUnitScope_Guid] ON [app].[OrganizationUnitScope] ([OrganizationUnitScopeGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_OrgScope_Main' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_OrgScope_Main] ON [app].[OrganizationUnitScope] ([OrganizationUnitId], [CompanyId], [CompanyOfficeId]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OrganizationUnitScope_OrganizationUnitId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_OrganizationUnitScope_OrganizationUnitId] ON [app].[OrganizationUnitScope] ([OrganizationUnitId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OrganizationUnitScope_CompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_OrganizationUnitScope_CompanyId] ON [app].[OrganizationUnitScope] ([CompanyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OrganizationUnitScope_CompanyOfficeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_OrganizationUnitScope_CompanyOfficeId] ON [app].[OrganizationUnitScope] ([CompanyOfficeId]);

-- ---------------------------------------------------------------------------
-- app.AutoNumber
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[AutoNumber];
CREATE TABLE [app].[AutoNumber]
(
    [AutoNumberId] BIGINT IDENTITY(1,1) NOT NULL,
    [AutoNumberGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Prefik] NVARCHAR(7) NOT NULL,
    [SeparatorAfterPrefik] NVARCHAR(1) NOT NULL,
    [IsUsedDepartment] BIT DEFAULT 0,
    [SeparatorAfterDept] NVARCHAR(1) NOT NULL,
    [IsUsedYear] BIT DEFAULT 0,
    [YearDigit] SMALLINT,
    [SeparatorAfterYear] NVARCHAR(1) NOT NULL,
    [IsUsedMonth] BIT DEFAULT 0,
    [IsMonthInRomawi] BIT DEFAULT 0,
    [SeparatorAfterMonth] NVARCHAR(1) NOT NULL,
    [IsUsedDay] BIT DEFAULT 0,
    [SeparatorAfterDay] NVARCHAR(1) NOT NULL,
    [NumberLength] SMALLINT,
    [NumberGroupLength] SMALLINT,
    [NumberGroupSeparator] NVARCHAR(1) NOT NULL,
    [NumberFormat] NVARCHAR(20) NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_AutoNumber] PRIMARY KEY ([AutoNumberId]),
    CONSTRAINT [UQ_AutoNumber_AutoNumberGuid] UNIQUE ([AutoNumberGuid])
);

-- ---------------------------------------------------------------------------
-- app.AutoNumberTemplate
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[AutoNumberTemplate];
CREATE TABLE [app].[AutoNumberTemplate]
(
    [AutoNumberTemplateId] BIGINT IDENTITY(1,1) NOT NULL,
    [AutoNumberTemplateGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [TemplateName] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(200),
    [EffectiveDate] DATETIME2(7) NOT NULL,
    [TemplateScopeType] NVARCHAR(20) NOT NULL,
    [SrFormMappingNumbering] BIGINT NOT NULL,
    [CompanyId] BIGINT,
    [CompanyOfficeId] BIGINT,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_AutoNumberTemplate] PRIMARY KEY ([AutoNumberTemplateId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_AutoNumberTemplate_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_AutoNumberTemplate_Guid] ON [app].[AutoNumberTemplate] ([AutoNumberTemplateGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AutoNumberTemplate_EffectiveDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AutoNumberTemplate_EffectiveDate_Active] ON [app].[AutoNumberTemplate] ([EffectiveDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AutoNumberTemplate_SrFormMappingNumbering' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AutoNumberTemplate_SrFormMappingNumbering] ON [app].[AutoNumberTemplate] ([SrFormMappingNumbering]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AutoNumberTemplate_CompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AutoNumberTemplate_CompanyId] ON [app].[AutoNumberTemplate] ([CompanyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AutoNumberTemplate_CompanyOfficeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AutoNumberTemplate_CompanyOfficeId] ON [app].[AutoNumberTemplate] ([CompanyOfficeId]);

-- ---------------------------------------------------------------------------
-- app.AutoNumberComponent
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[AutoNumberComponent];
CREATE TABLE [app].[AutoNumberComponent]
(
    [AutoNumberComponentId] BIGINT IDENTITY(1,1) NOT NULL,
    [AutoNumberComponentGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [AutoNumberTemplateId] BIGINT NOT NULL,
    [SeqNo] SMALLINT NOT NULL,
    [ComponentType] NVARCHAR(20) NOT NULL,
    [StaticValue] NVARCHAR(50),
    [Format] NVARCHAR(20),
    [CounterLength] SMALLINT,
    [IsResetKey] BIT NOT NULL DEFAULT 0,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_AutoNumberComponent] PRIMARY KEY ([AutoNumberComponentId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_AutoNumberComponent_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_AutoNumberComponent_Guid] ON [app].[AutoNumberComponent] ([AutoNumberComponentGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_AutoNumberComponent_Seq' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_AutoNumberComponent_Seq] ON [app].[AutoNumberComponent] ([AutoNumberTemplateId], [SeqNo]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AutoNumberComponent_AutoNumberTemplateId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AutoNumberComponent_AutoNumberTemplateId] ON [app].[AutoNumberComponent] ([AutoNumberTemplateId]);

-- ---------------------------------------------------------------------------
-- app.AutoNumberCounter
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[AutoNumberCounter];
CREATE TABLE [app].[AutoNumberCounter]
(
    [AutoNumberCounterId] BIGINT IDENTITY(1,1) NOT NULL,
    [AutoNumberCounterGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [AutoNumberTemplateId] BIGINT NOT NULL,
    [CompanyId] BIGINT,
    [CompanyOfficeId] BIGINT,
    [OrganizationUnitId] INT,
    [YearNo] INT,
    [MonthNo] INT,
    [DayNo] INT,
    [LastNumber] INT NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_AutoNumberCounter] PRIMARY KEY ([AutoNumberCounterId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AutoNumberCounter_Scope' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [IX_AutoNumberCounter_Scope] ON [app].[AutoNumberCounter] ([AutoNumberTemplateId], [CompanyId], [CompanyOfficeId], [OrganizationUnitId], [YearNo], [MonthNo], [DayNo]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_AutoNumberCounter_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_AutoNumberCounter_Guid] ON [app].[AutoNumberCounter] ([AutoNumberCounterGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AutoNumberCounter_AutoNumberTemplateId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AutoNumberCounter_AutoNumberTemplateId] ON [app].[AutoNumberCounter] ([AutoNumberTemplateId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AutoNumberCounter_CompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AutoNumberCounter_CompanyId] ON [app].[AutoNumberCounter] ([CompanyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AutoNumberCounter_CompanyOfficeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AutoNumberCounter_CompanyOfficeId] ON [app].[AutoNumberCounter] ([CompanyOfficeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AutoNumberCounter_OrganizationUnitId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AutoNumberCounter_OrganizationUnitId] ON [app].[AutoNumberCounter] ([OrganizationUnitId]);

-- ---------------------------------------------------------------------------
-- app.AutoNumberLog
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[AutoNumberLog];
CREATE TABLE [app].[AutoNumberLog]
(
    [AutoNumberLogId] BIGINT IDENTITY(1,1) NOT NULL,
    [AutoNumberLogGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [AutoNumberTemplateId] BIGINT NOT NULL,
    [GeneratedNumber] NVARCHAR(200) NOT NULL,
    [CounterValue] INT NOT NULL,
    [Status] NVARCHAR(20) NOT NULL,
    [CompanyId] BIGINT,
    [CompanyOfficeId] BIGINT,
    [OrganizationUnitId] BIGINT,
    [YearNo] INT,
    [MonthNo] INT,
    [DayNo] INT,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [ReferenceId] NVARCHAR(100),
    CONSTRAINT [PK_AutoNumberLog] PRIMARY KEY ([AutoNumberLogId])
);

-- AutoNumberLog is append-only (no StatusId/DeletedTime) — unique Guid without soft-delete filter.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_AutoNumberLog_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_AutoNumberLog_Guid] ON [app].[AutoNumberLog] ([AutoNumberLogGuid]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AutoNumberLog_AutoNumberTemplateId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AutoNumberLog_AutoNumberTemplateId] ON [app].[AutoNumberLog] ([AutoNumberTemplateId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AutoNumberLog_CompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AutoNumberLog_CompanyId] ON [app].[AutoNumberLog] ([CompanyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AutoNumberLog_CompanyOfficeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AutoNumberLog_CompanyOfficeId] ON [app].[AutoNumberLog] ([CompanyOfficeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AutoNumberLog_OrganizationUnitId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AutoNumberLog_OrganizationUnitId] ON [app].[AutoNumberLog] ([OrganizationUnitId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AutoNumberLog_ReferenceId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AutoNumberLog_ReferenceId] ON [app].[AutoNumberLog] ([ReferenceId]);

-- ---------------------------------------------------------------------------
-- app.DocumentNumberRequest
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[DocumentNumberRequest];
CREATE TABLE [app].[DocumentNumberRequest]
(
    [DocumentNumberRequestId] BIGINT IDENTITY(1,1) NOT NULL,
    [DocumentNumberRequestGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [DocumentType] NVARCHAR(50) NOT NULL,
    [DocumentNo] NVARCHAR(100) NOT NULL,
    [AutoNumberLogId] BIGINT NOT NULL,
    [ExternalReference] NVARCHAR(100),
    [Description] NVARCHAR(255),
    [Status] NVARCHAR(20) NOT NULL,
    [CompanyId] BIGINT,
    [OfficeId] BIGINT,
    [OrgUnitId] BIGINT,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    CONSTRAINT [PK_DocumentNumberRequest] PRIMARY KEY ([DocumentNumberRequestId])
);

-- DocumentNumberRequest has Status (nvarchar), not StatusId/DeletedTime.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_DocumentNumberRequest_DocumentNo' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_DocumentNumberRequest_DocumentNo] ON [app].[DocumentNumberRequest] ([DocumentNo]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_DocumentNumberRequest_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_DocumentNumberRequest_Guid] ON [app].[DocumentNumberRequest] ([DocumentNumberRequestGuid]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_DocumentNumberRequest_AutoNumberLogId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_DocumentNumberRequest_AutoNumberLogId] ON [app].[DocumentNumberRequest] ([AutoNumberLogId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_DocumentNumberRequest_CompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_DocumentNumberRequest_CompanyId] ON [app].[DocumentNumberRequest] ([CompanyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_DocumentNumberRequest_OfficeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_DocumentNumberRequest_OfficeId] ON [app].[DocumentNumberRequest] ([OfficeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_DocumentNumberRequest_OrgUnitId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_DocumentNumberRequest_OrgUnitId] ON [app].[DocumentNumberRequest] ([OrgUnitId]);

-- ---------------------------------------------------------------------------
-- app.Hospital
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[Hospital];
CREATE TABLE [app].[Hospital]
(
    [HospitalId] BIGINT IDENTITY(1,1) NOT NULL,
    [HospitalGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [HospitalName] NVARCHAR(200) NOT NULL,
    [HospitalCode] NVARCHAR(20),
    [ShortName] NVARCHAR(100),
    [LicenseNo] NVARCHAR(50),
    [HospitalType] NVARCHAR(30),
    [HospitalClass] NVARCHAR(10),
    [PhoneNo] NVARCHAR(30),
    [Email] NVARCHAR(200),
    [Website] NVARCHAR(200),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_Hospital] PRIMARY KEY ([HospitalId]),
    CONSTRAINT [UQ_Hospital_HospitalGuid] UNIQUE ([HospitalGuid]),
    CONSTRAINT [UQ_Hospital_Code] UNIQUE ([HospitalCode]),
    CONSTRAINT [UQ_Hospital_Name] UNIQUE ([HospitalName])
);

-- ---------------------------------------------------------------------------
-- app.Skill
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[Skill];
CREATE TABLE [app].[Skill]
(
    [SkillId] BIGINT IDENTITY(1,1) NOT NULL,
    [SkillGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [SkillCode] NVARCHAR(30) NOT NULL,
    [SkillName] NVARCHAR(200) NOT NULL,
    [SkillShortName] NVARCHAR(100),
    [SrSkillCategory] BIGINT NOT NULL,
    [SrSkillType] BIGINT,
    [SrDefaultSkillLevel] BIGINT,
    [IsCertificationRequired] BIT NOT NULL DEFAULT 0,
    [IsMandatory] BIT NOT NULL DEFAULT 0,
    [FileStorageId] BIGINT,
    [Description] NVARCHAR(MAX),
    [IsActive] BIT NOT NULL DEFAULT 0,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_Skill] PRIMARY KEY ([SkillId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Skill_Code' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [IX_Skill_Code] ON [app].[Skill] ([SkillCode]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Skill_Code' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Skill_Code] ON [app].[Skill] ([SkillCode]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Skill_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Skill_Guid] ON [app].[Skill] ([SkillGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Skill_SrSkillCategory' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Skill_SrSkillCategory] ON [app].[Skill] ([SrSkillCategory]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Skill_SrSkillType' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Skill_SrSkillType] ON [app].[Skill] ([SrSkillType]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Skill_SrDefaultSkillLevel' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Skill_SrDefaultSkillLevel] ON [app].[Skill] ([SrDefaultSkillLevel]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Skill_FileStorageId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Skill_FileStorageId] ON [app].[Skill] ([FileStorageId]);

-- ---------------------------------------------------------------------------
-- app.Certification
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[Certification];
CREATE TABLE [app].[Certification]
(
    [CertificationId] BIGINT IDENTITY(1,1) NOT NULL,
    [CertificationGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CertificationCode] NVARCHAR(30) NOT NULL,
    [CertificationName] NVARCHAR(200) NOT NULL,
    [CertificationShortName] NVARCHAR(100),
    [SrCertificationCategory] BIGINT NOT NULL,
    [SrCertificationProvider] BIGINT NOT NULL,
    [DefaultValidityMonth] INT,
    [IsLifetime] BIT NOT NULL DEFAULT 0,
    [IsRenewable] BIT NOT NULL DEFAULT 0,
    [LogoFileStorageId] BIGINT,
    [Description] NVARCHAR(1000),
    [IsActive] BIT NOT NULL DEFAULT 0,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_Certification] PRIMARY KEY ([CertificationId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Certification_Code' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Certification_Code] ON [app].[Certification] ([CertificationCode]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Certification_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Certification_Guid] ON [app].[Certification] ([CertificationGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Certification_SrCertificationCategory' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Certification_SrCertificationCategory] ON [app].[Certification] ([SrCertificationCategory]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Certification_SrCertificationProvider' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Certification_SrCertificationProvider] ON [app].[Certification] ([SrCertificationProvider]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Certification_LogoFileStorageId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Certification_LogoFileStorageId] ON [app].[Certification] ([LogoFileStorageId]);

-- ---------------------------------------------------------------------------
-- app.Asset
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[Asset];
CREATE TABLE [app].[Asset]
(
    [AssetId] BIGINT IDENTITY(1,1) NOT NULL,
    [AssetGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [AssetCode] NVARCHAR(30) NOT NULL,
    [AssetName] NVARCHAR(200) NOT NULL,
    [AssetCategoryId] BIGINT NOT NULL,
    [AssetBrand] NVARCHAR(100),
    [AssetModel] NVARCHAR(100),
    [SerialNumber] NVARCHAR(100),
    [Barcode] NVARCHAR(100),
    [CompanyId] BIGINT NOT NULL,
    [CompanyOfficeId] BIGINT,
    [CostCenterId] BIGINT,
    [PurchaseDate] DATE,
    [PurchasePrice] DECIMAL(18,2),
    [CurrencyId] BIGINT,
    [WarrantyStartDate] DATE,
    [WarrantyEndDate] DATE,
    [SrAssetCondition] BIGINT,
    [SrAssetStatus] BIGINT NOT NULL,
    [CurrentLocation] NVARCHAR(200),
    [FileStorageId] BIGINT,
    [Description] NVARCHAR(1000),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_Asset] PRIMARY KEY ([AssetId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Asset_Code' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Asset_Code] ON [app].[Asset] ([AssetCode]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Asset_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Asset_Guid] ON [app].[Asset] ([AssetGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Asset_AssetCategoryId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Asset_AssetCategoryId] ON [app].[Asset] ([AssetCategoryId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Asset_CompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Asset_CompanyId] ON [app].[Asset] ([CompanyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Asset_CompanyOfficeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Asset_CompanyOfficeId] ON [app].[Asset] ([CompanyOfficeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Asset_CostCenterId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Asset_CostCenterId] ON [app].[Asset] ([CostCenterId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Asset_PurchaseDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Asset_PurchaseDate_Active] ON [app].[Asset] ([PurchaseDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Asset_CurrencyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Asset_CurrencyId] ON [app].[Asset] ([CurrencyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Asset_WarrantyStartDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Asset_WarrantyStartDate_Active] ON [app].[Asset] ([WarrantyStartDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Asset_WarrantyEndDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Asset_WarrantyEndDate_Active] ON [app].[Asset] ([WarrantyEndDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Asset_SrAssetCondition' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Asset_SrAssetCondition] ON [app].[Asset] ([SrAssetCondition]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Asset_SrAssetStatus' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Asset_SrAssetStatus] ON [app].[Asset] ([SrAssetStatus]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Asset_FileStorageId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Asset_FileStorageId] ON [app].[Asset] ([FileStorageId]);

-- ---------------------------------------------------------------------------
-- app.Training
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[Training];
CREATE TABLE [app].[Training]
(
    [TrainingId] BIGINT IDENTITY(1,1) NOT NULL,
    [TrainingGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [TrainingCode] NVARCHAR(30) NOT NULL,
    [TrainingName] NVARCHAR(200) NOT NULL,
    [TrainingShortName] NVARCHAR(100),
    [SrTrainingCategory] BIGINT NOT NULL,
    [SrTrainingType] BIGINT NOT NULL,
    [SrTrainingLevel] BIGINT,
    [DurationHour] DECIMAL(8,2),
    [ValidityMonth] INT,
    [SrTrainingProvider] BIGINT,
    [IsMandatory] BIT NOT NULL DEFAULT 0,
    [IsCertification] BIT NOT NULL DEFAULT 0,
    [IsExternal] BIT NOT NULL DEFAULT 0,
    [FileStorageId] BIGINT,
    [Description] NVARCHAR(MAX),
    [IsActive] BIT NOT NULL DEFAULT 0,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_Training] PRIMARY KEY ([TrainingId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Training_Code' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [IX_Training_Code] ON [app].[Training] ([TrainingCode]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Training_Code' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Training_Code] ON [app].[Training] ([TrainingCode]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Training_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Training_Guid] ON [app].[Training] ([TrainingGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Training_SrTrainingCategory' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Training_SrTrainingCategory] ON [app].[Training] ([SrTrainingCategory]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Training_SrTrainingType' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Training_SrTrainingType] ON [app].[Training] ([SrTrainingType]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Training_SrTrainingLevel' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Training_SrTrainingLevel] ON [app].[Training] ([SrTrainingLevel]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Training_SrTrainingProvider' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Training_SrTrainingProvider] ON [app].[Training] ([SrTrainingProvider]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Training_FileStorageId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Training_FileStorageId] ON [app].[Training] ([FileStorageId]);

-- ---------------------------------------------------------------------------
-- app.Holiday
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[Holiday];
CREATE TABLE [app].[Holiday]
(
    [HolidayId] BIGINT IDENTITY(1,1) NOT NULL,
    [HolidayGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [HolidayDate] DATE NOT NULL,
    [HolidayName] NVARCHAR(200) NOT NULL,
    [HolidayDescription] NVARCHAR(500),
    [SrHolidayType] BIGINT,
    [IsNational] BIT NOT NULL DEFAULT 1,
    [IsCompanyHoliday] BIT NOT NULL DEFAULT 0,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_Holiday] PRIMARY KEY ([HolidayId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Holiday_HolidayGuid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Holiday_HolidayGuid] ON [app].[Holiday] ([HolidayGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Holiday_HolidayDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Holiday_HolidayDate_Active] ON [app].[Holiday] ([HolidayDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Holiday_SrHolidayType' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Holiday_SrHolidayType] ON [app].[Holiday] ([SrHolidayType]);

-- ---------------------------------------------------------------------------
-- app.Award
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[Award];
CREATE TABLE [app].[Award]
(
    [AwardId] BIGINT IDENTITY(1,1) NOT NULL,
    [AwardGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [AwardCode] NVARCHAR(20) NOT NULL,
    [AwardName] NVARCHAR(400) NOT NULL,
    [SrAwardType] BIGINT NOT NULL,
    [ValidFrom] DATE NOT NULL,
    [ValidTo] DATE NOT NULL,
    [AwardPrize] NVARCHAR(200),
    [Note] NVARCHAR(4000),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_Award] PRIMARY KEY ([AwardId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Award_Code' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Award_Code] ON [app].[Award] ([AwardCode]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Award_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Award_Guid] ON [app].[Award] ([AwardGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Award_SrAwardType' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Award_SrAwardType] ON [app].[Award] ([SrAwardType]);

-- ---------------------------------------------------------------------------
-- app.AwardCriteria
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[AwardCriteria];
CREATE TABLE [app].[AwardCriteria]
(
    [AwardCriteriaId] BIGINT IDENTITY(1,1) NOT NULL,
    [AwardCriteriaGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [AwardId] BIGINT NOT NULL,
    [LineNo] INT NOT NULL,
    [CriteriaCode] NVARCHAR(20) NOT NULL,
    [CriteriaName] NVARCHAR(400) NOT NULL,
    [MinimumScore] DECIMAL(18,2),
    [MaximumScore] DECIMAL(18,2),
    [Weight] DECIMAL(18,2),
    [Note] NVARCHAR(4000),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    [AwardGuid] UNIQUEIDENTIFIER,
    CONSTRAINT [PK_AwardCriteria] PRIMARY KEY ([AwardCriteriaId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_AwardCriteria_Award_LineNo' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_AwardCriteria_Award_LineNo] ON [app].[AwardCriteria] ([AwardId], [LineNo]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_AwardCriteria_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_AwardCriteria_Guid] ON [app].[AwardCriteria] ([AwardCriteriaGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AwardCriteria_AwardId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AwardCriteria_AwardId] ON [app].[AwardCriteria] ([AwardId]);

-- ---------------------------------------------------------------------------
-- app.AwardCriteriaRule
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[AwardCriteriaRule];
CREATE TABLE [app].[AwardCriteriaRule]
(
    [AwardCriteriaRuleId] BIGINT IDENTITY(1,1) NOT NULL,
    [AwardCriteriaRuleGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [AwardCriteriaId] BIGINT NOT NULL,
    [RuleCode] NVARCHAR(20) NOT NULL,
    [RuleName] NVARCHAR(400) NOT NULL,
    [Operator] NVARCHAR(20) NOT NULL,
    [ValueFrom] DECIMAL(18,2),
    [ValueTo] DECIMAL(18,2),
    [Score] DECIMAL(18,2),
    [Note] NVARCHAR(4000),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    [AwardCriteriaGuid] UNIQUEIDENTIFIER,
    CONSTRAINT [PK_AwardCriteriaRule] PRIMARY KEY ([AwardCriteriaRuleId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_AwardCriteriaRule_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_AwardCriteriaRule_Guid] ON [app].[AwardCriteriaRule] ([AwardCriteriaRuleGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AwardCriteriaRule_AwardCriteriaId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AwardCriteriaRule_AwardCriteriaId] ON [app].[AwardCriteriaRule] ([AwardCriteriaId]);

-- ---------------------------------------------------------------------------
-- app.CostCenter
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[CostCenter];
CREATE TABLE [app].[CostCenter]
(
    [CostCenterId] BIGINT IDENTITY(1,1) NOT NULL,
    [CostCenterGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CostCenterCode] NVARCHAR(20) NOT NULL,
    [CostCenterName] NVARCHAR(200) NOT NULL,
    [CostCenterDescription] NVARCHAR(500),
    [ParentCostCenterId] BIGINT,
    [LevelDepth] INT NOT NULL,
    [HierarchyPath] NVARCHAR(500),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_CostCenter] PRIMARY KEY ([CostCenterId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_CostCenterCode' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_CostCenterCode] ON [app].[CostCenter] ([CostCenterCode]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_CostCenterGuid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_CostCenterGuid] ON [app].[CostCenter] ([CostCenterGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CostCenter_ParentCostCenterId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_CostCenter_ParentCostCenterId] ON [app].[CostCenter] ([ParentCostCenterId]);

-- ---------------------------------------------------------------------------
-- app.CostCenterScope
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[CostCenterScope];
CREATE TABLE [app].[CostCenterScope]
(
    [CostCenterScopeId] BIGINT IDENTITY(1,1) NOT NULL,
    [CostCenterScopeGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CostCenterId] BIGINT NOT NULL,
    [CompanyId] BIGINT NOT NULL,
    [CompanyOfficeId] BIGINT,
    [ScopeType] NVARCHAR(20) NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_CostCenterScope] PRIMARY KEY ([CostCenterScopeId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_CostCenterScopeGuid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_CostCenterScopeGuid] ON [app].[CostCenterScope] ([CostCenterScopeGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CostCenterScope_CostCenterId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_CostCenterScope_CostCenterId] ON [app].[CostCenterScope] ([CostCenterId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CostCenterScope_CompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_CostCenterScope_CompanyId] ON [app].[CostCenterScope] ([CompanyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CostCenterScope_CompanyOfficeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_CostCenterScope_CompanyOfficeId] ON [app].[CostCenterScope] ([CompanyOfficeId]);

-- ---------------------------------------------------------------------------
-- app.CostCenterAssignment
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[CostCenterAssignment];
CREATE TABLE [app].[CostCenterAssignment]
(
    [CostCenterAssignmentId] BIGINT IDENTITY(1,1) NOT NULL,
    [CostCenterAssignmentGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [CostCenterId] BIGINT NOT NULL,
    [CompanyId] BIGINT NOT NULL,
    [CompanyOfficeId] BIGINT,
    [ProfitCenterId] BIGINT,
    [CostCenterManagerEmployeeId] BIGINT,
    [BudgetControlType] SMALLINT NOT NULL,
    [EffectiveDate] DATE NOT NULL,
    [ExpiredDate] DATE,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_CostCenterAssignment] PRIMARY KEY ([CostCenterAssignmentId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_CostCenterAssignmentGuid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_CostCenterAssignmentGuid] ON [app].[CostCenterAssignment] ([CostCenterAssignmentGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CostCenterAssignment_CostCenterId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_CostCenterAssignment_CostCenterId] ON [app].[CostCenterAssignment] ([CostCenterId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CostCenterAssignment_CompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_CostCenterAssignment_CompanyId] ON [app].[CostCenterAssignment] ([CompanyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CostCenterAssignment_CompanyOfficeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_CostCenterAssignment_CompanyOfficeId] ON [app].[CostCenterAssignment] ([CompanyOfficeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CostCenterAssignment_ProfitCenterId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_CostCenterAssignment_ProfitCenterId] ON [app].[CostCenterAssignment] ([ProfitCenterId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CostCenterAssignment_CostCenterManagerEmployeeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_CostCenterAssignment_CostCenterManagerEmployeeId] ON [app].[CostCenterAssignment] ([CostCenterManagerEmployeeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CostCenterAssignment_EffectiveDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_CostCenterAssignment_EffectiveDate_Active] ON [app].[CostCenterAssignment] ([EffectiveDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CostCenterAssignment_ExpiredDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_CostCenterAssignment_ExpiredDate_Active] ON [app].[CostCenterAssignment] ([ExpiredDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);

-- ---------------------------------------------------------------------------
-- app.FileStorage
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[FileStorage];
CREATE TABLE [app].[FileStorage]
(
    [FileStorageId] BIGINT IDENTITY(1,1) NOT NULL,
    [FileStorageGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [FileStorageName] NVARCHAR(300) NOT NULL,
    [StoredFileName] NVARCHAR(300) NOT NULL,
    [Extension] NVARCHAR(20) NOT NULL,
    [MimeType] NVARCHAR(100) NOT NULL,
    [FileSize] BIGINT NOT NULL,
    [SrFileCategory] BIGINT NOT NULL,
    [StorageProvider] NVARCHAR(30) NOT NULL,
    [RelativePath] NVARCHAR(500) NOT NULL,
    [Width] INT,
    [Height] INT,
    [IsImage] BIT NOT NULL DEFAULT 0,
    [FileHash] NVARCHAR(128),
    [DownloadCount] BIGINT NOT NULL,
    [LastAccessTime] DATETIME2(7),
    [IsTemporary] BIT NOT NULL DEFAULT 0,
    [Description] NVARCHAR(500),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_FileStorage] PRIMARY KEY ([FileStorageId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_FileStorage_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_FileStorage_Guid] ON [app].[FileStorage] ([FileStorageGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_FileStorage_SrFileCategory' AND object_id IS NOT NULL)
    CREATE INDEX [IX_FileStorage_SrFileCategory] ON [app].[FileStorage] ([SrFileCategory]);

-- ---------------------------------------------------------------------------
-- app.Person
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[Person];
CREATE TABLE [app].[Person]
(
    [PersonId] BIGINT IDENTITY(1,1) NOT NULL,
    [PersonGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [FirstName] NVARCHAR(100) NOT NULL,
    [MiddleName] NVARCHAR(100),
    [LastName] NVARCHAR(100),
    [PreTitle] NVARCHAR(30),
    [PostTitle] NVARCHAR(30),
    [BirthName] NVARCHAR(200),
    [PlaceOfBirth] NVARCHAR(100),
    [BirthDate] DATE NOT NULL,
    [NationalIdNo] NVARCHAR(30),
    [SrGender] BIGINT NOT NULL,
    [SrReligion] BIGINT NOT NULL,
    [SrSalutation] BIGINT,
    [SrBloodType] BIGINT,
    [SrMaritalStatus] BIGINT,
    [FileStorageId] BIGINT,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_Person] PRIMARY KEY ([PersonId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Person_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Person_Guid] ON [app].[Person] ([PersonGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Person_BirthDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Person_BirthDate_Active] ON [app].[Person] ([BirthDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Person_SrGender' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Person_SrGender] ON [app].[Person] ([SrGender]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Person_SrReligion' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Person_SrReligion] ON [app].[Person] ([SrReligion]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Person_SrSalutation' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Person_SrSalutation] ON [app].[Person] ([SrSalutation]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Person_SrBloodType' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Person_SrBloodType] ON [app].[Person] ([SrBloodType]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Person_SrMaritalStatus' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Person_SrMaritalStatus] ON [app].[Person] ([SrMaritalStatus]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Person_FileStorageId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Person_FileStorageId] ON [app].[Person] ([FileStorageId]);

-- ---------------------------------------------------------------------------
-- app.PersonAddress
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[PersonAddress];
CREATE TABLE [app].[PersonAddress]
(
    [PersonAddressId] BIGINT IDENTITY(1,1) NOT NULL,
    [PersonAddressGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [PersonId] BIGINT NOT NULL,
    [SrAddressType] BIGINT NOT NULL,
    [Address] NVARCHAR(1000) NOT NULL,
    [CountryId] BIGINT NOT NULL,
    [ProvinceId] BIGINT NOT NULL,
    [CityId] BIGINT NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_PersonAddress] PRIMARY KEY ([PersonAddressId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_PersonAddress_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_PersonAddress_Guid] ON [app].[PersonAddress] ([PersonAddressGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonAddress_PersonId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonAddress_PersonId] ON [app].[PersonAddress] ([PersonId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonAddress_SrAddressType' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonAddress_SrAddressType] ON [app].[PersonAddress] ([SrAddressType]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonAddress_CountryId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonAddress_CountryId] ON [app].[PersonAddress] ([CountryId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonAddress_ProvinceId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonAddress_ProvinceId] ON [app].[PersonAddress] ([ProvinceId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonAddress_CityId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonAddress_CityId] ON [app].[PersonAddress] ([CityId]);

-- ---------------------------------------------------------------------------
-- app.PersonContact
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[PersonContact];
CREATE TABLE [app].[PersonContact]
(
    [PersonContactId] BIGINT IDENTITY(1,1) NOT NULL,
    [PersonContactGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [PersonId] BIGINT NOT NULL,
    [SrContactType] BIGINT NOT NULL,
    [ContactValue] NVARCHAR(200) NOT NULL,
    [IsPrimary] BIT NOT NULL DEFAULT 0,
    [Remark] NVARCHAR(500),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_PersonContact] PRIMARY KEY ([PersonContactId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_PersonContact_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_PersonContact_Guid] ON [app].[PersonContact] ([PersonContactGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonContact_PersonId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonContact_PersonId] ON [app].[PersonContact] ([PersonId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonContact_SrContactType' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonContact_SrContactType] ON [app].[PersonContact] ([SrContactType]);

-- ---------------------------------------------------------------------------
-- app.PersonEducation
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[PersonEducation];
CREATE TABLE [app].[PersonEducation]
(
    [PersonEducationId] BIGINT IDENTITY(1,1) NOT NULL,
    [PersonEducationGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [PersonId] BIGINT NOT NULL,
    [SrEducationLevel] BIGINT NOT NULL,
    [InstitutionName] NVARCHAR(200) NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_PersonEducation] PRIMARY KEY ([PersonEducationId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_PersonEducation_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_PersonEducation_Guid] ON [app].[PersonEducation] ([PersonEducationGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonEducation_PersonId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonEducation_PersonId] ON [app].[PersonEducation] ([PersonId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonEducation_SrEducationLevel' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonEducation_SrEducationLevel] ON [app].[PersonEducation] ([SrEducationLevel]);

-- ---------------------------------------------------------------------------
-- app.PersonIdentification
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[PersonIdentification];
CREATE TABLE [app].[PersonIdentification]
(
    [PersonIdentificationId] BIGINT IDENTITY(1,1) NOT NULL,
    [PersonIdentificationGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [PersonId] BIGINT NOT NULL,
    [SrIdentificationType] BIGINT NOT NULL,
    [IdentificationValue] NVARCHAR(100) NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_PersonIdentification] PRIMARY KEY ([PersonIdentificationId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_PersonIdentification_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_PersonIdentification_Guid] ON [app].[PersonIdentification] ([PersonIdentificationGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_PersonIdentification_Value' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_PersonIdentification_Value] ON [app].[PersonIdentification] ([SrIdentificationType], [IdentificationValue]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonIdentification_PersonId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonIdentification_PersonId] ON [app].[PersonIdentification] ([PersonId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonIdentification_SrIdentificationType' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonIdentification_SrIdentificationType] ON [app].[PersonIdentification] ([SrIdentificationType]);

-- ---------------------------------------------------------------------------
-- app.PersonEmergencyContact
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[PersonEmergencyContact];
CREATE TABLE [app].[PersonEmergencyContact]
(
    [PersonEmergencyContactId] BIGINT IDENTITY(1,1) NOT NULL,
    [PersonEmergencyContactGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [PersonId] BIGINT NOT NULL,
    [ContactName] NVARCHAR(200) NOT NULL,
    [SrRelationship] BIGINT NOT NULL,
    [Phone] NVARCHAR(30),
    [IsPrimary] BIT NOT NULL DEFAULT 0,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_PersonEmergencyContact] PRIMARY KEY ([PersonEmergencyContactId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_PersonEmergencyContact_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_PersonEmergencyContact_Guid] ON [app].[PersonEmergencyContact] ([PersonEmergencyContactGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonEmergencyContact_PersonId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonEmergencyContact_PersonId] ON [app].[PersonEmergencyContact] ([PersonId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonEmergencyContact_SrRelationship' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonEmergencyContact_SrRelationship] ON [app].[PersonEmergencyContact] ([SrRelationship]);

-- ---------------------------------------------------------------------------
-- app.PersonFamily
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[PersonFamily];
CREATE TABLE [app].[PersonFamily]
(
    [PersonFamilyId] BIGINT IDENTITY(1,1) NOT NULL,
    [PersonFamilyGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [PersonId] BIGINT NOT NULL,
    [SrFamilyRelation] BIGINT NOT NULL,
    [FamilyName] NVARCHAR(100) NOT NULL,
    [DateBirth] DATETIME2(7) NOT NULL,
    [SrEducationLevel] BIGINT,
    [Address] NVARCHAR(400),
    [StateId] BIGINT,
    [CityId] BIGINT,
    [ZipCode] NVARCHAR(10),
    [Phone] NVARCHAR(20),
    [SrMaritalStatus] BIGINT,
    [SrGender] BIGINT,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_PersonFamily] PRIMARY KEY ([PersonFamilyId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_PersonFamily_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_PersonFamily_Guid] ON [app].[PersonFamily] ([PersonFamilyGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonFamily_PersonId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonFamily_PersonId] ON [app].[PersonFamily] ([PersonId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonFamily_SrFamilyRelation' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonFamily_SrFamilyRelation] ON [app].[PersonFamily] ([SrFamilyRelation]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonFamily_SrEducationLevel' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonFamily_SrEducationLevel] ON [app].[PersonFamily] ([SrEducationLevel]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonFamily_StateId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonFamily_StateId] ON [app].[PersonFamily] ([StateId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonFamily_CityId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonFamily_CityId] ON [app].[PersonFamily] ([CityId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonFamily_SrMaritalStatus' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonFamily_SrMaritalStatus] ON [app].[PersonFamily] ([SrMaritalStatus]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonFamily_SrGender' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonFamily_SrGender] ON [app].[PersonFamily] ([SrGender]);

-- ---------------------------------------------------------------------------
-- app.PersonBankAccount
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[PersonBankAccount];
CREATE TABLE [app].[PersonBankAccount]
(
    [PersonBankAccountId] BIGINT IDENTITY(1,1) NOT NULL,
    [PersonBankAccountGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [PersonId] BIGINT NOT NULL,
    [BankId] BIGINT NOT NULL,
    [BankBranch] NVARCHAR(150),
    [AccountNumber] NVARCHAR(50) NOT NULL,
    [AccountName] NVARCHAR(200) NOT NULL,
    [SwiftCode] NVARCHAR(30),
    [IBAN] NVARCHAR(50),
    [CurrencyId] BIGINT,
    [SrBankAccountType] BIGINT NOT NULL,
    [IsPrimary] BIT NOT NULL DEFAULT 0,
    [IsPayrollAccount] BIT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 0,
    [Remarks] NVARCHAR(500),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_PersonBankAccount] PRIMARY KEY ([PersonBankAccountId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_PersonBankAccount_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_PersonBankAccount_Guid] ON [app].[PersonBankAccount] ([PersonBankAccountGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonBankAccount_PersonId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonBankAccount_PersonId] ON [app].[PersonBankAccount] ([PersonId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonBankAccount_BankId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonBankAccount_BankId] ON [app].[PersonBankAccount] ([BankId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonBankAccount_CurrencyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonBankAccount_CurrencyId] ON [app].[PersonBankAccount] ([CurrencyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonBankAccount_SrBankAccountType' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonBankAccount_SrBankAccountType] ON [app].[PersonBankAccount] ([SrBankAccountType]);

-- ---------------------------------------------------------------------------
-- app.PersonWorkExperience
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[PersonWorkExperience];
CREATE TABLE [app].[PersonWorkExperience]
(
    [PersonWorkExperienceId] BIGINT IDENTITY(1,1) NOT NULL,
    [PersonWorkExperienceGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [PersonId] BIGINT NOT NULL,
    [SrIndustry] BIGINT,
    [SrEmploymentType] BIGINT,
    [CompanyName] NVARCHAR(200) NOT NULL,
    [JobTitle] NVARCHAR(150) NOT NULL,
    [Department] NVARCHAR(150),
    [Location] NVARCHAR(200),
    [Supervisor] NVARCHAR(150),
    [JobDescription] NVARCHAR(MAX),
    [StartDate] DATE NOT NULL,
    [EndDate] DATE,
    [IsCurrentEmployment] BIT NOT NULL DEFAULT 0,
    [LastSalary] DECIMAL(18,2),
    [ReasonForLeaving] NVARCHAR(MAX),
    [Remarks] NVARCHAR(500),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_PersonWorkExperience] PRIMARY KEY ([PersonWorkExperienceId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_PersonWorkExperience_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_PersonWorkExperience_Guid] ON [app].[PersonWorkExperience] ([PersonWorkExperienceGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonWorkExperience_PersonId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonWorkExperience_PersonId] ON [app].[PersonWorkExperience] ([PersonId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonWorkExperience_SrIndustry' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonWorkExperience_SrIndustry] ON [app].[PersonWorkExperience] ([SrIndustry]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonWorkExperience_SrEmploymentType' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonWorkExperience_SrEmploymentType] ON [app].[PersonWorkExperience] ([SrEmploymentType]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonWorkExperience_StartDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonWorkExperience_StartDate_Active] ON [app].[PersonWorkExperience] ([StartDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonWorkExperience_EndDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonWorkExperience_EndDate_Active] ON [app].[PersonWorkExperience] ([EndDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);

-- ---------------------------------------------------------------------------
-- app.PersonPhysicalCharacteristic
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[PersonPhysicalCharacteristic];
CREATE TABLE [app].[PersonPhysicalCharacteristic]
(
    [PersonPhysicalCharacteristicId] BIGINT IDENTITY(1,1) NOT NULL,
    [PersonPhysicalCharacteristicGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [PersonId] BIGINT NOT NULL,
    [SrPhysicalCharacteristic] BIGINT NOT NULL,
    [PhysicalValue] NVARCHAR(100) NOT NULL,
    [SrMeasurementUnit] BIGINT,
    [RecordedDate] DATE NOT NULL,
    [Remarks] NVARCHAR(500),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_PersonPhysicalCharacteristic] PRIMARY KEY ([PersonPhysicalCharacteristicId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_PersonPhysicalCharacteristic_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_PersonPhysicalCharacteristic_Guid] ON [app].[PersonPhysicalCharacteristic] ([PersonPhysicalCharacteristicGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonPhysicalCharacteristic_PersonId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonPhysicalCharacteristic_PersonId] ON [app].[PersonPhysicalCharacteristic] ([PersonId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonPhysicalCharacteristic_SrPhysicalCharacteristic' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonPhysicalCharacteristic_SrPhysicalCharacteristic] ON [app].[PersonPhysicalCharacteristic] ([SrPhysicalCharacteristic]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonPhysicalCharacteristic_SrMeasurementUnit' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonPhysicalCharacteristic_SrMeasurementUnit] ON [app].[PersonPhysicalCharacteristic] ([SrMeasurementUnit]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PersonPhysicalCharacteristic_RecordedDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_PersonPhysicalCharacteristic_RecordedDate_Active] ON [app].[PersonPhysicalCharacteristic] ([RecordedDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);

-- ---------------------------------------------------------------------------
-- app.Employee
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[Employee];
CREATE TABLE [app].[Employee]
(
    [EmployeeId] BIGINT IDENTITY(1,1) NOT NULL,
    [EmployeeGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [PersonId] BIGINT NOT NULL,
    [EmployeeNo] NVARCHAR(30) NOT NULL,
    [HireDate] DATE NOT NULL,
    [ProbationEndDate] DATE,
    [ConfirmationDate] DATE,
    [TerminationDate] DATE,
    [SrEmploymentStatus] BIGINT NOT NULL,
    [SrEmploymentType] BIGINT NOT NULL,
    [SrRemunerationType] BIGINT,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_Employee] PRIMARY KEY ([EmployeeId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Employee_EmployeeNo' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Employee_EmployeeNo] ON [app].[Employee] ([EmployeeNo]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Employee_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Employee_Guid] ON [app].[Employee] ([EmployeeGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Employee_PersonId' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_Employee_PersonId] ON [app].[Employee] ([PersonId]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Employee_PersonId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Employee_PersonId] ON [app].[Employee] ([PersonId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Employee_HireDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Employee_HireDate_Active] ON [app].[Employee] ([HireDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Employee_ProbationEndDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Employee_ProbationEndDate_Active] ON [app].[Employee] ([ProbationEndDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Employee_ConfirmationDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Employee_ConfirmationDate_Active] ON [app].[Employee] ([ConfirmationDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Employee_TerminationDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Employee_TerminationDate_Active] ON [app].[Employee] ([TerminationDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Employee_SrEmploymentStatus' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Employee_SrEmploymentStatus] ON [app].[Employee] ([SrEmploymentStatus]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Employee_SrEmploymentType' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Employee_SrEmploymentType] ON [app].[Employee] ([SrEmploymentType]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Employee_SrRemunerationType' AND object_id IS NOT NULL)
    CREATE INDEX [IX_Employee_SrRemunerationType] ON [app].[Employee] ([SrRemunerationType]);

-- ---------------------------------------------------------------------------
-- app.EmployeeAssignment
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[EmployeeAssignment];
CREATE TABLE [app].[EmployeeAssignment]
(
    [EmployeeAssignmentId] BIGINT IDENTITY(1,1) NOT NULL,
    [EmployeeAssignmentGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [EmployeeId] BIGINT NOT NULL,
    [CompanyId] BIGINT NOT NULL,
    [CompanyOfficeId] BIGINT,
    [OrganizationUnitId] BIGINT NOT NULL,
    [PositionId] BIGINT,
    [JobTitleId] BIGINT,
    [JobGradeId] BIGINT,
    [JobLevelId] BIGINT,
    [ManagerEmployeeId] BIGINT,
    [SupervisorEmployeeId] BIGINT,
    [CostCenterId] BIGINT,
    [PayrollGroupId] BIGINT,
    [ShiftId] BIGINT,
    [WorkLocationId] BIGINT,
    [SrEmploymentStatus] BIGINT NOT NULL,
    [SrEmploymentType] BIGINT NOT NULL,
    [SrAssignmentType] BIGINT NOT NULL,
    [SrAssignmentReason] BIGINT,
    [EffectiveFrom] DATE NOT NULL,
    [EffectiveTo] DATE,
    [IsPrimary] BIT NOT NULL DEFAULT 0,
    [IsCurrent] BIT NOT NULL DEFAULT 0,
    [Remarks] NVARCHAR(500),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_EmployeeAssignment] PRIMARY KEY ([EmployeeAssignmentId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_EmployeeAssignment_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_EmployeeAssignment_Guid] ON [app].[EmployeeAssignment] ([EmployeeAssignmentGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_EmployeeAssignment_Current' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UX_EmployeeAssignment_Current] ON [app].[EmployeeAssignment] ([EmployeeId]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_EmployeeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_EmployeeId] ON [app].[EmployeeAssignment] ([EmployeeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_CompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_CompanyId] ON [app].[EmployeeAssignment] ([CompanyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_CompanyOfficeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_CompanyOfficeId] ON [app].[EmployeeAssignment] ([CompanyOfficeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_OrganizationUnitId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_OrganizationUnitId] ON [app].[EmployeeAssignment] ([OrganizationUnitId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_PositionId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_PositionId] ON [app].[EmployeeAssignment] ([PositionId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_JobTitleId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_JobTitleId] ON [app].[EmployeeAssignment] ([JobTitleId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_JobGradeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_JobGradeId] ON [app].[EmployeeAssignment] ([JobGradeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_JobLevelId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_JobLevelId] ON [app].[EmployeeAssignment] ([JobLevelId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_ManagerEmployeeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_ManagerEmployeeId] ON [app].[EmployeeAssignment] ([ManagerEmployeeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_SupervisorEmployeeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_SupervisorEmployeeId] ON [app].[EmployeeAssignment] ([SupervisorEmployeeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_CostCenterId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_CostCenterId] ON [app].[EmployeeAssignment] ([CostCenterId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_PayrollGroupId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_PayrollGroupId] ON [app].[EmployeeAssignment] ([PayrollGroupId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_ShiftId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_ShiftId] ON [app].[EmployeeAssignment] ([ShiftId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_WorkLocationId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_WorkLocationId] ON [app].[EmployeeAssignment] ([WorkLocationId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_SrEmploymentStatus' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_SrEmploymentStatus] ON [app].[EmployeeAssignment] ([SrEmploymentStatus]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_SrEmploymentType' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_SrEmploymentType] ON [app].[EmployeeAssignment] ([SrEmploymentType]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_SrAssignmentType' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_SrAssignmentType] ON [app].[EmployeeAssignment] ([SrAssignmentType]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAssignment_SrAssignmentReason' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAssignment_SrAssignmentReason] ON [app].[EmployeeAssignment] ([SrAssignmentReason]);

-- ---------------------------------------------------------------------------
-- app.EmployeeContract
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[EmployeeContract];
CREATE TABLE [app].[EmployeeContract]
(
    [EmployeeContractId] BIGINT IDENTITY(1,1) NOT NULL,
    [EmployeeContractGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [EmployeeId] BIGINT NOT NULL,
    [EmployeeAssignmentId] BIGINT,
    [ContractNo] NVARCHAR(50) NOT NULL,
    [SrContractType] BIGINT NOT NULL,
    [SrContractStatus] BIGINT NOT NULL,
    [ContractStartDate] DATE NOT NULL,
    [ContractEndDate] DATE,
    [EffectiveDate] DATE NOT NULL,
    [RenewalSequence] INT NOT NULL,
    [IsCurrent] BIT NOT NULL DEFAULT 0,
    [FileStorageId] BIGINT,
    [Remarks] NVARCHAR(1000),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_EmployeeContract] PRIMARY KEY ([EmployeeContractId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_EmployeeContract_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_EmployeeContract_Guid] ON [app].[EmployeeContract] ([EmployeeContractGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_EmployeeContract_Current' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UX_EmployeeContract_Current] ON [app].[EmployeeContract] ([EmployeeId]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeContract_EmployeeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeContract_EmployeeId] ON [app].[EmployeeContract] ([EmployeeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeContract_EmployeeAssignmentId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeContract_EmployeeAssignmentId] ON [app].[EmployeeContract] ([EmployeeAssignmentId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeContract_SrContractType' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeContract_SrContractType] ON [app].[EmployeeContract] ([SrContractType]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeContract_SrContractStatus' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeContract_SrContractStatus] ON [app].[EmployeeContract] ([SrContractStatus]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeContract_ContractStartDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeContract_ContractStartDate_Active] ON [app].[EmployeeContract] ([ContractStartDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeContract_ContractEndDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeContract_ContractEndDate_Active] ON [app].[EmployeeContract] ([ContractEndDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeContract_EffectiveDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeContract_EffectiveDate_Active] ON [app].[EmployeeContract] ([EffectiveDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeContract_FileStorageId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeContract_FileStorageId] ON [app].[EmployeeContract] ([FileStorageId]);

-- ---------------------------------------------------------------------------
-- app.EmployeeCareer
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[EmployeeCareer];
CREATE TABLE [app].[EmployeeCareer]
(
    [EmployeeCareerId] BIGINT IDENTITY(1,1) NOT NULL,
    [EmployeeCareerGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [EmployeeId] BIGINT NOT NULL,
    [EmployeeAssignmentId] BIGINT,
    [EmployeeContractId] BIGINT,
    [SrCareerEvent] BIGINT NOT NULL,
    [SrCareerReason] BIGINT,
    [EffectiveDate] DATE NOT NULL,
    [FromCompanyId] BIGINT,
    [FromOrganizationUnitId] BIGINT,
    [FromPositionId] BIGINT,
    [FromJobTitleId] BIGINT,
    [FromJobGradeId] BIGINT,
    [FromJobLevelId] BIGINT,
    [FromManagerEmployeeId] BIGINT,
    [ToCompanyId] BIGINT,
    [ToOrganizationUnitId] BIGINT,
    [ToPositionId] BIGINT,
    [ToJobTitleId] BIGINT,
    [ToJobGradeId] BIGINT,
    [ToJobLevelId] BIGINT,
    [ToManagerEmployeeId] BIGINT,
    [ReferenceNo] NVARCHAR(50),
    [ReferenceDate] DATE,
    [Remarks] NVARCHAR(1000),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_EmployeeCareer] PRIMARY KEY ([EmployeeCareerId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_EmployeeCareer_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_EmployeeCareer_Guid] ON [app].[EmployeeCareer] ([EmployeeCareerGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_EmployeeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_EmployeeId] ON [app].[EmployeeCareer] ([EmployeeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_EmployeeAssignmentId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_EmployeeAssignmentId] ON [app].[EmployeeCareer] ([EmployeeAssignmentId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_EmployeeContractId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_EmployeeContractId] ON [app].[EmployeeCareer] ([EmployeeContractId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_SrCareerEvent' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_SrCareerEvent] ON [app].[EmployeeCareer] ([SrCareerEvent]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_SrCareerReason' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_SrCareerReason] ON [app].[EmployeeCareer] ([SrCareerReason]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_EffectiveDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_EffectiveDate_Active] ON [app].[EmployeeCareer] ([EffectiveDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_FromCompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_FromCompanyId] ON [app].[EmployeeCareer] ([FromCompanyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_FromOrganizationUnitId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_FromOrganizationUnitId] ON [app].[EmployeeCareer] ([FromOrganizationUnitId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_FromPositionId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_FromPositionId] ON [app].[EmployeeCareer] ([FromPositionId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_FromJobTitleId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_FromJobTitleId] ON [app].[EmployeeCareer] ([FromJobTitleId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_FromJobGradeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_FromJobGradeId] ON [app].[EmployeeCareer] ([FromJobGradeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_FromJobLevelId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_FromJobLevelId] ON [app].[EmployeeCareer] ([FromJobLevelId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_FromManagerEmployeeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_FromManagerEmployeeId] ON [app].[EmployeeCareer] ([FromManagerEmployeeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_ToCompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_ToCompanyId] ON [app].[EmployeeCareer] ([ToCompanyId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_ToOrganizationUnitId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_ToOrganizationUnitId] ON [app].[EmployeeCareer] ([ToOrganizationUnitId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_ToPositionId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_ToPositionId] ON [app].[EmployeeCareer] ([ToPositionId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_ToJobTitleId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_ToJobTitleId] ON [app].[EmployeeCareer] ([ToJobTitleId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_ToJobGradeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_ToJobGradeId] ON [app].[EmployeeCareer] ([ToJobGradeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_ToJobLevelId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_ToJobLevelId] ON [app].[EmployeeCareer] ([ToJobLevelId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_ToManagerEmployeeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_ToManagerEmployeeId] ON [app].[EmployeeCareer] ([ToManagerEmployeeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCareer_ReferenceDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCareer_ReferenceDate_Active] ON [app].[EmployeeCareer] ([ReferenceDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);

-- ---------------------------------------------------------------------------
-- app.EmployeeSkill
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[EmployeeSkill];
CREATE TABLE [app].[EmployeeSkill]
(
    [EmployeeSkillId] BIGINT IDENTITY(1,1) NOT NULL,
    [EmployeeSkillGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [EmployeeId] BIGINT NOT NULL,
    [SkillId] BIGINT NOT NULL,
    [SrSkillLevel] BIGINT,
    [YearsOfExperience] DECIMAL(8,2),
    [LastUsedDate] DATE,
    [IsPrimary] BIT NOT NULL DEFAULT 0,
    [IsCertified] BIT NOT NULL DEFAULT 0,
    [CertificateNumber] NVARCHAR(100),
    [CertificateIssueDate] DATE,
    [CertificateExpiredDate] DATE,
    [FileStorageId] BIGINT,
    [Remarks] NVARCHAR(1000),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_EmployeeSkill] PRIMARY KEY ([EmployeeSkillId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_EmployeeSkill_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_EmployeeSkill_Guid] ON [app].[EmployeeSkill] ([EmployeeSkillGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_EmployeeSkill_Employee_Skill' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UX_EmployeeSkill_Employee_Skill] ON [app].[EmployeeSkill] ([EmployeeId], [SkillId]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeSkill_EmployeeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeSkill_EmployeeId] ON [app].[EmployeeSkill] ([EmployeeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeSkill_SkillId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeSkill_SkillId] ON [app].[EmployeeSkill] ([SkillId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeSkill_SrSkillLevel' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeSkill_SrSkillLevel] ON [app].[EmployeeSkill] ([SrSkillLevel]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeSkill_LastUsedDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeSkill_LastUsedDate_Active] ON [app].[EmployeeSkill] ([LastUsedDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeSkill_CertificateIssueDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeSkill_CertificateIssueDate_Active] ON [app].[EmployeeSkill] ([CertificateIssueDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeSkill_CertificateExpiredDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeSkill_CertificateExpiredDate_Active] ON [app].[EmployeeSkill] ([CertificateExpiredDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeSkill_FileStorageId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeSkill_FileStorageId] ON [app].[EmployeeSkill] ([FileStorageId]);

-- ---------------------------------------------------------------------------
-- app.EmployeeLanguage
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[EmployeeLanguage];
CREATE TABLE [app].[EmployeeLanguage]
(
    [EmployeeLanguageId] BIGINT IDENTITY(1,1) NOT NULL,
    [EmployeeLanguageGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [EmployeeId] BIGINT NOT NULL,
    [SrLanguage] BIGINT NOT NULL,
    [SrReadingLevel] BIGINT,
    [SrWritingLevel] BIGINT,
    [SrListeningLevel] BIGINT,
    [SrSpeakingLevel] BIGINT,
    [CertificationName] NVARCHAR(100),
    [Score] DECIMAL(10,2),
    [IssuedDate] DATE,
    [ExpiredDate] DATE,
    [FileStorageId] BIGINT,
    [IsPrimary] BIT NOT NULL DEFAULT 0,
    [Remarks] NVARCHAR(1000),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_EmployeeLanguage] PRIMARY KEY ([EmployeeLanguageId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_EmployeeLanguage_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_EmployeeLanguage_Guid] ON [app].[EmployeeLanguage] ([EmployeeLanguageGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_EmployeeLanguage' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UX_EmployeeLanguage] ON [app].[EmployeeLanguage] ([EmployeeId], [SrLanguage]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeLanguage_EmployeeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeLanguage_EmployeeId] ON [app].[EmployeeLanguage] ([EmployeeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeLanguage_SrLanguage' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeLanguage_SrLanguage] ON [app].[EmployeeLanguage] ([SrLanguage]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeLanguage_SrReadingLevel' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeLanguage_SrReadingLevel] ON [app].[EmployeeLanguage] ([SrReadingLevel]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeLanguage_SrWritingLevel' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeLanguage_SrWritingLevel] ON [app].[EmployeeLanguage] ([SrWritingLevel]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeLanguage_SrListeningLevel' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeLanguage_SrListeningLevel] ON [app].[EmployeeLanguage] ([SrListeningLevel]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeLanguage_SrSpeakingLevel' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeLanguage_SrSpeakingLevel] ON [app].[EmployeeLanguage] ([SrSpeakingLevel]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeLanguage_IssuedDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeLanguage_IssuedDate_Active] ON [app].[EmployeeLanguage] ([IssuedDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeLanguage_ExpiredDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeLanguage_ExpiredDate_Active] ON [app].[EmployeeLanguage] ([ExpiredDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeLanguage_FileStorageId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeLanguage_FileStorageId] ON [app].[EmployeeLanguage] ([FileStorageId]);

-- ---------------------------------------------------------------------------
-- app.EmployeeTraining
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[EmployeeTraining];
CREATE TABLE [app].[EmployeeTraining]
(
    [EmployeeTrainingId] BIGINT IDENTITY(1,1) NOT NULL,
    [EmployeeTrainingGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [EmployeeId] BIGINT NOT NULL,
    [TrainingId] BIGINT NOT NULL,
    [StartDate] DATE,
    [EndDate] DATE,
    [DurationHour] DECIMAL(8,2),
    [Score] DECIMAL(10,2),
    [Grade] NVARCHAR(20),
    [IsPassed] BIT NOT NULL DEFAULT 0,
    [CertificateNumber] NVARCHAR(100),
    [FileStorageId] BIGINT,
    [Remarks] NVARCHAR(1000),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_EmployeeTraining] PRIMARY KEY ([EmployeeTrainingId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_EmployeeTraining_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_EmployeeTraining_Guid] ON [app].[EmployeeTraining] ([EmployeeTrainingGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeTraining_EmployeeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeTraining_EmployeeId] ON [app].[EmployeeTraining] ([EmployeeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeTraining_TrainingId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeTraining_TrainingId] ON [app].[EmployeeTraining] ([TrainingId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeTraining_StartDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeTraining_StartDate_Active] ON [app].[EmployeeTraining] ([StartDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeTraining_EndDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeTraining_EndDate_Active] ON [app].[EmployeeTraining] ([EndDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeTraining_FileStorageId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeTraining_FileStorageId] ON [app].[EmployeeTraining] ([FileStorageId]);

-- ---------------------------------------------------------------------------
-- app.EmployeeCertification
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[EmployeeCertification];
CREATE TABLE [app].[EmployeeCertification]
(
    [EmployeeCertificationId] BIGINT IDENTITY(1,1) NOT NULL,
    [EmployeeCertificationGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [EmployeeId] BIGINT NOT NULL,
    [CertificationId] BIGINT NOT NULL,
    [CertificateNumber] NVARCHAR(100),
    [IssuedDate] DATE,
    [ExpiredDate] DATE,
    [IssuedBy] NVARCHAR(200),
    [Score] DECIMAL(10,2),
    [FileStorageId] BIGINT,
    [IsLifetime] BIT NOT NULL DEFAULT 0,
    [IsCurrent] BIT NOT NULL DEFAULT 0,
    [Remarks] NVARCHAR(1000),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_EmployeeCertification] PRIMARY KEY ([EmployeeCertificationId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_EmployeeCertification_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_EmployeeCertification_Guid] ON [app].[EmployeeCertification] ([EmployeeCertificationGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCertification_EmployeeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCertification_EmployeeId] ON [app].[EmployeeCertification] ([EmployeeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCertification_CertificationId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCertification_CertificationId] ON [app].[EmployeeCertification] ([CertificationId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCertification_IssuedDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCertification_IssuedDate_Active] ON [app].[EmployeeCertification] ([IssuedDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCertification_ExpiredDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCertification_ExpiredDate_Active] ON [app].[EmployeeCertification] ([ExpiredDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeCertification_FileStorageId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeCertification_FileStorageId] ON [app].[EmployeeCertification] ([FileStorageId]);

-- ---------------------------------------------------------------------------
-- app.EmployeeAsset
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[EmployeeAsset];
CREATE TABLE [app].[EmployeeAsset]
(
    [EmployeeAssetId] BIGINT IDENTITY(1,1) NOT NULL,
    [EmployeeAssetGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [EmployeeId] BIGINT NOT NULL,
    [AssetId] BIGINT NOT NULL,
    [AssignedDate] DATE NOT NULL,
    [ReturnedDate] DATE,
    [ExpectedReturnDate] DATE,
    [IsCurrent] BIT NOT NULL DEFAULT 0,
    [ConditionWhenAssigned] NVARCHAR(200),
    [ConditionWhenReturned] NVARCHAR(200),
    [Remarks] NVARCHAR(1000),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_EmployeeAsset] PRIMARY KEY ([EmployeeAssetId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_EmployeeAsset_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_EmployeeAsset_Guid] ON [app].[EmployeeAsset] ([EmployeeAssetGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAsset_EmployeeId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAsset_EmployeeId] ON [app].[EmployeeAsset] ([EmployeeId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAsset_AssetId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAsset_AssetId] ON [app].[EmployeeAsset] ([AssetId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAsset_AssignedDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAsset_AssignedDate_Active] ON [app].[EmployeeAsset] ([AssignedDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAsset_ReturnedDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAsset_ReturnedDate_Active] ON [app].[EmployeeAsset] ([ReturnedDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmployeeAsset_ExpectedReturnDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_EmployeeAsset_ExpectedReturnDate_Active] ON [app].[EmployeeAsset] ([ExpectedReturnDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);

-- ---------------------------------------------------------------------------
-- app.MeetingRoom
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[MeetingRoom];
CREATE TABLE [app].[MeetingRoom]
(
    [MeetingRoomId] BIGINT IDENTITY(1,1) NOT NULL,
    [MeetingRoomGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [RoomCode] NVARCHAR(20) NOT NULL,
    [RoomName] NVARCHAR(200) NOT NULL,
    [Capacity] INT,
    [Location] NVARCHAR(200),
    [Note] NVARCHAR(4000),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_MeetingRoom] PRIMARY KEY ([MeetingRoomId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_MeetingRoom_Code' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_MeetingRoom_Code] ON [app].[MeetingRoom] ([RoomCode]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_MeetingRoom_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_MeetingRoom_Guid] ON [app].[MeetingRoom] ([MeetingRoomGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);

-- ---------------------------------------------------------------------------
-- app.RoomBooking
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[RoomBooking];
CREATE TABLE [app].[RoomBooking]
(
    [RoomBookingId] BIGINT IDENTITY(1,1) NOT NULL,
    [RoomBookingGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [RoomBookingCode] NVARCHAR(20) NOT NULL,
    [MeetingRoomId] BIGINT NOT NULL,
    [Subject] NVARCHAR(400) NOT NULL,
    [StartDateTime] DATETIME2(7) NOT NULL,
    [EndDateTime] DATETIME2(7) NOT NULL,
    [BookedByUserId] BIGINT,
    [Note] NVARCHAR(4000),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_RoomBooking] PRIMARY KEY ([RoomBookingId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_RoomBooking_Code' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_RoomBooking_Code] ON [app].[RoomBooking] ([RoomBookingCode]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_RoomBooking_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_RoomBooking_Guid] ON [app].[RoomBooking] ([RoomBookingGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RoomBooking_MeetingRoomId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_RoomBooking_MeetingRoomId] ON [app].[RoomBooking] ([MeetingRoomId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RoomBooking_BookedByUserId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_RoomBooking_BookedByUserId] ON [app].[RoomBooking] ([BookedByUserId]);

-- ---------------------------------------------------------------------------
-- app.WorkItem
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [app].[WorkItem];
CREATE TABLE [app].[WorkItem]
(
    [WorkItemId] BIGINT IDENTITY(1,1) NOT NULL,
    [WorkItemGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [WorkItemCode] NVARCHAR(20) NOT NULL,
    [Title] NVARCHAR(400) NOT NULL,
    [Description] NVARCHAR(4000),
    [Module] NVARCHAR(100),
    [WorkItemType] NVARCHAR(50),
    [OwnerName] NVARCHAR(100),
    [OwnerUserId] BIGINT,
    [WorkflowStatus] NVARCHAR(30) NOT NULL,
    [Priority] NVARCHAR(20),
    [CompletionPercent] INT,
    [Tags] NVARCHAR(200),
    [Framework] NVARCHAR(50),
    [DueDate] DATE,
    [Note] NVARCHAR(4000),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_WorkItem] PRIMARY KEY ([WorkItemId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_WorkItem_Code' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_WorkItem_Code] ON [app].[WorkItem] ([WorkItemCode]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_WorkItem_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_WorkItem_Guid] ON [app].[WorkItem] ([WorkItemGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_WorkItem_OwnerUserId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_WorkItem_OwnerUserId] ON [app].[WorkItem] ([OwnerUserId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_WorkItem_DueDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_WorkItem_DueDate_Active] ON [app].[WorkItem] ([DueDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);

-- ---------------------------------------------------------------------------
-- apv.ApprovalTemplate
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [apv].[ApprovalTemplate];
CREATE TABLE [apv].[ApprovalTemplate]
(
    [ApprovalTemplateId] BIGINT IDENTITY(1,1) NOT NULL,
    [ApprovalTemplateGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [TemplateName] NVARCHAR(200) NOT NULL,
    [ModuleCode] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500),
    [IsActive] BIT NOT NULL DEFAULT 0,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_ApprovalTemplate] PRIMARY KEY ([ApprovalTemplateId]),
    CONSTRAINT [UQ_ApprovalTemplate_Guid] UNIQUE ([ApprovalTemplateGuid])
);

-- ---------------------------------------------------------------------------
-- apv.ApprovalTemplateLevel
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [apv].[ApprovalTemplateLevel];
CREATE TABLE [apv].[ApprovalTemplateLevel]
(
    [ApprovalTemplateLevelId] BIGINT IDENTITY(1,1) NOT NULL,
    [ApprovalTemplateLevelGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ApprovalTemplateId] BIGINT NOT NULL,
    [LevelOrder] INT NOT NULL,
    [LevelName] NVARCHAR(200) NOT NULL,
    [ApproverType] NVARCHAR(50) NOT NULL,
    [RequireAllApprovers] BIT NOT NULL DEFAULT 0,
    [CanSkipIfPreviousNotApproved] BIT NOT NULL DEFAULT 0,
    [SlaHours] INT NOT NULL,
    [EscalateToLevelOrder] INT,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_ApprovalTemplateLevel] PRIMARY KEY ([ApprovalTemplateLevelId]),
    CONSTRAINT [UQ_ApprovalTemplateLevel_Guid] UNIQUE ([ApprovalTemplateLevelGuid])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApprovalTemplateLevel_ApprovalTemplateId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ApprovalTemplateLevel_ApprovalTemplateId] ON [apv].[ApprovalTemplateLevel] ([ApprovalTemplateId]);

-- ---------------------------------------------------------------------------
-- apv.ApprovalTemplateLevelApprover
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [apv].[ApprovalTemplateLevelApprover];
CREATE TABLE [apv].[ApprovalTemplateLevelApprover]
(
    [ApprovalTemplateLevelApproverId] BIGINT IDENTITY(1,1) NOT NULL,
    [ApprovalTemplateLevelApproverGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ApprovalTemplateLevelId] BIGINT NOT NULL,
    [UserId] BIGINT,
    [UserGroupId] BIGINT,
    [StatusId] INT NOT NULL DEFAULT 1,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_ApprovalTemplateLevelApprover] PRIMARY KEY ([ApprovalTemplateLevelApproverId]),
    CONSTRAINT [UQ_ATLevelApprover_Guid] UNIQUE ([ApprovalTemplateLevelApproverGuid])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApprovalTemplateLevelApprover_ApprovalTemplateLevelId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ApprovalTemplateLevelApprover_ApprovalTemplateLevelId] ON [apv].[ApprovalTemplateLevelApprover] ([ApprovalTemplateLevelId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApprovalTemplateLevelApprover_UserId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ApprovalTemplateLevelApprover_UserId] ON [apv].[ApprovalTemplateLevelApprover] ([UserId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApprovalTemplateLevelApprover_UserGroupId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ApprovalTemplateLevelApprover_UserGroupId] ON [apv].[ApprovalTemplateLevelApprover] ([UserGroupId]);

-- ---------------------------------------------------------------------------
-- apv.ApprovalRequest
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [apv].[ApprovalRequest];
CREATE TABLE [apv].[ApprovalRequest]
(
    [ApprovalRequestId] BIGINT IDENTITY(1,1) NOT NULL,
    [ApprovalRequestGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ApprovalTemplateId] BIGINT NOT NULL,
    [RequestNumber] NVARCHAR(50) NOT NULL,
    [ModuleCode] NVARCHAR(100) NOT NULL,
    [ReferenceGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ReferenceNumber] NVARCHAR(100),
    [RequestedByUserId] BIGINT NOT NULL,
    [CurrentLevelOrder] INT NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [Notes] NVARCHAR(500),
    [RequestedTime] DATETIME2(7) NOT NULL,
    [CompletedTime] DATETIME2(7),
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_ApprovalRequest] PRIMARY KEY ([ApprovalRequestId]),
    CONSTRAINT [UQ_ApprovalRequest_Guid] UNIQUE ([ApprovalRequestGuid])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApprovalRequest_ApprovalTemplateId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ApprovalRequest_ApprovalTemplateId] ON [apv].[ApprovalRequest] ([ApprovalTemplateId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApprovalRequest_RequestedByUserId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ApprovalRequest_RequestedByUserId] ON [apv].[ApprovalRequest] ([RequestedByUserId]);

-- ---------------------------------------------------------------------------
-- apv.ApprovalStep
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [apv].[ApprovalStep];
CREATE TABLE [apv].[ApprovalStep]
(
    [ApprovalStepId] BIGINT IDENTITY(1,1) NOT NULL,
    [ApprovalStepGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ApprovalRequestId] BIGINT NOT NULL,
    [LevelOrder] INT NOT NULL,
    [LevelName] NVARCHAR(200) NOT NULL,
    [ApproverUserId] BIGINT NOT NULL,
    [DelegatedFromUserId] BIGINT,
    [StatusId] INT NOT NULL DEFAULT 1,
    [ActionTime] DATETIME2(7),
    [Comment] NVARCHAR(1000),
    [SlaDeadline] DATETIME2(7),
    [IsEscalated] BIT NOT NULL DEFAULT 0,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    CONSTRAINT [PK_ApprovalStep] PRIMARY KEY ([ApprovalStepId]),
    CONSTRAINT [UQ_ApprovalStep_Guid] UNIQUE ([ApprovalStepGuid])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApprovalStep_ApprovalRequestId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ApprovalStep_ApprovalRequestId] ON [apv].[ApprovalStep] ([ApprovalRequestId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApprovalStep_ApproverUserId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ApprovalStep_ApproverUserId] ON [apv].[ApprovalStep] ([ApproverUserId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApprovalStep_DelegatedFromUserId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ApprovalStep_DelegatedFromUserId] ON [apv].[ApprovalStep] ([DelegatedFromUserId]);

-- ---------------------------------------------------------------------------
-- apv.ApprovalHistory
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [apv].[ApprovalHistory];
CREATE TABLE [apv].[ApprovalHistory]
(
    [ApprovalHistoryId] BIGINT IDENTITY(1,1) NOT NULL,
    [ApprovalHistoryGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ApprovalRequestId] BIGINT NOT NULL,
    [ApprovalStepId] BIGINT,
    [ActionType] NVARCHAR(50) NOT NULL,
    [ActionByUserId] BIGINT NOT NULL,
    [ActionTime] DATETIME2(7) NOT NULL,
    [Comment] NVARCHAR(1000),
    [FromStatus] NVARCHAR(50),
    [ToStatus] NVARCHAR(50),
    [LevelOrder] INT,
    CONSTRAINT [PK_ApprovalHistory] PRIMARY KEY ([ApprovalHistoryId]),
    CONSTRAINT [UQ_ApprovalHistory_Guid] UNIQUE ([ApprovalHistoryGuid])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApprovalHistory_ApprovalRequestId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ApprovalHistory_ApprovalRequestId] ON [apv].[ApprovalHistory] ([ApprovalRequestId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApprovalHistory_ApprovalStepId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ApprovalHistory_ApprovalStepId] ON [apv].[ApprovalHistory] ([ApprovalStepId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApprovalHistory_ActionByUserId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ApprovalHistory_ActionByUserId] ON [apv].[ApprovalHistory] ([ActionByUserId]);

-- ---------------------------------------------------------------------------
-- apv.ApprovalDelegation
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [apv].[ApprovalDelegation];
CREATE TABLE [apv].[ApprovalDelegation]
(
    [ApprovalDelegationId] BIGINT IDENTITY(1,1) NOT NULL,
    [ApprovalDelegationGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [DelegatorUserId] BIGINT NOT NULL,
    [DelegateUserId] BIGINT NOT NULL,
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 0,
    [Notes] NVARCHAR(500),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_ApprovalDelegation] PRIMARY KEY ([ApprovalDelegationId]),
    CONSTRAINT [UQ_ApprovalDelegation_Guid] UNIQUE ([ApprovalDelegationGuid])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApprovalDelegation_DelegatorUserId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ApprovalDelegation_DelegatorUserId] ON [apv].[ApprovalDelegation] ([DelegatorUserId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApprovalDelegation_DelegateUserId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ApprovalDelegation_DelegateUserId] ON [apv].[ApprovalDelegation] ([DelegateUserId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApprovalDelegation_StartDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ApprovalDelegation_StartDate_Active] ON [apv].[ApprovalDelegation] ([StartDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ApprovalDelegation_EndDate_Active' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ApprovalDelegation_EndDate_Active] ON [apv].[ApprovalDelegation] ([EndDate]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);

-- ---------------------------------------------------------------------------
-- JOIN display stubs (generator uses FK-column table names)
-- ---------------------------------------------------------------------------
IF OBJECT_ID(N'[app].[JobGrade]', N'U') IS NULL
BEGIN
CREATE TABLE [app].[JobGrade] (
    [JobGradeId] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [JobGradeName] NVARCHAR(200),
    [StatusId] INT NOT NULL DEFAULT 1
);
END
IF OBJECT_ID(N'[app].[JobLevel]', N'U') IS NULL
BEGIN
CREATE TABLE [app].[JobLevel] (
    [JobLevelId] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [JobLevelName] NVARCHAR(200),
    [StatusId] INT NOT NULL DEFAULT 1
);
END
IF OBJECT_ID(N'[app].[JobTitle]', N'U') IS NULL
BEGIN
CREATE TABLE [app].[JobTitle] (
    [JobTitleId] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [JobTitleName] NVARCHAR(200),
    [StatusId] INT NOT NULL DEFAULT 1
);
END
IF OBJECT_ID(N'[app].[Position]', N'U') IS NULL
BEGIN
CREATE TABLE [app].[Position] (
    [PositionId] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [PositionName] NVARCHAR(200),
    [StatusId] INT NOT NULL DEFAULT 1
);
END
IF OBJECT_ID(N'[app].[Shift]', N'U') IS NULL
BEGIN
CREATE TABLE [app].[Shift] (
    [ShiftId] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [ShiftName] NVARCHAR(200),
    [StatusId] INT NOT NULL DEFAULT 1
);
END
IF OBJECT_ID(N'[app].[PayrollGroup]', N'U') IS NULL
BEGIN
CREATE TABLE [app].[PayrollGroup] (
    [PayrollGroupId] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [PayrollGroupName] NVARCHAR(200),
    [StatusId] INT NOT NULL DEFAULT 1
);
END
IF OBJECT_ID(N'[app].[WorkLocation]', N'U') IS NULL
BEGIN
CREATE TABLE [app].[WorkLocation] (
    [WorkLocationId] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [WorkLocationName] NVARCHAR(200),
    [StatusId] INT NOT NULL DEFAULT 1
);
END

GO
-- Display name from First/Middle/Last only — PersonName is computed in Dev and omitted from platform DDL.
CREATE OR ALTER VIEW [app].[ManagerEmployee] AS
SELECT e.[EmployeeId] AS [ManagerEmployeeId],
       NULLIF(TRIM(CONCAT_WS(' ', p.[FirstName], p.[MiddleName], p.[LastName])), '') AS [ManagerEmployeeName],
       e.[StatusId]
FROM [app].[Employee] e
LEFT JOIN [app].[Person] p ON p.[PersonId] = e.[PersonId];
GO

GO
CREATE OR ALTER VIEW [app].[SupervisorEmployee] AS
SELECT e.[EmployeeId] AS [SupervisorEmployeeId],
       NULLIF(TRIM(CONCAT_WS(' ', p.[FirstName], p.[MiddleName], p.[LastName])), '') AS [SupervisorEmployeeName],
       e.[StatusId]
FROM [app].[Employee] e
LEFT JOIN [app].[Person] p ON p.[PersonId] = e.[PersonId];
GO

GO
CREATE OR ALTER VIEW [app].[FromCompany] AS
SELECT [CompanyId] AS [FromCompanyId], [CompanyName] AS [FromCompanyName], [StatusId] FROM [app].[Company];
GO

GO
CREATE OR ALTER VIEW [app].[ToCompany] AS
SELECT [CompanyId] AS [ToCompanyId], [CompanyName] AS [ToCompanyName], [StatusId] FROM [app].[Company];
GO

GO
CREATE OR ALTER VIEW [app].[FromOrganizationUnit] AS
SELECT [OrganizationUnitId] AS [FromOrganizationUnitId], [OrganizationUnitName] AS [FromOrganizationUnitName], [StatusId] FROM [app].[OrganizationUnit];
GO

GO
CREATE OR ALTER VIEW [app].[ToOrganizationUnit] AS
SELECT [OrganizationUnitId] AS [ToOrganizationUnitId], [OrganizationUnitName] AS [ToOrganizationUnitName], [StatusId] FROM [app].[OrganizationUnit];
GO

GO
CREATE OR ALTER VIEW [app].[FromManagerEmployee] AS
SELECT [ManagerEmployeeId] AS [FromManagerEmployeeId], [ManagerEmployeeName] AS [FromManagerEmployeeName], [StatusId] FROM [app].[ManagerEmployee];
GO

GO
CREATE OR ALTER VIEW [app].[ToManagerEmployee] AS
SELECT [ManagerEmployeeId] AS [ToManagerEmployeeId], [ManagerEmployeeName] AS [ToManagerEmployeeName], [StatusId] FROM [app].[ManagerEmployee];
GO

GO
CREATE OR ALTER VIEW [app].[FromJobGrade] AS SELECT [JobGradeId] AS [FromJobGradeId], [JobGradeName] AS [FromJobGradeName], [StatusId] FROM [app].[JobGrade];
GO

GO
CREATE OR ALTER VIEW [app].[ToJobGrade] AS SELECT [JobGradeId] AS [ToJobGradeId], [JobGradeName] AS [ToJobGradeName], [StatusId] FROM [app].[JobGrade];
GO

GO
CREATE OR ALTER VIEW [app].[FromJobLevel] AS SELECT [JobLevelId] AS [FromJobLevelId], [JobLevelName] AS [FromJobLevelName], [StatusId] FROM [app].[JobLevel];
GO

GO
CREATE OR ALTER VIEW [app].[ToJobLevel] AS SELECT [JobLevelId] AS [ToJobLevelId], [JobLevelName] AS [ToJobLevelName], [StatusId] FROM [app].[JobLevel];
GO

GO
CREATE OR ALTER VIEW [app].[FromJobTitle] AS SELECT [JobTitleId] AS [FromJobTitleId], [JobTitleName] AS [FromJobTitleName], [StatusId] FROM [app].[JobTitle];
GO

GO
CREATE OR ALTER VIEW [app].[ToJobTitle] AS SELECT [JobTitleId] AS [ToJobTitleId], [JobTitleName] AS [ToJobTitleName], [StatusId] FROM [app].[JobTitle];
GO

GO
CREATE OR ALTER VIEW [app].[FromPosition] AS SELECT [PositionId] AS [FromPositionId], [PositionName] AS [FromPositionName], [StatusId] FROM [app].[Position];
GO

GO
CREATE OR ALTER VIEW [app].[ToPosition] AS SELECT [PositionId] AS [ToPositionId], [PositionName] AS [ToPositionName], [StatusId] FROM [app].[Position];
GO

-- ---------------------------------------------------------------------------
-- Lookup / geo views
-- ---------------------------------------------------------------------------

GO
CREATE OR ALTER VIEW [app].[vw_StandardReference_Display] AS
SELECT
    i.[StandardReferenceItemId],
    i.[StandardReferenceItemName],
    CAST(0 AS BIGINT) AS [CompanyId],
    CAST(0 AS BIGINT) AS [CompanyOfficeId],
    CAST(NULL AS BIGINT) AS [StandardReferenceGroupId],
    r.[StandardReferenceInitial]
FROM [app].[StandardReferenceItem] i
INNER JOIN [app].[StandardReference] r ON r.[StandardReferenceId] = i.[StandardReferenceId]
WHERE i.[StatusId] > 0 AND i.[DeletedTime] IS NULL;
GO

GO
CREATE OR ALTER VIEW [app].[vw_StandardReferenceItem] AS
SELECT
    i.[StandardReferenceItemId],
    i.[StandardReferenceItemGuid],
    CAST(0 AS BIGINT) AS [CompanyId],
    CAST(NULL AS UNIQUEIDENTIFIER) AS [CompanyGuid],
    CAST(0 AS BIGINT) AS [CompanyOfficeId],
    CAST(NULL AS UNIQUEIDENTIFIER) AS [CompanyOfficeGuid],
    i.[StandardReferenceId],
    r.[StandardReferenceGuid],
    r.[StandardReferenceInitial],
    i.[StandardReferenceItemInitial],
    i.[StandardReferenceItemName],
    CAST(NULL AS NVARCHAR(MAX)) AS [Note]
FROM [app].[StandardReferenceItem] i
INNER JOIN [app].[StandardReference] r ON r.[StandardReferenceId] = i.[StandardReferenceId]
WHERE i.[StatusId] > 0 AND i.[DeletedTime] IS NULL;
GO

GO
CREATE OR ALTER VIEW [core].[vw_AdministrativeArea] AS
SELECT
   A.[CountryId], A.[CountryGuid], A.[CountryIso], A.[CountryIso3], A.[CountryName], A.[PhoneCode], A.[CurrencyCode],
   B.[ProvinceId], B.[ProvinceGuid], B.[ProvinceName],
   C.[CityId], C.[CityGuid], C.[CityName],
   D.[DistrictId], D.[DistrictGuid], D.[DistrictName],
   E.[SubDistrictId], E.[SubDistrictGuid], E.[SubDistrictName],
   F.[PostalCodeId], F.[PostalCodeGuid], F.[PostalCode]
FROM [core].[Country] A
INNER JOIN [core].[Province] B ON A.[CountryId] = B.[CountryId] AND A.[StatusId] > 0 AND B.[StatusId] > 0
INNER JOIN [core].[City] C ON B.[ProvinceId] = C.[ProvinceId] AND C.[StatusId] > 0
INNER JOIN [core].[District] D ON C.[CityId] = D.[CityId] AND D.[StatusId] > 0
INNER JOIN [core].[SubDistrict] E ON D.[DistrictId] = E.[DistrictId] AND E.[StatusId] > 0
INNER JOIN [core].[PostalCode] F ON E.[SubDistrictId] = F.[SubDistrictId];
GO

-- ---------------------------------------------------------------------------
-- Audit (log) - matches BamboeUp.Audit RebuildAuditSchemaAsync PostgreSQL
-- ---------------------------------------------------------------------------
IF OBJECT_ID(N'[log].[AuditSnapshot]', N'U') IS NULL
BEGIN
CREATE TABLE [log].[AuditSnapshot] (
    [SnapshotId] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [TableName] NVARCHAR(128) NOT NULL,
    [EntityKey] NVARCHAR(128) NOT NULL,
    [SnapshotJson] NVARCHAR(MAX) NOT NULL,
    [SnapshotDateTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME()
);
END
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AuditSnapshot_TableName_EntityKey' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AuditSnapshot_TableName_EntityKey] ON [log].[AuditSnapshot] ([TableName], [EntityKey]);

IF OBJECT_ID(N'[log].[AuditSession]', N'U') IS NULL
BEGIN
CREATE TABLE [log].[AuditSession] (
    [AuditSessionId] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [SessionGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [SessionType] NVARCHAR(10) NOT NULL,
    [RootTableName] NVARCHAR(128) NOT NULL,
    [RootEntityKey] NVARCHAR(128) NOT NULL,
    [RootDisplayName] NVARCHAR(256) NULL,
    [ActionByUserId] NVARCHAR(128) NOT NULL,
    [LogDateTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME()
);
END
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AuditSession_LogDateTime' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AuditSession_LogDateTime] ON [log].[AuditSession] ([LogDateTime] DESC);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AuditSession_ActionByUserId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AuditSession_ActionByUserId] ON [log].[AuditSession] ([ActionByUserId]);

IF OBJECT_ID(N'[log].[AuditLog]', N'U') IS NULL
BEGIN
CREATE TABLE [log].[AuditLog] (
    [AuditLogId] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [AuditSessionId] BIGINT NOT NULL REFERENCES [log].[AuditSession]([AuditSessionId]),
    [TableName] NVARCHAR(128) NOT NULL,
    [EntityKey] NVARCHAR(128) NOT NULL,
    [EntityDisplayName] NVARCHAR(256) NULL,
    [ParentTableName] NVARCHAR(128) NULL,
    [ParentEntityKey] NVARCHAR(128) NULL,
    [AuditActionType] NVARCHAR(10) NOT NULL,
    [ActionByUserID] NVARCHAR(128) NOT NULL,
    [LogDateTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [SnapshotId] BIGINT NULL REFERENCES [log].[AuditSnapshot]([SnapshotId])
);
END
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AuditLog_AuditSessionId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AuditLog_AuditSessionId] ON [log].[AuditLog] ([AuditSessionId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AuditLog_TableName_EntityKey' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AuditLog_TableName_EntityKey] ON [log].[AuditLog] ([TableName], [EntityKey]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AuditLog_ParentTableName_ParentEntityKey' AND object_id IS NOT NULL)
    CREATE INDEX [IX_AuditLog_ParentTableName_ParentEntityKey] ON [log].[AuditLog] ([ParentTableName], [ParentEntityKey]);

IF OBJECT_ID(N'[log].[AuditLogData]', N'U') IS NULL
BEGIN
CREATE TABLE [log].[AuditLogData] (
    [AuditLogId] BIGINT NOT NULL REFERENCES [log].[AuditLog]([AuditLogId]) ON DELETE CASCADE,
    [ColumnName] NVARCHAR(128) NOT NULL,
    [OldValue] NVARCHAR(MAX) NULL,
    [NewValue] NVARCHAR(MAX) NULL,
    PRIMARY KEY ([AuditLogId], [ColumnName])
);
END
