using System.Text.RegularExpressions;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class BootstrapAssetConsistencyTests
{
    private static readonly string RepoRoot = ResolveRepositoryRoot();

    [Fact]
    public void BootstrapAssetList_ShouldOnlyReferenceCanonicalSchemaAndDefaultRulesetSeed()
    {
        var initializerPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Infrastructure", "DatabaseInitialization.cs");
        var apiProjectPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Cashflowpoly.Api.csproj");
        var apiTestProjectPath = Path.Combine(RepoRoot, "tests", "Cashflowpoly.Api.Tests", "Cashflowpoly.Api.Tests.csproj");

        Assert.DoesNotContain("02_seed_full_inspection.sql", File.ReadAllText(initializerPath), StringComparison.Ordinal);
        Assert.DoesNotContain("02_seed_full_inspection.sql", File.ReadAllText(apiProjectPath), StringComparison.Ordinal);
        Assert.DoesNotContain("02_seed_full_inspection.sql", File.ReadAllText(apiTestProjectPath), StringComparison.Ordinal);
        Assert.DoesNotContain("03_seed_inspection_matrix.sql", File.ReadAllText(initializerPath), StringComparison.Ordinal);
        Assert.DoesNotContain("03_seed_inspection_matrix.sql", File.ReadAllText(apiProjectPath), StringComparison.Ordinal);
        Assert.DoesNotContain("03_seed_inspection_matrix.sql", File.ReadAllText(apiTestProjectPath), StringComparison.Ordinal);
        Assert.DoesNotContain("03_session_state_seed.sql", File.ReadAllText(initializerPath), StringComparison.Ordinal);
        Assert.DoesNotContain("03_session_state_seed.sql", File.ReadAllText(apiProjectPath), StringComparison.Ordinal);
        Assert.DoesNotContain("03_session_state_seed.sql", File.ReadAllText(apiTestProjectPath), StringComparison.Ordinal);
        Assert.DoesNotContain("02_seed_simulation_sessions_events.sql", File.ReadAllText(initializerPath), StringComparison.Ordinal);
        Assert.DoesNotContain("02_seed_simulation_sessions_events.sql", File.ReadAllText(apiProjectPath), StringComparison.Ordinal);
        Assert.DoesNotContain("02_seed_simulation_sessions_events.sql", File.ReadAllText(apiTestProjectPath), StringComparison.Ordinal);
    }

    [Fact]
    public void Bootstrap_ShouldNotCarryLegacyMigrationHistoryCompatibility()
    {
        var initializerPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Infrastructure", "DatabaseInitialization.cs");
        var initializerContent = File.ReadAllText(initializerPath);

        Assert.DoesNotContain("__EFMigrationsHistory", initializerContent, StringComparison.Ordinal);
        Assert.DoesNotContain("BaselineLegacySchemaAsync", initializerContent, StringComparison.Ordinal);
        Assert.DoesNotContain("InitialSchema", initializerContent, StringComparison.Ordinal);
        Assert.DoesNotContain("SchemaParityAndDefaultSeed", initializerContent, StringComparison.Ordinal);
        Assert.DoesNotContain("SessionStateSchemaConsolidation", initializerContent, StringComparison.Ordinal);
        Assert.Contains("reset", initializerContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables()
    {
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        var schemaContent = File.ReadAllText(schemaPath);
        var sessionParticipantsTable = ExtractCreateTable(schemaContent, "session_participants");
        var gameAssetsTable = ExtractCreateTable(schemaContent, "ruleset_game_assets");

        Assert.Contains("create extension if not exists citext", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists session_participants", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("player_order_no", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("player_name varchar(80)", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ruleset_version_id uuid not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("create table if not exists session_ruleset_activations", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("role varchar(20)", sessionParticipantsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists ruleset_game_assets", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ruleset_game_asset_id uuid not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("asset_type varchar(40) not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("constraint uq_ruleset_game_assets_scope unique (ruleset_version_id, asset_type, asset_code)", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("'QUEST'", gameAssetsTable, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("'NARRATIVE'", gameAssetsTable, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("'COLLECTION_MISSION'", gameAssetsTable, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("'FINANCIAL_GOAL'", gameAssetsTable, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("'LOAN'", gameAssetsTable, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("'INSURANCE'", gameAssetsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists event_asset_references", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(
            "references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id)",
            schemaContent,
            StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ruleset_action_id uuid not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(
            "foreign key (ruleset_version_id, ruleset_action_id)",
            schemaContent,
            StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists session_participant_inventory", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists session_participant_gold_holdings", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists session_participant_loans", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists session_participant_insurances", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists session_participant_tie_breakers", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists session_final_scores", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists session_final_score_components", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(
            "foreign key (session_id, source_event_id)",
            schemaContent,
            StringComparison.OrdinalIgnoreCase);
        Assert.Contains("last_event_id uuid null", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists session_card_positions", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("card_instance_id uuid not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("fk_session_card_positions_last_event_id", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists ruleset_need_set_bonuses", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists ruleset_game_settings", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists ruleset_trigger_conditions", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ruleset_action_id uuid not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists ruleset_narrative_scenes", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists session_narrative_logs", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("create table if not exists ruleset_quests", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("create table if not exists session_participant_quest_progress", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists session_projection_checkpoints", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("fk_session_projection_checkpoints_last_event_id", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("last_event_pk", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create or replace function project_session_event", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create or replace function project_session_events", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create or replace function rebuild_session_projection", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("SQL-only projection rebuild is disabled to prevent destructive state loss", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("v_projected := project_session_events", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("enforce_event_session_scope", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("trg_events_session_scope", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("create table if not exists ruleset_player_ordering_instructor_users", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists ruleset_orders", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists ruleset_collection_mission_requirements", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("created_by_user_id uuid null", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("created_by varchar", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("payload_version varchar(20) not null default '1.0'", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("uq_events_session_client_request", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("is_archived boolean not null default false", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("foreign key (ruleset_version_id, ruleset_game_asset_id)", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("foreign key (ruleset_version_id, required_asset_id)", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("create table if not exists session_participant_narrative_logs", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists event_cashflow_projections", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("payload jsonb not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("raw_payload_json jsonb not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("metric_payload_json jsonb", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("last_event_id uuid", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("source_event_id uuid", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("source_json jsonb", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("status varchar(20) not null default 'NOT_STARTED'", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("enforce_session_ruleset_activation", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("enforce_session_card_position_catalog", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("coalesce(new.player_count, 0) < coalesce(v_min_players, 2)", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ix_events_session_player_seq", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ix_events_received_at", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ix_metric_snapshots_computed_at", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ix_validation_logs_created_at", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ix_event_cashflow_projections_event_pk", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("apply_default_log_retention_policies", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("('events'", schemaContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings()
    {
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        var schemaContent = File.ReadAllText(schemaPath);
        var eventsTable = ExtractCreateTable(schemaContent, "events");
        var sessionStatesTable = ExtractCreateTable(schemaContent, "session_states");
        var narrativeLogsTable = ExtractCreateTable(schemaContent, "session_narrative_logs");

        Assert.DoesNotContain("action_slot between 1 and 2", eventsTable, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("action_slot between 1 and 2", sessionStatesTable, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("action_slot between 1 and 2", narrativeLogsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ck_events_action_slot check (action_slot >= 0)", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ck_session_states_action_slot check (action_slot >= 0)", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ck_session_states_current_action_slot check (current_action_slot >= 0)", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ck_session_narrative_logs_action_slot check (action_slot >= 1)", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("enforce_action_slot_within_ruleset_limit", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("tg_table_name = 'events' and new.actor_type = 'SYSTEM'", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("tg_table_name = 'session_states' and coalesce(new.turn_number, 0) = 0", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("rgs.actions_per_turn", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("trg_events_action_slot_limit", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("trg_session_states_action_slot_limit", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("trg_session_narrative_logs_action_slot_limit", schemaContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder()
    {
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        var schemaContent = File.ReadAllText(schemaPath);
        var eventsTable = ExtractCreateTable(schemaContent, "events");
        var sessionStatesTable = ExtractCreateTable(schemaContent, "session_states");

        Assert.Contains("turn_number int not null", eventsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("turn_number int not null", sessionStatesTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ck_events_turn_number check (turn_number between 0 and 4)", eventsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ck_session_states_turn_number check (turn_number between 0 and 4)", sessionStatesTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ck_events_actor_turn_slot_shape", eventsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("new.actor_type = 'SYSTEM'", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("new.turn_number <> 0", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("new.action_slot <> 0", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("new.actor_type = 'PLAYER'", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("new.turn_number <> v_player_order_no", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("new.action_slot < 1", schemaContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CanonicalSchema_ShouldBindEventActionTypeToRulesetActionBehavior()
    {
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        var schemaContent = File.ReadAllText(schemaPath);

        Assert.Contains("behavior_id varchar(80) not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("v_behavior_id", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("select a.mode, ra.behavior_id", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("new.action_type is distinct from v_behavior_id", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Event action_type % must match ruleset action behavior_id %", schemaContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency()
    {
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        var schemaContent = File.ReadAllText(schemaPath);
        var projectionsTable = ExtractCreateTable(schemaContent, "event_cashflow_projections");

        Assert.Contains("allowed_asset_type", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("session_participant_inventory' then 'INGREDIENT'", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("session_participant_gold_holdings' then 'GOLD'", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("session_participant_tie_breakers' then 'TIE_BREAKER'", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("session_card_positions' then 'CARD_POSITION'", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("event_asset_references' then 'EVENT_REFERENCE'", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("copy_number > coalesce(v_card_qty, 0)", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("new.zone = 'PLAYER' and new.owner_session_participant_id is null", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("new.zone <> 'PLAYER' and new.owner_session_participant_id is not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("projection_order int not null default 1", projectionsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("unique (session_id, event_id, projection_order)", projectionsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("enforce_event_cashflow_projection_consistency", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("trg_event_cashflow_projection_consistency", schemaContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CanonicalSchema_ShouldTrackSchemaBaselineFingerprint()
    {
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        var schemaContent = File.ReadAllText(schemaPath);

        Assert.Contains("create table if not exists schema_baseline_versions", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("baseline_name varchar", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("schema_version varchar", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("checksum varchar", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("compute_schema_fingerprint", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("assert_schema_baseline", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("information_schema.columns", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("pg_constraint", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("pg_indexes", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("pg_trigger", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("pg_proc", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("pg_get_functiondef(p.oid)", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("p.prokind in ('f', 'p')", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("d.deptype = 'e'", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("pg_views", schemaContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints()
    {
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        var schemaContent = File.ReadAllText(schemaPath);
        var seedPath = Path.Combine(RepoRoot, "database", "01_seed_default_rulesets_components.sql");
        var seedContent = File.ReadAllText(seedPath);

        Assert.Contains("create table if not exists ruleset_rank_points", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("rank_type varchar(20) not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("rank_type in ('DONATION', 'PENSION')", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("create table if not exists ruleset_donation_rank_points", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("create table if not exists ruleset_pension_rank_points", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create or replace view ruleset_catalog_item_requirements", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create or replace view ruleset_collection_mission_requirement_items", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("rcmr.ruleset_collection_mission_id", ExtractView(schemaContent, "ruleset_catalog_item_requirements"), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into ruleset_rank_points", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into ruleset_donation_rank_points", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into ruleset_pension_rank_points", seedContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CanonicalSchema_ShouldNotContainLegacyCatalogPlayerOrPlayerOrderArtifacts()
    {
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        var schemaContent = File.ReadAllText(schemaPath);
        var appDbContextPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Data", "AppDbContext.cs");
        var appDbContextContent = File.ReadAllText(appDbContextPath);

        var forbidden = new[]
        {
            "create table if not exists ruleset_catalog_items",
            "create table if not exists ruleset_catalog_item_requirements",
            "create table if not exists app_menus",
            "create table if not exists role_menu_permissions",
            "create table if not exists ruleset_player_ordering_instructor_users",
            "create table if not exists ingredients",
            "create table if not exists game_components",
            "create table if not exists session_players",
            "create table if not exists session_player_assets",
            "create table if not exists session_participant_assets",
            "create table if not exists session_action_logs",
            "create table if not exists session_ruleset_activations",
            "create table if not exists ruleset_quests",
            "create table if not exists session_participant_quest_progress",
            "create table if not exists session_participant_narrative_logs",
            "create table if not exists interpreter_commands",
            "create table if not exists quest_scripts",
            "create table if not exists narrative_assets",
            "create table if not exists narrative_scripts",
            "create table if not exists session_donation_event_rankings",
            "create table if not exists session_pension_rankings",
            "create table if not exists event_inventory_effects",
            "create table if not exists event_need_effects",
            "create table if not exists event_goal_effects",
            "create table if not exists event_asset_effects",
            "create table if not exists event_score_effects",
            "create table if not exists event_turn_effects",
            "config_json jsonb",
            "ix_ruleset_versions_config_gin",
            "refresh_ruleset_version_config_json",
            "sync_ruleset_version_relational_content",
            "trg_ruleset_versions_sync_relational_content",
            "rankings_cache_json",
            "ruleset_game_asset_id uuid not null,\r\n  narrative_code",
            "ruleset_quest_id",
            "card_ref_id varchar",
            "ingredient_value varchar",
            "requirement_value varchar",
            "event_action_type varchar",
            "action_id varchar(80) null",
            "details_json jsonb not null default '{}'::jsonb",
            "source_json jsonb not null default '{}'::jsonb",
            "player_index",
            "rankings_json jsonb",
            "session_participants_role"
        };

        foreach (var entry in forbidden)
        {
            Assert.DoesNotContain(entry, schemaContent, StringComparison.OrdinalIgnoreCase);
        }

        var forbiddenEfMappings = new[]
        {
            "ConfigJson",
            "session_players",
            "session_player_assets",
            "session_participant_assets",
            "SessionParticipantAsset",
            "session_ruleset_activations",
            "ruleset_quests",
            "session_participant_quest_progress",
            "interpreter_commands",
            "narrative_assets",
            "quest_scripts",
            "narrative_scripts",
            "session_donation_event_rankings",
            "session_pension_rankings",
            "session_player_states",
            "session_player_ingredients",
            "session_player_needs",
            "session_player_financial_goals",
            "session_player_collection_missions",
            "session_player_quest_progress",
            "session_player_action_counters"
        };

        foreach (var entry in forbiddenEfMappings)
        {
            Assert.DoesNotContain(entry, appDbContextContent, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes()
    {
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        var schemaContent = File.ReadAllText(schemaPath);

        var duplicateIndexes = new[]
        {
            "ix_ruleset_versions_ruleset",
            "ix_ruleset_player_ordering_rules_ruleset",
            "ix_ruleset_player_ordering_instructor_users_ruleset",
            "ix_session_participants_session",
            "ix_session_donation_events_session",
            "ix_session_donation_event_rankings_event",
            "ix_events_session_seq"
        };

        foreach (var indexName in duplicateIndexes)
        {
            Assert.DoesNotContain(indexName, schemaContent, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes()
    {
        var seedPath = Path.Combine(RepoRoot, "database", "01_seed_default_rulesets_components.sql");
        var seedContent = File.ReadAllText(seedPath);

        Assert.Contains("created_by_user_id", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into app_menus", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into role_menu_permissions", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ruleset_player_ordering_instructor_users", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into ruleset_game_assets", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into ruleset_game_settings", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into ruleset_orders", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into ruleset_collection_mission_requirements", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into ruleset_need_set_bonuses", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("'THREE_DIFFERENT'", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("'THREE_SAME'", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into ruleset_gold_prices", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into ruleset_tie_breakers", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into ruleset_sharia_loans", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into ruleset_insurance_products", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into ruleset_life_risks", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into ruleset_narratives", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into ruleset_narrative_scenes", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("BagikanEmasAwal", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("AmbilKartuDariDeck", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("KartuDiambilDariPasar", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("KartuMasukDiscard", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("IsiUlangPasar", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("gold.initial.granted", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("card.drawn", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("card.taken", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("card.discarded", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("market.refilled", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into ruleset_trigger_conditions", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into ruleset_quests", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"""quest""", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("asset_type = 'NARRATIVE'", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("asset_type = 'QUEST'", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into interpreter_commands", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into quest_scripts", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into narrative_scripts", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("refresh_ruleset_version_config_json", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("sync_ruleset_version_relational_content", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""starting_cash"": 20", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""starting_cash"": 10", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""actions_per_turn"": 2", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""finishDay"": 25", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"""finishDay"": 13", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("min_players,", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("resep-sayur-bumbu", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""nama"": ""jam""", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("gold_price_6", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("loan_syariah_10", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""card_supply"":8", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("risk_perbaikan_atap", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""premium"":1", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"""scripts"": [", seedContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook()
    {
        var seedPath = Path.Combine(RepoRoot, "database", "01_seed_default_rulesets_components.sql");
        var seedContent = File.ReadAllText(seedPath);

        var startingCashValues = Regex.Matches(seedContent, @"""starting_cash""\s*:\s*(\d+)", RegexOptions.IgnoreCase)
            .Select(match => int.Parse(match.Groups[1].Value))
            .ToList();
        var actionsPerTurnValues = Regex.Matches(seedContent, @"""actions_per_turn""\s*:\s*(\d+)", RegexOptions.IgnoreCase)
            .Select(match => int.Parse(match.Groups[1].Value))
            .ToList();
        var finishDayValues = Regex.Matches(seedContent, @"""finishDay""\s*:\s*(\d+)", RegexOptions.IgnoreCase)
            .Select(match => int.Parse(match.Groups[1].Value))
            .ToList();
        var donationPointBlocks = Regex.Matches(seedContent, @"""score_source""\s*:\s*""DONATION""\s*,\s*""rank""\s*:\s*(?<rank>\d)\s*,\s*""points""\s*:\s*(?<points>\d+)", RegexOptions.IgnoreCase)
            .Select(match => $"{match.Groups["rank"].Value}:{match.Groups["points"].Value}")
            .ToList();
        var pensionPointBlocks = Regex.Matches(seedContent, @"""score_source""\s*:\s*""PENSION""\s*,\s*""rank""\s*:\s*(?<rank>\d)\s*,\s*""points""\s*:\s*(?<points>\d+)", RegexOptions.IgnoreCase)
            .Select(match => $"{match.Groups["rank"].Value}:{match.Groups["points"].Value}")
            .ToList();
        var missionPenaltyValues = Regex.Matches(seedContent, @"""success_points""\s*:\s*(?<success>-?\d+)\s*,\s*""failure_points""\s*:\s*(?<failure>-?\d+)", RegexOptions.IgnoreCase)
            .Select(match => $"{match.Groups["success"].Value}:{match.Groups["failure"].Value}")
            .ToList();

        Assert.NotEmpty(startingCashValues);
        Assert.NotEmpty(actionsPerTurnValues);
        Assert.NotEmpty(finishDayValues);
        Assert.NotEmpty(donationPointBlocks);
        Assert.NotEmpty(pensionPointBlocks);
        Assert.NotEmpty(missionPenaltyValues);
        Assert.Contains(20, startingCashValues);
        Assert.Contains(10, startingCashValues);
        Assert.All(startingCashValues, value => Assert.Contains(value, new[] { 10, 20 }));
        Assert.All(actionsPerTurnValues, value => Assert.Equal(2, value));
        Assert.All(finishDayValues, value => Assert.Equal(25, value));
        Assert.Equal(
            new[] { "1:7", "2:5", "3:2", "1:7", "2:5", "3:2" },
            donationPointBlocks);
        Assert.Equal(
            new[] { "1:5", "2:3", "3:1", "1:5", "2:3", "3:1" },
            pensionPointBlocks);
        Assert.All(missionPenaltyValues, value => Assert.Equal("0:-10", value));
    }

    [Fact]
    public void ManualSimulationSeed_ShouldExist_AsStandaloneManualSql()
    {
        var seedPath = Path.Combine(RepoRoot, "database", "02_seed_simulation_sessions_events.sql");

        Assert.True(File.Exists(seedPath), $"Seed simulasi manual harus tersedia pada path '{seedPath}'.");

        var seedContent = File.ReadAllText(seedPath);
        Assert.Contains("Simulasi Cashflowpoly Kelas XI IPS 2 - Mode Pemula - Kelompok A", seedContent, StringComparison.Ordinal);
        Assert.Contains("Simulasi Cashflowpoly Kelas XI IPS 2 - Mode Mahir - Kelompok B", seedContent, StringComparison.Ordinal);
        Assert.DoesNotContain("insert into session_participant_assets", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into session_participant_gold_holdings", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into session_participant_loans", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into session_participant_insurances", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into session_participant_tie_breakers", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into session_pension_rankings", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into session_final_scores", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into session_final_score_components", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("BagikanTieBreaker", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("tie_breaker.assigned", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("session_participant_collection_missions", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("session_participant_quest_progress", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("session_ruleset_activations", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("spb.coins + spb.saving + coalesce(ingredients.leftover_qty, 0)", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("least(latest.day_index, rgs.finish_day)", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"""ingredient_name"":""Bumbu""", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(
            "'donation.rank.awarded', 'donation.rank.awarded', '{\"rank\":1,\"points\":5}'",
            seedContent,
            StringComparison.OrdinalIgnoreCase);

        var forbidden = new[]
        {
            "session_donation_event_rankings",
            "event_inventory_effects",
            "event_need_effects",
            "event_goal_effects",
            "event_asset_effects",
            "event_score_effects",
            "event_turn_effects",
            "ruleset_versions.config_json"
        };

        foreach (var entry in forbidden)
        {
            Assert.DoesNotContain(entry, seedContent, StringComparison.OrdinalIgnoreCase);
        }
    }

    private static string ResolveRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Cashflowpoly.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Tidak dapat menemukan root repositori (Cashflowpoly.sln).");
    }

    private static string ExtractCreateTable(string sql, string tableName)
    {
        var match = Regex.Match(
            sql,
            $@"create table if not exists {Regex.Escape(tableName)}\s*\((?<body>.*?)\n\);",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        Assert.True(match.Success, $"Table '{tableName}' should exist in canonical schema.");
        return match.Value;
    }

    private static string ExtractView(string sql, string viewName)
    {
        var match = Regex.Match(
            sql,
            $@"create or replace view {Regex.Escape(viewName)} as(?<body>.*?);",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        Assert.True(match.Success, $"View '{viewName}' should exist in canonical schema.");
        return match.Value;
    }
}
