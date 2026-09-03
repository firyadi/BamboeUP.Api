USE [Dev.BamboeHR]
GO

SET NOCOUNT ON
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

/****** Seed UI-08 calendar sample tables (BamboeUp standard) ******/
DECLARE @CreatedById BIGINT = 1
DECLARE @Today DATE = CAST(SYSDATETIME() AS DATE)
DECLARE @MonthStart DATE = DATEFROMPARTS(YEAR(@Today), MONTH(@Today), 1)

/* MeetingRoom */
IF NOT EXISTS (SELECT 1 FROM [app].[MeetingRoom] WHERE [RoomCode] = 'R-A' AND [DeletedTime] IS NULL)
BEGIN
    INSERT INTO [app].[MeetingRoom]
    ([RoomCode], [RoomName], [Capacity], [Location], [StatusId], [CreatedById], [CreatedTime])
    VALUES
    ('R-A', 'Meeting Room 1', 12, 'Lantai 3', 1, @CreatedById, SYSDATETIME()),
    ('R-B', 'Meeting Room 2', 8, 'Lantai 3', 1, @CreatedById, SYSDATETIME()),
    ('R-C', 'Training Room', 24, 'Lantai 5', 1, @CreatedById, SYSDATETIME()),
    ('R-D', 'Board Room', 10, 'Lantai 5', 1, @CreatedById, SYSDATETIME())
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [app].[MeetingRoom] WHERE [RoomCode] = 'R-C' AND [DeletedTime] IS NULL)
        INSERT INTO [app].[MeetingRoom] ([RoomCode], [RoomName], [Capacity], [Location], [StatusId], [CreatedById], [CreatedTime])
        VALUES ('R-C', 'Training Room', 24, 'Lantai 5', 1, @CreatedById, SYSDATETIME());
    IF NOT EXISTS (SELECT 1 FROM [app].[MeetingRoom] WHERE [RoomCode] = 'R-D' AND [DeletedTime] IS NULL)
        INSERT INTO [app].[MeetingRoom] ([RoomCode], [RoomName], [Capacity], [Location], [StatusId], [CreatedById], [CreatedTime])
        VALUES ('R-D', 'Board Room', 10, 'Lantai 5', 1, @CreatedById, SYSDATETIME());
END
GO

DECLARE @CreatedById BIGINT = 1
DECLARE @Today DATE = CAST(SYSDATETIME() AS DATE)
DECLARE @MonthStart DATE = DATEFROMPARTS(YEAR(@Today), MONTH(@Today), 1)

/* Meeting */
IF NOT EXISTS (SELECT 1 FROM [app].[Meeting] WHERE [MeetingCode] = 'MTG-001' AND [DeletedTime] IS NULL)
BEGIN
    INSERT INTO [app].[Meeting]
    ([MeetingCode], [Subject], [StartDateTime], [EndDateTime], [Location], [SrMeetingCategory], [IsAllDay], [OrganizerUserId], [Note], [StatusId], [CreatedById], [CreatedTime])
    VALUES
    ('MTG-001', 'Daily Standup',
        DATEADD(HOUR, 9, CAST(@Today AS DATETIME2(7))),
        DATEADD(MINUTE, 30, DATEADD(HOUR, 9, CAST(@Today AS DATETIME2(7)))),
        'Meeting Room A', NULL, 0, 1, 'Scrum sync', 1, @CreatedById, SYSDATETIME()),
    ('MTG-002', 'Project Review',
        DATEADD(HOUR, 14, DATEADD(DAY, 1, CAST(@Today AS DATETIME2(7)))),
        DATEADD(HOUR, 15, DATEADD(DAY, 1, CAST(@Today AS DATETIME2(7)))),
        'Meeting Room B', NULL, 0, 1, 'Sprint review', 1, @CreatedById, SYSDATETIME()),
    ('MTG-003', 'Team Training',
        CAST(DATEADD(DAY, 3, @MonthStart) AS DATETIME2(7)),
        DATEADD(HOUR, 23, CAST(DATEADD(DAY, 3, @MonthStart) AS DATETIME2(7))),
        NULL, NULL, 1, 1, 'All-day training', 1, @CreatedById, SYSDATETIME())
END
GO

DECLARE @CreatedById BIGINT = 1
DECLARE @Today DATE = CAST(SYSDATETIME() AS DATE)

