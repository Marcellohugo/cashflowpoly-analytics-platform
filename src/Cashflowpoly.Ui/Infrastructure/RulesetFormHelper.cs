// Fungsi file: Menyediakan helper formulir ruleset untuk default config, format JSON, mode resolver, dan pesan error API.
using System.Text.Json;
using System.Text.Json.Nodes;
using Cashflowpoly.Contracts;
using Cashflowpoly.Ui.Models;

namespace Cashflowpoly.Ui.Infrastructure;

public static class RulesetFormHelper
{
    public static CreateRulesetViewModel BuildDefaultCreateViewModel()
    {
        return new CreateRulesetViewModel
        {
            IsEditMode = false,
            ConfigJson = """
            {
              "mode": "PEMULA",
              "actions_per_turn": 2,
              "starting_cash": 20,
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
            """
        };
    }

    public static bool TryResolveMode(JsonObject configObject, out string mode)
    {
        mode = string.Empty;
        if (!configObject.TryGetPropertyValue("mode", out var modeNode))
        {
            return false;
        }

        if (modeNode is not JsonValue modeValue || !modeValue.TryGetValue<string>(out var rawMode))
        {
            return false;
        }

        var modeText = rawMode?.Trim().ToUpperInvariant();
        if (!string.Equals(modeText, "PEMULA", StringComparison.Ordinal) &&
            !string.Equals(modeText, "MAHIR", StringComparison.Ordinal))
        {
            return false;
        }

        mode = modeText!;
        return true;
    }

    public static async Task<string> BuildRulesetApiErrorMessage(HttpResponseMessage response, string prefix, CancellationToken ct)
    {
        var error = await response.Content.TryReadFromJsonAsync<ErrorResponse>(ct);
        return error?.Message ?? $"{prefix}. Status: {(int)response.StatusCode}";
    }

    public static string SerializeIndentedJson(JsonElement? configJson)
    {
        if (!configJson.HasValue)
        {
            return "{}";
        }

        try
        {
            return JsonSerializer.Serialize(configJson.Value, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
        catch (JsonException)
        {
            return "{}";
        }
    }
}
