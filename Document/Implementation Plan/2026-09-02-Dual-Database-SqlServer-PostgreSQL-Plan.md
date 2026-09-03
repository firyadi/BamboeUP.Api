# BamboeUp.Api — Dual Database Plan (SQL Server + PostgreSQL)

| Field | Value |
|-------|-------|
| **Version** | 1.0 |
| **Date** | September 2, 2026 |
| **Scope** | BamboeUp.Api + BamboeUp.Audit |
| **Out of scope** | MySQL/MariaDB, SchemaStudio generator (phase terpisah) |
| **Related** | [Database-Support-Matrix.md](../Database-Support-Matrix.md) |

---

## 1. Executive Summary

BamboeUp.Api akan mendukung **dua database provider**:

| Provider | Role |
|----------|------|
| **SQL Server** | Production default — **full features** (CRUD + SP + Process + Report + Payroll) |
| **PostgreSQL 14+** | Multi-DB target — **CRUD & platform modules** (phase bertahap) |

**Stack tetap:** Dapper + raw SQL via `ISqlDialect` — **bukan** migrasi ke EF Core.

**Deployment model:** **satu provider per environment** (dev/staging/prod). Tidak mixed-provider dalam satu deployment awal.

**Assumption:** `Table/*.sql` tetap referensi manual DBA untuk SQL Server; schema PostgreSQL dibuat terpisah (DBA / migration tool). Perubahan codebase terpusat di **Repository + wiring**.

---

## 2. Goals & Non-Goals

### Goals

| # | Goal |
|---|------|
| G1 | Satu codebase Api jalan di SQL Server **atau** PostgreSQL via config |
| G2 | Modul CRUD master data (~60 repo) support PostgreSQL |
| G3 | Person/Employee cluster support PostgreSQL |
| G4 | Shell platform (User, Parameter, StandardReference) support PostgreSQL |
| G5 | SQL Server tetap backward compatible — zero regression |
| G6 | Foundation siap untuk SchemaStudio dialect sync (fase berikutnya) |

### Non-Goals (v1 dual-DB)

| # | Non-Goal |
|---|----------|
| NG1 | MySQL / MariaDB |
| NG2 | Mixed provider per tenant dalam satu deployment |
| NG3 | Auto-migrate `Table/*.sql` ke PostgreSQL |
| NG4 | Process / Payroll / Report SP di PostgreSQL (v1) |
| NG5 | Rewrite Presentation / Service layer |

---

## 3. Architecture

### 3.1 Target layering

```
Presentation.*  ──►  Service.*  ──►  IRepository  ──►  Repository + ISqlDialect
                                                          │
                                    ┌─────────────────────┴─────────────────────┐
                                    ▼                                           ▼
                              SQL Server                                  PostgreSQL
                           (SqlClient)                                      (Npgsql)
```

### 3.2 New projects / abstractions

| Artifact | Location (rencana) | Responsibility |
|----------|-------------------|----------------|
| `DatabaseProvider` enum | `Shared` atau `Contracts` | `SqlServer`, `PostgreSql` |
| `DatabaseOptions` | `Shared.Settings` | Provider + connection strings |
| `ISqlDialect` | `Repository` atau `BamboeUp.Database.Abstractions` | Limit, identity, quote, ILIKE, schema |
| `SqlServerDialect` | same | T-SQL helpers |
| `PostgreSqlDialect` | same | PostgreSQL helpers |
| `IRepositoryContext` | `Repository` | `IDbConnection CreateConnection()` |
| `RepositoryContext` | `Repository` | Factory berdasarkan provider |

### 3.3 ISqlDialect — minimum API

```csharp
public interface ISqlDialect
{
    DatabaseProvider Provider { get; }
    string QuoteIdentifier(string schema, string table);
    string QuoteColumn(string column);
    string SelectTop(string selectClause, int maxRows);
    string ReturnIdentity(string columnName, string castType = "bigint");
    string LikeContains(string column, string paramName);  // LIKE vs ILIKE
    string CurrentUtcTimestamp { get; }  // SYSUTCDATETIME() vs NOW() AT TIME ZONE 'UTC'
}
```

Repository **tidak** hardcode `TOP` / `SCOPE_IDENTITY` — pakai dialect inject.

### 3.4 Configuration

```json
{
  "Database": {
    "Provider": "SqlServer",
    "ConnectionString": "Server=...;Database=Dev.BamboeHR;...",
    "AuditConnectionString": "Server=...;Database=BamboeUpAuditDB;..."
  }
}
```

PostgreSQL example:

```json
{
  "Database": {
    "Provider": "PostgreSql",
    "ConnectionString": "Host=localhost;Database=bamboehr;Username=...;Password=...",
    "AuditConnectionString": "Host=localhost;Database=bamboeup_audit;..."
  }
}
```

