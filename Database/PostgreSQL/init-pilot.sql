-- BamboeUp.Api — PostgreSQL pilot schema (M1)
-- Modules: Hospital, Skill, Bank, Certification + StandardReference lookup
-- Run automatically via docker-compose.postgres.yml (docker-entrypoint-initdb.d)

CREATE SCHEMA IF NOT EXISTS app;
CREATE SCHEMA IF NOT EXISTS core;

-- ---------------------------------------------------------------------------
-- StandardReference (lookup for Certification + Skill FK columns)
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS app."StandardReference"
(
    "StandardReferenceId"       BIGSERIAL PRIMARY KEY,
    "StandardReferenceGuid"     UUID NOT NULL DEFAULT gen_random_uuid(),
    "StandardReferenceName"     VARCHAR(100) NOT NULL,
    "StandardReferenceInitial"  VARCHAR(50) NOT NULL,
    "Description"               VARCHAR(500),
    "StatusId"                  INT NOT NULL DEFAULT 1,
    "CreatedById"               BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"               TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"               BIGINT,
    "UpdatedTime"               TIMESTAMPTZ,
    "DeletedById"               BIGINT,
    "DeletedTime"               TIMESTAMPTZ,
    CONSTRAINT "UQ_StandardReference_Initial" UNIQUE ("StandardReferenceInitial")
);

CREATE TABLE IF NOT EXISTS app."StandardReferenceItem"
(
    "StandardReferenceItemId"       BIGSERIAL PRIMARY KEY,
    "StandardReferenceItemGuid"     UUID NOT NULL DEFAULT gen_random_uuid(),
    "StandardReferenceId"           BIGINT NOT NULL REFERENCES app."StandardReference"("StandardReferenceId"),
    "StandardReferenceGuid"         UUID NOT NULL,
    "StandardReferenceItemInitial"  VARCHAR(50) NOT NULL,
    "StandardReferenceItemName"     VARCHAR(100) NOT NULL,
    "StandardReferenceItemValue"    VARCHAR(100),
    "DisplayOrder"                  INT NOT NULL DEFAULT 0,
    "StatusId"                      INT NOT NULL DEFAULT 1,
    "CreatedById"                   BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"                   TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"                   BIGINT,
    "UpdatedTime"                   TIMESTAMPTZ,
    "DeletedById"                   BIGINT,
    "DeletedTime"                   TIMESTAMPTZ,
    CONSTRAINT "UX_StandardReferenceItem" UNIQUE ("StandardReferenceId", "StandardReferenceItemInitial")
);

-- ---------------------------------------------------------------------------
-- Hospital
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS app."Hospital"
(
    "HospitalId"    BIGSERIAL PRIMARY KEY,
    "HospitalGuid"  UUID NOT NULL DEFAULT gen_random_uuid(),
    "HospitalName"  VARCHAR(200) NOT NULL,
    "HospitalCode"  VARCHAR(20),
    "ShortName"     VARCHAR(100),
    "LicenseNo"     VARCHAR(50),
    "HospitalType"  VARCHAR(30),
    "HospitalClass" VARCHAR(10),
    "PhoneNo"       VARCHAR(30),
    "Email"         VARCHAR(200),
    "Website"       VARCHAR(200),
    "StatusId"      INT NOT NULL DEFAULT 1,
    "CreatedById"   BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"   TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"   BIGINT,
    "UpdatedTime"   TIMESTAMPTZ,
    "DeletedById"   BIGINT,
    "DeletedTime"   TIMESTAMPTZ,
    CONSTRAINT "UQ_Hospital_Guid" UNIQUE ("HospitalGuid"),
    CONSTRAINT "UQ_Hospital_Code" UNIQUE ("HospitalCode"),
    CONSTRAINT "UQ_Hospital_Name" UNIQUE ("HospitalName")
);

-- ---------------------------------------------------------------------------
-- Skill
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS app."Skill"
(
    "SkillId"                   BIGSERIAL PRIMARY KEY,
    "SkillGuid"                 UUID NOT NULL DEFAULT gen_random_uuid(),
    "SkillCode"                 VARCHAR(50) NOT NULL,
    "SkillName"                 VARCHAR(200) NOT NULL,
    "SkillShortName"            VARCHAR(100),
    "SrSkillCategory"           BIGINT NOT NULL DEFAULT 0,
    "SrSkillType"               BIGINT,
    "SrDefaultSkillLevel"       BIGINT,
    "IsCertificationRequired"   BOOLEAN NOT NULL DEFAULT FALSE,
    "IsMandatory"               BOOLEAN NOT NULL DEFAULT FALSE,
    "FileStorageId"             BIGINT,
    "Description"               TEXT,
    "IsActive"                  BOOLEAN NOT NULL DEFAULT TRUE,
    "StatusId"                  BIGINT NOT NULL DEFAULT 1,
    "CreatedById"               BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"               TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"               BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"               TIMESTAMPTZ,
    "DeletedById"               BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"               TIMESTAMPTZ,
    CONSTRAINT "UQ_Skill_Guid" UNIQUE ("SkillGuid"),
    CONSTRAINT "UQ_Skill_Code" UNIQUE ("SkillCode")
);

