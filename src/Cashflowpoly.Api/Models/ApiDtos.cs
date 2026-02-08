using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cashflowpoly.Api.Models;

public sealed record CreateSessionRequest(
    [property: JsonPropertyName("session_name")] string SessionName,
    [property: JsonPropertyName("mode")] string Mode,
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId);

public sealed record CreateSessionResponse([property: JsonPropertyName("session_id")] Guid SessionId);

public sealed record SessionStatusResponse([property: JsonPropertyName("status")] string Status);

public sealed record CreatePlayerRequest(
    [property: JsonPropertyName("display_name")] string DisplayName);

public sealed record PlayerResponse(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("display_name")] string DisplayName);

public sealed record PlayerListResponse([property: JsonPropertyName("items")] List<PlayerResponse> Items);

public sealed record AddSessionPlayerRequest(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("role")] string? Role,
    [property: JsonPropertyName("join_order")] int? JoinOrder);

public sealed record SessionListItem(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("session_name")] string SessionName,
    [property: JsonPropertyName("mode")] string Mode,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("started_at")] DateTimeOffset? StartedAt,
    [property: JsonPropertyName("ended_at")] DateTimeOffset? EndedAt);

public sealed record SessionListResponse([property: JsonPropertyName("items")] List<SessionListItem> Items);

public sealed record ActivateRulesetRequest(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("version")] int Version);

public sealed record ActivateRulesetResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId);

public sealed record CreateRulesetRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("config")] JsonElement Config);

public sealed record UpdateRulesetRequest(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("config")] JsonElement? Config);

public sealed record CreateRulesetResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("version")] int Version);

public sealed record RulesetListItem(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("latest_version")] int LatestVersion);

public sealed record RulesetListResponse([property: JsonPropertyName("items")] List<RulesetListItem> Items);

public sealed record RulesetVersionItem(
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt);

public sealed record RulesetDetailResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("is_archived")] bool IsArchived,
    [property: JsonPropertyName("versions")] List<RulesetVersionItem> Versions,
    [property: JsonPropertyName("config_json")] JsonElement? ConfigJson);

public sealed record EventRequest(
    [property: JsonPropertyName("event_id")] Guid EventId,
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("player_id")] Guid? PlayerId,
    [property: JsonPropertyName("actor_type")] string ActorType,
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    [property: JsonPropertyName("day_index")] int DayIndex,
    [property: JsonPropertyName("weekday")] string Weekday,
    [property: JsonPropertyName("turn_number")] int TurnNumber,
    [property: JsonPropertyName("sequence_number")] long SequenceNumber,
    [property: JsonPropertyName("action_type")] string ActionType,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("payload")] JsonElement Payload,
    [property: JsonPropertyName("client_request_id")] string? ClientRequestId);

public sealed record EventStoredResponse(
    [property: JsonPropertyName("stored")] bool Stored,
    [property: JsonPropertyName("event_id")] Guid EventId);

public sealed record EventBatchRequest([property: JsonPropertyName("events")] List<EventRequest> Events);

public sealed record EventBatchFailed(
    [property: JsonPropertyName("event_id")] Guid EventId,
    [property: JsonPropertyName("error_code")] string ErrorCode);

public sealed record EventBatchResponse(
    [property: JsonPropertyName("stored_count")] int StoredCount,
    [property: JsonPropertyName("failed")] List<EventBatchFailed> Failed);

public sealed record EventsBySessionResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("events")] List<EventRequest> Events);

public sealed record AnalyticsSessionSummary(
    [property: JsonPropertyName("event_count")] int EventCount,
    [property: JsonPropertyName("cash_in_total")] double CashInTotal,
    [property: JsonPropertyName("cash_out_total")] double CashOutTotal,
    [property: JsonPropertyName("cashflow_net_total")] double CashflowNetTotal,
    [property: JsonPropertyName("rules_violations_count")] int RulesViolationsCount);

public sealed record AnalyticsByPlayerItem(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("cash_in_total")] double CashInTotal,
    [property: JsonPropertyName("cash_out_total")] double CashOutTotal,
    [property: JsonPropertyName("donation_total")] double DonationTotal,
    [property: JsonPropertyName("gold_qty")] int GoldQty,
    [property: JsonPropertyName("happiness_points_total")] double HappinessPointsTotal,
    [property: JsonPropertyName("need_points_total")] double NeedPointsTotal,
    [property: JsonPropertyName("need_set_bonus_points")] double NeedSetBonusPoints,
    [property: JsonPropertyName("donation_points_total")] double DonationPointsTotal,
    [property: JsonPropertyName("gold_points_total")] double GoldPointsTotal,
    [property: JsonPropertyName("pension_points_total")] double PensionPointsTotal,
    [property: JsonPropertyName("saving_goal_points_total")] double SavingGoalPointsTotal,
    [property: JsonPropertyName("mission_penalty_total")] double MissionPenaltyTotal,
    [property: JsonPropertyName("loan_penalty_total")] double LoanPenaltyTotal,
    [property: JsonPropertyName("has_unpaid_loan")] bool HasUnpaidLoan);

public sealed record AnalyticsSessionResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("summary")] AnalyticsSessionSummary Summary,
    [property: JsonPropertyName("by_player")] List<AnalyticsByPlayerItem> ByPlayer);

public sealed record GameplayMetricsResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("computed_at")] DateTimeOffset? ComputedAt,
    [property: JsonPropertyName("raw")] JsonElement? Raw,
    [property: JsonPropertyName("derived")] JsonElement? Derived);

public sealed record TransactionHistoryItem(
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    [property: JsonPropertyName("direction")] string Direction,
    [property: JsonPropertyName("amount")] double Amount,
    [property: JsonPropertyName("category")] string Category);

public sealed record TransactionHistoryResponse(
    [property: JsonPropertyName("items")] List<TransactionHistoryItem> Items);

public sealed record RulesetAnalyticsPlayerItem(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("learning_performance_individual_score")] double? LearningPerformanceIndividualScore,
    [property: JsonPropertyName("mission_performance_individual_score")] double? MissionPerformanceIndividualScore);

public sealed record RulesetAnalyticsSessionItem(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("session_name")] string SessionName,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("event_count")] int EventCount,
    [property: JsonPropertyName("learning_performance_aggregate_score")] double? LearningPerformanceAggregateScore,
    [property: JsonPropertyName("mission_performance_aggregate_score")] double? MissionPerformanceAggregateScore,
    [property: JsonPropertyName("players")] List<RulesetAnalyticsPlayerItem> Players);

public sealed record RulesetAnalyticsSummaryResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("ruleset_name")] string RulesetName,
    [property: JsonPropertyName("session_count")] int SessionCount,
    [property: JsonPropertyName("learning_performance_aggregate_score")] double? LearningPerformanceAggregateScore,
    [property: JsonPropertyName("mission_performance_aggregate_score")] double? MissionPerformanceAggregateScore,
    [property: JsonPropertyName("sessions")] List<RulesetAnalyticsSessionItem> Sessions);

public sealed record LoginRequest(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password);

public sealed record LoginResponse(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);

public sealed record RegisterRequest(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("role")] string Role);

public sealed record RegisterResponse(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);
