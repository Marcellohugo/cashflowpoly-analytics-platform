// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui RulesetDetailsLayoutTests.
// Mengimpor namespace `System.Text.RegularExpressions` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.RegularExpressions;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `RulesetDetailsLayoutTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetDetailsLayoutTests
// Membuka scope tipe RulesetDetailsLayoutTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulesetModeCallout_ShouldKeepBadgeOnOneLineWhileDescriptionCanShrink` dengan hasil bertipe `void`; operasi ini menangani
    // aturan mode callout should keep badge on one line while description can shrink.
    public void RulesetModeCallout_ShouldKeepBadgeOnOneLineWhileDescriptionCanShrink()
    // Membuka scope metode RulesetModeCallout_ShouldKeepBadgeOnOneLineWhileDescriptionCanShrink; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam RulesetModeCallout_ShouldKeepBadgeOnOneLineWhileDescriptionCanShrink.
    {
        // Menyiapkan variabel lokal `css` untuk nilai css dengan memanggil `File.ReadAllText` dengan `Path.Combine(ResolveRepositoryRoot(), ”src”,
        // ”Cashflowpoly.Ui”, ”wwwroot”, ”css”, ”tailwind.input.css”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var css = File.ReadAllText(Path.Combine(ResolveRepositoryRoot(), "src", "Cashflowpoly.Ui", "wwwroot", "css", "tailwind.input.css"));
        // Menyiapkan variabel lokal `badgeRule` untuk nilai badge rule dengan memanggil `Regex.Match` dengan `css`,
        // `@”\.ruleset-mode-badge,\s*\.ruleset-default-badge\s*\{(?<body>[^}]+)\}”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var badgeRule = Regex.Match(css, @"\.ruleset-mode-badge,\s*\.ruleset-default-badge\s*\{(?<body>[^}]+)\}");
        // Menyiapkan variabel lokal `descriptionRule` untuk nilai description rule dengan memanggil `Regex.Match` dengan `css`, `@”\.ruleset-mode-callout
        // p\s*\{(?<body>[^}]+)\}”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var descriptionRule = Regex.Match(css, @"\.ruleset-mode-callout p\s*\{(?<body>[^}]+)\}");

        // Menjalankan pemeriksaan bahwa `badgeRule.Success` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // RulesetModeCallout_ShouldKeepBadgeOnOneLineWhileDescriptionCanShrink.
        Assert.True(badgeRule.Success);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”flex: 0 0 auto;”`,
        // `badgeRule.Groups[”body”].Value`, `StringComparison.Ordinal` dalam RulesetModeCallout_ShouldKeepBadgeOnOneLineWhileDescriptionCanShrink.
        Assert.Contains("flex: 0 0 auto;", badgeRule.Groups["body"].Value, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”min-width: max-content;”`,
        // `badgeRule.Groups[”body”].Value`, `StringComparison.Ordinal` dalam RulesetModeCallout_ShouldKeepBadgeOnOneLineWhileDescriptionCanShrink.
        Assert.Contains("min-width: max-content;", badgeRule.Groups["body"].Value, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”white-space: nowrap;”`,
        // `badgeRule.Groups[”body”].Value`, `StringComparison.Ordinal` dalam RulesetModeCallout_ShouldKeepBadgeOnOneLineWhileDescriptionCanShrink.
        Assert.Contains("white-space: nowrap;", badgeRule.Groups["body"].Value, StringComparison.Ordinal);
        // Menjalankan pemeriksaan bahwa `descriptionRule.Success` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // RulesetModeCallout_ShouldKeepBadgeOnOneLineWhileDescriptionCanShrink.
        Assert.True(descriptionRule.Success);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”min-width: 0;”`,
        // `descriptionRule.Groups[”body”].Value`, `StringComparison.Ordinal` dalam RulesetModeCallout_ShouldKeepBadgeOnOneLineWhileDescriptionCanShrink.
        Assert.Contains("min-width: 0;", descriptionRule.Groups["body"].Value, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”flex: 1 1 auto;”`,
        // `descriptionRule.Groups[”body”].Value`, `StringComparison.Ordinal` dalam RulesetModeCallout_ShouldKeepBadgeOnOneLineWhileDescriptionCanShrink.
        Assert.Contains("flex: 1 1 auto;", descriptionRule.Groups["body"].Value, StringComparison.Ordinal);
    // Menutup scope metode RulesetModeCallout_ShouldKeepBadgeOnOneLineWhileDescriptionCanShrink; bagian berikut berada di luar batas blok tersebut
    // dalam RulesetModeCallout_ShouldKeepBadgeOnOneLineWhileDescriptionCanShrink.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulesetDetailsView_ShouldNotRenderComponentCatalogSection` dengan hasil bertipe `void`; operasi ini menangani aturan
    // rincian view should not render komponen catalog section.
    public void RulesetDetailsView_ShouldNotRenderComponentCatalogSection()
    // Membuka scope metode RulesetDetailsView_ShouldNotRenderComponentCatalogSection; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // RulesetDetailsView_ShouldNotRenderComponentCatalogSection.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `viewPath` untuk nilai view path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Shared”`, `”_RulesetDetailContent.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var viewPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Shared", "_RulesetDetailContent.cshtml");
        // Menyiapkan variabel lokal `viewContent` untuk nilai view content dengan memanggil `File.ReadAllText` dengan `viewPath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var viewContent = File.ReadAllText(viewPath);

        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulesets.components.title”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulesetDetailsView_ShouldNotRenderComponentCatalogSection.
        Assert.DoesNotContain("rulesets.components.title", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulesets.components.raw_json”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulesetDetailsView_ShouldNotRenderComponentCatalogSection.
        Assert.DoesNotContain("rulesets.components.raw_json", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”CompatibilityComponentCatalog”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulesetDetailsView_ShouldNotRenderComponentCatalogSection.
        Assert.DoesNotContain("CompatibilityComponentCatalog", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulesets.config_summary”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulesetDetailsView_ShouldNotRenderComponentCatalogSection.
        Assert.Contains("rulesets.config_summary", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset-detail-id-label”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulesetDetailsView_ShouldNotRenderComponentCatalogSection.
        Assert.DoesNotContain("ruleset-detail-id-label", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”id-chip”`, `viewContent`,
        // `StringComparison.Ordinal` dalam RulesetDetailsView_ShouldNotRenderComponentCatalogSection.
        Assert.DoesNotContain("id-chip", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset-overview-grid”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulesetDetailsView_ShouldNotRenderComponentCatalogSection.
        Assert.DoesNotContain("ruleset-overview-grid", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”displayDescription”`, `viewContent`,
        // `StringComparison.Ordinal` dalam RulesetDetailsView_ShouldNotRenderComponentCatalogSection.
        Assert.Contains("displayDescription", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”rulesets.default_description_advanced”`, `viewContent`, `StringComparison.Ordinal` dalam
        // RulesetDetailsView_ShouldNotRenderComponentCatalogSection.
        Assert.Contains("rulesets.default_description_advanced", viewContent, StringComparison.Ordinal);
    // Menutup scope metode RulesetDetailsView_ShouldNotRenderComponentCatalogSection; bagian berikut berada di luar batas blok tersebut dalam
    // RulesetDetailsView_ShouldNotRenderComponentCatalogSection.
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
// Menutup scope tipe RulesetDetailsLayoutTests; bagian berikut berada di luar batas blok tersebut.
}
