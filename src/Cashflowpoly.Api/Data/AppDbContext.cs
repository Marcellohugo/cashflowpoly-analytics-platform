using Microsoft.EntityFrameworkCore;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// EF Core context untuk manajemen migrasi skema database. Tidak digunakan untuk query runtime — Dapper menangani semua akses data.
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUserEf> AppUsers { get; set; } = null!;
    public DbSet<SessionDb> Sessions { get; set; } = null!;
    public DbSet<RulesetDb> Rulesets { get; set; } = null!;
    public DbSet<RulesetVersionDb> RulesetVersions { get; set; } = null!;
    public DbSet<SessionPlayerEf> SessionPlayers { get; set; } = null!;
    public DbSet<SessionRulesetActivationEf> SessionRulesetActivations { get; set; } = null!;
    public DbSet<SessionStateEf> SessionStates { get; set; } = null!;
    public DbSet<SessionPlayerStateEf> SessionPlayerStates { get; set; } = null!;
    public DbSet<SessionPlayerIngredientEf> SessionPlayerIngredients { get; set; } = null!;
    public DbSet<SessionPlayerNeedEf> SessionPlayerNeeds { get; set; } = null!;
    public DbSet<SessionPlayerFinancialGoalEf> SessionPlayerFinancialGoals { get; set; } = null!;
    public DbSet<SessionPlayerCollectionMissionEf> SessionPlayerCollectionMissions { get; set; } = null!;
    public DbSet<SessionPlayerQuestProgressEf> SessionPlayerQuestProgress { get; set; } = null!;
    public DbSet<SessionPlayerActionCounterEf> SessionPlayerActionCounters { get; set; } = null!;
    public DbSet<SessionPlayerAssetEf> SessionPlayerAssets { get; set; } = null!;
    public DbSet<SessionPlayerPeduliDonasiEf> SessionPlayerPeduliDonasi { get; set; } = null!;
    public DbSet<SessionDonationEventEf> SessionDonationEvents { get; set; } = null!;
    public DbSet<SessionCardPositionEf> SessionCardPositions { get; set; } = null!;
    public DbSet<SessionActionLogEf> SessionActionLogs { get; set; } = null!;
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
            entity.Property(e => e.DisplayName).HasColumnName("display_name").HasMaxLength(80);
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
            entity.Property(e => e.Mode).HasColumnName("mode").HasMaxLength(10);
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
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.JoinOrder).HasColumnName("join_order").HasDefaultValue(1);
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(20).HasDefaultValue("PLAYER");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne<SessionDb>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_players_session_id_fkey");
            entity.HasOne<AppUserEf>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("session_players_user_id_fkey");

            entity.HasIndex(e => new { e.SessionId, e.UserId })
                .IsUnique()
                .HasDatabaseName("session_players_session_id_user_id_key");
            entity.HasIndex(e => new { e.SessionId, e.JoinOrder })
                .HasDatabaseName("ix_session_players_session");
            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("ix_session_players_user");
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

        modelBuilder.Entity<SessionStateEf>(entity =>
        {
            entity.ToTable("session_states", table =>
            {
                table.HasCheckConstraint("session_states_day_check", "day >= 1");
                table.HasCheckConstraint("session_states_turn_number_check", "turn_number >= 1");
                table.HasCheckConstraint("session_states_moves_left_check", "moves_left >= 0");
                table.HasCheckConstraint("session_states_finish_day_check", "finish_day >= 1");
                table.HasCheckConstraint("session_states_state_version_check", "state_version >= 1");
            });
            entity.HasKey(e => e.SessionId);
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.Day).HasColumnName("day");
            entity.Property(e => e.TurnNumber).HasColumnName("turn_number");
            entity.Property(e => e.MovesLeft).HasColumnName("moves_left");
            entity.Property(e => e.FinishDay).HasColumnName("finish_day");
            entity.Property(e => e.IsGameOver).HasColumnName("is_game_over").HasDefaultValue(false);
            entity.Property(e => e.StateVersion).HasColumnName("state_version").HasDefaultValue(1L);
            entity.Property(e => e.UiStateJson).HasColumnName("ui_state_json").HasColumnType("jsonb").HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

            entity.HasOne<SessionDb>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_states_session_id_fkey");
        });

        modelBuilder.Entity<SessionPlayerStateEf>(entity =>
        {
            entity.ToTable("session_player_states", table =>
            {
                table.HasCheckConstraint("session_player_states_coins_check", "coins >= 0");
                table.HasCheckConstraint("session_player_states_happiness_check", "happiness >= 0");
                table.HasCheckConstraint("session_player_states_saving_check", "saving >= 0");
            });
            entity.HasKey(e => e.SessionPlayerId);
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            entity.Property(e => e.Coins).HasColumnName("coins");
            entity.Property(e => e.Happiness).HasColumnName("happiness");
            entity.Property(e => e.Saving).HasColumnName("saving");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

            entity.HasOne<SessionPlayerEf>()
                .WithMany()
                .HasForeignKey(e => e.SessionPlayerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_player_states_session_player_id_fkey");
        });

        modelBuilder.Entity<SessionPlayerIngredientEf>(entity =>
        {
            entity.ToTable("session_player_ingredients", table =>
            {
                table.HasCheckConstraint("session_player_ingredients_qty_check", "qty >= 0");
            });
            entity.HasKey(e => new { e.SessionPlayerId, e.IngredientId });
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id").HasMaxLength(80);
            entity.Property(e => e.Qty).HasColumnName("qty");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

            entity.HasOne<SessionPlayerEf>()
                .WithMany()
                .HasForeignKey(e => e.SessionPlayerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_player_ingredients_session_player_id_fkey");
        });

        modelBuilder.Entity<SessionPlayerNeedEf>(entity =>
        {
            entity.ToTable("session_player_needs", table =>
            {
                table.HasCheckConstraint("session_player_needs_sort_order_check", "sort_order >= 1");
                table.HasCheckConstraint("session_player_needs_paid_amount_check", "paid_amount >= 0");
                table.HasCheckConstraint("session_player_needs_purchased_at_day_check", "purchased_at_day >= 1");
            });
            entity.HasKey(e => e.SessionPlayerNeedId);
            entity.Property(e => e.SessionPlayerNeedId).HasColumnName("session_player_need_id");
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            entity.Property(e => e.RulesetCatalogItemId).HasColumnName("ruleset_catalog_item_id");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.PaidAmount).HasColumnName("paid_amount");
            entity.Property(e => e.HappinessDelta).HasColumnName("happiness_delta");
            entity.Property(e => e.PurchasedAtDay).HasColumnName("purchased_at_day");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne<SessionPlayerEf>()
                .WithMany()
                .HasForeignKey(e => e.SessionPlayerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_player_needs_session_player_id_fkey");

            entity.HasIndex(e => new { e.SessionPlayerId, e.SortOrder })
                .HasDatabaseName("ix_session_player_needs_player");
        });

        modelBuilder.Entity<SessionPlayerFinancialGoalEf>(entity =>
        {
            entity.ToTable("session_player_financial_goals", table =>
            {
                table.HasCheckConstraint("session_player_financial_goals_purchased_at_day_check", "purchased_at_day >= 1");
            });
            entity.HasKey(e => new { e.SessionPlayerId, e.RulesetCatalogItemId });
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            entity.Property(e => e.RulesetCatalogItemId).HasColumnName("ruleset_catalog_item_id");
            entity.Property(e => e.PurchasedAtDay).HasColumnName("purchased_at_day");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne<SessionPlayerEf>()
                .WithMany()
                .HasForeignKey(e => e.SessionPlayerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_player_financial_goals_session_player_id_fkey");
        });

        modelBuilder.Entity<SessionPlayerCollectionMissionEf>(entity =>
        {
            entity.ToTable("session_player_collection_missions");
            entity.HasKey(e => new { e.SessionPlayerId, e.RulesetCatalogItemId });
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            entity.Property(e => e.RulesetCatalogItemId).HasColumnName("ruleset_catalog_item_id");
            entity.Property(e => e.IsCompleted).HasColumnName("is_completed").HasDefaultValue(false);
            entity.Property(e => e.IsFailed).HasColumnName("is_failed").HasDefaultValue(false);
            entity.Property(e => e.RewardApplied).HasColumnName("reward_applied").HasDefaultValue(false);
            entity.Property(e => e.AssignedAt).HasColumnName("assigned_at").HasDefaultValueSql("now()");

            entity.HasOne<SessionPlayerEf>()
                .WithMany()
                .HasForeignKey(e => e.SessionPlayerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_player_collection_missions_session_player_id_fkey");
        });

        modelBuilder.Entity<SessionPlayerQuestProgressEf>(entity =>
        {
            entity.ToTable("session_player_quest_progress", table =>
            {
                table.HasCheckConstraint("session_player_quest_progress_progress_check", "progress >= 0");
                table.HasCheckConstraint("session_player_quest_progress_target_check", "target >= 0");
            });
            entity.HasKey(e => new { e.SessionPlayerId, e.QuestId });
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            entity.Property(e => e.QuestId).HasColumnName("quest_id").HasMaxLength(120);
            entity.Property(e => e.Progress).HasColumnName("progress");
            entity.Property(e => e.Target).HasColumnName("target");
            entity.Property(e => e.IsCompleted).HasColumnName("is_completed").HasDefaultValue(false);
            entity.Property(e => e.IsRewardClaimed).HasColumnName("is_reward_claimed").HasDefaultValue(false);

            entity.HasOne<SessionPlayerEf>()
                .WithMany()
                .HasForeignKey(e => e.SessionPlayerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_player_quest_progress_session_player_id_fkey");
        });

        modelBuilder.Entity<SessionPlayerActionCounterEf>(entity =>
        {
            entity.ToTable("session_player_action_counters", table =>
            {
                table.HasCheckConstraint("session_player_action_counters_count_check", "count >= 0");
            });
            entity.HasKey(e => new { e.SessionPlayerId, e.ActionId });
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            entity.Property(e => e.ActionId).HasColumnName("action_id").HasMaxLength(80);
            entity.Property(e => e.Count).HasColumnName("count");

            entity.HasOne<SessionPlayerEf>()
                .WithMany()
                .HasForeignKey(e => e.SessionPlayerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_player_action_counters_session_player_id_fkey");
        });

        modelBuilder.Entity<SessionPlayerAssetEf>(entity =>
        {
            entity.ToTable("session_player_assets", table =>
            {
                table.HasCheckConstraint("session_player_assets_asset_type_check", "asset_type in ('GOLD','LOAN','INSURANCE')");
                table.HasCheckConstraint("session_player_assets_quantity_check", "quantity >= 0");
            });
            entity.HasKey(e => e.SessionPlayerAssetId);
            entity.Property(e => e.SessionPlayerAssetId).HasColumnName("session_player_asset_id");
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            entity.Property(e => e.AssetType).HasColumnName("asset_type").HasMaxLength(20);
            entity.Property(e => e.AssetCode).HasColumnName("asset_code").HasMaxLength(120);
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.MetadataJson).HasColumnName("metadata_json").HasColumnType("jsonb").HasDefaultValueSql("'{}'::jsonb");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

            entity.HasOne<SessionPlayerEf>()
                .WithMany()
                .HasForeignKey(e => e.SessionPlayerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_player_assets_session_player_id_fkey");

            entity.HasIndex(e => new { e.SessionPlayerId, e.AssetType, e.AssetCode })
                .IsUnique()
                .HasDatabaseName("session_player_assets_session_player_id_asset_type_asset_code_key");
            entity.HasIndex(e => new { e.SessionPlayerId, e.AssetType })
                .HasDatabaseName("ix_session_player_assets_player");
        });

        modelBuilder.Entity<SessionPlayerPeduliDonasiEf>(entity =>
        {
            entity.ToTable("session_player_peduli_donasi", table =>
            {
                table.HasCheckConstraint("session_player_peduli_donasi_total_donasi_check", "total_donasi >= 0");
            });
            entity.HasKey(e => e.SessionPlayerId);
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            entity.Property(e => e.TotalDonasi).HasColumnName("total_donasi").HasDefaultValue(0);

            entity.HasOne<SessionPlayerEf>()
                .WithMany()
                .HasForeignKey(e => e.SessionPlayerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_player_peduli_donasi_session_player_id_fkey");
        });

        modelBuilder.Entity<SessionDonationEventEf>(entity =>
        {
            entity.ToTable("session_donation_events", table =>
            {
                table.HasCheckConstraint("session_donation_events_event_ke_check", "event_ke >= 1");
                table.HasCheckConstraint("session_donation_events_day_check", "day >= 1");
            });
            entity.HasKey(e => e.DonationEventId);
            entity.Property(e => e.DonationEventId).HasColumnName("donation_event_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.EventKe).HasColumnName("event_ke");
            entity.Property(e => e.Day).HasColumnName("day");
            entity.Property(e => e.RankingsJson).HasColumnName("rankings_json").HasColumnType("jsonb").HasDefaultValueSql("'[]'::jsonb");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne<SessionDb>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_donation_events_session_id_fkey");

            entity.HasIndex(e => new { e.SessionId, e.EventKe })
                .IsUnique()
                .HasDatabaseName("session_donation_events_session_id_event_ke_key");
            entity.HasIndex(e => new { e.SessionId, e.EventKe })
                .HasDatabaseName("ix_session_donation_events_session");
        });

        modelBuilder.Entity<SessionCardPositionEf>(entity =>
        {
            entity.ToTable("session_card_positions");
            entity.HasKey(e => e.CardPositionId);
            entity.Property(e => e.CardPositionId).HasColumnName("card_position_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            entity.Property(e => e.CardType).HasColumnName("card_type").HasMaxLength(40);
            entity.Property(e => e.CardRefId).HasColumnName("card_ref_id").HasMaxLength(120);
            entity.Property(e => e.PositionZone).HasColumnName("position_zone").HasMaxLength(80);
            entity.Property(e => e.PositionOrder).HasColumnName("position_order");
            entity.Property(e => e.SlotCode).HasColumnName("slot_code").HasMaxLength(80);
            entity.Property(e => e.SlotGroup).HasColumnName("slot_group").HasMaxLength(80);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("now()");

            entity.HasOne<SessionDb>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_card_positions_session_id_fkey");
            entity.HasOne<SessionPlayerEf>()
                .WithMany()
                .HasForeignKey(e => e.SessionPlayerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_card_positions_session_player_id_fkey");

            entity.HasIndex(e => new { e.SessionId, e.SessionPlayerId, e.CardType, e.PositionOrder })
                .HasDatabaseName("ix_session_card_positions_session");
        });

        modelBuilder.Entity<SessionActionLogEf>(entity =>
        {
            entity.ToTable("session_action_logs", table =>
            {
                table.HasCheckConstraint("session_action_logs_state_version_check", "state_version >= 1");
            });
            entity.HasKey(e => e.ActionLogId);
            entity.Property(e => e.ActionLogId).HasColumnName("action_log_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            entity.Property(e => e.StateVersion).HasColumnName("state_version");
            entity.Property(e => e.ActionId).HasColumnName("action_id").HasMaxLength(80);
            entity.Property(e => e.ActionJson).HasColumnName("action_json").HasColumnType("jsonb");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne<SessionDb>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("session_action_logs_session_id_fkey");
            entity.HasOne<SessionPlayerEf>()
                .WithMany()
                .HasForeignKey(e => e.SessionPlayerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("session_action_logs_session_player_id_fkey");

            entity.HasIndex(e => new { e.SessionId, e.StateVersion, e.CreatedAt })
                .HasDatabaseName("ix_session_action_logs_session");
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
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ActorType).HasColumnName("actor_type").HasMaxLength(10);
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.Property(e => e.DayIndex).HasColumnName("day_index");
            entity.Property(e => e.Weekday).HasColumnName("weekday").HasMaxLength(3);
            entity.Property(e => e.TurnNumber).HasColumnName("turn_number");
            entity.Property(e => e.SequenceNumber).HasColumnName("sequence_number");
            entity.Property(e => e.ActionId).HasColumnName("action_id").HasMaxLength(80);
            entity.Property(e => e.ActionType).HasColumnName("action_type").HasMaxLength(80);
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            entity.Property(e => e.Payload).HasColumnName("payload").HasColumnType("jsonb");
            entity.Property(e => e.ReceivedAt).HasColumnName("received_at").HasDefaultValueSql("now()");
            entity.Property(e => e.ClientRequestId).HasColumnName("client_request_id").HasMaxLength(120);

            entity.HasOne<SessionDb>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("events_session_id_fkey");
            entity.HasOne<SessionPlayerEf>()
                .WithMany()
                .HasForeignKey(e => e.SessionPlayerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("events_session_player_id_fkey");
            entity.HasOne<AppUserEf>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("events_user_id_fkey");
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
            entity.HasIndex(e => new { e.UserId, e.Timestamp })
                .HasDatabaseName("ix_events_user_time");
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
            entity.Property(e => e.UserId).HasColumnName("user_id");
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
            entity.HasOne<AppUserEf>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("event_cashflow_projections_user_id_fkey");
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
            entity.HasIndex(e => new { e.SessionId, e.UserId, e.Timestamp })
                .HasDatabaseName("ix_ecp_session_user_time");
            entity.HasIndex(e => e.Category)
                .HasDatabaseName("ix_ecp_category");
        });

        modelBuilder.Entity<MetricSnapshotDb>(entity =>
        {
            entity.ToTable("metric_snapshots");
            entity.HasKey(e => e.MetricSnapshotId);
            entity.Property(e => e.MetricSnapshotId).HasColumnName("metric_snapshot_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.SessionPlayerId).HasColumnName("session_player_id");
            entity.Property(e => e.ComputedAt).HasColumnName("computed_at").HasDefaultValueSql("now()");
            entity.Property(e => e.MetricName).HasColumnName("metric_name").HasMaxLength(120);
            entity.Property(e => e.MetricValueNumeric).HasColumnName("metric_value_numeric");
            entity.Property(e => e.MetricValueJson).HasColumnName("metric_payload_json").HasColumnType("jsonb");
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");

            entity.HasOne<SessionDb>()
                .WithMany()
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("metric_snapshots_session_id_fkey");
            entity.HasOne<AppUserEf>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("metric_snapshots_user_id_fkey");
            entity.HasOne<SessionPlayerEf>()
                .WithMany()
                .HasForeignKey(e => e.SessionPlayerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("metric_snapshots_session_player_id_fkey");
            entity.HasOne<RulesetVersionDb>()
                .WithMany()
                .HasForeignKey(e => e.RulesetVersionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("metric_snapshots_ruleset_version_id_fkey");

            entity.HasIndex(e => new { e.SessionId, e.MetricName, e.ComputedAt })
                .HasDatabaseName("ix_metrics_session_name_time");
            entity.HasIndex(e => new { e.SessionId, e.UserId, e.MetricName, e.ComputedAt })
                .HasDatabaseName("ix_metrics_session_user_name_time");
            entity.HasIndex(e => new { e.SessionId, e.UserId, e.ComputedAt })
                .HasDatabaseName("ix_metrics_session_user_time");
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
    public string DisplayName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// Entitas EF Core untuk tabel session_players — digunakan hanya untuk generasi migrasi skema.
/// </summary>
public sealed class SessionPlayerEf
{
    public Guid SessionPlayerId { get; set; }
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
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

public sealed class SessionStateEf
{
    public Guid SessionId { get; set; }
    public int Day { get; set; }
    public int TurnNumber { get; set; }
    public int MovesLeft { get; set; }
    public int FinishDay { get; set; }
    public bool IsGameOver { get; set; }
    public long StateVersion { get; set; }
    public string UiStateJson { get; set; } = "{}";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class SessionPlayerStateEf
{
    public Guid SessionPlayerId { get; set; }
    public int Coins { get; set; }
    public int Happiness { get; set; }
    public int Saving { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class SessionPlayerIngredientEf
{
    public Guid SessionPlayerId { get; set; }
    public string IngredientId { get; set; } = string.Empty;
    public int Qty { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class SessionPlayerNeedEf
{
    public Guid SessionPlayerNeedId { get; set; }
    public Guid SessionPlayerId { get; set; }
    public Guid RulesetCatalogItemId { get; set; }
    public int SortOrder { get; set; }
    public int PaidAmount { get; set; }
    public int HappinessDelta { get; set; }
    public int PurchasedAtDay { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class SessionPlayerFinancialGoalEf
{
    public Guid SessionPlayerId { get; set; }
    public Guid RulesetCatalogItemId { get; set; }
    public int PurchasedAtDay { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class SessionPlayerCollectionMissionEf
{
    public Guid SessionPlayerId { get; set; }
    public Guid RulesetCatalogItemId { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsFailed { get; set; }
    public bool RewardApplied { get; set; }
    public DateTimeOffset AssignedAt { get; set; }
}

public sealed class SessionPlayerQuestProgressEf
{
    public Guid SessionPlayerId { get; set; }
    public string QuestId { get; set; } = string.Empty;
    public int Progress { get; set; }
    public int Target { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsRewardClaimed { get; set; }
}

public sealed class SessionPlayerActionCounterEf
{
    public Guid SessionPlayerId { get; set; }
    public string ActionId { get; set; } = string.Empty;
    public int Count { get; set; }
}

public sealed class SessionPlayerAssetEf
{
    public Guid SessionPlayerAssetId { get; set; }
    public Guid SessionPlayerId { get; set; }
    public string AssetType { get; set; } = string.Empty;
    public string AssetCode { get; set; } = string.Empty;
    public int? Quantity { get; set; }
    public int? Amount { get; set; }
    public string MetadataJson { get; set; } = "{}";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class SessionPlayerPeduliDonasiEf
{
    public Guid SessionPlayerId { get; set; }
    public int TotalDonasi { get; set; }
}

public sealed class SessionDonationEventEf
{
    public Guid DonationEventId { get; set; }
    public Guid SessionId { get; set; }
    public int EventKe { get; set; }
    public int Day { get; set; }
    public string RankingsJson { get; set; } = "[]";
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class SessionCardPositionEf
{
    public Guid CardPositionId { get; set; }
    public Guid SessionId { get; set; }
    public Guid? SessionPlayerId { get; set; }
    public string CardType { get; set; } = string.Empty;
    public string CardRefId { get; set; } = string.Empty;
    public string? PositionZone { get; set; }
    public int? PositionOrder { get; set; }
    public string? SlotCode { get; set; }
    public string? SlotGroup { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class SessionActionLogEf
{
    public Guid ActionLogId { get; set; }
    public Guid SessionId { get; set; }
    public Guid? SessionPlayerId { get; set; }
    public long StateVersion { get; set; }
    public string? ActionId { get; set; }
    public string ActionJson { get; set; } = "{}";
    public DateTimeOffset CreatedAt { get; set; }
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
