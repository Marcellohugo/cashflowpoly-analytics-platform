// Fungsi file: Mendefinisikan seluruh Data Transfer Object (DTO) untuk request dan response API, mencakup sesi, pemain, ruleset, event, analitika, autentikasi, dan audit keamanan.
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cashflowpoly.Api.Models;

/// <summary>
/// Request untuk membuat sesi permainan baru dengan nama, mode, dan ruleset.
/// </summary>
public sealed record CreateSessionRequest(
    [property: JsonPropertyName("session_name")] string SessionName,
    [property: JsonPropertyName("mode")] string Mode,
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId);

/// <summary>
/// Respons berisi ID sesi yang baru dibuat.
/// </summary>
public sealed record CreateSessionResponse([property: JsonPropertyName("session_id")] Guid SessionId);

/// <summary>
/// Respons berisi status terkini dari sebuah sesi permainan.
/// </summary>
public sealed record SessionStatusResponse([property: JsonPropertyName("status")] string Status);

/// <summary>
/// Request untuk membuat pemain baru dengan display name, username, dan password.
/// </summary>
public sealed record CreatePlayerRequest(
    [property: JsonPropertyName("display_name")] string DisplayName,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password);

/// <summary>
/// Respons data pemain yang berisi ID dan display name.
/// </summary>
public sealed record PlayerResponse(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("display_name")] string DisplayName);

/// <summary>
/// Respons berisi daftar pemain.
/// </summary>
public sealed record PlayerListResponse([property: JsonPropertyName("items")] List<PlayerResponse> Items);

/// <summary>
/// Request untuk menambahkan pemain ke sesi, bisa melalui player_id atau username.
/// </summary>
public sealed record AddSessionPlayerRequest(
    [property: JsonPropertyName("player_id")] Guid? PlayerId,
    [property: JsonPropertyName("username")] string? Username,
    [property: JsonPropertyName("join_order")] int? JoinOrder,
    [property: JsonPropertyName("role")] string? Role);

/// <summary>
/// Respons setelah pemain ditambahkan ke sesi, berisi ID pemain dan urutan bergabung.
/// </summary>
public sealed record AddSessionPlayerResponse(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("join_order")] int JoinOrder);

/// <summary>
/// Item ringkasan sesi pada daftar sesi (ID, nama, mode, status, tanggal).
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
/// Respons berisi daftar item sesi.
/// </summary>
public sealed record SessionListResponse([property: JsonPropertyName("items")] List<SessionListItem> Items);

/// <summary>
/// Request untuk mengaktifkan versi ruleset tertentu pada sebuah sesi.
/// </summary>
public sealed record ActivateRulesetRequest(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("version")] int Version);

/// <summary>
/// Respons aktivasi ruleset berisi ID sesi dan ID versi ruleset yang diaktifkan.
/// </summary>
public sealed record ActivateRulesetResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId);

/// <summary>
/// Request untuk membuat ruleset baru dengan nama, deskripsi, dan konfigurasi JSON.
/// </summary>
public sealed record CreateRulesetRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("config")] JsonElement Config);

/// <summary>
/// Request untuk memperbarui ruleset (nama, deskripsi, dan/atau konfigurasi).
/// </summary>
public sealed record UpdateRulesetRequest(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("config")] JsonElement? Config);

/// <summary>
/// Respons pembuatan/pembaruan ruleset berisi ID ruleset dan nomor versi.
/// </summary>
public sealed record CreateRulesetResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("version")] int Version);

/// <summary>
/// Item ringkasan ruleset pada daftar ruleset (ID, nama, versi terbaru, status).
/// </summary>
public sealed record RulesetListItem(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("latest_version")] int LatestVersion,
    [property: JsonPropertyName("status")] string Status);

/// <summary>
/// Respons berisi daftar item ruleset.
/// </summary>
public sealed record RulesetListResponse([property: JsonPropertyName("items")] List<RulesetListItem> Items);

/// <summary>
/// Item versi ruleset pada daftar versi (ID versi, nomor versi, status, tanggal dibuat).
/// </summary>
public sealed record RulesetVersionItem(
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt);

/// <summary>
/// Respons detail ruleset lengkap termasuk daftar versi dan konfigurasi JSON.
/// </summary>
public sealed record RulesetDetailResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("versions")] List<RulesetVersionItem> Versions,
    [property: JsonPropertyName("config_json")] JsonElement? ConfigJson);

/// <summary>
/// Respons komponen ruleset berisi katalog komponen permainan untuk versi tertentu.
/// </summary>
public sealed record RulesetComponentsResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("mode")] string? Mode,
    [property: JsonPropertyName("component_catalog")] JsonElement? ComponentCatalog);

/// <summary>
/// Item komponen ruleset bawaan dari seed, termasuk katalog komponen dan mode permainan.
/// </summary>
public sealed record DefaultRulesetComponentItem(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("mode")] string? Mode,
    [property: JsonPropertyName("component_catalog")] JsonElement? ComponentCatalog);

/// <summary>
/// Respons berisi daftar komponen ruleset bawaan.
/// </summary>
public sealed record DefaultRulesetComponentsResponse(
    [property: JsonPropertyName("items")] List<DefaultRulesetComponentItem> Items);

