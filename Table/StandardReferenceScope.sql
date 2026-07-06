CREATE TABLE [app].[StandardReferenceScope]
(
    [StandardReferenceScopeId]        BIGINT IDENTITY(1,1) NOT NULL,
    [StandardReferenceScopeGuid]      UNIQUEIDENTIFIER NOT NULL,

    [StandardReferenceId]             BIGINT NOT NULL,
    [StandardReferenceGuid]           UNIQUEIDENTIFIER NOT NULL,

    [CompanyId]                       BIGINT NULL,
    [CompanyGuid]                     UNIQUEIDENTIFIER NULL,

    [CompanyOfficeId]                 BIGINT NULL,
    [CompanyOfficeGuid]               UNIQUEIDENTIFIER NULL,

    [StatusId]                        INT NOT NULL,
    [RowVersion]                      ROWVERSION NOT NULL,

    [CreatedById]                     BIGINT NOT NULL,
    [CreatedTime]                     DATETIME2(7) NOT NULL,

    [UpdatedById]                     BIGINT NULL,
    [UpdatedTime]                     DATETIME2(7) NULL,

    [DeletedById]                     BIGINT NULL,
    [DeletedTime]                     DATETIME2(7) NULL,

    CONSTRAINT PK_StandardReferenceScope
        PRIMARY KEY(StandardReferenceScopeId),

    CONSTRAINT FK_StandardReferenceScope_StandardReference
        FOREIGN KEY(StandardReferenceId)
        REFERENCES app.StandardReference(StandardReferenceId)
);

CREATE UNIQUE INDEX UX_StandardReferenceScope
ON app.StandardReferenceScope
(
    StandardReferenceId,
    CompanyId,
    CompanyOfficeId
);