// Fungsi file: Mendefinisikan kontrak data API pada domain ApiDtos.
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cashflowpoly.Api.Models;

/// <summary>
/// Menyatakan peran utama tipe CreateSessionRequest pada modul ini.
/// </summary>
public sealed record CreateSessionRequest(
    [property: JsonPropertyName("session_name")] string SessionName,
    [property: JsonPropertyName("mode")] string Mode,
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId);

/// <summary>
/// Menyatakan peran utama tipe CreateSessionResponse pada modul ini.
/// </summary>
public sealed record CreateSessionResponse([property: JsonPropertyName("session_id")] Guid SessionId);

/// <summary>
/// Menyatakan peran utama tipe SessionStatusResponse pada modul ini.
/// </summary>
public sealed record SessionStatusResponse([property: JsonPropertyName("status")] string Status);

/// <summary>
/// Menyatakan peran utama tipe CreatePlayerRequest pada modul ini.
/// </summary>
public sealed record CreatePlayerRequest(
    [property: JsonPropertyName("display_name")] string DisplayName,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password);

/// <summary>
/// Menyatakan peran utama tipe PlayerResponse pada modul ini.
/// </summary>
public sealed record PlayerResponse(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("display_name")] string DisplayName);

/// <summary>
/// Menyatakan peran utama tipe PlayerListResponse pada modul ini.
/// </summary>
public sealed record PlayerListResponse([property: JsonPropertyName("items")] List<PlayerResponse> Items);

/// <summary>
/// Menyatakan peran utama tipe AddSessionPlayerRequest pada modul ini.
/// </summary>
public sealed record AddSessionPlayerRequest(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("role")] string? Role);

/// <summary>
/// Menyatakan peran utama tipe AddSessionPlayerResponse pada modul ini.
/// </summary>
public sealed record AddSessionPlayerResponse(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("join_order")] int JoinOrder);

/// <summary>
/// Menyatakan peran utama tipe SessionListItem pada modul ini.
/// </summary>
public sealed record SessionListItem(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("session_name")] string SessionName,
    [property: JsonPropertyName("mode")] string Mode,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("started_at")] DateTimeOffset? StartedAt,
    [property: JsonPropertyName("ended_at")] DateTimeOffset? EndedAt);

/// <summary>
/// Menyatakan peran utama tipe SessionListResponse pada modul ini.
/// </summary>
public sealed record SessionListResponse([property: JsonPropertyName("items")] List<SessionListItem> Items);

/// <summary>
/// Menyatakan peran utama tipe ActivateRulesetRequest pada modul ini.
/// </summary>
public sealed record ActivateRulesetRequest(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("version")] int Version);

/// <summary>
/// Menyatakan peran utama tipe ActivateRulesetResponse pada modul ini.
/// </summary>
public sealed record ActivateRulesetResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId);

/// <summary>
/// Menyatakan peran utama tipe CreateRulesetRequest pada modul ini.
/// </summary>
public sealed record CreateRulesetRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("config")] JsonElement Config);

/// <summary>
/// Menyatakan peran utama tipe UpdateRulesetRequest pada modul ini.
/// </summary>
public sealed record UpdateRulesetRequest(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("config")] JsonElement? Config);

/// <summary>
/// Menyatakan peran utama tipe CreateRulesetResponse pada modul ini.
/// </summary>
public sealed record CreateRulesetResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("version")] int Version);

/// <summary>
/// Menyatakan peran utama tipe RulesetListItem pada modul ini.
/// </summary>
public sealed record RulesetListItem(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("latest_version")] int LatestVersion);

/// <summary>
/// Menyatakan peran utama tipe RulesetListResponse pada modul ini.
/// </summary>
public sealed record RulesetListResponse([property: JsonPropertyName("items")] List<RulesetListItem> Items);

/// <summary>
/// Menyatakan peran utama tipe RulesetVersionItem pada modul ini.
/// </summary>
public sealed record RulesetVersionItem(
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt);

