USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

PRINT '=== Seeding StandardReference: ContractType / ContractStatus ==='
GO

DECLARE @RefId BIGINT;
DECLARE @RefGuid UNIQUEIDENTIFIER;

-- --------------------------------------------------
-- 1. ContractType  (SrContractType)
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'ContractType')
BEGIN
    INSERT INTO [app].[StandardReference]
        ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES
        (NEWID(), 'Contract Type', 'ContractType', 'Contract type reference values', 1, 0, GETUTCDATE());
END

SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid]
FROM [app].[StandardReference]
WHERE [StandardReferenceInitial] = 'ContractType';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'PKWT')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'PKWT', 'PKWT (Contract)', 'PKWT', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'PKWTT')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'PKWTT', 'PKWTT (Permanent)', 'PKWTT', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'INTERN')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'INTERN', 'Internship', 'Internship', 3, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 2. ContractStatus  (SrContractStatus)
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'ContractStatus')
BEGIN
    INSERT INTO [app].[StandardReference]
        ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES
        (NEWID(), 'Contract Status', 'ContractStatus', 'Contract status reference values', 1, 0, GETUTCDATE());
END

SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid]
FROM [app].[StandardReference]
WHERE [StandardReferenceInitial] = 'ContractStatus';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'DRAFT')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'DRAFT', 'Draft', 'Draft', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'ACTIVE')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'ACTIVE', 'Active', 'Active', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'EXPIRED')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'EXPIRED', 'Expired', 'Expired', 3, 1, 0, GETUTCDATE());

PRINT '=== Done: Contract StandardReference seed ==='
GO
