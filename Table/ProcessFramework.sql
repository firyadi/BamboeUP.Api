USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================================
-- Process Framework (UI-07) — schema [core]
-- MVP: 8 core tables + ResultDetail, Approval, Audit (Recommended)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'core')
    EXEC('CREATE SCHEMA [core]');
GO

-- ---------------------------------------------------------------------------
-- 1. ProcessDefinition
-- ---------------------------------------------------------------------------
IF OBJECT_ID(N'[core].[ProcessDefinition]', N'U') IS NULL
BEGIN
    CREATE TABLE [core].[ProcessDefinition]
    (
        [ProcessDefinitionId]   BIGINT IDENTITY(1,1) NOT NULL,
        [ProcessDefinitionGuid] UNIQUEIDENTIFIER NOT NULL,
        [ProcessCode]           VARCHAR(50) NOT NULL,
        [ProcessName]           NVARCHAR(200) NOT NULL,
        [Description]           NVARCHAR(MAX) NULL,
        [Category]              VARCHAR(50) NULL,
        [Version]               INT NOT NULL,
        [ExecutionMode]         TINYINT NOT NULL,
        [RequiresApproval]      BIT NOT NULL,
        [AllowRetry]            BIT NOT NULL,
        [AllowCancel]           BIT NOT NULL,
        [AllowSchedule]         BIT NOT NULL,
        [IsActive]              BIT NOT NULL,
        [StatusId]              INT NOT NULL,
        [RowVersion]            ROWVERSION NOT NULL,
        [CreatedById]           BIGINT NOT NULL,
        [CreatedTime]           DATETIME2 NOT NULL,
        [UpdatedById]           BIGINT NULL,
        [UpdatedTime]           DATETIME2 NULL,
        [DeletedById]           BIGINT NULL,
        [DeletedTime]           DATETIME2 NULL,
        CONSTRAINT [PK_ProcessDefinition] PRIMARY KEY CLUSTERED ([ProcessDefinitionId] ASC)
    );

    ALTER TABLE [core].[ProcessDefinition] ADD CONSTRAINT [DF_ProcessDefinition_Guid] DEFAULT (NEWID()) FOR [ProcessDefinitionGuid];
    ALTER TABLE [core].[ProcessDefinition] ADD CONSTRAINT [DF_ProcessDefinition_Version] DEFAULT ((1)) FOR [Version];
    ALTER TABLE [core].[ProcessDefinition] ADD CONSTRAINT [DF_ProcessDefinition_ExecutionMode] DEFAULT ((2)) FOR [ExecutionMode];
    ALTER TABLE [core].[ProcessDefinition] ADD CONSTRAINT [DF_ProcessDefinition_RequiresApproval] DEFAULT ((0)) FOR [RequiresApproval];
    ALTER TABLE [core].[ProcessDefinition] ADD CONSTRAINT [DF_ProcessDefinition_AllowRetry] DEFAULT ((1)) FOR [AllowRetry];
    ALTER TABLE [core].[ProcessDefinition] ADD CONSTRAINT [DF_ProcessDefinition_AllowCancel] DEFAULT ((1)) FOR [AllowCancel];
    ALTER TABLE [core].[ProcessDefinition] ADD CONSTRAINT [DF_ProcessDefinition_AllowSchedule] DEFAULT ((0)) FOR [AllowSchedule];
    ALTER TABLE [core].[ProcessDefinition] ADD CONSTRAINT [DF_ProcessDefinition_IsActive] DEFAULT ((1)) FOR [IsActive];
    ALTER TABLE [core].[ProcessDefinition] ADD CONSTRAINT [DF_ProcessDefinition_StatusId] DEFAULT ((1)) FOR [StatusId];
    ALTER TABLE [core].[ProcessDefinition] ADD CONSTRAINT [DF_ProcessDefinition_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime];

    CREATE UNIQUE NONCLUSTERED INDEX [UX_ProcessDefinition_ProcessCode]
        ON [core].[ProcessDefinition]([ProcessCode]) WHERE [DeletedTime] IS NULL;

    CREATE NONCLUSTERED INDEX [IX_ProcessDefinition_IsActive]
        ON [core].[ProcessDefinition]([IsActive], [Category]) WHERE [DeletedTime] IS NULL;
END
GO

