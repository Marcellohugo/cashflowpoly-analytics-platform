// Fungsi file: Menyediakan utilitas infrastruktur UI untuk kebutuhan BearerTokenHandler.
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
    /// Menjalankan fungsi BearerTokenHandler sebagai bagian dari alur file ini.
    /// </summary>
    public BearerTokenHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Menjalankan fungsi SendAsync sebagai bagian dari alur file ini.
    /// </summary>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString(AuthConstants.SessionAccessTokenKey);
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            request.Headers.Authorization = null;
        }

        return base.SendAsync(request, cancellationToken);
    }
}
