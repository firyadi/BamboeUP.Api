# TemplateApi1Table_V2 — Panduan AI Membuat API untuk 1 Tabel di BamboeUp

> **Referensi presisi dari modul Bank & UserCompanyScope** (berlaku untuk `Presentation.Modules` atau `Presentation.Shell`).
> Dokumen ini berisi boilerplate lengkap yang harus diikuti oleh AI untuk menghasilkan kode API yang konsisten dengan standar BamboeUP.
> **PENTING:** Template ini sudah diverifikasi dari kode nyata. Jangan ubah pola tanpa alasan kuat.
> **V2:** Ditambahkan sistem FK Auto-Join untuk otomatis mendeteksi dan menampilkan relasi antar tabel.
> **Canonical V3:** `BamboeUP.SchemaStudio/SchemaStudio.Generator/Templates/Api/TemplateApi1Table.md` — salinan ini diselaraskan untuk nullable CS8601–CS8604, CS8618, CS8625.

---

## Nullable reference types (CS8601–CS8604, CS8618, CS8625)

Lihat tabel lengkap di template canonical V3. Ringkas: Repository `Task<{Entity}?>`, Service `Task<{Entity}Dto?>`, DTO/Entity string NOT NULL `= string.Empty`, transaksi `IDbTransaction? transaction = null`, Controller `NotFound()` jika data null.

---

## CARA MENGGUNAKAN TEMPLATE INI

Ganti semua placeholder berikut:

| Placeholder | Contoh (Bank) | Penjelasan |
|---|---|---|
| `{TargetLayer}` | `Modules` | `Shell` atau `Modules` |
| `{Entity}` | `Bank` | Nama entitas PascalCase (singular) |
| `{entity}` | `bank` | Nama entitas camelCase / lowercase |
| `{EntityPlural}` | `Banks` | Plural PascalCase |
| `{entityPlural}` | `banks` | Plural lowercase untuk URL route |
| `{Field1}` | `BankName` | Field utama tabel |
| `{Field2}` | `BankInitial` | Field kedua tabel |
| `{field1}` | `bankName` | camelCase Field1 (untuk parameter search) |
| `{field2}` | `bankInitial` | camelCase Field2 (untuk parameter search) |
| `{IdColumn}` | `BankId` | Nama kolom PK (bigint) |
| `{GuidColumn}` | `BankGuid` | Nama kolom GUID |
| `{TableName}` | `Bank` | Nama tabel di database |
| `{Schema}` | `core` | Nama skema di database (core, app, dll) |

### FK Join Placeholders (per FK entry — lihat FK Join Configuration)

| Placeholder | Contoh | Penjelasan |
|---|---|---|
| `{FK.Field}` | `CompanyId` atau `SrAddressType` | Nama kolom FK |
| `{FK.Type}` | `FK` atau `Sr` | Tipe join |
| `{FK.Table}` | `Company` atau `vw_StandardReference_Display` | Tabel target |
| `{FK.Schema}` | `app` | Schema tabel target |
| `{FK.DisplayColumn}` | `CompanyName` atau `StandardReferenceItemName` | Kolom display di tabel target |
| `{FK.VirtualField}` | `CompanyName` atau `AddressTypeName` | Virtual field di DTO |
| `{FK.virtualField}` | `companyName` atau `addressTypeName` | camelCase virtual field |
| `{FK.JoinType}` | `LEFT JOIN` | `JOIN` atau `LEFT JOIN` |
| `{FK.Alias}` | `j_CompanyId` atau `v_SrAddressType` | Alias SQL |

---

## FK JOIN CONFIGURATION (Auto-Detect)

> **Konsep:** Setiap kolom berakhiran `Id` yang bukan system field otomatis terdeteksi sebagai FK candidate.
> Hasilnya bisa dikoreksi user di form SchemaStudio sebelum generate. Konfigurasi disimpan untuk API, UI, Help.

### Aturan Auto-Deteksi

