SET NOCOUNT ON;
SET XACT_ABORT ON;

CREATE OR ALTER VIEW [app].[vw_StandardReference_Display] AS
SELECT
    i.[StandardReferenceItemId],
    i.[StandardReferenceItemName],
    CAST(0 AS BIGINT) AS [CompanyId],
    CAST(0 AS BIGINT) AS [CompanyOfficeId],
    CAST(NULL AS BIGINT) AS [StandardReferenceGroupId],
    r.[StandardReferenceInitial]
FROM [app].[StandardReferenceItem] i
INNER JOIN [app].[StandardReference] r ON r.[StandardReferenceId] = i.[StandardReferenceId]
WHERE i.[StatusId] > 0 AND i.[DeletedTime] IS NULL;
GO

CREATE OR ALTER VIEW [app].[vw_StandardReferenceItem] AS
SELECT
    i.[StandardReferenceItemId],
    i.[StandardReferenceItemGuid],
    CAST(0 AS BIGINT) AS [CompanyId],
    CAST(NULL AS UNIQUEIDENTIFIER) AS [CompanyGuid],
    CAST(0 AS BIGINT) AS [CompanyOfficeId],
    CAST(NULL AS UNIQUEIDENTIFIER) AS [CompanyOfficeGuid],
    i.[StandardReferenceId],
    r.[StandardReferenceGuid],
    r.[StandardReferenceInitial],
    i.[StandardReferenceItemInitial],
    i.[StandardReferenceItemName],
    CAST(NULL AS NVARCHAR(MAX)) AS [Note]
FROM [app].[StandardReferenceItem] i
INNER JOIN [app].[StandardReference] r ON r.[StandardReferenceId] = i.[StandardReferenceId]
WHERE i.[StatusId] > 0 AND i.[DeletedTime] IS NULL;
GO

BEGIN TRAN;

DECLARE @Now datetime2(7) = SYSUTCDATETIME();
DECLARE @SrGuid uniqueidentifier = NEWID();
DECLARE @SrId bigint;

IF EXISTS (SELECT 1 FROM app.StandardReference WHERE StandardReferenceInitial = N'FormMappingNumbering' AND DeletedTime IS NULL)
BEGIN
    SELECT @SrId = StandardReferenceId, @SrGuid = StandardReferenceGuid
    FROM app.StandardReference
    WHERE StandardReferenceInitial = N'FormMappingNumbering' AND DeletedTime IS NULL;
END
ELSE
BEGIN
    INSERT INTO app.StandardReference
        (StandardReferenceGuid, StandardReferenceName, StandardReferenceInitial, Description, StatusId, CreatedById, CreatedTime)
    VALUES
        (@SrGuid, N'Form Mapping Numbering', N'FormMappingNumbering', N'Maps auto-number templates to business forms/modules', 1, 1, @Now);

    SET @SrId = SCOPE_IDENTITY();
END

DECLARE @Items TABLE (
    Initial nvarchar(50),
    Name nvarchar(255),
    DisplayOrder int,
    ItemId bigint NULL
);

INSERT INTO @Items (Initial, Name, DisplayOrder) VALUES
 (N'LEAVE',     N'Leave Request (Cuti)', 1),
 (N'INVOICE',   N'Invoice', 2),
 (N'OVERTIME',  N'Overtime (Lembur)', 3),
 (N'EMPLOYEE',  N'Employee', 4),
 (N'PURCHASE',  N'Purchase Request', 5),
 (N'SALES',     N'Sales Order', 6);

-- Upsert items by Initial under this SR
DECLARE @Initial nvarchar(50), @Name nvarchar(255), @Ord int, @ItemId bigint, @ItemGuid uniqueidentifier;

DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT Initial, Name, DisplayOrder FROM @Items;
OPEN c;
FETCH NEXT FROM c INTO @Initial, @Name, @Ord;
WHILE @@FETCH_STATUS = 0
BEGIN
    SELECT @ItemId = StandardReferenceItemId
    FROM app.StandardReferenceItem
    WHERE StandardReferenceId = @SrId
      AND StandardReferenceItemInitial = @Initial
      AND DeletedTime IS NULL;

    IF @ItemId IS NULL
    BEGIN
        SET @ItemGuid = NEWID();
        INSERT INTO app.StandardReferenceItem
            (StandardReferenceItemGuid, StandardReferenceId, StandardReferenceGuid,
             StandardReferenceItemInitial, StandardReferenceItemName, StandardReferenceItemValue,
             DisplayOrder, StatusId, CreatedById, CreatedTime)
        VALUES
            (@ItemGuid, @SrId, @SrGuid, @Initial, @Name, @Initial, @Ord, 1, 1, @Now);
        SET @ItemId = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        UPDATE app.StandardReferenceItem
        SET StandardReferenceItemName = @Name,
            DisplayOrder = @Ord,
            StatusId = 1,
            UpdatedById = 1,
            UpdatedTime = @Now
        WHERE StandardReferenceItemId = @ItemId;
    END

    UPDATE @Items SET ItemId = @ItemId WHERE Initial = @Initial;
    SET @ItemId = NULL;
    FETCH NEXT FROM c INTO @Initial, @Name, @Ord;
END
CLOSE c; DEALLOCATE c;

-- Relink AutoNumber templates by name heuristic
UPDATE t
SET t.SrFormMappingNumbering = i.ItemId,
    t.UpdatedById = 1,
    t.UpdatedTime = @Now
FROM app.AutoNumberTemplate t
CROSS APPLY (
    SELECT TOP 1 ItemId
    FROM @Items
    WHERE
        (UPPER(t.TemplateName) LIKE N'%CUTI%' AND Initial = N'LEAVE')
        OR (UPPER(t.TemplateName) LIKE N'%LEAVE%' AND Initial = N'LEAVE')
        OR (UPPER(t.TemplateName) LIKE N'%INVOICE%' AND Initial = N'INVOICE')
        OR (UPPER(t.TemplateName) LIKE N'%LEMBUR%' AND Initial = N'OVERTIME')
        OR (UPPER(t.TemplateName) LIKE N'%OVERTIME%' AND Initial = N'OVERTIME')
) i
WHERE t.DeletedTime IS NULL;

-- Active templates that still point to missing SR item -> set to 0 (unmapped) rather than orphan IDs
UPDATE t
SET t.SrFormMappingNumbering = 0,
    t.UpdatedById = 1,
    t.UpdatedTime = @Now
FROM app.AutoNumberTemplate t
WHERE t.DeletedTime IS NULL
  AND t.SrFormMappingNumbering <> 0
  AND NOT EXISTS (
        SELECT 1 FROM app.StandardReferenceItem i
        WHERE i.StandardReferenceItemId = t.SrFormMappingNumbering
          AND i.StatusId > 0 AND i.DeletedTime IS NULL
  );

COMMIT;

SELECT 'SR' AS Kind, StandardReferenceId, StandardReferenceInitial, StandardReferenceName
FROM app.StandardReference WHERE StandardReferenceInitial = N'FormMappingNumbering';

SELECT 'ITEM' AS Kind, StandardReferenceItemId, StandardReferenceItemInitial, StandardReferenceItemName, DisplayOrder
FROM app.StandardReferenceItem
WHERE StandardReferenceId = (SELECT StandardReferenceId FROM app.StandardReference WHERE StandardReferenceInitial = N'FormMappingNumbering' AND DeletedTime IS NULL)
ORDER BY DisplayOrder;

SELECT AutoNumberTemplateId, TemplateName, SrFormMappingNumbering, StatusId
FROM app.AutoNumberTemplate
ORDER BY AutoNumberTemplateId;

SELECT OBJECT_ID('app.vw_StandardReferenceItem') AS VwItem, OBJECT_ID('app.vw_StandardReference_Display') AS VwDisplay;