-- ---------------------------------------------------------------------------
-- Certification
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS app."Certification"
(
    "CertificationId"           BIGSERIAL PRIMARY KEY,
    "CertificationGuid"         UUID NOT NULL DEFAULT gen_random_uuid(),
    "CertificationCode"         VARCHAR(50) NOT NULL,
    "CertificationName"         VARCHAR(200) NOT NULL,
    "CertificationShortName"    VARCHAR(100),
    "SrCertificationCategory"   BIGINT NOT NULL DEFAULT 0,
    "SrCertificationProvider"   BIGINT NOT NULL DEFAULT 0,
    "DefaultValidityMonth"      INT,
    "IsLifetime"                BOOLEAN NOT NULL DEFAULT FALSE,
    "IsRenewable"               BOOLEAN NOT NULL DEFAULT FALSE,
    "LogoFileStorageId"         BIGINT,
    "Description"               TEXT,
    "IsActive"                  BOOLEAN NOT NULL DEFAULT TRUE,
    "StatusId"                  BIGINT NOT NULL DEFAULT 1,
    "CreatedById"               BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"               TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"               BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"               TIMESTAMPTZ,
    "DeletedById"               BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"               TIMESTAMPTZ,
    CONSTRAINT "UQ_Certification_Guid" UNIQUE ("CertificationGuid"),
    CONSTRAINT "UQ_Certification_Code" UNIQUE ("CertificationCode")
);

-- ---------------------------------------------------------------------------
-- Bank (core schema)
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS core."Bank"
(
    "BankId"        BIGSERIAL PRIMARY KEY,
    "BankGuid"      UUID NOT NULL DEFAULT gen_random_uuid(),
    "BankName"      VARCHAR(200) NOT NULL,
    "BankInitial"   VARCHAR(15) NOT NULL,
    "StatusId"      BIGINT NOT NULL DEFAULT 1,
    "CreatedById"   BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"   TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"   BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"   TIMESTAMPTZ,
    "DeletedById"   BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"   TIMESTAMPTZ,
    CONSTRAINT "UQ_Bank_Guid" UNIQUE ("BankGuid")
);

-- ---------------------------------------------------------------------------
-- Seed: StandardReference for pilot modules
-- ---------------------------------------------------------------------------
INSERT INTO app."StandardReference"
    ("StandardReferenceGuid", "StandardReferenceName", "StandardReferenceInitial", "Description")
VALUES
    (gen_random_uuid(), 'Skill Category', 'SkillCategory', 'Pilot seed'),
    (gen_random_uuid(), 'Certification Category', 'CertificationCategory', 'Pilot seed'),
    (gen_random_uuid(), 'Certification Provider', 'CertificationProvider', 'Pilot seed')
ON CONFLICT ("StandardReferenceInitial") DO NOTHING;

INSERT INTO app."StandardReferenceItem"
    ("StandardReferenceItemGuid", "StandardReferenceId", "StandardReferenceGuid",
     "StandardReferenceItemInitial", "StandardReferenceItemName", "StandardReferenceItemValue", "DisplayOrder")
SELECT gen_random_uuid(), sr."StandardReferenceId", sr."StandardReferenceGuid",
       v.initial, v.name, v.value, v.ord
FROM app."StandardReference" sr
CROSS JOIN (VALUES
    ('SkillCategory', 'TECH', 'Technical', 'Technical', 1),
    ('SkillCategory', 'SOFT', 'Soft Skill', 'Soft Skill', 2),
    ('CertificationCategory', 'PROF', 'Professional', 'Professional', 1),
    ('CertificationCategory', 'TECH', 'Technical', 'Technical', 2),
    ('CertificationProvider', 'INTL', 'International Body', 'International', 1),
    ('CertificationProvider', 'LOCAL', 'Local Authority', 'Local', 2)
) AS v(ref_initial, initial, name, value, ord)
WHERE sr."StandardReferenceInitial" = v.ref_initial
ON CONFLICT ("StandardReferenceId", "StandardReferenceItemInitial") DO NOTHING;

-- ---------------------------------------------------------------------------
-- Audit schema (M2)
-- ---------------------------------------------------------------------------
CREATE SCHEMA IF NOT EXISTS log;

CREATE TABLE IF NOT EXISTS log."AuditSnapshot" (
    "SnapshotId" BIGSERIAL PRIMARY KEY,
    "TableName" VARCHAR(128) NOT NULL,
    "EntityKey" VARCHAR(128) NOT NULL,
    "SnapshotJson" TEXT NOT NULL,
    "SnapshotDateTime" TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
);

CREATE TABLE IF NOT EXISTS log."AuditSession" (
    "AuditSessionId" BIGSERIAL PRIMARY KEY,
    "SessionGuid" UUID NOT NULL DEFAULT gen_random_uuid(),
    "SessionType" VARCHAR(10) NOT NULL,
    "RootTableName" VARCHAR(128) NOT NULL,
    "RootEntityKey" VARCHAR(128) NOT NULL,
    "RootDisplayName" VARCHAR(256),
    "ActionByUserId" VARCHAR(128) NOT NULL,
    "LogDateTime" TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
);

CREATE TABLE IF NOT EXISTS log."AuditLog" (
    "AuditLogId" BIGSERIAL PRIMARY KEY,
    "AuditSessionId" BIGINT NOT NULL REFERENCES log."AuditSession"("AuditSessionId"),
    "TableName" VARCHAR(128) NOT NULL,
    "EntityKey" VARCHAR(128) NOT NULL,
    "EntityDisplayName" VARCHAR(256),
    "ParentTableName" VARCHAR(128),
    "ParentEntityKey" VARCHAR(128),
    "AuditActionType" VARCHAR(10) NOT NULL,
    "ActionByUserID" VARCHAR(128) NOT NULL,
    "LogDateTime" TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "SnapshotId" BIGINT NULL REFERENCES log."AuditSnapshot"("SnapshotId")
);

CREATE TABLE IF NOT EXISTS log."AuditLogData" (
    "AuditLogId" BIGINT NOT NULL REFERENCES log."AuditLog"("AuditLogId") ON DELETE CASCADE,
    "ColumnName" VARCHAR(128) NOT NULL,
    "OldValue" TEXT,
    "NewValue" TEXT,
    PRIMARY KEY ("AuditLogId", "ColumnName")
);

