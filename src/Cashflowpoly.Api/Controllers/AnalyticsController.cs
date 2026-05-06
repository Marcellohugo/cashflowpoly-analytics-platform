// Fungsi file: Menyediakan endpoint analitika sesi (ringkasan, per-pemain, transaksi, metrik gameplay, ringkasan ruleset) dengan komputasi skor performa dan snapshot metrik.
using System.Text.Json;
using System.Security.Claims;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Cashflowpoly.Api.Domain.AnalyticsPayloadReader;
using static Cashflowpoly.Api.Domain.AnalyticsScoreCalculator;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api/v1/analytics")]
[Authorize]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
/// <summary>
/// Controller analitika yang menyediakan endpoint ringkasan sesi, analitik per pemain, histori transaksi, metrik gameplay, dan ringkasan ruleset.
/// </summary>
public sealed class AnalyticsController : ControllerBase
{
    private readonly SessionRepository _sessions;
    private readonly EventRepository _events;
    private readonly RulesetRepository _rulesets;
    private readonly MetricsRepository _metrics;
    private readonly PlayerRepository _players;
    private readonly UserRepository _users;
    /// <summary>
    /// Menginisialisasi controller dengan dependensi repositori sesi, event, ruleset, metrik, pemain, dan user.
    /// </summary>
    public AnalyticsController(
        SessionRepository sessions,
        EventRepository events,
        RulesetRepository rulesets,
        MetricsRepository metrics,
        PlayerRepository players,
        UserRepository users)
    {
        _sessions = sessions;
        _events = events;
        _rulesets = rulesets;
        _metrics = metrics;
        _players = players;
        _users = users;
    }

    /// <summary>
    /// Menghitung ulang metrik dan menyimpan snapshot terbaru.
    /// </summary>
    [HttpPost("sessions/{sessionId:guid}/recompute")]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(AnalyticsSessionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Recompute(Guid sessionId, CancellationToken ct)
    {
        var instructorScopeError = await EnsureInstructorSessionAccessAsync(sessionId, ct);
        if (instructorScopeError is not null)
        {
            return instructorScopeError;
        }

        var session = await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
        }

        var events = await _events.GetAllEventsBySessionAsync(sessionId, ct);
        var projections = await _events.GetCashflowProjectionsAsync(sessionId, ct);
        var violations = await _metrics.CountValidationViolationsAsync(sessionId, null, ct);

        var activeRulesetVersionId = await _sessions.GetActiveRulesetVersionIdAsync(sessionId, ct);
        RulesetConfig? config = null;
        Guid? activeRulesetId = null;
        string? activeRulesetName = null;
        if (activeRulesetVersionId.HasValue)
        {
            var rulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(activeRulesetVersionId.Value, ct);
            if (rulesetVersion is not null)
            {
                activeRulesetId = rulesetVersion.RulesetId;
                var ruleset = await _rulesets.GetRulesetAsync(rulesetVersion.RulesetId, ct);
                activeRulesetName = ruleset?.Name;

                if (RulesetConfigParser.TryParse(rulesetVersion.ConfigJson, out var parsed, out _))
                {
                    config = parsed;
                }
            }
        }

        var happinessByPlayer = AnalyticsHappinessCalculator.ComputeByPlayer(events, projections, config);
        var summary = BuildSummary(events, projections, violations);
        var playerJoinOrders = await _players.GetSessionPlayerJoinOrderMapAsync(sessionId, ct);
        var byPlayer = await BuildByPlayerAsync(sessionId, events, projections, happinessByPlayer, config, playerJoinOrders, ct);

        if (activeRulesetVersionId.HasValue)
        {
            await WriteSnapshotsAsync(sessionId, activeRulesetVersionId.Value, events, projections, config, happinessByPlayer, ct);
        }

        return Ok(new AnalyticsSessionResponse(sessionId, summary, byPlayer, activeRulesetId, activeRulesetName));
    }