1. Scan semua kolom berakhiran `Id`
2. **Kecualikan:** PK sendiri (`{IdColumn}`), `StatusId`, `CreatedById`, `UpdatedById`, `DeletedById`
3. Kolom dimulai `Sr` → Type = `Sr`, Table = `vw_StandardReference_Display`, Schema = `app`, DisplayColumn = `StandardReferenceItemName`, VirtualField = `{FieldTanpaSr}Name`
4. Kolom lainnya → Type = `FK`, Table = nama tanpa `Id`, DisplayColumn = `{Table}Name`, VirtualField = `{Table}Name`
5. Default JoinType = `LEFT JOIN` (bisa diubah ke `JOIN` jika field NOT NULL / wajib ada)
6. **User bisa override** semua kolom di form SchemaStudio

### Alias Convention

- FK Reguler: `j_{FK.Field}` — contoh: `j_CompanyId`
- Sr Reference: `v_{FK.Field}` — contoh: `v_SrAddressType`

### Excluded System Fields

```
{IdColumn}, StatusId, CreatedById, UpdatedById, DeletedById
```

### Contoh Tabel FK Join Configuration (UserCompanyScope)

| # | FK.Field | FK.Type | FK.Table | FK.Schema | FK.DisplayColumn | FK.VirtualField | FK.JoinType | Searchable |
|---|---|---|---|---|---|---|---|---|
| 1 | CompanyId | FK | Company | app | CompanyName | CompanyName | LEFT JOIN | Yes |
| 2 | CompanyOfficeId | FK | CompanyOffice | app | CompanyOfficeName | CompanyOfficeName | LEFT JOIN | Yes |
| 3 | UserId | FK | Users | core | FullName | UserFullName | LEFT JOIN | No |

> **Catatan:** `UserId` → tabel `Users` memiliki `FullName` bukan `UsersName`, maka `FK.DisplayColumn` di-override ke `FullName`.
> Field `IsDefaultCompany`, `IsDefaultOffice` tidak berakhiran `Id` sehingga tidak terdeteksi sebagai FK.

---

## 1. DTO — `Shared/DataTransferObjects/{Entity}Dto.cs`

> File ini ada di project `Shared`. Dibuat sekali, dipakai bersama oleh API dan UI.

```csharp
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record {Entity}Dto
    {
        public long {IdColumn} { get; set; }
        public Guid {GuidColumn} { get; init; }
        public string {Field1} { get; set; } = string.Empty;
        public string {Field2} { get; set; } = string.Empty;
        // ── FK Virtual Fields (repeat per FK Join entry) ──
        // Untuk setiap FK: public string? {FK.VirtualField} { get; set; }
        // Contoh:
        // public string? CompanyName { get; set; }          // FK: CompanyId → Company
        // public string? CompanyOfficeName { get; set; }    // FK: CompanyOfficeId → CompanyOffice
        // public string? AddressTypeName { get; set; }      // Sr: SrAddressType → vw_StandardReference_Display
        // ── End FK Virtual Fields ──
        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record {Entity}ForCreationDto
    {
        public string {Field1} { get; set; } = string.Empty;
        public string {Field2} { get; set; } = string.Empty;
        // ── FK Fields (hanya physical FK columns, BUKAN virtual) ──
        // Contoh: public long CompanyId { get; set; }
        //         public long SrAddressType { get; set; }
        // ── End FK Fields ──
        public long CreatedById { get; set; } = 0;
    }

    public record {Entity}ForUpdateDto
    {
        public string {Field1} { get; set; } = string.Empty;
        public string {Field2} { get; set; } = string.Empty;
        // ── FK Fields (sama seperti CreationDto) ──
        public long UpdatedById { get; set; }
    }

    public record {Entity}ForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class {Entity}SearchDto
    {
        public string? {Field1} { get; set; }
        public SearchType {Field1}SearchType { get; set; } = SearchType.Contains;

        public string? {Field2} { get; set; }
        public SearchType {Field2}SearchType { get; set; } = SearchType.Contains;

        // ── FK Virtual Search Fields (repeat per FK Join entry yg Searchable=Yes) ──
        // Untuk setiap FK Searchable:
        // public string? {FK.VirtualField} { get; set; }
        // public SearchType {FK.VirtualField}SearchType { get; set; } = SearchType.Contains;
        // ── End FK Virtual Search Fields ──
    }
}
```

---

## 2. Entity — `Entities/Models/{Entity}.cs`

