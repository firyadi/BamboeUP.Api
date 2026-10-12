<#
  Load TSV seed into a SQL Server platform database (keeps identity values).
  Example:
    powershell -File Load-PlatformSeed.ps1 -Database ViolettaHR
#>
param(
    [string]$Server = "localhost",
    [Parameter(Mandatory)][string]$Database,
    [string]$User = "sa",
    [string]$Password = "123",
    [string]$SeedDir = $PSScriptRoot
)

$ErrorActionPreference = "Stop"
if ([string]::IsNullOrWhiteSpace($SeedDir)) {
    if ($PSScriptRoot) {
        $SeedDir = $PSScriptRoot
    }
    elseif ($MyInvocation.MyCommand.Path) {
        $SeedDir = Split-Path -Parent $MyInvocation.MyCommand.Path
    }
    else {
        $SeedDir = (Get-Location).Path
    }
}
$manifestPath = Join-Path $SeedDir "seed-manifest.json"
if (-not (Test-Path $manifestPath)) {
    throw "Missing seed-manifest.json in $SeedDir - run Export-PlatformSeed.ps1 first."
}
$manifest = Get-Content $manifestPath -Raw | ConvertFrom-Json
$cs = "Server=$Server;Database=$Database;User Id=$User;Password=$Password;TrustServerCertificate=True;Encrypt=False;"

$useMds = $false
try {
    $conn = New-Object Microsoft.Data.SqlClient.SqlConnection $cs
    $conn.Open()
    $useMds = $true
} catch {
    Add-Type -AssemblyName "System.Data"
    $conn = New-Object System.Data.SqlClient.SqlConnection $cs
    $conn.Open()
}

function Get-SqlColumnTypes($connection, $schemaName, $tableName) {
    $cmd = $connection.CreateCommand()
    $cmd.CommandText = @"
SELECT c.name, ty.name, c.is_nullable
FROM sys.columns c
INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
INNER JOIN sys.tables t ON c.object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE s.name = @schema AND t.name = @table
"@
    [void]$cmd.Parameters.AddWithValue("@schema", $schemaName)
    [void]$cmd.Parameters.AddWithValue("@table", $tableName)
    $map = @{}
    $reader = $cmd.ExecuteReader()
    try {
        while ($reader.Read()) {
            $map[[string]$reader.GetValue(0)] = @{
                Type = [string]$reader.GetValue(1)
                Nullable = [bool]$reader.GetValue(2)
            }
        }
    }
    finally {
        $reader.Close()
    }
    return ,$map
}

function Get-NetColumnType($sqlType) {
    switch ($sqlType) {
        "uniqueidentifier" { return [guid] }
        "bigint" { return [int64] }
        "int" { return [int] }
        "smallint" { return [int16] }
        "tinyint" { return [byte] }
        "bit" { return [bool] }
        "datetime" { return [datetime] }
        "datetime2" { return [datetime] }
        "date" { return [datetime] }
        "smalldatetime" { return [datetime] }
        "decimal" { return [decimal] }
        "numeric" { return [decimal] }
        "money" { return [decimal] }
        "smallmoney" { return [decimal] }
        "float" { return [double] }
        "real" { return [single] }
        default { return [string] }
    }
}

function Convert-SeedValue($meta, $val) {
    $sqlType = if ($meta) { $meta.Type } else { "nvarchar" }
    $nullable = if ($meta) { [bool]$meta.Nullable } else { $true }
    $empty = [string]::IsNullOrEmpty($val) -or $val -eq '\N'
    if ($empty) {
        if ($nullable) { return [DBNull]::Value }
        switch ($sqlType) {
            "uniqueidentifier" { return [guid]::Empty }
            "bigint" { return [int64]0 }
            "int" { return [int]0 }
            "smallint" { return [int16]0 }
            "tinyint" { return [byte]0 }
            "bit" { return $false }
            "datetime" { return [datetime]"1900-01-01" }
            "datetime2" { return [datetime]"1900-01-01" }
            "date" { return [datetime]"1900-01-01" }
            "smalldatetime" { return [datetime]"1900-01-01" }
            "decimal" { return [decimal]0 }
            "numeric" { return [decimal]0 }
            "money" { return [decimal]0 }
            "smallmoney" { return [decimal]0 }
            "float" { return [double]0 }
            "real" { return [single]0 }
            default { return "" }
        }
    }
    switch ($sqlType) {
        "uniqueidentifier" { return [guid]$val }
        "bigint" { return [int64]$val }
        "int" { return [int]$val }
        "smallint" { return [int16]$val }
        "tinyint" { return [byte]$val }
        "bit" { return [bool](($val -eq "1") -or ($val -eq "true") -or ($val -eq "True")) }
        "datetime" { return [datetime]$val }
        "datetime2" { return [datetime]$val }
        "date" { return [datetime]$val }
        "smalldatetime" { return [datetime]$val }
        "decimal" { return [decimal]$val }
        "numeric" { return [decimal]$val }
        "money" { return [decimal]$val }
        "smallmoney" { return [decimal]$val }
        "float" { return [double]$val }
        "real" { return [single]$val }
        default { return $val }
    }
}

