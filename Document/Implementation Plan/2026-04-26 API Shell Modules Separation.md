# Implementation Plan: API Shell & Modules Separation

**Tanggal:** 2026-04-26
**Author:** Developer Utama
**Status:** Draft — Menunggu Persetujuan

---

## 1. Latar Belakang & Tujuan

Project UI sudah menerapkan pola pemisahan:
- **BamboeUp.AppShell** → Form krusial, restricted, hanya developer utama
- **BamboeUp.AppModules** → Form development, open, semua developer

Tujuan rencana ini adalah menerapkan pola yang **sama** untuk project API, sehingga:
- Source code fitur krusial (Auth, User, Security) **tidak bisa dilihat atau diubah** oleh developer biasa
- Developer biasa tetap bisa develop fitur module baru dengan bebas
- **API tetap berjalan sebagai satu server** — tidak ada perubahan di frontend

---

## 2. Analisis Struktur Saat Ini

### Solution: Server.Api.sln

```
BamboeUp.Api/
├── BamboeUp.Api/           ← Entry point (Program.cs, MappingProfile, dll)
├── Presentation/           ← Semua Controllers (28 controllers) ← AKAN DIPECAH
├── Service/                ← Semua Services (27 services) ← AKAN DIPECAH
├── Service.Contracts/      ← Interface services (IServiceManager, dll) ← AKAN DIPECAH
├── Repository/             ← Data access (tetap)
├── Contracts/              ← Interface repository (tetap)
├── Entities/               ← Entity/Model DB (tetap)
├── Shared/                 ← DTOs (tetap)
├── Audit/                  ← Audit log DLL (tetap)
├── LoggerService/          ← Logger (tetap)
└── Table/                  ← Table definitions (tetap)
```

---

## 3. Target Struktur Setelah Implementasi

```
BamboeUp.Api/
├── BamboeUp.Api/                  ← Entry point (TIDAK BERUBAH)
├── [SHARED LAYERS — Tidak Berubah]
│   ├── Repository/
│   ├── Contracts/
│   ├── Entities/
│   ├── Shared/
│   ├── Audit/
│   ├── LoggerService/
│   └── Table/
│
├── [SHELL — Restricted, Developer Utama Only]
│   ├── Presentation.Shell/
│   │   ├── ActionFilters/
│   │   │   └── ValidationFilterAttribute.cs
│   │   ├── Controllers/                       (19 controller krusial)
│   │   └── Presentation.Shell.csproj
│   ├── Service.Shell/
│   │   ├── Approval/                          (4 service approval)
│   │   ├── Extensions/
│   │   │   └── ServiceShellConfiguration.cs
│   │   ├── ServiceShellManager.cs
│   │   └── Service.Shell.csproj
│   └── Service.Contracts.Shell/
│       ├── Approval/
│       ├── IServiceShellManager.cs
│       └── Service.Contracts.Shell.csproj
│
└── [MODULES — Open, Semua Developer]
    ├── Presentation.Modules/
    │   ├── Controllers/                       (9 controller module)
    │   └── Presentation.Modules.csproj
    ├── Service.Modules/
    │   ├── Extensions/
    │   │   └── ServiceModulesConfiguration.cs
    │   ├── ServiceModulesManager.cs
    │   └── Service.Modules.csproj
    └── Service.Contracts.Modules/
        ├── IServiceModulesManager.cs
        └── Service.Contracts.Modules.csproj
```

---

## 4. Kategorisasi Controller & Service

### SHELL — Controller Krusial (Restricted)

