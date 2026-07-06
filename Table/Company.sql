USE [Dev.BamboeHR]
GO

/****** Object:  Table [app].[Company] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[Company]
(
	[CompanyId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyGuid] [uniqueidentifier] NOT NULL,
	[CompanyName] [nvarchar](200) NOT NULL,
	[InitialName] [varchar](20) NULL,
	[TaxCompulsionNo] [varchar](30) NULL,
	[RegistrationNo] [varchar](30) NULL,
	[ParentCompanyId] [bigint] NULL,
	[DefaultCurrency] [char](3) NULL,
	[CompanyLogo] [varbinary](max) NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2] NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2] NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2] NULL
CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
    [CompanyId] ASC
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

ALTER TABLE [core].[Company] ADD CONSTRAINT [DF_Company_CompanyGuid] DEFAULT (newid()) FOR [CompanyGuid]
GO
ALTER TABLE [core].[Company] ADD CONSTRAINT [DF_Company_StatusId] DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [core].[Company] ADD CONSTRAINT [DF_Company_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
