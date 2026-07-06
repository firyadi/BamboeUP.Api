USE [Dev.BamboeHR]
GO

/****** Object: Table [apv].[ApprovalHistory] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Audit trail lengkap untuk setiap aksi dalam proses approval.
-- ActionType: 'SUBMITTED' | 'APPROVED' | 'REJECTED' | 'DELEGATED'
--            | 'ESCALATED' | 'CANCELLED' | 'SKIPPED' | 'REMINDED'
CREATE TABLE [apv].[ApprovalHistory](
    [ApprovalHistoryId]   [bigint] IDENTITY(1,1) NOT NULL,
    [ApprovalHistoryGuid] [uniqueidentifier] NOT NULL,
    [ApprovalRequestId]   [bigint] NOT NULL,
    [ApprovalStepId]      [bigint] NULL,
    [ActionType]          [nvarchar](50) NOT NULL,
    [ActionByUserId]      [bigint] NOT NULL,
    [ActionTime]          [datetime2] NOT NULL,
    [Comment]             [nvarchar](1000) NULL,
    [FromStatus]          [nvarchar](50) NULL,
    [ToStatus]            [nvarchar](50) NULL,
    [LevelOrder]          [int] NULL,
    CONSTRAINT [PK_ApprovalHistory] PRIMARY KEY CLUSTERED ([ApprovalHistoryId] ASC),
    CONSTRAINT [FK_ApprovalHistory_Request] FOREIGN KEY ([ApprovalRequestId])
        REFERENCES [apv].[ApprovalRequest]([ApprovalRequestId])
) ON [PRIMARY]
GO

ALTER TABLE [apv].[ApprovalHistory] ADD CONSTRAINT [DF_ApprovalHistory_Guid]       DEFAULT (newid())       FOR [ApprovalHistoryGuid]
ALTER TABLE [apv].[ApprovalHistory] ADD CONSTRAINT [DF_ApprovalHistory_ActionTime] DEFAULT (sysdatetime()) FOR [ActionTime]
GO
