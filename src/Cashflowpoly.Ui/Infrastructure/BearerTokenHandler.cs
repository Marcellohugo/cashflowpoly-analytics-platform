// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui BearerTokenHandler.
// Mengimpor namespace `System.Net.Http.Headers` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Headers;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Meneruskan konteks pengguna UI yang diperlukan API: token, bahasa, alamat klien,
/// dan identitas request untuk autentikasi, lokalisasi, audit, serta rate limiting.
/// </summary>
// Mendefinisikan tipe class `BearerTokenHandler` yang mewarisi atau menerapkan `DelegatingHandler`; sealed mencegah tipe ini diturunkan lagi.
public sealed class BearerTokenHandler : DelegatingHandler
// Membuka scope tipe BearerTokenHandler; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `IHttpContextAccessor`: `_httpContextAccessor` menyimpan akses ke konteks HTTP aktif, termasuk pengguna, request,
    // dan identitas penelusuran. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Menginisialisasi handler dengan accessor HTTP context untuk membaca sesi pengguna.
    /// </summary>
    /// <param name="httpContextAccessor">Accessor untuk mengambil HttpContext aktif.</param>
    // Mendefinisikan konstruktor BearerTokenHandler yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `httpContextAccessor` bertipe `IHttpContextAccessor` membawa akses ke konteks HTTP aktif, termasuk pengguna, request, dan identitas penelusuran.
    public BearerTokenHandler(IHttpContextAccessor httpContextAccessor)
    // Membuka scope konstruktor BearerTokenHandler; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BearerTokenHandler.
    {
        // Memperbarui `_httpContextAccessor` menggunakan `httpContextAccessor` (akses ke konteks HTTP aktif, termasuk pengguna, request, dan identitas
        // penelusuran) dalam BearerTokenHandler.
        _httpContextAccessor = httpContextAccessor;
    // Menutup scope konstruktor BearerTokenHandler; bagian berikut berada di luar batas blok tersebut dalam BearerTokenHandler.
    }

    /// <summary>
    /// Menyisipkan header antar-layanan dari konteks request UI tanpa menimpa nilai
    /// yang sengaja sudah diberikan oleh pemanggil.
    /// </summary>
    /// <param name="request">Pesan HTTP request yang akan dikirim.</param>
    /// <param name="cancellationToken">Token pembatalan untuk operasi asinkron.</param>
    /// <returns>Task yang menghasilkan pesan HTTP response dari handler berikutnya.</returns>
    // Mendefinisikan metode `SendAsync` dengan hasil bertipe `Task<HttpResponseMessage>`. Menyisipkan header antar-layanan dari konteks request UI
    // tanpa menimpa nilai yang sengaja sudah diberikan oleh pemanggil. Masukan: Parameter `request` bertipe `HttpRequestMessage` membawa data masukan
    // permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan
    // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    // Membuka scope metode SendAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SendAsync.
    {
        // Menyiapkan variabel lokal `httpContext` untuk konteks operasi yang menyediakan data lingkungan pemrosesan saat ini dengan
        // `_httpContextAccessor.HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini). Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var httpContext = _httpContextAccessor.HttpContext;
        // Memeriksa hasil pencocokan `httpContext` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam SendAsync.
        if (httpContext is null)
        // Membuka scope cabang if untuk kondisi `httpContext is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SendAsync.
        {
            // Mengembalikan memanggil `base.SendAsync` dengan `request`, `cancellationToken` kepada pemanggil dalam SendAsync; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return base.SendAsync(request, cancellationToken);
        // Menutup scope cabang if untuk kondisi `httpContext is null`; bagian berikut berada di luar batas blok tersebut dalam SendAsync.
        }

        // Memeriksa hasil pencocokan `request.Headers.Authorization` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // SendAsync.
        if (request.Headers.Authorization is null)
        // Membuka scope cabang if untuk kondisi `request.Headers.Authorization is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // SendAsync.
        {
            // Menyiapkan variabel lokal `token` untuk token yang diteruskan pada operasi terkait dengan
            // `httpContext.User.FindFirst(AuthConstants.AccessTokenClaim)?.Value`; akses setelah ?. hanya dilakukan bila penerimanya tidak null. Tipe variabel
            // disimpulkan dari ekspresi nilai awal.
            var token = httpContext.User.FindFirst(AuthConstants.AccessTokenClaim)?.Value;
            // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(token)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam SendAsync.
            if (!string.IsNullOrWhiteSpace(token))
            // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(token)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // SendAsync.
            {
                // Memperbarui `request.Headers.Authorization` menggunakan objek baru bertipe `AuthenticationHeaderValue` dengan argumen (”Bearer”, token) dalam
                // SendAsync.
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(token)`; bagian berikut berada di luar batas blok tersebut dalam SendAsync.
            }
        // Menutup scope cabang if untuk kondisi `request.Headers.Authorization is null`; bagian berikut berada di luar batas blok tersebut dalam SendAsync.
        }

        // Memeriksa perbandingan kesamaan antara `request.Headers.AcceptLanguage.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam SendAsync.
        if (request.Headers.AcceptLanguage.Count == 0)
        // Membuka scope cabang if untuk kondisi `request.Headers.AcceptLanguage.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam SendAsync.
        {
            // Menyiapkan variabel lokal `language` untuk nilai language dengan memanggil `UiText.NormalizeLanguage` dengan
            // `httpContext.Session.GetString(AuthConstants.SessionLanguageKey)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var language = UiText.NormalizeLanguage(httpContext.Session.GetString(AuthConstants.SessionLanguageKey));
            // Menjalankan memanggil `request.Headers.AcceptLanguage.ParseAdd` dengan `language` dalam SendAsync.
            request.Headers.AcceptLanguage.ParseAdd(language);
        // Menutup scope cabang if untuk kondisi `request.Headers.AcceptLanguage.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam
        // SendAsync.
        }

        // Memeriksa kebalikan kondisi `request.Headers.Contains(”X-Forwarded-For”)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // SendAsync.
        if (!request.Headers.Contains("X-Forwarded-For"))
        // Membuka scope cabang if untuk kondisi `!request.Headers.Contains(”X-Forwarded-For”)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam SendAsync.
        {
            // Menyiapkan variabel lokal `remoteIp` untuk nilai remote ip dengan `httpContext.Connection.RemoteIpAddress?.ToString()`; akses setelah ?. hanya
            // dilakukan bila penerimanya tidak null. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString();
            // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(remoteIp)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam SendAsync.
            if (!string.IsNullOrWhiteSpace(remoteIp))
            // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(remoteIp)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // SendAsync.
            {
                // Menjalankan memanggil `request.Headers.TryAddWithoutValidation` dengan `”X-Forwarded-For”`, `remoteIp` dalam SendAsync.
                request.Headers.TryAddWithoutValidation("X-Forwarded-For", remoteIp);
            // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(remoteIp)`; bagian berikut berada di luar batas blok tersebut dalam SendAsync.
            }
        // Menutup scope cabang if untuk kondisi `!request.Headers.Contains(”X-Forwarded-For”)`; bagian berikut berada di luar batas blok tersebut dalam
        // SendAsync.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!request.Headers.Contains(”X-Client-Request-Id”)` dan
        // `!string.IsNullOrWhiteSpace(httpContext.TraceIdentifier)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam SendAsync.
        if (!request.Headers.Contains("X-Client-Request-Id") && !string.IsNullOrWhiteSpace(httpContext.TraceIdentifier))
        // Membuka scope cabang if untuk kondisi `!request.Headers.Contains(”X-Client-Request-Id”) &&
        // !string.IsNullOrWhiteSpace(httpContext.TraceIdentifier)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SendAsync.
        {
            // Menjalankan memanggil `request.Headers.TryAddWithoutValidation` dengan `”X-Client-Request-Id”`, `httpContext.TraceIdentifier` dalam SendAsync.
            request.Headers.TryAddWithoutValidation("X-Client-Request-Id", httpContext.TraceIdentifier);
        // Menutup scope cabang if untuk kondisi `!request.Headers.Contains(”X-Client-Request-Id”) &&
        // !string.IsNullOrWhiteSpace(httpContext.TraceIdentifier)`; bagian berikut berada di luar batas blok tersebut dalam SendAsync.
        }

        // Mengembalikan memanggil `base.SendAsync` dengan `request`, `cancellationToken` kepada pemanggil dalam SendAsync; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return base.SendAsync(request, cancellationToken);
    // Menutup scope metode SendAsync; bagian berikut berada di luar batas blok tersebut dalam SendAsync.
    }
// Menutup scope tipe BearerTokenHandler; bagian berikut berada di luar batas blok tersebut.
}
