$files = @("Service.Shell/ServiceShellManager.cs", "Service.Modules/ServiceModulesManager.cs")
foreach ($file in $files) {
    $content = Get-Content $file -Raw
    
    # 1. Remove IMapper from the constructor of ServiceShellManager/ServiceModulesManager
    $content = $content -replace ',\s*IMapper\s+mapper', ''
    $content = $content -replace 'IMapper\s+mapper\s*,', ''
    
    # 2. Remove mapper from the new Service(...) calls
    $content = $content -replace ',\s*mapper', ''
    $content = $content -replace 'mapper\s*,', ''
    
    Set-Content -Path $file -Value $content -NoNewline
}
Write-Host "Managers fixed."
