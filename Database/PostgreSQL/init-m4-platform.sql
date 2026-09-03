
-- ---------------------------------------------------------------------------
-- M4 platform shell stubs (Auth / Parameter / AutoNumber / Approval / views)
-- ---------------------------------------------------------------------------
CREATE SCHEMA IF NOT EXISTS apv;

CREATE TABLE IF NOT EXISTS core."Users" (
    "UserId"        BIGSERIAL PRIMARY KEY,
    "UserGuid"      UUID NOT NULL DEFAULT gen_random_uuid(),
    "UserName"      VARCHAR(100) NOT NULL,
    "PasswordHash"  TEXT NOT NULL,
    "PasswordSalt"  VARCHAR(128) NOT NULL,
    "FullName"      VARCHAR(200),
    "Email"         VARCHAR(200),
    "IsAdmin"       INT NOT NULL DEFAULT 0,
    "StatusId"      BIGINT NOT NULL DEFAULT 1,
    "CreatedById"   BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"   TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"   BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"   TIMESTAMPTZ,
    "DeletedById"   BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"   TIMESTAMPTZ,
    CONSTRAINT "UQ_Users_Guid" UNIQUE ("UserGuid"),
    CONSTRAINT "UQ_Users_UserName" UNIQUE ("UserName")
);

CREATE TABLE IF NOT EXISTS core."UserGroup" (
    "UserGroupId"   BIGSERIAL PRIMARY KEY,
    "UserGroupGuid" UUID NOT NULL DEFAULT gen_random_uuid(),
    "UserGroupName" VARCHAR(200) NOT NULL,
    "StatusId"      BIGINT NOT NULL DEFAULT 1,
    "CreatedById"   BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"   TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"   BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"   TIMESTAMPTZ,
    "DeletedById"   BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"   TIMESTAMPTZ,
    CONSTRAINT "UQ_UserGroup_Guid" UNIQUE ("UserGroupGuid")
);

CREATE TABLE IF NOT EXISTS core."Programs" (
    "ProgramId"                 BIGSERIAL PRIMARY KEY,
    "ProgramGuid"               UUID NOT NULL DEFAULT gen_random_uuid(),
    "ProgramCode"               VARCHAR(50) NOT NULL,
    "ProgramName"               VARCHAR(200) NOT NULL,
    "ParentId"                  BIGINT,
    "IconCode"                  VARCHAR(50),
    "TopLevelProgramId"         BIGINT,
    "RootLevel"                 INT,
    "RowIndex"                  INT,
    "Note"                      TEXT,
    "IsParentProgram"           INT NOT NULL DEFAULT 0,
    "IsProgram"                 INT NOT NULL DEFAULT 1,
    "IsBeginGroup"              INT NOT NULL DEFAULT 0,
    "ProgramType"               VARCHAR(10) NOT NULL DEFAULT 'MNU',
    "IsProgramAddAble"          INT NOT NULL DEFAULT 0,
    "IsProgramEditAble"         INT NOT NULL DEFAULT 0,
    "IsProgramDeleteAble"       INT NOT NULL DEFAULT 0,
    "IsProgramViewAble"         INT NOT NULL DEFAULT 1,
    "IsProgramApprovalAble"     INT NOT NULL DEFAULT 0,
    "IsProgramUnApprovalAble"   INT NOT NULL DEFAULT 0,
    "IsProgramVoidAble"         INT NOT NULL DEFAULT 0,
    "IsProgramUnVoidAble"       INT NOT NULL DEFAULT 0,
    "IsProgramDirectVoid"       INT NOT NULL DEFAULT 0,
    "IsProgramPrintAble"        INT NOT NULL DEFAULT 0,
    "IsMenuAddVisible"          INT NOT NULL DEFAULT 0,
    "IsMenuHomeVisible"         INT NOT NULL DEFAULT 0,
    "IsVisible"                 INT NOT NULL DEFAULT 1,
    "NavigateUrl"               VARCHAR(500),
    "HelpLinkId"                BIGINT,
    "AssemblyName"              VARCHAR(200),
    "AssemblyClassName"         VARCHAR(200),
    "StoreProcedureName"        VARCHAR(200),
    "AccessKey"                 VARCHAR(50),
    "IsActive"                  INT NOT NULL DEFAULT 1,
    "StatusId"                  BIGINT NOT NULL DEFAULT 1,
    "CreatedById"               BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"               TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"               BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"               TIMESTAMPTZ,
    "DeletedById"               BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"               TIMESTAMPTZ,
    CONSTRAINT "UQ_Programs_Guid" UNIQUE ("ProgramGuid")
);

