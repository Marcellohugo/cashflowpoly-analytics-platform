using System.Text.Json;

namespace Cashflowpoly.Api.Domain;

public interface IAnalyticsPayloadReader
{
    bool TryReadTransaction(string payloadJson, out string direction, out double amount, out string category);
    bool TryReadAmount(string payloadJson, out double amount);
    bool TryReadGoldTrade(string payloadJson, out string tradeType, out int qty);
    bool IsActionEvent(string actionType);
    bool TryReadActionUsed(string payloadJson, out int used, out int remaining);
    bool TryReadGoldTradeDetailed(string payloadJson, out string tradeType, out int qty, out int unitPrice, out int amount);
    bool TryReadIngredientPurchaseDetailed(string payloadJson, out string cardId, out string ingredientName, out int amount);
    bool TryReadSavingDeposit(string payloadJson, out string goalId, out int amount);
    bool TryReadIngredientPurchase(string payloadJson, out string cardId, out int amount);
    bool TryReadNeedPurchase(string payloadJson, out int amount, out string cardId, out int points);
    bool TryReadMissionAssigned(string payloadJson, out string missionId, out string targetTertiaryCardId, out int penaltyPoints, out bool requirePrimary, out bool requireSecondary);
    bool TryReadTieBreaker(string payloadJson, out int number);
    bool TryReadRankAwarded(string payloadJson, out int rank, out int points);
    bool TryReadPointsAwarded(string payloadJson, out int points);
    bool TryReadSavingGoalAchievedDetailed(string payloadJson, out string goalId, out int points, out int cost);
    bool TryReadSavingGoalAchieved(string payloadJson, out int points);
    bool TryReadLoanTaken(string payloadJson, out string loanId, out int principal, out int penaltyPoints);
    bool TryReadLoanRepay(string payloadJson, out string loanId, out int amount);
    bool TryReadOrderClaim(string payloadJson, out List<string> requiredCards, out int income);
}

public interface IEventPayloadReader
{
    JsonElement ReadPayload(string payload);
    bool TryGetString(JsonElement payload, string propertyName, out string value);
    bool TryGetOptionalString(JsonElement payload, string propertyName, out string? value);
    bool TryGetInt32(JsonElement payload, string propertyName, out int value, bool required = true);
    bool TryGetDouble(JsonElement payload, string propertyName, out double value, bool required = true);
    bool TryReadTransaction(JsonElement payload, out string direction, out double amount, out string category, out string? counterparty);
    bool TryReadAmount(JsonElement payload, out double amount);
    bool TryReadGoldTrade(JsonElement payload, out string tradeType, out int qty, out int unitPrice, out int amount);
    bool TryReadActionUsed(JsonElement payload, out int used, out int remaining);
    bool TryReadIngredientPurchase(JsonElement payload, out string cardId, out int amount);
    bool TryReadOrderClaim(JsonElement payload, out List<string> requiredCards, out int income);
    bool TryReadNeedPurchase(JsonElement payload, out string cardId, out int amount, out int points);
    bool TryReadMissionAssigned(JsonElement payload, out string missionId, out string targetCardId, out int penaltyPoints);
    bool TryReadTieBreaker(JsonElement payload, out int number);
    bool TryReadRankAwarded(JsonElement payload, out int rank, out int points);
    bool TryReadPointsAwarded(JsonElement payload, out int points);
    bool TryReadSavingDeposit(JsonElement payload, out string goalId, out int amount);
    bool TryReadSavingGoalAchieved(JsonElement payload, out string goalId, out int points, out int cost);
    bool TryReadRiskLife(JsonElement payload, out string riskId, out string direction, out int amount);
    bool TryReadInsuranceUsed(JsonElement payload, out string riskEventId);
    bool TryReadEmergencyOption(JsonElement payload, out string riskEventId, out string optionType, out string direction, out int amount);
    bool TryReadLoanTaken(JsonElement payload, out string loanId, out int principal, out int installment, out int duration, out int penaltyPoints);
    bool TryReadLoanRepay(JsonElement payload, out string loanId, out int amount);
    bool TryReadInsurance(JsonElement payload, out int premium);
}
