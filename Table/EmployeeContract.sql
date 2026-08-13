USE [Dev.BamboeHR]
GO

/****** Object:  Table [app].[EmployeeContract]    Script Date: 7/20/2026 2:49:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[EmployeeContract](
	[EmployeeContractId] [bigint] IDENTITY(1,1) NOT NULL,
	[EmployeeContractGuid] [uniqueidentifier] NOT NULL,
	[EmployeeId] [bigint] NOT NULL,
	[EmployeeAssignmentId] [bigint] NULL,
	[ContractNo] [nvarchar](50) NOT NULL,
	[SrContractType] [bigint] NOT NULL,
	[SrContractStatus] [bigint] NOT NULL,
	[ContractStartDate] [date] NOT NULL,
	[ContractEndDate] [date] NULL,
	[EffectiveDate] [date] NOT NULL,
	[RenewalSequence] [int] NOT NULL,
	[IsCurrent] [bit] NOT NULL,
	[FileStorageId] [bigint] NULL,
	[Remarks] [nvarchar](1000) NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2](7) NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2](7) NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2](7) NULL,
 CONSTRAINT [PK_EmployeeContract] PRIMARY KEY CLUSTERED 
(
	[EmployeeContractId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_EmployeeContract_Guid] UNIQUE NONCLUSTERED 
(
	[EmployeeContractGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [app].[EmployeeContract] ADD  CONSTRAINT [DF_EmployeeContract_Guid]  DEFAULT (newid()) FOR [EmployeeContractGuid]
GO

ALTER TABLE [app].[EmployeeContract] ADD  CONSTRAINT [DF_EmployeeContract_Renewal]  DEFAULT ((1)) FOR [RenewalSequence]
GO

ALTER TABLE [app].[EmployeeContract] ADD  CONSTRAINT [DF_EmployeeContract_Current]  DEFAULT ((1)) FOR [IsCurrent]
GO

ALTER TABLE [app].[EmployeeContract] ADD  CONSTRAINT [DF_EmployeeContract_Status]  DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [app].[EmployeeContract] ADD  CONSTRAINT [DF_EmployeeContract_CreatedTime]  DEFAULT (sysdatetime()) FOR [CreatedTime]
GO

ALTER TABLE [app].[EmployeeContract]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeContract_Assignment] FOREIGN KEY([EmployeeAssignmentId])
REFERENCES [app].[EmployeeAssignment] ([EmployeeAssignmentId])
GO

ALTER TABLE [app].[EmployeeContract] CHECK CONSTRAINT [FK_EmployeeContract_Assignment]
GO

ALTER TABLE [app].[EmployeeContract]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeContract_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [app].[Employee] ([EmployeeId])
GO

ALTER TABLE [app].[EmployeeContract] CHECK CONSTRAINT [FK_EmployeeContract_Employee]
GO

ALTER TABLE [app].[EmployeeContract]  WITH CHECK ADD  CONSTRAINT [CHK_EmployeeContract_Status] CHECK  (([StatusId]=(2) OR [StatusId]=(1) OR [StatusId]=(0)))
GO

ALTER TABLE [app].[EmployeeContract] CHECK CONSTRAINT [CHK_EmployeeContract_Status]
GO