CREATE TABLE IF NOT EXISTS core."UserGroupProgram" (
    "UserGroupProgramId"        BIGSERIAL PRIMARY KEY,
    "UserGroupProgramGuid"      UUID NOT NULL DEFAULT gen_random_uuid(),
    "UserGroupId"               BIGINT NOT NULL,
    "ProgramsId"                BIGINT NOT NULL,
    "IsUserGroupViewAble"       INT NOT NULL DEFAULT 1,
    "IsUserGroupAddAble"        INT NOT NULL DEFAULT 0,
    "IsUserGroupEditAble"       INT NOT NULL DEFAULT 0,
    "IsUserGroupDeleteAble"     INT NOT NULL DEFAULT 0,
    "IsUserGroupApprovalAble"   INT NOT NULL DEFAULT 0,
    "IsUserGroupUnApprovalAble" INT NOT NULL DEFAULT 0,
    "IsUserGroupVoidAble"       INT NOT NULL DEFAULT 0,
    "IsUserGroupUnVoidAble"     INT NOT NULL DEFAULT 0,
    "IsUserGroupExportAble"     INT NOT NULL DEFAULT 0,
    "StatusId"                  BIGINT NOT NULL DEFAULT 1,
    "CreatedById"               BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"               TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"               BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"               TIMESTAMPTZ,
    "DeletedById"               BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"               TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS core."UserGroupScope" (
    "UserGroupScopeId"      BIGSERIAL PRIMARY KEY,
    "UserGroupScopeGuid"    UUID NOT NULL DEFAULT gen_random_uuid(),
    "UserId"                BIGINT NOT NULL,
    "UserGuid"              UUID,
    "UserGroupId"           BIGINT NOT NULL,
    "UserGroupGuid"         UUID,
    "CompanyId"             BIGINT,
    "CompanyGuid"           UUID,
    "CompanyOfficeId"       BIGINT,
    "CompanyOfficeGuid"     UUID,
    "IsDefault"             INT NOT NULL DEFAULT 0,
    "StatusId"              BIGINT NOT NULL DEFAULT 1,
    "CreatedById"           BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"           TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"           BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"           TIMESTAMPTZ,
    "DeletedById"           BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"           TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS core."UserCompanyScope" (
    "UserCompanyScopeId"    BIGSERIAL PRIMARY KEY,
    "UserCompanyScopeGuid"  UUID NOT NULL DEFAULT gen_random_uuid(),
    "UserId"                BIGINT NOT NULL,
    "CompanyId"             BIGINT NOT NULL,
    "CompanyOfficeId"       BIGINT,
    "IsDefaultCompany"      INT NOT NULL DEFAULT 0,
    "IsDefaultOffice"       INT NOT NULL DEFAULT 0,
    "StatusId"              BIGINT NOT NULL DEFAULT 1,
    "CreatedById"           BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"           TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"           BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"           TIMESTAMPTZ,
    "DeletedById"           BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"           TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS app."Parameter" (
    "ParameterId"       BIGSERIAL PRIMARY KEY,
    "ParameterGuid"     UUID NOT NULL DEFAULT gen_random_uuid(),
    "ParameterName"     VARCHAR(200) NOT NULL,
    "ParameterValue"    TEXT,
    "Description"       TEXT,
    "StatusId"          BIGINT NOT NULL DEFAULT 1,
    "CreatedById"       BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"       TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"       BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"       TIMESTAMPTZ,
    "DeletedById"       BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"       TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS app."ParameterScope" (
    "ParameterScopeId"      BIGSERIAL PRIMARY KEY,
    "ParameterscopeGuid"    UUID NOT NULL DEFAULT gen_random_uuid(),
    "ParameterId"           BIGINT NOT NULL,
    "ParameterGuid"         UUID,
    "CompanyId"             BIGINT,
    "CompanyOfficeId"       BIGINT,
    "OverrideValue"         TEXT,
    "StatusId"              BIGINT NOT NULL DEFAULT 1,
    "CreatedById"           BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"           TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"           BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"           TIMESTAMPTZ,
    "DeletedById"           BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"           TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS app."StandardReferenceScope" (
    "StandardReferenceScopeId"  BIGSERIAL PRIMARY KEY,
    "StandardReferenceScopeGuid" UUID NOT NULL DEFAULT gen_random_uuid(),
    "StandardReferenceId"       BIGINT NOT NULL,
    "StandardReferenceGuid"     UUID,
    "CompanyId"                 BIGINT,
    "CompanyOfficeId"           BIGINT,
    "StatusId"                  BIGINT NOT NULL DEFAULT 1,
    "CreatedById"               BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"               TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"               BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"               TIMESTAMPTZ,
    "DeletedById"               BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"               TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS app."StandardReferenceScopeItem" (
    "StandardReferenceScopeItemId"      BIGSERIAL PRIMARY KEY,
    "StandardReferenceScopeItemGuid"    UUID NOT NULL DEFAULT gen_random_uuid(),
    "StandardReferenceScopeId"          BIGINT NOT NULL,
    "StandardReferenceScopeItemInitial" VARCHAR(50),
    "StandardReferenceScopeItemName"    VARCHAR(200),
    "StandardReferenceScopeItemValue"   VARCHAR(200),
    "DisplayOrder"                      INT NOT NULL DEFAULT 0,
    "StatusId"                          BIGINT NOT NULL DEFAULT 1,
    "CreatedById"                       BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"                       TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"                       BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"                       TIMESTAMPTZ,
    "DeletedById"                       BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"                       TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS app."AutoNumberTemplate" (
    "AutoNumberTemplateId"      BIGSERIAL PRIMARY KEY,
    "AutoNumberTemplateGuid"    UUID NOT NULL DEFAULT gen_random_uuid(),
    "TemplateName"              VARCHAR(200) NOT NULL,
    "StatusId"                  BIGINT NOT NULL DEFAULT 1,
    "CreatedById"               BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"               TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"               BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"               TIMESTAMPTZ,
    "DeletedById"               BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"               TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS app."AutoNumberCounter" (
    "AutoNumberCounterId"       BIGSERIAL PRIMARY KEY,
    "AutoNumberCounterGuid"     UUID NOT NULL DEFAULT gen_random_uuid(),
    "AutoNumberTemplateId"      BIGINT NOT NULL,
    "CompanyId"                 BIGINT,
    "CompanyOfficeId"           BIGINT,
    "OrganizationUnitId"        BIGINT,
    "YearNo"                    INT,
    "MonthNo"                   INT,
    "DayNo"                     INT,
    "LastNumber"                INT NOT NULL DEFAULT 0,
    "StatusId"                  BIGINT NOT NULL DEFAULT 1,
    "CreatedById"               BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"               TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"               BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"               TIMESTAMPTZ,
    "DeletedById"               BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"               TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS app."AutoNumberLog" (
    "AutoNumberLogId"           BIGSERIAL PRIMARY KEY,
    "AutoNumberLogGuid"         UUID NOT NULL DEFAULT gen_random_uuid(),
    "AutoNumberTemplateId"      BIGINT NOT NULL,
    "GeneratedNumber"           VARCHAR(100),
    "CounterValue"              INT,
    "Status"                    VARCHAR(50),
    "ReferenceId"               VARCHAR(100),
    "CompanyId"                 BIGINT,
    "CompanyOfficeId"           BIGINT,
    "OrganizationUnitId"        BIGINT,
    "YearNo"                    INT,
    "MonthNo"                   INT,
    "DayNo"                     INT,
    "CreatedById"               BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"               TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
);

