USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[PersonIdentification]
(
    [PersonIdentificationId]   BIGINT IDENTITY(1,1) NOT NULL,
    [PersonIdentificationGuid] UNIQUEIDENTIFIER NOT NULL,

    [PersonId]                 BIGINT NOT NULL,

    [SrIdentificationTypeId]   BIGINT NOT NULL,
    [IdentificationValue]      NVARCHAR(100) NOT NULL,

    [StatusId]                 INT NOT NULL,
    [RowVersion]               ROWVERSION NOT NULL,

    [CreatedById]              BIGINT NOT NULL,
    [CreatedTime]              DATETIME2(7) NOT NULL,
    [UpdatedById]              BIGINT NULL,
    [UpdatedTime]              DATETIME2(7) NULL,
    [DeletedById]              BIGINT NULL,
    [DeletedTime]              DATETIME2(7) NULL,

    CONSTRAINT [PK_PersonIdentification]
        PRIMARY KEY CLUSTERED ([PersonIdentificationId]),

    CONSTRAINT [UQ_PersonIdentification_Guid]
        UNIQUE ([PersonIdentificationGuid]),

    CONSTRAINT [FK_PersonIdentification_Person]
        FOREIGN KEY ([PersonId])
        REFERENCES [app].[Person] ([PersonId])
)
GO

ALTER TABLE [app].[PersonIdentification]
ADD CONSTRAINT [DF_PersonIdentification_Guid] DEFAULT (NEWID()) FOR [PersonIdentificationGuid]
GO

ALTER TABLE [app].[PersonIdentification]
ADD CONSTRAINT [DF_PersonIdentification_Status] DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [app].[PersonIdentification]
ADD CONSTRAINT [DF_PersonIdentification_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime]
GO

ALTER TABLE [app].[PersonIdentification] WITH CHECK
ADD CONSTRAINT [CHK_PersonIdentification_Status]
CHECK ([StatusId] IN (0, 1, 2))
GO

CREATE NONCLUSTERED INDEX [IX_PersonIdentification_Person]
ON [app].[PersonIdentification] ([PersonId])
GO

CREATE UNIQUE NONCLUSTERED INDEX [UQ_PersonIdentification_Value]
ON [app].[PersonIdentification] ([SrIdentificationTypeId], [IdentificationValue])
WHERE [DeletedTime] IS NULL
GO
