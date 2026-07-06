# TemplateApiHeaderDetail_V2 — Panduan AI Membuat API Header-Detail di BamboeUp

> **Pola Header-Detail** digunakan untuk transaksi yang memiliki header (dokumen induk) dan baris detail (line items).
> Contoh: Company → CompanyOffice, PurchaseOrder → PurchaseOrderLine, dll.
> **PENTING:** Template ini menghasilkan 2 controller, 2 service, repository terpisah untuk Header dan Detail, berdasarkan implementasi nyata Company & CompanyOffice.
> **V2:** Ditambahkan sistem FK Auto-Join (lihat `TemplateApi1Table_V2.md` untuk dokumentasi lengkap FK Join Configuration).

---

## CARA MENGGUNAKAN TEMPLATE INI

Ganti semua placeholder berikut:

| Placeholder | Contoh (Company) | Penjelasan |
|---|---|---|
| `{TargetLayer}` | `Shell` | `Shell` atau `Modules` |
| `{Header}` | `Company` | Nama entity Header PascalCase |
| `{header}` | `company` | Header camelCase |
| `{HeaderPlural}` | `Companies` | Header plural PascalCase |
| `{headerPlural}` | `companies` | Header plural lowercase (URL route) |
| `{Detail}` | `CompanyOffice` | Nama entity Detail PascalCase |
| `{detail}` | `companyOffice` | Detail camelCase |
| `{DetailPlural}` | `CompanyOffices` | Detail plural PascalCase |
| `{detailPlural}` | `offices` | Detail plural lowercase (URL route nested) |
| `{HeaderId}` | `CompanyId` | PK header (bigint) |
| `{HeaderGuid}` | `CompanyGuid` | GUID header |
| `{DetailId}` | `CompanyOfficeId` | PK detail (bigint) |
| `{DetailGuid}` | `CompanyOfficeGuid` | GUID detail |
| `{FKId}` | `CompanyId` | FK ID di tabel detail yang referensi ke header ID |
| `{FKGuid}` | `CompanyGuid` | FK Guid di tabel detail yang referensi ke header Guid |
| `{HeaderField1}` | `CompanyName` | Field utama header |
| `{HeaderField2}` | `InitialName` | Field kedua header |
| `{DetailField1}` | `CompanyOfficeName` | Field utama detail |
| `{DetailField2}` | `Address` | Field kedua detail |
| `{HeaderTable}` | `Company` | Nama tabel header di DB |
| `{DetailTable}` | `CompanyOffice` | Nama tabel detail di DB |
| `{Schema}` | `app` | Schema di database |

### FK Join Placeholders

> **Lihat `TemplateApi1Table_V2.md` > FK JOIN CONFIGURATION** untuk dokumentasi lengkap.
> FK Join diterapkan terpisah untuk Header dan Detail masing-masing.
> Placeholder: `{FK.Field}`, `{FK.Type}`, `{FK.Table}`, `{FK.Schema}`, `{FK.DisplayColumn}`, `{FK.VirtualField}`, `{FK.virtualField}`, `{FK.JoinType}`, `{FK.Alias}`

---

## 1. DTOs — `Shared/DataTransferObjects/`

### 1.a Header DTO — `{Header}Dto.cs`

```csharp
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    // DTO utama (dipakai untuk read)
    public record {Header}Dto
    {
        public long {HeaderId} { get; set; }
        public Guid {HeaderGuid} { get; init; }
        public string {HeaderField1} { get; set; }
        public string {HeaderField2} { get; set; }
        // ── FK Virtual Fields (repeat per Header FK Join entry) ──
        // public string? {FK.VirtualField} { get; set; }
        // ── End FK Virtual Fields ──
        public int StatusId { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record {Header}ForCreationDto
    {
        public string {HeaderField1} { get; set; }
        public string {HeaderField2} { get; set; }
        // ── FK Physical Fields (repeat per Header FK) ──
        // public long {FK.Field} { get; set; }
        // ── End FK Fields ──
        public long CreatedById { get; set; } = 0;

        // Header bisa di-create bersama initial details
        public IEnumerable<{Detail}ForCreationDto>? {DetailPlural} { get; set; }
    }

    public record {Header}ForUpdateDto
    {
        public string {HeaderField1} { get; set; }
        public string {HeaderField2} { get; set; }
        // ── FK Physical Fields (sama seperti CreationDto) ──
        public long UpdatedById { get; set; } = 0;
    }

    public record {Header}ForDeleteDto
    {
        public long DeletedById { get; set; } = 0;
    }

    public class {Header}SearchDto
    {
        public string? {HeaderField1} { get; set; }
        public SearchType {HeaderField1}SearchType { get; set; } = SearchType.Contains;

        public string? {HeaderField2} { get; set; }
        public SearchType {HeaderField2}SearchType { get; set; } = SearchType.Contains;

        // ── FK Virtual Search Fields (repeat per Header FK Searchable=Yes) ──
        // public string? {FK.VirtualField} { get; set; }
        // public SearchType {FK.VirtualField}SearchType { get; set; } = SearchType.Contains;
        // ── End FK Virtual Search Fields ──
    }
}
```

### 1.b Detail DTO — `{Detail}Dto.cs`

