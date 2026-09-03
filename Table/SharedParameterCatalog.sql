USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   Shared Parameter Catalog (library + profiles)
   Source of truth for Report / Process / Pivot form fields
   ========================================================= */

IF OBJECT_ID(N'[core].[SharedParameterProfileField]', N'U') IS NOT NULL
    DROP TABLE [core].[SharedParameterProfileField];
GO

IF OBJECT_ID(N'[core].[SharedParameterProfile]', N'U') IS NOT NULL
    DROP TABLE [core].[SharedParameterProfile];
GO

IF OBJECT_ID(N'[core].[SharedParameterCatalog]', N'U') IS NOT NULL
    DROP TABLE [core].[SharedParameterCatalog];
GO

CREATE TABLE [core].[SharedParameterCatalog]
(
    [SharedParameterCatalogId]   BIGINT           IDENTITY(1,1) NOT NULL,
    [SharedParameterCatalogGuid] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_SharedParameterCatalog_Guid] DEFAULT (NEWID()),
    [Code]                       VARCHAR(50)      NOT NULL,
    [Label]                      NVARCHAR(200)    NOT NULL,
    [DataType]                   VARCHAR(30)      NOT NULL,
    [CanonicalControlType]       VARCHAR(30)      NOT NULL,
    [ProcessControlType]         VARCHAR(30)      NOT NULL,
    [ReportControlType]          VARCHAR(30)      NOT NULL,
    [IsRequired]                 BIT              NOT NULL CONSTRAINT [DF_SharedParameterCatalog_IsRequired] DEFAULT (0),
    [DefaultValue]               NVARCHAR(500)    NULL,
    [LookupSource]               NVARCHAR(200)    NULL,
    [GroupName]                  NVARCHAR(100)    NULL,
    [SortOrder]                  INT              NOT NULL CONSTRAINT [DF_SharedParameterCatalog_SortOrder] DEFAULT (0),
    [IsSystemField]              BIT              NOT NULL CONSTRAINT [DF_SharedParameterCatalog_IsSystem] DEFAULT (1),
    [StatusId]                   INT              NOT NULL CONSTRAINT [DF_SharedParameterCatalog_StatusId] DEFAULT (1),
    [CreatedById]                BIGINT           NOT NULL,
    [CreatedTime]                DATETIME2        NOT NULL CONSTRAINT [DF_SharedParameterCatalog_CreatedTime] DEFAULT (SYSDATETIME()),
    [UpdatedById]                BIGINT           NULL,
    [UpdatedTime]                DATETIME2        NULL,
    [DeletedById]                BIGINT           NULL,
    [DeletedTime]                DATETIME2        NULL,
    CONSTRAINT [PK_SharedParameterCatalog] PRIMARY KEY CLUSTERED ([SharedParameterCatalogId] ASC),
    CONSTRAINT [UQ_SharedParameterCatalog_Code] UNIQUE NONCLUSTERED ([Code] ASC)
) ON [PRIMARY];
GO

CREATE NONCLUSTERED INDEX [IX_SharedParameterCatalog_Active]
ON [core].[SharedParameterCatalog] ([StatusId], [SortOrder], [Code])
WHERE ([DeletedTime] IS NULL);
GO

CREATE TABLE [core].[SharedParameterProfile]
(
    [SharedParameterProfileId]   BIGINT           IDENTITY(1,1) NOT NULL,
    [SharedParameterProfileGuid] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_SharedParameterProfile_Guid] DEFAULT (NEWID()),
    [ProfileToken]               VARCHAR(30)      NOT NULL,
    [ProfileLabel]               NVARCHAR(200)    NOT NULL,
    [StatusId]                   INT              NOT NULL CONSTRAINT [DF_SharedParameterProfile_StatusId] DEFAULT (1),
    [CreatedById]                BIGINT           NOT NULL,
    [CreatedTime]                DATETIME2        NOT NULL CONSTRAINT [DF_SharedParameterProfile_CreatedTime] DEFAULT (SYSDATETIME()),
    [UpdatedById]                BIGINT           NULL,
    [UpdatedTime]                DATETIME2        NULL,
    [DeletedById]                BIGINT           NULL,
    [DeletedTime]                DATETIME2        NULL,
    CONSTRAINT [PK_SharedParameterProfile] PRIMARY KEY CLUSTERED ([SharedParameterProfileId] ASC),
    CONSTRAINT [UQ_SharedParameterProfile_Token] UNIQUE NONCLUSTERED ([ProfileToken] ASC)
) ON [PRIMARY];
GO

CREATE TABLE [core].[SharedParameterProfileField]
(
    [SharedParameterProfileFieldId]   BIGINT           IDENTITY(1,1) NOT NULL,
    [SharedParameterProfileFieldGuid] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_SharedParameterProfileField_Guid] DEFAULT (NEWID()),
    [SharedParameterProfileId]        BIGINT           NOT NULL,
    [SharedParameterCatalogId]        BIGINT           NOT NULL,
    [Audience]                        VARCHAR(20)      NOT NULL,
    [SortOrder]                       INT              NOT NULL CONSTRAINT [DF_SharedParameterProfileField_SortOrder] DEFAULT (0),
    [OverrideLabel]                   NVARCHAR(200)    NULL,
    [OverrideDataType]                VARCHAR(30)      NULL,
    [OverrideControlType]             VARCHAR(30)      NULL,
    [OverrideIsRequired]              BIT              NULL,
    [OverrideLookupSource]            NVARCHAR(200)    NULL,
    [StatusId]                        INT              NOT NULL CONSTRAINT [DF_SharedParameterProfileField_StatusId] DEFAULT (1),
    [CreatedById]                     BIGINT           NOT NULL,
    [CreatedTime]                     DATETIME2        NOT NULL CONSTRAINT [DF_SharedParameterProfileField_CreatedTime] DEFAULT (SYSDATETIME()),
    [UpdatedById]                     BIGINT           NULL,
    [UpdatedTime]                     DATETIME2        NULL,
    [DeletedById]                     BIGINT           NULL,
    [DeletedTime]                     DATETIME2        NULL,
    CONSTRAINT [PK_SharedParameterProfileField] PRIMARY KEY CLUSTERED ([SharedParameterProfileFieldId] ASC),
    CONSTRAINT [FK_SharedParameterProfileField_Profile]
        FOREIGN KEY ([SharedParameterProfileId]) REFERENCES [core].[SharedParameterProfile] ([SharedParameterProfileId]),
    CONSTRAINT [FK_SharedParameterProfileField_Catalog]
        FOREIGN KEY ([SharedParameterCatalogId]) REFERENCES [core].[SharedParameterCatalog] ([SharedParameterCatalogId]),
    CONSTRAINT [UQ_SharedParameterProfileField_Member]
        UNIQUE NONCLUSTERED ([SharedParameterProfileId], [SharedParameterCatalogId], [Audience])
) ON [PRIMARY];
GO

PRINT 'SharedParameterCatalog tables created.';
GO
