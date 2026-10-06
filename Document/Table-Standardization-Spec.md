# BamboeUP — Table Standardization Spec

**Status:** Binding for new / regenerated `Table/*.sql`  
**Last updated:** 2026-09-03  
**Database:** SQL Server · default catalog `Dev.BamboeHR`  
**Related skill:** [`.cursor/skills/bamboeup-table-standardizer/SKILL.md`](../../.cursor/skills/bamboeup-table-standardizer/SKILL.md)  
**SchemaStudio form:** AI Engine → **Table Standardizer** (`/table-standardizer`)

This document is the source of truth for turning a raw / partial `CREATE TABLE` paste into a full BamboeUP DDL script (constraints, defaults, indexes).

---

## 1. Goals

1. Every CRUD table has dual identity: `{Table}Id` (clustered PK) + `{Table}Guid` (unique business key for APIs).
2. All foreign-key style `*Id` columns are `bigint` (except `StatusId`).
3. Soft-delete + audit columns are always present and ordered consistently.
4. Indexes cover FK / `Sr*` / date / active-list paths used by Dapper repositories (`StatusId > 0 AND DeletedTime IS NULL`).

**Out of scope for physical FK:** auto `CONSTRAINT FK_…` (unless present in input or explicitly requested).

**Dual output:** the SchemaStudio Table Standardizer emits **both** SQL Server (T-SQL) and PostgreSQL DDL from the same paste.
PostgreSQL follows `init-pilot.sql` conventions (`UUID`, `TIMESTAMPTZ`, quoted identifiers).
`RowVersion` is still emitted on PG as `BYTEA` (schema parity with `byte[]`; no auto-increment like SQL Server `timestamp`).

---

## 2. Transform checklist

Apply in order when standardizing a paste:

| # | Rule | Action |
|---|------|--------|
| 1 | Header | Emit `USE [Dev.BamboeHR]`, `SET ANSI_NULLS ON`, `SET QUOTED_IDENTIFIER ON` |
| 2 | Schema | Keep from paste (`app` / `core` / …); default **`app`** if missing |
| 3 | PK | `{Table}Id` → `bigint IDENTITY(1,1) NOT NULL` + `PK_{Table}` clustered |
| 4 | Guid | Ensure `{Table}Guid uniqueidentifier NOT NULL` + `UQ_{Table}_{Table}Guid` |
| 5 | Guid default | `DF_{Table}_{Table}Guid` → `(newsequentialid())` (replace legacy `newid()`) |
| 6 | `*Id` types | Every column ending in `Id` → `bigint`, **except** `StatusId` → `int` |
| 7 | `Sr*` | `bigint NULL` unless input marks `NOT NULL` |
| 8 | Preserve business cols | Keep names/meanings from paste; only upgrade type / nullability per rules |
| 9 | Audit block | Append mandatory audit columns (see §4) if missing. Map legacy aliases (`LastUpdateDateTime` → `UpdatedTime`, `LastUpdateByUserID` → `UpdatedById`, …) and **do not emit the old names** |
| 10 | Defaults | `StatusId = (1)`, `CreatedTime = (sysdatetime())`, plus bit defaults if present |
| 11 | Status check | Optional `CHK_{Table}_Status` for master data: `StatusId IN (0,1,2)` |
| 12 | Indexes | Emit PK/UQ + FK/Sr/Date/Active indexes (see §5) |
| 13 | Physical FK | Do **not** emit `CONSTRAINT FK_…` unless input had it or user asked |

Do **not** invent optional business columns (e.g. `HolidayDescription`) unless the user included them or clearly requested enrichment. Required additions are only: `{Table}Guid` + audit block.

---

## 3. Column type matrix

