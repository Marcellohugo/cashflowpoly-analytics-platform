using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class InspectionSeedConsolidationTests
{
    private static readonly string RepoRoot = ResolveRepositoryRoot();

    [Fact]
    public void InspectionSeed_ShouldBeSingleConsolidatedAsset()
    {
        var databaseDir = Path.Combine(RepoRoot, "database");
        var fullSeedPath = Path.Combine(databaseDir, "02_seed_full_inspection.sql");
        var matrixSeedPath = Path.Combine(databaseDir, "03_seed_inspection_matrix.sql");
        var initializerPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Infrastructure", "DatabaseInitialization.cs");
        var apiProjectPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Cashflowpoly.Api.csproj");
        var apiTestProjectPath = Path.Combine(RepoRoot, "tests", "Cashflowpoly.Api.Tests", "Cashflowpoly.Api.Tests.csproj");

        Assert.True(File.Exists(fullSeedPath), "Seed inspeksi gabungan harus tetap tersedia sebagai 02_seed_full_inspection.sql.");
        Assert.False(File.Exists(matrixSeedPath), "Seed matriks lama harus digabung ke 02_seed_full_inspection.sql, bukan dipasang sebagai file terpisah.");
        Assert.DoesNotContain("03_seed_inspection_matrix.sql", File.ReadAllText(initializerPath), StringComparison.Ordinal);
        Assert.DoesNotContain("03_seed_inspection_matrix.sql", File.ReadAllText(apiProjectPath), StringComparison.Ordinal);
        Assert.DoesNotContain("03_seed_inspection_matrix.sql", File.ReadAllText(apiTestProjectPath), StringComparison.Ordinal);
    }

    [Fact]
    public void InspectionSeed_ShouldCoverPhysicalAndDerivedMetricsFromReferenceMarkdown()
    {
        var seedPath = Path.Combine(RepoRoot, "database", "02_seed_full_inspection.sql");
        var seedContent = File.ReadAllText(seedPath);
        var requiredSignals = new[]
        {
            "starting_coins",
            "coins_held_current",
            "coins_spent_per_turn",
            "coins_earned_per_turn",
            "ingredients_wasted",
            "meal_orders_available_passed",
            "business_efficiency_ratio",
            "need_fulfillment_diversity_index",
            "donation_consistency_score",
            "gold_roi_percentage",
            "pension_fund_total",
            "life_risk_mitigated_with_insurance",
            "financial_goals_incomplete_coins_wasted",
            "debt_leverage_ratio",
            "action_diversity_score",
            "actions_skipped",
            "growth_pattern",
            "happiness_portfolio"
        };

        foreach (var signal in requiredSignals)
        {
            Assert.Contains(signal, seedContent, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void InspectionSeed_ShouldUpsertSeedUsersAndPlayersWithoutDeletingReusableAccounts()
    {
        var seedPath = Path.Combine(RepoRoot, "database", "02_seed_full_inspection.sql");
        var seedContent = File.ReadAllText(seedPath);

        Assert.DoesNotMatch(
            new Regex(@"\bdelete\s+from\s+players\b", RegexOptions.IgnoreCase),
            seedContent);
        Assert.DoesNotMatch(
            new Regex(@"\bdelete\s+from\s+app_users\b", RegexOptions.IgnoreCase),
            seedContent);
        Assert.Contains("on conflict (user_id) do update", seedContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("on conflict (player_id) do update", seedContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void InspectionSeed_MetricSnapshotsShouldReferenceSeededSessionsPlayersAndRulesetVersions()
    {
        var seedPath = Path.Combine(RepoRoot, "database", "02_seed_full_inspection.sql");
        var seedContent = File.ReadAllText(seedPath);
        var sessionIds = ExtractInsertedIds(seedContent, "sessions");
        var playerIds = ExtractInsertedIds(seedContent, "players");
        var rulesetVersionIds = ExtractSelectedRulesetVersionIds(seedContent);
        var metricRows = ExtractInsertRows(seedContent, "metric_snapshots");

        Assert.NotEmpty(metricRows);

        foreach (var row in metricRows)
        {
            var fields = SplitTopLevelFields(row);
            if (fields.Count < 8 || !fields[4].TrimStart().StartsWith('\''))
            {
                continue;
            }

            var snapshotId = TrimSqlLiteral(fields[0]);
            var sessionId = TrimSqlLiteral(fields[1]);
            var playerId = fields[2].Trim();
            var rulesetVersionId = TrimSqlLiteral(fields[7]);

            Assert.Contains(sessionId, sessionIds);
            Assert.Contains(rulesetVersionId, rulesetVersionIds);
            if (!string.Equals(playerId, "null", StringComparison.OrdinalIgnoreCase))
            {
                Assert.Contains(TrimSqlLiteral(playerId), playerIds);
            }

            Assert.False(string.IsNullOrWhiteSpace(snapshotId));
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

    private static HashSet<string> ExtractInsertedIds(string sql, string tableName)
    {
        return ExtractInsertRows(sql, tableName)
            .Select(row => TrimSqlLiteral(SplitTopLevelFields(row)[0]))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static HashSet<string> ExtractSelectedRulesetVersionIds(string sql)
    {
        return Regex.Matches(
                sql,
                @"insert\s+into\s+ruleset_versions\s*\([\s\S]*?\)\s*select\s*'(?<id>[0-9a-f]{8}-[0-9a-f-]{27})'",
                RegexOptions.IgnoreCase)
            .Select(match => match.Groups["id"].Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static List<string> ExtractInsertRows(string sql, string tableName)
    {
        var escapedTableName = Regex.Escape(tableName);
        var blocks = Regex.Matches(
            sql,
            $@"insert\s+into\s+{escapedTableName}\s*\([\s\S]*?\)\s*values(?<values>[\s\S]*?)on\s+conflict",
            RegexOptions.IgnoreCase);
        var rows = new List<string>();
        foreach (Match block in blocks)
        {
            rows.AddRange(ExtractTopLevelTuples(block.Groups["values"].Value));
        }

        return rows;
    }

    private static List<string> ExtractTopLevelTuples(string valuesSql)
    {
        var rows = new List<string>();
        var current = new StringBuilder();
        var depth = 0;
        var inString = false;

        for (var i = 0; i < valuesSql.Length; i++)
        {
            var c = valuesSql[i];
            if (c == '\'' && (i + 1 >= valuesSql.Length || valuesSql[i + 1] != '\''))
            {
                inString = !inString;
            }
            else if (c == '\'' && i + 1 < valuesSql.Length && valuesSql[i + 1] == '\'')
            {
                if (depth > 0)
                {
                    current.Append(c);
                    current.Append(valuesSql[i + 1]);
                }

                i++;
                continue;
            }

            if (!inString && c == '(')
            {
                depth++;
                if (depth == 1)
                {
                    current.Clear();
                    continue;
                }
            }

            if (!inString && c == ')')
            {
                depth--;
                if (depth == 0)
                {
                    rows.Add(current.ToString());
                    current.Clear();
                    continue;
                }
            }

            if (depth > 0)
            {
                current.Append(c);
            }
        }

        return rows;
    }

    private static List<string> SplitTopLevelFields(string row)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        var depth = 0;
        var inString = false;

        for (var i = 0; i < row.Length; i++)
        {
            var c = row[i];
            if (c == '\'' && (i + 1 >= row.Length || row[i + 1] != '\''))
            {
                inString = !inString;
            }
            else if (c == '\'' && i + 1 < row.Length && row[i + 1] == '\'')
            {
                current.Append(c);
                current.Append(row[i + 1]);
                i++;
                continue;
            }

            if (!inString)
            {
                if (c == '(')
                {
                    depth++;
                }
                else if (c == ')')
                {
                    depth--;
                }
                else if (c == ',' && depth == 0)
                {
                    fields.Add(current.ToString().Trim());
                    current.Clear();
                    continue;
                }
            }

            current.Append(c);
        }

        fields.Add(current.ToString().Trim());
        return fields;
    }

    private static string TrimSqlLiteral(string value)
    {
        return value.Trim().Trim('\'');
    }
}
