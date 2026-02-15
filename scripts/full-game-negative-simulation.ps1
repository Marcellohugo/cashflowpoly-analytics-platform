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

function Invoke-ApiCapture {
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
            $response = Invoke-RestMethod -Method $Method -Uri $uri -Headers $Headers -ContentType "application/json"
        }
        else {
            $jsonBody = $Body | ConvertTo-Json -Depth $JsonDepth
            $response = Invoke-RestMethod -Method $Method -Uri $uri -Headers $Headers -ContentType "application/json" -Body $jsonBody
        }

        return [pscustomobject]@{
            ok = $true
            status = 200
            body = $response
            raw = $null
        }
    }
    catch {
        $status = 0
        if ($null -ne $_.Exception.Response -and $null -ne $_.Exception.Response.StatusCode) {
            $status = [int]$_.Exception.Response.StatusCode
        }

        $raw = $_.ErrorDetails.Message
        if ([string]::IsNullOrWhiteSpace($raw) -and $null -ne $_.Exception.Response) {
            try {
                $stream = $_.Exception.Response.GetResponseStream()
                if ($null -ne $stream) {
                    $reader = New-Object System.IO.StreamReader($stream)
                    $raw = $reader.ReadToEnd()
                    $reader.Close()
                }
            }
            catch {
                # abaikan parse body error
            }
        }

        $json = $null
        if (-not [string]::IsNullOrWhiteSpace($raw)) {
            try {
                $json = $raw | ConvertFrom-Json
            }
            catch {
                $json = $null
            }
        }

        return [pscustomobject]@{
            ok = $false
            status = $status
            body = $json
            raw = $raw
        }
    }
}

function Invoke-Api {
    param(
        [string]$Method,
        [string]$Path,
        [hashtable]$Headers,
        [object]$Body = $null
    )

    $result = Invoke-ApiCapture -Method $Method -Path $Path -Headers $Headers -Body $Body
    if (-not $result.ok) {
        $detail = if ($null -ne $result.body) { ($result.body | ConvertTo-Json -Depth 6) } else { $result.raw }
        throw "API $Method $Path gagal (status=$($result.status)). Detail: $detail"
    }

    return $result.body
}

function Add-NegativeCaseResult {
    param(
        [string]$CaseName,
        [int]$ExpectedStatus,
        [string]$ExpectedErrorCode,
        [pscustomobject]$ActualResult,
        [System.Collections.Generic.List[object]]$Results
    )

    $pass = $true
    $reason = "OK"

    if ($ActualResult.ok) {
        $pass = $false
        $reason = "Expected gagal tapi request sukses"
    }
    elseif ($ActualResult.status -ne $ExpectedStatus) {
        $pass = $false
        $reason = "Status mismatch. actual=$($ActualResult.status), expected=$ExpectedStatus"
    }
    elseif (-not [string]::IsNullOrWhiteSpace($ExpectedErrorCode)) {
        $actualErrorCode = $null
        if ($null -ne $ActualResult.body -and $null -ne $ActualResult.body.error_code) {
            $actualErrorCode = [string]$ActualResult.body.error_code
        }

        if ($actualErrorCode -ne $ExpectedErrorCode) {
            $pass = $false
            $reason = "Error code mismatch. actual='$actualErrorCode', expected='$ExpectedErrorCode'"
        }
    }

    $Results.Add([pscustomobject]@{
        case = $CaseName
        expected_status = $ExpectedStatus
        actual_status = if ($ActualResult.ok) { 200 } else { $ActualResult.status }
        expected_error_code = $ExpectedErrorCode
        actual_error_code = if ($null -ne $ActualResult.body) { [string]$ActualResult.body.error_code } else { "" }
        result = if ($pass) { "PASS" } else { "FAIL" }
        note = $reason
    }) | Out-Null

    return $pass
}