-- ---------------------------------------------------------------------------
-- 2. ProcessParameterDefinition
-- ---------------------------------------------------------------------------
IF OBJECT_ID(N'[core].[ProcessParameterDefinition]', N'U') IS NULL
BEGIN
    CREATE TABLE [core].[ProcessParameterDefinition]
    (
        [ProcessParameterDefinitionId] BIGINT IDENTITY(1,1) NOT NULL,
        [ProcessParameterDefinitionGuid] UNIQUEIDENTIFIER NOT NULL,
        [ProcessDefinitionId]       BIGINT NOT NULL,
        [ParameterCode]             VARCHAR(50) NOT NULL,
        [ParameterName]             NVARCHAR(200) NOT NULL,
        [DataType]                  VARCHAR(30) NOT NULL,
        [ControlType]               VARCHAR(30) NOT NULL,
        [IsRequired]                BIT NOT NULL,
        [DefaultValue]              NVARCHAR(500) NULL,
        [ValidationRuleJson]        NVARCHAR(MAX) NULL,
        [LookupSource]              NVARCHAR(200) NULL,
        [DisplayOrder]              INT NOT NULL,
        [GroupName]                 NVARCHAR(100) NULL,
        [IsVisible]                 BIT NOT NULL,
        [StatusId]                  INT NOT NULL,
        [CreatedById]               BIGINT NOT NULL,
        [CreatedTime]               DATETIME2 NOT NULL,
        [UpdatedById]               BIGINT NULL,
        [UpdatedTime]               DATETIME2 NULL,
        CONSTRAINT [PK_ProcessParameterDefinition] PRIMARY KEY CLUSTERED ([ProcessParameterDefinitionId] ASC),
        CONSTRAINT [FK_ProcessParameterDefinition_ProcessDefinition]
            FOREIGN KEY ([ProcessDefinitionId]) REFERENCES [core].[ProcessDefinition]([ProcessDefinitionId])
    );

    ALTER TABLE [core].[ProcessParameterDefinition] ADD CONSTRAINT [DF_ProcessParameterDefinition_Guid] DEFAULT (NEWID()) FOR [ProcessParameterDefinitionGuid];
    ALTER TABLE [core].[ProcessParameterDefinition] ADD CONSTRAINT [DF_ProcessParameterDefinition_IsRequired] DEFAULT ((0)) FOR [IsRequired];
    ALTER TABLE [core].[ProcessParameterDefinition] ADD CONSTRAINT [DF_ProcessParameterDefinition_DisplayOrder] DEFAULT ((0)) FOR [DisplayOrder];
    ALTER TABLE [core].[ProcessParameterDefinition] ADD CONSTRAINT [DF_ProcessParameterDefinition_IsVisible] DEFAULT ((1)) FOR [IsVisible];
    ALTER TABLE [core].[ProcessParameterDefinition] ADD CONSTRAINT [DF_ProcessParameterDefinition_StatusId] DEFAULT ((1)) FOR [StatusId];
    ALTER TABLE [core].[ProcessParameterDefinition] ADD CONSTRAINT [DF_ProcessParameterDefinition_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime];

    CREATE UNIQUE NONCLUSTERED INDEX [UX_ProcessParameterDefinition_Code]
        ON [core].[ProcessParameterDefinition]([ProcessDefinitionId], [ParameterCode]);
END
GO

