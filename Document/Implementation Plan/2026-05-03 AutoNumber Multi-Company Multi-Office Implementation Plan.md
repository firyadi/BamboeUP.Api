# AutoNumber Multi-Company & Multi-Office UI Integration Plan

This plan outlines the UI updates required to align the AutoNumber module (Templates, Components, Counters) with the new Multi-Company and Multi-Office architecture, as well as recent DTO modifications.

## Proposed Changes

### 1. AutoNumber Template Module

#### [MODIFY] AutoNumberTemplateForm.razor
- **Add conditional UI rendering for Scope Selection**: 
  - A dropdown/radio for `TemplateScopeType` (GLOBAL, COMPANY, OFFICE, DEPARTMENT).
  - Conditionally render `CompanyId` selection dropdown if Scope is 'COMPANY' or 'OFFICE'.
  - Conditionally render `CompanyOfficeId` selection dropdown if Scope is 'OFFICE'.
  - Clear these values when scope switches to GLOBAL.
- *Dependencies*: Need to inject `ICompanyService` and `ICompanyOfficeService` (or similar) to populate the dropdown lists.

#### [MODIFY] AutoNumberTemplateList.razor
- Add `TemplateScopeType` to the MudTable columns to show the scope of the templates.

#### [MODIFY] AutoNumberTemplateSearch.razor
- Add inputs for `TemplateScopeType`, `CompanyId`, and `CompanyOfficeId` to support filtering on the new parameters.

#### [MODIFY] AutoNumberTemplateDetail.razor
- Handle potential backend validation exceptions (`409 Conflict`, `400 Bad Request`) gracefully and display them via `Snackbar`.

---

### 2. AutoNumber Component Module

#### [MODIFY] AutoNumberComponentSection.razor
- Remove the `ValueSource` column from the grid (`MudTh` and `MudTd`).
- Remove the `ValueSource` input from the entry form.
- Replace bindings and references from `ComponentFormat` to `Format` (Note: `AutoNumberComponentDto` already uses `Format` so we need to update the UI bindings from `ComponentFormat` to `Format` to resolve any existing or future compilation errors).
- Update the default assignment logic for `ComponentType` changes to remove `ValueSource`.

---

### 3. AutoNumber Counter (Monitor) Module

#### [MODIFY] AutoNumberCounterMonitor.razor
- **Grid Changes**:
  - Remove `DepartmentCode` and `LastCompleteNumber` columns.
  - Add columns for `Company`, `Office`, and `OrganizationUnit`. (May require showing ID if names are not readily available via API, or fetching lookup lists).
- **Filter Bar Changes**:
  - Remove `Department` input.
  - Add filters for `CompanyId`, `CompanyOfficeId`, and `OrganizationUnitId` (probably using standard MudSelects).
- **Action Bar changes**:
  - Reflect removal of `DepartmentCode` in the selected item text.

## User Review Required

> [!IMPORTANT]
> - Do we have existing reusable UI components for Company and Office selection (like a shared `CompanySelector` component) or should I build them inline using `MudSelect` and the respective services?
> - For the `AutoNumberCounterMonitor`, when displaying Company, Office, and OrgUnit, do you prefer to fetch all lookup data once on page load to display names, or is displaying the IDs sufficient for the monitor for now?

## Verification Plan

### Automated Tests
- None specified for UI.

### Manual Verification
1. Open `AutoNumberTemplateList`, verify `TemplateScopeType` column displays.
2. Open `AutoNumberTemplateSearch`, verify the new scope fields filter the list successfully.
3. Open `AutoNumberTemplateDetail` (New), verify scope type dropdown controls visibility of Company/Office. Save and verify data persists.
4. Add components, ensure `Format` is bound correctly and `ValueSource` is gone.
5. Open `AutoNumberCounterMonitor`, verify filters work with Company/Office instead of Department and the table displays correctly without `LastCompleteNumber`.