```csharp
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record {Detail}Dto
    {
        public long {DetailId} { get; set; }
        public Guid {DetailGuid} { get; init; }
        public long {FKId} { get; set; }
        public Guid {FKGuid} { get; set; }
        public string {DetailField1} { get; set; }
        public string {DetailField2} { get; set; }
        // ── FK Virtual Fields (repeat per Detail FK Join entry) ──
        // public string? {FK.VirtualField} { get; set; }
        // ── End FK Virtual Fields ──
        public int StatusId { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record {Detail}ForCreationDto
    {
        public long {FKId} { get; set; }
        public Guid {FKGuid} { get; set; }
        public string {DetailField1} { get; set; }
        public string {DetailField2} { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public record {Detail}ForUpdateDto
    {
        public long {FKId} { get; set; }
        public Guid {FKGuid} { get; set; }
        public string {DetailField1} { get; set; }
        public string {DetailField2} { get; set; }
        public long UpdatedById { get; set; } = 0;
    }

    public record {Detail}ForDeleteDto
    {
        public long DeletedById { get; set; } = 0;
    }

    public class {Detail}SearchDto
    {
        public string? {DetailField1} { get; set; }
        public SearchType {DetailField1}SearchType { get; set; } = SearchType.Contains;

        public string? {DetailField2} { get; set; }
        public SearchType {DetailField2}SearchType { get; set; } = SearchType.Contains;

        // ── FK Virtual Search Fields (repeat per Detail FK Searchable=Yes) ──
        // public string? {FK.VirtualField} { get; set; }
        // public SearchType {FK.VirtualField}SearchType { get; set; } = SearchType.Contains;
        // ── End FK Virtual Search Fields ──
    }
}
```

---

## 2. Entities — `Entities/Models/`

### 2.a Header Entity

```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("{HeaderTable}", Schema = "{Schema}")]
    public class {Header}
    {
        [Column("{HeaderId}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long {HeaderId} { get; set; }

        [Key]
        [Column("{HeaderGuid}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid {HeaderGuid} { get; set; } = Guid.NewGuid();

        public string {HeaderField1} { get; set; }
        public string {HeaderField2} { get; set; }
        // ── FK Physical Columns (repeat per Header FK) ──
        // ── FK Virtual Fields untuk Dapper mapping (repeat per Header FK) ──

        public long StatusId { get; set; } = 0;
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; } = 0;
        public DateTime CreatedTime { get; set; }
        public long UpdatedById { get; set; } = 0;
        public DateTime? UpdatedTime { get; set; }
        public long DeletedById { get; set; } = 0;
        public DateTime? DeletedTime { get; set; }
    }
}
```

### 2.b Detail Entity

```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("{DetailTable}", Schema = "{Schema}")]
    public class {Detail}
    {
        [Column("{DetailId}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long {DetailId} { get; set; }

        [Key]
        [Column("{DetailGuid}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid {DetailGuid} { get; set; } = Guid.NewGuid();

        [Column("{FKId}")]
        public long {FKId} { get; set; }

        [Column("{FKGuid}")]
        public Guid {FKGuid} { get; set; }

        public string {DetailField1} { get; set; }
        public string {DetailField2} { get; set; }

        public long StatusId { get; set; } = 0;
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; } = 0;
        public DateTime CreatedTime { get; set; }
        public long UpdatedById { get; set; } = 0;
        public DateTime? UpdatedTime { get; set; }
        public long DeletedById { get; set; } = 0;
        public DateTime? DeletedTime { get; set; }
    }
}
```

---

## 3. Repository Contracts — `Contracts/`

### 3.a `I{Header}Repository.cs`

```csharp
using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface I{Header}Repository
    {
        Task<{Header}?> Get{Header}Async(Guid {header}Guid, bool trackChanges);
        Task<{Header}?> Get{Header}ByIdAsync(long {header}Id, bool trackChanges);
        Task<IEnumerable<{Header}>> GetAll{HeaderPlural}Async(bool trackChanges);

        Task Create{Header}Async({Header} {header}, IDbTransaction? transaction = null);
        Task Update{Header}Async({Header} {header}, IDbTransaction? transaction = null);
        Task SoftDelete{Header}Async({Header} {header}, long deletedBy, IDbTransaction? transaction = null);
        Task Delete{Header}Async(Guid {header}Guid, IDbTransaction? transaction = null);

        Task<IEnumerable<{Header}>> Search{Header}Async(
            string? {headerField1}, string? {headerField1}SearchType,
            string? {headerField2}, string? {headerField2}SearchType,
            // ── FK Virtual Search Params (repeat per Header FK Searchable=Yes) ──
            // string? {FK.virtualField}, string? {FK.virtualField}SearchType,
            // ── End ──
            IDbTransaction? transaction = null);
    }
}
```

### 3.b `I{Detail}Repository.cs`

