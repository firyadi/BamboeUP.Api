CREATE TABLE [app].[StandardReferenceItem]
(
    [StandardReferenceItemId]          BIGINT IDENTITY(1,1) NOT NULL,
    [StandardReferenceItemGuid]        UNIQUEIDENTIFIER NOT NULL,

    [StandardReferenceId]              BIGINT NOT NULL,
    [StandardReferenceGuid]            UNIQUEIDENTIFIER NOT NULL,

    [StandardReferenceItemInitial]     VARCHAR(50) NOT NULL,
    [StandardReferenceItemName]        VARCHAR(100) NOT NULL,
    [StandardReferenceItemValue]       VARCHAR(100) NULL,

    [DisplayOrder]                     INT NOT NULL,

    [StatusId]                         INT NOT NULL,
    [RowVersion]                       ROWVERSION NOT NULL,

    [CreatedById]                      BIGINT NOT NULL,
    [CreatedTime]                      DATETIME2(7) NOT NULL,

    [UpdatedById]                      BIGINT NULL,
    [UpdatedTime]                      DATETIME2(7) NULL,

    [DeletedById]                      BIGINT NULL,
    [DeletedTime]                      DATETIME2(7) NULL,

    CONSTRAINT PK_StandardReferenceItem
        PRIMARY KEY(StandardReferenceItemId),

    CONSTRAINT FK_StandardReferenceItem_StandardReference
        FOREIGN KEY(StandardReferenceId)
        REFERENCES [app].[StandardReference](StandardReferenceId)
);

CREATE UNIQUE INDEX UX_StandardReferenceItem
ON app.StandardReferenceItem
(
    StandardReferenceId,
    StandardReferenceItemInitial
);