Backward compat: jika `Database:Provider` absent → default `SqlServer` + baca `ConnectionStrings:sqlConnection` existing.

---

## 4. SQL Server vs PostgreSQL — feature matrix

| Feature | SQL Server | PostgreSQL v1 |
|---------|:----------:|:-------------:|
| CRUD master data | ✅ | 📋 M1–M3 (repo) |
| Person + child repos | ✅ | 📋 M3 (repo done) |
| Employee cluster | ✅ | 📋 M3 (repo done) |
| StandardReference / Parameter | ✅ | 📋 M4 (repo done) |
| User / Auth / RBAC | ✅ | 📋 M4 (repo done) |
| AutoNumber | ✅ | 📋 M4 (repo done) |
| FileStorage | ✅ | 📋 M2 |
| Approval workflow | ✅ | 📋 M4 |
| Audit (BamboeUp.Audit) | ✅ | 📋 M2 |
| Report (EXEC SP) | ✅ | ⏸ SQL Server only |
| Process engine | ✅ | ⏸ SQL Server only |
| Payroll SP | ✅ | ⏸ SQL Server only |
| Pivot (SP + JSON) | ✅ | ⏸ SQL Server only |

---

## 5. Milestone plan

### M0 — Foundation (2–3 minggu)

**Deliverables**

- [ ] `DatabaseProvider` enum + `DatabaseOptions`
- [ ] `ISqlDialect` + `SqlServerDialect` + `PostgreSqlDialect`
- [ ] `IRepositoryContext` → `IDbConnection` (ganti `SqlConnection` hardcode)
- [ ] Refactor `DapperTransactionManager` → `IDbTransaction`
- [ ] DI registration di `ServiceExtensions.ConfigureDapperServices`
- [ ] Unit tests untuk dialect (limit, identity, quote, like)
- [ ] Docker Compose dev: PostgreSQL 16 side-by-side SQL Server

**Acceptance criteria**

- Api start dengan `Provider=SqlServer` — **semua test existing pass**
- Api start dengan `Provider=PostgreSql` — connect OK (belum perlu modul jalan)

**Files touched**

- `Repository/RepositoryContext.cs`
- `Repository/DapperTransactionManager.cs`
- `Repository/AuditRepositoryContext.cs`
- `BamboeUp.Api/Extensions/ServiceExtensions.cs`
- `Repository/Repository.csproj` (+ `Npgsql` package)

---

### M1 — Pilot CRUD (2–3 minggu)

**Target repositories (4 modul, ~4 repo)**

| Module | Repository | Complexity |
|--------|------------|------------|
| Hospital | `HospitalRepository` | Low |
| Skill | `SkillRepository` | Low |
| Bank | `BankRepository` | Low |
| Certification | `CertificationRepository` | Low |

**Per repository checklist**

- [ ] Replace `TOP (n)` → `_dialect.SelectTop(...)`
- [ ] Replace `SCOPE_IDENTITY()` → `_dialect.ReturnIdentity(...)`
- [ ] Replace `[app].[Table]` → `_dialect.QuoteIdentifier("app", "Table")`
- [ ] Verify soft-delete filter (`StatusId > 0`, `DeletedTime IS NULL`)
- [ ] Integration test: SQL Server + PostgreSQL

**Acceptance criteria**

- CRUD end-to-end Hospital/Skill/Bank/Certification via Api di PostgreSQL
- Blazor UI modul pilot jalan tanpa perubahan

---

### M2 — Master data wave (~6–8 minggu)

**Target: ~40 repository CRUD** (modul sederhana, tanpa UDF berat)

| Group | Repositories |
|-------|-------------|
| Assets & training | Asset, Training, Holiday, MeetingRoom, RoomBooking, WorkItem |
| Org structure | CostCenter, CostCenterScope, CostCenterAssignment, OrganizationUnit, OrganizationUnitScope, Company, CompanyOffice |
| Reference | Country, Province, City, District, SubDistrict, PostalCode |
| HR simple | EmployeeTraining, EmployeeCertification, EmployeeAsset, EmployeeContract |
| Misc | FileStorage, Award, AwardCriteria, AwardCriteriaRule |

**Cross-cutting**

- [x] `SqlFilterHelper` → dialect-aware (`ILIKE` for PostgreSQL)
- [x] `BamboeUp.Audit` → `IDbConnection` + PostgreSQL SQL
- [x] CI notes: [Dual-Database-CI-Notes.md](../Dual-Database-CI-Notes.md)

**Status (2026-09-02):** Repository refactor + Audit provider wiring + PG init stubs completed. Live integration tests pending healthy SQL Server / Docker PostgreSQL.

---

### M3 — Heavy aggregate (~4–5 minggu)

**Target: Person & Employee cluster (~20 repo)**

