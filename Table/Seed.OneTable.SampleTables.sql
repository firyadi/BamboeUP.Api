USE [Dev.BamboeHR]
GO

SET NOCOUNT ON
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

/****** Seed UI-01 / API-01 1-Table sample ******/
DECLARE @CreatedById BIGINT = 1

IF NOT EXISTS (SELECT 1 FROM [app].[WorkNotice] WHERE [WorkNoticeCode] = 'WN-001' AND [DeletedTime] IS NULL)
BEGIN
    INSERT INTO [app].[WorkNotice]
    ([WorkNoticeCode], [WorkNoticeName], [WorkNoticeDate], [IsEditAble], [Note], [StatusId], [CreatedById], [CreatedTime])
    VALUES
    ('WN-001', N'Ramadan office hours',      '2026-03-01', 1, N'Shorter hours during Ramadan.', 1, @CreatedById, SYSDATETIME()),
    ('WN-002', N'Payroll cutoff reminder',   '2026-03-20', 1, N'Submit overtime before cutoff.', 1, @CreatedById, SYSDATETIME()),
    ('WN-003', N'Annual medical checkup',    '2026-04-10', 0, N'Plant staff — schedule with HR.', 1, @CreatedById, SYSDATETIME()),
    ('WN-004', N'Independence day holiday',  '2026-08-17', 1, N'National holiday — office closed.', 1, @CreatedById, SYSDATETIME())
END
GO