/// <summary>
/// Menyatakan peran utama tipe RulesetDetailResponse pada modul ini.
/// </summary>
public sealed record RulesetDetailResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("is_archived")] bool IsArchived,
    [property: JsonPropertyName("versions")] List<RulesetVersionItem> Versions,
    [property: JsonPropertyName("config_json")] JsonElement? ConfigJson);

/// <summary>
/// Menyatakan peran utama tipe EventRequest pada modul ini.
/// </summary>
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

/// <summary>
/// Menyatakan peran utama tipe EventStoredResponse pada modul ini.
/// </summary>
public sealed record EventStoredResponse(
    [property: JsonPropertyName("stored")] bool Stored,
    [property: JsonPropertyName("event_id")] Guid EventId);

/// <summary>
/// Menyatakan peran utama tipe EventBatchRequest pada modul ini.
/// </summary>
public sealed record EventBatchRequest([property: JsonPropertyName("events")] List<EventRequest> Events);

/// <summary>
/// Menyatakan peran utama tipe EventBatchFailed pada modul ini.
/// </summary>
public sealed record EventBatchFailed(
    [property: JsonPropertyName("event_id")] Guid EventId,
    [property: JsonPropertyName("error_code")] string ErrorCode);

/// <summary>
/// Menyatakan peran utama tipe EventBatchResponse pada modul ini.
/// </summary>
public sealed record EventBatchResponse(
    [property: JsonPropertyName("stored_count")] int StoredCount,
    [property: JsonPropertyName("failed")] List<EventBatchFailed> Failed);

/// <summary>
/// Menyatakan peran utama tipe EventsBySessionResponse pada modul ini.
/// </summary>
public sealed record EventsBySessionResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("events")] List<EventRequest> Events);

/// <summary>
/// Menyatakan peran utama tipe AnalyticsSessionSummary pada modul ini.
/// </summary>
public sealed record AnalyticsSessionSummary(
    [property: JsonPropertyName("event_count")] int EventCount,
    [property: JsonPropertyName("cash_in_total")] double CashInTotal,
    [property: JsonPropertyName("cash_out_total")] double CashOutTotal,
    [property: JsonPropertyName("cashflow_net_total")] double CashflowNetTotal,
    [property: JsonPropertyName("rules_violations_count")] int RulesViolationsCount);

/// <summary>
/// Menyatakan peran utama tipe AnalyticsByPlayerItem pada modul ini.
/// </summary>
public sealed record AnalyticsByPlayerItem(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("join_order")] int JoinOrder,
    [property: JsonPropertyName("cash_in_total")] double CashInTotal,
    [property: JsonPropertyName("cash_out_total")] double CashOutTotal,
    [property: JsonPropertyName("donation_total")] double DonationTotal,
    [property: JsonPropertyName("gold_qty")] int GoldQty,
    [property: JsonPropertyName("orders_completed_count")] int OrdersCompletedCount,
    [property: JsonPropertyName("inventory_ingredient_total")] int InventoryIngredientTotal,
    [property: JsonPropertyName("actions_used_total")] int ActionsUsedTotal,
    [property: JsonPropertyName("compliance_primary_need_rate")] double CompliancePrimaryNeedRate,
    [property: JsonPropertyName("rules_violations_count")] int RulesViolationsCount,
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

/// <summary>
/// Menyatakan peran utama tipe AnalyticsSessionResponse pada modul ini.
/// </summary>
public sealed record AnalyticsSessionResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("summary")] AnalyticsSessionSummary Summary,
    [property: JsonPropertyName("by_player")] List<AnalyticsByPlayerItem> ByPlayer,
    [property: JsonPropertyName("ruleset_id")] Guid? RulesetId,
    [property: JsonPropertyName("ruleset_name")] string? RulesetName);

/// <summary>
/// Menyatakan peran utama tipe GameplayMetricsResponse pada modul ini.
/// </summary>
public sealed record GameplayMetricsResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("computed_at")] DateTimeOffset? ComputedAt,
    [property: JsonPropertyName("raw")] JsonElement? Raw,
    [property: JsonPropertyName("derived")] JsonElement? Derived);

