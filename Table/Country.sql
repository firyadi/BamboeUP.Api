USE [Dev.BamboeHR]
GO

/****** Object:  Table [core].[Country]    Script Date: 4/1/2026 12:35:43 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [core].[Country](
	[CountryId] [bigint] IDENTITY(1,1) NOT NULL,
	[CountryGuid] [uniqueidentifier] NOT NULL,
	[CountryIso] [char](2) NOT NULL,
	[CountryName] [varchar](200) NOT NULL,
	[CountryIso3] [char](3) NULL,
	[PhoneCode] [int] NOT NULL,
	[CurrencyCode] [char](3) NOT NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2](7) NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2](7) NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2](7) NULL,
 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [core].[Country] ADD  CONSTRAINT [DF_Country_CountryGuid]  DEFAULT (newid()) FOR [CountryGuid]
GO

ALTER TABLE [core].[Country] ADD  CONSTRAINT [DF_Country_StatusId]  DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [core].[Country] ADD  CONSTRAINT [DF_Country_CreatedTime]  DEFAULT (sysdatetime()) FOR [CreatedTime]
GO


