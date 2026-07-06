USE [Dev.BamboeHR]
GO

/****** Object:  Table [core].[City] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [core].[City](
	[CityId] [bigint] IDENTITY(1,1) NOT NULL,
	[CityGuid] [uniqueidentifier] NOT NULL,
	[CityName] [nvarchar](255) NOT NULL,
	[ProvinceId] [bigint] NOT NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2](7) NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2](7) NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2](7) NULL,
 CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED 
(
	[CityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [core].[City] ADD  CONSTRAINT [DF_City_CityGuid]  DEFAULT (newid()) FOR [CityGuid]
GO

ALTER TABLE [core].[City] ADD  CONSTRAINT [DF_City_StatusId]  DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [core].[City] ADD  CONSTRAINT [DF_City_CreatedTime]  DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
