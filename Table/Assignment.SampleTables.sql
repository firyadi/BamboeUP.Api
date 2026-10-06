USE [Dev.BamboeHR]
GO

/****** Object:  UI-06 TemplateUiAssignment sample tables (BamboeUp standard) ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/* Drop dependents first */
IF OBJECT_ID(N'[app].[WorkTeamCapability]', N'U') IS NOT NULL
    DROP TABLE [app].[WorkTeamCapability]
GO

IF OBJECT_ID(N'[app].[WorkTeam]', N'U') IS NOT NULL
    DROP TABLE [app].[WorkTeam]
GO

IF OBJECT_ID(N'[app].[WorkCapability]', N'U') IS NOT NULL
    DROP TABLE [app].[WorkCapability]
GO

/****** Object:  Table [app].[WorkCapability] — catalog for the dual-list ******/
CREATE TABLE [app].[WorkCapability](
	[WorkCapabilityId] [bigint] IDENTITY(1,1) NOT NULL,
	[WorkCapabilityGuid] [uniqueidentifier] NOT NULL,
	[CapabilityCode] [varchar](20) NOT NULL,
	[CapabilityName] [nvarchar](200) NOT NULL,
	[CapabilityDescription] [nvarchar](500) NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2](7) NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2](7) NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2](7) NULL,
 CONSTRAINT [PK_WorkCapability] PRIMARY KEY CLUSTERED
(
	[WorkCapabilityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_WorkCapability_WorkCapabilityGuid] UNIQUE NONCLUSTERED
(
	[WorkCapabilityGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_WorkCapability_Code] UNIQUE NONCLUSTERED
(
	[CapabilityCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [app].[WorkCapability] ADD CONSTRAINT [DF_WorkCapability_WorkCapabilityGuid] DEFAULT (newsequentialid()) FOR [WorkCapabilityGuid]
GO
ALTER TABLE [app].[WorkCapability] ADD CONSTRAINT [DF_WorkCapability_StatusId] DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [app].[WorkCapability] ADD CONSTRAINT [DF_WorkCapability_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
ALTER TABLE [app].[WorkCapability] WITH CHECK ADD CONSTRAINT [CHK_WorkCapability_Status] CHECK (([StatusId]=(2) OR [StatusId]=(1) OR [StatusId]=(0)))
GO

/****** Object:  Table [app].[WorkTeam] — assignment header ******/
CREATE TABLE [app].[WorkTeam](
	[WorkTeamId] [bigint] IDENTITY(1,1) NOT NULL,
	[WorkTeamGuid] [uniqueidentifier] NOT NULL,
	[WorkTeamCode] [varchar](20) NOT NULL,
	[WorkTeamName] [nvarchar](200) NOT NULL,
	[IsEditAble] [bit] NOT NULL,
	[Note] [nvarchar](500) NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2](7) NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2](7) NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2](7) NULL,
 CONSTRAINT [PK_WorkTeam] PRIMARY KEY CLUSTERED
(
	[WorkTeamId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_WorkTeam_WorkTeamGuid] UNIQUE NONCLUSTERED
(
	[WorkTeamGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_WorkTeam_Code] UNIQUE NONCLUSTERED
(
	[WorkTeamCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [app].[WorkTeam] ADD CONSTRAINT [DF_WorkTeam_WorkTeamGuid] DEFAULT (newsequentialid()) FOR [WorkTeamGuid]
GO
ALTER TABLE [app].[WorkTeam] ADD CONSTRAINT [DF_WorkTeam_IsEditAble] DEFAULT ((1)) FOR [IsEditAble]
GO
ALTER TABLE [app].[WorkTeam] ADD CONSTRAINT [DF_WorkTeam_StatusId] DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [app].[WorkTeam] ADD CONSTRAINT [DF_WorkTeam_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
ALTER TABLE [app].[WorkTeam] WITH CHECK ADD CONSTRAINT [CHK_WorkTeam_Status] CHECK (([StatusId]=(2) OR [StatusId]=(1) OR [StatusId]=(0)))
GO

/****** Object:  Table [app].[WorkTeamCapability] — junction + permission flags ******/
CREATE TABLE [app].[WorkTeamCapability](
	[WorkTeamCapabilityId] [bigint] IDENTITY(1,1) NOT NULL,
	[WorkTeamCapabilityGuid] [uniqueidentifier] NOT NULL,
	[WorkTeamId] [bigint] NOT NULL,
	[WorkCapabilityId] [bigint] NOT NULL,
	[IsViewAble] [bit] NOT NULL,
	[IsAddAble] [bit] NULL,
	[IsEditAble] [bit] NULL,
	[IsApproveAble] [bit] NULL,
	[IsExportAble] [bit] NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2](7) NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2](7) NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2](7) NULL,
 CONSTRAINT [PK_WorkTeamCapability] PRIMARY KEY CLUSTERED
(
	[WorkTeamCapabilityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_WorkTeamCapability_WorkTeamCapabilityGuid] UNIQUE NONCLUSTERED
(
	[WorkTeamCapabilityGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [app].[WorkTeamCapability] ADD CONSTRAINT [DF_WorkTeamCapability_WorkTeamCapabilityGuid] DEFAULT (newsequentialid()) FOR [WorkTeamCapabilityGuid]
GO
ALTER TABLE [app].[WorkTeamCapability] ADD CONSTRAINT [DF_WorkTeamCapability_IsViewAble] DEFAULT ((1)) FOR [IsViewAble]
GO
ALTER TABLE [app].[WorkTeamCapability] ADD CONSTRAINT [DF_WorkTeamCapability_StatusId] DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [app].[WorkTeamCapability] ADD CONSTRAINT [DF_WorkTeamCapability_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
ALTER TABLE [app].[WorkTeamCapability] WITH CHECK ADD CONSTRAINT [CHK_WorkTeamCapability_Status] CHECK (([StatusId]=(2) OR [StatusId]=(1) OR [StatusId]=(0)))
GO

CREATE NONCLUSTERED INDEX [IX_WorkTeamCapability_WorkTeamId]
ON [app].[WorkTeamCapability]
(
	[WorkTeamId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_WorkTeamCapability_WorkCapabilityId]
ON [app].[WorkTeamCapability]
(
	[WorkCapabilityId] ASC
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_WorkTeamCapability_Team_Capability_Active]
ON [app].[WorkTeamCapability]
(
	[WorkTeamId] ASC,
	[WorkCapabilityId] ASC
)
WHERE ([DeletedTime] IS NULL AND [StatusId]>(0))
GO
