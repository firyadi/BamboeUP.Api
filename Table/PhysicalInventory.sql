USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF SCHEMA_ID(N'sync') IS NULL EXEC(N'CREATE SCHEMA [sync]')
GO

IF OBJECT_ID(N'[sync].[OfflineAccessPermit]', N'U') IS NULL
BEGIN
    CREATE TABLE [sync].[OfflineAccessPermit]
    (
        [OfflineAccessPermitId] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_OfflineAccessPermit] PRIMARY KEY,
        [OfflineAccessPermitGuid] UNIQUEIDENTIFIER NOT NULL,
        [DeviceGuid] UNIQUEIDENTIFIER NOT NULL,
        [UserId] BIGINT NOT NULL,
        [UserGuid] UNIQUEIDENTIFIER NOT NULL,
        [CompanyId] BIGINT NOT NULL,
        [CompanyOfficeId] BIGINT NULL,
        [AppCode] NVARCHAR(80) NOT NULL,
        [CanCapture] BIT NOT NULL CONSTRAINT [DF_OfflineAccessPermit_CanCapture] DEFAULT (1),
        [CanVerify] BIT NOT NULL CONSTRAINT [DF_OfflineAccessPermit_CanVerify] DEFAULT (0),
        [PermissionVersion] INT NOT NULL CONSTRAINT [DF_OfflineAccessPermit_PermissionVersion] DEFAULT (1),
        [IssuedAt] DATETIMEOFFSET(7) NOT NULL,
        [ExpiresAt] DATETIMEOFFSET(7) NOT NULL,
        [RevokedTime] DATETIME2(7) NULL,
        [PermitJson] NVARCHAR(MAX) NOT NULL,
        [StatusId] INT NOT NULL CONSTRAINT [DF_OfflineAccessPermit_StatusId] DEFAULT (1),
        [RowVersion] TIMESTAMP NOT NULL,
        [CreatedById] BIGINT NOT NULL CONSTRAINT [DF_OfflineAccessPermit_CreatedById] DEFAULT (0),
        [CreatedTime] DATETIME2(7) NOT NULL CONSTRAINT [DF_OfflineAccessPermit_CreatedTime] DEFAULT (SYSUTCDATETIME()),
        [UpdatedById] BIGINT NULL,
        [UpdatedTime] DATETIME2(7) NULL,
        [DeletedById] BIGINT NULL,
        [DeletedTime] DATETIME2(7) NULL,
        CONSTRAINT [UQ_OfflineAccessPermit_OfflineAccessPermitGuid] UNIQUE ([OfflineAccessPermitGuid]),
        CONSTRAINT [CK_OfflineAccessPermit_PermitJson] CHECK ([PermitJson] IS NULL OR ISJSON([PermitJson]) = 1)
    );
    CREATE UNIQUE INDEX [UQ_OfflineAccessPermit_DeviceUserApp_Active]
        ON [sync].[OfflineAccessPermit] ([DeviceGuid], [UserId], [AppCode])
        WHERE ([DeletedTime] IS NULL AND [StatusId] > 0 AND [RevokedTime] IS NULL);
    CREATE INDEX [IX_OfflineAccessPermit_UserId] ON [sync].[OfflineAccessPermit] ([UserId]);
    CREATE INDEX [IX_OfflineAccessPermit_CompanyId] ON [sync].[OfflineAccessPermit] ([CompanyId]);
END
GO

IF OBJECT_ID(N'[app].[InventoryItem]', N'U') IS NULL
BEGIN
    CREATE TABLE [app].[InventoryItem]
    (
        [InventoryItemId] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_InventoryItem] PRIMARY KEY,
        [InventoryItemGuid] UNIQUEIDENTIFIER NOT NULL,
        [CompanyId] BIGINT NOT NULL,
        [CompanyOfficeId] BIGINT NULL,
        [ItemCode] NVARCHAR(50) NOT NULL,
        [ItemName] NVARCHAR(200) NOT NULL,
        [Barcode] NVARCHAR(80) NOT NULL,
        [UnitOfMeasure] NVARCHAR(20) NOT NULL,
        [StatusId] INT NOT NULL CONSTRAINT [DF_InventoryItem_StatusId] DEFAULT (1),
        [RowVersion] TIMESTAMP NOT NULL,
        [CreatedById] BIGINT NOT NULL CONSTRAINT [DF_InventoryItem_CreatedById] DEFAULT (0),
        [CreatedTime] DATETIME2(7) NOT NULL CONSTRAINT [DF_InventoryItem_CreatedTime] DEFAULT (SYSUTCDATETIME()),
        [UpdatedById] BIGINT NULL,
        [UpdatedTime] DATETIME2(7) NULL,
        [DeletedById] BIGINT NULL,
        [DeletedTime] DATETIME2(7) NULL,
        CONSTRAINT [UQ_InventoryItem_InventoryItemGuid] UNIQUE ([InventoryItemGuid])
    );
    ALTER TABLE [app].[InventoryItem] ADD CONSTRAINT [DF_InventoryItem_InventoryItemGuid] DEFAULT (NEWSEQUENTIALID()) FOR [InventoryItemGuid];
    CREATE UNIQUE INDEX [UQ_InventoryItem_CompanyBarcode_Active]
        ON [app].[InventoryItem] ([CompanyId], [Barcode])
        WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
    CREATE INDEX [IX_InventoryItem_CompanyId] ON [app].[InventoryItem] ([CompanyId]);
    CREATE INDEX [IX_InventoryItem_CompanyOfficeId] ON [app].[InventoryItem] ([CompanyOfficeId]);
