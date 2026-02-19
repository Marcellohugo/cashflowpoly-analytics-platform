// Fungsi file: Mendefinisikan ViewModel/DTO UI untuk domain ApiDtos.
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cashflowpoly.Ui.Models;

/// <summary>
/// Menyatakan peran utama tipe RulesetListItemDto pada modul ini.
/// </summary>
public sealed record RulesetListItemDto(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("latest_version")] int LatestVersion);

/// <summary>
/// Menyatakan peran utama tipe RulesetListResponseDto pada modul ini.
/// </summary>
public sealed record RulesetListResponseDto(
    [property: JsonPropertyName("items")] List<RulesetListItemDto> Items);

/// <summary>
/// Menyatakan peran utama tipe SessionListItemDto pada modul ini.
/// </summary>
public sealed record SessionListItemDto(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("session_name")] string SessionName,
    [property: JsonPropertyName("mode")] string Mode,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("started_at")] DateTimeOffset? StartedAt,
    [property: JsonPropertyName("ended_at")] DateTimeOffset? EndedAt);

/// <summary>
/// Menyatakan peran utama tipe SessionListResponseDto pada modul ini.
/// </summary>
public sealed record SessionListResponseDto(
    [property: JsonPropertyName("items")] List<SessionListItemDto> Items);

/// <summary>
/// Menyatakan peran utama tipe RulesetVersionItemDto pada modul ini.
/// </summary>
public sealed record RulesetVersionItemDto(
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt);

/// <summary>
/// Menyatakan peran utama tipe RulesetDetailResponseDto pada modul ini.
/// </summary>
public sealed record RulesetDetailResponseDto(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("is_archived")] bool IsArchived,
    [property: JsonPropertyName("versions")] List<RulesetVersionItemDto> Versions,
    [property: JsonPropertyName("config_json")] JsonElement? ConfigJson);

/// <summary>
/// Menyatakan peran utama tipe AnalyticsSessionSummaryDto pada modul ini.
/// </summary>
public sealed record AnalyticsSessionSummaryDto(
    [property: JsonPropertyName("event_count")] int EventCount,
    [property: JsonPropertyName("cash_in_total")] double CashInTotal,
    [property: JsonPropertyName("cash_out_total")] double CashOutTotal,
    [property: JsonPropertyName("cashflow_net_total")] double CashflowNetTotal,
    [property: JsonPropertyName("rules_violations_count")] int RulesViolationsCount);

/// <summary>
/// Menyatakan peran utama tipe AnalyticsByPlayerItemDto pada modul ini.
/// </summary>
public sealed record AnalyticsByPlayerItemDto(
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
/// Menyatakan peran utama tipe AnalyticsSessionResponseDto pada modul ini.
/// </summary>
public sealed record AnalyticsSessionResponseDto(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("summary")] AnalyticsSessionSummaryDto Summary,
    [property: JsonPropertyName("by_player")] List<AnalyticsByPlayerItemDto> ByPlayer,
    [property: JsonPropertyName("ruleset_id")] Guid? RulesetId,
    [property: JsonPropertyName("ruleset_name")] string? RulesetName);

/// <summary>
/// Menyatakan peran utama tipe GameplayMetricsResponseDto pada modul ini.
/// </summary>
public sealed record GameplayMetricsResponseDto(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("computed_at")] DateTimeOffset? ComputedAt,
    [property: JsonPropertyName("raw")] JsonElement? Raw,
    [property: JsonPropertyName("derived")] JsonElement? Derived);

/// <summary>
/// Menyatakan peran utama tipe TransactionHistoryItemDto pada modul ini.
/// </summary>
public sealed record TransactionHistoryItemDto(
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    [property: JsonPropertyName("direction")] string Direction,
    [property: JsonPropertyName("amount")] double Amount,
    [property: JsonPropertyName("category")] string Category);

