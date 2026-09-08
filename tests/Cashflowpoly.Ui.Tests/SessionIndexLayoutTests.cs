// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui SessionIndexLayoutTests.
// Mengimpor namespace `System.Text.RegularExpressions` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.RegularExpressions;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `SessionIndexLayoutTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionIndexLayoutTests
// Membuka scope tipe SessionIndexLayoutTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SiteCss_ShouldRenderSessionSummaryCardsWithResponsiveColumns` dengan hasil bertipe `void`; operasi ini menangani site css
    // should render sesi summary kartu dengan responsive columns.
    public void SiteCss_ShouldRenderSessionSummaryCardsWithResponsiveColumns()
    // Membuka scope metode SiteCss_ShouldRenderSessionSummaryCardsWithResponsiveColumns; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam SiteCss_ShouldRenderSessionSummaryCardsWithResponsiveColumns.
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

        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan `new Regex(
        // @”\.session-stats-grid\s*\{[^}]*grid-template-columns:\s*1fr\s*!important;”, RegexOptions.Singleline)`, `cssContent`; ketidaksesuaian dengan
        // ekspektasi membuat pengujian gagal dalam SiteCss_ShouldRenderSessionSummaryCardsWithResponsiveColumns.
        Assert.Matches(
            // Meneruskan objek baru bertipe `Regex` dengan argumen ( @”\.session-stats-grid\s*\{[^}]*grid-template-columns:\s*1fr\s*!important;”,
            // RegexOptions.Singleline) sebagai argumen ke `Assert.Matches`.
            new Regex(
                // Meneruskan nilai literal `@”\.session-stats-grid\s*\{[^}]*grid-template-columns:\s*1fr\s*!important;”` sebagai argumen ke konstruktor `Regex`.
                @"\.session-stats-grid\s*\{[^}]*grid-template-columns:\s*1fr\s*!important;",
                // Meneruskan `RegexOptions.Singleline` (nilai singleline) sebagai argumen ke konstruktor `Regex`.
                RegexOptions.Singleline),
            // Meneruskan `cssContent` (nilai css content) sebagai argumen ke `Assert.Matches`.
            cssContent);
        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan `new Regex(
        // @”@media\s*\(min-width:\s*640px\)\s*\{\s*\.session-stats-grid\s*\{[^}]*grid-template-columns:\s*repeat\(2,\s*minmax\(0,\s*1fr\)\)\s*!important;”,
        // RegexOptions.Singl...`, `cssContent`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // SiteCss_ShouldRenderSessionSummaryCardsWithResponsiveColumns.
        Assert.Matches(
            // Meneruskan objek baru bertipe `Regex` dengan argumen (
            // @”@media\s*\(min-width:\s*640px\)\s*\{\s*\.session-stats-grid\s*\{[^}]*grid-template-columns:\s*repeat\(2,\s*minmax\(0,\s*1fr\)\)\s*!important;”,
            // RegexOptions... sebagai argumen ke `Assert.Matches`.
            new Regex(
                // Meneruskan nilai literal `@”@media\s*\(min-width:\s*640px\)\s*\{\s*\.session-stats-grid\s*\{[^}]*grid-template-columns:\s*repeat\(2,\s*minmax\(0,
                // \s*1fr\)\)\s*!important;”` sebagai argumen ke konstruktor `Regex`.
                @"@media\s*\(min-width:\s*640px\)\s*\{\s*\.session-stats-grid\s*\{[^}]*grid-template-columns:\s*repeat\(2,\s*minmax\(0,\s*1fr\)\)\s*!important;",
                // Meneruskan `RegexOptions.Singleline` (nilai singleline) sebagai argumen ke konstruktor `Regex`.
                RegexOptions.Singleline),
            // Meneruskan `cssContent` (nilai css content) sebagai argumen ke `Assert.Matches`.
            cssContent);
        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan `new Regex(
        // @”@media\s*\(min-width:\s*1024px\)\s*\{\s*\.session-stats-grid\s*\{[^}]*grid-template-columns:\s*repeat\(3,\s*minmax\(0,\s*1fr\)\)\s*!important;”
        // , RegexOptions.Sing...`, `cssContent`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // SiteCss_ShouldRenderSessionSummaryCardsWithResponsiveColumns.
        Assert.Matches(
            // Meneruskan objek baru bertipe `Regex` dengan argumen (
            // @”@media\s*\(min-width:\s*1024px\)\s*\{\s*\.session-stats-grid\s*\{[^}]*grid-template-columns:\s*repeat\(3,\s*minmax\(0,\s*1fr\)\)\s*!important;”
            // , RegexOption... sebagai argumen ke `Assert.Matches`.
            new Regex(
                // Meneruskan nilai literal `@”@media\s*\(min-width:\s*1024px\)\s*\{\s*\.session-stats-grid\s*\{[^}]*grid-template-columns:\s*repeat\(3,\s*minmax\(0
                // ,\s*1fr\)\)\s*!important;”` sebagai argumen ke konstruktor `Regex`.
                @"@media\s*\(min-width:\s*1024px\)\s*\{\s*\.session-stats-grid\s*\{[^}]*grid-template-columns:\s*repeat\(3,\s*minmax\(0,\s*1fr\)\)\s*!important;",
                // Meneruskan `RegexOptions.Singleline` (nilai singleline) sebagai argumen ke konstruktor `Regex`.
                RegexOptions.Singleline),
            // Meneruskan `cssContent` (nilai css content) sebagai argumen ke `Assert.Matches`.
            cssContent);
        // Menjalankan pemeriksaan hasil dengan `Assert.DoesNotMatch` menggunakan `new Regex(@”\.session-total-card\s*\{[^}]*grid-column:\s*1\s*/\s*-1”,
        // RegexOptions.Singleline)`, `cssContent`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // SiteCss_ShouldRenderSessionSummaryCardsWithResponsiveColumns.
        Assert.DoesNotMatch(
            // Meneruskan objek baru bertipe `Regex` dengan argumen (@”\.session-total-card\s*\{[^}]*grid-column:\s*1\s*/\s*-1”, RegexOptions.Singleline)
            // sebagai argumen ke `Assert.DoesNotMatch`; Meneruskan nilai literal `@”\.session-total-card\s*\{[^}]*grid-column:\s*1\s*/\s*-1”` sebagai argumen
            // ke konstruktor `Regex`; Meneruskan `RegexOptions.Singleline` (nilai singleline) sebagai argumen ke konstruktor `Regex`.
            new Regex(@"\.session-total-card\s*\{[^}]*grid-column:\s*1\s*/\s*-1", RegexOptions.Singleline),
            // Meneruskan `cssContent` (nilai css content) sebagai argumen ke `Assert.DoesNotMatch`.
            cssContent);
        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan `new Regex(@”\.session-total-card\s*\{[^}]*grid-column:\s*auto\s*!important;”,
        // RegexOptions.Singleline)`, `cssContent`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // SiteCss_ShouldRenderSessionSummaryCardsWithResponsiveColumns.
        Assert.Matches(
            // Meneruskan objek baru bertipe `Regex` dengan argumen (@”\.session-total-card\s*\{[^}]*grid-column:\s*auto\s*!important;”,
            // RegexOptions.Singleline) sebagai argumen ke `Assert.Matches`; Meneruskan nilai literal
            // `@”\.session-total-card\s*\{[^}]*grid-column:\s*auto\s*!important;”` sebagai argumen ke konstruktor `Regex`; Meneruskan `RegexOptions.Singleline`
            // (nilai singleline) sebagai argumen ke konstruktor `Regex`.
            new Regex(@"\.session-total-card\s*\{[^}]*grid-column:\s*auto\s*!important;", RegexOptions.Singleline),
            // Meneruskan `cssContent` (nilai css content) sebagai argumen ke `Assert.Matches`.
            cssContent);
    // Menutup scope metode SiteCss_ShouldRenderSessionSummaryCardsWithResponsiveColumns; bagian berikut berada di luar batas blok tersebut dalam
    // SiteCss_ShouldRenderSessionSummaryCardsWithResponsiveColumns.
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
// Menutup scope tipe SessionIndexLayoutTests; bagian berikut berada di luar batas blok tersebut.
}
