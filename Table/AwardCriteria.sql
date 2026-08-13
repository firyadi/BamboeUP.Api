USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   TABLE: app.AwardCriteria  (Detail under Award)
   Target shape (compatible with existing Dev.BamboeHR data):
     AwardId FK, AwardGuid, LineNo, CriteriaCode, CriteriaName,
     MinimumScore, MaximumScore, Weight, Note + audit
   ========================================================= */

IF OBJECT_ID(N'[app].[AwardCriteria]', N'U') IS NULL
BEGIN
    CREATE TABLE [app].[AwardCriteria]
    (
        [AwardCriteriaId]     BIGINT IDENTITY(1,1) NOT NULL,
        [AwardCriteriaGuid]   UNIQUEIDENTIFIER NOT NULL,
        [AwardId]             BIGINT NOT NULL,
        -- Legacy denormalized parent GUID. Internal relationships use AwardId.
        [AwardGuid]           UNIQUEIDENTIFIER NULL,
        [LineNo]              INT NOT NULL,
        [CriteriaCode]        VARCHAR(20) NOT NULL,
        [CriteriaName]        NVARCHAR(200) NOT NULL,
        [MinimumScore]        DECIMAL(9,2) NULL,
        [MaximumScore]        DECIMAL(9,2) NULL,
        [Weight]              DECIMAL(9,2) NULL,
        [Note]                NVARCHAR(1000) NULL,
        [StatusId]            INT NOT NULL,
        [RowVersion]          ROWVERSION NOT NULL,
        [CreatedById]         BIGINT NOT NULL,
        [CreatedTime]         DATETIME2(7) NOT NULL,
        [UpdatedById]         BIGINT NULL,
        [UpdatedTime]         DATETIME2(7) NULL,
        [DeletedById]         BIGINT NULL,
        [DeletedTime]         DATETIME2(7) NULL,
        CONSTRAINT [PK_AwardCriteria] PRIMARY KEY CLUSTERED ([AwardCriteriaId]),
        CONSTRAINT [UQ_AwardCriteria_Guid] UNIQUE ([AwardCriteriaGuid]),
        CONSTRAINT [UQ_AwardCriteria_Award_Code] UNIQUE ([AwardId], [CriteriaCode]),
        CONSTRAINT [FK_AwardCriteria_Award] FOREIGN KEY ([AwardId]) REFERENCES [app].[Award] ([AwardId])
    );

    ALTER TABLE [app].[AwardCriteria] ADD CONSTRAINT [DF_AwardCriteria_Guid] DEFAULT (NEWID()) FOR [AwardCriteriaGuid];
    ALTER TABLE [app].[AwardCriteria] ADD CONSTRAINT [DF_AwardCriteria_LineNo] DEFAULT ((1)) FOR [LineNo];
    ALTER TABLE [app].[AwardCriteria] ADD CONSTRAINT [DF_AwardCriteria_Status] DEFAULT ((1)) FOR [StatusId];
    ALTER TABLE [app].[AwardCriteria] ADD CONSTRAINT [DF_AwardCriteria_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime];
    ALTER TABLE [app].[AwardCriteria] WITH CHECK ADD CONSTRAINT [CHK_AwardCriteria_Status] CHECK ([StatusId] IN (0, 1, 2));
    CREATE NONCLUSTERED INDEX [IX_AwardCriteria_AwardId] ON [app].[AwardCriteria] ([AwardId]);
    PRINT 'Created [app].[AwardCriteria].';
END
ELSE
    PRINT '[app].[AwardCriteria] already exists — use Migration_Award_3Tier.sql to align.';
GO
