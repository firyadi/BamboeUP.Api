USE [Dev.BamboeHR]
GO

/****** Object:  UI-09 TemplateUiKanban sample table (BamboeUp standard) ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[app].[WorkItem]', N'U') IS NOT NULL
    DROP TABLE [app].[WorkItem]
GO

/****** Object:  Table [app].[WorkItem]
      Lifecycle board sample for TemplateUiKanban.
      WorkflowStatus codes align with default Kanban columns:
        DRAFT | IN_PROGRESS | REVIEW | DONE | ARCHIVED
      StatusId = soft-delete / row active flag (1 = active).
******/
CREATE TABLE [app].[WorkItem](
	[WorkItemId] [bigint] IDENTITY(1,1) NOT NULL,
	[WorkItemGuid] [uniqueidentifier] NOT NULL,
	[WorkItemCode] [varchar](20) NOT NULL,
	[Title] [varchar](400) NOT NULL,
	[Description] [varchar](4000) NULL,
	[Module] [varchar](100) NULL,
	[WorkItemType] [varchar](50) NULL,
	[OwnerName] [varchar](100) NULL,
	[OwnerUserId] [bigint] NULL,
	[WorkflowStatus] [varchar](30) NOT NULL,
	[Priority] [varchar](20) NULL,
	[CompletionPercent] [int] NULL,
	[Tags] [varchar](200) NULL,
	[Framework] [varchar](50) NULL,
	[DueDate] [date] NULL,
	[Note] [varchar](4000) NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2](7) NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2](7) NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2](7) NULL,
 CONSTRAINT [PK_WorkItem] PRIMARY KEY CLUSTERED
(
	[WorkItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_WorkItem_Code] UNIQUE NONCLUSTERED
(
	[WorkItemCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_WorkItem_Guid] UNIQUE NONCLUSTERED
(
	[WorkItemGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [app].[WorkItem] ADD CONSTRAINT [DF_WorkItem_WorkItemGuid] DEFAULT (newsequentialid()) FOR [WorkItemGuid]
GO
ALTER TABLE [app].[WorkItem] ADD CONSTRAINT [DF_WorkItem_WorkflowStatus] DEFAULT ('DRAFT') FOR [WorkflowStatus]
GO
ALTER TABLE [app].[WorkItem] ADD CONSTRAINT [DF_WorkItem_StatusId] DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [app].[WorkItem] ADD CONSTRAINT [DF_WorkItem_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO

ALTER TABLE [app].[WorkItem] WITH CHECK ADD CONSTRAINT [CK_WorkItem_WorkflowStatus] CHECK
(
	[WorkflowStatus] IN (N'DRAFT', N'IN_PROGRESS', N'REVIEW', N'DONE', N'ARCHIVED')
)
GO
ALTER TABLE [app].[WorkItem] CHECK CONSTRAINT [CK_WorkItem_WorkflowStatus]
GO

ALTER TABLE [app].[WorkItem] WITH CHECK ADD CONSTRAINT [CK_WorkItem_CompletionPercent] CHECK
(
	[CompletionPercent] IS NULL OR ([CompletionPercent] >= 0 AND [CompletionPercent] <= 100)
)
GO
ALTER TABLE [app].[WorkItem] CHECK CONSTRAINT [CK_WorkItem_CompletionPercent]
GO

CREATE NONCLUSTERED INDEX [IX_WorkItem_Workflow_Active]
ON [app].[WorkItem]
(
	[WorkflowStatus] ASC,
	[DeletedTime] ASC
)
INCLUDE
(
	[Title],
	[Module],
	[OwnerName],
	[CompletionPercent],
	[UpdatedTime]
)
GO

CREATE NONCLUSTERED INDEX [IX_WorkItem_Module_Active]
ON [app].[WorkItem]
(
	[Module] ASC,
	[DeletedTime] ASC
)
GO