-- ---------------------------------------------------------------------------
-- 3. ProcessStepDefinition
-- ---------------------------------------------------------------------------
IF OBJECT_ID(N'[core].[ProcessStepDefinition]', N'U') IS NULL
BEGIN
    CREATE TABLE [core].[ProcessStepDefinition]
    (
        [ProcessStepDefinitionId]   BIGINT IDENTITY(1,1) NOT NULL,
        [ProcessStepDefinitionGuid] UNIQUEIDENTIFIER NOT NULL,
        [ProcessDefinitionId]       BIGINT NOT NULL,
        [StepNo]                    INT NOT NULL,
        [StepCode]                  VARCHAR(50) NOT NULL,
        [StepName]                  NVARCHAR(200) NOT NULL,
        [StepType]                  VARCHAR(30) NOT NULL,
        [IsMandatory]               BIT NOT NULL,
        [AllowRetry]                BIT NOT NULL,
        [AllowSkip]                 BIT NOT NULL,
        [ExecutionOrder]            INT NOT NULL,
        [ConfigurationJson]         NVARCHAR(MAX) NULL,
        [StatusId]                  INT NOT NULL,
        [CreatedById]               BIGINT NOT NULL,
        [CreatedTime]               DATETIME2 NOT NULL,
        [UpdatedById]               BIGINT NULL,
        [UpdatedTime]               DATETIME2 NULL,
        CONSTRAINT [PK_ProcessStepDefinition] PRIMARY KEY CLUSTERED ([ProcessStepDefinitionId] ASC),
        CONSTRAINT [FK_ProcessStepDefinition_ProcessDefinition]
            FOREIGN KEY ([ProcessDefinitionId]) REFERENCES [core].[ProcessDefinition]([ProcessDefinitionId])
    );

    ALTER TABLE [core].[ProcessStepDefinition] ADD CONSTRAINT [DF_ProcessStepDefinition_Guid] DEFAULT (NEWID()) FOR [ProcessStepDefinitionGuid];
    ALTER TABLE [core].[ProcessStepDefinition] ADD CONSTRAINT [DF_ProcessStepDefinition_IsMandatory] DEFAULT ((1)) FOR [IsMandatory];
    ALTER TABLE [core].[ProcessStepDefinition] ADD CONSTRAINT [DF_ProcessStepDefinition_AllowRetry] DEFAULT ((1)) FOR [AllowRetry];
    ALTER TABLE [core].[ProcessStepDefinition] ADD CONSTRAINT [DF_ProcessStepDefinition_AllowSkip] DEFAULT ((0)) FOR [AllowSkip];
    ALTER TABLE [core].[ProcessStepDefinition] ADD CONSTRAINT [DF_ProcessStepDefinition_StatusId] DEFAULT ((1)) FOR [StatusId];
    ALTER TABLE [core].[ProcessStepDefinition] ADD CONSTRAINT [DF_ProcessStepDefinition_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime];

    CREATE UNIQUE NONCLUSTERED INDEX [UX_ProcessStepDefinition_Code]
        ON [core].[ProcessStepDefinition]([ProcessDefinitionId], [StepCode]);
END
GO

-- ---------------------------------------------------------------------------
-- 4. ProcessExecution
-- ---------------------------------------------------------------------------
IF OBJECT_ID(N'[core].[ProcessExecution]', N'U') IS NULL
BEGIN
    CREATE TABLE [core].[ProcessExecution]
    (
        [ProcessExecutionId]        BIGINT IDENTITY(1,1) NOT NULL,
        [ProcessExecutionGuid]      UNIQUEIDENTIFIER NOT NULL,
        [ProcessDefinitionId]       BIGINT NOT NULL,
        [ExecutionNo]               VARCHAR(30) NOT NULL,
        [Status]                    VARCHAR(30) NOT NULL,
        [StartedAt]                 DATETIME2 NULL,
        [CompletedAt]               DATETIME2 NULL,
        [StartedById]               BIGINT NOT NULL,
        [ProgressPercent]           DECIMAL(5,2) NOT NULL,
        [CurrentStepCode]           VARCHAR(50) NULL,
        [ErrorCount]                INT NOT NULL,
        [WarningCount]              INT NOT NULL,
        [CorrelationId]             UNIQUEIDENTIFIER NOT NULL,
        [CompanyId]                 BIGINT NULL,
        [CompanyOfficeId]           BIGINT NULL,
        [ParentExecutionId]         BIGINT NULL,
        [CancellationRequestedAt]   DATETIME2 NULL,
        [RowVersion]                ROWVERSION NOT NULL,
        [CreatedById]               BIGINT NOT NULL,
        [CreatedTime]               DATETIME2 NOT NULL,
        CONSTRAINT [PK_ProcessExecution] PRIMARY KEY CLUSTERED ([ProcessExecutionId] ASC),
        CONSTRAINT [FK_ProcessExecution_ProcessDefinition]
            FOREIGN KEY ([ProcessDefinitionId]) REFERENCES [core].[ProcessDefinition]([ProcessDefinitionId]),
        CONSTRAINT [FK_ProcessExecution_Parent]
            FOREIGN KEY ([ParentExecutionId]) REFERENCES [core].[ProcessExecution]([ProcessExecutionId])
    );

    ALTER TABLE [core].[ProcessExecution] ADD CONSTRAINT [DF_ProcessExecution_Guid] DEFAULT (NEWID()) FOR [ProcessExecutionGuid];
    ALTER TABLE [core].[ProcessExecution] ADD CONSTRAINT [DF_ProcessExecution_Status] DEFAULT ('Draft') FOR [Status];
    ALTER TABLE [core].[ProcessExecution] ADD CONSTRAINT [DF_ProcessExecution_ProgressPercent] DEFAULT ((0)) FOR [ProgressPercent];
    ALTER TABLE [core].[ProcessExecution] ADD CONSTRAINT [DF_ProcessExecution_ErrorCount] DEFAULT ((0)) FOR [ErrorCount];
    ALTER TABLE [core].[ProcessExecution] ADD CONSTRAINT [DF_ProcessExecution_WarningCount] DEFAULT ((0)) FOR [WarningCount];
    ALTER TABLE [core].[ProcessExecution] ADD CONSTRAINT [DF_ProcessExecution_CorrelationId] DEFAULT (NEWID()) FOR [CorrelationId];
    ALTER TABLE [core].[ProcessExecution] ADD CONSTRAINT [DF_ProcessExecution_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime];

    CREATE UNIQUE NONCLUSTERED INDEX [UX_ProcessExecution_Guid] ON [core].[ProcessExecution]([ProcessExecutionGuid]);
    CREATE UNIQUE NONCLUSTERED INDEX [UX_ProcessExecution_ExecutionNo] ON [core].[ProcessExecution]([ExecutionNo]);
    CREATE NONCLUSTERED INDEX [IX_ProcessExecution_Definition_Status] ON [core].[ProcessExecution]([ProcessDefinitionId], [Status]);
    CREATE NONCLUSTERED INDEX [IX_ProcessExecution_CorrelationId] ON [core].[ProcessExecution]([CorrelationId]);
    CREATE NONCLUSTERED INDEX [IX_ProcessExecution_StartedAt] ON [core].[ProcessExecution]([StartedAt] DESC);
