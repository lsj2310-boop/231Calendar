# 231 Calendar dependency bootstrapper. Modified 2026-11-16. AGPL-3.0-or-later.
param([switch]$CheckOnly)
$ErrorActionPreference = 'Stop'
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$work = Join-Path $env:TEMP ('231Calendar-' + [Guid]::NewGuid().ToString('N'))
$data = Join-Path $env:LOCALAPPDATA '231Calendar'
$log = Join-Path $data 'setup.log'
New-Item -ItemType Directory -Force -Path $data | Out-Null
function Log([string]$message) {
    $line = (Get-Date -Format s) + ' ' + $message
    Add-Content -LiteralPath $log -Value $line -Encoding UTF8
    Write-Host $line
}
function HasDesktopRuntime {
    $pf = $env:ProgramW6432
    if (-not $pf) { $pf = $env:ProgramFiles }
    $folder = Join-Path $pf 'dotnet\shared\Microsoft.WindowsDesktop.App'
    if (-not (Test-Path -LiteralPath $folder)) { return $false }
    return [bool](Get-ChildItem -LiteralPath $folder -Directory |
        Where-Object { $_.Name -match '^8\.0\.\d+$' -and (Test-Path -LiteralPath (Join-Path $_.FullName 'System.Windows.Forms.dll')) } |
        Select-Object -First 1)
}
function HasWebView {
    $id = '{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}'
    $paths = @("Registry::HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\$id",
               "Registry::HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\EdgeUpdate\Clients\$id",
               "Registry::HKEY_CURRENT_USER\Software\Microsoft\EdgeUpdate\Clients\$id")
    foreach ($key in $paths) {
        $v = (Get-ItemProperty -LiteralPath $key -Name pv -ErrorAction SilentlyContinue).pv
        if ($v -and $v -ne '0.0.0.0') { return $true }
    }
    return $false
}
function Download([string]$url, [string]$name) {
    $path = Join-Path $work $name
    Log ('Downloading ' + $name + ' from Microsoft')
    $client = New-Object Net.WebClient
    try { $client.DownloadFile($url, $path) } finally { $client.Dispose() }
    # Do not execute an HTML error page or an unsigned download.
    $sig = Get-AuthenticodeSignature -LiteralPath $path
    if ($sig.Status -ne 'Valid' -or $sig.SignerCertificate.Subject -notmatch 'O=Microsoft Corporation') {
        throw ('Microsoft signature verification failed: ' + $name + ' (' + $sig.Status + ')')
    }
    return $path
}
try {
    $dotnet = HasDesktopRuntime
    $webview = HasWebView
    Log ('.NET 8 Desktop x64: ' + $dotnet + '; WebView2: ' + $webview)
    if ($CheckOnly) { if ($dotnet -and $webview) { exit 0 } else { exit 2 } }
    New-Item -ItemType Directory -Path $work | Out-Null
    if (-not $dotnet) {
        $file = Download 'https://aka.ms/dotnet/8.0/windowsdesktop-runtime-win-x64.exe' 'dotnet8-desktop.exe'
        Log 'Installing .NET 8 Desktop Runtime (Windows may request administrator approval)'
        $process = Start-Process -FilePath $file -ArgumentList '/install /passive /norestart' -Verb RunAs -PassThru -Wait
        if ($process.ExitCode -notin @(0,3010,1641)) { throw ('.NET installer exit code: ' + $process.ExitCode) }
        if (-not (HasDesktopRuntime)) { throw '.NET Desktop Runtime could not be detected after installation. Restart Windows and run setup again.' }
    }
    if (-not $webview) {
        $file = Download 'https://go.microsoft.com/fwlink/p/?LinkId=2124703' 'webview2-bootstrapper.exe'
        Log 'Installing Microsoft Edge WebView2 Runtime'
        $process = Start-Process -FilePath $file -ArgumentList '/silent /install' -PassThru -Wait
        if ($process.ExitCode -notin @(0,3010,1641)) { throw ('WebView2 installer exit code: ' + $process.ExitCode) }
        if (-not (HasWebView)) { throw 'WebView2 Runtime could not be detected after installation. Restart Windows and run setup again.' }
    }
    Log 'Dependencies ready'
    exit 0
} catch {
    Log ('FAILED: ' + $_.Exception.Message)
    exit 1
} finally {
    if (Test-Path -LiteralPath $work) { Remove-Item -LiteralPath $work -Recurse -Force -ErrorAction SilentlyContinue }
}