function New-EventBody {
    param(
        [Guid]$SessionId,
        [Guid]$RulesetVersionId,
        [Guid]$PlayerId,
        [string]$ActorType,
        [int]$DayIndex,
        [string]$Weekday,
        [int]$TurnNumber,
        [long]$SequenceNumber,
        [string]$ActionType,
        [hashtable]$Payload,
        [Guid]$EventId
    )

    return @{
        event_id = $EventId
        session_id = $SessionId
        player_id = if ($ActorType -eq "SYSTEM") { $null } else { $PlayerId }
        actor_type = $ActorType
        timestamp = $script:eventTimestamp.ToString("o")
        day_index = $DayIndex
        weekday = $Weekday
        turn_number = $TurnNumber
        sequence_number = $SequenceNumber
        action_type = $ActionType
        ruleset_version_id = $RulesetVersionId
        payload = $Payload
        client_request_id = "negative-sim-$script:simulationTag-$SequenceNumber"
    }
}

$simulationTag = (Get-Date).ToUniversalTime().ToString("yyyyMMddHHmmss")
Write-Step "Negative simulation dimulai [$simulationTag]"

Write-Step "Autentikasi instructor"
$instructorToken = Ensure-UserToken -Username $InstructorUsername -Password $InstructorPassword -Role "INSTRUCTOR"
$bearerHeaders = @{ Authorization = "Bearer $instructorToken" }

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
    donation = @{ min_amount = 1; max_amount = 20 }
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

Write-Step "Setup session untuk uji negatif"
$ruleset = Invoke-Api -Method Post -Path "/api/v1/rulesets" -Headers $bearerHeaders -Body @{
    name = "Negative Simulation Ruleset $simulationTag"
    description = "Ruleset untuk uji validasi negatif"
    config = $rulesetConfig
}

$session = Invoke-Api -Method Post -Path "/api/v1/sessions" -Headers $bearerHeaders -Body @{
    session_name = "Negative Simulation Session $simulationTag"
    mode = "PEMULA"
    ruleset_id = $ruleset.ruleset_id
}

$rulesetDetail = Invoke-Api -Method Get -Path "/api/v1/rulesets/$($ruleset.ruleset_id)" -Headers $bearerHeaders
$activeVersion = $rulesetDetail.versions | Where-Object { $_.status -eq "ACTIVE" } | Select-Object -First 1
if ($null -eq $activeVersion) {
    throw "Ruleset version ACTIVE tidak ditemukan."
}

$player = Invoke-Api -Method Post -Path "/api/v1/players" -Headers $bearerHeaders -Body @{
    display_name = "Negative Player $simulationTag"
    username = "negative_player_$simulationTag"
    password = "NegativePlayerPass!123"
}

Invoke-Api -Method Post -Path "/api/v1/sessions/$($session.session_id)/players" -Headers $bearerHeaders -Body @{
    player_id = $player.player_id
    role = "PLAYER"
    join_order = 1
} | Out-Null

Invoke-Api -Method Post -Path "/api/v1/sessions/$($session.session_id)/start" -Headers $bearerHeaders | Out-Null

$sequenceNumber = 1
$eventTimestamp = [DateTimeOffset]::UtcNow

Write-Step "Seed event valid untuk baseline sequence"
$seedEvent = New-EventBody -SessionId $session.session_id `
    -RulesetVersionId $activeVersion.ruleset_version_id `
    -PlayerId $player.player_id `
    -ActorType "PLAYER" `
    -DayIndex 0 `
    -Weekday "MON" `
    -TurnNumber 0 `
    -SequenceNumber $sequenceNumber `
    -ActionType "transaction.recorded" `
    -Payload @{
        direction = "IN"
        amount = 2
        category = "BONUS"
        counterparty = "BANK"
    } `
    -EventId ([Guid]::NewGuid())

Invoke-Api -Method Post -Path "/api/v1/events" -Headers $bearerHeaders -Body $seedEvent | Out-Null
$sequenceNumber++
$eventTimestamp = $eventTimestamp.AddSeconds(1)

$results = New-Object System.Collections.Generic.List[object]

Write-Step "Jalankan skenario negatif"

# Case 1: Duplicate event_id
$dupEventId = [Guid]::NewGuid()
$validDupSeed = New-EventBody -SessionId $session.session_id `
    -RulesetVersionId $activeVersion.ruleset_version_id `
    -PlayerId $player.player_id `
    -ActorType "PLAYER" `
    -DayIndex 0 `
    -Weekday "MON" `
    -TurnNumber 0 `
    -SequenceNumber $sequenceNumber `
    -ActionType "transaction.recorded" `
    -Payload @{
        direction = "IN"
        amount = 3
        category = "BONUS"
        counterparty = "BANK"
    } `
    -EventId $dupEventId

