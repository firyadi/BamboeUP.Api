USE [Dev.BamboeHR]
GO

IF SCHEMA_ID(N'sync') IS NULL EXEC(N'CREATE SCHEMA [sync]')
GO

IF COL_LENGTH(N'app.Attendance', N'SyncVersion') IS NULL
    ALTER TABLE [app].[Attendance] ADD [SyncVersion] BIGINT NOT NULL
        CONSTRAINT [DF_Attendance_SyncVersion] DEFAULT (1)
GO

IF OBJECT_ID(N'[sync].[MobileDevice]', N'U') IS NULL
BEGIN
    CREATE TABLE [sync].[MobileDevice]
    (
        [MobileDeviceId] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MobileDevice] PRIMARY KEY,
        [DeviceGuid] UNIQUEIDENTIFIER NOT NULL,
        [CompanyId] BIGINT NOT NULL,
        [UserId] BIGINT NOT NULL,
        [LastSeenAt] DATETIME2(7) NOT NULL,
        [StatusId] INT NOT NULL CONSTRAINT [DF_MobileDevice_StatusId] DEFAULT (1),
        [CreatedById] BIGINT NOT NULL CONSTRAINT [DF_MobileDevice_CreatedById] DEFAULT (0),
        [CreatedTime] DATETIME2(7) NOT NULL CONSTRAINT [DF_MobileDevice_CreatedTime] DEFAULT (SYSUTCDATETIME()),
        [UpdatedById] BIGINT NULL,
        [UpdatedTime] DATETIME2(7) NULL,
        [DeletedById] BIGINT NULL,
        [DeletedTime] DATETIME2(7) NULL,
        CONSTRAINT [UQ_MobileDevice_DeviceTenantUser] UNIQUE ([DeviceGuid], [CompanyId], [UserId])
    );
    CREATE INDEX [IX_MobileDevice_TenantUser] ON [sync].[MobileDevice] ([CompanyId], [UserId]);
END
GO

IF OBJECT_ID(N'[sync].[MobileSyncBatch]', N'U') IS NULL
BEGIN
    CREATE TABLE [sync].[MobileSyncBatch]
    (
        [MobileSyncBatchId] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MobileSyncBatch] PRIMARY KEY,
        [BatchGuid] UNIQUEIDENTIFIER NOT NULL,
        [DeviceGuid] UNIQUEIDENTIFIER NOT NULL,
        [CompanyId] BIGINT NOT NULL,
        [UserId] BIGINT NOT NULL,
        [ClientCreatedAt] DATETIMEOFFSET(7) NOT NULL,
        [Status] VARCHAR(20) NOT NULL,
        [CompletedTime] DATETIME2(7) NULL,
        [StatusId] INT NOT NULL CONSTRAINT [DF_MobileSyncBatch_StatusId] DEFAULT (1),
        [CreatedById] BIGINT NOT NULL CONSTRAINT [DF_MobileSyncBatch_CreatedById] DEFAULT (0),
        [CreatedTime] DATETIME2(7) NOT NULL CONSTRAINT [DF_MobileSyncBatch_CreatedTime] DEFAULT (SYSUTCDATETIME()),
        [UpdatedById] BIGINT NULL,
        [UpdatedTime] DATETIME2(7) NULL,
        [DeletedById] BIGINT NULL,
        [DeletedTime] DATETIME2(7) NULL,
        CONSTRAINT [UQ_MobileSyncBatch_BatchTenantUser] UNIQUE ([BatchGuid], [CompanyId], [UserId])
    );
    CREATE INDEX [IX_MobileSyncBatch_Device] ON [sync].[MobileSyncBatch] ([DeviceGuid], [CompanyId], [UserId]);
END
GO

IF OBJECT_ID(N'[sync].[MobileSyncOperation]', N'U') IS NULL
BEGIN
    CREATE TABLE [sync].[MobileSyncOperation]
    (
        [MobileSyncOperationId] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MobileSyncOperation] PRIMARY KEY,
        [OperationGuid] UNIQUEIDENTIFIER NOT NULL,
        [BatchGuid] UNIQUEIDENTIFIER NOT NULL,
        [CompanyId] BIGINT NOT NULL,
        [UserId] BIGINT NOT NULL,
        [EntityType] VARCHAR(80) NOT NULL,
        [OperationType] VARCHAR(80) NOT NULL,
        [EntityGuid] UNIQUEIDENTIFIER NOT NULL,
        [BaseVersion] BIGINT NULL,
        [Status] VARCHAR(20) NOT NULL,
        [ReceiptJson] NVARCHAR(MAX) NULL,
        [ProcessedTime] DATETIME2(7) NULL,
        [StatusId] INT NOT NULL CONSTRAINT [DF_MobileSyncOperation_StatusId] DEFAULT (1),
        [CreatedById] BIGINT NOT NULL CONSTRAINT [DF_MobileSyncOperation_CreatedById] DEFAULT (0),
        [CreatedTime] DATETIME2(7) NOT NULL CONSTRAINT [DF_MobileSyncOperation_CreatedTime] DEFAULT (SYSUTCDATETIME()),
        [UpdatedById] BIGINT NULL,
        [UpdatedTime] DATETIME2(7) NULL,
        [DeletedById] BIGINT NULL,
        [DeletedTime] DATETIME2(7) NULL,
        CONSTRAINT [UQ_MobileSyncOperation_OperationTenantUser] UNIQUE ([OperationGuid], [CompanyId], [UserId]),
        CONSTRAINT [CK_MobileSyncOperation_ReceiptJson] CHECK ([ReceiptJson] IS NULL OR ISJSON([ReceiptJson]) = 1)
    );
    CREATE INDEX [IX_MobileSyncOperation_Batch] ON [sync].[MobileSyncOperation]
        ([BatchGuid], [CompanyId], [UserId], [MobileSyncOperationId]);
    CREATE INDEX [IX_MobileSyncOperation_Entity] ON [sync].[MobileSyncOperation]
        ([EntityType], [EntityGuid], [CompanyId]);
END
GO
