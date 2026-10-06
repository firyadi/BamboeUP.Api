# PostgreSQL BamboeUP.HR — Minimal Table List

**Status:** Binding for production PostgreSQL cutover (CRUD + platform)  
**Source UAT:** `Dev.BamboeHR` (SQL Server) + `BamboeUpAuditDB`  
**Target:** PostgreSQL database `"BamboeUP.HR"` (quoted name)  
**Generated:** from live metadata via [`Database/PostgreSQL/tools/Generate-BamboeUpHr.ps1`](../Database/PostgreSQL/tools/Generate-BamboeUpHr.ps1)  
**DDL:** [`Database/PostgreSQL/init-bamboeup-hr.sql`](../Database/PostgreSQL/init-bamboeup-hr.sql)  
**Related:** [Database-Support-Matrix.md](Database-Support-Matrix.md) · [PostgreSQL-Deploy-Runbook.md](PostgreSQL-Deploy-Runbook.md)

---

## Scope

Minimal set to run BamboeUp.Api on PostgreSQL v1:

- Login / RBAC, company / org, geo, StandardReference, Parameter
- AutoNumber, Approval, masters, Person / Employee, FileStorage
- Calendar/Kanban sample tables that have repositories
- Audit `log.*` (co-located in `"BamboeUP.HR"`)
- JOIN display stubs/views for Assignment / Career

**Not included (SQL Server only in v1):** Report, Process, Payroll, SharedParameterCatalog, pivot sample `SalesTransaction`.

**Seed:** platform + geo from UAT (not Person/Employee transactional data).

---

## Live inventory notes (`Dev.BamboeHR`)

| Finding | Action |
|---------|--------|
| `app.Hospital` missing in UAT | Added from [`Table/Hospital.sql`](../Table/Hospital.sql) |
| `app.AutoNumber` missing (legacy API table) | Added from entity / repository contract |
| `apv.*` Approval tables missing | Added from [`Table/Approval/*.sql`](../Table/Approval/) |
| `vw_StandardReference_*` missing (SS uses TVF) | Created PG views for dialect-aware repos |
| JOIN tables `JobTitle`, `FromCompany`, … missing | Stub tables + views |
| Extra UAT tables (`PurchaseRequest`, `UserOrganizationScope`, Process/Report/Payroll, …) | Excluded |

Audit tables live in `BamboeUpAuditDB` on SQL Server; on PostgreSQL they are created in schema `log` inside `"BamboeUP.HR"`.

---

## Required tables (wajib)

### Login / RBAC (`core`) — 6

- Users
- UserGroup
- Programs
- UserGroupProgram
- UserGroupScope
- UserCompanyScope

### Organisasi (`app`) — 4

- Company
- CompanyOffice
- OrganizationUnit
- OrganizationUnitScope

### Geo + Bank (`core`) — 7

- Country
- Province
- City
- District
- SubDistrict
- PostalCode
- Bank

### Lookup + parameter (`app`) — 6

- StandardReference
- StandardReferenceItem
- StandardReferenceScope
- StandardReferenceScopeItem
- Parameter
- ParameterScope

### AutoNumber (`app`) — 6

- AutoNumber *(extra — not in live UAT)*
- AutoNumberTemplate
- AutoNumberComponent
- AutoNumberCounter
- AutoNumberLog
- DocumentNumberRequest

### Approval (`apv`) — 7 *(extra — not in live UAT)*

- ApprovalTemplate
- ApprovalTemplateLevel
- ApprovalTemplateLevelApprover
- ApprovalRequest
- ApprovalStep
- ApprovalHistory
- ApprovalDelegation

### Master HR (`app`) — 13

- Hospital *(extra — not in live UAT)*
- Skill
- Certification
- Asset
- Training
- Holiday
- Award
- AwardCriteria
- AwardCriteriaRule
- CostCenter
- CostCenterScope
- CostCenterAssignment
- FileStorage

### Person (`app`) — 10

- Person
- PersonAddress
- PersonContact
- PersonEducation
- PersonIdentification
- PersonEmergencyContact
- PersonFamily
- PersonBankAccount
- PersonWorkExperience
- PersonPhysicalCharacteristic

