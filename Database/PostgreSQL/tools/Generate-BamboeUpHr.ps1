#Requires -Version 5.1
<#
.SYNOPSIS
  Generate PostgreSQL init-bamboeup-hr.sql (+ optional platform seed) from Dev.BamboeHR metadata.
#>
param(
    [string]$SqlServer = "localhost",
    [string]$SqlDatabase = "Dev.BamboeHR",
    [string]$SqlUser = "sa",
    [string]$SqlPassword = "123",
    [string]$OutDir = (Join-Path $PSScriptRoot ".."),
    [switch]$SkipSeed
)

$ErrorActionPreference = "Stop"
$OutDir = [System.IO.Path]::GetFullPath($OutDir)
$SeedDir = Join-Path $OutDir "seed"
New-Item -ItemType Directory -Force -Path $OutDir, $SeedDir | Out-Null

$cs = "Server=$SqlServer;Database=$SqlDatabase;User Id=$SqlUser;Password=$SqlPassword;Encrypt=False;TrustServerCertificate=True;"
Add-Type -AssemblyName System.Data
$conn = New-Object System.Data.SqlClient.SqlConnection $cs
$conn.Open()

function Invoke-SqlDataTable([string]$sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sql
    $cmd.CommandTimeout = 300
    $da = New-Object System.Data.SqlClient.SqlDataAdapter $cmd
    $dt = New-Object System.Data.DataTable
    [void]$da.Fill($dt)
    return $dt
}

$required = @(
    # core
    "core.Users","core.UserGroup","core.Programs","core.UserGroupProgram","core.UserGroupScope","core.UserCompanyScope",
    "core.Country","core.Province","core.City","core.District","core.SubDistrict","core.PostalCode","core.Bank",
    # org
    "app.Company","app.CompanyOffice","app.OrganizationUnit","app.OrganizationUnitScope",
    # lookups
    "app.StandardReference","app.StandardReferenceItem","app.StandardReferenceScope","app.StandardReferenceScopeItem",
    "app.Parameter","app.ParameterScope",
    # autonumber (live)
    "app.AutoNumberTemplate","app.AutoNumberComponent","app.AutoNumberCounter","app.AutoNumberLog","app.DocumentNumberRequest",
    # masters
    "app.Skill","app.Certification","app.Asset","app.Training","app.Holiday",
    "app.Award","app.AwardCriteria","app.AwardCriteriaRule",
    "app.CostCenter","app.CostCenterScope","app.CostCenterAssignment","app.FileStorage",
    # person / employee
    "app.Person","app.PersonAddress","app.PersonContact","app.PersonEducation","app.PersonIdentification",
    "app.PersonEmergencyContact","app.PersonFamily","app.PersonBankAccount","app.PersonWorkExperience","app.PersonPhysicalCharacteristic",
    "app.Employee","app.EmployeeAssignment","app.EmployeeContract","app.EmployeeCareer",
    "app.EmployeeSkill","app.EmployeeLanguage","app.EmployeeTraining","app.EmployeeCertification","app.EmployeeAsset",
    # UI samples
    "app.MeetingRoom","app.RoomBooking","app.WorkItem"
)

$excludeLive = @(
    "app.EmployeeLeave","app.Meeting","app.PayrollRun","app.PayrollRunDetail","app.PurchaseRequest","app.SalesTransaction",
    "core.ProcessApproval","core.ProcessAudit","core.ProcessDefinition","core.ProcessExecution","core.ProcessMessage",
    "core.ProcessParameterDefinition","core.ProcessParameterValue","core.ProcessResult","core.ProcessResultDetail",
    "core.ProcessStepDefinition","core.ProcessStepExecution",
    "core.ReportDefinition","core.ReportExecutionLog","core.ReportParameter","core.ReportParameterCatalog",
    "core.SharedParameterCatalog","core.SharedParameterProfile","core.SharedParameterProfileField",
    "core.UserOrganizationScope","dbo.sysdiagrams"
)

$schemaSql = @"
SELECT s.name AS SchemaName, t.name AS TableName, c.column_id AS ColumnId,
       c.name AS ColumnName, ty.name AS TypeName,
       c.max_length AS MaxLength, c.precision AS [Precision], c.scale AS Scale,
       CAST(c.is_nullable AS bit) AS IsNullable,
       CAST(c.is_identity AS bit) AS IsIdentity,
       CAST(c.is_computed AS bit) AS IsComputed
FROM sys.tables t
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
INNER JOIN sys.columns c ON c.object_id = t.object_id
INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
WHERE s.name IN ('app','core')
ORDER BY s.name, t.name, c.column_id;
"@

$uqSql = @"
SELECT s.name AS SchemaName, t.name AS TableName, i.name AS IndexName,
       CAST(i.is_primary_key AS bit) AS IsPrimaryKey,
       CAST(i.is_unique_constraint AS bit) AS IsUniqueConstraint,
       CAST(i.is_unique AS bit) AS IsUnique,
       i.filter_definition AS FilterDefinition,
       STRING_AGG(c.name, '|') WITHIN GROUP (ORDER BY ic.key_ordinal) AS Columns
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
INNER JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id AND ic.is_included_column = 0
INNER JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
WHERE s.name IN ('app','core') AND i.is_hypothetical = 0 AND i.is_unique = 1
GROUP BY s.name, t.name, i.name, i.is_primary_key, i.is_unique_constraint, i.is_unique, i.filter_definition
ORDER BY s.name, t.name, i.name;
"@

Write-Host "Reading metadata from $SqlDatabase ..."
$cols = Invoke-SqlDataTable $schemaSql
$uqs = Invoke-SqlDataTable $uqSql

$liveTables = $cols | Select-Object -ExpandProperty SchemaName -Unique | ForEach-Object { $_ } | Out-Null
$liveKeys = @{}
foreach ($r in ($cols | Select-Object SchemaName, TableName -Unique)) {
    $liveKeys["$($r.SchemaName).$($r.TableName)"] = $true
}

$missingRequired = @()
foreach ($k in $required) {
    if (-not $liveKeys.ContainsKey($k)) { $missingRequired += $k }
}
if ($missingRequired.Count -gt 0) {
    Write-Warning ("Required live tables missing: " + ($missingRequired -join ", "))
}

