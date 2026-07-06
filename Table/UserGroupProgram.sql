USE [Dev.BamboeHR]
GO

/****** Object:  Table [core].[UserGroupProgram] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [core].[UserGroupProgram](
	[UserGroupProgramId] [bigint] IDENTITY(1,1) NOT NULL,
	[UserGroupProgramGuid] [uniqueidentifier] NOT NULL,
	[UserGroupId] [bigint] NOT NULL,
	[ProgramsId] [bigint] NOT NULL,
	[IsUserGroupAddAble] [bit] NULL,
	[IsUserGroupEditAble] [bit] NULL,
	[IsUserGroupDeleteAble] [bit] NULL,
	[IsUserGroupApprovalAble] [bit] NULL,
	[IsUserGroupUnApprovalAble] [bit] NULL,
	[IsUserGroupVoidAble] [bit] NULL,
	[IsUserGroupUnVoidAble] [bit] NULL,
	[IsUserGroupExportAble] [bit] NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2] NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2] NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2] NULL,
 CONSTRAINT [PK_UserGroupProgram] PRIMARY KEY CLUSTERED 
(
	[UserGroupProgramId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [core].[UserGroupProgram] ADD CONSTRAINT [DF_UserGroupProgram_UserGroupProgramGuid] DEFAULT (newid()) FOR [UserGroupProgramGuid]
GO
ALTER TABLE [core].[UserGroupProgram] ADD CONSTRAINT [DF_UserGroupProgram_StatusId] DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [core].[UserGroupProgram] ADD CONSTRAINT [DF_UserGroupProgram_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
