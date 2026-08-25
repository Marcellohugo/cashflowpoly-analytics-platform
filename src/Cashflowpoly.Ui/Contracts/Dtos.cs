// Fungsi file: Mendefinisikan kontrak pertukaran data UI dengan API untuk Dtos.
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cashflowpoly.Ui.Contracts;

public sealed record PlayerResponse(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("display_name")] string DisplayName);

public sealed record PlayerListResponse(
    [property: JsonPropertyName("items")] List<PlayerResponse> Items);

public sealed record SessionPlayerResponse(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("display_name")] string DisplayName,
    [property: JsonPropertyName("player_order_no")] int PlayerOrder);

public sealed record SessionPlayerListResponse(
    [property: JsonPropertyName("items")] List<SessionPlayerResponse> Items);

public sealed record SessionListItem(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("session_name")] string SessionName,
    [property: JsonPropertyName("mode")] string Mode,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("started_at")] DateTimeOffset? StartedAt,
    [property: JsonPropertyName("ended_at")] DateTimeOffset? EndedAt);

public sealed record SessionListResponse(
    [property: JsonPropertyName("items")] List<SessionListItem> Items);

public sealed record CreateRulesetRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("definition")] RulesetDefinitionDto? Definition);

public sealed record UpdateRulesetRequest(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("definition")] RulesetDefinitionDto? Definition);

public sealed record CreateRulesetResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version);

public sealed record RulesetListItem(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("latest_version")] int LatestVersion,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("is_default")] bool IsDefault = false,
    [property: JsonPropertyName("is_locked_by_session")] bool IsLockedBySession = false,
    [property: JsonPropertyName("mode")] string? Mode = null);

public sealed record RulesetListResponse(
    [property: JsonPropertyName("items")] List<RulesetListItem> Items);

public sealed record RulesetVersionItem(
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt);

public sealed record RulesetSectionCatalogResponse(
    [property: JsonPropertyName("gameConfig")] JsonElement GameConfig,
    [property: JsonPropertyName("bahan")] JsonElement Bahan,
    [property: JsonPropertyName("resep")] JsonElement Resep,
    [property: JsonPropertyName("kebutuhan")] JsonElement Kebutuhan,
    [property: JsonPropertyName("targetKebutuhan")] JsonElement TargetKebutuhan,
    [property: JsonPropertyName("tujuanFinansial")] JsonElement TujuanFinansial,
    [property: JsonPropertyName("narasi")] JsonElement Narasi);

public sealed record RulesetDetailResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("versions")] List<RulesetVersionItem> Versions,
    [property: JsonPropertyName("ruleset_version_id")] Guid? RulesetVersionId = null,
    [property: JsonPropertyName("version")] int? Version = null,
    [property: JsonPropertyName("mode")] string? Mode = null,
    [property: JsonPropertyName("definition")] RulesetDefinitionDto? Definition = null,
    [property: JsonPropertyName("is_default")] bool IsDefault = false,
    [property: JsonPropertyName("is_locked_by_session")] bool IsLockedBySession = false);

public sealed record RulesetComponentsResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("mode")] string? Mode,
    [property: JsonPropertyName("definition")] RulesetDefinitionDto? Definition = null);

public sealed record DefaultRulesetComponentItem(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("mode")] string? Mode,
    [property: JsonPropertyName("definition")] RulesetDefinitionDto? Definition = null);

public sealed record DefaultRulesetComponentsResponse(
    [property: JsonPropertyName("items")] List<DefaultRulesetComponentItem> Items);

public sealed record EventRequest(
    [property: JsonPropertyName("event_id")] Guid EventId,
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("user_id")] Guid? UserId,
    [property: JsonPropertyName("actor_type")] string ActorType,
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    [property: JsonPropertyName("day_index")] int DayIndex,
    [property: JsonPropertyName("weekday")] string Weekday,
    [property: JsonPropertyName("action_slot")] int ActionSlot,
    [property: JsonPropertyName("sequence_number")] long SequenceNumber,
    [property: JsonPropertyName("action_type")] string ActionType,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("payload")] JsonElement Payload,
    [property: JsonPropertyName("client_request_id")] string? ClientRequestId,
    [property: JsonPropertyName("turn_number")] int TurnNumber = 0);

public sealed record EventsBySessionResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("items")] List<EventRequest> Items,
    [property: JsonPropertyName("next_cursor")] string? NextCursor,
    [property: JsonPropertyName("has_more")] bool HasMore);

