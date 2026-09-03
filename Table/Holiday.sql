USE [Dev.BamboeHR]
GO

/****** Object:  Table [app].[Holiday]    Script Date: 9/3/2026 11:14:42 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[Holiday](
	[HolidayId] [bigint] IDENTITY(1,1) NOT NULL,
	[HolidayGuid] [uniqueidentifier] NOT NULL,
	[CountryId] [bigint] NOT NULL,
	[HolidayDate] [date] NOT NULL,
	[HolidayName] [nvarchar](200) NOT NULL,
	[HolidayDescription] [nvarchar](500) NULL,
	[SrHolidayType] [bigint] NULL,
	[IsNational] [bit] NOT NULL,
	[IsCompanyHoliday] [bit] NOT NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2](7) NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2](7) NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2](7) NULL,
 CONSTRAINT [PK_Holiday] PRIMARY KEY CLUSTERED
(
	[HolidayId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Holiday_HolidayGuid] UNIQUE NONCLUSTERED
(
	[HolidayGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [app].[Holiday] ADD  CONSTRAINT [DF_Holiday_HolidayGuid]  DEFAULT (newsequentialid()) FOR [HolidayGuid]
GO

ALTER TABLE [app].[Holiday] ADD  CONSTRAINT [DF_Holiday_IsNational]  DEFAULT ((1)) FOR [IsNational]
GO

ALTER TABLE [app].[Holiday] ADD  CONSTRAINT [DF_Holiday_IsCompanyHoliday]  DEFAULT ((0)) FOR [IsCompanyHoliday]
GO

ALTER TABLE [app].[Holiday] ADD  CONSTRAINT [DF_Holiday_StatusId]  DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [app].[Holiday] ADD  CONSTRAINT [DF_Holiday_CreatedTime]  DEFAULT (sysdatetime()) FOR [CreatedTime]
GO

ALTER TABLE [app].[Holiday]  WITH CHECK ADD  CONSTRAINT [CHK_Holiday_Status] CHECK  (([StatusId]=(2) OR [StatusId]=(1) OR [StatusId]=(0)))
GO

ALTER TABLE [app].[Holiday] CHECK CONSTRAINT [CHK_Holiday_Status]
GO

CREATE NONCLUSTERED INDEX [IX_Holiday_CountryId]
ON [app].[Holiday]
(
	[CountryId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Holiday_SrHolidayType]
ON [app].[Holiday]
(
	[SrHolidayType] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Holiday_HolidayDate_Active]
ON [app].[Holiday]
(
	[HolidayDate] ASC
)
INCLUDE([HolidayGuid],[HolidayName],[HolidayDescription],[SrHolidayType],[IsNational],[IsCompanyHoliday],[StatusId])
WHERE ([DeletedTime] IS NULL AND [StatusId]>(0))
GO
