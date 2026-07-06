USE [Dev.BamboeHR]
GO

/****** Object:  Table [app].[Parameter] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[Parameter]
(
	[ParameterId] [bigint] IDENTITY(1,1) NOT NULL,
	[ParameterGuid] [uniqueidentifier] NOT NULL,
	[ParameterName] [varchar](50) NOT NULL,
	[ParameterValue] [varchar](100) NOT NULL,
	[ParameterType] [varchar](10) NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2] NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2] NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2] NULL
CONSTRAINT [PK_Parameter] PRIMARY KEY CLUSTERED 
(
    [ParameterId] ASC
)WITH (
    PAD_INDEX = OFF, 
    STATISTICS_NORECOMPUTE = OFF, 
    IGNORE_DUP_KEY = OFF, 
    ALLOW_ROW_LOCKS = ON, 
    ALLOW_PAGE_LOCKS = ON, 
    OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
) ON [PRIMARY],
CONSTRAINT [UQ_Parameter_ParameterName] UNIQUE NONCLUSTERED 
(
	[ParameterName] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [app].[Parameter] ADD CONSTRAINT [DF_Parameter_ParameterGuid] DEFAULT (newid()) FOR [ParameterGuid]
GO
ALTER TABLE [app].[Parameter] ADD CONSTRAINT [DF_Parameter_StatusId] DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [app].[Parameter] ADD CONSTRAINT [DF_Parameter_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO

-- 100% Performance Indexes
CREATE NONCLUSTERED INDEX [IX_Parameter_Name_Status] ON [app].[Parameter]
(
	[ParameterName] ASC,
	[StatusId] ASC
)
INCLUDE([ParameterGuid],[ParameterValue],[ParameterType]) 
WHERE ([DeletedTime] IS NULL)
GO
