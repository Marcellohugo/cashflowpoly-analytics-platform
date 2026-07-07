using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class RulesetDefinitionMapperTests
{
    [Fact]
    public void FromConfigJson_IgnoresRemovedQuestAndScripts_AndKeepsDataDrivenNarrativeConditions()
    {
        const string configJson = """
            {
              "mode": "MAHIR",
              "actions_per_turn": 2,
              "starting_cash": 10,
              "player_ordering": "PLAYER_ORDER",
              "weekday_rules": {
                "friday": { "feature": "DONATION", "enabled": true },
                "saturday": { "feature": "GOLD_TRADE", "enabled": true },
                "sunday": { "feature": "REST", "enabled": true }
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
                "loan": { "enabled": true },
                "insurance": { "enabled": true },
                "saving_goal": { "enabled": true }
              },
              "component_catalog": {
                "gameConfig": {
                  "initialCoins": 10,
                  "initialHappiness": 0,
                  "initialSaving": 0,
                  "finishDay": 25,
                  "minPlayers": 2,
                  "maxPlayers": 4
                },
                "bahan": [],
                "resep": [],
                "kebutuhan": [],
                "targetKebutuhan": [],
                "tujuanFinansial": [],
                "narasi": [
                  {
                    "id": "jual_pertama",
                    "nama": "Penjualan Pertama",
                    "teks": [ "baris 1" ],
                    "prerequisiteAksi": [ { "aksi": "JualMasakan", "value": 1 } ],
                    "scripts": [ { "command": "setbackground" } ]
                  }
                ],
                "quest": [
                  {
                    "id": "jual_3",
                    "nama": "Jual tiga masakan",
                    "aksi": "JualMasakan",
                    "target": 3,
                    "rewardCoins": 5,
                    "rewardHappiness": 2,
                    "scripts": [ { "command": "checkAction" } ]
                  }
                ]
              }
            }
            """;

        var definition = RulesetDefinitionMapper.FromConfigJson(configJson);

        var narrative = Assert.Single(definition.Narratives);
        var prerequisite = Assert.Single(narrative.PrerequisiteAksi);
        Assert.Equal("JualMasakan", prerequisite.Aksi);
        Assert.Equal(1, prerequisite.Value);
    }

    [Fact]
    public void ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog()
    {
        var definition = new RulesetDefinitionDto
        {
            Mode = "MAHIR",
            Settings = new RulesetSettingsDto
            {
                ActionsPerTurn = 2,
                StartingCash = 10,
                InitialCoins = 10,
                FinishDay = 25,
                MinPlayers = 2,
                MaxPlayers = 4
            },
            Narratives =
            [
                new RulesetNarrativeDto
                {
                    Id = "jual_pertama",
                    Nama = "Penjualan Pertama",
                    Teks = ["baris 1"],
                    PrerequisiteAksi =
                    [
                        new RulesetNarrativePrerequisiteDto
                        {
                            Aksi = "JualMasakan",
                            Value = 1
                        }
                    ]
                }
            ]
        };

        using var document = JsonDocument.Parse(RulesetDefinitionMapper.ToConfigJson(definition));
        var root = document.RootElement;
        var catalog = root.GetProperty("component_catalog");
        var narrative = catalog.GetProperty("narasi")[0];

        Assert.False(root.TryGetProperty("interpreter_commands", out _));
        Assert.False(root.TryGetProperty("narrative_assets", out _));
        Assert.False(narrative.TryGetProperty("scripts", out _));
        Assert.False(catalog.TryGetProperty("quest", out _));
        Assert.Equal("JualMasakan", narrative.GetProperty("prerequisiteAksi")[0].GetProperty("aksi").GetString());
    }
}
