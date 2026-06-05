using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cashflowpoly.Api.Contracts;

public sealed class CreateSessionRequest
{
    public CreateSessionRequest()
    {
    }

    public CreateSessionRequest(string sessionName, string mode, Guid rulesetId)
    {
        SessionName = sessionName;
        Mode = mode;
        RulesetId = rulesetId;
    }

    [JsonPropertyName("session_name")]
    public string? SessionName { get; init; }

    [JsonPropertyName("mode")]
    public string? Mode { get; init; }

    [JsonPropertyName("ruleset_id")]
    public Guid? RulesetId { get; init; }

    [JsonPropertyName("player_names")]
    public List<string>? PlayerNames { get; init; }
}

public sealed class CreateSessionResponse
{
    public CreateSessionResponse()
    {
    }

    public CreateSessionResponse(Guid sessionId)
    {
        SessionId = sessionId;
    }

    public CreateSessionResponse(Guid sessionId, Guid rulesetId, Guid rulesetVersionId, SessionStateResponse state)
    {
        SessionId = sessionId;
        RulesetId = rulesetId;
        RulesetVersionId = rulesetVersionId;
        State = state;
    }

    [JsonPropertyName("session_id")]
    public Guid SessionId { get; init; }

    [JsonPropertyName("ruleset_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? RulesetId { get; init; }

    [JsonPropertyName("ruleset_version_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? RulesetVersionId { get; init; }

    [JsonPropertyName("state")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SessionStateResponse? State { get; init; }
}

public sealed record SessionStatusResponse([property: JsonPropertyName("status")] string Status);

public sealed record CreatePlayerRequest(
    [property: JsonPropertyName("display_name")] string DisplayName,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password);

public sealed record PlayerResponse(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("display_name")] string DisplayName);

public sealed record PlayerListResponse([property: JsonPropertyName("items")] List<PlayerResponse> Items);

public sealed record AddSessionPlayerRequest(
    [property: JsonPropertyName("user_id")] Guid? UserId,
    [property: JsonPropertyName("username")] string? Username,
    [property: JsonPropertyName("join_order")] int? JoinOrder,
    [property: JsonPropertyName("role")] string? Role);

public sealed record AddSessionPlayerResponse(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("join_order")] int JoinOrder);

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
    [property: JsonPropertyName("latest_version")] int LatestVersion,
    [property: JsonPropertyName("status")] string Status);

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
    [property: JsonPropertyName("versions")] List<RulesetVersionItem> Versions,
    [property: JsonPropertyName("config_json")] JsonElement? ConfigJson,
    [property: JsonPropertyName("ruleset_version_id")] Guid? RulesetVersionId = null,
    [property: JsonPropertyName("version")] int? Version = null,
    [property: JsonPropertyName("mode")] string? Mode = null,
    [property: JsonPropertyName("sections")] RulesetSectionCatalogResponse? Sections = null);

public sealed record RulesetComponentsResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("mode")] string? Mode,
    [property: JsonPropertyName("component_catalog")] JsonElement? ComponentCatalog,
    [property: JsonPropertyName("sections")] RulesetSectionCatalogResponse? Sections = null);

public sealed record DefaultRulesetComponentItem(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("mode")] string? Mode,
    [property: JsonPropertyName("component_catalog")] JsonElement? ComponentCatalog,
    [property: JsonPropertyName("sections")] RulesetSectionCatalogResponse? Sections = null);

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
    [property: JsonPropertyName("user_id")] Guid UserId,
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

public sealed record AnalyticsSessionResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("summary")] AnalyticsSessionSummary Summary,
    [property: JsonPropertyName("by_player")] List<AnalyticsByPlayerItem> ByPlayer,
    [property: JsonPropertyName("ruleset_id")] Guid? RulesetId,
    [property: JsonPropertyName("ruleset_name")] string? RulesetName);

public sealed record GameplayMetricsResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("user_id")] Guid UserId,
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
    [property: JsonPropertyName("user_id")] Guid UserId,
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

public sealed record SecurityAuditLogResponse(
    [property: JsonPropertyName("items")] List<SecurityAuditLogItem> Items);

public sealed record RulesetSectionCatalogResponse(
    [property: JsonPropertyName("gameConfig")] JsonElement GameConfig,
    [property: JsonPropertyName("bahan")] JsonElement Bahan,
    [property: JsonPropertyName("resep")] JsonElement Resep,
    [property: JsonPropertyName("kebutuhan")] JsonElement Kebutuhan,
    [property: JsonPropertyName("targetKebutuhan")] JsonElement TargetKebutuhan,
    [property: JsonPropertyName("tujuanFinansial")] JsonElement TujuanFinansial,
    [property: JsonPropertyName("narasi")] JsonElement Narasi,
    [property: JsonPropertyName("quest")] JsonElement Quest);

public sealed record RulesetSectionsResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("mode")] string Mode,
    [property: JsonPropertyName("gameConfig")] JsonElement GameConfig,
    [property: JsonPropertyName("bahan")] JsonElement Bahan,
    [property: JsonPropertyName("resep")] JsonElement Resep,
    [property: JsonPropertyName("kebutuhan")] JsonElement Kebutuhan,
    [property: JsonPropertyName("targetKebutuhan")] JsonElement TargetKebutuhan,
    [property: JsonPropertyName("tujuanFinansial")] JsonElement TujuanFinansial,
    [property: JsonPropertyName("narasi")] JsonElement Narasi,
    [property: JsonPropertyName("quest")] JsonElement Quest);

