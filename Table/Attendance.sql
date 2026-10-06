USE [Dev.BamboeHR]
GO

/****** Object:  Table [app].[Attendance]    Script Date: 9/24/2026 10:18:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[Attendance](
	[AttendanceId] [bigint] IDENTITY(1,1) NOT NULL,
	[AttendanceGuid] [uniqueidentifier] NOT NULL,
	[CompanyId] [bigint] NOT NULL,
	[CompanyOfficeId] [bigint] NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[AttendanceDate] [date] NOT NULL,
	[CheckInTime] [datetime2](7) NULL,
	[CheckOutTime] [datetime2](7) NULL,
	[CheckInChannel] [nvarchar](10) NULL,
	[CheckOutChannel] [nvarchar](10) NULL,
	[CheckInFileStorageId] [bigint] NULL,
	[CheckOutFileStorageId] [bigint] NULL,
	[CheckInLatitude] [decimal](9, 6) NULL,
	[CheckInLongitude] [decimal](9, 6) NULL,
	[CheckInAccuracyMeters] [int] NULL,
	[CheckOutLatitude] [decimal](9, 6) NULL,
	[CheckOutLongitude] [decimal](9, 6) NULL,
	[CheckOutAccuracyMeters] [int] NULL,
	[Note] [nvarchar](500) NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2](7) NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2](7) NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2](7) NULL,
 CONSTRAINT [PK_Attendance] PRIMARY KEY CLUSTERED
(
	[AttendanceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Attendance_AttendanceGuid] UNIQUE NONCLUSTERED
(
	[AttendanceGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [app].[Attendance] ADD CONSTRAINT [DF_Attendance_AttendanceGuid] DEFAULT (newsequentialid()) FOR [AttendanceGuid]
GO

ALTER TABLE [app].[Attendance] ADD CONSTRAINT [DF_Attendance_StatusId] DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [app].[Attendance] ADD CONSTRAINT [DF_Attendance_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO

CREATE UNIQUE NONCLUSTERED INDEX [UQ_Attendance_PersonDate_Active]
ON [app].[Attendance]
(
	[PersonId] ASC,
	[AttendanceDate] ASC
)
WHERE ([DeletedTime] IS NULL AND [StatusId] > (0))
GO

CREATE NONCLUSTERED INDEX [IX_Attendance_CompanyId]
ON [app].[Attendance]
(
	[CompanyId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Attendance_CompanyOfficeId]
ON [app].[Attendance]
(
	[CompanyOfficeId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Attendance_CheckInFileStorageId]
ON [app].[Attendance]
(
	[CheckInFileStorageId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Attendance_CheckOutFileStorageId]
ON [app].[Attendance]
(
	[CheckOutFileStorageId] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Attendance_AttendanceDate_Active]
ON [app].[Attendance]
(
	[AttendanceDate] ASC
)
INCLUDE ([AttendanceGuid], [PersonId], [CompanyId], [CompanyOfficeId])
WHERE ([DeletedTime] IS NULL AND [StatusId] > (0))
GO
