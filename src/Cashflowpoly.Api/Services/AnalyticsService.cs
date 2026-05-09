using System.Security.Claims;
using System.Text.Json;
using Cashflowpoly.Api.Infrastructure;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Api.Services;

internal sealed class AnalyticsService : IAnalyticsService
{
    private readonly SessionRepository _sessions;
    private readonly EventRepository _events;
    private readonly RulesetRepository _rulesets;
    private readonly MetricsRepository _metrics;
    private readonly PlayerRepository _players;
    private readonly UserRepository _users;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHappinessCalculator _happinessCalc;
    private readonly IIngredientInventoryCalculator _inventoryCalc;
    private readonly IPrimaryNeedComplianceEvaluator _complianceEvaluator;
    private readonly IPlayerOrdering _playerOrdering;
    private readonly ISessionMetricCalculator _sessionMetricCalc;
    private readonly IMetricSnapshotBuilder _metricSnapshotBuilder;
    private readonly IScoreCalculator _scoreCalc;
    private readonly IAnalyticsPayloadReader _payloadReader;
    private readonly IGameplaySnapshotBuilder _gameplaySnapshotBuilder;

    public AnalyticsService(
        SessionRepository sessions,
        EventRepository events,
        RulesetRepository rulesets,
        MetricsRepository metrics,
        PlayerRepository players,
        UserRepository users,
        IHttpContextAccessor httpContextAccessor,
        IHappinessCalculator happinessCalc,
        IIngredientInventoryCalculator inventoryCalc,
        IPrimaryNeedComplianceEvaluator complianceEvaluator,
        IPlayerOrdering playerOrdering,
        ISessionMetricCalculator sessionMetricCalc,
        IMetricSnapshotBuilder metricSnapshotBuilder,
        IScoreCalculator scoreCalc,
        IAnalyticsPayloadReader payloadReader,
        IGameplaySnapshotBuilder gameplaySnapshotBuilder)
    {
        _sessions = sessions;
        _events = events;
        _rulesets = rulesets;
        _metrics = metrics;
        _players = players;
        _users = users;
        _httpContextAccessor = httpContextAccessor;
        _happinessCalc = happinessCalc;
        _inventoryCalc = inventoryCalc;
        _complianceEvaluator = complianceEvaluator;
        _playerOrdering = playerOrdering;
        _sessionMetricCalc = sessionMetricCalc;
        _metricSnapshotBuilder = metricSnapshotBuilder;
        _scoreCalc = scoreCalc;
        _payloadReader = payloadReader;
        _gameplaySnapshotBuilder = gameplaySnapshotBuilder;
    }