> Jika entity belum ada, buat di sini. Ikuti pola yang sudah ada persis.
> **Entity hanya berisi physical columns dari database.** Virtual fields dari FK Join TIDAK ada di Entity.

```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("{TableName}", Schema = "{Schema}")]
    public class {Entity}
    {
        [Column("{IdColumn}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long {IdColumn} { get; set; }

        [Key]
        [Column("{GuidColumn}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid {GuidColumn} { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string {Field1} { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string {Field2} { get; set; } = string.Empty;

        // ── FK Physical Columns (repeat per FK Join entry) ──
        // Contoh: public long CompanyId { get; set; }
        //         public long SrAddressType { get; set; }
        // ── End FK Physical Columns ──

        // ── FK Virtual Fields untuk Dapper mapping (repeat per FK Join entry) ──
        // PENTING: Tambahkan virtual property agar Dapper bisa map hasil JOIN
        // Contoh: public string? CompanyName { get; set; }
        //         public string? AddressTypeName { get; set; }
        // ── End FK Virtual Fields ──

        [Column("StatusId")]
        public long StatusId { get; set; } = 0;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Column("CreatedById")]
        public long CreatedById { get; set; } = 0;

        [Required]
        public DateTime CreatedTime { get; set; }

        [Column("UpdatedById")]
        public long UpdatedById { get; set; } = 0;

        public DateTime? UpdatedTime { get; set; }

        [Column("DeletedById")]
        public long DeletedById { get; set; } = 0;

        public DateTime? DeletedTime { get; set; }
    }
}
```

---

## 3. Repository Contract — `Contracts/I{Entity}Repository.cs`

```csharp
using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface I{Entity}Repository
    {
        Task<{Entity}?> Get{Entity}Async(Guid {entity}Guid, bool trackChanges);
        Task<IEnumerable<{Entity}>> GetAll{EntityPlural}Async(bool trackChanges);

        Task Create{Entity}Async({Entity} {entity}, IDbTransaction? transaction = null);
        Task Update{Entity}Async({Entity} {entity}, IDbTransaction? transaction = null);
        Task Delete{Entity}Async(Guid {entity}Guid, IDbTransaction? transaction = null);
        Task SoftDelete{Entity}Async({Entity} {entity}, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<{Entity}>> Search{Entity}Async(
            string? {field1}, string? {field1}SearchType,
            string? {field2}, string? {field2}SearchType,
            // ── FK Virtual Search Params (repeat per FK Searchable=Yes) ──
            // string? {FK.virtualField}, string? {FK.virtualField}SearchType,
            // ── End FK Virtual Search Params ──
            IDbTransaction? transaction = null);
    }
}
```

---

## 4. Repository Implementation — `Repository/{Entity}Repository.cs`

