// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsRiskLoanCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Cashflowpoly.Api.Domain.AnalyticsMath` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using static Cashflowpoly.Api.Domain.AnalyticsMath;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsRiskLoanMetrics`; sealed mencegah tipe ini diturunkan lagi.
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

// Mendefinisikan tipe class `RiskLoanCalculator` yang mewarisi atau menerapkan `IRiskLoanCalculator`; sealed mencegah tipe ini diturunkan lagi.
internal sealed class RiskLoanCalculator : IRiskLoanCalculator
// Membuka scope tipe RiskLoanCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `AnalyticsPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly AnalyticsPayloadReader _payloadReader = new();

    // Mendefinisikan metode `Compute` dengan hasil bertipe `AnalyticsRiskLoanMetrics`; operasi ini menangani compute. Masukan: Parameter `playerEvents`
    // bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event; Parameter `playerProjections` bertipe
    // `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections; Parameter `startingCoins` bertipe `int` membawa nilai starting
    // coins; Parameter `coinsNetEndGame` bertipe `double` membawa nilai coins net end game; Parameter `totalIncome` bertipe `double` membawa nilai
    // total pemasukan; Parameter `lifeRisks` bertipe `IReadOnlyCollection<RulesetLifeRiskDto>?` membawa nilai life risks; nilai null diizinkan ketika
    // data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
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
    // Membuka scope metode Compute; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
    {
        // Menyiapkan variabel lokal `riskEvents` untuk nilai risiko event dengan mematerialisasi urutan `playerEvents.Where(e => e.ActionType ==
        // ”RisikoKehidupan”)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var riskEvents = playerEvents.Where(e => e.ActionType == "RisikoKehidupan").ToList();
        // Menyiapkan variabel lokal `riskDefinitions` untuk nilai risiko definitions dengan membangun kamus dari `(lifeRisks ?? [])` dengan pemilihan
        // kunci/nilai `item => item.RiskCode`, `StringComparer.OrdinalIgnoreCase`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var riskDefinitions = (lifeRisks ?? [])
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary(item => item.RiskCode, StringComparer.OrdinalIgnoreCase); dalam
            // Compute; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(item => item.RiskCode, StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `riskCostsPerCard` untuk nilai risiko costs per kartu dengan objek baru bertipe `List<int>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var riskCostsPerCard = new List<int>();
        // Mengulangi setiap elemen `riskEvents`; elemen saat ini disimpan sebagai `riskEvent` bertipe `var` untuk diproses oleh badan loop dalam Compute.
        foreach (var riskEvent in riskEvents)
        // Membuka scope loop setiap riskEvent dari `riskEvents`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
        {
            // Menyiapkan variabel lokal `cost` untuk nilai biaya dengan hasil pemilihan bersyarat: ketika `TryReadRiskId(riskEvent.Payload, out var riskId) &&
            // riskDefinitions.TryGetValue(riskId, out var definition)` benar gunakan `definition.Direction == ”OUT” &&
            // definition.EffectType.Contains(”COIN_EFFECT”, StringComparison.OrdinalIgnoreCase) ? definition.Amount : 0`, jika tidak gunakan `playerProjections
            // .Where(p => p.Category == ”RISK_LIFE” && p.Direction == ”OUT” && (p.EventId == riskEvent.EventId || ReferencesRiskEvent(p.Reference,
            // riskEvent.EventId))) .Su...`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var cost = TryReadRiskId(riskEvent.Payload, out var riskId) &&
                       // Melanjutkan pengolahan dengan mencari kunci `riskId` pada `riskDefinitions`; hasil boolean menandakan kunci ditemukan dan argumen out menerima
                       // nilainya dalam Compute.
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
                    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(p => p.Category == ”RISK_LIFE” && dalam Compute; token pada baris ini
                    // menyambungkan bagian kode sebelum dan sesudahnya.
                    .Where(p => p.Category == "RISK_LIFE" &&
                                // Meneruskan fungsi lambda `p => p.Category == ”RISK_LIFE” && p.Direction == ”OUT” && (p.EventId == riskEvent.EventId ||
                                // ReferencesRiskEvent(p.Reference, riskEvent.EventId))` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                                // `playerProjections .Where`.
                                p.Direction == "OUT" &&
                                // Meneruskan `p.Reference` (nilai reference) sebagai argumen ke `ReferencesRiskEvent`; Meneruskan `riskEvent.EventId` (identitas unik event untuk
                                // pencatatan dan pemeriksaan duplikasi) sebagai argumen ke `ReferencesRiskEvent`.
                                (p.EventId == riskEvent.EventId || ReferencesRiskEvent(p.Reference, riskEvent.EventId)))
                    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Sum(p => p.Amount); dalam Compute; token pada baris ini menyambungkan bagian
                    // kode sebelum dan sesudahnya.
                    .Sum(p => p.Amount);
            // Menjalankan menambahkan `cost` ke `riskCostsPerCard` dalam Compute.
            riskCostsPerCard.Add(cost);
        // Menutup scope loop setiap riskEvent dari `riskEvents`; bagian berikut berada di luar batas blok tersebut dalam Compute.
        }

        // Menyiapkan variabel lokal `riskCostsTotal` untuk nilai risiko costs total dengan menjumlahkan nilai `riskCostsPerCard`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var riskCostsTotal = riskCostsPerCard.Sum();
        // Menyiapkan variabel lokal `riskCardsDrawn` untuk nilai risiko kartu drawn dengan `riskEvents.Count`, yaitu jumlah elemen atau panjang data. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var riskCardsDrawn = riskEvents.Count;
        // Menyiapkan variabel lokal `riskMitigated` untuk nilai risiko mitigated dengan memanggil `playerEvents.Count` dengan `e => e.ActionType ==
        // GameActionCatalog.Asuransi && e.Payload.Contains(”\”risk_event_id\””, StringComparison.OrdinalIgnoreCase)`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var riskMitigated = playerEvents.Count(e =>
            // Meneruskan fungsi lambda `e => e.ActionType == GameActionCatalog.Asuransi && e.Payload.Contains(”\”risk_event_id\””,
            // StringComparison.OrdinalIgnoreCase)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
            // `playerEvents.Count`.
            e.ActionType == GameActionCatalog.Asuransi &&
            // Meneruskan nilai literal `”\”risk_event_id\””` sebagai argumen ke `e.Payload.Contains`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai
            // ordinal ignore case) sebagai argumen ke `e.Payload.Contains`.
            e.Payload.Contains("\"risk_event_id\"", StringComparison.OrdinalIgnoreCase));
        // Menyiapkan variabel lokal `riskAccepted` untuk nilai risiko accepted dengan menentukan nilai terbesar dari `0`, `riskCardsDrawn - riskMitigated`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var riskAccepted = Math.Max(0, riskCardsDrawn - riskMitigated);
        // Menyiapkan variabel lokal `insurancePayments` untuk nilai asuransi payments dengan menjumlahkan nilai `playerProjections .Where(p => p.Category
        // == ”INSURANCE_PREMIUM” && p.Direction == ”OUT”)` berdasarkan `p => p.Amount`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var insurancePayments = playerProjections
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(p => p.Category == ”INSURANCE_PREMIUM” && p.Direction == ”OUT”) dalam
            // Compute; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(p => p.Category == "INSURANCE_PREMIUM" && p.Direction == "OUT")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Sum(p => p.Amount); dalam Compute; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .Sum(p => p.Amount);
        // Menyiapkan variabel lokal `emergencyOptionsUsed` untuk nilai emergency options used dengan memanggil `playerEvents.Count` dengan `e =>
        // e.ActionType == ”GunakanOpsiDarurat”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var emergencyOptionsUsed = playerEvents.Count(e => e.ActionType == "GunakanOpsiDarurat");

        // Menyiapkan variabel lokal `loanStates` untuk nilai pinjaman states dengan memanggil `BuildLoanStates` dengan `playerEvents`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var loanStates = BuildLoanStates(playerEvents);
        // Menyiapkan variabel lokal `loansTaken` untuk nilai pinjaman taken dengan `loanStates.Count`, yaitu jumlah elemen atau panjang data. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var loansTaken = loanStates.Count;
        // Menyiapkan variabel lokal `loansRepaid` untuk nilai pinjaman dilunasi dengan memanggil `loanStates.Values.Count` dengan `l => l.RepaidAmount >=
        // l.Principal`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var loansRepaid = loanStates.Values.Count(l => l.RepaidAmount >= l.Principal);
        // Menyiapkan variabel lokal `loansUnpaid` untuk nilai pinjaman unpaid dengan memanggil `loanStates.Values.Count` dengan `l => l.RepaidAmount <
        // l.Principal`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var loansUnpaid = loanStates.Values.Count(l => l.RepaidAmount < l.Principal);
        // Menyiapkan variabel lokal `loansOutstandingAmount` untuk nilai pinjaman belum dilunasi nominal dengan menjumlahkan nilai `loanStates.Values`
        // berdasarkan `l => Math.Max(0, l.Principal - l.RepaidAmount)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var loansOutstandingAmount = loanStates.Values.Sum(l => Math.Max(0, l.Principal - l.RepaidAmount));

        // Menyiapkan variabel lokal `averageRiskCost` untuk nilai rata-rata risiko biaya dengan hasil pemilihan bersyarat: ketika `riskCardsDrawn > 0`
        // benar gunakan `(double)riskCostsTotal / riskCardsDrawn`, jika tidak gunakan `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var averageRiskCost = riskCardsDrawn > 0 ? (double)riskCostsTotal / riskCardsDrawn : 0;
        // Menyiapkan variabel lokal `riskAcceptanceRate` untuk nilai risiko acceptance rate dengan memanggil `SafeRatio` dengan `riskAccepted`,
        // `riskCardsDrawn`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var riskAcceptanceRate = SafeRatio(riskAccepted, riskCardsDrawn);
        // Menyiapkan variabel lokal `insuranceCoverageRate` untuk nilai asuransi coverage rate dengan memanggil `SafeRatio` dengan `riskMitigated`,
        // `riskCardsDrawn`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var insuranceCoverageRate = SafeRatio(riskMitigated, riskCardsDrawn);
        // Menyiapkan variabel lokal `riskCostIntensity` untuk nilai risiko biaya intensity dengan memanggil `SafeRatio` dengan `averageRiskCost`,
        // `startingCoins`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var riskCostIntensity = SafeRatio(averageRiskCost, startingCoins);
        // Menyiapkan variabel lokal `riskAppetiteScore` untuk nilai risiko appetite skor dengan hasil pemilihan bersyarat: ketika
        // `riskAcceptanceRate.HasValue && riskCostIntensity.HasValue` benar gunakan `Clamp(riskAcceptanceRate.Value * riskCostIntensity.Value * 100, 0,
        // 100)`, jika tidak gunakan `(double?)null`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var riskAppetiteScore =
            // Melanjutkan ekspresi dengan gabungan syarat AND: kedua kondisi wajib benar antara `riskAcceptanceRate.HasValue` dan `riskCostIntensity.HasValue`;
            // sisi kanan diperiksa hanya jika sisi kiri benar dalam Compute.
            riskAcceptanceRate.HasValue &&
            // Menggunakan `riskCostIntensity` (nilai risiko biaya intensity) sebagai bagian ekspresi yang sedang disusun dalam Compute.
            riskCostIntensity.HasValue
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: Clamp(riskAcceptanceRate.Value * riskCostIntensity.Value * 100, 0,
                // 100) dalam Compute.
                ? Clamp(riskAcceptanceRate.Value * riskCostIntensity.Value * 100, 0, 100)
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (double?)null; dalam Compute.
                : (double?)null;

        // Mengembalikan objek baru bertipe `AnalyticsRiskLoanMetrics` dengan argumen ( riskCostsPerCard, riskCostsTotal, riskCardsDrawn, riskMitigated,
        // riskAccepted, insurancePayments, emergencyOptionsUsed, loansTaken, loansRepaid, loansUnpaid, ... kepada pemanggil dalam Compute; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return new AnalyticsRiskLoanMetrics(
            // Meneruskan `riskCostsPerCard` (nilai risiko costs per kartu) sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`.
            riskCostsPerCard,
            // Meneruskan `riskCostsTotal` (nilai risiko costs total) sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`.
            riskCostsTotal,
            // Meneruskan `riskCardsDrawn` (nilai risiko kartu drawn) sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`.
            riskCardsDrawn,
            // Meneruskan `riskMitigated` (nilai risiko mitigated) sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`.
            riskMitigated,
            // Meneruskan `riskAccepted` (nilai risiko accepted) sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`.
            riskAccepted,
            // Meneruskan `insurancePayments` (nilai asuransi payments) sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`.
            insurancePayments,
            // Meneruskan `emergencyOptionsUsed` (nilai emergency options used) sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`.
            emergencyOptionsUsed,
            // Meneruskan `loansTaken` (nilai pinjaman taken) sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`.
            loansTaken,
            // Meneruskan `loansRepaid` (nilai pinjaman dilunasi) sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`.
            loansRepaid,
            // Meneruskan `loansUnpaid` (nilai pinjaman unpaid) sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`.
            loansUnpaid,
            // Meneruskan `loansOutstandingAmount` (nilai pinjaman belum dilunasi nominal) sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`.
            loansOutstandingAmount,
            // Meneruskan memanggil `SafeRatio` dengan `riskCostsTotal`, `totalIncome`, `true` sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`;
            // Meneruskan `riskCostsTotal` (nilai risiko costs total) sebagai argumen ke `SafeRatio`; Meneruskan `totalIncome` (nilai total pemasukan) sebagai
            // argumen ke `SafeRatio`; Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `SafeRatio`.
            SafeRatio(riskCostsTotal, totalIncome, true),
            // Meneruskan memanggil `SafeRatio` dengan `riskMitigated`, `riskCardsDrawn`, `true` sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`;
            // Meneruskan `riskMitigated` (nilai risiko mitigated) sebagai argumen ke `SafeRatio`; Meneruskan `riskCardsDrawn` (nilai risiko kartu drawn)
            // sebagai argumen ke `SafeRatio`; Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `SafeRatio`.
            SafeRatio(riskMitigated, riskCardsDrawn, true),
            // Meneruskan `averageRiskCost` (nilai rata-rata risiko biaya) sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`.
            averageRiskCost,
            // Meneruskan `riskAcceptanceRate` (nilai risiko acceptance rate) sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`.
            riskAcceptanceRate,
            // Meneruskan `insuranceCoverageRate` (nilai asuransi coverage rate) sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`.
            insuranceCoverageRate,
            // Meneruskan `riskCostIntensity` (nilai risiko biaya intensity) sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`.
            riskCostIntensity,
            // Meneruskan `riskAppetiteScore` (nilai risiko appetite skor) sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`.
            riskAppetiteScore,
            // Meneruskan memanggil `SafeRatio` dengan `loansOutstandingAmount`, `coinsNetEndGame`, `true` sebagai argumen ke konstruktor
            // `AnalyticsRiskLoanMetrics`; Meneruskan `loansOutstandingAmount` (nilai pinjaman belum dilunasi nominal) sebagai argumen ke `SafeRatio`;
            // Meneruskan `coinsNetEndGame` (nilai coins net end game) sebagai argumen ke `SafeRatio`; Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai
            // argumen ke `SafeRatio`.
            SafeRatio(loansOutstandingAmount, coinsNetEndGame, true),
            // Meneruskan memanggil `SafeRatio` dengan `loansRepaid`, `loansTaken`, `true` sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`; Meneruskan
            // `loansRepaid` (nilai pinjaman dilunasi) sebagai argumen ke `SafeRatio`; Meneruskan `loansTaken` (nilai pinjaman taken) sebagai argumen ke
            // `SafeRatio`; Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke `SafeRatio`.
            SafeRatio(loansRepaid, loansTaken, true),
            // Meneruskan memanggil `SafeRatio` dengan `loansUnpaid`, `loansTaken` sebagai argumen ke konstruktor `AnalyticsRiskLoanMetrics`; Meneruskan
            // `loansUnpaid` (nilai pinjaman unpaid) sebagai argumen ke `SafeRatio`; Meneruskan `loansTaken` (nilai pinjaman taken) sebagai argumen ke
            // `SafeRatio`.
            SafeRatio(loansUnpaid, loansTaken));
    // Menutup scope metode Compute; bagian berikut berada di luar batas blok tersebut dalam Compute.
    }

    // Mendefinisikan metode `ReferencesRiskEvent` dengan hasil bertipe `bool`; operasi ini menangani references risiko event. Masukan: Parameter
    // `reference` bertipe `string?` membawa nilai reference; nilai null diizinkan ketika data opsional belum tersedia; Parameter `riskEventId` bertipe
    // `Guid` membawa nilai risiko event identitas. Nilai hasil langsung berasal dari gabungan syarat AND: kedua kondisi wajib benar antara
    // `Guid.TryParse(reference, out var referencedEventId)` dan `referencedEventId == riskEventId`; sisi kanan diperiksa hanya jika sisi kiri benar.
    private static bool ReferencesRiskEvent(string? reference, Guid riskEventId)
        // Melengkapi struktur ekspresi ArrowExpressionClause melalui => Guid.TryParse(reference, out var referencedEventId) && referencedEventId ==
        // riskEventId; dalam ReferencesRiskEvent; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
        => Guid.TryParse(reference, out var referencedEventId) && referencedEventId == riskEventId;

    // Mendefinisikan metode `TryReadRiskId` dengan hasil bertipe `bool`; operasi ini menangani try read risiko identitas. Masukan: Parameter `payload`
    // bertipe `string` membawa muatan detail event dalam format JSON; Parameter `riskId` bertipe `string` membawa nilai risiko identitas; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    private static bool TryReadRiskId(string payload, out string riskId)
    // Membuka scope metode TryReadRiskId; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadRiskId.
    {
        // Memperbarui `riskId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadRiskId.
        riskId = string.Empty;
        // Memulai blok try dalam TryReadRiskId; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadRiskId.
        {
            // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `System.Text.Json.JsonDocument.Parse` dengan `payload`. Tipe variabel
            // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var document = System.Text.Json.JsonDocument.Parse(payload);
            // Memeriksa kebalikan kondisi `document.RootElement.TryGetProperty(”risk_id”, out var riskIdElement)`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam TryReadRiskId.
            if (!document.RootElement.TryGetProperty("risk_id", out var riskIdElement))
            // Membuka scope cabang if untuk kondisi `!document.RootElement.TryGetProperty(”risk_id”, out var riskIdElement)`; pernyataan/deklarasi berikut
            // berada di dalam batas blok ini dalam TryReadRiskId.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadRiskId; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!document.RootElement.TryGetProperty(”risk_id”, out var riskIdElement)`; bagian berikut berada di luar
            // batas blok tersebut dalam TryReadRiskId.
            }

            // Memperbarui `riskId` menggunakan `riskIdElement.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
            // TryReadRiskId.
            riskId = riskIdElement.GetString() ?? string.Empty;
            // Mengembalikan pemeriksaan lebih besar antara `riskId.Length` dan `0` kepada pemanggil dalam TryReadRiskId; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return riskId.Length > 0;
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadRiskId.
        }
        // Menangani exception `System.Text.Json.JsonException` melalui variabel dalam TryReadRiskId.
        catch (System.Text.Json.JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadRiskId.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadRiskId; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadRiskId.
        }
    // Menutup scope metode TryReadRiskId; bagian berikut berada di luar batas blok tersebut dalam TryReadRiskId.
    }

    // Mendefinisikan metode `BuildLoanStates` dengan hasil bertipe `Dictionary<string, LoanState>`; operasi ini menangani build pinjaman states.
    // Masukan: Parameter `playerEvents` bertipe `IEnumerable<EventDb>` membawa nilai pemain event.
    private Dictionary<string, LoanState> BuildLoanStates(IEnumerable<EventDb> playerEvents)
    // Membuka scope metode BuildLoanStates; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildLoanStates.
    {
        // Menyiapkan variabel lokal `loanStates` untuk nilai pinjaman states dengan objek baru bertipe `Dictionary<string, LoanState>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var loanStates = new Dictionary<string, LoanState>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `playerEvents`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh badan loop dalam
        // BuildLoanStates.
        foreach (var evt in playerEvents)
        // Membuka scope loop setiap evt dari `playerEvents`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildLoanStates.
        {
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `(evt.ActionType == GameActionCatalog.PinjamanSyariah || evt.ActionType ==
            // GameActionCatalog.SetupPinjamanAwal || IsEmergencyLoan(evt.Payload))` dan `_payloadReader.TryReadLoanTaken(evt.Payload, out var loanId, out var
            // principal, out var penaltyPoints)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar
            // dalam BuildLoanStates.
            if ((evt.ActionType == GameActionCatalog.PinjamanSyariah ||
                 // Melanjutkan ekspresi dengan perbandingan kesamaan antara `evt.ActionType` dan `GameActionCatalog.SetupPinjamanAwal` dalam BuildLoanStates.
                 evt.ActionType == GameActionCatalog.SetupPinjamanAwal ||
                 // Melanjutkan pengolahan dengan memanggil `IsEmergencyLoan` dengan `evt.Payload` dalam BuildLoanStates.
                 IsEmergencyLoan(evt.Payload)) &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadLoanTaken` dengan `evt.Payload`, `var loanId`, `var principal`, `var
                // penaltyPoints` dalam BuildLoanStates.
                _payloadReader.TryReadLoanTaken(evt.Payload, out var loanId, out var principal, out var penaltyPoints))
            // Membuka scope cabang if untuk kondisi `(evt.ActionType == GameActionCatalog.PinjamanSyariah || evt.ActionType ==
            // GameActionCatalog.SetupPinjamanAwal || IsEmergencyLoan(evt.Payload)) && _payloadReader.TryReadLoanTak...`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam BuildLoanStates.
            {
                // Memperbarui `loanStates[loanId]` menggunakan objek baru bertipe `LoanState` dengan argumen (loanId, principal, penaltyPoints, 0) dalam
                // BuildLoanStates.
                loanStates[loanId] = new LoanState(loanId, principal, penaltyPoints, 0);
            // Menutup scope cabang if untuk kondisi `(evt.ActionType == GameActionCatalog.PinjamanSyariah || evt.ActionType ==
            // GameActionCatalog.SetupPinjamanAwal || IsEmergencyLoan(evt.Payload)) && _payloadReader.TryReadLoanTak...`; bagian berikut berada di luar batas
            // blok tersebut dalam BuildLoanStates.
            }

            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `evt.ActionType == ”BayarPinjaman” &&
            // _payloadReader.TryReadLoanRepay(evt.Payload, out var repayLoanId, out var repayAmount)` dan `loanStates.TryGetValue(repayLoanId, out var state)`;
            // sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildLoanStates.
            if (evt.ActionType == "BayarPinjaman" &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadLoanRepay` dengan `evt.Payload`, `var repayLoanId`, `var repayAmount` dalam
                // BuildLoanStates.
                _payloadReader.TryReadLoanRepay(evt.Payload, out var repayLoanId, out var repayAmount) &&
                // Melanjutkan pengolahan dengan mencari kunci `repayLoanId` pada `loanStates`; hasil boolean menandakan kunci ditemukan dan argumen out menerima
                // nilainya dalam BuildLoanStates.
                loanStates.TryGetValue(repayLoanId, out var state))
            // Membuka scope cabang if untuk kondisi `evt.ActionType == ”BayarPinjaman” && _payloadReader.TryReadLoanRepay(evt.Payload, out var repayLoanId, out
            // var repayAmount) && loanStates.TryGetValue(repayLoanId, out var stat...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildLoanStates.
            {
                // Memperbarui `loanStates[repayLoanId]` menggunakan `state with { RepaidAmount = state.RepaidAmount + repayAmount }` dalam BuildLoanStates.
                loanStates[repayLoanId] = state with { RepaidAmount = state.RepaidAmount + repayAmount };
            // Menutup scope cabang if untuk kondisi `evt.ActionType == ”BayarPinjaman” && _payloadReader.TryReadLoanRepay(evt.Payload, out var repayLoanId, out
            // var repayAmount) && loanStates.TryGetValue(repayLoanId, out var stat...`; bagian berikut berada di luar batas blok tersebut dalam
            // BuildLoanStates.
            }
        // Menutup scope loop setiap evt dari `playerEvents`; bagian berikut berada di luar batas blok tersebut dalam BuildLoanStates.
        }

        // Mengembalikan `loanStates` (nilai pinjaman states) kepada pemanggil dalam BuildLoanStates; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return loanStates;
    // Menutup scope metode BuildLoanStates; bagian berikut berada di luar batas blok tersebut dalam BuildLoanStates.
    }

    // Mendefinisikan metode `IsEmergencyLoan` dengan hasil bertipe `bool`; operasi ini menangani berstatus emergency pinjaman. Masukan: Parameter
    // `payload` bertipe `string` membawa muatan detail event dalam format JSON.
    private static bool IsEmergencyLoan(string payload)
    // Membuka scope metode IsEmergencyLoan; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IsEmergencyLoan.
    {
        // Memulai blok try dalam IsEmergencyLoan; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IsEmergencyLoan.
        {
            // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `System.Text.Json.JsonDocument.Parse` dengan `payload`. Tipe variabel
            // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var document = System.Text.Json.JsonDocument.Parse(payload);
            // Mengembalikan gabungan syarat AND: kedua kondisi wajib benar antara `document.RootElement.TryGetProperty(”option_type”, out var optionType)` dan
            // `string.Equals(optionType.GetString(), ”TAKE_SHARIA_LOAN”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri benar
            // kepada pemanggil dalam IsEmergencyLoan; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return document.RootElement.TryGetProperty("option_type", out var optionType) &&
                   // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `optionType.GetString()`, `”TAKE_SHARIA_LOAN”`,
                   // `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan comparer yang diberikan dalam IsEmergencyLoan.
                   string.Equals(optionType.GetString(), "TAKE_SHARIA_LOAN", StringComparison.OrdinalIgnoreCase);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam IsEmergencyLoan.
        }
        // Menangani exception `System.Text.Json.JsonException` melalui variabel dalam IsEmergencyLoan.
        catch (System.Text.Json.JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IsEmergencyLoan.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam IsEmergencyLoan; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam IsEmergencyLoan.
        }
    // Menutup scope metode IsEmergencyLoan; bagian berikut berada di luar batas blok tersebut dalam IsEmergencyLoan.
    }

    // Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `LoanState`; sealed mencegah tipe ini diturunkan lagi.
    private sealed record LoanState(
        // Parameter `LoanId` bertipe `string` membawa nilai pinjaman identitas.
        string LoanId,
        // Parameter `Principal` bertipe `int` membawa nilai principal.
        int Principal,
        // Parameter `PenaltyPoints` bertipe `int` membawa nilai penalti poin.
        int PenaltyPoints,
        // Parameter `RepaidAmount` bertipe `double` membawa nilai dilunasi nominal.
        double RepaidAmount);
// Menutup scope tipe RiskLoanCalculator; bagian berikut berada di luar batas blok tersebut.
}
