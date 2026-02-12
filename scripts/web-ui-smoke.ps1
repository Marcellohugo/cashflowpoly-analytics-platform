param(
    [string]$UiBaseUrl = "http://localhost:5203",
    [string]$ApiBaseUrl = "http://localhost:5041",
    [string]$Username = "webui_instructor",
    [string]$Password = "WebUiInstructorPass!123",
    [string]$Role = "INSTRUCTOR"
)

$ErrorActionPreference = "Stop"

function Write-Pass {
    param([string]$Message)
    Write-Host "[PASS] $Message" -ForegroundColor Green
}

function Write-Fail {
    param([string]$Message)
    Write-Host "[FAIL] $Message" -ForegroundColor Red
}

function Assert-Status {
    param(
        [int]$Actual,
        [int[]]$Allowed,
        [string]$Label
    )

    if ($Allowed -contains $Actual) {
        Write-Pass "$Label (status=$Actual)"
        return
    }

    throw "$Label gagal. status=$Actual, expected one of: $($Allowed -join ', ')"
}

function Get-AntiForgeryToken {
    param([string]$Html)

    $match = [regex]::Match($Html, 'name="__RequestVerificationToken"\s+type="hidden"\s+value="([^"]+)"')
    if (-not $match.Success) {
        throw "Token antiforgery tidak ditemukan pada halaman login."
    }

    return $match.Groups[1].Value
}

function Ensure-ApiUser {
    param(
        [string]$Username,
        [string]$Password,
        [string]$Role
    )

    $loginBody = @{
        username = $Username
        password = $Password
    }

    try {
        $login = Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/v1/auth/login" -ContentType "application/json" -Body ($loginBody | ConvertTo-Json)
        if (-not [string]::IsNullOrWhiteSpace($login.access_token)) {
            return
        }
    }
    catch {
        # lanjut ke register
    }

    $registerBody = @{
        username = $Username
        password = $Password
        role = $Role
    }

    try {
        Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/v1/auth/register" -ContentType "application/json" -Body ($registerBody | ConvertTo-Json) | Out-Null
    }
    catch {
        # Bisa gagal jika user sudah ada atau registrasi role dibatasi.
    }
}

Write-Host "Web UI smoke test dimulai..."

$webSession = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$results = New-Object System.Collections.Generic.List[object]
$failedChecks = 0

try {
    Ensure-ApiUser -Username $Username -Password $Password -Role $Role

    $loginPage = Invoke-WebRequest -Uri "$UiBaseUrl/auth/login" -Method Get -WebSession $webSession -UseBasicParsing
    Assert-Status -Actual ([int]$loginPage.StatusCode) -Allowed @(200) -Label "GET /auth/login"
    $token = Get-AntiForgeryToken -Html $loginPage.Content

    $loginForm = @{
        __RequestVerificationToken = $token
        Username = $Username
        Password = $Password
        ReturnUrl = ""
    }

    $loginResponse = Invoke-WebRequest -Uri "$UiBaseUrl/auth/login" -Method Post -Body $loginForm -WebSession $webSession -UseBasicParsing
    Assert-Status -Actual ([int]$loginResponse.StatusCode) -Allowed @(200) -Label "POST /auth/login"
    if ($loginResponse.Content -match "Login gagal|Sign In|Masuk dengan akun") {
        throw "Login UI terdeteksi gagal (masih berada di halaman login)."
    }
    Write-Pass "Autentikasi UI berhasil"

    $targets = @(
        @{ Path = "/"; Name = "Home"; MustContain = "Cashflowpoly" },
        @{ Path = "/sessions"; Name = "Sessions"; MustContain = "Daftar sesi" },
        @{ Path = "/players"; Name = "Players"; MustContain = "Daftar pemain" },
        @{ Path = "/rulesets"; Name = "Rulesets"; MustContain = "Daftar ruleset" },
        @{ Path = "/analytics"; Name = "Analytics"; MustContain = "Cari analitika sesi" },
        @{ Path = "/home/rulebook"; Name = "Rulebook"; MustContain = "Rulebook" }
    )

    foreach ($target in $targets) {
        try {
            $resp = Invoke-WebRequest -Uri "$UiBaseUrl$($target.Path)" -Method Get -WebSession $webSession -UseBasicParsing
            Assert-Status -Actual ([int]$resp.StatusCode) -Allowed @(200) -Label "GET $($target.Path)"

            if ($resp.Content -notmatch [regex]::Escape($target.MustContain)) {
                throw "Konten kunci '$($target.MustContain)' tidak ditemukan."
            }

            Write-Pass "Konten $($target.Name) terdeteksi"
            $results.Add([pscustomobject]@{ Page = $target.Name; Path = $target.Path; Status = "PASS"; Detail = "OK" }) | Out-Null
        }
        catch {
            Write-Fail "$($target.Name): $($_.Exception.Message)"
            $failedChecks++
            $results.Add([pscustomobject]@{ Page = $target.Name; Path = $target.Path; Status = "FAIL"; Detail = $_.Exception.Message }) | Out-Null
        }
    }

    try {
        $swagger = Invoke-WebRequest -Uri "$ApiBaseUrl/swagger/index.html" -Method Get -UseBasicParsing
        Assert-Status -Actual ([int]$swagger.StatusCode) -Allowed @(200) -Label "GET API Swagger"
        $results.Add([pscustomobject]@{ Page = "API Swagger"; Path = "/swagger/index.html"; Status = "PASS"; Detail = "OK" }) | Out-Null
    }
    catch {
        Write-Fail "API Swagger: $($_.Exception.Message)"
        $failedChecks++
        $results.Add([pscustomobject]@{ Page = "API Swagger"; Path = "/swagger/index.html"; Status = "FAIL"; Detail = $_.Exception.Message }) | Out-Null
    }

    Write-Host ""
    Write-Host "Ringkasan hasil:"
    $results | Format-Table -AutoSize

    if ($failedChecks -gt 0) {
        throw "Smoke test selesai dengan $failedChecks kegagalan."
    }

    Write-Host ""
    Write-Host "Web UI smoke test selesai tanpa error." -ForegroundColor Green
}
catch {
    Write-Host ""
    Write-Fail $_.Exception.Message
    exit 1
}
