USE [Dev.BamboeHR]
GO

/****** Object: Table [apv].[ApprovalDelegation] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Delegation: seorang approver mendelegasikan perannya ke orang lain
-- dalam rentang tanggal tertentu.
CREATE TABLE [apv].[ApprovalDelegation](
    [ApprovalDelegationId]   [bigint] IDENTITY(1,1) NOT NULL,
    [ApprovalDelegationGuid] [uniqueidentifier] NOT NULL,
    [DelegatorUserId]        [bigint] NOT NULL,    -- yang mendelegasikan
    [DelegateUserId]         [bigint] NOT NULL,    -- yang menerima (wakil)
    [StartDate]              [date] NOT NULL,
    [EndDate]                [date] NOT NULL,
    [IsActive]               [bit] NOT NULL,
    [Notes]                  [nvarchar](500) NULL,
    [StatusId]               [int] NOT NULL,
    [RowVersion]             [timestamp] NOT NULL,
    [CreatedById]            [bigint] NOT NULL,
    [CreatedTime]            [datetime2] NOT NULL,
    [UpdatedById]            [bigint] NULL,
    [UpdatedTime]            [datetime2] NULL,
    [DeletedById]            [bigint] NULL,
    [DeletedTime]            [datetime2] NULL,
    CONSTRAINT [PK_ApprovalDelegation] PRIMARY KEY CLUSTERED ([ApprovalDelegationId] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [apv].[ApprovalDelegation] ADD CONSTRAINT [DF_ApprovalDelegation_Guid]        DEFAULT (newid())       FOR [ApprovalDelegationGuid]
ALTER TABLE [apv].[ApprovalDelegation] ADD CONSTRAINT [DF_ApprovalDelegation_IsActive]    DEFAULT ((1))           FOR [IsActive]
ALTER TABLE [apv].[ApprovalDelegation] ADD CONSTRAINT [DF_ApprovalDelegation_StatusId]    DEFAULT ((1))           FOR [StatusId]
ALTER TABLE [apv].[ApprovalDelegation] ADD CONSTRAINT [DF_ApprovalDelegation_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
