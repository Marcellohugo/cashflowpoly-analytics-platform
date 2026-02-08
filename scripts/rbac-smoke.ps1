param(
    [string]$ApiBaseUrl = "http://localhost:5041",
    [switch]$CheckRateLimit
)

$ErrorActionPreference = "Stop"

function Invoke-ApiRequest {
    param(
        [string]$Method,
        [string]$Url,
        [hashtable]$Headers = $null,
        [string]$Body = $null,
        [int]$Retry429 = 1
    )

    for ($attempt = 0; $attempt -le $Retry429; $attempt++) {
        $params = @{
            Method          = $Method
            Uri             = $Url
            UseBasicParsing = $true
            ErrorAction     = "Stop"
        }

        if ($Headers) {
            $params.Headers = $Headers
        }

        if (-not [string]::IsNullOrWhiteSpace($Body)) {
            $params.ContentType = "application/json"
            $params.Body = $Body
        }

        try {
            $response = Invoke-WebRequest @params
            $result = [pscustomobject]@{
                StatusCode = [int]$response.StatusCode
                Body       = $response.Content
            }
        }
        catch {
            $statusCode = 0
            $bodyText = $null
            $exceptionMessage = $_.Exception.Message

            if ($_.Exception.Response) {
                $resp = $_.Exception.Response
                try {
                    $statusCode = [int]$resp.StatusCode
                }
                catch {
                    $statusCode = 0
                }

                if ($statusCode -eq 0) {
                    try {
                        $statusRaw = $resp.StatusCode
                        if ($statusRaw -is [System.Enum]) {
                            $statusCode = [int]$statusRaw.value__
                        }
                        elseif ($statusRaw -is [int]) {
                            $statusCode = $statusRaw
                        }
                        else {
                            $parsed = 0
                            if ([int]::TryParse([string]$statusRaw, [ref]$parsed)) {
                                $statusCode = $parsed
                            }
                        }
                    }
                    catch {
                        $statusCode = 0
                    }
                }

                try {
                    $stream = $resp.GetResponseStream()
                    if ($stream) {
                        $reader = New-Object System.IO.StreamReader($stream)
                        $bodyText = $reader.ReadToEnd()
                        $reader.Close()
                    }
                }
                catch {
                    $bodyText = $null
                }
            }

            if ($statusCode -eq 0 -and $exceptionMessage -match "\((\d{3})\)") {
                $statusCode = [int]$matches[1]
            }

            $result = [pscustomobject]@{
                StatusCode = $statusCode
                Body       = $bodyText
            }
        }

        if ($result.StatusCode -eq 429 -and $attempt -lt $Retry429) {
            Write-Host "Rate limited pada $Method $Url. Menunggu 65 detik lalu retry..."
            Start-Sleep -Seconds 65
            continue
        }

        return $result
    }
}

function Assert-Status {
    param(
        [int]$Actual,
        [int]$Expected,
        [string]$Label
    )

    if ($Actual -ne $Expected) {
        throw "$Label gagal. Expected=$Expected, Actual=$Actual"
    }

    Write-Host "$Label => $Actual"
}

function Get-Token {
    param(
        [string]$Username,
        [string]$Password
    )

    $payload = @{
        username = $Username
        password = $Password
    } | ConvertTo-Json

    $login = Invoke-ApiRequest -Method Post -Url "$ApiBaseUrl/api/v1/auth/login" -Body $payload
    Assert-Status -Actual $login.StatusCode -Expected 200 -Label "Login $Username"

    return (ConvertFrom-Json $login.Body).access_token
}

Write-Host "RBAC smoke dimulai..."

