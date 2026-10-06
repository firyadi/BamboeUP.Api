-- Platform + geo seed (from Dev.BamboeHR)
-- Apply AFTER init DDL, from Database/PostgreSQL directory:
--   psql ... -f seed/seed-bamboeup-hr.sql
SET client_min_messages TO WARNING;

TRUNCATE TABLE "core"."Users" CASCADE;
\copy "core"."Users" ("UserId", "UserGuid", "UserName", "PasswordHash", "PasswordSalt", "FullName", "Email", "IsAdmin", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/core.Users.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "core"."UserGroup" CASCADE;
\copy "core"."UserGroup" ("UserGroupId", "UserGroupGuid", "UserGroupName", "IsEditAble", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/core.UserGroup.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "core"."Programs" CASCADE;
\copy "core"."Programs" ("ProgramId", "ProgramGuid", "ProgramCode", "ParentId", "IconCode", "ProgramName", "TopLevelProgramId", "RootLevel", "RowIndex", "Note", "IsParentProgram", "IsProgram", "IsBeginGroup", "ProgramType", "IsProgramAddAble", "IsProgramEditAble", "IsProgramDeleteAble", "IsProgramViewAble", "IsProgramApprovalAble", "IsProgramUnApprovalAble", "IsProgramVoidAble", "IsProgramUnVoidAble", "IsProgramDirectVoid", "IsProgramPrintAble", "IsMenuAddVisible", "IsMenuHomeVisible", "IsVisible", "NavigateUrl", "HelpLinkId", "AssemblyName", "AssemblyClassName", "StoreProcedureName", "AccessKey", "IsActive", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/core.Programs.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "core"."UserGroupProgram" CASCADE;
\copy "core"."UserGroupProgram" ("UserGroupProgramId", "UserGroupProgramGuid", "UserGroupId", "ProgramsId", "IsUserGroupAddAble", "IsUserGroupEditAble", "IsUserGroupDeleteAble", "IsUserGroupApprovalAble", "IsUserGroupUnApprovalAble", "IsUserGroupVoidAble", "IsUserGroupUnVoidAble", "IsUserGroupExportAble", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime", "IsUserGroupViewAble") FROM 'seed/core.UserGroupProgram.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "app"."Company" CASCADE;
\copy "app"."Company" ("CompanyId", "CompanyGuid", "CompanyName", "InitialName", "TaxCompulsionNo", "RegistrationNo", "ParentCompanyId", "DefaultCurrency", "CompanyLogo", "LeaderId", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/app.Company.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "app"."CompanyOffice" CASCADE;
\copy "app"."CompanyOffice" ("CompanyOfficeId", "CompanyOfficeGuid", "CompanyId", "CompanyGuid", "CompanyOfficeName", "SrAddressType", "CountryId", "StateId", "CityId", "PostalCodeId", "Address", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime", "StatusId") FROM 'seed/app.CompanyOffice.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "app"."OrganizationUnit" CASCADE;
\copy "app"."OrganizationUnit" ("OrganizationUnitId", "OrganizationUnitGuid", "OrganizationUnitCode", "OrganizationUnitName", "ParentOrganizationUnitId", "SrOrganizationLevel", "LevelDepth", "HierarchyPath", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/app.OrganizationUnit.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "core"."UserGroupScope" CASCADE;
\copy "core"."UserGroupScope" ("UserGroupScopeId", "UserGroupScopeGuid", "UserId", "UserGuid", "UserGroupId", "UserGroupGuid", "CompanyId", "CompanyGuid", "CompanyOfficeId", "CompanyOfficeGuid", "IsDefault", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/core.UserGroupScope.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "core"."UserCompanyScope" CASCADE;
\copy "core"."UserCompanyScope" ("UserCompanyScopeId", "UserCompanyScopeGuid", "UserId", "CompanyId", "CompanyOfficeId", "IsDefaultCompany", "IsDefaultOffice", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/core.UserCompanyScope.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "app"."Parameter" CASCADE;
\copy "app"."Parameter" ("ParameterId", "ParameterGuid", "ParameterName", "ParameterValue", "ParameterType", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/app.Parameter.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "app"."ParameterScope" CASCADE;
\copy "app"."ParameterScope" ("ParameterScopeId", "ParameterScopeGuid", "ParameterId", "CompanyId", "CompanyGuid", "CompanyOfficeId", "CompanyOfficeGuid", "OverrideValue", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime", "ParameterGuid") FROM 'seed/app.ParameterScope.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "app"."StandardReference" CASCADE;
\copy "app"."StandardReference" ("StandardReferenceId", "StandardReferenceGuid", "StandardReferenceName", "StandardReferenceInitial", "Description", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/app.StandardReference.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "app"."StandardReferenceItem" CASCADE;
\copy "app"."StandardReferenceItem" ("StandardReferenceItemId", "StandardReferenceItemGuid", "StandardReferenceId", "StandardReferenceGuid", "StandardReferenceItemInitial", "StandardReferenceItemName", "StandardReferenceItemValue", "DisplayOrder", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/app.StandardReferenceItem.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "app"."StandardReferenceScope" CASCADE;
\copy "app"."StandardReferenceScope" ("StandardReferenceScopeId", "StandardReferenceScopeGuid", "StandardReferenceId", "StandardReferenceGuid", "CompanyId", "CompanyGuid", "CompanyOfficeId", "CompanyOfficeGuid", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/app.StandardReferenceScope.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "app"."StandardReferenceScopeItem" CASCADE;
\copy "app"."StandardReferenceScopeItem" ("StandardReferenceScopeItemId", "StandardReferenceScopeItemGuid", "StandardReferenceScopeId", "StandardReferenceScopeGuid", "StandardReferenceScopeItemInitial", "StandardReferenceScopeItemName", "StandardReferenceScopeItemValue", "DisplayOrder", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/app.StandardReferenceScopeItem.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "core"."Country" CASCADE;
\copy "core"."Country" ("CountryId", "CountryGuid", "CountryIso", "CountryName", "CountryIso3", "PhoneCode", "CurrencyCode", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/core.Country.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "core"."Province" CASCADE;
\copy "core"."Province" ("ProvinceId", "ProvinceGuid", "ProvinceName", "CountryId", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/core.Province.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "core"."City" CASCADE;
\copy "core"."City" ("CityId", "CityGuid", "CityName", "ProvinceId", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/core.City.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "core"."District" CASCADE;
\copy "core"."District" ("DistrictId", "DistrictGuid", "DistrictName", "CityId", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/core.District.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "core"."SubDistrict" CASCADE;
\copy "core"."SubDistrict" ("SubDistrictId", "SubDistrictGuid", "SubDistrictName", "DistrictId", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/core.SubDistrict.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "core"."PostalCode" CASCADE;
\copy "core"."PostalCode" ("PostalCodeId", "PostalCodeGuid", "SubDistrictId", "PostalCode", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/core.PostalCode.tsv' WITH (FORMAT text, NULL '\N')

TRUNCATE TABLE "core"."Bank" CASCADE;
\copy "core"."Bank" ("BankId", "BankGuid", "BankName", "BankInitial", "StatusId", "CreatedById", "CreatedTime", "UpdatedById", "UpdatedTime", "DeletedById", "DeletedTime") FROM 'seed/core.Bank.tsv' WITH (FORMAT text, NULL '\N')

DO $$
DECLARE r record;
        mx bigint;
BEGIN
  FOR r IN
    SELECT n.nspname AS sch, c.relname AS tbl, a.attname AS col,
           pg_get_serial_sequence(format('%I.%I', n.nspname, c.relname), a.attname) AS seq
    FROM pg_class c
    JOIN pg_namespace n ON n.oid = c.relnamespace
    JOIN pg_attribute a ON a.attrelid = c.oid AND a.attnum > 0 AND NOT a.attisdropped
    WHERE n.nspname IN ('app','core','apv')
      AND c.relkind = 'r'
      AND a.attidentity <> ''
  LOOP
    IF r.seq IS NULL THEN CONTINUE; END IF;
    EXECUTE format('SELECT MAX(%I) FROM %I.%I', r.col, r.sch, r.tbl) INTO mx;
    IF mx IS NULL THEN
      EXECUTE format('SELECT setval(%L, 1, false)', r.seq);
    ELSE
      EXECUTE format('SELECT setval(%L, %s, true)', r.seq, mx);
    END IF;
  END LOOP;
END
$$;