```csharp
using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface I{Detail}Repository
    {
        Task<{Detail}?> Get{Detail}Async(Guid {detail}Guid, bool trackChanges);
        Task<{Detail}?> GetBy{Header}GuidAnd{Detail}GuidAsync(Guid {header}Guid, Guid {detail}Guid);
        Task<IEnumerable<{Detail}>> GetBy{Header}GuidAsync(Guid {header}Guid, bool trackChanges);
        Task<IEnumerable<{Detail}>> GetBy{Header}IdAsync(long {header}Id, bool trackChanges);

        Task Create{Detail}Async({Detail} {detail}, IDbTransaction? transaction = null);
        Task Update{Detail}Async({Detail} {detail}, IDbTransaction? transaction = null);
        Task SoftDelete{Detail}Async({Detail} {detail}, long deletedBy, IDbTransaction? transaction = null);
        Task Delete{Detail}Async(Guid {detail}Guid, IDbTransaction? transaction = null);
        Task DeleteBy{Header}GuidAsync(Guid {header}Guid, IDbTransaction? transaction = null);

        Task<IEnumerable<{Detail}>> Search{Detail}Async(
            string? {detailField1}, string? {detailField1}SearchType,
            string? {detailField2}, string? {detailField2}SearchType,
            Guid {header}Guid, Guid {detail}Guid,
            IDbTransaction? transaction = null);
    }
}
```

---

## 4. Repository Implementations — `Repository/`

### 4.a `{Header}Repository.cs`

```csharp
using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public class {Header}Repository : I{Header}Repository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;

        public {Header}Repository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _audit = auditService;
        }

        public async Task<{Header}?> Get{Header}Async(Guid {header}Guid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT a.*
                    -- ── FK JOIN SELECT (repeat per Header FK) ──
                    -- FK:  , j_{FK.Field}.{FK.DisplayColumn} AS {FK.VirtualField}
                    -- Sr:  , v_{FK.Field}.StandardReferenceItemName AS {FK.VirtualField}
                    -- ── End FK JOIN SELECT ──
                FROM [{Schema}].[{HeaderTable}] a
                    -- ── FK JOIN FROM (repeat per Header FK) ──
                    -- FK:  {FK.JoinType} [{FK.Schema}].[{FK.Table}] j_{FK.Field} ON a.{FK.Field} = j_{FK.Field}.{FK.Field}
                    -- Sr:  LEFT JOIN [app].[vw_StandardReference_Display] v_{FK.Field} ON a.{FK.Field} = v_{FK.Field}.StandardReferenceItemId
                    -- ── End FK JOIN FROM ──
                WHERE a.{HeaderGuid} = @{header}Guid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<{Header}>(sql, new { {header}Guid });
        }

        public async Task<{Header}?> Get{Header}ByIdAsync(long {header}Id, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT a.*
                    -- ── FK JOIN SELECT (same as Get) ──
                FROM [{Schema}].[{HeaderTable}] a
                    -- ── FK JOIN FROM (same as Get) ──
                WHERE a.{HeaderId} = @{header}Id
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<{Header}>(sql, new { {header}Id });
        }

        public async Task<IEnumerable<{Header}>> GetAll{HeaderPlural}Async(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT a.*
                    -- ── FK JOIN SELECT (same as Get) ──
                FROM [{Schema}].[{HeaderTable}] a
                    -- ── FK JOIN FROM (same as Get) ──
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.{HeaderId} DESC";
            return await connection.QueryAsync<{Header}>(sql);
        }

        public async Task Create{Header}Async({Header} {header}, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [{Schema}].[{HeaderTable}]
                ({HeaderGuid}, CreatedById, StatusId, CreatedTime, {HeaderField1}, {HeaderField2}
                    -- ── FK Physical Columns: , {FK.Field} (repeat per Header FK) ──
                )
                OUTPUT INSERTED.{HeaderId}
                VALUES
                (@{HeaderGuid}, @CreatedById, @StatusId, @CreatedTime, @{HeaderField1}, @{HeaderField2}
                    -- ── FK Physical Params: , @{FK.Field} (repeat per Header FK) ──
                )";
            {header}.{HeaderId} = await conn.ExecuteScalarAsync<long>(sql, {header}, transaction);

            await _audit.LogAsync("CREATE", "{HeaderTable}", {header}.{HeaderGuid}.ToString(), {header}.CreatedById.ToString(), null, {header});
        }

        public async Task Update{Header}Async({Header} {header}, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await Get{Header}Async({header}.{HeaderGuid}, false);

            const string sql = @"
                UPDATE [{Schema}].[{HeaderTable}]
                SET {HeaderField1} = @{HeaderField1},
                    {HeaderField2} = @{HeaderField2},
                    -- ── FK Physical Columns: {FK.Field} = @{FK.Field}, (repeat per Header FK) ──
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE {HeaderGuid} = @{HeaderGuid}";
            await conn.ExecuteAsync(sql, {header}, transaction);

            await _audit.LogAsync("UPDATE", "{HeaderTable}", {header}.{HeaderGuid}.ToString(), {header}.UpdatedById.ToString(), oldData, {header});
        }

        public async Task SoftDelete{Header}Async({Header} {header}, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await Get{Header}Async({header}.{HeaderGuid}, false);

            const string sql = @"
                UPDATE [{Schema}].[{HeaderTable}]
                SET StatusId = 0,
                    DeletedById = @DeletedBy,
                    DeletedTime = GETUTCDATE()
                WHERE {HeaderGuid} = @{HeaderGuid}";

            await conn.ExecuteAsync(sql, new { {header}.{HeaderGuid}, DeletedBy = deletedBy }, transaction);

            await _audit.LogAsync("DELETE", "{HeaderTable}", {header}.{HeaderGuid}.ToString(), deletedBy.ToString(), oldData, null);
        }

        public async Task Delete{Header}Async(Guid {header}Guid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await Get{Header}Async({header}Guid, false);

            const string sql = @"DELETE FROM [{Schema}].[{HeaderTable}] WHERE {HeaderGuid} = @{header}Guid";
            await conn.ExecuteAsync(sql, new { {header}Guid }, transaction);

            await _audit.LogAsync("DELETE_ADMIN", "{HeaderTable}", {header}Guid.ToString(), oldData?.DeletedById.ToString() ?? "system", oldData, null);
        }

        public async Task<IEnumerable<{Header}>> Search{Header}Async(
            string? {headerField1}, string? {headerField1}SearchType,
            string? {headerField2}, string? {headerField2}SearchType,
            // ── FK Virtual Search Params (repeat per Header FK Searchable=Yes) ──
            // string? {FK.virtualField}, string? {FK.virtualField}SearchType,
            // ── End ──
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();
            var whereClauses = new List<string> { "a.StatusId > 0", "a.DeletedTime IS NULL" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace({headerField1}))
            {
                var param = SqlFilterHelper.BuildFilter("a.{HeaderField1}", "@{headerField1}", {headerField1}SearchType, parameters, "{headerField1}", {headerField1});
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace({headerField2}))
            {
                var param = SqlFilterHelper.BuildFilter("a.{HeaderField2}", "@{headerField2}", {headerField2}SearchType, parameters, "{headerField2}", {headerField2});
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            // ── FK Virtual Search Filters (repeat per Header FK Searchable=Yes) ──
            // FK:  SqlFilterHelper.BuildFilter("j_{FK.Field}.{FK.DisplayColumn}", ...)
            // Sr:  SqlFilterHelper.BuildFilter("v_{FK.Field}.StandardReferenceItemName", ...)
            // ── End FK Virtual Search Filters ──

            var sql = $@"
                SELECT a.*
                    -- ── FK JOIN SELECT (repeat per Header FK) ──
                FROM [{Schema}].[{HeaderTable}] a
                    -- ── FK JOIN FROM (repeat per Header FK) ──
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.{HeaderId} DESC";
            return await connection.QueryAsync<{Header}>(sql, parameters, transaction);
        }
    }
}
```