/* EmployeeLeave — SrLeaveType = StandardReference item id (placeholder 1) */
IF NOT EXISTS (SELECT 1 FROM [app].[EmployeeLeave] WHERE [EmployeeLeaveCode] = 'LV-001' AND [DeletedTime] IS NULL)
BEGIN
    INSERT INTO [app].[EmployeeLeave]
    ([EmployeeLeaveCode], [EmployeeId], [SrLeaveType], [StartDate], [EndDate], [Reason], [Note], [StatusId], [CreatedById], [CreatedTime])
    VALUES
    ('LV-001', 1, 1, DATEADD(DAY, 5, @Today), DATEADD(DAY, 7, @Today), 'Family event', NULL, 1, @CreatedById, SYSDATETIME()),
    ('LV-002', 1, 1, DATEADD(DAY, -2, @Today), DATEADD(DAY, -1, @Today), 'Sick leave', NULL, 1, @CreatedById, SYSDATETIME())
END
GO

DECLARE @CreatedById BIGINT = 1
DECLARE @Today DATE = CAST(SYSDATETIME() AS DATE)
DECLARE @RoomA BIGINT = (SELECT TOP (1) [MeetingRoomId] FROM [app].[MeetingRoom] WHERE [RoomCode] = 'R-A' AND [DeletedTime] IS NULL)
DECLARE @RoomB BIGINT = (SELECT TOP (1) [MeetingRoomId] FROM [app].[MeetingRoom] WHERE [RoomCode] = 'R-B' AND [DeletedTime] IS NULL)
DECLARE @RoomC BIGINT = (SELECT TOP (1) [MeetingRoomId] FROM [app].[MeetingRoom] WHERE [RoomCode] = 'R-C' AND [DeletedTime] IS NULL)
DECLARE @RoomD BIGINT = (SELECT TOP (1) [MeetingRoomId] FROM [app].[MeetingRoom] WHERE [RoomCode] = 'R-D' AND [DeletedTime] IS NULL)

/* RoomBooking */
IF @RoomA IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM [app].[RoomBooking] WHERE [RoomBookingCode] = 'RB-001' AND [DeletedTime] IS NULL)
BEGIN
    INSERT INTO [app].[RoomBooking]
    ([RoomBookingCode], [MeetingRoomId], [Subject], [StartDateTime], [EndDateTime], [BookedByUserId], [Note], [StatusId], [CreatedById], [CreatedTime])
    VALUES
    ('RB-001', @RoomA, 'Project Kickoff Meeting',
        DATEADD(HOUR, 9, CAST(@Today AS DATETIME2(7))),
        DATEADD(MINUTE, 90, DATEADD(HOUR, 9, CAST(@Today AS DATETIME2(7)))),
        1, 'IT Dept', 1, @CreatedById, SYSDATETIME()),
    ('RB-002', @RoomB, 'Budget Review',
        DATEADD(HOUR, 14, CAST(@Today AS DATETIME2(7))),
        DATEADD(MINUTE, 90, DATEADD(HOUR, 14, CAST(@Today AS DATETIME2(7)))),
        1, 'Finance', 2, @CreatedById, SYSDATETIME()),
    ('RB-003', @RoomA, 'Client Presentation',
        DATEADD(HOUR, 13, CAST(@Today AS DATETIME2(7))),
        DATEADD(MINUTE, 90, DATEADD(HOUR, 13, CAST(@Today AS DATETIME2(7)))),
        1, 'Marketing', 3, @CreatedById, SYSDATETIME())
END

IF @RoomC IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM [app].[RoomBooking] WHERE [RoomBookingCode] = 'RB-004' AND [DeletedTime] IS NULL)
BEGIN
    INSERT INTO [app].[RoomBooking]
    ([RoomBookingCode], [MeetingRoomId], [Subject], [StartDateTime], [EndDateTime], [BookedByUserId], [Note], [StatusId], [CreatedById], [CreatedTime])
    VALUES
    ('RB-004', @RoomC, 'System Training',
        DATEADD(MINUTE, 30, DATEADD(HOUR, 8, CAST(@Today AS DATETIME2(7)))),
        DATEADD(HOUR, 10, CAST(@Today AS DATETIME2(7))),
        1, 'HR Dept', 1, @CreatedById, SYSDATETIME())
END

IF @RoomD IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM [app].[RoomBooking] WHERE [RoomBookingCode] = 'RB-005' AND [DeletedTime] IS NULL)
BEGIN
    INSERT INTO [app].[RoomBooking]
    ([RoomBookingCode], [MeetingRoomId], [Subject], [StartDateTime], [EndDateTime], [BookedByUserId], [Note], [StatusId], [CreatedById], [CreatedTime])
    VALUES
    ('RB-005', @RoomD, 'Management Meeting',
        DATEADD(HOUR, 9, CAST(@Today AS DATETIME2(7))),
        DATEADD(HOUR, 11, CAST(@Today AS DATETIME2(7))),
        1, 'Board of Director', 1, @CreatedById, SYSDATETIME())
END
GO

PRINT 'Seed.Calendar.SampleTables done.'
GO
