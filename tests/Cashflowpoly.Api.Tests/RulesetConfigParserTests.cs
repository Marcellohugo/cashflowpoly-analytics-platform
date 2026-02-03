using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public class RulesetConfigParserTests
{
    [Fact]
    public void Parse_ruleset_with_scoring_and_freelance_succeeds()
    {
        var json = """
        {
          "mode": "PEMULA",
          "actions_per_turn": 2,
          "starting_cash": 20,
          "weekday_rules": {
            "friday": { "enabled": true },
            "saturday": { "enabled": true },
            "sunday": { "enabled": true }
          },
          "constraints": {
            "cash_min": 0,
            "max_ingredient_total": 6,
            "max_same_ingredient": 3,
            "primary_need_max_per_day": 1,
            "require_primary_before_others": true
          },
          "donation": { "min_amount": 1, "max_amount": 999999 },
          "gold_trade": { "allow_buy": true, "allow_sell": true },
          "advanced": {
            "loan": { "enabled": false },
            "insurance": { "enabled": false },
            "saving_goal": { "enabled": false }
          },
          "freelance": { "income": 1 },
          "scoring": {
            "donation_rank_points": [
              { "rank": 1, "points": 7 },
              { "rank": 2, "points": 5 },
              { "rank": 3, "points": 2 }
            ],
            "gold_points_by_qty": [
              { "qty": 1, "points": 3 },
              { "qty": 2, "points": 5 },
              { "qty": 3, "points": 8 },
              { "qty": 4, "points": 12 }
            ],
            "pension_rank_points": [
              { "rank": 1, "points": 5 },
              { "rank": 2, "points": 3 },
              { "rank": 3, "points": 1 }
            ]
          }
        }
        """;

        var ok = RulesetConfigParser.TryParse(json, out var config, out var errors);

        Assert.True(ok, string.Join(",", errors.Select(e => e.Field)));
        Assert.NotNull(config);
        Assert.Equal(1, config!.FreelanceIncome);
        Assert.NotNull(config.Scoring);
        Assert.Equal(3, config.Scoring!.DonationRankPoints.Count);
        Assert.Equal(4, config.Scoring.GoldPointsByQty.Count);
        Assert.Equal(3, config.Scoring.PensionRankPoints.Count);
    }

    [Fact]
    public void Parse_ruleset_without_scoring_uses_defaults()
    {
        var json = """
        {
          "mode": "PEMULA",
          "actions_per_turn": 2,
          "starting_cash": 20,
          "weekday_rules": {
            "friday": { "enabled": true },
            "saturday": { "enabled": true },
            "sunday": { "enabled": true }
          },
          "constraints": {
            "cash_min": 0,
            "max_ingredient_total": 6,
            "max_same_ingredient": 3,
            "primary_need_max_per_day": 1,
            "require_primary_before_others": true
          },
          "donation": { "min_amount": 1, "max_amount": 999999 },
          "gold_trade": { "allow_buy": true, "allow_sell": true },
          "advanced": {
            "loan": { "enabled": false },
            "insurance": { "enabled": false },
            "saving_goal": { "enabled": false }
          }
        }
        """;

        var ok = RulesetConfigParser.TryParse(json, out var config, out var errors);

        Assert.True(ok, string.Join(",", errors.Select(e => e.Field)));
        Assert.NotNull(config);
        Assert.Equal(1, config!.FreelanceIncome);
        Assert.Null(config.Scoring);
    }
}