### 4.b `{Detail}Repository.cs`

```csharp
using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public class {Detail}Repository : I{Detail}Repository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;

        public {Detail}Repository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _audit = auditService;
        }

        public async Task<{Detail}?> Get{Detail}Async(Guid {detail}Guid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT a.*
                    -- ── FK JOIN SELECT (repeat per Detail FK) ──
                FROM [{Schema}].[{DetailTable}] a
                    -- ── FK JOIN FROM (repeat per Detail FK) ──
                WHERE a.{DetailGuid} = @{detail}Guid AND a.StatusId > 0 AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<{Detail}>(sql, new { {detail}Guid });
        }

        public async Task<{Detail}?> GetBy{Header}GuidAnd{Detail}GuidAsync(Guid {header}Guid, Guid {detail}Guid)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT a.*
                    -- ── FK JOIN SELECT (same as Detail Get) ──
                FROM [{Schema}].[{DetailTable}] a
                    -- ── FK JOIN FROM (same as Detail Get) ──
                WHERE a.{DetailGuid} = @{detail}Guid AND a.{FKGuid} = @{header}Guid AND a.StatusId > 0 AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<{Detail}>(sql, new { {detail}Guid, {header}Guid });
        }

        public async Task<IEnumerable<{Detail}>> GetBy{Header}GuidAsync(Guid {header}Guid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT a.*
                    -- ── FK JOIN SELECT (same as Detail Get) ──
                FROM [{Schema}].[{DetailTable}] a
                    -- ── FK JOIN FROM (same as Detail Get) ──
                WHERE a.{FKGuid} = @{header}Guid AND a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.{DetailId} DESC";
            return await connection.QueryAsync<{Detail}>(sql, new { {header}Guid });
        }

        public async Task<IEnumerable<{Detail}>> GetBy{Header}IdAsync(long {header}Id, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT a.* FROM [{Schema}].[{DetailTable}] a
                WHERE a.{FKId} = @{header}Id AND a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.{DetailId} DESC";
            return await connection.QueryAsync<{Detail}>(sql, new { {header}Id });
        }

        public async Task Create{Detail}Async({Detail} {detail}, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [{Schema}].[{DetailTable}]
                ({DetailGuid}, {FKId}, {FKGuid}, CreatedById, StatusId, CreatedTime, {DetailField1}, {DetailField2})
                OUTPUT INSERTED.{DetailId}
                VALUES
                (@{DetailGuid}, @{FKId}, @{FKGuid}, @CreatedById, @StatusId, @CreatedTime, @{DetailField1}, @{DetailField2})";
            {detail}.{DetailId} = await conn.ExecuteScalarAsync<long>(sql, {detail}, transaction);

            await _audit.LogAsync("CREATE", "{DetailTable}", {detail}.{DetailGuid}.ToString(), {detail}.CreatedById.ToString(), null, {detail});
        }

        public async Task Update{Detail}Async({Detail} {detail}, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await Get{Detail}Async({detail}.{DetailGuid}, false);

            const string sql = @"
                UPDATE [{Schema}].[{DetailTable}]
                SET {FKId} = @{FKId},
                    {FKGuid} = @{FKGuid},
                    {DetailField1} = @{DetailField1},
                    {DetailField2} = @{DetailField2},
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE {DetailGuid} = @{DetailGuid}";
            await conn.ExecuteAsync(sql, {detail}, transaction);

            await _audit.LogAsync("UPDATE", "{DetailTable}", {detail}.{DetailGuid}.ToString(), {detail}.UpdatedById.ToString(), oldData, {detail});
        }

        public async Task SoftDelete{Detail}Async({Detail} {detail}, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await Get{Detail}Async({detail}.{DetailGuid}, false);

            const string sql = @"
                UPDATE [{Schema}].[{DetailTable}]
                SET StatusId = 0, DeletedById = @DeletedBy, DeletedTime = GETUTCDATE()
                WHERE {DetailGuid} = @{DetailGuid}";
            await conn.ExecuteAsync(sql, new { {detail}.{DetailGuid}, DeletedBy = deletedBy }, transaction);

            await _audit.LogAsync("DELETE", "{DetailTable}", {detail}.{DetailGuid}.ToString(), deletedBy.ToString(), oldData, null);
        }

        public async Task Delete{Detail}Async(Guid {detail}Guid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await Get{Detail}Async({detail}Guid, false);

            const string sql = @"DELETE FROM [{Schema}].[{DetailTable}] WHERE {DetailGuid} = @{detail}Guid";
            await conn.ExecuteAsync(sql, new { {detail}Guid }, transaction);

            await _audit.LogAsync("DELETE_ADMIN", "{DetailTable}", {detail}Guid.ToString(), oldData?.DeletedById.ToString() ?? "system", oldData, null);
        }

        public async Task DeleteBy{Header}GuidAsync(Guid {header}Guid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"DELETE FROM [{Schema}].[{DetailTable}] WHERE {FKGuid} = @{header}Guid";
            await conn.ExecuteAsync(sql, new { {header}Guid }, transaction);
        }

        public async Task<IEnumerable<{Detail}>> Search{Detail}Async(
            string? {detailField1}, string? {detailField1}SearchType,
            string? {detailField2}, string? {detailField2}SearchType,
            Guid {header}Guid, Guid {detail}Guid,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();
            var whereClauses = new List<string> { "a.StatusId > 0", "a.DeletedTime IS NULL" };
            var parameters = new DynamicParameters();

            if ({header}Guid != Guid.Empty)
            {
                whereClauses.Add("a.{FKGuid} = @{header}Guid");
                parameters.Add("@{header}Guid", {header}Guid);
            }
            if ({detail}Guid != Guid.Empty)
            {
                whereClauses.Add("a.{DetailGuid} = @{detail}Guid");
                parameters.Add("@{detail}Guid", {detail}Guid);
            }

            if (!string.IsNullOrWhiteSpace({detailField1}))
            {
                var param = SqlFilterHelper.BuildFilter("a.{DetailField1}", "@{detailField1}", {detailField1}SearchType, parameters, "{detailField1}", {detailField1});
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace({detailField2}))
            {
                var param = SqlFilterHelper.BuildFilter("a.{DetailField2}", "@{detailField2}", {detailField2}SearchType, parameters, "{detailField2}", {detailField2});
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT a.*
                    -- ── FK JOIN SELECT (repeat per Detail FK) ──
                FROM [{Schema}].[{DetailTable}] a
                    -- ── FK JOIN FROM (repeat per Detail FK) ──
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.{DetailId} DESC";
            return await connection.QueryAsync<{Detail}>(sql, parameters, transaction);
        }
    }
}
```

