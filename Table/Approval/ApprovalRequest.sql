USE [Dev.BamboeHR]
GO

/****** Object: Table [apv].[ApprovalRequest] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- StatusId:
--   0 = Draft
--   1 = Pending (submitted, menunggu approval)
--   2 = InProgress (sudah ada action dari approver)
--   3 = Approved (semua level approved)
--   4 = Rejected
--   5 = Cancelled
CREATE TABLE [apv].[ApprovalRequest](
    [ApprovalRequestId]   [bigint] IDENTITY(1,1) NOT NULL,
    [ApprovalRequestGuid] [uniqueidentifier] NOT NULL,
    [ApprovalTemplateId]  [bigint] NOT NULL,
    [RequestNumber]       [nvarchar](50) NOT NULL,          -- auto-number
    [ModuleCode]          [nvarchar](100) NOT NULL,
    [ReferenceGuid]       [uniqueidentifier] NOT NULL,      -- GUID dokumen asal (Leave, Overtime, dll)
    [ReferenceNumber]     [nvarchar](100) NULL,             -- no dokumen asal
    [RequestedByUserId]   [bigint] NOT NULL,
    [CurrentLevelOrder]   [int] NOT NULL,
    [StatusId]            [int] NOT NULL,
    [Notes]               [nvarchar](500) NULL,
    [RequestedTime]       [datetime2] NOT NULL,
    [CompletedTime]       [datetime2] NULL,
    [RowVersion]          [timestamp] NOT NULL,
    [CreatedById]         [bigint] NOT NULL,
    [CreatedTime]         [datetime2] NOT NULL,
    [UpdatedById]         [bigint] NULL,
    [UpdatedTime]         [datetime2] NULL,
    [DeletedById]         [bigint] NULL,
    [DeletedTime]         [datetime2] NULL,
    CONSTRAINT [PK_ApprovalRequest] PRIMARY KEY CLUSTERED ([ApprovalRequestId] ASC),
    CONSTRAINT [FK_ApprovalRequest_Template] FOREIGN KEY ([ApprovalTemplateId])
        REFERENCES [apv].[ApprovalTemplate]([ApprovalTemplateId])
) ON [PRIMARY]
GO

ALTER TABLE [apv].[ApprovalRequest] ADD CONSTRAINT [DF_ApprovalRequest_Guid]          DEFAULT (newid())       FOR [ApprovalRequestGuid]
ALTER TABLE [apv].[ApprovalRequest] ADD CONSTRAINT [DF_ApprovalRequest_StatusId]      DEFAULT ((1))           FOR [StatusId]
ALTER TABLE [apv].[ApprovalRequest] ADD CONSTRAINT [DF_ApprovalRequest_CurrentLevel]  DEFAULT ((1))           FOR [CurrentLevelOrder]
ALTER TABLE [apv].[ApprovalRequest] ADD CONSTRAINT [DF_ApprovalRequest_RequestedTime] DEFAULT (sysdatetime()) FOR [RequestedTime]
ALTER TABLE [apv].[ApprovalRequest] ADD CONSTRAINT [DF_ApprovalRequest_CreatedTime]   DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