public sealed record AnalyticsSessionSummary(
    [property: JsonPropertyName("event_count")] int EventCount,
    [property: JsonPropertyName("cash_in_total")] double CashInTotal,
    [property: JsonPropertyName("cash_out_total")] double CashOutTotal,
    [property: JsonPropertyName("cashflow_net_total")] double CashflowNetTotal,
    [property: JsonPropertyName("rules_violations_count")] int RulesViolationsCount);

public sealed record AnalyticsByPlayerItem(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("player_order_no")] int PlayerOrder,
    [property: JsonPropertyName("cash_in_total")] double CashInTotal,
    [property: JsonPropertyName("cash_out_total")] double CashOutTotal,
    [property: JsonPropertyName("donation_total")] double DonationTotal,
    [property: JsonPropertyName("gold_qty")] int GoldQty,
    [property: JsonPropertyName("orders_completed_count")] int OrdersCompletedCount,
    [property: JsonPropertyName("inventory_ingredient_total")] int InventoryIngredientTotal,
    [property: JsonPropertyName("actions_used_total")] int ActionsUsedTotal,
    [property: JsonPropertyName("fulfillment_diversity")] double FulfillmentDiversity,
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

public sealed record AnalyticsLeaderboardItem(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("player_order_no")] int PlayerOrder,
    [property: JsonPropertyName("rank")] int Rank,
    [property: JsonPropertyName("happiness_points_total")] double HappinessPointsTotal);

public sealed record AnalyticsSessionResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("summary")] AnalyticsSessionSummary Summary,
    [property: JsonPropertyName("by_player")] List<AnalyticsByPlayerItem> ByPlayer,
    [property: JsonPropertyName("ruleset_id")] Guid? RulesetId,
    [property: JsonPropertyName("ruleset_name")] string? RulesetName,
    [property: JsonPropertyName("leaderboard")] List<AnalyticsLeaderboardItem>? Leaderboard = null);

public sealed record GameplayMetricsResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("computed_at")] DateTimeOffset? ComputedAt,
    [property: JsonPropertyName("economy")] GameplayEconomyMetrics Economy,
    [property: JsonPropertyName("progress")] GameplayProgressMetrics Progress,
    [property: JsonPropertyName("score")] GameplayScoreMetrics Score,
    [property: JsonPropertyName("needs")] GameplayNeedMetrics Needs,
    [property: JsonPropertyName("rules_violations_count")] int RulesViolationsCount,
    [property: JsonPropertyName("raw_json")] JsonElement? RawJson = null,
    [property: JsonPropertyName("derived_json")] JsonElement? DerivedJson = null);

public sealed record GameplayEconomyMetrics(
    [property: JsonPropertyName("starting_cash")] double StartingCash,
    [property: JsonPropertyName("cash_in_total")] double CashInTotal,
    [property: JsonPropertyName("cash_out_total")] double CashOutTotal,
    [property: JsonPropertyName("cashflow_net_total")] double CashflowNetTotal,
    [property: JsonPropertyName("donation_total")] double DonationTotal);

public sealed record GameplayProgressMetrics(
    [property: JsonPropertyName("gold_qty")] int GoldQty,
    [property: JsonPropertyName("orders_completed_count")] int OrdersCompletedCount,
    [property: JsonPropertyName("inventory_ingredient_total")] int InventoryIngredientTotal,
    [property: JsonPropertyName("actions_used_total")] int ActionsUsedTotal);

public sealed record GameplayScoreMetrics(
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

public sealed record GameplayNeedMetrics(
    [property: JsonPropertyName("fulfillment_diversity")] double FulfillmentDiversity);

public sealed record TransactionHistoryItem(
    [property: JsonPropertyName("transaction_id")] Guid TransactionId,
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    [property: JsonPropertyName("direction")] string Direction,
    [property: JsonPropertyName("amount")] double Amount,
    [property: JsonPropertyName("category")] string Category);

public sealed record TransactionHistoryResponse(
    [property: JsonPropertyName("items")] List<TransactionHistoryItem> Items,
    [property: JsonPropertyName("next_cursor")] string? NextCursor,
    [property: JsonPropertyName("has_more")] bool HasMore);

public sealed record LoginRequest(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password);

public sealed record LoginResponse(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("display_name")] string? DisplayName,
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);

public sealed record RegisterRequest(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("display_name")] string? DisplayName);

public sealed record RegisterResponse(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("display_name")] string? DisplayName,
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);
