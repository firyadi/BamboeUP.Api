USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   TABLE: app.Award  (Header — 3-tier with AwardCriteria / AwardCriteriaRule)
   Note : SrAwardCriteria removed; use child table AwardCriteria.
          Column ValidTo (not Validto).
   ========================================================= */

IF OBJECT_ID(N'[app].[Award]', N'U') IS NULL
BEGIN
    CREATE TABLE [app].[Award](
        [AwardId] [bigint] IDENTITY(1,1) NOT NULL,
        [AwardGuid] [uniqueidentifier] NOT NULL,
        [AwardCode] [varchar](20) NOT NULL,
        [AwardName] [nvarchar](400) NOT NULL,
        [SrAwardType] [bigint] NOT NULL,
        [ValidFrom] [datetime2](7) NOT NULL,
        [ValidTo] [datetime2](7) NOT NULL,
        [AwardPrize] [nvarchar](200) NULL,
        [Note] [nvarchar](4000) NULL,
        [StatusId] [int] NOT NULL,
        [RowVersion] [timestamp] NOT NULL,
        [CreatedById] [bigint] NOT NULL,
        [CreatedTime] [datetime2](7) NOT NULL,
        [UpdatedById] [bigint] NULL,
        [UpdatedTime] [datetime2](7) NULL,
        [DeletedById] [bigint] NULL,
        [DeletedTime] [datetime2](7) NULL,
     CONSTRAINT [PK_Award] PRIMARY KEY CLUSTERED ([AwardId] ASC),
     CONSTRAINT [UQ_Award_Guid] UNIQUE NONCLUSTERED ([AwardGuid] ASC),
     CONSTRAINT [UQ_Award_Code] UNIQUE NONCLUSTERED ([AwardCode] ASC)
    ) ON [PRIMARY];

    ALTER TABLE [app].[Award] ADD CONSTRAINT [DF_Award_Guid] DEFAULT (NEWID()) FOR [AwardGuid];
    ALTER TABLE [app].[Award] ADD CONSTRAINT [DF_Award_Status] DEFAULT ((1)) FOR [StatusId];
    ALTER TABLE [app].[Award] ADD CONSTRAINT [DF_Award_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime];
    ALTER TABLE [app].[Award] WITH CHECK ADD CONSTRAINT [CHK_Award_Status] CHECK ([StatusId] IN (0, 1, 2));

    PRINT 'Created [app].[Award] (3-tier header, no SrAwardCriteria).';
END
ELSE
    PRINT '[app].[Award] already exists — use Migration_Award_3Tier.sql for upgrades.';
GO
