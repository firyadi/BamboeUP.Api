USE [Dev.BamboeHR]
GO

/****** Object:  UI-08 TemplateUiCalendar sample tables (BamboeUp standard) ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/* Drop dependents first */
IF OBJECT_ID(N'[app].[RoomBooking]', N'U') IS NOT NULL
    DROP TABLE [app].[RoomBooking]
GO

IF OBJECT_ID(N'[app].[Meeting]', N'U') IS NOT NULL
    DROP TABLE [app].[Meeting]
GO

IF OBJECT_ID(N'[app].[EmployeeLeave]', N'U') IS NOT NULL
    DROP TABLE [app].[EmployeeLeave]
GO

IF OBJECT_ID(N'[app].[MeetingRoom]', N'U') IS NOT NULL
    DROP TABLE [app].[MeetingRoom]
GO

/****** Object:  Table [app].[MeetingRoom] ******/
CREATE TABLE [app].[MeetingRoom](
	[MeetingRoomId] [bigint] IDENTITY(1,1) NOT NULL,
	[MeetingRoomGuid] [uniqueidentifier] NOT NULL,
	[RoomCode] [varchar](20) NOT NULL,
	[RoomName] [varchar](200) NOT NULL,
	[Capacity] [int] NULL,
	[Location] [varchar](200) NULL,
	[Note] [varchar](4000) NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2](7) NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2](7) NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2](7) NULL,
 CONSTRAINT [PK_MeetingRoom] PRIMARY KEY CLUSTERED
