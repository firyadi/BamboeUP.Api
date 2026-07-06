$files = Get-ChildItem -Path "Service.Shell", "Service.Modules" -Recurse -Filter "*.cs" | Select-String -Pattern "IMapper" | Select-Object -ExpandProperty Path -Unique

foreach ($file in $files) {
    $content = Get-Content $file -Raw
    
    # 1. Replace using statement
    $content = $content -replace 'using AutoMapper;', 'using Mapster;'
    
    # 2. Remove private readonly field
    $content = $content -replace 'private readonly IMapper _mapper;\r?\n\s*', ''
    
    # 3. Remove IMapper from constructor signature. 
    $content = $content -replace ',\s*IMapper\s+mapper', ''
    $content = $content -replace 'IMapper\s+mapper\s*,', ''
    $content = $content -replace 'IMapper\s+mapper', ''
    
    # 4. Remove assignment in constructor body
    $content = $content -replace '_mapper\s*=\s*mapper;\r?\n\s*', ''
    
    # 5. Replace _mapper.Map<Target>(source)
    $content = [System.Text.RegularExpressions.Regex]::Replace($content, '_mapper\.Map<([^>]+)>\((.*?)\)', { param($m) $m.Groups[2].Value + '.Adapt<' + $m.Groups[1].Value + '>()' })
    
    # 6. Replace _mapper.Map(source, target)
    $content = [System.Text.RegularExpressions.Regex]::Replace($content, '_mapper\.Map\(([^,]+),\s*([^)]+)\)', { param($m) $m.Groups[1].Value + '.Adapt(' + $m.Groups[2].Value + ')' })

    Set-Content -Path $file -Value $content -NoNewline
}
Write-Host "Migration script completed successfully."
