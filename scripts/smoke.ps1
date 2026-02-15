param(
    [string]$ApiBaseUrl = "http://localhost:5041",
    [string]$InstructorUsername = "smoke_instructor",
    [string]$InstructorPassword = "SmokeInstructorPass!123"
)

$ErrorActionPreference = "Stop"

function Try-LoginToken {
    param(
        [string]$Username,
        [string]$Password
    )

    $loginBody = @{
        username = $Username
        password = $Password
    }

    try {
        $login = Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/v1/auth/login" -ContentType "application/json" -Body ($loginBody | ConvertTo-Json)
        return $login.access_token
    }
    catch {
        return $null
    }
}

function Ensure-UserToken {
    param(
        [string]$Username,
        [string]$Password,
        [string]$Role
    )

    $token = Try-LoginToken -Username $Username -Password $Password
    if (-not [string]::IsNullOrWhiteSpace($token)) {
        return $token
    }

    Write-Host "Register user '$Username' ($Role)..."
    $registerBody = @{
        username = $Username
        password = $Password
        role = $Role
    }

    try {
        $registered = Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/v1/auth/register" -ContentType "application/json" -Body ($registerBody | ConvertTo-Json)
        if (-not [string]::IsNullOrWhiteSpace($registered.access_token)) {
            return $registered.access_token
        }
    }
    catch {
        # Bisa gagal karena user sudah ada, registrasi instruktur ditutup, atau policy lain.
    }

    $token = Try-LoginToken -Username $Username -Password $Password
    if (-not [string]::IsNullOrWhiteSpace($token)) {
        return $token
    }

    throw "Gagal memperoleh token untuk user '$Username'. Pastikan kredensial benar atau bootstrap instruktur sudah dikonfigurasi."
}

Write-Host "Login/register instructor..."
$instructorToken = Ensure-UserToken -Username $InstructorUsername -Password $InstructorPassword -Role "INSTRUCTOR"
$bearerHeaders = @{ Authorization = "Bearer $instructorToken" }

$rulesetBody = @{
    name = "Ruleset Smoke"
    description = "Smoke test ruleset"
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

Write-Host "Create ruleset..."
$ruleset = Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/v1/rulesets" -Headers $bearerHeaders -ContentType "application/json" -Body ($rulesetBody | ConvertTo-Json -Depth 8)

Write-Host "Create draft ruleset version..."
$rulesetUpdateBody = @{
    name = "Ruleset Smoke V2"
    description = "Smoke test ruleset draft"
    config = $rulesetBody.config
}
$rulesetUpdateBody.config.starting_cash = 21
$rulesetUpdate = Invoke-RestMethod -Method Put -Uri "$ApiBaseUrl/api/v1/rulesets/$($ruleset.ruleset_id)" -Headers $bearerHeaders -ContentType "application/json" -Body ($rulesetUpdateBody | ConvertTo-Json -Depth 8)

Write-Host "Get ruleset detail..."
$rulesetDetail = Invoke-RestMethod -Method Get -Uri "$ApiBaseUrl/api/v1/rulesets/$($ruleset.ruleset_id)" -Headers $bearerHeaders -ContentType "application/json"
$draftVersion = $rulesetDetail.versions | Where-Object { $_.version -eq $rulesetUpdate.version } | Select-Object -First 1
if ($null -eq $draftVersion -or $draftVersion.status -ne "DRAFT") {
    throw "Expected draft version status DRAFT after update."
}

Write-Host "Activate draft ruleset version..."
Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/v1/rulesets/$($ruleset.ruleset_id)/versions/$($rulesetUpdate.version)/activate" -Headers $bearerHeaders -ContentType "application/json" | Out-Null

Write-Host "Recheck ruleset detail..."
$rulesetDetail = Invoke-RestMethod -Method Get -Uri "$ApiBaseUrl/api/v1/rulesets/$($ruleset.ruleset_id)" -Headers $bearerHeaders -ContentType "application/json"
$latestVersion = $rulesetDetail.versions |
    Where-Object { $_.status -eq "ACTIVE" } |
    Sort-Object -Property version -Descending |
    Select-Object -First 1
if ($null -eq $latestVersion -or $latestVersion.version -ne $rulesetUpdate.version) {
    throw "Expected updated version to become ACTIVE after activation."
}

Write-Host "Create session..."
$sessionBody = @{
    session_name = "Smoke Session"
    mode = "PEMULA"
    ruleset_id = $ruleset.ruleset_id
}
$session = Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/v1/sessions" -Headers $bearerHeaders -ContentType "application/json" -Body ($sessionBody | ConvertTo-Json)

Write-Host "Create player..."
$playerStamp = [guid]::NewGuid().ToString("N").Substring(0, 8)
$playerBody = @{
    display_name = "Player Smoke"
    username = "smoke_player_$playerStamp"
    password = "SmokePlayerPass!123"
}
$player = Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/v1/players" -Headers $bearerHeaders -ContentType "application/json" -Body ($playerBody | ConvertTo-Json)

Write-Host "Add player to session..."
$addPlayerBody = @{
    player_id = $player.player_id
    role = "PLAYER"
    join_order = 1
}
Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/v1/sessions/$($session.session_id)/players" -Headers $bearerHeaders -ContentType "application/json" -Body ($addPlayerBody | ConvertTo-Json) | Out-Null

Write-Host "Start session..."
Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/v1/sessions/$($session.session_id)/start" -Headers $bearerHeaders -ContentType "application/json" | Out-Null

Write-Host "Post event..."
$eventBody = @{
    event_id = [guid]::NewGuid()
    session_id = $session.session_id
    player_id = $player.player_id
    actor_type = "PLAYER"
    timestamp = (Get-Date).ToUniversalTime().ToString("o")
    day_index = 0
    weekday = "MON"
    turn_number = 0
    sequence_number = 1
    action_type = "transaction.recorded"
    ruleset_version_id = $latestVersion.ruleset_version_id
    payload = @{
        direction = "IN"
        amount = 5
        category = "NEED_PRIMARY"
        counterparty = "BANK"
    }
}
Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/v1/events" -Headers $bearerHeaders -ContentType "application/json" -Body ($eventBody | ConvertTo-Json -Depth 6) | Out-Null

Write-Host "Fetch analytics..."
Invoke-RestMethod -Method Get -Uri "$ApiBaseUrl/api/v1/analytics/sessions/$($session.session_id)" -Headers $bearerHeaders -ContentType "application/json" | Out-Null

Write-Host "Smoke test selesai."
