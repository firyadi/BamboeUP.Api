-- ============================================================
-- Table: app.DocumentNumberRequest
-- Desc : Menyimpan request dan detail penggunaan nomor dokumen
-- ============================================================

IF NOT EXISTS (
    SELECT 1 FROM sys.tables t
    INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
    WHERE s.name = 'app' AND t.name = 'DocumentNumberRequest'
)
BEGIN
    CREATE TABLE [app].[DocumentNumberRequest] (
        -- Identity
        [DocumentNumberRequestId]   BIGINT          IDENTITY(1,1) NOT NULL,
        [DocumentNumberRequestGuid] UNIQUEIDENTIFIER              NOT NULL DEFAULT NEWID(),

        -- Core
        [DocumentType]              VARCHAR(50)                   NOT NULL, -- INVOICE, SURAT, dll
        [DocumentNo]                VARCHAR(100)                  NOT NULL,
        [AutoNumberLogId]           BIGINT                        NOT NULL,

        -- External / Flexible
        [ExternalReference]         VARCHAR(100)                      NULL, -- nomor dari sistem luar
        [Description]               VARCHAR(255)                      NULL,

        -- Status
        [Status]                    VARCHAR(20)                   NOT NULL DEFAULT 'Used', 
        -- Draft / Used / Void

        -- Dimension (optional tapi disarankan, biar konsisten dengan counter)
        [CompanyId]                 BIGINT                            NULL,
        [OfficeId]                  BIGINT                            NULL,
        [OrgUnitId]                 BIGINT                            NULL,

        -- Audit
        [CreatedById]               BIGINT                        NOT NULL DEFAULT 0,
        [CreatedTime]               DATETIME                      NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedById]               BIGINT                            NULL,
        [UpdatedTime]               DATETIME                          NULL,

        -- Constraints
        CONSTRAINT [PK_DocumentNumberRequest] PRIMARY KEY CLUSTERED ([DocumentNumberRequestId] ASC),
        CONSTRAINT [UQ_DocumentNumberRequest_Guid] UNIQUE ([DocumentNumberRequestGuid]),
        CONSTRAINT [UQ_DocumentNumberRequest_DocumentNo] UNIQUE ([DocumentNo]),

        CONSTRAINT [FK_DocumentNumberRequest_AutoNumberLog] 
            FOREIGN KEY ([AutoNumberLogId]) 
            REFERENCES [app].[AutoNumberLog] ([AutoNumberLogId])
    );

    -- Cari berdasarkan nomor
    CREATE NONCLUSTERED INDEX [IX_DocumentNumberRequest_DocumentNo]
        ON [app].[DocumentNumberRequest] ([DocumentNo] ASC);

    -- Filter per jenis dokumen
    CREATE NONCLUSTERED INDEX [IX_DocumentNumberRequest_DocumentType]
        ON [app].[DocumentNumberRequest] ([DocumentType] ASC);

    -- Audit / reporting
    CREATE NONCLUSTERED INDEX [IX_DocumentNumberRequest_CreatedTime]
        ON [app].[DocumentNumberRequest] ([CreatedTime] ASC);

    -- Relasi ke log
    CREATE NONCLUSTERED INDEX [IX_DocumentNumberRequest_AutoNumberLogId]
        ON [app].[DocumentNumberRequest] ([AutoNumberLogId] ASC);

    PRINT 'Table app.DocumentNumberRequest created successfully.';
END
ELSE
BEGIN
    PRINT 'Table app.DocumentNumberRequest already exists. Skipped.';
END
GO
