// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui BearerTokenHandler.
using System.Net.Http.Headers;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Meneruskan konteks pengguna UI yang diperlukan API: token, bahasa, alamat klien,
/// dan identitas request untuk autentikasi, lokalisasi, audit, serta rate limiting.
/// </summary>
public sealed class BearerTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Menginisialisasi handler dengan accessor HTTP context untuk membaca sesi pengguna.
    /// </summary>
    /// <param name="httpContextAccessor">Accessor untuk mengambil HttpContext aktif.</param>
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
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return base.SendAsync(request, cancellationToken);
        }

        if (request.Headers.Authorization is null)
        {
            var token = httpContext.Session.GetString(AuthConstants.SessionAccessTokenKey);
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
