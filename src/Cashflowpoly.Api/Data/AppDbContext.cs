// Fungsi file: EF Core DbContext digunakan hanya untuk manajemen skema via migrasi — semua query tetap menggunakan Dapper.
using Microsoft.EntityFrameworkCore;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// EF Core context untuk manajemen migrasi skema database. Tidak digunakan untuk query runtime — Dapper menangani semua akses data.
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUserEf> AppUsers { get; set; } = null!;
    public DbSet<PlayerDb> Players { get; set; } = null!;
    public DbSet<UserPlayerLinkEf> UserPlayerLinks { get; set; } = null!;
    public DbSet<SessionDb> Sessions { get; set; } = null!;
    public DbSet<RulesetDb> Rulesets { get; set; } = null!;
    public DbSet<RulesetVersionDb> RulesetVersions { get; set; } = null!;
    public DbSet<SessionPlayerEf> SessionPlayers { get; set; } = null!;
    public DbSet<SessionRulesetActivationEf> SessionRulesetActivations { get; set; } = null!;
    public DbSet<EventDb> Events { get; set; } = null!;
    public DbSet<CashflowProjectionDb> EventCashflowProjections { get; set; } = null!;
    public DbSet<MetricSnapshotDb> MetricSnapshots { get; set; } = null!;
    public DbSet<ValidationLogEf> ValidationLogs { get; set; } = null!;
    public DbSet<SecurityAuditLogDb> SecurityAuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // app_users
        modelBuilder.Entity<AppUserEf>(entity =>
        {
            entity.ToTable("app_users");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        // players
        modelBuilder.Entity<PlayerDb>(entity =>
        {
            entity.ToTable("players");
            entity.HasKey(e => e.PlayerId);
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.DisplayName).HasColumnName("display_name");
            entity.Property(e => e.InstructorUserId).HasColumnName("instructor_user_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        // user_player_links
        modelBuilder.Entity<UserPlayerLinkEf>(entity =>
        {
            entity.ToTable("user_player_links");
            entity.HasKey(e => e.LinkId);
            entity.Property(e => e.LinkId).HasColumnName("link_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        // sessions
        modelBuilder.Entity<SessionDb>(entity =>
        {
            entity.ToTable("sessions");
            entity.HasKey(e => e.SessionId);
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.SessionName).HasColumnName("session_name");
            entity.Property(e => e.Mode).HasColumnName("mode");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.EndedAt).HasColumnName("ended_at");
            entity.Property(e => e.InstructorUserId).HasColumnName("instructor_user_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        // rulesets
        modelBuilder.Entity<RulesetDb>(entity =>
        {
            entity.ToTable("rulesets");
            entity.HasKey(e => e.RulesetId);
            entity.Property(e => e.RulesetId).HasColumnName("ruleset_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.InstructorUserId).HasColumnName("instructor_user_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
        });

        // ruleset_versions
        modelBuilder.Entity<RulesetVersionDb>(entity =>
        {
            entity.ToTable("ruleset_versions");
            entity.HasKey(e => e.RulesetVersionId);
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            entity.Property(e => e.RulesetId).HasColumnName("ruleset_id");
            entity.Property(e => e.Version).HasColumnName("version");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.ConfigJson).HasColumnName("config_json").HasColumnType("jsonb");
            entity.Property(e => e.ConfigHash).HasColumnName("config_hash");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
        });

        // session_players
        modelBuilder.Entity<SessionPlayerEf>(entity =>
        {
            entity.ToTable("session_players");
            entity.HasKey(e => e.SessionPlayerId);
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.JoinOrder).HasColumnName("join_order");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        // session_ruleset_activations
        modelBuilder.Entity<SessionRulesetActivationEf>(entity =>
        {
            entity.ToTable("session_ruleset_activations");
            entity.HasKey(e => e.ActivationId);
            entity.Property(e => e.ActivationId).HasColumnName("activation_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            entity.Property(e => e.ActivatedAt).HasColumnName("activated_at");
            entity.Property(e => e.ActivatedBy).HasColumnName("activated_by");
        });

        // events
        modelBuilder.Entity<EventDb>(entity =>
        {
            entity.ToTable("events");
            entity.HasKey(e => e.EventPk);
            entity.Property(e => e.EventPk).HasColumnName("event_pk");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.ActorType).HasColumnName("actor_type");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.Property(e => e.DayIndex).HasColumnName("day_index");
            entity.Property(e => e.Weekday).HasColumnName("weekday");
            entity.Property(e => e.TurnNumber).HasColumnName("turn_number");
            entity.Property(e => e.SequenceNumber).HasColumnName("sequence_number");
            entity.Property(e => e.ActionType).HasColumnName("action_type");
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            entity.Property(e => e.Payload).HasColumnName("payload").HasColumnType("jsonb");
            entity.Property(e => e.ReceivedAt).HasColumnName("received_at");
            entity.Property(e => e.ClientRequestId).HasColumnName("client_request_id");
        });

        // event_cashflow_projections
        modelBuilder.Entity<CashflowProjectionDb>(entity =>
        {
            entity.ToTable("event_cashflow_projections");
            entity.HasKey(e => e.ProjectionId);
            entity.Property(e => e.ProjectionId).HasColumnName("projection_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.EventPk).HasColumnName("event_pk");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.Property(e => e.Direction).HasColumnName("direction");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.Counterparty).HasColumnName("counterparty");
            entity.Property(e => e.Reference).HasColumnName("reference");
            entity.Property(e => e.Note).HasColumnName("note");
        });

        // metric_snapshots
        modelBuilder.Entity<MetricSnapshotDb>(entity =>
        {
            entity.ToTable("metric_snapshots");
            entity.HasKey(e => e.MetricSnapshotId);
            entity.Property(e => e.MetricSnapshotId).HasColumnName("metric_snapshot_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.ComputedAt).HasColumnName("computed_at");
            entity.Property(e => e.MetricName).HasColumnName("metric_name");
            entity.Property(e => e.MetricValueNumeric).HasColumnName("metric_value_numeric");
            entity.Property(e => e.MetricValueJson).HasColumnName("metric_value_json").HasColumnType("jsonb");
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
        });

        // validation_logs
        modelBuilder.Entity<ValidationLogEf>(entity =>
        {
            entity.ToTable("validation_logs");
            entity.HasKey(e => e.ValidationLogId);
            entity.Property(e => e.ValidationLogId).HasColumnName("validation_log_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.EventPk).HasColumnName("event_pk");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.IsValid).HasColumnName("is_valid");
            entity.Property(e => e.ErrorCode).HasColumnName("error_code");
            entity.Property(e => e.ErrorMessage).HasColumnName("error_message");
            entity.Property(e => e.DetailsJson).HasColumnName("details_json").HasColumnType("jsonb");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        // security_audit_logs
        modelBuilder.Entity<SecurityAuditLogDb>(entity =>
        {
            entity.ToTable("security_audit_logs");
            entity.HasKey(e => e.SecurityAuditLogId);
            entity.Property(e => e.SecurityAuditLogId).HasColumnName("security_audit_log_id");
            entity.Property(e => e.OccurredAt).HasColumnName("occurred_at");
            entity.Property(e => e.TraceId).HasColumnName("trace_id");
            entity.Property(e => e.EventType).HasColumnName("event_type");
            entity.Property(e => e.Outcome).HasColumnName("outcome");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.IpAddress).HasColumnName("ip_address");
            entity.Property(e => e.UserAgent).HasColumnName("user_agent");
            entity.Property(e => e.Method).HasColumnName("method");
            entity.Property(e => e.Path).HasColumnName("path");
            entity.Property(e => e.StatusCode).HasColumnName("status_code");
            entity.Property(e => e.DetailJson).HasColumnName("detail_json").HasColumnType("jsonb");
        });
    }
}

/// <summary>
/// Entitas EF Core untuk tabel app_users — digunakan hanya untuk generasi migrasi skema.
/// </summary>
public sealed class AppUserEf
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// Entitas EF Core untuk tabel user_player_links — digunakan hanya untuk generasi migrasi skema.
/// </summary>
public sealed class UserPlayerLinkEf
{
    public Guid LinkId { get; set; }
    public Guid UserId { get; set; }
    public Guid PlayerId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// Entitas EF Core untuk tabel session_players — digunakan hanya untuk generasi migrasi skema.
/// </summary>
public sealed class SessionPlayerEf
{
    public Guid SessionPlayerId { get; set; }
    public Guid SessionId { get; set; }
    public Guid PlayerId { get; set; }
    public int JoinOrder { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// Entitas EF Core untuk tabel session_ruleset_activations — digunakan hanya untuk generasi migrasi skema.
/// </summary>
public sealed class SessionRulesetActivationEf
{
    public Guid ActivationId { get; set; }
    public Guid SessionId { get; set; }
    public Guid RulesetVersionId { get; set; }
    public DateTimeOffset ActivatedAt { get; set; }
    public string? ActivatedBy { get; set; }
}

/// <summary>
/// Entitas EF Core untuk tabel validation_logs — digunakan hanya untuk generasi migrasi skema.
/// </summary>
public sealed class ValidationLogEf
{
    public Guid ValidationLogId { get; set; }
    public Guid SessionId { get; set; }
    public Guid? EventPk { get; set; }
    public Guid EventId { get; set; }
    public bool IsValid { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? DetailsJson { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
