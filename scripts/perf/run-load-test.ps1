# Fungsi file: Menjalankan skenario uji beban untuk mengukur performa endpoint API.
param(
    [string]$BaseUrl = "http://localhost:5041",
    [int]$IngestRequests = 120,
    [int]$AnalyticsRequests = 60,
    [string]$AccessToken = "",
    [string]$Username = "",
    [string]$Password = ""
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ($IngestRequests -lt 1) {
    throw "IngestRequests minimal 1."
}

if ($AnalyticsRequests -lt 1) {
    throw "AnalyticsRequests minimal 1."
}

$normalizedBaseUrl = $BaseUrl.TrimEnd("/")
$httpClient = [System.Net.Http.HttpClient]::new()
$httpClient.Timeout = [TimeSpan]::FromSeconds(30)
$httpClient.BaseAddress = [Uri]::new($normalizedBaseUrl)

function Convert-ToJson {
    param([Parameter(Mandatory = $true)][object]$Value)
    return ($Value | ConvertTo-Json -Depth 12 -Compress)
}

function Resolve-HttpMethod {
    param([Parameter(Mandatory = $true)][string]$Method)

    switch ($Method.ToUpperInvariant()) {
        "GET" { return [System.Net.Http.HttpMethod]::Get }
        "POST" { return [System.Net.Http.HttpMethod]::Post }
        "PUT" { return [System.Net.Http.HttpMethod]::Put }
        "DELETE" { return [System.Net.Http.HttpMethod]::Delete }
        default { throw "HTTP method '$Method' tidak didukung." }
    }
}

function Invoke-ApiRequest {
    param(
        [Parameter(Mandatory = $true)][string]$Method,
        [Parameter(Mandatory = $true)][string]$Path,
        [object]$Body = $null,
        [string]$Token = ""
    )

    $resolvedPath = if ($Path.StartsWith("/")) { $Path } else { "/$Path" }
    $request = [System.Net.Http.HttpRequestMessage]::new((Resolve-HttpMethod -Method $Method), $resolvedPath)

    if (-not [string]::IsNullOrWhiteSpace($Token)) {
        $request.Headers.Authorization = [System.Net.Http.Headers.AuthenticationHeaderValue]::new("Bearer", $Token)
    }

    if ($null -ne $Body) {
        $jsonBody = Convert-ToJson -Value $Body
        $request.Content = [System.Net.Http.StringContent]::new($jsonBody, [System.Text.Encoding]::UTF8, "application/json")
    }

    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    $response = $null
    try {
        $response = $script:httpClient.SendAsync($request).GetAwaiter().GetResult()
    }
    finally {
        $stopwatch.Stop()
        $request.Dispose()
    }

    try {
        $bodyText = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
        $bodyJson = $null
        if (-not [string]::IsNullOrWhiteSpace($bodyText)) {
            try {
                $bodyJson = $bodyText | ConvertFrom-Json -ErrorAction Stop
            }
            catch {
                $bodyJson = $null
            }
        }

        return [PSCustomObject]@{
            StatusCode = [int]$response.StatusCode
            BodyText = $bodyText
            BodyJson = $bodyJson
            LatencyMs = [Math]::Round($stopwatch.Elapsed.TotalMilliseconds, 2)
        }
    }
    finally {
        $response.Dispose()
    }
}

function Get-Percentile {
    param(
        [Parameter(Mandatory = $true)][double[]]$Values,
        [Parameter(Mandatory = $true)][double]$Percentile
    )

    if ($Values.Count -eq 0) {
        return 0
    }

    $ordered = $Values | Sort-Object
    $rank = [Math]::Ceiling(($Percentile / 100) * $ordered.Count)
    if ($rank -lt 1) {
        $rank = 1
    }

    return [Math]::Round([double]$ordered[$rank - 1], 2)
}

function Resolve-AccessToken {
    param(
        [string]$ExistingToken,
        [string]$LoginUsername,
        [string]$LoginPassword
    )

    if (-not [string]::IsNullOrWhiteSpace($ExistingToken)) {
        return [PSCustomObject]@{
            Token = $ExistingToken
            Identity = "token-parameter"
        }
    }

    if (-not [string]::IsNullOrWhiteSpace($LoginUsername) -and -not [string]::IsNullOrWhiteSpace($LoginPassword)) {
        $loginResponse = Invoke-ApiRequest -Method "POST" -Path "/api/v1/auth/login" -Body @{
            username = $LoginUsername
            password = $LoginPassword
        }

        if ($loginResponse.StatusCode -ne 200 -or $null -eq $loginResponse.BodyJson.access_token) {
            throw "Login gagal untuk user '$LoginUsername'. Berikan kredensial valid atau -AccessToken."
        }

        return [PSCustomObject]@{
            Token = [string]$loginResponse.BodyJson.access_token
            Identity = "login:$LoginUsername"
        }
    }

    $suffix = [Guid]::NewGuid().ToString("N").Substring(0, 8)
    $autoUsername = "perf_instructor_$suffix"
    $autoPassword = "PerfInstructor!123"

    $registerResponse = Invoke-ApiRequest -Method "POST" -Path "/api/v1/auth/register" -Body @{
        username = $autoUsername
        password = $autoPassword
        role = "INSTRUCTOR"
        display_name = "Perf Instructor $suffix"
    }

    if ($registerResponse.StatusCode -eq 201 -and $null -ne $registerResponse.BodyJson.access_token) {
        return [PSCustomObject]@{
            Token = [string]$registerResponse.BodyJson.access_token
            Identity = "auto-register:$autoUsername"
        }
    }

    if ($registerResponse.StatusCode -eq 403) {
        throw "Registrasi otomatis ditolak oleh kebijakan server. Jalankan ulang script dengan -Username/-Password atau -AccessToken."
    }

    throw "Gagal memperoleh token akses otomatis. Status: $($registerResponse.StatusCode)."
}

function New-RulesetConfig {
    param([int]$StartingCash = 100)

    return @{
        mode = "PEMULA"
        actions_per_turn = 2
        starting_cash = $StartingCash
        weekday_rules = @{
            friday = @{ feature = "DONATION"; enabled = $true }
            saturday = @{ feature = "GOLD_TRADE"; enabled = $true }
            sunday = @{ feature = "REST"; enabled = $true }
        }
        constraints = @{
            cash_min = 0
            max_ingredient_total = 6
            max_same_ingredient = 3
            primary_need_max_per_day = 1
            require_primary_before_others = $true
        }
        donation = @{
            min_amount = 1
            max_amount = 999999
        }
        gold_trade = @{
            allow_buy = $true
            allow_sell = $true
        }
        advanced = @{
            loan = @{ enabled = $false }
            insurance = @{ enabled = $false }
            saving_goal = @{ enabled = $false }
        }
        freelance = @{
            income = 1
        }
        scoring = @{
            donation_rank_points = @(
                @{ rank = 1; points = 7 },
                @{ rank = 2; points = 5 },
                @{ rank = 3; points = 2 }
            )
            gold_points_by_qty = @(
                @{ qty = 1; points = 3 },
                @{ qty = 2; points = 5 },
                @{ qty = 3; points = 8 },
                @{ qty = 4; points = 12 }
            )
            pension_rank_points = @(
                @{ rank = 1; points = 5 },
                @{ rank = 2; points = 3 },
                @{ rank = 3; points = 1 }
            )
        }
    }
}

try {
    $tokenResolution = Resolve-AccessToken -ExistingToken $AccessToken -LoginUsername $Username -LoginPassword $Password
    $token = $tokenResolution.Token
    $identity = $tokenResolution.Identity

    $suffix = [Guid]::NewGuid().ToString("N").Substring(0, 8)
    $playerUsername = "perf_player_$suffix"
    $playerPassword = "PerfPlayer!123"
    $nowUtc = [DateTimeOffset]::UtcNow

    $createRulesetResponse = Invoke-ApiRequest -Method "POST" -Path "/api/v1/rulesets" -Token $token -Body @{
        name = "Perf Ruleset $suffix"
        description = "Baseline load test ruleset"
        config = New-RulesetConfig -StartingCash 100
    }
    if ($createRulesetResponse.StatusCode -ne 201 -or $null -eq $createRulesetResponse.BodyJson.ruleset_id) {
        throw "Gagal membuat ruleset. Status: $($createRulesetResponse.StatusCode)."
    }
    $rulesetId = [Guid]$createRulesetResponse.BodyJson.ruleset_id

    $createSessionResponse = Invoke-ApiRequest -Method "POST" -Path "/api/v1/sessions" -Token $token -Body @{
        session_name = "Perf Session $suffix"
        mode = "PEMULA"
        ruleset_id = $rulesetId
    }
    if ($createSessionResponse.StatusCode -ne 201 -or $null -eq $createSessionResponse.BodyJson.session_id) {
        throw "Gagal membuat sesi. Status: $($createSessionResponse.StatusCode)."
    }
    $sessionId = [Guid]$createSessionResponse.BodyJson.session_id

    $createPlayerResponse = Invoke-ApiRequest -Method "POST" -Path "/api/v1/players" -Token $token -Body @{
        display_name = "Perf Player $suffix"
        username = $playerUsername
        password = $playerPassword
    }
    if ($createPlayerResponse.StatusCode -ne 201 -or $null -eq $createPlayerResponse.BodyJson.player_id) {
        throw "Gagal membuat player. Status: $($createPlayerResponse.StatusCode)."
    }
    $playerId = [Guid]$createPlayerResponse.BodyJson.player_id

    $addPlayerResponse = Invoke-ApiRequest -Method "POST" -Path "/api/v1/sessions/$sessionId/players" -Token $token -Body @{
        player_id = $playerId
        role = "PLAYER"
    }
    if ($addPlayerResponse.StatusCode -ne 200) {
        throw "Gagal menambahkan player ke sesi. Status: $($addPlayerResponse.StatusCode)."
    }

    $startSessionResponse = Invoke-ApiRequest -Method "POST" -Path "/api/v1/sessions/$sessionId/start" -Token $token
    if ($startSessionResponse.StatusCode -ne 200) {
        throw "Gagal memulai sesi. Status: $($startSessionResponse.StatusCode)."
    }

    $rulesetDetailResponse = Invoke-ApiRequest -Method "GET" -Path "/api/v1/rulesets/$rulesetId" -Token $token
    if ($rulesetDetailResponse.StatusCode -ne 200 -or $null -eq $rulesetDetailResponse.BodyJson.versions) {
        throw "Gagal mengambil detail ruleset. Status: $($rulesetDetailResponse.StatusCode)."
    }

    $activeVersion = $rulesetDetailResponse.BodyJson.versions |
        Where-Object { $_.status -eq "ACTIVE" } |
        Sort-Object version -Descending |
        Select-Object -First 1
    if ($null -eq $activeVersion -or [string]::IsNullOrWhiteSpace([string]$activeVersion.ruleset_version_id)) {
        throw "Ruleset aktif tidak ditemukan untuk sesi beban."
    }

    $rulesetVersionId = [Guid]$activeVersion.ruleset_version_id
    $ingestLatencies = New-Object System.Collections.Generic.List[double]
    $analyticsLatencies = New-Object System.Collections.Generic.List[double]
    $failedRequests = 0
    $failedSamples = New-Object System.Collections.Generic.List[string]

    for ($i = 1; $i -le $IngestRequests; $i++) {
        $direction = if ($i % 2 -eq 0) { "OUT" } else { "IN" }
        $amount = if ($direction -eq "IN") { 2 } else { 1 }
        $eventTimestamp = $nowUtc.AddMilliseconds($i).ToString("O")

        $eventResponse = Invoke-ApiRequest -Method "POST" -Path "/api/v1/events" -Token $token -Body @{
            event_id = [Guid]::NewGuid()
            session_id = $sessionId
            player_id = $playerId
            actor_type = "PLAYER"
            timestamp = $eventTimestamp
            day_index = 0
            weekday = "MON"
            turn_number = 1
            sequence_number = $i
            action_type = "transaction.recorded"
            ruleset_version_id = $rulesetVersionId
            payload = @{
                direction = $direction
                amount = $amount
                category = "PERF_TEST"
                counterparty = "BANK"
            }
        }

        $ingestLatencies.Add([double]$eventResponse.LatencyMs)
        if ($eventResponse.StatusCode -ne 201) {
            $failedRequests += 1
            if ($failedSamples.Count -lt 5) {
                $failedSamples.Add("INGEST[$i] status=$($eventResponse.StatusCode)")
            }
        }
    }

    for ($i = 1; $i -le $AnalyticsRequests; $i++) {
        $analyticsResponse = Invoke-ApiRequest -Method "GET" -Path "/api/v1/analytics/sessions/$sessionId" -Token $token
        $analyticsLatencies.Add([double]$analyticsResponse.LatencyMs)
        if ($analyticsResponse.StatusCode -ne 200) {
            $failedRequests += 1
            if ($failedSamples.Count -lt 5) {
                $failedSamples.Add("ANALYTICS[$i] status=$($analyticsResponse.StatusCode)")
            }
        }
    }

    $ingestAvg = [Math]::Round((($ingestLatencies | Measure-Object -Average).Average), 2)
    $analyticsAvg = [Math]::Round((($analyticsLatencies | Measure-Object -Average).Average), 2)
    $ingestP95 = Get-Percentile -Values $ingestLatencies.ToArray() -Percentile 95
    $analyticsP95 = Get-Percentile -Values $analyticsLatencies.ToArray() -Percentile 95
    $totalRequests = $IngestRequests + $AnalyticsRequests
    $errorRate = [Math]::Round(($failedRequests / [double]$totalRequests) * 100, 2)

    $ingestTargetMs = 500
    $analyticsTargetMs = 1500
    $ingestPass = $ingestP95 -le $ingestTargetMs
    $analyticsPass = $analyticsP95 -le $analyticsTargetMs
    $errorPass = $errorRate -eq 0
    $overallStatus = if ($ingestPass -and $analyticsPass -and $errorPass) { "PASS" } else { "CHECK" }

    $evidenceRoot = Join-Path -Path (Get-Location) -ChildPath "docs\\evidence"
    $evidenceDateDir = Join-Path -Path $evidenceRoot -ChildPath (Get-Date -Format "yyyy-MM-dd")
    New-Item -ItemType Directory -Force -Path $evidenceDateDir | Out-Null
    $summaryPath = Join-Path -Path $evidenceDateDir -ChildPath "load-test-summary.md"

    $sampleFailures = if ($failedSamples.Count -eq 0) { "-" } else { ($failedSamples -join "; ") }
    $generatedAt = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    $summary = @"
# Load Test Summary

- Generated at (UTC): $generatedAt
- Base URL: $normalizedBaseUrl
- Auth source: $identity
- Session ID: $sessionId
- Ruleset ID: $rulesetId
- Player ID: $playerId
- Overall status: **$overallStatus**

## Configuration
| Item | Value |
|---|---:|
| Ingest requests | $IngestRequests |
| Analytics requests | $AnalyticsRequests |
| Total requests | $totalRequests |

## Results
| Metric | Value | Target | Status |
|---|---:|---:|---|
| Ingest avg latency (ms) | $ingestAvg | - | - |
| Ingest p95 latency (ms) | $ingestP95 | <= $ingestTargetMs | $(if ($ingestPass) { "PASS" } else { "FAIL" }) |
| Analytics avg latency (ms) | $analyticsAvg | - | - |
| Analytics p95 latency (ms) | $analyticsP95 | <= $analyticsTargetMs | $(if ($analyticsPass) { "PASS" } else { "FAIL" }) |
| Error rate (%) | $errorRate | 0 | $(if ($errorPass) { "PASS" } else { "FAIL" }) |

## Failure Samples
$sampleFailures
"@

    Set-Content -Path $summaryPath -Value $summary -Encoding UTF8

    Write-Host "Load test selesai."
    Write-Host "Summary: $summaryPath"
    Write-Host "Ingest p95: $ingestP95 ms | Analytics p95: $analyticsP95 ms | Error rate: $errorRate%"

    if ($failedRequests -gt 0) {
        exit 2
    }

    if (-not ($ingestPass -and $analyticsPass -and $errorPass)) {
        exit 1
    }
}
finally {
    $httpClient.Dispose()
}