Invoke-Api -Method Post -Path "/api/v1/events" -Headers $bearerHeaders -Body $validDupSeed | Out-Null
$sequenceNumber++
$eventTimestamp = $eventTimestamp.AddSeconds(1)

$duplicateResult = Invoke-ApiCapture -Method Post -Path "/api/v1/events" -Headers $bearerHeaders -Body $validDupSeed
[void](Add-NegativeCaseResult -CaseName "Duplicate event_id" -ExpectedStatus 409 -ExpectedErrorCode "DUPLICATE" -ActualResult $duplicateResult -Results $results)
$eventTimestamp = $eventTimestamp.AddSeconds(1)

# Case 2: Sequence loncat
$seqJumpBody = New-EventBody -SessionId $session.session_id `
    -RulesetVersionId $activeVersion.ruleset_version_id `
    -PlayerId $player.player_id `
    -ActorType "PLAYER" `
    -DayIndex 0 `
    -Weekday "MON" `
    -TurnNumber 0 `
    -SequenceNumber ($sequenceNumber + 2) `
    -ActionType "transaction.recorded" `
    -Payload @{
        direction = "IN"
        amount = 1
        category = "BONUS"
        counterparty = "BANK"
    } `
    -EventId ([Guid]::NewGuid())

$seqJumpResult = Invoke-ApiCapture -Method Post -Path "/api/v1/events" -Headers $bearerHeaders -Body $seqJumpBody
[void](Add-NegativeCaseResult -CaseName "Sequence loncat" -ExpectedStatus 422 -ExpectedErrorCode "DOMAIN_RULE_VIOLATION" -ActualResult $seqJumpResult -Results $results)
$eventTimestamp = $eventTimestamp.AddSeconds(1)

# Case 3: Donation weekday salah
$donationWeekdayBody = New-EventBody -SessionId $session.session_id `
    -RulesetVersionId $activeVersion.ruleset_version_id `
    -PlayerId $player.player_id `
    -ActorType "PLAYER" `
    -DayIndex 1 `
    -Weekday "MON" `
    -TurnNumber 0 `
    -SequenceNumber $sequenceNumber `
    -ActionType "day.friday.donation" `
    -Payload @{ amount = 2 } `
    -EventId ([Guid]::NewGuid())

$donationWeekdayResult = Invoke-ApiCapture -Method Post -Path "/api/v1/events" -Headers $bearerHeaders -Body $donationWeekdayBody
[void](Add-NegativeCaseResult -CaseName "Donation weekday salah" -ExpectedStatus 400 -ExpectedErrorCode "VALIDATION_ERROR" -ActualResult $donationWeekdayResult -Results $results)
$eventTimestamp = $eventTimestamp.AddSeconds(1)

# Case 4: Action token melebihi ruleset
$tokenExceedBody = New-EventBody -SessionId $session.session_id `
    -RulesetVersionId $activeVersion.ruleset_version_id `
    -PlayerId $player.player_id `
    -ActorType "PLAYER" `
    -DayIndex 1 `
    -Weekday "TUE" `
    -TurnNumber 1 `
    -SequenceNumber $sequenceNumber `
    -ActionType "turn.action.used" `
    -Payload @{
        used = 3
        remaining = 0
    } `
    -EventId ([Guid]::NewGuid())

$tokenExceedResult = Invoke-ApiCapture -Method Post -Path "/api/v1/events" -Headers $bearerHeaders -Body $tokenExceedBody
[void](Add-NegativeCaseResult -CaseName "Action token melebihi batas" -ExpectedStatus 422 -ExpectedErrorCode "DOMAIN_RULE_VIOLATION" -ActualResult $tokenExceedResult -Results $results)
$eventTimestamp = $eventTimestamp.AddSeconds(1)

# Case 5: Need secondary tanpa primary
$needSecondaryBody = New-EventBody -SessionId $session.session_id `
    -RulesetVersionId $activeVersion.ruleset_version_id `
    -PlayerId $player.player_id `
    -ActorType "PLAYER" `
    -DayIndex 2 `
    -Weekday "WED" `
    -TurnNumber 2 `
    -SequenceNumber $sequenceNumber `
    -ActionType "need.secondary.purchased" `
    -Payload @{
        card_id = "NS-NEG-1"
        amount = 2
        points = 1
    } `
    -EventId ([Guid]::NewGuid())

