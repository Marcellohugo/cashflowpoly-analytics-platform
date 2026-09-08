// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui HttpsRedirectionPolicy.
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;
// Mengimpor namespace `Microsoft.Extensions.Configuration` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.Extensions.Configuration;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

// Mendefinisikan tipe class `HttpsRedirectionPolicy`.
public static class HttpsRedirectionPolicy
// Membuka scope tipe HttpsRedirectionPolicy; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `ResolveCookieSecurePolicy` dengan hasil bertipe `CookieSecurePolicy`; operasi ini menangani resolve cookie secure policy.
    // Masukan: Parameter `configuration` bertipe `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar.
    public static CookieSecurePolicy ResolveCookieSecurePolicy(IConfiguration configuration)
    // Membuka scope metode ResolveCookieSecurePolicy; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveCookieSecurePolicy.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `ShouldUseHttpsRedirection(configuration)` benar gunakan `CookieSecurePolicy.Always`, jika tidak
        // gunakan `CookieSecurePolicy.SameAsRequest` kepada pemanggil dalam ResolveCookieSecurePolicy; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return ShouldUseHttpsRedirection(configuration)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: CookieSecurePolicy.Always dalam ResolveCookieSecurePolicy.
            ? CookieSecurePolicy.Always
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: CookieSecurePolicy.SameAsRequest; dalam ResolveCookieSecurePolicy.
            : CookieSecurePolicy.SameAsRequest;
    // Menutup scope metode ResolveCookieSecurePolicy; bagian berikut berada di luar batas blok tersebut dalam ResolveCookieSecurePolicy.
    }

    // Mendefinisikan metode `ShouldUseHttpsRedirection` dengan hasil bertipe `bool`; operasi ini menangani should use https redirection. Masukan:
    // Parameter `configuration` bertipe `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar.
    public static bool ShouldUseHttpsRedirection(IConfiguration configuration)
    // Membuka scope metode ShouldUseHttpsRedirection; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ShouldUseHttpsRedirection.
    {
        // Memeriksa memanggil `configuration.GetValue<bool>` dengan `”Security:RequireHttps”`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ShouldUseHttpsRedirection.
        if (configuration.GetValue<bool>("Security:RequireHttps"))
        // Membuka scope cabang if untuk kondisi `configuration.GetValue<bool>(”Security:RequireHttps”)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam ShouldUseHttpsRedirection.
        {
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam ShouldUseHttpsRedirection; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `configuration.GetValue<bool>(”Security:RequireHttps”)`; bagian berikut berada di luar batas blok tersebut
        // dalam ShouldUseHttpsRedirection.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!string.IsNullOrWhiteSpace(configuration[”ASPNETCORE_HTTPS_PORT”])` dan
        // `!string.IsNullOrWhiteSpace(configuration[”HTTPS_PORT”])`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam ShouldUseHttpsRedirection.
        if (!string.IsNullOrWhiteSpace(configuration["ASPNETCORE_HTTPS_PORT"]) ||
            // Menggunakan kebalikan kondisi `string.IsNullOrWhiteSpace(configuration[”HTTPS_PORT”])` sebagai bagian ekspresi yang sedang disusun dalam
            // ShouldUseHttpsRedirection.
            !string.IsNullOrWhiteSpace(configuration["HTTPS_PORT"]))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(configuration[”ASPNETCORE_HTTPS_PORT”]) ||
        // !string.IsNullOrWhiteSpace(configuration[”HTTPS_PORT”])`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ShouldUseHttpsRedirection.
        {
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam ShouldUseHttpsRedirection; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(configuration[”ASPNETCORE_HTTPS_PORT”]) ||
        // !string.IsNullOrWhiteSpace(configuration[”HTTPS_PORT”])`; bagian berikut berada di luar batas blok tersebut dalam ShouldUseHttpsRedirection.
        }

        // Menyiapkan variabel lokal `urls` untuk nilai urls dengan `configuration[”ASPNETCORE_URLS”]` bila tidak null; jika null gunakan
        // `configuration[”URLS”]` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var urls = configuration["ASPNETCORE_URLS"] ?? configuration["URLS"];
        // Memeriksa memeriksa apakah `urls` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ShouldUseHttpsRedirection.
        if (string.IsNullOrWhiteSpace(urls))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(urls)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ShouldUseHttpsRedirection.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam ShouldUseHttpsRedirection; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(urls)`; bagian berikut berada di luar batas blok tersebut dalam
        // ShouldUseHttpsRedirection.
        }

        // Mengembalikan memeriksa apakah `urls .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)` memiliki setidaknya
        // satu elemen yang memenuhi `url => url.StartsWith(”https://”, StringComparison.OrdinalIgnoreCase)` kepada pemanggil dalam
        // ShouldUseHttpsRedirection; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return urls
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Split(';', StringSplitOptions.RemoveEmptyEntries |
            // StringSplitOptions.TrimEntries) dalam ShouldUseHttpsRedirection; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Any(url => url.StartsWith(”https://”, StringComparison.OrdinalIgnoreCase));
            // dalam ShouldUseHttpsRedirection; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Any(url => url.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
    // Menutup scope metode ShouldUseHttpsRedirection; bagian berikut berada di luar batas blok tersebut dalam ShouldUseHttpsRedirection.
    }
// Menutup scope tipe HttpsRedirectionPolicy; bagian berikut berada di luar batas blok tersebut.
}
