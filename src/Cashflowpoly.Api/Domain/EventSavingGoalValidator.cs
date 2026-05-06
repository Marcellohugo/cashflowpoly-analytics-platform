// Fungsi file: Memvalidasi event tabungan tujuan dan pencapaian goal berbasis histori saldo tabungan.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Http;
using static Cashflowpoly.Api.Domain.EventDerivedStateCalculator;
using static Cashflowpoly.Api.Domain.EventPayloadReader;

namespace Cashflowpoly.Api.Domain;

internal sealed record EventSavingGoalValidation(
    EventDomainValidationResult Validation,
    int? OutgoingAmount);

internal static class EventSavingGoalValidator
{
    private const int RulebookSavingMaxDeposit = 15;

    internal static bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventSavingGoalValidation result)
    {
        if (string.Equals(request.ActionType, "saving.deposit.created", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(request.ActionType, "saving.deposit.withdrawn", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateDeposit(request, config, history);
            return true;
        }

        if (string.Equals(request.ActionType, "saving.goal.achieved", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateGoalAchieved(request, config, history);
            return true;
        }

        result = new EventSavingGoalValidation(EventDomainValidationResult.Valid, null);
        return false;
    }

    private static EventSavingGoalValidation ValidateDeposit(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history)
    {
        var featureValidation = ValidateFeatureAndPlayer(request, config);
        if (!featureValidation.IsValid)
        {
            return new EventSavingGoalValidation(featureValidation, null);
        }

        if (!TryReadSavingDeposit(request.Payload, out var goalId, out var amount))
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload tabungan tidak valid",
                new ErrorDetail("payload.amount", "REQUIRED"));
        }

        var payloadValidation = ValidateSavingDepositPayload(goalId, amount);
        if (!payloadValidation.IsValid)
        {
            return new EventSavingGoalValidation(payloadValidation, null);
        }

        var isCreate = string.Equals(request.ActionType, "saving.deposit.created", StringComparison.OrdinalIgnoreCase);
        if (isCreate && amount > RulebookSavingMaxDeposit)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Maksimal tabungan per aksi adalah 15 koin");
        }

        var balance = ComputeSavingBalance(history, request.PlayerId!.Value, goalId);
        if (!isCreate && balance < amount)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Saldo tabungan tidak mencukupi");
        }

        return new EventSavingGoalValidation(EventDomainValidationResult.Valid, isCreate ? amount : null);
    }

    private static EventSavingGoalValidation ValidateGoalAchieved(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history)
    {
        var featureValidation = ValidateFeatureAndPlayer(request, config);
        if (!featureValidation.IsValid)
        {
            return new EventSavingGoalValidation(featureValidation, null);
        }

        if (!TryReadSavingGoalAchieved(request.Payload, out var goalId, out var points, out var cost))
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Payload goal tidak valid",
                new ErrorDetail("payload.goal_id", "REQUIRED"));
        }

        if (points < 0)
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Points tidak valid",
                new ErrorDetail("payload.points", "OUT_OF_RANGE"));
        }

        if (cost < 0)
        {
            return Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Cost tidak valid",
                new ErrorDetail("payload.cost", "OUT_OF_RANGE"));
        }

        var balance = ComputeSavingBalance(history, request.PlayerId!.Value, goalId);
        if (cost > 0 && balance < cost)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Saldo tabungan tidak mencukupi untuk goal");
        }

        return new EventSavingGoalValidation(EventDomainValidationResult.Valid, null);
    }

    private static EventDomainValidationResult ValidateFeatureAndPlayer(EventRequest request, RulesetConfig config)
    {
        if (!config.SavingGoalEnabled)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                "Fitur tabungan tujuan tidak aktif");
        }

        if (request.PlayerId is null)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Player wajib diisi",
                new ErrorDetail("player_id", "REQUIRED"));
        }

        return EventDomainValidationResult.Valid;
    }

    private static EventDomainValidationResult ValidateSavingDepositPayload(string goalId, int amount)
    {
        if (string.IsNullOrWhiteSpace(goalId))
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Goal ID wajib diisi",
                new ErrorDetail("payload.goal_id", "REQUIRED"));
        }

        if (amount <= 0)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Amount harus > 0",
                new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
        }

        return EventDomainValidationResult.Valid;
    }

    private static EventSavingGoalValidation Fail(
        int statusCode,
        string errorCode,
        string message,
        params ErrorDetail[] details)
    {
        return new EventSavingGoalValidation(
            EventDomainValidationResult.Fail(statusCode, errorCode, message, details),
            null);
    }
}
