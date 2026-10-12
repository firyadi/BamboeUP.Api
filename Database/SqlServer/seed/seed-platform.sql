-- Platform seed for SQL Server (from Dev.BamboeHR)
-- Prefer: pwsh -File Load-PlatformSeed.ps1 -Database YourDb
-- Or BULK INSERT below (paths are absolute to this machine's export folder).
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

-- core.Users (7 rows)
DELETE FROM [core].[Users];
GO
BULK INSERT [core].[Users] ([UserId], [UserGuid], [UserName], [PasswordHash], [PasswordSalt], [FullName], [Email], [IsAdmin], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime], [PersonId])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.Users.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.UserGroup (3 rows)
DELETE FROM [core].[UserGroup];
GO
BULK INSERT [core].[UserGroup] ([UserGroupId], [UserGroupGuid], [UserGroupName], [IsEditAble], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.UserGroup.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.Programs (185 rows)
DELETE FROM [core].[Programs];
GO
BULK INSERT [core].[Programs] ([ProgramId], [ProgramGuid], [ProgramCode], [ParentId], [IconCode], [ProgramName], [TopLevelProgramId], [RootLevel], [RowIndex], [Note], [IsParentProgram], [IsProgram], [IsBeginGroup], [ProgramType], [IsProgramAddAble], [IsProgramEditAble], [IsProgramDeleteAble], [IsProgramViewAble], [IsProgramApprovalAble], [IsProgramUnApprovalAble], [IsProgramVoidAble], [IsProgramUnVoidAble], [IsProgramDirectVoid], [IsProgramPrintAble], [IsMenuAddVisible], [IsMenuHomeVisible], [IsVisible], [NavigateUrl], [HelpLinkId], [AssemblyName], [AssemblyClassName], [StoreProcedureName], [AccessKey], [IsActive], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.Programs.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.UserGroupProgram (144 rows)
DELETE FROM [core].[UserGroupProgram];
GO
BULK INSERT [core].[UserGroupProgram] ([UserGroupProgramId], [UserGroupProgramGuid], [UserGroupId], [ProgramsId], [IsUserGroupAddAble], [IsUserGroupEditAble], [IsUserGroupDeleteAble], [IsUserGroupApprovalAble], [IsUserGroupUnApprovalAble], [IsUserGroupVoidAble], [IsUserGroupUnVoidAble], [IsUserGroupExportAble], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime], [IsUserGroupViewAble])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.UserGroupProgram.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.UserGroupScope (8 rows)
DELETE FROM [core].[UserGroupScope];
GO
BULK INSERT [core].[UserGroupScope] ([UserGroupScopeId], [UserGroupScopeGuid], [UserId], [UserGuid], [UserGroupId], [UserGroupGuid], [CompanyId], [CompanyGuid], [CompanyOfficeId], [CompanyOfficeGuid], [IsDefault], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime], [PersonId])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.UserGroupScope.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.UserCompanyScope (0 rows)
DELETE FROM [core].[UserCompanyScope];
GO
BULK INSERT [core].[UserCompanyScope] ([UserCompanyScopeId], [UserCompanyScopeGuid], [UserId], [CompanyId], [CompanyOfficeId], [IsDefaultCompany], [IsDefaultOffice], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.UserCompanyScope.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.UserOrganizationScope (0 rows)
DELETE FROM [core].[UserOrganizationScope];
GO
BULK INSERT [core].[UserOrganizationScope] ([UserOrganizationScopeId], [UserOrganizationScopeGuid], [UserId], [OrganizationUnitId], [CompanyId], [CompanyOfficeId], [AccessType], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.UserOrganizationScope.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- app.Parameter (3 rows)
DELETE FROM [app].[Parameter];
GO
BULK INSERT [app].[Parameter] ([ParameterId], [ParameterGuid], [ParameterName], [ParameterValue], [ParameterType], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\app.Parameter.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- app.ParameterScope (10 rows)
DELETE FROM [app].[ParameterScope];
GO
BULK INSERT [app].[ParameterScope] ([ParameterScopeId], [ParameterScopeGuid], [ParameterId], [CompanyId], [CompanyGuid], [CompanyOfficeId], [CompanyOfficeGuid], [OverrideValue], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime], [ParameterGuid])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\app.ParameterScope.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- app.StandardReference (68 rows)
DELETE FROM [app].[StandardReference];
GO
BULK INSERT [app].[StandardReference] ([StandardReferenceId], [StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\app.StandardReference.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- app.StandardReferenceItem (416 rows)
DELETE FROM [app].[StandardReferenceItem];
GO
BULK INSERT [app].[StandardReferenceItem] ([StandardReferenceItemId], [StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\app.StandardReferenceItem.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- app.StandardReferenceScope (0 rows)
DELETE FROM [app].[StandardReferenceScope];
GO
BULK INSERT [app].[StandardReferenceScope] ([StandardReferenceScopeId], [StandardReferenceScopeGuid], [StandardReferenceId], [StandardReferenceGuid], [CompanyId], [CompanyGuid], [CompanyOfficeId], [CompanyOfficeGuid], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\app.StandardReferenceScope.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- app.StandardReferenceScopeItem (0 rows)
DELETE FROM [app].[StandardReferenceScopeItem];
GO
BULK INSERT [app].[StandardReferenceScopeItem] ([StandardReferenceScopeItemId], [StandardReferenceScopeItemGuid], [StandardReferenceScopeId], [StandardReferenceScopeGuid], [StandardReferenceScopeItemInitial], [StandardReferenceScopeItemName], [StandardReferenceScopeItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\app.StandardReferenceScopeItem.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.Country (250 rows)
DELETE FROM [core].[Country];
GO
BULK INSERT [core].[Country] ([CountryId], [CountryGuid], [CountryIso], [CountryName], [CountryIso3], [PhoneCode], [CurrencyCode], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.Country.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.Province (34 rows)
DELETE FROM [core].[Province];
GO
BULK INSERT [core].[Province] ([ProvinceId], [ProvinceGuid], [ProvinceName], [CountryId], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.Province.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.City (475 rows)
DELETE FROM [core].[City];
GO
BULK INSERT [core].[City] ([CityId], [CityGuid], [CityName], [ProvinceId], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.City.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.District (6994 rows)
DELETE FROM [core].[District];
GO
BULK INSERT [core].[District] ([DistrictId], [DistrictGuid], [DistrictName], [CityId], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.District.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.SubDistrict (81225 rows)
DELETE FROM [core].[SubDistrict];
GO
BULK INSERT [core].[SubDistrict] ([SubDistrictId], [SubDistrictGuid], [SubDistrictName], [DistrictId], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.SubDistrict.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.PostalCode (81248 rows)
DELETE FROM [core].[PostalCode];
GO
BULK INSERT [core].[PostalCode] ([PostalCodeId], [PostalCodeGuid], [SubDistrictId], [PostalCode], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.PostalCode.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.Bank (51 rows)
DELETE FROM [core].[Bank];
GO
BULK INSERT [core].[Bank] ([BankId], [BankGuid], [BankName], [BankInitial], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.Bank.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.ReportDefinition (29 rows)
DELETE FROM [core].[ReportDefinition];
GO
BULK INSERT [core].[ReportDefinition] ([ReportDefinitionId], [ReportDefinitionGuid], [ProgramId], [ReportScope], [CompanyId], [ReportKind], [DefinitionKey], [FilePath], [StoreProcedureName], [RequiresPrintId], [PrintIdPolicy], [PrintIdPrefix], [Version], [IsActive], [EffectiveFrom], [EffectiveTo], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [IsTracked], [RendererType], [LayoutJson])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.ReportDefinition.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.ReportParameter (36 rows)
DELETE FROM [core].[ReportParameter];
GO
BULK INSERT [core].[ReportParameter] ([ReportParameterId], [ReportDefinitionId], [ParameterName], [DisplayLabel], [DataType], [IsRequired], [DefaultValue], [SortOrder], [LookupType], [IsSensitive], [StatusId], [CreatedById], [CreatedTime], [FieldKey], [ControlType], [ColumnGroup], [ColumnSpan], [RowGroup], [VisibleWhen], [CatalogFieldId])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.ReportParameter.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.ReportParameterCatalog (0 rows)
DELETE FROM [core].[ReportParameterCatalog];
GO
BULK INSERT [core].[ReportParameterCatalog] ([CatalogFieldId], [FieldKey], [DefaultLabel], [ControlType], [DataType], [LookupType], [DefaultColumnGroup], [IsSystemField], [StatusId])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.ReportParameterCatalog.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.SharedParameterCatalog (10 rows)
DELETE FROM [core].[SharedParameterCatalog];
GO
BULK INSERT [core].[SharedParameterCatalog] ([SharedParameterCatalogId], [SharedParameterCatalogGuid], [Code], [Label], [DataType], [CanonicalControlType], [ProcessControlType], [ReportControlType], [IsRequired], [DefaultValue], [LookupSource], [GroupName], [SortOrder], [IsSystemField], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.SharedParameterCatalog.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.SharedParameterProfile (5 rows)
DELETE FROM [core].[SharedParameterProfile];
GO
BULK INSERT [core].[SharedParameterProfile] ([SharedParameterProfileId], [SharedParameterProfileGuid], [ProfileToken], [ProfileLabel], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.SharedParameterProfile.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- core.SharedParameterProfileField (34 rows)
DELETE FROM [core].[SharedParameterProfileField];
GO
BULK INSERT [core].[SharedParameterProfileField] ([SharedParameterProfileFieldId], [SharedParameterProfileFieldGuid], [SharedParameterProfileId], [SharedParameterCatalogId], [Audience], [SortOrder], [OverrideLabel], [OverrideDataType], [OverrideControlType], [OverrideIsRequired], [OverrideLookupSource], [StatusId], [CreatedById], [CreatedTime], [UpdatedById], [UpdatedTime], [DeletedById], [DeletedTime])
FROM 'D:\Project\BamboeUp\BamboeUp.Api\Database\SqlServer\seed\core.SharedParameterProfileField.tsv'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    KEEPIDENTITY,
    TABLOCK,
    CODEPAGE = '65001'
);
GO

-- Reseed identities to MAX
DECLARE @t sysname, @s sysname, @c sysname, @m bigint, @sql nvarchar(400);
DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
SELECT s.name, t.name, ic.name
FROM sys.tables t
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
INNER JOIN sys.identity_columns ic ON ic.object_id = t.object_id
WHERE s.name IN (N'app', N'core', N'apv');
OPEN cur;
FETCH NEXT FROM cur INTO @s, @t, @c;
WHILE @@FETCH_STATUS = 0
BEGIN
  SET @sql = N'SELECT @mx = ISNULL(MAX(' + QUOTENAME(@c) + N'), 0) FROM ' + QUOTENAME(@s) + N'.' + QUOTENAME(@t);
  EXEC sp_executesql @sql, N'@mx bigint OUTPUT', @mx = @m OUTPUT;
  IF @m < 1 SET @m = 1;
  DBCC CHECKIDENT (@s + N'.' + @t, RESEED, @m) WITH NO_INFOMSGS;
  FETCH NEXT FROM cur INTO @s, @t, @c;
END
CLOSE cur; DEALLOCATE cur;
GO