END
GO

IF OBJECT_ID(N'[app].[PhysicalInventoryDocument]', N'U') IS NULL
BEGIN
    CREATE TABLE [app].[PhysicalInventoryDocument]
    (
        [PhysicalInventoryDocumentId] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_PhysicalInventoryDocument] PRIMARY KEY,
        [PhysicalInventoryDocumentGuid] UNIQUEIDENTIFIER NOT NULL,
        [CompanyId] BIGINT NOT NULL,
        [CompanyOfficeId] BIGINT NULL,
        [DocumentNumber] NVARCHAR(50) NOT NULL,
        [CountDate] DATE NOT NULL,
        [DocumentStatus] NVARCHAR(40) NOT NULL CONSTRAINT [DF_PhysicalInventoryDocument_DocumentStatus] DEFAULT (N'Open'),
        [StatusId] INT NOT NULL CONSTRAINT [DF_PhysicalInventoryDocument_StatusId] DEFAULT (1),
        [RowVersion] TIMESTAMP NOT NULL,
        [CreatedById] BIGINT NOT NULL CONSTRAINT [DF_PhysicalInventoryDocument_CreatedById] DEFAULT (0),
        [CreatedTime] DATETIME2(7) NOT NULL CONSTRAINT [DF_PhysicalInventoryDocument_CreatedTime] DEFAULT (SYSUTCDATETIME()),
        [UpdatedById] BIGINT NULL,
        [UpdatedTime] DATETIME2(7) NULL,
        [DeletedById] BIGINT NULL,
        [DeletedTime] DATETIME2(7) NULL,
        CONSTRAINT [UQ_PhysicalInventoryDocument_PhysicalInventoryDocumentGuid] UNIQUE ([PhysicalInventoryDocumentGuid])
    );
    ALTER TABLE [app].[PhysicalInventoryDocument] ADD CONSTRAINT [DF_PhysicalInventoryDocument_PhysicalInventoryDocumentGuid] DEFAULT (NEWSEQUENTIALID()) FOR [PhysicalInventoryDocumentGuid];
    CREATE UNIQUE INDEX [UQ_PhysicalInventoryDocument_CompanyNumber_Active]
        ON [app].[PhysicalInventoryDocument] ([CompanyId], [DocumentNumber])
        WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
    CREATE INDEX [IX_PhysicalInventoryDocument_CompanyOfficeId] ON [app].[PhysicalInventoryDocument] ([CompanyOfficeId]);
END
GO

IF OBJECT_ID(N'[app].[PhysicalInventoryDocumentPage]', N'U') IS NULL
BEGIN
    CREATE TABLE [app].[PhysicalInventoryDocumentPage]
    (
        [PhysicalInventoryDocumentPageId] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_PhysicalInventoryDocumentPage] PRIMARY KEY,
        [PhysicalInventoryDocumentPageGuid] UNIQUEIDENTIFIER NOT NULL,
        [PhysicalInventoryDocumentId] BIGINT NOT NULL,
        [PageNumber] INT NOT NULL,
        [PageStatus] NVARCHAR(40) NOT NULL CONSTRAINT [DF_PhysicalInventoryDocumentPage_PageStatus] DEFAULT (N'Open'),
        [AssignedUserId] BIGINT NULL,
        [StatusId] INT NOT NULL CONSTRAINT [DF_PhysicalInventoryDocumentPage_StatusId] DEFAULT (1),
        [RowVersion] TIMESTAMP NOT NULL,
        [CreatedById] BIGINT NOT NULL CONSTRAINT [DF_PhysicalInventoryDocumentPage_CreatedById] DEFAULT (0),
        [CreatedTime] DATETIME2(7) NOT NULL CONSTRAINT [DF_PhysicalInventoryDocumentPage_CreatedTime] DEFAULT (SYSUTCDATETIME()),
        [UpdatedById] BIGINT NULL,
        [UpdatedTime] DATETIME2(7) NULL,
        [DeletedById] BIGINT NULL,
        [DeletedTime] DATETIME2(7) NULL,
        CONSTRAINT [UQ_PhysicalInventoryDocumentPage_PhysicalInventoryDocumentPageGuid] UNIQUE ([PhysicalInventoryDocumentPageGuid])
    );
    ALTER TABLE [app].[PhysicalInventoryDocumentPage] ADD CONSTRAINT [DF_PhysicalInventoryDocumentPage_PhysicalInventoryDocumentPageGuid] DEFAULT (NEWSEQUENTIALID()) FOR [PhysicalInventoryDocumentPageGuid];
    CREATE UNIQUE INDEX [UQ_PhysicalInventoryDocumentPage_DocumentPage_Active]
        ON [app].[PhysicalInventoryDocumentPage] ([PhysicalInventoryDocumentId], [PageNumber])
        WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
    CREATE INDEX [IX_PhysicalInventoryDocumentPage_PhysicalInventoryDocumentId]
        ON [app].[PhysicalInventoryDocumentPage] ([PhysicalInventoryDocumentId]);
