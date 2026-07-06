# AutoMapper to Mapster Migration Plan

This document serves as the checklist and plan for migrating from AutoMapper to Mapster.

## Checklist

### Phase 1: Package Management
- [x] Remove `AutoMapper` and `AutoMapper.Extensions.Microsoft.DependencyInjection` from `BamboeUp.Api.csproj`.
- [x] Remove `AutoMapper` from `Service.Shell.csproj`.
- [x] Remove `AutoMapper` from `Service.Modules.csproj`.
- [x] Remove `AutoMapper` from `Presentation.Shell.csproj`.
- [x] Remove `AutoMapper` from `Presentation.Modules.csproj`.
- [x] Remove `AutoMapper` from `Repository.csproj`.
- [x] Install `Mapster` to `BamboeUp.Api`, `Service.Shell`, and `Service.Modules`.

### Phase 2: Configuration
- [x] Create `BamboeUp.Api\MapsterConfig.cs` for custom mappings (like `ApprovalRequest.StatusName`).
- [x] Delete `BamboeUp.Api\MappingProfile.cs`.
- [x] Remove AutoMapper DI registration from `BamboeUp.Api\Program.cs` / `ServiceExtensions.cs` and initialize Mapster.

### Phase 3: Service Layer Refactoring
Refactor services to remove `IMapper` and use `.Adapt<T>()`.
- [x] Refactor `Service.Shell` Services — AutoNumber, AutoNumberTemplate, AutoNumberCounter, AutoNumberComponent, Approval, ApprovalTemplate, ApprovalDelegation, OrganizationUnit, OrganizationUnitScope, Company, CompanyOffice, User, UserGroup, UserGroupScope, UserGroupProgram, UserCompanyScope, Parameter, Program, StandardReference, StandardReferenceItem, StandardReferenceDisplay, VwStandardReferenceItem.
- [x] Refactor `Service.Modules` Services — Bank, Country, City, Province, District, SubDistrict, PostalCode, Holiday, AdministrativeAreaDisplay.
- [x] Fix `ServiceManager.cs` (Shell & Modules) to remove legacy `mapper` constructor argument.

### Phase 4: Verification
- [x] Build solution successfully — **`dotnet build` result: 0 Error(s)** ✅ (2026-05-04).
- [ ] Test API endpoints to verify object mapping works properly (manual runtime test by developer).