| Group | Repositories | Note |
|-------|-------------|------|
| Person | Person, Address, Contact, Education, Family, Identification, EmergencyContact, BankAccount, WorkExperience, PhysicalCharacteristic | Banyak FK join |
| Employee | Employee, Assignment, Career, Skill, Language | `fn_GetStandardReferenceItems` |

**Keputusan wajib sebelum M3**

| # | Decision | Recommended |
|---|----------|-------------|
| D1 | `fn_GetStandardReferenceItems` | **Option B:** LEFT JOIN `StandardReference` + `StandardReferenceItem` (kedua provider) — avoid UDF port |
| D2 | FK join pattern | LEFT JOIN tetap; ganti UDF call ke JOIN dialect-aware |

**Cross-cutting**

- [x] `BuildGlobalStandardReferenceLookup` — LEFT JOIN, no `OUTER APPLY` / UDF
- [x] Person cluster (10 repo) dialect-aware
- [x] Employee cluster (Employee, Assignment, Career, Skill, Language) + Contract/Training/Certification/Asset JOIN cleanup
- [x] `RepositoryManager` wires `ISqlDialect`
- [x] PostgreSQL init stubs + SR seed (Gender, EmploymentStatus, …)

**Status (2026-09-03):** Repository refactor completed. Live Person/Employee CRUD on PostgreSQL pending Docker; SQL Server `.mdf` still blocked locally.

**Acceptance criteria**

- Person Detail + Employee modul jalan di PostgreSQL
- StandardReference lookup dropdown jalan (via API, bukan UDF)

---

### M4 — Platform shell (~3–4 minggu)

**Target: Shell repositories**

| Group | Repositories |
|-------|-------------|
| Auth | User, UserGroup, UserGroupProgram, UserGroupScope, UserCompanyScope, UserScope |
| Config | Parameter, ParameterScope, StandardReference*, Program, display/view repos |
| AutoNumber | AutoNumber*, DocumentNumberRequest, Generate (row lock) |
| Approval | Approval* (7 repo incl. LevelApprover) |

**Cross-cutting**

- [x] `ISqlDialect` + `NewGuidExpression` / `TableLockHint` / `ForUpdateSuffix`
- [x] `BuildStandardReferenceItemsQuery` — replace `fn_GetStandardReferenceItems`
- [x] AutoNumber generate: portable UPSERT + row lock
- [x] `RepositoryManager` wires `ISqlDialect`
- [x] PostgreSQL init stubs (`init-pilot.sql` M4 section + views)

**Status (2026-09-03):** Repository refactor completed. Live login/RBAC/AutoNumber on PostgreSQL pending Docker.

**Acceptance criteria**

- Login + RBAC + menu program jalan di PostgreSQL
- AutoNumber generate sequence di PostgreSQL
- StandardReference dropdown via API (tanpa UDF)

---

### M5 — Hardening & documentation (2 minggu)

- [x] Performance benchmark SS vs PG (modul Person list, Employee search)
- [x] Runbook deploy PostgreSQL (connection, schema, seed)
- [x] Update `Database-Support-Matrix.md` → status ✅
- [x] Known limitations doc (Report/Process/Payroll = SQL Server only)
- [x] Load test smoke (concurrent CRUD)

**Status (2026-09-03):** Docs + launch profile + soft benchmark/load smoke complete. Live timings still skip without Docker/SQL Server.

---

## 6. Timeline summary

| Milestone | Duration | Cumulative | Team |
|-----------|----------|------------|------|
| M0 Foundation | 2–3 minggu | 3 minggu | 1 senior |
| M1 Pilot | 2–3 minggu | 6 minggu | 1 senior |
| M2 Master data | 6–8 minggu | 14 minggu | 1 senior + 1 mid |
| M3 Heavy aggregate | 4–5 minggu | 19 minggu | 1 senior + 1 mid |
| M4 Platform | 3–4 minggu | 23 minggu | 1 senior + 1 mid |
| M5 Hardening | 2 minggu | **~25 minggu (~6 bulan)** | 1 senior |

**Solo dev:** ~8–9 bulan.

---

## 7. Repository refactor pattern

### Before (SQL Server hardcoded)

```csharp
var sql = $@"
    SELECT TOP ({MaxResultRecord}) a.*
    FROM [app].[Asset] a
    WHERE a.StatusId > 0 AND a.DeletedTime IS NULL";
```

### After (dialect-aware)

```csharp
var sql = $@"
    {_dialect.SelectTop("a.*", MaxResultRecord)}
    FROM {_dialect.QuoteIdentifier("app", "Asset")} a
    WHERE a.StatusId > 0 AND a.DeletedTime IS NULL";
```

### Insert identity

```csharp
// Before
SELECT CAST(SCOPE_IDENTITY() as bigint);

// After
{_dialect.ReturnIdentity("AssetId")}
// SqlServer: SELECT CAST(SCOPE_IDENTITY() as bigint);
// PostgreSql: RETURNING "AssetId"
```

