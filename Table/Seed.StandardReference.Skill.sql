USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

PRINT '=== Seeding StandardReference: SkillCategory / SkillType / DefaultSkillLevel ==='
GO

-- Matches UI/API lookup: standardReferenceInitial = column without "Sr" prefix
--   SrSkillCategory      → SkillCategory
--   SrSkillType          → SkillType
--   SrDefaultSkillLevel  → DefaultSkillLevel

DECLARE @RefId BIGINT;
DECLARE @RefGuid UNIQUEIDENTIFIER;

-- --------------------------------------------------
-- 1. SkillCategory  (SrSkillCategory)
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'SkillCategory')
BEGIN
    INSERT INTO [app].[StandardReference]
        ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES
        (NEWID(), 'Skill Category', 'SkillCategory', 'Skill category reference values for [app].[Skill].SrSkillCategory', 1, 0, GETUTCDATE());
END

SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid]
FROM [app].[StandardReference]
WHERE [StandardReferenceInitial] = 'SkillCategory';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'TECH')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'TECH', 'Technical', 'Technical', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'SOFT')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'SOFT', 'Soft Skill', 'Soft Skill', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'LANG')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'LANG', 'Language', 'Language', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'MGMT')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'MGMT', 'Management', 'Management', 4, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'CERT')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'CERT', 'Certification', 'Certification', 5, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 2. SkillType  (SrSkillType)
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'SkillType')
BEGIN
    INSERT INTO [app].[StandardReference]
        ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES
        (NEWID(), 'Skill Type', 'SkillType', 'Skill type reference values for [app].[Skill].SrSkillType', 1, 0, GETUTCDATE());
END

SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid]
FROM [app].[StandardReference]
WHERE [StandardReferenceInitial] = 'SkillType';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'CORE')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'CORE', 'Core', 'Core', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'FUNC')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'FUNC', 'Functional', 'Functional', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'BEHV')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'BEHV', 'Behavioral', 'Behavioral', 3, 1, 0, GETUTCDATE());

-- --------------------------------------------------
-- 3. DefaultSkillLevel  (SrDefaultSkillLevel)
-- --------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [app].[StandardReference] WHERE [StandardReferenceInitial] = 'DefaultSkillLevel')
BEGIN
    INSERT INTO [app].[StandardReference]
        ([StandardReferenceGuid], [StandardReferenceName], [StandardReferenceInitial], [Description], [StatusId], [CreatedById], [CreatedTime])
    VALUES
        (NEWID(), 'Default Skill Level', 'DefaultSkillLevel', 'Default skill level reference values for [app].[Skill].SrDefaultSkillLevel', 1, 0, GETUTCDATE());
END

SELECT @RefId = [StandardReferenceId], @RefGuid = [StandardReferenceGuid]
FROM [app].[StandardReference]
WHERE [StandardReferenceInitial] = 'DefaultSkillLevel';

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'BEG')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'BEG', 'Beginner', '1', 1, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'INT')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'INT', 'Intermediate', '2', 2, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'ADV')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'ADV', 'Advanced', '3', 3, 1, 0, GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM [app].[StandardReferenceItem] WHERE [StandardReferenceId] = @RefId AND [StandardReferenceItemInitial] = 'EXP')
    INSERT INTO [app].[StandardReferenceItem]
        ([StandardReferenceItemGuid], [StandardReferenceId], [StandardReferenceGuid], [StandardReferenceItemInitial], [StandardReferenceItemName], [StandardReferenceItemValue], [DisplayOrder], [StatusId], [CreatedById], [CreatedTime])
    VALUES (NEWID(), @RefId, @RefGuid, 'EXP', 'Expert', '4', 4, 1, 0, GETUTCDATE());

PRINT '=== Done: Skill StandardReference seed ==='
GO
