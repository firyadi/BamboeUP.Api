USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[OrganizationUnitScope](
	[OrganizationUnitScopeId]   BIGINT IDENTITY(1,1) NOT NULL,
	[OrganizationUnitScopeGuid] UNIQUEIDENTIFIER NOT NULL,

	[OrganizationUnitId]        INT NOT NULL,

	[CompanyId]                BIGINT NOT NULL,
	[CompanyOfficeId]          BIGINT NULL,

	[ScopeType]                VARCHAR(20) NOT NULL, -- COMPANY / OFFICE

	[StatusId]                  INT NOT NULL,
	[RowVersion]                TIMESTAMP NOT NULL,

	[CreatedById]               BIGINT NOT NULL,
	[CreatedTime]               DATETIME NOT NULL,
	[UpdatedById]               BIGINT NULL,
	[UpdatedTime]               DATETIME NULL,
	[DeletedById]               BIGINT NULL,
	[DeletedTime]               DATETIME NULL,

 CONSTRAINT [PK_OrganizationUnitScope] PRIMARY KEY CLUSTERED 
(
	[OrganizationUnitScopeId] ASC
)WITH (OPTIMIZE_FOR_SEQUENTIAL_KEY = ON) ON [PRIMARY],

 CONSTRAINT [UQ_OrganizationUnitScope_Guid] UNIQUE NONCLUSTERED
(
	[OrganizationUnitScopeGuid] ASC
)
) ON [PRIMARY]
GO

-- DEFAULT
ALTER TABLE [app].[OrganizationUnitScope] 
ADD CONSTRAINT [DF_OrgScope_Guid] DEFAULT (NEWID()) FOR [OrganizationUnitScopeGuid]
GO

ALTER TABLE [app].[OrganizationUnitScope] 
ADD CONSTRAINT [DF_OrgScope_Status] DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [app].[OrganizationUnitScope] 
ADD CONSTRAINT [DF_OrgScope_CreatedTime] DEFAULT (SYSDATETIME()) FOR [CreatedTime]
GO

-- CHECK STATUS
ALTER TABLE [app].[OrganizationUnitScope] WITH CHECK 
ADD CONSTRAINT [CHK_OrgScope_Status] 
CHECK ([StatusId] IN (0,1,2))
GO

-- 🔥 VALIDASI SCOPE
ALTER TABLE [app].[OrganizationUnitScope] WITH CHECK 
ADD CONSTRAINT [CHK_OrgScope_Type]
CHECK (
	(ScopeType = 'COMPANY' AND CompanyOfficeId IS NULL)
	OR
	(ScopeType = 'OFFICE' AND CompanyOfficeId IS NOT NULL)
)
GO

-- FK
ALTER TABLE [app].[OrganizationUnitScope] WITH CHECK 
ADD CONSTRAINT [FK_OrgScope_OrganizationUnit]
FOREIGN KEY ([OrganizationUnitId])
REFERENCES [app].[OrganizationUnit]([OrganizationUnitId])
GO

ALTER TABLE [app].[OrganizationUnitScope] WITH CHECK 
ADD CONSTRAINT [FK_OrgScope_Company]
FOREIGN KEY ([CompanyId])
REFERENCES [app].[Company]([CompanyId])
GO

ALTER TABLE [app].[OrganizationUnitScope] WITH CHECK 
ADD CONSTRAINT [FK_OrgScope_Office]
FOREIGN KEY ([CompanyOfficeId])
REFERENCES [app].[CompanyOffice]([CompanyOfficeId])
GO

-- 🔥 UNIQUE (NO DUPLICATE SCOPE)
CREATE UNIQUE NONCLUSTERED INDEX [UQ_OrgScope_Main]
ON [app].[OrganizationUnitScope]
(
	OrganizationUnitId,
	CompanyId,
	CompanyOfficeId
)
WHERE DeletedTime IS NULL
GO

-- 🔥 QUERY PERFORMANCE
CREATE NONCLUSTERED INDEX [IX_OrgScope_Company]
ON [app].[OrganizationUnitScope]
(
	CompanyId,
	CompanyOfficeId
)
INCLUDE (OrganizationUnitId, ScopeType, StatusId)
GO

CREATE NONCLUSTERED INDEX [IX_OrgScope_Org]
ON [app].[OrganizationUnitScope]
(
	OrganizationUnitId
)
INCLUDE (CompanyId, CompanyOfficeId, ScopeType)
GO
