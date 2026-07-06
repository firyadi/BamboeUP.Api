-- ============================================================
-- Table: app.AutoNumberLog
-- Desc : Audit log setiap nomor yang di-generate oleh engine
-- ============================================================

IF NOT EXISTS (
    SELECT 1 FROM sys.tables t
    INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
    WHERE s.name = 'app' AND t.name = 'AutoNumberLog'
)
BEGIN
    CREATE TABLE [app].[AutoNumberLog] (
        -- PK
        [AutoNumberLogId]      BIGINT          IDENTITY(1,1)  NOT NULL,
        [AutoNumberLogGuid]    UNIQUEIDENTIFIER               NOT NULL  DEFAULT NEWID(),

        -- FK ke Template
        [AutoNumberTemplateId] BIGINT                         NOT NULL,

        -- Hasil generate
        [GeneratedNumber]      VARCHAR(200)                   NOT NULL,
        [CounterValue]         INT                            NOT NULL,
        [Status]               VARCHAR(20)                    NOT NULL  DEFAULT 'Used',
        -- 'Used' | 'Cancelled' | 'Voided'
        [ReferenceId]          NVARCHAR(100)                      NULL,

        -- Scope saat generate (untuk multi-tenant filtering)
        [CompanyId]            BIGINT                             NULL,
        [CompanyOfficeId]      BIGINT                             NULL,
        [OrganizationUnitId]   BIGINT                             NULL,
        [YearNo]               INT                                NULL,
        [MonthNo]              INT                                NULL,
        [DayNo]                INT                                NULL,

        -- Audit
        [CreatedById]          BIGINT                         NOT NULL  DEFAULT 0,
        [CreatedTime]          DATETIME                       NOT NULL  DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_AutoNumberLog] PRIMARY KEY CLUSTERED ([AutoNumberLogId] ASC),
        CONSTRAINT [UQ_AutoNumberLog_Guid] UNIQUE ([AutoNumberLogGuid]),
        CONSTRAINT [FK_AutoNumberLog_Template]
            FOREIGN KEY ([AutoNumberTemplateId])
            REFERENCES [app].[AutoNumberTemplate] ([AutoNumberTemplateId])
    );

    -- Index untuk query log by Template
    CREATE NONCLUSTERED INDEX [IX_AutoNumberLog_TemplateId]
        ON [app].[AutoNumberLog] ([AutoNumberTemplateId] ASC)
        INCLUDE ([GeneratedNumber], [CounterValue], [CreatedTime]);

    -- Index untuk query log by Scope
    CREATE NONCLUSTERED INDEX [IX_AutoNumberLog_Scope]
        ON [app].[AutoNumberLog] ([AutoNumberTemplateId], [CompanyId], [CompanyOfficeId], [YearNo], [MonthNo]);

    PRINT 'Table app.AutoNumberLog created successfully.';
END
ELSE
BEGIN
    PRINT 'Table app.AutoNumberLog already exists. Skipped.';
END
GO
