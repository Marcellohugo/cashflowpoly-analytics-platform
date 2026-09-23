// Fungsi file: Mengorkestrasi alur aplikasi dan domain melalui AnalyticsService.
using System.Security.Claims;
using System.Text.Json;
using Cashflowpoly.Api.Infrastructure;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Contracts;
using Microsoft.AspNetCore.Http;
using Dapper;

namespace Cashflowpoly.Api.Services;

internal sealed class AnalyticsService : IAnalyticsService
{
    private sealed record ActiveRulesetContext(Guid? VersionId, Guid? RulesetId, string? Name, RulesetConfig? Config);
    private static readonly EventPayloadReader _eventPayloadReader = new();

    private readonly SessionRepository _sessions;
    private readonly EventRepository _events;
    private readonly RulesetRepository _rulesets;
    private readonly MetricsRepository _metrics;
    private readonly PlayerRepository _players;
    private readonly UserRepository _users;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHappinessCalculator _happinessCalc;
    private readonly IIngredientInventoryCalculator _inventoryCalc;
    private readonly INeedMissionCalculator _needMissionCalculator;
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
        INeedMissionCalculator needMissionCalculator,
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
        _needMissionCalculator = needMissionCalculator;
        _playerOrdering = playerOrdering;
        _sessionMetricCalc = sessionMetricCalc;
        _metricSnapshotBuilder = metricSnapshotBuilder;
        _scoreCalc = scoreCalc;
        _payloadReader = payloadReader;
        _gameplaySnapshotBuilder = gameplaySnapshotBuilder;
    }

    public async Task<(SessionRostersResponse? Result, int StatusCode, ErrorResponse? Error)> GetSessionRostersAsync(
        bool includeResults, ClaimsPrincipal user, CancellationToken ct)
    {
        var (sessions, code, error) = await ListAccessibleSessionsAsync(user, ct);
        if (error is not null) return (null, code, error);
        var rows = (await _players.ListSessionRostersAsync(sessions.Select(s => s.SessionId).ToArray(), ct))
            .ToLookup(p => p.SessionId);
        var items = new List<SessionRosterResponse>();
        foreach (var session in sessions)
        {
            var players = rows[session.SessionId].Select(p => new SessionRosterPlayer(
                p.UserId, p.DisplayName, p.PlayerOrder,
                includeResults && session.Status == "ENDED" ? p.FinalRank : null,
                includeResults && session.Status == "ENDED" ? p.HappinessPointsTotal : null)).ToList();
            var available = includeResults && session.Status == "ENDED" && players.Count > 0
                && players.All(p => p.HappinessPointsTotal.HasValue && p.FinalRank.HasValue);
            // Older sessions may not have final-score rows; preserve the detail page's live fallback.
            if (includeResults && session.Status == "ENDED" && !available)
            {
                AnalyticsSessionResponse? analytics = null;
                try { (analytics, _, _) = await GetSessionAnalyticsAsync(session.SessionId, user, ct); }
                catch (Npgsql.NpgsqlException) { }
                catch (TimeoutException) { }
                available = analytics is not null;
                players = players.Select(p => p with
                {
                    FinalRank = analytics?.Leaderboard?.FirstOrDefault(r => r.UserId == p.UserId)?.Rank,
                    HappinessPointsTotal = analytics?.Leaderboard?.FirstOrDefault(r => r.UserId == p.UserId)?.HappinessPointsTotal
                        ?? analytics?.ByPlayer?.FirstOrDefault(r => r.UserId == p.UserId)?.HappinessPointsTotal
                }).ToList();
            }
            items.Add(new SessionRosterResponse(session.SessionId, players, available));
        }
        return (new SessionRostersResponse(items), 200, null);
    }

    public async Task<(PlayerGameplayHistoryResponse? Result, int StatusCode, ErrorResponse? Error)> GetPlayerGameplayHistoryAsync(
        Guid playerId, string mode, string? status, ClaimsPrincipal user, CancellationToken ct)
    {
        mode = mode?.Trim().ToUpperInvariant() ?? string.Empty;
        status = status?.Trim().ToUpperInvariant() ?? "ALL";
        if (mode is not ("PEMULA" or "MAHIR") || status is not ("ALL" or "CREATED" or "STARTED" or "ENDED"))
            return (null, 400, BuildError("VALIDATION_ERROR", "Pilih satu mode dan status sesi yang valid"));
        if (user.IsInRole("PLAYER") && (!TryGetCurrentUserId(user, out var currentUserId) || currentUserId != playerId))
            return (null, 403, BuildError("FORBIDDEN", "Player hanya dapat melihat metrik miliknya"));
        var (sessions, code, error) = await ListAccessibleSessionsAsync(user, ct);
        if (error is not null) return (null, code, error);
        var players = await _players.ListSessionRostersAsync(sessions.Select(s => s.SessionId).ToArray(), ct);
        var memberships = players.Where(p => p.UserId == playerId).ToDictionary(p => p.SessionId);
        if (user.IsInRole("INSTRUCTOR") && memberships.Count == 0)
            return (null, 404, BuildError("NOT_FOUND", "Pemain tidak ditemukan dalam sesi Anda"));
        var selected = sessions.Where(s => memberships.ContainsKey(s.SessionId) && s.Mode == mode
                && (status == "ALL" || s.Status == status))
            .OrderBy(s => s.StartedAt ?? s.CreatedAt).ThenBy(s => s.SessionId).ToList();
        var contexts = new Dictionary<Guid, ActiveRulesetContext>();
        foreach (var group in selected.Where(s => s.Status != "CREATED").GroupBy(s => s.RulesetVersionId))
            contexts[group.Key] = await GetActiveRulesetContextAsync(group.First().SessionId, ct);
        using var gate = new SemaphoreSlim(4);
        var items = await Task.WhenAll(selected.Select(async session =>
        {
            if (session.Status == "CREATED") return new SessionGameplayItem(session.SessionId, null);
            await gate.WaitAsync(ct);
            try
            {
                var gameplay = await BuildGameplayMetricsAsync(session, playerId, contexts[session.RulesetVersionId],
                    memberships[session.SessionId].DisplayName, ct);
                return new SessionGameplayItem(session.SessionId, gameplay);
            }
            catch (Npgsql.NpgsqlException) { return new SessionGameplayItem(session.SessionId, null); }
            catch (TimeoutException) { return new SessionGameplayItem(session.SessionId, null); }
            finally { gate.Release(); }
        }));
        return (new PlayerGameplayHistoryResponse(items.ToList()), 200, null);
    }

    private async Task<(List<SessionDb> Sessions, int StatusCode, ErrorResponse? Error)> ListAccessibleSessionsAsync(
        ClaimsPrincipal user, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(user, out var userId))
            return ([], 401, BuildError("UNAUTHORIZED", "Token pengguna tidak valid"));
        if (user.IsInRole("INSTRUCTOR"))
            return (await _sessions.ListSessionsByInstructorAsync(userId, ct), 200, null);
        var scope = await ResolvePlayerScopeAsync(null, user, ct);
        if (scope.Error.HasValue) return ([], scope.Error.Value.StatusCode, scope.Error.Value.ErrorResponse);
        return (await _sessions.ListSessionsByPlayerAsync(scope.UserId!.Value, ct), 200, null);
    }

    public async Task<(AnalyticsSessionResponse? Result, int StatusCode, ErrorResponse? Error)> RecomputeAsync(
        Guid sessionId, ClaimsPrincipal user, CancellationToken ct)
    {
        // Keep event reads and persisted metrics on the same side of any ingest, undo, or session end.
        await using var conn = await _events.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition(
            "select pg_advisory_xact_lock(hashtextextended(@sessionId::text, 0))", new { sessionId }, tx, cancellationToken: ct));
        var access = await ResolveSessionAccessAsync(sessionId, user, ct);
        if (access.Error is not null)
        {
            return (null, access.StatusCode, access.Error);
        }

        var events = await _events.GetAllEventsBySessionAsync(sessionId, ct);
        var projections = await _events.GetCashflowProjectionsAsync(sessionId, ct);
        var participantCount = await _players.CountPlayersInSessionAsync(sessionId, ct);
        var hasSealedDonations = DonationVisibility.RemoveSealed(events, projections, participantCount);
        var activeRuleset = await GetActiveRulesetContextAsync(sessionId, ct);
        var playerRoster = await _players.GetSessionPlayerPlayerOrderMapAsync(sessionId, ct);
        var happinessByPlayer = _happinessCalc.ComputeByPlayer(events, projections, activeRuleset.Config, playerRoster.Keys);
        var finalScores = await ResolveFinalScoresAsync(sessionId, access.Session?.Status, ct);
        happinessByPlayer = ApplyFinalScores(happinessByPlayer, finalScores);
        var summary = _scoreCalc.BuildSummary(events, projections);

        var byPlayer = await BuildByPlayerAsync(sessionId, events, projections, happinessByPlayer, activeRuleset.Config, playerRoster, ct);
        var leaderboard = string.Equals(access.Session?.Status, "ENDED", StringComparison.OrdinalIgnoreCase)
            ? BuildFinalLeaderboard(byPlayer, finalScores)
            : [];

        if (activeRuleset.VersionId.HasValue)
        {
            await WriteSnapshotsAsync(
                sessionId,
                activeRuleset.VersionId.Value,
                events,
                projections,
                activeRuleset.Config,
                happinessByPlayer,
                finalScores,
                string.Equals(access.Session?.Status, "ENDED", StringComparison.OrdinalIgnoreCase),
                ct);
        }

        return (new AnalyticsSessionResponse(sessionId, summary, byPlayer, activeRuleset.RulesetId, activeRuleset.Name, leaderboard, activeRuleset.VersionId, hasSealedDonations), 200, null);
    }

    public async Task<(AnalyticsSessionResponse? Result, int StatusCode, ErrorResponse? Error)> GetSessionAnalyticsAsync(
        Guid sessionId, ClaimsPrincipal user, CancellationToken ct)
    {
        var access = await ResolveSessionAccessAsync(sessionId, user, ct);
        if (access.Error is not null)
        {
            return (null, access.StatusCode, access.Error);
        }

        var scope = await ResolvePlayerScopeAsync(sessionId, user, ct);
        if (scope.Error is not null)
        {
            return (null, scope.Error.Value.StatusCode, scope.Error.Value.ErrorResponse);
        }

        var events = await _events.GetAllEventsBySessionAsync(sessionId, ct);
        var projections = await _events.GetCashflowProjectionsAsync(sessionId, ct);
        var participantCount = await _players.CountPlayersInSessionAsync(sessionId, ct);
        var hasSealedDonations = DonationVisibility.RemoveSealed(events, projections, participantCount);
        var activeRuleset = await GetActiveRulesetContextAsync(sessionId, ct);
        var playerRoster = await _players.GetSessionPlayerPlayerOrderMapAsync(sessionId, ct);
        var happinessByPlayer = _happinessCalc.ComputeByPlayer(events, projections, activeRuleset.Config, playerRoster.Keys);
        var finalScores = await ResolveFinalScoresAsync(sessionId, access.Session?.Status, ct);
        happinessByPlayer = ApplyFinalScores(happinessByPlayer, finalScores);
        var summary = _scoreCalc.BuildSummary(events, projections);

        var byPlayer = await BuildByPlayerAsync(sessionId, events, projections, happinessByPlayer, activeRuleset.Config, playerRoster, ct);
        var leaderboard = string.Equals(access.Session?.Status, "ENDED", StringComparison.OrdinalIgnoreCase)
            ? BuildFinalLeaderboard(byPlayer, finalScores)
            : [];
        if (scope.UserId.HasValue)
        {
            byPlayer = byPlayer.Where(item => item.UserId == scope.UserId.Value).ToList();
        }

        return (new AnalyticsSessionResponse(sessionId, summary, byPlayer, activeRuleset.RulesetId, activeRuleset.Name, leaderboard, activeRuleset.VersionId, hasSealedDonations), 200, null);
    }

    public async Task<(TransactionHistoryResponse? Result, int StatusCode, ErrorResponse? Error)> GetTransactionsAsync(
        Guid sessionId, Guid? userId, string? cursor, int limit, ClaimsPrincipal user, CancellationToken ct)
    {
        var access = await ResolveSessionAccessAsync(sessionId, user, ct);
        if (access.Error is not null)
        {
            return (null, access.StatusCode, access.Error);
        }

        var scope = await ResolvePlayerScopeAsync(sessionId, user, ct);
        if (scope.Error is not null)
        {
            return (null, scope.Error.Value.StatusCode, scope.Error.Value.ErrorResponse);
        }

        var effectiveUserId = scope.UserId ?? userId;

        if (!OpaqueCursor.TryDecodeTransaction(cursor, out var afterTimestamp, out var afterTransactionId))
        {
            return (null, StatusCodes.Status400BadRequest, BuildError("VALIDATION_ERROR", "Cursor transaksi tidak valid",
                new ErrorDetail("cursor", "INVALID_FORMAT")));
        }

        if (limit is < 1 or > 100)
        {
            return (null, StatusCodes.Status400BadRequest, BuildError("VALIDATION_ERROR", "limit harus antara 1 sampai 100",
                new ErrorDetail("limit", "OUT_OF_RANGE")));
        }

        var projections = await _events.GetCashflowProjectionPageAsync(
            sessionId,
            effectiveUserId,
            string.IsNullOrWhiteSpace(cursor) ? null : afterTimestamp,
            string.IsNullOrWhiteSpace(cursor) ? null : afterTransactionId,
            limit + 1,
            ct);
        var hasMore = projections.Count > limit;
        if (hasMore)
        {
            projections.RemoveAt(projections.Count - 1);
        }

        var items = projections
            .Select(p => new TransactionHistoryItem(p.ProjectionId, p.Timestamp, p.Direction, p.Amount, p.Category))
            .ToList();

        var nextCursor = projections.Count > 0
            ? OpaqueCursor.EncodeTransaction(projections[^1].Timestamp, projections[^1].ProjectionId)
            : null;
        return (new TransactionHistoryResponse(items, nextCursor, hasMore), 200, null);
    }

    public async Task<(GameplayMetricsResponse? Result, int StatusCode, ErrorResponse? Error)> GetGameplayMetricsAsync(
        Guid sessionId, Guid userId, ClaimsPrincipal user, CancellationToken ct)
    {
        var access = await ResolveSessionAccessAsync(sessionId, user, ct);
        if (access.Error is not null)
        {
            return (null, access.StatusCode, access.Error);
        }

        var scope = await ResolvePlayerScopeAsync(sessionId, user, ct);
        if (scope.Error is not null)
        {
            return (null, scope.Error.Value.StatusCode, scope.Error.Value.ErrorResponse);
        }

        if (scope.UserId.HasValue && scope.UserId.Value != userId)
        {
            return (null, 403, BuildError("FORBIDDEN", "Player hanya dapat melihat metrik miliknya"));
        }

        var activeRuleset = await GetActiveRulesetContextAsync(sessionId, ct);
        var playerAlias = (await _players.ListSessionPlayersAsync(sessionId, ct))
            .FirstOrDefault(player => player.UserId == userId)?.DisplayName;
        return (await BuildGameplayMetricsAsync(access.Session!, userId, activeRuleset, playerAlias, ct), 200, null);
    }

    private async Task<GameplayMetricsResponse> BuildGameplayMetricsAsync(
        SessionDb session, Guid userId, ActiveRulesetContext activeRuleset, string? playerAlias, CancellationToken ct)
    {
        var sessionId = session.SessionId;
        var startingCash = activeRuleset.Config?.StartingCash ?? 0;
        var hasSealedDonations = false;
        Dictionary<string, double> values;
        DateTimeOffset? computedAt;
        string? rawJsonText;
        string? derivedJsonText;

        if (activeRuleset.VersionId.HasValue)
        {
            var events = await _events.GetAllEventsBySessionAsync(sessionId, ct);
            var projections = await _events.GetCashflowProjectionsAsync(sessionId, ct);
            var participantCount = await _players.CountPlayersInSessionAsync(sessionId, ct);
            hasSealedDonations = DonationVisibility.RemoveSealed(events, projections, participantCount);
            var playerRoster = await _players.GetSessionPlayerPlayerOrderMapAsync(sessionId, ct);
            var happinessByPlayer = _happinessCalc.ComputeByPlayer(events, projections, activeRuleset.Config, playerRoster.Keys);
            var finalScores = await ResolveFinalScoresAsync(sessionId, session.Status, ct);
            happinessByPlayer = ApplyFinalScores(happinessByPlayer, finalScores);
            var liveMetrics = ComputePlayerMetrics(
                userId,
                events,
                projections,
                happinessByPlayer.GetValueOrDefault(userId),
                activeRuleset.Config,
                finalScores.FirstOrDefault(score => score.UserId == userId),
                playerAlias,
                string.Equals(session.Status, "ENDED", StringComparison.OrdinalIgnoreCase),
                playerRoster.Keys);
            values = liveMetrics.ToDictionary(
                item => item.Key,
                item => item.Value.Numeric ?? 0d,
                StringComparer.Ordinal);
            computedAt = events
                .Where(item => item.UserId == userId)
                .Select(item => (DateTimeOffset?)item.Timestamp)
                .Max();
            rawJsonText = liveMetrics.GetValueOrDefault("gameplay.raw.variables").Json;
            derivedJsonText = liveMetrics.GetValueOrDefault("gameplay.derived.metrics").Json;
        }
        else
        {
            var metricNames = new[]
            {
                "cashflow.in.total", "cashflow.out.total", "cashflow.net.total", "donation.total",
                "gold.qty.current", "orders.completed.count", "inventory.ingredient.total", "actions.used.total",
                "happiness.points.total", "happiness.need.points", "happiness.need.bonus", "happiness.donation.points",
                "happiness.gold.points", "happiness.pension.points", "happiness.saving_goal.points",
                "happiness.mission.penalty", "happiness.loan.penalty", "loan.unpaid.flag",
                "happiness.initial", "happiness.mission.reward",
                "needs.fulfillment_diversity"
            };
            var snapshots = await _metrics.GetLatestMetricValuesAsync(sessionId, userId, metricNames, ct);
            values = snapshots.ToDictionary(
                item => item.MetricName,
                item => item.MetricValueNumeric ?? 0d,
                StringComparer.Ordinal);
            computedAt = snapshots.Count == 0 ? null : snapshots.Max(item => item.ComputedAt);
            var snapshotsJson = await _metrics.GetLatestGameplaySnapshotsAsync(sessionId, userId, ct);
            rawJsonText = snapshotsJson.FirstOrDefault(s => s.MetricName == "gameplay.raw.variables")?.MetricValueJson;
            derivedJsonText = snapshotsJson.FirstOrDefault(s => s.MetricName == "gameplay.derived.metrics")?.MetricValueJson;
        }

        JsonElement? rawJson = null;
        JsonElement? derivedJson = null;

        if (!string.IsNullOrWhiteSpace(rawJsonText))
        {
            try
            {
                rawJson = JsonDocument.Parse(rawJsonText).RootElement.Clone();
            }
            catch (JsonException)
            {
            }
        }
        if (!string.IsNullOrWhiteSpace(derivedJsonText))
        {
            try
            {
                derivedJson = JsonDocument.Parse(derivedJsonText).RootElement.Clone();
            }
            catch (JsonException)
            {
            }
        }

        return new GameplayMetricsResponse(
            sessionId,
            userId,
            computedAt,
            new GameplayEconomyMetrics(
                startingCash,
                ReadMetric(values, "cashflow.in.total"),
                ReadMetric(values, "cashflow.out.total"),
                ReadMetric(values, "cashflow.net.total"),
                ReadMetric(values, "donation.total")),
            new GameplayProgressMetrics(
                (int)ReadMetric(values, "gold.qty.current"),
                (int)ReadMetric(values, "orders.completed.count"),
                (int)ReadMetric(values, "inventory.ingredient.total"),
                (int)ReadMetric(values, "actions.used.total")),
            new GameplayScoreMetrics(
                ReadMetric(values, "happiness.points.total"),
                ReadMetric(values, "happiness.need.points"),
                ReadMetric(values, "happiness.need.bonus"),
                ReadMetric(values, "happiness.donation.points"),
                ReadMetric(values, "happiness.gold.points"),
                ReadMetric(values, "happiness.pension.points"),
                ReadMetric(values, "happiness.saving_goal.points"),
                ReadMetric(values, "happiness.mission.penalty"),
                ReadMetric(values, "happiness.loan.penalty"),
                ReadMetric(values, "loan.unpaid.flag") > 0.5d,
                ReadMetric(values, "happiness.initial"),
                ReadMetric(values, "happiness.mission.reward")),
            new GameplayNeedMetrics(
                ReadMetric(values, "needs.fulfillment_diversity")),
            rawJson,
            derivedJson, hasSealedDonations);
    }

    public async Task<(RulesetAnalyticsSummaryResponse? Result, int StatusCode, ErrorResponse? Error)> GetRulesetAnalyticsSummaryAsync(
        Guid rulesetId, ClaimsPrincipal user, CancellationToken ct)
    {
        var role = user.FindFirstValue(ClaimTypes.Role);
        var isInstructor = string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase);
        var isPlayer = string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase);
        RulesetDb? ruleset = null;
        List<SessionDb> sessions;
        Guid? scopedUserId = null;

        if (isInstructor)
        {
            if (!TryGetCurrentUserId(user, out var instructorUserId))
            {
                return (null, 401, BuildError("UNAUTHORIZED", "Token pengguna tidak valid"));
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

            scopedUserId = scope.UserId;
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

            if (scopedUserId.HasValue)
            {
                var inSession = await _players.IsPlayerInSessionAsync(session.SessionId, scopedUserId.Value, ct);
                if (!inSession)
                {
                    continue;
                }
            }

            var events = await _events.GetAllEventsBySessionAsync(session.SessionId, ct);
            var projections = await _events.GetCashflowProjectionsAsync(session.SessionId, ct);
            var participantCount = await _players.CountPlayersInSessionAsync(session.SessionId, ct);
            DonationVisibility.RemoveSealed(events, projections, participantCount);
            RulesetConfig? config = null;
            TryBuildRuntimeConfig(activeVersion, out config);

            var playerRoster = await _players.GetSessionPlayerPlayerOrderMapAsync(session.SessionId, ct);
            var happinessByPlayer = _happinessCalc.ComputeByPlayer(events, projections, config, playerRoster.Keys);
            var finalScores = await ResolveFinalScoresAsync(session.SessionId, session.Status, ct);
            happinessByPlayer = ApplyFinalScores(happinessByPlayer, finalScores);

            var byPlayer = await BuildByPlayerAsync(session.SessionId, events, projections, happinessByPlayer, config, playerRoster, ct);
            var allPlayerItems = new List<RulesetAnalyticsPlayerItem>();

            foreach (var player in byPlayer)
            {
                var learningScore = _scoreCalc.ComputeLearningPerformanceScore(
                    player.CashInTotal,
                    player.CashOutTotal,
                    player.HappinessPointsTotal,
                    player.FulfillmentDiversity);

                var missionScore = _scoreCalc.ComputeMissionPerformanceScore(
                    player.MissionPenaltyTotal,
                    player.LoanPenaltyTotal);

                allPlayerItems.Add(new RulesetAnalyticsPlayerItem(player.UserId, learningScore, missionScore));
            }

            var learningAggregate = _scoreCalc.AverageNullable(allPlayerItems.Select(item => item.LearningPerformanceIndividualScore));
            var missionAggregate = _scoreCalc.AverageNullable(allPlayerItems.Select(item => item.MissionPerformanceIndividualScore));
            var visiblePlayers = scopedUserId.HasValue
                ? allPlayerItems.Where(item => item.UserId == scopedUserId.Value).ToList()
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

    private async Task<(SessionDb? Session, bool IsInstructor, int StatusCode, ErrorResponse? Error)> ResolveSessionAccessAsync(
        Guid sessionId, ClaimsPrincipal user, CancellationToken ct)
    {
        var role = user.FindFirstValue(ClaimTypes.Role);
        var isInstructor = string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase);
        var isPlayer = string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase);
        if (!isInstructor && !isPlayer)
        {
            return (null, false, 403, BuildError("FORBIDDEN", "Role tidak dikenali"));
        }

        var (session, errorStatus, errorResponse) = await EnsureInstructorSessionAccessAsync(sessionId, user, isInstructor, ct);
        if (errorResponse is not null)
        {
            return (null, isInstructor, errorStatus, errorResponse);
        }

        session ??= await _sessions.GetSessionAsync(sessionId, ct);
        if (session is null)
        {
            return (null, isInstructor, 404, BuildError("NOT_FOUND", "Sesi tidak ditemukan"));
        }

        return (session, isInstructor, 200, null);
    }

    private async Task<ActiveRulesetContext> GetActiveRulesetContextAsync(Guid sessionId, CancellationToken ct)
    {
        var versionId = await _sessions.GetActiveRulesetVersionIdAsync(sessionId, ct);
        if (!versionId.HasValue)
        {
            return new ActiveRulesetContext(null, null, null, null);
        }

        var rulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(versionId.Value, ct);
        if (rulesetVersion is null)
        {
            return new ActiveRulesetContext(versionId.Value, null, null, null);
        }

        var ruleset = await _rulesets.GetRulesetAsync(rulesetVersion.RulesetId, ct);
        TryBuildRuntimeConfig(rulesetVersion, out var config);

        return new ActiveRulesetContext(versionId.Value, rulesetVersion.RulesetId, ruleset?.Name, config);
    }

    private async Task<(Guid? UserId, (int StatusCode, ErrorResponse ErrorResponse)? Error)> ResolvePlayerScopeAsync(
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
            return (null, (401, BuildError("UNAUTHORIZED", "Token pengguna tidak valid")));
        }

        var playerUserId = await _users.GetPlayerUserIdAsync(userId, ct);
        if (!playerUserId.HasValue)
        {
            return (null, (403, BuildError("FORBIDDEN", "Akun PLAYER belum terhubung ke profil pemain")));
        }

        if (sessionId.HasValue)
        {
            var inSession = await _players.IsPlayerInSessionAsync(sessionId.Value, playerUserId.Value, ct);
            if (!inSession)
            {
                return (null, (403, BuildError("FORBIDDEN", "Player tidak terdaftar di sesi ini")));
            }
        }

        return (playerUserId.Value, null);
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
            return (null, 401, BuildError("UNAUTHORIZED", "Token pengguna tidak valid"));
        }

        var session = await _sessions.GetSessionForInstructorAsync(sessionId, instructorUserId, ct);
        if (session is null)
        {
            return (null, 404, BuildError("NOT_FOUND", "Sesi tidak ditemukan"));
        }

        return (session, 0, null);
    }

    private static bool TryGetCurrentUserId(ClaimsPrincipal user, out Guid userId)
    {
        var userIdRaw = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdRaw, out userId);
    }

    private ErrorResponse BuildError(string code, string message, params ErrorDetail[] details)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            return ApiErrorHelper.BuildError(httpContext, code, message, details);
        }

        return new ErrorResponse(code, message, details.ToList(), "unknown");
    }

    private static double ReadMetric(IReadOnlyDictionary<string, double> values, string name)
        => values.TryGetValue(name, out var value) ? value : 0d;

    private async Task<List<AnalyticsByPlayerItem>> BuildByPlayerAsync(
        Guid sessionId,
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        Dictionary<Guid, AnalyticsHappinessBreakdown> happinessByPlayer,
        RulesetConfig? config,
        Dictionary<Guid, int> playerPlayerOrders,
        CancellationToken ct)
    {
        var cashTotals = projections
            .GroupBy(p => p.UserId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    In = g.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount),
                    Out = g.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount)
                });

        var result = new List<AnalyticsByPlayerItem>();
        var eventsByPlayer = events.Where(e => e.UserId.HasValue)
            .GroupBy(e => e.UserId!.Value)
            .ToDictionary(group => group.Key, group => group.ToList());
        var playerIds = playerPlayerOrders.Keys
            .Union(eventsByPlayer.Keys)
            .Distinct()
            .ToList();
        var firstEventSequenceByPlayer = playerIds.ToDictionary(
            playerId => playerId,
            playerId => eventsByPlayer.TryGetValue(playerId, out var items) && items.Count > 0
                ? items.Min(item => item.SequenceNumber)
                : long.MaxValue);
        var usernamesByPlayer = await _users.GetUsernamesByUserIdsAsync(playerIds, ct);

        foreach (var playerId in playerIds)
        {
            var playerEvents = eventsByPlayer.TryGetValue(playerId, out var items)
                ? items
                : [];
            var playerOrder = playerPlayerOrders.TryGetValue(playerId, out var assignedPlayerOrder) ? assignedPlayerOrder : 0;

            var totals = cashTotals.TryGetValue(playerId, out var t) ? t : new { In = 0d, Out = 0d };
            var donationTotal = SumDonationTotal(playerEvents);
            var goldQty = SumGoldQuantity(playerEvents);
            var ordersCompletedCount = playerEvents.Count(e => e.ActionType == "JualMasakan");
            var inventoryIngredientTotal = _inventoryCalc.BuildIngredientInventory(playerEvents).Total;
            var actionsUsedTotal = SumActionsUsed(playerEvents);
            var playerProjections = projections.Where(p => p.UserId == playerId).ToList();
            var fulfillmentDiversity = _needMissionCalculator.Compute(playerEvents, playerProjections, config).FulfillmentDiversity ?? 0d;
            var happiness = happinessByPlayer.TryGetValue(playerId, out var breakdown)
                ? breakdown
                : _happinessCalc.ComputeBreakdown(playerEvents, 0, 0, 0);

            result.Add(new AnalyticsByPlayerItem(
                playerId,
                playerOrder,
                totals.In,
                totals.Out,
                donationTotal,
                goldQty,
                ordersCompletedCount,
                inventoryIngredientTotal,
                actionsUsedTotal,
                fulfillmentDiversity,
                happiness.Total,
                happiness.NeedPoints,
                happiness.NeedSetBonusPoints,
                happiness.DonationPoints,
                happiness.GoldPoints,
                happiness.PensionPoints,
                happiness.SavingGoalPointsEffective,
                happiness.MissionPenaltyPoints,
                happiness.LoanPenaltyPoints,
                happiness.HasUnpaidLoan,
                happiness.InitialHappinessPoints,
                happiness.MissionRewardPoints));
        }

        return _playerOrdering.OrderPlayers(
            result,
            config?.PlayerOrdering ?? PlayerOrdering.PlayerOrder,
            playerPlayerOrders,
            firstEventSequenceByPlayer,
            usernamesByPlayer);
    }

    internal static List<AnalyticsLeaderboardItem> BuildFinalLeaderboard(
        IEnumerable<AnalyticsByPlayerItem> players,
        IReadOnlyCollection<SessionFinalScoreDb>? finalScores = null)
    {
        if (finalScores is { Count: > 0 })
        {
            return finalScores
                .OrderBy(score => score.Rank)
                .ThenBy(score => score.PlayerOrder)
                .Select(score => new AnalyticsLeaderboardItem(
                    score.UserId,
                    score.PlayerOrder,
                    score.Rank,
                    score.TotalPoints))
                .ToList();
        }

        return players
            .OrderByDescending(player => player.HappinessPointsTotal)
            .ThenByDescending(player => player.CashInTotal - player.CashOutTotal)
            .ThenBy(player => player.PlayerOrder > 0 ? player.PlayerOrder : int.MaxValue)
            .ThenBy(player => player.UserId)
            .Select((player, index) => new AnalyticsLeaderboardItem(
                player.UserId,
                player.PlayerOrder,
                index + 1,
                player.HappinessPointsTotal))
            .ToList();
    }

    internal static Dictionary<Guid, AnalyticsHappinessBreakdown> ApplyFinalScores(
        Dictionary<Guid, AnalyticsHappinessBreakdown> computed,
        IReadOnlyCollection<SessionFinalScoreDb> finalScores)
    {
        if (finalScores.Count == 0)
        {
            return computed;
        }

        var authoritative = new Dictionary<Guid, AnalyticsHappinessBreakdown>(computed);
        foreach (var score in finalScores)
        {
            authoritative[score.UserId] = new AnalyticsHappinessBreakdown(
                score.TotalPoints,
                score.NeedPoints,
                score.NeedSetBonusPoints,
                score.DonationPoints,
                score.GoldPoints,
                score.PensionPoints,
                score.SavingGoalPoints,
                score.MissionPenaltyPoints,
                score.LoanPenaltyPoints,
                score.HasUnpaidLoan,
                score.InitialHappinessPoints,
                score.MissionRewardPoints);
        }

        return authoritative;
    }

    private async Task<List<SessionFinalScoreDb>> ResolveFinalScoresAsync(
        Guid sessionId,
        string? status,
        CancellationToken ct)
    {
        return string.Equals(status, "ENDED", StringComparison.OrdinalIgnoreCase)
            ? await _sessions.GetFinalScoresAsync(sessionId, ct)
            : [];
    }

    private async Task WriteSnapshotsAsync(
        Guid sessionId,
        Guid rulesetVersionId,
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        RulesetConfig? config,
        Dictionary<Guid, AnalyticsHappinessBreakdown> happinessByPlayer,
        IReadOnlyCollection<SessionFinalScoreDb> finalScores,
        bool sessionEnded,
        CancellationToken ct)
    {
        var computedAt = DateTimeOffset.UtcNow;
        var snapshots = new List<MetricSnapshotDb>();

        var sessionMetrics = _sessionMetricCalc.ComputeSessionMetrics(events, projections, happinessByPlayer);
        snapshots.AddRange(_metricSnapshotBuilder.BuildMetricSnapshots(sessionId, null, rulesetVersionId, computedAt, sessionMetrics));

        var playerConfig = config;
        if (playerConfig is null)
        {
            var rulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(rulesetVersionId, ct);
            TryBuildRuntimeConfig(rulesetVersion, out playerConfig);
        }

        var sessionPlayersByUser = (await _players.ListSessionPlayersAsync(sessionId, ct))
            .ToDictionary(player => player.UserId);
        foreach (var playerId in sessionPlayersByUser.Keys)
        {
            var hasHappiness = happinessByPlayer.TryGetValue(playerId, out var breakdown);
            var playerMetrics = ComputePlayerMetrics(playerId, events, projections,
                hasHappiness ? breakdown : null,
                playerConfig,
                finalScores.FirstOrDefault(score => score.UserId == playerId),
                sessionPlayersByUser.GetValueOrDefault(playerId)?.DisplayName,
                sessionEnded,
                sessionPlayersByUser.Keys);
            var playerSnapshots = _metricSnapshotBuilder.BuildMetricSnapshots(
                sessionId,
                playerId,
                rulesetVersionId,
                computedAt,
                playerMetrics);
            var sessionPlayerId = sessionPlayersByUser.GetValueOrDefault(playerId)?.SessionPlayerId;
            foreach (var snapshot in playerSnapshots)
            {
                snapshot.SessionPlayerId = sessionPlayerId;
            }

            snapshots.AddRange(playerSnapshots);
        }

        if (snapshots.Count > 0)
        {
            var lastEventId = events
                .OrderByDescending(item => item.SequenceNumber)
                .Select(item => (Guid?)item.EventId)
                .FirstOrDefault();
            foreach (var snapshot in snapshots)
            {
                snapshot.LastEventId = lastEventId;
            }

            await _metrics.InsertSnapshotsAsync(snapshots, ct);
        }
    }

    private Dictionary<string, (double? Numeric, string? Json)> ComputePlayerMetrics(
        Guid playerId,
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        AnalyticsHappinessBreakdown? happiness,
        RulesetConfig? config,
        SessionFinalScoreDb? finalScore,
        string? playerAlias,
        bool sessionEnded,
        IEnumerable<Guid> participantIds)
    {
        var metrics = new Dictionary<string, (double? Numeric, string? Json)>();
        var playerEvents = events.Where(e => e.UserId == playerId).ToList();
        var playerProjections = projections.Where(p => p.UserId == playerId).ToList();

        var cashIn = playerProjections.Where(p => p.Direction == "IN").Sum(p => (double)p.Amount);
        var cashOut = playerProjections.Where(p => p.Direction == "OUT").Sum(p => (double)p.Amount);
        metrics["cashflow.in.total"] = (cashIn, null);
        metrics["cashflow.out.total"] = (cashOut, null);
        metrics["cashflow.net.total"] = (cashIn - cashOut, null);

        var donationTotal = SumDonationTotal(playerEvents);
        metrics["donation.total"] = (donationTotal, null);

        var goldQty = SumGoldQuantity(playerEvents);
        metrics["gold.qty.current"] = (goldQty, null);

        var ordersCompleted = playerEvents.Count(e => e.ActionType == "JualMasakan");
        metrics["orders.completed.count"] = (ordersCompleted, null);

        var inventory = _inventoryCalc.BuildIngredientInventory(playerEvents);
        metrics["inventory.ingredient.total"] = (inventory.Total, null);

        var actionsUsed = SumActionsUsed(playerEvents);
        metrics["actions.used.total"] = (actionsUsed, null);

        var fulfillmentDiversity = _needMissionCalculator.Compute(playerEvents, playerProjections, config).FulfillmentDiversity;
        metrics["needs.fulfillment_diversity"] = (fulfillmentDiversity, null);

        var resolvedHappiness = happiness ?? _happinessCalc.ComputeBreakdown(
            playerEvents,
            _happinessCalc.SumRankAwarded(playerEvents, "PoinPeringkatDonasi"),
            _happinessCalc.SumPointsAwarded(playerEvents, "PoinEmas"),
            _happinessCalc.SumRankAwarded(playerEvents, "PoinPeringkatPensiun"));
        var pensionRank = config is not null && sessionEnded
            ? _happinessCalc.ComputePensionRanks(events, projections, config, participantIds).GetValueOrDefault(playerId)
            : (int?)null;

        metrics["happiness.points.total"] = (resolvedHappiness.Total, null);
        metrics["happiness.need.points"] = (resolvedHappiness.NeedPoints, null);
        metrics["happiness.need.bonus"] = (resolvedHappiness.NeedSetBonusPoints, null);
        metrics["happiness.donation.points"] = (resolvedHappiness.DonationPoints, null);
        metrics["happiness.gold.points"] = (resolvedHappiness.GoldPoints, null);
        metrics["happiness.pension.points"] = (resolvedHappiness.PensionPoints, null);
        metrics["happiness.saving_goal.points"] = (resolvedHappiness.SavingGoalPointsEffective, null);
        metrics["happiness.mission.penalty"] = (resolvedHappiness.MissionPenaltyPoints, null);
        metrics["happiness.initial"] = (resolvedHappiness.InitialHappinessPoints, null);
        metrics["happiness.mission.reward"] = (resolvedHappiness.MissionRewardPoints, null);
        metrics["happiness.loan.penalty"] = (resolvedHappiness.LoanPenaltyPoints, null);
        metrics["loan.unpaid.flag"] = (resolvedHappiness.HasUnpaidLoan ? 1 : 0, null);

        var gameplaySnapshots = _gameplaySnapshotBuilder.Build(
            playerEvents,
            playerProjections,
            events,
            config,
            resolvedHappiness,
            finalScore,
            pensionRank,
            playerAlias,
            sessionEnded);
        metrics["gameplay.raw.variables"] = (null, gameplaySnapshots.RawJson);
        metrics["gameplay.derived.metrics"] = (null, gameplaySnapshots.DerivedJson);

        return metrics;
    }

    private double SumDonationTotal(IEnumerable<EventDb> events)
        => events
            .Where(e => e.ActionType == "JumatBerkah")
            .Select(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0)
            .Sum();

    private int SumGoldQuantity(IEnumerable<EventDb> events)
        => events
            .Where(e => e.ActionType == GameActionCatalog.SetupEmasAwal ||
                        e.ActionType == GameActionCatalog.InvestasiEmas ||
                        e.ActionType == GameActionCatalog.JualEmas ||
                        IsEmergencyGoldSale(e))
            .Select(e =>
            {
                if (e.ActionType == GameActionCatalog.SetupEmasAwal)
                {
                    return TryReadPayloadInt(e.Payload, "qty", out var initialQty) ? initialQty : 1;
                }

                if (e.ActionType == GameActionCatalog.RiskEmergencyUsed)
                {
                    return TryReadPayloadInt(e.Payload, "qty", out var emergencyQty) ? -emergencyQty : 0;
                }

                if (!_payloadReader.TryReadGoldTrade(e.Payload, out var tradeType, out var qty))
                {
                    return 0;
                }

                return e.ActionType == GameActionCatalog.JualEmas ||
                       string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase)
                    ? -qty
                    : qty;
            })
            .Sum();

    private bool IsEmergencyGoldSale(EventDb evt) =>
        evt.ActionType == GameActionCatalog.RiskEmergencyUsed &&
        TryReadPayloadString(evt.Payload, "option_type", out var optionType) &&
        string.Equals(optionType, "SELL_GOLD", StringComparison.OrdinalIgnoreCase);

    private static bool TryReadPayloadString(string payload, string propertyName, out string value)
    {
        value = string.Empty;
        try
        {
            using var document = JsonDocument.Parse(payload);
            if (!document.RootElement.TryGetProperty(propertyName, out var property) || property.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            value = property.GetString() ?? string.Empty;
            return value.Length > 0;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryReadPayloadInt(string payload, string propertyName, out int value)
    {
        value = 0;
        try
        {
            using var document = JsonDocument.Parse(payload);
            return document.RootElement.TryGetProperty(propertyName, out var property) && property.TryGetInt32(out value);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private int SumActionsUsed(IEnumerable<EventDb> events)
        => events
            .Count(e => string.Equals(e.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
                        GameActionCatalog.GetPlayerActionSlotPolicy(
                            e.ActionType,
                            _eventPayloadReader.ReadPayload(string.IsNullOrWhiteSpace(e.Payload) ? "{}" : e.Payload)) == PlayerActionSlotPolicy.Consumes);

    private static bool TryBuildRuntimeConfig(RulesetVersionDb? rulesetVersion, out RulesetConfig? config)
    {
        config = null;
        return rulesetVersion?.Definition is not null &&
               RulesetRuntimeMapper.TryBuildConfig(rulesetVersion.Definition, out config, out _);
    }
}
