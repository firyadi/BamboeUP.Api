<#
.SYNOPSIS
  Export platform seed data from Dev.BamboeHR for SQL Server + PostgreSQL.

.DESCRIPTION
  Tables = SchemaStudio Platform export Include data (live catalog on SchemaStudio_DB).
  DDL-only catalog rows (Company, Person, …) are not seeded.

  Output:
    BamboeUp.Api/Database/SqlServer/seed/   (*.tsv + seed-platform.sql + Load-PlatformSeed.ps1)
    BamboeUp.Api/Database/PostgreSQL/seed/  (*.tsv + seed-bamboeup-hr.sql)

.EXAMPLE
  pwsh -File Export-PlatformSeed.ps1
  pwsh -File Export-PlatformSeed.ps1 -AlsoWriteVioletta
#>
[CmdletBinding()]
param(
    [string]$Server = "localhost",
    [string]$Database = "Dev.BamboeHR",
    [string]$User = "sa",
    [string]$Password = "123",
    [string]$ApiDatabaseRoot = "",
    [switch]$AlsoWriteVioletta,
    [string]$ViolettaRoot = "D:\Project\ViolettaHR",
    [switch]$SqlServerOnly,
    [switch]$TrustedConnection
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($ApiDatabaseRoot)) {
    $ApiDatabaseRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
}

$SsSeedDir = Join-Path $ApiDatabaseRoot "SqlServer\seed"
$PgSeedDir = Join-Path $ApiDatabaseRoot "PostgreSQL\seed"
New-Item -ItemType Directory -Force -Path $SsSeedDir | Out-Null
New-Item -ItemType Directory -Force -Path $PgSeedDir | Out-Null

# Include data = Yes on http://localhost:5003/platform-export
$seedTables = @(
    "core.Users",
    "core.UserGroup",
    "core.Programs",
    "core.UserGroupProgram",
    "core.UserGroupScope",
    "core.UserCompanyScope",
    "core.UserOrganizationScope",
    "app.Parameter",
    "app.ParameterScope",
    "app.StandardReference",
    "app.StandardReferenceItem",
    "app.StandardReferenceScope",
    "app.StandardReferenceScopeItem",
    "core.Country",
    "core.Province",
    "core.City",
    "core.District",
    "core.SubDistrict",
    "core.PostalCode",
    "core.Bank",
    "core.ReportDefinition",
    "core.ReportParameter",
    "core.ReportParameterCatalog",
    "core.SharedParameterCatalog",
    "core.SharedParameterProfile",
    "core.SharedParameterProfileField"
)