END
GO

-- ---------------------------------------------------------------------------
-- 5. ProcessParameterValue
-- ---------------------------------------------------------------------------
IF OBJECT_ID(N'[core].[ProcessParameterValue]', N'U') IS NULL
BEGIN
    CREATE TABLE [core].[ProcessParameterValue]
    (
        [ProcessParameterValueId]   BIGINT IDENTITY(1,1) NOT NULL,
        [ProcessExecutionId]        BIGINT NOT NULL,
        [ParameterCode]             VARCHAR(50) NOT NULL,
        [ValueText]                 NVARCHAR(MAX) NULL,
        [ValueNumber]               DECIMAL(18,4) NULL,
        [ValueDate]                 DATETIME2 NULL,
        [ValueJson]                 NVARCHAR(MAX) NULL,
        [ValueBlob]                 VARBINARY(MAX) NULL,
        CONSTRAINT [PK_ProcessParameterValue] PRIMARY KEY CLUSTERED ([ProcessParameterValueId] ASC),
        CONSTRAINT [FK_ProcessParameterValue_ProcessExecution]
            FOREIGN KEY ([ProcessExecutionId]) REFERENCES [core].[ProcessExecution]([ProcessExecutionId])
    );

    CREATE UNIQUE NONCLUSTERED INDEX [UX_ProcessParameterValue_Code]
        ON [core].[ProcessParameterValue]([ProcessExecutionId], [ParameterCode]);
END
GO