(
	[MeetingRoomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_MeetingRoom_Code] UNIQUE NONCLUSTERED
(
	[RoomCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_MeetingRoom_Guid] UNIQUE NONCLUSTERED
(
	[MeetingRoomGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [app].[MeetingRoom] ADD CONSTRAINT [DF_MeetingRoom_MeetingRoomGuid] DEFAULT (newsequentialid()) FOR [MeetingRoomGuid]
GO
ALTER TABLE [app].[MeetingRoom] ADD CONSTRAINT [DF_MeetingRoom_StatusId] DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [app].[MeetingRoom] ADD CONSTRAINT [DF_MeetingRoom_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO

/****** Object:  Table [app].[Meeting] — DateTimeRange calendar ******/
CREATE TABLE [app].[Meeting](
	[MeetingId] [bigint] IDENTITY(1,1) NOT NULL,
	[MeetingGuid] [uniqueidentifier] NOT NULL,
	[MeetingCode] [varchar](20) NOT NULL,
	[Subject] [varchar](400) NOT NULL,
	[StartDateTime] [datetime2](7) NOT NULL,
	[EndDateTime] [datetime2](7) NOT NULL,
	[Location] [varchar](200) NULL,
	[SrMeetingCategory] [bigint] NULL,
	[IsAllDay] [bit] NOT NULL,
	[OrganizerUserId] [bigint] NULL,
	[Note] [varchar](4000) NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2](7) NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2](7) NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2](7) NULL,
 CONSTRAINT [PK_Meeting] PRIMARY KEY CLUSTERED
(
	[MeetingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Meeting_Code] UNIQUE NONCLUSTERED
(
	[MeetingCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Meeting_Guid] UNIQUE NONCLUSTERED
(
	[MeetingGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [app].[Meeting] ADD CONSTRAINT [DF_Meeting_MeetingGuid] DEFAULT (newsequentialid()) FOR [MeetingGuid]
GO
ALTER TABLE [app].[Meeting] ADD CONSTRAINT [DF_Meeting_IsAllDay] DEFAULT ((0)) FOR [IsAllDay]
GO
ALTER TABLE [app].[Meeting] ADD CONSTRAINT [DF_Meeting_StatusId] DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [app].[Meeting] ADD CONSTRAINT [DF_Meeting_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO

ALTER TABLE [app].[Meeting] WITH CHECK ADD CONSTRAINT [CK_Meeting_DateTimeRange] CHECK (([StartDateTime]<=[EndDateTime]))
GO
ALTER TABLE [app].[Meeting] CHECK CONSTRAINT [CK_Meeting_DateTimeRange]
GO

CREATE NONCLUSTERED INDEX [IX_Meeting_Range_Active]
ON [app].[Meeting]
(
	[StartDateTime] ASC,
	[EndDateTime] ASC
)
INCLUDE([MeetingGuid],[MeetingCode],[Subject],[Location],[SrMeetingCategory],[IsAllDay],[StatusId])
WHERE ([DeletedTime] IS NULL AND [StatusId]>(0))
GO

/****** Object:  Table [app].[EmployeeLeave] — DateRange calendar ******/
CREATE TABLE [app].[EmployeeLeave](
	[EmployeeLeaveId] [bigint] IDENTITY(1,1) NOT NULL,
	[EmployeeLeaveGuid] [uniqueidentifier] NOT NULL,
	[EmployeeLeaveCode] [varchar](20) NOT NULL,
	[EmployeeId] [bigint] NOT NULL,
	[SrLeaveType] [bigint] NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
	[Reason] [varchar](400) NULL,
	[Note] [varchar](4000) NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2](7) NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2](7) NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2](7) NULL,
 CONSTRAINT [PK_EmployeeLeave] PRIMARY KEY CLUSTERED
(
	[EmployeeLeaveId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_EmployeeLeave_Code] UNIQUE NONCLUSTERED
(
	[EmployeeLeaveCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_EmployeeLeave_Guid] UNIQUE NONCLUSTERED
(
	[EmployeeLeaveGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [app].[EmployeeLeave] ADD CONSTRAINT [DF_EmployeeLeave_EmployeeLeaveGuid] DEFAULT (newsequentialid()) FOR [EmployeeLeaveGuid]
GO
ALTER TABLE [app].[EmployeeLeave] ADD CONSTRAINT [DF_EmployeeLeave_StatusId] DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [app].[EmployeeLeave] ADD CONSTRAINT [DF_EmployeeLeave_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO

ALTER TABLE [app].[EmployeeLeave] WITH CHECK ADD CONSTRAINT [CK_EmployeeLeave_ValidDate] CHECK (([StartDate]<=[EndDate]))
GO
ALTER TABLE [app].[EmployeeLeave] CHECK CONSTRAINT [CK_EmployeeLeave_ValidDate]
GO

CREATE NONCLUSTERED INDEX [IX_EmployeeLeave_Range_Active]
ON [app].[EmployeeLeave]
(
	[StartDate] ASC,
	[EndDate] ASC
)
INCLUDE([EmployeeLeaveGuid],[EmployeeLeaveCode],[EmployeeId],[SrLeaveType],[Reason],[StatusId])
WHERE ([DeletedTime] IS NULL AND [StatusId]>(0))
GO

CREATE NONCLUSTERED INDEX [IX_EmployeeLeave_Employee]
ON [app].[EmployeeLeave]
(
	[EmployeeId] ASC,
	[StartDate] ASC
)
WHERE ([DeletedTime] IS NULL AND [StatusId]>(0))
GO

/****** Object:  Table [app].[RoomBooking] — Resource calendar ******/
CREATE TABLE [app].[RoomBooking](
	[RoomBookingId] [bigint] IDENTITY(1,1) NOT NULL,
	[RoomBookingGuid] [uniqueidentifier] NOT NULL,
	[RoomBookingCode] [varchar](20) NOT NULL,
	[MeetingRoomId] [bigint] NOT NULL,
	[Subject] [varchar](400) NOT NULL,
	[StartDateTime] [datetime2](7) NOT NULL,
	[EndDateTime] [datetime2](7) NOT NULL,
	[BookedByUserId] [bigint] NULL,
	[Note] [varchar](4000) NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2](7) NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2](7) NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2](7) NULL,
 CONSTRAINT [PK_RoomBooking] PRIMARY KEY CLUSTERED
(
	[RoomBookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_RoomBooking_Code] UNIQUE NONCLUSTERED
(
	[RoomBookingCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_RoomBooking_Guid] UNIQUE NONCLUSTERED
(
	[RoomBookingGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [app].[RoomBooking] ADD CONSTRAINT [DF_RoomBooking_RoomBookingGuid] DEFAULT (newsequentialid()) FOR [RoomBookingGuid]
GO
ALTER TABLE [app].[RoomBooking] ADD CONSTRAINT [DF_RoomBooking_StatusId] DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [app].[RoomBooking] ADD CONSTRAINT [DF_RoomBooking_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO

ALTER TABLE [app].[RoomBooking] WITH CHECK ADD CONSTRAINT [CK_RoomBooking_DateTimeRange] CHECK (([StartDateTime]<[EndDateTime]))
GO
ALTER TABLE [app].[RoomBooking] CHECK CONSTRAINT [CK_RoomBooking_DateTimeRange]
GO

ALTER TABLE [app].[RoomBooking] WITH CHECK ADD CONSTRAINT [FK_RoomBooking_MeetingRoom] FOREIGN KEY([MeetingRoomId])
REFERENCES [app].[MeetingRoom] ([MeetingRoomId])
GO
ALTER TABLE [app].[RoomBooking] CHECK CONSTRAINT [FK_RoomBooking_MeetingRoom]
GO

CREATE NONCLUSTERED INDEX [IX_RoomBooking_Room_Range]
ON [app].[RoomBooking]
(
	[MeetingRoomId] ASC,
	[StartDateTime] ASC,
	[EndDateTime] ASC
)
INCLUDE([RoomBookingGuid],[RoomBookingCode],[Subject],[StatusId])
WHERE ([DeletedTime] IS NULL AND [StatusId]>(0))
GO

/* Holiday range index (existing table) */
IF OBJECT_ID(N'[app].[Holiday]', N'U') IS NOT NULL
   AND NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE name = N'IX_Holiday_Date_Active' AND object_id = OBJECT_ID(N'[app].[Holiday]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Holiday_Date_Active]
    ON [app].[Holiday]
    (
        [HolidayDate] ASC
    )
    INCLUDE([HolidayGuid],[HolidayName],[HolidayDescription],[SrHolidayType],[IsNational],[IsCompanyHoliday],[StatusId])
    WHERE ([DeletedTime] IS NULL AND [StatusId]>(0))
END
GO
