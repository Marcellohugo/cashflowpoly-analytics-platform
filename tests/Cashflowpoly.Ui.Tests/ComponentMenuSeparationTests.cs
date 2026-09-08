// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui ComponentMenuSeparationTests.
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `ComponentMenuSeparationTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class ComponentMenuSeparationTests
// Membuka scope tipe ComponentMenuSeparationTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
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
    // Mendefinisikan metode `Layout_ShouldNotRenderComponentsAsSeparatePrimaryMenu` dengan hasil bertipe `void`; operasi ini menangani layout should
    // not render komponen as separate primary menu.
    public void Layout_ShouldNotRenderComponentsAsSeparatePrimaryMenu()
    // Membuka scope metode Layout_ShouldNotRenderComponentsAsSeparatePrimaryMenu; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Layout_ShouldNotRenderComponentsAsSeparatePrimaryMenu.
    {
        // Menyiapkan variabel lokal `layout` untuk nilai layout dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Shared”,
        // ”_Layout.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var layout = File.ReadAllText(Path.Combine(UiRoot, "Views", "Shared", "_Layout.cshtml"));

        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”asp-controller=\”Components\””`,
        // `layout`, `StringComparison.Ordinal` dalam Layout_ShouldNotRenderComponentsAsSeparatePrimaryMenu.
        Assert.DoesNotContain("asp-controller=\"Components\"", layout, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”data-nav=\”components\””`,
        // `layout`, `StringComparison.Ordinal` dalam Layout_ShouldNotRenderComponentsAsSeparatePrimaryMenu.
        Assert.DoesNotContain("data-nav=\"components\"", layout, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”nav.components”`, `layout`,
        // `StringComparison.Ordinal` dalam Layout_ShouldNotRenderComponentsAsSeparatePrimaryMenu.
        Assert.DoesNotContain("nav.components", layout, StringComparison.Ordinal);
    // Menutup scope metode Layout_ShouldNotRenderComponentsAsSeparatePrimaryMenu; bagian berikut berada di luar batas blok tersebut dalam
    // Layout_ShouldNotRenderComponentsAsSeparatePrimaryMenu.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulesetIndex_ShouldRenderDefaultRulesetsInMainTableOnly` dengan hasil bertipe `void`; operasi ini menangani aturan index
    // should render bawaan aturan in main table only.
    public void RulesetIndex_ShouldRenderDefaultRulesetsInMainTableOnly()
    // Membuka scope metode RulesetIndex_ShouldRenderDefaultRulesetsInMainTableOnly; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // RulesetIndex_ShouldRenderDefaultRulesetsInMainTableOnly.
    {
        // Menyiapkan variabel lokal `rulesetIndex` untuk nilai aturan index dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”,
        // ”Rulesets”, ”Index.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetIndex = File.ReadAllText(Path.Combine(UiRoot, "Views", "Rulesets", "Index.cshtml"));
        // Menyiapkan variabel lokal `rulesetsController` untuk nilai aturan controller dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot,
        // ”Controllers”, ”RulesetsController.cs”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetsController = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "RulesetsController.cs"));

        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”rulesets.default_components.title”`, `rulesetIndex`, `StringComparison.Ordinal` dalam RulesetIndex_ShouldRenderDefaultRulesetsInMainTableOnly.
        Assert.DoesNotContain("rulesets.default_components.title", rulesetIndex, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”DefaultComponentItems”`,
        // `rulesetIndex`, `StringComparison.Ordinal` dalam RulesetIndex_ShouldRenderDefaultRulesetsInMainTableOnly.
        Assert.DoesNotContain("DefaultComponentItems", rulesetIndex, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”item.IsDefault”`, `rulesetIndex`,
        // `StringComparison.Ordinal` dalam RulesetIndex_ShouldRenderDefaultRulesetsInMainTableOnly.
        Assert.Contains("item.IsDefault", rulesetIndex, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”DefaultCatalogSource”`,
        // `rulesetsController`, `StringComparison.Ordinal` dalam RulesetIndex_ShouldRenderDefaultRulesetsInMainTableOnly.
        Assert.Contains("DefaultCatalogSource", rulesetsController, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”DefaultComponentItems =”`,
        // `rulesetsController`, `StringComparison.Ordinal` dalam RulesetIndex_ShouldRenderDefaultRulesetsInMainTableOnly.
        Assert.DoesNotContain("DefaultComponentItems =", rulesetsController, StringComparison.Ordinal);
    // Menutup scope metode RulesetIndex_ShouldRenderDefaultRulesetsInMainTableOnly; bagian berikut berada di luar batas blok tersebut dalam
    // RulesetIndex_ShouldRenderDefaultRulesetsInMainTableOnly.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ComponentsPage_ShouldBeRemoved` dengan hasil bertipe `void`; operasi ini menangani komponen page should be removed.
    public void ComponentsPage_ShouldBeRemoved()
    // Membuka scope metode ComponentsPage_ShouldBeRemoved; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ComponentsPage_ShouldBeRemoved.
    {
        // Menyiapkan variabel lokal `componentsViewPath` untuk nilai komponen view path dengan memanggil `Path.Combine` dengan `UiRoot`, `”Views”`,
        // `”Components”`, `”Index.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var componentsViewPath = Path.Combine(UiRoot, "Views", "Components", "Index.cshtml");
        // Menyiapkan variabel lokal `componentsControllerPath` untuk nilai komponen controller path dengan memanggil `Path.Combine` dengan `UiRoot`,
        // `”Controllers”`, `”ComponentsController.cs”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var componentsControllerPath = Path.Combine(UiRoot, "Controllers", "ComponentsController.cs");

        // Menjalankan pemeriksaan bahwa `File.Exists(componentsViewPath)`, `”Views/Components/Index.cshtml harus dihapus.”` bernilai salah; pengujian gagal
        // jika kondisi justru terpenuhi dalam ComponentsPage_ShouldBeRemoved.
        Assert.False(File.Exists(componentsViewPath), "Views/Components/Index.cshtml harus dihapus.");
        // Menjalankan pemeriksaan bahwa `File.Exists(componentsControllerPath)`, `”ComponentsController harus dihapus.”` bernilai salah; pengujian gagal
        // jika kondisi justru terpenuhi dalam ComponentsPage_ShouldBeRemoved.
        Assert.False(File.Exists(componentsControllerPath), "ComponentsController harus dihapus.");
    // Menutup scope metode ComponentsPage_ShouldBeRemoved; bagian berikut berada di luar batas blok tersebut dalam ComponentsPage_ShouldBeRemoved.
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

        // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Repository root tidak ditemukan.”) dalam
        // ResolveRepositoryRoot; pemanggil atau middleware penanganan error menerima kegagalan ini.
        throw new InvalidOperationException("Repository root tidak ditemukan.");
    // Menutup scope metode ResolveRepositoryRoot; bagian berikut berada di luar batas blok tersebut dalam ResolveRepositoryRoot.
    }
// Menutup scope tipe ComponentMenuSeparationTests; bagian berikut berada di luar batas blok tersebut.
}
