USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[PersonEmergencyContact]
(
    [PersonEmergencyContactId]   BIGINT IDENTITY(1,1) NOT NULL,
    [PersonEmergencyContactGuid] UNIQUEIDENTIFIER NOT NULL,

    [PersonId]                   BIGINT NOT NULL,

    [ContactName]                NVARCHAR(200) NOT NULL,
    [SrRelationship]           BIGINT NOT NULL,
    [Phone]                      VARCHAR(30) NULL,
    [IsPrimary]                  BIT NOT NULL,

    [StatusId]                   INT NOT NULL,
    [RowVersion]                 ROWVERSION NOT NULL,

    [CreatedById]                BIGINT NOT NULL,
    [CreatedTime]                DATETIME2(7) NOT NULL,
    [UpdatedById]                BIGINT NULL,
    [UpdatedTime]                DATETIME2(7) NULL,
    [DeletedById]                  BIGINT NULL,
    [DeletedTime]                  DATETIME2(7) NULL,

    CONSTRAINT [PK_PersonEmergencyContact]
        PRIMARY KEY CLUSTERED ([PersonEmergencyContactId]),

    CONSTRAINT [UQ_PersonEmergencyContact_Guid]
        UNIQUE ([PersonEmergencyContactGuid]),

    CONSTRAINT [FK_PersonEmergencyContact_Person]
        FOREIGN KEY ([PersonId])
        REFERENCES [app].[Person] ([PersonId])
)
GO

ALTER TABLE [app].[PersonEmergencyContact]
ADD CONSTRAINT [DF_PersonEmergencyContact_Guid] DEFAULT (NEWID()) FOR [PersonEmergencyContactGuid]
GO

ALTER TABLE [app].[PersonEmergencyContact]
ADD CONSTRAINT [DF_PersonEmergencyContact_IsPrimary] DEFAULT ((1)) FOR [IsPrimary]
GO

ALTER TABLE [app].[PersonEmergencyContact]
ADD CONSTRAINT [DF_PersonEmergencyContact_Status] DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [app].[PersonEmergencyContact]
ADD CONSTRAINT [DF_PersonEmergencyContact_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime]
GO

ALTER TABLE [app].[PersonEmergencyContact] WITH CHECK
ADD CONSTRAINT [CHK_PersonEmergencyContact_Status]
CHECK ([StatusId] IN (0, 1, 2))
GO

CREATE NONCLUSTERED INDEX [IX_PersonEmergencyContact_Person]
ON [app].[PersonEmergencyContact] ([PersonId])
GO
