USE [Dev.BamboeHR]
GO

/****** Object:  Table [core].[PostalCode] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [core].[PostalCode](
	[PostalCodeId] [bigint] IDENTITY(1,1) NOT NULL,
	[PostalCodeGuid] [uniqueidentifier] NOT NULL,
	[SubDistrictId] [bigint] NOT NULL,
	[PostalCode] [varchar](10) NOT NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2](7) NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2](7) NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2](7) NULL,
 CONSTRAINT [PK_PostalCode] PRIMARY KEY CLUSTERED 
(
	[PostalCodeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [core].[PostalCode] ADD  CONSTRAINT [DF_PostalCode_PostalCodeGuid]  DEFAULT (newid()) FOR [PostalCodeGuid]
GO

ALTER TABLE [core].[PostalCode] ADD  CONSTRAINT [DF_PostalCode_StatusId]  DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [core].[PostalCode] ADD  CONSTRAINT [DF_PostalCode_CreatedTime]  DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
