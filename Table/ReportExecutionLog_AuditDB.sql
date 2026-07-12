USE [BamboeUpAuditDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'rpt')
    EXEC(N'CREATE SCHEMA [rpt]');
GO

IF OBJECT_ID(N'[rpt].[ReportExecutionLog]', N'U') IS NULL
BEGIN
    CREATE TABLE [rpt].[ReportExecutionLog](
        [ReportExecutionLogId] BIGINT           IDENTITY(1,1) NOT NULL,
        [ReportExecutionGuid]  UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_RptReportExecutionLog_Guid] DEFAULT (NEWID()),
        [ProgramId]            BIGINT           NOT NULL,
        [ProgramCode]          NVARCHAR(50)     NOT NULL,
        [ProgramName]          NVARCHAR(200)    NOT NULL,
        [ReportDefinitionId]   BIGINT           NULL,
        [UserId]               BIGINT           NOT NULL,
        [UserDisplayName]      NVARCHAR(200)    NULL,
        [CompanyId]            BIGINT           NULL,
        [CompanyOfficeId]      BIGINT           NULL,
        [ReportKind]           NVARCHAR(5)      NOT NULL,
        [ParametersJson]       NVARCHAR(MAX)    NULL,
        [ReportPrintId]        NVARCHAR(50)     NULL,
        [ReportPrintIdMasked]  NVARCHAR(50)     NULL,
        [IsReprint]            BIT              NOT NULL CONSTRAINT [DF_RptReportExecutionLog_IsReprint] DEFAULT (0),
        [ReprintOfPrintId]     NVARCHAR(50)     NULL,
        [ReprintReason]        NVARCHAR(500)    NULL,
        [SubjectKey]           NVARCHAR(200)    NULL,
        [Status]               NVARCHAR(20)     NOT NULL,
        [OutputFormat]         NVARCHAR(20)     NULL,
        [DurationMs]           INT              NULL,
        [ErrorMessage]         NVARCHAR(2000)   NULL,
        [CreatedTime]          DATETIME2(7)     NOT NULL CONSTRAINT [DF_RptReportExecutionLog_CreatedTime] DEFAULT (SYSDATETIME()),
        CONSTRAINT [PK_RptReportExecutionLog] PRIMARY KEY CLUSTERED ([ReportExecutionLogId] ASC)
    ) ON [PRIMARY];

    CREATE UNIQUE NONCLUSTERED INDEX [UX_RptReportExecutionLog_PrintId]
        ON [rpt].[ReportExecutionLog]([ReportPrintId])
        WHERE [ReportPrintId] IS NOT NULL;

    CREATE NONCLUSTERED INDEX [IX_RptReportExecutionLog_UserCreated]
        ON [rpt].[ReportExecutionLog]([UserId], [CreatedTime] DESC);

    CREATE NONCLUSTERED INDEX [IX_RptReportExecutionLog_ProgramCreated]
        ON [rpt].[ReportExecutionLog]([ProgramId], [CreatedTime] DESC);

    CREATE NONCLUSTERED INDEX [IX_RptReportExecutionLog_CompanyCreated]
        ON [rpt].[ReportExecutionLog]([CompanyId], [CompanyOfficeId], [CreatedTime] DESC);
END
GO

PRINT 'Report execution log table ready in BamboeUpAuditDB (rpt.ReportExecutionLog).';
GO