| Pattern | Target type | Nullability |
|---------|-------------|-------------|
| `{Table}Id` (PK) | `bigint IDENTITY(1,1)` | NOT NULL |
| `{Table}Guid` | `uniqueidentifier` | NOT NULL |
| `*Id` FK / lookup (not StatusId) | `bigint` | keep input; prefer NOT NULL for required parents |
| `StatusId` | `int` | NOT NULL |
| `Sr*` | `bigint` | NULL (default) |
| `Is*` flags | `bit` | NOT NULL + DF when sensible |
| `*Name` | `nvarchar(200)` (or keep length if specified) | per input |
| `*Description` / `*Desc` | `nvarchar(500)` | usually NULL |
| `*Code` / `*No` / `*Initial` | `varchar`/`nvarchar` per input | per input |
| Business `*Date` | `date` | per input |
| Business date-time | `datetime2(7)` | per input |
| `CreatedTime` / `UpdatedTime` / `DeletedTime` | `datetime2(7)` | see audit |
| `RowVersion` | SS: `timestamp` · PG: `BYTEA` | NOT NULL |

Legacy upgrades: `int`/`smallint` on `*Id` (non-StatusId) → `bigint`; `datetime` audit → `datetime2(7)`.

---

## 4. Mandatory audit block

Always last columns before constraints, in this order:

| Column | Type | Null | Default |
|--------|------|------|---------|
| `StatusId` | `int` | NOT NULL | `((1))` |
| `RowVersion` | SS: `timestamp` · PG: `BYTEA` | NOT NULL | SS: system · PG: 8-byte zero default (no auto-bump) |
| `CreatedById` | `bigint` | NOT NULL | — |
| `CreatedTime` | `datetime2(7)` | NOT NULL | `(sysdatetime())` |
| `UpdatedById` | `bigint` | NULL | — |
| `UpdatedTime` | `datetime2(7)` | NULL | — |
| `DeletedById` | `bigint` | NULL | — |
| `DeletedTime` | `datetime2(7)` | NULL | — |

Repository convention: active rows = `StatusId > 0 AND DeletedTime IS NULL`.

PostgreSQL audit types: `StatusId` `INT`, times `TIMESTAMPTZ`, `RowVersion` `BYTEA` with zero default — column present for mapping/`SELECT` parity only; it does not auto-increment like SQL Server `timestamp`.

### 4.1 Legacy audit aliases

If the paste uses older HRIS / SSMS names, **rename and drop** them — do not keep both. Canonical BamboeUP names are the only ones in the output.

| Legacy name (examples) | Becomes |
|------------------------|---------|
| `LastUpdateDateTime`, `LastModifiedDate`, `ModifiedDateTime` | `UpdatedTime` |
| `LastUpdateByUserID`, `ModifiedById`, `UpdatedByUserId` | `UpdatedById` |
| `CreateDate`, `CreatedOn`, `CreatedDateTime` | `CreatedTime` |
| `CreatedByUserID`, `CreatedBy` | `CreatedById` |
| `DeletedDate`, `DeleteDateTime` | `DeletedTime` |
| `DeletedByUserID`, `DeletedBy` | `DeletedById` |

---

## 5. Naming

| Object | Pattern | Example |
|--------|---------|---------|
| Primary key | `PK_{Table}` | `PK_Holiday` |
| Guid unique | `UQ_{Table}_{Table}Guid` | `UQ_Holiday_HolidayGuid` |
| Other unique | `UQ_{Table}_{Column}` or `UX_{Table}_…` | `UQ_Hospital_Code` |
| Default | `DF_{Table}_{Column}` | `DF_Holiday_StatusId` |
| Check | `CHK_{Table}_{Name}` | `CHK_Hospital_Status` |
| Nonclustered index | `IX_{Table}_{Column(s)}` | `IX_Holiday_CountryId` |
| Filtered active index | `IX_{Table}_{Column}_Active` | `IX_Holiday_HolidayDate_Active` |

Script date comment: `/****** Object:  Table [{schema}].[{Table}]    Script Date: … ******/`

---

## 6. Index policy (performance)

### Always

1. Clustered `PK_{Table}` on `{Table}Id`
2. Unique nonclustered on `{Table}Guid`

### Auto-add

