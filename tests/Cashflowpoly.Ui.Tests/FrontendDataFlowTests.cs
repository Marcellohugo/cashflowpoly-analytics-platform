// Fungsi file: Memverifikasi pengambilan data realtime frontend tetap memakai status dan autentikasi yang benar.
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `FrontendDataFlowTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class FrontendDataFlowTests
// Membuka scope tipe FrontendDataFlowTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
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
    // Mendefinisikan metode `HomeRealtimeStats_ShouldCountOnlyStartedSessionsAsActive` dengan hasil bertipe `void`; operasi ini menangani home realtime
    // stats should jumlah only started sessions as aktif.
    public void HomeRealtimeStats_ShouldCountOnlyStartedSessionsAsActive()
    // Membuka scope metode HomeRealtimeStats_ShouldCountOnlyStartedSessionsAsActive; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // HomeRealtimeStats_ShouldCountOnlyStartedSessionsAsActive.
    {
        // Menyiapkan variabel lokal `controller` untuk nilai controller dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Controllers”,
        // ”HomeController.cs”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var controller = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "HomeController.cs"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”string.Equals(s.Status, \”STARTED\””`,
        // `controller`, `StringComparison.Ordinal` dalam HomeRealtimeStats_ShouldCountOnlyStartedSessionsAsActive.
        Assert.Contains("string.Equals(s.Status, \"STARTED\"", controller, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ActiveSessions = sessions.Count(s
        // => string.Equals(s.Status, \”ENDED\””`, `controller`, `StringComparison.Ordinal` dalam HomeRealtimeStats_ShouldCountOnlyStartedSessionsAsActive.
        Assert.DoesNotContain("ActiveSessions = sessions.Count(s => string.Equals(s.Status, \"ENDED\"", controller, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”TotalRulesets = rulesets.Count(r =>
        // string.Equals(r.Status, \”ACTIVE\””`, `controller`, `StringComparison.Ordinal` dalam HomeRealtimeStats_ShouldCountOnlyStartedSessionsAsActive.
        Assert.Contains("TotalRulesets = rulesets.Count(r => string.Equals(r.Status, \"ACTIVE\"", controller, StringComparison.Ordinal);
        Assert.Contains("api/v1/players?inMySessions=true", controller, StringComparison.Ordinal);
    // Menutup scope metode HomeRealtimeStats_ShouldCountOnlyStartedSessionsAsActive; bagian berikut berada di luar batas blok tersebut dalam
    // HomeRealtimeStats_ShouldCountOnlyStartedSessionsAsActive.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Authentication_ShouldUseEncryptedCookieClaimsInsteadOfServerMemorySession` dengan hasil bertipe `void`; operasi ini
    // menangani authentication should use encrypted cookie claims instead of server memory sesi.
    public void Authentication_ShouldUseEncryptedCookieClaimsInsteadOfServerMemorySession()
    // Membuka scope metode Authentication_ShouldUseEncryptedCookieClaimsInsteadOfServerMemorySession; pernyataan/deklarasi berikut berada di dalam
    // batas blok ini dalam Authentication_ShouldUseEncryptedCookieClaimsInsteadOfServerMemorySession.
    {
        // Menyiapkan variabel lokal `program` untuk nilai program dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Program.cs”)`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var program = File.ReadAllText(Path.Combine(UiRoot, "Program.cs"));
        // Menyiapkan variabel lokal `bearerHandler` untuk nilai bearer handler dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot,
        // ”Infrastructure”, ”BearerTokenHandler.cs”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var bearerHandler = File.ReadAllText(Path.Combine(UiRoot, "Infrastructure", "BearerTokenHandler.cs"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”AddCookie”`, `program`,
        // `StringComparison.Ordinal` dalam Authentication_ShouldUseEncryptedCookieClaimsInsteadOfServerMemorySession.
        Assert.Contains("AddCookie", program, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”UseAuthentication”`, `program`,
        // `StringComparison.Ordinal` dalam Authentication_ShouldUseEncryptedCookieClaimsInsteadOfServerMemorySession.
        Assert.Contains("UseAuthentication", program, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”AccessTokenClaim”`, `bearerHandler`,
        // `StringComparison.Ordinal` dalam Authentication_ShouldUseEncryptedCookieClaimsInsteadOfServerMemorySession.
        Assert.Contains("AccessTokenClaim", bearerHandler, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”SessionAccessTokenKey”`,
        // `bearerHandler`, `StringComparison.Ordinal` dalam Authentication_ShouldUseEncryptedCookieClaimsInsteadOfServerMemorySession.
        Assert.DoesNotContain("SessionAccessTokenKey", bearerHandler, StringComparison.Ordinal);
    // Menutup scope metode Authentication_ShouldUseEncryptedCookieClaimsInsteadOfServerMemorySession; bagian berikut berada di luar batas blok tersebut
    // dalam Authentication_ShouldUseEncryptedCookieClaimsInsteadOfServerMemorySession.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerDirectory_ShouldBoundRequestsAndReuseEndedSessionAnalytics` dengan hasil bertipe `void`; operasi ini menangani
    // pemain directory should bound requests dan reuse ended sesi analytics.
    public void PlayerDirectory_ShouldBoundRequestsAndReuseEndedSessionAnalytics()
    // Membuka scope metode PlayerDirectory_ShouldBoundRequestsAndReuseEndedSessionAnalytics; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam PlayerDirectory_ShouldBoundRequestsAndReuseEndedSessionAnalytics.
    {
        // Menyiapkan variabel lokal `controller` untuk nilai controller dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Controllers”,
        // ”PlayerDirectoryController.cs”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var controller = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "PlayerDirectoryController.cs"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”new SemaphoreSlim(8)”`, `controller`,
        // `StringComparison.Ordinal` dalam PlayerDirectory_ShouldBoundRequestsAndReuseEndedSessionAnalytics.
        Assert.Contains("new SemaphoreSlim(8)", controller, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”analyticsParticipants”`, `controller`,
        // `StringComparison.Ordinal` dalam PlayerDirectory_ShouldBoundRequestsAndReuseEndedSessionAnalytics.
        Assert.Contains("analyticsParticipants", controller, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”players.error.load_session_details_partial”`, `controller`, `StringComparison.Ordinal` dalam
        // PlayerDirectory_ShouldBoundRequestsAndReuseEndedSessionAnalytics.
        Assert.Contains("players.error.load_session_details_partial", controller, StringComparison.Ordinal);
    // Menutup scope metode PlayerDirectory_ShouldBoundRequestsAndReuseEndedSessionAnalytics; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerDirectory_ShouldBoundRequestsAndReuseEndedSessionAnalytics.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `HomeRealtimePolling_ShouldRedirectWhenAuthenticationExpires` dengan hasil bertipe `void`; operasi ini menangani home
    // realtime polling should redirect when authentication expires.
    public void HomeRealtimePolling_ShouldRedirectWhenAuthenticationExpires()
    // Membuka scope metode HomeRealtimePolling_ShouldRedirectWhenAuthenticationExpires; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam HomeRealtimePolling_ShouldRedirectWhenAuthenticationExpires.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Home”,
        // ”Index.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Home", "Index.cshtml"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”response.redirected &&
        // response.url.includes(\”/auth/login\”)”`, `view`, `StringComparison.Ordinal` dalam HomeRealtimePolling_ShouldRedirectWhenAuthenticationExpires.
        Assert.Contains("response.redirected && response.url.includes(\"/auth/login\")", view, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”window.location.assign(response.url)”`,
        // `view`, `StringComparison.Ordinal` dalam HomeRealtimePolling_ShouldRedirectWhenAuthenticationExpires.
        Assert.Contains("window.location.assign(response.url)", view, StringComparison.Ordinal);
    // Menutup scope metode HomeRealtimePolling_ShouldRedirectWhenAuthenticationExpires; bagian berikut berada di luar batas blok tersebut dalam
    // HomeRealtimePolling_ShouldRedirectWhenAuthenticationExpires.
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
// Menutup scope tipe FrontendDataFlowTests; bagian berikut berada di luar batas blok tersebut.
}
