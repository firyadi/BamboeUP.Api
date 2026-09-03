# BamboeUp.Api — Database Support Matrix

**Status:** ✅ Dual-database foundation complete (M0–M5)  
**Implementation plan:** [2026-09-02-Dual-Database-SqlServer-PostgreSQL-Plan.md](Implementation%20Plan/2026-09-02-Dual-Database-SqlServer-PostgreSQL-Plan.md)  
**Stack data access:** Dapper + `IDbConnection` + `ISqlDialect`  
**Last updated:** 2026-09-03

**Docs:** [PostgreSQL-Deploy-Runbook.md](PostgreSQL-Deploy-Runbook.md) · [Dual-Database-Known-Limitations.md](Dual-Database-Known-Limitations.md) · [Dual-Database-CI-Notes.md](Dual-Database-CI-Notes.md)

---

## Supported databases

| # | Database | Driver (.NET) | Role | Status |
|---|----------|---------------|------|--------|
| 1 | **Microsoft SQL Server** 2019+ | `Microsoft.Data.SqlClient` | Primary — full features | ✅ Production |
| 2 | **PostgreSQL** 14+ (rec. 16+) | `Npgsql` | Secondary — CRUD & platform | ✅ Supported (v1) |

**Explicitly out of scope:** MySQL, MariaDB, Oracle, SQLite, MongoDB.

---

## Feature matrix

| Feature | SQL Server | PostgreSQL |
|---------|:----------:|:----------:|
| CRUD master data | ✅ | ✅ |
| Person / Employee cluster | ✅ | ✅ |
| Platform (User, Parameter, SR, AutoNumber, Approval) | ✅ | ✅ |
| Audit DB | ✅ | ✅ |
| Report (EXEC SP) | ✅ | ⏸ v1 SS only |
| Process / Payroll | ✅ | ⏸ v1 SS only |

Legend: ✅ supported · ⏸ deferred (see known limitations)

---

## Deployment model

**One provider per environment** — tidak mixed SQL Server + PostgreSQL dalam satu deployment (v1).

```json
{
  "Database": {
    "Provider": "SqlServer",
    "ConnectionString": "...",
    "AuditConnectionString": "..."
  }
}
```

| `Provider` | Driver |
|------------|--------|
| `SqlServer` | `Microsoft.Data.SqlClient` |
| `PostgreSql` | `Npgsql` |

Local PostgreSQL: `dotnet run --project BamboeUp.Api --launch-profile PostgreSql` (see runbook).

---

## Milestone overview

| Milestone | Scope | Status |
|-----------|-------|--------|
| **M0** | Foundation — dialect, IDbConnection, DI | ✅ |
| **M1** | Pilot — Hospital, Skill, Bank, Certification | ✅ |
| **M2** | Master data ~40 repo + Audit | ✅ |
| **M3** | Person / Employee cluster | ✅ |
| **M4** | Platform shell (Auth, SR, AutoNumber, Approval) | ✅ |
| **M5** | Hardening + docs + benchmark/load smoke | ✅ |

---

## Code impact (Api only)

| Layer | Change |
|-------|--------|
| Repository (~80+ files) | Dialect-aware SQL via `ISqlDialect` |
| BamboeUp.Audit | Provider-aware (`ConfigureProvider`, Npgsql / SqlClient) |
| BamboeUp.Api host (DI/config) | `AddBamboeUpDatabase`, PG launch profile |
| Presentation / Service / DTO / Entities | None required for provider switch |
| `Table/*.sql` | SS reference only — PG schema separate |

---

## References

- Implementation plan: `Document/Implementation Plan/2026-09-02-Dual-Database-SqlServer-PostgreSQL-Plan.md`
- Repository: `BamboeUp.Api/Repository/`
- Connection: `RepositoryContext` → `IDbConnection` via `DbConnectionFactory`
