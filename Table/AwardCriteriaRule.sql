USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   TABLE: app.AwardCriteriaRule  (Sub-Detail under AwardCriteria)
   Target shape (compatible with existing Dev.BamboeHR data):
     AwardCriteriaId FK, AwardCriteriaGuid, LineNo, RuleCode, RuleName,
     Operator, ValueFrom, ValueTo, Score, Note + audit
   ========================================================= */

IF OBJECT_ID(N'[app].[AwardCriteriaRule]', N'U') IS NULL
BEGIN
    CREATE TABLE [app].[AwardCriteriaRule]
    (
        [AwardCriteriaRuleId]     BIGINT IDENTITY(1,1) NOT NULL,
        [AwardCriteriaRuleGuid]   UNIQUEIDENTIFIER NOT NULL,
        [AwardCriteriaId]         BIGINT NOT NULL,
        -- Legacy denormalized parent GUID. Internal relationships use AwardCriteriaId.
        [AwardCriteriaGuid]       UNIQUEIDENTIFIER NULL,
        [LineNo]                  INT NOT NULL,
        [RuleCode]                VARCHAR(20) NOT NULL,
        [RuleName]                NVARCHAR(200) NOT NULL,
        [Operator]                NVARCHAR(30) NOT NULL,
        [ValueFrom]               DECIMAL(18,2) NULL,
        [ValueTo]                 DECIMAL(18,2) NULL,
        [Score]                   DECIMAL(9,2) NULL,
        [Note]                    NVARCHAR(1000) NULL,
        [StatusId]                INT NOT NULL,
        [RowVersion]              ROWVERSION NOT NULL,
        [CreatedById]             BIGINT NOT NULL,
        [CreatedTime]             DATETIME2(7) NOT NULL,
        [UpdatedById]             BIGINT NULL,
        [UpdatedTime]             DATETIME2(7) NULL,
        [DeletedById]             BIGINT NULL,
        [DeletedTime]             DATETIME2(7) NULL,
        CONSTRAINT [PK_AwardCriteriaRule] PRIMARY KEY CLUSTERED ([AwardCriteriaRuleId]),
        CONSTRAINT [UQ_AwardCriteriaRule_Guid] UNIQUE ([AwardCriteriaRuleGuid]),
        CONSTRAINT [UQ_AwardCriteriaRule_Criteria_Code] UNIQUE ([AwardCriteriaId], [RuleCode]),
        CONSTRAINT [FK_AwardCriteriaRule_AwardCriteria]
            FOREIGN KEY ([AwardCriteriaId]) REFERENCES [app].[AwardCriteria] ([AwardCriteriaId])
    );

    ALTER TABLE [app].[AwardCriteriaRule] ADD CONSTRAINT [DF_AwardCriteriaRule_Guid] DEFAULT (NEWID()) FOR [AwardCriteriaRuleGuid];
    ALTER TABLE [app].[AwardCriteriaRule] ADD CONSTRAINT [DF_AwardCriteriaRule_LineNo] DEFAULT ((1)) FOR [LineNo];
    ALTER TABLE [app].[AwardCriteriaRule] ADD CONSTRAINT [DF_AwardCriteriaRule_Status] DEFAULT ((1)) FOR [StatusId];
    ALTER TABLE [app].[AwardCriteriaRule] ADD CONSTRAINT [DF_AwardCriteriaRule_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime];
    ALTER TABLE [app].[AwardCriteriaRule] WITH CHECK ADD CONSTRAINT [CHK_AwardCriteriaRule_Status] CHECK ([StatusId] IN (0, 1, 2));
    CREATE NONCLUSTERED INDEX [IX_AwardCriteriaRule_AwardCriteriaId] ON [app].[AwardCriteriaRule] ([AwardCriteriaId]);
    PRINT 'Created [app].[AwardCriteriaRule].';
END
ELSE
    PRINT '[app].[AwardCriteriaRule] already exists — use Migration_Award_3Tier.sql to align.';
GO
