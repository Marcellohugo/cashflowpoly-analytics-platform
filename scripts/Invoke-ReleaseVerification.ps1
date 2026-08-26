# Fungsi file: Menjalankan seluruh gerbang verifikasi lokal sebelum perubahan boleh dirilis.
[CmdletBinding()]
param(
    [switch]$SkipBrowser,
    [switch]$SkipDockerBuild,
    [switch]$SkipPerformance,
    [string]$EnvironmentFile = "config/env/.env.dev"
)

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$e2eRoot = Join-Path $repositoryRoot "tests/e2e"
$resolvedEnvironmentFile = Join-Path $repositoryRoot $EnvironmentFile
$composeFiles = @(
    "-f", (Join-Path $repositoryRoot "infra/docker/docker-compose.yml"),
    "-f", (Join-Path $repositoryRoot "infra/docker/docker-compose.watch.yml")
)

function Invoke-Checked {
    param(
        [Parameter(Mandatory)][string]$Label,
        [Parameter(Mandatory)][scriptblock]$Command
    )

    Write-Host "`n=== $Label ===" -ForegroundColor Cyan
    & $Command
    if ($LASTEXITCODE -ne 0) {
        throw "$Label gagal dengan exit code $LASTEXITCODE."
    }
}

foreach ($tool in @("dotnet", "node", "npm", "docker")) {
    if (-not (Get-Command $tool -ErrorAction SilentlyContinue)) {
        throw "Tool wajib tidak tersedia: $tool"
    }
}

if (-not (Test-Path -LiteralPath $resolvedEnvironmentFile -PathType Leaf)) {
    throw "Environment development tidak ditemukan: $resolvedEnvironmentFile"
}

Push-Location $repositoryRoot
try {
    Invoke-Checked "Restore .NET" { dotnet restore Cashflowpoly.sln }
    Invoke-Checked "Build Release tanpa warning" { dotnet build Cashflowpoly.sln -c Release --no-restore /warnaserror }
    Invoke-Checked "Unit, integration PostgreSQL, dan contract test" { dotnet test Cashflowpoly.sln -c Release --no-build --no-restore --filter "Category!=Performance" }

    Invoke-Checked "Audit kerentanan NuGet" {
        dotnet list Cashflowpoly.sln package --vulnerable --include-transitive --format json | Out-File -LiteralPath (Join-Path $env:TEMP "cashflowpoly-nuget-audit.json") -Encoding utf8
    }
    $nugetAudit = Get-Content -LiteralPath (Join-Path $env:TEMP "cashflowpoly-nuget-audit.json") -Raw | ConvertFrom-Json
    $vulnerablePackages = @($nugetAudit.projects.frameworks.topLevelPackages + $nugetAudit.projects.frameworks.transitivePackages) |
        Where-Object { $_.vulnerabilities -and $_.vulnerabilities.Count -gt 0 }
    if ($vulnerablePackages.Count -gt 0) {
        throw "Audit NuGet menemukan $($vulnerablePackages.Count) package rentan."
    }

    Invoke-Checked "Install dependensi UI" {
        Push-Location (Join-Path $repositoryRoot "src/Cashflowpoly.Ui")
        try { npm ci --no-audit --no-fund } finally { Pop-Location }
    }
    Invoke-Checked "Audit kerentanan npm UI" {
        Push-Location (Join-Path $repositoryRoot "src/Cashflowpoly.Ui")
        try { npm audit --audit-level=high } finally { Pop-Location }
    }
    Invoke-Checked "Konsistensi dokumentasi dan Postman" { & (Join-Path $PSScriptRoot "Test-DocumentationConsistency.ps1") }

    if (-not $SkipPerformance) {
        Invoke-Checked "Target performa 100 akun, 20 sesi aktif, 20 pengguna bersamaan" {
            dotnet test tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj -c Release --no-build --no-restore --filter "Category=Performance"
        }
    }

    if (-not $SkipBrowser) {
        Invoke-Checked "Validasi Docker Compose development" {
            docker compose --env-file $resolvedEnvironmentFile @composeFiles config -q
        }
        $env:DATABASE_MIGRATIONS_SEED_SIMULATION = "true"
        Invoke-Checked "Migrasi dan Seed 2 idempoten" {
            docker compose --env-file $resolvedEnvironmentFile @composeFiles run --rm api `
                dotnet run --project src/Cashflowpoly.Api/Cashflowpoly.Api.csproj --no-launch-profile -- --migrate-only
        }
        Remove-Item Env:DATABASE_MIGRATIONS_SEED_SIMULATION -ErrorAction SilentlyContinue
        Invoke-Checked "Jalankan aplikasi lokal" {
            docker compose --env-file $resolvedEnvironmentFile @composeFiles up -d db api ui
        }

        $healthUri = "http://localhost:5203/health/ready"
        $healthy = $false
        for ($attempt = 1; $attempt -le 30; $attempt += 1) {
            try {
                $response = Invoke-WebRequest -Uri $healthUri -UseBasicParsing -TimeoutSec 3
                if ($response.StatusCode -eq 200) {
                    $healthy = $true
                    break
                }
            }
            catch {
                Start-Sleep -Seconds 2
            }
        }
        if (-not $healthy) {
            throw "UI lokal tidak sehat setelah 60 detik: $healthUri"
        }

        Invoke-Checked "Install Playwright" {
            Push-Location $e2eRoot
            try { npm ci --no-audit --no-fund } finally { Pop-Location }
        }
        Invoke-Checked "Audit kerentanan npm E2E" {
            Push-Location $e2eRoot
            try { npm audit --audit-level=high } finally { Pop-Location }
        }
        Invoke-Checked "Install Chromium Playwright" {
            Push-Location $e2eRoot
            try { npx playwright install chromium } finally { Pop-Location }
        }
        Invoke-Checked "E2E Chromium desktop dan ponsel" {
            Push-Location $e2eRoot
            try { npm test } finally { Pop-Location }
        }
    }

    if (-not $SkipDockerBuild) {
        Invoke-Checked "Build image API" { docker build -f src/Cashflowpoly.Api/Dockerfile -t cashflowpoly-api:release-gate . }
        Invoke-Checked "Build image UI" { docker build -f src/Cashflowpoly.Ui/Dockerfile -t cashflowpoly-ui:release-gate . }
    }

    Write-Host "`nSeluruh gerbang verifikasi rilis lulus." -ForegroundColor Green
}
finally {
    Remove-Item Env:DATABASE_MIGRATIONS_SEED_SIMULATION -ErrorAction SilentlyContinue
    Pop-Location
}
