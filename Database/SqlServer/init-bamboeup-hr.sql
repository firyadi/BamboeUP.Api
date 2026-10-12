-- BamboeUP.HR - SQL Server platform schema
-- Matches SchemaStudio Platform export (Include table).
-- Audit trail is NOT here: run create-audit-db.sql + init-audit.sql.
-- Create the database yourself first. This script does not DROP DATABASE.
-- Physical FKs omitted (indexes on *Id / Sr* instead).
-- Script Date: 2026-10-07

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Drop views first so DROP TABLE succeeds on re-run
DROP VIEW IF EXISTS [app].[FromCompany];
DROP VIEW IF EXISTS [app].[ToCompany];
DROP VIEW IF EXISTS [app].[FromOrganizationUnit];
DROP VIEW IF EXISTS [app].[ToOrganizationUnit];
DROP VIEW IF EXISTS [app].[vw_StandardReference_Display];
DROP VIEW IF EXISTS [app].[vw_StandardReferenceItem];
DROP VIEW IF EXISTS [core].[vw_AdministrativeArea];

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'app')
    EXEC(N'CREATE SCHEMA [app]');

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'core')
    EXEC(N'CREATE SCHEMA [core]');

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
-- core.UserOrganizationScope
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [core].[UserOrganizationScope];
CREATE TABLE [core].[UserOrganizationScope]
(
    [UserOrganizationScopeId] BIGINT IDENTITY(1,1) NOT NULL,
    [UserOrganizationScopeGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [UserId] BIGINT NOT NULL,
    [OrganizationUnitId] BIGINT NOT NULL,
    [CompanyId] BIGINT NOT NULL,
    [CompanyOfficeId] BIGINT,
    [AccessType] VARCHAR(20) NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_UserOrganizationScope] PRIMARY KEY ([UserOrganizationScopeId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_UserOrganizationScope_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_UserOrganizationScope_Guid] ON [core].[UserOrganizationScope] ([UserOrganizationScopeGuid]) WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_UserOrganizationScope_UserId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_UserOrganizationScope_UserId] ON [core].[UserOrganizationScope] ([UserId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_UserOrganizationScope_OrganizationUnitId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_UserOrganizationScope_OrganizationUnitId] ON [core].[UserOrganizationScope] ([OrganizationUnitId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_UserOrganizationScope_CompanyId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_UserOrganizationScope_CompanyId] ON [core].[UserOrganizationScope] ([CompanyId]);

-- ---------------------------------------------------------------------------
-- core.ReportDefinition
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [core].[ReportParameter];
DROP TABLE IF EXISTS [core].[ReportDefinition];
CREATE TABLE [core].[ReportDefinition]
(
    [ReportDefinitionId] BIGINT IDENTITY(1,1) NOT NULL,
    [ReportDefinitionGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ProgramId] BIGINT NOT NULL,
    [ReportScope] NVARCHAR(20) NOT NULL DEFAULT N'Standard',
    [CompanyId] BIGINT,
    [ReportKind] NVARCHAR(5) NOT NULL,
    [DefinitionKey] NVARCHAR(100) NOT NULL,
    [FilePath] NVARCHAR(500),
    [StoreProcedureName] NVARCHAR(200),
    [IsTracked] BIT NOT NULL DEFAULT 0,
    [RequiresPrintId] BIT NOT NULL DEFAULT 0,
    [PrintIdPolicy] NVARCHAR(30),
    [PrintIdPrefix] NVARCHAR(10),
    [Version] INT NOT NULL DEFAULT 1,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [EffectiveFrom] DATETIME2(7),
    [EffectiveTo] DATETIME2(7),
    [RendererType] NVARCHAR(30),
    [LayoutJson] NVARCHAR(MAX),
    [StatusId] INT NOT NULL DEFAULT 1,
    [RowVersion] ROWVERSION NOT NULL,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    CONSTRAINT [PK_ReportDefinition] PRIMARY KEY ([ReportDefinitionId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_ReportDefinition_Guid' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_ReportDefinition_Guid] ON [core].[ReportDefinition] ([ReportDefinitionGuid]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ReportDefinition_ProgramId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ReportDefinition_ProgramId] ON [core].[ReportDefinition] ([ProgramId]);

-- ---------------------------------------------------------------------------
-- core.ReportParameterCatalog
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [core].[ReportParameterCatalog];
CREATE TABLE [core].[ReportParameterCatalog]
(
    [CatalogFieldId] BIGINT IDENTITY(1,1) NOT NULL,
    [FieldKey] NVARCHAR(50) NOT NULL,
    [DefaultLabel] NVARCHAR(200) NOT NULL,
    [ControlType] NVARCHAR(30) NOT NULL,
    [DataType] NVARCHAR(30) NOT NULL,
    [LookupType] NVARCHAR(50),
    [DefaultColumnGroup] TINYINT NOT NULL DEFAULT 3,
    [IsSystemField] BIT NOT NULL DEFAULT 0,
    [StatusId] INT NOT NULL DEFAULT 1,
    CONSTRAINT [PK_ReportParameterCatalog] PRIMARY KEY ([CatalogFieldId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_ReportParameterCatalog_FieldKey' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_ReportParameterCatalog_FieldKey] ON [core].[ReportParameterCatalog] ([FieldKey]);

-- ---------------------------------------------------------------------------
-- core.ReportParameter
-- ---------------------------------------------------------------------------
CREATE TABLE [core].[ReportParameter]
(
    [ReportParameterId] BIGINT IDENTITY(1,1) NOT NULL,
    [ReportDefinitionId] BIGINT NOT NULL,
    [ParameterName] NVARCHAR(100) NOT NULL,
    [DisplayLabel] NVARCHAR(200) NOT NULL,
    [DataType] NVARCHAR(30) NOT NULL,
    [IsRequired] BIT NOT NULL DEFAULT 0,
    [DefaultValue] NVARCHAR(500),
    [SortOrder] INT NOT NULL DEFAULT 0,
    [LookupType] NVARCHAR(50),
    [IsSensitive] BIT NOT NULL DEFAULT 0,
    [FieldKey] NVARCHAR(50),
    [ControlType] NVARCHAR(30) NOT NULL DEFAULT N'TextBox',
    [ColumnGroup] TINYINT NOT NULL DEFAULT 1,
    [ColumnSpan] TINYINT NOT NULL DEFAULT 12,
    [RowGroup] NVARCHAR(30),
    [VisibleWhen] NVARCHAR(200),
    [CatalogFieldId] BIGINT,
    [StatusId] INT NOT NULL DEFAULT 1,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_ReportParameter] PRIMARY KEY ([ReportParameterId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ReportParameter_ReportDefinitionId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_ReportParameter_ReportDefinitionId] ON [core].[ReportParameter] ([ReportDefinitionId]);

-- ---------------------------------------------------------------------------
-- core.SharedParameterCatalog
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS [core].[SharedParameterProfileField];
DROP TABLE IF EXISTS [core].[SharedParameterProfile];
DROP TABLE IF EXISTS [core].[SharedParameterCatalog];
CREATE TABLE [core].[SharedParameterCatalog]
(
    [SharedParameterCatalogId] BIGINT IDENTITY(1,1) NOT NULL,
    [SharedParameterCatalogGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Code] VARCHAR(50) NOT NULL,
    [Label] NVARCHAR(200) NOT NULL,
    [DataType] VARCHAR(30) NOT NULL,
    [CanonicalControlType] VARCHAR(30) NOT NULL,
    [ProcessControlType] VARCHAR(30) NOT NULL,
    [ReportControlType] VARCHAR(30) NOT NULL,
    [IsRequired] BIT NOT NULL DEFAULT 0,
    [DefaultValue] NVARCHAR(500),
    [LookupSource] NVARCHAR(200),
    [GroupName] NVARCHAR(100),
    [SortOrder] INT NOT NULL DEFAULT 0,
    [IsSystemField] BIT NOT NULL DEFAULT 1,
    [StatusId] INT NOT NULL DEFAULT 1,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_SharedParameterCatalog] PRIMARY KEY ([SharedParameterCatalogId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_SharedParameterCatalog_Code' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_SharedParameterCatalog_Code] ON [core].[SharedParameterCatalog] ([Code]);

-- ---------------------------------------------------------------------------
-- core.SharedParameterProfile
-- ---------------------------------------------------------------------------
CREATE TABLE [core].[SharedParameterProfile]
(
    [SharedParameterProfileId] BIGINT IDENTITY(1,1) NOT NULL,
    [SharedParameterProfileGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ProfileToken] VARCHAR(30) NOT NULL,
    [ProfileLabel] NVARCHAR(200) NOT NULL,
    [StatusId] INT NOT NULL DEFAULT 1,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_SharedParameterProfile] PRIMARY KEY ([SharedParameterProfileId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_SharedParameterProfile_Token' AND object_id IS NOT NULL)
    CREATE UNIQUE INDEX [UQ_SharedParameterProfile_Token] ON [core].[SharedParameterProfile] ([ProfileToken]);

-- ---------------------------------------------------------------------------
-- core.SharedParameterProfileField
-- ---------------------------------------------------------------------------
CREATE TABLE [core].[SharedParameterProfileField]
(
    [SharedParameterProfileFieldId] BIGINT IDENTITY(1,1) NOT NULL,
    [SharedParameterProfileFieldGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [SharedParameterProfileId] BIGINT NOT NULL,
    [SharedParameterCatalogId] BIGINT NOT NULL,
    [Audience] VARCHAR(20) NOT NULL,
    [SortOrder] INT NOT NULL DEFAULT 0,
    [OverrideLabel] NVARCHAR(200),
    [OverrideDataType] VARCHAR(30),
    [OverrideControlType] VARCHAR(30),
    [OverrideIsRequired] BIT,
    [OverrideLookupSource] NVARCHAR(200),
    [StatusId] INT NOT NULL DEFAULT 1,
    [CreatedById] BIGINT NOT NULL DEFAULT 0,
    [CreatedTime] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedById] BIGINT,
    [UpdatedTime] DATETIME2(7),
    [DeletedById] BIGINT,
    [DeletedTime] DATETIME2(7),
    CONSTRAINT [PK_SharedParameterProfileField] PRIMARY KEY ([SharedParameterProfileFieldId])
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SharedParameterProfileField_ProfileId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_SharedParameterProfileField_ProfileId] ON [core].[SharedParameterProfileField] ([SharedParameterProfileId]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SharedParameterProfileField_CatalogId' AND object_id IS NOT NULL)
    CREATE INDEX [IX_SharedParameterProfileField_CatalogId] ON [core].[SharedParameterProfileField] ([SharedParameterCatalogId]);

-- ---------------------------------------------------------------------------
-- Lookup / geo views
-- ---------------------------------------------------------------------------
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

