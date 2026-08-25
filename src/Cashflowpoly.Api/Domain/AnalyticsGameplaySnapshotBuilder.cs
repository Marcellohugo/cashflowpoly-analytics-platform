// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsGameplaySnapshotBuilder.
using System.Text.Json;
using System.Text.Json.Nodes;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Contracts;
using static Cashflowpoly.Api.Domain.AnalyticsMath;

namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsGameplaySnapshot(string RawJson, string DerivedJson);

internal sealed class GameplaySnapshotBuilder : IGameplaySnapshotBuilder
{
    private static readonly AnalyticsPayloadReader _payloadReader = new();
    private static readonly CashTimelineCalculator _cashTimeline = new();
    private static readonly DonationGameplayCalculator _donationCalc = new();
    private static readonly SavingGoalCalculator _savingGoalCalc = new();
    private static readonly IngredientMealCalculator _ingredientMealCalc = new();
    private static readonly NeedMissionCalculator _needMissionCalc = new();
    private static readonly GoldGameplayCalculator _goldCalc = new();
    private static readonly RiskLoanCalculator _riskLoanCalc = new();
    private static readonly ActionUsageCalculator _actionUsageCalc = new();
    private static readonly IncomeDiversificationCalculator _incomeDivCalc = new();

    public AnalyticsGameplaySnapshot Build(
        List<EventDb> playerEvents,
        List<CashflowProjectionDb> playerProjections,
        List<EventDb> allEvents,
        RulesetConfig? config,
        AnalyticsHappinessBreakdown happiness,
        SessionFinalScoreDb? finalScore = null,
        int? pensionRank = null,
        string? playerAlias = null,
        bool sessionEnded = false)
    {
        var notesRaw = new List<string>();
        var notesDerived = new List<string>();

        var cashTimeline = _cashTimeline.Compute(
            playerEvents,
            playerProjections,
            config?.StartingCash ?? 0);
        var startingCoins = cashTimeline.StartingCoins;
        var cashInTotal = cashTimeline.CashInTotal;
        var cashOutTotal = cashTimeline.CashOutTotal;
        var coinsNetEndGame = cashTimeline.CoinsNetEndGame;
        var coinsHeldCurrent = cashTimeline.CoinsHeldCurrent;

        var donationMetrics = _donationCalc.Compute(playerEvents, allEvents, coinsNetEndGame);
        var donationTotal = donationMetrics.DonationTotalCoins;

        var savingGoalMetrics = _savingGoalCalc.Compute(playerEvents, config?.FinancialGoals.Count);
        var coinsSaved = savingGoalMetrics.CoinsSaved;

        var ingredientMealMetrics = _ingredientMealCalc.Compute(playerEvents, playerProjections);
        var inventory = ingredientMealMetrics.Inventory;
        var ingredientsCollected = ingredientMealMetrics.IngredientsCollected;
        var ingredientTypesHeld = ingredientMealMetrics.IngredientTypesHeld;
        var ingredientsUsedPerMeal = ingredientMealMetrics.IngredientsUsedPerMeal;
        var ingredientsUsedTotal = ingredientMealMetrics.IngredientsUsedTotal;
        var ingredientsWasted = ingredientMealMetrics.IngredientsWasted;
        var ingredientInvestmentTotal = ingredientMealMetrics.IngredientInvestmentTotal;
        var mealOrderIncomeValues = ingredientMealMetrics.MealOrderIncomeValues;
        var mealOrdersClaimed = ingredientMealMetrics.MealOrdersClaimed;
        var mealOrderIncomeTotal = ingredientMealMetrics.MealOrderIncomeTotal;
        var mealOrdersPerTurnAverage = ingredientMealMetrics.MealOrdersPerTurnAverage;
        var essentialIngredientExpenses = ingredientMealMetrics.EssentialIngredientExpenses;
        var latestDayIndex = ingredientMealMetrics.LatestDayIndex;

        var needMissionMetrics = _needMissionCalc.Compute(playerEvents, playerProjections);

        var goldMetrics = _goldCalc.Compute(playerEvents);
        var goldInvestmentEarned = goldMetrics.GoldInvestmentEarned;
        var goldInvestmentSpent = goldMetrics.GoldInvestmentSpent;
        var goldInvestmentNet = goldMetrics.GoldInvestmentNet;

        var pensionRankFromEvent = playerEvents
            .Where(e => e.ActionType == "PoinPeringkatPensiun")
            .Select(e => _payloadReader.TryReadRankAwarded(e.Payload, out var rank, out _) ? rank : 0)
            .FirstOrDefault(rank => rank > 0);
        pensionRank ??= pensionRankFromEvent > 0
            ? pensionRankFromEvent
            : config?.Scoring?.PensionRankPoints
                .Where(item => Math.Abs(item.Points - happiness.PensionPoints) < 0.000001)
                .Select(item => (int?)item.Rank)
                .FirstOrDefault();

        var riskLoanMetrics = _riskLoanCalc.Compute(
            playerEvents,
            playerProjections,
            startingCoins,
            coinsNetEndGame,
            cashInTotal);

        var actionMetrics = _actionUsageCalc.Compute(
            playerEvents,
            playerProjections,
            latestDayIndex,
            config?.ActionsPerTurn ?? 2);
        var latestEvent = playerEvents
            .OrderByDescending(e => e.SequenceNumber)
            .FirstOrDefault();
        var sessionId = playerEvents
            .Select(e => (Guid?)e.SessionId)
            .FirstOrDefault()
            ?? allEvents.Select(e => (Guid?)e.SessionId).FirstOrDefault();
        var resolvedUserId = playerEvents
            .Select(e => e.UserId)
            .FirstOrDefault(id => id.HasValue);
        var gameMode = string.Equals(config?.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase)
            ? "advanced"
            : "beginner";
        var hasSessionEndEvent = allEvents.Any(e => e.ActionType == "AkhiriSesi");
        var sessionCompleted = sessionEnded || hasSessionEndEvent;
        var finishLineReached = latestDayIndex >= (config?.FinishDay ?? 25);
        int? finalRank = finalScore?.Rank;
        bool? winnerFlag = finalRank.HasValue ? finalRank.Value == 1 : null;
        bool? dnfFlag = sessionCompleted ? !finishLineReached : null;

        var raw = new
        {
            metadata = new
            {
                game_id = sessionId,
                session_id = sessionId,
                user_id = resolvedUserId,
                player_alias = playerAlias,
                game_mode = gameMode,
                latest_event_action_slot = latestEvent?.ActionSlot,
                day_label = latestEvent?.Weekday,
                action_slot = actionMetrics.LatestActionSlot,
                action_slot_timeline = actionMetrics.ActionSlotTimeline,
                event_timestamp = latestEvent?.Timestamp
            },
            coins = new
            {
                starting_coins = startingCoins,
                coins_held_current = coinsHeldCurrent,
                coins_spent_per_turn = cashTimeline.CoinsSpentPerTurn,
                coins_earned_per_turn = cashTimeline.CoinsEarnedPerTurn,
                coins_donated = donationTotal,
                coins_saved = coinsSaved,
                coins_net_end_game = coinsNetEndGame
            },
            ingredients = new
            {
                ingredients_collected = ingredientsCollected,
                ingredients_held_current = inventory.Total,
                ingredient_types_held = ingredientTypesHeld,
                ingredients_used_per_meal = ingredientsUsedPerMeal,
                ingredients_used_total = ingredientsUsedTotal,
                ingredients_used_per_meal_average = mealOrdersClaimed > 0
                    ? (double)ingredientsUsedTotal / mealOrdersClaimed
                    : (double?)null,
                ingredients_wasted = ingredientsWasted,
                ingredient_investment_coins_total = ingredientInvestmentTotal
            },
            meal_orders = new
            {
                meal_orders_claimed = mealOrdersClaimed,
                meal_order_income_per_order = mealOrderIncomeValues,
                meal_order_income_total = mealOrderIncomeTotal,
                meal_orders_per_turn_average = mealOrdersPerTurnAverage
            },
            needs = new
            {
                need_cards_purchased = needMissionMetrics.NeedCardsPurchased,
                need_cards_owned_current = needMissionMetrics.NeedCardsOwnedCurrent,
                primary_needs_owned = needMissionMetrics.PrimaryNeeds,
                secondary_needs_owned = needMissionMetrics.SecondaryNeeds,
                tertiary_needs_owned = needMissionMetrics.TertiaryNeeds,
                need_profile = new
                {
                    basic_profile = needMissionMetrics.HasBasicNeedProfile,
                    collector_profile = needMissionMetrics.IsCollectorNeedProfile,
                    specialist_profile = needMissionMetrics.IsSpecialistNeedProfile
                },
                specific_tertiary_need = needMissionMetrics.SpecificTertiaryAcquired,
                collection_mission_complete = needMissionMetrics.CollectionMissionComplete,
                need_cards_coins_spent = needMissionMetrics.NeedCoinsSpent
            },
            donations = new
            {
                donation_amount_per_friday = donationMetrics.DonationAmountPerFriday,
                donation_rank_per_friday = donationMetrics.DonationRankPerFriday,
                donation_total_coins = donationTotal,
                donation_champion_cards_earned = donationMetrics.DonationChampionCardsEarned,
                donation_happiness_points = happiness.DonationPoints
            },
            gold = new
            {
                gold_cards_purchased = goldMetrics.GoldBuyQty,
                gold_cards_sold = goldMetrics.GoldSellQty,
                gold_cards_held_end = goldMetrics.GoldHeldEnd,
                gold_prices_per_purchase = goldMetrics.GoldPurchasePrices,
                gold_price_per_sale = goldMetrics.GoldSalePrices,
                gold_investment_coins_spent = goldInvestmentSpent,
                gold_investment_coins_earned = goldInvestmentEarned,
                gold_investment_net = goldInvestmentNet
            },
            pension = new
            {
                leftover_coins_end_game = coinsHeldCurrent,
                ingredient_cards_value_end = inventory.Total,
                coins_in_savings_goal = coinsSaved,
                pension_fund_total = coinsHeldCurrent + inventory.Total + coinsSaved,
                pension_fund_rank_per_game = pensionRank is null or 0 ? (int?)null : pensionRank,
                pension_fund_happiness_points = happiness.PensionPoints
            },
            life_risk = new
            {
                life_risk_cards_drawn = riskLoanMetrics.RiskCardsDrawn,
                life_risk_costs_per_card = riskLoanMetrics.RiskCostsPerCard,
                life_risk_costs_total = riskLoanMetrics.RiskCostsTotal,
                life_risk_mitigated_with_insurance = riskLoanMetrics.RiskMitigated,
                insurance_payments_made = riskLoanMetrics.InsurancePayments,
                emergency_options_used = riskLoanMetrics.EmergencyOptionsUsed
            },
            financial_goals = new
            {
                financial_goals_attempted = savingGoalMetrics.FinancialGoalsAttempted,
                financial_goals_completed = savingGoalMetrics.FinancialGoalsCompleted,
                financial_goals_coins_per_goal = savingGoalMetrics.SavingGoalCostsByGoal,
                financial_goals_balance_per_goal = savingGoalMetrics.SavingBalancesByGoal,
                financial_goals_coins_total_invested = savingGoalMetrics.FinancialGoalsCoinsTotalInvested,
                financial_goals_incomplete_coins_wasted = savingGoalMetrics.FinancialGoalsIncompleteCoinsWasted,
                sharia_loans_taken = riskLoanMetrics.LoansTaken,
                sharia_loans_repaid = riskLoanMetrics.LoansRepaid,
                sharia_loans_unpaid_end = riskLoanMetrics.LoansUnpaid,
                sharia_loans_outstanding_coins = riskLoanMetrics.LoansOutstandingAmount,
                loan_penalty_if_unpaid = happiness.LoanPenaltyPoints
            },
            actions = new
            {
                actions_per_turn = config?.ActionsPerTurn ?? 2,
                action_repetitions_per_turn = actionMetrics.ActionRepetitions,
                action_sequence = actionMetrics.ActionSequences
            },
            turns = new
            {
                coins_per_turn_progression = cashTimeline.CoinsProgression,
                net_income_per_turn = cashTimeline.NetIncomePerTurn,
                day_when_debt_introduced = playerEvents
                    .Where(e => e.ActionType == "PinjamanSyariah")
                    .Select(e => (int?)e.DayIndex)
                    .OrderBy(t => t)
                    .FirstOrDefault(),
                day_when_first_risk_hit = playerEvents
                    .Where(e => e.ActionType == "RisikoKehidupan")
                    .Select(e => (int?)e.DayIndex)
                    .OrderBy(t => t)
                    .FirstOrDefault(),
                day_game_completion = latestDayIndex < 0 ? (int?)null : latestDayIndex
            },
            outcomes = new
            {
                total_happiness_points = happiness.Total,
                final_rank = finalRank,
                winner_flag = winnerFlag,
                finish_line_reached = finishLineReached,
                dnf_flag = dnfFlag
            },
            notes = notesRaw
        };

        var totalExpenses = cashOutTotal;
        var cashGrowthPercent = SafeRatio(coinsNetEndGame, startingCoins, true);
        var incomeDiversificationMetrics = _incomeDivCalc.Compute(
            playerEvents,
            mealOrderIncomeTotal,
            goldInvestmentEarned);
        if (incomeDiversificationMetrics.RequiresIncomeNote)
        {
            notesDerived.Add("income_diversification_requires_income");
        }
        var incomeDiversificationIndex = incomeDiversificationMetrics.IncomeDiversificationIndex;
        var businessExpenseSharePercent = SafeRatio(ingredientInvestmentTotal, totalExpenses, true);
        var mealOrderProfitMarginPercent = SafeRatio(
            mealOrderIncomeTotal - essentialIngredientExpenses,
            mealOrderIncomeTotal,
            true);
        var incomeActionFocusPercent = actionMetrics.ActionEfficiencyPercent;
        var ingredientUtilizationPercent = SafeRatio(ingredientsUsedTotal, ingredientsCollected, true);
        var primaryNeedShare = SafeRatio(needMissionMetrics.PrimaryNeeds, needMissionMetrics.NeedCardsOwnedCurrent);
        var secondaryNeedShare = SafeRatio(needMissionMetrics.SecondaryNeeds, needMissionMetrics.NeedCardsOwnedCurrent);
        var tertiaryNeedShare = SafeRatio(needMissionMetrics.TertiaryNeeds, needMissionMetrics.NeedCardsOwnedCurrent);
        var needFulfillmentDiversityPercent = needMissionMetrics.FulfillmentDiversity.HasValue
            ? needMissionMetrics.FulfillmentDiversity.Value * 100
            : (double?)null;
        var donationResourceShare = SafeRatio(
            donationTotal,
            Math.Max(0, coinsNetEndGame) + donationTotal);
        var donationCommitmentScore = donationMetrics.DonationStabilityIndex.HasValue &&
                                      donationResourceShare.HasValue &&
                                      donationMetrics.FridayParticipationRate.HasValue
            ? Clamp(
                donationMetrics.DonationStabilityIndex.Value *
                donationResourceShare.Value *
                donationMetrics.FridayParticipationRate.Value,
                0,
                100)
            : (double?)null;

        var derived = new Dictionary<string, object?>
        {
            ["cash_growth_percent"] = cashGrowthPercent,
            ["cash_growth_components"] = new
            {
                coins_net_end_game = coinsNetEndGame,
                starting_coins = startingCoins
            },
            ["income_diversification_index"] = incomeDiversificationIndex,
            ["income_diversification_components"] = new
            {
                freelance_income = incomeDiversificationMetrics.FreelanceIncome,
                meal_order_income = incomeDiversificationMetrics.MealIncome,
                gold_sale_income = incomeDiversificationMetrics.GoldIncome,
                active_income_source_count = incomeDiversificationMetrics.ActiveIncomeSourceCount,
                income_shares = incomeDiversificationMetrics.IncomeShares
            },
            ["business_expense_share_percent"] = businessExpenseSharePercent,
            ["business_expense_share_components"] = new
            {
                ingredient_investment_coins_total = ingredientInvestmentTotal,
                total_cash_out = totalExpenses
            },
            ["meal_order_profit_margin_percent"] = mealOrderProfitMarginPercent,
            ["meal_order_profit_margin_components"] = new
            {
                meal_order_income_total = mealOrderIncomeTotal,
                ingredient_cost_used = essentialIngredientExpenses
            },
            ["income_action_focus_percent"] = incomeActionFocusPercent,
            ["income_action_focus_components"] = new
            {
                income_main_actions = actionMetrics.IncomeActions,
                total_main_actions = actionMetrics.ActionEventCount
            },
            ["ingredient_utilization_percent"] = ingredientUtilizationPercent,
            ["ingredient_utilization_components"] = new
            {
                ingredients_used_in_completed_orders = ingredientsUsedTotal,
                ingredients_collected = ingredientsCollected
            },
            ["need_fulfillment_diversity_percent"] = needFulfillmentDiversityPercent,
            ["need_fulfillment_diversity_components"] = new
            {
                primary_need_share = primaryNeedShare,
                secondary_need_share = secondaryNeedShare,
                tertiary_need_share = tertiaryNeedShare
            },
            ["donation_commitment_score"] = donationCommitmentScore,
            ["donation_commitment_components"] = new
            {
                donation_stability_index = donationMetrics.DonationStabilityIndex,
                donated_resource_share = donationResourceShare,
                friday_participation_rate = donationMetrics.FridayParticipationRate
            },
            ["happiness_points_composition"] = new
            {
                total_happiness_points = happiness.Total,
                need_card_points = happiness.NeedPoints,
                need_set_bonus_points = happiness.NeedSetBonusPoints,
                donation_points = happiness.DonationPoints,
                gold_points = happiness.GoldPoints,
                pension_points = happiness.PensionPoints,
                financial_goal_points = happiness.SavingGoalPointsEffective,
                mission_penalty_points = 0 - happiness.MissionPenaltyPoints,
                loan_penalty_points = 0 - happiness.LoanPenaltyPoints
            },
            ["notes"] = notesDerived
        };

        if (string.Equals(config?.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase))
        {
            var risksResolvedWithoutEmergency = Math.Max(0, riskLoanMetrics.RiskCardsDrawn - riskLoanMetrics.EmergencyOptionsUsed);
            var liquidAssets = Math.Max(0, coinsHeldCurrent) + Math.Max(0, coinsSaved);
            var attemptedGoalIds = savingGoalMetrics.SavingDepositsByGoal.Keys
                .Concat(savingGoalMetrics.SavingGoalsAchieved)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var attemptedGoalTargetTotal = config!.FinancialGoals
                .Where(goal => attemptedGoalIds.Contains(goal.Id))
                .Sum(goal => goal.HargaBeli);
            var savingsActionCount = playerEvents.Count(e => e.ActorType == "PLAYER" && e.ActionType == GameActionCatalog.Menabung);
            var financialGoalActionCount = playerEvents.Count(e => e.ActorType == "PLAYER" && e.ActionType == GameActionCatalog.TujuanFinansial);
            var insuranceActionCount = playerEvents.Count(e => e.ActorType == "PLAYER" && e.ActionType == GameActionCatalog.Asuransi);
            var loanRepaymentActionCount = playerEvents.Count(e => e.ActorType == "PLAYER" && e.ActionType == GameActionCatalog.BayarPinjaman);
            var longTermActionCount = savingsActionCount + financialGoalActionCount + insuranceActionCount + loanRepaymentActionCount;

            derived["risk_readiness_percent"] = SafeRatio(
                risksResolvedWithoutEmergency,
                riskLoanMetrics.RiskCardsDrawn,
                true);
            derived["risk_readiness_components"] = new
            {
                risks_resolved_without_emergency = risksResolvedWithoutEmergency,
                life_risk_cards_drawn = riskLoanMetrics.RiskCardsDrawn
            };
            derived["loan_burden_percent"] = SafeRatio(
                riskLoanMetrics.LoansOutstandingAmount,
                riskLoanMetrics.LoansOutstandingAmount + liquidAssets,
                true);
            derived["loan_burden_components"] = new
            {
                outstanding_loan = riskLoanMetrics.LoansOutstandingAmount,
                liquid_assets = liquidAssets
            };
            derived["financial_goal_progress_percent"] = SafeRatio(
                savingGoalMetrics.FinancialGoalsCoinsTotalInvested,
                attemptedGoalTargetTotal,
                true);
            derived["financial_goal_progress_components"] = new
            {
                coins_committed_to_goals = savingGoalMetrics.FinancialGoalsCoinsTotalInvested,
                attempted_goal_target_total = attemptedGoalTargetTotal
            };
            derived["long_term_action_share_percent"] = SafeRatio(
                longTermActionCount,
                actionMetrics.ActionEventCount,
                true);
            derived["long_term_action_share_components"] = new
            {
                saving_actions = savingsActionCount,
                financial_goal_actions = financialGoalActionCount,
                insurance_actions = insuranceActionCount,
                loan_repayment_actions = loanRepaymentActionCount,
                total_main_actions = actionMetrics.ActionEventCount
            };
        }

        var rawNode = JsonSerializer.SerializeToNode(raw)!.AsObject();
        if (!string.Equals(config?.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase))
        {
            rawNode.Remove("life_risk");
            rawNode.Remove("financial_goals");
            if (rawNode["turns"] is JsonObject turns)
            {
                turns.Remove("day_when_debt_introduced");
                turns.Remove("day_when_first_risk_hit");
            }
        }

        var rawJson = rawNode.ToJsonString();
        var derivedJson = JsonSerializer.Serialize(derived);
        return new AnalyticsGameplaySnapshot(rawJson, derivedJson);
    }
}
