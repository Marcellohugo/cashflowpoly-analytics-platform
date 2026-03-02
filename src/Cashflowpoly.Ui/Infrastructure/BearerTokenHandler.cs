// Fungsi file: Menangani penyisipan otomatis token Bearer dari sesi pengguna ke setiap HTTP request menuju API backend.
using System.Net.Http.Headers;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Menyisipkan token Bearer dari session UI ke setiap request API.
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
    /// Menyisipkan header Authorization Bearer dari token sesi sebelum meneruskan request ke handler berikutnya.
    /// Jika token tidak tersedia, header Authorization dihapus.
    /// </summary>
    /// <param name="request">Pesan HTTP request yang akan dikirim.</param>
    /// <param name="cancellationToken">Token pembatalan untuk operasi asinkron.</param>
    /// <returns>Task yang menghasilkan pesan HTTP response dari handler berikutnya.</returns>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization is not null)
        {
            return base.SendAsync(request, cancellationToken);
        }

        var token = _httpContextAccessor.HttpContext?.Session.GetString(AuthConstants.SessionAccessTokenKey);
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
