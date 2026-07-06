CREATE TABLE [app].[StandardReference]
(
    [StandardReferenceId]          BIGINT IDENTITY(1,1) NOT NULL,
    [StandardReferenceGuid]        UNIQUEIDENTIFIER NOT NULL,

    [StandardReferenceName]        VARCHAR(100) NOT NULL,
    [StandardReferenceInitial]     VARCHAR(50) NOT NULL,
    [Description]                  VARCHAR(500) NULL,

    [StatusId]                     INT NOT NULL,
    [RowVersion]                   ROWVERSION NOT NULL,

    [CreatedById]                  BIGINT NOT NULL,
    [CreatedTime]                  DATETIME2(7) NOT NULL,

    [UpdatedById]                  BIGINT NULL,
    [UpdatedTime]                  DATETIME2(7) NULL,

    [DeletedById]                  BIGINT NULL,
    [DeletedTime]                  DATETIME2(7) NULL,

    CONSTRAINT PK_StandardReference
        PRIMARY KEY(StandardReferenceId),

    CONSTRAINT UQ_StandardReference_StandardReferenceInitial
        UNIQUE(StandardReferenceInitial)
);

ALTER TABLE [app].[StandardReference]
ADD CONSTRAINT DF_StandardReference_StandardReferenceGuid
DEFAULT NEWID() FOR StandardReferenceGuid;

ALTER TABLE [app].[StandardReference]
ADD CONSTRAINT DF_StandardReference_StatusId
DEFAULT(1) FOR StatusId;

ALTER TABLE [app].[StandardReference]
ADD CONSTRAINT DF_StandardReference_CreatedTime
DEFAULT(sysdatetime()) FOR CreatedTime;