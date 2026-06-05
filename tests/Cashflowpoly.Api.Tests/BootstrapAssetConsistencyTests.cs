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
    }

    [Fact]
    public void CanonicalSchema_ShouldUseCatalogAndJsonDetailTables()
    {
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        var schemaContent = File.ReadAllText(schemaPath);

        Assert.Contains("create table if not exists ruleset_catalog_items", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists ruleset_catalog_item_requirements", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("payload jsonb", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("metric_payload_json jsonb", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists session_player_assets", schemaContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("create table if not exists session_donation_events", schemaContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CanonicalSchema_ShouldNotContainLegacyTypedDetailTables()
    {
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        var schemaContent = File.ReadAllText(schemaPath);

        var forbidden = new[]
        {
            "create table if not exists ruleset_settings",
            "create table if not exists ruleset_features",
            "create table if not exists ruleset_weekday_rules",
            "create table if not exists ruleset_rank_points",
            "create table if not exists ruleset_gold_points",
            "create table if not exists ingredient_cards",
            "create table if not exists order_cards",
            "create table if not exists need_cards",
            "create table if not exists collection_mission_cards",
            "create table if not exists financial_goal_cards",
            "create table if not exists gold_price_cards",
            "create table if not exists gold_cards",
            "create table if not exists donation_rank_cards",
            "create table if not exists pension_rank_cards",
            "create table if not exists tie_breaker_cards",
            "create table if not exists sharia_loan_cards",
            "create table if not exists insurance_cards",
            "create table if not exists life_risk_cards",
            "create table if not exists event_buy_ingredient_details",
            "create table if not exists event_sell_order_details",
            "create table if not exists event_buy_need_details",
            "create table if not exists event_freelance_details",
            "create table if not exists event_gold_trade_details",
            "create table if not exists event_turn_details",
            "create table if not exists metric_snapshot_fields",
            "create table if not exists validation_rule_results",
            "create table if not exists session_action_log_fields",
            "create table if not exists security_audit_log_fields",
            "create table if not exists session_player_gold",
            "create table if not exists session_player_loans",
            "create table if not exists session_player_insurances",
            "create table if not exists session_board_slots",
            "create table if not exists peduli_donasi_events",
            "create table if not exists peduli_donasi_rankings"
        };

        foreach (var entry in forbidden)
        {
            Assert.DoesNotContain(entry, schemaContent, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void DefaultRulesetSeed_ShouldDeclareUnifiedRulesetDataForBothModes()
    {
        var seedPath = Path.Combine(RepoRoot, "database", "01_seed_default_rulesets_components.sql");
        var seedContent = File.ReadAllText(seedPath);

        Assert.Contains("system-seed-relational-v2", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into ruleset_catalog_items", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("insert into ruleset_catalog_item_requirements", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""starting_cash"": 20", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""starting_cash"": 10", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(@"""actions_per_turn"": 2", seedContent, StringComparison.OrdinalIgnoreCase);
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
        var pensionPointBlocks = Regex.Matches(seedContent, @"""score_source""\s*:\s*""PENSION""\s*,\s*""rank""\s*:\s*(?<rank>\d)\s*,\s*""points""\s*:\s*(?<points>\d+)", RegexOptions.IgnoreCase)
            .Select(match => $"{match.Groups["rank"].Value}:{match.Groups["points"].Value}")
            .ToList();
        var missionPenaltyValues = Regex.Matches(seedContent, @"""success_points""\s*:\s*(?<success>-?\d+)\s*,\s*""failure_points""\s*:\s*(?<failure>-?\d+)", RegexOptions.IgnoreCase)
            .Select(match => $"{match.Groups["success"].Value}:{match.Groups["failure"].Value}")
            .ToList();

        Assert.NotEmpty(startingCashValues);
        Assert.NotEmpty(actionsPerTurnValues);
        Assert.NotEmpty(pensionPointBlocks);
        Assert.NotEmpty(missionPenaltyValues);
        Assert.Contains(20, startingCashValues);
        Assert.Contains(10, startingCashValues);
        Assert.All(startingCashValues, value => Assert.Contains(value, new[] { 10, 20 }));
        Assert.All(actionsPerTurnValues, value => Assert.Equal(2, value));
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
        Assert.Contains("[SEED] Simulasi Pemula", seedContent, StringComparison.Ordinal);
        Assert.Contains("[SEED] Simulasi Mahir", seedContent, StringComparison.Ordinal);
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
}
