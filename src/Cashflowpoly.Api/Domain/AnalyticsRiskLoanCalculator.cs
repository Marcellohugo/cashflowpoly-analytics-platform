// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsRiskLoanCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Cashflowpoly.Api.Domain.AnalyticsMath` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using static Cashflowpoly.Api.Domain.AnalyticsMath;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public sealed record AnalyticsRiskLoanMetrics(
    // Parameter `RiskCostsPerCard` bertipe `IReadOnlyList<int>` membawa nilai risiko costs per kartu.
    IReadOnlyList<int> RiskCostsPerCard,
    // Parameter `RiskCostsTotal` bertipe `int` membawa nilai risiko costs total.
    int RiskCostsTotal,
    // Parameter `RiskCardsDrawn` bertipe `int` membawa nilai risiko kartu drawn.
    int RiskCardsDrawn,
    // Parameter `RiskMitigated` bertipe `int` membawa nilai risiko mitigated.
    int RiskMitigated,
    // Parameter `RiskAccepted` bertipe `int` membawa nilai risiko accepted.
    int RiskAccepted,
    // Parameter `InsurancePayments` bertipe `int` membawa nilai asuransi payments.
    int InsurancePayments,
    // Parameter `EmergencyOptionsUsed` bertipe `int` membawa nilai emergency options used.
    int EmergencyOptionsUsed,
    // Parameter `LoansTaken` bertipe `int` membawa nilai pinjaman taken.
    int LoansTaken,
    // Parameter `LoansRepaid` bertipe `int` membawa nilai pinjaman dilunasi.
    int LoansRepaid,
    // Parameter `LoansUnpaid` bertipe `int` membawa nilai pinjaman unpaid.
    int LoansUnpaid,
    // Parameter `LoansOutstandingAmount` bertipe `double` membawa nilai pinjaman belum dilunasi nominal.
    double LoansOutstandingAmount,
    // Parameter `RiskExposurePercentage` bertipe `double?` membawa nilai risiko exposure percentage; nilai null diizinkan ketika data opsional belum
    // tersedia.
    double? RiskExposurePercentage,
    // Parameter `RiskMitigationEffectiveness` bertipe `double?` membawa nilai risiko mitigation effectiveness; nilai null diizinkan ketika data
    // opsional belum tersedia.
    double? RiskMitigationEffectiveness,
    // Parameter `AverageRiskCost` bertipe `double` membawa nilai rata-rata risiko biaya.
    double AverageRiskCost,
    // Parameter `RiskAcceptanceRate` bertipe `double?` membawa nilai risiko acceptance rate; nilai null diizinkan ketika data opsional belum tersedia.
    double? RiskAcceptanceRate,
    // Parameter `InsuranceCoverageRate` bertipe `double?` membawa nilai asuransi coverage rate; nilai null diizinkan ketika data opsional belum
    // tersedia.
    double? InsuranceCoverageRate,
    // Parameter `RiskCostIntensity` bertipe `double?` membawa nilai risiko biaya intensity; nilai null diizinkan ketika data opsional belum tersedia.
    double? RiskCostIntensity,
    // Parameter `RiskAppetiteScore` bertipe `double?` membawa nilai risiko appetite skor; nilai null diizinkan ketika data opsional belum tersedia.
    double? RiskAppetiteScore,
    // Parameter `DebtLeverageRatio` bertipe `double?` membawa nilai debt leverage ratio; nilai null diizinkan ketika data opsional belum tersedia.
    double? DebtLeverageRatio,
    // Parameter `LoanRepaymentDiscipline` bertipe `double?` membawa nilai pinjaman repayment discipline; nilai null diizinkan ketika data opsional
    // belum tersedia.
    double? LoanRepaymentDiscipline,
    // Parameter `DebtRatio` bertipe `double?` membawa nilai debt ratio; nilai null diizinkan ketika data opsional belum tersedia.
    double? DebtRatio);

internal sealed class RiskLoanCalculator : IRiskLoanCalculator
{
    private static readonly AnalyticsPayloadReader _payloadReader = new();

