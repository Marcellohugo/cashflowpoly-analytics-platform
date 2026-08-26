// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk AppDbContext.
using Microsoft.EntityFrameworkCore;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// EF Core context untuk design-time schema tooling. Runtime data access memakai Dapper.
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUserEf> AppUsers { get; set; } = null!;
    public DbSet<SessionDb> Sessions { get; set; } = null!;
    public DbSet<RulesetDb> Rulesets { get; set; } = null!;
    public DbSet<RulesetVersionDb> RulesetVersions { get; set; } = null!;
    public DbSet<SessionParticipantEf> SessionParticipants { get; set; } = null!;
    public DbSet<SessionStateEf> SessionStates { get; set; } = null!;
    public DbSet<EventDb> Events { get; set; } = null!;
    public DbSet<CashflowProjectionDb> EventCashflowProjections { get; set; } = null!;
    public DbSet<MetricSnapshotDb> MetricSnapshots { get; set; } = null!;
    public DbSet<ValidationLogEf> ValidationLogs { get; set; } = null!;
    public DbSet<SecurityAuditLogDb> SecurityAuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<AppUserEf>(entity =>
        {
            entity.ToTable("app_users", table =>
            {
                table.HasCheckConstraint("ck_app_users_role", "role in ('INSTRUCTOR', 'PLAYER')");
            });
            entity.HasKey(e => e.UserId).HasName("pk_app_users");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Username).HasColumnName("username").HasColumnType("citext");
            entity.Property(e => e.DisplayName).HasColumnName("display_name").HasMaxLength(80);
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(20);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasIndex(e => e.Username).IsUnique().HasDatabaseName("uq_app_users_username");
            entity.HasIndex(e => new { e.Role, e.IsActive }).HasDatabaseName("ix_app_users_role_active");
        });

        modelBuilder.Entity<SessionDb>(entity =>
        {
            entity.ToTable("sessions", table =>
            {
                table.HasCheckConstraint("ck_sessions_mode", "mode in ('PEMULA', 'MAHIR')");
                table.HasCheckConstraint("ck_sessions_status", "status in ('CREATED', 'STARTED', 'ENDED')");
                table.HasCheckConstraint("ck_sessions_player_count", "player_count between 0 and 4");
            });
            entity.HasKey(e => e.SessionId).HasName("pk_sessions");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.SessionName).HasColumnName("session_name").HasMaxLength(120);
            entity.Property(e => e.Mode).HasColumnName("mode").HasMaxLength(10);
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(10);
            entity.Property<int>("PlayerCount").HasColumnName("player_count").HasDefaultValue(0);
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.EndedAt).HasColumnName("ended_at");
            entity.Property(e => e.InstructorUserId).HasColumnName("instructor_user_id");
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            entity.Property(e => e.IsArchived).HasColumnName("is_archived").HasDefaultValue(false);
            entity.Property(e => e.ArchivedAt).HasColumnName("archived_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne<AppUserEf>()
                .WithMany()
                .HasForeignKey(e => e.InstructorUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_sessions_instructor_user_id");

            entity.HasOne<RulesetVersionDb>()
                .WithMany()
                .HasForeignKey(e => e.RulesetVersionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_sessions_ruleset_version_id");

            entity.HasIndex(e => e.Status).HasDatabaseName("ix_sessions_status");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("ix_sessions_created_at");
            entity.HasIndex(e => new { e.InstructorUserId, e.IsArchived, e.CreatedAt }).HasDatabaseName("ix_sessions_instructor_user");
            entity.HasIndex(e => e.RulesetVersionId).HasDatabaseName("ix_sessions_ruleset_version");
        });

        modelBuilder.Entity<RulesetDb>(entity =>
        {
            entity.ToTable("rulesets");
            entity.HasKey(e => e.RulesetId).HasName("pk_rulesets");
            entity.Property(e => e.RulesetId).HasColumnName("ruleset_id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(120);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.InstructorUserId).HasColumnName("instructor_user_id");
            entity.Property(e => e.IsArchived).HasColumnName("is_archived").HasDefaultValue(false);
            entity.Property(e => e.ArchivedAt).HasColumnName("archived_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");

            entity.HasOne<AppUserEf>()
                .WithMany()
                .HasForeignKey(e => e.InstructorUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_rulesets_instructor_user_id");

            entity.HasOne<AppUserEf>()
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_rulesets_created_by_user_id");

            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("ix_rulesets_created_at");
            entity.HasIndex(e => new { e.InstructorUserId, e.IsArchived, e.CreatedAt }).HasDatabaseName("ix_rulesets_instructor_user");
        });

        modelBuilder.Entity<RulesetVersionDb>(entity =>
        {
            entity.ToTable("ruleset_versions", table =>
            {
                table.HasCheckConstraint("ck_ruleset_versions_version", "version >= 1");
                table.HasCheckConstraint("ck_ruleset_versions_status", "status in ('DRAFT', 'ACTIVE', 'ARCHIVED')");
                table.HasCheckConstraint("ck_ruleset_versions_mode", "mode in ('PEMULA', 'MAHIR')");
            });
            entity.HasKey(e => e.RulesetVersionId).HasName("pk_ruleset_versions");
            entity.Ignore(e => e.Definition);
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            entity.Property(e => e.RulesetId).HasColumnName("ruleset_id");
            entity.Property(e => e.Version).HasColumnName("version");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(10);
            entity.Property(e => e.Mode).HasColumnName("mode").HasMaxLength(10);
            entity.Property<string>("SchemaVersion").HasColumnName("schema_version").HasMaxLength(20).HasDefaultValue("3.0.0");
            entity.Property(e => e.ConfigHash).HasColumnName("config_hash").HasMaxLength(128);
            entity.Property<string?>("ChangeNote").HasColumnName("change_note");
            entity.Property<DateTimeOffset?>("PublishedAt").HasColumnName("published_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");

            entity.HasOne<RulesetDb>()
                .WithMany()
                .HasForeignKey(e => e.RulesetId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_ruleset_versions_ruleset_id");

            entity.HasOne<AppUserEf>()
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_ruleset_versions_created_by_user_id");

            entity.HasIndex(e => new { e.RulesetId, e.Version }).IsUnique().HasDatabaseName("uq_ruleset_versions_ruleset_version");
            entity.HasIndex(e => new { e.RulesetId, e.ConfigHash }).IsUnique().HasDatabaseName("uq_ruleset_versions_ruleset_config_hash");
            entity.HasIndex(e => new { e.Status, e.Mode, e.CreatedAt }).HasDatabaseName("ix_ruleset_versions_status_mode");
            entity.HasIndex(e => e.RulesetId)
                .IsUnique()
                .HasFilter("status = 'ACTIVE'")
                .HasDatabaseName("uq_ruleset_versions_one_active_per_ruleset");
        });

        modelBuilder.Entity<SessionParticipantEf>(entity =>
        {
            entity.ToTable("session_participants", table =>
            {
                table.HasCheckConstraint("ck_session_participants_player_order_no", "player_order_no between 1 and 4");
                table.HasCheckConstraint("ck_session_participants_player_name_not_blank", "player_name is null or nullif(btrim(player_name), '') is not null");
            });
            entity.HasKey(e => e.SessionParticipantId).HasName("pk_session_participants");
            entity.HasAlternateKey(e => new { e.SessionId, e.SessionParticipantId }).HasName("uq_session_participants_session_participant");
            entity.Property(e => e.SessionParticipantId).HasColumnName("session_participant_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.PlayerOrderNo).HasColumnName("player_order_no");
            entity.Property(e => e.PlayerName).HasColumnName("player_name").HasMaxLength(80);
            entity.Property(e => e.JoinedAt).HasColumnName("joined_at").HasDefaultValueSql("now()");

            entity.HasOne<SessionDb>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_session_participants_session_id");
            entity.HasOne<AppUserEf>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_session_participants_user_id");

            entity.HasIndex(e => new { e.SessionId, e.UserId }).IsUnique().HasDatabaseName("uq_session_participants_session_user");
            entity.HasIndex(e => new { e.SessionId, e.PlayerOrderNo }).IsUnique().HasDatabaseName("uq_session_participants_session_seat");
            entity.HasIndex(e => e.UserId).HasDatabaseName("ix_session_participants_user");
        });

        modelBuilder.Entity<SessionStateEf>(entity =>
        {
            entity.ToTable("session_states");
            entity.HasKey(e => e.SessionId).HasName("pk_session_states");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.Day).HasColumnName("day");
            entity.Property(e => e.Weekday).HasColumnName("weekday").HasMaxLength(3);
            entity.Property(e => e.TurnNumber).HasColumnName("turn_number");
            entity.Property(e => e.ActionSlot).HasColumnName("action_slot");
            entity.Property(e => e.CurrentSessionPlayerId).HasColumnName("current_session_player_id");
            entity.Property(e => e.CurrentActionSlot).HasColumnName("current_action_slot").HasDefaultValue(1);
            entity.Property(e => e.ActionSlotsLeft).HasColumnName("action_slots_left");
            entity.Property(e => e.FinishDay).HasColumnName("finish_day");
            entity.Property(e => e.Phase).HasColumnName("phase").HasMaxLength(30).HasDefaultValue("PLAYER_TURN");
            entity.Property(e => e.IsGameOver).HasColumnName("is_game_over").HasDefaultValue(false);
            entity.Property(e => e.StateVersion).HasColumnName("state_version").HasDefaultValue(1L);
            entity.Property(e => e.LastEventId).HasColumnName("last_event_id");
            entity.Property(e => e.UiStateJson).HasColumnName("ui_state_json").HasColumnType("jsonb").HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

            entity.HasOne<SessionDb>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_session_states_session_id");
        });

        modelBuilder.Entity<EventDb>(entity =>
        {
            entity.ToTable("events");
            entity.HasKey(e => e.EventPk).HasName("pk_events");
            entity.Property(e => e.EventPk).HasColumnName("event_pk");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ActorType).HasColumnName("actor_type").HasMaxLength(10);
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.Property(e => e.DayIndex).HasColumnName("day_index");
            entity.Property(e => e.Weekday).HasColumnName("weekday").HasMaxLength(3);
            entity.Property(e => e.TurnNumber).HasColumnName("turn_number");
            entity.Property(e => e.ActionSlot).HasColumnName("action_slot");
            entity.Property(e => e.SequenceNumber).HasColumnName("sequence_number");
            entity.Property(e => e.RulesetActionId).HasColumnName("ruleset_action_id");
            entity.Ignore(e => e.ActionId);
            entity.Property(e => e.ActionType).HasColumnName("action_type").HasMaxLength(80);
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            entity.Property(e => e.PayloadVersion).HasColumnName("payload_version").HasMaxLength(20).HasDefaultValue("1.0");
            entity.Property(e => e.Payload).HasColumnName("payload").HasColumnType("jsonb");
            entity.Property(e => e.ReceivedAt).HasColumnName("received_at").HasDefaultValueSql("now()");
            entity.Property(e => e.ClientRequestId).HasColumnName("client_request_id").HasMaxLength(120);

            entity.HasIndex(e => new { e.SessionId, e.EventId }).IsUnique().HasDatabaseName("uq_events_session_event");
            entity.HasIndex(e => new { e.SessionId, e.SequenceNumber }).IsUnique().HasDatabaseName("uq_events_session_sequence");
            entity.HasIndex(e => new { e.SessionId, e.ClientRequestId })
                .IsUnique()
                .HasFilter("client_request_id is not null")
                .HasDatabaseName("uq_events_session_client_request");
            entity.HasIndex(e => new { e.SessionId, e.SessionPlayerId, e.SequenceNumber }).HasDatabaseName("ix_events_session_player_seq");
            entity.HasIndex(e => new { e.SessionId, e.Timestamp }).HasDatabaseName("ix_events_session_time");
            entity.HasIndex(e => new { e.SessionId, e.ActionType }).HasDatabaseName("ix_events_session_action");
            entity.HasIndex(e => new { e.UserId, e.Timestamp }).HasDatabaseName("ix_events_user_time");
            entity.HasIndex(e => e.ReceivedAt).HasDatabaseName("ix_events_received_at");
        });

        modelBuilder.Entity<CashflowProjectionDb>(entity =>
        {
            entity.ToTable("event_cashflow_projections");
            entity.HasKey(e => e.ProjectionId).HasName("pk_event_cashflow_projections");
            entity.Property(e => e.ProjectionId).HasColumnName("projection_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.EventPk).HasColumnName("event_pk");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.ProjectionOrder).HasColumnName("projection_order").HasDefaultValue(1);
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.Property(e => e.Direction).HasColumnName("direction").HasMaxLength(3);
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Category).HasColumnName("category").HasMaxLength(40);
            entity.Property(e => e.Counterparty).HasColumnName("counterparty").HasMaxLength(40);
            entity.Property(e => e.Reference).HasColumnName("reference").HasMaxLength(120);
            entity.Property(e => e.Note).HasColumnName("note").HasMaxLength(200);

            entity.HasOne<EventDb>()
                .WithMany()
                .HasForeignKey(e => e.EventPk)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_event_cashflow_projections_event_pk");

            entity.HasIndex(e => new { e.SessionId, e.EventId, e.ProjectionOrder }).IsUnique().HasDatabaseName("uq_event_cashflow_projections_session_event_order");
            entity.HasIndex(e => new { e.SessionId, e.Timestamp }).HasDatabaseName("ix_ecp_session_time");
            entity.HasIndex(e => new { e.SessionId, e.UserId, e.Timestamp }).HasDatabaseName("ix_ecp_session_user_time");
            entity.HasIndex(e => e.Category).HasDatabaseName("ix_ecp_category");
            entity.HasIndex(e => e.EventPk).HasDatabaseName("ix_event_cashflow_projections_event_pk");
        });

        modelBuilder.Entity<MetricSnapshotDb>(entity =>
        {
            entity.ToTable("metric_snapshots");
            entity.HasKey(e => e.MetricSnapshotId).HasName("pk_metric_snapshots");
            entity.Property(e => e.MetricSnapshotId).HasColumnName("metric_snapshot_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            entity.Property(e => e.ComputedAt).HasColumnName("computed_at").HasDefaultValueSql("now()");
            entity.Property(e => e.MetricName).HasColumnName("metric_name").HasMaxLength(120);
            entity.Property(e => e.MetricValueNumeric).HasColumnName("metric_value_numeric");
            entity.Property<string?>("MetricValueText").HasColumnName("metric_value_text");
            entity.Property<bool?>("MetricValueBoolean").HasColumnName("metric_value_boolean");
            entity.Property(e => e.MetricValueJson).HasColumnName("metric_payload_json").HasColumnType("jsonb");
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            entity.Property(e => e.LastEventId).HasColumnName("last_event_id");

            entity.HasIndex(e => new { e.SessionId, e.MetricName, e.ComputedAt }).HasDatabaseName("ix_metrics_session_name_time");
            entity.HasIndex(e => new { e.SessionId, e.UserId, e.MetricName, e.ComputedAt }).HasDatabaseName("ix_metrics_session_user_name_time");
            entity.HasIndex(e => new { e.SessionId, e.SessionPlayerId, e.ComputedAt }).HasDatabaseName("ix_metric_snapshots_session_player_time");
            entity.HasIndex(e => e.RulesetVersionId).HasDatabaseName("ix_metrics_ruleset_version");
            entity.HasIndex(e => e.ComputedAt).HasDatabaseName("ix_metric_snapshots_computed_at");
        });

        modelBuilder.Entity<ValidationLogEf>(entity =>
        {
            entity.ToTable("validation_logs");
            entity.HasKey(e => e.ValidationLogId).HasName("pk_validation_logs");
            entity.Property(e => e.ValidationLogId).HasColumnName("validation_log_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.RawPayloadJson).HasColumnName("raw_payload_json").HasColumnType("jsonb");
            entity.Property(e => e.ErrorCode).HasColumnName("error_code").HasMaxLength(40);
            entity.Property(e => e.ErrorMessage).HasColumnName("error_message").HasMaxLength(240);
            entity.Property(e => e.StatusCode).HasColumnName("status_code").HasDefaultValue(422);
            entity.Property(e => e.TraceId).HasColumnName("trace_id").HasMaxLength(64).HasDefaultValue("legacy");
            entity.Property(e => e.DetailsJson).HasColumnName("details_json").HasColumnType("jsonb");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne<SessionDb>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_validation_logs_session_id");

            entity.HasOne<RulesetVersionDb>()
                .WithMany()
                .HasForeignKey(e => e.RulesetVersionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_validation_logs_ruleset_version_id");

            entity.HasIndex(e => new { e.SessionId, e.EventId }).IsUnique().HasDatabaseName("uq_validation_logs_session_event");
            entity.HasIndex(e => new { e.SessionId, e.CreatedAt }).HasDatabaseName("ix_validation_session_time");
            entity.HasIndex(e => e.ErrorCode).HasDatabaseName("ix_validation_logs_error_code");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("ix_validation_logs_created_at");
        });

        modelBuilder.Entity<SecurityAuditLogDb>(entity =>
        {
            entity.ToTable("security_audit_logs");
            entity.HasKey(e => e.SecurityAuditLogId).HasName("pk_security_audit_logs");
            entity.Property(e => e.SecurityAuditLogId).HasColumnName("security_audit_log_id");
            entity.Property(e => e.OccurredAt).HasColumnName("occurred_at").HasDefaultValueSql("now()");
            entity.Property(e => e.TraceId).HasColumnName("trace_id").HasMaxLength(64);
            entity.Property(e => e.EventType).HasColumnName("event_type").HasMaxLength(80);
            entity.Property(e => e.Outcome).HasColumnName("outcome").HasMaxLength(40);
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(80);
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(20);
            entity.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(80);
            entity.Property(e => e.UserAgent).HasColumnName("user_agent").HasMaxLength(300);
            entity.Property(e => e.Method).HasColumnName("method").HasMaxLength(16);
            entity.Property(e => e.Path).HasColumnName("path").HasMaxLength(240);
            entity.Property(e => e.StatusCode).HasColumnName("status_code");
            entity.Property(e => e.DetailJson).HasColumnName("detail_json").HasColumnType("jsonb");

            entity.HasIndex(e => e.OccurredAt).HasDatabaseName("ix_security_audit_logs_occurred");
            entity.HasIndex(e => new { e.EventType, e.OccurredAt }).HasDatabaseName("ix_security_audit_logs_event");
            entity.HasIndex(e => new { e.UserId, e.OccurredAt }).HasDatabaseName("ix_security_audit_logs_user");
        });
    }
}

public sealed class AppUserEf
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class SessionParticipantEf
{
    public Guid SessionParticipantId { get; set; }
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public int PlayerOrderNo { get; set; }
    public string? PlayerName { get; set; }
    public DateTimeOffset JoinedAt { get; set; }
}

public sealed class SessionStateEf
{
    public Guid SessionId { get; set; }
    public int Day { get; set; }
    public string Weekday { get; set; } = "MON";
    public int TurnNumber { get; set; }
    public int ActionSlot { get; set; }
    public Guid? CurrentSessionPlayerId { get; set; }
    public int CurrentActionSlot { get; set; }
    public int ActionSlotsLeft { get; set; }
    public int FinishDay { get; set; }
    public string Phase { get; set; } = "PLAYER_TURN";
    public bool IsGameOver { get; set; }
    public long StateVersion { get; set; }
    public Guid? LastEventId { get; set; }
    public string UiStateJson { get; set; } = "{}";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class ValidationLogEf
{
    public Guid ValidationLogId { get; set; }
    public Guid SessionId { get; set; }
    public Guid? RulesetVersionId { get; set; }
    public Guid EventId { get; set; }
    public string RawPayloadJson { get; set; } = "{}";
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public int StatusCode { get; set; }
    public string TraceId { get; set; } = "legacy";
    public string? DetailsJson { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