-- ---------------------------------------------------------------------------
-- M2 master data tables (minimal columns for dialect smoke tests)
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS app."Asset" (
    "AssetId" BIGSERIAL PRIMARY KEY,
    "AssetGuid" UUID NOT NULL DEFAULT gen_random_uuid(),
    "AssetCode" VARCHAR(50) NOT NULL,
    "AssetName" VARCHAR(200) NOT NULL,
    "AssetCategoryId" BIGINT,
    "AssetBrand" VARCHAR(100),
    "AssetModel" VARCHAR(100),
    "SerialNumber" VARCHAR(100),
    "Barcode" VARCHAR(100),
    "CompanyId" BIGINT,
    "CompanyOfficeId" BIGINT,
    "CostCenterId" BIGINT,
    "PurchaseDate" DATE,
    "PurchasePrice" NUMERIC(18,2),
    "CurrencyId" BIGINT,
    "WarrantyStartDate" DATE,
    "WarrantyEndDate" DATE,
    "SrAssetCondition" BIGINT,
    "SrAssetStatus" BIGINT,
    "CurrentLocation" VARCHAR(200),
    "FileStorageId" BIGINT,
    "Description" TEXT,
    "StatusId" BIGINT NOT NULL DEFAULT 1,
    "CreatedById" BIGINT NOT NULL DEFAULT 0,
    "CreatedTime" TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById" BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime" TIMESTAMPTZ,
    "DeletedById" BIGINT NOT NULL DEFAULT 0,
    "DeletedTime" TIMESTAMPTZ,
    CONSTRAINT "UQ_Asset_Guid" UNIQUE ("AssetGuid")
);

CREATE TABLE IF NOT EXISTS app."Training" (
    "TrainingId" BIGSERIAL PRIMARY KEY,
    "TrainingGuid" UUID NOT NULL DEFAULT gen_random_uuid(),
    "TrainingCode" VARCHAR(50) NOT NULL,
    "TrainingName" VARCHAR(200) NOT NULL,
    "TrainingShortName" VARCHAR(100),
    "SrTrainingCategory" BIGINT,
    "SrTrainingType" BIGINT,
    "SrTrainingLevel" BIGINT,
    "DurationHour" NUMERIC(10,2),
    "ValidityMonth" INT,
    "SrTrainingProvider" BIGINT,
    "IsMandatory" BOOLEAN NOT NULL DEFAULT FALSE,
    "IsCertification" BOOLEAN NOT NULL DEFAULT FALSE,
    "IsExternal" BOOLEAN NOT NULL DEFAULT FALSE,
    "FileStorageId" BIGINT,
    "Description" TEXT,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "StatusId" BIGINT NOT NULL DEFAULT 1,
    "CreatedById" BIGINT NOT NULL DEFAULT 0,
    "CreatedTime" TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById" BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime" TIMESTAMPTZ,
    "DeletedById" BIGINT NOT NULL DEFAULT 0,
    "DeletedTime" TIMESTAMPTZ,
    CONSTRAINT "UQ_Training_Guid" UNIQUE ("TrainingGuid")
);

CREATE TABLE IF NOT EXISTS core."Country" (
    "CountryId" BIGSERIAL PRIMARY KEY,
    "CountryGuid" UUID NOT NULL DEFAULT gen_random_uuid(),
    "CountryIso" VARCHAR(3) NOT NULL,
    "CountryName" VARCHAR(200) NOT NULL,
    "CountryIso3" VARCHAR(3),
    "PhoneCode" VARCHAR(10),
    "CurrencyCode" VARCHAR(3),
    "StatusId" INT NOT NULL DEFAULT 1,
    "CreatedById" BIGINT NOT NULL DEFAULT 0,
    "CreatedTime" TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById" BIGINT,
    "UpdatedTime" TIMESTAMPTZ,
    "DeletedById" BIGINT,
    "DeletedTime" TIMESTAMPTZ,
    CONSTRAINT "UQ_Country_Guid" UNIQUE ("CountryGuid")
);

CREATE TABLE IF NOT EXISTS app."Company" (
    "CompanyId" BIGSERIAL PRIMARY KEY,
    "CompanyGuid" UUID NOT NULL DEFAULT gen_random_uuid(),
    "CompanyName" VARCHAR(200) NOT NULL,
    "InitialName" VARCHAR(50),
    "TaxCompulsionNo" VARCHAR(50),
    "RegistrationNo" VARCHAR(50),
    "DefaultCurrency" VARCHAR(10),
    "CompanyLogo" VARCHAR(500),
    "LeaderId" BIGINT,
    "ParentCompanyId" BIGINT,
    "StatusId" BIGINT NOT NULL DEFAULT 1,
    "CreatedById" BIGINT NOT NULL DEFAULT 0,
    "CreatedTime" TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById" BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime" TIMESTAMPTZ,
    "DeletedById" BIGINT NOT NULL DEFAULT 0,
    "DeletedTime" TIMESTAMPTZ,
    CONSTRAINT "UQ_Company_Guid" UNIQUE ("CompanyGuid")
);

