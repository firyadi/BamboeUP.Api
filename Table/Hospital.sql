USE [Dev.BamboeHR]
GO

/****** Object:  Table [app].[Hospital]    Script Date: 7/3/2026 9:02:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[Hospital](
	[HospitalId] [bigint] IDENTITY(1,1) NOT NULL,
	[HospitalGuid] [uniqueidentifier] NOT NULL,
	[HospitalName] [nvarchar](200) NOT NULL,
	[HospitalCode] [varchar](20) NULL,
	[ShortName] [nvarchar](100) NULL,
	[LicenseNo] [varchar](50) NULL,
	[HospitalType] [varchar](30) NULL,
	[HospitalClass] [varchar](10) NULL,
	[PhoneNo] [varchar](30) NULL,
	[Email] [varchar](200) NULL,
	[Website] [varchar](200) NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2](7) NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2](7) NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2](7) NULL,
 CONSTRAINT [PK_Hospital] PRIMARY KEY CLUSTERED 
(
	[HospitalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Hospital_Code] UNIQUE NONCLUSTERED 
(
	[HospitalCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Hospital_Guid] UNIQUE NONCLUSTERED 
(
	[HospitalGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Hospital_Name] UNIQUE NONCLUSTERED 
(
	[HospitalName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [app].[Hospital] ADD  DEFAULT (newid()) FOR [HospitalGuid]
GO

ALTER TABLE [app].[Hospital] ADD  DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [app].[Hospital] ADD  DEFAULT (sysdatetime()) FOR [CreatedTime]
GO

ALTER TABLE [app].[Hospital]  WITH CHECK ADD  CONSTRAINT [CHK_Hospital_Status] CHECK  (([StatusId]=(2) OR [StatusId]=(1) OR [StatusId]=(0)))
GO

ALTER TABLE [app].[Hospital] CHECK CONSTRAINT [CHK_Hospital_Status]
GO