/// <summary>
/// Menyatakan peran utama tipe TransactionHistoryItem pada modul ini.
/// </summary>
public sealed record TransactionHistoryItem(
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    [property: JsonPropertyName("direction")] string Direction,
    [property: JsonPropertyName("amount")] double Amount,
    [property: JsonPropertyName("category")] string Category);

/// <summary>
/// Menyatakan peran utama tipe TransactionHistoryResponse pada modul ini.
/// </summary>
public sealed record TransactionHistoryResponse(
    [property: JsonPropertyName("items")] List<TransactionHistoryItem> Items);

/// <summary>
/// Menyatakan peran utama tipe RulesetAnalyticsPlayerItem pada modul ini.
/// </summary>
public sealed record RulesetAnalyticsPlayerItem(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("learning_performance_individual_score")] double? LearningPerformanceIndividualScore,
    [property: JsonPropertyName("mission_performance_individual_score")] double? MissionPerformanceIndividualScore);

/// <summary>
/// Menyatakan peran utama tipe RulesetAnalyticsSessionItem pada modul ini.
/// </summary>
public sealed record RulesetAnalyticsSessionItem(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("session_name")] string SessionName,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("event_count")] int EventCount,
    [property: JsonPropertyName("learning_performance_aggregate_score")] double? LearningPerformanceAggregateScore,
    [property: JsonPropertyName("mission_performance_aggregate_score")] double? MissionPerformanceAggregateScore,
    [property: JsonPropertyName("players")] List<RulesetAnalyticsPlayerItem> Players);

/// <summary>
/// Menyatakan peran utama tipe RulesetAnalyticsSummaryResponse pada modul ini.
/// </summary>
public sealed record RulesetAnalyticsSummaryResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("ruleset_name")] string RulesetName,
    [property: JsonPropertyName("session_count")] int SessionCount,
    [property: JsonPropertyName("learning_performance_aggregate_score")] double? LearningPerformanceAggregateScore,
    [property: JsonPropertyName("mission_performance_aggregate_score")] double? MissionPerformanceAggregateScore,
    [property: JsonPropertyName("sessions")] List<RulesetAnalyticsSessionItem> Sessions);

/// <summary>
/// Menyatakan peran utama tipe LoginRequest pada modul ini.
/// </summary>
public sealed record LoginRequest(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password);

/// <summary>
/// Menyatakan peran utama tipe LoginResponse pada modul ini.
/// </summary>
public sealed record LoginResponse(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);

/// <summary>
/// Menyatakan peran utama tipe RegisterRequest pada modul ini.
/// </summary>
public sealed record RegisterRequest(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("display_name")] string? DisplayName);

/// <summary>
/// Menyatakan peran utama tipe RegisterResponse pada modul ini.
/// </summary>
public sealed record RegisterResponse(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);

/// <summary>
/// Menyatakan peran utama tipe SecurityAuditLogItem pada modul ini.
/// </summary>
public sealed record SecurityAuditLogItem(
    [property: JsonPropertyName("security_audit_log_id")] Guid SecurityAuditLogId,
    [property: JsonPropertyName("occurred_at")] DateTimeOffset OccurredAt,
    [property: JsonPropertyName("trace_id")] string TraceId,
    [property: JsonPropertyName("event_type")] string EventType,
    [property: JsonPropertyName("outcome")] string Outcome,
    [property: JsonPropertyName("user_id")] Guid? UserId,
    [property: JsonPropertyName("username")] string? Username,
    [property: JsonPropertyName("role")] string? Role,
    [property: JsonPropertyName("ip_address")] string? IpAddress,
    [property: JsonPropertyName("user_agent")] string? UserAgent,
    [property: JsonPropertyName("method")] string Method,
    [property: JsonPropertyName("path")] string Path,
    [property: JsonPropertyName("status_code")] int StatusCode,
    [property: JsonPropertyName("detail")] JsonElement? Detail);

/// <summary>
/// Menyatakan peran utama tipe SecurityAuditLogResponse pada modul ini.
/// </summary>
public sealed record SecurityAuditLogResponse(
    [property: JsonPropertyName("items")] List<SecurityAuditLogItem> Items);
