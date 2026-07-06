USE [Dev.BamboeHR]
GO

/****** Object: Table [apv].[ApprovalStep] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Satu row per level per request. StatusId:
--   0 = Waiting   (belum giliran, level ini belum aktif)
--   1 = Pending   (giliran aktif, menunggu action approver)
--   2 = Approved
--   3 = Rejected
--   4 = Skipped   (di-skip karena CanSkipIfPreviousNotApproved = true)
--   5 = Escalated (SLA terlewat, sudah di-escalate)
--   6 = Delegated (didelegasikan ke orang lain)
CREATE TABLE [apv].[ApprovalStep](
    [ApprovalStepId]       [bigint] IDENTITY(1,1) NOT NULL,
    [ApprovalStepGuid]     [uniqueidentifier] NOT NULL,
    [ApprovalRequestId]    [bigint] NOT NULL,
    [LevelOrder]           [int] NOT NULL,
    [LevelName]            [nvarchar](200) NOT NULL,
    [ApproverUserId]       [bigint] NOT NULL,             -- user yg bertugas approve
    [DelegatedFromUserId]  [bigint] NULL,                 -- jika ini adalah langkah delegasi
    [StatusId]             [int] NOT NULL,
    [ActionTime]           [datetime2] NULL,
    [Comment]              [nvarchar](1000) NULL,
    [SlaDeadline]          [datetime2] NULL,              -- kapan SLA berakhir
    [IsEscalated]          [bit] NOT NULL,
    [CreatedById]          [bigint] NOT NULL,
    [CreatedTime]          [datetime2] NOT NULL,
    [UpdatedById]          [bigint] NULL,
    [UpdatedTime]          [datetime2] NULL,
    CONSTRAINT [PK_ApprovalStep] PRIMARY KEY CLUSTERED ([ApprovalStepId] ASC),
    CONSTRAINT [FK_ApprovalStep_Request] FOREIGN KEY ([ApprovalRequestId])
        REFERENCES [apv].[ApprovalRequest]([ApprovalRequestId])
) ON [PRIMARY]
GO

ALTER TABLE [apv].[ApprovalStep] ADD CONSTRAINT [DF_ApprovalStep_Guid]        DEFAULT (newid())       FOR [ApprovalStepGuid]
ALTER TABLE [apv].[ApprovalStep] ADD CONSTRAINT [DF_ApprovalStep_StatusId]    DEFAULT ((0))           FOR [StatusId]
ALTER TABLE [apv].[ApprovalStep] ADD CONSTRAINT [DF_ApprovalStep_IsEscalated] DEFAULT ((0))           FOR [IsEscalated]
ALTER TABLE [apv].[ApprovalStep] ADD CONSTRAINT [DF_ApprovalStep_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
