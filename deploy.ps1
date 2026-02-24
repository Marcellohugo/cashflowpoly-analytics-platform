# Fungsi file: Menjalankan otomasi deployment aplikasi melalui skrip PowerShell.
# ============================================================
# deploy.ps1 - Script deployment Cashflowpoly Analytics Platform
# ============================================================
# Penggunaan:
#   .\deploy.ps1               -> deploy production
#   .\deploy.ps1 -Mode dev     -> deploy development
#   .\deploy.ps1 -Mode dev-watch -> deploy development + auto-redeploy
#   .\deploy.ps1 -Mode prod -EnvFile .env.prod -> pakai env file spesifik
#   .\deploy.ps1 -Mode status  -> cek status container
#   .\deploy.ps1 -Mode logs    -> tail logs
#   .\deploy.ps1 -Mode down    -> stop semua
#   .\deploy.ps1 -Mode restart -> restart semua
# ============================================================

param(
    [ValidateSet("prod", "dev", "dev-watch", "status", "logs", "down", "restart")]
    [string]$Mode = "prod",
    [string]$EnvFile
)

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

function Write-Log   { param($msg) Write-Host "[OK] $msg" -ForegroundColor Green }
function Write-Warn  { param($msg) Write-Host "[!]  $msg" -ForegroundColor Yellow }
function Write-Err   { param($msg) Write-Host "[X]  $msg" -ForegroundColor Red }
function Write-Info  { param($msg) Write-Host "[->] $msg" -ForegroundColor Cyan }

function Get-DefaultEnvFileForMode {
    param([Parameter(Mandatory = $true)][string]$SelectedMode)

    if ($SelectedMode -in @("dev", "dev-watch")) {
        return ".env.dev"
    }

    return ".env.prod"
}

function Resolve-EnvFilePath {
    if (-not [string]::IsNullOrWhiteSpace($EnvFile)) {
        return $EnvFile
    }

    $modeDefault = Get-DefaultEnvFileForMode -SelectedMode $Mode
    if (Test-Path $modeDefault) {
        return $modeDefault
    }

    if (Test-Path ".env") {
        return ".env"
    }

    return $modeDefault
}

function Get-TemplateEnvFileForTarget {
    param([Parameter(Mandatory = $true)][string]$TargetEnvFile)

    $normalized = $TargetEnvFile.ToLowerInvariant()
    if ($normalized.EndsWith(".env.dev") -and (Test-Path ".env.dev.example")) {
        return ".env.dev.example"
    }

    if ($normalized.EndsWith(".env.prod") -and (Test-Path ".env.prod.example")) {
        return ".env.prod.example"
    }

    if (Test-Path ".env.example") {
        return ".env.example"
    }

    return $null
}

$script:ComposeEnvFile = Resolve-EnvFilePath
Write-Info "Menggunakan env file: $script:ComposeEnvFile"

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
    if (-not (Test-Path $script:ComposeEnvFile)) {
        Write-Warn "File $script:ComposeEnvFile belum ada."
        $templateEnvFile = Get-TemplateEnvFileForTarget -TargetEnvFile $script:ComposeEnvFile
        if ($templateEnvFile) {
            Write-Info "Membuat $script:ComposeEnvFile dari $templateEnvFile..."
            Copy-Item $templateEnvFile $script:ComposeEnvFile
            Write-Warn "PENTING: Edit file $script:ComposeEnvFile dan ganti nilai default sebelum deploy."
            Write-Warn "  notepad $script:ComposeEnvFile"
            exit 1
        }

        Write-Err "Template env file tidak ditemukan."
        exit 1
    }

    Write-Log "File $script:ComposeEnvFile ditemukan."

    $envContent = Get-Content $script:ComposeEnvFile -Raw
    if ($envContent -match "GANTI_DENGAN_PASSWORD_KUAT" -or $envContent -match "GANTI_DENGAN_SIGNING_KEY") {
        Write-Err "Variabel $script:ComposeEnvFile masih menggunakan nilai default."
        Write-Err "Edit $script:ComposeEnvFile dan ganti POSTGRES_PASSWORD, JWT_SIGNING_KEY, dll."
        exit 1
    }

    Write-Log "Variabel $script:ComposeEnvFile tervalidasi."
}

function Get-EnvFileValue {
    param([Parameter(Mandatory = $true)][string]$Key)

    if (-not (Test-Path $script:ComposeEnvFile)) {
        return $null
    }

    $pattern = "^\s*$([Regex]::Escape($Key))\s*=\s*(.*)$"
    foreach ($line in Get-Content $script:ComposeEnvFile) {
        if ($line -match $pattern) {
            return $Matches[1].Trim().Trim("'`"")
        }
    }

    return $null
}

function Get-ComposeEnvArgs {
    return @("--env-file", $script:ComposeEnvFile)
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

    $envArgs = Get-ComposeEnvArgs

    if ($Production) {
        $prodComposeArgs = Get-ProductionComposeArgs
        docker compose @envArgs @prodComposeArgs config | Out-Null
        if ($LASTEXITCODE -ne 0) {
            Write-Err "Validasi compose production gagal."
            exit 1
        }

        Write-Log "Compose production tervalidasi."
        return
    }

    docker compose @envArgs config | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Err "Validasi compose development gagal."
        exit 1
    }

    Write-Log "Compose development tervalidasi."
}

