USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

PRINT '=== Seeding StandardReference: AwardType / RuleOperator ==='
GO

-- SrAwardType on Award header → initial AwardType
-- SrRuleOperator on AwardCriteriaRule → initial RuleOperator

DECLARE @RefId BIGINT;
DECLARE @RefGuid UNIQUEIDENTIFIER;

-- --------------------------------------------------
-- 1. AwardType  (SrAwardType)
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'AwardType')
BEGIN
    INSERT INTO [app].[StandardReference]
        ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES
        (NEWID(), 'Award Type', 'AwardType', 'Award category/type for [app].[Award].SrAwardType', 1, 0, GETUTCDATE());
END

SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid]
FROM [app].[StandardReference]
WHERE [StandardReferenceInitial] = 'AwardType';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'ANNUAL')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'ANNUAL', 'Annual', 'Annual', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SPOT')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SPOT', 'Spot Award', 'Spot Award', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SERVICE')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SERVICE', 'Service / Tenure', 'Service / Tenure', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'TEAM')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'TEAM', 'Team Award', 'Team Award', 4, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 2. RuleOperator  (SrRuleOperator on AwardCriteriaRule)
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'RuleOperator')
BEGIN
    INSERT INTO [app].[StandardReference]
        ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES
        (NEWID(), 'Rule Operator', 'RuleOperator', 'Comparison operator for [app].[AwardCriteriaRule].SrRuleOperator', 1, 0, GETUTCDATE());
END

SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid]
FROM [app].[StandardReference]
WHERE [StandardReferenceInitial] = 'RuleOperator';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'Gte')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'Gte', 'Greater or Equal (>=)', 'Gte', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'Lte')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'Lte', 'Less or Equal (<=)', 'Lte', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'Eq')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'Eq', 'Equal (=)', 'Eq', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'Neq')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'Neq', 'Not Equal (<>)', 'Neq', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'Between')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'Between', 'Between (use ThresholdValue + ThresholdValueTo)', 'Between', 5, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'Contains')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'Contains', 'Contains text', 'Contains', 6, 1, 0, GETUTCDATE());

PRINT '=== Seed AwardType / RuleOperator completed ==='
GO
