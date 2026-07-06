USE [Dev.BamboeHR]
GO

/****** Object:  Table [app].[CompanyOffice] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[CompanyOffice]
(
	[CompanyOfficeId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyOfficeGuid] [uniqueidentifier] NOT NULL,
	[CompanyId] [bigint] NOT NULL,
	[CompanyGuid] [uniqueidentifier] NOT NULL,
	[CompanyOfficeName] [varchar](150) NOT NULL,
	[SrAddressType] [bigint] NOT NULL,
	[CountryId] [bigint] NOT NULL,
	[StateId] [bigint] NOT NULL,
	[CityId] [bigint] NOT NULL,
	[PostalCodeId] [varchar](8) NOT NULL,
	[Address] [varchar](500) NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2] NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2] NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2] NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL
CONSTRAINT [PK_CompanyOffice] PRIMARY KEY CLUSTERED 
(
    [CompanyOfficeId] ASC
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

ALTER TABLE [core].[CompanyOffice] ADD CONSTRAINT [DF_CompanyOffice_CompanyOfficeGuid] DEFAULT (newid()) FOR [CompanyOfficeGuid]
GO
ALTER TABLE [core].[CompanyOffice] ADD CONSTRAINT [DF_CompanyOffice_CompanyGuid] DEFAULT (newid()) FOR [CompanyGuid]
GO
ALTER TABLE [core].[CompanyOffice] ADD CONSTRAINT [DF_CompanyOffice_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
ALTER TABLE [core].[CompanyOffice] ADD CONSTRAINT [DF_CompanyOffice_StatusId] DEFAULT ((1)) FOR [StatusId]
GO