---

## 5. Service Contracts — `Service.Contracts.{TargetLayer}/`

### 5.a `I{Header}Service.cs`

```csharp
using Shared.DataTransferObjects;

namespace Service.Contracts.{TargetLayer}
{
    public interface I{Header}Service
    {
        Task<IEnumerable<{Header}Dto>> GetAll{HeaderPlural}Async(bool trackChanges);
        Task<{Header}Dto> Get{Header}ByGuidAsync(Guid {header}Guid, bool trackChanges);
        Task<{Header}Dto> Create{Header}Async({Header}ForCreationDto input);
        Task Update{Header}Async(Guid {header}Guid, {Header}ForUpdateDto input, bool trackChanges);
        Task Delete{Header}Async(Guid {header}Guid, {Header}ForDeleteDto input, bool trackChanges);
        Task Delete{Header}ByAdminAsync(Guid {header}Guid, bool trackChanges);

        Task<IEnumerable<{Header}Dto>> Search{Header}Async(
            string? {headerField1}, string? {headerField1}SearchType, 
            string? {headerField2}, string? {headerField2}SearchType
            // ── FK Virtual Search Params (repeat per Header FK Searchable=Yes) ──
            // , string? {FK.virtualField}, string? {FK.virtualField}SearchType
            // ── End ──
            );
    }
}
```

### 5.b `I{Detail}Service.cs`

