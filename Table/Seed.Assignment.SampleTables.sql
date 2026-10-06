USE [Dev.BamboeHR]
GO

SET NOCOUNT ON
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

/****** Seed UI-06 TemplateUiAssignment sample tables ******/
DECLARE @CreatedById BIGINT = 1

/* Catalog — items shown in the Available list */
IF NOT EXISTS (SELECT 1 FROM [app].[WorkCapability] WHERE [CapabilityCode] = 'LEAVE' AND [DeletedTime] IS NULL)
BEGIN
    INSERT INTO [app].[WorkCapability]
    ([CapabilityCode], [CapabilityName], [CapabilityDescription], [StatusId], [CreatedById], [CreatedTime])
    VALUES
    ('LEAVE',       N'Leave',        N'Submit and approve employee leave', 1, @CreatedById, SYSDATETIME()),
    ('ATTEND',      N'Attendance',   N'Daily attendance and timesheet', 1, @CreatedById, SYSDATETIME()),
    ('PAYROLL',     N'Payroll',      N'Payroll run and payslip', 1, @CreatedById, SYSDATETIME()),
    ('OVERTIME',    N'Overtime',     N'Overtime request and approval', 1, @CreatedById, SYSDATETIME()),
    ('CLAIM',       N'Claim',        N'Medical and travel claims', 1, @CreatedById, SYSDATETIME()),
    ('RECRUIT',     N'Recruitment',  N'Vacancy and candidate pipeline', 1, @CreatedById, SYSDATETIME()),
    ('TRAINING',    N'Training',     N'Training catalog and enrollment', 1, @CreatedById, SYSDATETIME()),
    ('ASSET',       N'Asset',        N'Company asset assignment', 1, @CreatedById, SYSDATETIME())
END
GO

DECLARE @CreatedById BIGINT = 1

/* Header — teams that receive assigned capabilities */
IF NOT EXISTS (SELECT 1 FROM [app].[WorkTeam] WHERE [WorkTeamCode] = 'HR-OPS' AND [DeletedTime] IS NULL)
BEGIN
    INSERT INTO [app].[WorkTeam]
    ([WorkTeamCode], [WorkTeamName], [IsEditAble], [Note], [StatusId], [CreatedById], [CreatedTime])
    VALUES
    ('HR-OPS',  N'HR Operations',      1, N'Leave, attendance, and recruitment desk', 1, @CreatedById, SYSDATETIME()),
    ('FIN-DSK', N'Finance Desk',        1, N'Payroll and claim processing', 1, @CreatedById, SYSDATETIME()),
    ('PLANT-1', N'Plant Supervisors',   0, N'Shop-floor overtime and attendance only', 1, @CreatedById, SYSDATETIME())
END
GO

DECLARE @CreatedById BIGINT = 1

DECLARE @HrOps   BIGINT = (SELECT TOP (1) [WorkTeamId] FROM [app].[WorkTeam] WHERE [WorkTeamCode] = 'HR-OPS' AND [DeletedTime] IS NULL)
DECLARE @FinDsk  BIGINT = (SELECT TOP (1) [WorkTeamId] FROM [app].[WorkTeam] WHERE [WorkTeamCode] = 'FIN-DSK' AND [DeletedTime] IS NULL)
DECLARE @Plant   BIGINT = (SELECT TOP (1) [WorkTeamId] FROM [app].[WorkTeam] WHERE [WorkTeamCode] = 'PLANT-1' AND [DeletedTime] IS NULL)

DECLARE @Leave     BIGINT = (SELECT TOP (1) [WorkCapabilityId] FROM [app].[WorkCapability] WHERE [CapabilityCode] = 'LEAVE' AND [DeletedTime] IS NULL)
DECLARE @Attend    BIGINT = (SELECT TOP (1) [WorkCapabilityId] FROM [app].[WorkCapability] WHERE [CapabilityCode] = 'ATTEND' AND [DeletedTime] IS NULL)
DECLARE @Payroll   BIGINT = (SELECT TOP (1) [WorkCapabilityId] FROM [app].[WorkCapability] WHERE [CapabilityCode] = 'PAYROLL' AND [DeletedTime] IS NULL)
DECLARE @Overtime  BIGINT = (SELECT TOP (1) [WorkCapabilityId] FROM [app].[WorkCapability] WHERE [CapabilityCode] = 'OVERTIME' AND [DeletedTime] IS NULL)
DECLARE @Claim     BIGINT = (SELECT TOP (1) [WorkCapabilityId] FROM [app].[WorkCapability] WHERE [CapabilityCode] = 'CLAIM' AND [DeletedTime] IS NULL)
DECLARE @Recruit   BIGINT = (SELECT TOP (1) [WorkCapabilityId] FROM [app].[WorkCapability] WHERE [CapabilityCode] = 'RECRUIT' AND [DeletedTime] IS NULL)

/* Junction — Assigned list + permission matrix */
IF @HrOps IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [app].[WorkTeamCapability] WHERE [WorkTeamId] = @HrOps AND [DeletedTime] IS NULL)
BEGIN
    INSERT INTO [app].[WorkTeamCapability]
    ([WorkTeamId], [WorkCapabilityId], [IsViewAble], [IsAddAble], [IsEditAble], [IsApproveAble], [IsExportAble], [StatusId], [CreatedById], [CreatedTime])
    VALUES
    (@HrOps, @Leave,   1, 1, 1, 1, 1, 1, @CreatedById, SYSDATETIME()),
    (@HrOps, @Attend,  1, 1, 1, 0, 1, 1, @CreatedById, SYSDATETIME()),
    (@HrOps, @Recruit, 1, 1, 1, 1, 0, 1, @CreatedById, SYSDATETIME())
END

IF @FinDsk IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [app].[WorkTeamCapability] WHERE [WorkTeamId] = @FinDsk AND [DeletedTime] IS NULL)
BEGIN
    INSERT INTO [app].[WorkTeamCapability]
    ([WorkTeamId], [WorkCapabilityId], [IsViewAble], [IsAddAble], [IsEditAble], [IsApproveAble], [IsExportAble], [StatusId], [CreatedById], [CreatedTime])
    VALUES
    (@FinDsk, @Payroll, 1, 0, 1, 1, 1, 1, @CreatedById, SYSDATETIME()),
    (@FinDsk, @Claim,   1, 1, 1, 1, 1, 1, @CreatedById, SYSDATETIME())
END

IF @Plant IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [app].[WorkTeamCapability] WHERE [WorkTeamId] = @Plant AND [DeletedTime] IS NULL)
BEGIN
    INSERT INTO [app].[WorkTeamCapability]
    ([WorkTeamId], [WorkCapabilityId], [IsViewAble], [IsAddAble], [IsEditAble], [IsApproveAble], [IsExportAble], [StatusId], [CreatedById], [CreatedTime])
    VALUES
    (@Plant, @Attend,   1, 1, 0, 0, 0, 1, @CreatedById, SYSDATETIME()),
    (@Plant, @Overtime, 1, 1, 0, 1, 0, 1, @CreatedById, SYSDATETIME())
END
GO
