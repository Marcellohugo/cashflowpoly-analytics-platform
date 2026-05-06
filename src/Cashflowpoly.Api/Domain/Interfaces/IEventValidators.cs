using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

internal interface IEventShapeValidator
{
    EventDomainValidationResult Validate(EventRequest request, Guid? scopedPlayerId);
}

internal interface IEventSimpleActionValidator
{
    bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        out EventDomainValidationResult result);
}

internal interface IEventTurnProgressValidator
{
    bool RequiresHistory(EventRequest request, RulesetConfig config);

    bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventDomainValidationResult result);
}

internal interface IEventNeedPurchaseValidator
{
    bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventNeedPurchaseValidation result);
}

internal interface IEventIngredientOrderValidator
{
    bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventIngredientOrderValidation result);
}

internal interface IEventSavingGoalValidator
{
    bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventSavingGoalValidation result);
}

internal interface IEventEconomyActionValidator
{
    bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventEconomyActionValidation result);
}

internal interface IEventAssignmentValidator
{
    bool TryValidate(
        EventRequest request,
        IEnumerable<EventDb> history,
        out EventDomainValidationResult result);
}