### Employee (`app`) — 9

- Employee
- EmployeeAssignment
- EmployeeContract
- EmployeeCareer
- EmployeeSkill
- EmployeeLanguage
- EmployeeTraining
- EmployeeCertification
- EmployeeAsset

### Audit (`log`) — 4

- AuditSnapshot
- AuditSession
- AuditLog
- AuditLogData

### UI sample (repositories exist) — 3

- MeetingRoom
- RoomBooking
- WorkItem

**Subtotal tables:** ~75

---

## Views & JOIN layer

### Views

- `app.vw_StandardReference_Display`
- `app.vw_StandardReferenceItem`
- `core.vw_AdministrativeArea`

### Stub tables (empty masters for generator JOINs)

- JobGrade, JobLevel, JobTitle, Position, Shift, PayrollGroup, WorkLocation

### Views over real tables

- ManagerEmployee, SupervisorEmployee → Employee + Person
- FromCompany / ToCompany → Company
- FromOrganizationUnit / ToOrganizationUnit → OrganizationUnit
- FromManagerEmployee / ToManagerEmployee → ManagerEmployee
- FromJobGrade / ToJobGrade / FromJobLevel / ToJobLevel / FromJobTitle / ToJobTitle / FromPosition / ToPosition → stubs

---

## Excluded from PostgreSQL minimal set

### Process (`core`)

ProcessDefinition, ProcessParameterDefinition, ProcessStepDefinition, ProcessExecution, ProcessParameterValue, ProcessStepExecution, ProcessMessage, ProcessResult, ProcessResultDetail, ProcessApproval, ProcessAudit

### Payroll (`app`)

PayrollRun, PayrollRunDetail

### Report

ReportDefinition, ReportParameter, ReportParameterCatalog, `core.ReportExecutionLog`, `rpt.ReportExecutionLog` (Audit DB), SalesTransaction

### SharedParameter

SharedParameterCatalog, SharedParameterProfile, SharedParameterProfileField

### Calendar without dedicated API repo

Meeting, EmployeeLeave

### Other UAT leftovers

PurchaseRequest, UserOrganizationScope, dbo.sysdiagrams, SchemaStudio_DB

---

## Apply on local PostgreSQL 18

```powershell
# 1) Generate (optional refresh)
powershell -File BamboeUp.Api\Database\PostgreSQL\tools\Generate-BamboeUpHr.ps1

# 2) Create role + database (postgres / 123)
$env:PGPASSWORD = '123'
& "C:\Program Files\PostgreSQL\18\bin\psql.exe" -h localhost -U postgres -d postgres -c "DO `$`$ BEGIN IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'bamboeup') THEN CREATE ROLE bamboeup LOGIN PASSWORD 'bamboeup_dev'; END IF; END `$`$;"
& "C:\Program Files\PostgreSQL\18\bin\psql.exe" -h localhost -U postgres -d postgres -c 'CREATE DATABASE "BamboeUP.HR" OWNER bamboeup;'

# 3) Schema + seed (cwd = Database/PostgreSQL)
cd BamboeUp.Api\Database\PostgreSQL
$env:PGPASSWORD = 'bamboeup_dev'
& "C:\Program Files\PostgreSQL\18\bin\psql.exe" -h localhost -U bamboeup -d "BamboeUP.HR" -v ON_ERROR_STOP=1 -f init-bamboeup-hr.sql
& "C:\Program Files\PostgreSQL\18\bin\psql.exe" -h localhost -U bamboeup -d "BamboeUP.HR" -v ON_ERROR_STOP=1 -f seed\seed-bamboeup-hr.sql
```

API config: [`BamboeUp.Api/appsettings.Development.PostgreSql.json`](../BamboeUp.Api/appsettings.Development.PostgreSql.json) → `Database=BamboeUP.HR`.

Smoke: login → menu Programs → Person list → Employee search → StandardReference dropdown. Report / Process menus may appear from seed but are **not** supported on PostgreSQL v1.