END
GO

IF OBJECT_ID(N'[app].[PhysicalInventoryExpectedLine]', N'U') IS NULL
BEGIN
    CREATE TABLE [app].[PhysicalInventoryExpectedLine]
    (
        [PhysicalInventoryExpectedLineId] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_PhysicalInventoryExpectedLine] PRIMARY KEY,
        [PhysicalInventoryExpectedLineGuid] UNIQUEIDENTIFIER NOT NULL,
        [PhysicalInventoryDocumentPageId] BIGINT NOT NULL,
        [InventoryItemId] BIGINT NOT NULL,
        [ExpectedQuantity] DECIMAL(18,4) NOT NULL,
        [StatusId] INT NOT NULL CONSTRAINT [DF_PhysicalInventoryExpectedLine_StatusId] DEFAULT (1),
        [RowVersion] TIMESTAMP NOT NULL,
        [CreatedById] BIGINT NOT NULL CONSTRAINT [DF_PhysicalInventoryExpectedLine_CreatedById] DEFAULT (0),
        [CreatedTime] DATETIME2(7) NOT NULL CONSTRAINT [DF_PhysicalInventoryExpectedLine_CreatedTime] DEFAULT (SYSUTCDATETIME()),
        [UpdatedById] BIGINT NULL,
        [UpdatedTime] DATETIME2(7) NULL,
        [DeletedById] BIGINT NULL,
        [DeletedTime] DATETIME2(7) NULL,
        CONSTRAINT [UQ_PhysicalInventoryExpectedLine_PhysicalInventoryExpectedLineGuid] UNIQUE ([PhysicalInventoryExpectedLineGuid])
    );
    ALTER TABLE [app].[PhysicalInventoryExpectedLine] ADD CONSTRAINT [DF_PhysicalInventoryExpectedLine_PhysicalInventoryExpectedLineGuid] DEFAULT (NEWSEQUENTIALID()) FOR [PhysicalInventoryExpectedLineGuid];
    CREATE UNIQUE INDEX [UQ_PhysicalInventoryExpectedLine_PageItem_Active]
        ON [app].[PhysicalInventoryExpectedLine] ([PhysicalInventoryDocumentPageId], [InventoryItemId])
        WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
    CREATE INDEX [IX_PhysicalInventoryExpectedLine_InventoryItemId]
        ON [app].[PhysicalInventoryExpectedLine] ([InventoryItemId]);
END
GO