    [HttpGet("sessions/{sessionId:guid}")]
    [ProducesResponseType(typeof(AnalyticsSessionResponse), StatusCodes.Status200OK)]
    /// <summary>
    /// Mengambil ringkasan analitik sesi termasuk skor per pemain, cashflow, dan happiness.
    /// </summary>
    public async Task<IActionResult> GetSessionAnalytics(Guid sessionId, CancellationToken ct)
    {
        var instructorScopeError = await EnsureInstructorSessionAccessAsync(sessionId, ct);
        if (instructorScopeError is not null)
        {
            return instructorScopeError;
        }

        var session = await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
        }

        var scope = await ResolvePlayerScopeAsync(sessionId, ct);
        if (scope.Error is not null)
        {
            return scope.Error;
        }

        var events = await _events.GetAllEventsBySessionAsync(sessionId, ct);
        var projections = await _events.GetCashflowProjectionsAsync(sessionId, ct);
        var violations = await _metrics.CountValidationViolationsAsync(sessionId, null, ct);

        var activeRulesetVersionId = await _sessions.GetActiveRulesetVersionIdAsync(sessionId, ct);
        RulesetConfig? config = null;
        Guid? activeRulesetId = null;
        string? activeRulesetName = null;
        if (activeRulesetVersionId.HasValue)
        {
            var rulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(activeRulesetVersionId.Value, ct);
            if (rulesetVersion is not null)
            {
                activeRulesetId = rulesetVersion.RulesetId;
                var ruleset = await _rulesets.GetRulesetAsync(rulesetVersion.RulesetId, ct);
                activeRulesetName = ruleset?.Name;

                if (RulesetConfigParser.TryParse(rulesetVersion.ConfigJson, out var parsed, out _))
                {
                    config = parsed;
                }
            }
        }

        var happinessByPlayer = AnalyticsHappinessCalculator.ComputeByPlayer(events, projections, config);
        var summary = BuildSummary(events, projections, violations);
        var playerJoinOrders = await _players.GetSessionPlayerJoinOrderMapAsync(sessionId, ct);
        var byPlayer = await BuildByPlayerAsync(sessionId, events, projections, happinessByPlayer, config, playerJoinOrders, ct);
        if (scope.PlayerId.HasValue)
        {
            byPlayer = byPlayer.Where(item => item.PlayerId == scope.PlayerId.Value).ToList();
        }

