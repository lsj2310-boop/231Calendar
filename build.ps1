# Build 231 Calendar on Windows with .NET 8 SDK and NSIS 3.x.
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot
$out = Join-Path $PSScriptRoot 'artifacts'
$publish = Join-Path $out 'publish'
New-Item -ItemType Directory -Force -Path $publish | Out-Null
& dotnet publish Calendar231/231Calendar.csproj -c Release -r win-x64 --self-contained false -o $publish -p:DebugType=None -p:DebugSymbols=false
if ($LASTEXITCODE -ne 0) { throw 'Publish failed' }
Copy-Item LICENSE $publish
Copy-Item README.md (Join-Path $publish 'README.md')
Get-Content README.md -Raw -Encoding UTF8 | Set-Content (Join-Path $publish '사용안내.txt') -Encoding UTF8
$stage = Join-Path ([IO.Path]::GetTempPath()) ('231Calendar-source-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $stage | Out-Null
try {
    Get-ChildItem $PSScriptRoot -File -Recurse | Where-Object {
        $_.FullName.Substring($PSScriptRoot.Length) -notmatch '(^|[\\/])(bin|obj|artifacts|\.git)([\\/]|$)'
    } | ForEach-Object {
        $relative=$_.FullName.Substring($PSScriptRoot.Length).TrimStart('\','/')
        $target=Join-Path $stage $relative
        New-Item -ItemType Directory -Force -Path (Split-Path $target) | Out-Null
        Copy-Item -LiteralPath $_.FullName -Destination $target
    }
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $source=Join-Path $publish 'Source.zip'
    if (Test-Path $source) { Remove-Item $source }
    [IO.Compression.ZipFile]::CreateFromDirectory($stage,$source)
} finally { Remove-Item $stage -Recurse -Force }
$lines=@()
Get-ChildItem $publish -File -Recurse | ForEach-Object {
    $rel=$_.FullName.Substring($publish.Length).TrimStart('\','/')
    $lines+='Delete "$INSTDIR\'+$rel+'"'
}
Get-ChildItem $publish -Directory -Recurse | Sort-Object { $_.FullName.Length } -Descending | ForEach-Object {
    $rel=$_.FullName.Substring($publish.Length).TrimStart('\','/')
    $lines+='RMDir "$INSTDIR\'+$rel+'"'
}
$lines | Set-Content installer/uninstall-files.nsh -Encoding UTF8
$nsis=(Get-Command makensis.exe -ErrorAction SilentlyContinue).Source
if (-not $nsis) { $nsis=Join-Path ${env:ProgramFiles(x86)} 'NSIS\makensis.exe' }
& $nsis "/DPAYLOAD=$publish" "/DOUTPUT=$out\231Calendar_Setup_v1.0.1.exe" installer/231Calendar.nsi
if ($LASTEXITCODE -ne 0) { throw 'Installer build failed' }
Write-Host "Created $out\231Calendar_Setup_v1.0.1.exe"