$needSecondaryResult = Invoke-ApiCapture -Method Post -Path "/api/v1/events" -Headers $bearerHeaders -Body $needSecondaryBody
[void](Add-NegativeCaseResult -CaseName "Need secondary tanpa primary" -ExpectedStatus 422 -ExpectedErrorCode "DOMAIN_RULE_VIOLATION" -ActualResult $needSecondaryResult -Results $results)
$eventTimestamp = $eventTimestamp.AddSeconds(1)

# Case 6: Gold trade amount mismatch
$goldMismatchBody = New-EventBody -SessionId $session.session_id `
    -RulesetVersionId $activeVersion.ruleset_version_id `
    -PlayerId $player.player_id `
    -ActorType "PLAYER" `
    -DayIndex 5 `
    -Weekday "SAT" `
    -TurnNumber 2 `
    -SequenceNumber $sequenceNumber `
    -ActionType "day.saturday.gold_trade" `
    -Payload @{
        trade_type = "BUY"
        qty = 2
        unit_price = 4
        amount = 5
    } `
    -EventId ([Guid]::NewGuid())

$goldMismatchResult = Invoke-ApiCapture -Method Post -Path "/api/v1/events" -Headers $bearerHeaders -Body $goldMismatchBody
[void](Add-NegativeCaseResult -CaseName "Gold trade amount mismatch" -ExpectedStatus 422 -ExpectedErrorCode "DOMAIN_RULE_VIOLATION" -ActualResult $goldMismatchResult -Results $results)
$eventTimestamp = $eventTimestamp.AddSeconds(1)

# Case 7: Saving goal event saat fitur disabled
$savingDisabledBody = New-EventBody -SessionId $session.session_id `
    -RulesetVersionId $activeVersion.ruleset_version_id `
    -PlayerId $player.player_id `
    -ActorType "PLAYER" `
    -DayIndex 2 `
    -Weekday "WED" `
    -TurnNumber 2 `
    -SequenceNumber $sequenceNumber `
    -ActionType "saving.deposit.created" `
    -Payload @{
        goal_id = "GOAL-NEG-1"
        amount = 5
    } `
    -EventId ([Guid]::NewGuid())

$savingDisabledResult = Invoke-ApiCapture -Method Post -Path "/api/v1/events" -Headers $bearerHeaders -Body $savingDisabledBody
[void](Add-NegativeCaseResult -CaseName "Saving goal disabled" -ExpectedStatus 422 -ExpectedErrorCode "DOMAIN_RULE_VIOLATION" -ActualResult $savingDisabledResult -Results $results)
$eventTimestamp = $eventTimestamp.AddSeconds(1)

# Case 8: Loan event saat fitur disabled
$loanDisabledBody = New-EventBody -SessionId $session.session_id `
    -RulesetVersionId $activeVersion.ruleset_version_id `
    -PlayerId $player.player_id `
    -ActorType "PLAYER" `
    -DayIndex 2 `
    -Weekday "WED" `
    -TurnNumber 2 `
    -SequenceNumber $sequenceNumber `
    -ActionType "loan.syariah.taken" `
    -Payload @{
        loan_id = "LOAN-NEG-1"
        principal = 10
        installment = 10
        duration_turn = 1
        penalty_points = 15
    } `
    -EventId ([Guid]::NewGuid())

$loanDisabledResult = Invoke-ApiCapture -Method Post -Path "/api/v1/events" -Headers $bearerHeaders -Body $loanDisabledBody
[void](Add-NegativeCaseResult -CaseName "Loan disabled" -ExpectedStatus 422 -ExpectedErrorCode "DOMAIN_RULE_VIOLATION" -ActualResult $loanDisabledResult -Results $results)
$eventTimestamp = $eventTimestamp.AddSeconds(1)

