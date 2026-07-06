USE [Dev.BamboeHR]
GO

/****** Object: Table [apv].[ApprovalTemplateLevelApprover] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Mendefinisikan siapa saja yang bisa approve di setiap level.
-- UserId diisi jika ApproverType = SPECIFIC_USER
-- UserGroupId diisi jika ApproverType = USER_GROUP
-- Keduanya NULL jika ApproverType = DIRECT_MANAGER (dicari otomatis via OrganizationUnit.LeaderUserId)
CREATE TABLE [apv].[ApprovalTemplateLevelApprover](
    [ApprovalTemplateLevelApproverId]   [bigint] IDENTITY(1,1) NOT NULL,
    [ApprovalTemplateLevelApproverGuid] [uniqueidentifier] NOT NULL,
    [ApprovalTemplateLevelId]           [bigint] NOT NULL,
    [UserId]                            [bigint] NULL,
    [UserGroupId]                       [bigint] NULL,
    [StatusId]                          [int] NOT NULL,
    [CreatedById]                       [bigint] NOT NULL,
    [CreatedTime]                       [datetime2] NOT NULL,
    [UpdatedById]                       [bigint] NULL,
    [UpdatedTime]                       [datetime2] NULL,
    [DeletedById]                       [bigint] NULL,
    [DeletedTime]                       [datetime2] NULL,
    CONSTRAINT [PK_ApprovalTemplateLevelApprover] PRIMARY KEY CLUSTERED ([ApprovalTemplateLevelApproverId] ASC),
    CONSTRAINT [FK_ATLevelApprover_Level] FOREIGN KEY ([ApprovalTemplateLevelId])
        REFERENCES [apv].[ApprovalTemplateLevel]([ApprovalTemplateLevelId])
) ON [PRIMARY]
GO

ALTER TABLE [apv].[ApprovalTemplateLevelApprover] ADD CONSTRAINT [DF_ATLevelApprover_Guid]        DEFAULT (newid())       FOR [ApprovalTemplateLevelApproverGuid]
ALTER TABLE [apv].[ApprovalTemplateLevelApprover] ADD CONSTRAINT [DF_ATLevelApprover_StatusId]    DEFAULT ((1))           FOR [StatusId]
ALTER TABLE [apv].[ApprovalTemplateLevelApprover] ADD CONSTRAINT [DF_ATLevelApprover_CreatedTime] DEFAULT (sysdatetime()) FOR [CreatedTime]
GO
