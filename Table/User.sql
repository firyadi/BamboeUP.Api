USE [Dev.BamboeHR]
GO

/****** Object:  Table [core].[User] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [core].[User]
(
	[UserId] [bigint] IDENTITY(1,1) NOT NULL,
	[UserGuid] [uniqueidentifier] NOT NULL,
	[UserName] [nvarchar](100) NOT NULL,
	[PasswordHash] [nvarchar](max) NOT NULL,
	[PasswordSalt] [nvarchar](128) NOT NULL CONSTRAINT [DF_User_PasswordSalt] DEFAULT (''),
	[FullName] [nvarchar](200) NULL,
	[Email] [nvarchar](200) NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2] NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2] NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2] NULL
CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
    [UserId] ASC
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

ALTER TABLE [core].[User] ADD CONSTRAINT [DF_User_UserGuid] DEFAULT (newid()) FOR [UserGuid]
GO
ALTER TABLE [core].[User] ADD CONSTRAINT [DF_User_StatusId] DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [core].[User] ADD CONSTRAINT [DF_User_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
