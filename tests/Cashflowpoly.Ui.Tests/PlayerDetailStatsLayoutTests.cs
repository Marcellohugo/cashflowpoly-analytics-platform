// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui PlayerDetailStatsLayoutTests.
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `PlayerDetailStatsLayoutTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerDetailStatsLayoutTests
// Membuka scope tipe PlayerDetailStatsLayoutTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `RepoRoot` menyimpan nilai repo root dengan nilai awal memanggil `ResolveRepositoryRoot` dengan tanpa
    // argumen. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan
    // dibagikan antar instance.
    private static readonly string RepoRoot = ResolveRepositoryRoot();
    // Mendeklarasikan field bertipe `string`: `UiRoot` menyimpan nilai ui root dengan nilai awal memanggil `Path.Combine` dengan `RepoRoot`, `”src”`,
    // `”Cashflowpoly.Ui”`. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik
    // tipe dan dibagikan antar instance.
    private static readonly string UiRoot = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui");

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerDetails_ShouldRenderLayeredPlayerAnalysis` dengan hasil bertipe `void`; operasi ini menangani pemain rincian should
    // render layered pemain analysis.
    public void PlayerDetails_ShouldRenderLayeredPlayerAnalysis()
    // Membuka scope metode PlayerDetails_ShouldRenderLayeredPlayerAnalysis; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // PlayerDetails_ShouldRenderLayeredPlayerAnalysis.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-analysis-overview”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderLayeredPlayerAnalysis.
        Assert.Contains("player-analysis-overview", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-analysis-scorecard”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderLayeredPlayerAnalysis.
        Assert.Contains("player-analysis-scorecard", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-analysis-atlas”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderLayeredPlayerAnalysis.
        Assert.Contains("player-analysis-atlas", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-analysis-chapters”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderLayeredPlayerAnalysis.
        Assert.Contains("player-analysis-chapters", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-analysis-card__takeaway”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderLayeredPlayerAnalysis.
        Assert.Contains("player-analysis-card__takeaway", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-analysis-grid”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderLayeredPlayerAnalysis.
        Assert.Contains("player-analysis-grid", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-evidence-library”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderLayeredPlayerAnalysis.
        Assert.Contains("player-evidence-library", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-stats-actions”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderLayeredPlayerAnalysis.
        Assert.Contains("player-stats-actions", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-stats-pillar-grid”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderLayeredPlayerAnalysis.
        Assert.DoesNotContain("player-stats-pillar-grid", view, StringComparison.Ordinal);
    // Menutup scope metode PlayerDetails_ShouldRenderLayeredPlayerAnalysis; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerDetails_ShouldRenderLayeredPlayerAnalysis.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerSummary_ShouldAlwaysShowCollectionMissionInsteadOfActionEfficiency` dengan hasil bertipe `void`; operasi ini
    // menangani pemain summary should always show collection misi instead of aksi efficiency.
    public void PlayerSummary_ShouldAlwaysShowCollectionMissionInsteadOfActionEfficiency()
    // Membuka scope metode PlayerSummary_ShouldAlwaysShowCollectionMissionInsteadOfActionEfficiency; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam PlayerSummary_ShouldAlwaysShowCollectionMissionInsteadOfActionEfficiency.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-analysis-scorecard__mission”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerSummary_ShouldAlwaysShowCollectionMissionInsteadOfActionEfficiency.
        Assert.Contains("player-analysis-scorecard__mission", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”statSummary.CollectionMissionComplete
        // switch”`, `view`, `StringComparison.Ordinal` dalam PlayerSummary_ShouldAlwaysShowCollectionMissionInsteadOfActionEfficiency.
        Assert.Contains("statSummary.CollectionMissionComplete switch", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”actionEfficiencyMetric”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerSummary_ShouldAlwaysShowCollectionMissionInsteadOfActionEfficiency.
        Assert.DoesNotContain("actionEfficiencyMetric", view, StringComparison.Ordinal);
    // Menutup scope metode PlayerSummary_ShouldAlwaysShowCollectionMissionInsteadOfActionEfficiency; bagian berikut berada di luar batas blok tersebut
    // dalam PlayerSummary_ShouldAlwaysShowCollectionMissionInsteadOfActionEfficiency.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerDetails_ShouldNotRenderStandaloneTransactionHistory` dengan hasil bertipe `void`; operasi ini menangani pemain
    // rincian should not render standalone transaction history.
    public void PlayerDetails_ShouldNotRenderStandaloneTransactionHistory()
    // Membuka scope metode PlayerDetails_ShouldNotRenderStandaloneTransactionHistory; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // PlayerDetails_ShouldNotRenderStandaloneTransactionHistory.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        // Menyiapkan variabel lokal `css` untuk nilai css dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”wwwroot”, ”css”, ”site.css”)`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-transaction-history”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldNotRenderStandaloneTransactionHistory.
        Assert.DoesNotContain("player-transaction-history", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-transactions-section”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldNotRenderStandaloneTransactionHistory.
        Assert.DoesNotContain("player-transactions-section", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-transactions-section”`,
        // `css`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldNotRenderStandaloneTransactionHistory.
        Assert.DoesNotContain("player-transactions-section", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”id=\”player-statistics-dashboard\””`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldNotRenderStandaloneTransactionHistory.
        Assert.Contains("id=\"player-statistics-dashboard\"", view, StringComparison.Ordinal);
    // Menutup scope metode PlayerDetails_ShouldNotRenderStandaloneTransactionHistory; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerDetails_ShouldNotRenderStandaloneTransactionHistory.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions` dengan hasil bertipe `void`; operasi ini menangani
    // pemain rincian should render three main statistic sections as accordions.
    public void PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions()
    // Membuka scope metode PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        // Menyiapkan variabel lokal `css` untuk nilai css dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”wwwroot”, ”css”, ”site.css”)`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”<details
        // id=\”player-statistics-summary\””`, `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
        Assert.Contains("<details id=\"player-statistics-summary\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”<details
        // id=\”player-analysis-atlas\””`, `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
        Assert.Contains("<details id=\"player-analysis-atlas\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”<details
        // id=\”player-evidence-library\””`, `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
        Assert.Contains("<details id=\"player-evidence-library\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `CountOccurrences(view, ”open
        // aria-labelledby=\”player-”)`); pengujian gagal jika keduanya berbeda dalam PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
        Assert.Equal(1, CountOccurrences(view, "open aria-labelledby=\"player-"));
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”<summary class=\”players-fusion-head
        // ruleset-section-head player-stats-hero\””`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
        Assert.Contains("<summary class=\"players-fusion-head ruleset-section-head player-stats-hero\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `CountOccurrences(view, ”<summary
        // class=\”player-analysis-section-head\””)`); pengujian gagal jika keduanya berbeda dalam
        // PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
        Assert.Equal(2, CountOccurrences(view, "<summary class=\"player-analysis-section-head\""));
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”players.stats.read_time”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
        Assert.DoesNotContain("players.stats.read_time", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `CountOccurrences(view, ”
        // data-player-section-accordion data-accordion-key”)`); pengujian gagal jika keduanya berbeda dalam
        // PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
        Assert.Equal(3, CountOccurrences(view, " data-player-section-accordion data-accordion-key"));
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”data-accordion-key=\”summary\””`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
        Assert.Contains("data-accordion-key=\"summary\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”data-accordion-key=\”analysis\””`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
        Assert.Contains("data-accordion-key=\"analysis\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”data-accordion-key=\”evidence\””`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
        Assert.Contains("data-accordion-key=\"evidence\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”cashflowpoly.player-section-accordions”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
        Assert.Contains("cashflowpoly.player-section-accordions", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”localStorage.getItem(storageKey)”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
        Assert.Contains("localStorage.getItem(storageKey)", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”localStorage.setItem(storageKey”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
        Assert.Contains("localStorage.setItem(storageKey", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-dashboard-accordion>summary::after”`, `css`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
        Assert.Contains(".player-detail-overhaul .player-dashboard-accordion>summary::after", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-dashboard-accordion[open]>summary::after”`, `css`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
        Assert.Contains(".player-detail-overhaul .player-dashboard-accordion[open]>summary::after", css, StringComparison.Ordinal);
    // Menutup scope metode PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerDetails_ShouldRenderThreeMainStatisticSectionsAsAccordions.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerDetails_ShouldUseProminentIdentityWithoutPlayerId` dengan hasil bertipe `void`; operasi ini menangani pemain rincian
    // should use prominent identity tanpa pemain identitas.
    public void PlayerDetails_ShouldUseProminentIdentityWithoutPlayerId()
    // Membuka scope metode PlayerDetails_ShouldUseProminentIdentityWithoutPlayerId; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // PlayerDetails_ShouldUseProminentIdentityWithoutPlayerId.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        // Menyiapkan variabel lokal `css` untuk nilai css dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”wwwroot”, ”css”, ”site.css”)`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-profile-identity”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldUseProminentIdentityWithoutPlayerId.
        Assert.Contains("player-profile-identity", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-profile-avatar”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldUseProminentIdentityWithoutPlayerId.
        Assert.Contains("player-profile-avatar", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-profile-turn”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldUseProminentIdentityWithoutPlayerId.
        Assert.Contains("player-profile-turn", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”players.analytics_title”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldUseProminentIdentityWithoutPlayerId.
        Assert.DoesNotContain("players.analytics_title", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”@Model.PlayerId”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldUseProminentIdentityWithoutPlayerId.
        Assert.DoesNotContain("@Model.PlayerId", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Model.GameplayComputedAt”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldUseProminentIdentityWithoutPlayerId.
        Assert.DoesNotContain("Model.GameplayComputedAt", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-profile-avatar-shell”`, `css`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldUseProminentIdentityWithoutPlayerId.
        Assert.Contains(".player-detail-overhaul .player-profile-avatar-shell", css, StringComparison.Ordinal);
    // Menutup scope metode PlayerDetails_ShouldUseProminentIdentityWithoutPlayerId; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerDetails_ShouldUseProminentIdentityWithoutPlayerId.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation` dengan hasil bertipe `void`; operasi ini menangani
    // pemain rincian should expose source formula guidance dan recommendation.
    public void PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation()
    // Membuka scope metode PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        // Menyiapkan variabel lokal `lexicon` untuk nilai lexicon dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Infrastructure”,
        // ”UiTextLexicon.Players.cs”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var lexicon = File.ReadAllText(Path.Combine(UiRoot, "Infrastructure", "UiTextLexicon.Players.cs"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”overallDescriptionKey”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Contains("overallDescriptionKey", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-analysis-card__method”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Contains("player-analysis-card__method", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”players.support.field.source”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Contains("players.support.field.source", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”players.analysis.source.{analysis.Key}”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Contains("players.analysis.source.{analysis.Key}", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”analysis.FormulaKey”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Contains("analysis.FormulaKey", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”PlayerMetricCollectionHelper.BuildActualCalculation”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Contains("PlayerMetricCollectionHelper.BuildActualCalculation", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”players.support.field.actual_calculation”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Contains("players.support.field.actual_calculation", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”primaryMetric.Guidance”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Contains("primaryMetric.Guidance", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”primaryMetric.Recommendation”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Contains("primaryMetric.Recommendation", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan bahwa `view.IndexOf(”players.support.field.source”, StringComparison.Ordinal) <
        // view.IndexOf(”players.support.field.formula”, StringComparison.Ordinal)`, `”Data source must appear immediately before the calculation note.”`
        // bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.True(
            // Meneruskan pemeriksaan lebih kecil antara `view.IndexOf(”players.support.field.source”, StringComparison.Ordinal)` dan
            // `view.IndexOf(”players.support.field.formula”, StringComparison.Ordinal)` sebagai argumen ke `Assert.True`; Meneruskan nilai literal
            // `”players.support.field.source”` sebagai argumen ke `view.IndexOf`; Meneruskan `StringComparison.Ordinal` (nilai ordinal) sebagai argumen ke
            // `view.IndexOf`; Meneruskan nilai literal `”players.support.field.formula”` sebagai argumen ke `view.IndexOf`; Meneruskan
            // `StringComparison.Ordinal` (nilai ordinal) sebagai argumen ke `view.IndexOf`.
            view.IndexOf("players.support.field.source", StringComparison.Ordinal) < view.IndexOf("players.support.field.formula", StringComparison.Ordinal),
            // Meneruskan nilai literal `”Data source must appear immediately before the calculation note.”` sebagai argumen ke `Assert.True`.
            "Data source must appear immediately before the calculation note.");
        // Menjalankan pemeriksaan bahwa `view.IndexOf(”players.support.field.formula”, StringComparison.Ordinal) <
        // view.IndexOf(”players.support.field.actual_calculation”, StringComparison.Ordinal) && view.IndexOf(”p...`, `”The numeric substitution must appear
        // directly after the formula and before the recommendation.”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.True(
            // Meneruskan gabungan syarat AND: kedua kondisi wajib benar antara `view.IndexOf(”players.support.field.formula”, StringComparison.Ordinal) <
            // view.IndexOf(”players.support.field.actual_calculation”, StringComparison.Ordinal)` dan `view.IndexOf(”players.support.field.actual_calculation”,
            // StringComparison.Ordinal) < view.IndexOf(”players.support.field.recommendation”, StringComparison.Ordinal)`; sisi kanan diperiksa hanya jika sisi
            // kiri benar sebagai argumen ke `Assert.True`; Meneruskan nilai literal `”players.support.field.formula”` sebagai argumen ke `view.IndexOf`;
            // Meneruskan `StringComparison.Ordinal` (nilai ordinal) sebagai argumen ke `view.IndexOf`; Meneruskan nilai literal
            // `”players.support.field.actual_calculation”` sebagai argumen ke `view.IndexOf`; Meneruskan `StringComparison.Ordinal` (nilai ordinal) sebagai
            // argumen ke `view.IndexOf`.
            view.IndexOf("players.support.field.formula", StringComparison.Ordinal) < view.IndexOf("players.support.field.actual_calculation", StringComparison.Ordinal) &&
            // Meneruskan nilai literal `”players.support.field.actual_calculation”` sebagai argumen ke `view.IndexOf`; Meneruskan `StringComparison.Ordinal`
            // (nilai ordinal) sebagai argumen ke `view.IndexOf`; Meneruskan nilai literal `”players.support.field.recommendation”` sebagai argumen ke
            // `view.IndexOf`; Meneruskan `StringComparison.Ordinal` (nilai ordinal) sebagai argumen ke `view.IndexOf`.
            view.IndexOf("players.support.field.actual_calculation", StringComparison.Ordinal) < view.IndexOf("players.support.field.recommendation", StringComparison.Ordinal),
            // Meneruskan nilai literal `”The numeric substitution must appear directly after the formula and before the recommendation.”` sebagai argumen ke
            // `Assert.True`.
            "The numeric substitution must appear directly after the formula and before the recommendation.");
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`14`, `CountOccurrences(lexicon,
        // ”terms[\”players.analysis.source.”)`); pengujian gagal jika keduanya berbeda dalam
        // PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Equal(14, CountOccurrences(lexicon, "terms[\"players.analysis.source."));
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Sumber data\”, \”Data source\”)”`,
        // `lexicon`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Contains("(\"Sumber data\", \"Data source\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Cara menghitung\”, \”How it is
        // calculated\”)”`, `lexicon`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Contains("(\"Cara menghitung\", \"How it is calculated\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Hitungan dengan angka pemain\”,
        // \”Calculation with the player's numbers\”)”`, `lexicon`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Contains("(\"Hitungan dengan angka pemain\", \"Calculation with the player's numbers\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Koin Tersisa dibanding Koin Awal = Koin
        // Tersisa ÷ Koin Awal × 100%.”`, `lexicon`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Contains("Perubahan Koin dari Awal = (Koin Tersisa − Koin Awal) ÷ Koin Awal × 100%.", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”jumlah kuadrat setiap (Persentase
        // Pendapatan per Sumber ÷ 100)”`, `lexicon`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Contains("jumlah kuadrat setiap (Persentase Pendapatan per Sumber ÷ 100)", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Persentase Risiko Selesai tanpa
        // Tindakan Darurat = Risiko Selesai tanpa Tindakan Darurat ÷ Kartu Risiko Kehidupan yang Muncul × 100%.”`, `lexicon`, `StringComparison.Ordinal`
        // dalam PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Contains("Persentase Risiko Selesai tanpa Tindakan Darurat = Risiko Selesai tanpa Tindakan Darurat ÷ Kartu Risiko Kehidupan yang Muncul × 100%.", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Skor Komitmen Donasi = Keteraturan
        // Jumlah Donasi × (Porsi Donasi dari Koin Tersisa dan Donasi ÷ 100) × (Persentase Jumat dengan Donasi ÷ 100), dibatasi 0–100.”`, `lexicon`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Contains("Skor Komitmen Donasi = Keteraturan Jumlah Donasi × (Porsi Donasi dari Koin Tersisa dan Donasi ÷ 100) × (Persentase Jumat dengan Donasi ÷ 100), dibatasi 0–100.", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Poin Kebahagiaan Kartu Kebutuhan + Poin
        // Kebahagiaan Bonus Set Kebutuhan + Poin Kebahagiaan dari Donasi”`, `lexicon`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.Contains("Poin Kebahagiaan Kartu Kebutuhan + Poin Kebahagiaan Bonus Set Kebutuhan + Poin Kebahagiaan dari Donasi", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”tingkat tanpa perlindungan”`,
        // `lexicon`, `StringComparison.OrdinalIgnoreCase` dalam PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.DoesNotContain("tingkat tanpa perlindungan", lexicon, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Konsentrasi Kebutuhan memakai
        // rumus dokumen yang ditampilkan terpisah”`, `lexicon`, `StringComparison.OrdinalIgnoreCase` dalam
        // PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
        Assert.DoesNotContain("Konsentrasi Kebutuhan memakai rumus dokumen yang ditampilkan terpisah", lexicon, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerDetails_ShouldExposeSourceFormulaGuidanceAndRecommendation.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary` dengan hasil bertipe `void`; operasi ini menangani pemain
    // rincian should render complete raw dan derived metric library.
    public void PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary()
    // Membuka scope metode PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        // Menyiapkan variabel lokal `controller` untuk nilai controller dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Controllers”,
        // ”PlayersController.cs”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var controller = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "PlayersController.cs"));
        // Menyiapkan variabel lokal `model` untuk nilai model dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Models”,
        // ”AnalyticsViewModels.cs”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var model = File.ReadAllText(Path.Combine(UiRoot, "Models", "AnalyticsViewModels.cs"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”GameplayRaw = gameplay?.RawJson”`,
        // `controller`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary.
        Assert.Contains("GameplayRaw = gameplay?.RawJson", controller, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”GameplayDerived =
        // gameplay?.DerivedJson”`, `controller`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary.
        Assert.Contains("GameplayDerived = gameplay?.DerivedJson", controller, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”JsonElement? GameplayRaw”`, `model`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary.
        Assert.Contains("JsonElement? GameplayRaw", model, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”JsonElement? GameplayDerived”`,
        // `model`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary.
        Assert.Contains("JsonElement? GameplayDerived", model, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”BuildMetricVariableGroups(Model.GameplayRaw”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary.
        Assert.Contains("BuildMetricVariableGroups(Model.GameplayRaw", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”BuildMetricGroups(Model.GameplayDerived”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary.
        Assert.Contains("BuildMetricGroups(Model.GameplayDerived", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-analysis-atlas”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary.
        Assert.Contains("player-analysis-atlas", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-evidence-library”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary.
        Assert.Contains("player-evidence-library", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”PlayerMetricLabelFormatter.DescribeMetric”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary.
        Assert.Contains("PlayerMetricLabelFormatter.DescribeMetric", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”players.support.field.recommendation”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary.
        Assert.Contains("players.support.field.recommendation", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”player-evidence-domain__recommendation”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary.
        Assert.Contains("player-evidence-domain__recommendation", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”players.support.mode.advanced”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary.
        Assert.Contains("players.support.mode.advanced", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan bahwa `view.IndexOf(”player-analysis-atlas”, StringComparison.Ordinal) < view.IndexOf(”player-evidence-library”,
        // StringComparison.Ordinal)`, `”Gameplay analysis results must appear before physical gameplay variables.”` bernilai benar; pengujian gagal jika
        // kondisi tidak terpenuhi dalam PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary.
        Assert.True(
            // Meneruskan pemeriksaan lebih kecil antara `view.IndexOf(”player-analysis-atlas”, StringComparison.Ordinal)` dan
            // `view.IndexOf(”player-evidence-library”, StringComparison.Ordinal)` sebagai argumen ke `Assert.True`; Meneruskan nilai literal
            // `”player-analysis-atlas”` sebagai argumen ke `view.IndexOf`; Meneruskan `StringComparison.Ordinal` (nilai ordinal) sebagai argumen ke
            // `view.IndexOf`; Meneruskan nilai literal `”player-evidence-library”` sebagai argumen ke `view.IndexOf`; Meneruskan `StringComparison.Ordinal`
            // (nilai ordinal) sebagai argumen ke `view.IndexOf`.
            view.IndexOf("player-analysis-atlas", StringComparison.Ordinal) < view.IndexOf("player-evidence-library", StringComparison.Ordinal),
            // Meneruskan nilai literal `”Gameplay analysis results must appear before physical gameplay variables.”` sebagai argumen ke `Assert.True`.
            "Gameplay analysis results must appear before physical gameplay variables.");
    // Menutup scope metode PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerDetails_ShouldRenderCompleteRawAndDerivedMetricLibrary.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerEvidenceLibrary_ShouldOnlyOpenTheFirstDomain` dengan hasil bertipe `void`; operasi ini menangani pemain evidence
    // library should only open the first domain.
    public void PlayerEvidenceLibrary_ShouldOnlyOpenTheFirstDomain()
    // Membuka scope metode PlayerEvidenceLibrary_ShouldOnlyOpenTheFirstDomain; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // PlayerEvidenceLibrary_ShouldOnlyOpenTheFirstDomain.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”@(domainIndex == 0 ? \”open\” :
        // null)”`, `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldOnlyOpenTheFirstDomain.
        Assert.Contains("@(domainIndex == 0 ? \"open\" : null)", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”open=\”@(domainIndex == 0)\””`,
        // `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldOnlyOpenTheFirstDomain.
        Assert.DoesNotContain("open=\"@(domainIndex == 0)\"", view, StringComparison.Ordinal);
    // Menutup scope metode PlayerEvidenceLibrary_ShouldOnlyOpenTheFirstDomain; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerEvidenceLibrary_ShouldOnlyOpenTheFirstDomain.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups` dengan hasil bertipe `void`; operasi ini
    // menangani pemain evidence library should present eleven sources in ten non redundant groups.
    public void PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups()
    // Membuka scope metode PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        // Mengulangi setiap elemen `new[] { ”coins”, ”ingredients”, ”meal_orders”, ”needs”, ”donations”, ”gold”, ”pension”, ”life_risk”, ”financial_goals”,
        // ”actions”, ”turns” }`; elemen saat ini disimpan sebagai `rawGroup` bertipe `var` untuk diproses oleh badan loop dalam
        // PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups.
        foreach (var rawGroup in new[] { "coins", "ingredients", "meal_orders", "needs", "donations", "gold", "pension", "life_risk", "financial_goals", "actions", "turns" })
        // Membuka scope loop setiap rawGroup dari `new[] { ”coins”, ”ingredients”, ”meal_orders”, ”needs”, ”donations”, ”gold”, ”pension”, ”life_risk”,
        // ”financial_goals”, ”actions”, ”turns” }`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups.
        {
            // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `$”GetGroupRows(rawGroupMap,
            // \”{rawGroup}\”)”`, `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups.
            Assert.Contains($"GetGroupRows(rawGroupMap, \"{rawGroup}\")", view, StringComparison.Ordinal);
        // Menutup scope loop setiap rawGroup dari `new[] { ”coins”, ”ingredients”, ”meal_orders”, ”needs”, ”donations”, ”gold”, ”pension”, ”life_risk”,
        // ”financial_goals”, ”actions”, ”turns” }`; bagian berikut berada di luar batas blok tersebut dalam
        // PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups.
        }

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”players.details.raw.actions.title”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups.
        Assert.Contains("players.details.raw.actions.title", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`, `CountOccurrences(view, ”TitleKey =
        // \”players.details.raw.”)`); pengujian gagal jika keduanya berbeda dalam PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups.
        Assert.Equal(10, CountOccurrences(view, "TitleKey = \"players.details.raw."));
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”players.details.raw.turns.title”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups.
        Assert.DoesNotContain("players.details.raw.turns.title", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”domain.Number”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups.
        Assert.DoesNotContain("domain.Number", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-evidence-domain__number”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups.
        Assert.DoesNotContain("player-evidence-domain__number", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”player-evidence-library__summary”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups.
        Assert.DoesNotContain("player-evidence-library__summary", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rawMetricCount”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups.
        Assert.DoesNotContain("rawMetricCount", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”derivedMetricCount”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups.
        Assert.DoesNotContain("derivedMetricCount", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”players.metric_items_suffix”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups.
        Assert.DoesNotContain("players.metric_items_suffix", view, StringComparison.Ordinal);
    // Menutup scope metode PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups; bagian berikut berada di luar batas blok tersebut
    // dalam PlayerEvidenceLibrary_ShouldPresentElevenSourcesInTenNonRedundantGroups.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly` dengan hasil bertipe `void`; operasi ini menangani pemain
    // evidence library should exclude redundant aliases only.
    public void PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly()
    // Membuka scope metode PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        // Menyiapkan variabel lokal `css` untuk nilai css dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”wwwroot”, ”css”, ”site.css”)`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));
        // Menyiapkan variabel lokal `lexicon` untuk nilai lexicon dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Infrastructure”,
        // ”UiTextLexicon.Players.cs”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var lexicon = File.ReadAllText(Path.Combine(UiRoot, "Infrastructure", "UiTextLexicon.Players.cs"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”BuildMetricVariableGroups”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.Contains("BuildMetricVariableGroups", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rawMetricKeysToShow”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.DoesNotContain("rawMetricKeysToShow", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rows.RemoveAll”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.DoesNotContain("rows.RemoveAll", view, StringComparison.Ordinal);
        // Mengulangi setiap elemen `new[] { ”coins_net_end_game”, ”coins_donated”, ”coins_saved”, ”leftover_coins_end_game” }`; elemen saat ini disimpan
        // sebagai `redundantKey` bertipe `var` untuk diproses oleh badan loop dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        foreach (var redundantKey in new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        {
            // Menggunakan nilai literal `”coins_net_end_game”` sebagai bagian ekspresi yang sedang disusun dalam
            // PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
            "coins_net_end_game",
            // Menggunakan nilai literal `”coins_donated”` sebagai bagian ekspresi yang sedang disusun dalam
            // PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
            "coins_donated",
            // Menggunakan nilai literal `”coins_saved”` sebagai bagian ekspresi yang sedang disusun dalam
            // PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
            "coins_saved",
            // Menggunakan nilai literal `”leftover_coins_end_game”` sebagai bagian ekspresi yang sedang disusun dalam
            // PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
            "leftover_coins_end_game"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        })
        // Membuka scope loop setiap redundantKey dari `new[] { ”coins_net_end_game”, ”coins_donated”, ”coins_saved”, ”leftover_coins_end_game” }`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        {
            // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `redundantKey`, `view`,
            // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
            Assert.Contains(redundantKey, view, StringComparison.Ordinal);
        // Menutup scope loop setiap redundantKey dari `new[] { ”coins_net_end_game”, ”coins_donated”, ”coins_saved”, ”leftover_coins_end_game” }`; bagian
        // berikut berada di luar batas blok tersebut dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        }
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ExcludeRows”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.Contains("ExcludeRows", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”List<(string Path, string Value)>
        // BuildActionRows()”`, `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.Contains("List<(string Path, string Value)> BuildActionRows()", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ExcludeRows(rows, \”actions_per_turn\”,
        // \”action_sequence\”, \”action_repetitions_per_turn\”)”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.Contains("ExcludeRows(rows, \"actions_per_turn\", \"action_sequence\", \"action_repetitions_per_turn\")", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”BuildActionUsageHistoryJson”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.Contains("BuildActionUsageHistoryJson", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”isCollectionValue”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.Contains("isCollectionValue", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-metric-card--series”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.Contains("player-metric-card--series", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”PlayerMetricJsonMapper.BuildCollectionTable”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.Contains("PlayerMetricJsonMapper.BuildCollectionTable", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”<table>”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.Contains("<table>", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”FormatActionSlot(actionSlot)”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.Contains("FormatActionSlot(actionSlot)", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”player-metric-series__outside-quota”`, `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.DoesNotContain("player-metric-series__outside-quota", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”players.details.action_slot.outside_quota”`, `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.DoesNotContain("players.details.action_slot.outside_quota", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”player-metric-series__outside-quota”`, `css`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.DoesNotContain("player-metric-series__outside-quota", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”players.details.action_slot.outside_quota”`, `lexicon`, `StringComparison.Ordinal` dalam
        // PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.DoesNotContain("players.details.action_slot.outside_quota", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”var hasOutsideQuotaRows”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.DoesNotContain("var hasOutsideQuotaRows", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”var seriesRows =
        // collectionTable.Rows.ToList()”`, `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.Contains("var seriesRows = collectionTable.Rows.ToList()", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”collectionUnits”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.Contains("collectionUnits", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”FormatCollectionCell(row.Path”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.Contains("FormatCollectionCell(row.Path", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”FirstOrDefault(seriesRow”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.DoesNotContain("FirstOrDefault(seriesRow", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”seriesRow[actionSlotColumnIndex]
        // != \”0\””`, `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.DoesNotContain("seriesRow[actionSlotColumnIndex] != \"0\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”zeroBasedDay + 1”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.DoesNotContain("zeroBasedDay + 1", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”font-family: ui-monospace”`,
        // `css`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.DoesNotContain("font-family: ui-monospace", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Hari\”, \”Day\”)”`, `lexicon`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.Contains("(\"Hari\", \"Day\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Persiapan\”, \”Setup\”)”`,
        // `lexicon`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
        Assert.Contains("(\"Persiapan\", \"Setup\")", lexicon, StringComparison.Ordinal);
    // Menutup scope metode PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerEvidenceLibrary_ShouldExcludeRedundantAliasesOnly.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory` dengan hasil bertipe `void`; operasi ini
    // menangani pemain evidence library should merge transactions changes dan balances into one history.
    public void PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory()
    // Membuka scope metode PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory; pernyataan/deklarasi berikut berada di dalam
    // batas blok ini dalam PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        // Menyiapkan variabel lokal `lexicon` untuk nilai lexicon dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Infrastructure”,
        // ”UiTextLexicon.Players.cs”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var lexicon = File.ReadAllText(Path.Combine(UiRoot, "Infrastructure", "UiTextLexicon.Players.cs"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”BuildTransactionHistoryJson”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory.
        Assert.Contains("BuildTransactionHistoryJson", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”coins_spent_per_turn\”,”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory.
        Assert.Contains("\"coins_spent_per_turn\",", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”coins_earned_per_turn\”);”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory.
        Assert.Contains("\"coins_earned_per_turn\");", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”row.Path == \”net_income_per_turn\””`,
        // `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory.
        Assert.Contains("row.Path == \"net_income_per_turn\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”row.Path ==
        // \”coins_per_turn_progression\””`, `view`, `StringComparison.Ordinal` dalam
        // PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory.
        Assert.Contains("row.Path == \"coins_per_turn_progression\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”coins_per_turn_progression\”,\n
        // \”net_income_per_turn\””`, `view`, `StringComparison.Ordinal` dalam
        // PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory.
        Assert.Contains("\"coins_per_turn_progression\",\n            \"net_income_per_turn\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”result.Add((\”transaction_history\”,
        // transactionHistory))”`, `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory.
        Assert.Contains("result.Add((\"transaction_history\", transactionHistory))", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Riwayat Transaksi\”, \”Transaction
        // History\”)”`, `lexicon`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory.
        Assert.Contains("(\"Riwayat Transaksi\", \"Transaction History\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Koin Masuk\”, \”Coins In\”)”`,
        // `lexicon`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory.
        Assert.Contains("(\"Koin Masuk\", \"Coins In\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Koin Keluar\”, \”Coins Out\”)”`,
        // `lexicon`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory.
        Assert.Contains("(\"Koin Keluar\", \"Coins Out\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Perubahan Koin\”, \”Coin
        // Change\”)”`, `lexicon`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory.
        Assert.Contains("(\"Perubahan Koin\", \"Coin Change\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Saldo setelah Kejadian\”, \”Balance
        // after Event\”)”`, `lexicon`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory.
        Assert.Contains("(\"Saldo setelah Kejadian\", \"Balance after Event\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”players.raw.transaction_direction”`, `lexicon`, `StringComparison.Ordinal` dalam
        // PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory.
        Assert.DoesNotContain("players.raw.transaction_direction", lexicon, StringComparison.Ordinal);
    // Menutup scope metode PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory; bagian berikut berada di luar batas blok
    // tersebut dalam PlayerEvidenceLibrary_ShouldMergeTransactionsChangesAndBalancesIntoOneHistory.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerEvidenceLibrary_ShouldGroupRepeatedGuidanceByDomain` dengan hasil bertipe `void`; operasi ini menangani pemain
    // evidence library should group repeated guidance berdasarkan domain.
    public void PlayerEvidenceLibrary_ShouldGroupRepeatedGuidanceByDomain()
    // Membuka scope metode PlayerEvidenceLibrary_ShouldGroupRepeatedGuidanceByDomain; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // PlayerEvidenceLibrary_ShouldGroupRepeatedGuidanceByDomain.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.GroupBy(metric => (metric.Guidance,
        // metric.Recommendation))”`, `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldGroupRepeatedGuidanceByDomain.
        Assert.Contains(".GroupBy(metric => (metric.Guidance, metric.Recommendation))", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-evidence-domain__guide”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldGroupRepeatedGuidanceByDomain.
        Assert.Contains("player-evidence-domain__guide", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-metric-card__guide”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerEvidenceLibrary_ShouldGroupRepeatedGuidanceByDomain.
        Assert.DoesNotContain("player-metric-card__guide", view, StringComparison.Ordinal);
    // Menutup scope metode PlayerEvidenceLibrary_ShouldGroupRepeatedGuidanceByDomain; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerEvidenceLibrary_ShouldGroupRepeatedGuidanceByDomain.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents` dengan hasil bertipe `void`; operasi ini menangani pemain
    // analysis pemetaan should render thirteen metrics dan their komponen.
    public void PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents()
    // Membuka scope metode PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        // Mengulangi setiap elemen `new[] { ”cash_growth_components”, ”income_diversification_components”, ”business_expense_share_components”,
        // ”meal_order_profit_margin_components”, ”risk_readiness_components”,...`; elemen saat ini disimpan sebagai `componentGroup` bertipe `var` untuk
        // diproses oleh badan loop dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        foreach (var componentGroup in new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        {
            // Menggunakan nilai literal `”cash_growth_components”` sebagai bagian ekspresi yang sedang disusun dalam
            // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
            "cash_growth_components",
            // Menggunakan nilai literal `”income_diversification_components”` sebagai bagian ekspresi yang sedang disusun dalam
            // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
            "income_diversification_components",
            // Menggunakan nilai literal `”business_expense_share_components”` sebagai bagian ekspresi yang sedang disusun dalam
            // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
            "business_expense_share_components",
            // Menggunakan nilai literal `”meal_order_profit_margin_components”` sebagai bagian ekspresi yang sedang disusun dalam
            // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
            "meal_order_profit_margin_components",
            // Menggunakan nilai literal `”risk_readiness_components”` sebagai bagian ekspresi yang sedang disusun dalam
            // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
            "risk_readiness_components",
            // Menggunakan nilai literal `”loan_burden_components”` sebagai bagian ekspresi yang sedang disusun dalam
            // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
            "loan_burden_components",
            // Menggunakan nilai literal `”financial_goal_progress_components”` sebagai bagian ekspresi yang sedang disusun dalam
            // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
            "financial_goals_purchase_cost_total",
            // Menggunakan nilai literal `”income_action_focus_components”` sebagai bagian ekspresi yang sedang disusun dalam
            // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
            "income_action_focus_components",
            // Menggunakan nilai literal `”ingredient_utilization_components”` sebagai bagian ekspresi yang sedang disusun dalam
            // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
            "ingredient_utilization_components",
            // Menggunakan nilai literal `”long_term_action_share_components”` sebagai bagian ekspresi yang sedang disusun dalam
            // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
            "long_term_action_share_components",
            // Menggunakan nilai literal `”need_fulfillment_diversity_components”` sebagai bagian ekspresi yang sedang disusun dalam
            // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
            "need_fulfillment_diversity_components",
            // Menggunakan nilai literal `”donation_commitment_components”` sebagai bagian ekspresi yang sedang disusun dalam
            // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
            "donation_commitment_components",
            // Menggunakan nilai literal `”happiness_points_composition”` sebagai bagian ekspresi yang sedang disusun dalam
            // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
            "happiness_points_composition"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        })
        // Membuka scope loop setiap componentGroup dari `new[] { ”cash_growth_components”, ”income_diversification_components”,
        // ”business_expense_share_components”, ”meal_order_profit_margin_components”, ”risk_readiness_components”,...`; pernyataan/deklarasi berikut berada
        // di dalam batas blok ini dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        {
            // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `componentGroup`, `view`,
            // `StringComparison.Ordinal` dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
            Assert.Contains(componentGroup, view, StringComparison.Ordinal);
        // Menutup scope loop setiap componentGroup dari `new[] { ”cash_growth_components”, ”income_diversification_components”,
        // ”business_expense_share_components”, ”meal_order_profit_margin_components”, ”risk_readiness_components”,...`; bagian berikut berada di luar batas
        // blok tersebut dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        }

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`13`, `CountOccurrences(view, ”Chapter =
        // \””)`); pengujian gagal jika keduanya berbeda dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.Equal(13, CountOccurrences(view, "Chapter = \""));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `CountOccurrences(view, ”Items =
        // analysisSections.Where”)`); pengujian gagal jika keduanya berbeda dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.Equal(4, CountOccurrences(view, "Items = analysisSections.Where"));
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Key = \”happiness-portfolio\””`,
        // `view`, `StringComparison.Ordinal` dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.Contains("Key = \"happiness-portfolio\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”analysis.Number”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.DoesNotContain("analysis.Number", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”chapter.Number”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.DoesNotContain("chapter.Number", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-analysis-card__number”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.DoesNotContain("player-analysis-card__number", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-stat-action__number”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.DoesNotContain("player-stat-action__number", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-stat-action__signal”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.Contains("player-stat-action__signal", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`13`, `CountOccurrences(view, ”PrimaryKey =
        // \””)`); pengujian gagal jika keduanya berbeda dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.Equal(13, CountOccurrences(view, "PrimaryKey = \""));
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”PrimaryKey = \”cash_growth_percent\””`,
        // `view`, `StringComparison.Ordinal` dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.Contains("PrimaryKey = \"cash_growth_percent\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”PrimaryKey =
        // \”risk_readiness_percent\””`, `view`, `StringComparison.Ordinal` dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.Contains("PrimaryKey = \"risk_readiness_percent\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”GetGroupRows(derivedGroupMap,
        // \”risk_readiness_components\”)”`, `view`, `StringComparison.Ordinal` dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.Contains("GetGroupRows(derivedGroupMap, \"risk_readiness_components\")", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”PrimaryKey =
        // \”income_action_focus_percent\””`, `view`, `StringComparison.Ordinal` dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.Contains("PrimaryKey = \"income_action_focus_percent\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”GetGroupRows(derivedGroupMap,
        // \”income_action_focus_components\”)”`, `view`, `StringComparison.Ordinal` dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.Contains("GetGroupRows(derivedGroupMap, \"income_action_focus_components\")", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”FormulaKey =
        // \”players.support.formula.goal_ambition_index\””`, `view`, `StringComparison.Ordinal` dalam
        // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.Contains("PrimaryKey = \"financial_goals_completed\"", view, StringComparison.Ordinal);
        Assert.Contains("FormulaKey = \"players.support.formula.goal_purchases\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”PrimaryKey =
        // \”long_term_action_share_percent\””`, `view`, `StringComparison.Ordinal` dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.Contains("PrimaryKey = \"long_term_action_share_percent\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”GetGroupRows(derivedGroupMap,
        // \”long_term_action_share_components\”)”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.Contains("GetGroupRows(derivedGroupMap, \"long_term_action_share_components\")", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”PrimaryKey =
        // \”donation_commitment_score\””`, `view`, `StringComparison.Ordinal` dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.Contains("PrimaryKey = \"donation_commitment_score\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”PrimaryKey =
        // \”total_happiness_points\””`, `view`, `StringComparison.Ordinal` dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.Contains("PrimaryKey = \"happiness_source_diversity_percent\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”row.Path.Equals(analysis.PrimaryKey”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
        Assert.Contains("row.Path.Equals(analysis.PrimaryKey", view, StringComparison.Ordinal);
    // Menutup scope metode PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerAnalysisMap_ShouldRenderThirteenMetricsAndTheirComponents.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerDetails_ShouldHideAdvancedOnlyContentInBeginnerMode` dengan hasil bertipe `void`; operasi ini menangani pemain
    // rincian should hide advanced only content in beginner mode.
    public void PlayerDetails_ShouldHideAdvancedOnlyContentInBeginnerMode()
    // Membuka scope metode PlayerDetails_ShouldHideAdvancedOnlyContentInBeginnerMode; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // PlayerDetails_ShouldHideAdvancedOnlyContentInBeginnerMode.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `CountOccurrences(view, ”.Where(item =>
        // isAdvancedMode || !item.AdvancedOnly)”)`); pengujian gagal jika keduanya berbeda dalam PlayerDetails_ShouldHideAdvancedOnlyContentInBeginnerMode.
        Assert.Equal(2, CountOccurrences(view, ".Where(item => isAdvancedMode || !item.AdvancedOnly)"));
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.Where(chapter => chapter.Items.Count >
        // 0)”`, `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldHideAdvancedOnlyContentInBeginnerMode.
        Assert.Contains(".Where(chapter => chapter.Items.Count > 0)", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”analysis.AdvancedOnly &&
        // !chapter.AdvancedOnly”`, `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldHideAdvancedOnlyContentInBeginnerMode.
        Assert.Contains("analysis.AdvancedOnly && !chapter.AdvancedOnly", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ExcludeAdvancedRowsInBeginner”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldHideAdvancedOnlyContentInBeginnerMode.
        Assert.Contains("ExcludeAdvancedRowsInBeginner", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”players.analysis.happiness_portfolio.beginner.desc”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldHideAdvancedOnlyContentInBeginnerMode.
        Assert.Contains("players.analysis.happiness_portfolio.beginner.desc", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”players.support.formula.happiness_portfolio.beginner”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldHideAdvancedOnlyContentInBeginnerMode.
        Assert.Contains("players.support.formula.happiness_portfolio.beginner", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”players.analysis.source.happiness-portfolio.beginner”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldHideAdvancedOnlyContentInBeginnerMode.
        Assert.Contains("players.analysis.source.happiness-portfolio.beginner", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”players.details.raw.coin_finance.beginner.desc”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldHideAdvancedOnlyContentInBeginnerMode.
        Assert.Contains("players.details.raw.coin_finance.beginner.desc", view, StringComparison.Ordinal);
    // Menutup scope metode PlayerDetails_ShouldHideAdvancedOnlyContentInBeginnerMode; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerDetails_ShouldHideAdvancedOnlyContentInBeginnerMode.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions` dengan hasil bertipe `void`; operasi ini menangani
    // pemain stats should use scorecard analysis pemetaan evidence dan prioritized aksi.
    public void PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions()
    // Membuka scope metode PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        // Menyiapkan variabel lokal `css` untuk nilai css dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”wwwroot”, ”css”, ”site.css”)`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-analysis-overview”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("player-analysis-overview", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-analysis-scorecard”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("player-analysis-scorecard", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-analysis-atlas”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("player-analysis-atlas", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-evidence-library”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("player-evidence-library", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”statSummary.Insights.Take(3)”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("statSummary.Insights.Take(3)", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-analysis-overview {”`, `css`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains(".player-detail-overhaul .player-analysis-overview {", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-analysis-grid {”`, `css`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains(".player-detail-overhaul .player-analysis-grid {", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-evidence-domain>summary {”`, `css`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains(".player-detail-overhaul .player-evidence-domain>summary {", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-stats-actions {”`, `css`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains(".player-detail-overhaul .player-stats-actions {", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-metric-card-grid {”`, `css`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid {", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”display: flex;”`, `css`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("display: flex;", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”flex-wrap: wrap;”`, `css`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("flex-wrap: wrap;", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”flex: 1 1 min(250px, 100%);”`, `css`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("flex: 1 1 min(250px, 100%);", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-metric-card-grid--columns-1”`, `css`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid--columns-1", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-metric-card-grid--columns-2”`, `css`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid--columns-2", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-metric-card-grid--columns-3”`, `css`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid--columns-3", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”flex-basis: calc((100% - 1.16rem) /
        // 3);”`, `css`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("flex-basis: calc((100% - 1.16rem) / 3);", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”2 or 4 or 8 => 2”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("2 or 4 or 8 => 2", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”player-metric-card-grid--columns-{scalarGridColumns}”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("player-metric-card-grid--columns-{scalarGridColumns}", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”player-analysis-card__metrics--columns-@supportingGridColumns”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("player-analysis-card__metrics--columns-@supportingGridColumns", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”align-items: stretch;”`, `css`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("align-items: stretch;", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”block-size: 100%;”`, `css`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("block-size: 100%;", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-metric-card-grid>.player-metric-card--series {”`, `css`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid>.player-metric-card--series {", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”flex: 1 0 100%;”`, `css`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("flex: 1 0 100%;", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”inline-size: 100%;”`, `css`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("inline-size: 100%;", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”PlayerMetricCollectionHelper.GetGroupRows(rawGroupMap, \”turns\”)”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("PlayerMetricCollectionHelper.GetGroupRows(rawGroupMap, \"turns\")", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”coins_per_turn_progression\”,”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("\"coins_per_turn_progression\",", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”player-metric-card-grid--coin-finance”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("player-metric-card-grid--coin-finance", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-metric-card--coin-overview”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("player-metric-card--coin-overview", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-metric-card-grid--coin-finance>.player-metric-card--coin-overview {”`, `css`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid--coin-finance>.player-metric-card--coin-overview {", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”.player-metric-card-grid--columns-3.player-metric-card-grid--coin-finance”`, `css`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains(".player-metric-card-grid--columns-3.player-metric-card-grid--coin-finance", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”flex: 1 1 calc((100% - 0.58rem) /
        // 2);”`, `css`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("flex: 1 1 calc((100% - 0.58rem) / 2);", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”meal-orders\” =>
        // domain.Rows.OrderBy”`, `view`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("\"meal-orders\" => domain.Rows.OrderBy", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”meal_orders_claimed\” => 0”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("\"meal_orders_claimed\" => 0", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”meal_order_income_per_order\” => 1”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("\"meal_order_income_per_order\" => 1", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”meal_orders_per_turn_average\” =>
        // 3”`, `view`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("\"meal_orders_per_turn_average\" => 3", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-metric-card-grid--meal-orders”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("player-metric-card-grid--meal-orders", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”player-metric-card--meal-orders-primary”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("player-metric-card--meal-orders-primary", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”player-metric-card--meal-orders-summary”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("player-metric-card--meal-orders-summary", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-metric-card-grid--meal-orders>.player-metric-card--meal-orders-primary”`, `css`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid--meal-orders>.player-metric-card--meal-orders-primary", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”donations\” => domain.Rows.OrderBy”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("\"donations\" => domain.Rows.OrderBy", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”PlayerMetricJsonMapper.BuildDonationHistoryJson(”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("PlayerMetricJsonMapper.BuildDonationHistoryJson(", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ExcludeRows(rows,
        // \”donation_amount_per_friday\”, \”donation_rank_per_friday\”)”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("ExcludeRows(rows, \"donation_amount_per_friday\", \"donation_rank_per_friday\")", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”donation_history\” => 0”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("\"donation_history\" => 0", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”donation_happiness_points\” => 3”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("\"donation_happiness_points\" => 3", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-metric-card-grid--donations”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("player-metric-card-grid--donations", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-metric-card-grid--donations>.player-metric-card--series {”`, `css`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.DoesNotContain(".player-detail-overhaul .player-metric-card-grid--donations>.player-metric-card--series {", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”life-risk\” => domain.Rows.OrderBy”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("\"life-risk\" => domain.Rows.OrderBy", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”life_risk_cards_drawn\” => 0”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("\"life_risk_cards_drawn\" => 0", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”life_risk_costs_per_card\” => 1”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("\"life_risk_costs_per_card\" => 1", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”life_risk_costs_total\” => 2”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("\"life_risk_costs_total\" => 2", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-metric-card--featured”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("player-metric-card--featured", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-metric-card-grid>.player-metric-card--featured {”`, `css`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid>.player-metric-card--featured {", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”gold\” => domain.Rows.OrderBy”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("\"gold\" => domain.Rows.OrderBy", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”gold_cards_initial\” => 0”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("\"gold_cards_initial\" => 0", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”gold_prices_per_purchase\” => 4”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("\"gold_prices_per_purchase\" => 4", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”gold_investment_net\” => 8”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("\"gold_investment_net\" => 8", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-metric-card-grid--gold”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("player-metric-card-grid--gold", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-metric-card--gold-overview”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains("player-metric-card--gold-overview", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-metric-card-grid--gold>.player-metric-card--gold-overview”`, `css`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
        Assert.Contains(".player-detail-overhaul .player-metric-card-grid--gold>.player-metric-card--gold-overview", css, StringComparison.Ordinal);
    // Menutup scope metode PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions; bagian berikut berada di luar batas blok tersebut
    // dalam PlayerStats_ShouldUseScorecardAnalysisMapEvidenceAndPrioritizedActions.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerStats_HeaderBadgeAndEvidenceRows_ShouldStayVisuallyAligned` dengan hasil bertipe `void`; operasi ini menangani
    // pemain stats header badge dan evidence baris should stay visually aligned.
    public void PlayerStats_HeaderBadgeAndEvidenceRows_ShouldStayVisuallyAligned()
    // Membuka scope metode PlayerStats_HeaderBadgeAndEvidenceRows_ShouldStayVisuallyAligned; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam PlayerStats_HeaderBadgeAndEvidenceRows_ShouldStayVisuallyAligned.
    {
        // Menyiapkan variabel lokal `css` untuk nilai css dengan memanggil `File.ReadAllText(Path.Combine(UiRoot, ”wwwroot”, ”css”, ”site.css”))
        // .ReplaceLineEndings` dengan `”\n”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ReplaceLineEndings(”\n”); dalam
            // PlayerStats_HeaderBadgeAndEvidenceRows_ShouldStayVisuallyAligned; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ReplaceLineEndings("\n");

        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-stats-hero::after”`, `css`, `StringComparison.Ordinal` dalam PlayerStats_HeaderBadgeAndEvidenceRows_ShouldStayVisuallyAligned.
        Assert.DoesNotContain(".player-detail-overhaul .player-stats-hero::after", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-evidence-domain>summary {\n display: flex;”`, `css`, `StringComparison.Ordinal` dalam
        // PlayerStats_HeaderBadgeAndEvidenceRows_ShouldStayVisuallyAligned.
        Assert.Contains(".player-detail-overhaul .player-evidence-domain>summary {\n    display: flex;", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.player-detail-overhaul
        // .player-evidence-domain__copy {\n display: grid;\n flex: 1 1 auto;”`, `css`, `StringComparison.Ordinal` dalam
        // PlayerStats_HeaderBadgeAndEvidenceRows_ShouldStayVisuallyAligned.
        Assert.Contains(".player-detail-overhaul .player-evidence-domain__copy {\n    display: grid;\n    flex: 1 1 auto;", css, StringComparison.Ordinal);
    // Menutup scope metode PlayerStats_HeaderBadgeAndEvidenceRows_ShouldStayVisuallyAligned; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerStats_HeaderBadgeAndEvidenceRows_ShouldStayVisuallyAligned.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerStats_ShouldAnimateProgressivelyAndRespectReducedMotion` dengan hasil bertipe `void`; operasi ini menangani pemain
    // stats should animate progressively dan respect reduced motion.
    public void PlayerStats_ShouldAnimateProgressivelyAndRespectReducedMotion()
    // Membuka scope metode PlayerStats_ShouldAnimateProgressivelyAndRespectReducedMotion; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam PlayerStats_ShouldAnimateProgressivelyAndRespectReducedMotion.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        // Menyiapkan variabel lokal `css` untuk nilai css dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”wwwroot”, ”css”, ”site.css”)`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var css = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "css", "site.css"));
        // Menyiapkan variabel lokal `script` untuk nilai script dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”wwwroot”, ”js”,
        // ”site.js”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var script = File.ReadAllText(Path.Combine(UiRoot, "wwwroot", "js", "site.js"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”data-player-stats-reveal”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldAnimateProgressivelyAndRespectReducedMotion.
        Assert.Contains("data-player-stats-reveal", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-stats-motion-ready”`, `css`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldAnimateProgressivelyAndRespectReducedMotion.
        Assert.Contains("player-stats-motion-ready", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”@keyframes player-stats-shine”`, `css`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldAnimateProgressivelyAndRespectReducedMotion.
        Assert.Contains("@keyframes player-stats-shine", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”@media (prefers-reduced-motion:
        // reduce)”`, `css`, `StringComparison.Ordinal` dalam PlayerStats_ShouldAnimateProgressivelyAndRespectReducedMotion.
        Assert.Contains("@media (prefers-reduced-motion: reduce)", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”IntersectionObserver”`, `script`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldAnimateProgressivelyAndRespectReducedMotion.
        Assert.Contains("IntersectionObserver", script, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”prefers-reduced-motion: reduce”`,
        // `script`, `StringComparison.Ordinal` dalam PlayerStats_ShouldAnimateProgressivelyAndRespectReducedMotion.
        Assert.Contains("prefers-reduced-motion: reduce", script, StringComparison.Ordinal);
    // Menutup scope metode PlayerStats_ShouldAnimateProgressivelyAndRespectReducedMotion; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerStats_ShouldAnimateProgressivelyAndRespectReducedMotion.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerStats_ShouldUsePlainLanguageExplanations` dengan hasil bertipe `void`; operasi ini menangani pemain stats should use
    // plain language explanations.
    public void PlayerStats_ShouldUsePlainLanguageExplanations()
    // Membuka scope metode PlayerStats_ShouldUsePlainLanguageExplanations; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // PlayerStats_ShouldUsePlainLanguageExplanations.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        // Menyiapkan variabel lokal `lexicon` untuk nilai lexicon dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Infrastructure”,
        // ”UiTextLexicon.Players.cs”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var lexicon = File.ReadAllText(Path.Combine(UiRoot, "Infrastructure", "UiTextLexicon.Players.cs"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Kesimpulan cepat\”, \”Quick
        // conclusion\”)”`, `lexicon`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.Contains("(\"Kesimpulan cepat\", \"Quick conclusion\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Cerita di Balik Hasil Pemain\”,
        // \”The Story Behind the Player's Result\”)”`, `lexicon`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.Contains("(\"Cerita di Balik Hasil Pemain\", \"The Story Behind the Player's Result\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Penjelasan hasil\”, \”Result
        // explanation\”)”`, `lexicon`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.Contains("(\"Penjelasan hasil\", \"Result explanation\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”analysisSections.Count”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.DoesNotContain("analysisSections.Count", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”players.analysis.metric_suffix”`,
        // `lexicon`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.DoesNotContain("players.analysis.metric_suffix", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Uang dan usaha\”, \”Money and
        // business\”)”`, `lexicon`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.Contains("(\"Uang dan usaha\", \"Money and business\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Berapa koin tersisa dibandingkan
        // koin awal?\”, \”How do remaining coins compare with starting coins?\”)”`, `lexicon`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.Contains("(\"Berapa persen koin naik atau turun dari awal?\", \"By what percentage have coins increased or decreased from the start?\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Persentase Pengeluaran untuk Membeli
        // Bahan\”, \”Ingredient Purchase Share of Spending\”)”`, `lexicon`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.Contains("(\"Persentase Pengeluaran untuk Membeli Bahan\", \"Ingredient Purchase Share of Spending\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”apakah dua aksi per giliran
        // digunakan secara beragam”`, `lexicon`, `StringComparison.OrdinalIgnoreCase` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.DoesNotContain("apakah dua aksi per giliran digunakan secara beragam", lexicon, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”kecocokannya dengan misi koleksi
        // pribadi”`, `lexicon`, `StringComparison.OrdinalIgnoreCase` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.DoesNotContain("kecocokannya dengan misi koleksi pribadi", lexicon, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Prioritas pembahasan\”, \”Discussion
        // priorities\”)”`, `lexicon`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.Contains("(\"Prioritas pembahasan\", \"Discussion priorities\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Kondisi uang\”, \”Money
        // condition\”)”`, `lexicon`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.Contains("(\"Kondisi uang\", \"Money condition\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Pemerataan Kartu Kebutuhan\”, \”Need
        // Card Balance\”)”`, `lexicon`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.Contains("(\"Pemerataan Kartu Kebutuhan\", \"Need Card Balance\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Dari mana Poin Kebahagiaan
        // berasal?\”, \”Where did Happiness Points come from?\”)”`, `lexicon`, `StringComparison.Ordinal` dalam
        // PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.Contains("(\"Seberapa merata sumber Poin Kebahagiaan?\", \"How evenly are Happiness Points spread across sources?\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Data permainan\”, \”Gameplay
        // data\”)”`, `lexicon`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.Contains("(\"Data permainan\", \"Gameplay data\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”Saran\”, \”Suggestion\”)”`,
        // `lexicon`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.Contains("(\"Saran\", \"Suggestion\")", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”players.support.recommendation.savings”`, `lexicon`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.Contains("players.support.recommendation.savings", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”players.support.recommendation.debt”`,
        // `lexicon`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.Contains("players.support.recommendation.debt", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Lihat fungsi metrik”`, `lexicon`,
        // `StringComparison.Ordinal` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.DoesNotContain("Lihat fungsi metrik", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Pustaka Metrik Pemain”`,
        // `lexicon`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.DoesNotContain("Pustaka Metrik Pemain", lexicon, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Data pendukung lengkap”`,
        // `lexicon`, `StringComparison.Ordinal` dalam PlayerStats_ShouldUsePlainLanguageExplanations.
        Assert.DoesNotContain("Data pendukung lengkap", lexicon, StringComparison.Ordinal);
    // Menutup scope metode PlayerStats_ShouldUsePlainLanguageExplanations; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerStats_ShouldUsePlainLanguageExplanations.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerDetails_FinancialGoals_ShouldOnlyShowTheThreeEssentialValues` dengan hasil bertipe `void`; operasi ini menangani
    // pemain rincian keuangan target should only show the three essential nilai.
    public void PlayerDetails_FinancialGoals_ShouldOnlyShowTheThreeEssentialValues()
    // Membuka scope metode PlayerDetails_FinancialGoals_ShouldOnlyShowTheThreeEssentialValues; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam PlayerDetails_FinancialGoals_ShouldOnlyShowTheThreeEssentialValues.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”new[] { \”financial_goals_completed\”,
        // \”coins_saved\”, \”sharia_loans_outstanding_coins\” }”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_FinancialGoals_ShouldOnlyShowTheThreeEssentialValues.
        Assert.Contains("new[] { \"financial_goals_completed\", \"coins_saved\", \"sharia_loans_outstanding_coins\" }", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Rows = financialGoalSummaryRows”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_FinancialGoals_ShouldOnlyShowTheThreeEssentialValues.
        Assert.Contains("Rows = financialGoalSummaryRows", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Value ?? nullText”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_FinancialGoals_ShouldOnlyShowTheThreeEssentialValues.
        Assert.Contains("Value ?? nullText", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”domainGuides.Count > 0 && domain.Key !=
        // \”financial-goals\””`, `view`, `StringComparison.Ordinal` dalam PlayerDetails_FinancialGoals_ShouldOnlyShowTheThreeEssentialValues.
        Assert.Contains("domainGuides.Count > 0 && domain.Key != \"financial-goals\"", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”TryParseMetricNumber(attemptedFinancialGoals, out var attemptedCount)”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_FinancialGoals_ShouldOnlyShowTheThreeEssentialValues.
        Assert.Contains("TryParseMetricNumber(attemptedFinancialGoals, out var attemptedCount)", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”players.details.financial_goals.attempted_note”`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_FinancialGoals_ShouldOnlyShowTheThreeEssentialValues.
        Assert.Contains("players.details.financial_goals.attempted_note", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.Where(item => isAdvancedMode ||
        // !item.AdvancedOnly)”`, `view`, `StringComparison.Ordinal` dalam PlayerDetails_FinancialGoals_ShouldOnlyShowTheThreeEssentialValues.
        Assert.Contains(".Where(item => isAdvancedMode || !item.AdvancedOnly)", view, StringComparison.Ordinal);
    // Menutup scope metode PlayerDetails_FinancialGoals_ShouldOnlyShowTheThreeEssentialValues; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerDetails_FinancialGoals_ShouldOnlyShowTheThreeEssentialValues.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerDetails_ShouldHideDerivedNeedProfilesFromTheEvidenceLibrary` dengan hasil bertipe `void`; operasi ini menangani
    // pemain rincian should hide derived kebutuhan profiles dari the evidence library.
    public void PlayerDetails_ShouldHideDerivedNeedProfilesFromTheEvidenceLibrary()
    // Membuka scope metode PlayerDetails_ShouldHideDerivedNeedProfilesFromTheEvidenceLibrary; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam PlayerDetails_ShouldHideDerivedNeedProfilesFromTheEvidenceLibrary.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”ExcludeRows(PlayerMetricCollectionHelper.GetGroupRows(rawGroupMap, \”needs\”), \”need_profile.basic_profile\”,
        // \”need_profile.collector_profile\”, \”need_profile.specialist_p...`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldHideDerivedNeedProfilesFromTheEvidenceLibrary.
        Assert.Contains("ExcludeRows(PlayerMetricCollectionHelper.GetGroupRows(rawGroupMap, \"needs\"), \"need_profile.basic_profile\", \"need_profile.collector_profile\", \"need_profile.specialist_profile\")", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Key = \”fulfillment-diversity\”,
        // PrimaryKey = \”need_fulfillment_diversity_percent\””`, `view`, `StringComparison.Ordinal` dalam
        // PlayerDetails_ShouldHideDerivedNeedProfilesFromTheEvidenceLibrary.
        Assert.Contains("Key = \"fulfillment-diversity\", PrimaryKey = \"need_fulfillment_diversity_percent\"", view, StringComparison.Ordinal);
    // Menutup scope metode PlayerDetails_ShouldHideDerivedNeedProfilesFromTheEvidenceLibrary; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerDetails_ShouldHideDerivedNeedProfilesFromTheEvidenceLibrary.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerDetails_ShouldMergeDailyActionTables` dengan hasil bertipe `void`; operasi ini menangani pemain rincian should merge
    // daily aksi tables.
    public void PlayerDetails_ShouldMergeDailyActionTables()
    // Membuka scope metode PlayerDetails_ShouldMergeDailyActionTables; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // PlayerDetails_ShouldMergeDailyActionTables.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Players”,
        // ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Players", "Details.cshtml"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”BuildActionUsageHistoryJson”`, `view`,
        // `StringComparison.Ordinal` dalam PlayerDetails_ShouldMergeDailyActionTables.
        Assert.Contains("BuildActionUsageHistoryJson", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”(\”action_usage_history\”, history)”`,
        // `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldMergeDailyActionTables.
        Assert.Contains("(\"action_usage_history\", history)", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”actions_per_turn\”,
        // \”action_sequence\”, \”action_repetitions_per_turn\””`, `view`, `StringComparison.Ordinal` dalam PlayerDetails_ShouldMergeDailyActionTables.
        Assert.Contains("\"actions_per_turn\", \"action_sequence\", \"action_repetitions_per_turn\"", view, StringComparison.Ordinal);
    // Menutup scope metode PlayerDetails_ShouldMergeDailyActionTables; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerDetails_ShouldMergeDailyActionTables.
    }

    // Mendefinisikan metode `CountOccurrences` dengan hasil bertipe `int`; operasi ini menangani jumlah occurrences. Masukan: Parameter `source`
    // bertipe `string` membawa nilai source; Parameter `value` bertipe `string` membawa nilai nilai. Nilai hasil langsung berasal dari pembagian antara
    // `(source.Length - source.Replace(value, string.Empty, StringComparison.Ordinal).Length)` dan `value.Length`.
    private static int CountOccurrences(string source, string value) =>
        // Melanjutkan ekspresi dengan pembagian antara `(source.Length - source.Replace(value, string.Empty, StringComparison.Ordinal).Length)` dan
        // `value.Length` dalam CountOccurrences.
        (source.Length - source.Replace(value, string.Empty, StringComparison.Ordinal).Length) / value.Length;

    // Mendefinisikan metode `ResolveRepositoryRoot` dengan hasil bertipe `string`; operasi ini menangani resolve repositori root.
    private static string ResolveRepositoryRoot()
    // Membuka scope metode ResolveRepositoryRoot; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveRepositoryRoot.
    {
        // Menyiapkan variabel lokal `current` untuk nilai saat ini dengan objek baru bertipe `DirectoryInfo` dengan argumen (AppContext.BaseDirectory).
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        // Mengulangi blok selama hasil pencocokan `current` dengan pola `not null`; kondisi diperiksa lagi sebelum setiap iterasi dalam
        // ResolveRepositoryRoot.
        while (current is not null)
        // Membuka scope loop selama `current is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveRepositoryRoot.
        {
            // Memeriksa memanggil `File.Exists` dengan `Path.Combine(current.FullName, ”Cashflowpoly.sln”)`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam ResolveRepositoryRoot.
            if (File.Exists(Path.Combine(current.FullName, "Cashflowpoly.sln")))
            // Membuka scope cabang if untuk kondisi `File.Exists(Path.Combine(current.FullName, ”Cashflowpoly.sln”))`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam ResolveRepositoryRoot.
            {
                // Mengembalikan `current.FullName` (nilai full nama) kepada pemanggil dalam ResolveRepositoryRoot; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return current.FullName;
            // Menutup scope cabang if untuk kondisi `File.Exists(Path.Combine(current.FullName, ”Cashflowpoly.sln”))`; bagian berikut berada di luar batas blok
            // tersebut dalam ResolveRepositoryRoot.
            }

            // Memperbarui `current` menggunakan `current.Parent` (nilai parent) dalam ResolveRepositoryRoot.
            current = current.Parent;
        // Menutup scope loop selama `current is not null`; bagian berikut berada di luar batas blok tersebut dalam ResolveRepositoryRoot.
        }

        // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Repository root tidak ditemukan.”) dalam
        // ResolveRepositoryRoot; pemanggil atau middleware penanganan error menerima kegagalan ini.
        throw new InvalidOperationException("Repository root tidak ditemukan.");
    // Menutup scope metode ResolveRepositoryRoot; bagian berikut berada di luar batas blok tersebut dalam ResolveRepositoryRoot.
    }
// Menutup scope tipe PlayerDetailStatsLayoutTests; bagian berikut berada di luar batas blok tersebut.
}
