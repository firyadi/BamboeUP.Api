# PostgreSQL Deploy Runbook — BamboeUp.Api

**Audience:** platform / DevOps  
**Provider:** PostgreSQL 14+ (recommended 16+)  
**Companion:** [Database-Support-Matrix.md](Database-Support-Matrix.md) · [Dual-Database-Known-Limitations.md](Dual-Database-Known-Limitations.md)

---

## 1. Prerequisites

| Item | Notes |
|------|--------|
| PostgreSQL 16 | Docker image `postgres:16-alpine` or managed PG |
| .NET 10 SDK | same as SQL Server host |
| Npgsql | already referenced by `Repository` |
| Schema SQL | `Database/PostgreSQL/init-pilot.sql` (dev smoke stubs) |

Production schema is owned by DBA — `init-pilot.sql` is **smoke/dev only**, not a full port of `Table/*.sql`.

---

## 2. Local Docker (dev)

```powershell
cd BamboeUp.Api
docker compose -f docker-compose.postgres.yml up -d
docker compose -f docker-compose.postgres.yml ps
```

Defaults from compose:

| Setting | Value |
|---------|--------|
| Host | `localhost:5432` |
| Database | `Dev_BamboeHR` |
| User / Password | `bamboeup` / `bamboeup_dev` |
| Init script | `Database/PostgreSQL/init-pilot.sql` |

Health: `pg_isready -U bamboeup -d Dev_BamboeHR` (also wired as compose healthcheck).

---

## 3. Configuration

### Option A — launch profile (recommended)

```powershell
dotnet run --project BamboeUp.Api --launch-profile PostgreSql
```

Sets `BAMBOEUP_USE_POSTGRES=1`, which loads `appsettings.Development.PostgreSql.json`.

### Option B — environment variables

```powershell
$env:BAMBOEUP_USE_POSTGRES = "1"
$env:Database__Provider = "PostgreSql"
$env:Database__ConnectionString = "Host=localhost;Port=5432;Database=Dev_BamboeHR;Username=bamboeup;Password=bamboeup_dev"
$env:Database__AuditConnectionString = $env:Database__ConnectionString
dotnet run --project BamboeUp.Api
```

### Option C — appsettings

`BamboeUp.Api/appsettings.Development.PostgreSql.json`:

```json
{
  "Database": {
    "Provider": "PostgreSql",
    "ConnectionString": "Host=localhost;Port=5432;Database=Dev_BamboeHR;Username=bamboeup;Password=bamboeup_dev",
    "AuditConnectionString": "Host=localhost;Port=5432;Database=Dev_BamboeHR;Username=bamboeup;Password=bamboeup_dev"
  }
}
```

**One provider per deployment** — do not mix SQL Server + PostgreSQL in one process.

---

## 4. Staging / production checklist

1. Provision PostgreSQL 16+ (disk, backups, connection limits).
2. Create databases: app DB + optional dedicated audit DB.
3. Apply **production** DDL (DBA) — not only `init-pilot.sql`.
4. Seed StandardReference templates required by the modules in use.
5. Set secrets via vault / env (`Database__ConnectionString`, `Database__AuditConnectionString`).
6. Set `Database:Provider` = `PostgreSql`.
7. Disable or do not route Report / Process / Payroll traffic to this deployment (SQL Server only in v1).
8. Smoke: login → menu programs → Person list → Employee search → AutoNumber generate → SR dropdown.
9. Point health probes at `/health` (or your existing probe) after DB is reachable.

---

## 5. Schema notes

| Area | Dev stub | Production |
|------|----------|------------|
| Pilot master (Hospital, Skill, …) | `init-pilot.sql` | Full DDL |
| Person / Employee | M3 stubs in init | Full HR schema |
| Auth / Parameter / SR / AutoNumber / Approval | M4 stubs + views | Full platform schema |
| Audit (`log.*`) | M2 stubs | Match BamboeUp.Audit rebuild |
| Report / Process / Payroll SP | — | **Not supported on PG v1** |

Views for dropdown smoke:

- `app.vw_StandardReference_Display`
- `app.vw_StandardReferenceItem`

---

## 6. Verification commands

```powershell
# Dialect / guard unit tests (no Docker)
dotnet test BamboeUp.Api.Tests --filter "FullyQualifiedName~Database&Category!=Integration"

# Integration + optional benchmarks (needs Docker for PG)
dotnet test BamboeUp.Api.Tests --filter "Category=Integration"

# Benchmarks only
dotnet test BamboeUp.Api.Tests --filter "Category=Benchmark"

# Concurrent CRUD smoke
dotnet test BamboeUp.Api.Tests --filter "Category=Load"
```

---

## 7. Rollback

1. Point the deployment back to SQL Server (`Database:Provider=SqlServer` + SS connection strings).
2. Redeploy / restart API.
3. Keep PostgreSQL instance for forensics; do not drop until cutover is abandoned.

---

## 8. Troubleshooting

| Symptom | Check |
|---------|--------|
| Api starts but queries fail on identifiers | PG uses quoted mixed-case tables from Qualify; ensure DDL matches |
| Login OK, menu empty | `Programs` / `UserGroupProgram` / `UserGroupScope` seed |
| SR dropdown empty | `StandardReference` + items; API uses CTE (no UDF) |
| AutoNumber race / lock errors | Generate must run inside a transaction; PG uses `FOR UPDATE` |
| Report endpoints fail | Expected on PG — see known limitations |
