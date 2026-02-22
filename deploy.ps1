# Fungsi file: Menjalankan otomasi deployment aplikasi melalui skrip PowerShell.
# ============================================================
# deploy.ps1 - Script deployment Cashflowpoly Analytics Platform
# ============================================================
# Penggunaan:
#   .\deploy.ps1               -> deploy production
#   .\deploy.ps1 -Mode dev     -> deploy development
#   .\deploy.ps1 -Mode dev-watch -> deploy development + auto-redeploy
#   .\deploy.ps1 -Mode status  -> cek status container
#   .\deploy.ps1 -Mode logs    -> tail logs
#   .\deploy.ps1 -Mode down    -> stop semua
#   .\deploy.ps1 -Mode restart -> restart semua
# ============================================================

param(
    [ValidateSet("prod", "dev", "dev-watch", "status", "logs", "down", "restart")]
    [string]$Mode = "prod"
)

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

function Write-Log   { param($msg) Write-Host "[OK] $msg" -ForegroundColor Green }
function Write-Warn  { param($msg) Write-Host "[!]  $msg" -ForegroundColor Yellow }
function Write-Err   { param($msg) Write-Host "[X]  $msg" -ForegroundColor Red }
function Write-Info  { param($msg) Write-Host "[->] $msg" -ForegroundColor Cyan }

function Invoke-ComposeWithOrphanSuppressed {
    param([Parameter(Mandatory = $true)][scriptblock]$Command)

    $previousIgnoreOrphans = $null
    $hadIgnoreOrphans = Test-Path Env:COMPOSE_IGNORE_ORPHANS
    if ($hadIgnoreOrphans) {
        $previousIgnoreOrphans = $env:COMPOSE_IGNORE_ORPHANS
    }

    $env:COMPOSE_IGNORE_ORPHANS = "true"
    try {
        & $Command
    } finally {
        if ($hadIgnoreOrphans) {
            $env:COMPOSE_IGNORE_ORPHANS = $previousIgnoreOrphans
        } else {
            Remove-Item Env:COMPOSE_IGNORE_ORPHANS -ErrorAction SilentlyContinue
        }
    }
}

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

