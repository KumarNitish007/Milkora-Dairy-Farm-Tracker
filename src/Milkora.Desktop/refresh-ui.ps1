# Rebuilds the Angular app (production) and copies it into this host's wwwroot.
# Run from src/Milkora.Desktop:  pwsh ./refresh-ui.ps1
$ErrorActionPreference = "Stop"

$here    = $PSScriptRoot
$client  = Join-Path $here "..\..\Milkora.Client"
$dist    = Join-Path $client "dist\milkora.client\browser"
$wwwroot = Join-Path $here "wwwroot"

Write-Host "Building Angular (production)..." -ForegroundColor Cyan
Push-Location $client
try { ng build --configuration production } finally { Pop-Location }

if (-not (Test-Path (Join-Path $dist "index.html"))) {
    throw "Angular build output not found at $dist"
}

Write-Host "Refreshing wwwroot..." -ForegroundColor Cyan
if (Test-Path $wwwroot) { Remove-Item "$wwwroot\*" -Recurse -Force }
else { New-Item -ItemType Directory -Path $wwwroot | Out-Null }
Copy-Item "$dist\*" $wwwroot -Recurse -Force

Write-Host "Done. wwwroot updated." -ForegroundColor Green
