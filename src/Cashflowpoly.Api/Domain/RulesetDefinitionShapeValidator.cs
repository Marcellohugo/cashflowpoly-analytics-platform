// Fungsi file: Menolak koleksi dan elemen null sebelum definisi ruleset dipakai.
using Cashflowpoly.Api.Contracts;

namespace Cashflowpoly.Api.Domain;

internal static class RulesetDefinitionShapeValidator
{
    internal static List<ErrorDetail> Validate(RulesetDefinitionDto definition)
    {
        var errors = new List<ErrorDetail>();
        if (definition.Settings is null) errors.Add(new("definition.settings", "REQUIRED"));
        if (definition.PlayerOrdering is null) errors.Add(new("definition.player_ordering", "REQUIRED"));

        Check(definition.Actions, "actions");
        Check(definition.Ingredients, "ingredients");
        Check(definition.Orders, "orders");
        Check(definition.Needs, "needs");
        Check(definition.NeedSetBonuses, "need_set_bonuses");
        Check(definition.CollectionMissions, "collection_missions");
        Check(definition.FinancialGoals, "financial_goals");
        Check(definition.Narratives, "narratives");
        Check(definition.DonationRankPoints, "donation_rank_points");
        Check(definition.GoldPointsByQty, "gold_points_by_qty");
        Check(definition.GoldPrices, "gold_prices");
        Check(definition.PensionRankPoints, "pension_rank_points");
        Check(definition.TieBreakers, "tie_breakers");
        Check(definition.ShariaLoans, "sharia_loans");
        Check(definition.InsuranceProducts, "insurance_products");
        Check(definition.LifeRisks, "life_risks");
        if (errors.Count > 0) return errors;

        for (var i = 0; i < definition.Orders.Count; i++)
            Check(definition.Orders[i].Bahan, $"orders[{i}].bahan");
        for (var i = 0; i < definition.CollectionMissions.Count; i++)
        {
            Check(definition.CollectionMissions[i].KebutuhanTarget, $"collection_missions[{i}].kebutuhanTarget");
            var mission = definition.CollectionMissions[i];
            var field = $"definition.collection_missions[{i}]";
            if (mission.SuccessPoints < 0) errors.Add(new($"{field}.success_points", "OUT_OF_RANGE"));
            if (mission.FailurePoints > 0 || mission.FailurePoints == int.MinValue)
                errors.Add(new($"{field}.failure_points", "OUT_OF_RANGE"));
            if (mission.PenaltyPoints < 0) errors.Add(new($"{field}.penaltyPoints", "OUT_OF_RANGE"));
            if (mission.FailurePoints != 0 && mission.PenaltyPoints != 0 &&
                (long)mission.FailurePoints != -(long)mission.PenaltyPoints)
                errors.Add(new($"{field}.failure_points", "MUST_MATCH_NEGATIVE_PENALTY"));
        }
        for (var i = 0; i < definition.Narratives.Count; i++)
        {
            Check(definition.Narratives[i].Teks, $"narratives[{i}].teks");
            Check(definition.Narratives[i].PrerequisiteAksi, $"narratives[{i}].prerequisiteAksi");
        }
        return errors;

        void Check<T>(IReadOnlyList<T>? values, string field)
        {
            if (values is null)
            {
                errors.Add(new($"definition.{field}", "REQUIRED"));
                return;
            }
            for (var i = 0; i < values.Count; i++)
                if (values[i] is null) errors.Add(new($"definition.{field}[{i}]", "REQUIRED"));
        }
    }
}
