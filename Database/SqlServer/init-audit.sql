-- Audit database schema — clone of BamboeUpAuditDB (log.* + rpt.ReportExecutionLog).
-- Placeholder {{AuditDatabaseName}} is replaced by SchemaStudio Create Solution.
-- Run after create-audit-db.sql against the audit database (or rely on USE below).
-- Do NOT run against the production / platform database.

USE [{{AuditDatabaseName}}];
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'log')
    EXEC(N'CREATE SCHEMA [log]');
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'rpt')
    EXEC(N'CREATE SCHEMA [rpt]');
GO

IF OBJECT_ID(N'[log].[AuditSnapshot]', N'U') IS NULL
BEGIN
    CREATE TABLE [log].[AuditSnapshot] (
        [SnapshotId]       BIGINT         IDENTITY(1,1) NOT NULL,
        [TableName]        VARCHAR(100)   NOT NULL,
        [EntityKey]        VARCHAR(500)   NOT NULL,
        [SnapshotJson]     NVARCHAR(MAX)  NOT NULL,
        [SnapshotDateTime] DATETIME2(3)   NOT NULL,
        CONSTRAINT [PK_AuditSnapshot] PRIMARY KEY CLUSTERED ([SnapshotId] ASC)
    );
END
GO

IF OBJECT_ID(N'[log].[AuditSession]', N'U') IS NULL
BEGIN
    CREATE TABLE [log].[AuditSession] (
        [AuditSessionId]  BIGINT           IDENTITY(1,1) NOT NULL,
        [SessionGuid]     UNIQUEIDENTIFIER NOT NULL,
        [SessionType]     CHAR(1)          NOT NULL,
        [RootTableName]   VARCHAR(100)     NOT NULL,
        [RootEntityKey]   VARCHAR(500)     NOT NULL,
        [RootDisplayName] VARCHAR(500)     NULL,
        [ActionByUserId]  VARCHAR(40)      NOT NULL,
        [LogDateTime]     DATETIME2(3)     NOT NULL,
        CONSTRAINT [PK_AuditSession] PRIMARY KEY CLUSTERED ([AuditSessionId] ASC)
    );
END
GO

IF OBJECT_ID(N'[log].[AuditLog]', N'U') IS NULL
BEGIN
    CREATE TABLE [log].[AuditLog] (
        [AuditLogId]        BIGINT       IDENTITY(1,1) NOT NULL,
        [AuditSessionId]    BIGINT       NOT NULL,
        [TableName]         VARCHAR(100) NOT NULL,
        [EntityKey]         VARCHAR(500) NOT NULL,
        [EntityDisplayName] VARCHAR(500) NULL,
        [ParentTableName]   VARCHAR(100) NULL,
        [ParentEntityKey]   VARCHAR(500) NULL,
        [AuditActionType]   CHAR(1)      NOT NULL,
        [ActionByUserId]    VARCHAR(40)  NOT NULL,
        [LogDateTime]       DATETIME2(3) NOT NULL,
        [SnapshotId]        BIGINT       NULL,
        CONSTRAINT [PK_AuditLog] PRIMARY KEY CLUSTERED ([AuditLogId] ASC),
        CONSTRAINT [FK_AuditLog_Session] FOREIGN KEY ([AuditSessionId])
            REFERENCES [log].[AuditSession]([AuditSessionId])
    );

    CREATE NONCLUSTERED INDEX [IX_AuditLog_Session]
        ON [log].[AuditLog]([AuditSessionId]);
    CREATE NONCLUSTERED INDEX [IX_AuditLog_Date]
        ON [log].[AuditLog]([LogDateTime] DESC);
    CREATE NONCLUSTERED INDEX [IX_AuditLog_Entity]
        ON [log].[AuditLog]([TableName], [EntityKey], [LogDateTime] DESC);
    CREATE NONCLUSTERED INDEX [IX_AuditLog_Parent]
        ON [log].[AuditLog]([ParentTableName], [ParentEntityKey], [LogDateTime] DESC);
    CREATE NONCLUSTERED INDEX [IX_AuditLog_User]
        ON [log].[AuditLog]([ActionByUserId], [LogDateTime] DESC);
END
GO

IF OBJECT_ID(N'[log].[AuditLogData]', N'U') IS NULL
BEGIN
    CREATE TABLE [log].[AuditLogData] (
        [AuditLogId] BIGINT        NOT NULL,
        [ColumnName] VARCHAR(100)  NOT NULL,
        [OldValue]   NVARCHAR(MAX) NULL,
        [NewValue]   NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_AuditLogData] PRIMARY KEY CLUSTERED ([AuditLogId] ASC, [ColumnName] ASC),
        CONSTRAINT [FK_AuditLogData_Log] FOREIGN KEY ([AuditLogId])
            REFERENCES [log].[AuditLog]([AuditLogId])
    );
END
GO

IF OBJECT_ID(N'[rpt].[ReportExecutionLog]', N'U') IS NULL
BEGIN
    CREATE TABLE [rpt].[ReportExecutionLog] (
        [ReportExecutionLogId] BIGINT           IDENTITY(1,1) NOT NULL,
        [ReportExecutionGuid]  UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT [DF_RptReportExecutionLog_Guid] DEFAULT (NEWID()),
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
        [IsReprint]            BIT              NOT NULL
            CONSTRAINT [DF_RptReportExecutionLog_IsReprint] DEFAULT (0),
        [ReprintOfPrintId]     NVARCHAR(50)     NULL,
        [ReprintReason]        NVARCHAR(500)    NULL,
        [SubjectKey]           NVARCHAR(200)    NULL,
        [Status]               NVARCHAR(20)     NOT NULL,
        [OutputFormat]         NVARCHAR(20)     NULL,
        [DurationMs]           INT              NULL,
        [ErrorMessage]         NVARCHAR(2000)   NULL,
        [CreatedTime]          DATETIME2(7)     NOT NULL
            CONSTRAINT [DF_RptReportExecutionLog_CreatedTime] DEFAULT (SYSDATETIME()),
        CONSTRAINT [PK_RptReportExecutionLog] PRIMARY KEY CLUSTERED ([ReportExecutionLogId] ASC)
    );

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

PRINT '{{AuditDatabaseName}} ready: log.Audit* + rpt.ReportExecutionLog';
GO
