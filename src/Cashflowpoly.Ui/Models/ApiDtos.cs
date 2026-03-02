// Fungsi file: Mendefinisikan Data Transfer Object (DTO) untuk serialisasi dan deserialisasi respons serta permintaan REST API backend.
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cashflowpoly.Ui.Models;

/// <summary>
/// DTO yang merepresentasikan satu item ruleset dalam daftar, memuat ID, nama, versi terbaru, dan status.
/// </summary>
public sealed record RulesetListItemDto(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("latest_version")] int LatestVersion,
    [property: JsonPropertyName("status")] string Status);

/// <summary>
/// DTO respons yang membungkus daftar item ruleset dari API.
/// </summary>
public sealed record RulesetListResponseDto(
    [property: JsonPropertyName("items")] List<RulesetListItemDto> Items);

/// <summary>
/// DTO yang merepresentasikan satu item sesi permainan dalam daftar, memuat ID, nama, mode, status, dan cap waktu.
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
/// DTO respons yang membungkus daftar item sesi permainan dari API.
/// </summary>
public sealed record SessionListResponseDto(
    [property: JsonPropertyName("items")] List<SessionListItemDto> Items);

/// <summary>
/// DTO yang merepresentasikan satu versi dari sebuah ruleset, memuat ID versi, nomor versi, status, dan waktu pembuatan.
/// </summary>
public sealed record RulesetVersionItemDto(
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt);

/// <summary>
/// DTO respons detail ruleset yang memuat ID, nama, deskripsi, daftar versi, dan konfigurasi JSON.
/// </summary>
public sealed record RulesetDetailResponseDto(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("versions")] List<RulesetVersionItemDto> Versions,
    [property: JsonPropertyName("config_json")] JsonElement? ConfigJson);

/// <summary>
/// DTO respons komponen ruleset yang memuat ID ruleset, ID versi, nomor versi, mode, dan katalog komponen.
/// </summary>
public sealed record RulesetComponentsResponseDto(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("mode")] string? Mode,
    [property: JsonPropertyName("component_catalog")] JsonElement? ComponentCatalog);

/// <summary>
/// DTO satu item komponen default ruleset bawaan, memuat metadata ruleset beserta katalog komponen.
/// </summary>
public sealed record DefaultRulesetComponentItemDto(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("mode")] string? Mode,
    [property: JsonPropertyName("component_catalog")] JsonElement? ComponentCatalog);

/// <summary>
/// DTO respons yang membungkus daftar komponen default bawaan ruleset dari API.
/// </summary>
public sealed record DefaultRulesetComponentsResponseDto(
    [property: JsonPropertyName("items")] List<DefaultRulesetComponentItemDto> Items);

/// <summary>
/// DTO ringkasan analitik sesi yang memuat jumlah event, total arus kas masuk/keluar, arus kas bersih, dan jumlah pelanggaran aturan.
/// </summary>
public sealed record AnalyticsSessionSummaryDto(
    [property: JsonPropertyName("event_count")] int EventCount,
    [property: JsonPropertyName("cash_in_total")] double CashInTotal,
    [property: JsonPropertyName("cash_out_total")] double CashOutTotal,
    [property: JsonPropertyName("cashflow_net_total")] double CashflowNetTotal,
    [property: JsonPropertyName("rules_violations_count")] int RulesViolationsCount);

/// <summary>
/// DTO analitik per pemain dalam satu sesi, memuat metrik keuangan, inventaris, skor kebahagiaan, donasi, penalti, dan status pinjaman.
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
/// DTO respons analitik sesi lengkap yang memuat ringkasan agregat, data per pemain, dan informasi ruleset terkait.
/// </summary>
public sealed record AnalyticsSessionResponseDto(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("summary")] AnalyticsSessionSummaryDto Summary,
    [property: JsonPropertyName("by_player")] List<AnalyticsByPlayerItemDto> ByPlayer,
    [property: JsonPropertyName("ruleset_id")] Guid? RulesetId,
    [property: JsonPropertyName("ruleset_name")] string? RulesetName);

/// <summary>
/// DTO respons metrik gameplay seorang pemain dalam sesi, memuat data mentah (raw) dan turunan (derived) dalam format JSON.
/// </summary>
public sealed record GameplayMetricsResponseDto(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("computed_at")] DateTimeOffset? ComputedAt,
    [property: JsonPropertyName("raw")] JsonElement? Raw,
    [property: JsonPropertyName("derived")] JsonElement? Derived);

