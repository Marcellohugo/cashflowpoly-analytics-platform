# Fungsi file: Memvalidasi rahasia, bootstrap akun, domain, TLS tunnel, dan konfigurasi Compose sebelum deployment production.
[CmdletBinding()]
param(
    [string]$EnvironmentFile = "config/env/.env.prod"
)

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$resolvedEnvironmentFile = Join-Path $repositoryRoot $EnvironmentFile

if (-not (Test-Path -LiteralPath $resolvedEnvironmentFile -PathType Leaf)) {
    throw "File environment production tidak ditemukan: $resolvedEnvironmentFile"
}

# Membaca dotenv tanpa mengeksekusi isinya agar nilai rahasia tidak menjadi perintah shell.
$settings = @{}
foreach ($line in Get-Content -LiteralPath $resolvedEnvironmentFile) {
    $trimmed = $line.Trim()
    if ($trimmed.Length -eq 0 -or $trimmed.StartsWith("#") -or -not $trimmed.Contains("=")) {
        continue
    }

    $name, $value = $trimmed.Split("=", 2)
    $settings[$name.Trim()] = $value.Trim()
}

# Menolak nilai kosong dan placeholder yang aman untuk development tetapi berbahaya di production.
$required = @("POSTGRES_PASSWORD", "JWT_SIGNING_KEY", "DOMAIN", "CLOUDFLARE_TUNNEL_TOKEN")
foreach ($name in $required) {
    if (-not $settings.ContainsKey($name) -or [string]::IsNullOrWhiteSpace($settings[$name])) {
        throw "$name wajib diisi untuk deployment production."
    }
}

$placeholderPattern = "(?i)GANTI_|change-me|dev-local|example|placeholder"
if ($settings["POSTGRES_PASSWORD"] -eq "cashflowpoly" -or $settings["POSTGRES_PASSWORD"] -match $placeholderPattern) {
    throw "POSTGRES_PASSWORD masih menggunakan nilai development/placeholder."
}
if ($settings["POSTGRES_PASSWORD"].Length -lt 16) {
    throw "POSTGRES_PASSWORD minimal 16 karakter untuk production."
}
if ($settings["JWT_SIGNING_KEY"] -match $placeholderPattern -or $settings["JWT_SIGNING_KEY"].Length -lt 32) {
    throw "JWT_SIGNING_KEY wajib berupa rahasia non-placeholder minimal 32 karakter."
}
if ($settings["DOMAIN"] -match "(?i)^(localhost|127\.0\.0\.1)$") {
    throw "DOMAIN production tidak boleh localhost."
}

function Test-BootstrapCredentialPair {
    param(
        [string]$Role,
        [string]$UsernameKey,
        [string]$PasswordKey
    )

    $username = if ($settings.ContainsKey($UsernameKey)) { $settings[$UsernameKey] } else { "" }
    $password = if ($settings.ContainsKey($PasswordKey)) { $settings[$PasswordKey] } else { "" }
    $hasUsername = -not [string]::IsNullOrWhiteSpace($username)
    $hasPassword = -not [string]::IsNullOrWhiteSpace($password)

    if ($hasUsername -xor $hasPassword) {
        throw "Bootstrap $Role harus mengisi $UsernameKey dan $PasswordKey sebagai pasangan."
    }
    if (-not $hasUsername) {
        return $false
    }
    if ($password.Length -lt 12 -or [Text.Encoding]::UTF8.GetByteCount($password) -gt 72) {
        throw "Password bootstrap $Role harus minimal 12 karakter dan maksimal 72 byte UTF-8."
    }

    return $true
}

$bootstrapEnabled = "false"
if ($settings.ContainsKey("AUTH_BOOTSTRAP_SEED_DEFAULT_USERS")) {
    $bootstrapEnabled = $settings["AUTH_BOOTSTRAP_SEED_DEFAULT_USERS"]
}
if ($bootstrapEnabled -notmatch "(?i)^(true|false)$") {
    throw "AUTH_BOOTSTRAP_SEED_DEFAULT_USERS harus bernilai true atau false."
}

if ($bootstrapEnabled -match "(?i)^true$") {
    $hasInstructorBootstrap = Test-BootstrapCredentialPair `
        -Role "Instruktur" `
        -UsernameKey "AUTH_BOOTSTRAP_INSTRUCTOR_USERNAME" `
        -PasswordKey "AUTH_BOOTSTRAP_INSTRUCTOR_PASSWORD"
    $hasPlayerBootstrap = Test-BootstrapCredentialPair `
        -Role "Player" `
        -UsernameKey "AUTH_BOOTSTRAP_PLAYER_USERNAME" `
        -PasswordKey "AUTH_BOOTSTRAP_PLAYER_PASSWORD"

    if (-not $hasInstructorBootstrap -and -not $hasPlayerBootstrap) {
        throw "AUTH_BOOTSTRAP_SEED_DEFAULT_USERS=true membutuhkan minimal satu pasangan credential bootstrap."
    }
}

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    throw "Docker CLI tidak tersedia untuk memvalidasi konfigurasi production."
}

# Meminta Docker Compose merender konfigurasi final; kegagalan interpolasi atau YAML menghentikan deployment.
& docker compose `
    --env-file $resolvedEnvironmentFile `
    -f (Join-Path $repositoryRoot "infra/docker/docker-compose.yml") `
    -f (Join-Path $repositoryRoot "infra/docker/docker-compose.prod.yml") `
    --profile tunnel `
    config -q
if ($LASTEXITCODE -ne 0) {
    throw "Konfigurasi Docker Compose production tidak valid."
}

Write-Output "Konfigurasi production valid: rahasia, bootstrap akun, domain, tunnel TLS, dan Docker Compose siap digunakan."