        return Ok(new AnalyticsSessionResponse(sessionId, summary, byPlayer, activeRulesetId, activeRulesetName));
    }

    [HttpGet("sessions/{sessionId:guid}/transactions")]
    [ProducesResponseType(typeof(TransactionHistoryResponse), StatusCodes.Status200OK)]
    /// <summary>
    /// Mengambil histori transaksi cashflow per sesi dengan opsi filter per pemain.
    /// </summary>
    public async Task<IActionResult> GetTransactions(Guid sessionId, [FromQuery] Guid? playerId = null, CancellationToken ct = default)
    {
        var instructorScopeError = await EnsureInstructorSessionAccessAsync(sessionId, ct);
        if (instructorScopeError is not null)
        {
            return instructorScopeError;
        }

        var session = await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
        }

        var scope = await ResolvePlayerScopeAsync(sessionId, ct);
        if (scope.Error is not null)
        {
            return scope.Error;
        }

        var effectivePlayerId = scope.PlayerId ?? playerId;

        var projections = await _events.GetCashflowProjectionsAsync(sessionId, ct);
        var items = projections
            .Where(p => !effectivePlayerId.HasValue || p.PlayerId == effectivePlayerId.Value)
            .OrderBy(p => p.Timestamp)
            .Select(p => new TransactionHistoryItem(p.Timestamp, p.Direction, p.Amount, p.Category))
            .ToList();

        return Ok(new TransactionHistoryResponse(items));
    }

    [HttpGet("sessions/{sessionId:guid}/players/{playerId:guid}/gameplay")]
    [ProducesResponseType(typeof(GameplayMetricsResponse), StatusCodes.Status200OK)]
    /// <summary>
    /// Mengambil snapshot metrik gameplay mentah dan turunan untuk pemain tertentu dalam sesi.
    /// </summary>
    public async Task<IActionResult> GetGameplayMetrics(Guid sessionId, Guid playerId, CancellationToken ct)
    {
        var instructorScopeError = await EnsureInstructorSessionAccessAsync(sessionId, ct);
        if (instructorScopeError is not null)
        {
            return instructorScopeError;
        }

        var session = await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
        }

        var scope = await ResolvePlayerScopeAsync(sessionId, ct);
        if (scope.Error is not null)
        {
            return scope.Error;
        }

        if (scope.PlayerId.HasValue && scope.PlayerId.Value != playerId)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Player hanya dapat melihat metrik miliknya"));
        }

        var snapshots = await _metrics.GetLatestGameplaySnapshotsAsync(sessionId, playerId, ct);
        var rawJson = snapshots.FirstOrDefault(item => item.MetricName == "gameplay.raw.variables")?.MetricValueJson;
        var derivedJson = snapshots.FirstOrDefault(item => item.MetricName == "gameplay.derived.metrics")?.MetricValueJson;
        var computedAt = snapshots.Count == 0 ? (DateTimeOffset?)null : snapshots.Max(item => item.ComputedAt);

        return Ok(new GameplayMetricsResponse(
            sessionId,
            playerId,
            computedAt,
            ParseJsonElement(rawJson),
            ParseJsonElement(derivedJson)));
    }

    [HttpGet("rulesets/{rulesetId:guid}/summary")]
    [ProducesResponseType(typeof(RulesetAnalyticsSummaryResponse), StatusCodes.Status200OK)]
    /// <summary>
    /// Mengambil ringkasan analitik agregat untuk semua sesi yang menggunakan ruleset tertentu.
    /// </summary>
    public async Task<IActionResult> GetRulesetAnalyticsSummary(Guid rulesetId, CancellationToken ct)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var isInstructor = string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase);
        var isPlayer = string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase);
        RulesetDb? ruleset = null;
        List<SessionDb> sessions;
        Guid? scopedPlayerId = null;

        if (isInstructor)
        {
            if (!TryGetCurrentUserId(out var instructorUserId))
            {
                return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
            }

            ruleset = await _rulesets.GetRulesetForInstructorAsync(rulesetId, instructorUserId, ct);
            if (ruleset is null)
            {
                return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan"));
            }

            sessions = await _sessions.ListSessionsByInstructorAsync(instructorUserId, ct);
        }
        else if (isPlayer)
        {
            var scope = await ResolvePlayerScopeAsync(null, ct);
            if (scope.Error is not null)
            {
                return scope.Error;
            }

            scopedPlayerId = scope.PlayerId;
            sessions = await _sessions.ListSessionsAsync(ct);
        }
        else
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak diizinkan"));
        }

        var sessionItems = new List<RulesetAnalyticsSessionItem>();

        foreach (var session in sessions)
        {
            var activeRulesetVersionId = await _sessions.GetActiveRulesetVersionIdAsync(session.SessionId, ct);
            if (!activeRulesetVersionId.HasValue)
            {
                continue;
            }

            var activeVersion = await _rulesets.GetRulesetVersionByIdAsync(activeRulesetVersionId.Value, ct);
            if (activeVersion is null || activeVersion.RulesetId != rulesetId)
            {
                continue;
            }

            if (scopedPlayerId.HasValue)
            {
                var inSession = await _players.IsPlayerInSessionAsync(session.SessionId, scopedPlayerId.Value, ct);
                if (!inSession)
                {
                    continue;
                }
            }

            var events = await _events.GetAllEventsBySessionAsync(session.SessionId, ct);
            var projections = await _events.GetCashflowProjectionsAsync(session.SessionId, ct);
            RulesetConfig? config = null;
            if (RulesetConfigParser.TryParse(activeVersion.ConfigJson, out var parsed, out _))
            {
                config = parsed;
            }

            var happinessByPlayer = AnalyticsHappinessCalculator.ComputeByPlayer(events, projections, config);
            var playerJoinOrders = await _players.GetSessionPlayerJoinOrderMapAsync(session.SessionId, ct);
            var byPlayer = await BuildByPlayerAsync(session.SessionId, events, projections, happinessByPlayer, config, playerJoinOrders, ct);
            var allPlayerItems = new List<RulesetAnalyticsPlayerItem>();

            foreach (var player in byPlayer)
            {
                var complianceRate = await _metrics.GetLatestMetricNumericAsync(
                    session.SessionId,
                    player.PlayerId,
                    "compliance.primary_need.rate",
                    ct);

                var learningScore = ComputeLearningPerformanceScore(
                    player.CashInTotal,
                    player.CashOutTotal,
                    player.HappinessPointsTotal,
                    complianceRate);

                var missionScore = ComputeMissionPerformanceScore(
                    player.MissionPenaltyTotal,
                    player.LoanPenaltyTotal);

                allPlayerItems.Add(new RulesetAnalyticsPlayerItem(player.PlayerId, learningScore, missionScore));
            }

            var learningAggregate = AverageNullable(allPlayerItems.Select(item => item.LearningPerformanceIndividualScore));
            var missionAggregate = AverageNullable(allPlayerItems.Select(item => item.MissionPerformanceIndividualScore));
            var visiblePlayers = scopedPlayerId.HasValue
                ? allPlayerItems.Where(item => item.PlayerId == scopedPlayerId.Value).ToList()
                : allPlayerItems;

            sessionItems.Add(new RulesetAnalyticsSessionItem(
                session.SessionId,
                session.SessionName,
                session.Status,
                events.Count,
                learningAggregate,
                missionAggregate,
                visiblePlayers));
        }

        if (isPlayer && sessionItems.Count == 0)
        {
            // Jangan menyingkap ruleset yang tidak pernah dipakai oleh sesi player ini.
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan"));
        }

        if (!isInstructor)
        {
            ruleset = await _rulesets.GetRulesetAsync(rulesetId, ct);
            if (ruleset is null)
            {
                return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan"));
            }
        }
        else if (ruleset is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan"));
        }

        var learningOverall = AverageNullable(sessionItems.Select(item => item.LearningPerformanceAggregateScore));
        var missionOverall = AverageNullable(sessionItems.Select(item => item.MissionPerformanceAggregateScore));

        return Ok(new RulesetAnalyticsSummaryResponse(
            rulesetId,
            ruleset.Name,
            sessionItems.Count,
            learningOverall,
            missionOverall,
            sessionItems));
    }

    /// <summary>
    /// Menentukan scope pemain: null untuk instruktur (semua pemain), playerId untuk role PLAYER.
    /// </summary>
    private async Task<(Guid? PlayerId, IActionResult? Error)> ResolvePlayerScopeAsync(Guid? sessionId, CancellationToken ct)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase))
        {
            return (null, null);
        }

        if (!string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            return (null, StatusCode(StatusCodes.Status403Forbidden,
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak dikenali")));
        }

        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            return (null, Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid")));
        }

        var playerId = await _users.GetLinkedPlayerIdAsync(userId, ct);
        if (!playerId.HasValue)
        {
            return (null, StatusCode(StatusCodes.Status403Forbidden,
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Akun PLAYER belum terhubung ke profil pemain")));
        }

        if (sessionId.HasValue)
        {
            var inSession = await _players.IsPlayerInSessionAsync(sessionId.Value, playerId.Value, ct);
            if (!inSession)
            {
                return (null, StatusCode(StatusCodes.Status403Forbidden,
                    ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Player tidak terdaftar di sesi ini")));
            }
        }

        return (playerId.Value, null);
    }

    /// <summary>
    /// Memvalidasi bahwa instruktur memiliki akses ke sesi yang diminta.
    /// </summary>
    private async Task<IActionResult?> EnsureInstructorSessionAccessAsync(Guid sessionId, CancellationToken ct)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (!string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak dikenali"));
        }

        if (!TryGetCurrentUserId(out var instructorUserId))
        {
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        }

        var session = await _sessions.GetSessionForInstructorAsync(sessionId, instructorUserId, ct);
        if (session is null)
        {
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Session tidak ditemukan"));
        }

        return null;
    }

    /// <summary>
    /// Mencoba mengekstrak user ID dari claim JWT NameIdentifier.
    /// </summary>
    private bool TryGetCurrentUserId(out Guid userId)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdRaw, out userId);
    }

    /// <summary>
    /// Mem-parse string JSON menjadi JsonElement, mengembalikan null jika string kosong.
    /// </summary>
    private static JsonElement? ParseJsonElement(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }

    /// <summary>
    /// Membangun daftar analitik per pemain berisi cashflow, happiness, compliance, dan skor performa.
    /// </summary>
    private async Task<List<AnalyticsByPlayerItem>> BuildByPlayerAsync(
        Guid sessionId,
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        Dictionary<Guid, AnalyticsHappinessBreakdown> happinessByPlayer,
        RulesetConfig? config,
        Dictionary<Guid, int> playerJoinOrders,
        CancellationToken ct)
    {
        var cashTotals = projections
            .GroupBy(p => p.PlayerId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    In = g.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount),
                    Out = g.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount)
                });

        var result = new List<AnalyticsByPlayerItem>();
        var eventsByPlayer = events.Where(e => e.PlayerId.HasValue)
            .GroupBy(e => e.PlayerId!.Value)
            .ToDictionary(group => group.Key, group => group.ToList());
        var playerIds = playerJoinOrders.Keys
            .Union(eventsByPlayer.Keys)
            .Distinct()
            .ToList();
        var firstEventSequenceByPlayer = playerIds.ToDictionary(
            playerId => playerId,
            playerId => eventsByPlayer.TryGetValue(playerId, out var items) && items.Count > 0
                ? items.Min(item => item.SequenceNumber)
                : long.MaxValue);
        var usernamesByPlayer = await _users.GetUsernamesByPlayerIdsAsync(playerIds, ct);

        foreach (var playerId in playerIds)
        {
            var playerEvents = eventsByPlayer.TryGetValue(playerId, out var items)
                ? items
                : new List<EventDb>();
            var joinOrder = playerJoinOrders.TryGetValue(playerId, out var assignedJoinOrder) ? assignedJoinOrder : 0;

            var totals = cashTotals.TryGetValue(playerId, out var t) ? t : new { In = 0d, Out = 0d };
            var donationTotal = playerEvents.Where(e => e.ActionType == "day.friday.donation")
                .Select(e => TryReadAmount(e.Payload, out var amount) ? amount : 0)
                .Sum();

            var goldQty = playerEvents.Where(e => e.ActionType == "day.saturday.gold_trade")
                .Select(e =>
                {
                    if (!TryReadGoldTrade(e.Payload, out var tradeType, out var qty))
                    {
                        return 0;
                    }

                    return string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) ? qty : -qty;
                })
                .Sum();

            var ordersCompletedCount = playerEvents.Count(e => e.ActionType == "order.claimed");
            var inventoryIngredientTotal = AnalyticsIngredientInventoryCalculator.BuildIngredientInventory(playerEvents).Total;
            var actionsUsedTotal = playerEvents.Where(e => e.ActionType == "turn.action.used")
                .Select(e => TryReadActionUsed(e.Payload, out var used, out _) ? used : 0)
                .Sum();
            var compliancePrimaryNeedRate = AnalyticsPrimaryNeedComplianceEvaluator.Evaluate(playerEvents, config).Rate;
            var rulesViolationsCount = await _metrics.CountValidationViolationsAsync(sessionId, playerId, ct);

            var happiness = happinessByPlayer.TryGetValue(playerId, out var breakdown)
                ? breakdown
                : AnalyticsHappinessCalculator.ComputeBreakdown(playerEvents, 0, 0, 0);

            result.Add(new AnalyticsByPlayerItem(
                playerId,
                joinOrder,
                totals.In,
                totals.Out,
                donationTotal,
                goldQty,
                ordersCompletedCount,
                inventoryIngredientTotal,
                actionsUsedTotal,
                compliancePrimaryNeedRate,
                rulesViolationsCount,
                happiness.Total,
                happiness.NeedPoints,
                happiness.NeedSetBonusPoints,
                happiness.DonationPoints,
                happiness.GoldPoints,
                happiness.PensionPoints,
                happiness.SavingGoalPointsEffective,
                happiness.MissionPenaltyPoints,
                happiness.LoanPenaltyPoints,
                happiness.HasUnpaidLoan));
        }

        return AnalyticsPlayerOrdering.OrderPlayers(
            result,
            config?.PlayerOrdering ?? PlayerOrdering.JoinOrder,
            playerJoinOrders,
            firstEventSequenceByPlayer,
            usernamesByPlayer);
    }

    /// <summary>
    /// Menyimpan snapshot metrik sesi dan per-pemain ke database setelah komputasi ulang.
    /// </summary>
    private async Task WriteSnapshotsAsync(
        Guid sessionId,
        Guid rulesetVersionId,
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        RulesetConfig? config,
        Dictionary<Guid, AnalyticsHappinessBreakdown> happinessByPlayer,
        CancellationToken ct)
    {
        var computedAt = DateTimeOffset.UtcNow;
        var snapshots = new List<MetricSnapshotDb>();

        var sessionMetrics = AnalyticsSessionMetricCalculator.ComputeSessionMetrics(events, projections, happinessByPlayer);
        var sessionViolations = await _metrics.CountValidationViolationsAsync(sessionId, null, ct);
        sessionMetrics["rules.violations.count"] = (sessionViolations, null);
        snapshots.AddRange(AnalyticsMetricSnapshotBuilder.BuildMetricSnapshots(sessionId, null, rulesetVersionId, computedAt, sessionMetrics));

        RulesetConfig? playerConfig = null;
        var rulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(rulesetVersionId, ct);
        if (rulesetVersion is not null &&
            RulesetConfigParser.TryParse(rulesetVersion.ConfigJson, out var parsed, out _))
        {
            playerConfig = parsed;
        }

        var players = events.Where(e => e.PlayerId.HasValue).Select(e => e.PlayerId!.Value).Distinct().ToList();
        foreach (var playerId in players)
        {
            var hasHappiness = happinessByPlayer.TryGetValue(playerId, out var breakdown);
            var playerMetrics = await ComputePlayerMetricsAsync(sessionId, playerId, rulesetVersionId, events, projections,
                hasHappiness ? breakdown : null, playerConfig, ct);
            snapshots.AddRange(AnalyticsMetricSnapshotBuilder.BuildMetricSnapshots(sessionId, playerId, rulesetVersionId, computedAt, playerMetrics));
        }

        if (snapshots.Count > 0)
        {
            await _metrics.InsertSnapshotsAsync(snapshots, ct);
        }
    }

    /// <summary>
    /// Menghitung metrik detail per pemain: cashflow, donasi, emas, inventaris, compliance, happiness, dan gameplay.
    /// </summary>
    private async Task<Dictionary<string, (double? Numeric, string? Json)>> ComputePlayerMetricsAsync(
        Guid sessionId,
        Guid playerId,
        Guid rulesetVersionId,
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        AnalyticsHappinessBreakdown? happiness,
        RulesetConfig? config,
        CancellationToken ct)
    {
        var metrics = new Dictionary<string, (double? Numeric, string? Json)>();
        var playerEvents = events.Where(e => e.PlayerId == playerId).ToList();
        var playerProjections = projections.Where(p => p.PlayerId == playerId).ToList();

        var cashIn = playerProjections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        var cashOut = playerProjections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        metrics["cashflow.in.total"] = (cashIn, null);
        metrics["cashflow.out.total"] = (cashOut, null);
        metrics["cashflow.net.total"] = (cashIn - cashOut, null);

        var donationTotal = playerEvents.Where(e => e.ActionType == "day.friday.donation")
            .Select(e => TryReadAmount(e.Payload, out var amount) ? amount : 0)
            .Sum();
        metrics["donation.total"] = (donationTotal, null);

        var goldQty = playerEvents.Where(e => e.ActionType == "day.saturday.gold_trade")
            .Select(e =>
            {
                if (!TryReadGoldTrade(e.Payload, out var tradeType, out var qty))
                {
                    return 0;
                }

                return string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) ? qty : -qty;
            })
            .Sum();
        metrics["gold.qty.current"] = (goldQty, null);

        var ordersCompleted = playerEvents.Count(e => e.ActionType == "order.claimed");
        metrics["orders.completed.count"] = (ordersCompleted, null);

        var inventory = AnalyticsIngredientInventoryCalculator.BuildIngredientInventory(playerEvents);
        metrics["inventory.ingredient.total"] = (inventory.Total, null);

        var actionsUsed = playerEvents.Where(e => e.ActionType == "turn.action.used")
            .Select(e => TryReadActionUsed(e.Payload, out var used, out _) ? used : 0)
            .Sum();
        metrics["actions.used.total"] = (actionsUsed, null);

        var compliance = await ComputePrimaryNeedComplianceAsync(rulesetVersionId, playerEvents, ct);
        metrics["compliance.primary_need.rate"] = (compliance.Rate, compliance.JsonDetail);

        var violations = await _metrics.CountValidationViolationsAsync(sessionId, playerId, ct);
        metrics["rules.violations.count"] = (violations, null);

        var resolvedHappiness = happiness ?? AnalyticsHappinessCalculator.ComputeBreakdown(
            playerEvents,
            AnalyticsHappinessCalculator.SumRankAwarded(playerEvents, "donation.rank.awarded"),
            AnalyticsHappinessCalculator.SumPointsAwarded(playerEvents, "gold.points.awarded"),
            AnalyticsHappinessCalculator.SumRankAwarded(playerEvents, "pension.rank.awarded"));

        metrics["happiness.points.total"] = (resolvedHappiness.Total, null);
        metrics["happiness.need.points"] = (resolvedHappiness.NeedPoints, null);
        metrics["happiness.need.bonus"] = (resolvedHappiness.NeedSetBonusPoints, null);
        metrics["happiness.donation.points"] = (resolvedHappiness.DonationPoints, null);
        metrics["happiness.gold.points"] = (resolvedHappiness.GoldPoints, null);
        metrics["happiness.pension.points"] = (resolvedHappiness.PensionPoints, null);
        metrics["happiness.saving_goal.points"] = (resolvedHappiness.SavingGoalPointsEffective, null);
        metrics["happiness.mission.penalty"] = (resolvedHappiness.MissionPenaltyPoints, null);
        metrics["happiness.loan.penalty"] = (resolvedHappiness.LoanPenaltyPoints, null);
        metrics["loan.unpaid.flag"] = (resolvedHappiness.HasUnpaidLoan ? 1 : 0, null);

        var gameplaySnapshots = AnalyticsGameplaySnapshotBuilder.Build(playerEvents, playerProjections, events, config, resolvedHappiness);
        metrics["gameplay.raw.variables"] = (null, gameplaySnapshots.RawJson);
        metrics["gameplay.derived.metrics"] = (null, gameplaySnapshots.DerivedJson);

        return metrics;
    }

    /// <summary>
    /// Menghitung tingkat kepatuhan kebutuhan primer pemain berdasarkan aturan ruleset per hari.
    /// </summary>
    private async Task<(double Rate, string? JsonDetail)> ComputePrimaryNeedComplianceAsync(
        Guid rulesetVersionId,
        List<EventDb> playerEvents,
        CancellationToken ct)
    {
        var rulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(rulesetVersionId, ct);
        if (rulesetVersion is null || !RulesetConfigParser.TryParse(rulesetVersion.ConfigJson, out var config, out _))
        {
            return (0, null);
        }

        var evaluation = AnalyticsPrimaryNeedComplianceEvaluator.Evaluate(playerEvents, config);
        if (evaluation.EvaluatedDays == 0)
        {
            return (0, null);
        }

        var json = JsonSerializer.Serialize(new
        {
            days = evaluation.Details,
            evaluated_days = evaluation.EvaluatedDays,
            compliant_days = evaluation.CompliantDays
        });

        return (evaluation.Rate, json);
    }

}
