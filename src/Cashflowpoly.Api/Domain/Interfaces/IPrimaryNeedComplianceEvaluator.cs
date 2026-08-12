// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IPrimaryNeedComplianceEvaluator.
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

public interface IPrimaryNeedComplianceEvaluator
{
    PrimaryNeedComplianceResult Evaluate(List<EventDb> playerEvents, RulesetConfig? config);
}
