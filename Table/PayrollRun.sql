USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   TABLE: app.PayrollRun  (Header — payroll period batch)
   UI-07 / Process Framework pilot for PAYROLL_RUN
   ========================================================= */

IF OBJECT_ID(N'[app].[PayrollRun]', N'U') IS NULL
BEGIN
    CREATE TABLE [app].[PayrollRun](
        [PayrollRunId]      [bigint] IDENTITY(1,1) NOT NULL,
        [PayrollRunGuid]    [uniqueidentifier] NOT NULL,
        [PayrollRunCode]    [varchar](30) NOT NULL,
        [CompanyId]         [bigint] NOT NULL,
        [PeriodYear]        [int] NOT NULL,
        [PeriodMonth]       [int] NOT NULL,
        [PeriodStart]       [date] NOT NULL,
        [PeriodEnd]         [date] NOT NULL,
        [CurrencyCode]      [char](3) NOT NULL,
        [RunStatus]         [varchar](20) NOT NULL,  -- Draft | Validated | Calculated | Posted | Cancelled
        [ValidateOnly]      [bit] NOT NULL,
        [EmployeeCount]     [int] NOT NULL,
        [SuccessCount]      [int] NOT NULL,
        [WarningCount]      [int] NOT NULL,
        [FailedCount]       [int] NOT NULL,
        [TotalGross]        [decimal](18,2) NOT NULL,
        [TotalAllowance]    [decimal](18,2) NOT NULL,
        [TotalDeduction]    [decimal](18,2) NOT NULL,
        [TotalTax]          [decimal](18,2) NOT NULL,
        [TotalNet]          [decimal](18,2) NOT NULL,
        [Note]              [nvarchar](1000) NULL,
        [ProcessExecutionGuid] [uniqueidentifier] NULL,
        [StatusId]          [int] NOT NULL,
        [RowVersion]        [timestamp] NOT NULL,
        [CreatedById]       [bigint] NOT NULL,
        [CreatedTime]       [datetime2](7) NOT NULL,
        [UpdatedById]       [bigint] NULL,
        [UpdatedTime]       [datetime2](7) NULL,
        [DeletedById]       [bigint] NULL,
        [DeletedTime]       [datetime2](7) NULL,
     CONSTRAINT [PK_PayrollRun] PRIMARY KEY CLUSTERED ([PayrollRunId] ASC),
     CONSTRAINT [UQ_PayrollRun_Guid] UNIQUE NONCLUSTERED ([PayrollRunGuid] ASC),
     CONSTRAINT [UQ_PayrollRun_Code] UNIQUE NONCLUSTERED ([PayrollRunCode] ASC)
    ) ON [PRIMARY];

    ALTER TABLE [app].[PayrollRun] ADD CONSTRAINT [DF_PayrollRun_Guid] DEFAULT (NEWID()) FOR [PayrollRunGuid];
    ALTER TABLE [app].[PayrollRun] ADD CONSTRAINT [DF_PayrollRun_Currency] DEFAULT ('IDR') FOR [CurrencyCode];
    ALTER TABLE [app].[PayrollRun] ADD CONSTRAINT [DF_PayrollRun_RunStatus] DEFAULT ('Draft') FOR [RunStatus];
    ALTER TABLE [app].[PayrollRun] ADD CONSTRAINT [DF_PayrollRun_ValidateOnly] DEFAULT ((0)) FOR [ValidateOnly];
    ALTER TABLE [app].[PayrollRun] ADD CONSTRAINT [DF_PayrollRun_EmployeeCount] DEFAULT ((0)) FOR [EmployeeCount];
    ALTER TABLE [app].[PayrollRun] ADD CONSTRAINT [DF_PayrollRun_SuccessCount] DEFAULT ((0)) FOR [SuccessCount];
    ALTER TABLE [app].[PayrollRun] ADD CONSTRAINT [DF_PayrollRun_WarningCount] DEFAULT ((0)) FOR [WarningCount];
    ALTER TABLE [app].[PayrollRun] ADD CONSTRAINT [DF_PayrollRun_FailedCount] DEFAULT ((0)) FOR [FailedCount];
    ALTER TABLE [app].[PayrollRun] ADD CONSTRAINT [DF_PayrollRun_TotalGross] DEFAULT ((0)) FOR [TotalGross];
    ALTER TABLE [app].[PayrollRun] ADD CONSTRAINT [DF_PayrollRun_TotalAllowance] DEFAULT ((0)) FOR [TotalAllowance];
    ALTER TABLE [app].[PayrollRun] ADD CONSTRAINT [DF_PayrollRun_TotalDeduction] DEFAULT ((0)) FOR [TotalDeduction];
    ALTER TABLE [app].[PayrollRun] ADD CONSTRAINT [DF_PayrollRun_TotalTax] DEFAULT ((0)) FOR [TotalTax];
    ALTER TABLE [app].[PayrollRun] ADD CONSTRAINT [DF_PayrollRun_TotalNet] DEFAULT ((0)) FOR [TotalNet];
    ALTER TABLE [app].[PayrollRun] ADD CONSTRAINT [DF_PayrollRun_Status] DEFAULT ((1)) FOR [StatusId];
    ALTER TABLE [app].[PayrollRun] ADD CONSTRAINT [DF_PayrollRun_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime];

    ALTER TABLE [app].[PayrollRun] WITH CHECK ADD CONSTRAINT [CHK_PayrollRun_Status]
        CHECK ([StatusId] IN (0, 1, 2));
    ALTER TABLE [app].[PayrollRun] WITH CHECK ADD CONSTRAINT [CHK_PayrollRun_PeriodMonth]
        CHECK ([PeriodMonth] BETWEEN 1 AND 12);
    ALTER TABLE [app].[PayrollRun] WITH CHECK ADD CONSTRAINT [CHK_PayrollRun_PeriodYear]
        CHECK ([PeriodYear] BETWEEN 2000 AND 2100);
    ALTER TABLE [app].[PayrollRun] WITH CHECK ADD CONSTRAINT [CHK_PayrollRun_RunStatus]
        CHECK ([RunStatus] IN ('Draft', 'Validated', 'Calculated', 'Posted', 'Cancelled'));

    CREATE UNIQUE NONCLUSTERED INDEX [UX_PayrollRun_Company_Period]
        ON [app].[PayrollRun]([CompanyId], [PeriodYear], [PeriodMonth])
        WHERE [DeletedTime] IS NULL AND [RunStatus] <> 'Cancelled';

    CREATE NONCLUSTERED INDEX [IX_PayrollRun_Company_Status]
        ON [app].[PayrollRun]([CompanyId], [RunStatus])
        WHERE [DeletedTime] IS NULL;

    PRINT 'Created [app].[PayrollRun].';
