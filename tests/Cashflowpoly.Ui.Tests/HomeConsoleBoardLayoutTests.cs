// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui HomeConsoleBoardLayoutTests.
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;
// Mengimpor namespace `System.Text.RegularExpressions` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.RegularExpressions;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `HomeCommandCenterLayoutTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class HomeCommandCenterLayoutTests
// Membuka scope tipe HomeCommandCenterLayoutTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `HomeIndexView_ShouldUseCommandCenterHeroStructure` dengan hasil bertipe `void`; operasi ini menangani home index view
    // should use command center hero structure.
    public void HomeIndexView_ShouldUseCommandCenterHeroStructure()
    // Membuka scope metode HomeIndexView_ShouldUseCommandCenterHeroStructure; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // HomeIndexView_ShouldUseCommandCenterHeroStructure.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `viewPath` untuk nilai view path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Home”`, `”Index.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var viewPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Home", "Index.cshtml");
        // Menyiapkan variabel lokal `viewContent` untuk nilai view content dengan memanggil `File.ReadAllText` dengan `viewPath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var viewContent = File.ReadAllText(viewPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-command-main”`, `viewContent`
        // dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Contains("home-command-main", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”<section class=\”home-hero”`,
        // `viewContent` dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Contains("<section class=\"home-hero", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-command-intro”`, `viewContent`
        // dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Contains("home-command-intro", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-live-card”`, `viewContent` dalam
        // HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Contains("home-live-card", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-session-ring”`, `viewContent`
        // dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Contains("home-session-ring", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-stat-grid”`, `viewContent` dalam
        // HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Contains("home-stat-grid", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-guide-shell”`, `viewContent` dalam
        // HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Contains("home-guide-shell", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”<details class=\”home-guide-shell mt-6
        // data-toggle\”>”`, `viewContent` dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Contains("<details class=\"home-guide-shell mt-6 data-toggle\">", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”<details class=\”home-guide-shell mt-6
        // player-guide data-toggle\”>”`, `viewContent` dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Contains("<details class=\"home-guide-shell mt-6 player-guide data-toggle\">", viewContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home.player_flow”`, `viewContent`
        // dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.DoesNotContain("home.player_flow", viewContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”<details class=\”home-guide-shell
        // mt-6 data-toggle\” open”`, `viewContent` dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.DoesNotContain("<details class=\"home-guide-shell mt-6 data-toggle\" open", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”activeSessionPercent”`, `viewContent`
        // dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Contains("activeSessionPercent", viewContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-console-”`, `viewContent`
        // dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.DoesNotContain("home-console-", viewContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”section-shell home-hero”`,
        // `viewContent` dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.DoesNotContain("section-shell home-hero", viewContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-command-footer”`,
        // `viewContent` dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.DoesNotContain("home-command-footer", viewContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home.auto_refresh_note”`,
        // `viewContent` dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.DoesNotContain("home.auto_refresh_note", viewContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”section-shell mt-6
        // player-guide”`, `viewContent` dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.DoesNotContain("section-shell mt-6 player-guide", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-total-sessions”`, `viewContent`
        // dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Contains("home-total-sessions", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-active-sessions”`, `viewContent`
        // dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Contains("home-active-sessions", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-active-total-sessions”`,
        // `viewContent` dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Contains("home-active-total-sessions", viewContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-active-percentage”`,
        // `viewContent` dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.DoesNotContain("home-active-percentage", viewContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-live-summary”`,
        // `viewContent` dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.DoesNotContain("home-live-summary", viewContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-stat-mark”`, `viewContent`
        // dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.DoesNotContain("home-stat-mark", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-total-players”`, `viewContent`
        // dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Contains("home-total-players", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-total-rulesets”`, `viewContent`
        // dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Contains("home-total-rulesets", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-realtime-error”`, `viewContent`
        // dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Contains("home-realtime-error", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”sessionRingEl.style.setProperty”`,
        // `viewContent` dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Contains("sessionRingEl.style.setProperty", viewContent);
        // Menjalankan pemeriksaan hasil dengan `Assert.Single` menggunakan `Regex.Matches(viewContent, ”id=\”home-active-sessions\””)`; ketidaksesuaian
        // dengan ekspektasi membuat pengujian gagal dalam HomeIndexView_ShouldUseCommandCenterHeroStructure.
        Assert.Single(Regex.Matches(viewContent, "id=\"home-active-sessions\""));
    // Menutup scope metode HomeIndexView_ShouldUseCommandCenterHeroStructure; bagian berikut berada di luar batas blok tersebut dalam
    // HomeIndexView_ShouldUseCommandCenterHeroStructure.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SiteCss_ShouldDefineResponsiveCommandCenterCards` dengan hasil bertipe `void`; operasi ini menangani site css should
    // define responsive command center kartu.
    public void SiteCss_ShouldDefineResponsiveCommandCenterCards()
    // Membuka scope metode SiteCss_ShouldDefineResponsiveCommandCenterCards; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // SiteCss_ShouldDefineResponsiveCommandCenterCards.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `cssPath` untuk nilai css path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”wwwroot”`, `”css”`, `”site.css”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var cssPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "wwwroot", "css", "site.css");
        // Menyiapkan variabel lokal `cssContent` untuk nilai css content dengan memanggil `File.ReadAllText` dengan `cssPath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var cssContent = File.ReadAllText(cssPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.home-command-main”`, `cssContent`
        // dalam SiteCss_ShouldDefineResponsiveCommandCenterCards.
        Assert.Contains(".home-command-main", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.home-session-ring”`, `cssContent`
        // dalam SiteCss_ShouldDefineResponsiveCommandCenterCards.
        Assert.Contains(".home-session-ring", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”conic-gradient”`, `cssContent` dalam
        // SiteCss_ShouldDefineResponsiveCommandCenterCards.
        Assert.Contains("conic-gradient", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.home-stat-card-rulesets”`,
        // `cssContent` dalam SiteCss_ShouldDefineResponsiveCommandCenterCards.
        Assert.Contains(".home-stat-card-rulesets", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.home-guide-summary-copy”`,
        // `cssContent` dalam SiteCss_ShouldDefineResponsiveCommandCenterCards.
        Assert.Contains(".home-guide-summary-copy", cssContent);
        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan `new Regex(@”\.home-guide-shell\s*\{[\s\S]*?background:”,
        // RegexOptions.Singleline)`, `cssContent`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // SiteCss_ShouldDefineResponsiveCommandCenterCards.
        Assert.Matches(new Regex(@"\.home-guide-shell\s*\{[\s\S]*?background:", RegexOptions.Singleline), cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.home-guide-shell[open]”`, `cssContent`
        // dalam SiteCss_ShouldDefineResponsiveCommandCenterCards.
        Assert.Contains(".home-guide-shell[open]", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”grid-template-areas:”`, `cssContent`
        // dalam SiteCss_ShouldDefineResponsiveCommandCenterCards.
        Assert.Contains("grid-template-areas:", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”live-header live-ring\””`,
        // `cssContent` dalam SiteCss_ShouldDefineResponsiveCommandCenterCards.
        Assert.Contains("\"live-header live-ring\"", cssContent);
        // Menjalankan pemeriksaan bahwa `cssContent.LastIndexOf(”@media (max-width: 1024px)”, StringComparison.Ordinal) >
        // cssContent.IndexOf(”.home-command-main {”, cssContent.IndexOf(”.home-hero {”, StringComparison...`, `”Aturan responsif beranda harus
        // didefinisikan setelah aturan dasarnya agar tidak tertimpa cascade CSS.”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // SiteCss_ShouldDefineResponsiveCommandCenterCards.
        Assert.True(
            // Meneruskan pemeriksaan lebih besar antara `cssContent.LastIndexOf(”@media (max-width: 1024px)”, StringComparison.Ordinal)` dan
            // `cssContent.IndexOf(”.home-command-main {”, cssContent.IndexOf(”.home-hero {”, StringComparison.Ordinal), StringComparison.Ordinal)` sebagai
            // argumen ke `Assert.True`; Meneruskan nilai literal `”@media (max-width: 1024px)”` sebagai argumen ke `cssContent.LastIndexOf`; Meneruskan
            // `StringComparison.Ordinal` (nilai ordinal) sebagai argumen ke `cssContent.LastIndexOf`.
            cssContent.LastIndexOf("@media (max-width: 1024px)", StringComparison.Ordinal) >
            // Meneruskan nilai literal `”.home-command-main {”` sebagai argumen ke `cssContent.IndexOf`; Meneruskan memanggil `cssContent.IndexOf` dengan
            // `”.home-hero {”`, `StringComparison.Ordinal` sebagai argumen ke `cssContent.IndexOf`; Meneruskan nilai literal `”.home-hero {”` sebagai argumen
            // ke `cssContent.IndexOf`; Meneruskan `StringComparison.Ordinal` (nilai ordinal) sebagai argumen ke `cssContent.IndexOf`; Meneruskan
            // `StringComparison.Ordinal` (nilai ordinal) sebagai argumen ke `cssContent.IndexOf`.
            cssContent.IndexOf(".home-command-main {", cssContent.IndexOf(".home-hero {", StringComparison.Ordinal), StringComparison.Ordinal),
            // Meneruskan nilai literal `”Aturan responsif beranda harus didefinisikan setelah aturan dasarnya agar tidak tertimpa cascade CSS.”` sebagai
            // argumen ke `Assert.True`.
            "Aturan responsif beranda harus didefinisikan setelah aturan dasarnya agar tidak tertimpa cascade CSS.");
        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan `new Regex(@”@media \(max-width:
        // 640px\)[\s\S]*?\.home-stat-grid\s*\{\s*grid-template-columns:\s*1fr;”, RegexOptions.Singleline)`, `cssContent`; ketidaksesuaian dengan ekspektasi
        // membuat pengujian gagal dalam SiteCss_ShouldDefineResponsiveCommandCenterCards.
        Assert.Matches(new Regex(@"@media \(max-width: 640px\)[\s\S]*?\.home-stat-grid\s*\{\s*grid-template-columns:\s*1fr;", RegexOptions.Singleline), cssContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.home-console-”`, `cssContent`
        // dalam SiteCss_ShouldDefineResponsiveCommandCenterCards.
        Assert.DoesNotContain(".home-console-", cssContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.home-command-footer”`,
        // `cssContent` dalam SiteCss_ShouldDefineResponsiveCommandCenterCards.
        Assert.DoesNotContain(".home-command-footer", cssContent);
    // Menutup scope metode SiteCss_ShouldDefineResponsiveCommandCenterCards; bagian berikut berada di luar batas blok tersebut dalam
    // SiteCss_ShouldDefineResponsiveCommandCenterCards.
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
// Menutup scope tipe HomeCommandCenterLayoutTests; bagian berikut berada di luar batas blok tersebut.
}