-- ---------------------------------------------------------------------------
-- M3 Person + Employee cluster (minimal columns for dialect smoke tests)
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS app."Person" (
    "PersonId"          BIGSERIAL PRIMARY KEY,
    "PersonGuid"        UUID NOT NULL DEFAULT gen_random_uuid(),
    "FirstName"         VARCHAR(100) NOT NULL,
    "MiddleName"        VARCHAR(100),
    "LastName"          VARCHAR(100),
    "PreTitle"          VARCHAR(50),
    "PostTitle"         VARCHAR(50),
    "PersonName"        VARCHAR(300) GENERATED ALWAYS AS (
        TRIM(BOTH FROM CONCAT_WS(' ', "FirstName", NULLIF("MiddleName", ''), NULLIF("LastName", '')))
    ) STORED,
    "BirthName"         VARCHAR(200),
    "PlaceofBirth"      VARCHAR(100),
    "BirthDate"         DATE,
    "NationalIdNo"      VARCHAR(50),
    "SrGender"          BIGINT NOT NULL DEFAULT 0,
    "SrReligion"        BIGINT NOT NULL DEFAULT 0,
    "SrSalutation"      BIGINT,
    "SrBloodType"       BIGINT,
    "SrMaritalStatus"   BIGINT,
    "FileStorageId"     BIGINT,
    "StatusId"          BIGINT NOT NULL DEFAULT 1,
    "CreatedById"       BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"       TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"       BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"       TIMESTAMPTZ,
    "DeletedById"       BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"       TIMESTAMPTZ,
    CONSTRAINT "UQ_Person_Guid" UNIQUE ("PersonGuid")
);

