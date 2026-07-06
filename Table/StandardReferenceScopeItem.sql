CREATE TABLE [app].[StandardReferenceScopeItem]
(
    [StandardReferenceScopeItemId]          BIGINT IDENTITY(1,1) NOT NULL,
    [StandardReferenceScopeItemGuid]        UNIQUEIDENTIFIER NOT NULL,

    [StandardReferenceScopeId]              BIGINT NOT NULL,
    [StandardReferenceScopeGuid]            UNIQUEIDENTIFIER NOT NULL,

    [StandardReferenceScopeItemInitial]     VARCHAR(50) NOT NULL,
    [StandardReferenceScopeItemName]        VARCHAR(100) NOT NULL,
    [StandardReferenceScopeItemValue]       VARCHAR(100) NULL,

    [DisplayOrder]                          INT NOT NULL,

    [StatusId]                              INT NOT NULL,
    [RowVersion]                            ROWVERSION NOT NULL,

    [CreatedById]                           BIGINT NOT NULL,
    [CreatedTime]                           DATETIME2(7) NOT NULL,

    [UpdatedById]                           BIGINT NULL,
    [UpdatedTime]                           DATETIME2(7) NULL,

    [DeletedById]                           BIGINT NULL,
    [DeletedTime]                           DATETIME2(7) NULL,

    CONSTRAINT PK_StandardReferenceScopeItem
        PRIMARY KEY(StandardReferenceScopeItemId),

    CONSTRAINT FK_StandardReferenceScopeItem_StandardReferenceScope
        FOREIGN KEY(StandardReferenceScopeId)
        REFERENCES app.StandardReferenceScope(StandardReferenceScopeId)
);

CREATE UNIQUE INDEX UX_StandardReferenceScopeItem
ON app.StandardReferenceScopeItem
(
    StandardReferenceScopeId,
    StandardReferenceScopeItemInitial
);