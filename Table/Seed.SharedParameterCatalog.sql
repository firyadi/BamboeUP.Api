USE [Dev.BamboeHR]
GO

SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* =========================================================
   Seed Shared Parameter Catalog (idempotent by Code / Token)
   Mirrors BuiltIn library + ApplyProfile memberships
   ========================================================= */

DECLARE @CreatedById BIGINT = 1;

/* ---- Library fields ---- */
;WITH Src AS (
    SELECT * FROM (VALUES
        (N'CompanyId',       N'Company',       N'bigint',   N'ReadonlyText', N'ReadonlyText', N'ReadonlyText', 1, NULL,      NULL,         N'Scope',   10, 1),
        (N'CompanyOfficeId', N'Office',        N'bigint',   N'ReadonlyText', N'ReadonlyText', N'ReadonlyText', 0, NULL,      NULL,         N'Scope',   20, 1),
        (N'EmployeeId',      N'Employee',      N'bigint',   N'ComboBox',     N'Lookup',       N'ComboBox',     0, NULL,      N'Employee',  N'Scope',   25, 1),
        (N'PeriodYear',      N'Period Year',   N'int',      N'NumberBox',    N'Number',       N'NumberBox',    1, NULL,      NULL,         N'Period',  30, 1),
        (N'PeriodMonth',     N'Period Month',  N'int',      N'NumberBox',    N'Number',       N'NumberBox',    1, NULL,      NULL,         N'Period',  40, 1),
        (N'DateFrom',        N'Valid From',    N'datetime', N'DatePicker',   N'Date',         N'DatePicker',   1, NULL,      NULL,         N'Period',  30, 1),
        (N'DateTo',          N'Valid To',      N'datetime', N'DatePicker',   N'Date',         N'DatePicker',   1, NULL,      NULL,         N'Period',  40, 1),
        (N'ImportType',      N'Import Type',   N'string',   N'ComboBox',     N'Dropdown',     N'ComboBox',     1, NULL,      N'ImportType',N'Input',   45, 1),
        (N'SourceFile',      N'Source File',   N'string',   N'FileUpload',   N'FileUpload',   N'FileUpload',   1, NULL,      NULL,         N'Input',   50, 1),
        (N'ValidateOnly',    N'Validate Only', N'boolean',  N'CheckBox',     N'Boolean',      N'CheckBox',     0, N'false',  NULL,         N'Options', 90, 1)
    ) AS v(Code, Label, DataType, CanonicalControlType, ProcessControlType, ReportControlType, IsRequired, DefaultValue, LookupSource, GroupName, SortOrder, IsSystemField)
)
MERGE [core].[SharedParameterCatalog] AS t
USING Src AS s ON t.[Code] = s.Code
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([SharedParameterCatalogGuid], [Code], [Label], [DataType], [CanonicalControlType], [ProcessControlType], [ReportControlType],
            [IsRequired], [DefaultValue], [LookupSource], [GroupName], [SortOrder], [IsSystemField], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), s.Code, s.Label, s.DataType, s.CanonicalControlType, s.ProcessControlType, s.ReportControlType,
            s.IsRequired, s.DefaultValue, s.LookupSource, s.GroupName, s.SortOrder, s.IsSystemField, 1, @CreatedById, SYSDATETIME())
WHEN MATCHED AND t.[DeletedTime] IS NULL THEN
    UPDATE SET
        t.[Label] = s.Label,
        t.[DataType] = s.DataType,
        t.[CanonicalControlType] = s.CanonicalControlType,
        t.[ProcessControlType] = s.ProcessControlType,
        t.[ReportControlType] = s.ReportControlType,
        t.[IsRequired] = s.IsRequired,
        t.[DefaultValue] = s.DefaultValue,
        t.[LookupSource] = s.LookupSource,
        t.[GroupName] = s.GroupName,
        t.[SortOrder] = s.SortOrder,
        t.[IsSystemField] = s.IsSystemField,
        t.[StatusId] = 1,
        t.[UpdatedById] = @CreatedById,
        t.[UpdatedTime] = SYSDATETIME();