CREATE TABLE IF NOT EXISTS apv."ApprovalTemplate" (
    "ApprovalTemplateId"    BIGSERIAL PRIMARY KEY,
    "ApprovalTemplateGuid"  UUID NOT NULL DEFAULT gen_random_uuid(),
    "TemplateName"          VARCHAR(200) NOT NULL,
    "ModuleCode"            VARCHAR(50),
    "Description"           TEXT,
    "IsActive"              INT NOT NULL DEFAULT 1,
    "StatusId"              BIGINT NOT NULL DEFAULT 1,
    "CreatedById"           BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"           TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"           BIGINT,
    "UpdatedTime"           TIMESTAMPTZ,
    "DeletedById"           BIGINT,
    "DeletedTime"           TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS apv."ApprovalRequest" (
    "ApprovalRequestId"     BIGSERIAL PRIMARY KEY,
    "ApprovalRequestGuid"   UUID NOT NULL DEFAULT gen_random_uuid(),
    "ModuleCode"            VARCHAR(50),
    "ReferenceGuid"         UUID,
    "StatusId"              INT NOT NULL DEFAULT 1,
    "CreatedById"           BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"           TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
);

CREATE OR REPLACE VIEW app."vw_StandardReference_Display" AS
SELECT
    i."StandardReferenceItemId",
    i."StandardReferenceItemName",
    0::BIGINT AS "CompanyId",
    0::BIGINT AS "CompanyOfficeId",
    NULL::BIGINT AS "StandardReferenceGroupId",
    r."StandardReferenceInitial"
FROM app."StandardReferenceItem" i
INNER JOIN app."StandardReference" r ON r."StandardReferenceId" = i."StandardReferenceId"
WHERE i."StatusId" > 0 AND i."DeletedTime" IS NULL;

CREATE OR REPLACE VIEW app."vw_StandardReferenceItem" AS
SELECT
    i."StandardReferenceItemId",
    i."StandardReferenceItemGuid",
    0::BIGINT AS "CompanyId",
    NULL::UUID AS "CompanyGuid",
    0::BIGINT AS "CompanyOfficeId",
    NULL::UUID AS "CompanyOfficeGuid",
    i."StandardReferenceId",
    r."StandardReferenceGuid",
    r."StandardReferenceInitial",
    i."StandardReferenceItemInitial",
    i."StandardReferenceItemName",
    NULL::TEXT AS "Note"
FROM app."StandardReferenceItem" i
INNER JOIN app."StandardReference" r ON r."StandardReferenceId" = i."StandardReferenceId"
WHERE i."StatusId" > 0 AND i."DeletedTime" IS NULL;