function New-ExtraCol($schema, $table, $id, $name, $type, $maxLen, $prec, $scale, $nullable, $identity) {
    return [pscustomobject]@{
        SchemaName = $schema; TableName = $table; ColumnId = $id; ColumnName = $name
        TypeName = $type; MaxLength = [int16]$maxLen; Precision = [byte]$prec; Scale = [byte]$scale
        IsNullable = [bool]$nullable; IsIdentity = [bool]$identity; IsComputed = $false
    }
}

function Add-AuditCols($list, $schema, $table, [ref]$id, [bool]$withRowVersion = $true, [bool]$withSoftDelete = $true) {
    $list.Add((New-ExtraCol $schema $table (++$id.Value) "StatusId" "int" 4 10 0 $false $false)) | Out-Null
    if ($withRowVersion) {
        $list.Add((New-ExtraCol $schema $table (++$id.Value) "RowVersion" "timestamp" 8 0 0 $false $false)) | Out-Null
    }
    $list.Add((New-ExtraCol $schema $table (++$id.Value) "CreatedById" "bigint" 8 19 0 $false $false)) | Out-Null
    $list.Add((New-ExtraCol $schema $table (++$id.Value) "CreatedTime" "datetime2" 8 27 7 $false $false)) | Out-Null
    $list.Add((New-ExtraCol $schema $table (++$id.Value) "UpdatedById" "bigint" 8 19 0 $true $false)) | Out-Null
    $list.Add((New-ExtraCol $schema $table (++$id.Value) "UpdatedTime" "datetime2" 8 27 7 $true $false)) | Out-Null
    if ($withSoftDelete) {
        $list.Add((New-ExtraCol $schema $table (++$id.Value) "DeletedById" "bigint" 8 19 0 $true $false)) | Out-Null
        $list.Add((New-ExtraCol $schema $table (++$id.Value) "DeletedTime" "datetime2" 8 27 7 $true $false)) | Out-Null
    }
}

$extraCols = New-Object System.Collections.Generic.List[object]
$extraUqs = New-Object System.Collections.Generic.List[object]

# Hospital (missing in live UAT)
$id = 0
$extraCols.Add((New-ExtraCol "app" "Hospital" (++$id) "HospitalId" "bigint" 8 19 0 $false $true)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "Hospital" (++$id) "HospitalGuid" "uniqueidentifier" 16 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "Hospital" (++$id) "HospitalName" "nvarchar" 400 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "Hospital" (++$id) "HospitalCode" "varchar" 20 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "Hospital" (++$id) "ShortName" "nvarchar" 200 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "Hospital" (++$id) "LicenseNo" "varchar" 50 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "Hospital" (++$id) "HospitalType" "varchar" 30 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "Hospital" (++$id) "HospitalClass" "varchar" 10 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "Hospital" (++$id) "PhoneNo" "varchar" 30 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "Hospital" (++$id) "Email" "varchar" 200 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "Hospital" (++$id) "Website" "varchar" 200 0 0 $true $false)) | Out-Null
Add-AuditCols $extraCols "app" "Hospital" ([ref]$id)
$extraUqs.Add([pscustomobject]@{ SchemaName="app"; TableName="Hospital"; IndexName="UQ_Hospital_HospitalGuid"; IsPrimaryKey=$false; IsUniqueConstraint=$true; IsUnique=$true; FilterDefinition=$null; Columns="HospitalGuid" }) | Out-Null
$extraUqs.Add([pscustomobject]@{ SchemaName="app"; TableName="Hospital"; IndexName="UQ_Hospital_Code"; IsPrimaryKey=$false; IsUniqueConstraint=$true; IsUnique=$true; FilterDefinition=$null; Columns="HospitalCode" }) | Out-Null
$extraUqs.Add([pscustomobject]@{ SchemaName="app"; TableName="Hospital"; IndexName="UQ_Hospital_Name"; IsPrimaryKey=$false; IsUniqueConstraint=$true; IsUnique=$true; FilterDefinition=$null; Columns="HospitalName" }) | Out-Null

# AutoNumber (API still uses legacy table; not present in live UAT)
$id = 0
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "AutoNumberId" "bigint" 8 19 0 $false $true)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "AutoNumberGuid" "uniqueidentifier" 16 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "Prefik" "nvarchar" 14 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "SeparatorAfterPrefik" "nvarchar" 2 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "IsUsedDepartment" "bit" 1 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "SeparatorAfterDept" "nvarchar" 2 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "IsUsedYear" "bit" 1 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "YearDigit" "tinyint" 1 3 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "SeparatorAfterYear" "nvarchar" 2 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "IsUsedMonth" "bit" 1 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "IsMonthInRomawi" "bit" 1 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "SeparatorAfterMonth" "nvarchar" 2 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "IsUsedDay" "bit" 1 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "SeparatorAfterDay" "nvarchar" 2 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "NumberLength" "tinyint" 1 3 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "NumberGroupLength" "tinyint" 1 3 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "NumberGroupSeparator" "nvarchar" 2 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "app" "AutoNumber" (++$id) "NumberFormat" "nvarchar" 40 0 0 $false $false)) | Out-Null
Add-AuditCols $extraCols "app" "AutoNumber" ([ref]$id)
$extraUqs.Add([pscustomobject]@{ SchemaName="app"; TableName="AutoNumber"; IndexName="UQ_AutoNumber_AutoNumberGuid"; IsPrimaryKey=$false; IsUniqueConstraint=$true; IsUnique=$true; FilterDefinition=$null; Columns="AutoNumberGuid" }) | Out-Null

# ApprovalTemplate
$id = 0
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplate" (++$id) "ApprovalTemplateId" "bigint" 8 19 0 $false $true)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplate" (++$id) "ApprovalTemplateGuid" "uniqueidentifier" 16 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplate" (++$id) "TemplateName" "nvarchar" 400 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplate" (++$id) "ModuleCode" "nvarchar" 200 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplate" (++$id) "Description" "nvarchar" 1000 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplate" (++$id) "IsActive" "bit" 1 0 0 $false $false)) | Out-Null
Add-AuditCols $extraCols "apv" "ApprovalTemplate" ([ref]$id)
$extraUqs.Add([pscustomobject]@{ SchemaName="apv"; TableName="ApprovalTemplate"; IndexName="UQ_ApprovalTemplate_Guid"; IsPrimaryKey=$false; IsUniqueConstraint=$true; IsUnique=$true; FilterDefinition=$null; Columns="ApprovalTemplateGuid" }) | Out-Null

