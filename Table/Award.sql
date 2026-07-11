USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[app].[Award]', N'U') IS NULL
BEGIN
    CREATE TABLE [app].[Award](
        [AwardId] [bigint] IDENTITY(1,1) NOT NULL,
        [AwardGuid] [uniqueidentifier] NOT NULL,
        [AwardCode] [varchar](20) NOT NULL,
        [AwardName] [nvarchar](400) NOT NULL,
        [SrAwardCriteria] [bigint] NOT NULL,
        [SrAwardType] [bigint] NOT NULL,
        [ValidFrom] [datetime2](7) NOT NULL,
        [Validto] [datetime2](7) NOT NULL,
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
END
GO