CREATE TABLE IF NOT EXISTS core."Province" (
    "ProvinceId"    BIGSERIAL PRIMARY KEY,
    "ProvinceGuid"  UUID NOT NULL DEFAULT gen_random_uuid(),
    "ProvinceName"  VARCHAR(200) NOT NULL,
    "CountryId"     BIGINT,
    "StatusId"      INT NOT NULL DEFAULT 1,
    "CreatedById"   BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"   TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"   BIGINT,
    "UpdatedTime"   TIMESTAMPTZ,
    "DeletedById"   BIGINT,
    "DeletedTime"   TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS core."City" (
    "CityId"        BIGSERIAL PRIMARY KEY,
    "CityGuid"      UUID NOT NULL DEFAULT gen_random_uuid(),
    "CityName"      VARCHAR(200) NOT NULL,
    "ProvinceId"    BIGINT,
    "StatusId"      INT NOT NULL DEFAULT 1,
    "CreatedById"   BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"   TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"   BIGINT,
    "UpdatedTime"   TIMESTAMPTZ,
    "DeletedById"   BIGINT,
    "DeletedTime"   TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS app."PersonAddress" (
    "PersonAddressId"   BIGSERIAL PRIMARY KEY,
    "PersonAddressGuid" UUID NOT NULL DEFAULT gen_random_uuid(),
    "PersonId"          BIGINT NOT NULL,
    "SrAddressType"     BIGINT NOT NULL DEFAULT 0,
    "Address"           VARCHAR(1000) NOT NULL,
    "CountryId"         BIGINT NOT NULL DEFAULT 0,
    "ProvinceId"        BIGINT NOT NULL DEFAULT 0,
    "CityId"            BIGINT NOT NULL DEFAULT 0,
    "StatusId"          BIGINT NOT NULL DEFAULT 1,
    "CreatedById"       BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"       TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"       BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"       TIMESTAMPTZ,
    "DeletedById"       BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"       TIMESTAMPTZ,
    CONSTRAINT "UQ_PersonAddress_Guid" UNIQUE ("PersonAddressGuid")
);

CREATE TABLE IF NOT EXISTS app."PersonContact" (
    "PersonContactId"   BIGSERIAL PRIMARY KEY,
    "PersonContactGuid" UUID NOT NULL DEFAULT gen_random_uuid(),
    "PersonId"          BIGINT NOT NULL,
    "SrContactType"     BIGINT NOT NULL DEFAULT 0,
    "ContactValue"      VARCHAR(200) NOT NULL,
    "IsPrimary"         BOOLEAN NOT NULL DEFAULT FALSE,
    "Remark"            VARCHAR(500),
    "StatusId"          BIGINT NOT NULL DEFAULT 1,
    "CreatedById"       BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"       TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"       BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"       TIMESTAMPTZ,
    "DeletedById"       BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"       TIMESTAMPTZ,
    CONSTRAINT "UQ_PersonContact_Guid" UNIQUE ("PersonContactGuid")
);

CREATE TABLE IF NOT EXISTS app."PersonEducation" (
    "PersonEducationId"   BIGSERIAL PRIMARY KEY,
    "PersonEducationGuid" UUID NOT NULL DEFAULT gen_random_uuid(),
    "PersonId"            BIGINT NOT NULL,
    "SrEducationLevel"    BIGINT NOT NULL DEFAULT 0,
    "InstitutionName"     VARCHAR(200) NOT NULL,
    "StatusId"            BIGINT NOT NULL DEFAULT 1,
    "CreatedById"         BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"         TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"         BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"         TIMESTAMPTZ,
    "DeletedById"         BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"         TIMESTAMPTZ,
    CONSTRAINT "UQ_PersonEducation_Guid" UNIQUE ("PersonEducationGuid")
);

CREATE TABLE IF NOT EXISTS app."PersonIdentification" (
    "PersonIdentificationId"    BIGSERIAL PRIMARY KEY,
    "PersonIdentificationGuid"  UUID NOT NULL DEFAULT gen_random_uuid(),
    "PersonId"                  BIGINT NOT NULL,
    "SrIdentificationType"      BIGINT NOT NULL DEFAULT 0,
    "IdentificationValue"       VARCHAR(100) NOT NULL,
    "StatusId"                  BIGINT NOT NULL DEFAULT 1,
    "CreatedById"               BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"               TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"               BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"               TIMESTAMPTZ,
    "DeletedById"               BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"               TIMESTAMPTZ,
    CONSTRAINT "UQ_PersonIdentification_Guid" UNIQUE ("PersonIdentificationGuid")
);

CREATE TABLE IF NOT EXISTS app."PersonEmergencyContact" (
    "PersonEmergencyContactId"   BIGSERIAL PRIMARY KEY,
    "PersonEmergencyContactGuid" UUID NOT NULL DEFAULT gen_random_uuid(),
    "PersonId"                   BIGINT NOT NULL,
    "ContactName"                VARCHAR(200) NOT NULL,
    "SrRelationship"             BIGINT NOT NULL DEFAULT 0,
    "Phone"                      VARCHAR(30),
    "IsPrimary"                  BOOLEAN NOT NULL DEFAULT FALSE,
    "StatusId"                   BIGINT NOT NULL DEFAULT 1,
    "CreatedById"                BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"                TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"                BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"                TIMESTAMPTZ,
    "DeletedById"                BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"                TIMESTAMPTZ,
    CONSTRAINT "UQ_PersonEmergencyContact_Guid" UNIQUE ("PersonEmergencyContactGuid")
);

CREATE TABLE IF NOT EXISTS app."PersonFamily" (
    "PersonFamilyId"    BIGSERIAL PRIMARY KEY,
    "PersonFamilyGuid"  UUID NOT NULL DEFAULT gen_random_uuid(),
    "PersonId"          BIGINT NOT NULL,
    "SrFamilyRelation"  BIGINT NOT NULL DEFAULT 0,
    "FamilyName"        VARCHAR(200) NOT NULL,
    "DateBirth"         DATE,
    "SrEducationLevel"  BIGINT,
    "Address"           VARCHAR(500),
    "StateId"           BIGINT,
    "CityId"            BIGINT,
    "ZipCode"           VARCHAR(20),
    "Phone"             VARCHAR(30),
    "SrMaritalStatus"   BIGINT,
    "SrGender"          BIGINT,
    "StatusId"          BIGINT NOT NULL DEFAULT 1,
    "CreatedById"       BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"       TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"       BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"       TIMESTAMPTZ,
    "DeletedById"       BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"       TIMESTAMPTZ,
    CONSTRAINT "UQ_PersonFamily_Guid" UNIQUE ("PersonFamilyGuid")
);

CREATE TABLE IF NOT EXISTS app."PersonBankAccount" (
    "PersonBankAccountId"   BIGSERIAL PRIMARY KEY,
    "PersonBankAccountGuid" UUID NOT NULL DEFAULT gen_random_uuid(),
    "PersonId"              BIGINT NOT NULL,
    "BankBranch"            VARCHAR(200),
    "AccountNumber"         VARCHAR(50) NOT NULL,
    "AccountName"           VARCHAR(200) NOT NULL,
    "SwiftCode"             VARCHAR(20),
    "IBAN"                  VARCHAR(50),
    "CurrencyId"            BIGINT,
    "SrBankAccountType"     BIGINT NOT NULL DEFAULT 0,
    "IsPrimary"             BOOLEAN NOT NULL DEFAULT FALSE,
    "IsPayrollAccount"      BOOLEAN NOT NULL DEFAULT FALSE,
    "IsActive"              BOOLEAN NOT NULL DEFAULT TRUE,
    "Remarks"               VARCHAR(500),
    "StatusId"              BIGINT NOT NULL DEFAULT 1,
    "CreatedById"           BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"           TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"           BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"           TIMESTAMPTZ,
    "DeletedById"           BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"           TIMESTAMPTZ,
    CONSTRAINT "UQ_PersonBankAccount_Guid" UNIQUE ("PersonBankAccountGuid")
);

CREATE TABLE IF NOT EXISTS app."PersonWorkExperience" (
    "PersonWorkExperienceId"    BIGSERIAL PRIMARY KEY,
    "PersonWorkExperienceGuid"  UUID NOT NULL DEFAULT gen_random_uuid(),
    "PersonId"                  BIGINT NOT NULL,
    "SrIndustry"                BIGINT,
    "SrEmploymentType"          BIGINT,
    "CompanyName"               VARCHAR(200) NOT NULL,
    "JobTitle"                  VARCHAR(200) NOT NULL,
    "Department"                VARCHAR(200),
    "Location"                  VARCHAR(200),
    "Supervisor"                VARCHAR(200),
    "JobDescription"            TEXT,
    "StartDate"                 DATE,
    "EndDate"                   DATE,
    "IsCurrentEmployment"       BOOLEAN NOT NULL DEFAULT FALSE,
    "LastSalary"                NUMERIC(18,2),
    "ReasonforLeaving"          VARCHAR(500),
    "Remarks"                   VARCHAR(500),
    "StatusId"                  BIGINT NOT NULL DEFAULT 1,
    "CreatedById"               BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"               TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"               BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"               TIMESTAMPTZ,
    "DeletedById"               BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"               TIMESTAMPTZ,
    CONSTRAINT "UQ_PersonWorkExperience_Guid" UNIQUE ("PersonWorkExperienceGuid")
);

CREATE TABLE IF NOT EXISTS app."PersonPhysicalCharacteristic" (
    "PersonPhysicalCharacteristicId"    BIGSERIAL PRIMARY KEY,
    "PersonPhysicalCharacteristicGuid"  UUID NOT NULL DEFAULT gen_random_uuid(),
    "PersonId"                          BIGINT NOT NULL,
    "SrPhysicalCharacteristic"          BIGINT NOT NULL DEFAULT 0,
    "PhysicalValue"                     VARCHAR(100) NOT NULL,
    "SrMeasurementUnit"                 BIGINT,
    "RecordedDate"                      DATE,
    "Remarks"                           VARCHAR(500),
    "StatusId"                          BIGINT NOT NULL DEFAULT 1,
    "CreatedById"                       BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"                       TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"                       BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"                       TIMESTAMPTZ,
    "DeletedById"                       BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"                       TIMESTAMPTZ,
    CONSTRAINT "UQ_PersonPhysicalCharacteristic_Guid" UNIQUE ("PersonPhysicalCharacteristicGuid")
);

CREATE TABLE IF NOT EXISTS app."Employee" (
    "EmployeeId"            BIGSERIAL PRIMARY KEY,
    "EmployeeGuid"          UUID NOT NULL DEFAULT gen_random_uuid(),
    "PersonId"              BIGINT NOT NULL,
    "EmployeeNo"            VARCHAR(30) NOT NULL,
    "HireDate"              DATE NOT NULL,
    "ProbationEndDate"      DATE,
    "ConfirmationDate"      DATE,
    "TerminationDate"       DATE,
    "SrEmploymentStatus"    BIGINT NOT NULL DEFAULT 0,
    "SrEmploymentType"      BIGINT NOT NULL DEFAULT 0,
    "SrRemunerationType"    BIGINT,
    "StatusId"              BIGINT NOT NULL DEFAULT 1,
    "CreatedById"           BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"           TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"           BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"           TIMESTAMPTZ,
    "DeletedById"           BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"           TIMESTAMPTZ,
    CONSTRAINT "UQ_Employee_Guid" UNIQUE ("EmployeeGuid"),
    CONSTRAINT "UQ_Employee_No" UNIQUE ("EmployeeNo")
);

CREATE TABLE IF NOT EXISTS app."EmployeeAssignment" (
    "EmployeeAssignmentId"      BIGSERIAL PRIMARY KEY,
    "EmployeeAssignmentGuid"    UUID NOT NULL DEFAULT gen_random_uuid(),
    "EmployeeAssignmentName"    VARCHAR(200),
    "EmployeeId"                BIGINT NOT NULL,
    "CompanyId"                 BIGINT NOT NULL DEFAULT 0,
    "CompanyOfficeId"           BIGINT,
    "OrganizationUnitId"        BIGINT NOT NULL DEFAULT 0,
    "PositionId"                BIGINT,
    "JobTitleId"                BIGINT,
    "JobGradeId"                BIGINT,
    "JobLevelId"                BIGINT,
    "ManagerEmployeeId"         BIGINT,
    "SupervisorEmployeeId"      BIGINT,
    "CostCenterId"              BIGINT,
    "PayrollGroupId"            BIGINT,
    "ShiftId"                   BIGINT,
    "WorkLocationId"            BIGINT,
    "SrEmploymentStatus"        BIGINT NOT NULL DEFAULT 0,
    "SrEmploymentType"          BIGINT NOT NULL DEFAULT 0,
    "SrAssignmentType"          BIGINT NOT NULL DEFAULT 0,
    "SrAssignmentReason"        BIGINT,
    "EffectiveFrom"             DATE NOT NULL DEFAULT CURRENT_DATE,
    "Effectiveto"               DATE,
    "IsPrimary"                 BOOLEAN NOT NULL DEFAULT TRUE,
    "IsCurrent"                 BOOLEAN NOT NULL DEFAULT TRUE,
    "Remarks"                   VARCHAR(500),
    "StatusId"                  BIGINT NOT NULL DEFAULT 1,
    "CreatedById"               BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"               TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"               BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"               TIMESTAMPTZ,
    "DeletedById"               BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"               TIMESTAMPTZ,
    CONSTRAINT "UQ_EmployeeAssignment_Guid" UNIQUE ("EmployeeAssignmentGuid")
);

CREATE TABLE IF NOT EXISTS app."EmployeeContract" (
    "EmployeeContractId"    BIGSERIAL PRIMARY KEY,
    "EmployeeContractGuid"  UUID NOT NULL DEFAULT gen_random_uuid(),
    "EmployeeContractName"  VARCHAR(200),
    "EmployeeId"            BIGINT NOT NULL,
    "EmployeeAssignmentId"  BIGINT,
    "ContractNo"            VARCHAR(50) NOT NULL,
    "SrContractType"        BIGINT NOT NULL DEFAULT 0,
    "SrContractStatus"      BIGINT NOT NULL DEFAULT 0,
    "ContractStartDate"     DATE,
    "ContractEndDate"       DATE,
    "EffectiveDate"         DATE,
    "RenewalSequence"       INT,
    "IsCurrent"             BOOLEAN NOT NULL DEFAULT TRUE,
    "FileStorageId"         BIGINT,
    "Remarks"               VARCHAR(500),
    "StatusId"              BIGINT NOT NULL DEFAULT 1,
    "CreatedById"           BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"           TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"           BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"           TIMESTAMPTZ,
    "DeletedById"           BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"           TIMESTAMPTZ,
    CONSTRAINT "UQ_EmployeeContract_Guid" UNIQUE ("EmployeeContractGuid")
);

CREATE TABLE IF NOT EXISTS app."EmployeeCareer" (
    "EmployeeCareerId"          BIGSERIAL PRIMARY KEY,
    "EmployeeCareerGuid"        UUID NOT NULL DEFAULT gen_random_uuid(),
    "EmployeeId"                BIGINT NOT NULL,
    "EmployeeAssignmentId"      BIGINT,
    "EmployeeContractId"        BIGINT,
    "SrCareerEvent"             BIGINT NOT NULL DEFAULT 0,
    "SrCareerReason"            BIGINT,
    "EffectiveDate"             DATE,
    "FromCompanyId"             BIGINT,
    "FromOrganizationUnitId"    BIGINT,
    "FromPositionId"            BIGINT,
    "FromJobTitleId"            BIGINT,
    "FromJobGradeId"            BIGINT,
    "FromJobLevelId"            BIGINT,
    "FromManagerEmployeeId"     BIGINT,
    "ToCompanyId"               BIGINT,
    "ToOrganizationUnitId"      BIGINT,
    "ToPositionId"              BIGINT,
    "ToJobTitleId"              BIGINT,
    "ToJobGradeId"              BIGINT,
    "ToJobLevelId"              BIGINT,
    "ToManagerEmployeeId"       BIGINT,
    "ReferenceNo"               VARCHAR(50),
    "ReferenceDate"             DATE,
    "Remarks"                   VARCHAR(500),
    "StatusId"                  BIGINT NOT NULL DEFAULT 1,
    "CreatedById"               BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"               TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"               BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"               TIMESTAMPTZ,
    "DeletedById"               BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"               TIMESTAMPTZ,
    CONSTRAINT "UQ_EmployeeCareer_Guid" UNIQUE ("EmployeeCareerGuid")
);

CREATE TABLE IF NOT EXISTS app."EmployeeSkill" (
    "EmployeeSkillId"           BIGSERIAL PRIMARY KEY,
    "EmployeeSkillGuid"         UUID NOT NULL DEFAULT gen_random_uuid(),
    "EmployeeId"                BIGINT NOT NULL,
    "SkillId"                   BIGINT NOT NULL DEFAULT 0,
    "SrSkillLevel"              BIGINT,
    "YearsofExperience"         NUMERIC(10,2),
    "LastUsedDate"              DATE,
    "IsPrimary"                 BOOLEAN NOT NULL DEFAULT FALSE,
    "IsCertified"               BOOLEAN NOT NULL DEFAULT FALSE,
    "CertificateNumber"         VARCHAR(100),
    "CertificateIssueDate"      DATE,
    "CertificateExpiredDate"    DATE,
    "FileStorageId"             BIGINT,
    "Remarks"                   VARCHAR(500),
    "StatusId"                  BIGINT NOT NULL DEFAULT 1,
    "CreatedById"               BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"               TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"               BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"               TIMESTAMPTZ,
    "DeletedById"               BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"               TIMESTAMPTZ,
    CONSTRAINT "UQ_EmployeeSkill_Guid" UNIQUE ("EmployeeSkillGuid")
);

CREATE TABLE IF NOT EXISTS app."EmployeeLanguage" (
    "EmployeeLanguageId"    BIGSERIAL PRIMARY KEY,
    "EmployeeLanguageGuid"  UUID NOT NULL DEFAULT gen_random_uuid(),
    "EmployeeId"            BIGINT NOT NULL,
    "SrLanguage"            BIGINT NOT NULL DEFAULT 0,
    "SrReadingLevel"        BIGINT,
    "SrWritingLevel"        BIGINT,
    "SrListeningLevel"      BIGINT,
    "SrSpeakingLevel"       BIGINT,
    "CertificationName"     VARCHAR(200),
    "Score"                 NUMERIC(10,2),
    "IssuedDate"            DATE,
    "ExpiredDate"           DATE,
    "FileStorageId"         BIGINT,
    "IsPrimary"             BOOLEAN NOT NULL DEFAULT FALSE,
    "Remarks"               VARCHAR(500),
    "StatusId"              BIGINT NOT NULL DEFAULT 1,
    "CreatedById"           BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"           TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"           BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"           TIMESTAMPTZ,
    "DeletedById"           BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"           TIMESTAMPTZ,
    CONSTRAINT "UQ_EmployeeLanguage_Guid" UNIQUE ("EmployeeLanguageGuid")
);

CREATE TABLE IF NOT EXISTS app."EmployeeTraining" (
    "EmployeeTrainingId"    BIGSERIAL PRIMARY KEY,
    "EmployeeTrainingGuid"  UUID NOT NULL DEFAULT gen_random_uuid(),
    "EmployeeId"            BIGINT NOT NULL,
    "TrainingId"            BIGINT NOT NULL DEFAULT 0,
    "StartDate"             DATE,
    "EndDate"               DATE,
    "DurationHour"          NUMERIC(10,2),
    "Score"                 NUMERIC(10,2),
    "Grade"                 VARCHAR(20),
    "IsPassed"              BOOLEAN,
    "CertificateNumber"     VARCHAR(100),
    "FileStorageId"         BIGINT,
    "Remarks"               VARCHAR(500),
    "StatusId"              BIGINT NOT NULL DEFAULT 1,
    "CreatedById"           BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"           TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"           BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"           TIMESTAMPTZ,
    "DeletedById"           BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"           TIMESTAMPTZ,
    CONSTRAINT "UQ_EmployeeTraining_Guid" UNIQUE ("EmployeeTrainingGuid")
);

CREATE TABLE IF NOT EXISTS app."EmployeeCertification" (
    "EmployeeCertificationId"   BIGSERIAL PRIMARY KEY,
    "EmployeeCertificationGuid" UUID NOT NULL DEFAULT gen_random_uuid(),
    "EmployeeId"                BIGINT NOT NULL,
    "CertificationId"           BIGINT NOT NULL DEFAULT 0,
    "CertificateNumber"         VARCHAR(100),
    "IssuedDate"                DATE,
    "ExpiredDate"               DATE,
    "Issuedby"                  VARCHAR(200),
    "Score"                     NUMERIC(10,2),
    "FileStorageId"             BIGINT,
    "IsLifetime"                BOOLEAN NOT NULL DEFAULT FALSE,
    "IsCurrent"                 BOOLEAN NOT NULL DEFAULT TRUE,
    "Remarks"                   VARCHAR(500),
    "StatusId"                  BIGINT NOT NULL DEFAULT 1,
    "CreatedById"               BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"               TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"               BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"               TIMESTAMPTZ,
    "DeletedById"               BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"               TIMESTAMPTZ,
    CONSTRAINT "UQ_EmployeeCertification_Guid" UNIQUE ("EmployeeCertificationGuid")
);

CREATE TABLE IF NOT EXISTS app."EmployeeAsset" (
    "EmployeeAssetId"           BIGSERIAL PRIMARY KEY,
    "EmployeeAssetGuid"         UUID NOT NULL DEFAULT gen_random_uuid(),
    "EmployeeId"                BIGINT NOT NULL,
    "AssetId"                   BIGINT NOT NULL DEFAULT 0,
    "AssignedDate"              DATE,
    "ReturnedDate"              DATE,
    "ExpectedReturnDate"        DATE,
    "IsCurrent"                 BOOLEAN NOT NULL DEFAULT TRUE,
    "ConditionWhenAssigned"     VARCHAR(100),
    "ConditionWhenReturned"     VARCHAR(100),
    "Remarks"                   VARCHAR(500),
    "StatusId"                  BIGINT NOT NULL DEFAULT 1,
    "CreatedById"               BIGINT NOT NULL DEFAULT 0,
    "CreatedTime"               TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "UpdatedById"               BIGINT NOT NULL DEFAULT 0,
    "UpdatedTime"               TIMESTAMPTZ,
    "DeletedById"               BIGINT NOT NULL DEFAULT 0,
    "DeletedTime"               TIMESTAMPTZ,
    CONSTRAINT "UQ_EmployeeAsset_Guid" UNIQUE ("EmployeeAssetGuid")
);

-- Assignment / Career LEFT JOIN stubs (generator uses FK-column table names)
DO $$
DECLARE
    t text;
BEGIN
    FOREACH t IN ARRAY ARRAY[
        'CompanyOffice', 'CostCenter', 'OrganizationUnit',
        'JobGrade', 'JobLevel', 'JobTitle', 'Position', 'Shift',
        'PayrollGroup', 'WorkLocation', 'ManagerEmployee', 'SupervisorEmployee',
        'FromCompany', 'FromJobGrade', 'FromJobLevel', 'FromJobTitle',
        'FromManagerEmployee', 'FromOrganizationUnit', 'FromPosition',
        'ToCompany', 'ToJobGrade', 'ToJobLevel', 'ToJobTitle',
        'ToManagerEmployee', 'ToOrganizationUnit', 'ToPosition'
    ]
    LOOP
        EXECUTE format(
            'CREATE TABLE IF NOT EXISTS app.%I (
                %I BIGSERIAL PRIMARY KEY,
                %I VARCHAR(200),
                "StatusId" BIGINT NOT NULL DEFAULT 1
             )',
            t, t || 'Id', t || 'Name');
    END LOOP;
END $$;

INSERT INTO app."StandardReference"
    ("StandardReferenceGuid", "StandardReferenceName", "StandardReferenceInitial", "Description")
VALUES
    (gen_random_uuid(), 'Gender', 'Gender', 'M3 seed'),
    (gen_random_uuid(), 'Religion', 'Religion', 'M3 seed'),
    (gen_random_uuid(), 'Salutation', 'Salutation', 'M3 seed'),
    (gen_random_uuid(), 'Blood Type', 'BloodType', 'M3 seed'),
    (gen_random_uuid(), 'Marital Status', 'MaritalStatus', 'M3 seed'),
    (gen_random_uuid(), 'Employment Status', 'EmploymentStatus', 'M3 seed'),
    (gen_random_uuid(), 'Employment Type', 'EmploymentType', 'M3 seed'),
    (gen_random_uuid(), 'Remuneration Type', 'RemunerationType', 'M3 seed'),
    (gen_random_uuid(), 'Address Type', 'AddressType', 'M3 seed'),
    (gen_random_uuid(), 'Contact Type', 'ContactType', 'M3 seed'),
    (gen_random_uuid(), 'Language', 'Language', 'M3 seed'),
    (gen_random_uuid(), 'Language Level', 'LanguageLevel', 'M3 seed'),
    (gen_random_uuid(), 'Contract Status', 'ContractStatus', 'M3 seed'),
    (gen_random_uuid(), 'Contract Type', 'ContractType', 'M3 seed')
ON CONFLICT ("StandardReferenceInitial") DO NOTHING;

INSERT INTO app."StandardReferenceItem"
    ("StandardReferenceItemGuid", "StandardReferenceId", "StandardReferenceGuid",
     "StandardReferenceItemInitial", "StandardReferenceItemName", "StandardReferenceItemValue", "DisplayOrder")
SELECT gen_random_uuid(), sr."StandardReferenceId", sr."StandardReferenceGuid",
       v.initial, v.name, v.value, v.ord
FROM app."StandardReference" sr
CROSS JOIN (VALUES
    ('Gender', 'M', 'Male', 'Male', 1),
    ('Religion', 'IS', 'Islam', 'Islam', 1),
    ('EmploymentStatus', 'ACT', 'Active', 'Active', 1),
    ('EmploymentType', 'FT', 'Full Time', 'Full Time', 1),
    ('RemunerationType', 'SAL', 'Salary', 'Salary', 1),
    ('AddressType', 'HOME', 'Home', 'Home', 1),
    ('Language', 'ID', 'Indonesian', 'Indonesian', 1),
    ('LanguageLevel', 'GOOD', 'Good', 'Good', 1),
    ('ContractStatus', 'CUR', 'Current', 'Current', 1),
    ('ContractType', 'PKWTT', 'Permanent', 'Permanent', 1)
) AS v(ref_initial, initial, name, value, ord)
WHERE sr."StandardReferenceInitial" = v.ref_initial
ON CONFLICT ("StandardReferenceId", "StandardReferenceItemInitial") DO NOTHING;

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
    "PersonId"      BIGINT,
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

CREATE UNIQUE INDEX IF NOT EXISTS "UQ_Users_PersonId_Active" ON core."Users" ("PersonId") WHERE "PersonId" IS NOT NULL AND "DeletedTime" IS NULL AND "StatusId" > 0;

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
    "PersonId"              BIGINT,
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
