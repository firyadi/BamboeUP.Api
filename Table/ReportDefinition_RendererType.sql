USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    §9B — ReportDefinition.RendererType
    Values: FastReport | Telerik | QuestPDF | DevExpress | TelerikPivot | DevExpressPivot
*/

IF COL_LENGTH(N'[core].[ReportDefinition]', N'RendererType') IS NULL
BEGIN
    ALTER TABLE [core].[ReportDefinition]
        ADD [RendererType] NVARCHAR(30) NULL;
END
GO

-- Bank DOC slips: QuestPDF (C# fluent), keep .frx path as optional future FastReport template
UPDATE rd
SET rd.RendererType = N'QuestPDF'
FROM [core].[ReportDefinition] rd
INNER JOIN [core].[Programs] p ON p.ProgramId = rd.ProgramId
WHERE rd.ReportKind = N'DOC'
  AND p.ProgramCode IN (N'02.09.04.01', N'02.09.04.03', N'02.09.04.04', N'02.09.04.05')
  AND rd.RendererType IS NULL;
GO

-- Infer renderer from template extension for existing RPT definitions
UPDATE [core].[ReportDefinition]
SET RendererType = CASE
        WHEN FilePath LIKE '%.frx' OR FilePath LIKE '%.fr3' THEN N'FastReport'
        WHEN FilePath LIKE '%.trdp' OR FilePath LIKE '%.trdx' THEN N'Telerik'
        WHEN FilePath LIKE '%.repx' THEN N'DevExpress'
        ELSE RendererType
    END
WHERE RendererType IS NULL
  AND FilePath IS NOT NULL;
GO
