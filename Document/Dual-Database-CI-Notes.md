# Dual Database CI Notes (M0–M5)

## Local commands

```powershell
# Unit (dialect / filter / guards) — no Docker required
dotnet test BamboeUp.Api.Tests --filter "FullyQualifiedName~Database&Category!=Integration"

# Integration — requires healthy SQL Server and/or Docker for PostgreSQL
dotnet test BamboeUp.Api.Tests --filter "Category=Integration"

# M5 smoke: benchmarks + concurrent CRUD (skip if DB/Docker unavailable)
pwsh ./Scripts/run-dual-db-benchmarks.ps1
# or:
dotnet test BamboeUp.Api.Tests --filter "Category=Benchmark"
dotnet test BamboeUp.Api.Tests --filter "Category=Load"
```

## Suggested CI matrix

| Job | Env | Filter |
|-----|-----|--------|
| unit | none | `FullyQualifiedName~Database&Category!=Integration` |
| integration-sqlserver | SQL Server connection string | `Provider=SqlServer` |
| integration-postgresql | Docker / Testcontainers | `Category=Integration&Provider!=SqlServer` |
| benchmark (optional) | SQL Server and/or Docker | `Category=Benchmark` |
| load-smoke (optional) | Docker | `Category=Load` |

Integration / benchmark / load tests skip automatically when the target database is unavailable.

## M2 status

- ~27 master-data repositories dialect-aware
- `SqlFilterHelper` supports optional `ISqlDialect` (`ILIKE` on PostgreSQL)
- `BamboeUp.Audit` supports `ConfigureProvider` + Npgsql / SqlClient
- PostgreSQL init: `Database/PostgreSQL/init-pilot.sql` (pilot + audit + Asset/Training/Country/Company stubs)

## M3 status

- Person + Employee cluster dialect-aware (`ISqlDialect`, no `fn_GetStandardReferenceItems`)
- Standard-reference names via `BuildGlobalStandardReferenceLookup` (LEFT JOIN both providers)
- PostgreSQL init stubs for Person/Employee (+ Assignment/Career join stubs)
- Guard tests: `PersonEmployeeDialectGuardTests` (no Docker)

## M4 status

- Auth / Parameter / StandardReference / Program / AutoNumber / Approval dialect-aware
- Dropdown API: `BuildStandardReferenceItemsQuery` (no UDF)
- AutoNumber generate: `TableLockHint` + `ForUpdateSuffix` + `NewGuidExpression`
- PostgreSQL stubs + `vw_StandardReference_*` views in `init-pilot.sql`
- Guard tests: `PlatformDialectGuardTests`

## M5 status

- Docs: [PostgreSQL-Deploy-Runbook.md](PostgreSQL-Deploy-Runbook.md), [Dual-Database-Known-Limitations.md](Dual-Database-Known-Limitations.md), matrix ✅
- Launch profile `PostgreSql` + `BAMBOEUP_USE_POSTGRES=1` → `appsettings.Development.PostgreSql.json`
- Benchmark smoke: `DualDatabaseBenchmarkPostgreSqlTests` / `DualDatabaseBenchmarkSqlServerTests` (`Category=Benchmark`)
- Load smoke: `ConcurrentCrudSmokePostgreSqlTests` (`Category=Load`)
- Soft latency budgets only — not an SLA gate until staging baselines exist
