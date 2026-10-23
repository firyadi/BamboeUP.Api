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
