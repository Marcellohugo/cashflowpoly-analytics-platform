// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui HttpsRedirectionPolicy.
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Cashflowpoly.Ui.Infrastructure;

public static class HttpsRedirectionPolicy
{
    public static CookieSecurePolicy ResolveCookieSecurePolicy(IConfiguration configuration)
    {
        return ShouldUseHttpsRedirection(configuration)
            ? CookieSecurePolicy.Always
            : CookieSecurePolicy.SameAsRequest;
    }

    public static bool ShouldUseHttpsRedirection(IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("Security:RequireHttps"))
        {
            return true;
        }

        if (!string.IsNullOrWhiteSpace(configuration["ASPNETCORE_HTTPS_PORT"]) ||
            !string.IsNullOrWhiteSpace(configuration["HTTPS_PORT"]))
        {
            return true;
        }

        var urls = configuration["ASPNETCORE_URLS"] ?? configuration["URLS"];
        if (string.IsNullOrWhiteSpace(urls))
        {
            return false;
        }

        return urls
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(url => url.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
    }
}
