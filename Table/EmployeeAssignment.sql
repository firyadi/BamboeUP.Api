USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [app].[EmployeeAssignment]
(
    -------------------------------------------------------------------------
    -- Primary Key
    -------------------------------------------------------------------------
    EmployeeAssignmentId      BIGINT IDENTITY(1,1) NOT NULL,
    EmployeeAssignmentGuid    UNIQUEIDENTIFIER NOT NULL,

    -------------------------------------------------------------------------
    -- Employee
    -------------------------------------------------------------------------
    EmployeeId                BIGINT NOT NULL,

    -------------------------------------------------------------------------
    -- Company Structure
    -------------------------------------------------------------------------
    CompanyId                 BIGINT NOT NULL,
    CompanyOfficeId           BIGINT NULL,
    OrganizationUnitId        BIGINT NOT NULL,

    -------------------------------------------------------------------------
    -- Job
    -------------------------------------------------------------------------
    PositionId                BIGINT NULL,
    JobTitleId                BIGINT NULL,
    JobGradeId                BIGINT NULL,
    JobLevelId                BIGINT NULL,

    -------------------------------------------------------------------------
    -- Reporting
    -------------------------------------------------------------------------
    ManagerEmployeeId         BIGINT NULL,
    SupervisorEmployeeId      BIGINT NULL,

    -------------------------------------------------------------------------
    -- HR
    -------------------------------------------------------------------------
    CostCenterId              BIGINT NULL,
    PayrollGroupId            BIGINT NULL,
    ShiftId                   BIGINT NULL,
    WorkLocationId            BIGINT NULL,

    -------------------------------------------------------------------------
    -- Employment Snapshot
    -------------------------------------------------------------------------
    SrEmploymentStatus        BIGINT NOT NULL,
    SrEmploymentType          BIGINT NOT NULL,

    -------------------------------------------------------------------------
    -- Assignment
    -------------------------------------------------------------------------
    SrAssignmentType          BIGINT NOT NULL,
    SrAssignmentReason        BIGINT NULL,

    EffectiveFrom             DATE NOT NULL,
    EffectiveTo               DATE NULL,

    IsPrimary                 BIT NOT NULL,
    IsCurrent                 BIT NOT NULL,

    Remarks                   NVARCHAR(500) NULL,

    -------------------------------------------------------------------------
    -- Standard BamboeUP Audit
    -------------------------------------------------------------------------
    StatusId                  INT NOT NULL,

    RowVersion                ROWVERSION NOT NULL,

    CreatedById               BIGINT NOT NULL,
    CreatedTime               DATETIME2(7) NOT NULL,

    UpdatedById               BIGINT NULL,
    UpdatedTime               DATETIME2(7) NULL,

    DeletedById               BIGINT NULL,
    DeletedTime               DATETIME2(7) NULL,

    -------------------------------------------------------------------------
    -- PK
    -------------------------------------------------------------------------
    CONSTRAINT PK_EmployeeAssignment
        PRIMARY KEY CLUSTERED
        (
            EmployeeAssignmentId
        ),

    -------------------------------------------------------------------------
    -- Unique Guid
    -------------------------------------------------------------------------
    CONSTRAINT UQ_EmployeeAssignment_Guid
        UNIQUE(EmployeeAssignmentGuid)
)
GO

-----------------------------------------------------------------------------
-- DEFAULT
-----------------------------------------------------------------------------

ALTER TABLE app.EmployeeAssignment
ADD CONSTRAINT DF_EmployeeAssignment_Guid
DEFAULT NEWID()
FOR EmployeeAssignmentGuid
GO

ALTER TABLE app.EmployeeAssignment
ADD CONSTRAINT DF_EmployeeAssignment_IsPrimary
DEFAULT(1)
FOR IsPrimary
GO

ALTER TABLE app.EmployeeAssignment
ADD CONSTRAINT DF_EmployeeAssignment_IsCurrent
DEFAULT(1)
FOR IsCurrent
GO

ALTER TABLE app.EmployeeAssignment
ADD CONSTRAINT DF_EmployeeAssignment_Status
DEFAULT(1)
FOR StatusId
GO

ALTER TABLE app.EmployeeAssignment
ADD CONSTRAINT DF_EmployeeAssignment_CreatedTime
DEFAULT(SYSDATETIME())
FOR CreatedTime
GO

-----------------------------------------------------------------------------
-- CHECK
-----------------------------------------------------------------------------

ALTER TABLE app.EmployeeAssignment
ADD CONSTRAINT CHK_EmployeeAssignment_Status
CHECK (StatusId IN (0,1,2))
GO

-----------------------------------------------------------------------------
-- FOREIGN KEY
-----------------------------------------------------------------------------

ALTER TABLE app.EmployeeAssignment
ADD CONSTRAINT FK_EmployeeAssignment_Employee
FOREIGN KEY(EmployeeId)
REFERENCES app.Employee(EmployeeId)
GO

-----------------------------------------------------------------------------
-- UNIQUE CURRENT ASSIGNMENT
-----------------------------------------------------------------------------

CREATE UNIQUE NONCLUSTERED INDEX UX_EmployeeAssignment_Current
ON app.EmployeeAssignment(EmployeeId)
WHERE IsCurrent = 1
GO

-----------------------------------------------------------------------------
-- PERFORMANCE INDEX
-----------------------------------------------------------------------------

CREATE NONCLUSTERED INDEX IX_EmployeeAssignment_Employee
ON app.EmployeeAssignment
(
    EmployeeId
)
GO

CREATE NONCLUSTERED INDEX IX_EmployeeAssignment_Current
ON app.EmployeeAssignment
(
    EmployeeId,
    IsCurrent
)
GO

CREATE NONCLUSTERED INDEX IX_EmployeeAssignment_Organization
ON app.EmployeeAssignment
(
    OrganizationUnitId,
    IsCurrent
)
GO

CREATE NONCLUSTERED INDEX IX_EmployeeAssignment_Company
ON app.EmployeeAssignment
(
    CompanyId,
    IsCurrent
)
GO

CREATE NONCLUSTERED INDEX IX_EmployeeAssignment_Office
ON app.EmployeeAssignment
(
    CompanyOfficeId,
    IsCurrent
)
GO

CREATE NONCLUSTERED INDEX IX_EmployeeAssignment_Manager
ON app.EmployeeAssignment
(
    ManagerEmployeeId,
    IsCurrent
)
GO

CREATE NONCLUSTERED INDEX IX_EmployeeAssignment_CostCenter
ON app.EmployeeAssignment
(
    CostCenterId,
    IsCurrent
)
GO

CREATE NONCLUSTERED INDEX IX_EmployeeAssignment_Payroll
ON app.EmployeeAssignment
(
    PayrollGroupId,
    IsCurrent
)
GO

CREATE NONCLUSTERED INDEX IX_EmployeeAssignment_Effective
ON app.EmployeeAssignment
(
    EmployeeId,
    EffectiveFrom DESC
)
GO