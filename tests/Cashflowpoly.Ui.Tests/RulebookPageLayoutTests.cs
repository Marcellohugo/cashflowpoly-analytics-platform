// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui RulebookPageLayoutTests.
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `RulebookPageLayoutTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulebookPageLayoutTests
// Membuka scope tipe RulebookPageLayoutTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ProgramAndLayout_ShouldExposeRulebookAsCanonicalRootRoute` dengan hasil bertipe `void`; operasi ini menangani program dan
    // layout should expose rulebook as canonical root route.
    public void ProgramAndLayout_ShouldExposeRulebookAsCanonicalRootRoute()
    // Membuka scope metode ProgramAndLayout_ShouldExposeRulebookAsCanonicalRootRoute; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ProgramAndLayout_ShouldExposeRulebookAsCanonicalRootRoute.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `programPath` untuk nilai program path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Program.cs”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var programPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Program.cs");
        // Menyiapkan variabel lokal `controllerPath` untuk nilai controller path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`,
        // `”Cashflowpoly.Ui”`, `”Controllers”`, `”HomeController.cs”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var controllerPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Controllers", "HomeController.cs");
        // Menyiapkan variabel lokal `layoutPath` untuk nilai layout path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Shared”`, `”_Layout.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var layoutPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Shared", "_Layout.cshtml");
        // Menyiapkan variabel lokal `homePath` untuk nilai home path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Home”`, `”Index.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var homePath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Home", "Index.cshtml");

        // Menyiapkan variabel lokal `programContent` untuk nilai program content dengan memanggil `File.ReadAllText` dengan `programPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var programContent = File.ReadAllText(programPath);
        // Menyiapkan variabel lokal `controllerContent` untuk nilai controller content dengan memanggil `File.ReadAllText` dengan `controllerPath`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var controllerContent = File.ReadAllText(controllerPath);
        // Menyiapkan variabel lokal `layoutContent` untuk nilai layout content dengan memanggil `File.ReadAllText` dengan `layoutPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var layoutContent = File.ReadAllText(layoutPath);
        // Menyiapkan variabel lokal `homeContent` untuk nilai home content dengan memanggil `File.ReadAllText` dengan `homePath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var homeContent = File.ReadAllText(homePath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”name: \”rulebook\””`, `programContent`,
        // `StringComparison.Ordinal` dalam ProgramAndLayout_ShouldExposeRulebookAsCanonicalRootRoute.
        Assert.Contains("name: \"rulebook\"", programContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”pattern: \”rulebook\””`,
        // `programContent`, `StringComparison.Ordinal` dalam ProgramAndLayout_ShouldExposeRulebookAsCanonicalRootRoute.
        Assert.Contains("pattern: \"rulebook\"", programContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Redirect(\”/rulebook\”)”`,
        // `controllerContent`, `StringComparison.Ordinal` dalam ProgramAndLayout_ShouldExposeRulebookAsCanonicalRootRoute.
        Assert.Contains("Redirect(\"/rulebook\")", controllerContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”href=\”/rulebook\””`, `layoutContent`,
        // `StringComparison.Ordinal` dalam ProgramAndLayout_ShouldExposeRulebookAsCanonicalRootRoute.
        Assert.Contains("href=\"/rulebook\"", layoutContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”href=\”/rulebook#rulebook-copyright\””`, `layoutContent`, `StringComparison.Ordinal` dalam
        // ProgramAndLayout_ShouldExposeRulebookAsCanonicalRootRoute.
        Assert.Contains("href=\"/rulebook#rulebook-copyright\"", layoutContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”href=\”/rulebook\””`, `homeContent`,
        // `StringComparison.Ordinal` dalam ProgramAndLayout_ShouldExposeRulebookAsCanonicalRootRoute.
        Assert.Contains("href=\"/rulebook\"", homeContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”asp-controller=\”Home\”
        // asp-action=\”Rulebook\””`, `layoutContent`, `StringComparison.Ordinal` dalam ProgramAndLayout_ShouldExposeRulebookAsCanonicalRootRoute.
        Assert.DoesNotContain("asp-controller=\"Home\" asp-action=\"Rulebook\"", layoutContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”asp-controller=\”Home\”
        // asp-action=\”Rulebook\””`, `homeContent`, `StringComparison.Ordinal` dalam ProgramAndLayout_ShouldExposeRulebookAsCanonicalRootRoute.
        Assert.DoesNotContain("asp-controller=\"Home\" asp-action=\"Rulebook\"", homeContent, StringComparison.Ordinal);
    // Menutup scope metode ProgramAndLayout_ShouldExposeRulebookAsCanonicalRootRoute; bagian berikut berada di luar batas blok tersebut dalam
    // ProgramAndLayout_ShouldExposeRulebookAsCanonicalRootRoute.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulebookView_ShouldUseStructuredReadableLayout` dengan hasil bertipe `void`; operasi ini menangani rulebook view should
    // use structured readable layout.
    public void RulebookView_ShouldUseStructuredReadableLayout()
    // Membuka scope metode RulebookView_ShouldUseStructuredReadableLayout; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // RulebookView_ShouldUseStructuredReadableLayout.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `viewPath` untuk nilai view path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Home”`, `”Privacy.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var viewPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Home", "Privacy.cshtml");
        // Menyiapkan variabel lokal `viewContent` untuk nilai view content dengan memanggil `File.ReadAllText` dengan `viewPath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var viewContent = File.ReadAllText(viewPath);

        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”<style>”`, `viewContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.DoesNotContain("<style>", viewContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulebook-page-grid”`, `viewContent`,
        // `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.Contains("rulebook-page-grid", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulebook-sidebar”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.DoesNotContain("rulebook-sidebar", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Daftar Isi Aturan”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.DoesNotContain("Daftar Isi Aturan", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulebook-page-chip”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.DoesNotContain("rulebook-page-chip", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Urutan A-M mengikuti buku
        // aturan”`, `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.DoesNotContain("Urutan A-M mengikuti buku aturan", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulebook-flow-map”`, `viewContent`,
        // `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.Contains("rulebook-flow-map", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulebook-guide-grid”`, `viewContent`,
        // `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.Contains("rulebook-guide-grid", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulebook-visual-stack”`, `viewContent`,
        // `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.Contains("rulebook-visual-stack", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulebook-reference-grid”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.Contains("rulebook-reference-grid", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulebook-score-grid”`, `viewContent`,
        // `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.Contains("rulebook-score-grid", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulebook-anchor-target”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.Contains("rulebook-anchor-target", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Panduan ringkas untuk mulai main”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.Contains("Panduan ringkas untuk mulai main", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Referensi cepat saat bermain”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.Contains("Referensi cepat saat bermain", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Gambar tetap besar”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.DoesNotContain("Gambar tetap besar", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Aturan mengacu PDF”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.DoesNotContain("Aturan mengacu PDF", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Metrik ke aturan”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.DoesNotContain("Metrik ke aturan", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Apa yang dilihat di analitika”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.DoesNotContain("Apa yang dilihat di analitika", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulebook-reference-table”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.DoesNotContain("rulebook-reference-table", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulebook-chapter-grid”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.DoesNotContain("rulebook-chapter-grid", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulebook-section-block”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.DoesNotContain("rulebook-section-block", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulebook-scoring-table”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.DoesNotContain("rulebook-scoring-table", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulebook-mode-flow-grid”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.DoesNotContain("rulebook-mode-flow-grid", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”@(index + 3).ToString”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.DoesNotContain("@(index + 3).ToString", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”@(sections.Count + 3).ToString”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulebookView_ShouldUseStructuredReadableLayout.
        Assert.DoesNotContain("@(sections.Count + 3).ToString", viewContent, StringComparison.Ordinal);
    // Menutup scope metode RulebookView_ShouldUseStructuredReadableLayout; bagian berikut berada di luar batas blok tersebut dalam
    // RulebookView_ShouldUseStructuredReadableLayout.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout` dengan hasil bertipe `void`; operasi ini menangani site css should
    // give rulebook responsive wide screen layout.
    public void SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout()
    // Membuka scope metode SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
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

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-page-grid”`, `cssContent`,
        // `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains(".rulebook-page-grid", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”grid-template-columns: minmax(0,
        // 1fr);”`, `cssContent`, `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains("grid-template-columns: minmax(0, 1fr);", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-flow-map”`, `cssContent`,
        // `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains(".rulebook-flow-map", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-guide-grid”`, `cssContent`,
        // `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains(".rulebook-guide-grid", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-guide-card”`, `cssContent`,
        // `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains(".rulebook-guide-card", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-visual-stack”`, `cssContent`,
        // `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains(".rulebook-visual-stack", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-visual-panel”`, `cssContent`,
        // `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains(".rulebook-visual-panel", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-reference-grid”`,
        // `cssContent`, `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains(".rulebook-reference-grid", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-reference-card”`,
        // `cssContent`, `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains(".rulebook-reference-card", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-score-grid”`, `cssContent`,
        // `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains(".rulebook-score-grid", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-anchor-target”`,
        // `cssContent`, `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains(".rulebook-anchor-target", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-anchor-target:target”`,
        // `cssContent`, `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains(".rulebook-anchor-target:target", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”list-style: disc;”`, `cssContent`,
        // `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains("list-style: disc;", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”list-style-position: outside;”`,
        // `cssContent`, `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains("list-style-position: outside;", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-list li::marker”`,
        // `cssContent`, `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains(".rulebook-list li::marker", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”scroll-margin-top”`, `cssContent`,
        // `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains("scroll-margin-top", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”grid-template-columns:
        // repeat(auto-fit, minmax(min(100%, 21rem), 1fr));”`, `cssContent`, `StringComparison.Ordinal` dalam
        // SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.DoesNotContain("grid-template-columns: repeat(auto-fit, minmax(min(100%, 21rem), 1fr));", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”grid-template-columns: repeat(3,
        // minmax(22rem, 1fr));”`, `cssContent`, `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.DoesNotContain("grid-template-columns: repeat(3, minmax(22rem, 1fr));", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”aspect-ratio: 16 / 10;”`, `cssContent`,
        // `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains("aspect-ratio: 16 / 10;", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”object-fit: contain;”`, `cssContent`,
        // `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains("object-fit: contain;", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-page-chip”`,
        // `cssContent`, `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.DoesNotContain(".rulebook-page-chip", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-sidebar”`,
        // `cssContent`, `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.DoesNotContain(".rulebook-sidebar", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-outline-list”`,
        // `cssContent`, `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.DoesNotContain(".rulebook-outline-list", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-reference-table”`,
        // `cssContent`, `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.DoesNotContain(".rulebook-reference-table", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-scoring-table”`,
        // `cssContent`, `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.DoesNotContain(".rulebook-scoring-table", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-chapter-grid”`,
        // `cssContent`, `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.DoesNotContain(".rulebook-chapter-grid", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.rulebook-mode-flow-grid”`,
        // `cssContent`, `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.DoesNotContain(".rulebook-mode-flow-grid", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”@media (min-width: 1600px)”`,
        // `cssContent`, `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains("@media (min-width: 1600px)", cssContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”@media (max-width: 900px)”`,
        // `cssContent`, `StringComparison.Ordinal` dalam SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
        Assert.Contains("@media (max-width: 900px)", cssContent, StringComparison.Ordinal);
    // Menutup scope metode SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout; bagian berikut berada di luar batas blok tersebut dalam
    // SiteCss_ShouldGiveRulebookResponsiveWideScreenLayout.
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
// Menutup scope tipe RulebookPageLayoutTests; bagian berikut berada di luar batas blok tersebut.
}
