USE [Dev.BamboeHR]
GO

/****** Object:  Table [app].[ParameterScope] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[ParameterScope]
(
	[ParameterScopeId] [bigint] IDENTITY(1,1) NOT NULL,
	[ParameterScopeGuid] [uniqueidentifier] NOT NULL,
	[ParameterId] [bigint] NOT NULL,
	[ParameterGuid] [uniqueidentifier] NOT NULL,
	[CompanyId] [bigint] NULL,
	[CompanyGuid] [uniqueidentifier] NULL,
	[CompanyOfficeId] [bigint] NULL,
	[CompanyOfficeGuid] [uniqueidentifier] NULL,
	[OverrideValue] [varchar](100) NOT NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2] NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2] NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2] NULL
CONSTRAINT [PK_ParameterScope] PRIMARY KEY CLUSTERED 
(
    [ParameterScopeId] ASC
)WITH (
    PAD_INDEX = OFF, 
    STATISTICS_NORECOMPUTE = OFF, 
    IGNORE_DUP_KEY = OFF, 
    ALLOW_ROW_LOCKS = ON, 
    ALLOW_PAGE_LOCKS = ON, 
    OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [app].[ParameterScope] ADD  CONSTRAINT [DF_ParameterScope_ParameterScopeGuid]  DEFAULT (newid()) FOR [ParameterScopeGuid]
GO

ALTER TABLE [app].[ParameterScope] ADD  CONSTRAINT [DF_ParameterScope_StatusId]  DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [app].[ParameterScope] ADD  CONSTRAINT [DF_ParameterScope_CreatedTime]  DEFAULT (sysdatetime()) FOR [CreatedTime]
GO

ALTER TABLE [app].[ParameterScope]  WITH CHECK ADD  CONSTRAINT [FK_ParameterScope_Parameter] FOREIGN KEY([ParameterId])
REFERENCES [app].[Parameter] ([ParameterId])
GO

ALTER TABLE [app].[ParameterScope] CHECK CONSTRAINT [FK_ParameterScope_Parameter]
GO

-- 100% Performance Indexes
-- Index 1: Filtered Unique Index untuk CompanyOffice overrides
CREATE UNIQUE NONCLUSTERED INDEX [UQ_ParameterScope_Office] ON [app].[ParameterScope]
(
	[ParameterId] ASC,
	[CompanyId] ASC,
	[CompanyOfficeId] ASC
)
WHERE ([CompanyOfficeId] IS NOT NULL AND [DeletedTime] IS NULL AND [StatusId] > 0)
GO

-- Index 2: Filtered Unique Index untuk Company overrides (Semua office)
CREATE UNIQUE NONCLUSTERED INDEX [UQ_ParameterScope_Company] ON [app].[ParameterScope]
(
	[ParameterId] ASC,
	[CompanyId] ASC
)
WHERE ([CompanyOfficeId] IS NULL AND [DeletedTime] IS NULL AND [StatusId] > 0)
GO

-- Index 3: Index for fast lookup based on ParameterId (often used to get all scopes for a parameter)
CREATE NONCLUSTERED INDEX [IX_ParameterScope_ParameterId_Status] ON [app].[ParameterScope]
(
	[ParameterId] ASC,
	[StatusId] ASC
)
INCLUDE([CompanyId],[CompanyOfficeId],[OverrideValue])
WHERE ([DeletedTime] IS NULL)
GO
