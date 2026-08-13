USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   MIGRATION: Align Award 3-tier (existing tables + header cleanup)

   Dev.BamboeHR already has AwardCriteria / AwardCriteriaRule with FKs.
   This script:
     A. Adds AwardGuid on AwardCriteria (denorm) if missing
     B. Adds AwardCriteriaGuid on AwardCriteriaRule (denorm) if missing
     C. Backfills criteria from SrAwardCriteria ONLY when Award has no criteria rows
     D. Drops Award.SrAwardCriteria
     E. Renames Award.Validto → ValidTo

   Run order:
     1. AwardCriteria.sql / AwardCriteriaRule.sql (no-op if exist)
     2. Seed.StandardReference.Award.sql
     3. This file
   ========================================================= */

PRINT '=== Migration: Award 3-Tier Align ==='
GO

IF OBJECT_ID(N'[app].[AwardCriteria]', N'U') IS NULL
   OR OBJECT_ID(N'[app].[AwardCriteriaRule]', N'U') IS NULL
BEGIN
    RAISERROR('AwardCriteria / AwardCriteriaRule missing. Run create scripts first.', 16, 1);
    RETURN;
END
GO

-- --------------------------------------------------
-- A. AwardCriteria.AwardGuid
-- --------------------------------------------------
IF COL_LENGTH(N'app.AwardCriteria', N'AwardGuid') IS NULL
BEGIN
    ALTER TABLE [app].[AwardCriteria] ADD [AwardGuid] UNIQUEIDENTIFIER NULL;
    PRINT '  -> Added AwardCriteria.AwardGuid (nullable, backfill next).';
END
GO

UPDATE c
SET c.[AwardGuid] = a.[AwardGuid]
FROM [app].[AwardCriteria] c
INNER JOIN [app].[Award] a ON a.[AwardId] = c.[AwardId]
WHERE c.[AwardGuid] IS NULL;

IF COL_LENGTH(N'app.AwardCriteria', N'AwardGuid') IS NOT NULL
   AND EXISTS (
        SELECT 1 FROM sys.columns
        WHERE object_id = OBJECT_ID(N'app.AwardCriteria') AND name = N'AwardGuid' AND is_nullable = 1)
BEGIN
    ALTER TABLE [app].[AwardCriteria] ALTER COLUMN [AwardGuid] UNIQUEIDENTIFIER NOT NULL;
    PRINT '  -> AwardCriteria.AwardGuid set NOT NULL.';
END
GO

-- --------------------------------------------------
-- B. AwardCriteriaRule.AwardCriteriaGuid
-- --------------------------------------------------
IF COL_LENGTH(N'app.AwardCriteriaRule', N'AwardCriteriaGuid') IS NULL
BEGIN
    ALTER TABLE [app].[AwardCriteriaRule] ADD [AwardCriteriaGuid] UNIQUEIDENTIFIER NULL;
    PRINT '  -> Added AwardCriteriaRule.AwardCriteriaGuid (nullable, backfill next).';
END
GO

UPDATE r
SET r.[AwardCriteriaGuid] = c.[AwardCriteriaGuid]
FROM [app].[AwardCriteriaRule] r
INNER JOIN [app].[AwardCriteria] c ON c.[AwardCriteriaId] = r.[AwardCriteriaId]
WHERE r.[AwardCriteriaGuid] IS NULL;

IF COL_LENGTH(N'app.AwardCriteriaRule', N'AwardCriteriaGuid') IS NOT NULL
   AND EXISTS (
        SELECT 1 FROM sys.columns
        WHERE object_id = OBJECT_ID(N'app.AwardCriteriaRule') AND name = N'AwardCriteriaGuid' AND is_nullable = 1)
BEGIN
    ALTER TABLE [app].[AwardCriteriaRule] ALTER COLUMN [AwardCriteriaGuid] UNIQUEIDENTIFIER NOT NULL;
    PRINT '  -> AwardCriteriaRule.AwardCriteriaGuid set NOT NULL.';
