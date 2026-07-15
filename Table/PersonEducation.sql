USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[PersonEducation]
(
    [PersonEducationId]   BIGINT IDENTITY(1,1) NOT NULL,
    [PersonEducationGuid] UNIQUEIDENTIFIER NOT NULL,

    [PersonId]            BIGINT NOT NULL,

    [SrEducationLevel]    BIGINT NOT NULL,
    [InstitutionName]     NVARCHAR(200) NOT NULL,

    [StatusId]            INT NOT NULL,
    [RowVersion]          ROWVERSION NOT NULL,

    [CreatedById]         BIGINT NOT NULL,
    [CreatedTime]         DATETIME2(7) NOT NULL,
    [UpdatedById]         BIGINT NULL,
    [UpdatedTime]         DATETIME2(7) NULL,
    [DeletedById]         BIGINT NULL,
    [DeletedTime]         DATETIME2(7) NULL,

    CONSTRAINT [PK_PersonEducation]
        PRIMARY KEY CLUSTERED ([PersonEducationId]),

    CONSTRAINT [UQ_PersonEducation_Guid]
        UNIQUE ([PersonEducationGuid]),

    CONSTRAINT [FK_PersonEducation_Person]
        FOREIGN KEY ([PersonId])
        REFERENCES [app].[Person] ([PersonId])
)
GO

ALTER TABLE [app].[PersonEducation]
ADD CONSTRAINT [DF_PersonEducation_Guid] DEFAULT (NEWID()) FOR [PersonEducationGuid]
GO

ALTER TABLE [app].[PersonEducation]
ADD CONSTRAINT [DF_PersonEducation_Status] DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [app].[PersonEducation]
ADD CONSTRAINT [DF_PersonEducation_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime]
GO

ALTER TABLE [app].[PersonEducation] WITH CHECK
ADD CONSTRAINT [CHK_PersonEducation_Status]
CHECK ([StatusId] IN (0, 1, 2))
GO

CREATE NONCLUSTERED INDEX [IX_PersonEducation_Person]
ON [app].[PersonEducation] ([PersonId])
GO
