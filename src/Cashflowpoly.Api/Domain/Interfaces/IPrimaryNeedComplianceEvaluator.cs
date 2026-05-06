using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

internal interface IPrimaryNeedComplianceEvaluator
{
    PrimaryNeedComplianceResult Evaluate(List<EventDb> playerEvents, RulesetConfig? config);
}
