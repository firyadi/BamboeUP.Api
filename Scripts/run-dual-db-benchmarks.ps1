# Dual-database smoke benchmarks & load tests
# Usage (from BamboeUp.Api):
#   pwsh ./Scripts/run-dual-db-benchmarks.ps1

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot

Write-Host "== Unit (no Docker) ==" -ForegroundColor Cyan
dotnet test "$root/BamboeUp.Api.Tests" --filter "FullyQualifiedName~Database&Category!=Integration" --nologo

Write-Host "`n== Benchmarks (skip if DB unavailable) ==" -ForegroundColor Cyan
dotnet test "$root/BamboeUp.Api.Tests" --filter "Category=Benchmark" --nologo

Write-Host "`n== Load smoke (skip if Docker unavailable) ==" -ForegroundColor Cyan
dotnet test "$root/BamboeUp.Api.Tests" --filter "Category=Load" --nologo

Write-Host "`nDone." -ForegroundColor Green