/// <summary>
/// DTO satu item riwayat transaksi keuangan pemain, memuat cap waktu, arah transaksi, jumlah, dan kategori.
/// </summary>
public sealed record TransactionHistoryItemDto(
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    [property: JsonPropertyName("direction")] string Direction,
    [property: JsonPropertyName("amount")] double Amount,
    [property: JsonPropertyName("category")] string Category);

/// <summary>
/// DTO respons yang membungkus daftar riwayat transaksi keuangan pemain dari API.
/// </summary>
public sealed record TransactionHistoryResponseDto(
    [property: JsonPropertyName("items")] List<TransactionHistoryItemDto> Items);

/// <summary>
/// DTO analitik pemain pada tingkat ruleset, memuat skor performa pembelajaran dan misi individual.
/// </summary>
public sealed record RulesetAnalyticsPlayerItemDto(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("learning_performance_individual_score")] double? LearningPerformanceIndividualScore,
    [property: JsonPropertyName("mission_performance_individual_score")] double? MissionPerformanceIndividualScore);

/// <summary>
/// DTO analitik sesi pada tingkat ruleset, memuat nama sesi, status, jumlah event, skor agregat, dan data pemain.
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
/// DTO respons ringkasan analitik tingkat ruleset yang memuat jumlah sesi, skor performa agregat, dan daftar sesi terkait.
/// </summary>
public sealed record RulesetAnalyticsSummaryResponseDto(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("ruleset_name")] string RulesetName,
    [property: JsonPropertyName("session_count")] int SessionCount,
    [property: JsonPropertyName("learning_performance_aggregate_score")] double? LearningPerformanceAggregateScore,
    [property: JsonPropertyName("mission_performance_aggregate_score")] double? MissionPerformanceAggregateScore,
    [property: JsonPropertyName("sessions")] List<RulesetAnalyticsSessionItemDto> Sessions);

/// <summary>
/// DTO data pemain yang memuat ID pemain dan nama tampilan.
/// </summary>
public sealed record PlayerResponseDto(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("display_name")] string DisplayName);

/// <summary>
/// DTO respons yang membungkus daftar pemain dari API.
/// </summary>
public sealed record PlayerListResponseDto(
    [property: JsonPropertyName("items")] List<PlayerResponseDto> Items);

/// <summary>
/// DTO permintaan event gameplay yang memuat metadata sesi, pemain, aksi, hari, giliran, dan payload JSON.
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
/// DTO respons yang membungkus daftar event gameplay berdasarkan sesi dari API.
/// </summary>
public sealed record EventsBySessionResponseDto(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("events")] List<EventRequestDto> Events);

/// <summary>
/// DTO detail kesalahan API yang memuat nama field dan deskripsi masalah validasi.
/// </summary>
public sealed record ApiErrorDetailDto(
    [property: JsonPropertyName("field")] string Field,
    [property: JsonPropertyName("issue")] string Issue);

/// <summary>
/// DTO respons error standar API yang memuat kode error, pesan, daftar detail kesalahan, dan trace ID.
/// </summary>
public sealed record ApiErrorResponseDto(
    [property: JsonPropertyName("error_code")] string ErrorCode,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("details")] List<ApiErrorDetailDto> Details,
    [property: JsonPropertyName("trace_id")] string TraceId);

/// <summary>
/// DTO permintaan login yang memuat nama pengguna dan kata sandi untuk autentikasi ke API.
/// </summary>
public sealed record LoginRequestDto(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password);

/// <summary>
/// DTO respons login yang memuat ID pengguna, nama pengguna, peran, token akses, dan waktu kedaluwarsa token.
/// </summary>
public sealed record LoginResponseDto(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);

/// <summary>
/// DTO permintaan registrasi pengguna baru yang memuat nama pengguna, kata sandi, peran, dan nama tampilan.
/// </summary>
public sealed record RegisterRequestDto(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("display_name")] string? DisplayName);

/// <summary>
/// DTO respons registrasi yang memuat ID pengguna, nama pengguna, peran, token akses, dan waktu kedaluwarsa token.
/// </summary>
public sealed record RegisterResponseDto(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);