$toggleFk = $conn.CreateCommand()
$toggleFk.CommandTimeout = 600
$toggleFk.CommandText = "EXEC sys.sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';"
[void]$toggleFk.ExecuteNonQuery()

foreach ($m in $manifest) {
    $schema = $m.schema
    $table = $m.table
    $tsv = Join-Path $SeedDir $m.file
    if (-not (Test-Path $tsv)) {
        Write-Warning "Missing $tsv"
        continue
    }

    Write-Host "Loading $schema.$table ..."
    $sqlTypes = Get-SqlColumnTypes $conn $schema $table
    $del = $conn.CreateCommand()
    $del.CommandText = "DELETE FROM [$schema].[$table];"
    $del.CommandTimeout = 600
    [void]$del.ExecuteNonQuery()

    $dt = New-Object System.Data.DataTable
    foreach ($c in $m.columns) {
        $sqlType = if ($sqlTypes[$c]) { $sqlTypes[$c].Type } else { "nvarchar" }
        [void]$dt.Columns.Add($c, (Get-NetColumnType $sqlType))
    }

    $sr = New-Object System.IO.StreamReader($tsv, (New-Object System.Text.UTF8Encoding $false))
    try {
        while ($null -ne ($line = $sr.ReadLine())) {
            $parts = $line.Split([char]9)
            $row = $dt.NewRow()
            for ($i = 0; $i -lt $m.columns.Count; $i++) {
                $colName = [string]$m.columns[$i]
                $val = if ($i -lt $parts.Length) { $parts[$i] } else { "" }
                $row[$i] = Convert-SeedValue $sqlTypes[$colName] $val
            }
            $dt.Rows.Add($row)
        }
    }
    finally {
        $sr.Close()
    }

    if ($useMds) {
        $opts = [Microsoft.Data.SqlClient.SqlBulkCopyOptions]::KeepIdentity
        $bulk = New-Object Microsoft.Data.SqlClient.SqlBulkCopy($conn, $opts, $null)
    }
    else {
        $opts = [System.Data.SqlClient.SqlBulkCopyOptions]::KeepIdentity
        $bulk = New-Object System.Data.SqlClient.SqlBulkCopy($conn, $opts, $null)
    }
    $bulk.DestinationTableName = "[$schema].[$table]"
    $bulk.BulkCopyTimeout = 0
    foreach ($c in $m.columns) {
        [void]$bulk.ColumnMappings.Add($c, $c)
    }
    $bulk.WriteToServer($dt)
    $bulk.Close()
    Write-Host ("  loaded {0} rows" -f $dt.Rows.Count)
}

$reseed = $conn.CreateCommand()
$reseed.CommandTimeout = 600
$reseed.CommandText = @'
DECLARE @t sysname, @s sysname, @c sysname, @m bigint, @sql nvarchar(400), @full nvarchar(300);
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
  SET @full = @s + N'.' + @t;
  DBCC CHECKIDENT (@full, RESEED, @m) WITH NO_INFOMSGS;
  FETCH NEXT FROM cur INTO @s, @t, @c;
END
CLOSE cur; DEALLOCATE cur;
'@
[void]$reseed.ExecuteNonQuery()
$toggleFk.CommandText = "EXEC sys.sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';"
[void]$toggleFk.ExecuteNonQuery()
$conn.Close()
Write-Host "Load complete."
