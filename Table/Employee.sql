USE [Dev.BamboeHR]
GO

/****** Object:  Table [app].[Employee]    Script Date: 7/20/2026 1:55:06 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[Employee](
	[EmployeeId] [bigint] IDENTITY(1,1) NOT NULL,
	[EmployeeGuid] [uniqueidentifier] NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[EmployeeNo] [nvarchar](30) NOT NULL,
	[HireDate] [date] NOT NULL,
	[ProbationEndDate] [date] NULL,
	[ConfirmationDate] [date] NULL,
	[TerminationDate] [date] NULL,
	[SrEmploymentStatus] [bigint] NOT NULL,
	[SrEmploymentType] [bigint] NOT NULL,
	[SrRemunerationType] [bigint] NULL,
	[StatusId] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[CreatedById] [bigint] NOT NULL,
	[CreatedTime] [datetime2](7) NOT NULL,
	[UpdatedById] [bigint] NULL,
	[UpdatedTime] [datetime2](7) NULL,
	[DeletedById] [bigint] NULL,
	[DeletedTime] [datetime2](7) NULL,
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Employee_EmployeeNo] UNIQUE NONCLUSTERED 
(
	[EmployeeNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Employee_Guid] UNIQUE NONCLUSTERED 
(
	[EmployeeGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Employee_PersonId] UNIQUE NONCLUSTERED 
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [app].[Employee] ADD  CONSTRAINT [DF_Employee_EmployeeGuid]  DEFAULT (newid()) FOR [EmployeeGuid]
GO

ALTER TABLE [app].[Employee] ADD  CONSTRAINT [DF_Employee_StatusId]  DEFAULT ((1)) FOR [StatusId]
GO

ALTER TABLE [app].[Employee] ADD  CONSTRAINT [DF_Employee_CreatedTime]  DEFAULT (sysdatetime()) FOR [CreatedTime]
GO

ALTER TABLE [app].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Person] FOREIGN KEY([PersonId])
REFERENCES [app].[Person] ([PersonId])
GO

ALTER TABLE [app].[Employee] CHECK CONSTRAINT [FK_Employee_Person]
GO

ALTER TABLE [app].[Employee]  WITH CHECK ADD  CONSTRAINT [CHK_Employee_Status] CHECK  (([StatusId]=(2) OR [StatusId]=(1) OR [StatusId]=(0)))
GO

ALTER TABLE [app].[Employee] CHECK CONSTRAINT [CHK_Employee_Status]
GO


