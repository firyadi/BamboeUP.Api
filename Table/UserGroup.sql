USE [Dev.BamboeHR]
GO

/****** Object:  Table [core].[UserGroup] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [core].[UserGroup](
	[UserGroupId] [bigint] IDENTITY(1,1) NOT NULL,
	[UserGroupGuid] [uniqueidentifier] NOT NULL,
	[UserGroupName] [nvarchar](100) NOT NULL,
	[IsEditAble] [bit] NOT NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2] NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2] NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2] NULL,
 CONSTRAINT [PK_UserGroup] PRIMARY KEY CLUSTERED 
(
	[UserGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [core].[UserGroup] ADD CONSTRAINT [DF_UserGroup_UserGroupGuid] DEFAULT (newid()) FOR [UserGroupGuid]
GO
ALTER TABLE [core].[UserGroup] ADD CONSTRAINT [DF_UserGroup_StatusId] DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [core].[UserGroup] ADD CONSTRAINT [DF_UserGroup_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
