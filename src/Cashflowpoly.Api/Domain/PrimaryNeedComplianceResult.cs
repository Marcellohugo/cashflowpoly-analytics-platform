// Fungsi file: Model hasil evaluasi compliance kebutuhan primer untuk analitik.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Detail compliance satu hari. Nama properti dipertahankan snake_case agar JSON snapshot kompatibel.
/// </summary>
internal sealed record PrimaryNeedComplianceDayDetail(int day_index, bool compliant, List<string> reason);

/// <summary>
/// Hasil evaluasi compliance kebutuhan primer lintas hari.
/// </summary>
internal sealed record PrimaryNeedComplianceResult(
    double Rate,
    List<PrimaryNeedComplianceDayDetail> Details,
    int EvaluatedDays,
    int CompliantDays);
