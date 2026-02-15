param(
    [string]$ApiBaseUrl = "http://localhost:5041",
    [string]$InstructorUsername = "smoke_instructor",
    [string]$InstructorPassword = "SmokeInstructorPass!123"
)

$ErrorActionPreference = "Stop"

function Write-Step {
    param([string]$Message)
    Write-Host "==> $Message"
}

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
        # Registrasi bisa gagal jika user sudah ada atau registrasi instruktur dibatasi.
    }

    $token = Try-LoginToken -Username $Username -Password $Password
    if (-not [string]::IsNullOrWhiteSpace($token)) {
        return $token
    }

    throw "Gagal memperoleh token untuk user '$Username'."
}

function Invoke-Api {
    param(
        [string]$Method,
        [string]$Path,
        [hashtable]$Headers,
        [object]$Body = $null,
        [int]$JsonDepth = 12
    )

    $uri = "$ApiBaseUrl$Path"

    try {
        if ($null -eq $Body) {
            return Invoke-RestMethod -Method $Method -Uri $uri -Headers $Headers -ContentType "application/json"
        }

        $jsonBody = $Body | ConvertTo-Json -Depth $JsonDepth
        return Invoke-RestMethod -Method $Method -Uri $uri -Headers $Headers -ContentType "application/json" -Body $jsonBody
    }
    catch {
        $statusText = "unknown"
        if ($null -ne $_.Exception.Response -and $null -ne $_.Exception.Response.StatusCode) {
            $statusText = [string][int]$_.Exception.Response.StatusCode
        }

        $detail = $_.ErrorDetails.Message
        if ([string]::IsNullOrWhiteSpace($detail)) {
            $detail = $_.Exception.Message
        }

        throw "API $Method $Path gagal (status=$statusText). Detail: $detail"
    }
}

$simulationTag = (Get-Date).ToUniversalTime().ToString("yyyyMMddHHmmss")
Write-Step "Full game simulation dimulai [$simulationTag]"

Write-Step "Autentikasi instructor"
$instructorToken = Ensure-UserToken -Username $InstructorUsername -Password $InstructorPassword -Role "INSTRUCTOR"
$bearerHeaders = @{ Authorization = "Bearer $instructorToken" }

Write-Step "Siapkan 4 akun auth PLAYER (username + password)"
$playerAuthAccounts = @(
    [pscustomobject]@{ username = "sim_player_a_$simulationTag"; password = "SimPlayerA!123"; display_name = "Sim Player A $simulationTag" },
    [pscustomobject]@{ username = "sim_player_b_$simulationTag"; password = "SimPlayerB!123"; display_name = "Sim Player B $simulationTag" },
    [pscustomobject]@{ username = "sim_player_c_$simulationTag"; password = "SimPlayerC!123"; display_name = "Sim Player C $simulationTag" },
    [pscustomobject]@{ username = "sim_player_d_$simulationTag"; password = "SimPlayerD!123"; display_name = "Sim Player D $simulationTag" }
)