| Controller | Service | Service Contract |
|---|---|---|
| AuthController | AuthenticationService | IAuthenticationService |
| UsersController | UserService | IUserService |
| UserGroupsController | UserGroupService | IUserGroupService |
| UserGroupProgramsController | UserGroupProgramService | IUserGroupProgramService |
| UserGroupScopeController | UserGroupScopeService | IUserGroupScopeService |
| UserCompanyScopeController | UserCompanyScopeService | IUserCompanyScopeService |
| ProgramsController | ProgramService | IProgramService |
| CompaniesController | CompanyService | ICompanyService |
| CompanyOfficesController | CompanyOfficeService | ICompanyOfficeService |
| AuditLogViewController | (pakai Audit DLL) | (N/A) |
| AutoNumbersController | AutoNumberService | IAutoNumberService |
| ApprovalTemplatesController | ApprovalTemplateService | IApprovalTemplateService |
| ApprovalsController | ApprovalService | IApprovalService |
| ApprovalDelegationsController | ApprovalDelegationService | IApprovalDelegationService |
| ParametersController | ParameterService | IParameterService |
| StandardReferencesController | StandardReferenceService | IStandardReferenceService |
| StandardReferenceItemsController | StandardReferenceItemService | IStandardReferenceItemService |
| StandardReferenceDisplaysController | StandardReferenceDisplayService | IStandardReferenceDisplayService |
| VwStandardReferenceItemsController | VwStandardReferenceItemService | IVwStandardReferenceItemService |

**Total: 19 controllers, ~19 services**

### MODULES — Controller Development (Open)

| Controller | Service | Service Contract |
|---|---|---|
| BanksController | BankService | IBankService |
| CitiesController | CityService | ICityService |
| CountriesController | CountryService | ICountryService |
| DistrictsController | DistrictService | IDistrictService |
| ProvincesController | ProvinceService | IProvinceService |
| SubDistrictsController | SubDistrictService | ISubDistrictService |
| PostalCodesController | PostalCodeService | IPostalCodeService |
| HolidaysController | HolidayService | IHolidayService |
| AdministrativeAreaDisplaysController | AdministrativeAreaDisplayService | IAdministrativeAreaDisplayService |

**Total: 9 controllers, 9 services**

---

## 5. Dependency Graph Baru

```
BamboeUp.Api (Entry Point)
├── Presentation.Shell  → Service.Shell  → Service.Contracts.Shell  → Entities, Shared
│                                        ↘ Contracts (IRepository)
├── Presentation.Modules → Service.Modules → Service.Contracts.Modules → Entities, Shared
│                                          ↘ Contracts (IRepository)
├── Repository (shared oleh kedua Service)
├── Contracts (shared)
└── [Shared Layers lainnya]
```

### Project Reference Table

| Project | References |
|---|---|
| Service.Contracts.Shell | Entities, Shared |
| Service.Contracts.Modules | Entities, Shared |
| Service.Shell | Service.Contracts.Shell, Contracts, Entities, Shared |
| Service.Modules | Service.Contracts.Modules, Contracts, Entities, Shared |
| Presentation.Shell | Service.Shell, Service.Contracts.Shell, Shared |
| Presentation.Modules | Service.Modules, Service.Contracts.Modules, Shared |
| BamboeUp.Api | Presentation.Shell, Presentation.Modules, Repository, Contracts, Shared, LoggerService |

---

## 6. Rencana Pengerjaan (Step-by-Step)

### Phase 1: Setup Project Baru

- Buat 6 project baru dengan dotnet CLI
- Daftarkan ke Server.Api.sln
- Setup .csproj dengan NuGet packages dan project references

### Phase 2: Migrasi Service.Contracts

- Buat IServiceShellManager.cs di Service.Contracts.Shell
- Pindahkan 19 interface krusial ke Service.Contracts.Shell
- Buat IServiceModulesManager.cs di Service.Contracts.Modules
- Pindahkan 9 interface modules ke Service.Contracts.Modules
- Update namespace dari Service.Contracts → Service.Contracts.Shell / Modules

### Phase 3: Migrasi Service Implementation

- Pindahkan 19 service beserta Approval/ ke Service.Shell
- Buat ServiceShellManager.cs
- Buat ServiceShellConfiguration.cs (DI extension)
- Pindahkan 9 service ke Service.Modules
- Buat ServiceModulesManager.cs
- Buat ServiceModulesConfiguration.cs (DI extension)
- Update namespace: Service → Service.Shell / Service.Modules

### Phase 4: Migrasi Presentation (Controllers)

- Pindahkan ValidationFilterAttribute.cs ke Presentation.Shell/ActionFilters
- Pindahkan 19 controller ke Presentation.Shell/Controllers
- Update namespace dan using ke IServiceShellManager
- Pindahkan 9 controller ke Presentation.Modules/Controllers
- Update namespace dan using ke IServiceModulesManager