    public AnalyticsRiskLoanMetrics Compute(
        // Parameter `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event.
        IReadOnlyCollection<EventDb> playerEvents,
        // Parameter `playerProjections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections.
        IReadOnlyCollection<CashflowProjectionDb> playerProjections,
        // Parameter `startingCoins` bertipe `int` membawa nilai starting coins.
        int startingCoins,
        // Parameter `coinsNetEndGame` bertipe `double` membawa nilai coins net end game.
        double coinsNetEndGame,
        // Parameter `totalIncome` bertipe `double` membawa nilai total pemasukan.
        double totalIncome,
        // Parameter `lifeRisks` bertipe `IReadOnlyCollection<RulesetLifeRiskDto>?` membawa nilai life risks; nilai null diizinkan ketika data opsional
        // belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
        IReadOnlyCollection<RulesetLifeRiskDto>? lifeRisks = null)
    {
        var riskEvents = playerEvents.Where(e => e.ActionType == "RisikoKehidupan").ToList();
        var riskDefinitions = (lifeRisks ?? [])
            .ToDictionary(item => item.RiskCode, StringComparer.OrdinalIgnoreCase);
        var riskCostsPerCard = new List<int>();
        // Mengulangi setiap elemen `riskEvents`; elemen saat ini disimpan sebagai `riskEvent` bertipe `var` untuk diproses oleh badan loop dalam Compute.
        foreach (var riskEvent in riskEvents)
        {
            var cost = TryReadRiskId(riskEvent.Payload, out var riskId) &&
                       riskDefinitions.TryGetValue(riskId, out var definition)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: definition.Direction == ”OUT” &&
                // definition.EffectType.Contains(”COIN_EFFECT”, StringComparison.OrdinalIgnoreCase) dalam Compute.
                ? definition.Direction == "OUT" && definition.EffectType.Contains("COIN_EFFECT", StringComparison.OrdinalIgnoreCase)
                    // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: definition.Amount dalam Compute.
                    ? definition.Amount
                    // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: 0 dalam Compute.
                    : 0
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: playerProjections dalam Compute.
                : playerProjections
                    .Where(p => p.Category == "RISK_LIFE" &&
                                p.Direction == "OUT" &&
                                (p.EventId == riskEvent.EventId || ReferencesRiskEvent(p.Reference, riskEvent.EventId)))
                    .Sum(p => p.Amount);
            riskCostsPerCard.Add(cost);
        }

        var riskCostsTotal = riskCostsPerCard.Sum();
        var riskCardsDrawn = riskEvents.Count;
        var riskMitigated = playerEvents.Count(e => e.ActionType == GameActionCatalog.Asuransi &&
            TryReadRiskId(e.Payload, out _, "risk_event_id"));
        var insuredRisks = playerEvents.Where(e => e.ActionType == GameActionCatalog.Asuransi)
            .Select(e => TryReadRiskId(e.Payload, out var reference, "risk_event_id") ? reference : string.Empty)
            .Select(reference => Guid.TryParse(reference, out var id) ? id.ToString() : reference)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        // Draw-based metrics exclude claims on mass cards drawn by another participant.
        var drawnRisksMitigated = riskEvents.Count(e => insuredRisks.Contains(e.EventId.ToString()) ||
            (TryReadRiskId(e.Payload, out var code) && insuredRisks.Contains(code)));
        var riskAccepted = Math.Max(0, riskCardsDrawn - drawnRisksMitigated);
        var insurancePayments = playerProjections
            .Where(p => p.Category == "INSURANCE_PREMIUM" && p.Direction == "OUT")
            .Sum(p => p.Amount);
        var emergencyOptionsUsed = playerEvents.Count(e => e.ActionType == "GunakanOpsiDarurat");

        var loanStates = BuildLoanStates(playerEvents);
        var loansTaken = loanStates.Count;
        var loansRepaid = loanStates.Values.Count(l => l.RepaidAmount >= l.Principal);
        var loansUnpaid = loanStates.Values.Count(l => l.RepaidAmount < l.Principal);
        var loansOutstandingAmount = loanStates.Values.Sum(l => Math.Max(0, l.Principal - l.RepaidAmount));

        var averageRiskCost = riskCardsDrawn > 0 ? (double)riskCostsTotal / riskCardsDrawn : 0;
        var riskAcceptanceRate = SafeRatio(riskAccepted, riskCardsDrawn);
        var insuranceCoverageRate = SafeRatio(drawnRisksMitigated, riskCardsDrawn);
        var riskCostIntensity = SafeRatio(averageRiskCost, startingCoins);
        var riskAppetiteScore =
            riskAcceptanceRate.HasValue &&
            riskCostIntensity.HasValue
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: Clamp(riskAcceptanceRate.Value * riskCostIntensity.Value * 100, 0,
                // 100) dalam Compute.
                ? Clamp(riskAcceptanceRate.Value * riskCostIntensity.Value * 100, 0, 100)
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (double?)null; dalam Compute.
                : (double?)null;

        return new AnalyticsRiskLoanMetrics(
            riskCostsPerCard,
            riskCostsTotal,
            riskCardsDrawn,
            riskMitigated,
            riskAccepted,
            insurancePayments,
            emergencyOptionsUsed,
            loansTaken,
            loansRepaid,
            loansUnpaid,
            loansOutstandingAmount,
            SafeRatio(riskCostsTotal, totalIncome, true),
            SafeRatio(drawnRisksMitigated, riskCardsDrawn, true),
            averageRiskCost,
            riskAcceptanceRate,
            insuranceCoverageRate,
            riskCostIntensity,
            riskAppetiteScore,
            SafeRatio(loansOutstandingAmount, coinsNetEndGame, true),
            SafeRatio(loansRepaid, loansTaken, true),
            SafeRatio(loansUnpaid, loansTaken));
    }

    private static bool ReferencesRiskEvent(string? reference, Guid riskEventId)
        => Guid.TryParse(reference, out var referencedEventId) && referencedEventId == riskEventId;

    private static bool TryReadRiskId(string payload, out string riskId, string property = "risk_id")
    {
        riskId = string.Empty;
        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(payload);
            if (!document.RootElement.TryGetProperty(property, out var riskIdElement) ||
                riskIdElement.ValueKind != System.Text.Json.JsonValueKind.String)
            {
                return false;
            }

            riskId = riskIdElement.GetString() ?? string.Empty;
            return riskId.Length > 0;
        }
        // Menangani exception `System.Text.Json.JsonException` melalui variabel dalam TryReadRiskId.
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
    }

