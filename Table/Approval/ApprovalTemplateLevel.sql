USE [Dev.BamboeHR]
GO

/****** Object: Table [apv].[ApprovalTemplateLevel] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [apv].[ApprovalTemplateLevel](
    [ApprovalTemplateLevelId]            [bigint] IDENTITY(1,1) NOT NULL,
    [ApprovalTemplateLevelGuid]          [uniqueidentifier] NOT NULL,
    [ApprovalTemplateId]                 [bigint] NOT NULL,
    [LevelOrder]                         [int] NOT NULL,          -- 1, 2, 3 ...
    [LevelName]                          [nvarchar](200) NOT NULL, -- "Atasan Langsung", "HRD"
    -- ApproverType: 'SPECIFIC_USER' | 'USER_GROUP' | 'DIRECT_MANAGER'
    [ApproverType]                       [nvarchar](50) NOT NULL,
    -- true = semua approver di level ini harus approve
    -- false = cukup 1 approver
    [RequireAllApprovers]                [bit] NOT NULL,
    -- true = level ini bisa di-skip jika level sebelumnya timeout/tidak tersedia
    [CanSkipIfPreviousNotApproved]       [bit] NOT NULL,
    -- SLA dalam jam; 0 = tidak ada SLA
    [SlaHours]                           [int] NOT NULL,
    -- Escalate ke level order berapa saat SLA terlewat; NULL = tidak ada eskalasi
    [EscalateToLevelOrder]               [int] NULL,
    [StatusId]                           [int] NOT NULL,
    [RowVersion]                         [timestamp] NOT NULL,
    [CreatedById]                        [bigint] NOT NULL,
    [CreatedTime]                        [datetime2] NOT NULL,
    [UpdatedById]                        [bigint] NULL,
    [UpdatedTime]                        [datetime2] NULL,
    [DeletedById]                        [bigint] NULL,
    [DeletedTime]                        [datetime2] NULL,
    CONSTRAINT [PK_ApprovalTemplateLevel] PRIMARY KEY CLUSTERED ([ApprovalTemplateLevelId] ASC),
    CONSTRAINT [FK_ApprovalTemplateLevel_Template] FOREIGN KEY ([ApprovalTemplateId])
        REFERENCES [apv].[ApprovalTemplate]([ApprovalTemplateId])
) ON [PRIMARY]
GO

ALTER TABLE [apv].[ApprovalTemplateLevel] ADD CONSTRAINT [DF_ApprovalTemplateLevel_Guid]              DEFAULT (newid())       FOR [ApprovalTemplateLevelGuid]
ALTER TABLE [apv].[ApprovalTemplateLevel] ADD CONSTRAINT [DF_ApprovalTemplateLevel_RequireAll]        DEFAULT ((0))           FOR [RequireAllApprovers]
ALTER TABLE [apv].[ApprovalTemplateLevel] ADD CONSTRAINT [DF_ApprovalTemplateLevel_CanSkip]           DEFAULT ((0))           FOR [CanSkipIfPreviousNotApproved]
ALTER TABLE [apv].[ApprovalTemplateLevel] ADD CONSTRAINT [DF_ApprovalTemplateLevel_SlaHours]          DEFAULT ((0))           FOR [SlaHours]
ALTER TABLE [apv].[ApprovalTemplateLevel] ADD CONSTRAINT [DF_ApprovalTemplateLevel_StatusId]          DEFAULT ((1))           FOR [StatusId]
ALTER TABLE [apv].[ApprovalTemplateLevel] ADD CONSTRAINT [DF_ApprovalTemplateLevel_CreatedTime]       DEFAULT (sysdatetime())  FOR [CreatedTime]
GO
