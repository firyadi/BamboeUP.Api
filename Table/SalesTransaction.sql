USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    TABLE: app.SalesTransaction
    Pivot-friendly fact table — row/column dimensions + SUM measures.
    Bulk seed: Seed.SalesTransaction.Bulk100K.sql (100 000 rows)
    Pivot SP:  SalesTransactionPivotProcedures.sql → app.pvt_SalesTransaction
*/

IF OBJECT_ID(N'[app].[SalesTransaction]', N'U') IS NULL
BEGIN
    CREATE TABLE [app].[SalesTransaction](
        [SalesTransactionId]   [bigint] IDENTITY(1,1) NOT NULL,
        [SalesTransactionGuid] [uniqueidentifier] NOT NULL,
        [TransactionCode]      [varchar](30) NOT NULL,

        [CompanyId]            [bigint] NOT NULL,
        [CompanyOfficeId]      [bigint] NULL,
        [PeriodYear]           [int] NOT NULL,
        [PeriodMonth]          [int] NOT NULL,
        [TransactionDate]      [date] NOT NULL,

        [RegionCode]           [varchar](10) NOT NULL,
        [ProductCategory]      [varchar](50) NOT NULL,
        [ProductCode]          [varchar](30) NOT NULL,
        [ProductName]          [nvarchar](200) NOT NULL,
        [SalesChannel]         [varchar](30) NOT NULL,
        [CustomerSegment]      [varchar](30) NOT NULL,

        [Quantity]             [int] NOT NULL,
        [UnitPrice]            [decimal](18, 2) NOT NULL,
        [DiscountAmount]       [decimal](18, 2) NOT NULL,
        [NetAmount]            [decimal](18, 2) NOT NULL,
        [CostAmount]           [decimal](18, 2) NOT NULL,

        [StatusId]             [int] NOT NULL,
        [RowVersion]           [timestamp] NOT NULL,
        [CreatedById]          [bigint] NOT NULL,
        [CreatedTime]          [datetime2](7) NOT NULL,
        [UpdatedById]          [bigint] NULL,
        [UpdatedTime]          [datetime2](7) NULL,
        [DeletedById]          [bigint] NULL,
        [DeletedTime]          [datetime2](7) NULL,

        CONSTRAINT [PK_SalesTransaction] PRIMARY KEY CLUSTERED ([SalesTransactionId] ASC),
        CONSTRAINT [UQ_SalesTransaction_Guid] UNIQUE NONCLUSTERED ([SalesTransactionGuid] ASC),
        CONSTRAINT [UQ_SalesTransaction_Code] UNIQUE NONCLUSTERED ([TransactionCode] ASC),
        CONSTRAINT [CHK_SalesTransaction_Status] CHECK ([StatusId] IN (0, 1, 2)),
        CONSTRAINT [CHK_SalesTransaction_PeriodMonth] CHECK ([PeriodMonth] BETWEEN 1 AND 12)
    ) ON [PRIMARY];

    ALTER TABLE [app].[SalesTransaction] ADD CONSTRAINT [DF_SalesTransaction_Guid] DEFAULT (NEWID()) FOR [SalesTransactionGuid];
    ALTER TABLE [app].[SalesTransaction] ADD CONSTRAINT [DF_SalesTransaction_Discount] DEFAULT ((0)) FOR [DiscountAmount];
    ALTER TABLE [app].[SalesTransaction] ADD CONSTRAINT [DF_SalesTransaction_Status] DEFAULT ((1)) FOR [StatusId];
    ALTER TABLE [app].[SalesTransaction] ADD CONSTRAINT [DF_SalesTransaction_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime];

    CREATE NONCLUSTERED INDEX [IX_SalesTransaction_Pivot]
        ON [app].[SalesTransaction]([CompanyId], [PeriodYear], [PeriodMonth], [RegionCode], [ProductCategory])
        INCLUDE ([Quantity], [NetAmount], [CostAmount], [SalesChannel], [CustomerSegment], [ProductName])
        WHERE ([DeletedTime] IS NULL AND [StatusId] > 0);

    PRINT 'Created [app].[SalesTransaction].';
END
ELSE
    PRINT '[app].[SalesTransaction] already exists.';
GO