    public async Task<(AnalyticsSessionResponse? Result, int StatusCode, ErrorResponse? Error)> RecomputeAsync(
        Guid sessionId, ClaimsPrincipal user, CancellationToken ct)
    {
        var role = user.FindFirstValue(ClaimTypes.Role);
        var isInstructor = string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase);
        var isPlayer = string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase);
        if (!isInstructor && !isPlayer)
        {
            return (null, 403, BuildError("FORBIDDEN", "Role tidak dikenali"));
        }

        var (fetchedSession, errorStatus, errorResponse) = await EnsureInstructorSessionAccessAsync(sessionId, user, isInstructor, ct);
        if (errorResponse is not null)
        {
            return (null, errorStatus, errorResponse);
        }

        var session = fetchedSession ?? await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return (null, 404, BuildError("NOT_FOUND", "Session tidak ditemukan"));
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

        var happinessByPlayer = _happinessCalc.ComputeByPlayer(events, projections, config);
        var summary = _scoreCalc.BuildSummary(events, projections, violations);
        var playerJoinOrders = await _players.GetSessionPlayerJoinOrderMapAsync(sessionId, ct);
        var byPlayer = await BuildByPlayerAsync(sessionId, events, projections, happinessByPlayer, config, playerJoinOrders, ct);

        if (activeRulesetVersionId.HasValue)
        {
            await WriteSnapshotsAsync(sessionId, activeRulesetVersionId.Value, events, projections, config, happinessByPlayer, ct);
        }

        return (new AnalyticsSessionResponse(sessionId, summary, byPlayer, activeRulesetId, activeRulesetName), 200, null);
    }

    public async Task<(AnalyticsSessionResponse? Result, int StatusCode, ErrorResponse? Error)> GetSessionAnalyticsAsync(
        Guid sessionId, ClaimsPrincipal user, CancellationToken ct)
    {
        var role = user.FindFirstValue(ClaimTypes.Role);
        var isInstructor = string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase);
        var isPlayer = string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase);
        if (!isInstructor && !isPlayer)
        {
            return (null, 403, BuildError("FORBIDDEN", "Role tidak dikenali"));
        }

        var (fetchedSession, errorStatus, errorResponse) = await EnsureInstructorSessionAccessAsync(sessionId, user, isInstructor, ct);
        if (errorResponse is not null)
        {
            return (null, errorStatus, errorResponse);
        }

        var session = fetchedSession ?? await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return (null, 404, BuildError("NOT_FOUND", "Session tidak ditemukan"));
        }

        var scope = await ResolvePlayerScopeAsync(sessionId, user, ct);
        if (scope.Error is not null)
        {
            return (null, scope.Error.Value.StatusCode, scope.Error.Value.ErrorResponse);
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

        var happinessByPlayer = _happinessCalc.ComputeByPlayer(events, projections, config);
        var summary = _scoreCalc.BuildSummary(events, projections, violations);
        var playerJoinOrders = await _players.GetSessionPlayerJoinOrderMapAsync(sessionId, ct);
        var byPlayer = await BuildByPlayerAsync(sessionId, events, projections, happinessByPlayer, config, playerJoinOrders, ct);
        if (scope.PlayerId.HasValue)
        {
            byPlayer = byPlayer.Where(item => item.PlayerId == scope.PlayerId.Value).ToList();
        }

        return (new AnalyticsSessionResponse(sessionId, summary, byPlayer, activeRulesetId, activeRulesetName), 200, null);
    }

    public async Task<(TransactionHistoryResponse? Result, int StatusCode, ErrorResponse? Error)> GetTransactionsAsync(
        Guid sessionId, Guid? playerId, ClaimsPrincipal user, CancellationToken ct)
    {
        var role = user.FindFirstValue(ClaimTypes.Role);
        var isInstructor = string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase);
        var isPlayer = string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase);
        if (!isInstructor && !isPlayer)
        {
            return (null, 403, BuildError("FORBIDDEN", "Role tidak dikenali"));
        }

        var (fetchedSession, errorStatus, errorResponse) = await EnsureInstructorSessionAccessAsync(sessionId, user, isInstructor, ct);
        if (errorResponse is not null)
        {
            return (null, errorStatus, errorResponse);
        }

        var session = fetchedSession ?? await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return (null, 404, BuildError("NOT_FOUND", "Session tidak ditemukan"));
        }

        var scope = await ResolvePlayerScopeAsync(sessionId, user, ct);
        if (scope.Error is not null)
        {
            return (null, scope.Error.Value.StatusCode, scope.Error.Value.ErrorResponse);
        }

        var effectivePlayerId = scope.PlayerId ?? playerId;

        var projections = await _events.GetCashflowProjectionsAsync(sessionId, ct);
        var items = projections
            .Where(p => !effectivePlayerId.HasValue || p.PlayerId == effectivePlayerId.Value)
            .OrderBy(p => p.Timestamp)
            .Select(p => new TransactionHistoryItem(p.Timestamp, p.Direction, p.Amount, p.Category))
            .ToList();

        return (new TransactionHistoryResponse(items), 200, null);
    }

    public async Task<(GameplayMetricsResponse? Result, int StatusCode, ErrorResponse? Error)> GetGameplayMetricsAsync(
        Guid sessionId, Guid playerId, ClaimsPrincipal user, CancellationToken ct)
    {
        var role = user.FindFirstValue(ClaimTypes.Role);
        var isInstructor = string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase);
        var isPlayer = string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase);
        if (!isInstructor && !isPlayer)
        {
            return (null, 403, BuildError("FORBIDDEN", "Role tidak dikenali"));
        }

        var (fetchedSession, errorStatus, errorResponse) = await EnsureInstructorSessionAccessAsync(sessionId, user, isInstructor, ct);
        if (errorResponse is not null)
        {
            return (null, errorStatus, errorResponse);
        }

        var session = fetchedSession ?? await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return (null, 404, BuildError("NOT_FOUND", "Session tidak ditemukan"));
        }

        var scope = await ResolvePlayerScopeAsync(sessionId, user, ct);
        if (scope.Error is not null)
        {
            return (null, scope.Error.Value.StatusCode, scope.Error.Value.ErrorResponse);
        }

        if (scope.PlayerId.HasValue && scope.PlayerId.Value != playerId)
        {
            return (null, 403, BuildError("FORBIDDEN", "Player hanya dapat melihat metrik miliknya"));
        }

        var snapshots = await _metrics.GetLatestGameplaySnapshotsAsync(sessionId, playerId, ct);
        var rawJson = snapshots.FirstOrDefault(item => item.MetricName == "gameplay.raw.variables")?.MetricValueJson;
        var derivedJson = snapshots.FirstOrDefault(item => item.MetricName == "gameplay.derived.metrics")?.MetricValueJson;
        var computedAt = snapshots.Count == 0 ? (DateTimeOffset?)null : snapshots.Max(item => item.ComputedAt);

        return (new GameplayMetricsResponse(
            sessionId,
            playerId,
            computedAt,
            ParseJsonElement(rawJson),
            ParseJsonElement(derivedJson)), 200, null);
    }

    public async Task<(RulesetAnalyticsSummaryResponse? Result, int StatusCode, ErrorResponse? Error)> GetRulesetAnalyticsSummaryAsync(
        Guid rulesetId, ClaimsPrincipal user, CancellationToken ct)
    {
        var role = user.FindFirstValue(ClaimTypes.Role);
        var isInstructor = string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase);
        var isPlayer = string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase);
        RulesetDb? ruleset = null;
        List<SessionDb> sessions;
        Guid? scopedPlayerId = null;

        if (isInstructor)
        {
            if (!TryGetCurrentUserId(user, out var instructorUserId))
            {
                return (null, 401, BuildError("UNAUTHORIZED", "Token user tidak valid"));
            }

            ruleset = await _rulesets.GetRulesetForInstructorAsync(rulesetId, instructorUserId, ct);
            if (ruleset is null)
            {
                return (null, 404, BuildError("NOT_FOUND", "Ruleset tidak ditemukan"));
            }

            sessions = await _sessions.ListSessionsByInstructorAsync(instructorUserId, ct);
        }
        else if (isPlayer)
        {
            var scope = await ResolvePlayerScopeAsync(null, user, ct);
            if (scope.Error is not null)
            {
                return (null, scope.Error.Value.StatusCode, scope.Error.Value.ErrorResponse);
            }

            scopedPlayerId = scope.PlayerId;
            sessions = await _sessions.ListSessionsAsync(ct);
        }
        else
        {
            return (null, 403, BuildError("FORBIDDEN", "Role tidak diizinkan"));
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

            var happinessByPlayer = _happinessCalc.ComputeByPlayer(events, projections, config);
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

                var learningScore = _scoreCalc.ComputeLearningPerformanceScore(
                    player.CashInTotal,
                    player.CashOutTotal,
                    player.HappinessPointsTotal,
                    complianceRate);

                var missionScore = _scoreCalc.ComputeMissionPerformanceScore(
                    player.MissionPenaltyTotal,
                    player.LoanPenaltyTotal);

                allPlayerItems.Add(new RulesetAnalyticsPlayerItem(player.PlayerId, learningScore, missionScore));
            }

            var learningAggregate = _scoreCalc.AverageNullable(allPlayerItems.Select(item => item.LearningPerformanceIndividualScore));
            var missionAggregate = _scoreCalc.AverageNullable(allPlayerItems.Select(item => item.MissionPerformanceIndividualScore));
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
            return (null, 404, BuildError("NOT_FOUND", "Ruleset tidak ditemukan"));
        }

        if (!isInstructor)
        {
            ruleset = await _rulesets.GetRulesetAsync(rulesetId, ct);
            if (ruleset is null)
            {
                return (null, 404, BuildError("NOT_FOUND", "Ruleset tidak ditemukan"));
            }
        }

        var learningOverall = _scoreCalc.AverageNullable(sessionItems.Select(item => item.LearningPerformanceAggregateScore));
        var missionOverall = _scoreCalc.AverageNullable(sessionItems.Select(item => item.MissionPerformanceAggregateScore));

        return (new RulesetAnalyticsSummaryResponse(
            rulesetId,
            ruleset!.Name,
            sessionItems.Count,
            learningOverall,
            missionOverall,
            sessionItems), 200, null);
    }

    private async Task<(Guid? PlayerId, (int StatusCode, ErrorResponse ErrorResponse)? Error)> ResolvePlayerScopeAsync(
        Guid? sessionId, ClaimsPrincipal user, CancellationToken ct)
    {
        var role = user.FindFirstValue(ClaimTypes.Role);
        if (string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase))
        {
            return (null, null);
        }

        if (!string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        {
            return (null, (403, BuildError("FORBIDDEN", "Role tidak dikenali")));
        }

        var userIdRaw = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            return (null, (401, BuildError("UNAUTHORIZED", "Token user tidak valid")));
        }

        var playerId = await _users.GetLinkedPlayerIdAsync(userId, ct);
        if (!playerId.HasValue)
        {
            return (null, (403, BuildError("FORBIDDEN", "Akun PLAYER belum terhubung ke profil pemain")));
        }

        if (sessionId.HasValue)
        {
            var inSession = await _players.IsPlayerInSessionAsync(sessionId.Value, playerId.Value, ct);
            if (!inSession)
            {
                return (null, (403, BuildError("FORBIDDEN", "Player tidak terdaftar di sesi ini")));
            }
        }

        return (playerId.Value, null);
    }

    private async Task<(SessionDb? Session, int StatusCode, ErrorResponse? Error)> EnsureInstructorSessionAccessAsync(
        Guid sessionId, ClaimsPrincipal user, bool isInstructor, CancellationToken ct)
    {
        if (!isInstructor)
        {
            return (null, 0, null);
        }

        if (!TryGetCurrentUserId(user, out var instructorUserId))
        {
            return (null, 401, BuildError("UNAUTHORIZED", "Token user tidak valid"));
        }

        var session = await _sessions.GetSessionForInstructorAsync(sessionId, instructorUserId, ct);
        if (session is null)
        {
            return (null, 404, BuildError("NOT_FOUND", "Session tidak ditemukan"));
        }

        return (session, 0, null);
    }

    private static bool TryGetCurrentUserId(ClaimsPrincipal user, out Guid userId)
    {
        var userIdRaw = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdRaw, out userId);
    }

    private ErrorResponse BuildError(string code, string message)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            return ApiErrorHelper.BuildError(httpContext, code, message);
        }

        return new ErrorResponse(code, message, new List<ErrorDetail>(), "unknown");
    }

    private static JsonElement? ParseJsonElement(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }

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
                .Select(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0)
                .Sum();

            var goldQty = playerEvents.Where(e => e.ActionType == "day.saturday.gold_trade")
                .Select(e =>
                {
                    if (!_payloadReader.TryReadGoldTrade(e.Payload, out var tradeType, out var qty))
                    {
                        return 0;
                    }

                    return string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) ? qty : -qty;
                })
                .Sum();

            var ordersCompletedCount = playerEvents.Count(e => e.ActionType == "order.claimed");
            var inventoryIngredientTotal = _inventoryCalc.BuildIngredientInventory(playerEvents).Total;
            var actionsUsedTotal = playerEvents.Where(e => e.ActionType == "turn.action.used")
                .Select(e => _payloadReader.TryReadActionUsed(e.Payload, out var used, out _) ? used : 0)
                .Sum();
            var compliancePrimaryNeedRate = _complianceEvaluator.Evaluate(playerEvents, config).Rate;
            var rulesViolationsCount = await _metrics.CountValidationViolationsAsync(sessionId, playerId, ct);

            var happiness = happinessByPlayer.TryGetValue(playerId, out var breakdown)
                ? breakdown
                : _happinessCalc.ComputeBreakdown(playerEvents, 0, 0, 0);

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

        return _playerOrdering.OrderPlayers(
            result,
            config?.PlayerOrdering ?? PlayerOrdering.JoinOrder,
            playerJoinOrders,
            firstEventSequenceByPlayer,
            usernamesByPlayer);
    }

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

        var sessionMetrics = _sessionMetricCalc.ComputeSessionMetrics(events, projections, happinessByPlayer);
        var sessionViolations = await _metrics.CountValidationViolationsAsync(sessionId, null, ct);
        sessionMetrics["rules.violations.count"] = (sessionViolations, null);
        snapshots.AddRange(_metricSnapshotBuilder.BuildMetricSnapshots(sessionId, null, rulesetVersionId, computedAt, sessionMetrics));

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
            snapshots.AddRange(_metricSnapshotBuilder.BuildMetricSnapshots(sessionId, playerId, rulesetVersionId, computedAt, playerMetrics));
        }

        if (snapshots.Count > 0)
        {
            await _metrics.InsertSnapshotsAsync(snapshots, ct);
        }
    }

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
            .Select(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0)
            .Sum();
        metrics["donation.total"] = (donationTotal, null);

        var goldQty = playerEvents.Where(e => e.ActionType == "day.saturday.gold_trade")
            .Select(e =>
            {
                if (!_payloadReader.TryReadGoldTrade(e.Payload, out var tradeType, out var qty))
                {
                    return 0;
                }

                return string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) ? qty : -qty;
            })
            .Sum();
        metrics["gold.qty.current"] = (goldQty, null);

        var ordersCompleted = playerEvents.Count(e => e.ActionType == "order.claimed");
        metrics["orders.completed.count"] = (ordersCompleted, null);

        var inventory = _inventoryCalc.BuildIngredientInventory(playerEvents);
        metrics["inventory.ingredient.total"] = (inventory.Total, null);

        var actionsUsed = playerEvents.Where(e => e.ActionType == "turn.action.used")
            .Select(e => _payloadReader.TryReadActionUsed(e.Payload, out var used, out _) ? used : 0)
            .Sum();
        metrics["actions.used.total"] = (actionsUsed, null);

        var compliance = await ComputePrimaryNeedComplianceAsync(rulesetVersionId, playerEvents, ct);
        metrics["compliance.primary_need.rate"] = (compliance.Rate, compliance.JsonDetail);

        var violations = await _metrics.CountValidationViolationsAsync(sessionId, playerId, ct);
        metrics["rules.violations.count"] = (violations, null);

        var resolvedHappiness = happiness ?? _happinessCalc.ComputeBreakdown(
            playerEvents,
            _happinessCalc.SumRankAwarded(playerEvents, "donation.rank.awarded"),
            _happinessCalc.SumPointsAwarded(playerEvents, "gold.points.awarded"),
            _happinessCalc.SumRankAwarded(playerEvents, "pension.rank.awarded"));

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

        var gameplaySnapshots = _gameplaySnapshotBuilder.Build(playerEvents, playerProjections, events, config, resolvedHappiness);
        metrics["gameplay.raw.variables"] = (null, gameplaySnapshots.RawJson);
        metrics["gameplay.derived.metrics"] = (null, gameplaySnapshots.DerivedJson);

        return metrics;
    }

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

        var evaluation = _complianceEvaluator.Evaluate(playerEvents, config);
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
