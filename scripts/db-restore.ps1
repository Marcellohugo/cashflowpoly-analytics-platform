param(
    [Parameter(Mandatory = $true)]
    [string]$InputFile,
    [string]$ContainerName = "cashflowpoly-db",
    [string]$Database = "cashflowpoly",
    [string]$DbUser = "cashflowpoly",
    [switch]$ResetPublicSchema
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $InputFile)) {
    throw "File backup tidak ditemukan: $InputFile"
}

if ($ResetPublicSchema) {
    Write-Host "Reset schema public pada database '$Database'..."
    docker exec $ContainerName sh -c "psql -v ON_ERROR_STOP=1 -U $DbUser -d $Database -c \"drop schema if exists public cascade; create schema public;\""
    if ($LASTEXITCODE -ne 0) {
        throw "Reset schema gagal."
    }
}

Write-Host "Restore database '$Database' dari '$InputFile'..."
Get-Content -Raw $InputFile | docker exec -i $ContainerName sh -c "psql -v ON_ERROR_STOP=1 -U $DbUser -d $Database"

if ($LASTEXITCODE -ne 0) {
    throw "Restore gagal. Periksa file backup dan koneksi database."
}

Write-Host "Restore selesai."
