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
$manifestPath = Join-Path $SeedDir "seed-manifest.json"
if (-not (Test-Path $manifestPath)) {
    throw "Missing seed-manifest.json in $SeedDir — run Export-PlatformSeed.ps1 first."
}
$manifest = Get-Content $manifestPath -Raw | ConvertFrom-Json
$cs = "Server=$Server;Database=$Database;User Id=$User;Password=$Password;TrustServerCertificate=True;Encrypt=False;"

$useMds = $false
try {
    $conn = [Microsoft.Data.SqlClient.SqlConnection]::new($cs)
    $conn.Open()
    $useMds = $true
} catch {
    Add-Type -AssemblyName "System.Data"
    $conn = New-Object System.Data.SqlClient.SqlConnection $cs
    $conn.Open()
}

foreach ($m in $manifest) {
    $schema = $m.schema
    $table = $m.table
    $tsv = Join-Path $SeedDir $m.file
    if (-not (Test-Path $tsv)) {
        Write-Warning "Missing $tsv"
        continue
    }

    Write-Host "Loading $schema.$table ..."
    $del = $conn.CreateCommand()
    $del.CommandText = "DELETE FROM [$schema].[$table];"
    $del.CommandTimeout = 600
    [void]$del.ExecuteNonQuery()

    $dt = New-Object System.Data.DataTable
    foreach ($c in $m.columns) {
        [void]$dt.Columns.Add([string]$c)
    }

    $sr = New-Object System.IO.StreamReader($tsv, [System.Text.UTF8Encoding]::new($false))
    try {
        while ($null -ne ($line = $sr.ReadLine())) {
            $parts = $line.Split([char]9)
            $row = $dt.NewRow()
            for ($i = 0; $i -lt $m.columns.Count; $i++) {
                $val = if ($i -lt $parts.Length) { $parts[$i] } else { "" }
                if ([string]::IsNullOrEmpty($val)) {
                    $row[$i] = [DBNull]::Value
                }
                else {
                    $row[$i] = $val
                }
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
    Write-Host "  -> $($dt.Rows.Count) rows"
}

$reseed = $conn.CreateCommand()
$reseed.CommandTimeout = 600
$reseed.CommandText = @"
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
"@
[void]$reseed.ExecuteNonQuery()
$conn.Close()
Write-Host "Load complete."
