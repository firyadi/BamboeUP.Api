USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

PRINT '=== ALTER [core].[ReportParameter] — Phase 3 layout & control metadata ==='

IF NOT EXISTS (SELECT 1 FROM sys.columns
               WHERE object_id = OBJECT_ID(N'[core].[ReportParameter]') AND name = N'FieldKey')
BEGIN
    ALTER TABLE [core].[ReportParameter] ADD
        [FieldKey]       NVARCHAR(50)  NULL,
        [ControlType]    NVARCHAR(30)  NOT NULL CONSTRAINT [DF_ReportParameter_ControlType] DEFAULT (N'TextBox'),
        [ColumnGroup]    TINYINT       NOT NULL CONSTRAINT [DF_ReportParameter_ColumnGroup] DEFAULT (1),
        [ColumnSpan]     TINYINT       NOT NULL CONSTRAINT [DF_ReportParameter_ColumnSpan] DEFAULT (12),
        [RowGroup]       NVARCHAR(30)  NULL,
        [VisibleWhen]    NVARCHAR(200) NULL,
        [CatalogFieldId] BIGINT        NULL;

    PRINT '  -> Added FieldKey, ControlType, ColumnGroup, ColumnSpan, RowGroup, VisibleWhen, CatalogFieldId.'
END
ELSE
BEGIN
    PRINT '  -> Phase 3 columns already exist, skipped.'
END
GO

IF OBJECT_ID(N'[core].[ReportParameterCatalog]', N'U') IS NULL
BEGIN
    CREATE TABLE [core].[ReportParameterCatalog](
        [CatalogFieldId]     BIGINT        IDENTITY(1,1) NOT NULL,
        [FieldKey]           NVARCHAR(50)  NOT NULL,
        [DefaultLabel]       NVARCHAR(200) NOT NULL,
        [ControlType]        NVARCHAR(30)  NOT NULL,
        [DataType]           NVARCHAR(30)  NOT NULL,
        [LookupType]         NVARCHAR(50)  NULL,
        [DefaultColumnGroup] TINYINT       NOT NULL CONSTRAINT [DF_Catalog_ColumnGroup] DEFAULT (3),
        [IsSystemField]      BIT           NOT NULL CONSTRAINT [DF_Catalog_IsSystem] DEFAULT (0),
        [StatusId]           INT           NOT NULL CONSTRAINT [DF_Catalog_StatusId] DEFAULT (1),
        CONSTRAINT [PK_ReportParameterCatalog] PRIMARY KEY CLUSTERED ([CatalogFieldId] ASC),
        CONSTRAINT [UQ_ReportParameterCatalog_FieldKey] UNIQUE NONCLUSTERED ([FieldKey] ASC)
    ) ON [PRIMARY];

    PRINT '  -> Created [core].[ReportParameterCatalog].'
END
ELSE
BEGIN
    PRINT '  -> [core].[ReportParameterCatalog] already exists, skipped.'
END
GO

PRINT '=== ReportParameter Phase 3 schema ready ==='
GO
