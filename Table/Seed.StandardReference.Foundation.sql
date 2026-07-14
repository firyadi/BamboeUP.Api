USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

PRINT '=== Seeding Data: [app].[StandardReference] & [app].[StandardReferenceItem] ==='
GO

-- --------------------------------------------------
-- Helper variables for ID mapping
-- --------------------------------------------------
DECLARE @RefId BIGINT;
DECLARE @RefGuid UNIQUEIDENTIFIER;

-- --------------------------------------------------
-- 1. Gender
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'GENDER')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Gender', 'GENDER', 'Gender reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'GENDER';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'M')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'M', 'Male', 'Male', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'F')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'F', 'Female', 'Female', 2, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 2. Religion
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'RELIGION')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Religion', 'RELIGION', 'Religion reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'RELIGION';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'ISL')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'ISL', 'Islam', 'Islam', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'CHR')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'CHR', 'Christian', 'Christian', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'CAT')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'CAT', 'Catholic', 'Catholic', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'HIN')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'HIN', 'Hindu', 'Hindu', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'BUD')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'BUD', 'Buddhist', 'Buddhist', 5, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'CON')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'CON', 'Confucian', 'Confucian', 6, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 3. Nationality
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'NATIONALITY')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Nationality', 'NATIONALITY', 'Nationality reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'NATIONALITY';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'ID')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'ID', 'Indonesia', 'Indonesia', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'FOR')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'FOR', 'Foreign National', 'Foreign National', 2, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 4. Language
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'LANGUAGE')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Language', 'LANGUAGE', 'Language reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'LANGUAGE';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'ID')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'ID', 'Bahasa Indonesia', 'Bahasa Indonesia', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'EN')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'EN', 'English', 'English', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'ZH')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'ZH', 'Mandarin', 'Mandarin', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'JA')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'JA', 'Japanese', 'Japanese', 4, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 5. Currency
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'CURRENCY')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Currency', 'CURRENCY', 'Currency reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'CURRENCY';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'IDR')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'IDR', 'Rupiah', 'Rupiah', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'USD')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'USD', 'US Dollar', 'US Dollar', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'EUR')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'EUR', 'Euro', 'Euro', 3, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 6. Identification Type
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'IDENTIFICATION_TYPE')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Identification Type', 'IDENTIFICATION_TYPE', 'Identification Type reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'IDENTIFICATION_TYPE';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'EMPNO')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'EMPNO', 'Employee Number', 'Employee Number', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'NIK')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'NIK', 'National ID', 'National ID', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'PASS')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'PASS', 'Passport', 'Passport', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'NPWP')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'NPWP', 'Tax ID', 'Tax ID', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'BPJSKES')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'BPJSKES', 'BPJS Kesehatan', 'BPJS Kesehatan', 5, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'BPJSTK')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'BPJSTK', 'BPJS Ketenagakerjaan', 'BPJS Ketenagakerjaan', 6, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SIMA')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SIMA', 'Driver License A', 'Driver License A', 7, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SIMB')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SIMB', 'Driver License B', 'Driver License B', 8, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SIMC')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SIMC', 'Driver License C', 'Driver License C', 9, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'KITAS')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'KITAS', 'KITAS', 'KITAS', 10, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'KITAP')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'KITAP', 'KITAP', 'KITAP', 11, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'KK')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'KK', 'Family Card', 'Family Card', 12, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 7. Blood Type
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'BLOOD_TYPE')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Blood Type', 'BLOOD_TYPE', 'Blood Type reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'BLOOD_TYPE';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'A')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'A', 'A', 'A', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'B')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'B', 'B', 'B', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'AB')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'AB', 'AB', 'AB', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'O')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'O', 'O', 'O', 4, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 8. Marital Status
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'MARITAL_STATUS')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Marital Status', 'MARITAL_STATUS', 'Marital Status reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'MARITAL_STATUS';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SINGLE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SINGLE', 'Single', 'Single', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'MARRIED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'MARRIED', 'Married', 'Married', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'DIVORCED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'DIVORCED', 'Divorced', 'Divorced', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'WIDOWED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'WIDOWED', 'Widowed', 'Widowed', 4, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 9. Physical Characteristic
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'PHYSICAL_CHARACTERISTIC')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Physical Characteristic', 'PHYSICAL_CHARACTERISTIC', 'Physical Characteristic reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'PHYSICAL_CHARACTERISTIC';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'HEIGHT')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'HEIGHT', 'Height', 'Height', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'WEIGHT')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'WEIGHT', 'Weight', 'Weight', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'BLOOD_TYPE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'BLOOD_TYPE', 'Blood Type', 'Blood Type', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'EYE_COLOR')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'EYE_COLOR', 'Eye Color', 'Eye Color', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'HAIR_COLOR')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'HAIR_COLOR', 'Hair Color', 'Hair Color', 5, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SHIRT_SIZE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SHIRT_SIZE', 'Shirt Size', 'Shirt Size', 6, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SHOE_SIZE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SHOE_SIZE', 'Shoe Size', 'Shoe Size', 7, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'WAIST_SIZE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'WAIST_SIZE', 'Waist Size', 'Waist Size', 8, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'BMI')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'BMI', 'BMI', 'BMI', 9, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 10. Measurement Unit
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'MEASUREMENT_UNIT')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Measurement Unit', 'MEASUREMENT_UNIT', 'Measurement Unit reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'MEASUREMENT_UNIT';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'CM')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'CM', 'cm', 'cm', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'KG')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'KG', 'kg', 'kg', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'M')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'M', 'm', 'm', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'INCH')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'INCH', 'inch', 'inch', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'EU_SIZE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'EU_SIZE', 'EU Size', 'EU Size', 5, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 11. Address Type
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'ADDRESS_TYPE')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Address Type', 'ADDRESS_TYPE', 'Address Type reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'ADDRESS_TYPE';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'HOME')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'HOME', 'Home', 'Home', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'DOMICILE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'DOMICILE', 'Domicile', 'Domicile', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'OFFICE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'OFFICE', 'Office', 'Office', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'MAILING')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'MAILING', 'Mailing', 'Mailing', 4, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 12. Contact Type
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'CONTACT_TYPE')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Contact Type', 'CONTACT_TYPE', 'Contact Type reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'CONTACT_TYPE';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'MOBILE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'MOBILE', 'Mobile', 'Mobile', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'HOME')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'HOME', 'Home', 'Home', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'OFFICE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'OFFICE', 'Office', 'Office', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'EMERGENCY')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'EMERGENCY', 'Emergency', 'Emergency', 4, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 13. Relationship
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'RELATIONSHIP')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Relationship', 'RELATIONSHIP', 'Relationship reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'RELATIONSHIP';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'FATHER')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'FATHER', 'Father', 'Father', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'MOTHER')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'MOTHER', 'Mother', 'Mother', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'HUSBAND')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'HUSBAND', 'Husband', 'Husband', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'WIFE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'WIFE', 'Wife', 'Wife', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'CHILD')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'CHILD', 'Child', 'Child', 5, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'BROTHER')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'BROTHER', 'Brother', 'Brother', 6, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SISTER')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SISTER', 'Sister', 'Sister', 7, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'FRIEND')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'FRIEND', 'Friend', 'Friend', 8, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 14. Employment Type
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'EMPLOYMENT_TYPE')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Employment Type', 'EMPLOYMENT_TYPE', 'Employment Type reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'EMPLOYMENT_TYPE';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'PERMANENT')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'PERMANENT', 'Permanent', 'Permanent', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'CONTRACT')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'CONTRACT', 'Contract', 'Contract', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'INTERNSHIP')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'INTERNSHIP', 'Internship', 'Internship', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'FREELANCE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'FREELANCE', 'Freelance', 'Freelance', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'CONSULTANT')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'CONSULTANT', 'Consultant', 'Consultant', 5, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'PART_TIME')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'PART_TIME', 'Part Time', 'Part Time', 6, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'FULL_TIME')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'FULL_TIME', 'Full Time', 'Full Time', 7, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 15. Employment Status
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'EMPLOYMENT_STATUS')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Employment Status', 'EMPLOYMENT_STATUS', 'Employment Status reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'EMPLOYMENT_STATUS';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'ACTIVE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'ACTIVE', 'Active', 'Active', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'PROBATION')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'PROBATION', 'Probation', 'Probation', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'LEAVE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'LEAVE', 'Leave', 'Leave', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SUSPENDED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SUSPENDED', 'Suspended', 'Suspended', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'RESIGNED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'RESIGNED', 'Resigned', 'Resigned', 5, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'RETIRED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'RETIRED', 'Retired', 'Retired', 6, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'TERMINATED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'TERMINATED', 'Terminated', 'Terminated', 7, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 16. Industry
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'INDUSTRY')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Industry', 'INDUSTRY', 'Industry reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'INDUSTRY';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'MANUFACTURING')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'MANUFACTURING', 'Manufacturing', 'Manufacturing', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'BANKING')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'BANKING', 'Banking', 'Banking', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'INFORMATION_TECHNOLOGY')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'INFORMATION_TECHNOLOGY', 'Information Technology', 'Information Technology', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'MINING')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'MINING', 'Mining', 'Mining', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'OIL_GAS')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'OIL_GAS', 'Oil & Gas', 'Oil & Gas', 5, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'RETAIL')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'RETAIL', 'Retail', 'Retail', 6, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'HEALTHCARE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'HEALTHCARE', 'Healthcare', 'Healthcare', 7, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'EDUCATION')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'EDUCATION', 'Education', 'Education', 8, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'LOGISTICS')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'LOGISTICS', 'Logistics', 'Logistics', 9, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'GOVERNMENT')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'GOVERNMENT', 'Government', 'Government', 10, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 17. Job Level
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'JOB_LEVEL')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Job Level', 'JOB_LEVEL', 'Job Level reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'JOB_LEVEL';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'STAFF')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'STAFF', 'Staff', 'Staff', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SENIOR_STAFF')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SENIOR_STAFF', 'Senior Staff', 'Senior Staff', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SUPERVISOR')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SUPERVISOR', 'Supervisor', 'Supervisor', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'ASSISTANT_MANAGER')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'ASSISTANT_MANAGER', 'Assistant Manager', 'Assistant Manager', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'MANAGER')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'MANAGER', 'Manager', 'Manager', 5, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SENIOR_MANAGER')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SENIOR_MANAGER', 'Senior Manager', 'Senior Manager', 6, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'GENERAL_MANAGER')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'GENERAL_MANAGER', 'General Manager', 'General Manager', 7, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'DIRECTOR')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'DIRECTOR', 'Director', 'Director', 8, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'COMMISSIONER')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'COMMISSIONER', 'Commissioner', 'Commissioner', 9, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 18. Grade
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'GRADE')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Grade', 'GRADE', 'Grade reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'GRADE';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'G01')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'G01', 'G01', 'G01', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'G02')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'G02', 'G02', 'G02', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'G03')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'G03', 'G03', 'G03', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'G04')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'G04', 'G04', 'G04', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'G05')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'G05', 'G05', 'G05', 5, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'G06')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'G06', 'G06', 'G06', 6, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'G07')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'G07', 'G07', 'G07', 7, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'G08')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'G08', 'G08', 'G08', 8, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'G09')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'G09', 'G09', 'G09', 9, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'G10')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'G10', 'G10', 'G10', 10, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 19. Position Type
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'POSITION_TYPE')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Position Type', 'POSITION_TYPE', 'Position Type reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'POSITION_TYPE';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'STRUCTURAL')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'STRUCTURAL', 'Structural', 'Structural', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'FUNCTIONAL')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'FUNCTIONAL', 'Functional', 'Functional', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'OPERATIONAL')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'OPERATIONAL', 'Operational', 'Operational', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'PROJECT')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'PROJECT', 'Project', 'Project', 4, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 20. Company Type
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'COMPANY_TYPE')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Company Type', 'COMPANY_TYPE', 'Company Type reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'COMPANY_TYPE';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'HEAD_OFFICE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'HEAD_OFFICE', 'Head Office', 'Head Office', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'BRANCH')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'BRANCH', 'Branch', 'Branch', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SUBSIDIARY')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SUBSIDIARY', 'Subsidiary', 'Subsidiary', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'REPRESENTATIVE_OFFICE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'REPRESENTATIVE_OFFICE', 'Representative Office', 'Representative Office', 4, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 21. Cost Center Type
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'COST_CENTER_TYPE')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Cost Center Type', 'COST_CENTER_TYPE', 'Cost Center Type reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'COST_CENTER_TYPE';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'REVENUE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'REVENUE', 'Revenue', 'Revenue', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'EXPENSE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'EXPENSE', 'Expense', 'Expense', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'PROFIT_CENTER')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'PROFIT_CENTER', 'Profit Center', 'Profit Center', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'INVESTMENT')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'INVESTMENT', 'Investment', 'Investment', 4, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 22. Organization Unit Type
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'ORGANIZATION_UNIT_TYPE')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Organization Unit Type', 'ORGANIZATION_UNIT_TYPE', 'Organization Unit Type reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'ORGANIZATION_UNIT_TYPE';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'COMPANY')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'COMPANY', 'Company', 'Company', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'DIVISION')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'DIVISION', 'Division', 'Division', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'DEPARTMENT')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'DEPARTMENT', 'Department', 'Department', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SECTION')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SECTION', 'Section', 'Section', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'TEAM')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'TEAM', 'Team', 'Team', 5, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 23. Payroll Frequency
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'PAYROLL_FREQUENCY')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Payroll Frequency', 'PAYROLL_FREQUENCY', 'Payroll Frequency reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'PAYROLL_FREQUENCY';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'WEEKLY')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'WEEKLY', 'Weekly', 'Weekly', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'BI_WEEKLY')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'BI_WEEKLY', 'Bi Weekly', 'Bi Weekly', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'MONTHLY')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'MONTHLY', 'Monthly', 'Monthly', 3, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 24. Tax Status
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'TAX_STATUS')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Tax Status', 'TAX_STATUS', 'Tax Status reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'TAX_STATUS';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'TK0')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'TK0', 'TK0', 'TK0', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'TK1')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'TK1', 'TK1', 'TK1', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'TK2')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'TK2', 'TK2', 'TK2', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'TK3')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'TK3', 'TK3', 'TK3', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'K0')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'K0', 'K0', 'K0', 5, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'K1')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'K1', 'K1', 'K1', 6, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'K2')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'K2', 'K2', 'K2', 7, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'K3')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'K3', 'K3', 'K3', 8, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 25. Applicant Status
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'APPLICANT_STATUS')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Applicant Status', 'APPLICANT_STATUS', 'Applicant Status reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'APPLICANT_STATUS';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'APPLIED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'APPLIED', 'Applied', 'Applied', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SCREENING')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SCREENING', 'Screening', 'Screening', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'INTERVIEW')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'INTERVIEW', 'Interview', 'Interview', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'OFFERED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'OFFERED', 'Offered', 'Offered', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'HIRED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'HIRED', 'Hired', 'Hired', 5, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'REJECTED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'REJECTED', 'Rejected', 'Rejected', 6, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 26. Source of Hire
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'SOURCE_OF_HIRE')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Source of Hire', 'SOURCE_OF_HIRE', 'Source of Hire reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'SOURCE_OF_HIRE';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'JOB_PORTAL')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'JOB_PORTAL', 'Job Portal', 'Job Portal', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'COMPANY_WEBSITE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'COMPANY_WEBSITE', 'Company Website', 'Company Website', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'REFERRAL')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'REFERRAL', 'Referral', 'Referral', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'CAMPUS')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'CAMPUS', 'Campus', 'Campus', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SOCIAL_MEDIA')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SOCIAL_MEDIA', 'Social Media', 'Social Media', 5, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'WALK_IN')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'WALK_IN', 'Walk In', 'Walk In', 6, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'RECRUITMENT_AGENCY')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'RECRUITMENT_AGENCY', 'Recruitment Agency', 'Recruitment Agency', 7, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 27. Interview Result
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'INTERVIEW_RESULT')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Interview Result', 'INTERVIEW_RESULT', 'Interview Result reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'INTERVIEW_RESULT';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'STRONG_HIRE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'STRONG_HIRE', 'Strong Hire', 'Strong Hire', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'HIRE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'HIRE', 'Hire', 'Hire', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'CONSIDER')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'CONSIDER', 'Consider', 'Consider', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'REJECT')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'REJECT', 'Reject', 'Reject', 4, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 28. KPI Category
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'KPI_CATEGORY')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'KPI Category', 'KPI_CATEGORY', 'KPI Category reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'KPI_CATEGORY';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'FINANCIAL')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'FINANCIAL', 'Financial', 'Financial', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'CUSTOMER')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'CUSTOMER', 'Customer', 'Customer', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'INTERNAL_PROCESS')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'INTERNAL_PROCESS', 'Internal Process', 'Internal Process', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'LEARNING_GROWTH')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'LEARNING_GROWTH', 'Learning & Growth', 'Learning & Growth', 4, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 29. Competency Level
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'COMPETENCY_LEVEL')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Competency Level', 'COMPETENCY_LEVEL', 'Competency Level reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'COMPETENCY_LEVEL';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'BEGINNER')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'BEGINNER', 'Beginner', 'Beginner', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'INTERMEDIATE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'INTERMEDIATE', 'Intermediate', 'Intermediate', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'ADVANCED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'ADVANCED', 'Advanced', 'Advanced', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'EXPERT')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'EXPERT', 'Expert', 'Expert', 4, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 30. Rating Scale
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'RATING_SCALE')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Rating Scale', 'RATING_SCALE', 'Rating Scale reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'RATING_SCALE';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = '1')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, '1', '1', '1', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = '2')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, '2', '2', '2', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = '3')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, '3', '3', '3', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = '4')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, '4', '4', '4', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = '5')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, '5', '5', '5', 5, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 31. Course Type
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'COURSE_TYPE')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Course Type', 'COURSE_TYPE', 'Course Type reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'COURSE_TYPE';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'ONLINE')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'ONLINE', 'Online', 'Online', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'CLASSROOM')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'CLASSROOM', 'Classroom', 'Classroom', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'WORKSHOP')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'WORKSHOP', 'Workshop', 'Workshop', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SEMINAR')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SEMINAR', 'Seminar', 'Seminar', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'CERTIFICATION')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'CERTIFICATION', 'Certification', 'Certification', 5, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 32. Certificate Type
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'CERTIFICATE_TYPE')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Certificate Type', 'CERTIFICATE_TYPE', 'Certificate Type reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'CERTIFICATE_TYPE';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'INTERNAL')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'INTERNAL', 'Internal', 'Internal', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'EXTERNAL')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'EXTERNAL', 'External', 'External', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'PROFESSIONAL')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'PROFESSIONAL', 'Professional', 'Professional', 3, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 33. Approval Status
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'APPROVAL_STATUS')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Approval Status', 'APPROVAL_STATUS', 'Approval Status reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'APPROVAL_STATUS';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'DRAFT')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'DRAFT', 'Draft', 'Draft', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SUBMITTED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SUBMITTED', 'Submitted', 'Submitted', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'APPROVED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'APPROVED', 'Approved', 'Approved', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'REJECTED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'REJECTED', 'Rejected', 'Rejected', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'CANCELLED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'CANCELLED', 'Cancelled', 'Cancelled', 5, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 34. Workflow Status
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'WORKFLOW_STATUS')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Workflow Status', 'WORKFLOW_STATUS', 'Workflow Status reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'WORKFLOW_STATUS';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'NEW')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'NEW', 'New', 'New', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'IN_PROGRESS')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'IN_PROGRESS', 'In Progress', 'In Progress', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'WAITING_APPROVAL')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'WAITING_APPROVAL', 'Waiting Approval', 'Waiting Approval', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'COMPLETED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'COMPLETED', 'Completed', 'Completed', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'CANCELLED')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'CANCELLED', 'Cancelled', 'Cancelled', 5, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 35. Notification Type
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'NOTIFICATION_TYPE')
BEGIN
    INSERT INTO [app].[StandardReference] ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), 'Notification Type', 'NOTIFICATION_TYPE', 'Notification Type reference values', 1, 0, GETUTCDATE());
END
SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid] FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'NOTIFICATION_TYPE';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'EMAIL')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'EMAIL', 'Email', 'Email', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SMS')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SMS', 'SMS', 'SMS', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'WHATSAPP')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'WHATSAPP', 'WhatsApp', 'WhatsApp', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'PUSH_NOTIFICATION')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'PUSH_NOTIFICATION', 'Push Notification', 'Push Notification', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'IN_APP')
    INSERT INTO [app].[StandardReferenceItem] ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'IN_APP', 'In App', 'In App', 5, 1, 0, GETUTCDATE());

PRINT '=== Seeding Completed Successfully! ==='
GO