END
ELSE
    PRINT '[app].[PayrollRun] already exists.';
GO

/* =========================================================
   TABLE: app.PayrollRunDetail  (Detail — employee pay lines)
   ========================================================= */

IF OBJECT_ID(N'[app].[PayrollRunDetail]', N'U') IS NULL
BEGIN
    CREATE TABLE [app].[PayrollRunDetail](
        [PayrollRunDetailId]    [bigint] IDENTITY(1,1) NOT NULL,
        [PayrollRunDetailGuid]  [uniqueidentifier] NOT NULL,
        [PayrollRunId]          [bigint] NOT NULL,
        [LineNo]                [int] NOT NULL,
        [EmployeeId]            [bigint] NULL,
        [EmployeeNo]            [nvarchar](30) NOT NULL,
        [EmployeeName]          [nvarchar](200) NOT NULL,
        [BaseSalary]            [decimal](18,2) NOT NULL,
        [AllowanceAmount]       [decimal](18,2) NOT NULL,
        [OvertimeAmount]        [decimal](18,2) NOT NULL,
        [DeductionAmount]       [decimal](18,2) NOT NULL,
        [TaxAmount]             [decimal](18,2) NOT NULL,
        [GrossPay]              [decimal](18,2) NOT NULL,
        [NetPay]                [decimal](18,2) NOT NULL,
        [LineStatus]            [varchar](20) NOT NULL, -- OK | Warning | Error | Skipped
        [Message]               [nvarchar](500) NULL,
        [StatusId]              [int] NOT NULL,
        [RowVersion]            [timestamp] NOT NULL,
        [CreatedById]           [bigint] NOT NULL,
        [CreatedTime]           [datetime2](7) NOT NULL,
        [UpdatedById]           [bigint] NULL,
        [UpdatedTime]           [datetime2](7) NULL,
        [DeletedById]           [bigint] NULL,
        [DeletedTime]           [datetime2](7) NULL,
     CONSTRAINT [PK_PayrollRunDetail] PRIMARY KEY CLUSTERED ([PayrollRunDetailId] ASC),
     CONSTRAINT [UQ_PayrollRunDetail_Guid] UNIQUE NONCLUSTERED ([PayrollRunDetailGuid] ASC),
     CONSTRAINT [FK_PayrollRunDetail_PayrollRun]
        FOREIGN KEY ([PayrollRunId]) REFERENCES [app].[PayrollRun]([PayrollRunId])
    ) ON [PRIMARY];

    ALTER TABLE [app].[PayrollRunDetail] ADD CONSTRAINT [DF_PayrollRunDetail_Guid] DEFAULT (NEWID()) FOR [PayrollRunDetailGuid];
    ALTER TABLE [app].[PayrollRunDetail] ADD CONSTRAINT [DF_PayrollRunDetail_BaseSalary] DEFAULT ((0)) FOR [BaseSalary];
    ALTER TABLE [app].[PayrollRunDetail] ADD CONSTRAINT [DF_PayrollRunDetail_Allowance] DEFAULT ((0)) FOR [AllowanceAmount];
    ALTER TABLE [app].[PayrollRunDetail] ADD CONSTRAINT [DF_PayrollRunDetail_Overtime] DEFAULT ((0)) FOR [OvertimeAmount];
    ALTER TABLE [app].[PayrollRunDetail] ADD CONSTRAINT [DF_PayrollRunDetail_Deduction] DEFAULT ((0)) FOR [DeductionAmount];
    ALTER TABLE [app].[PayrollRunDetail] ADD CONSTRAINT [DF_PayrollRunDetail_Tax] DEFAULT ((0)) FOR [TaxAmount];
    ALTER TABLE [app].[PayrollRunDetail] ADD CONSTRAINT [DF_PayrollRunDetail_Gross] DEFAULT ((0)) FOR [GrossPay];
    ALTER TABLE [app].[PayrollRunDetail] ADD CONSTRAINT [DF_PayrollRunDetail_Net] DEFAULT ((0)) FOR [NetPay];
    ALTER TABLE [app].[PayrollRunDetail] ADD CONSTRAINT [DF_PayrollRunDetail_LineStatus] DEFAULT ('OK') FOR [LineStatus];
    ALTER TABLE [app].[PayrollRunDetail] ADD CONSTRAINT [DF_PayrollRunDetail_Status] DEFAULT ((1)) FOR [StatusId];
    ALTER TABLE [app].[PayrollRunDetail] ADD CONSTRAINT [DF_PayrollRunDetail_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime];

    ALTER TABLE [app].[PayrollRunDetail] WITH CHECK ADD CONSTRAINT [CHK_PayrollRunDetail_Status]
        CHECK ([StatusId] IN (0, 1, 2));
    ALTER TABLE [app].[PayrollRunDetail] WITH CHECK ADD CONSTRAINT [CHK_PayrollRunDetail_LineStatus]
        CHECK ([LineStatus] IN ('OK', 'Warning', 'Error', 'Skipped'));

    CREATE UNIQUE NONCLUSTERED INDEX [UX_PayrollRunDetail_Run_Line]
        ON [app].[PayrollRunDetail]([PayrollRunId], [LineNo])
        WHERE [DeletedTime] IS NULL;

    CREATE NONCLUSTERED INDEX [IX_PayrollRunDetail_Run_Status]
        ON [app].[PayrollRunDetail]([PayrollRunId], [LineStatus])
        WHERE [DeletedTime] IS NULL;

    PRINT 'Created [app].[PayrollRunDetail].';
END
ELSE
    PRINT '[app].[PayrollRunDetail] already exists.';
GO
