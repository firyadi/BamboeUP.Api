USE [Dev.BamboeHR]
GO

/****** Object:  Table [core].[Bank] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [core].[Bank]
(
	[BankId] [bigint] IDENTITY(1,1) NOT NULL,
	[BankGuid] [uniqueidentifier] NOT NULL,
	[BankName] [nvarchar](200) NOT NULL,
	[BankInitial] [varchar](15) NOT NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2] NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2] NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2] NULL
CONSTRAINT [PK_Bank] PRIMARY KEY CLUSTERED 
(
    [BankId] ASC
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

ALTER TABLE [core].[Bank] ADD CONSTRAINT [DF_Bank_BankGuid] DEFAULT (newid()) FOR [BankGuid]
GO
ALTER TABLE [core].[Bank] ADD CONSTRAINT [DF_Bank_StatusId] DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [core].[Bank] ADD CONSTRAINT [DF_Bank_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
