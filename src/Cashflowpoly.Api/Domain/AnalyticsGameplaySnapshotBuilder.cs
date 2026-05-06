// Fungsi file: Membangun JSON snapshot gameplay mentah dan metrik turunan per pemain.
using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;
using static Cashflowpoly.Api.Domain.AnalyticsMath;
using static Cashflowpoly.Api.Domain.AnalyticsPayloadReader;

namespace Cashflowpoly.Api.Domain;

internal sealed record AnalyticsGameplaySnapshot(string RawJson, string DerivedJson);

internal static class AnalyticsGameplaySnapshotBuilder
{
    internal static AnalyticsGameplaySnapshot Build(
        List<EventDb> playerEvents,
        List<CashflowProjectionDb> playerProjections,
        List<EventDb> allEvents,
        RulesetConfig? config,
        AnalyticsHappinessBreakdown happiness)
    {
        var notesRaw = new List<string>();
        var notesDerived = new List<string>();

        var cashTimeline = AnalyticsCashTimelineCalculator.Compute(
            playerEvents,
            playerProjections,
            config?.StartingCash ?? 0);
        var startingCoins = cashTimeline.StartingCoins;
        var cashInTotal = cashTimeline.CashInTotal;
        var cashOutTotal = cashTimeline.CashOutTotal;
        var coinsNetEndGame = cashTimeline.CoinsNetEndGame;
        var coinsHeldCurrent = cashTimeline.CoinsHeldCurrent;

        var donationMetrics = AnalyticsDonationGameplayCalculator.Compute(playerEvents, allEvents, coinsNetEndGame);
        var donationTotal = donationMetrics.DonationTotalCoins;

        var savingGoalMetrics = AnalyticsSavingGoalCalculator.Compute(playerEvents);
        var coinsSaved = savingGoalMetrics.CoinsSaved;

        var ingredientMealMetrics = AnalyticsIngredientMealCalculator.Compute(playerEvents, playerProjections);
        var inventory = ingredientMealMetrics.Inventory;
        var ingredientsCollected = ingredientMealMetrics.IngredientsCollected;
        var ingredientTypesHeld = ingredientMealMetrics.IngredientTypesHeld;
        var ingredientsUsedTotal = ingredientMealMetrics.IngredientsUsedTotal;
        var ingredientsWasted = ingredientMealMetrics.IngredientsWasted;
        var ingredientInvestmentTotal = ingredientMealMetrics.IngredientInvestmentTotal;
        var mealOrderIncomeValues = ingredientMealMetrics.MealOrderIncomeValues;
        var mealOrdersClaimed = ingredientMealMetrics.MealOrdersClaimed;
        var mealOrdersPassed = ingredientMealMetrics.MealOrdersPassed;
        var mealOrderIncomeTotal = ingredientMealMetrics.MealOrderIncomeTotal;
        var mealOrdersPerTurnAverage = ingredientMealMetrics.MealOrdersPerTurnAverage;
        var essentialIngredientExpenses = ingredientMealMetrics.EssentialIngredientExpenses;
        var maxTurnNumber = ingredientMealMetrics.MaxTurnNumber;

        var needMissionMetrics = AnalyticsNeedMissionCalculator.Compute(playerEvents, playerProjections);

        var goldMetrics = AnalyticsGoldGameplayCalculator.Compute(playerEvents);
        var goldInvestmentEarned = goldMetrics.GoldInvestmentEarned;
        var goldInvestmentSpent = goldMetrics.GoldInvestmentSpent;
        var goldInvestmentNet = goldMetrics.GoldInvestmentNet;

        var pensionRank = playerEvents
            .Where(e => e.ActionType == "pension.rank.awarded")
            .Select(e => TryReadRankAwarded(e.Payload, out var rank, out _) ? rank : 0)
            .FirstOrDefault(rank => rank > 0);

        var riskLoanMetrics = AnalyticsRiskLoanCalculator.Compute(
            playerEvents,
            playerProjections,
            startingCoins,
            coinsNetEndGame,
            cashInTotal);

        var actionMetrics = AnalyticsActionUsageCalculator.Compute(
            playerEvents,
            playerProjections,
            maxTurnNumber,
            config?.ActionsPerTurn ?? 2);
        var latestEvent = playerEvents
            .OrderByDescending(e => e.SequenceNumber)
            .FirstOrDefault();
        var sessionId = playerEvents
            .Select(e => (Guid?)e.SessionId)
            .FirstOrDefault()
            ?? allEvents.Select(e => (Guid?)e.SessionId).FirstOrDefault();
        var resolvedPlayerId = playerEvents
            .Select(e => e.PlayerId)
            .FirstOrDefault(id => id.HasValue);
        var gameMode = string.Equals(config?.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase)
            ? "advanced"
            : "beginner";
        var finishLineReached = allEvents.Any(e => e.ActionType == "session.ended");
        int? finalRank = null;
        bool? winnerFlag = finalRank.HasValue ? finalRank.Value == 1 : null;
        bool? dnfFlag = finishLineReached ? false : null;

        var raw = new
        {
            metadata = new
            {
                game_id = sessionId,
                session_id = sessionId,
                player_id = resolvedPlayerId,
                player_alias = (string?)null,
                game_mode = gameMode,
                turn_number = latestEvent?.TurnNumber,
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
                ingredients_used_per_meal = ingredientsUsedTotal,
                ingredients_wasted = ingredientsWasted,
                ingredient_investment_coins_total = ingredientInvestmentTotal
            },
            meal_orders = new
            {
                meal_orders_claimed = mealOrdersClaimed,
                meal_orders_available_passed = mealOrdersPassed,
                meal_order_income_per_order = mealOrderIncomeValues,
                meal_order_income_total = mealOrderIncomeTotal,
                meal_orders_per_turn_average = mealOrdersPerTurnAverage
            },
            needs = new
            {
                need_cards_purchased = needMissionMetrics.NeedCardsPurchased,
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
                pension_fund_rank_per_game = pensionRank == 0 ? (int?)null : pensionRank,
                pension_fund_happiness_points = happiness.PensionPoints
            },
            life_risk = new
            {
                life_risks_available = riskLoanMetrics.RiskCardsDrawn,
                life_risk_cards_drawn = riskLoanMetrics.RiskCardsDrawn,
                life_risks_accepted = riskLoanMetrics.RiskAccepted,
                life_risk_costs_per_card = riskLoanMetrics.RiskCostsPerCard,
                life_risk_costs_total = riskLoanMetrics.RiskCostsTotal,
                life_risk_mitigated_with_insurance = riskLoanMetrics.RiskMitigated,
                insurance_payments_made = riskLoanMetrics.InsurancePayments,
                emergency_options_used = riskLoanMetrics.EmergencyOptionsUsed
            },
            financial_goals = new
            {
                financial_goals_available_total = savingGoalMetrics.FinancialGoalsAvailableTotal,
                financial_goals_attempted = savingGoalMetrics.FinancialGoalsAttempted,
                financial_goals_completed = savingGoalMetrics.FinancialGoalsCompleted,
                financial_goals_coins_per_goal = savingGoalMetrics.SavingBalancesByGoal,
                financial_goals_coins_total_invested = savingGoalMetrics.FinancialGoalsCoinsTotalInvested,
                financial_goals_incomplete_coins_wasted = savingGoalMetrics.FinancialGoalsIncompleteCoinsWasted,
                sharia_loans_taken = riskLoanMetrics.LoansTaken,
                sharia_loan_cards_taken = riskLoanMetrics.LoansTaken,
                sharia_loans_repaid = riskLoanMetrics.LoansRepaid,
                sharia_loans_unpaid_end = riskLoanMetrics.LoansUnpaid,
                sharia_loans_outstanding_coins = riskLoanMetrics.LoansOutstandingAmount,
                loan_penalty_if_unpaid = happiness.LoanPenaltyPoints
            },
            actions = new
            {
                actions_per_turn = config?.ActionsPerTurn ?? 2,
                action_repetitions_per_turn = actionMetrics.ActionRepetitions,
                action_sequence = actionMetrics.ActionSequences,
                actions_skipped = actionMetrics.ActionsSkipped
            },
            turns = new
            {
                coins_per_turn_progression = cashTimeline.CoinsProgression,
                net_income_per_turn = cashTimeline.NetIncomePerTurn,
                turn_number_when_debt_introduced = playerEvents
                    .Where(e => e.ActionType == "loan.syariah.taken")
                    .Select(e => (int?)e.TurnNumber)
                    .OrderBy(t => t)
                    .FirstOrDefault(),
                turn_number_when_first_risk_hit = playerEvents
                    .Where(e => e.ActionType == "risk.life.drawn")
                    .Select(e => (int?)e.TurnNumber)
                    .OrderBy(t => t)
                    .FirstOrDefault(),
                turn_number_game_completion = maxTurnNumber == 0 ? (int?)null : maxTurnNumber
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

        var totalIncome = cashInTotal;
        var totalExpenses = cashOutTotal;
        var businessEfficiencyRatio = SafeRatio(mealOrderIncomeTotal, ingredientInvestmentTotal);
        var goldRoiPercentage = SafeRatio(goldInvestmentNet, goldInvestmentSpent, true);
        var riskExposurePercentage = riskLoanMetrics.RiskExposurePercentage;
        var riskMitigationEffectiveness = riskLoanMetrics.RiskMitigationEffectiveness;

        var netWorthIndex = SafeRatio(coinsNetEndGame, startingCoins, true);
        var incomeDiversificationMetrics = AnalyticsIncomeDiversificationCalculator.Compute(
            playerEvents,
            playerProjections,
            totalIncome,
            mealOrderIncomeTotal,
            goldInvestmentEarned);
        if (incomeDiversificationMetrics.RequiresIncomeNote)
        {
            notesDerived.Add("income_diversification_requires_income");
        }
        var incomeDiversification = incomeDiversificationMetrics.IncomeDiversification;

        var Risk_Acceptance_Rate = riskLoanMetrics.RiskAcceptanceRate;
        var Insurance_Coverage_Rate = riskLoanMetrics.InsuranceCoverageRate;
        var Risk_Cost_Intensity = riskLoanMetrics.RiskCostIntensity;
        var insuranceActivationRate = riskLoanMetrics.InsuranceCoverageRate;
        var riskAppetiteScore = riskLoanMetrics.RiskAppetiteScore;
        if (riskLoanMetrics.RiskCardsDrawn == 0)
        {
            notesDerived.Add("risk_appetite_requires_risk_events");
        }
        var debtLeverageRatio = riskLoanMetrics.DebtLeverageRatio;
        var loanRepaymentDiscipline = riskLoanMetrics.LoanRepaymentDiscipline;
        var debtRatio = riskLoanMetrics.DebtRatio;

        var actionEfficiency = actionMetrics.ActionEfficiency;
        var actionEfficiencyPercent = actionMetrics.ActionEfficiencyPercent;
        var actionDiversityAverage = actionMetrics.ActionDiversityAverage;

        var derivedRatioMetrics = AnalyticsDerivedRatioCalculator.Compute(
            essentialIngredientExpenses,
            totalExpenses,
            mealOrderIncomeTotal,
            ingredientInvestmentTotal,
            savingGoalMetrics,
            coinsNetEndGame,
            playerEvents,
            actionMetrics.ActionEventCount,
            mealOrdersClaimed,
            mealOrdersPassed,
            needMissionMetrics,
            startingCoins,
            riskAppetiteScore);

        var derived = new
        {
            net_worth_index = netWorthIndex,
            income_diversification_index = incomeDiversification,
            income_diversification_ratio = incomeDiversification,
            income_diversification_components = new
            {
                freelance_income = incomeDiversificationMetrics.FreelanceIncome,
                meal_income = incomeDiversificationMetrics.MealIncome,
                gold_income = incomeDiversificationMetrics.GoldIncome,
                donations_received = incomeDiversificationMetrics.DonationIncome,
                total_income = totalIncome,
                N_active_income_sources = incomeDiversificationMetrics.ActiveIncomeSourceCount,
                Income_Share_i = incomeDiversificationMetrics.IncomeShares
            },
            expense_management_efficiency = derivedRatioMetrics.ExpenseEfficiency,
            expense_management_components = new
            {
                essential_expenses = essentialIngredientExpenses,
                total_expenses = totalExpenses
            },
            business_profit_margin = derivedRatioMetrics.BusinessProfitMargin,
            business_efficiency_ratio = businessEfficiencyRatio,
            gold_roi_percentage = goldRoiPercentage,
            risk_exposure_percentage = riskExposurePercentage,
            risk_mitigation_effectiveness = riskMitigationEffectiveness,
            risk_appetite_score = riskAppetiteScore,
            risk_appetite_components = new
            {
                life_risks_accepted = riskLoanMetrics.RiskAccepted,
                life_risks_available = riskLoanMetrics.RiskCardsDrawn,
                risk_acceptance_rate = Risk_Acceptance_Rate,
                average_risk_cost = riskLoanMetrics.AverageRiskCost,
                insurance_activation_rate = insuranceActivationRate,
                Insurance_Coverage_Rate,
                Risk_Cost_Intensity
            },
            debt_leverage_ratio = debtLeverageRatio,
            loan_repayment_discipline = loanRepaymentDiscipline,
            debt_ratio = debtRatio,
            goal_ambition = derivedRatioMetrics.GoalSettingAmbition,
            goal_setting_ambition = derivedRatioMetrics.GoalSettingAmbition,
            goal_setting_components = new
            {
                Goal_Attempt_Rate = derivedRatioMetrics.GoalAttemptRate,
                Goal_Investment_Rate = derivedRatioMetrics.GoalInvestmentRate
            },
            action_efficiency = actionEfficiency,
            action_efficiency_percent = actionEfficiencyPercent,
            action_diversity_score_avg = actionDiversityAverage,
            meal_order_success_rate = derivedRatioMetrics.MealOrderSuccessRate,
            planning_horizon = derivedRatioMetrics.PlanningHorizon,
            planning_horizon_percent = derivedRatioMetrics.PlanningHorizonPercent,
            fulfillment_diversity = needMissionMetrics.FulfillmentDiversity,
            fulfillment_diversity_components = new
            {
                p_primary = derivedRatioMetrics.PrimaryNeedShare,
                p_secondary = derivedRatioMetrics.SecondaryNeedShare,
                p_tertiary = derivedRatioMetrics.TertiaryNeedShare
            },
            mission_achievement = needMissionMetrics.MissionAchievement,
            growth_pattern_ratio = derivedRatioMetrics.GrowthPatternRatio,
            donation_aggressiveness_percent = donationMetrics.DonationAggressivenessPercent,
            donation_stability_std_deviation = donationMetrics.DonationStabilityStdDeviation,
            donation_ratio = donationMetrics.DonationRatio,
            friday_participation_rate = donationMetrics.FridayParticipationRate,
            donation_commitment_score = donationMetrics.DonationCommitmentScore,
            donation_commitment_components = new
            {
                donation_stability = donationMetrics.DonationStability,
                donation_ratio = donationMetrics.DonationRatio,
                friday_participation_rate = donationMetrics.FridayParticipationRate
            },
            risk_appetite_score_normalized = derivedRatioMetrics.RiskAppetiteScoreNormalized,
            sharia_loans_outstanding_coins = riskLoanMetrics.LoansOutstandingAmount,
            happiness_portfolio = new
            {
                need_cards_pts = happiness.NeedPoints,
                donations_pts = happiness.DonationPoints,
                gold_pts = happiness.GoldPoints,
                pension_pts = happiness.PensionPoints,
                financial_goals_pts = happiness.SavingGoalPointsEffective,
                mission_bonus_pts = 0 - happiness.MissionPenaltyPoints
            },
            notes = notesDerived
        };

        var rawJson = JsonSerializer.Serialize(raw);
        var derivedJson = JsonSerializer.Serialize(derived);
        return new AnalyticsGameplaySnapshot(rawJson, derivedJson);
    }
}
