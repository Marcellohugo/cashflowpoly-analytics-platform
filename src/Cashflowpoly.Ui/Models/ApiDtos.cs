using System.Text.Json.Serialization;

namespace Cashflowpoly.Ui.Models;

public sealed record RulesetListItemDto(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("latest_version")] int LatestVersion);

public sealed record RulesetListResponseDto(
    [property: JsonPropertyName("items")] List<RulesetListItemDto> Items);

public sealed record AnalyticsSessionSummaryDto(
    [property: JsonPropertyName("event_count")] int EventCount,
    [property: JsonPropertyName("cash_in_total")] double CashInTotal,
    [property: JsonPropertyName("cash_out_total")] double CashOutTotal);

public sealed record AnalyticsByPlayerItemDto(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("cash_in_total")] double CashInTotal,
    [property: JsonPropertyName("cash_out_total")] double CashOutTotal,
    [property: JsonPropertyName("donation_total")] double DonationTotal,
    [property: JsonPropertyName("gold_qty")] int GoldQty);

public sealed record AnalyticsSessionResponseDto(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("summary")] AnalyticsSessionSummaryDto Summary,
    [property: JsonPropertyName("by_player")] List<AnalyticsByPlayerItemDto> ByPlayer);

public sealed record ApiErrorDetailDto(
    [property: JsonPropertyName("field")] string Field,
    [property: JsonPropertyName("issue")] string Issue);

public sealed record ApiErrorResponseDto(
    [property: JsonPropertyName("error_code")] string ErrorCode,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("details")] List<ApiErrorDetailDto> Details,
    [property: JsonPropertyName("trace_id")] string TraceId);