END
GO

-- --------------------------------------------------
-- C. Backfill criteria from SrAwardCriteria when Award has none
-- --------------------------------------------------
IF COL_LENGTH(N'app.Award', N'SrAwardCriteria') IS NOT NULL
   AND COL_LENGTH(N'app.AwardCriteria', N'AwardGuid') IS NOT NULL
BEGIN
    PRINT 'Backfilling AwardCriteria from SrAwardCriteria for awards without children...';

    INSERT INTO [app].[AwardCriteria]
    (
        [AwardCriteriaGuid], [AwardId], [AwardGuid], [LineNo],
        [CriteriaCode], [CriteriaName], [MinimumScore], [MaximumScore], [Weight], [Note],
        [StatusId], [CreatedById], [CreatedTime]
    )
    SELECT
        NEWID(),
        a.[AwardId],
        a.[AwardGuid],
        1,
        LEFT(COALESCE(NULLIF(LTRIM(RTRIM(sri.[StandardReferenceItemInitial])), ''), CONCAT('CRIT', a.[SrAwardCriteria])), 20),
        COALESCE(NULLIF(LTRIM(RTRIM(sri.[StandardReferenceItemName])), ''), CONCAT(N'Criteria ', a.[SrAwardCriteria])),
        NULL, NULL, NULL,
        N'Migrated from Award.SrAwardCriteria',
        1,
        COALESCE(a.[CreatedById], 0),
        SYSDATETIME()
    FROM [app].[Award] a
    LEFT JOIN [app].[StandardReferenceItem] sri
        ON sri.[StandardReferenceItemId] = a.[SrAwardCriteria]
    WHERE a.[DeletedTime] IS NULL
      AND NOT EXISTS (
          SELECT 1 FROM [app].[AwardCriteria] c
          WHERE c.[AwardId] = a.[AwardId] AND c.[DeletedTime] IS NULL);

    PRINT CONCAT('  -> Inserted ', @@ROWCOUNT, ' AwardCriteria row(s).');
END
GO

-- --------------------------------------------------
-- D. Drop SrAwardCriteria
-- --------------------------------------------------
IF COL_LENGTH(N'app.Award', N'SrAwardCriteria') IS NOT NULL
BEGIN
    DECLARE @dfName SYSNAME;
    SELECT @dfName = dc.name
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c ON c.default_object_id = dc.object_id
    WHERE dc.parent_object_id = OBJECT_ID(N'[app].[Award]') AND c.name = N'SrAwardCriteria';

    IF @dfName IS NOT NULL
    BEGIN
        DECLARE @sqlDf NVARCHAR(400) = N'ALTER TABLE [app].[Award] DROP CONSTRAINT [' + @dfName + N']';
        EXEC sp_executesql @sqlDf;
    END

    ALTER TABLE [app].[Award] DROP COLUMN [SrAwardCriteria];
    PRINT '  -> Dropped Award.SrAwardCriteria.';
END
ELSE
    PRINT 'Award.SrAwardCriteria already absent.';
GO

-- --------------------------------------------------
-- E. Rename Validto → ValidTo
-- --------------------------------------------------
IF COL_LENGTH(N'app.Award', N'Validto') IS NOT NULL
   AND COL_LENGTH(N'app.Award', N'ValidTo') IS NULL
BEGIN
    EXEC sp_rename N'app.Award.Validto', N'ValidTo', N'COLUMN';
    PRINT '  -> Renamed Award.Validto → ValidTo.';
END
ELSE
    PRINT 'Award.ValidTo OK (or already renamed).';
GO

PRINT '=== Migration Award 3-Tier Align completed ==='
PRINT 'SchemaStudio: Capture Award + AwardCriteria + AwardCriteriaRule, then generate'
PRINT '  TemplatePath = Api.TemplateApiHeaderDetailSubDetail.md'
PRINT '  Header=Award, Detail=AwardCriteria, SubDetail=AwardCriteriaRule'
GO
