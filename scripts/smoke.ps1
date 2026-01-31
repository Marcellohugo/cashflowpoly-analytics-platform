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
    }
}

Write-Host "Create ruleset..."
$ruleset = Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/rulesets" -ContentType "application/json" -Body ($rulesetBody | ConvertTo-Json -Depth 6)

Write-Host "Create session..."
$sessionBody = @{
    session_name = "Smoke Session"
    mode = "PEMULA"
    ruleset_id = $ruleset.ruleset_id
}
$session = Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/sessions" -ContentType "application/json" -Body ($sessionBody | ConvertTo-Json)

Write-Host "Start session..."
Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/sessions/$($session.session_id)/start" -ContentType "application/json" | Out-Null

Write-Host "Smoke test selesai."
