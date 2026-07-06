USE [Dev.BamboeHR]
GO

-- Create schema apv if not exists
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'apv')
BEGIN
    EXEC('CREATE SCHEMA [apv]')
END
GO

/****** Object: Table [apv].[ApprovalTemplate] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [apv].[ApprovalTemplate](
    [ApprovalTemplateId]   [bigint] IDENTITY(1,1) NOT NULL,
    [ApprovalTemplateGuid] [uniqueidentifier] NOT NULL,
    [TemplateName]         [nvarchar](200) NOT NULL,
    [ModuleCode]           [nvarchar](100) NOT NULL,   -- e.g. 'LEAVE','OVERTIME','PURCHASE'
    [Description]          [nvarchar](500) NULL,
    [IsActive]             [bit] NOT NULL,
    [StatusId]             [int] NOT NULL,
    [RowVersion]           [timestamp] NOT NULL,
    [CreatedById]          [bigint] NOT NULL,
    [CreatedTime]          [datetime2] NOT NULL,
    [UpdatedById]          [bigint] NULL,
    [UpdatedTime]          [datetime2] NULL,
    [DeletedById]          [bigint] NULL,
    [DeletedTime]          [datetime2] NULL,
    CONSTRAINT [PK_ApprovalTemplate] PRIMARY KEY CLUSTERED ([ApprovalTemplateId] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [apv].[ApprovalTemplate] ADD CONSTRAINT [DF_ApprovalTemplate_Guid]        DEFAULT (newid())      FOR [ApprovalTemplateGuid]
ALTER TABLE [apv].[ApprovalTemplate] ADD CONSTRAINT [DF_ApprovalTemplate_IsActive]    DEFAULT ((1))          FOR [IsActive]
ALTER TABLE [apv].[ApprovalTemplate] ADD CONSTRAINT [DF_ApprovalTemplate_StatusId]    DEFAULT ((1))          FOR [StatusId]
ALTER TABLE [apv].[ApprovalTemplate] ADD CONSTRAINT [DF_ApprovalTemplate_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
