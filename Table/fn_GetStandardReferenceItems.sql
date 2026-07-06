/*
SELECT *
FROM app.fn_GetStandardReferenceItems
(
    40002,                  -- CompanyId
    NULL,                  -- CompanyOfficeId
    NULL,               -- DepartmentId (belum digunakan)
    'EMP_STATUS'        -- StandardReferenceInitial
)
ORDER BY DisplayOrder;
*/
CREATE OR ALTER FUNCTION app.fn_GetStandardReferenceItems
(
      @CompanyId                 BIGINT = NULL
    , @CompanyOfficeId           BIGINT = NULL
    , @DepartmentId              BIGINT = NULL
    , @StandardReferenceInitial  VARCHAR(50)
)

RETURNS TABLE
AS
RETURN
(

WITH ReferenceData
AS
(
    SELECT
          SR.StandardReferenceId
        , SR.StandardReferenceGuid
        , SR.StandardReferenceInitial
        , SR.StandardReferenceName
    FROM app.StandardReference SR
    WHERE SR.StatusId > 0
      AND SR.StandardReferenceInitial = @StandardReferenceInitial
),

---------------------------------------------------------------------
-- Cari Scope dengan prioritas tertinggi
---------------------------------------------------------------------
CandidateScope
AS
(
    SELECT

          S.StandardReferenceScopeId
        , S.StandardReferenceGuid

        , ScopeLevel =
            CASE

                ----------------------------------------------------
                -- Company + Office
                ----------------------------------------------------
                WHEN
                    S.CompanyId = @CompanyId
                AND S.CompanyOfficeId = @CompanyOfficeId
                THEN 'CompanyOffice'

                ----------------------------------------------------
                -- Company
                ----------------------------------------------------
                WHEN
                    S.CompanyId = @CompanyId
                AND S.CompanyOfficeId IS NULL
                THEN 'Company'

                ----------------------------------------------------
                -- Global
                ----------------------------------------------------
                WHEN
                    S.CompanyId IS NULL
                THEN 'Global'

            END

        , Priority =
            CASE

                WHEN
                    S.CompanyId = @CompanyId
                AND S.CompanyOfficeId = @CompanyOfficeId
                THEN 1

                WHEN
                    S.CompanyId = @CompanyId
                AND S.CompanyOfficeId IS NULL
                THEN 2

                WHEN
                    S.CompanyId IS NULL
                THEN 99

            END

    FROM app.StandardReferenceScope S

    INNER JOIN ReferenceData R
        ON R.StandardReferenceId = S.StandardReferenceId

    WHERE

        S.StatusId > 0

        AND
        (

            ---------------------------------------------------------
            -- Company + Office
            ---------------------------------------------------------
            (
                S.CompanyId = @CompanyId
                AND
                S.CompanyOfficeId = @CompanyOfficeId
            )

            OR

            ---------------------------------------------------------
            -- Company
            ---------------------------------------------------------
            (
                S.CompanyId = @CompanyId
                AND
                S.CompanyOfficeId IS NULL
            )

            OR

            ---------------------------------------------------------
            -- Global
            ---------------------------------------------------------
            (
                S.CompanyId IS NULL
            )

        )

),

SelectedScope
AS
(
    SELECT TOP (1)
           *
    FROM CandidateScope
    ORDER BY Priority
)

---------------------------------------------------------------------
-- Final Result
---------------------------------------------------------------------

SELECT

      R.StandardReferenceId
    , R.StandardReferenceGuid
    , R.StandardReferenceInitial
    , R.StandardReferenceName

    , ISNULL(SI.StandardReferenceScopeItemId,
             TI.StandardReferenceItemId)              AS StandardReferenceItemId

    , ISNULL(SI.StandardReferenceScopeItemGuid,
             TI.StandardReferenceItemGuid)            AS StandardReferenceItemGuid

    , ISNULL(SI.StandardReferenceScopeItemInitial,
             TI.StandardReferenceItemInitial)         AS StandardReferenceItemInitial

    , ISNULL(SI.StandardReferenceScopeItemName,
             TI.StandardReferenceItemName)            AS StandardReferenceItemName

    , ISNULL(SI.StandardReferenceScopeItemValue,
             TI.StandardReferenceItemValue)           AS StandardReferenceItemValue

    , ISNULL(SI.DisplayOrder,
             TI.DisplayOrder)                         AS DisplayOrder

    , CASE
        WHEN SS.StandardReferenceScopeId IS NULL
            THEN 'Template'
        ELSE
            'Scope'
      END                                             AS DataSource

    , ISNULL(SS.ScopeLevel,'Template')                AS ScopeLevel

FROM ReferenceData R

LEFT JOIN SelectedScope SS
    ON 1 = 1

LEFT JOIN app.StandardReferenceScopeItem SI
    ON  SS.StandardReferenceScopeId = SI.StandardReferenceScopeId
    AND SI.StatusId > 0

LEFT JOIN app.StandardReferenceItem TI
    ON  SS.StandardReferenceScopeId IS NULL
    AND TI.StandardReferenceId = R.StandardReferenceId
    AND TI.StatusId > 0

);
GO