```csharp
using Shared.DataTransferObjects;

namespace Service.Contracts.{TargetLayer}
{
    public interface I{Detail}Service
    {
        Task<IEnumerable<{Detail}Dto>> GetAllBy{Header}GuidAsync(Guid {header}Guid);
        Task<{Detail}Dto> Get{Detail}ByGuidAsync(Guid {detail}Guid, bool trackChanges);
        Task<{Detail}Dto> GetBy{Header}GuidAnd{Detail}GuidAsync(Guid {header}Guid, Guid {detail}Guid);
        
        Task<{Detail}Dto> Create{Detail}Async({Detail}ForCreationDto input);
        Task Update{Detail}Async(Guid {detail}Guid, {Detail}ForUpdateDto input, bool trackChanges);
        Task Delete{Detail}Async(Guid {detail}Guid, {Detail}ForDeleteDto input, bool trackChanges);
        Task Delete{Detail}ByAdminAsync(Guid {detail}Guid, bool trackChanges);

        Task<IEnumerable<{Detail}Dto>> Search{Detail}Async(
            string? {detailField1}, string? {detailField1}SearchType, 
            string? {detailField2}, string? {detailField2}SearchType, 
            Guid {header}Guid, Guid {detail}Guid);
    }
}
```

---

## 6. Service Implementations — `Service.{TargetLayer}/`

### 6.a `{Header}Service.cs`

```csharp
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.{TargetLayer};
using Shared.DataTransferObjects;

namespace Service.{TargetLayer}
{
    public class {Header}Service : I{Header}Service
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public {Header}Service(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<{Header}Dto>> GetAll{HeaderPlural}Async(bool trackChanges)
        {
            var entities = await _repository.{Header}.GetAll{HeaderPlural}Async(trackChanges);
            return entities.Adapt<IEnumerable<{Header}Dto>>();
        }

        public async Task<{Header}Dto> Get{Header}ByGuidAsync(Guid {header}Guid, bool trackChanges)
        {
            var entity = await _repository.{Header}.Get{Header}Async({header}Guid, trackChanges);
            return entity.Adapt<{Header}Dto>();
        }

        public async Task<{Header}Dto> Create{Header}Async({Header}ForCreationDto input)
        {
            var model = input.Adapt<{Header}>();
            model.StatusId = 1;
            await _repository.{Header}.Create{Header}Async(model);
            
            // Dapper doesn't automatically insert children, manually insert nested Details
            if (input.{DetailPlural} != null && input.{DetailPlural}.Any())
            {
                foreach (var detailDto in input.{DetailPlural})
                {
                    var detail = detailDto.Adapt<{Detail}>();
                    detail.{FKId} = model.{HeaderId};
                    detail.{FKGuid} = model.{HeaderGuid};
                    detail.StatusId = 1;
                    await _repository.{Detail}.Create{Detail}Async(detail);
                }
            }
            
            return model.Adapt<{Header}Dto>();
        }

        public async Task Update{Header}Async(Guid {header}Guid, {Header}ForUpdateDto input, bool trackChanges)
        {
            var model = input.Adapt<{Header}>();
            model.{HeaderGuid} = {header}Guid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.{Header}.Update{Header}Async(model);
        }

        public async Task Delete{Header}Async(Guid {header}Guid, {Header}ForDeleteDto input, bool trackChanges)
        {
            var model = new {Header} { {HeaderGuid} = {header}Guid };
            await _repository.{Header}.SoftDelete{Header}Async(model, input.DeletedById);
        }

        public async Task Delete{Header}ByAdminAsync(Guid {header}Guid, bool trackChanges)
        {
            await _repository.{Header}.Delete{Header}Async({header}Guid);
        }

        public async Task<IEnumerable<{Header}Dto>> Search{Header}Async(
            string? {headerField1}, string? {headerField1}SearchType, 
            string? {headerField2}, string? {headerField2}SearchType
            // ── FK Virtual Search Params (repeat per Header FK Searchable=Yes) ──
            // , string? {FK.virtualField}, string? {FK.virtualField}SearchType
            // ── End ──
            )
        {
            var data = await _repository.{Header}.Search{Header}Async(
                {headerField1}, {headerField1}SearchType, {headerField2}, {headerField2}SearchType
                // ── FK pass-through: , {FK.virtualField}, {FK.virtualField}SearchType ──
                );
            return data.Adapt<IEnumerable<{Header}Dto>>();
        }
    }
}
```

### 6.b `{Detail}Service.cs`

