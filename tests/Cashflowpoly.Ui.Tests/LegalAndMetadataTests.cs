// Fungsi file: Memverifikasi halaman legal serta metadata publik mengikuti konfigurasi DOMAIN.
// Mengimpor namespace `Cashflowpoly.Ui.Controllers` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Controllers;
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;
// Mengimpor namespace `Microsoft.Extensions.Caching.Memory` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.Extensions.Caching.Memory;
// Mengimpor namespace `Microsoft.Extensions.Configuration` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.Extensions.Configuration;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `LegalAndMetadataTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class LegalAndMetadataTests
// Membuka scope tipe LegalAndMetadataTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SiteUrlResolver_ShouldNormalizeConfiguredDomain` dengan hasil bertipe `void`; operasi ini menangani site url resolver
    // should normalize configured domain.
    public void SiteUrlResolver_ShouldNormalizeConfiguredDomain()
    // Membuka scope metode SiteUrlResolver_ShouldNormalizeConfiguredDomain; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // SiteUrlResolver_ShouldNormalizeConfiguredDomain.
    {
        // Menyiapkan variabel lokal `configuration` untuk konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar dengan memanggil
        // `BuildConfiguration` dengan `”narafin.org”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var configuration = BuildConfiguration("narafin.org");
        // Menyiapkan variabel lokal `context` untuk konteks operasi yang menyediakan data lingkungan pemrosesan saat ini dengan objek baru bertipe
        // `DefaultHttpContext` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var context = new DefaultHttpContext();
        // Memperbarui `context.Request.Scheme` menggunakan nilai literal `”http”` dalam SiteUrlResolver_ShouldNormalizeConfiguredDomain.
        context.Request.Scheme = "http";
        // Memperbarui `context.Request.Host` menggunakan objek baru bertipe `HostString` dengan argumen (”localhost”, 5203) dalam
        // SiteUrlResolver_ShouldNormalizeConfiguredDomain.
        context.Request.Host = new HostString("localhost", 5203);

        // Menyiapkan variabel lokal `baseUrl` untuk nilai base url dengan memanggil `SiteUrlResolver.ResolveBaseUrl` dengan `configuration`,
        // `context.Request`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var baseUrl = SiteUrlResolver.ResolveBaseUrl(configuration, context.Request);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”https://narafin.org”`, `baseUrl`); pengujian
        // gagal jika keduanya berbeda dalam SiteUrlResolver_ShouldNormalizeConfiguredDomain.
        Assert.Equal("https://narafin.org", baseUrl);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”https://narafin.org/privacy”`,
        // `SiteUrlResolver.BuildAbsoluteUrl(baseUrl, ”/privacy”)`); pengujian gagal jika keduanya berbeda dalam
        // SiteUrlResolver_ShouldNormalizeConfiguredDomain.
        Assert.Equal("https://narafin.org/privacy", SiteUrlResolver.BuildAbsoluteUrl(baseUrl, "/privacy"));
    // Menutup scope metode SiteUrlResolver_ShouldNormalizeConfiguredDomain; bagian berikut berada di luar batas blok tersebut dalam
    // SiteUrlResolver_ShouldNormalizeConfiguredDomain.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SiteUrlResolver_ShouldPreserveConfiguredLocalSchemeAndFallbackToRequest` dengan hasil bertipe `void`; operasi ini
    // menangani site url resolver should preserve configured local scheme dan fallback ke permintaan.
    public void SiteUrlResolver_ShouldPreserveConfiguredLocalSchemeAndFallbackToRequest()
    // Membuka scope metode SiteUrlResolver_ShouldPreserveConfiguredLocalSchemeAndFallbackToRequest; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam SiteUrlResolver_ShouldPreserveConfiguredLocalSchemeAndFallbackToRequest.
    {
        // Menyiapkan variabel lokal `context` untuk konteks operasi yang menyediakan data lingkungan pemrosesan saat ini dengan objek baru bertipe
        // `DefaultHttpContext` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var context = new DefaultHttpContext();
        // Memperbarui `context.Request.Scheme` menggunakan nilai literal `”http”` dalam
        // SiteUrlResolver_ShouldPreserveConfiguredLocalSchemeAndFallbackToRequest.
        context.Request.Scheme = "http";
        // Memperbarui `context.Request.Host` menggunakan objek baru bertipe `HostString` dengan argumen (”localhost”, 5203) dalam
        // SiteUrlResolver_ShouldPreserveConfiguredLocalSchemeAndFallbackToRequest.
        context.Request.Host = new HostString("localhost", 5203);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”http://localhost:5203”`,
        // `SiteUrlResolver.ResolveBaseUrl(BuildConfiguration(”http://localhost:5203”), context.Request)`); pengujian gagal jika keduanya berbeda dalam
        // SiteUrlResolver_ShouldPreserveConfiguredLocalSchemeAndFallbackToRequest.
        Assert.Equal(
            // Meneruskan nilai literal `”http://localhost:5203”` sebagai argumen ke `Assert.Equal`.
            "http://localhost:5203",
            // Meneruskan memanggil `SiteUrlResolver.ResolveBaseUrl` dengan `BuildConfiguration(”http://localhost:5203”)`, `context.Request` sebagai argumen ke
            // `Assert.Equal`; Meneruskan memanggil `BuildConfiguration` dengan `”http://localhost:5203”` sebagai argumen ke `SiteUrlResolver.ResolveBaseUrl`;
            // Meneruskan nilai literal `”http://localhost:5203”` sebagai argumen ke `BuildConfiguration`; Meneruskan `context.Request` (data masukan permintaan
            // yang akan divalidasi atau diteruskan ke layanan) sebagai argumen ke `SiteUrlResolver.ResolveBaseUrl`.
            SiteUrlResolver.ResolveBaseUrl(BuildConfiguration("http://localhost:5203"), context.Request));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”http://localhost:5203”`,
        // `SiteUrlResolver.ResolveBaseUrl(BuildConfiguration(null), context.Request)`); pengujian gagal jika keduanya berbeda dalam
        // SiteUrlResolver_ShouldPreserveConfiguredLocalSchemeAndFallbackToRequest.
        Assert.Equal(
            // Meneruskan nilai literal `”http://localhost:5203”` sebagai argumen ke `Assert.Equal`.
            "http://localhost:5203",
            // Meneruskan memanggil `SiteUrlResolver.ResolveBaseUrl` dengan `BuildConfiguration(null)`, `context.Request` sebagai argumen ke `Assert.Equal`;
            // Meneruskan memanggil `BuildConfiguration` dengan `null` sebagai argumen ke `SiteUrlResolver.ResolveBaseUrl`; Meneruskan null, yaitu penanda tidak
            // ada nilai sebagai argumen ke `BuildConfiguration`; Meneruskan `context.Request` (data masukan permintaan yang akan divalidasi atau diteruskan ke
            // layanan) sebagai argumen ke `SiteUrlResolver.ResolveBaseUrl`.
            SiteUrlResolver.ResolveBaseUrl(BuildConfiguration(null), context.Request));
    // Menutup scope metode SiteUrlResolver_ShouldPreserveConfiguredLocalSchemeAndFallbackToRequest; bagian berikut berada di luar batas blok tersebut
    // dalam SiteUrlResolver_ShouldPreserveConfiguredLocalSchemeAndFallbackToRequest.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles` dengan hasil bertipe `void`; operasi ini menangani
    // home controller should expose separate legal pages dan configured crawler files.
    public void HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles()
    // Membuka scope metode HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
    {
        // Menyiapkan variabel lokal `cache` untuk nilai cache dengan objek baru bertipe `MemoryCache` dengan argumen (new MemoryCacheOptions()). Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var cache = new MemoryCache(new MemoryCacheOptions());
        // Menyiapkan variabel lokal `controller` untuk nilai controller dengan objek baru bertipe `HomeController` dengan argumen (new
        // UnusedHttpClientFactory(), cache, BuildConfiguration(”narafin.org”)). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var controller = new HomeController(new UnusedHttpClientFactory(), cache, BuildConfiguration("narafin.org"))
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
        {
            // Memperbarui `ControllerContext` menggunakan objek baru bertipe `ControllerContext` dengan nilai awal sesuai konstruktornya dalam
            // HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
            ControllerContext = new ControllerContext
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
            {
                // Memperbarui `HttpContext` menggunakan objek baru bertipe `DefaultHttpContext` dengan nilai awal sesuai konstruktornya dalam
                // HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
                HttpContext = new DefaultHttpContext()
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
            // HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
            }
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
        };

        // Menyiapkan variabel lokal `privacy` untuk nilai privacy dengan pemeriksaan hasil dengan `Assert.IsType<ViewResult>` menggunakan
        // `controller.Privacy()`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var privacy = Assert.IsType<ViewResult>(controller.Privacy());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”PrivacyPolicy”`, `privacy.ViewName`);
        // pengujian gagal jika keduanya berbeda dalam HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
        Assert.Equal("PrivacyPolicy", privacy.ViewName);
        // Menjalankan pemeriksaan hasil dengan `Assert.IsType<ViewResult>` menggunakan `controller.Terms()`; ketidaksesuaian dengan ekspektasi membuat
        // pengujian gagal dalam HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
        Assert.IsType<ViewResult>(controller.Terms());

        // Menyiapkan variabel lokal `robots` untuk nilai robots dengan pemeriksaan hasil dengan `Assert.IsType<ContentResult>` menggunakan
        // `controller.Robots()`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var robots = Assert.IsType<ContentResult>(controller.Robots());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”text/plain”`, `robots.ContentType`);
        // pengujian gagal jika keduanya berbeda dalam HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
        Assert.Equal("text/plain", robots.ContentType);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Sitemap:
        // https://narafin.org/sitemap.xml”`, `robots.Content`, `StringComparison.Ordinal` dalam
        // HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
        Assert.Contains("Sitemap: https://narafin.org/sitemap.xml", robots.Content, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Disallow: /auth/”`, `robots.Content`,
        // `StringComparison.Ordinal` dalam HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
        Assert.Contains("Disallow: /auth/", robots.Content, StringComparison.Ordinal);

        // Menyiapkan variabel lokal `sitemap` untuk nilai sitemap dengan pemeriksaan hasil dengan `Assert.IsType<ContentResult>` menggunakan
        // `controller.Sitemap()`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sitemap = Assert.IsType<ContentResult>(controller.Sitemap());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”application/xml”`, `sitemap.ContentType`);
        // pengujian gagal jika keduanya berbeda dalam HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
        Assert.Equal("application/xml", sitemap.ContentType);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”<loc>https://narafin.org/</loc>”`,
        // `sitemap.Content`, `StringComparison.Ordinal` dalam HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
        Assert.Contains("<loc>https://narafin.org/</loc>", sitemap.Content, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”<loc>https://narafin.org/rulebook</loc>”`, `sitemap.Content`, `StringComparison.Ordinal` dalam
        // HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
        Assert.Contains("<loc>https://narafin.org/rulebook</loc>", sitemap.Content, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”/login”`, `sitemap.Content`,
        // `StringComparison.Ordinal` dalam HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
        Assert.DoesNotContain("/login", sitemap.Content, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”/register”`, `sitemap.Content`,
        // `StringComparison.Ordinal` dalam HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
        Assert.DoesNotContain("/register", sitemap.Content, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”/privacy</loc>”`,
        // `sitemap.Content`, `StringComparison.Ordinal` dalam HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
        Assert.DoesNotContain("/privacy</loc>", sitemap.Content, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”/terms</loc>”`,
        // `sitemap.Content`, `StringComparison.Ordinal` dalam HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
        Assert.DoesNotContain("/terms</loc>", sitemap.Content, StringComparison.Ordinal);
    // Menutup scope metode HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles; bagian berikut berada di luar batas blok tersebut
    // dalam HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex` dengan hasil bertipe `void`; operasi ini menangani
    // layout dan routes should keep legal pages public dan private pages no index.
    public void LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex()
    // Membuka scope metode LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `layout` untuk nilai layout dengan memanggil `File.ReadAllText` dengan `Path.Combine(repoRoot, ”src”,
        // ”Cashflowpoly.Ui”, ”Views”, ”Shared”, ”_Layout.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var layout = File.ReadAllText(Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Shared", "_Layout.cshtml"));
        // Menyiapkan variabel lokal `program` untuk nilai program dengan memanggil `File.ReadAllText` dengan `Path.Combine(repoRoot, ”src”,
        // ”Cashflowpoly.Ui”, ”Program.cs”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var program = File.ReadAllText(Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Program.cs"));
        // Menyiapkan variabel lokal `webRoot` untuk nilai web root dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”wwwroot”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var webRoot = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "wwwroot");

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”SiteUrlResolver.ResolveBaseUrl”`,
        // `layout`, `StringComparison.Ordinal` dalam LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex.
        Assert.Contains("SiteUrlResolver.ResolveBaseUrl", layout, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”? \”noindex, follow\””`, `layout`,
        // `StringComparison.Ordinal` dalam LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex.
        Assert.Contains("? \"noindex, follow\"", layout, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”: \”noindex, nofollow\””`, `layout`,
        // `StringComparison.Ordinal` dalam LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex.
        Assert.Contains(": \"noindex, nofollow\"", layout, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”href=\”/privacy\””`, `layout`,
        // `StringComparison.Ordinal` dalam LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex.
        Assert.Contains("href=\"/privacy\"", layout, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”href=\”/terms\””`, `layout`,
        // `StringComparison.Ordinal` dalam LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex.
        Assert.Contains("href=\"/terms\"", layout, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”https://narafin.org”`, `layout`,
        // `StringComparison.Ordinal` dalam LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex.
        Assert.DoesNotContain("https://narafin.org", layout, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”isLegalPath”`, `program`,
        // `StringComparison.Ordinal` dalam LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex.
        Assert.Contains("isLegalPath", program, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”pattern: \”privacy\””`, `program`,
        // `StringComparison.Ordinal` dalam LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex.
        Assert.Contains("pattern: \"privacy\"", program, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”pattern: \”terms\””`, `program`,
        // `StringComparison.Ordinal` dalam LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex.
        Assert.Contains("pattern: \"terms\"", program, StringComparison.Ordinal);
        // Menjalankan pemeriksaan bahwa `File.Exists(Path.Combine(webRoot, ”robots.txt”))` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
        // dalam LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex.
        Assert.False(File.Exists(Path.Combine(webRoot, "robots.txt")));
        // Menjalankan pemeriksaan bahwa `File.Exists(Path.Combine(webRoot, ”sitemap.xml”))` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
        // dalam LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex.
        Assert.False(File.Exists(Path.Combine(webRoot, "sitemap.xml")));
    // Menutup scope metode LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex; bagian berikut berada di luar batas blok tersebut dalam
    // LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex.
    }

    // Mendefinisikan metode `BuildConfiguration` dengan hasil bertipe `IConfiguration`; operasi ini menangani build configuration. Masukan: Parameter
    // `domain` bertipe `string?` membawa nilai domain; nilai null diizinkan ketika data opsional belum tersedia.
    private static IConfiguration BuildConfiguration(string? domain)
    // Membuka scope metode BuildConfiguration; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildConfiguration.
    {
        // Menyiapkan variabel lokal `values` untuk nilai nilai dengan objek baru bertipe `Dictionary<string, string?>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var values = new Dictionary<string, string?>();
        // Memeriksa hasil pencocokan `domain` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildConfiguration.
        if (domain is not null)
        // Membuka scope cabang if untuk kondisi `domain is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildConfiguration.
        {
            // Memperbarui `values[”DOMAIN”]` menggunakan `domain` (nilai domain) dalam BuildConfiguration.
            values["DOMAIN"] = domain;
        // Menutup scope cabang if untuk kondisi `domain is not null`; bagian berikut berada di luar batas blok tersebut dalam BuildConfiguration.
        }

        // Mengembalikan memanggil `new ConfigurationBuilder() .AddInMemoryCollection(values) .Build` dengan tanpa argumen kepada pemanggil dalam
        // BuildConfiguration; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new ConfigurationBuilder()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .AddInMemoryCollection(values) dalam BuildConfiguration; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .AddInMemoryCollection(values)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Build(); dalam BuildConfiguration; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .Build();
    // Menutup scope metode BuildConfiguration; bagian berikut berada di luar batas blok tersebut dalam BuildConfiguration.
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

    // Mendefinisikan tipe class `UnusedHttpClientFactory` yang mewarisi atau menerapkan `IHttpClientFactory`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class UnusedHttpClientFactory : IHttpClientFactory
    // Membuka scope tipe UnusedHttpClientFactory; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan metode `CreateClient` dengan hasil bertipe `HttpClient`; operasi ini menangani create client. Masukan: Parameter `name` bertipe
        // `string` membawa nilai nama. Nilai hasil langsung berasal dari `throw new InvalidOperationException(”Tidak digunakan oleh pengujian ini.”)`.
        public HttpClient CreateClient(string name) => throw new InvalidOperationException("Tidak digunakan oleh pengujian ini.");
    // Menutup scope tipe UnusedHttpClientFactory; bagian berikut berada di luar batas blok tersebut.
    }
// Menutup scope tipe LegalAndMetadataTests; bagian berikut berada di luar batas blok tersebut.
}
