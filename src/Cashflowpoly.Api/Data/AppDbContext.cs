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
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<AppUserEf>(entity =>
        {
            entity.ToTable("app_users", table =>
            {
                table.HasCheckConstraint("app_users_role_check", "role in ('INSTRUCTOR','PLAYER')");
            });
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(80);
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(20);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasIndex(e => e.Username)
                .IsUnique()
                .HasDatabaseName("app_users_username_key");
            entity.HasIndex(e => new { e.Role, e.IsActive })
                .HasDatabaseName("ix_app_users_role_active");
        });

        modelBuilder.Entity<PlayerDb>(entity =>
        {
            entity.ToTable("players");
            entity.HasKey(e => e.PlayerId);
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.DisplayName).HasColumnName("display_name").HasMaxLength(80);
            entity.Property(e => e.InstructorUserId).HasColumnName("instructor_user_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne<AppUserEf>()
                .WithMany()
                .HasForeignKey(e => e.InstructorUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("players_instructor_user_id_fkey");

            entity.HasIndex(e => new { e.InstructorUserId, e.CreatedAt })
                .HasDatabaseName("ix_players_instructor_user");
        });

        modelBuilder.Entity<UserPlayerLinkEf>(entity =>
        {
            entity.ToTable("user_player_links");
            entity.HasKey(e => e.LinkId);
            entity.Property(e => e.LinkId).HasColumnName("link_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne<AppUserEf>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("user_player_links_user_id_fkey");
            entity.HasOne<PlayerDb>()
                .WithMany()
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("user_player_links_player_id_fkey");

            entity.HasIndex(e => e.UserId)
                .IsUnique()
                .HasDatabaseName("user_player_links_user_id_key");
            entity.HasIndex(e => e.PlayerId)
                .IsUnique()
                .HasDatabaseName("user_player_links_player_id_key");
            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("ix_user_player_links_user");
            entity.HasIndex(e => e.PlayerId)
                .HasDatabaseName("ix_user_player_links_player");
        });

        modelBuilder.Entity<SessionDb>(entity =>
        {
            entity.ToTable("sessions", table =>
            {
                table.HasCheckConstraint("sessions_mode_check", "mode in ('PEMULA','MAHIR')");
                table.HasCheckConstraint("sessions_status_check", "status in ('CREATED','STARTED','ENDED')");
            });
            entity.HasKey(e => e.SessionId);
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.SessionName).HasColumnName("session_name").HasMaxLength(120);
            entity.Property(e => e.Mode).HasColumnName("mode").HasMaxLength(10);
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(10);
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.EndedAt).HasColumnName("ended_at");
            entity.Property(e => e.InstructorUserId).HasColumnName("instructor_user_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne<AppUserEf>()
                .WithMany()
                .HasForeignKey(e => e.InstructorUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("sessions_instructor_user_id_fkey");

            entity.HasIndex(e => e.Status)
                .HasDatabaseName("ix_sessions_status");
            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("ix_sessions_created_at");
            entity.HasIndex(e => new { e.InstructorUserId, e.CreatedAt })
                .HasDatabaseName("ix_sessions_instructor_user");
        });

        modelBuilder.Entity<RulesetDb>(entity =>
        {
            entity.ToTable("rulesets");
            entity.HasKey(e => e.RulesetId);
            entity.Property(e => e.RulesetId).HasColumnName("ruleset_id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(120);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.InstructorUserId).HasColumnName("instructor_user_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(80);

            entity.HasOne<AppUserEf>()
                .WithMany()
                .HasForeignKey(e => e.InstructorUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("rulesets_instructor_user_id_fkey");

            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("ix_rulesets_created_at");
            entity.HasIndex(e => new { e.InstructorUserId, e.CreatedAt })
                .HasDatabaseName("ix_rulesets_instructor_user");
        });

        modelBuilder.Entity<RulesetVersionDb>(entity =>
        {
            entity.ToTable("ruleset_versions", table =>
            {
                table.HasCheckConstraint("ruleset_versions_version_check", "version >= 1");
                table.HasCheckConstraint("ruleset_versions_status_check", "status in ('DRAFT','ACTIVE','RETIRED')");
            });
            entity.HasKey(e => e.RulesetVersionId);
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            entity.Property(e => e.RulesetId).HasColumnName("ruleset_id");
            entity.Property(e => e.Version).HasColumnName("version");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(10);
            entity.Property(e => e.ConfigJson).HasColumnName("config_json").HasColumnType("jsonb");
            entity.Property(e => e.ConfigHash).HasColumnName("config_hash").HasMaxLength(128);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(80);

            entity.HasOne<RulesetDb>()
                .WithMany()
                .HasForeignKey(e => e.RulesetId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ruleset_versions_ruleset_id_fkey");

            entity.HasIndex(e => new { e.RulesetId, e.Version })
                .IsUnique()
                .HasDatabaseName("ruleset_versions_ruleset_id_version_key");
            entity.HasIndex(e => new { e.RulesetId, e.ConfigHash })
                .IsUnique()
                .HasDatabaseName("ruleset_versions_ruleset_id_config_hash_key");
            entity.HasIndex(e => new { e.RulesetId, e.Version })
                .HasDatabaseName("ix_ruleset_versions_ruleset");
            entity.HasIndex(e => e.Status)
                .HasDatabaseName("ix_ruleset_versions_status");
            entity.HasIndex(e => e.ConfigJson)
                .HasMethod("gin")
                .HasDatabaseName("ix_ruleset_versions_config_gin");
        });

        modelBuilder.Entity<SessionPlayerEf>(entity =>
        {
            entity.ToTable("session_players", table =>
            {
                table.HasCheckConstraint("session_players_join_order_check", "join_order >= 1");
                table.HasCheckConstraint("ck_session_players_role", "role in ('PLAYER')");
            });
            entity.HasKey(e => e.SessionPlayerId);
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.JoinOrder).HasColumnName("join_order").HasDefaultValue(1);
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(20).HasDefaultValue("PLAYER");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne<SessionDb>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_players_session_id_fkey");
            entity.HasOne<PlayerDb>()
                .WithMany()
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("session_players_player_id_fkey");

            entity.HasIndex(e => new { e.SessionId, e.PlayerId })
                .IsUnique()
                .HasDatabaseName("session_players_session_id_player_id_key");
            entity.HasIndex(e => new { e.SessionId, e.JoinOrder })
                .HasDatabaseName("ix_session_players_session");
            entity.HasIndex(e => e.PlayerId)
                .HasDatabaseName("ix_session_players_player");
        });

        modelBuilder.Entity<SessionRulesetActivationEf>(entity =>
        {
            entity.ToTable("session_ruleset_activations");
            entity.HasKey(e => e.ActivationId);
            entity.Property(e => e.ActivationId).HasColumnName("activation_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            entity.Property(e => e.ActivatedAt).HasColumnName("activated_at").HasDefaultValueSql("now()");
            entity.Property(e => e.ActivatedBy).HasColumnName("activated_by").HasMaxLength(80);

            entity.HasOne<SessionDb>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_ruleset_activations_session_id_fkey");
            entity.HasOne<RulesetVersionDb>()
                .WithMany()
                .HasForeignKey(e => e.RulesetVersionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("session_ruleset_activations_ruleset_version_id_fkey");

            entity.HasIndex(e => new { e.SessionId, e.ActivatedAt })
                .HasDatabaseName("ix_sra_session");
            entity.HasIndex(e => e.RulesetVersionId)
                .HasDatabaseName("ix_sra_ruleset_version");
        });

        modelBuilder.Entity<EventDb>(entity =>
        {
            entity.ToTable("events", table =>
            {
                table.HasCheckConstraint("events_actor_type_check", "actor_type in ('PLAYER','SYSTEM')");
                table.HasCheckConstraint("events_day_index_check", "day_index >= 0");
                table.HasCheckConstraint("events_weekday_check", "weekday in ('MON','TUE','WED','THU','FRI','SAT','SUN')");
                table.HasCheckConstraint("events_turn_number_check", "turn_number >= 1");
                table.HasCheckConstraint("events_sequence_number_check", "sequence_number >= 0");
            });
            entity.HasKey(e => e.EventPk);
            entity.Property(e => e.EventPk).HasColumnName("event_pk");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.ActorType).HasColumnName("actor_type").HasMaxLength(10);
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.Property(e => e.DayIndex).HasColumnName("day_index");
            entity.Property(e => e.Weekday).HasColumnName("weekday").HasMaxLength(3);
            entity.Property(e => e.TurnNumber).HasColumnName("turn_number");
            entity.Property(e => e.SequenceNumber).HasColumnName("sequence_number");
            entity.Property(e => e.ActionType).HasColumnName("action_type").HasMaxLength(64);
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            entity.Property(e => e.Payload).HasColumnName("payload").HasColumnType("jsonb");
            entity.Property(e => e.ReceivedAt).HasColumnName("received_at").HasDefaultValueSql("now()");
            entity.Property(e => e.ClientRequestId).HasColumnName("client_request_id").HasMaxLength(120);

            entity.HasOne<SessionDb>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("events_session_id_fkey");
            entity.HasOne<PlayerDb>()
                .WithMany()
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("events_player_id_fkey");
            entity.HasOne<RulesetVersionDb>()
                .WithMany()
                .HasForeignKey(e => e.RulesetVersionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("events_ruleset_version_id_fkey");

            entity.HasIndex(e => new { e.SessionId, e.EventId })
                .IsUnique()
                .HasDatabaseName("events_session_id_event_id_key");
            entity.HasIndex(e => new { e.SessionId, e.SequenceNumber })
                .IsUnique()
                .HasDatabaseName("events_session_id_sequence_number_key");
            entity.HasIndex(e => new { e.SessionId, e.SequenceNumber })
                .HasDatabaseName("ix_events_session_seq");
            entity.HasIndex(e => new { e.SessionId, e.Timestamp })
                .HasDatabaseName("ix_events_session_time");
            entity.HasIndex(e => new { e.SessionId, e.ActionType })
                .HasDatabaseName("ix_events_session_action");
            entity.HasIndex(e => new { e.PlayerId, e.Timestamp })
                .HasDatabaseName("ix_events_player_time");
            entity.HasIndex(e => e.Payload)
                .HasMethod("gin")
                .HasDatabaseName("ix_events_payload_gin");
        });

        modelBuilder.Entity<CashflowProjectionDb>(entity =>
        {
            entity.ToTable("event_cashflow_projections", table =>
            {
                table.HasCheckConstraint("event_cashflow_projections_direction_check", "direction in ('IN','OUT')");
                table.HasCheckConstraint("event_cashflow_projections_amount_check", "amount > 0");
            });
            entity.HasKey(e => e.ProjectionId);
            entity.Property(e => e.ProjectionId).HasColumnName("projection_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.EventPk).HasColumnName("event_pk");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.Property(e => e.Direction).HasColumnName("direction").HasMaxLength(3);
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Category).HasColumnName("category").HasMaxLength(40);
            entity.Property(e => e.Counterparty).HasColumnName("counterparty").HasMaxLength(20);
            entity.Property(e => e.Reference).HasColumnName("reference").HasMaxLength(80);
            entity.Property(e => e.Note).HasColumnName("note").HasMaxLength(160);

            entity.HasOne<SessionDb>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("event_cashflow_projections_session_id_fkey");
            entity.HasOne<PlayerDb>()
                .WithMany()
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("event_cashflow_projections_player_id_fkey");
            entity.HasOne<EventDb>()
                .WithMany()
                .HasForeignKey(e => e.EventPk)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("event_cashflow_projections_event_pk_fkey");

            entity.HasIndex(e => new { e.SessionId, e.EventId })
                .IsUnique()
                .HasDatabaseName("event_cashflow_projections_session_id_event_id_key");
            entity.HasIndex(e => new { e.SessionId, e.Timestamp })
                .HasDatabaseName("ix_ecp_session_time");
            entity.HasIndex(e => new { e.SessionId, e.PlayerId, e.Timestamp })
                .HasDatabaseName("ix_ecp_session_player_time");
            entity.HasIndex(e => e.Category)
                .HasDatabaseName("ix_ecp_category");
        });

        modelBuilder.Entity<MetricSnapshotDb>(entity =>
        {
            entity.ToTable("metric_snapshots");
            entity.HasKey(e => e.MetricSnapshotId);
            entity.Property(e => e.MetricSnapshotId).HasColumnName("metric_snapshot_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.ComputedAt).HasColumnName("computed_at").HasDefaultValueSql("now()");
            entity.Property(e => e.MetricName).HasColumnName("metric_name").HasMaxLength(80);
            entity.Property(e => e.MetricValueNumeric).HasColumnName("metric_value_numeric");
            entity.Property(e => e.MetricValueJson).HasColumnName("metric_value_json").HasColumnType("jsonb");
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");

            entity.HasOne<SessionDb>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("metric_snapshots_session_id_fkey");
            entity.HasOne<PlayerDb>()
                .WithMany()
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("metric_snapshots_player_id_fkey");
            entity.HasOne<RulesetVersionDb>()
                .WithMany()
                .HasForeignKey(e => e.RulesetVersionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("metric_snapshots_ruleset_version_id_fkey");

            entity.HasIndex(e => new { e.SessionId, e.MetricName, e.ComputedAt })
                .HasDatabaseName("ix_metrics_session_name_time");
            entity.HasIndex(e => new { e.SessionId, e.PlayerId, e.MetricName, e.ComputedAt })
                .HasDatabaseName("ix_metrics_session_player_name_time");
            entity.HasIndex(e => new { e.SessionId, e.PlayerId, e.ComputedAt })
                .HasDatabaseName("ix_metrics_session_player_time");
            entity.HasIndex(e => e.RulesetVersionId)
                .HasDatabaseName("ix_metrics_ruleset_version");
        });

        modelBuilder.Entity<ValidationLogEf>(entity =>
        {
            entity.ToTable("validation_logs");
            entity.HasKey(e => e.ValidationLogId);
            entity.Property(e => e.ValidationLogId).HasColumnName("validation_log_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.EventPk).HasColumnName("event_pk");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.IsValid).HasColumnName("is_valid");
            entity.Property(e => e.ErrorCode).HasColumnName("error_code").HasMaxLength(40);
            entity.Property(e => e.ErrorMessage).HasColumnName("error_message").HasMaxLength(200);
            entity.Property(e => e.DetailsJson).HasColumnName("details_json").HasColumnType("jsonb");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne<SessionDb>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("validation_logs_session_id_fkey");
            entity.HasOne<EventDb>()
                .WithMany()
                .HasForeignKey(e => e.EventPk)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("validation_logs_event_pk_fkey");

            entity.HasIndex(e => new { e.SessionId, e.EventId })
                .IsUnique()
                .HasDatabaseName("validation_logs_session_id_event_id_key");
            entity.HasIndex(e => new { e.SessionId, e.CreatedAt })
                .HasDatabaseName("ix_validation_session_time");
            entity.HasIndex(e => e.IsValid)
                .HasDatabaseName("ix_validation_valid");
        });

        modelBuilder.Entity<SecurityAuditLogDb>(entity =>
        {
            entity.ToTable("security_audit_logs", table =>
            {
                table.HasCheckConstraint("security_audit_logs_outcome_check", "outcome in ('SUCCESS','FAILURE','DENIED')");
                table.HasCheckConstraint("security_audit_logs_status_code_check", "status_code >= 100 and status_code <= 599");
            });
            entity.HasKey(e => e.SecurityAuditLogId);
            entity.Property(e => e.SecurityAuditLogId).HasColumnName("security_audit_log_id");
            entity.Property(e => e.OccurredAt).HasColumnName("occurred_at").HasDefaultValueSql("now()");
            entity.Property(e => e.TraceId).HasColumnName("trace_id").HasMaxLength(64);
            entity.Property(e => e.EventType).HasColumnName("event_type").HasMaxLength(80);
            entity.Property(e => e.Outcome).HasColumnName("outcome").HasMaxLength(20);
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(80);
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(20);
            entity.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(64);
            entity.Property(e => e.UserAgent).HasColumnName("user_agent").HasMaxLength(300);
            entity.Property(e => e.Method).HasColumnName("method").HasMaxLength(16);
            entity.Property(e => e.Path).HasColumnName("path").HasMaxLength(240);
            entity.Property(e => e.StatusCode).HasColumnName("status_code");
            entity.Property(e => e.DetailJson).HasColumnName("detail_json").HasColumnType("jsonb");

            entity.HasOne<AppUserEf>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("security_audit_logs_user_id_fkey");

            entity.HasIndex(e => e.OccurredAt)
                .HasDatabaseName("ix_security_audit_logs_occurred");
            entity.HasIndex(e => new { e.EventType, e.OccurredAt })
                .HasDatabaseName("ix_security_audit_logs_event");
            entity.HasIndex(e => new { e.UserId, e.OccurredAt })
                .HasDatabaseName("ix_security_audit_logs_user");
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