3. **FK indexes** — every `*Id` except `{Table}Id`, `StatusId`, `CreatedById`, `UpdatedById`, `DeletedById` →  
   `CREATE NONCLUSTERED INDEX [IX_{Table}_{Column}] ON [{schema}].[{Table}] ([{Column}])`
4. **Sr indexes** — every `Sr*` column → `IX_{Table}_{SrColumn}`
5. **Date / active** — columns matching `*Date` (or clear business date) → filtered index:  
   `WHERE ([DeletedTime] IS NULL AND [StatusId] > 0)`  
   Optionally `INCLUDE` Guid + display name columns (see Holiday example).
6. **Natural keys** — `*Code` / unique `*No` / unique name when business-unique → `UQ_` or filtered `IX_…_Active`
7. **Child / detail** — parent FK index required; prefer composite `(ParentId, StatusId)` when list-by-parent is the common path

### Do not

- Index audit `CreatedById` / `UpdatedById` / `DeletedById` by default
- Over-cover every column with INCLUDE (keep INCLUDE tight: Guid + primary display fields)

---

## 7. Required output sections

Emit **one** SSMS-style script:

1. `USE` + `SET ANSI_NULLS` / `SET QUOTED_IDENTIFIER`
2. `CREATE TABLE` with PK + Guid UQ (inline constraints OK)
3. `ALTER TABLE … ADD CONSTRAINT [DF_…]` for Guid, StatusId, CreatedTime, and bit defaults
4. Optional `CHK_{Table}_Status`
5. `CREATE NONCLUSTERED INDEX` statements (FK, Sr, Date_Active, …)

Also emit a short **decision diff** above or below the script (bullet list of transforms).

---

## 8. Example — Holiday (before → after)

### Input (raw / partial)

```sql
CREATE TABLE [app].[Holiday](
	[HolidayId] [bigint] IDENTITY(1,1) NOT NULL,	
   [CountryId] [int], 
	[HolidayDate] [date] NOT NULL,
	[HolidayName] [nvarchar](200) NOT NULL,
```

### Decision diff

- Add `HolidayGuid` + `UQ_Holiday_HolidayGuid` + `DF_Holiday_HolidayGuid` (`newsequentialid()`)
- `CountryId` `int` → `bigint` (nullable kept unless business requires NOT NULL; prefer NOT NULL when national calendar always scoped)
- Append full audit block + defaults
- Indexes: `IX_Holiday_CountryId`, `IX_Holiday_HolidayDate_Active` (+ `IX_Holiday_SrHolidayType` if `SrHolidayType` present)

### Output shape (canonical)

See [`BamboeUp.Api/Table/Holiday.sql`](../Table/Holiday.sql) — full CREATE + defaults + indexes matching this spec (smoke-verified 2026-09-03).

Minimal structural expectations:

```text
HolidayId bigint IDENTITY PK
HolidayGuid uniqueidentifier UQ + DF newsequentialid()
CountryId bigint
HolidayDate date NOT NULL
HolidayName nvarchar(200) NOT NULL
… business columns from paste / explicit enrichment …
StatusId, RowVersion, Created*, Updated*, Deleted*
IX_Holiday_CountryId
IX_Holiday_HolidayDate_Active (filtered)
```

---

## 9. Reference tables in repo

| File | Notes |
|------|--------|
| [`Table/Hospital.sql`](../Table/Hospital.sql) | Audit + Guid UQ + Status CHK (legacy Guid DF may still be `newid()`) |
| [`Table/Bank.sql`](../Table/Bank.sql) | Minimal master pattern |
| [`Table/CostCenter.sql`](../Table/CostCenter.sql) | Hierarchy + Guid UQ |
| [`Table/Calendar.SampleTables.sql`](../Table/Calendar.SampleTables.sql) | `IX_Holiday_Date_Active` filtered pattern |

New scripts should prefer **`newsequentialid()`** over legacy `newid()`.

---

## 10. Save location

When the user asks to save: write one file per table:

`BamboeUp.Api/Table/{Table}.sql`
