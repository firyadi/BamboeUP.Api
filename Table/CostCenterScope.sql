USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[app].[CostCenterScope]', N'U') IS NOT NULL
    DROP TABLE [app].[CostCenterScope]
GO

CREATE TABLE [app].[CostCenterScope]
(
    [CostCenterScopeId]          BIGINT IDENTITY(1,1) NOT NULL,
    [CostCenterScopeGuid]        UNIQUEIDENTIFIER NOT NULL,

    [CostCenterId]               BIGINT NOT NULL,

    [CompanyId]                  BIGINT NOT NULL,
    [CompanyOfficeId]            BIGINT NULL,

    [ScopeType]                  VARCHAR(20) NOT NULL,

    [StatusId]                   INT NOT NULL,
    [RowVersion]                 ROWVERSION NOT NULL,

    [CreatedById]                BIGINT NOT NULL,
    [CreatedTime]                DATETIME NOT NULL,

    [UpdatedById]                BIGINT NULL,
    [UpdatedTime]                DATETIME NULL,

    [DeletedById]                BIGINT NULL,
    [DeletedTime]                DATETIME NULL,

    CONSTRAINT [PK_CostCenterScope]
        PRIMARY KEY CLUSTERED ([CostCenterScopeId]),

    CONSTRAINT [UQ_CostCenterScope_Guid]
        UNIQUE ([CostCenterScopeGuid]),

    CONSTRAINT [FK_CostCenterScope_CostCenter]
        FOREIGN KEY ([CostCenterId])
        REFERENCES [app].[CostCenter] ([CostCenterId])
)
GO

ALTER TABLE [app].[CostCenterScope]
ADD CONSTRAINT [DF_CostCenterScope_Guid] DEFAULT (NEWID()) FOR [CostCenterScopeGuid]
GO

ALTER TABLE [app].[CostCenterScope]
ADD CONSTRAINT [DF_CostCenterScope_Status] DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [app].[CostCenterScope]
ADD CONSTRAINT [DF_CostCenterScope_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime]
GO

ALTER TABLE [app].[CostCenterScope] WITH CHECK
ADD CONSTRAINT [CHK_CostCenterScope_Status]
CHECK ([StatusId] IN (0, 1, 2))
GO

ALTER TABLE [app].[CostCenterScope] WITH CHECK
ADD CONSTRAINT [CHK_CostCenterScope_Type]
CHECK (
    ([ScopeType] = 'COMPANY' AND [CompanyOfficeId] IS NULL)
    OR
    ([ScopeType] = 'OFFICE' AND [CompanyOfficeId] IS NOT NULL)
)
GO

CREATE NONCLUSTERED INDEX [IX_CostCenterScope_CostCenter]
ON [app].[CostCenterScope] ([CostCenterId])
GO

CREATE NONCLUSTERED INDEX [IX_CostCenterScope_Company]
ON [app].[CostCenterScope] ([CompanyId])
GO

CREATE NONCLUSTERED INDEX [IX_CostCenterScope_CompanyOffice]
ON [app].[CostCenterScope] ([CompanyOfficeId])
GO

CREATE NONCLUSTERED INDEX [IX_CostCenterScope_Active]
ON [app].[CostCenterScope]
(
    [CostCenterId],
    [CompanyId],
    [StatusId]
)
WHERE [DeletedTime] IS NULL
GO

CREATE UNIQUE NONCLUSTERED INDEX [UQ_CostCenterScope_Main]
ON [app].[CostCenterScope]
(
    [CostCenterId],
    [CompanyId],
    [CompanyOfficeId]
)
WHERE [DeletedTime] IS NULL
GO