-- ---------------------------------------------------------------------------
-- 6. ProcessStepExecution
-- ---------------------------------------------------------------------------
IF OBJECT_ID(N'[core].[ProcessStepExecution]', N'U') IS NULL
BEGIN
    CREATE TABLE [core].[ProcessStepExecution]
    (
        [ProcessStepExecutionId]    BIGINT IDENTITY(1,1) NOT NULL,
        [ProcessExecutionId]        BIGINT NOT NULL,
        [StepCode]                  VARCHAR(50) NOT NULL,
        [StepName]                  NVARCHAR(200) NOT NULL,
        [Status]                    VARCHAR(30) NOT NULL,
        [StartedAt]                 DATETIME2 NULL,
        [CompletedAt]               DATETIME2 NULL,
        [ProgressPercent]           DECIMAL(5,2) NOT NULL,
        [ProcessedCount]            INT NOT NULL,
        [SuccessCount]              INT NOT NULL,
        [WarningCount]              INT NOT NULL,
        [FailedCount]               INT NOT NULL,
        [Message]                   NVARCHAR(500) NULL,
        [ErrorCode]                 VARCHAR(50) NULL,
        [ErrorMessage]              NVARCHAR(MAX) NULL,
        [RetryCount]                INT NOT NULL,
        CONSTRAINT [PK_ProcessStepExecution] PRIMARY KEY CLUSTERED ([ProcessStepExecutionId] ASC),
        CONSTRAINT [FK_ProcessStepExecution_ProcessExecution]
            FOREIGN KEY ([ProcessExecutionId]) REFERENCES [core].[ProcessExecution]([ProcessExecutionId])
    );

    ALTER TABLE [core].[ProcessStepExecution] ADD CONSTRAINT [DF_ProcessStepExecution_Status] DEFAULT ('Pending') FOR [Status];
    ALTER TABLE [core].[ProcessStepExecution] ADD CONSTRAINT [DF_ProcessStepExecution_ProgressPercent] DEFAULT ((0)) FOR [ProgressPercent];
    ALTER TABLE [core].[ProcessStepExecution] ADD CONSTRAINT [DF_ProcessStepExecution_ProcessedCount] DEFAULT ((0)) FOR [ProcessedCount];
    ALTER TABLE [core].[ProcessStepExecution] ADD CONSTRAINT [DF_ProcessStepExecution_SuccessCount] DEFAULT ((0)) FOR [SuccessCount];
    ALTER TABLE [core].[ProcessStepExecution] ADD CONSTRAINT [DF_ProcessStepExecution_WarningCount] DEFAULT ((0)) FOR [WarningCount];
    ALTER TABLE [core].[ProcessStepExecution] ADD CONSTRAINT [DF_ProcessStepExecution_FailedCount] DEFAULT ((0)) FOR [FailedCount];
    ALTER TABLE [core].[ProcessStepExecution] ADD CONSTRAINT [DF_ProcessStepExecution_RetryCount] DEFAULT ((0)) FOR [RetryCount];

    CREATE UNIQUE NONCLUSTERED INDEX [UX_ProcessStepExecution_Code]
        ON [core].[ProcessStepExecution]([ProcessExecutionId], [StepCode]);
END
GO

-- ---------------------------------------------------------------------------
-- 7. ProcessMessage
-- ---------------------------------------------------------------------------
IF OBJECT_ID(N'[core].[ProcessMessage]', N'U') IS NULL
BEGIN
    CREATE TABLE [core].[ProcessMessage]
    (
        [ProcessMessageId]          BIGINT IDENTITY(1,1) NOT NULL,
        [ProcessExecutionId]        BIGINT NOT NULL,
        [StepCode]                  VARCHAR(50) NULL,
        [Severity]                  VARCHAR(20) NOT NULL,
        [Source]                    VARCHAR(20) NOT NULL,
        [Message]                   NVARCHAR(500) NOT NULL,
        [DetailJson]                NVARCHAR(MAX) NULL,
        [CreatedTime]               DATETIME2 NOT NULL,
        CONSTRAINT [PK_ProcessMessage] PRIMARY KEY CLUSTERED ([ProcessMessageId] ASC),
        CONSTRAINT [FK_ProcessMessage_ProcessExecution]
            FOREIGN KEY ([ProcessExecutionId]) REFERENCES [core].[ProcessExecution]([ProcessExecutionId])
    );

    ALTER TABLE [core].[ProcessMessage] ADD CONSTRAINT [DF_ProcessMessage_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime];

    CREATE NONCLUSTERED INDEX [IX_ProcessMessage_Execution]
        ON [core].[ProcessMessage]([ProcessExecutionId], [CreatedTime] DESC);
END
GO

