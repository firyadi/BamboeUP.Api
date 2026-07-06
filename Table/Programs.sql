USE [Dev.BamboeHR]
GO

/****** Object:  Table [core].[Program] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [core].[Programs](
	[ProgramsId] [bigint] IDENTITY(1,1) NOT NULL,
	[ProgramGuid] [uniqueidentifier] NOT NULL,
	[ProgramCode] [nvarchar](30) NOT NULL,
	[ParentId] [bigint] NULL,
	[IconCode] [nvarchar](10) NULL,
	[ProgramName] [nvarchar](100) NOT NULL,
	[TopLevelProgramId] [bigint] NULL,
	[RootLevel] [tinyint] NOT NULL,
	[RowIndex] [tinyint] NOT NULL,
	[Note] [nvarchar](1000) NULL,
	[IsParentProgram] [bit] NULL,
	[IsProgram] [bit] NULL,
	[IsBeginGroup] [bit] NULL,
	[ProgramType] [nvarchar](5) NULL,
	[IsProgramAddAble] [bit] NULL,
	[IsProgramEditAble] [bit] NULL,
	[IsProgramDeleteAble] [bit] NULL,
	[IsProgramViewAble] [bit] NULL,
	[IsProgramApprovalAble] [bit] NULL,
	[IsProgramUnApprovalAble] [bit] NULL,
	[IsProgramVoidAble] [bit] NULL,
	[IsProgramUnVoidAble] [bit] NULL,
	[IsProgramDirectVoid] [bit] NULL,
	[IsProgramPrintAble] [bit] NULL,
	[IsMenuAddVisible] [bit] NULL,
	[IsMenuHomeVisible] [bit] NULL,
	[IsVisible] [bit] NULL,
	[NavigateUrl] [nvarchar](1000) NULL,
	[HelpLinkId] [nvarchar](255) NULL,
	[AssemblyName] [nvarchar](50) NULL,
	[AssemblyClassName] [nvarchar](200) NULL,
	[StoreProcedureName] [nvarchar](200) NULL,
	[AccessKey] [nvarchar](100) NULL,
	[IsActive] [bit] NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2] NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2] NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2] NULL,
 CONSTRAINT [PK_Programs] PRIMARY KEY CLUSTERED 
(
	[ProgramsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [core].[Programs] ADD CONSTRAINT [DF_Programs_ProgramGuid] DEFAULT (newid()) FOR [ProgramGuid]
GO
ALTER TABLE [core].[Programs] ADD CONSTRAINT [DF_Programs_StatusId] DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [core].[Programs] ADD CONSTRAINT [DF_Programs_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
