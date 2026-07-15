USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[PersonAddress]
(
    [PersonAddressId]   BIGINT IDENTITY(1,1) NOT NULL,
    [PersonAddressGuid] UNIQUEIDENTIFIER NOT NULL,

    [PersonId]          BIGINT NOT NULL,

    [SrAddressType]     BIGINT NOT NULL,
    [Address]           NVARCHAR(1000) NOT NULL,

    [CountryId]         BIGINT NOT NULL,
    [ProvinceId]        BIGINT NOT NULL,
    [CityId]            BIGINT NOT NULL,

    [StatusId]          INT NOT NULL,
    [RowVersion]        ROWVERSION NOT NULL,

    [CreatedById]       BIGINT NOT NULL,
    [CreatedTime]       DATETIME2(7) NOT NULL,
    [UpdatedById]       BIGINT NULL,
    [UpdatedTime]       DATETIME2(7) NULL,
    [DeletedById]       BIGINT NULL,
    [DeletedTime]       DATETIME2(7) NULL,

    CONSTRAINT [PK_PersonAddress]
        PRIMARY KEY CLUSTERED ([PersonAddressId]),

    CONSTRAINT [UQ_PersonAddress_Guid]
        UNIQUE ([PersonAddressGuid]),

    CONSTRAINT [FK_PersonAddress_Person]
        FOREIGN KEY ([PersonId])
        REFERENCES [app].[Person] ([PersonId]),

    CONSTRAINT [FK_PersonAddress_Country]
        FOREIGN KEY ([CountryId])
        REFERENCES [core].[Country] ([CountryId]),

    CONSTRAINT [FK_PersonAddress_Province]
        FOREIGN KEY ([ProvinceId])
        REFERENCES [core].[Province] ([ProvinceId]),

    CONSTRAINT [FK_PersonAddress_City]
        FOREIGN KEY ([CityId])
        REFERENCES [core].[City] ([CityId])
)
GO

ALTER TABLE [app].[PersonAddress]
ADD CONSTRAINT [DF_PersonAddress_Guid] DEFAULT (NEWID()) FOR [PersonAddressGuid]
GO

ALTER TABLE [app].[PersonAddress]
ADD CONSTRAINT [DF_PersonAddress_Status] DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [app].[PersonAddress]
ADD CONSTRAINT [DF_PersonAddress_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime]
GO

ALTER TABLE [app].[PersonAddress] WITH CHECK
ADD CONSTRAINT [CHK_PersonAddress_Status]
CHECK ([StatusId] IN (0, 1, 2))
GO

CREATE NONCLUSTERED INDEX [IX_PersonAddress_Person]
ON [app].[PersonAddress] ([PersonId])
GO