```csharp
using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public class {Entity}Repository : I{Entity}Repository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;

        public {Entity}Repository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _audit = auditService;
        }

        public async Task<{Entity}?> Get{Entity}Async(Guid {entity}Guid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT a.*
                    -- ── FK JOIN SELECT (repeat per FK) ──
                    -- FK:  , j_{FK.Field}.{FK.DisplayColumn} AS {FK.VirtualField}
                    -- Sr:  , v_{FK.Field}.StandardReferenceItemName AS {FK.VirtualField}
                    -- ── End FK JOIN SELECT ──
                FROM [{Schema}].[{TableName}] a
                    -- ── FK JOIN FROM (repeat per FK) ──
                    -- FK:  {FK.JoinType} [{FK.Schema}].[{FK.Table}] j_{FK.Field} ON a.{FK.Field} = j_{FK.Field}.{FK.Field}
                    -- Sr:  LEFT JOIN [app].[vw_StandardReference_Display] v_{FK.Field} ON a.{FK.Field} = v_{FK.Field}.StandardReferenceItemId
                    -- ── End FK JOIN FROM ──
                WHERE a.{GuidColumn} = @{entity}Guid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<{Entity}>(sql, new { {entity}Guid });
        }

        public async Task<IEnumerable<{Entity}>> GetAll{EntityPlural}Async(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT a.*
                    -- ── FK JOIN SELECT (same as Get) ──
                FROM [{Schema}].[{TableName}] a
                    -- ── FK JOIN FROM (same as Get) ──
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.{IdColumn} DESC";
            return await connection.QueryAsync<{Entity}>(sql);
        }

        public async Task Create{Entity}Async({Entity} {entity}, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [{Schema}].[{TableName}]
                ({GuidColumn}, CreatedById, StatusId, CreatedTime, {Field1}, {Field2}
                    -- ── FK Physical Columns: , {FK.Field} (repeat per FK) ──
                )
                VALUES
                (@{GuidColumn}, @CreatedById, @StatusId, @CreatedTime, @{Field1}, @{Field2}
                    -- ── FK Physical Params: , @{FK.Field} (repeat per FK) ──
                )";
            await conn.ExecuteAsync(sql, {entity}, transaction);

            await _audit.LogAsync(
                actionType: "CREATE",
                tableName: "{TableName}",
                primaryKey: {entity}.{GuidColumn}.ToString(),
                userId: {entity}.CreatedById.ToString(),
                oldEntity: null,
                newEntity: {entity});
        }

        public async Task Update{Entity}Async({Entity} {entity}, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await Get{Entity}Async({entity}.{GuidColumn}, false);

            const string sql = @"
                UPDATE [{Schema}].[{TableName}]
                SET {Field1} = @{Field1},
                    {Field2} = @{Field2},
                    -- ── FK Physical Columns: {FK.Field} = @{FK.Field}, (repeat per FK) ──
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE {GuidColumn} = @{GuidColumn}";
            await conn.ExecuteAsync(sql, {entity}, transaction);

            await _audit.LogAsync(
                actionType: "UPDATE",
                tableName: "{TableName}",
                primaryKey: {entity}.{GuidColumn}.ToString(),
                userId: {entity}.UpdatedById.ToString(),
                oldEntity: oldData,
                newEntity: {entity});
        }

        public async Task SoftDelete{Entity}Async({Entity} {entity}, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await Get{Entity}Async({entity}.{GuidColumn}, false);

            const string sql = @"
                UPDATE [{Schema}].[{TableName}]
                SET StatusId = 0,
                    DeletedById = @DeletedBy,
                    DeletedTime = GETUTCDATE()
                WHERE {GuidColumn} = @{GuidColumn}";

            await conn.ExecuteAsync(sql, new
            {
                {entity}.{GuidColumn},
                DeletedBy = deletedBy
            }, transaction);

            await _audit.LogAsync(
                actionType: "DELETE",
                tableName: "{TableName}",
                primaryKey: {entity}.{GuidColumn}.ToString(),
                userId: deletedBy.ToString(),
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task Delete{Entity}Async(Guid {entity}Guid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await Get{Entity}Async({entity}Guid, false);

            const string sql = @"DELETE FROM [{Schema}].[{TableName}] WHERE {GuidColumn} = @{entity}Guid";
            await conn.ExecuteAsync(sql, new { {entity}Guid }, transaction);

            await _audit.LogAsync(
                actionType: "DELETE_ADMIN",
                tableName: "{TableName}",
                primaryKey: {entity}Guid.ToString(),
                userId: oldData?.DeletedById.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task<IEnumerable<{Entity}>> Search{Entity}Async(
            string? {field1}, string? {field1}SearchType,
            string? {field2}, string? {field2}SearchType,
            // ── FK Virtual Search Params (repeat per FK Searchable=Yes) ──
            // string? {FK.virtualField}, string? {FK.virtualField}SearchType,
            // ── End ──
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string>
            {
                "a.StatusId > 0",
                "a.DeletedTime IS NULL"
            };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace({field1}))
            {
                var param = SqlFilterHelper.BuildFilter("a.{Field1}", "@{field1}", {field1}SearchType, parameters, "{field1}", {field1});
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace({field2}))
            {
                var param = SqlFilterHelper.BuildFilter("a.{Field2}", "@{field2}", {field2}SearchType, parameters, "{field2}", {field2});
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            // ── FK Virtual Search Filters (repeat per FK Searchable=Yes) ──
            // FK Reguler:
            // if (!string.IsNullOrWhiteSpace({FK.virtualField}))
            // {
            //     var param = SqlFilterHelper.BuildFilter("j_{FK.Field}.{FK.DisplayColumn}", "@{FK.virtualField}", {FK.virtualField}SearchType, parameters, "{FK.virtualField}", {FK.virtualField});
            //     if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            // }
            // Sr Reference:
            // if (!string.IsNullOrWhiteSpace({FK.virtualField}))
            // {
            //     var param = SqlFilterHelper.BuildFilter("v_{FK.Field}.StandardReferenceItemName", "@{FK.virtualField}", {FK.virtualField}SearchType, parameters, "{FK.virtualField}", {FK.virtualField});
            //     if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            // }
            // ── End FK Virtual Search Filters ──

            var sql = $@"
                SELECT a.*
                    -- ── FK JOIN SELECT (repeat per FK) ──
                    -- FK:  , j_{FK.Field}.{FK.DisplayColumn} AS {FK.VirtualField}
                    -- Sr:  , v_{FK.Field}.StandardReferenceItemName AS {FK.VirtualField}
                    -- ── End FK JOIN SELECT ──
                FROM [{Schema}].[{TableName}] a
                    -- ── FK JOIN FROM (repeat per FK) ──
                    -- FK:  {FK.JoinType} [{FK.Schema}].[{FK.Table}] j_{FK.Field} ON a.{FK.Field} = j_{FK.Field}.{FK.Field}
                    -- Sr:  LEFT JOIN [app].[vw_StandardReference_Display] v_{FK.Field} ON a.{FK.Field} = v_{FK.Field}.StandardReferenceItemId
                    -- ── End FK JOIN FROM ──
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.{IdColumn} DESC";

            return await connection.QueryAsync<{Entity}>(sql, parameters, transaction);
        }
    }
}
```

