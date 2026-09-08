// Fungsi file: Menyusun URL publik aplikasi dari konfigurasi DOMAIN atau request aktif.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

// Mendefinisikan tipe class `SiteUrlResolver`.
public static class SiteUrlResolver
// Membuka scope tipe SiteUrlResolver; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `ResolveBaseUrl` dengan hasil bertipe `string`; operasi ini menangani resolve base url. Masukan: Parameter `configuration`
    // bertipe `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar; Parameter `request` bertipe
    // `HttpRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
    public static string ResolveBaseUrl(IConfiguration configuration, HttpRequest request)
    // Membuka scope metode ResolveBaseUrl; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveBaseUrl.
    {
        // Menyiapkan variabel lokal `configuredDomain` untuk nilai configured domain dengan `configuration[”DOMAIN”]?.Trim()`; akses setelah ?. hanya
        // dilakukan bila penerimanya tidak null. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var configuredDomain = configuration["DOMAIN"]?.Trim();
        // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(configuredDomain)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveBaseUrl.
        if (!string.IsNullOrWhiteSpace(configuredDomain))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(configuredDomain)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ResolveBaseUrl.
        {
            // Menyiapkan variabel lokal `absoluteDomain` untuk nilai absolute domain dengan hasil pemilihan bersyarat: ketika `configuredDomain.Contains(”://”,
            // StringComparison.Ordinal)` benar gunakan `configuredDomain`, jika tidak gunakan `$”https://{configuredDomain}”`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal.
            var absoluteDomain = configuredDomain.Contains("://", StringComparison.Ordinal)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: configuredDomain dalam ResolveBaseUrl.
                ? configuredDomain
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: $”https://{configuredDomain}”; dalam ResolveBaseUrl.
                : $"https://{configuredDomain}";

            // Memeriksa memanggil `Uri.TryCreate` dengan `absoluteDomain`, `UriKind.Absolute`, `var configuredUri`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam ResolveBaseUrl.
            if (Uri.TryCreate(absoluteDomain, UriKind.Absolute, out var configuredUri))
            // Membuka scope cabang if untuk kondisi `Uri.TryCreate(absoluteDomain, UriKind.Absolute, out var configuredUri)`; pernyataan/deklarasi berikut
            // berada di dalam batas blok ini dalam ResolveBaseUrl.
            {
                // Mengembalikan membersihkan karakter tepi pada `configuredUri.GetLeftPart(UriPartial.Authority)` memakai `'/'` kepada pemanggil dalam
                // ResolveBaseUrl; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return configuredUri.GetLeftPart(UriPartial.Authority).TrimEnd('/');
            // Menutup scope cabang if untuk kondisi `Uri.TryCreate(absoluteDomain, UriKind.Absolute, out var configuredUri)`; bagian berikut berada di luar
            // batas blok tersebut dalam ResolveBaseUrl.
            }

            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Konfigurasi DOMAIN harus berupa nama host atau
            // URL absolut yang valid.”) dalam ResolveBaseUrl; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Konfigurasi DOMAIN harus berupa nama host atau URL absolut yang valid.");
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(configuredDomain)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveBaseUrl.
        }

        // Mengembalikan membersihkan karakter tepi pada `$”{request.Scheme}://{request.Host}”` memakai `'/'` kepada pemanggil dalam ResolveBaseUrl;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return $"{request.Scheme}://{request.Host}".TrimEnd('/');
    // Menutup scope metode ResolveBaseUrl; bagian berikut berada di luar batas blok tersebut dalam ResolveBaseUrl.
    }

    // Mendefinisikan metode `BuildAbsoluteUrl` dengan hasil bertipe `string`; operasi ini menangani build absolute url. Masukan: Parameter `baseUrl`
    // bertipe `string` membawa nilai base url; Parameter `path` bertipe `PathString` membawa nilai path.
    public static string BuildAbsoluteUrl(string baseUrl, PathString path)
    // Membuka scope metode BuildAbsoluteUrl; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildAbsoluteUrl.
    {
        // Menyiapkan variabel lokal `normalizedPath` untuk nilai normalized path dengan hasil pemilihan bersyarat: ketika `path.HasValue` benar gunakan
        // `path.Value!`, jika tidak gunakan `”/”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var normalizedPath = path.HasValue ? path.Value! : "/";
        // Mengembalikan teks interpolasi `$”{baseUrl.TrimEnd('/')}/{normalizedPath.TrimStart('/')}”`; nilai ekspresi di dalam kurung kurawal disisipkan
        // saat program berjalan kepada pemanggil dalam BuildAbsoluteUrl; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return $"{baseUrl.TrimEnd('/')}/{normalizedPath.TrimStart('/')}";
    // Menutup scope metode BuildAbsoluteUrl; bagian berikut berada di luar batas blok tersebut dalam BuildAbsoluteUrl.
    }
// Menutup scope tipe SiteUrlResolver; bagian berikut berada di luar batas blok tersebut.
}
