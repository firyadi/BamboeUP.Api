USE [Dev.BamboeHR]
GO

/****** Object:  Table [core].[SubDistrict] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [core].[SubDistrict](
	[SubDistrictId] [bigint] IDENTITY(1,1) NOT NULL,
	[SubDistrictGuid] [uniqueidentifier] NOT NULL,
	[SubDistrictName] [nvarchar](255) NOT NULL,
	[DistrictId] [bigint] NOT NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2](7) NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2](7) NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2](7) NULL,
 CONSTRAINT [PK_SubDistrict] PRIMARY KEY CLUSTERED 
(
	[SubDistrictId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [core].[SubDistrict] ADD  CONSTRAINT [DF_SubDistrict_SubDistrictGuid]  DEFAULT (newid()) FOR [SubDistrictGuid]
GO

ALTER TABLE [core].[SubDistrict] ADD  CONSTRAINT [DF_SubDistrict_StatusId]  DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [core].[SubDistrict] ADD  CONSTRAINT [DF_SubDistrict_CreatedTime]  DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
