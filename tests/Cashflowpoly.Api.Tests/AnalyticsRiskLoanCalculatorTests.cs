// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsRiskLoanCalculatorTests.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `AnalyticsRiskLoanCalculatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AnalyticsRiskLoanCalculatorTests
// Membuka scope tipe AnalyticsRiskLoanCalculatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_IncludesSetupAndEmergencyLoanInstances` dengan hasil bertipe `void`; operasi ini menangani compute includes setup
    // dan emergency pinjaman instances.
    public void Compute_IncludesSetupAndEmergencyLoanInstances()
    // Membuka scope metode Compute_IncludesSetupAndEmergencyLoanInstances; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Compute_IncludesSetupAndEmergencyLoanInstances.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_IncludesSetupAndEmergencyLoanInstances.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”SetupPinjamanAwal”`,
            // `”””{”loan_id”:”setup-loan”,”principal”:10,”penalty_points”:15}”””` dalam Compute_IncludesSetupAndEmergencyLoanInstances.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "SetupPinjamanAwal", """{"loan_id":"setup-loan","principal":10,"penalty_points":15}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”GunakanOpsiDarurat”`,
            // `”””{”option_type”:”TAKE_SHARIA_LOAN”,”loan_id”:”emergency-loan”,”principal”:10,”penalty_points”:15}”””` dalam
            // Compute_IncludesSetupAndEmergencyLoanInstances.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "GunakanOpsiDarurat", """{"option_type":"TAKE_SHARIA_LOAN","loan_id":"emergency-loan","principal":10,"penalty_points":15}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”BayarPinjaman”`,
            // `”””{”loan_id”:”setup-loan”,”amount”:10}”””` dalam Compute_IncludesSetupAndEmergencyLoanInstances.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "BayarPinjaman", """{"loan_id":"setup-loan","amount":10}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_IncludesSetupAndEmergencyLoanInstances.
        };

        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new RiskLoanCalculator().Compute` dengan `events`, `[]`, `10`, `10`,
        // `20`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new RiskLoanCalculator().Compute(events, [], 10, 10, 20);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `metrics.LoansTaken`); pengujian gagal
        // jika keduanya berbeda dalam Compute_IncludesSetupAndEmergencyLoanInstances.
        Assert.Equal(2, metrics.LoansTaken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.LoansRepaid`); pengujian gagal
        // jika keduanya berbeda dalam Compute_IncludesSetupAndEmergencyLoanInstances.
        Assert.Equal(1, metrics.LoansRepaid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.LoansUnpaid`); pengujian gagal
        // jika keduanya berbeda dalam Compute_IncludesSetupAndEmergencyLoanInstances.
        Assert.Equal(1, metrics.LoansUnpaid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`, `metrics.LoansOutstandingAmount`);
        // pengujian gagal jika keduanya berbeda dalam Compute_IncludesSetupAndEmergencyLoanInstances.
        Assert.Equal(10, metrics.LoansOutstandingAmount);
    // Menutup scope metode Compute_IncludesSetupAndEmergencyLoanInstances; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_IncludesSetupAndEmergencyLoanInstances.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_SummarizesRiskAndLoanMetrics` dengan hasil bertipe `void`; operasi ini menangani compute summarizes risiko dan
    // pinjaman metrics.
    public void Compute_SummarizesRiskAndLoanMetrics()
    // Membuka scope metode Compute_SummarizesRiskAndLoanMetrics; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Compute_SummarizesRiskAndLoanMetrics.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `riskOne` untuk nilai risiko one dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var riskOne = Guid.NewGuid();
        // Menyiapkan variabel lokal `riskTwo` untuk nilai risiko two dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var riskTwo = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_SummarizesRiskAndLoanMetrics.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `riskOne`, `sessionId`, `playerId`, `”RisikoKehidupan”`,
            // `”””{”risk_id”:”risk-1”,”direction”:”OUT”,”amount”:6}”””` dalam Compute_SummarizesRiskAndLoanMetrics.
            CreateEvent(riskOne, sessionId, playerId, "RisikoKehidupan", """{"risk_id":"risk-1","direction":"OUT","amount":6}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `riskTwo`, `sessionId`, `playerId`, `”RisikoKehidupan”`,
            // `”””{”risk_id”:”risk-2”,”direction”:”OUT”,”amount”:4}”””` dalam Compute_SummarizesRiskAndLoanMetrics.
            CreateEvent(riskTwo, sessionId, playerId, "RisikoKehidupan", """{"risk_id":"risk-2","direction":"OUT","amount":4}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”Asuransi”`,
            // `”””{”risk_event_id”:”risk-1”}”””` dalam Compute_SummarizesRiskAndLoanMetrics.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "Asuransi", """{"risk_event_id":"risk-1"}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”GunakanOpsiDarurat”`,
            // `”””{”risk_event_id”:”risk-2”,”option_type”:”OTHER”,”direction”:”OUT”,”amount”:2}”””` dalam Compute_SummarizesRiskAndLoanMetrics.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "GunakanOpsiDarurat", """{"risk_event_id":"risk-2","option_type":"OTHER","direction":"OUT","amount":2}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”PinjamanSyariah”`,
            // `”””{”loan_id”:”loan-a”,”principal”:10,”repayment_amount”:5,”duration_turns”:2,”penalty_points”:15}”””` dalam
            // Compute_SummarizesRiskAndLoanMetrics.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "PinjamanSyariah", """{"loan_id":"loan-a","principal":10,"repayment_amount":5,"duration_turns":2,"penalty_points":15}"""),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”BayarPinjaman”`,
            // `”””{”loan_id”:”loan-a”,”amount”:4}”””` dalam Compute_SummarizesRiskAndLoanMetrics.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "BayarPinjaman", """{"loan_id":"loan-a","amount":4}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_SummarizesRiskAndLoanMetrics.
        };
        // Menyiapkan variabel lokal `projections` untuk proyeksi transaksi arus kas yang diturunkan dari event permainan dengan objek baru bertipe
        // `List<CashflowProjectionDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projections = new List<CashflowProjectionDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_SummarizesRiskAndLoanMetrics.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `riskOne`, `sessionId`, `playerId`, `”OUT”`, `6`, `”RISK_LIFE”` dalam
            // Compute_SummarizesRiskAndLoanMetrics.
            CreateProjection(riskOne, sessionId, playerId, "OUT", 6, "RISK_LIFE"),
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”OUT”`, `4`, `”RISK_LIFE”`,
            // `riskTwo.ToString()` dalam Compute_SummarizesRiskAndLoanMetrics.
            CreateProjection(Guid.NewGuid(), sessionId, playerId, "OUT", 4, "RISK_LIFE", riskTwo.ToString()),
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”OUT”`, `1`, `”INSURANCE_PREMIUM”`
            // dalam Compute_SummarizesRiskAndLoanMetrics.
            CreateProjection(Guid.NewGuid(), sessionId, playerId, "OUT", 1, "INSURANCE_PREMIUM")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_SummarizesRiskAndLoanMetrics.
        };

        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new RiskLoanCalculator().Compute` dengan `events`, `projections`, `20`,
        // `30`, `50`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new RiskLoanCalculator().Compute(events, projections, startingCoins: 20, coinsNetEndGame: 30, totalIncome: 50);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { 6, 4 }`, `metrics.RiskCostsPerCard`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(new[] { 6, 4 }, metrics.RiskCostsPerCard);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`, `metrics.RiskCostsTotal`); pengujian
        // gagal jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(10, metrics.RiskCostsTotal);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `metrics.RiskCardsDrawn`); pengujian gagal
        // jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(2, metrics.RiskCardsDrawn);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.RiskMitigated`); pengujian gagal
        // jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(1, metrics.RiskMitigated);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.RiskAccepted`); pengujian gagal
        // jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(1, metrics.RiskAccepted);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.InsurancePayments`); pengujian
        // gagal jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(1, metrics.InsurancePayments);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.EmergencyOptionsUsed`); pengujian
        // gagal jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(1, metrics.EmergencyOptionsUsed);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.LoansTaken`); pengujian gagal
        // jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(1, metrics.LoansTaken);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `metrics.LoansRepaid`); pengujian gagal
        // jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(0, metrics.LoansRepaid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.LoansUnpaid`); pengujian gagal
        // jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(1, metrics.LoansUnpaid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`6`, `metrics.LoansOutstandingAmount`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(6, metrics.LoansOutstandingAmount);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`20`, `metrics.RiskExposurePercentage`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(20, metrics.RiskExposurePercentage);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`50`, `metrics.RiskMitigationEffectiveness`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(50, metrics.RiskMitigationEffectiveness);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`5`, `metrics.AverageRiskCost`); pengujian
        // gagal jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(5, metrics.AverageRiskCost);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0.5`, `metrics.RiskAcceptanceRate`); pengujian
        // gagal jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(0.5, metrics.RiskAcceptanceRate);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0.5`, `metrics.InsuranceCoverageRate`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(0.5, metrics.InsuranceCoverageRate);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0.25`, `metrics.RiskCostIntensity`); pengujian
        // gagal jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(0.25, metrics.RiskCostIntensity);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`12.5`, `metrics.RiskAppetiteScore`); pengujian
        // gagal jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(12.5, metrics.RiskAppetiteScore);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`20`, `metrics.DebtLeverageRatio`); pengujian
        // gagal jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(20, metrics.DebtLeverageRatio);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `metrics.LoanRepaymentDiscipline`);
        // pengujian gagal jika keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(0, metrics.LoanRepaymentDiscipline);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.DebtRatio`); pengujian gagal jika
        // keduanya berbeda dalam Compute_SummarizesRiskAndLoanMetrics.
        Assert.Equal(1, metrics.DebtRatio);
    // Menutup scope metode Compute_SummarizesRiskAndLoanMetrics; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_SummarizesRiskAndLoanMetrics.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_RiskAppetiteIncreasesWhenUnprotectedRisksAreAccepted` dengan hasil bertipe `void`; operasi ini menangani compute
    // risiko appetite increases when unprotected risks are accepted.
    public void Compute_RiskAppetiteIncreasesWhenUnprotectedRisksAreAccepted()
    // Membuka scope metode Compute_RiskAppetiteIncreasesWhenUnprotectedRisksAreAccepted; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam Compute_RiskAppetiteIncreasesWhenUnprotectedRisksAreAccepted.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `risk` untuk nilai risiko dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var risk = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan array baru dengan
        // tipe elemen disimpulkan dari nilai initializer. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_RiskAppetiteIncreasesWhenUnprotectedRisksAreAccepted.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `risk`, `sessionId`, `playerId`, `”RisikoKehidupan”`,
            // `”””{”risk_id”:”risk-1”,”direction”:”OUT”,”amount”:10}”””` dalam Compute_RiskAppetiteIncreasesWhenUnprotectedRisksAreAccepted.
            CreateEvent(risk, sessionId, playerId, "RisikoKehidupan", """{"risk_id":"risk-1","direction":"OUT","amount":10}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_RiskAppetiteIncreasesWhenUnprotectedRisksAreAccepted.
        };
        // Menyiapkan variabel lokal `projections` untuk proyeksi transaksi arus kas yang diturunkan dari event permainan dengan array baru dengan tipe
        // elemen disimpulkan dari nilai initializer. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projections = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute_RiskAppetiteIncreasesWhenUnprotectedRisksAreAccepted.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `risk`, `sessionId`, `playerId`, `”OUT”`, `10`, `”RISK_LIFE”` dalam
            // Compute_RiskAppetiteIncreasesWhenUnprotectedRisksAreAccepted.
            CreateProjection(risk, sessionId, playerId, "OUT", 10, "RISK_LIFE")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Compute_RiskAppetiteIncreasesWhenUnprotectedRisksAreAccepted.
        };

        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new RiskLoanCalculator().Compute` dengan `events`, `projections`, `20`,
        // `10`, `20`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metrics = new RiskLoanCalculator().Compute(events, projections, startingCoins: 20, coinsNetEndGame: 10, totalIncome: 20);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `metrics.RiskAcceptanceRate`); pengujian
        // gagal jika keduanya berbeda dalam Compute_RiskAppetiteIncreasesWhenUnprotectedRisksAreAccepted.
        Assert.Equal(1, metrics.RiskAcceptanceRate);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `metrics.InsuranceCoverageRate`);
        // pengujian gagal jika keduanya berbeda dalam Compute_RiskAppetiteIncreasesWhenUnprotectedRisksAreAccepted.
        Assert.Equal(0, metrics.InsuranceCoverageRate);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`50`, `metrics.RiskAppetiteScore`); pengujian
        // gagal jika keduanya berbeda dalam Compute_RiskAppetiteIncreasesWhenUnprotectedRisksAreAccepted.
        Assert.Equal(50, metrics.RiskAppetiteScore);
    // Menutup scope metode Compute_RiskAppetiteIncreasesWhenUnprotectedRisksAreAccepted; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_RiskAppetiteIncreasesWhenUnprotectedRisksAreAccepted.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Compute_UsesCardNominalEvenWhenPaymentIsNotYetRecorded` dengan hasil bertipe `void`; operasi ini menangani compute uses
    // kartu nominal even when payment berstatus not yet recorded.
    public void Compute_UsesCardNominalEvenWhenPaymentIsNotYetRecorded()
    // Membuka scope metode Compute_UsesCardNominalEvenWhenPaymentIsNotYetRecorded; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Compute_UsesCardNominalEvenWhenPaymentIsNotYetRecorded.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `riskEvent` untuk nilai risiko event dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`,
        // `”RisikoKehidupan”`, `”””{”risk_id”:”surgery”}”””`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var riskEvent = CreateEvent(
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke `CreateEvent`.
            Guid.NewGuid(),
            // Meneruskan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke `CreateEvent`.
            sessionId,
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke `CreateEvent`.
            playerId,
            // Meneruskan nilai literal `”RisikoKehidupan”` sebagai argumen ke `CreateEvent`.
            "RisikoKehidupan",
            // Meneruskan nilai literal `”””{”risk_id”:”surgery”}”””` sebagai argumen ke `CreateEvent`.
            """{"risk_id":"surgery"}""");

        // Menyiapkan variabel lokal `metrics` untuk nilai metrics dengan memanggil `new RiskLoanCalculator().Compute` dengan `[riskEvent]`, `[]`, `10`,
        // `10`, `10`, `[new RulesetLifeRiskDto { RiskCode = ”surgery”, EffectType = ”COIN_EFFECT”, Direction = ”OUT”, Amount = 6 }]`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var metrics = new RiskLoanCalculator().Compute(
            // Meneruskan koleksi berisi riskEvent sebagai argumen ke `new RiskLoanCalculator().Compute`.
            [riskEvent],
            // Meneruskan koleksi kosong dengan tipe mengikuti konteks tujuan sebagai argumen ke `new RiskLoanCalculator().Compute`.
            [],
            // Meneruskan nilai literal `10` sebagai argumen bernama `startingCoins`.
            startingCoins: 10,
            // Meneruskan nilai literal `10` sebagai argumen bernama `coinsNetEndGame`.
            coinsNetEndGame: 10,
            // Meneruskan nilai literal `10` sebagai argumen bernama `totalIncome`.
            totalIncome: 10,
            // Meneruskan koleksi berisi new RulesetLifeRiskDto { RiskCode = ”surgery”, Eff... sebagai argumen ke `new RiskLoanCalculator().Compute`.
            [new RulesetLifeRiskDto
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Compute_UsesCardNominalEvenWhenPaymentIsNotYetRecorded.
            {
                // Memperbarui `RiskCode` menggunakan nilai literal `”surgery”` dalam Compute_UsesCardNominalEvenWhenPaymentIsNotYetRecorded.
                RiskCode = "surgery",
                // Memperbarui `EffectType` menggunakan nilai literal `”COIN_EFFECT”` dalam Compute_UsesCardNominalEvenWhenPaymentIsNotYetRecorded.
                EffectType = "COIN_EFFECT",
                // Memperbarui `Direction` menggunakan nilai literal `”OUT”` dalam Compute_UsesCardNominalEvenWhenPaymentIsNotYetRecorded.
                Direction = "OUT",
                // Memperbarui `Amount` menggunakan nilai literal `6` dalam Compute_UsesCardNominalEvenWhenPaymentIsNotYetRecorded.
                Amount = 6
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
            // Compute_UsesCardNominalEvenWhenPaymentIsNotYetRecorded.
            }]);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`[6]`, `metrics.RiskCostsPerCard`); pengujian
        // gagal jika keduanya berbeda dalam Compute_UsesCardNominalEvenWhenPaymentIsNotYetRecorded.
        Assert.Equal([6], metrics.RiskCostsPerCard);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`6`, `metrics.RiskCostsTotal`); pengujian gagal
        // jika keduanya berbeda dalam Compute_UsesCardNominalEvenWhenPaymentIsNotYetRecorded.
        Assert.Equal(6, metrics.RiskCostsTotal);
    // Menutup scope metode Compute_UsesCardNominalEvenWhenPaymentIsNotYetRecorded; bagian berikut berada di luar batas blok tersebut dalam
    // Compute_UsesCardNominalEvenWhenPaymentIsNotYetRecorded.
    }

    // Mendefinisikan metode `CreateEvent` dengan hasil bertipe `EventDb`; operasi ini menangani create event. Masukan: Parameter `eventId` bertipe
    // `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi
    // permainan yang menjadi batas data operasi ini; Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas; Parameter `actionType` bertipe
    // `string` membawa nilai aksi jenis; Parameter `payload` bertipe `string` membawa muatan detail event dalam format JSON.
    private static EventDb CreateEvent(Guid eventId, Guid sessionId, Guid playerId, string actionType, string payload)
    // Membuka scope metode CreateEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
    {
        // Mengembalikan objek baru bertipe `EventDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam CreateEvent; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return new EventDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
        {
            // Memperbarui `EventId` menggunakan `eventId` (identitas unik event untuk pencatatan dan pemeriksaan duplikasi) dalam CreateEvent.
            EventId = eventId,
            // Memperbarui `SessionId` menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam CreateEvent.
            SessionId = sessionId,
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam CreateEvent.
            UserId = playerId,
            // Memperbarui `ActorType` menggunakan nilai literal `”PLAYER”` dalam CreateEvent.
            ActorType = "PLAYER",
            // Memperbarui `Timestamp` menggunakan objek baru bertipe `DateTimeOffset` dengan argumen (2026, 1, 2, 3, 4, 5, TimeSpan.Zero) dalam CreateEvent.
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            // Memperbarui `DayIndex` menggunakan nilai literal `0` dalam CreateEvent.
            DayIndex = 0,
            // Memperbarui `Weekday` menggunakan nilai literal `”MON”` dalam CreateEvent.
            Weekday = "MON",
            // Memperbarui `ActionSlot` menggunakan nilai literal `1` dalam CreateEvent.
            ActionSlot = 1,
            // Memperbarui `SequenceNumber` menggunakan nilai literal `1` dalam CreateEvent.
            SequenceNumber = 1,
            // Memperbarui `ActionType` menggunakan `actionType` (nilai aksi jenis) dalam CreateEvent.
            ActionType = actionType,
            // Memperbarui `RulesetVersionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateEvent.
            RulesetVersionId = Guid.NewGuid(),
            // Memperbarui `Payload` menggunakan `payload` (muatan detail event dalam format JSON) dalam CreateEvent.
            Payload = payload
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
        };
    // Menutup scope metode CreateEvent; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
    }

    // Mendefinisikan metode `CreateProjection` dengan hasil bertipe `CashflowProjectionDb`; operasi ini menangani create projection. Masukan: Parameter
    // `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter `sessionId` bertipe `Guid` membawa
    // identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas; Parameter
    // `direction` bertipe `string` membawa nilai direction; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai
    // dalam operasi; Parameter `category` bertipe `string` membawa nilai category; Parameter `reference` bertipe `string?` membawa nilai reference;
    // nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
    private static CashflowProjectionDb CreateProjection(
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas.
        Guid playerId,
        // Parameter `direction` bertipe `string` membawa nilai direction.
        string direction,
        // Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi.
        int amount,
        // Parameter `category` bertipe `string` membawa nilai category.
        string category,
        // Parameter `reference` bertipe `string?` membawa nilai reference; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak
        // diberikan digunakan null, yaitu penanda tidak ada nilai.
        string? reference = null)
    // Membuka scope metode CreateProjection; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateProjection.
    {
        // Mengembalikan objek baru bertipe `CashflowProjectionDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam CreateProjection; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return new CashflowProjectionDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateProjection.
        {
            // Memperbarui `ProjectionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateProjection.
            ProjectionId = Guid.NewGuid(),
            // Memperbarui `SessionId` menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam CreateProjection.
            SessionId = sessionId,
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam CreateProjection.
            UserId = playerId,
            // Memperbarui `EventPk` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateProjection.
            EventPk = Guid.NewGuid(),
            // Memperbarui `EventId` menggunakan `eventId` (identitas unik event untuk pencatatan dan pemeriksaan duplikasi) dalam CreateProjection.
            EventId = eventId,
            // Memperbarui `Timestamp` menggunakan objek baru bertipe `DateTimeOffset` dengan argumen (2026, 1, 2, 3, 4, 5, TimeSpan.Zero) dalam
            // CreateProjection.
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            // Memperbarui `Direction` menggunakan `direction` (nilai direction) dalam CreateProjection.
            Direction = direction,
            // Memperbarui `Amount` menggunakan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam CreateProjection.
            Amount = amount,
            // Memperbarui `Category` menggunakan `category` (nilai category) dalam CreateProjection.
            Category = category,
            // Memperbarui `Reference` menggunakan `reference` (nilai reference) dalam CreateProjection.
            Reference = reference
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateProjection.
        };
    // Menutup scope metode CreateProjection; bagian berikut berada di luar batas blok tersebut dalam CreateProjection.
    }
// Menutup scope tipe AnalyticsRiskLoanCalculatorTests; bagian berikut berada di luar batas blok tersebut.
}
