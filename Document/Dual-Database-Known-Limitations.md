# Dual Database — Known Limitations (v1)

**Status:** Binding for production cutover decisions  
**Last updated:** 2026-09-03  
**Related:** [Database-Support-Matrix.md](Database-Support-Matrix.md) · [PostgreSQL-Deploy-Runbook.md](PostgreSQL-Deploy-Runbook.md)

---

## Scope of PostgreSQL support (v1)

PostgreSQL is supported for **CRUD master data**, **Person/Employee**, **platform shell** (User/RBAC, Parameter, StandardReference, AutoNumber, Approval), and **Audit** — one provider per deployment.

It is **not** a full feature parity target with SQL Server in v1.

---

## SQL Server only (deferred)

| Area | Why deferred | Guidance |
|------|--------------|----------|
| **Report** (`ReportRepository`, EXEC SP, pivot) | Heavy stored-procedure surface; T-SQL specific | Keep Report API deployments on SQL Server |
| **Process engine** | State machine + SS procedures / dialect | SQL Server only |
| **Payroll** | SP + batch calculus on SS | SQL Server only |
| **`Table/*.sql` catalog** | Reference DDL for SS; not auto-ported | DBA maintains PG schema separately |
| **`SharedParameterCatalog`** | Not in M4 wave | Stay on SS or port in a later milestone |
| **SS-only views/UDF leftovers** | e.g. legacy display paths not fully stubbed | Prefer `BuildStandardReferenceItemsQuery` API |
| **SchemaStudio API templates (SS-PG3)** | Templates emit `ISqlDialect` SQL; Process seed tetap SS-only | Regenerate CRUD modules OK after golden tests; Process/Report tetap SS |
| **Process seed SQL from SchemaStudio** | `ProcessParameterSeedSqlBuilder` uses T-SQL identity | SQL Server only (same as Process engine) |

---

## Behavioral differences (both providers)

| Topic | SQL Server | PostgreSQL |
|-------|------------|------------|
| String contains filter | `LIKE` | `ILIKE` (via `SqlFilterHelper` / dialect) |
| Identity after INSERT | `SCOPE_IDENTITY` | `RETURNING` |
| Row limit | `TOP (n)` | `LIMIT n` |
| New GUID in SQL | `NEWID()` | `gen_random_uuid()` |
| AutoNumber row lock | `WITH (UPDLOCK, ROWLOCK)` | `FOR UPDATE` |
| StandardReference names | LEFT JOIN (no UDF) | Same CTE/JOIN helpers |
| Dropdown items API | `BuildStandardReferenceItemsQuery` | Same |

---

## Schema / environment caveats

1. **`init-pilot.sql` is smoke DDL**, not production BamboeHR. Production PG needs a reviewed schema + indexes.
2. **Quoted identifiers** from `ISqlDialect.Qualify` require matching quoted DDL (or consistent folding strategy).
3. **Live integration / benchmarks** skip when Docker or SQL Server is unavailable — green CI without databases does not prove runtime CRUD.
4. **Generator JOIN quirks** (e.g. Assignment display tables named from FK columns) may need DBA views/stubs on PG beyond init scripts.
5. **One provider per process** — no dual-write or cross-provider transactions in v1.
6. **SchemaStudio regenerate:** SS-PG3 golden tests green — regenerating CRUD Api modules from SchemaStudio should preserve dialect-aware SQL. Still review diffs before overwrite; Process templates remain SQL Server only.

---

## Recommended deployment shapes

| Workload | Recommended DB |
|----------|----------------|
| Blazor HR CRUD + login/RBAC | SQL Server **or** PostgreSQL |
| Reports / Process / Payroll | SQL Server only |
| Mixed campus | Separate deployments (or keep full stack on SQL Server) |

---

## Exit criteria for removing a limitation

A deferred area may move to PostgreSQL only after:

1. Dialect-aware repository (or explicit PG procedure pack),
2. Integration tests on both providers,
3. Matrix row updated to ✅,
4. Runbook section for that module.
