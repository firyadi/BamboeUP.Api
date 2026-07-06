USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
drop TABLE [app].[OrganizationUnit]
go

CREATE TABLE [app].[OrganizationUnit](
	[OrganizationUnitId]        INT IDENTITY(1,1) NOT NULL,
	[OrganizationUnitGuid]      UNIQUEIDENTIFIER NOT NULL,
	[OrganizationUnitCode]      VARCHAR(20) NOT NULL,
	[OrganizationUnitName]      VARCHAR(200) NOT NULL,

	[ParentOrganizationUnitId]  INT NULL,

	[SrOrganizationLevel]       BIGINT NOT NULL, -- BOARD / DIVISION / DEPARTMENT / SECTION

	-- 🔥 Performance Tree
	[LevelDepth]                INT NOT NULL DEFAULT (0),
	[HierarchyPath]             VARCHAR(500) NULL,

	[StatusId]                  INT NOT NULL,
	[RowVersion]                TIMESTAMP NOT NULL,

	[CreatedById]               BIGINT NOT NULL,
	[CreatedTime]               DATETIME NOT NULL,
	[UpdatedById]               BIGINT NULL,
	[UpdatedTime]               DATETIME NULL,
	[DeletedById]               BIGINT NULL,
	[DeletedTime]               DATETIME NULL,

 CONSTRAINT [PK_OrganizationUnit] PRIMARY KEY CLUSTERED 
(
	[OrganizationUnitId] ASC
)WITH (OPTIMIZE_FOR_SEQUENTIAL_KEY = ON) ON [PRIMARY],

 CONSTRAINT [UQ_OrganizationUnit_Guid] UNIQUE NONCLUSTERED
(
	[OrganizationUnitGuid] ASC
),

 CONSTRAINT [UQ_OrganizationUnit_Code] UNIQUE NONCLUSTERED
(
	[OrganizationUnitCode] ASC
)
) ON [PRIMARY]
GO

-- DEFAULT
ALTER TABLE [app].[OrganizationUnit] 
ADD CONSTRAINT [DF_OrganizationUnit_Guid] DEFAULT (NEWID()) FOR [OrganizationUnitGuid]
GO

ALTER TABLE [app].[OrganizationUnit] 
ADD CONSTRAINT [DF_OrganizationUnit_Status] DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [app].[OrganizationUnit] 
ADD CONSTRAINT [DF_OrganizationUnit_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime]
GO

-- CHECK STATUS
ALTER TABLE [app].[OrganizationUnit] WITH CHECK 
ADD CONSTRAINT [CHK_OrganizationUnit_Status] 
CHECK ([StatusId] IN (0,1,2))
GO

-- SELF FK (TREE)
ALTER TABLE [app].[OrganizationUnit]  WITH CHECK 
ADD CONSTRAINT [FK_OrganizationUnit_Parent]
FOREIGN KEY([ParentOrganizationUnitId])
REFERENCES [app].[OrganizationUnit] ([OrganizationUnitId])
GO

-- INDEX (TREE PERFORMANCE)
CREATE NONCLUSTERED INDEX [IX_OrganizationUnit_Parent]
ON [app].[OrganizationUnit] ([ParentOrganizationUnitId])
GO

CREATE NONCLUSTERED INDEX [IX_OrganizationUnit_Level]
ON [app].[OrganizationUnit] ([SrOrganizationLevel], [StatusId])
GO
