# TemplateApiHeaderDetail (documentation mirror)

> **Canonical template:** [`BamboeUP.SchemaStudio/SchemaStudio.Generator/Templates/Api/TemplateApiHeaderDetail.md`](../../../BamboeUP.SchemaStudio/SchemaStudio.Generator/Templates/Api/TemplateApiHeaderDetail.md)
>
> Related: `TemplateApiHeaderDetailSubDetail.md`, `TemplateApiNewAddDetail.md` (reuses HD detail subsections).
>
> **Multi-database (SS-PG3):** Header/Detail/SubDetail repositories inject `IAuditService` + `ISqlDialect`; SQL via `Qualify` / `SelectLimited` / `ReturnIdentitySuffix` / `AppendLimit` / `BuildGlobalStandardReferenceLookup`.

## Pointers

| Topic | Location |
|-------|----------|
| Header + Detail repos | Canonical template sections 4.a / 4.b |
| AddDetail | `Templates/Api/TemplateApiNewAddDetail.md` |
| Golden HD test | `ApiDialectAwareRepositoryGoldenTests.TemplateApiHeaderDetail_EmitsDialectAwareRepositories` |