    private Dictionary<string, LoanState> BuildLoanStates(IEnumerable<EventDb> playerEvents)
    {
        var loanStates = new Dictionary<string, LoanState>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `playerEvents`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh badan loop dalam
        // BuildLoanStates.
        foreach (var evt in playerEvents)
        {
            if ((evt.ActionType == GameActionCatalog.PinjamanSyariah ||
                 evt.ActionType == GameActionCatalog.SetupPinjamanAwal ||
                 IsEmergencyLoan(evt.Payload)) &&
                _payloadReader.TryReadLoanTaken(evt.Payload, out var loanId, out var principal, out var penaltyPoints))
            {
                loanStates[loanId] = new LoanState(loanId, principal, penaltyPoints, 0);
            }

            if (evt.ActionType == "BayarPinjaman" &&
                _payloadReader.TryReadLoanRepay(evt.Payload, out var repayLoanId, out var repayAmount) &&
                loanStates.TryGetValue(repayLoanId, out var state))
            {
                loanStates[repayLoanId] = state with { RepaidAmount = state.RepaidAmount + repayAmount };
            }
        }

        return loanStates;
    }

    private static bool IsEmergencyLoan(string payload)
    {
        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(payload);
            return document.RootElement.TryGetProperty("option_type", out var optionType) &&
                   string.Equals(optionType.GetString(), "TAKE_SHARIA_LOAN", StringComparison.OrdinalIgnoreCase);
        }
        // Menangani exception `System.Text.Json.JsonException` melalui variabel dalam IsEmergencyLoan.
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
    }

    private sealed record LoanState(
        // Parameter `LoanId` bertipe `string` membawa nilai pinjaman identitas.
        string LoanId,
        // Parameter `Principal` bertipe `int` membawa nilai principal.
        int Principal,
        // Parameter `PenaltyPoints` bertipe `int` membawa nilai penalti poin.
        int PenaltyPoints,
        // Parameter `RepaidAmount` bertipe `double` membawa nilai dilunasi nominal.
        double RepaidAmount);
}