### Phase 5: Update Entry Point (BamboeUp.Api)

- Update BamboeUp.Api.csproj → hapus ref lama, tambah Shell+Modules
- Update Program.cs → using statements baru
- Update ServiceExtensions.cs → register kedua ServiceManager
- Verify MappingProfile.cs → namespace imports tetap valid

### Phase 6: Bersihkan Project Lama

- Hapus folder Presentation/, Service/, Service.Contracts/ lama
- Update Server.Api.sln → hapus entry project lama

### Phase 7: Buat Modules Solution

- Buat Server.Api.Modules.sln
- Tambah shared layers + Modules project
- Setup HintPath untuk compiled DLL Shell

### Phase 8: Verifikasi & Testing

- Build Server.Api.sln → 0 error
- Test semua endpoint Shell dan Modules via Swagger
- Build Server.Api.Modules.sln → 0 error

---

## 7. Files Summary

### BARU (Create)
- Presentation.Shell/Presentation.Shell.csproj
- Presentation.Modules/Presentation.Modules.csproj
- Service.Shell/Service.Shell.csproj
- Service.Modules/Service.Modules.csproj
- Service.Contracts.Shell/Service.Contracts.Shell.csproj
- Service.Contracts.Modules/Service.Contracts.Modules.csproj
- Service.Contracts.Shell/IServiceShellManager.cs
- Service.Contracts.Modules/IServiceModulesManager.cs
- Service.Shell/ServiceShellManager.cs
- Service.Modules/ServiceModulesManager.cs
- Service.Shell/Extensions/ServiceShellConfiguration.cs
- Service.Modules/Extensions/ServiceModulesConfiguration.cs
- Server.Api.Modules.sln

### PINDAH + UPDATE NAMESPACE
- Presentation/ActionFilters/ValidationFilterAttribute.cs → Presentation.Shell/ActionFilters/
- 19 controller krusial → Presentation.Shell/Controllers/
- 9 controller modules → Presentation.Modules/Controllers/
- 19 service krusial → Service.Shell/
- 4 approval service → Service.Shell/Approval/
- 9 service modules → Service.Modules/
- 19 interface krusial → Service.Contracts.Shell/
- 4 approval interface → Service.Contracts.Shell/Approval/
- 9 interface modules → Service.Contracts.Modules/

### UPDATE (Modify)
- BamboeUp.Api/BamboeUp.Api.csproj
- BamboeUp.Api/Program.cs
- BamboeUp.Api/Extensions/ServiceExtensions.cs
- BamboeUp.Api/MappingProfile.cs
- Server.Api.sln

### HAPUS (Delete)
- Service/ServiceManager.cs
- Service.Contracts/IServiceManager.cs
- Service/Extensions/ServiceConfiguration.cs

---

## 8. Estimasi Waktu

| Phase | Estimasi |
|---|---|
| Phase 1: Setup Project Baru | 30 menit |
| Phase 2: Migrasi Service.Contracts | 45 menit |
| Phase 3: Migrasi Service Implementation | 60 menit |
| Phase 4: Migrasi Presentation | 45 menit |
| Phase 5: Update Entry Point | 20 menit |
| Phase 6: Bersihkan Project Lama | 15 menit |
| Phase 7: Buat Modules Solution | 20 menit |
| Phase 8: Verifikasi & Testing | 30 menit |
| **Total Estimasi** | **~4 jam** |

---

## 9. Risiko & Mitigasi

| Risiko | Dampak | Mitigasi |
|---|---|---|
| Namespace conflict setelah rename | Build error | Gunakan find-replace global per project |
| Circular reference antar project | Build error | Pastikan dependency graph satu arah |
| Controller tidak terdaftar di DI | Runtime error (404) | Test semua endpoint setelah migrasi |
| MappingProfile tidak menemukan type | Runtime error | Pastikan namespace baru diimport di assembly scanning |
| AuthenticationService butuh IConfiguration | Runtime error | Pastikan IConfiguration inject tetap berjalan |

---

## 10. Approval

Jika plan ini sudah disetujui, development akan dimulai dari **Phase 1** secara berurutan.