$rulesetPayloadObject = @{
    name = "Ruleset RBAC Smoke"
    description = "Validasi role-based access"
    config = @{
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
}
$rulesetPayload = $rulesetPayloadObject | ConvertTo-Json -Depth 8

$noToken = Invoke-ApiRequest -Method Get -Url "$ApiBaseUrl/api/v1/sessions"
Assert-Status -Actual $noToken.StatusCode -Expected 401 -Label "Endpoint terproteksi tanpa token"

$instructorToken = Get-Token -Username "instructor" -Password "instructor123"
$playerToken = Get-Token -Username "player" -Password "player123"
$instructorHeaders = @{ Authorization = "Bearer $instructorToken" }
$playerHeaders = @{ Authorization = "Bearer $playerToken" }

$playerReadSessions = Invoke-ApiRequest -Method Get -Url "$ApiBaseUrl/api/v1/sessions" -Headers $playerHeaders
Assert-Status -Actual $playerReadSessions.StatusCode -Expected 200 -Label "Player boleh GET sessions"

$playerCreateRuleset = Invoke-ApiRequest -Method Post -Url "$ApiBaseUrl/api/v1/rulesets" -Headers $playerHeaders -Body $rulesetPayload
Assert-Status -Actual $playerCreateRuleset.StatusCode -Expected 403 -Label "Player dilarang create ruleset"

$instructorCreateRuleset = Invoke-ApiRequest -Method Post -Url "$ApiBaseUrl/api/v1/rulesets" -Headers $instructorHeaders -Body $rulesetPayload
Assert-Status -Actual $instructorCreateRuleset.StatusCode -Expected 201 -Label "Instructor create ruleset"
$createdRuleset = ConvertFrom-Json $instructorCreateRuleset.Body
$rulesetId = $createdRuleset.ruleset_id

$rulesetUpdateObject = @{
    name = "Ruleset RBAC Smoke V2"
    description = "Draft activation path"
    config = $rulesetPayloadObject.config
}
$rulesetUpdateObject.config.starting_cash = 21
$rulesetUpdatePayload = $rulesetUpdateObject | ConvertTo-Json -Depth 8

$playerUpdateRuleset = Invoke-ApiRequest -Method Put -Url "$ApiBaseUrl/api/v1/rulesets/$rulesetId" -Headers $playerHeaders -Body $rulesetUpdatePayload
Assert-Status -Actual $playerUpdateRuleset.StatusCode -Expected 403 -Label "Player dilarang update ruleset"

$instructorUpdateRuleset = Invoke-ApiRequest -Method Put -Url "$ApiBaseUrl/api/v1/rulesets/$rulesetId" -Headers $instructorHeaders -Body $rulesetUpdatePayload
Assert-Status -Actual $instructorUpdateRuleset.StatusCode -Expected 200 -Label "Instructor update ruleset (create DRAFT)"
$draftVersion = (ConvertFrom-Json $instructorUpdateRuleset.Body).version

$playerActivateGlobal = Invoke-ApiRequest -Method Post -Url "$ApiBaseUrl/api/v1/rulesets/$rulesetId/versions/$draftVersion/activate" -Headers $playerHeaders
Assert-Status -Actual $playerActivateGlobal.StatusCode -Expected 403 -Label "Player dilarang activate ruleset version"

$instructorActivateGlobal = Invoke-ApiRequest -Method Post -Url "$ApiBaseUrl/api/v1/rulesets/$rulesetId/versions/$draftVersion/activate" -Headers $instructorHeaders
Assert-Status -Actual $instructorActivateGlobal.StatusCode -Expected 200 -Label "Instructor activate ruleset version"

$sessionPayload = @{
    session_name = "Session RBAC Smoke"
    mode = "PEMULA"
    ruleset_id = $rulesetId
} | ConvertTo-Json

$playerCreateSession = Invoke-ApiRequest -Method Post -Url "$ApiBaseUrl/api/v1/sessions" -Headers $playerHeaders -Body $sessionPayload
Assert-Status -Actual $playerCreateSession.StatusCode -Expected 403 -Label "Player dilarang create session"

$instructorCreateSession = Invoke-ApiRequest -Method Post -Url "$ApiBaseUrl/api/v1/sessions" -Headers $instructorHeaders -Body $sessionPayload
Assert-Status -Actual $instructorCreateSession.StatusCode -Expected 201 -Label "Instructor create session"
$sessionId = (ConvertFrom-Json $instructorCreateSession.Body).session_id

$playerStartSession = Invoke-ApiRequest -Method Post -Url "$ApiBaseUrl/api/v1/sessions/$sessionId/start" -Headers $playerHeaders
Assert-Status -Actual $playerStartSession.StatusCode -Expected 403 -Label "Player dilarang start session"

$instructorStartSession = Invoke-ApiRequest -Method Post -Url "$ApiBaseUrl/api/v1/sessions/$sessionId/start" -Headers $instructorHeaders
Assert-Status -Actual $instructorStartSession.StatusCode -Expected 200 -Label "Instructor start session"

if ($CheckRateLimit) {
    Write-Host "Cek rate limit non-ingest (expect 429 setelah kuota habis)..."
    $saw429 = $false
    for ($i = 1; $i -le 80; $i++) {
        $check = Invoke-ApiRequest -Method Get -Url "$ApiBaseUrl/api/v1/sessions" -Headers $playerHeaders -Retry429 0
        if ($check.StatusCode -eq 429) {
            $saw429 = $true
            break
        }
    }

    if (-not $saw429) {
        throw "Rate limit non-ingest tidak memunculkan 429."
    }

    Write-Host "Rate limit non-ingest terverifikasi (429 terdeteksi)."
}

Write-Host "RBAC smoke selesai."