```csharp
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.{TargetLayer};
using Shared.DataTransferObjects;

namespace Service.{TargetLayer}
{
    public class {Detail}Service : I{Detail}Service
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public {Detail}Service(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<{Detail}Dto>> GetAllBy{Header}GuidAsync(Guid {header}Guid)
        {
            var entities = await _repository.{Detail}.GetBy{Header}GuidAsync({header}Guid, trackChanges: false);
            return entities.Adapt<IEnumerable<{Detail}Dto>>();
        }

        public async Task<{Detail}Dto> Get{Detail}ByGuidAsync(Guid {detail}Guid, bool trackChanges)
        {
            var entity = await _repository.{Detail}.Get{Detail}Async({detail}Guid, trackChanges);
            return entity.Adapt<{Detail}Dto>();
        }

        public async Task<{Detail}Dto> GetBy{Header}GuidAnd{Detail}GuidAsync(Guid {header}Guid, Guid {detail}Guid)
        {
            var entity = await _repository.{Detail}.GetBy{Header}GuidAnd{Detail}GuidAsync({header}Guid, {detail}Guid);
            return entity.Adapt<{Detail}Dto>();
        }

        public async Task<{Detail}Dto> Create{Detail}Async({Detail}ForCreationDto input)
        {
            var model = input.Adapt<{Detail}>();
            
            // Lookup parent ID jika HeaderId tidak di set
            if (model.{FKId} == 0 && model.{FKGuid} != Guid.Empty)
            {
                var header = await _repository.{Header}.Get{Header}Async(model.{FKGuid}, trackChanges: false);
                if (header != null) model.{FKId} = header.{HeaderId};
            }

            model.StatusId = 1;
            await _repository.{Detail}.Create{Detail}Async(model);
            return model.Adapt<{Detail}Dto>();
        }

        public async Task Update{Detail}Async(Guid {detail}Guid, {Detail}ForUpdateDto input, bool trackChanges)
        {
            var model = input.Adapt<{Detail}>();
            model.{DetailGuid} = {detail}Guid;
            
            if (model.{FKId} == 0 && model.{FKGuid} != Guid.Empty)
            {
                var header = await _repository.{Header}.Get{Header}Async(model.{FKGuid}, trackChanges: false);
                if (header != null) model.{FKId} = header.{HeaderId};
            }

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.{Detail}.Update{Detail}Async(model);
        }

        public async Task Delete{Detail}Async(Guid {detail}Guid, {Detail}ForDeleteDto input, bool trackChanges)
        {
            var model = new {Detail} { {DetailGuid} = {detail}Guid };
            await _repository.{Detail}.SoftDelete{Detail}Async(model, input.DeletedById);
        }

        public async Task Delete{Detail}ByAdminAsync(Guid {detail}Guid, bool trackChanges)
        {
            await _repository.{Detail}.Delete{Detail}Async({detail}Guid);
        }

        public async Task<IEnumerable<{Detail}Dto>> Search{Detail}Async(
            string? {detailField1}, string? {detailField1}SearchType, 
            string? {detailField2}, string? {detailField2}SearchType, 
            Guid {header}Guid, Guid {detail}Guid)
        {
            var data = await _repository.{Detail}.Search{Detail}Async(
                {detailField1}, {detailField1}SearchType, {detailField2}, {detailField2}SearchType, {header}Guid, {detail}Guid);
            return data.Adapt<IEnumerable<{Detail}Dto>>();
        }
    }
}
```

---

## 7. Controllers — `Presentation.{TargetLayer}/Controllers/`

### 7.a `{HeaderPlural}Controller.cs`

> Sama seperti Controller pada 1 Tabel (`TemplateApi1Table.md`).