function Escape-PgCopy([object]$v) {
    if ($null -eq $v -or [DBNull]::Value.Equals($v)) { return "\N" }
    if ($v -is [bool]) { return $(if ($v) { "t" } else { "f" }) }
    if ($v -is [byte[]]) {
        $hex = ($v | ForEach-Object { $_.ToString("x2") }) -join ""
        return "\\x$hex"
    }
    if ($v -is [datetime]) { return ($v.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff") + "+00") }
    if ($v -is [guid]) { return $v.ToString() }
    $s = [string]$v
    return $s.Replace("\", "\\").Replace("`t", "\t").Replace("`n", "\n").Replace("`r", "\r")
}

function Escape-SsTsv([object]$v) {
    if ($null -eq $v -or [DBNull]::Value.Equals($v)) { return "" }
    if ($v -is [bool]) { return $(if ($v) { "1" } else { "0" }) }
    if ($v -is [byte[]]) { return "" }
    if ($v -is [datetime]) { return $v.ToString("yyyy-MM-ddTHH:mm:ss.fffffff") }
    if ($v -is [guid]) { return $v.ToString() }
    $s = [string]$v
    return $s.Replace("`t", " ").Replace("`r", "").Replace("`n", " ")
}

Write-Host "Connecting $Server / $Database ..."
$cs = if ($TrustedConnection) {
    "Server=$Server;Database=$Database;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;"
} else {
    "Server=$Server;Database=$Database;User Id=$User;Password=$Password;TrustServerCertificate=True;Encrypt=False;"
}

# Prefer Microsoft.Data.SqlClient if available; else System.Data.SqlClient
$conn = $null
try {
    $conn = [Microsoft.Data.SqlClient.SqlConnection]::new($cs)
    $conn.Open()
} catch {
    Add-Type -AssemblyName "System.Data"
    $conn = New-Object System.Data.SqlClient.SqlConnection $cs
    $conn.Open()
}

function Get-TableColumns([string]$schema, [string]$table) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = @"
SELECT c.name AS ColumnName, t.name AS TypeName, c.is_identity AS IsIdentity
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID(@obj)
  AND c.name <> N'RowVersion'
  AND t.name NOT IN (N'timestamp', N'rowversion')
ORDER BY c.column_id;
"@
    $null = $cmd.Parameters.AddWithValue("@obj", "[$schema].[$table]")
    $dt = New-Object System.Data.DataTable
    $r = $cmd.ExecuteReader()
    $dt.Load($r)
    $r.Close()
    return @($dt.Rows)
}

$ssAbs = (Resolve-Path $SsSeedDir).Path
$ssSql = New-Object System.Text.StringBuilder
[void]$ssSql.AppendLine("-- Platform seed for SQL Server (from Dev.BamboeHR)")
[void]$ssSql.AppendLine("-- Prefer: pwsh -File Load-PlatformSeed.ps1 -Database YourDb")
[void]$ssSql.AppendLine("-- Or BULK INSERT below (paths are absolute to this machine's export folder).")
[void]$ssSql.AppendLine("SET NOCOUNT ON;")
[void]$ssSql.AppendLine("SET XACT_ABORT ON;")
[void]$ssSql.AppendLine("GO")
[void]$ssSql.AppendLine()

$pgSql = New-Object System.Text.StringBuilder
[void]$pgSql.AppendLine("-- Platform + geo seed (from Dev.BamboeHR)")
[void]$pgSql.AppendLine("-- Apply AFTER init DDL, from Database/PostgreSQL directory:")
[void]$pgSql.AppendLine("--   psql ... -f seed/seed-bamboeup-hr.sql")
[void]$pgSql.AppendLine("SET client_min_messages TO WARNING;")
[void]$pgSql.AppendLine()

$manifest = New-Object System.Collections.Generic.List[object]
$loadOrder = New-Object System.Collections.Generic.List[string]

foreach ($key in $seedTables) {
    $parts = $key.Split(".")
    $schema = $parts[0]; $table = $parts[1]

    $objIdCmd = $conn.CreateCommand()
    $objIdCmd.CommandText = "SELECT OBJECT_ID(@o)"
    $null = $objIdCmd.Parameters.AddWithValue("@o", "[$schema].[$table]")
    $oid = $objIdCmd.ExecuteScalar()
    if ($null -eq $oid -or [DBNull]::Value.Equals($oid)) {
        Write-Warning "Skip missing $key"
        continue
    }

    $cols = Get-TableColumns $schema $table
    if ($cols.Count -eq 0) {
        Write-Warning "Skip no columns $key"
        continue
    }

    $colNames = @($cols | ForEach-Object { $_.ColumnName })
    $selectList = ($colNames | ForEach-Object { "[$_]" }) -join ", "
    $pgColList = ($colNames | ForEach-Object { "`"$_`"" }) -join ", "
    $ssColList = ($colNames | ForEach-Object { "[$_]" }) -join ", "
    $hasIdentity = @($cols | Where-Object { [bool]$_.IsIdentity }).Count -gt 0

    $fileName = "$schema.$table.tsv"
    $ssPath = Join-Path $SsSeedDir $fileName
    $pgPath = Join-Path $PgSeedDir $fileName

    Write-Host "Exporting $key ..."
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "SELECT $selectList FROM [$schema].[$table]"
    $cmd.CommandTimeout = 600
    $reader = $cmd.ExecuteReader()

    $ssSw = New-Object System.IO.StreamWriter($ssPath, $false, [System.Text.UTF8Encoding]::new($false))
    $pgSw = $null
    if (-not $SqlServerOnly) {
        $pgSw = New-Object System.IO.StreamWriter($pgPath, $false, [System.Text.UTF8Encoding]::new($false))
    }
    $rowCount = 0
    try {
        while ($reader.Read()) {
            $ssVals = New-Object string[] $colNames.Count
            $pgVals = New-Object string[] $colNames.Count
            for ($i = 0; $i -lt $colNames.Count; $i++) {
                $v = $reader.GetValue($i)
                $ssVals[$i] = Escape-SsTsv $v
                $pgVals[$i] = Escape-PgCopy $v
            }
            $ssSw.WriteLine(($ssVals -join "`t"))
            if ($pgSw) { $pgSw.WriteLine(($pgVals -join "`t")) }
            $rowCount++
        }
    }
    finally {
        $reader.Close()
        $ssSw.Close()
        if ($pgSw) { $pgSw.Close() }
    }
    Write-Host "  -> $rowCount rows"

    $manifest.Add([pscustomobject]@{
        schema = $schema; table = $table; rows = $rowCount
        file = $fileName; hasIdentity = $hasIdentity
        columns = $colNames
    })
    $loadOrder.Add($key) | Out-Null

    $absTsv = Join-Path $ssAbs $fileName
    [void]$ssSql.AppendLine("-- $key ($rowCount rows)")
    [void]$ssSql.AppendLine("DELETE FROM [$schema].[$table];")
    [void]$ssSql.AppendLine("GO")
    $keepId = if ($hasIdentity) { "KEEPIDENTITY," } else { "" }
    [void]$ssSql.AppendLine(@"
BULK INSERT [$schema].[$table] ($ssColList)
FROM '$($absTsv.Replace("'","''"))'
WITH (
    DATAFILETYPE = 'char',
    FIELDTERMINATOR = '\t',
    ROWTERMINATOR = '0x0a',
    KEEPNULLS,
    $keepId
    TABLOCK,
    CODEPAGE = '65001'
);
GO
"@)
    [void]$ssSql.AppendLine()

    [void]$pgSql.AppendLine("TRUNCATE TABLE `"$schema`".`"$table`" CASCADE;")
    [void]$pgSql.AppendLine("\copy `"$schema`".`"$table`" ($pgColList) FROM 'seed/$fileName' WITH (FORMAT text, NULL '\N')")
    [void]$pgSql.AppendLine()
}

[void]$ssSql.AppendLine(@"
-- Reseed identities to MAX
DECLARE @t sysname, @s sysname, @c sysname, @m bigint, @sql nvarchar(400);
DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
SELECT s.name, t.name, ic.name
FROM sys.tables t
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
INNER JOIN sys.identity_columns ic ON ic.object_id = t.object_id
WHERE s.name IN (N'app', N'core', N'apv');
OPEN cur;
FETCH NEXT FROM cur INTO @s, @t, @c;
WHILE @@FETCH_STATUS = 0
BEGIN
  SET @sql = N'SELECT @mx = ISNULL(MAX(' + QUOTENAME(@c) + N'), 0) FROM ' + QUOTENAME(@s) + N'.' + QUOTENAME(@t);
  EXEC sp_executesql @sql, N'@mx bigint OUTPUT', @mx = @m OUTPUT;
  IF @m < 1 SET @m = 1;
  DBCC CHECKIDENT (@s + N'.' + @t, RESEED, @m) WITH NO_INFOMSGS;
  FETCH NEXT FROM cur INTO @s, @t, @c;
END
CLOSE cur; DEALLOCATE cur;
GO
"@)

[void]$pgSql.AppendLine(@"
DO `$`$
DECLARE r record;
        mx bigint;
BEGIN
  FOR r IN
    SELECT n.nspname AS sch, c.relname AS tbl, a.attname AS col,
           pg_get_serial_sequence(format('%I.%I', n.nspname, c.relname), a.attname) AS seq
    FROM pg_class c
    JOIN pg_namespace n ON n.oid = c.relnamespace
    JOIN pg_attribute a ON a.attrelid = c.oid AND a.attnum > 0 AND NOT a.attisdropped
    WHERE n.nspname IN ('app','core','apv')
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
END
`$`$;
"@)

$ssSqlPath = Join-Path $SsSeedDir "seed-platform.sql"
$pgSqlPath = Join-Path $PgSeedDir "seed-bamboeup-hr.sql"
$manifestPath = Join-Path $SsSeedDir "seed-manifest.json"
$loadPs1 = Join-Path $SsSeedDir "Load-PlatformSeed.ps1"

[System.IO.File]::WriteAllText($ssSqlPath, $ssSql.ToString(), [System.Text.UTF8Encoding]::new($false))
if (-not $SqlServerOnly) {
    [System.IO.File]::WriteAllText($pgSqlPath, $pgSql.ToString(), [System.Text.UTF8Encoding]::new($false))
}
($manifest | ConvertTo-Json -Depth 5) | Set-Content -Path $manifestPath -Encoding UTF8

if (-not (Test-Path $loadPs1)) {
    Write-Warning "Load-PlatformSeed.ps1 missing at $loadPs1 (expected checked-in helper)."
}

Write-Host "Wrote $ssSqlPath"
if (-not $SqlServerOnly) { Write-Host "Wrote $pgSqlPath" }
Write-Host "Wrote $manifestPath"

if ($AlsoWriteVioletta -and (Test-Path $ViolettaRoot)) {
    $vSeed = Join-Path $ViolettaRoot "database\seed"
    New-Item -ItemType Directory -Force -Path $vSeed | Out-Null
    Copy-Item (Join-Path $SsSeedDir "*.tsv") $vSeed -Force
    Copy-Item $ssSqlPath (Join-Path $vSeed "seed-platform.sql") -Force
    Copy-Item $manifestPath (Join-Path $vSeed "seed-manifest.json") -Force
    Copy-Item $loadPs1 (Join-Path $vSeed "Load-PlatformSeed.ps1") -Force
    @"
Platform seed pack (SQL Server + PostgreSQL)
--------------------------------------------
Source: Dev.BamboeHR via Export-PlatformSeed.ps1
Tables: IncludeData catalog intersect init-platform (+ Company/Office/Org for scope FKs)

SQL Server:
  1. init-platform.sql
  2. pwsh -File Load-PlatformSeed.ps1 -Database ViolettaHR

PostgreSQL:
  Use BamboeUp.Api/Database/PostgreSQL/seed/seed-bamboeup-hr.sql after init DDL.
"@ | Set-Content (Join-Path $vSeed "README.txt") -Encoding UTF8
    Write-Host "Also wrote $vSeed"
}

# Update seed-platform README in Violetta database root
$readmePlatform = Join-Path (Split-Path $ViolettaRoot -ErrorAction SilentlyContinue) ""
if ($AlsoWriteVioletta -and (Test-Path (Join-Path $ViolettaRoot "database\seed-platform.README.txt"))) {
    $extra = @"

Seed data (dual dialect)
------------------------
SQL Server: database/seed/Load-PlatformSeed.ps1 + *.tsv
PostgreSQL: BamboeUp.Api/Database/PostgreSQL/seed/seed-bamboeup-hr.sql
Regenerate: BamboeUp.Api/Database/tools/Export-PlatformSeed.ps1 -AlsoWriteVioletta
"@
    Add-Content (Join-Path $ViolettaRoot "database\seed-platform.README.txt") $extra
}

$conn.Close()
$total = ($manifest | Measure-Object rows -Sum).Sum
Write-Host "Done. Tables: $($manifest.Count)  Total rows: $total"
