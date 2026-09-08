// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui WideViewportShellLayoutTests.
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `WideViewportShellLayoutTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class WideViewportShellLayoutTests
// Membuka scope tipe WideViewportShellLayoutTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SiteCss_ShouldUseSharedWideShellWidthForNavPageAndFooter` dengan hasil bertipe `void`; operasi ini menangani site css
    // should use shared wide shell width untuk nav page dan footer.
    public void SiteCss_ShouldUseSharedWideShellWidthForNavPageAndFooter()
    // Membuka scope metode SiteCss_ShouldUseSharedWideShellWidthForNavPageAndFooter; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // SiteCss_ShouldUseSharedWideShellWidthForNavPageAndFooter.
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

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”--shell-max-width:”`, `cssContent`
        // dalam SiteCss_ShouldUseSharedWideShellWidthForNavPageAndFooter.
        Assert.Contains("--shell-max-width:", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”--shell-gutter:”`, `cssContent` dalam
        // SiteCss_ShouldUseSharedWideShellWidthForNavPageAndFooter.
        Assert.Contains("--shell-gutter:", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.nav-shell,”`, `cssContent` dalam
        // SiteCss_ShouldUseSharedWideShellWidthForNavPageAndFooter.
        Assert.Contains(".nav-shell,", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.page-shell,”`, `cssContent` dalam
        // SiteCss_ShouldUseSharedWideShellWidthForNavPageAndFooter.
        Assert.Contains(".page-shell,", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”body > footer .footer-shell {”`,
        // `cssContent` dalam SiteCss_ShouldUseSharedWideShellWidthForNavPageAndFooter.
        Assert.Contains("body > footer .footer-shell {", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”width: min(var(--shell-max-width),
        // calc(100% - (var(--shell-gutter) * 2)));”`, `cssContent` dalam SiteCss_ShouldUseSharedWideShellWidthForNavPageAndFooter.
        Assert.Contains("width: min(var(--shell-max-width), calc(100% - (var(--shell-gutter) * 2)));", cssContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”width: min(1280px, calc(100% -
        // 2rem));”`, `cssContent` dalam SiteCss_ShouldUseSharedWideShellWidthForNavPageAndFooter.
        Assert.DoesNotContain("width: min(1280px, calc(100% - 2rem));", cssContent);
    // Menutup scope metode SiteCss_ShouldUseSharedWideShellWidthForNavPageAndFooter; bagian berikut berada di luar batas blok tersebut dalam
    // SiteCss_ShouldUseSharedWideShellWidthForNavPageAndFooter.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SiteCss_ShouldKeepCompactNavigationAndCalendarUsableOnMobile` dengan hasil bertipe `void`; operasi ini menangani site css
    // should keep compact navigation dan calendar usable on mobile.
    public void SiteCss_ShouldKeepCompactNavigationAndCalendarUsableOnMobile()
    // Membuka scope metode SiteCss_ShouldKeepCompactNavigationAndCalendarUsableOnMobile; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam SiteCss_ShouldKeepCompactNavigationAndCalendarUsableOnMobile.
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

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.nav-shell-auth .nav-brand {”`,
        // `cssContent` dalam SiteCss_ShouldKeepCompactNavigationAndCalendarUsableOnMobile.
        Assert.Contains(".nav-shell-auth .nav-brand {", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”min-width: min(17rem, calc(100vw -
        // 3rem));”`, `cssContent` dalam SiteCss_ShouldKeepCompactNavigationAndCalendarUsableOnMobile.
        Assert.Contains("min-width: min(17rem, calc(100vw - 3rem));", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”box-sizing: border-box;”`, `cssContent`
        // dalam SiteCss_ShouldKeepCompactNavigationAndCalendarUsableOnMobile.
        Assert.Contains("box-sizing: border-box;", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.auth-panel-brand-text {”`,
        // `cssContent` dalam SiteCss_ShouldKeepCompactNavigationAndCalendarUsableOnMobile.
        Assert.Contains(".auth-panel-brand-text {", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”overflow-wrap: normal !important;”`,
        // `cssContent` dalam SiteCss_ShouldKeepCompactNavigationAndCalendarUsableOnMobile.
        Assert.Contains("overflow-wrap: normal !important;", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”grid-template-columns: repeat(7,
        // minmax(0, 1fr));”`, `cssContent` dalam SiteCss_ShouldKeepCompactNavigationAndCalendarUsableOnMobile.
        Assert.Contains("grid-template-columns: repeat(7, minmax(0, 1fr));", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.work-calendar-cell.is-selected-day
        // {”`, `cssContent` dalam SiteCss_ShouldKeepCompactNavigationAndCalendarUsableOnMobile.
        Assert.Contains(".work-calendar-cell.is-selected-day {", cssContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”min-width: 24rem;”`, `cssContent`
        // dalam SiteCss_ShouldKeepCompactNavigationAndCalendarUsableOnMobile.
        Assert.DoesNotContain("min-width: 24rem;", cssContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”min-height: 2.75rem !important;”`,
        // `cssContent` dalam SiteCss_ShouldKeepCompactNavigationAndCalendarUsableOnMobile.
        Assert.Contains("min-height: 2.75rem !important;", cssContent);
    // Menutup scope metode SiteCss_ShouldKeepCompactNavigationAndCalendarUsableOnMobile; bagian berikut berada di luar batas blok tersebut dalam
    // SiteCss_ShouldKeepCompactNavigationAndCalendarUsableOnMobile.
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
// Menutup scope tipe WideViewportShellLayoutTests; bagian berikut berada di luar batas blok tersebut.
}