-- ---------------------------------------------------------------------------
-- 8. ProcessResult
-- ---------------------------------------------------------------------------
IF OBJECT_ID(N'[core].[ProcessResult]', N'U') IS NULL
BEGIN
    CREATE TABLE [core].[ProcessResult]
    (
        [ProcessResultId]           BIGINT IDENTITY(1,1) NOT NULL,
        [ProcessExecutionId]        BIGINT NOT NULL,
        [ResultStatus]                VARCHAR(30) NOT NULL,
        [ResultMessage]               NVARCHAR(500) NULL,
        [TotalRecords]                INT NOT NULL,
        [SuccessRecords]              INT NOT NULL,
        [WarningRecords]              INT NOT NULL,
        [FailedRecords]               INT NOT NULL,
        [ResultReference]             NVARCHAR(200) NULL,
        [CompletedAt]                 DATETIME2 NOT NULL,
        CONSTRAINT [PK_ProcessResult] PRIMARY KEY CLUSTERED ([ProcessResultId] ASC),
        CONSTRAINT [FK_ProcessResult_ProcessExecution]
            FOREIGN KEY ([ProcessExecutionId]) REFERENCES [core].[ProcessExecution]([ProcessExecutionId]),
        CONSTRAINT [UQ_ProcessResult_Execution] UNIQUE ([ProcessExecutionId])
    );

    ALTER TABLE [core].[ProcessResult] ADD CONSTRAINT [DF_ProcessResult_TotalRecords] DEFAULT ((0)) FOR [TotalRecords];
    ALTER TABLE [core].[ProcessResult] ADD CONSTRAINT [DF_ProcessResult_SuccessRecords] DEFAULT ((0)) FOR [SuccessRecords];
    ALTER TABLE [core].[ProcessResult] ADD CONSTRAINT [DF_ProcessResult_WarningRecords] DEFAULT ((0)) FOR [WarningRecords];
    ALTER TABLE [core].[ProcessResult] ADD CONSTRAINT [DF_ProcessResult_FailedRecords] DEFAULT ((0)) FOR [FailedRecords];
    ALTER TABLE [core].[ProcessResult] ADD CONSTRAINT [DF_ProcessResult_CompletedAt] DEFAULT (SYSDATETIME()) FOR [CompletedAt];
END
GO

-- ---------------------------------------------------------------------------
-- 9. ProcessResultDetail (Recommended)
-- ---------------------------------------------------------------------------
IF OBJECT_ID(N'[core].[ProcessResultDetail]', N'U') IS NULL
BEGIN
    CREATE TABLE [core].[ProcessResultDetail]
    (
        [ProcessResultDetailId]     BIGINT IDENTITY(1,1) NOT NULL,
        [ProcessExecutionId]        BIGINT NOT NULL,
        [RecordKey]                 NVARCHAR(200) NOT NULL,
        [RecordLabel]               NVARCHAR(500) NULL,
        [Severity]                  VARCHAR(20) NOT NULL,
        [Message]                   NVARCHAR(500) NOT NULL,
        [ReferenceId]               NVARCHAR(200) NULL,
        [CreatedTime]               DATETIME2 NOT NULL,
        CONSTRAINT [PK_ProcessResultDetail] PRIMARY KEY CLUSTERED ([ProcessResultDetailId] ASC),
        CONSTRAINT [FK_ProcessResultDetail_ProcessExecution]
            FOREIGN KEY ([ProcessExecutionId]) REFERENCES [core].[ProcessExecution]([ProcessExecutionId])
    );

    ALTER TABLE [core].[ProcessResultDetail] ADD CONSTRAINT [DF_ProcessResultDetail_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime];

    CREATE NONCLUSTERED INDEX [IX_ProcessResultDetail_Execution]
        ON [core].[ProcessResultDetail]([ProcessExecutionId], [Severity]);
END
GO

-- ---------------------------------------------------------------------------
-- 10. ProcessApproval (Recommended)
-- ---------------------------------------------------------------------------
IF OBJECT_ID(N'[core].[ProcessApproval]', N'U') IS NULL
BEGIN
    CREATE TABLE [core].[ProcessApproval]
    (
        [ProcessApprovalId]         BIGINT IDENTITY(1,1) NOT NULL,
        [ProcessExecutionId]        BIGINT NOT NULL,
        [ApproverUserId]            BIGINT NOT NULL,
        [Decision]                  VARCHAR(20) NOT NULL,
        [Comment]                   NVARCHAR(500) NULL,
        [DecidedAt]                 DATETIME2 NULL,
        [CreatedTime]               DATETIME2 NOT NULL,
        CONSTRAINT [PK_ProcessApproval] PRIMARY KEY CLUSTERED ([ProcessApprovalId] ASC),
        CONSTRAINT [FK_ProcessApproval_ProcessExecution]
            FOREIGN KEY ([ProcessExecutionId]) REFERENCES [core].[ProcessExecution]([ProcessExecutionId])
    );

    ALTER TABLE [core].[ProcessApproval] ADD CONSTRAINT [DF_ProcessApproval_Decision] DEFAULT ('Pending') FOR [Decision];
    ALTER TABLE [core].[ProcessApproval] ADD CONSTRAINT [DF_ProcessApproval_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime];
END
GO