public sealed class SaveSessionStateRequest
{
    [JsonPropertyName("state_version")]
    public long StateVersion { get; init; }

    [JsonPropertyName("day")]
    public int Day { get; init; }

    [JsonPropertyName("turn")]
    public int Turn { get; init; }

    [JsonPropertyName("moves_left")]
    public int MovesLeft { get; init; }

    [JsonPropertyName("finish_day")]
    public int FinishDay { get; init; }

    [JsonPropertyName("is_game_over")]
    public bool IsGameOver { get; init; }

    [JsonPropertyName("ui_state")]
    public JsonElement? UiState { get; init; }

    [JsonPropertyName("players")]
    public List<SessionPlayerStateDto>? Players { get; init; }

    [JsonPropertyName("donationEvents")]
    public List<DonationEventDto>? DonationEvents { get; init; }

    [JsonPropertyName("last_action")]
    public JsonElement? LastAction { get; init; }
}

public sealed class SessionStateResponse
{
    [JsonPropertyName("session_id")]
    public Guid SessionId { get; init; }

    [JsonPropertyName("state_version")]
    public long StateVersion { get; init; }

    [JsonPropertyName("day")]
    public int Day { get; init; }

    [JsonPropertyName("turn")]
    public int Turn { get; init; }

    [JsonPropertyName("moves_left")]
    public int MovesLeft { get; init; }

    [JsonPropertyName("finish_day")]
    public int FinishDay { get; init; }

    [JsonPropertyName("is_game_over")]
    public bool IsGameOver { get; init; }

    [JsonPropertyName("ui_state")]
    public JsonElement UiState { get; init; }

    [JsonPropertyName("players")]
    public List<SessionPlayerStateDto> Players { get; init; } = [];

    [JsonPropertyName("donationEvents")]
    public List<DonationEventDto> DonationEvents { get; init; } = [];
}

public sealed class SessionPlayerStateDto
{
    [JsonPropertyName("session_player_id")]
    public Guid SessionPlayerId { get; init; }

    [JsonPropertyName("user_id")]
    public Guid? UserId { get; init; }

    [JsonPropertyName("player_index")]
    public int PlayerIndex { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("coins")]
    public int Coins { get; init; }

    [JsonPropertyName("happiness")]
    public int Happiness { get; init; }

    [JsonPropertyName("saving")]
    public int Saving { get; init; }

    [JsonPropertyName("bahan")]
    public List<BahanItemDto> Bahan { get; init; } = [];

    [JsonPropertyName("kebutuhan")]
    public List<KebutuhanItemDto> Kebutuhan { get; init; } = [];

    [JsonPropertyName("tujuanFinansial")]
    public List<TujuanFinansialItemDto> TujuanFinansial { get; init; } = [];

    [JsonPropertyName("targetKebutuhan")]
    public List<TargetKebutuhanProgressDto> TargetKebutuhan { get; init; } = [];

    [JsonPropertyName("questProgress")]
    public List<QuestProgressDto> QuestProgress { get; init; } = [];

    [JsonPropertyName("actionCounters")]
    public List<ActionCounterDto> ActionCounters { get; init; } = [];

    [JsonPropertyName("totalDonasi")]
    public int TotalDonasi { get; init; }
}

public sealed record BahanItemDto(
    [property: JsonPropertyName("nama")] string Nama,
    [property: JsonPropertyName("jumlah")] int Jumlah);

public sealed record KebutuhanItemDto(
    [property: JsonPropertyName("nama")] string Nama,
    [property: JsonPropertyName("tipe")] string Tipe);

public sealed record TujuanFinansialItemDto(
    [property: JsonPropertyName("nama")] string Nama,
    [property: JsonPropertyName("purchased_at_day")] int PurchasedAtDay);

public sealed record TargetKebutuhanProgressDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("is_completed")] bool IsCompleted,
    [property: JsonPropertyName("is_failed")] bool IsFailed,
    [property: JsonPropertyName("reward_applied")] bool RewardApplied);

public sealed record QuestProgressDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("progress")] int Progress,
    [property: JsonPropertyName("target")] int Target,
    [property: JsonPropertyName("is_completed")] bool IsCompleted,
    [property: JsonPropertyName("is_reward_claimed")] bool IsRewardClaimed);

public sealed record ActionCounterDto(
    [property: JsonPropertyName("aksi")] string Aksi,
    [property: JsonPropertyName("count")] int Count);

public sealed class DonationEventDto
{
    [JsonPropertyName("event_ke")]
    public int EventKe { get; init; }

    [JsonPropertyName("day")]
    public int Day { get; init; }

    [JsonPropertyName("rankings")]
    public List<DonationRankingDto> Rankings { get; init; } = [];
}

public sealed class DonationRankingDto
{
    [JsonPropertyName("rank")]
    public int Rank { get; init; }

    [JsonPropertyName("session_player_id")]
    public Guid SessionPlayerId { get; init; }

    [JsonPropertyName("player_index")]
    public int? PlayerIndex { get; init; }

    [JsonPropertyName("total_donasi")]
    public int TotalDonasi { get; init; }
}
