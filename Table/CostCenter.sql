USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[app].[CostCenter]', N'U') IS NOT NULL
    DROP TABLE [app].[CostCenter]
GO

CREATE TABLE [app].[CostCenter]
(
    [CostCenterId]               BIGINT IDENTITY(1,1) NOT NULL,
    [CostCenterGuid]             UNIQUEIDENTIFIER NOT NULL,

    [CostCenterCode]             VARCHAR(20) NOT NULL,
    [CostCenterName]             VARCHAR(200) NOT NULL,
    [CostCenterDescription]      VARCHAR(500) NULL,

    [ParentCostCenterId]         BIGINT NULL,

    [LevelDepth]                 INT NOT NULL,
    [HierarchyPath]              VARCHAR(500) NULL,

    [StatusId]                   INT NOT NULL,
    [RowVersion]                 ROWVERSION NOT NULL,

    [CreatedById]                BIGINT NOT NULL,
    [CreatedTime]                DATETIME NOT NULL,

    [UpdatedById]                BIGINT NULL,
    [UpdatedTime]                DATETIME NULL,

    [DeletedById]                BIGINT NULL,
    [DeletedTime]                DATETIME NULL,

    CONSTRAINT [PK_CostCenter]
        PRIMARY KEY CLUSTERED ([CostCenterId]),

    CONSTRAINT [UQ_CostCenter_Guid]
        UNIQUE ([CostCenterGuid]),

    CONSTRAINT [UQ_CostCenter_Code]
        UNIQUE ([CostCenterCode])
)
GO

ALTER TABLE [app].[CostCenter]
ADD CONSTRAINT [DF_CostCenter_Guid] DEFAULT (NEWID()) FOR [CostCenterGuid]
GO

ALTER TABLE [app].[CostCenter]
ADD CONSTRAINT [DF_CostCenter_Status] DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [app].[CostCenter]
ADD CONSTRAINT [DF_CostCenter_LevelDepth] DEFAULT ((0)) FOR [LevelDepth]
GO

ALTER TABLE [app].[CostCenter]
ADD CONSTRAINT [DF_CostCenter_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime]
GO

ALTER TABLE [app].[CostCenter] WITH CHECK
ADD CONSTRAINT [CHK_CostCenter_Status]
CHECK ([StatusId] IN (0, 1, 2))
GO

ALTER TABLE [app].[CostCenter] WITH CHECK
ADD CONSTRAINT [FK_CostCenter_Parent]
FOREIGN KEY ([ParentCostCenterId])
REFERENCES [app].[CostCenter] ([CostCenterId])
GO

CREATE NONCLUSTERED INDEX [IX_CostCenter_Name]
ON [app].[CostCenter] ([CostCenterName])
GO

CREATE NONCLUSTERED INDEX [IX_CostCenter_Parent]
ON [app].[CostCenter] ([ParentCostCenterId])
GO

CREATE NONCLUSTERED INDEX [IX_CostCenter_Status]
ON [app].[CostCenter] ([StatusId])
GO

CREATE NONCLUSTERED INDEX [IX_CostCenter_Active]
ON [app].[CostCenter] ([StatusId])
WHERE [DeletedTime] IS NULL
GO
