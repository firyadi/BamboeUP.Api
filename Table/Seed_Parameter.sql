USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

PRINT '=== Seeding Data: [app].[Parameter] ==='

-- Cek apakah parameter sudah ada, jika belum, Insert.
IF NOT EXISTS (SELECT 1 FROM [app].[Parameter] WHERE ParameterName = 'MaxResultRecord')
BEGIN
    INSERT INTO [app].[Parameter]
    (
        [ParameterGuid], 
        [ParameterName], 
        [ParameterValue], 
        [ParameterType], 
        [StatusId], 
        [CreatedById], 
        [CreatedTime]
    )
    VALUES
    (
        NEWID(), 
        'MaxResultRecord', 
        '150',             -- Value 150
        'int',             -- Tipe Data Int
        1,                 -- Status Aktif
        0,                 -- System User (CreatedBy)
        GETUTCDATE()
    )
    
    PRINT '  -> Inserted Parameter: MaxResultRecord = 150 (Berlaku Global untuk semua Company)'
END
ELSE
BEGIN
    PRINT '  -> Parameter: MaxResultRecord sudah ada, skipped.'
END
GO
