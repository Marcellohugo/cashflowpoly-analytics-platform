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
    public void Bootstrap_ShouldNotCarryRemovedMigrationHistoryCompatibility()
    {
        var initializerPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Infrastructure", "DatabaseInitialization.cs");
        var initializerContent = File.ReadAllText(initializerPath);

        Assert.DoesNotContain("__EFMigrationsHistory", initializerContent, StringComparison.Ordinal);
        Assert.DoesNotContain(string.Concat("Baseline", "Le", "gacy", "SchemaAsync"), initializerContent, StringComparison.Ordinal);
        Assert.DoesNotContain("InitialSchema", initializerContent, StringComparison.Ordinal);
        Assert.DoesNotContain("SchemaParityAndDefaultSeed", initializerContent, StringComparison.Ordinal);
        Assert.DoesNotContain("SessionStateSchemaConsolidation", initializerContent, StringComparison.Ordinal);
        Assert.Contains("reset", initializerContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots()
    {
        var checkedFiles = new[]
        {
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Contracts", "RulesetDefinitionDtos.cs"),
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Contracts", "RulesetDefinitionDtos.cs"),
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Domain", "RulesetDefinitionMapper.cs"),
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Domain", "RulesetDefinitionMapper.cs"),
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Data", "RulesetRepository.cs"),
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Infrastructure", "UiTextLexicon.Rulesets.cs"),
            Path.Combine(RepoRoot, "postman", "Cashflowpoly.postman_collection.json")
        };

        foreach (var path in checkedFiles)
        {
            var content = File.ReadAllText(path);
            Assert.DoesNotContain(string.Concat("instructor", "_player", "_usernames"), content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(string.Concat("Instructor", "Player", "Usernames"), content, StringComparison.Ordinal);
            Assert.DoesNotContain(string.Concat("player", "_slots", "_title"), content, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void RulesetContracts_ShouldNotExposeAssignmentOrderingAliases()
    {
        var checkedFiles = new[]
        {
            Path.Combine(RepoRoot, "database", "00_create_schema.sql"),
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Domain", "RulesetConfig.cs"),
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Domain", "AnalyticsPlayerOrdering.cs"),
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Infrastructure", "UiTextLexicon.Rulesets.cs"),
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Views", "Rulesets", "Details.cshtml"),
            Path.Combine(RepoRoot, "tests", "Cashflowpoly.Api.Tests", "EventAnalyticsIntegrationTests.cs")
        };

        foreach (var path in checkedFiles)
        {
            var content = File.ReadAllText(path);
            Assert.DoesNotContain(string.Concat("INSTRUCTOR", "_ORDER"), content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(string.Concat("MANUAL", "_ORDER"), content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(string.Concat("USERNAME", "_IDN"), content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(string.Concat("Instructor", "Order"), content, StringComparison.Ordinal);
            Assert.DoesNotContain(string.Concat("player", "_ordering", "_instructor", "_order"), content, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void RulesetContracts_ShouldNotExposeNeedFamilyTable()
    {
        var checkedFiles = new[]
        {
            Path.Combine(RepoRoot, "database", "00_create_schema.sql"),
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Data", "RulesetRepository.cs")
        };

        foreach (var path in checkedFiles)
        {
            var content = File.ReadAllText(path);
            Assert.DoesNotContain(string.Concat("ruleset", "_need", "_families"), content, StringComparison.OrdinalIgnoreCase);
        }
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
        Assert.Matches(
            "create table if not exists ruleset_insurance_products[\\s\\S]*is_active boolean not null default true",
            schemaContent);
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
        AssertSqlContains("create or replace function project_session_event", schemaContent);
        AssertSqlContains("create or replace function project_session_events", schemaContent);
        AssertSqlContains("create or replace function ensure_session_card_positions_initialized", schemaContent);
        Assert.Contains("existing.ruleset_game_asset_id = rci.ruleset_catalog_item_id", schemaContent, StringComparison.OrdinalIgnoreCase);
        AssertSqlContains("create or replace function rebuild_session_projection", schemaContent);
        Assert.Contains("SQL-only projection rebuild is disabled to prevent destructive state loss", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("v_projected := project_session_events", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("enforce_event_session_scope", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("trg_events_session_scope", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(
            string.Concat("create table if not exists ruleset", "_player", "_ordering", "_instructor", "_users"),
            schemaContent,
            StringComparison.OrdinalIgnoreCase);
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
        AssertSqlContains("tg_table_name = 'events' and new.actor_type = 'SYSTEM'", schemaContent);
        AssertSqlContains("tg_table_name = 'session_states' and coalesce(new.turn_number, 0) = 0", schemaContent);
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
        AssertSqlContains("ck_events_turn_number check (turn_number between 0 and 4)", eventsTable);
        AssertSqlContains("ck_session_states_turn_number check (turn_number between 0 and 4)", sessionStatesTable);
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
        AssertSqlContains("select a.mode, ra.behavior_id", schemaContent);
        AssertSqlContains("new.action_type is distinct from v_behavior_id", schemaContent);
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
        AssertSqlContains("new.zone = 'PLAYER' and new.owner_session_participant_id is null", schemaContent);
        AssertSqlContains("new.zone <> 'PLAYER' and new.owner_session_participant_id is not null", schemaContent);
        Assert.Contains("projection_order int not null default 1", projectionsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("unique (session_id, event_id, projection_order)", projectionsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("enforce_event_cashflow_projection_consistency", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("trg_event_cashflow_projection_consistency", schemaContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements()
    {
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        var schemaContent = File.ReadAllText(schemaPath);
        var lifeRisksTable = ExtractCreateTable(schemaContent, "ruleset_life_risks");
        var ruleEffectsTable = ExtractCreateTable(schemaContent, "session_rule_effects");
        var missionRequirementsTable = ExtractCreateTable(schemaContent, "ruleset_collection_mission_requirements");

        Assert.Contains("duration_days int not null default 1", lifeRisksTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("target_scope varchar(40) not null default 'SELF'", lifeRisksTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ck_ruleset_life_risks_duration_days", lifeRisksTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ck_ruleset_life_risks_target_scope", lifeRisksTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("session_rule_effect_id uuid primary key default gen_random_uuid()", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("source_event_id uuid null", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("effect_type varchar(60) not null", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("target_scope varchar(40) not null", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("value_delta int null", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("starts_day int not null", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ends_day int null", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("metadata_json jsonb not null default '{}' :: jsonb", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("fk_session_rule_effects_session_id", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("required_need_family_code varchar(120) null", missionRequirementsTable, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("'NEED_FAMILY'", missionRequirementsTable, StringComparison.OrdinalIgnoreCase);
        AssertSqlContains("create or replace function apply_game_event", schemaContent);
    }

    [Fact]
    public void CanonicalSchema_ShouldProjectRuntimeStateFromAuthoritativeRulesetData()
    {
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        var schemaContent = File.ReadAllText(schemaPath);
        var projectorBody = ExtractFunction(schemaContent, "project_session_event");

        Assert.Contains("ruleset_order_requirements", projectorBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("jsonb_array_elements_text(coalesce(v_event.payload->'required_ingredient_card_ids'", projectorBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("v_event.action_type = 'Menabung'", projectorBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("v_event.action_type = 'TarikTabungan'", projectorBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("v_event.action_type = 'TujuanFinansial'", projectorBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("session_participant_financial_goals", projectorBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("happiness = session_participant_balances.happiness +", projectorBody, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades()
    {
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        var schemaContent = File.ReadAllText(schemaPath);
        var scopeValidatorBody = ExtractFunction(schemaContent, "enforce_event_session_scope");
        var projectorBody = ExtractFunction(schemaContent, "project_session_event");

        Assert.Contains("and not spnp.is_sold", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("v_option_type not in", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("'SELL_GOLD'", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("GunakanOpsiDarurat event payload must contain option_type", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("SELL_GOLD emergency option must contain qty, unit_price, and amount", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("v_active_gold_price", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("BukaHargaEmas", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Kepemilikan emas tidak cukup", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        Assert.Matches(@"upper\(v_event\.payload\s*->>\s*'option_type'\)\s*=\s*'USE_INSURANCE'", projectorBody);
        Assert.Matches(@"upper\(v_event\.payload\s*->>\s*'option_type'\)\s*=\s*'SELL_GOLD'", projectorBody);
        Assert.DoesNotContain("when v_event.action_type = 'GunakanOpsiDarurat' then upper(nullif(v_event.payload->>'direction", projectorBody, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules()
    {
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        var seedPath = Path.Combine(RepoRoot, "database", "01_seed_default_rulesets_components.sql");
        var schemaContent = File.ReadAllText(schemaPath);
        var seedContent = File.ReadAllText(seedPath);
        var scopeValidatorBody = ExtractFunction(schemaContent, "enforce_event_session_scope");
        var projectorBody = ExtractFunction(schemaContent, "project_session_event");

        Assert.Contains("Friday player actions are limited to donation", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Saturday player actions are limited to gold trades", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("v_new_risk_effect_type", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("v_new_risk_direction", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("RisikoKehidupan in MAHIR mode must immediately follow JualMasakan for the same player", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Emergency loan principal must match catalog principal", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Insurance premium must match catalog premium", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("'TarikTabungan'", seedContent, StringComparison.OrdinalIgnoreCase);
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
        Assert.DoesNotContain("E '\\n'", schemaContent, StringComparison.OrdinalIgnoreCase);
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
        AssertSqlContains("create or replace view ruleset_catalog_item_requirements", schemaContent);
        AssertSqlContains("create or replace view ruleset_collection_mission_requirement_items", schemaContent);
        Assert.DoesNotContain("rcmr.ruleset_collection_mission_id", ExtractView(schemaContent, "ruleset_catalog_item_requirements"), StringComparison.OrdinalIgnoreCase);
        AssertSqlContains("insert into ruleset_rank_points", seedContent);
        Assert.DoesNotContain("insert into ruleset_donation_rank_points", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into ruleset_pension_rank_points", seedContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts()
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
            string.Concat("create table if not exists ruleset", "_player", "_ordering", "_instructor", "_users"),
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
            string.Concat("ix_ruleset", "_player", "_ordering", "_instructor", "_users", "_ruleset"),
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
        Assert.DoesNotContain("$ json $", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into app_menus", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into role_menu_permissions", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(
            string.Concat("ruleset", "_player", "_ordering", "_instructor", "_users"),
            seedContent,
            StringComparison.OrdinalIgnoreCase);
        AssertSqlContains("insert into ruleset_game_assets", seedContent);
        AssertSqlContains("insert into ruleset_game_settings", seedContent);
        AssertSqlContains("insert into ruleset_orders", seedContent);
        AssertSqlContains("insert into ruleset_collection_mission_requirements", seedContent);
        AssertSqlContains("insert into ruleset_need_set_bonuses", seedContent);
        Assert.Contains("'THREE_DIFFERENT'", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("'THREE_SAME'", seedContent, StringComparison.OrdinalIgnoreCase);
        AssertSqlContains("insert into ruleset_gold_prices", seedContent);
        AssertSqlContains("insert into ruleset_tie_breakers", seedContent);
        AssertSqlContains("insert into ruleset_sharia_loans", seedContent);
        AssertSqlContains("insert into ruleset_insurance_products", seedContent);
        AssertSqlContains("insert into ruleset_life_risks", seedContent);
        AssertSqlContains("insert into ruleset_narratives", seedContent);
        AssertSqlContains("insert into ruleset_narrative_scenes", seedContent);
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
        AssertSqlContains("insert into ruleset_trigger_conditions", seedContent);
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
        Assert.Contains("gado_gado", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""nama"": ""jam""", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("gold_price_4", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("loan_syariah_10", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""card_supply"":8", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("risk_bencana_banjir", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""premium"":1", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"""scripts"": [", seedContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules()
    {
        var seedPath = Path.Combine(RepoRoot, "database", "01_seed_default_rulesets_components.sql");
        var seedContent = File.ReadAllText(seedPath);

        Assert.Contains(@"""primary_need_max_per_day"": null", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"""primary_need_max_per_day"": 1", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Matches(@"'risk_beli_peralatan_dapur'[\s\S]*?'COIN_EFFECT'[\s\S]*?'OUT'[\s\S]*?3\s*,", seedContent);
        Assert.Matches(@"'risk_sakit_gigi'[\s\S]*?'COIN_EFFECT'[\s\S]*?'OUT'[\s\S]*?4\s*,", seedContent);
        Assert.Matches(@"'risk_operasi_usus_buntu'[\s\S]*?'COIN_EFFECT'[\s\S]*?'OUT'[\s\S]*?6\s*,", seedContent);
        Assert.Contains("'risk_pemadaman_listrik',", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("'ALL_PLAYERS_COIN_EFFECT',", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""target_scope"":""ALL_PLAYERS""", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Matches(@"'risk_pemadaman_listrik'[\s\S]*?1\s*,\s*'ALL_PLAYERS'\s*,", seedContent);
        Assert.Contains("'risk_bbm_naik',", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("'INGREDIENT_PRICE_MODIFIER',", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""value_delta"":1", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Matches(@"'risk_bbm_naik'[\s\S]*?7\s*,\s*'ALL_PLAYERS'\s*,", seedContent);
        Assert.Contains("'risk_panen_melimpah',", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""value_delta"":-1", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Matches(@"'risk_panen_melimpah'[\s\S]*?7\s*,\s*'ALL_PLAYERS'\s*,", seedContent);
        Assert.Contains("'risk_investasi_emas',", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("'GOLD_TRADE',", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("'risk_ulang_tahun',", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("'PLAYER_TO_PLAYER_TRANSFER',", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""target_scope"":""OTHER_PLAYERS""", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Matches(@"'risk_ulang_tahun'[\s\S]*?1\s*,\s*'OTHER_PLAYERS'\s*,", seedContent);
        Assert.Contains(@"""type"": ""FAMILY"", ""value"": ""jam""", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""type"": ""FAMILY"", ""value"": ""boneka""", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""type"": ""FAMILY"", ""value"": ""gameboy""", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""type"": ""FAMILY"", ""value"": ""hiburan""", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"""value"": ""jam_2""", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"""value"": ""boneka_2""", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"""value"": ""gameboy_2""", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"""value"": ""hiburan_2""", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("required_need_family_code", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Matches(
            @"'multirisk_basic'[\s\S]*?0\s*,\s*'\{\""premium\"":\s*1",
            seedContent);
    }

    [Fact]
    public void DefaultRulesetSeed_ShouldIncludePemulaTieBreakerAssets()
    {
        var seedPath = Path.Combine(RepoRoot, "database", "01_seed_default_rulesets_components.sql");
        var seedContent = File.ReadAllText(seedPath);

        foreach (var tieBreakerCode in new[] { "tie_breaker_1", "tie_breaker_2", "tie_breaker_3", "tie_breaker_4" })
        {
            Assert.Matches(
                $@"\(\s*'f5b4c67b-0825-4970-9f07-3b68e8fcb524'\s*::\s*uuid\s*,\s*'TIE_BREAKER'\s*,\s*'{tieBreakerCode}'",
                seedContent);
            Assert.Matches(
                $@"\(\s*'f5b4c67b-0825-4970-9f07-3b68e8fcb524'\s*::\s*uuid\s*,\s*'{tieBreakerCode}'\s*,\s*\d+\s*,\s*\d+\s*,\s*1\s*,\s*'\{{\""number\"":",
                seedContent);
        }
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
        Assert.StartsWith("begin;", seedContent.TrimStart(), StringComparison.OrdinalIgnoreCase);
        Assert.EndsWith("commit;", seedContent.TrimEnd(), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Simulasi Cashflowpoly Kelas XI IPS 2 - Mode Pemula - Kelompok A", seedContent, StringComparison.Ordinal);
        Assert.Contains("Simulasi Cashflowpoly Kelas XI IPS 2 - Mode Mahir - Kelompok B", seedContent, StringComparison.Ordinal);
        Assert.DoesNotContain("insert into session_participant_assets", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into session_participant_gold_holdings", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into session_participant_loans", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into session_participant_insurances", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into session_participant_tie_breakers", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into session_participant_collection_missions", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into session_pension_rankings", seedContent, StringComparison.OrdinalIgnoreCase);
        AssertSqlContains("insert into session_final_scores", seedContent);
        AssertSqlContains("insert into session_final_score_components", seedContent);
        Assert.Contains("BagikanTieBreaker", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("tie_breaker.assigned", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("session_participant_collection_missions", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("session_participant_quest_progress", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("session_ruleset_activations", seedContent, StringComparison.OrdinalIgnoreCase);
        AssertSqlContains("spb.coins + spb.saving + coalesce(ingredients.leftover_qty, 0)", seedContent);
        Assert.DoesNotContain("least(latest.day_index, rgs.finish_day)", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"""ingredient_name"":""Bumbu""", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(
            "'donation.rank.awarded', 'donation.rank.awarded', '{\"rank\":1,\"points\":5}'",
            seedContent,
            StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("set local cashflowpoly.bypass_validation", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("disable trigger all", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("enable trigger all", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("order_setup_placeholder", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\"asset_code\":\"buku\"", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\"risk_event_ref\"", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Seed simulasi melampaui stok kartu fisik", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("risk_id", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("\"risk_id\":\"risk_investasi_emas\"", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\"direction\":\"IN\",\"amount\"", seedContent, StringComparison.OrdinalIgnoreCase);

        var applyIndex = seedContent.IndexOf("perform apply_game_event(", StringComparison.OrdinalIgnoreCase);
        var finalScoreIndex = seedContent.IndexOf("create temporary table seed_pension_rank_points", StringComparison.OrdinalIgnoreCase);
        Assert.True(applyIndex >= 0, "Manual simulation seed must insert events through apply_game_event.");
        Assert.True(finalScoreIndex > applyIndex, "Final score reporting must run after apply_game_event.");

        var postApplyRuntimeBlock = seedContent.Substring(applyIndex, finalScoreIndex - applyIndex);
        Assert.DoesNotContain("delete from event_cashflow_projections", postApplyRuntimeBlock, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into event_cashflow_projections", postApplyRuntimeBlock, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("delete from session_participant_balances", postApplyRuntimeBlock, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("insert into session_participant_financial_goals", postApplyRuntimeBlock, StringComparison.OrdinalIgnoreCase);

        var freeActionListPattern =
            @"'JumatBerkah'[\s\S]{0,160}'RisikoKehidupan'[\s\S]{0,160}'GunakanOpsiDarurat'[\s\S]{0,160}'InvestasiEmas'[\s\S]{0,160}'JualEmas'[\s\S]{0,160}'LewatiTransaksiEmas'[\s\S]{0,160}'HariMingguLibur'";
        Assert.True(
            Regex.Matches(seedContent, freeActionListPattern, RegexOptions.IgnoreCase).Count >= 3,
            "Manual simulation seed must use the canonical free-action list in all action_slot calculations.");

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

    [Fact]
    public void ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence()
    {
        var seedPath = Path.Combine(RepoRoot, "database", "02_seed_simulation_sessions_events.sql");
        var seedContent = File.ReadAllText(seedPath);

        Assert.Contains("resolve_gold_points(", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("sp.ruleset_version_id", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("rga.quantity = spgh.quantity", seedContent, StringComparison.OrdinalIgnoreCase);

        var events = ParseSimulationSeedEvents(seedContent).ToList();
        var pemulaTieBreakers = events
            .Where(e => e.Mode == "PEMULA" && e.ActionType == "BagikanTieBreaker")
            .ToList();
        Assert.Equal(4, pemulaTieBreakers.Count);
        Assert.Equal(
            new[] { 1, 2, 3, 4 },
            pemulaTieBreakers.Select(e => e.PlayerNo.GetValueOrDefault()).OrderBy(playerNo => playerNo).ToArray());
        Assert.All(pemulaTieBreakers, e => Assert.Equal(0, e.DayIndex));

        var mahirPlayerEvents = events
            .Where(e => e.Mode == "MAHIR" && e.ActorType == "PLAYER")
            .OrderBy(e => e.DayIndex)
            .ThenBy(e => e.EventOrder)
            .ThenBy(e => e.ActionType, StringComparer.Ordinal)
            .ThenBy(e => e.PlayerNo ?? 0)
            .ToList();
        var invalidRisks = mahirPlayerEvents
            .Select((e, index) => new { Event = e, Previous = index > 0 ? mahirPlayerEvents[index - 1] : null })
            .Where(pair => pair.Event.ActionType == "RisikoKehidupan"
                && (pair.Previous is null
                    || pair.Previous.ActionType != "JualMasakan"
                    || pair.Previous.PlayerNo != pair.Event.PlayerNo))
            .Select(pair => $"{pair.Event.RefKey ?? "<null>"} day={pair.Event.DayIndex} player={pair.Event.PlayerNo}")
            .ToList();
        Assert.Empty(invalidRisks);
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

    private static void AssertSqlContains(string expected, string actual)
    {
        Assert.Contains(NormalizeSql(expected), NormalizeSql(actual), StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeSql(string sql)
    {
        var normalized = Regex.Replace(sql, @"\s+", " ").Trim();
        normalized = Regex.Replace(normalized, @"\(\s+", "(");
        normalized = Regex.Replace(normalized, @"\s+\)", ")");
        return normalized;
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
            $@"create\s+or\s+replace\s+view\s+{Regex.Escape(viewName)}\s+as(?<body>.*?);",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        Assert.True(match.Success, $"View '{viewName}' should exist in canonical schema.");
        return match.Value;
    }

    private static string ExtractFunction(string sql, string functionName)
    {
        var match = Regex.Match(
            sql,
            $@"create\s+or\s+replace\s+function\s+{Regex.Escape(functionName)}\b[\s\S]*?\n\$\$;",
            RegexOptions.IgnoreCase);

        Assert.True(match.Success, $"Function '{functionName}' should exist in canonical schema.");
        return match.Value;
    }

    private static IEnumerable<SimulationSeedEvent> ParseSimulationSeedEvents(string seedContent)
    {
        const string pattern =
            @"(?s)\(\s*'(?<mode>PEMULA|MAHIR)'\s*,\s*(?<ref>null|'[^']*')\s*,\s*(?<day>-?\d+)\s*,\s*(?<eventOrder>-?\d+)\s*,\s*(?<actionSlot>null|\d+)\s*,\s*(?<playerNo>null|\d+)\s*,\s*'(?<actor>[^']+)'\s*,\s*(?<actionId>null|'[^']*')\s*,\s*'(?<action>[^']+)'\s*,\s*'(?<payload>(?:''|[^'])*)' :: jsonb\s*\)";

        foreach (Match match in Regex.Matches(seedContent, pattern, RegexOptions.IgnoreCase))
        {
            yield return new SimulationSeedEvent(
                match.Groups["mode"].Value.ToUpperInvariant(),
                match.Groups["ref"].Value.Equals("null", StringComparison.OrdinalIgnoreCase)
                    ? null
                    : match.Groups["ref"].Value.Trim('\''),
                int.Parse(match.Groups["day"].Value),
                int.Parse(match.Groups["eventOrder"].Value),
                match.Groups["playerNo"].Value.Equals("null", StringComparison.OrdinalIgnoreCase)
                    ? null
                    : int.Parse(match.Groups["playerNo"].Value),
                match.Groups["actor"].Value.ToUpperInvariant(),
                match.Groups["action"].Value);
        }
    }

    private sealed record SimulationSeedEvent(
        string Mode,
        string? RefKey,
        int DayIndex,
        int EventOrder,
        int? PlayerNo,
        string ActorType,
        string ActionType);
}