/* ---- Profiles ---- */
;WITH Prof AS (
    SELECT * FROM (VALUES
        (N'none',     N'None — no parameters'),
        (N'scope',    N'Scope — Company + Office'),
        (N'period',   N'Period — Scope + Year/Month (Process) / Date range (Report)'),
        (N'standard', N'Standard — same as Period'),
        (N'process',  N'Process — Period + ValidateOnly')
    ) AS v(ProfileToken, ProfileLabel)
)
MERGE [core].[SharedParameterProfile] AS t
USING Prof AS s ON t.[ProfileToken] = s.ProfileToken
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([SharedParameterProfileGuid], [ProfileToken], [ProfileLabel], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), s.ProfileToken, s.ProfileLabel, 1, @CreatedById, SYSDATETIME())
WHEN MATCHED AND t.[DeletedTime] IS NULL THEN
    UPDATE SET
        t.[ProfileLabel] = s.ProfileLabel,
        t.[StatusId] = 1,
        t.[UpdatedById] = @CreatedById,
        t.[UpdatedTime] = SYSDATETIME();

/* ---- Profile fields (membership + Report overrides) ----
   Audience: Process | Report | Pivot
   Override* only where Report differs from library (Company/Office)
*/
DECLARE @Members TABLE (
    ProfileToken VARCHAR(30),
    Code VARCHAR(50),
    Audience VARCHAR(20),
    SortOrder INT,
    OverrideLabel NVARCHAR(200) NULL,
    OverrideDataType VARCHAR(30) NULL,
    OverrideControlType VARCHAR(30) NULL,
    OverrideIsRequired BIT NULL,
    OverrideLookupSource NVARCHAR(200) NULL
);

INSERT INTO @Members (ProfileToken, Code, Audience, SortOrder, OverrideLabel, OverrideDataType, OverrideControlType, OverrideIsRequired, OverrideLookupSource)
VALUES
-- scope / Process
(N'scope', N'CompanyId', N'Process', 10, NULL, NULL, NULL, NULL, NULL),
(N'scope', N'CompanyOfficeId', N'Process', 20, NULL, NULL, NULL, NULL, NULL),
-- scope / Report (overrides)
(N'scope', N'CompanyId', N'Report', 10, N'Company', N'string', N'Lookup', 1, N'Company'),
(N'scope', N'CompanyOfficeId', N'Report', 20, N'Office', N'string', N'Lookup', 1, N'Office'),
-- period+standard / Process
(N'period', N'CompanyId', N'Process', 10, NULL, NULL, NULL, NULL, NULL),
(N'period', N'PeriodYear', N'Process', 30, NULL, NULL, NULL, NULL, NULL),
(N'period', N'PeriodMonth', N'Process', 40, NULL, NULL, NULL, NULL, NULL),
(N'standard', N'CompanyId', N'Process', 10, NULL, NULL, NULL, NULL, NULL),
(N'standard', N'PeriodYear', N'Process', 30, NULL, NULL, NULL, NULL, NULL),
(N'standard', N'PeriodMonth', N'Process', 40, NULL, NULL, NULL, NULL, NULL),
-- period+standard / Report
(N'period', N'CompanyId', N'Report', 10, N'Company', N'string', N'Lookup', 1, N'Company'),
(N'period', N'CompanyOfficeId', N'Report', 20, N'Office', N'string', N'Lookup', 1, N'Office'),
(N'period', N'DateFrom', N'Report', 30, NULL, NULL, NULL, NULL, NULL),
(N'period', N'DateTo', N'Report', 40, NULL, NULL, NULL, NULL, NULL),
(N'standard', N'CompanyId', N'Report', 10, N'Company', N'string', N'Lookup', 1, N'Company'),
(N'standard', N'CompanyOfficeId', N'Report', 20, N'Office', N'string', N'Lookup', 1, N'Office'),
(N'standard', N'DateFrom', N'Report', 30, NULL, NULL, NULL, NULL, NULL),
(N'standard', N'DateTo', N'Report', 40, NULL, NULL, NULL, NULL, NULL),
-- process / Process
(N'process', N'CompanyId', N'Process', 10, NULL, NULL, NULL, NULL, NULL),
(N'process', N'PeriodYear', N'Process', 30, NULL, NULL, NULL, NULL, NULL),
(N'process', N'PeriodMonth', N'Process', 40, NULL, NULL, NULL, NULL, NULL),
(N'process', N'ValidateOnly', N'Process', 90, NULL, NULL, NULL, NULL, NULL),
-- Pivot mirrors Process for scope/period/process (minimal)
(N'scope', N'CompanyId', N'Pivot', 10, NULL, NULL, NULL, NULL, NULL),
(N'scope', N'CompanyOfficeId', N'Pivot', 20, NULL, NULL, NULL, NULL, NULL),
(N'period', N'CompanyId', N'Pivot', 10, NULL, NULL, NULL, NULL, NULL),
(N'period', N'PeriodYear', N'Pivot', 30, NULL, NULL, NULL, NULL, NULL),
(N'period', N'PeriodMonth', N'Pivot', 40, NULL, NULL, NULL, NULL, NULL),
(N'standard', N'CompanyId', N'Pivot', 10, NULL, NULL, NULL, NULL, NULL),
(N'standard', N'PeriodYear', N'Pivot', 30, NULL, NULL, NULL, NULL, NULL),
(N'standard', N'PeriodMonth', N'Pivot', 40, NULL, NULL, NULL, NULL, NULL),
(N'process', N'CompanyId', N'Pivot', 10, NULL, NULL, NULL, NULL, NULL),
(N'process', N'PeriodYear', N'Pivot', 30, NULL, NULL, NULL, NULL, NULL),
(N'process', N'PeriodMonth', N'Pivot', 40, NULL, NULL, NULL, NULL, NULL),
(N'process', N'ValidateOnly', N'Pivot', 90, NULL, NULL, NULL, NULL, NULL);