function Get-EnvFileValue {
    param([Parameter(Mandatory = $true)][string]$Key)

    if (-not (Test-Path ".env")) {
        return $null
    }

    $pattern = "^\s*$([Regex]::Escape($Key))\s*=\s*(.*)$"
    foreach ($line in Get-Content ".env") {
        if ($line -match $pattern) {
            return $Matches[1].Trim().Trim("'`"")
        }
    }

    return $null
}

function Test-TunnelProfileEnabled {
    $token = if (-not [string]::IsNullOrWhiteSpace($env:CLOUDFLARE_TUNNEL_TOKEN)) {
        $env:CLOUDFLARE_TUNNEL_TOKEN
    } else {
        Get-EnvFileValue -Key "CLOUDFLARE_TUNNEL_TOKEN"
    }

    return -not [string]::IsNullOrWhiteSpace($token) -and -not $token.StartsWith("GANTI_", [StringComparison]::OrdinalIgnoreCase)
}

function Get-ProductionComposeArgs {
    $args = @("-f", "docker-compose.yml", "-f", "docker-compose.prod.yml")
    if (Test-TunnelProfileEnabled) {
        $args += @("--profile", "tunnel")
    }

    return $args
}

function Get-HttpStatusCode {
    param([Parameter(Mandatory = $true)][string]$Url)

    try {
        $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -MaximumRedirection 0 -TimeoutSec 20
        return [int]$response.StatusCode
    } catch {
        if ($_.Exception.Response -and $_.Exception.Response.StatusCode) {
            return [int]$_.Exception.Response.StatusCode.value__
        }

        return -1
    }
}

function Invoke-SmokeChecks {
    param([switch]$Production)

    $checks = if ($Production) {
        @(
            @{ Name = "Health"; Url = "http://localhost/health"; Allowed = @(200) },
            @{ Name = "Health Live"; Url = "http://localhost/health/live"; Allowed = @(200) },
            @{ Name = "Health Ready"; Url = "http://localhost/health/ready"; Allowed = @(200) },
            @{ Name = "UI CSS Site"; Url = "http://localhost/css/site.css"; Allowed = @(200) },
            @{ Name = "UI CSS Tailwind"; Url = "http://localhost/css/tailwind.css"; Allowed = @(200) },
            @{ Name = "Swagger"; Url = "http://localhost/swagger/index.html"; Allowed = @(200) }
        )
    } else {
        @(
            @{ Name = "UI Health Ready"; Url = "http://localhost:5203/health/ready"; Allowed = @(200) },
            @{ Name = "API Health Ready"; Url = "http://localhost:5041/health/ready"; Allowed = @(200) },
            @{ Name = "UI CSS Site"; Url = "http://localhost:5203/css/site.css"; Allowed = @(200) },
            @{ Name = "UI CSS Tailwind"; Url = "http://localhost:5203/css/tailwind.css"; Allowed = @(200) },
            @{ Name = "Swagger"; Url = "http://localhost:5041/swagger/index.html"; Allowed = @(200) }
        )
    }

    Write-Info "Menjalankan smoke checks..."
    foreach ($check in $checks) {
        $status = Get-HttpStatusCode -Url $check.Url
        if ($check.Allowed -contains $status) {
            Write-Log "$($check.Name): $status"
            continue
        }

        Write-Err "$($check.Name) gagal. URL: $($check.Url), status: $status"
        exit 1
    }
}

function Validate-ComposeFiles {
    param([switch]$Production)

    if ($Production) {
        $prodComposeArgs = Get-ProductionComposeArgs
        docker compose @prodComposeArgs config | Out-Null
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

function Validate-ComposeWatch {
    docker compose -f docker-compose.yml -f docker-compose.watch.yml config | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Err "Validasi compose development watch gagal."
        exit 1
    }

    Write-Log "Compose development watch tervalidasi."
}

function Test-ComposeWatchSupport {
    docker compose watch --help 2>$null | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Err "Docker Compose watch belum tersedia. Perbarui Docker Compose ke versi terbaru."
        exit 1
    }
}

function Deploy-Production {
    Write-Info "Deploying Cashflowpoly (PRODUCTION)..."
    Test-Dependencies
    Test-EnvFile
    Validate-ComposeFiles -Production
    $prodComposeArgs = Get-ProductionComposeArgs
    $ghcrRegistry = if ($env:GHCR_REGISTRY) { $env:GHCR_REGISTRY } else { "ghcr.io" }

    if (Test-TunnelProfileEnabled) {
        Write-Info "CLOUDFLARE_TUNNEL_TOKEN ditemukan. Tunnel profile akan dijalankan."
    } else {
        Write-Warn "CLOUDFLARE_TUNNEL_TOKEN tidak ditemukan. Deploy tetap berjalan tanpa profile tunnel."
    }

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
    docker compose @prodComposeArgs pull api ui
    if ($LASTEXITCODE -ne 0) {
        Write-Err "Pull image production gagal."
        exit 1
    }

    Write-Info "Starting semua service tanpa build ulang..."
    docker compose @prodComposeArgs up -d --no-build
    if ($LASTEXITCODE -ne 0) {
        Write-Err "Deploy production gagal."
        exit 1
    }

    Write-Info "Menunggu health check (15 detik)..."
    Start-Sleep -Seconds 15

    Invoke-SmokeChecks -Production
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

    Invoke-ComposeWithOrphanSuppressed { docker compose up -d --build }
    if ($LASTEXITCODE -ne 0) {
        Write-Err "Build/deploy development gagal."
        exit 1
    }

    Write-Info "Menunggu health check (15 detik)..."
    Start-Sleep -Seconds 15

    Invoke-SmokeChecks
    Show-Status
    Write-Host ""
    Write-Log "Deploy development selesai."
    Write-Info "Akses UI:      http://localhost:5203"
    Write-Info "Akses API:     http://localhost:5041"
    Write-Info "Swagger:       http://localhost:5041/swagger"
}

function Deploy-DevWatch {
    Write-Info "Deploying Cashflowpoly (DEVELOPMENT + WATCH)..."
    Test-Dependencies
    Test-ComposeWatchSupport
    $watchComposeArgs = @("-f", "docker-compose.yml", "-f", "docker-compose.watch.yml")

    if (-not (Test-Path ".env") -and (Test-Path ".env.example")) {
        Copy-Item ".env.example" ".env"
        Write-Warn "File .env dibuat dari template. Edit jika perlu."
    }

    Validate-ComposeWatch

    Invoke-ComposeWithOrphanSuppressed { docker compose @watchComposeArgs up -d --build }
    if ($LASTEXITCODE -ne 0) {
        Write-Err "Build/deploy development watch gagal."
        exit 1
    }

    Write-Info "Menunggu health check (15 detik)..."
    Start-Sleep -Seconds 15

    Invoke-SmokeChecks
    Show-Status
    Write-Host ""
    Write-Log "Mode development watch aktif."
    Write-Info "Akses UI:      http://localhost:5203"
    Write-Info "Akses API:     http://localhost:5041"
    Write-Info "Tekan Ctrl+C untuk berhenti mode watch."

    Invoke-ComposeWithOrphanSuppressed { docker compose @watchComposeArgs watch }
}

function Show-Status {
    $prodComposeArgs = Get-ProductionComposeArgs
    Write-Host ""
    Write-Info "Status Container:"
    Write-Host ("=" * 55)
    docker compose @prodComposeArgs ps 2>$null
    if ($LASTEXITCODE -ne 0) {
        docker compose ps
    }
    Write-Host ("=" * 55)
}

function Show-Logs {
    $prodComposeArgs = Get-ProductionComposeArgs
    docker compose @prodComposeArgs logs -f --tail=100 2>$null
    if ($LASTEXITCODE -ne 0) {
        docker compose logs -f --tail=100
    }
}

function Stop-All {
    $prodComposeArgs = Get-ProductionComposeArgs
    Write-Warn "Menghentikan semua service..."
    docker compose @prodComposeArgs down 2>$null
    if ($LASTEXITCODE -ne 0) {
        docker compose down
    }
    Write-Log "Semua service dihentikan."
}

function Restart-All {
    $prodComposeArgs = Get-ProductionComposeArgs
    Write-Info "Restart semua service..."
    docker compose @prodComposeArgs restart 2>$null
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
    "dev-watch" { Deploy-DevWatch }
    "status"  { Show-Status }
    "logs"    { Show-Logs }
    "down"    { Stop-All }
    "restart" { Restart-All }
}
