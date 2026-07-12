USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[core].[ReportDefinition]', N'U') IS NULL
BEGIN
    CREATE TABLE [core].[ReportDefinition](
        [ReportDefinitionId]   BIGINT           IDENTITY(1,1) NOT NULL,
        [ReportDefinitionGuid] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_ReportDefinition_Guid] DEFAULT (NEWID()),
        [ProgramId]            BIGINT           NOT NULL,
        [ReportScope]          NVARCHAR(20)     NOT NULL CONSTRAINT [DF_ReportDefinition_Scope] DEFAULT (N'Standard'),
        [CompanyId]            BIGINT           NULL,
        [ReportKind]           NVARCHAR(5)      NOT NULL,
        [DefinitionKey]        NVARCHAR(100)    NOT NULL,
        [FilePath]             NVARCHAR(500)    NULL,
        [StoreProcedureName]   NVARCHAR(200)    NULL,
        [IsTracked]            BIT              NOT NULL CONSTRAINT [DF_ReportDefinition_IsTracked] DEFAULT (0),
        [RequiresPrintId]      BIT              NOT NULL CONSTRAINT [DF_ReportDefinition_RequiresPrintId] DEFAULT (0),
        [PrintIdPolicy]        NVARCHAR(30)     NULL,
        [PrintIdPrefix]        NVARCHAR(10)     NULL,
        [Version]              INT              NOT NULL CONSTRAINT [DF_ReportDefinition_Version] DEFAULT (1),
        [IsActive]             BIT              NOT NULL CONSTRAINT [DF_ReportDefinition_IsActive] DEFAULT (1),
        [EffectiveFrom]        DATETIME2(7)     NULL,
        [EffectiveTo]          DATETIME2(7)     NULL,
        [StatusId]             INT              NOT NULL CONSTRAINT [DF_ReportDefinition_StatusId] DEFAULT (1),
        [RowVersion]           TIMESTAMP        NOT NULL,
        [CreatedById]          BIGINT           NOT NULL,
        [CreatedTime]          DATETIME2(7)     NOT NULL CONSTRAINT [DF_ReportDefinition_CreatedTime] DEFAULT (SYSDATETIME()),
        [UpdatedById]          BIGINT           NULL,
        [UpdatedTime]          DATETIME2(7)     NULL,
        CONSTRAINT [PK_ReportDefinition] PRIMARY KEY CLUSTERED ([ReportDefinitionId] ASC),
        CONSTRAINT [UQ_ReportDefinition_Guid] UNIQUE NONCLUSTERED ([ReportDefinitionGuid] ASC),
        CONSTRAINT [CK_ReportDefinition_PrintIdRequiresTracked] CHECK ([RequiresPrintId] = 0 OR [IsTracked] = 1)
    ) ON [PRIMARY];
END
GO

IF OBJECT_ID(N'[core].[ReportParameter]', N'U') IS NULL
BEGIN
    CREATE TABLE [core].[ReportParameter](
        [ReportParameterId]    BIGINT           IDENTITY(1,1) NOT NULL,
        [ReportDefinitionId]   BIGINT           NOT NULL,
        [ParameterName]        NVARCHAR(100)    NOT NULL,
        [DisplayLabel]         NVARCHAR(200)    NOT NULL,
        [DataType]             NVARCHAR(30)     NOT NULL,
        [IsRequired]           BIT              NOT NULL CONSTRAINT [DF_ReportParameter_IsRequired] DEFAULT (0),
        [DefaultValue]         NVARCHAR(500)    NULL,
        [SortOrder]            INT              NOT NULL CONSTRAINT [DF_ReportParameter_SortOrder] DEFAULT (0),
        [LookupType]           NVARCHAR(50)     NULL,
        [IsSensitive]          BIT              NOT NULL CONSTRAINT [DF_ReportParameter_IsSensitive] DEFAULT (0),
        [StatusId]             INT              NOT NULL CONSTRAINT [DF_ReportParameter_StatusId] DEFAULT (1),
        [CreatedById]          BIGINT           NOT NULL,
        [CreatedTime]          DATETIME2(7)     NOT NULL CONSTRAINT [DF_ReportParameter_CreatedTime] DEFAULT (SYSDATETIME()),
        CONSTRAINT [PK_ReportParameter] PRIMARY KEY CLUSTERED ([ReportParameterId] ASC)
    ) ON [PRIMARY];
END
GO

IF OBJECT_ID(N'[core].[ReportExecutionLog]', N'U') IS NULL
BEGIN
    -- Migrated to BamboeUpAuditDB: see ReportExecutionLog_AuditDB.sql ([rpt].[ReportExecutionLog])
    -- Legacy table kept for existing deployments; new installs should run audit DB script instead.
    CREATE TABLE [core].[ReportExecutionLog](
        [ReportExecutionLogId] BIGINT           IDENTITY(1,1) NOT NULL,
        [ReportExecutionGuid]  UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_ReportExecutionLog_Guid] DEFAULT (NEWID()),
        [ProgramId]            BIGINT           NOT NULL,
        [ReportDefinitionId]   BIGINT           NULL,
        [UserId]               BIGINT           NOT NULL,
        [CompanyId]            BIGINT           NULL,
        [CompanyOfficeId]      BIGINT           NULL,
        [ReportKind]           NVARCHAR(5)      NOT NULL,
        [ParametersJson]       NVARCHAR(MAX)    NULL,
        [ReportPrintId]        NVARCHAR(50)     NULL,
        [ReportPrintIdMasked]  NVARCHAR(50)     NULL,
        [IsReprint]            BIT              NOT NULL CONSTRAINT [DF_ReportExecutionLog_IsReprint] DEFAULT (0),
        [ReprintOfPrintId]     NVARCHAR(50)     NULL,
        [ReprintReason]        NVARCHAR(500)    NULL,
        [SubjectKey]           NVARCHAR(200)    NULL,
        [Status]               NVARCHAR(20)     NOT NULL,
        [OutputFormat]         NVARCHAR(20)     NULL,
        [DurationMs]           INT              NULL,
        [ErrorMessage]         NVARCHAR(2000)   NULL,
        [CreatedTime]          DATETIME2(7)     NOT NULL CONSTRAINT [DF_ReportExecutionLog_CreatedTime] DEFAULT (SYSDATETIME()),
        CONSTRAINT [PK_ReportExecutionLog] PRIMARY KEY CLUSTERED ([ReportExecutionLogId] ASC)
    ) ON [PRIMARY];

    CREATE UNIQUE NONCLUSTERED INDEX [UX_ReportExecutionLog_PrintId]
        ON [core].[ReportExecutionLog]([ReportPrintId])
        WHERE [ReportPrintId] IS NOT NULL;

    CREATE NONCLUSTERED INDEX [IX_ReportExecutionLog_UserCreated]
        ON [core].[ReportExecutionLog]([UserId], [CreatedTime] DESC);

    CREATE NONCLUSTERED INDEX [IX_ReportExecutionLog_ProgramCreated]
        ON [core].[ReportExecutionLog]([ProgramId], [CreatedTime] DESC);
END
GO