MERGE [core].[SharedParameterProfileField] AS t
USING (
    SELECT
        p.SharedParameterProfileId,
        c.SharedParameterCatalogId,
        m.Audience,
        m.SortOrder,
        m.OverrideLabel,
        m.OverrideDataType,
        m.OverrideControlType,
        m.OverrideIsRequired,
        m.OverrideLookupSource
    FROM @Members m
    INNER JOIN [core].[SharedParameterProfile] p ON p.ProfileToken = m.ProfileToken AND p.DeletedTime IS NULL
    INNER JOIN [core].[SharedParameterCatalog] c ON c.Code = m.Code AND c.DeletedTime IS NULL
) AS s
ON t.SharedParameterProfileId = s.SharedParameterProfileId
   AND t.SharedParameterCatalogId = s.SharedParameterCatalogId
   AND t.Audience = s.Audience
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([SharedParameterProfileFieldGuid], [SharedParameterProfileId], [SharedParameterCatalogId], [Audience], [SortOrder],
            [OverrideLabel], [OverrideDataType], [OverrideControlType], [OverrideIsRequired], [OverrideLookupSource],
            [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), s.SharedParameterProfileId, s.SharedParameterCatalogId, s.Audience, s.SortOrder,
            s.OverrideLabel, s.OverrideDataType, s.OverrideControlType, s.OverrideIsRequired, s.OverrideLookupSource,
            1, @CreatedById, SYSDATETIME())
WHEN MATCHED AND t.[DeletedTime] IS NULL THEN
    UPDATE SET
        t.[SortOrder] = s.SortOrder,
        t.[OverrideLabel] = s.OverrideLabel,
        t.[OverrideDataType] = s.OverrideDataType,
        t.[OverrideControlType] = s.OverrideControlType,
        t.[OverrideIsRequired] = s.OverrideIsRequired,
        t.[OverrideLookupSource] = s.OverrideLookupSource,
        t.[StatusId] = 1,
        t.[UpdatedById] = @CreatedById,
        t.[UpdatedTime] = SYSDATETIME();

PRINT 'Seed.SharedParameterCatalog done.';
GO
