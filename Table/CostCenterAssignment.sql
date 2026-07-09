USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[app].[CostCenterAssignment]', N'U') IS NOT NULL
    DROP TABLE [app].[CostCenterAssignment]
GO

CREATE TABLE [app].[CostCenterAssignment]
(
    [CostCenterAssignmentId]          BIGINT IDENTITY(1,1) NOT NULL,
    [CostCenterAssignmentGuid]        UNIQUEIDENTIFIER NOT NULL,

    [CostCenterId]                    BIGINT NOT NULL,

    [CompanyId]                       BIGINT NOT NULL,
    [CompanyOfficeId]                 BIGINT NULL,

    [ProfitCenterId]                  BIGINT NULL,
    [CostCenterManagerEmployeeId]     BIGINT NULL,

    [BudgetControlType]               TINYINT NOT NULL,

    [EffectiveDate]                   DATE NOT NULL,
    [ExpiredDate]                     DATE NULL,

    [StatusId]                        INT NOT NULL,
    [RowVersion]                      ROWVERSION NOT NULL,

    [CreatedById]                     BIGINT NOT NULL,
    [CreatedTime]                     DATETIME NOT NULL,

    [UpdatedById]                     BIGINT NULL,
    [UpdatedTime]                     DATETIME NULL,

    [DeletedById]                     BIGINT NULL,
    [DeletedTime]                     DATETIME NULL,

    CONSTRAINT [PK_CostCenterAssignment]
        PRIMARY KEY CLUSTERED ([CostCenterAssignmentId]),

    CONSTRAINT [UQ_CostCenterAssignment_Guid]
        UNIQUE ([CostCenterAssignmentGuid]),

    CONSTRAINT [FK_CostCenterAssignment_CostCenter]
        FOREIGN KEY ([CostCenterId])
        REFERENCES [app].[CostCenter] ([CostCenterId]),

    CONSTRAINT [CK_CostCenterAssignment_BudgetControlType]
        CHECK ([BudgetControlType] IN (0, 1, 2))
)
GO

ALTER TABLE [app].[CostCenterAssignment]
ADD CONSTRAINT [DF_CostCenterAssignment_Guid] DEFAULT (NEWID()) FOR [CostCenterAssignmentGuid]
GO

ALTER TABLE [app].[CostCenterAssignment]
ADD CONSTRAINT [DF_CostCenterAssignment_Status] DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [app].[CostCenterAssignment]
ADD CONSTRAINT [DF_CostCenterAssignment_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime]
GO

ALTER TABLE [app].[CostCenterAssignment] WITH CHECK
ADD CONSTRAINT [CHK_CostCenterAssignment_Status]
CHECK ([StatusId] IN (0, 1, 2))
GO

CREATE NONCLUSTERED INDEX [IX_CostCenterAssignment_CostCenter]
ON [app].[CostCenterAssignment] ([CostCenterId])
GO

CREATE NONCLUSTERED INDEX [IX_CostCenterAssignment_Manager]
ON [app].[CostCenterAssignment] ([CostCenterManagerEmployeeId])
GO

CREATE NONCLUSTERED INDEX [IX_CostCenterAssignment_ProfitCenter]
ON [app].[CostCenterAssignment] ([ProfitCenterId])
GO

CREATE NONCLUSTERED INDEX [IX_CostCenterAssignment_EffectiveDate]
ON [app].[CostCenterAssignment] ([EffectiveDate])
GO

CREATE NONCLUSTERED INDEX [IX_CostCenterAssignment_Active]
ON [app].[CostCenterAssignment]
(
    [CostCenterId],
    [CompanyId],
    [EffectiveDate],
    [StatusId]
)
WHERE [DeletedTime] IS NULL
GO