$id = 0
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplateLevel" (++$id) "ApprovalTemplateLevelId" "bigint" 8 19 0 $false $true)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplateLevel" (++$id) "ApprovalTemplateLevelGuid" "uniqueidentifier" 16 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplateLevel" (++$id) "ApprovalTemplateId" "bigint" 8 19 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplateLevel" (++$id) "LevelOrder" "int" 4 10 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplateLevel" (++$id) "LevelName" "nvarchar" 400 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplateLevel" (++$id) "ApproverType" "nvarchar" 100 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplateLevel" (++$id) "RequireAllApprovers" "bit" 1 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplateLevel" (++$id) "CanSkipIfPreviousNotApproved" "bit" 1 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplateLevel" (++$id) "SlaHours" "int" 4 10 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplateLevel" (++$id) "EscalateToLevelOrder" "int" 4 10 0 $true $false)) | Out-Null
Add-AuditCols $extraCols "apv" "ApprovalTemplateLevel" ([ref]$id)
$extraUqs.Add([pscustomobject]@{ SchemaName="apv"; TableName="ApprovalTemplateLevel"; IndexName="UQ_ApprovalTemplateLevel_Guid"; IsPrimaryKey=$false; IsUniqueConstraint=$true; IsUnique=$true; FilterDefinition=$null; Columns="ApprovalTemplateLevelGuid" }) | Out-Null

$id = 0
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplateLevelApprover" (++$id) "ApprovalTemplateLevelApproverId" "bigint" 8 19 0 $false $true)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplateLevelApprover" (++$id) "ApprovalTemplateLevelApproverGuid" "uniqueidentifier" 16 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplateLevelApprover" (++$id) "ApprovalTemplateLevelId" "bigint" 8 19 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplateLevelApprover" (++$id) "UserId" "bigint" 8 19 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalTemplateLevelApprover" (++$id) "UserGroupId" "bigint" 8 19 0 $true $false)) | Out-Null
Add-AuditCols $extraCols "apv" "ApprovalTemplateLevelApprover" ([ref]$id) $false $true
$extraUqs.Add([pscustomobject]@{ SchemaName="apv"; TableName="ApprovalTemplateLevelApprover"; IndexName="UQ_ATLevelApprover_Guid"; IsPrimaryKey=$false; IsUniqueConstraint=$true; IsUnique=$true; FilterDefinition=$null; Columns="ApprovalTemplateLevelApproverGuid" }) | Out-Null

$id = 0
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "ApprovalRequestId" "bigint" 8 19 0 $false $true)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "ApprovalRequestGuid" "uniqueidentifier" 16 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "ApprovalTemplateId" "bigint" 8 19 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "RequestNumber" "nvarchar" 100 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "ModuleCode" "nvarchar" 200 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "ReferenceGuid" "uniqueidentifier" 16 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "ReferenceNumber" "nvarchar" 200 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "RequestedByUserId" "bigint" 8 19 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "CurrentLevelOrder" "int" 4 10 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "StatusId" "int" 4 10 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "Notes" "nvarchar" 1000 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "RequestedTime" "datetime2" 8 27 7 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "CompletedTime" "datetime2" 8 27 7 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "RowVersion" "timestamp" 8 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "CreatedById" "bigint" 8 19 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "CreatedTime" "datetime2" 8 27 7 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "UpdatedById" "bigint" 8 19 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "UpdatedTime" "datetime2" 8 27 7 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "DeletedById" "bigint" 8 19 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalRequest" (++$id) "DeletedTime" "datetime2" 8 27 7 $true $false)) | Out-Null
$extraUqs.Add([pscustomobject]@{ SchemaName="apv"; TableName="ApprovalRequest"; IndexName="UQ_ApprovalRequest_Guid"; IsPrimaryKey=$false; IsUniqueConstraint=$true; IsUnique=$true; FilterDefinition=$null; Columns="ApprovalRequestGuid" }) | Out-Null

$id = 0
$extraCols.Add((New-ExtraCol "apv" "ApprovalStep" (++$id) "ApprovalStepId" "bigint" 8 19 0 $false $true)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalStep" (++$id) "ApprovalStepGuid" "uniqueidentifier" 16 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalStep" (++$id) "ApprovalRequestId" "bigint" 8 19 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalStep" (++$id) "LevelOrder" "int" 4 10 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalStep" (++$id) "LevelName" "nvarchar" 400 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalStep" (++$id) "ApproverUserId" "bigint" 8 19 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalStep" (++$id) "DelegatedFromUserId" "bigint" 8 19 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalStep" (++$id) "StatusId" "int" 4 10 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalStep" (++$id) "ActionTime" "datetime2" 8 27 7 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalStep" (++$id) "Comment" "nvarchar" 2000 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalStep" (++$id) "SlaDeadline" "datetime2" 8 27 7 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalStep" (++$id) "IsEscalated" "bit" 1 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalStep" (++$id) "CreatedById" "bigint" 8 19 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalStep" (++$id) "CreatedTime" "datetime2" 8 27 7 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalStep" (++$id) "UpdatedById" "bigint" 8 19 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalStep" (++$id) "UpdatedTime" "datetime2" 8 27 7 $true $false)) | Out-Null
$extraUqs.Add([pscustomobject]@{ SchemaName="apv"; TableName="ApprovalStep"; IndexName="UQ_ApprovalStep_Guid"; IsPrimaryKey=$false; IsUniqueConstraint=$true; IsUnique=$true; FilterDefinition=$null; Columns="ApprovalStepGuid" }) | Out-Null

$id = 0
$extraCols.Add((New-ExtraCol "apv" "ApprovalHistory" (++$id) "ApprovalHistoryId" "bigint" 8 19 0 $false $true)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalHistory" (++$id) "ApprovalHistoryGuid" "uniqueidentifier" 16 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalHistory" (++$id) "ApprovalRequestId" "bigint" 8 19 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalHistory" (++$id) "ApprovalStepId" "bigint" 8 19 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalHistory" (++$id) "ActionType" "nvarchar" 100 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalHistory" (++$id) "ActionByUserId" "bigint" 8 19 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalHistory" (++$id) "ActionTime" "datetime2" 8 27 7 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalHistory" (++$id) "Comment" "nvarchar" 2000 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalHistory" (++$id) "FromStatus" "nvarchar" 100 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalHistory" (++$id) "ToStatus" "nvarchar" 100 0 0 $true $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalHistory" (++$id) "LevelOrder" "int" 4 10 0 $true $false)) | Out-Null
$extraUqs.Add([pscustomobject]@{ SchemaName="apv"; TableName="ApprovalHistory"; IndexName="UQ_ApprovalHistory_Guid"; IsPrimaryKey=$false; IsUniqueConstraint=$true; IsUnique=$true; FilterDefinition=$null; Columns="ApprovalHistoryGuid" }) | Out-Null