# Case 9: Risk event pada mode PEMULA
$riskPemulaBody = New-EventBody -SessionId $session.session_id `
    -RulesetVersionId $activeVersion.ruleset_version_id `
    -PlayerId $player.player_id `
    -ActorType "PLAYER" `
    -DayIndex 2 `
    -Weekday "WED" `
    -TurnNumber 2 `
    -SequenceNumber $sequenceNumber `
    -ActionType "risk.life.drawn" `
    -Payload @{
        risk_id = "RISK-NEG-1"
        direction = "OUT"
        amount = 3
    } `
    -EventId ([Guid]::NewGuid())

$riskPemulaResult = Invoke-ApiCapture -Method Post -Path "/api/v1/events" -Headers $bearerHeaders -Body $riskPemulaBody
[void](Add-NegativeCaseResult -CaseName "Risk hanya mode MAHIR" -ExpectedStatus 422 -ExpectedErrorCode "DOMAIN_RULE_VIOLATION" -ActualResult $riskPemulaResult -Results $results)
$eventTimestamp = $eventTimestamp.AddSeconds(1)

# Case 10: Counterparty invalid
$counterpartyInvalidBody = New-EventBody -SessionId $session.session_id `
    -RulesetVersionId $activeVersion.ruleset_version_id `
    -PlayerId $player.player_id `
    -ActorType "PLAYER" `
    -DayIndex 2 `
    -Weekday "WED" `
    -TurnNumber 2 `
    -SequenceNumber $sequenceNumber `
    -ActionType "transaction.recorded" `
    -Payload @{
        direction = "IN"
        amount = 2
        category = "BONUS"
        counterparty = "SHOP"
    } `
    -EventId ([Guid]::NewGuid())

$counterpartyInvalidResult = Invoke-ApiCapture -Method Post -Path "/api/v1/events" -Headers $bearerHeaders -Body $counterpartyInvalidBody
[void](Add-NegativeCaseResult -CaseName "Counterparty invalid" -ExpectedStatus 400 -ExpectedErrorCode "VALIDATION_ERROR" -ActualResult $counterpartyInvalidResult -Results $results)
$eventTimestamp = $eventTimestamp.AddSeconds(1)

# Case 11: Saldo tidak cukup
$insufficientBalanceBody = New-EventBody -SessionId $session.session_id `
    -RulesetVersionId $activeVersion.ruleset_version_id `
    -PlayerId $player.player_id `
    -ActorType "PLAYER" `
    -DayIndex 2 `
    -Weekday "WED" `
    -TurnNumber 2 `
    -SequenceNumber $sequenceNumber `
    -ActionType "transaction.recorded" `
    -Payload @{
        direction = "OUT"
        amount = 999
        category = "NEED_PRIMARY"
        counterparty = "BANK"
    } `
    -EventId ([Guid]::NewGuid())

$insufficientBalanceResult = Invoke-ApiCapture -Method Post -Path "/api/v1/events" -Headers $bearerHeaders -Body $insufficientBalanceBody
[void](Add-NegativeCaseResult -CaseName "Saldo tidak cukup" -ExpectedStatus 422 -ExpectedErrorCode "DOMAIN_RULE_VIOLATION" -ActualResult $insufficientBalanceResult -Results $results)
$eventTimestamp = $eventTimestamp.AddSeconds(1)

Write-Step "End session"
Invoke-Api -Method Post -Path "/api/v1/sessions/$($session.session_id)/end" -Headers $bearerHeaders | Out-Null

$failedCount = ($results | Where-Object { $_.result -eq "FAIL" }).Count
$passedCount = ($results | Where-Object { $_.result -eq "PASS" }).Count

Write-Host ""
Write-Host "===== HASIL NEGATIVE SIMULATION ====="
Write-Host "Ruleset ID  : $($ruleset.ruleset_id)"
Write-Host "Session ID  : $($session.session_id)"
Write-Host "Player ID   : $($player.player_id)"
Write-Host "Total case  : $($results.Count)"
Write-Host "PASS        : $passedCount"
Write-Host "FAIL        : $failedCount"
Write-Host ""
$results | Format-Table -AutoSize

if ($failedCount -gt 0) {
    throw "Negative simulation selesai dengan $failedCount kegagalan verifikasi."
}

Write-Host ""
Write-Host "Negative simulation selesai. Semua case validasi sesuai ekspektasi."
