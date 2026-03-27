# Fungsi file: Menjalankan verifikasi lokal pengganti pipeline CI build-test yang dihapus.
[CmdletBinding()]
param(
    [switch]$SkipIntegrationTests
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

function Set-Or-ClearEnvVar {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name,
        [AllowNull()]
        [string]$Value
    )

    if ($null -eq $Value) {
        Remove-Item "Env:$Name" -ErrorAction SilentlyContinue
        return
    }

    Set-Item "Env:$Name" $Value
}

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$previousConnectionString = $env:ConnectionStrings__Default

Push-Location $repoRoot
try {
    Invoke-Step "Restore API" { dotnet restore src/Cashflowpoly.Api/Cashflowpoly.Api.csproj }
    Invoke-Step "Restore UI" { dotnet restore src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj }
    Invoke-Step "Restore Tests" { dotnet restore tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj }

    Invoke-Step "Build API" { dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj -c Release --no-restore /warnaserror }
    Invoke-Step "Build UI" { dotnet build src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj -c Release --no-restore /warnaserror }
    Invoke-Step "Build Tests" { dotnet build tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj -c Release --no-restore /warnaserror }

    Invoke-Step "Validate Docker Compose (development)" {
        docker compose --env-file .env.dev.example -f docker-compose.yml -f docker-compose.watch.yml config | Out-Null
    }

    Invoke-Step "Validate Docker Compose (production)" {
        docker compose --env-file .env.prod.example -f docker-compose.yml -f docker-compose.prod.yml config | Out-Null
    }

    $env:ConnectionStrings__Default = "Host=localhost;Port=5432;Database=cashflowpoly_test;Username=test;Password=test"
    Invoke-Step "Test (Non-Integration)" {
        dotnet test tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj -c Release --no-build --filter "Category!=Integration" --verbosity normal
    }

    if (-not $SkipIntegrationTests) {
        Invoke-Step "Test (Integration)" {
            dotnet test tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj -c Release --no-build --filter "Category=Integration" --verbosity normal
        }
    }
}
finally {
    Set-Or-ClearEnvVar -Name "ConnectionStrings__Default" -Value $previousConnectionString
    Pop-Location
}