IF OBJECT_ID(N'[app].[PhysicalInventoryCapture]', N'U') IS NULL
BEGIN
    CREATE TABLE [app].[PhysicalInventoryCapture]
    (
        [PhysicalInventoryCaptureId] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_PhysicalInventoryCapture] PRIMARY KEY,
        [PhysicalInventoryCaptureGuid] UNIQUEIDENTIFIER NOT NULL,
        [CompanyId] BIGINT NOT NULL,
        [CompanyOfficeId] BIGINT NULL,
        [DeviceGuid] UNIQUEIDENTIFIER NOT NULL,
        [OperationGuid] UNIQUEIDENTIFIER NOT NULL,
        [DocumentNumber] NVARCHAR(50) NOT NULL,
        [PageNumber] INT NOT NULL,
        [PhysicalInventoryDocumentId] BIGINT NULL,
        [PhysicalInventoryDocumentPageId] BIGINT NULL,
        [CounterUserId] BIGINT NOT NULL,
        [CounterUserGuid] UNIQUEIDENTIFIER NOT NULL,
        [CaptureStatus] NVARCHAR(40) NOT NULL,
        [PayloadHash] NVARCHAR(64) NOT NULL,
        [ConcurrencyToken] BIGINT NOT NULL CONSTRAINT [DF_PhysicalInventoryCapture_ConcurrencyToken] DEFAULT (1),
        [StartedAt] DATETIMEOFFSET(7) NOT NULL,
        [FinishedAt] DATETIMEOFFSET(7) NOT NULL,
        [UploadedAt] DATETIMEOFFSET(7) NOT NULL,
        [VerifiedById] BIGINT NULL,
        [VerifiedAt] DATETIMEOFFSET(7) NULL,
        [VerificationNote] NVARCHAR(500) NULL,
        [StatusId] INT NOT NULL CONSTRAINT [DF_PhysicalInventoryCapture_StatusId] DEFAULT (1),
        [RowVersion] TIMESTAMP NOT NULL,
        [CreatedById] BIGINT NOT NULL CONSTRAINT [DF_PhysicalInventoryCapture_CreatedById] DEFAULT (0),
        [CreatedTime] DATETIME2(7) NOT NULL CONSTRAINT [DF_PhysicalInventoryCapture_CreatedTime] DEFAULT (SYSUTCDATETIME()),
        [UpdatedById] BIGINT NULL,
        [UpdatedTime] DATETIME2(7) NULL,
        [DeletedById] BIGINT NULL,
        [DeletedTime] DATETIME2(7) NULL,
        CONSTRAINT [UQ_PhysicalInventoryCapture_PhysicalInventoryCaptureGuid] UNIQUE ([PhysicalInventoryCaptureGuid]),
        CONSTRAINT [UQ_PhysicalInventoryCapture_OperationGuid] UNIQUE ([OperationGuid])
    );
    CREATE INDEX [IX_PhysicalInventoryCapture_CompanyStatus]
        ON [app].[PhysicalInventoryCapture] ([CompanyId], [CaptureStatus])
        WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
    CREATE INDEX [IX_PhysicalInventoryCapture_DocumentPage]
        ON [app].[PhysicalInventoryCapture] ([CompanyId], [DocumentNumber], [PageNumber]);
END
GO

IF OBJECT_ID(N'[app].[PhysicalInventoryCaptureLine]', N'U') IS NULL
BEGIN
    CREATE TABLE [app].[PhysicalInventoryCaptureLine]
    (
        [PhysicalInventoryCaptureLineId] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_PhysicalInventoryCaptureLine] PRIMARY KEY,
        [PhysicalInventoryCaptureLineGuid] UNIQUEIDENTIFIER NOT NULL,
        [PhysicalInventoryCaptureId] BIGINT NOT NULL,
        [Barcode] NVARCHAR(80) NOT NULL,
        [CountedQuantity] DECIMAL(18,4) NOT NULL,
        [ScanCount] INT NOT NULL CONSTRAINT [DF_PhysicalInventoryCaptureLine_ScanCount] DEFAULT (1),
        [Source] NVARCHAR(20) NOT NULL,
        [InventoryItemId] BIGINT NULL,
        [ExpectedQuantity] DECIMAL(18,4) NULL,
        [Variance] DECIMAL(18,4) NULL,
        [ValidationResult] NVARCHAR(40) NOT NULL,
        [FirstCapturedAt] DATETIMEOFFSET(7) NOT NULL,
        [LastCapturedAt] DATETIMEOFFSET(7) NOT NULL,
        [StatusId] INT NOT NULL CONSTRAINT [DF_PhysicalInventoryCaptureLine_StatusId] DEFAULT (1),
        [RowVersion] TIMESTAMP NOT NULL,
        [CreatedById] BIGINT NOT NULL CONSTRAINT [DF_PhysicalInventoryCaptureLine_CreatedById] DEFAULT (0),
        [CreatedTime] DATETIME2(7) NOT NULL CONSTRAINT [DF_PhysicalInventoryCaptureLine_CreatedTime] DEFAULT (SYSUTCDATETIME()),
        [UpdatedById] BIGINT NULL,
        [UpdatedTime] DATETIME2(7) NULL,
        [DeletedById] BIGINT NULL,
        [DeletedTime] DATETIME2(7) NULL,
        CONSTRAINT [UQ_PhysicalInventoryCaptureLine_PhysicalInventoryCaptureLineGuid] UNIQUE ([PhysicalInventoryCaptureLineGuid])
    );
    CREATE UNIQUE INDEX [UQ_PhysicalInventoryCaptureLine_CaptureBarcode_Active]
        ON [app].[PhysicalInventoryCaptureLine] ([PhysicalInventoryCaptureId], [Barcode])
        WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);
    CREATE INDEX [IX_PhysicalInventoryCaptureLine_InventoryItemId]
        ON [app].[PhysicalInventoryCaptureLine] ([InventoryItemId]);
END
GO