/// <summary>
/// Request ingest event gameplay dari klien, berisi semua metadata dan payload event.
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
/// Respons konfirmasi bahwa event telah berhasil disimpan.
/// </summary>
public sealed record EventStoredResponse(
    [property: JsonPropertyName("stored")] bool Stored,
    [property: JsonPropertyName("event_id")] Guid EventId);

/// <summary>
/// Request untuk mengirim batch beberapa event sekaligus.
/// </summary>
public sealed record EventBatchRequest([property: JsonPropertyName("events")] List<EventRequest> Events);

/// <summary>
/// Informasi event yang gagal dalam batch beserta kode kesalahannya.
/// </summary>
public sealed record EventBatchFailed(
    [property: JsonPropertyName("event_id")] Guid EventId,
    [property: JsonPropertyName("error_code")] string ErrorCode);

/// <summary>
/// Respons batch event berisi jumlah tersimpan dan daftar event yang gagal.
/// </summary>
public sealed record EventBatchResponse(
    [property: JsonPropertyName("stored_count")] int StoredCount,
    [property: JsonPropertyName("failed")] List<EventBatchFailed> Failed);

/// <summary>
/// Respons daftar event berdasarkan sesi, mencakup semua event yang tercatat.
/// </summary>
public sealed record EventsBySessionResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("events")] List<EventRequest> Events);

/// <summary>
/// Ringkasan analitika sesi: jumlah event, total arus kas masuk/keluar, net, dan pelanggaran aturan.
/// </summary>
public sealed record AnalyticsSessionSummary(
    [property: JsonPropertyName("event_count")] int EventCount,
    [property: JsonPropertyName("cash_in_total")] double CashInTotal,
    [property: JsonPropertyName("cash_out_total")] double CashOutTotal,
    [property: JsonPropertyName("cashflow_net_total")] double CashflowNetTotal,
    [property: JsonPropertyName("rules_violations_count")] int RulesViolationsCount);

/// <summary>
/// Item analitika per pemain: arus kas, donasi, emas, pesanan, kepatuhan, dan poin kebahagiaan.
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
/// Respons analitika sesi lengkap termasuk ringkasan, data per pemain, dan info ruleset.
/// </summary>
public sealed record AnalyticsSessionResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("summary")] AnalyticsSessionSummary Summary,
    [property: JsonPropertyName("by_player")] List<AnalyticsByPlayerItem> ByPlayer,
    [property: JsonPropertyName("ruleset_id")] Guid? RulesetId,
    [property: JsonPropertyName("ruleset_name")] string? RulesetName);

/// <summary>
/// Respons metrik gameplay per pemain berisi snapshot variabel fisik dan turunan.
/// </summary>
public sealed record GameplayMetricsResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("computed_at")] DateTimeOffset? ComputedAt,
    [property: JsonPropertyName("raw")] JsonElement? Raw,
    [property: JsonPropertyName("derived")] JsonElement? Derived);

/// <summary>
/// Item riwayat transaksi: timestamp, arah (IN/OUT), jumlah, dan kategori.
/// </summary>
public sealed record TransactionHistoryItem(
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    [property: JsonPropertyName("direction")] string Direction,
    [property: JsonPropertyName("amount")] double Amount,
    [property: JsonPropertyName("category")] string Category);

/// <summary>
/// Respons berisi daftar riwayat transaksi arus kas.
/// </summary>
public sealed record TransactionHistoryResponse(
    [property: JsonPropertyName("items")] List<TransactionHistoryItem> Items);

/// <summary>
/// Item analitika per pemain pada ringkasan ruleset: skor kinerja pembelajaran dan misi.
/// </summary>
public sealed record RulesetAnalyticsPlayerItem(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("learning_performance_individual_score")] double? LearningPerformanceIndividualScore,
    [property: JsonPropertyName("mission_performance_individual_score")] double? MissionPerformanceIndividualScore);

/// <summary>
/// Item analitika sesi pada ringkasan ruleset: jumlah event, skor agregat, dan data per pemain.
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
/// Respons ringkasan analitika lintas sesi untuk sebuah ruleset.
/// </summary>
public sealed record RulesetAnalyticsSummaryResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("ruleset_name")] string RulesetName,
    [property: JsonPropertyName("session_count")] int SessionCount,
    [property: JsonPropertyName("learning_performance_aggregate_score")] double? LearningPerformanceAggregateScore,
    [property: JsonPropertyName("mission_performance_aggregate_score")] double? MissionPerformanceAggregateScore,
    [property: JsonPropertyName("sessions")] List<RulesetAnalyticsSessionItem> Sessions);

/// <summary>
/// Request login berisi username dan password.
/// </summary>
public sealed record LoginRequest(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password);

/// <summary>
/// Respons login berisi data user, access token JWT, dan waktu kedaluwarsa.
/// </summary>
public sealed record LoginResponse(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);

/// <summary>
/// Request registrasi user baru berisi username, password, role, dan display name opsional.
/// </summary>
public sealed record RegisterRequest(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("display_name")] string? DisplayName);

/// <summary>
/// Respons registrasi berisi data user yang baru dibuat beserta access token JWT.
/// </summary>
public sealed record RegisterResponse(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);

/// <summary>
/// Item log audit keamanan berisi informasi lengkap tentang event keamanan yang tercatat.
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
/// Respons berisi daftar log audit keamanan.
/// </summary>
public sealed record SecurityAuditLogResponse(
    [property: JsonPropertyName("items")] List<SecurityAuditLogItem> Items);