/// <summary>
/// Menyatakan peran utama tipe TransactionHistoryResponseDto pada modul ini.
/// </summary>
public sealed record TransactionHistoryResponseDto(
    [property: JsonPropertyName("items")] List<TransactionHistoryItemDto> Items);

/// <summary>
/// Menyatakan peran utama tipe RulesetAnalyticsPlayerItemDto pada modul ini.
/// </summary>
public sealed record RulesetAnalyticsPlayerItemDto(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("learning_performance_individual_score")] double? LearningPerformanceIndividualScore,
    [property: JsonPropertyName("mission_performance_individual_score")] double? MissionPerformanceIndividualScore);

/// <summary>
/// Menyatakan peran utama tipe RulesetAnalyticsSessionItemDto pada modul ini.
/// </summary>
public sealed record RulesetAnalyticsSessionItemDto(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("session_name")] string SessionName,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("event_count")] int EventCount,
    [property: JsonPropertyName("learning_performance_aggregate_score")] double? LearningPerformanceAggregateScore,
    [property: JsonPropertyName("mission_performance_aggregate_score")] double? MissionPerformanceAggregateScore,
    [property: JsonPropertyName("players")] List<RulesetAnalyticsPlayerItemDto> Players);

/// <summary>
/// Menyatakan peran utama tipe RulesetAnalyticsSummaryResponseDto pada modul ini.
/// </summary>
public sealed record RulesetAnalyticsSummaryResponseDto(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("ruleset_name")] string RulesetName,
    [property: JsonPropertyName("session_count")] int SessionCount,
    [property: JsonPropertyName("learning_performance_aggregate_score")] double? LearningPerformanceAggregateScore,
    [property: JsonPropertyName("mission_performance_aggregate_score")] double? MissionPerformanceAggregateScore,
    [property: JsonPropertyName("sessions")] List<RulesetAnalyticsSessionItemDto> Sessions);

/// <summary>
/// Menyatakan peran utama tipe PlayerResponseDto pada modul ini.
/// </summary>
public sealed record PlayerResponseDto(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("display_name")] string DisplayName);

/// <summary>
/// Menyatakan peran utama tipe PlayerListResponseDto pada modul ini.
/// </summary>
public sealed record PlayerListResponseDto(
    [property: JsonPropertyName("items")] List<PlayerResponseDto> Items);

/// <summary>
/// Menyatakan peran utama tipe EventRequestDto pada modul ini.
/// </summary>
public sealed record EventRequestDto(
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
/// Menyatakan peran utama tipe EventsBySessionResponseDto pada modul ini.
/// </summary>
public sealed record EventsBySessionResponseDto(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("events")] List<EventRequestDto> Events);

/// <summary>
/// Menyatakan peran utama tipe ApiErrorDetailDto pada modul ini.
/// </summary>
public sealed record ApiErrorDetailDto(
    [property: JsonPropertyName("field")] string Field,
    [property: JsonPropertyName("issue")] string Issue);

/// <summary>
/// Menyatakan peran utama tipe ApiErrorResponseDto pada modul ini.
/// </summary>
public sealed record ApiErrorResponseDto(
    [property: JsonPropertyName("error_code")] string ErrorCode,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("details")] List<ApiErrorDetailDto> Details,
    [property: JsonPropertyName("trace_id")] string TraceId);

/// <summary>
/// Menyatakan peran utama tipe LoginRequestDto pada modul ini.
/// </summary>
public sealed record LoginRequestDto(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password);

/// <summary>
/// Menyatakan peran utama tipe LoginResponseDto pada modul ini.
/// </summary>
public sealed record LoginResponseDto(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);

/// <summary>
/// Menyatakan peran utama tipe RegisterRequestDto pada modul ini.
/// </summary>
public sealed record RegisterRequestDto(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("display_name")] string? DisplayName);

/// <summary>
/// Menyatakan peran utama tipe RegisterResponseDto pada modul ini.
/// </summary>
public sealed record RegisterResponseDto(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);