function Validate-ComposeWatch {
    $envArgs = Get-ComposeEnvArgs
    docker compose @envArgs -f docker-compose.yml -f docker-compose.watch.yml config | Out-Null
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
    $envArgs = Get-ComposeEnvArgs
    $prodComposeArgs = Get-ProductionComposeArgs
    $ghcrRegistry = if ($env:GHCR_REGISTRY) { $env:GHCR_REGISTRY } else {
        $registryFromFile = Get-EnvFileValue -Key "GHCR_REGISTRY"
        if ($registryFromFile) { $registryFromFile } else { "ghcr.io" }
    }
    $ghcrUsername = if ($env:GHCR_USERNAME) { $env:GHCR_USERNAME } else { Get-EnvFileValue -Key "GHCR_USERNAME" }
    $ghcrToken = if ($env:GHCR_TOKEN) { $env:GHCR_TOKEN } else { Get-EnvFileValue -Key "GHCR_TOKEN" }

    if (Test-TunnelProfileEnabled) {
        Write-Info "CLOUDFLARE_TUNNEL_TOKEN ditemukan. Tunnel profile akan dijalankan."
    } else {
        Write-Warn "CLOUDFLARE_TUNNEL_TOKEN tidak ditemukan. Deploy tetap berjalan tanpa profile tunnel."
    }

    if ($ghcrUsername -and $ghcrToken) {
        Write-Info "Login ke GHCR ($ghcrRegistry)..."
        $ghcrToken | docker login $ghcrRegistry -u $ghcrUsername --password-stdin | Out-Null
        if ($LASTEXITCODE -ne 0) {
            Write-Err "Login GHCR gagal."
            exit 1
        }
    } else {
        Write-Warn "GHCR_USERNAME/GHCR_TOKEN tidak diset. Asumsi image publik atau host sudah login."
    }

    Write-Info "Pull image production..."
    docker compose @envArgs @prodComposeArgs pull api ui
    if ($LASTEXITCODE -ne 0) {
        Write-Err "Pull image production gagal."
        exit 1
    }

    Write-Info "Memastikan seluruh service production berjalan..."
    docker compose @envArgs @prodComposeArgs up -d --no-build
    if ($LASTEXITCODE -ne 0) {
        Write-Err "Start service production gagal."
        exit 1
    }

    Write-Info "Redeploy service production (api + ui) dengan force recreate..."
    docker compose @envArgs @prodComposeArgs up -d --no-build --force-recreate --no-deps api ui
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
    Test-EnvFile
    $envArgs = Get-ComposeEnvArgs

    Validate-ComposeFiles

    Invoke-ComposeWithOrphanSuppressed { docker compose @envArgs up -d --build }
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
    Test-EnvFile
    Test-ComposeWatchSupport
    $envArgs = Get-ComposeEnvArgs
    $watchComposeArgs = @("-f", "docker-compose.yml", "-f", "docker-compose.watch.yml")

    Validate-ComposeWatch

    Invoke-ComposeWithOrphanSuppressed { docker compose @envArgs @watchComposeArgs up -d --build }
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

    Invoke-ComposeWithOrphanSuppressed { docker compose @envArgs @watchComposeArgs watch }
}

function Show-Status {
    $envArgs = Get-ComposeEnvArgs
    $prodComposeArgs = Get-ProductionComposeArgs
    Write-Host ""
    Write-Info "Status Container:"
    Write-Host ("=" * 55)
    docker compose @envArgs @prodComposeArgs ps 2>$null
    if ($LASTEXITCODE -ne 0) {
        docker compose @envArgs ps
    }
    Write-Host ("=" * 55)
}

function Show-Logs {
    $envArgs = Get-ComposeEnvArgs
    $prodComposeArgs = Get-ProductionComposeArgs
    docker compose @envArgs @prodComposeArgs logs -f --tail=100 2>$null
    if ($LASTEXITCODE -ne 0) {
        docker compose @envArgs logs -f --tail=100
    }
}

function Stop-All {
    $envArgs = Get-ComposeEnvArgs
    $prodComposeArgs = Get-ProductionComposeArgs
    Write-Warn "Menghentikan semua service..."
    docker compose @envArgs @prodComposeArgs down 2>$null
    if ($LASTEXITCODE -ne 0) {
        docker compose @envArgs down
    }
    Write-Log "Semua service dihentikan."
}

function Restart-All {
    $envArgs = Get-ComposeEnvArgs
    $prodComposeArgs = Get-ProductionComposeArgs
    Write-Info "Restart semua service..."
    docker compose @envArgs @prodComposeArgs restart 2>$null
    if ($LASTEXITCODE -ne 0) {
        docker compose @envArgs restart
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