$baseConfig = @{
    mode = "MAHIR"
    actions_per_turn = 3
    starting_cash = 50
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
    donation = @{ min_amount = 1; max_amount = 20 }
    gold_trade = @{ allow_buy = $true; allow_sell = $true }
    advanced = @{
        loan = @{ enabled = $true }
        insurance = @{ enabled = $true }
        saving_goal = @{ enabled = $true }
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

Write-Step "Create ruleset"
$rulesetBody = @{
    name = "Full Simulation Ruleset $simulationTag"
    description = "Ruleset untuk simulasi penuh end-to-end"
    config = $baseConfig
}
$ruleset = Invoke-Api -Method Post -Path "/api/v1/rulesets" -Headers $bearerHeaders -Body $rulesetBody

Write-Step "Create DRAFT ruleset version (v2)"
$draftBody = @{
    name = "Full Simulation Ruleset $simulationTag v2"
    description = "Draft update ruleset untuk simulasi"
    config = $baseConfig
}
$draftBody.config.starting_cash = 55
$rulesetV2 = Invoke-Api -Method Put -Path "/api/v1/rulesets/$($ruleset.ruleset_id)" -Headers $bearerHeaders -Body $draftBody

Write-Step "Activate ruleset version v2"
Invoke-Api -Method Post -Path "/api/v1/rulesets/$($ruleset.ruleset_id)/versions/$($rulesetV2.version)/activate" -Headers $bearerHeaders | Out-Null

Write-Step "Create session"
$session = Invoke-Api -Method Post -Path "/api/v1/sessions" -Headers $bearerHeaders -Body @{
    session_name = "Full Simulation Session $simulationTag"
    mode = "MAHIR"
    ruleset_id = $ruleset.ruleset_id
}

Write-Step "Activate ruleset di level session"
Invoke-Api -Method Post -Path "/api/v1/sessions/$($session.session_id)/ruleset/activate" -Headers $bearerHeaders -Body @{
    ruleset_id = $ruleset.ruleset_id
    version = $rulesetV2.version
} | Out-Null

Write-Step "Ambil detail ruleset aktif"
$rulesetDetail = Invoke-Api -Method Get -Path "/api/v1/rulesets/$($ruleset.ruleset_id)" -Headers $bearerHeaders
$activeVersion = $rulesetDetail.versions |
    Where-Object { $_.version -eq $rulesetV2.version -and $_.status -eq "ACTIVE" } |
    Select-Object -First 1
if ($null -eq $activeVersion) {
    throw "Ruleset version aktif tidak ditemukan."
}

Write-Step "Create players (4 pemain)"
$player1 = Invoke-Api -Method Post -Path "/api/v1/players" -Headers $bearerHeaders -Body @{
    display_name = $playerAuthAccounts[0].display_name
    username = $playerAuthAccounts[0].username
    password = $playerAuthAccounts[0].password
}
$player2 = Invoke-Api -Method Post -Path "/api/v1/players" -Headers $bearerHeaders -Body @{
    display_name = $playerAuthAccounts[1].display_name
    username = $playerAuthAccounts[1].username
    password = $playerAuthAccounts[1].password
}
$player3 = Invoke-Api -Method Post -Path "/api/v1/players" -Headers $bearerHeaders -Body @{
    display_name = $playerAuthAccounts[2].display_name
    username = $playerAuthAccounts[2].username
    password = $playerAuthAccounts[2].password
}
$player4 = Invoke-Api -Method Post -Path "/api/v1/players" -Headers $bearerHeaders -Body @{
    display_name = $playerAuthAccounts[3].display_name
    username = $playerAuthAccounts[3].username
    password = $playerAuthAccounts[3].password
}

Write-Step "Add players ke session"
Invoke-Api -Method Post -Path "/api/v1/sessions/$($session.session_id)/players" -Headers $bearerHeaders -Body @{
    player_id = $player1.player_id
    role = "PLAYER"
    join_order = 1
} | Out-Null
Invoke-Api -Method Post -Path "/api/v1/sessions/$($session.session_id)/players" -Headers $bearerHeaders -Body @{
    player_id = $player2.player_id
    role = "PLAYER"
    join_order = 2
} | Out-Null
Invoke-Api -Method Post -Path "/api/v1/sessions/$($session.session_id)/players" -Headers $bearerHeaders -Body @{
    player_id = $player3.player_id
    role = "PLAYER"
    join_order = 3
} | Out-Null
Invoke-Api -Method Post -Path "/api/v1/sessions/$($session.session_id)/players" -Headers $bearerHeaders -Body @{
    player_id = $player4.player_id
    role = "PLAYER"
    join_order = 4
} | Out-Null

Write-Step "Start session"
Invoke-Api -Method Post -Path "/api/v1/sessions/$($session.session_id)/start" -Headers $bearerHeaders | Out-Null

$sequenceNumber = 1
$eventTimestamp = [DateTimeOffset]::UtcNow

function Post-Event {
    param(
        [Guid]$SessionId,
        [Guid]$RulesetVersionId,
        [Guid]$PlayerId,
        [string]$ActorType,
        [int]$DayIndex,
        [string]$Weekday,
        [int]$TurnNumber,
        [string]$ActionType,
        [hashtable]$Payload
    )

    $eventId = [Guid]::NewGuid()
    $eventBody = @{
        event_id = $eventId
        session_id = $SessionId
        player_id = if ($ActorType -eq "SYSTEM") { $null } else { $PlayerId }
        actor_type = $ActorType
        timestamp = $script:eventTimestamp.ToString("o")
        day_index = $DayIndex
        weekday = $Weekday
        turn_number = $TurnNumber
        sequence_number = $script:sequenceNumber
        action_type = $ActionType
        ruleset_version_id = $RulesetVersionId
        payload = $Payload
        client_request_id = "full-sim-$simulationTag-$script:sequenceNumber"
    }

    Invoke-Api -Method Post -Path "/api/v1/events" -Headers $script:bearerHeaders -Body $eventBody | Out-Null
    $script:sequenceNumber++
    $script:eventTimestamp = $script:eventTimestamp.AddSeconds(1)
    return $eventId
}

Write-Step "Kirim event gameplay lengkap"

# Turn 0 - Player 1
Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "mission.assigned" -Payload @{
    mission_id = "MISSION-A"
    target_tertiary_card_id = "NEED-T-001"
    penalty_points = 10
    require_primary = $true
    require_secondary = $true
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "tie_breaker.assigned" -Payload @{
    number = 9
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "transaction.recorded" -Payload @{
    direction = "IN"
    amount = 5
    category = "BONUS"
    counterparty = "BANK"
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "turn.action.used" -Payload @{
    used = 1
    remaining = 2
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "need.primary.purchased" -Payload @{
    card_id = "NP-001"
    amount = 5
    points = 2
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "need.secondary.purchased" -Payload @{
    card_id = "NS-001"
    amount = 3
    points = 1
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "need.tertiary.purchased" -Payload @{
    card_id = "NT-001"
    amount = 2
    points = 2
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "ingredient.purchased" -Payload @{
    card_id = "ING-001"
    ingredient_name = "Beras"
    amount = 1
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "ingredient.purchased" -Payload @{
    card_id = "ING-002"
    ingredient_name = "Ayam"
    amount = 1
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "order.claimed" -Payload @{
    order_card_id = "ORD-001"
    required_ingredient_card_ids = @("ING-001", "ING-002")
    income = 8
} | Out-Null

$riskEventId = Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "risk.life.drawn" -Payload @{
    risk_id = "RISK-001"
    direction = "OUT"
    amount = 3
    note = "Biaya kesehatan"
}

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "insurance.multirisk.purchased" -Payload @{
    policy_id = "INS-001"
    premium = 1
    coverage_type = "MULTIRISK"
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "insurance.multirisk.used" -Payload @{
    risk_event_id = $riskEventId
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "risk.emergency.used" -Payload @{
    risk_event_id = $riskEventId
    option_type = "SELL_NEED"
    direction = "IN"
    amount = 2
    note = "Menjual aset darurat"
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "loan.syariah.taken" -Payload @{
    loan_id = "LOAN-001"
    principal = 10
    installment = 10
    duration_turn = 1
    penalty_points = 15
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "loan.syariah.repaid" -Payload @{
    loan_id = "LOAN-001"
    amount = 5
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "saving.deposit.created" -Payload @{
    goal_id = "GOAL-001"
    amount = 10
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "saving.deposit.withdrawn" -Payload @{
    goal_id = "GOAL-001"
    amount = 2
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "saving.goal.achieved" -Payload @{
    goal_id = "GOAL-001"
    points = 5
    cost = 5
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 0 -Weekday "MON" -TurnNumber 0 -ActionType "work.freelance.completed" -Payload @{
    amount = 1
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 4 -Weekday "FRI" -TurnNumber 0 -ActionType "day.friday.donation" -Payload @{
    amount = 2
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 5 -Weekday "SAT" -TurnNumber 0 -ActionType "day.saturday.gold_trade" -Payload @{
    trade_type = "BUY"
    qty = 2
    unit_price = 4
    amount = 8
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 5 -Weekday "SAT" -TurnNumber 0 -ActionType "day.saturday.gold_trade" -Payload @{
    trade_type = "SELL"
    qty = 1
    unit_price = 5
    amount = 5
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 5 -Weekday "SAT" -TurnNumber 0 -ActionType "donation.rank.awarded" -Payload @{
    rank = 1
    points = 7
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 5 -Weekday "SAT" -TurnNumber 0 -ActionType "gold.points.awarded" -Payload @{
    points = 3
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "PLAYER" -DayIndex 5 -Weekday "SAT" -TurnNumber 0 -ActionType "pension.rank.awarded" -Payload @{
    rank = 2
    points = 3
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player1.player_id -ActorType "SYSTEM" -DayIndex 5 -Weekday "SAT" -TurnNumber 0 -ActionType "turn.ended" -Payload @{
    note = "Akhir giliran player A"
} | Out-Null

# Turn 1 - Player 2
Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player2.player_id -ActorType "PLAYER" -DayIndex 1 -Weekday "TUE" -TurnNumber 1 -ActionType "mission.assigned" -Payload @{
    mission_id = "MISSION-B"
    target_tertiary_card_id = "NEED-T-002"
    penalty_points = 10
    require_primary = $true
    require_secondary = $false
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player2.player_id -ActorType "PLAYER" -DayIndex 1 -Weekday "TUE" -TurnNumber 1 -ActionType "tie_breaker.assigned" -Payload @{
    number = 4
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player2.player_id -ActorType "PLAYER" -DayIndex 1 -Weekday "TUE" -TurnNumber 1 -ActionType "ingredient.purchased" -Payload @{
    card_id = "ING-010"
    ingredient_name = "Telur"
    amount = 2
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player2.player_id -ActorType "PLAYER" -DayIndex 1 -Weekday "TUE" -TurnNumber 1 -ActionType "ingredient.discarded" -Payload @{
    card_id = "ING-010"
    amount = 1
    reason = "HAND_LIMIT"
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player2.player_id -ActorType "PLAYER" -DayIndex 1 -Weekday "TUE" -TurnNumber 1 -ActionType "order.passed" -Payload @{
    order_card_id = "ORD-009"
    required_ingredient_card_ids = @("ING-010")
    income = 7
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player2.player_id -ActorType "PLAYER" -DayIndex 1 -Weekday "TUE" -TurnNumber 1 -ActionType "transaction.recorded" -Payload @{
    direction = "OUT"
    amount = 3
    category = "NEED_PRIMARY"
    counterparty = "BANK"
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player2.player_id -ActorType "PLAYER" -DayIndex 1 -Weekday "TUE" -TurnNumber 1 -ActionType "work.freelance.completed" -Payload @{
    amount = 1
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player2.player_id -ActorType "PLAYER" -DayIndex 1 -Weekday "TUE" -TurnNumber 1 -ActionType "turn.action.used" -Payload @{
    used = 1
    remaining = 2
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player2.player_id -ActorType "SYSTEM" -DayIndex 1 -Weekday "TUE" -TurnNumber 1 -ActionType "turn.ended" -Payload @{
    note = "Akhir giliran player B"
} | Out-Null

# Turn 2 - Player 3
Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player3.player_id -ActorType "PLAYER" -DayIndex 2 -Weekday "WED" -TurnNumber 2 -ActionType "mission.assigned" -Payload @{
    mission_id = "MISSION-C"
    target_tertiary_card_id = "NEED-T-003"
    penalty_points = 10
    require_primary = $true
    require_secondary = $true
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player3.player_id -ActorType "PLAYER" -DayIndex 2 -Weekday "WED" -TurnNumber 2 -ActionType "tie_breaker.assigned" -Payload @{
    number = 6
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player3.player_id -ActorType "PLAYER" -DayIndex 2 -Weekday "WED" -TurnNumber 2 -ActionType "transaction.recorded" -Payload @{
    direction = "IN"
    amount = 4
    category = "BONUS"
    counterparty = "BANK"
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player3.player_id -ActorType "PLAYER" -DayIndex 2 -Weekday "WED" -TurnNumber 2 -ActionType "work.freelance.completed" -Payload @{
    amount = 1
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player3.player_id -ActorType "SYSTEM" -DayIndex 2 -Weekday "WED" -TurnNumber 2 -ActionType "turn.ended" -Payload @{
    note = "Akhir giliran player C"
} | Out-Null

# Turn 3 - Player 4
Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player4.player_id -ActorType "PLAYER" -DayIndex 3 -Weekday "THU" -TurnNumber 3 -ActionType "mission.assigned" -Payload @{
    mission_id = "MISSION-D"
    target_tertiary_card_id = "NEED-T-004"
    penalty_points = 10
    require_primary = $true
    require_secondary = $true
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player4.player_id -ActorType "PLAYER" -DayIndex 3 -Weekday "THU" -TurnNumber 3 -ActionType "tie_breaker.assigned" -Payload @{
    number = 2
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player4.player_id -ActorType "PLAYER" -DayIndex 3 -Weekday "THU" -TurnNumber 3 -ActionType "need.primary.purchased" -Payload @{
    card_id = "NP-004"
    amount = 4
    points = 2
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player4.player_id -ActorType "PLAYER" -DayIndex 3 -Weekday "THU" -TurnNumber 3 -ActionType "work.freelance.completed" -Payload @{
    amount = 1
} | Out-Null

Post-Event -SessionId $session.session_id -RulesetVersionId $activeVersion.ruleset_version_id -PlayerId $player4.player_id -ActorType "SYSTEM" -DayIndex 3 -Weekday "THU" -TurnNumber 3 -ActionType "turn.ended" -Payload @{
    note = "Akhir giliran player D"
} | Out-Null

Write-Step "Ambil daftar endpoint utama"
$sessionList = Invoke-Api -Method Get -Path "/api/v1/sessions" -Headers $bearerHeaders
$playerList = Invoke-Api -Method Get -Path "/api/v1/players" -Headers $bearerHeaders
$rulesetList = Invoke-Api -Method Get -Path "/api/v1/rulesets" -Headers $bearerHeaders
$events = Invoke-Api -Method Get -Path "/api/v1/sessions/$($session.session_id)/events?fromSeq=0&limit=500" -Headers $bearerHeaders

Write-Step "Hitung dan ambil analytics"
$analyticsRecompute = Invoke-Api -Method Post -Path "/api/v1/analytics/sessions/$($session.session_id)/recompute" -Headers $bearerHeaders
$analyticsSession = Invoke-Api -Method Get -Path "/api/v1/analytics/sessions/$($session.session_id)" -Headers $bearerHeaders
$transactionHistory = Invoke-Api -Method Get -Path "/api/v1/analytics/sessions/$($session.session_id)/transactions" -Headers $bearerHeaders
$gameplayPlayer1 = Invoke-Api -Method Get -Path "/api/v1/analytics/sessions/$($session.session_id)/players/$($player1.player_id)/gameplay" -Headers $bearerHeaders
$rulesetSummary = Invoke-Api -Method Get -Path "/api/v1/analytics/rulesets/$($ruleset.ruleset_id)/summary" -Headers $bearerHeaders

Write-Step "End session"
Invoke-Api -Method Post -Path "/api/v1/sessions/$($session.session_id)/end" -Headers $bearerHeaders | Out-Null

Write-Host ""
Write-Host "===== HASIL SIMULASI FULL GAME ====="
Write-Host "Ruleset ID      : $($ruleset.ruleset_id)"
Write-Host "Session ID      : $($session.session_id)"
Write-Host "Player A ID     : $($player1.player_id)"
Write-Host "Player B ID     : $($player2.player_id)"
Write-Host "Player C ID     : $($player3.player_id)"
Write-Host "Player D ID     : $($player4.player_id)"
Write-Host "Versi ruleset   : $($rulesetV2.version) (ACTIVE)"
Write-Host "Total sessions  : $($sessionList.items.Count)"
Write-Host "Total players   : $($playerList.items.Count)"
Write-Host "Total rulesets  : $($rulesetList.items.Count)"
Write-Host "Total events    : $($events.events.Count)"
Write-Host "Total transaksi : $($transactionHistory.items.Count)"
Write-Host ""
Write-Host "Ringkasan sesi (recompute):"
$analyticsRecompute.summary | Format-List
Write-Host "Ringkasan sesi (final):"
$analyticsSession.summary | Format-List
Write-Host "By player:"
$analyticsSession.by_player |
    Select-Object player_id, cash_in_total, cash_out_total, donation_total, gold_qty, happiness_points_total, has_unpaid_loan |
    Format-Table -AutoSize

Write-Host "Gameplay snapshot Player A:"
Write-Host "computed_at: $($gameplayPlayer1.computed_at)"
Write-Host "raw tersedia     : $($null -ne $gameplayPlayer1.raw)"
Write-Host "derived tersedia : $($null -ne $gameplayPlayer1.derived)"

Write-Host ""
Write-Host "Ringkasan ruleset analytics:"
$rulesetSummary |
    Select-Object ruleset_id, ruleset_name, session_count, learning_performance_aggregate_score, mission_performance_aggregate_score |
    Format-List

Write-Host ""
Write-Host "Kredensial login 4 akun PLAYER:"
$playerAuthAccounts |
    Select-Object username, password |
    Format-Table -AutoSize

Write-Host "Full game simulation selesai tanpa error."