### 7.b `{DetailPlural}Controller.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.{TargetLayer};
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.{TargetLayer}.Controllers
{
    [Route("api/{headerPlural}/{{headerGuid:guid}}/{detailPlural}")]
    [ApiController]
    public class {DetailPlural}Controller : ControllerBase
    {
        private readonly IService{TargetLayer}Manager _service;

        public {DetailPlural}Controller(IService{TargetLayer}Manager service)
        {
            _service = service;
        }

        /// <summary>Get all by Header Guid</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get All By Header", Description = "Mengambil semua detail berdasarkan Guid header.")]
        public async Task<IActionResult> GetAll(Guid headerGuid)
        {
            var data = await _service.{Detail}Service.GetAllBy{Header}GuidAsync(headerGuid);
            return Ok(data);
        }

        /// <summary>Get {Detail} by Guid</summary>
        [HttpGet("{{guid:guid}}")]
        [SwaggerOperation(Summary = "Get By Guid", Description = "Mengambil satu detail.")]
        public async Task<IActionResult> GetByGuid(Guid headerGuid, Guid guid)
        {
            var data = await _service.{Detail}Service.Get{Detail}ByGuidAsync(guid, trackChanges: false);
            return Ok(data);
        }

        /// <summary>Create {Detail}</summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create", Description = "Membuat data baru detail untuk header ini.")]
        public async Task<IActionResult> Create(Guid headerGuid, [FromBody] {Detail}ForCreationDto input)
        {
            if (input.{FKGuid} == Guid.Empty) input.{FKGuid} = headerGuid;

            var created = await _service.{Detail}Service.Create{Detail}Async(input);
            return CreatedAtAction(nameof(GetByGuid), new { headerGuid = headerGuid, guid = created.{DetailGuid} }, created);
        }

        /// <summary>Update {Detail}</summary>
        [HttpPut("{{guid:guid}}")]
        [SwaggerOperation(Summary = "Update", Description = "Memperbarui data detail.")]
        public async Task<IActionResult> Update(Guid headerGuid, Guid guid, [FromBody] {Detail}ForUpdateDto input)
        {
            if (input.{FKGuid} == Guid.Empty) input.{FKGuid} = headerGuid;

            await _service.{Detail}Service.Update{Detail}Async(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Soft delete {Detail}</summary>
        [HttpDelete("{{guid:guid}}")]
        [SwaggerOperation(Summary = "Soft Delete", Description = "Menandai data sebagai dihapus.")]
        public async Task<IActionResult> SoftDelete(Guid headerGuid, Guid guid, [FromBody] {Detail}ForDeleteDto input)
        {
            await _service.{Detail}Service.Delete{Detail}Async(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Hard delete {Detail} (Admin)</summary>
        [HttpDelete("admin/{{guid:guid}}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)", Description = "Menghapus data permanen.")]
        public async Task<IActionResult> HardDelete(Guid headerGuid, Guid guid)
        {
            await _service.{Detail}Service.Delete{Detail}ByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        /// <summary>Get detail by Header & Item Guid (Admin)</summary>
        [HttpGet("admin/item/{{guid:guid}}")]
        [SwaggerOperation(Summary = "Get Detail By Header & Item Guid", Description = "Mengambil satu detail dengan validasi relasi Header.")]
        public async Task<IActionResult> AdminGetByHeaderAndItem(Guid headerGuid, Guid guid)
        {
            var data = await _service.{Detail}Service.GetBy{Header}GuidAnd{Detail}GuidAsync(headerGuid, guid);
            return Ok(data);
        }

        /// <summary>Search {Detail}</summary>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search", Description = "Pencarian fleksibel.")]
        public async Task<IActionResult> Search([FromQuery] {Detail}SearchDto input, [FromQuery] Guid headerGuid, [FromQuery] Guid detailGuid)
        {
            var result = await _service.{Detail}Service.Search{Detail}Async(
                input.{DetailField1}, input.{DetailField1}SearchType.ToString(), 
                input.{DetailField2}, input.{DetailField2}SearchType.ToString(), 
                headerGuid, detailGuid);
            return Ok(result);
        }
    }
}
```

---

## 8. IService{TargetLayer}Manager — Tambah 2 Properties

```csharp
I{Header}Service {Header}Service { get; }
I{Detail}Service {Detail}Service { get; }
```

Di `Service{TargetLayer}Manager.cs` tambah lazy init untuk keduanya.

---

## 9. Mapster Configuration

```csharp
TypeAdapterConfig<{Header}, {Header}Dto>.NewConfig();
TypeAdapterConfig<{Header}ForCreationDto, {Header}>.NewConfig();
TypeAdapterConfig<{Header}ForUpdateDto, {Header}>.NewConfig();

TypeAdapterConfig<{Detail}, {Detail}Dto>.NewConfig();
TypeAdapterConfig<{Detail}ForCreationDto, {Detail}>.NewConfig();
TypeAdapterConfig<{Detail}ForUpdateDto, {Detail}>.NewConfig();
```

---

## 10. RepositoryManager — Tambah 2 Repository

```csharp
// IRepositoryManager.cs
I{Header}Repository {Header} { get; }
I{Detail}Repository {Detail} { get; }

// RepositoryManager.cs
private readonly Lazy<I{Header}Repository> _{header}Repository;
private readonly Lazy<I{Detail}Repository> _{detail}Repository;

// Constructor:
_{header}Repository = new Lazy<I{Header}Repository>(() => new {Header}Repository(context, auditService));
_{detail}Repository = new Lazy<I{Detail}Repository>(() => new {Detail}Repository(context, auditService));

// Properties:
public I{Header}Repository {Header} => _{header}Repository.Value;
public I{Detail}Repository {Detail} => _{detail}Repository.Value;
```

---

## INTEGRASI AKHIR — CHECKLIST

- [ ] `Shared/DataTransferObjects/{Header}Dto.cs` — Header DTO
- [ ] `Shared/DataTransferObjects/{Detail}Dto.cs` — Detail DTO
- [ ] `Entities/Models/{Header}.cs` — Header entity
- [ ] `Entities/Models/{Detail}.cs` — Detail entity
- [ ] `Contracts/I{Header}Repository.cs` — Header repo interface
- [ ] `Contracts/I{Detail}Repository.cs` — Detail repo interface
- [ ] `Repository/{Header}Repository.cs` — Header repo impl
- [ ] `Repository/{Detail}Repository.cs` — Detail repo impl
- [ ] `Contracts/IRepositoryManager.cs` — 2 properties ditambahkan
- [ ] `Repository/RepositoryManager.cs` — 2 lazy init ditambahkan
- [ ] `Service.Contracts.{TargetLayer}/I{Header}Service.cs` — Interface
- [ ] `Service.Contracts.{TargetLayer}/I{Detail}Service.cs` — Interface
- [ ] `Service.{TargetLayer}/{Header}Service.cs` — Implementasi
- [ ] `Service.{TargetLayer}/{Detail}Service.cs` — Implementasi
- [ ] `Service.Contracts.{TargetLayer}/IService{TargetLayer}Manager.cs` — 2 properties
- [ ] `Service.{TargetLayer}/Service{TargetLayer}Manager.cs` — 2 lazy init
- [ ] `Presentation.{TargetLayer}/Controllers/{HeaderPlural}Controller.cs`
- [ ] `Presentation.{TargetLayer}/Controllers/{DetailPlural}Controller.cs`
- [ ] `BamboeUp.Api/MapsterConfig.cs` — Mapping ditambahkan jika perlu

---

## CONTOH PROMPT EKSEKUSI

> "Gunakan **TemplateApiHeaderDetail.md**. Buatkan modul API **PurchaseOrder** di layer **Modules**.
>
> **Data:**
> - Header: PurchaseOrder (PurchaseOrders), Table: PurchaseOrder, Schema: app
> - Detail: PurchaseOrderLine (PurchaseOrderLines), Table: PurchaseOrderLine
> - FK Detail → Header: PurchaseOrderId, PurchaseOrderGuid
> - Header Fields: DocNumber, DocDate
> - Detail Fields: ItemCode, Qty
> - TargetLayer: Modules"
