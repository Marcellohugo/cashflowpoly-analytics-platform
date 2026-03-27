# Fungsi file: Menjalankan rebuild dan redeploy production lokal tanpa registry image eksternal.
[CmdletBinding()]
param(
    [string]$EnvFile = ".env.prod",
    [switch]$WithoutTunnel
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Invoke-Step {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name,
        [Parameter(Mandatory = $true)]
        [scriptblock]$Action
    )

    Write-Host "==> $Name" -ForegroundColor Cyan
    & $Action
    if ($LASTEXITCODE -ne 0) {
        throw "Langkah gagal: $Name"
    }
}

function Get-EnvValue {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,
        [Parameter(Mandatory = $true)]
        [string]$Name
    )

    foreach ($line in [System.IO.File]::ReadAllLines($Path)) {
        if ([string]::IsNullOrWhiteSpace($line)) {
            continue
        }

        $trimmed = $line.Trim()
        if ($trimmed.StartsWith("#")) {
            continue
        }

        if ($trimmed -like "$Name=*") {
            return $trimmed.Substring($Name.Length + 1).Trim()
        }
    }

    return $null
}

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$resolvedEnvFile = if ([System.IO.Path]::IsPathRooted($EnvFile)) { $EnvFile } else { Join-Path $repoRoot $EnvFile }

if (-not (Test-Path $resolvedEnvFile -PathType Leaf)) {
    throw "File environment tidak ditemukan: $resolvedEnvFile"
}

$tunnelToken = Get-EnvValue -Path $resolvedEnvFile -Name "CLOUDFLARE_TUNNEL_TOKEN"
$includeTunnel = -not $WithoutTunnel
if ($includeTunnel -and [string]::IsNullOrWhiteSpace($tunnelToken)) {
    Write-Host "CLOUDFLARE_TUNNEL_TOKEN kosong, deploy dilanjutkan tanpa cloudflared." -ForegroundColor Yellow
    $includeTunnel = $false
}

$composeArgs = @(
    "--env-file", $resolvedEnvFile,
    "-f", "docker-compose.yml",
    "-f", "docker-compose.prod.yml"
)

if ($includeTunnel) {
    $composeArgs += @("--profile", "tunnel")
}

$services = @("db", "api", "ui", "nginx")
if ($includeTunnel) {
    $services += "cloudflared"
}

Push-Location $repoRoot
try {
    Invoke-Step "Build dan jalankan service production" {
        docker compose @composeArgs up -d --build @services
    }

    Invoke-Step "Status service production" {
        docker compose @composeArgs ps
    }
}
finally {
    Pop-Location
}