---

## 5. Service Contract — `Service.Contracts.{TargetLayer}/I{Entity}Service.cs`

> Simpan di `Service.Contracts.Shell` atau `Service.Contracts.Modules` sesuai target.

```csharp
using Shared.DataTransferObjects;

namespace Service.Contracts.{TargetLayer}
{
    public interface I{Entity}Service
    {
        Task<IEnumerable<{Entity}Dto>> GetAll{EntityPlural}Async(bool trackChanges);
        Task<{Entity}Dto?> Get{Entity}ByGuidAsync(Guid {entity}Guid, bool trackChanges);
        Task<{Entity}Dto> Create{Entity}Async({Entity}ForCreationDto input);
        Task Update{Entity}Async(Guid {entity}Guid, {Entity}ForUpdateDto input, bool trackChanges);
        Task Delete{Entity}Async(Guid {entity}Guid, {Entity}ForDeleteDto input, bool trackChanges);
        Task Delete{Entity}ByAdminAsync(Guid {entity}Guid, bool trackChanges);

        Task<IEnumerable<{Entity}Dto>> Search{Entity}Async(
            string? {field1}, string? {field1}SearchType, 
            string? {field2}, string? {field2}SearchType
            // ── FK Virtual Search Params (repeat per FK Searchable=Yes) ──
            // , string? {FK.virtualField}, string? {FK.virtualField}SearchType
            // ── End ──
        );
    }
}
```

---

## 6. Service Implementation — `Service.{TargetLayer}/{Entity}Service.cs`

