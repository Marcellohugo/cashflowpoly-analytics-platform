using Cashflowpoly.Api.Data;
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

public interface IEventRequestShapeValidator
{
    EventDomainValidationResult Validate(EventRequest request, Guid? scopedPlayerId);
}

public interface IEventSimpleActionValidator
{
    bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        out EventDomainValidationResult result);
}

public interface IEventTurnProgressValidator
{
    bool RequiresHistory(EventRequest request, RulesetConfig config);

    bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventDomainValidationResult result);
}

public interface IEventNeedPurchaseValidator
{
    bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventNeedPurchaseValidation result);
}

public interface IEventIngredientOrderValidator
{
    bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventIngredientOrderValidation result);
}

public interface IEventSavingGoalValidator
{
    bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventSavingGoalValidation result);
}

public interface IEventEconomyActionValidator
{
    bool TryValidate(
        EventRequest request,
        RulesetConfig config,
        IEnumerable<EventDb> history,
        out EventEconomyActionValidation result);
}

public interface IEventAssignmentValidator
{
    bool TryValidate(
        EventRequest request,
        IEnumerable<EventDb> history,
        out EventDomainValidationResult result);
}
