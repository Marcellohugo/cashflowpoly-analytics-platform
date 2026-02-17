param(
    [string]$BaseUrl = "http://localhost:5041",
    [int]$SeedEvents = 300,
    [int]$AnalyticsIterations = 80,
    [string]$OutputPath = "",
    [string]$InstructorUsername = "",
    [string]$InstructorPassword = ""
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Net.Http

if ($SeedEvents -lt 50) {
    throw "SeedEvents minimal 50 agar pengukuran P95 lebih bermakna."
}

if ($AnalyticsIterations -lt 20) {
    throw "AnalyticsIterations minimal 20 agar pengukuran P95 lebih bermakna."
}

function Invoke-ApiRequest {
    param(
        [System.Net.Http.HttpClient]$Client,
        [string]$Method,
        [string]$Path,
        [object]$Body = $null,
        [string]$BearerToken = ""
    )

    $request = [System.Net.Http.HttpRequestMessage]::new([System.Net.Http.HttpMethod]::new($Method.ToUpperInvariant()), $Path)
    try {
        if (-not [string]::IsNullOrWhiteSpace($BearerToken)) {
            $request.Headers.Authorization = [System.Net.Http.Headers.AuthenticationHeaderValue]::new("Bearer", $BearerToken)
        }

        if ($null -ne $Body) {
            $json = $Body | ConvertTo-Json -Depth 30 -Compress
            $request.Content = [System.Net.Http.StringContent]::new($json, [System.Text.Encoding]::UTF8, "application/json")
        }

        $sw = [System.Diagnostics.Stopwatch]::StartNew()
        $response = $Client.SendAsync($request).GetAwaiter().GetResult()
        $sw.Stop()

        $rawBody = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
        $parsedBody = $null
        if (-not [string]::IsNullOrWhiteSpace($rawBody)) {
            try {
                $parsedBody = $rawBody | ConvertFrom-Json
            }
            catch {
                $parsedBody = $null
            }
        }

        return [PSCustomObject]@{
            StatusCode = [int]$response.StatusCode
            IsSuccess = $response.IsSuccessStatusCode
            DurationMs = [Math]::Round($sw.Elapsed.TotalMilliseconds, 2)
            Body = $parsedBody
            RawBody = $rawBody
        }
    }
    finally {
        $request.Dispose()
    }
}

function Assert-Status {
    param(
        [object]$Response,
        [int[]]$ExpectedStatusCodes,
        [string]$StepName
    )

    if ($ExpectedStatusCodes -contains $Response.StatusCode) {
        return
    }

    throw "[$StepName] gagal. status=$($Response.StatusCode), body=$($Response.RawBody)"
}

function Get-Percentile {
    param(
        [double[]]$Values,
        [int]$Percentile
    )

    if ($Values.Count -eq 0) {
        return 0
    }

    $sorted = $Values | Sort-Object
    $p = [Math]::Min([Math]::Max($Percentile, 1), 100)
    $index = [Math]::Ceiling(($p / 100.0) * $sorted.Count) - 1
    if ($index -lt 0) {
        $index = 0
    }
    if ($index -ge $sorted.Count) {
        $index = $sorted.Count - 1
    }

    return [Math]::Round([double]$sorted[$index], 2)
}

function Get-LatencyStats {
    param(
        [double[]]$Durations,
        [int]$FailureCount
    )

    if ($Durations.Count -eq 0) {
        return [PSCustomObject]@{
            Requests = 0
            Success = 0
            Failure = $FailureCount
            ErrorRatePercent = 100
            AverageMs = 0
            P50Ms = 0
            P95Ms = 0
            MaxMs = 0
        }
    }

    $requestCount = $Durations.Count + $FailureCount
    $avg = [Math]::Round(($Durations | Measure-Object -Average).Average, 2)
    $max = [Math]::Round(($Durations | Measure-Object -Maximum).Maximum, 2)
    $p50 = Get-Percentile -Values $Durations -Percentile 50
    $p95 = Get-Percentile -Values $Durations -Percentile 95
    $errorRate = if ($requestCount -eq 0) { 0 } else { [Math]::Round(($FailureCount * 100.0) / $requestCount, 2) }

    return [PSCustomObject]@{
        Requests = $requestCount
        Success = $Durations.Count
        Failure = $FailureCount
        ErrorRatePercent = $errorRate
        AverageMs = $avg
        P50Ms = $p50
        P95Ms = $p95
        MaxMs = $max
    }
}

$normalizedBaseUrl = $BaseUrl.TrimEnd('/') + "/"
$client = [System.Net.Http.HttpClient]::new()
$client.BaseAddress = [Uri]$normalizedBaseUrl
$client.Timeout = [TimeSpan]::FromSeconds(30)

try {
    $health = Invoke-ApiRequest -Client $client -Method "GET" -Path "health/ready"
    Assert-Status -Response $health -ExpectedStatusCodes @(200) -StepName "health/ready"

    $runId = [Guid]::NewGuid().ToString("N").Substring(0, 10)
    $timestampUtc = [DateTimeOffset]::UtcNow
    $instructorUsername = if ([string]::IsNullOrWhiteSpace($InstructorUsername)) { "perf_instr_$runId" } else { $InstructorUsername.Trim() }
    $playerUsername = "perf_player_$runId"
    $password = if ([string]::IsNullOrWhiteSpace($InstructorPassword)) { "PerfPass!123" } else { $InstructorPassword }
    $token = ""

    if (-not [string]::IsNullOrWhiteSpace($InstructorUsername) -and -not [string]::IsNullOrWhiteSpace($InstructorPassword)) {
        $loginInstructor = Invoke-ApiRequest -Client $client -Method "POST" -Path "api/v1/auth/login" -Body @{
            username = $instructorUsername
            password = $password
        }
        Assert-Status -Response $loginInstructor -ExpectedStatusCodes @(200) -StepName "login instructor"
        $token = [string]$loginInstructor.Body.access_token
    }
    else {
        $registerInstructor = Invoke-ApiRequest -Client $client -Method "POST" -Path "api/v1/auth/register" -Body @{
            username = $instructorUsername
            password = $password
            role = "INSTRUCTOR"
            display_name = "Perf Instructor $runId"
        }
        if ($registerInstructor.StatusCode -eq 403) {
            throw "Registrasi INSTRUCTOR ditolak (403). Jalankan script dengan -InstructorUsername dan -InstructorPassword untuk akun instruktur yang sudah ada."
        }
        Assert-Status -Response $registerInstructor -ExpectedStatusCodes @(201) -StepName "register instructor"
        $token = [string]$registerInstructor.Body.access_token
    }

    if ([string]::IsNullOrWhiteSpace($token)) {
        throw "Token instruktur kosong."
    }

    $rulesetConfig = @{
        mode = "PEMULA"
        actions_per_turn = 2
        starting_cash = 20
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
        donation = @{ min_amount = 1; max_amount = 999999 }
        gold_trade = @{ allow_buy = $true; allow_sell = $true }
        advanced = @{
            loan = @{ enabled = $false }
            insurance = @{ enabled = $false }
            saving_goal = @{ enabled = $false }
        }
        freelance = @{ income = 1 }
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

    $createRuleset = Invoke-ApiRequest -Client $client -Method "POST" -Path "api/v1/rulesets" -BearerToken $token -Body @{
        name = "Perf Ruleset $runId"
        description = "Ruleset untuk load test otomatis"
        config = $rulesetConfig
    }
    Assert-Status -Response $createRuleset -ExpectedStatusCodes @(201) -StepName "create ruleset"
    $rulesetId = [Guid]$createRuleset.Body.ruleset_id

    $rulesetDetail = Invoke-ApiRequest -Client $client -Method "GET" -Path ("api/v1/rulesets/{0}" -f $rulesetId) -BearerToken $token
    Assert-Status -Response $rulesetDetail -ExpectedStatusCodes @(200) -StepName "get ruleset detail"

    $activeVersion = $rulesetDetail.Body.versions | Where-Object { $_.status -eq "ACTIVE" } | Select-Object -First 1
    if ($null -eq $activeVersion) {
        throw "Ruleset ACTIVE version tidak ditemukan."
    }
    $rulesetVersionId = [Guid]$activeVersion.ruleset_version_id

    $createSession = Invoke-ApiRequest -Client $client -Method "POST" -Path "api/v1/sessions" -BearerToken $token -Body @{
        session_name = "Perf Session $runId"
        mode = "PEMULA"
        ruleset_id = $rulesetId
    }
    Assert-Status -Response $createSession -ExpectedStatusCodes @(201) -StepName "create session"
    $sessionId = [Guid]$createSession.Body.session_id

    $startSession = Invoke-ApiRequest -Client $client -Method "POST" -Path ("api/v1/sessions/{0}/start" -f $sessionId) -BearerToken $token
    Assert-Status -Response $startSession -ExpectedStatusCodes @(200) -StepName "start session"

    $createPlayer = Invoke-ApiRequest -Client $client -Method "POST" -Path "api/v1/players" -BearerToken $token -Body @{
        display_name = "Perf Player $runId"
        username = $playerUsername
        password = "PerfPlayerPass!123"
    }
    Assert-Status -Response $createPlayer -ExpectedStatusCodes @(201) -StepName "create player"
    $playerId = [Guid]$createPlayer.Body.player_id

    $addPlayer = Invoke-ApiRequest -Client $client -Method "POST" -Path ("api/v1/sessions/{0}/players" -f $sessionId) -BearerToken $token -Body @{
        player_id = $playerId
        role = "PLAYER"
    }
    Assert-Status -Response $addPlayer -ExpectedStatusCodes @(200) -StepName "add player to session"

    $ingestDurations = New-Object System.Collections.Generic.List[double]
    $ingestFailures = 0
    for ($i = 1; $i -le $SeedEvents; $i++) {
        $eventTimestamp = $timestampUtc.AddSeconds($i).ToString("o")
        $eventPayload = @{
            event_id = [Guid]::NewGuid()
            session_id = $sessionId
            player_id = $playerId
            actor_type = "PLAYER"
            timestamp = $eventTimestamp
            day_index = [Math]::Floor(($i - 1) / 20)
            weekday = "MON"
            turn_number = $i
            sequence_number = $i
            action_type = "transaction.recorded"
            ruleset_version_id = $rulesetVersionId
            payload = @{
                direction = "IN"
                amount = 1
                category = "FREELANCE"
                counterparty = "BANK"
                reference = "LOAD-$i"
                note = "seed event load test"
            }
            client_request_id = "load-ingest-$runId-$i"
        }

        $response = Invoke-ApiRequest -Client $client -Method "POST" -Path "api/v1/events" -BearerToken $token -Body $eventPayload
        if ($response.StatusCode -eq 201) {
            [void]$ingestDurations.Add([double]$response.DurationMs)
        }
        else {
            $ingestFailures++
        }
    }

    $analyticsDurations = New-Object System.Collections.Generic.List[double]
    $analyticsFailures = 0
    for ($i = 1; $i -le $AnalyticsIterations; $i++) {
        $response = Invoke-ApiRequest -Client $client -Method "GET" -Path ("api/v1/analytics/sessions/{0}" -f $sessionId) -BearerToken $token
        if ($response.StatusCode -eq 200) {
            [void]$analyticsDurations.Add([double]$response.DurationMs)
        }
        else {
            $analyticsFailures++
        }
    }

    $ingestStats = Get-LatencyStats -Durations $ingestDurations.ToArray() -FailureCount $ingestFailures
    $analyticsStats = Get-LatencyStats -Durations $analyticsDurations.ToArray() -FailureCount $analyticsFailures

    $ingestSlaPass = ($ingestStats.P95Ms -le 500) -and ($ingestStats.ErrorRatePercent -eq 0)
    $analyticsSlaPass = ($analyticsStats.P95Ms -le 1500) -and ($analyticsStats.ErrorRatePercent -eq 0)

    if ([string]::IsNullOrWhiteSpace($OutputPath)) {
        $dateFolder = [DateTimeOffset]::UtcNow.ToString("yyyy-MM-dd")
        $OutputPath = "docs/evidence/$dateFolder/load-test-summary.md"
    }

    $outputDirectory = Split-Path -Path $OutputPath -Parent
    if (-not [string]::IsNullOrWhiteSpace($outputDirectory)) {
        New-Item -Path $outputDirectory -ItemType Directory -Force | Out-Null
    }

    $lines = @(
        "# Load Test Summary",
        "",
        "- Generated at (UTC): $([DateTimeOffset]::UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))",
        "- Base URL: $normalizedBaseUrl",
        "- Session ID: $sessionId",
        "- Player ID: $playerId",
        "- Ruleset Version ID: $rulesetVersionId",
        "",
        "## Scenario Parameters",
        "",
        "| Metric | Value |",
        "|---|---|",
        "| Seed events | $SeedEvents |",
        "| Analytics iterations | $AnalyticsIterations |",
        "",
        "## Results",
        "",
        "| Endpoint Group | Requests | Success | Failure | Error Rate (%) | Avg (ms) | P50 (ms) | P95 (ms) | Max (ms) | SLA |",
        "|---|---:|---:|---:|---:|---:|---:|---:|---:|---|",
        "| POST /api/v1/events | $($ingestStats.Requests) | $($ingestStats.Success) | $($ingestStats.Failure) | $($ingestStats.ErrorRatePercent) | $($ingestStats.AverageMs) | $($ingestStats.P50Ms) | $($ingestStats.P95Ms) | $($ingestStats.MaxMs) | $(if ($ingestSlaPass) { "PASS" } else { "FAIL" }) |",
        "| GET /api/v1/analytics/sessions/{sessionId} | $($analyticsStats.Requests) | $($analyticsStats.Success) | $($analyticsStats.Failure) | $($analyticsStats.ErrorRatePercent) | $($analyticsStats.AverageMs) | $($analyticsStats.P50Ms) | $($analyticsStats.P95Ms) | $($analyticsStats.MaxMs) | $(if ($analyticsSlaPass) { "PASS" } else { "FAIL" }) |",
        "",
        "## Notes",
        "",
        "- Ingest SLA target: P95 <= 500 ms, error rate = 0%",
        "- Analytics SLA target: P95 <= 1500 ms, error rate = 0%",
        "- Script ini menyiapkan data uji secara otomatis (register -> ruleset -> session -> player -> ingest -> analytics)."
    )

    Set-Content -Path $OutputPath -Value $lines -Encoding UTF8

    Write-Host "Load test selesai."
    Write-Host "Output: $OutputPath"
    Write-Host ("Ingest: P95={0}ms, ErrorRate={1}% => {2}" -f $ingestStats.P95Ms, $ingestStats.ErrorRatePercent, $(if ($ingestSlaPass) { "PASS" } else { "FAIL" }))
    Write-Host ("Analytics: P95={0}ms, ErrorRate={1}% => {2}" -f $analyticsStats.P95Ms, $analyticsStats.ErrorRatePercent, $(if ($analyticsSlaPass) { "PASS" } else { "FAIL" }))
}
finally {
    if ($null -ne $client) {
        $client.Dispose()
    }
}
