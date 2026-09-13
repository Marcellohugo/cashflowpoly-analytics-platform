// Fungsi file: Kontrak pembacaan roster dan gameplay lintas sesi dengan cakupan akses pengguna.
using System.Text.Json.Serialization;

namespace Cashflowpoly.Api.Contracts;

public sealed record SessionRostersResponse(
    [property: JsonPropertyName("items")] List<SessionRosterResponse> Items);

public sealed record SessionRosterResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("players")] List<SessionRosterPlayer> Players,
    [property: JsonPropertyName("results_available")] bool ResultsAvailable);

public sealed record SessionRosterPlayer(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("display_name")] string DisplayName,
    [property: JsonPropertyName("player_order")] int PlayerOrder,
    [property: JsonPropertyName("final_rank")] int? FinalRank,
    [property: JsonPropertyName("happiness_points_total")] double? HappinessPointsTotal);

public sealed record PlayerGameplayHistoryResponse(
    [property: JsonPropertyName("items")] List<SessionGameplayItem> Items);

public sealed record SessionGameplayItem(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("gameplay")] GameplayMetricsResponse? Gameplay);
