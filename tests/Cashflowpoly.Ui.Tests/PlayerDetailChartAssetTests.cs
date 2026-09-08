// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui PlayerDetailChartAssetTests.
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `PlayerDetailChartAssetTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerDetailChartAssetTests
// Membuka scope tipe PlayerDetailChartAssetTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `RepoRoot` menyimpan nilai repo root dengan nilai awal memanggil `ResolveRepositoryRoot` dengan tanpa
    // argumen. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan
    // dibagikan antar instance.
    private static readonly string RepoRoot = ResolveRepositoryRoot();

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `DetailsView_ShouldPreferExplainedValuesOverCharts` dengan hasil bertipe `void`; operasi ini menangani rincian view should
    // prefer explained nilai over charts.
    public void DetailsView_ShouldPreferExplainedValuesOverCharts()
    // Membuka scope metode DetailsView_ShouldPreferExplainedValuesOverCharts; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DetailsView_ShouldPreferExplainedValuesOverCharts.
    {
        // Menyiapkan variabel lokal `viewPath` untuk nilai view path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Players”`, `”Details.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var viewPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Views", "Players", "Details.cshtml");
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `viewPath`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var view = File.ReadAllText(viewPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-analysis-overview”`, `view`
        // dalam DetailsView_ShouldPreferExplainedValuesOverCharts.
        Assert.Contains("player-analysis-overview", view);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-analysis-card__description”`,
        // `view` dalam DetailsView_ShouldPreferExplainedValuesOverCharts.
        Assert.Contains("player-analysis-card__description", view);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player-analysis-card__method”`, `view`
        // dalam DetailsView_ShouldPreferExplainedValuesOverCharts.
        Assert.Contains("player-analysis-card__method", view);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”js-metric-line-chart”`, `view`
        // dalam DetailsView_ShouldPreferExplainedValuesOverCharts.
        Assert.DoesNotContain("js-metric-line-chart", view);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”window.cashflowpolyPlayerDetailCharts”`, `view` dalam DetailsView_ShouldPreferExplainedValuesOverCharts.
        Assert.DoesNotContain("window.cashflowpolyPlayerDetailCharts", view);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”~/js/player-detail-charts.js”`,
        // `view` dalam DetailsView_ShouldPreferExplainedValuesOverCharts.
        Assert.DoesNotContain("~/js/player-detail-charts.js", view);
    // Menutup scope metode DetailsView_ShouldPreferExplainedValuesOverCharts; bagian berikut berada di luar batas blok tersebut dalam
    // DetailsView_ShouldPreferExplainedValuesOverCharts.
    }

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

        // Menghentikan alur dengan melempar objek baru bertipe `DirectoryNotFoundException` dengan argumen (”Tidak dapat menemukan root repositori
        // (Cashflowpoly.sln).”) dalam ResolveRepositoryRoot; pemanggil atau middleware penanganan error menerima kegagalan ini.
        throw new DirectoryNotFoundException("Tidak dapat menemukan root repositori (Cashflowpoly.sln).");
    // Menutup scope metode ResolveRepositoryRoot; bagian berikut berada di luar batas blok tersebut dalam ResolveRepositoryRoot.
    }
// Menutup scope tipe PlayerDetailChartAssetTests; bagian berikut berada di luar batas blok tersebut.
}