```csharp
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.{TargetLayer};
using Shared.DataTransferObjects;

namespace Service.{TargetLayer}
{
    public class {Entity}Service : I{Entity}Service
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public {Entity}Service(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<{Entity}Dto>> GetAll{EntityPlural}Async(bool trackChanges)
        {
            var entities = await _repository.{Entity}.GetAll{EntityPlural}Async(trackChanges);
            return entities.Adapt<IEnumerable<{Entity}Dto>>();
        }

        public async Task<{Entity}Dto?> Get{Entity}ByGuidAsync(Guid {entity}Guid, bool trackChanges)
        {
            var entity = await _repository.{Entity}.Get{Entity}Async({entity}Guid, trackChanges);
            if (entity == null) return null;
            return entity.Adapt<{Entity}Dto>();
        }

        public async Task<{Entity}Dto> Create{Entity}Async({Entity}ForCreationDto input)
        {
            var model = input.Adapt<{Entity}>();
            model.StatusId = 1;
            await _repository.{Entity}.Create{Entity}Async(model);
            return model.Adapt<{Entity}Dto>();
        }

        public async Task Update{Entity}Async(Guid {entity}Guid, {Entity}ForUpdateDto input, bool trackChanges)
        {
            var model = input.Adapt<{Entity}>();
            model.{GuidColumn} = {entity}Guid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.{Entity}.Update{Entity}Async(model);
        }

        public async Task Delete{Entity}Async(Guid {entity}Guid, {Entity}ForDeleteDto input, bool trackChanges)
        {
            var model = new {Entity} { {GuidColumn} = {entity}Guid };
            await _repository.{Entity}.SoftDelete{Entity}Async(model, input.DeletedById);
        }

        public async Task Delete{Entity}ByAdminAsync(Guid {entity}Guid, bool trackChanges)
        {
            await _repository.{Entity}.Delete{Entity}Async({entity}Guid);
        }

        public async Task<IEnumerable<{Entity}Dto>> Search{Entity}Async(
            string? {field1}, string? {field1}SearchType, 
            string? {field2}, string? {field2}SearchType
            // ── FK Virtual Search Params (repeat per FK Searchable=Yes) ──
            // , string? {FK.virtualField}, string? {FK.virtualField}SearchType
            // ── End ──
            )
        {
            var data = await _repository.{Entity}.Search{Entity}Async(
                {field1}, {field1}SearchType, {field2}, {field2}SearchType
                // ── FK pass-through: , {FK.virtualField}, {FK.virtualField}SearchType ──
                );
            return data.Adapt<IEnumerable<{Entity}Dto>>();
        }
    }
}
```

---

## 7. Controller — `Presentation.{TargetLayer}/Controllers/{EntityPlural}Controller.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.{TargetLayer};
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.{TargetLayer}.Controllers
{
    [Route("api/{entityPlural}")]
    [ApiController]
    public class {EntityPlural}Controller : ControllerBase
    {
        private readonly IService{TargetLayer}Manager _service;

        public {EntityPlural}Controller(IService{TargetLayer}Manager service)
        {
            _service = service;
        }

        /// <summary>Get all {EntityPlural}</summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get All", Description = "Mengambil semua data {EntityPlural} yang aktif.")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.{Entity}Service.GetAll{EntityPlural}Async(trackChanges: false);
            return Ok(data);
        }

        /// <summary>Get {Entity} by Guid</summary>
        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid", Description = "Mengambil satu data berdasarkan Guid.")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await _service.{Entity}Service.Get{Entity}ByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        /// <summary>Create {Entity}</summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create", Description = "Membuat data baru.")]
        public async Task<IActionResult> Create([FromBody] {Entity}ForCreationDto input)
        {
            var created = await _service.{Entity}Service.Create{Entity}Async(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.{GuidColumn} }, created);
        }

        /// <summary>Update {Entity}</summary>
        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update", Description = "Memperbarui data berdasarkan Guid.")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] {Entity}ForUpdateDto input)
        {
            await _service.{Entity}Service.Update{Entity}Async(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Soft delete {Entity}</summary>
        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete", Description = "Menandai data sebagai dihapus (soft delete).")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] {Entity}ForDeleteDto input)
        {
            await _service.{Entity}Service.Delete{Entity}Async(guid, input, trackChanges: true);
            return NoContent();
        }

        /// <summary>Hard delete {Entity} (Admin)</summary>
        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)", Description = "Menghapus data secara permanen.")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await _service.{Entity}Service.Delete{Entity}ByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        /// <summary>Search {EntityPlural}</summary>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search", Description = "Pencarian fleksibel menggunakan filter dinamis.")]
        public async Task<IActionResult> Search([FromQuery] {Entity}SearchDto input)
        {
            var result = await _service.{Entity}Service.Search{Entity}Async(
                input.{Field1}, input.{Field1}SearchType.ToString(), 
                input.{Field2}, input.{Field2}SearchType.ToString()
                // ── FK Search pass-through (repeat per FK Searchable=Yes) ──
                // , input.{FK.VirtualField}, input.{FK.VirtualField}SearchType.ToString()
                // ── End ──
            );
            return Ok(result);
        }
    }
}
```

---

## 8. Mapster Configuration — `BamboeUp.Api/MapsterConfig.cs`

> Tambahkan register Mapster jika diperlukan (biasanya Mapster sudah otomatis menangani mapping properties yang sama namanya, tetapi terkadang butuh CustomConfig).

```csharp
TypeAdapterConfig<{Entity}, {Entity}Dto>.NewConfig();
TypeAdapterConfig<{Entity}ForCreationDto, {Entity}>.NewConfig();
TypeAdapterConfig<{Entity}ForUpdateDto, {Entity}>.NewConfig();
```

---

## 9. RepositoryManager — `Repository/RepositoryManager.cs`

Tambahkan property dan lazy initialization:

```csharp
// Di field declarations:
private readonly Lazy<I{Entity}Repository> _{entity}Repository;

