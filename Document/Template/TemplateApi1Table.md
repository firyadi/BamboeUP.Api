# TemplateApi1Table (documentation mirror)

> **Canonical template:** [`BamboeUP.SchemaStudio/SchemaStudio.Generator/Templates/Api/TemplateApi1Table.md`](../../../BamboeUP.SchemaStudio/SchemaStudio.Generator/Templates/Api/TemplateApi1Table.md)
>
> Salinan Document/Template ini **tidak** di-edit sebagai sumber generate. SchemaStudio membaca embedded resource dari path di atas.
>
> **Multi-database (SS-PG3):** repository hasil generate memakai `ISqlDialect` + `SqlDialectQueryHelper` (portable SQL Server + PostgreSQL). Lihat juga [2026-09-03-Api-Template-Dialect-Baseline.md](../../../BamboeUP.SchemaStudio/implementation-plan/2026-09-03-Api-Template-Dialect-Baseline.md).

## Pointers

| Topic | Location |
|-------|----------|
| Repository dialect pattern | Canonical §4 Repository Implementation |
| Sr display | `BuildGlobalStandardReferenceLookup` (bukan UDF) |
| Golden tests | `SchemaStudio.Tests/Generator/ApiDialectAwareRepositoryGoldenTests.cs` |
| Dual-DB runtime | BamboeUp.Api `ISqlDialect` / M0–M5 plan |