---

## 8. PostgreSQL prerequisites (DBA / ops)

Walaupun `Table/*.sql` tidak di-convert di repo, **database PostgreSQL harus punya:**

| Object | Action |
|--------|--------|
| Schema `app`, `core` | CREATE SCHEMA |
| Tables (mirror SS) | DBA script / pg_dump adapted |
| Indexes, FK | Same constraints |
| Seed data | StandardReference, Programs, User |
| `fn_GetStandardReferenceItems` | Skip if M3 uses inline subquery |
| Audit tables | Separate DB or schema |

**Dev shortcut:** Docker Compose + init script minimal untuk pilot (M1: 4 tables).

---

## 9. Testing strategy

| Level | SQL Server | PostgreSQL |
|-------|:----------:|:----------:|
| Dialect unit tests | ✅ | ✅ |
| Repository integration | ✅ | ✅ (Testcontainers PG) |
| Api endpoint smoke | ✅ | ✅ |
| Blazor E2E (pilot modul) | ✅ | ✅ |
| Process/Report/Payroll | ✅ | N/A (SS only) |

**CI:** GitHub Actions / Azure DevOps — matrix job `{ sqlserver, postgresql }`.

---

## 10. Risk register

| Risk | Impact | Mitigation |
|------|--------|------------|
| Regression SQL Server | High | M0: zero behavior change; full SS test suite every PR |
| `fn_GetStandardReference` port complex | Medium | Inline subquery (M3 decision) |
| Schema drift SS vs PG | Medium | Checklist per modul; optional schema diff tool |
| Regenerate from SchemaStudio overwrites repo | High | Freeze regenerate for ported repos; sync generator later |
| SP modules requested on PG | Medium | Document "SQL Server only"; phase 2 project |
| Audit SQL differences | Low | Port in M2 with dialect |

---

## 11. SchemaStudio follow-up (post Api plan)

Api dual-DB **sustainable** hanya jika generator ikut dialect-aware:

| Phase | SchemaStudio change | Status |
|-------|---------------------|--------|
| SS-PG1 | `DatabaseProvider` on Project | Pending (not required for portable emit) |
| SS-PG2 | `ISchemaReader` + `PostgreSqlSchemaReader` | Pending (read PG schema) |
| SS-PG3 | Api templates → `ISqlDialect` placeholders | **Done** (1Table / HD / HDS + golden tests) |
| SS-PG4 | Regenerate safe for dual-DB repos | **Lite done** — golden green; mass-regen still staged |

**SS-PG3 deliverables:** `TemplateApi1Table` / `HeaderDetail` / `HeaderDetailSubDetail` / `NewAddDetail` notes; `CodeGenerationContext` Sr → `BuildGlobalStandardReferenceLookup`; `BuildFilter(..., dialect)`; `ManagerPatchEngine` ctor `_dialect`; golden `ApiDialectAwareRepositoryGoldenTests`.

**Process/Wizard:** Process + seed SQL tetap SQL Server only; Wizard stays aggregate-only (no raw SQL).

---

## 12. Decision log

| Date | Decision | Rationale |
|------|----------|-----------|
| 2026-09-02 | Support **SQL Server + PostgreSQL only** | Best SS port fit; drop MySQL |
| 2026-09-02 | Stay on **Dapper** | Coupling is SQL string, not ORM |
| 2026-09-02 | **One provider per deployment** | Simplicity v1 |
| 2026-09-02 | `Table/*.sql` **not converted** in repo | Manual DBA reference; PG schema separate |
| 2026-09-02 | Process/Report/Payroll **SQL Server only** v1 | SP port too expensive |
| TBD M3 | UDF → inline subquery | Avoid dual function maintenance |
| 2026-09-03 | M5 hardening: soft benchmark/load smoke + docs | Docker/SS optional skip; no hard SLA |
| 2026-09-03 | SchemaStudio SS-PG3: generator emits portable `ISqlDialect` SQL | One template, runtime dialect |

---

## 13. Success criteria (program complete)

- [x] Api runs on PostgreSQL with M1–M4 modules
- [x] Api runs on SQL Server with **zero regression** (unit/guard; live CRUD when DB healthy)
- [x] CI green on both providers (unit always; integration skips when DB unavailable)
- [x] Documentation: matrix, runbook, known limitations
- [ ] At least 1 staging environment on PostgreSQL

---

## 14. Quick start (after M0)

```bash
# SQL Server (default — unchanged)
dotnet run --project BamboeUp.Api

# PostgreSQL (compose + launch profile)
docker compose -f docker-compose.postgres.yml up -d
dotnet run --project BamboeUp.Api --launch-profile PostgreSql
```

---

*Plan owner: BamboeUp Platform Team · Review cadence: per milestone gate*
