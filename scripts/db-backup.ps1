param(
    [string]$ContainerName = "cashflowpoly-db",
    [string]$Database = "cashflowpoly",
    [string]$DbUser = "cashflowpoly",
    [string]$OutputDir = "backups",
    [string]$OutputFile = ""
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

if ([string]::IsNullOrWhiteSpace($OutputFile)) {
    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $OutputFile = Join-Path $OutputDir "cashflowpoly_$timestamp.sql"
}

Write-Host "Backup database '$Database' dari container '$ContainerName' ke '$OutputFile'..."
docker exec $ContainerName sh -c "pg_dump -U $DbUser -d $Database" | Out-File -FilePath $OutputFile -Encoding utf8

if ($LASTEXITCODE -ne 0) {
    throw "Backup gagal. Periksa container/database/user."
}

Write-Host "Backup selesai."