-- ---------------------------------------------------------------------------
-- 11. ProcessAudit (Recommended)
-- ---------------------------------------------------------------------------
IF OBJECT_ID(N'[core].[ProcessAudit]', N'U') IS NULL
BEGIN
    CREATE TABLE [core].[ProcessAudit]
    (
        [ProcessAuditId]            BIGINT IDENTITY(1,1) NOT NULL,
        [ProcessExecutionId]        BIGINT NOT NULL,
        [Action]                    VARCHAR(50) NOT NULL,
        [StepCode]                  VARCHAR(50) NULL,
        [UserId]                    BIGINT NOT NULL,
        [ClientIp]                  VARCHAR(45) NULL,
        [DetailJson]                NVARCHAR(MAX) NULL,
        [CreatedTime]               DATETIME2 NOT NULL,
        CONSTRAINT [PK_ProcessAudit] PRIMARY KEY CLUSTERED ([ProcessAuditId] ASC),
        CONSTRAINT [FK_ProcessAudit_ProcessExecution]
            FOREIGN KEY ([ProcessExecutionId]) REFERENCES [core].[ProcessExecution]([ProcessExecutionId])
    );

    ALTER TABLE [core].[ProcessAudit] ADD CONSTRAINT [DF_ProcessAudit_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime];

    CREATE NONCLUSTERED INDEX [IX_ProcessAudit_Execution]
        ON [core].[ProcessAudit]([ProcessExecutionId], [CreatedTime] DESC);
END
GO

-- ============================================================================
-- Seed: IMPORT_DATA reference process (pilot)
-- ExecutionMode: 2 = Async/Background
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM [core].[ProcessDefinition] WHERE [ProcessCode] = 'IMPORT_DATA')
BEGIN
    DECLARE @DefId BIGINT;
    DECLARE @DefGuid UNIQUEIDENTIFIER = NEWID();

    INSERT INTO [core].[ProcessDefinition]
        ([ProcessDefinitionGuid], [ProcessCode], [ProcessName], [Description], [Category],
         [Version], [ExecutionMode], [RequiresApproval], [AllowRetry], [AllowCancel], [AllowSchedule],
         [IsActive], [StatusId], [CreatedById], [CreatedTime])
    VALUES
        (@DefGuid, 'IMPORT_DATA', N'Data Import', N'Import staging data from file into target tables',
         'Import', 1, 2, 0, 1, 1, 0, 1, 1, 1, SYSDATETIME());

    SET @DefId = SCOPE_IDENTITY();

    INSERT INTO [core].[ProcessParameterDefinition]
        ([ProcessDefinitionId], [ParameterCode], [ParameterName], [DataType], [ControlType],
         [IsRequired], [DisplayOrder], [GroupName], [IsVisible], [StatusId], [CreatedById], [CreatedTime])
    VALUES
        (@DefId, 'CompanyId', N'Company', 'bigint', 'Lookup', 1, 1, N'Scope', 1, 1, 1, SYSDATETIME()),
        (@DefId, 'ImportType', N'Import Type', 'string', 'Dropdown', 1, 2, N'Input', 1, 1, 1, SYSDATETIME()),
        (@DefId, 'SourceFile', N'Source File', 'string', 'FileUpload', 1, 3, N'Input', 1, 1, 1, SYSDATETIME()),
        (@DefId, 'ValidateOnly', N'Validate Only', 'boolean', 'Boolean', 0, 4, N'Options', 1, 1, 1, SYSDATETIME());

    INSERT INTO [core].[ProcessStepDefinition]
        ([ProcessDefinitionId], [StepNo], [StepCode], [StepName], [StepType],
         [IsMandatory], [AllowRetry], [AllowSkip], [ExecutionOrder], [ConfigurationJson],
         [StatusId], [CreatedById], [CreatedTime])
    VALUES
        (@DefId, 1, 'VALIDATE_FILE', N'Validate File', 'Validation', 1, 1, 0, 1,
         N'{"handlerKey":"ImportValidateFileStep"}', 1, 1, SYSDATETIME()),
        (@DefId, 2, 'PARSE_ROWS', N'Parse Rows', 'DataProcessing', 1, 1, 0, 2,
         N'{"handlerKey":"ImportParseRowsStep"}', 1, 1, SYSDATETIME()),
        (@DefId, 3, 'COMMIT_DATA', N'Commit Data', 'DatabaseCommand', 1, 1, 0, 3,
         N'{"handlerKey":"ImportCommitDataStep"}', 1, 1, SYSDATETIME());

    PRINT 'Seeded IMPORT_DATA process definition.';
END
GO
