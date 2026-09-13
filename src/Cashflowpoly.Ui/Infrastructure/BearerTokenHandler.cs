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
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Menginisialisasi handler dengan accessor HTTP context untuk membaca sesi pengguna.
    /// </summary>
    /// <param name="httpContextAccessor">Accessor untuk mengambil HttpContext aktif.</param>
    // Mendefinisikan konstruktor BearerTokenHandler yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `httpContextAccessor` bertipe `IHttpContextAccessor` membawa akses ke konteks HTTP aktif, termasuk pengguna, request, dan identitas penelusuran.
    public BearerTokenHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
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
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return base.SendAsync(request, cancellationToken);
        }

        if (request.Headers.Authorization is null)
        {
            var token = httpContext.User.FindFirst(AuthConstants.AccessTokenClaim)?.Value;
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        if (request.Headers.AcceptLanguage.Count == 0)
        {
            var language = UiText.NormalizeLanguage(httpContext.Session.GetString(AuthConstants.SessionLanguageKey));
            request.Headers.AcceptLanguage.ParseAdd(language);
        }

        if (!request.Headers.Contains("X-Forwarded-For"))
        {
            var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString();
            if (!string.IsNullOrWhiteSpace(remoteIp))
            {
                request.Headers.TryAddWithoutValidation("X-Forwarded-For", remoteIp);
            }
        }

        if (!request.Headers.Contains("X-Client-Request-Id") && !string.IsNullOrWhiteSpace(httpContext.TraceIdentifier))
        {
            request.Headers.TryAddWithoutValidation("X-Client-Request-Id", httpContext.TraceIdentifier);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