$id = 0
$extraCols.Add((New-ExtraCol "apv" "ApprovalDelegation" (++$id) "ApprovalDelegationId" "bigint" 8 19 0 $false $true)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalDelegation" (++$id) "ApprovalDelegationGuid" "uniqueidentifier" 16 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalDelegation" (++$id) "DelegatorUserId" "bigint" 8 19 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalDelegation" (++$id) "DelegateUserId" "bigint" 8 19 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalDelegation" (++$id) "StartDate" "date" 3 10 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalDelegation" (++$id) "EndDate" "date" 3 10 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalDelegation" (++$id) "IsActive" "bit" 1 0 0 $false $false)) | Out-Null
$extraCols.Add((New-ExtraCol "apv" "ApprovalDelegation" (++$id) "Notes" "nvarchar" 1000 0 0 $true $false)) | Out-Null
Add-AuditCols $extraCols "apv" "ApprovalDelegation" ([ref]$id)
$extraUqs.Add([pscustomobject]@{ SchemaName="apv"; TableName="ApprovalDelegation"; IndexName="UQ_ApprovalDelegation_Guid"; IsPrimaryKey=$false; IsUniqueConstraint=$true; IsUnique=$true; FilterDefinition=$null; Columns="ApprovalDelegationGuid" }) | Out-Null

# Merge columns
$allCols = New-Object System.Collections.Generic.List[object]
foreach ($r in $cols) {
    $key = "$($r.SchemaName).$($r.TableName)"
    if ($excludeLive -contains $key) { continue }
    if ($required -notcontains $key) { continue }
    if ($r.IsComputed) { continue }
    $allCols.Add($r) | Out-Null
}
foreach ($r in $extraCols) { $allCols.Add($r) | Out-Null }

$allUqs = New-Object System.Collections.Generic.List[object]
foreach ($r in $uqs) {
    $key = "$($r.SchemaName).$($r.TableName)"
    if ($excludeLive -contains $key) { continue }
    if ($required -notcontains $key) { continue }
    $allUqs.Add($r) | Out-Null
}
foreach ($r in $extraUqs) { $allUqs.Add($r) | Out-Null }

function Get-CharLen($typeName, $maxLength) {
    if ($maxLength -lt 0) { return $null }
    if ($typeName -match '^(ntext|nvarchar|nchar)$') { return [int]($maxLength / 2) }
    return [int]$maxLength
}