// Di constructor:
_{entity}Repository = new Lazy<I{Entity}Repository>(() => new {Entity}Repository(context, auditService));

// Di property accessor:
public I{Entity}Repository {Entity} => _{entity}Repository.Value;
```

---

## 10. IRepositoryManager — `Contracts/IRepositoryManager.cs`

```csharp
I{Entity}Repository {Entity} { get; }
```

---

## 11. IService{TargetLayer}Manager — Tambah Property

Di `Service.Contracts.{TargetLayer}/IService{TargetLayer}Manager.cs`:

```csharp
I{Entity}Service {Entity}Service { get; }
```

Di `Service.{TargetLayer}/Service{TargetLayer}Manager.cs`:

```csharp
// Di field:
private readonly Lazy<I{Entity}Service> _{entity}Service;

// Di constructor:
_{entity}Service = new Lazy<I{Entity}Service>(() =>
    new {Entity}Service(repositoryManager, logger, transactionManager));

// Di property:
public I{Entity}Service {Entity}Service => _{entity}Service.Value;
```

---

## INTEGRASI AKHIR — CHECKLIST

- [ ] `Shared/DataTransferObjects/{Entity}Dto.cs` — DTO selesai
- [ ] `Entities/Models/{Entity}.cs` — Entity class selesai
- [ ] `Contracts/I{Entity}Repository.cs` — Interface repository selesai
- [ ] `Repository/{Entity}Repository.cs` — Implementasi repository selesai (Dapper, IAuditService)
- [ ] `Contracts/IRepositoryManager.cs` — Property `{Entity}` ditambahkan
- [ ] `Repository/RepositoryManager.cs` — Lazy init ditambahkan
- [ ] `Service.Contracts.{TargetLayer}/I{Entity}Service.cs` — Interface service selesai
- [ ] `Service.{TargetLayer}/{Entity}Service.cs` — Implementasi service selesai (Mapster)
- [ ] `Service.Contracts.{TargetLayer}/IService{TargetLayer}Manager.cs` — Property ditambahkan
- [ ] `Service.{TargetLayer}/Service{TargetLayer}Manager.cs` — Lazy init ditambahkan
- [ ] `Presentation.{TargetLayer}/Controllers/{EntityPlural}Controller.cs` — Controller selesai (Swagger)
- [ ] `BamboeUp.Api/MapsterConfig.cs` — Mapping ditambahkan jika perlu

---

## CONTOH PROMPT EKSEKUSI

> "Gunakan **TemplateApi1Table_V2.md**. Buatkan modul API **Vehicle** di layer **Modules**.
>
> **Data:**
> - Entity: Vehicle (Vehicles), Table: Vehicle (VehicleId, VehicleGuid)
> - Schema: core
> - Fields: LicensePlate, Brand
> - Field camelCase: licensePlate, brand
> - TargetLayer: Modules
>
> **FK Join Configuration:**
> | FK.Field | FK.Type | FK.Table | FK.Schema | FK.DisplayColumn | FK.VirtualField | FK.JoinType | Searchable |
> |---|---|---|---|---|---|---|---|
> | SrVehicleType | Sr | vw_StandardReference_Display | app | StandardReferenceItemName | VehicleTypeName | LEFT JOIN | Yes |
> | CompanyId | FK | Company | app | CompanyName | CompanyName | LEFT JOIN | Yes |"
