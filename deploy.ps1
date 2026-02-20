# Fungsi file: Menjalankan otomasi deployment aplikasi melalui skrip PowerShell.
# ============================================================
# deploy.ps1 - Script deployment Cashflowpoly Analytics Platform
# ============================================================
# Penggunaan:
#   .\deploy.ps1               -> deploy production
#   .\deploy.ps1 -Mode dev     -> deploy development
#   .\deploy.ps1 -Mode status  -> cek status container
#   .\deploy.ps1 -Mode logs    -> tail logs
#   .\deploy.ps1 -Mode down    -> stop semua
#   .\deploy.ps1 -Mode restart -> restart semua
# ============================================================

param(
    [ValidateSet("prod", "dev", "status", "logs", "down", "restart")]
    [string]$Mode = "prod"
)

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

function Write-Log   { param($msg) Write-Host "[OK] $msg" -ForegroundColor Green }
function Write-Warn  { param($msg) Write-Host "[!]  $msg" -ForegroundColor Yellow }
function Write-Err   { param($msg) Write-Host "[X]  $msg" -ForegroundColor Red }
function Write-Info  { param($msg) Write-Host "[->] $msg" -ForegroundColor Cyan }

function Test-Dependencies {
    if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
        Write-Err "Docker tidak ditemukan. Install Docker Desktop terlebih dahulu."
        exit 1
    }

    docker compose version | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Err "Docker Compose V2 tidak tersedia."
        exit 1
    }

    Write-Log "Docker & Docker Compose tersedia."
}

function Test-EnvFile {
    if (-not (Test-Path ".env")) {
        Write-Warn "File .env belum ada."
        if (Test-Path ".env.example") {
            Write-Info "Membuat .env dari .env.example..."
            Copy-Item ".env.example" ".env"
            Write-Warn "PENTING: Edit file .env dan ganti nilai default sebelum deploy."
            Write-Warn "  notepad .env"
            exit 1
        }

        Write-Err ".env.example tidak ditemukan."
        exit 1
    }

    Write-Log "File .env ditemukan."

    $envContent = Get-Content ".env" -Raw
    if ($envContent -match "GANTI_DENGAN_PASSWORD_KUAT" -or $envContent -match "GANTI_DENGAN_SIGNING_KEY") {
        Write-Err "Variabel .env masih menggunakan nilai default."
        Write-Err "Edit .env dan ganti POSTGRES_PASSWORD, JWT_SIGNING_KEY, dll."
        exit 1
    }

    Write-Log "Variabel .env tervalidasi."
}

function Validate-ComposeFiles {
    param([switch]$Production)

    if ($Production) {
        docker compose -f docker-compose.yml -f docker-compose.prod.yml config | Out-Null
        if ($LASTEXITCODE -ne 0) {
            Write-Err "Validasi compose production gagal."
            exit 1
        }

        Write-Log "Compose production tervalidasi."
        return
    }

    docker compose config | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Err "Validasi compose development gagal."
        exit 1
    }

    Write-Log "Compose development tervalidasi."
}

function Deploy-Production {
    Write-Info "Deploying Cashflowpoly (PRODUCTION)..."
    Test-Dependencies
    Test-EnvFile
    Validate-ComposeFiles -Production
    $ghcrRegistry = if ($env:GHCR_REGISTRY) { $env:GHCR_REGISTRY } else { "ghcr.io" }

    if ($env:GHCR_USERNAME -and $env:GHCR_TOKEN) {
        Write-Info "Login ke GHCR ($ghcrRegistry)..."
        $env:GHCR_TOKEN | docker login $ghcrRegistry -u $env:GHCR_USERNAME --password-stdin | Out-Null
        if ($LASTEXITCODE -ne 0) {
            Write-Err "Login GHCR gagal."
            exit 1
        }
    } else {
        Write-Warn "GHCR_USERNAME/GHCR_TOKEN tidak diset. Asumsi image publik atau host sudah login."
    }

    Write-Info "Pull image production..."
    docker compose -f docker-compose.yml -f docker-compose.prod.yml pull api ui
    if ($LASTEXITCODE -ne 0) {
        Write-Err "Pull image production gagal."
        exit 1
    }

    Write-Info "Starting semua service tanpa build ulang..."
    docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --no-build
    if ($LASTEXITCODE -ne 0) {
        Write-Err "Deploy production gagal."
        exit 1
    }

    Write-Info "Menunggu health check (15 detik)..."
    Start-Sleep -Seconds 15

    Show-Status
    Write-Host ""
    Write-Log "Deploy production selesai."
    Write-Info "Akses dashboard: http://localhost"
    Write-Info "Akses API:       http://localhost/api/"
}

function Deploy-Dev {
    Write-Info "Deploying Cashflowpoly (DEVELOPMENT)..."
    Test-Dependencies

    if (-not (Test-Path ".env") -and (Test-Path ".env.example")) {
        Copy-Item ".env.example" ".env"
        Write-Warn "File .env dibuat dari template."
    }

    Validate-ComposeFiles

    docker compose up -d --build
    if ($LASTEXITCODE -ne 0) {
        Write-Err "Build/deploy development gagal."
        exit 1
    }

    Write-Info "Menunggu health check (15 detik)..."
    Start-Sleep -Seconds 15

    Show-Status
    Write-Host ""
    Write-Log "Deploy development selesai."
    Write-Info "Akses UI:      http://localhost:5203"
    Write-Info "Akses API:     http://localhost:5041"
    Write-Info "Swagger:       http://localhost:5041/swagger"
}

function Show-Status {
    Write-Host ""
    Write-Info "Status Container:"
    Write-Host ("=" * 55)
    docker compose -f docker-compose.yml -f docker-compose.prod.yml ps 2>$null
    if ($LASTEXITCODE -ne 0) {
        docker compose ps
    }
    Write-Host ("=" * 55)
}

function Show-Logs {
    docker compose -f docker-compose.yml -f docker-compose.prod.yml logs -f --tail=100 2>$null
    if ($LASTEXITCODE -ne 0) {
        docker compose logs -f --tail=100
    }
}

function Stop-All {
    Write-Warn "Menghentikan semua service..."
    docker compose -f docker-compose.yml -f docker-compose.prod.yml down 2>$null
    if ($LASTEXITCODE -ne 0) {
        docker compose down
    }
    Write-Log "Semua service dihentikan."
}

function Restart-All {
    Write-Info "Restart semua service..."
    docker compose -f docker-compose.yml -f docker-compose.prod.yml restart 2>$null
    if ($LASTEXITCODE -ne 0) {
        docker compose restart
    }
    Start-Sleep -Seconds 10
    Show-Status
    Write-Log "Restart selesai."
}

switch ($Mode) {
    "prod"    { Deploy-Production }
    "dev"     { Deploy-Dev }
    "status"  { Show-Status }
    "logs"    { Show-Logs }
    "down"    { Stop-All }
    "restart" { Restart-All }
}