function Map-PgType($col) {
    $t = $col.TypeName.ToLowerInvariant()
    $name = $col.ColumnName
    $nullSql = if ($col.IsNullable) { "" } else { " NOT NULL" }

    if ($col.IsIdentity) {
        return "`"$name`" BIGINT GENERATED BY DEFAULT AS IDENTITY NOT NULL"
    }
    if ($t -eq "uniqueidentifier") {
        if ($col.IsNullable) { return "`"$name`" UUID" }
        return "`"$name`" UUID NOT NULL DEFAULT gen_random_uuid()"
    }
    if ($t -eq "timestamp" -or $t -eq "rowversion" -or $name -eq "RowVersion") {
        return "`"RowVersion`" BYTEA NOT NULL DEFAULT decode('0000000000000000','hex')"
    }
    if ($name -eq "StatusId") {
        return "`"StatusId`" INT NOT NULL DEFAULT 1"
    }
    if ($name -eq "CreatedById" -and -not $col.IsNullable) {
        return "`"CreatedById`" BIGINT NOT NULL DEFAULT 0"
    }
    if ($name -eq "CreatedTime" -and -not $col.IsNullable) {
        return "`"CreatedTime`" TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')"
    }
    if ($t -eq "bit") {
        $def = if ($name -match 'IsNational') { "TRUE" } else { "FALSE" }
        if ($col.IsNullable) { return "`"$name`" BOOLEAN DEFAULT $def" }
        return "`"$name`" BOOLEAN NOT NULL DEFAULT $def"
    }
    if ($t -eq "date") { return "`"$name`" DATE$nullSql".TrimEnd() }
    if ($t -match '^(datetime2|datetime|smalldatetime)$') { return "`"$name`" TIMESTAMPTZ$nullSql".TrimEnd() }
    if ($t -eq "time") { return "`"$name`" TIME$nullSql".TrimEnd() }
    if ($t -eq "bigint") { return "`"$name`" BIGINT$nullSql".TrimEnd() }
    if ($t -eq "int") { return "`"$name`" INT$nullSql".TrimEnd() }
    if ($t -eq "smallint" -or $t -eq "tinyint") { return "`"$name`" SMALLINT$nullSql".TrimEnd() }
    if ($t -eq "float") { return "`"$name`" DOUBLE PRECISION$nullSql".TrimEnd() }
    if ($t -eq "real") { return "`"$name`" REAL$nullSql".TrimEnd() }
    if ($t -eq "decimal" -or $t -eq "numeric" -or $t -eq "money" -or $t -eq "smallmoney") {
        $p = [int]$col.Precision; $s = [int]$col.Scale
        if ($t -eq "money") { $p = 19; $s = 4 }
        return "`"$name`" NUMERIC($p,$s)$nullSql".TrimEnd()
    }
    if ($t -match '^(varbinary|binary|image)$') { return "`"$name`" BYTEA$nullSql".TrimEnd() }
    if ($t -eq "xml" -or $t -eq "text" -or $t -eq "ntext" -or (($t -match '^(nvarchar|varchar)$') -and [int]$col.MaxLength -lt 0)) {
        return "`"$name`" TEXT$nullSql".TrimEnd()
    }
    if ($t -match '^(nvarchar|varchar|nchar|char|sysname)$') {
        $len = Get-CharLen $t ([int]$col.MaxLength)
        if (-not $len -or $len -le 0) { return "`"$name`" TEXT$nullSql".TrimEnd() }
        $pg = if ($t -match '^(nchar|char)$') { "CHAR($len)" } else { "VARCHAR($len)" }
        return "`"$name`" $pg$nullSql".TrimEnd()
    }
    return "`"$name`" TEXT$nullSql".TrimEnd()
}

function Get-PkName($table, $colsForTable) {
    $idCol = $colsForTable | Where-Object { $_.IsIdentity } | Select-Object -First 1
    if ($idCol) { return $idCol.ColumnName }
    return "${table}Id"
}

$tablesOrdered = @(
    "core.Users","core.UserGroup","core.Programs",
    "app.Company","core.Country","core.Province","core.City","core.District","core.SubDistrict","core.PostalCode","core.Bank",
    "app.CompanyOffice","core.UserGroupProgram","core.UserCompanyScope","core.UserGroupScope",
    "app.StandardReference","app.StandardReferenceItem","app.StandardReferenceScope","app.StandardReferenceScopeItem",
    "app.Parameter","app.ParameterScope",
    "app.OrganizationUnit","app.OrganizationUnitScope",
    "app.AutoNumber","app.AutoNumberTemplate","app.AutoNumberComponent","app.AutoNumberCounter","app.AutoNumberLog","app.DocumentNumberRequest",
    "app.Hospital","app.Skill","app.Certification","app.Asset","app.Training","app.Holiday",
    "app.Award","app.AwardCriteria","app.AwardCriteriaRule",
    "app.CostCenter","app.CostCenterScope","app.CostCenterAssignment","app.FileStorage",
    "app.Person","app.PersonAddress","app.PersonContact","app.PersonEducation","app.PersonIdentification",
    "app.PersonEmergencyContact","app.PersonFamily","app.PersonBankAccount","app.PersonWorkExperience","app.PersonPhysicalCharacteristic",
    "app.Employee","app.EmployeeAssignment","app.EmployeeContract","app.EmployeeCareer",
    "app.EmployeeSkill","app.EmployeeLanguage","app.EmployeeTraining","app.EmployeeCertification","app.EmployeeAsset",
    "app.MeetingRoom","app.RoomBooking","app.WorkItem",
    "apv.ApprovalTemplate","apv.ApprovalTemplateLevel","apv.ApprovalTemplateLevelApprover",
    "apv.ApprovalRequest","apv.ApprovalStep","apv.ApprovalHistory","apv.ApprovalDelegation"
)

$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("-- BamboeUP.HR - PostgreSQL production schema (minimal runtime set)")
[void]$sb.AppendLine("-- Generated from Dev.BamboeHR metadata + Table/*.sql extras (Hospital, AutoNumber, Approval)")
[void]$sb.AppendLine("-- Target database: ""BamboeUP.HR"" (quoted)")
[void]$sb.AppendLine("-- RowVersion is BYTEA for mapping parity; it does not auto-increment.")
[void]$sb.AppendLine("-- Physical FKs omitted (indexes on *Id / Sr* instead).")
[void]$sb.AppendLine("-- Script Date: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
[void]$sb.AppendLine()
[void]$sb.AppendLine("CREATE EXTENSION IF NOT EXISTS pgcrypto;")
[void]$sb.AppendLine("CREATE SCHEMA IF NOT EXISTS app;")
[void]$sb.AppendLine("CREATE SCHEMA IF NOT EXISTS core;")
[void]$sb.AppendLine("CREATE SCHEMA IF NOT EXISTS apv;")
[void]$sb.AppendLine("CREATE SCHEMA IF NOT EXISTS log;")
[void]$sb.AppendLine()

$colsByTable = $allCols | Group-Object { "$($_.SchemaName).$($_.TableName)" } -AsHashTable -AsString
$uqByTable = $allUqs | Group-Object { "$($_.SchemaName).$($_.TableName)" } -AsHashTable -AsString

foreach ($key in $tablesOrdered) {
    if (-not $colsByTable.ContainsKey($key)) {
        Write-Warning "No columns for $key - skipped"
        continue
    }
    $parts = $key.Split(".")
    $schema = $parts[0]; $table = $parts[1]
    $tcols = @($colsByTable[$key] | Sort-Object ColumnId)
    $pk = Get-PkName $table $tcols

    [void]$sb.AppendLine("-- ---------------------------------------------------------------------------")
    [void]$sb.AppendLine("-- $schema.$table")
    [void]$sb.AppendLine("-- ---------------------------------------------------------------------------")
    [void]$sb.AppendLine("DROP TABLE IF EXISTS `"$schema`".`"$table`" CASCADE;")
    [void]$sb.AppendLine("CREATE TABLE `"$schema`".`"$table`"")
    [void]$sb.AppendLine("(")

    $lines = New-Object System.Collections.Generic.List[string]
    foreach ($c in $tcols) { $lines.Add("    $(Map-PgType $c)") | Out-Null }
    $lines.Add("    CONSTRAINT `"PK_$table`" PRIMARY KEY (`"$pk`")") | Out-Null

    $uqRows = @()
    if ($uqByTable.ContainsKey($key)) { $uqRows = @($uqByTable[$key]) }
    foreach ($uq in $uqRows) {
        if ($uq.IsPrimaryKey) { continue }
        $uqCols = ($uq.Columns -split '\|') | ForEach-Object { "`"$_`"" }
        $uqList = $uqCols -join ", "
        $uqName = $uq.IndexName
        if ($uq.FilterDefinition) {
            # emit later as unique index with WHERE
            continue
        }
        $lines.Add("    CONSTRAINT `"$uqName`" UNIQUE ($uqList)") | Out-Null
    }

    for ($i = 0; $i -lt $lines.Count; $i++) {
        $comma = if ($i -lt $lines.Count - 1) { "," } else { "" }
        [void]$sb.AppendLine($lines[$i] + $comma)
    }
    [void]$sb.AppendLine(");")
    [void]$sb.AppendLine()

    # Filtered unique indexes
    foreach ($uq in $uqRows) {
        if ($uq.IsPrimaryKey) { continue }
        if (-not $uq.FilterDefinition) { continue }
        $uqCols = (($uq.Columns -split '\|') | ForEach-Object { "`"$_`"" }) -join ", "
        $filter = 'WHERE ("DeletedTime" IS NULL AND "StatusId" > 0)'
        [void]$sb.AppendLine("CREATE UNIQUE INDEX IF NOT EXISTS `"$($uq.IndexName)`" ON `"$schema`".`"$table`" ($uqCols) $filter;")
    }

    # Performance indexes
    $colNames = @($tcols | ForEach-Object { $_.ColumnName })
    $hasDeleted = $colNames -contains "DeletedTime"
    $hasStatus = $colNames -contains "StatusId"
    foreach ($c in $tcols) {
        $n = $c.ColumnName
        if ($c.IsIdentity -or $n -eq $pk) { continue }
        if ($n -in @("StatusId","CreatedById","UpdatedById","DeletedById","CreatedTime","UpdatedTime","DeletedTime","RowVersion")) { continue }
        if ($n -match 'Guid$') { continue }
        if ($n -match 'Id$' -or $n -match '^Sr') {
            [void]$sb.AppendLine("CREATE INDEX IF NOT EXISTS `"IX_${table}_$n`" ON `"$schema`".`"$table`" (`"$n`");")
        }
        if ($n -match 'Date$' -and $hasDeleted -and $hasStatus) {
            [void]$sb.AppendLine("CREATE INDEX IF NOT EXISTS `"IX_${table}_${n}_Active`" ON `"$schema`".`"$table`" (`"$n`") WHERE (`"DeletedTime`" IS NULL AND `"StatusId`" > 0);")
        }
    }
    [void]$sb.AppendLine()
}

# JOIN stubs / views
[void]$sb.AppendLine("-- ---------------------------------------------------------------------------")
[void]$sb.AppendLine("-- JOIN display stubs (generator uses FK-column table names)")
[void]$sb.AppendLine("-- ---------------------------------------------------------------------------")
$stubTables = @(
    "JobGrade","JobLevel","JobTitle","Position","Shift","PayrollGroup","WorkLocation"
)
foreach ($t in $stubTables) {
    [void]$sb.AppendLine(@"
CREATE TABLE IF NOT EXISTS app."$t" (
    "$($t)Id" BIGINT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    "$($t)Name" VARCHAR(200),
    "StatusId" INT NOT NULL DEFAULT 1
);
"@)
}

[void]$sb.AppendLine(@"
-- Display name from First/Middle/Last only — PersonName is computed in Dev and omitted from platform DDL.
CREATE OR REPLACE VIEW app."ManagerEmployee" AS
SELECT e."EmployeeId" AS "ManagerEmployeeId",
       NULLIF(TRIM(CONCAT_WS(' ', p."FirstName", p."MiddleName", p."LastName")), '') AS "ManagerEmployeeName",
       e."StatusId"
FROM app."Employee" e
LEFT JOIN app."Person" p ON p."PersonId" = e."PersonId";

CREATE OR REPLACE VIEW app."SupervisorEmployee" AS
SELECT e."EmployeeId" AS "SupervisorEmployeeId",
       NULLIF(TRIM(CONCAT_WS(' ', p."FirstName", p."MiddleName", p."LastName")), '') AS "SupervisorEmployeeName",
       e."StatusId"
FROM app."Employee" e
LEFT JOIN app."Person" p ON p."PersonId" = e."PersonId";

CREATE OR REPLACE VIEW app."FromCompany" AS
SELECT "CompanyId" AS "FromCompanyId", "CompanyName" AS "FromCompanyName", "StatusId" FROM app."Company";
CREATE OR REPLACE VIEW app."ToCompany" AS
SELECT "CompanyId" AS "ToCompanyId", "CompanyName" AS "ToCompanyName", "StatusId" FROM app."Company";

CREATE OR REPLACE VIEW app."FromOrganizationUnit" AS
SELECT "OrganizationUnitId" AS "FromOrganizationUnitId", "OrganizationUnitName" AS "FromOrganizationUnitName", "StatusId" FROM app."OrganizationUnit";
CREATE OR REPLACE VIEW app."ToOrganizationUnit" AS
SELECT "OrganizationUnitId" AS "ToOrganizationUnitId", "OrganizationUnitName" AS "ToOrganizationUnitName", "StatusId" FROM app."OrganizationUnit";

CREATE OR REPLACE VIEW app."FromManagerEmployee" AS
SELECT "ManagerEmployeeId" AS "FromManagerEmployeeId", "ManagerEmployeeName" AS "FromManagerEmployeeName", "StatusId" FROM app."ManagerEmployee";
CREATE OR REPLACE VIEW app."ToManagerEmployee" AS
SELECT "ManagerEmployeeId" AS "ToManagerEmployeeId", "ManagerEmployeeName" AS "ToManagerEmployeeName", "StatusId" FROM app."ManagerEmployee";

CREATE OR REPLACE VIEW app."FromJobGrade" AS SELECT "JobGradeId" AS "FromJobGradeId", "JobGradeName" AS "FromJobGradeName", "StatusId" FROM app."JobGrade";
CREATE OR REPLACE VIEW app."ToJobGrade" AS SELECT "JobGradeId" AS "ToJobGradeId", "JobGradeName" AS "ToJobGradeName", "StatusId" FROM app."JobGrade";
CREATE OR REPLACE VIEW app."FromJobLevel" AS SELECT "JobLevelId" AS "FromJobLevelId", "JobLevelName" AS "FromJobLevelName", "StatusId" FROM app."JobLevel";
CREATE OR REPLACE VIEW app."ToJobLevel" AS SELECT "JobLevelId" AS "ToJobLevelId", "JobLevelName" AS "ToJobLevelName", "StatusId" FROM app."JobLevel";
CREATE OR REPLACE VIEW app."FromJobTitle" AS SELECT "JobTitleId" AS "FromJobTitleId", "JobTitleName" AS "FromJobTitleName", "StatusId" FROM app."JobTitle";
CREATE OR REPLACE VIEW app."ToJobTitle" AS SELECT "JobTitleId" AS "ToJobTitleId", "JobTitleName" AS "ToJobTitleName", "StatusId" FROM app."JobTitle";
CREATE OR REPLACE VIEW app."FromPosition" AS SELECT "PositionId" AS "FromPositionId", "PositionName" AS "FromPositionName", "StatusId" FROM app."Position";
CREATE OR REPLACE VIEW app."ToPosition" AS SELECT "PositionId" AS "ToPositionId", "PositionName" AS "ToPositionName", "StatusId" FROM app."Position";

"@)

[void]$sb.AppendLine("-- ---------------------------------------------------------------------------")
[void]$sb.AppendLine("-- Lookup / geo views")
[void]$sb.AppendLine("-- ---------------------------------------------------------------------------")
[void]$sb.AppendLine(@"
CREATE OR REPLACE VIEW app."vw_StandardReference_Display" AS
SELECT
    i."StandardReferenceItemId",
    i."StandardReferenceItemName",
    0::BIGINT AS "CompanyId",
    0::BIGINT AS "CompanyOfficeId",
    NULL::BIGINT AS "StandardReferenceGroupId",
    r."StandardReferenceInitial"
FROM app."StandardReferenceItem" i
INNER JOIN app."StandardReference" r ON r."StandardReferenceId" = i."StandardReferenceId"
WHERE i."StatusId" > 0 AND i."DeletedTime" IS NULL;

CREATE OR REPLACE VIEW app."vw_StandardReferenceItem" AS
SELECT
    i."StandardReferenceItemId",
    i."StandardReferenceItemGuid",
    0::BIGINT AS "CompanyId",
    NULL::UUID AS "CompanyGuid",
    0::BIGINT AS "CompanyOfficeId",
    NULL::UUID AS "CompanyOfficeGuid",
    i."StandardReferenceId",
    r."StandardReferenceGuid",
    r."StandardReferenceInitial",
    i."StandardReferenceItemInitial",
    i."StandardReferenceItemName",
    NULL::TEXT AS "Note"
FROM app."StandardReferenceItem" i
INNER JOIN app."StandardReference" r ON r."StandardReferenceId" = i."StandardReferenceId"
WHERE i."StatusId" > 0 AND i."DeletedTime" IS NULL;

CREATE OR REPLACE VIEW core."vw_AdministrativeArea" AS
SELECT
   A."CountryId", A."CountryGuid", A."CountryIso", A."CountryIso3", A."CountryName", A."PhoneCode", A."CurrencyCode",
   B."ProvinceId", B."ProvinceGuid", B."ProvinceName",
   C."CityId", C."CityGuid", C."CityName",
   D."DistrictId", D."DistrictGuid", D."DistrictName",
   E."SubDistrictId", E."SubDistrictGuid", E."SubDistrictName",
   F."PostalCodeId", F."PostalCodeGuid", F."PostalCode"
FROM core."Country" A
INNER JOIN core."Province" B ON A."CountryId" = B."CountryId" AND A."StatusId" > 0 AND B."StatusId" > 0
INNER JOIN core."City" C ON B."ProvinceId" = C."ProvinceId" AND C."StatusId" > 0
INNER JOIN core."District" D ON C."CityId" = D."CityId" AND D."StatusId" > 0
INNER JOIN core."SubDistrict" E ON D."DistrictId" = E."DistrictId" AND E."StatusId" > 0
INNER JOIN core."PostalCode" F ON E."SubDistrictId" = F."SubDistrictId";

"@)

[void]$sb.AppendLine("-- ---------------------------------------------------------------------------")
[void]$sb.AppendLine("-- Audit (log) - matches BamboeUp.Audit RebuildAuditSchemaAsync PostgreSQL")
[void]$sb.AppendLine("-- ---------------------------------------------------------------------------")
[void]$sb.AppendLine(@"
CREATE TABLE IF NOT EXISTS log."AuditSnapshot" (
    "SnapshotId" BIGINT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    "TableName" VARCHAR(128) NOT NULL,
    "EntityKey" VARCHAR(128) NOT NULL,
    "SnapshotJson" TEXT NOT NULL,
    "SnapshotDateTime" TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
);
CREATE INDEX IF NOT EXISTS "IX_AuditSnapshot_TableName_EntityKey"
    ON log."AuditSnapshot" ("TableName", "EntityKey");

CREATE TABLE IF NOT EXISTS log."AuditSession" (
    "AuditSessionId" BIGINT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    "SessionGuid" UUID NOT NULL DEFAULT gen_random_uuid(),
    "SessionType" VARCHAR(10) NOT NULL,
    "RootTableName" VARCHAR(128) NOT NULL,
    "RootEntityKey" VARCHAR(128) NOT NULL,
    "RootDisplayName" VARCHAR(256) NULL,
    "ActionByUserId" VARCHAR(128) NOT NULL,
    "LogDateTime" TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
);
CREATE INDEX IF NOT EXISTS "IX_AuditSession_LogDateTime" ON log."AuditSession" ("LogDateTime" DESC);
CREATE INDEX IF NOT EXISTS "IX_AuditSession_ActionByUserId" ON log."AuditSession" ("ActionByUserId");

CREATE TABLE IF NOT EXISTS log."AuditLog" (
    "AuditLogId" BIGINT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    "AuditSessionId" BIGINT NOT NULL REFERENCES log."AuditSession"("AuditSessionId"),
    "TableName" VARCHAR(128) NOT NULL,
    "EntityKey" VARCHAR(128) NOT NULL,
    "EntityDisplayName" VARCHAR(256) NULL,
    "ParentTableName" VARCHAR(128) NULL,
    "ParentEntityKey" VARCHAR(128) NULL,
    "AuditActionType" VARCHAR(10) NOT NULL,
    "ActionByUserID" VARCHAR(128) NOT NULL,
    "LogDateTime" TIMESTAMPTZ NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    "SnapshotId" BIGINT NULL REFERENCES log."AuditSnapshot"("SnapshotId")
);
CREATE INDEX IF NOT EXISTS "IX_AuditLog_AuditSessionId" ON log."AuditLog" ("AuditSessionId");
CREATE INDEX IF NOT EXISTS "IX_AuditLog_TableName_EntityKey" ON log."AuditLog" ("TableName", "EntityKey");
CREATE INDEX IF NOT EXISTS "IX_AuditLog_ParentTableName_ParentEntityKey" ON log."AuditLog" ("ParentTableName", "ParentEntityKey");

CREATE TABLE IF NOT EXISTS log."AuditLogData" (
    "AuditLogId" BIGINT NOT NULL REFERENCES log."AuditLog"("AuditLogId") ON DELETE CASCADE,
    "ColumnName" VARCHAR(128) NOT NULL,
    "OldValue" TEXT NULL,
    "NewValue" TEXT NULL,
    PRIMARY KEY ("AuditLogId", "ColumnName")
);

"@)

[void]$sb.AppendLine("-- Reset identity sequences after bulk seed (safe no-op on empty tables)")
[void]$sb.AppendLine(@"
DO $$
DECLARE r record;
        mx bigint;
BEGIN
  FOR r IN
    SELECT n.nspname AS sch, c.relname AS tbl, a.attname AS col,
           pg_get_serial_sequence(format('%I.%I', n.nspname, c.relname), a.attname) AS seq
    FROM pg_class c
    JOIN pg_namespace n ON n.oid = c.relnamespace
    JOIN pg_attribute a ON a.attrelid = c.oid AND a.attnum > 0 AND NOT a.attisdropped
    WHERE n.nspname IN ('app','core','apv','log')
      AND c.relkind = 'r'
      AND a.attidentity <> ''
  LOOP
    IF r.seq IS NULL THEN CONTINUE; END IF;
    EXECUTE format('SELECT MAX(%I) FROM %I.%I', r.col, r.sch, r.tbl) INTO mx;
    IF mx IS NULL THEN
      EXECUTE format('SELECT setval(%L, 1, false)', r.seq);
    ELSE
      EXECUTE format('SELECT setval(%L, %s, true)', r.seq, mx);
    END IF;
  END LOOP;
END $$;
"@)

$ddlPath = Join-Path $OutDir "init-bamboeup-hr.sql"
[System.IO.File]::WriteAllText($ddlPath, $sb.ToString(), [System.Text.UTF8Encoding]::new($false))
Write-Host "Wrote $ddlPath"

# Inventory JSON for documentation
$inventory = [ordered]@{
    generatedAt = (Get-Date).ToString("o")
    source = $SqlDatabase
    requiredLive = $required
    extrasAdded = @("app.Hospital","app.AutoNumber","apv.ApprovalTemplate","apv.ApprovalTemplateLevel","apv.ApprovalTemplateLevelApprover","apv.ApprovalRequest","apv.ApprovalStep","apv.ApprovalHistory","apv.ApprovalDelegation")
    excludedLive = $excludeLive
    tablesGenerated = @($tablesOrdered)
}
$invPath = Join-Path $OutDir "tools\inventory-bamboeup-hr.json"
[System.IO.File]::WriteAllText($invPath, ($inventory | ConvertTo-Json -Depth 5), [System.Text.UTF8Encoding]::new($false))

if ($SkipSeed) {
    $conn.Close()
    Write-Host "SkipSeed set - done."
    exit 0
}

# ---------- Seed export (platform + geo) ----------
$seedTables = @(
    "core.Users","core.UserGroup","core.Programs","core.UserGroupProgram","core.UserGroupScope","core.UserCompanyScope",
    "app.Company","app.CompanyOffice","app.OrganizationUnit",
    "app.Parameter","app.ParameterScope",
    "app.StandardReference","app.StandardReferenceItem","app.StandardReferenceScope","app.StandardReferenceScopeItem",
    "core.Country","core.Province","core.City","core.District","core.SubDistrict","core.PostalCode","core.Bank"
)

function Escape-CopyText([object]$v) {
    if ($null -eq $v -or [DBNull]::Value.Equals($v)) { return "\N" }
    if ($v -is [bool]) { return $(if ($v) { "t" } else { "f" }) }
    if ($v -is [byte[]]) {
        $hex = ($v | ForEach-Object { $_.ToString("x2") }) -join ""
        return "\\x$hex"
    }
    if ($v -is [datetime]) { return ($v.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff") + "+00") }
    if ($v -is [guid]) { return $v.ToString() }
    $s = [string]$v
    $s = $s.Replace("\", "\\").Replace("`t", "\t").Replace("`n", "\n").Replace("`r", "\r")
    return $s
}

$copySql = New-Object System.Text.StringBuilder
[void]$copySql.AppendLine("-- Platform + geo seed for BamboeUP.HR")
[void]$copySql.AppendLine("-- Apply AFTER init-bamboeup-hr.sql, from Database/PostgreSQL directory:")
[void]$copySql.AppendLine("--   psql ... -f seed/seed-bamboeup-hr.sql")
[void]$copySql.AppendLine("SET client_min_messages TO WARNING;")
[void]$copySql.AppendLine()

foreach ($key in $seedTables) {
    if (-not $colsByTable.ContainsKey($key)) {
        Write-Warning "Seed skip missing $key"
        continue
    }
    $parts = $key.Split(".")
    $schema = $parts[0]; $table = $parts[1]
    $tcols = @($colsByTable[$key] | Sort-Object ColumnId | Where-Object { $_.ColumnName -ne "RowVersion" })
    $colList = ($tcols | ForEach-Object { "`"$($_.ColumnName)`"" }) -join ", "
    $selectList = ($tcols | ForEach-Object { "[$($_.ColumnName)]" }) -join ", "
    $fileName = "$schema.$table.tsv"
    $filePath = Join-Path $SeedDir $fileName

    Write-Host "Exporting seed $key ..."
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "SELECT $selectList FROM [$schema].[$table]"
    $cmd.CommandTimeout = 600
    $reader = $cmd.ExecuteReader()
    $sw = New-Object System.IO.StreamWriter($filePath, $false, [System.Text.UTF8Encoding]::new($false))
    $rowCount = 0
    try {
        while ($reader.Read()) {
            $vals = New-Object string[] $tcols.Count
            for ($i = 0; $i -lt $tcols.Count; $i++) {
                $vals[$i] = Escape-CopyText $reader.GetValue($i)
            }
            $sw.WriteLine(($vals -join "`t"))
            $rowCount++
        }
    }
    finally {
        $reader.Close()
        $sw.Close()
    }
    Write-Host "  -> $rowCount rows"

    [void]$copySql.AppendLine("TRUNCATE TABLE `"$schema`".`"$table`" CASCADE;")
    [void]$copySql.AppendLine("\copy `"$schema`".`"$table`" ($colList) FROM 'seed/$fileName' WITH (FORMAT text, NULL '\N')")
    [void]$copySql.AppendLine()
}

[void]$copySql.AppendLine(@"
DO $$
DECLARE r record;
        mx bigint;
BEGIN
  FOR r IN
    SELECT n.nspname AS sch, c.relname AS tbl, a.attname AS col,
           pg_get_serial_sequence(format('%I.%I', n.nspname, c.relname), a.attname) AS seq
    FROM pg_class c
    JOIN pg_namespace n ON n.oid = c.relnamespace
    JOIN pg_attribute a ON a.attrelid = c.oid AND a.attnum > 0 AND NOT a.attisdropped
    WHERE n.nspname IN ('app','core')
      AND c.relkind = 'r'
      AND a.attidentity <> ''
  LOOP
    IF r.seq IS NULL THEN CONTINUE; END IF;
    EXECUTE format('SELECT MAX(%I) FROM %I.%I', r.col, r.sch, r.tbl) INTO mx;
    IF mx IS NULL THEN
      EXECUTE format('SELECT setval(%L, 1, false)', r.seq);
    ELSE
      EXECUTE format('SELECT setval(%L, %s, true)', r.seq, mx);
    END IF;
  END LOOP;
END $$;
"@)

$seedSqlPath = Join-Path $SeedDir "seed-bamboeup-hr.sql"
[System.IO.File]::WriteAllText($seedSqlPath, $copySql.ToString(), [System.Text.UTF8Encoding]::new($false))
Write-Host "Wrote $seedSqlPath"

$conn.Close()
Write-Host "Done."
