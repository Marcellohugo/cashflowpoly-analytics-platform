// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventSavingGoalValidator.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Contracts;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Api.Domain;

public sealed record EventSavingGoalValidation(
    EventDomainValidationResult Validation,
    int? OutgoingAmount);

internal sealed class EventSavingGoalValidator : IEventSavingGoalValidator
{
    private static readonly EventPayloadReader _payloadReader = new();
    private static readonly EventDerivedStateCalculator _derivedState = new();
    private const int RulebookSavingMaxDeposit = 15;

    public bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventSavingGoalValidation result)
    {
        if (string.Equals(request.ActionType, "TarikTabungan", StringComparison.OrdinalIgnoreCase))
        {
            result = Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                "TarikTabungan bukan aksi resmi ruleset rulebook");
            return true;
        }

        if (string.Equals(request.ActionType, "Menabung", StringComparison.OrdinalIgnoreCase))
        {
            result = ValidateDeposit(request, config, history);
            return true;
        }

        if (string.Equals(request.ActionType, "TujuanFinansial", StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(request.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase))
            {
                result = Fail(
                    StatusCodes.Status422UnprocessableEntity,
                    "DOMAIN_RULE_VIOLATION",
                    "TujuanFinansial bukan aksi pemain terpisah; kartu tujuan diperoleh otomatis saat Menabung mencapai target");
                return true;
            }

            result = ValidateGoalAchieved(request, config, history);
            return true;
        }

        result = new EventSavingGoalValidation(EventDomainValidationResult.Valid, null);
        return false;
    }

    private EventSavingGoalValidation ValidateDeposit(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history)
    {
        var featureValidation = ValidateFeatureAndPlayer(request, config);
        if (!featureValidation.IsValid)
        {
            return new EventSavingGoalValidation(featureValidation, null);
        }

        if (!_payloadReader.TryReadSavingDeposit(request.Payload, out var goalId, out var amount))
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

        if (config.FinancialGoals.All(goal => !string.Equals(goal.Id, goalId, StringComparison.OrdinalIgnoreCase)))
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                "Kartu Tujuan Finansial tidak ditemukan pada ruleset");
        }

        var isCreate = string.Equals(request.ActionType, "Menabung", StringComparison.OrdinalIgnoreCase);
        if (isCreate && amount > RulebookSavingMaxDeposit)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Maksimal tabungan per aksi adalah 15 koin");
        }

        var balance = _derivedState.ComputeSavingBalance(history, request.UserId!.Value, goalId);
        if (!isCreate && balance < amount)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Saldo tabungan tidak mencukupi");
        }

        return new EventSavingGoalValidation(EventDomainValidationResult.Valid, isCreate ? amount : null);
    }

    private EventSavingGoalValidation ValidateGoalAchieved(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history)
    {
        var featureValidation = ValidateFeatureAndPlayer(request, config);
        if (!featureValidation.IsValid)
        {
            return new EventSavingGoalValidation(featureValidation, null);
        }

        if (!_payloadReader.TryReadSavingGoalAchieved(request.Payload, out var goalId, out var points, out var cost))
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

        var goal = config.FinancialGoals.FirstOrDefault(item =>
            string.Equals(item.Id, goalId, StringComparison.OrdinalIgnoreCase));
        if (goal is null)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                "Kartu Tujuan Finansial tidak ditemukan pada ruleset");
        }

        if (cost != goal.HargaBeli || points != goal.PoinKebahagiaan)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                "Biaya atau poin tujuan tidak sesuai katalog ruleset");
        }

        var completedCount = history.Count(evt =>
            GameActionCatalog.Is(evt.ActionType, _payloadReader.ReadPayload(evt.Payload), GameActionCatalog.TujuanFinansial) &&
            _payloadReader.TryReadSavingGoalAchieved(_payloadReader.ReadPayload(evt.Payload), out var completedGoalId, out _, out _) &&
            string.Equals(completedGoalId, goalId, StringComparison.OrdinalIgnoreCase));
        if (completedCount >= Math.Max(1, goal.CardQty ?? 1))
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                "Kartu Tujuan Finansial ini sudah dimiliki pemain lain");
        }

        var balance = _derivedState.ComputeSavingBalance(history, request.UserId!.Value, goalId);
        if (cost > 0 && balance < cost)
        {
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Saldo tabungan tidak mencukupi untuk goal");
        }

        return new EventSavingGoalValidation(EventDomainValidationResult.Valid, null);
    }

    private EventDomainValidationResult ValidateFeatureAndPlayer(EventRequest request, RulesetConfig config)
    {
        if (!config.SavingGoalEnabled)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status422UnprocessableEntity,
                "DOMAIN_RULE_VIOLATION",
                "Fitur tabungan tujuan tidak aktif");
        }

        if (request.UserId is null)
        {
            return EventDomainValidationResult.Fail(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Player wajib diisi",
                new ErrorDetail("user_id", "REQUIRED"));
        }

        return EventDomainValidationResult.Valid;
    }

    private EventDomainValidationResult ValidateSavingDepositPayload(string goalId, int amount)
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

    private EventSavingGoalValidation Fail(
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
