param(
    [string]$ApiBaseUrl = "http://localhost:5041"
)

$ErrorActionPreference = "Stop"

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
$rulesetHeaders = @{ "X-Actor-Role" = "INSTRUCTOR" }
$ruleset = Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/rulesets" -Headers $rulesetHeaders -ContentType "application/json" -Body ($rulesetBody | ConvertTo-Json -Depth 8)

Write-Host "Get ruleset detail..."
$rulesetDetail = Invoke-RestMethod -Method Get -Uri "$ApiBaseUrl/api/rulesets/$($ruleset.ruleset_id)" -ContentType "application/json"
$latestVersion = $rulesetDetail.versions | Sort-Object -Property version -Descending | Select-Object -First 1

Write-Host "Create session..."
$sessionBody = @{
    session_name = "Smoke Session"
    mode = "PEMULA"
    ruleset_id = $ruleset.ruleset_id
}
$session = Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/sessions" -ContentType "application/json" -Body ($sessionBody | ConvertTo-Json)

Write-Host "Create player..."
$playerBody = @{
    display_name = "Player Smoke"
}
$player = Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/players" -ContentType "application/json" -Body ($playerBody | ConvertTo-Json)

Write-Host "Add player to session..."
$addPlayerBody = @{
    player_id = $player.player_id
    role = "PLAYER"
    join_order = 1
}
Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/sessions/$($session.session_id)/players" -ContentType "application/json" -Body ($addPlayerBody | ConvertTo-Json) | Out-Null

Write-Host "Start session..."
Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/sessions/$($session.session_id)/start" -ContentType "application/json" | Out-Null

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
Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/events" -ContentType "application/json" -Body ($eventBody | ConvertTo-Json -Depth 6) | Out-Null

Write-Host "Fetch analytics..."
Invoke-RestMethod -Method Get -Uri "$ApiBaseUrl/api/analytics/sessions/$($session.session_id)" -ContentType "application/json" | Out-Null

Write-Host "Smoke test selesai."
