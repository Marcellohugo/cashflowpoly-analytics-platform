// Fungsi file: Kumpulan record/class DTO untuk mapping baris database PostgreSQL ke objek C# via Dapper.
namespace Cashflowpoly.Api.Data;

/// <summary>
/// Representasi baris tabel rulesets — konfigurasi aturan permainan.
/// </summary>
public sealed class RulesetDb
{
    public Guid RulesetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? InstructorUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

/// <summary>
/// Representasi baris tabel ruleset_versions — versi snapshot konfigurasi aturan.
/// </summary>
public sealed class RulesetVersionDb
{
    public Guid RulesetVersionId { get; set; }
    public Guid RulesetId { get; set; }
    public int Version { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ConfigJson { get; set; } = string.Empty;
    public string ConfigHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

/// <summary>
/// Proyeksi gabungan ruleset + versi aktif untuk komponen default seed.
/// </summary>
public sealed class DefaultRulesetComponentDb
{
    public Guid RulesetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid RulesetVersionId { get; set; }
    public int Version { get; set; }
    public string ConfigJson { get; set; } = string.Empty;
}

/// <summary>
/// Representasi baris tabel sessions — sesi permainan Cashflowpoly.
/// </summary>
public sealed class SessionDb
{
    public Guid SessionId { get; set; }
    public string SessionName { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public Guid? InstructorUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// Representasi baris tabel players — profil pemain.
/// </summary>
public sealed class PlayerDb
{
    public Guid PlayerId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public Guid? InstructorUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// Representasi baris tabel events — event gameplay dari klien.
/// </summary>
public sealed class EventDb
{
    public Guid EventPk { get; set; }
    public Guid EventId { get; set; }
    public Guid SessionId { get; set; }
    public Guid? PlayerId { get; set; }
    public string ActorType { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
    public int DayIndex { get; set; }
    public string Weekday { get; set; } = string.Empty;
    public int TurnNumber { get; set; }
    public long SequenceNumber { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public Guid RulesetVersionId { get; set; }
    public string Payload { get; set; } = string.Empty;
    public DateTimeOffset ReceivedAt { get; set; }
    public string? ClientRequestId { get; set; }
}

/// <summary>
/// Representasi baris tabel event_cashflow_projections — proyeksi arus kas per event.
/// </summary>
public sealed class CashflowProjectionDb
{
    public Guid ProjectionId { get; set; }
    public Guid SessionId { get; set; }
    public Guid PlayerId { get; set; }
    public Guid EventPk { get; set; }
    public Guid EventId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string Direction { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Counterparty { get; set; }
    public string? Reference { get; set; }
    public string? Note { get; set; }
}

/// <summary>
/// Representasi baris tabel metric_snapshots — snapshot metrik numerik.
/// </summary>
public sealed class MetricSnapshotDb
{
    public Guid MetricSnapshotId { get; set; }
    public Guid SessionId { get; set; }
    public Guid? PlayerId { get; set; }
    public DateTimeOffset ComputedAt { get; set; }
    public string MetricName { get; set; } = string.Empty;
    public double? MetricValueNumeric { get; set; }
    public string? MetricValueJson { get; set; }
    public Guid RulesetVersionId { get; set; }
}

/// <summary>
/// Proyeksi ringan metric snapshot yang hanya berisi kolom JSON (untuk agregasi lanjut).
/// </summary>
public sealed class MetricSnapshotJsonDb
{
    public string MetricName { get; set; } = string.Empty;
    public string? MetricValueJson { get; set; }
    public DateTimeOffset ComputedAt { get; set; }
}

/// <summary>
/// Representasi baris tabel security_audit_logs — catatan audit keamanan.
/// </summary>
public sealed class SecurityAuditLogDb
{
    public Guid SecurityAuditLogId { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public string TraceId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Outcome { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string? Username { get; set; }
    public string? Role { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string? DetailJson { get; set; }
}
