// Fungsi file: Menyusun URL publik aplikasi dari konfigurasi DOMAIN atau request aktif.
namespace Cashflowpoly.Ui.Infrastructure;

public static class SiteUrlResolver
{
    public static string ResolveBaseUrl(IConfiguration configuration, HttpRequest request)
    {
        var configuredDomain = configuration["DOMAIN"]?.Trim();
        if (!string.IsNullOrWhiteSpace(configuredDomain))
        {
            var absoluteDomain = configuredDomain.Contains("://", StringComparison.Ordinal)
                ? configuredDomain
                : $"https://{configuredDomain}";

            if (Uri.TryCreate(absoluteDomain, UriKind.Absolute, out var configuredUri))
            {
                return configuredUri.GetLeftPart(UriPartial.Authority).TrimEnd('/');
            }

            throw new InvalidOperationException("Konfigurasi DOMAIN harus berupa nama host atau URL absolut yang valid.");
        }

        return $"{request.Scheme}://{request.Host}".TrimEnd('/');
    }

    public static string BuildAbsoluteUrl(string baseUrl, PathString path)
    {
        var normalizedPath = path.HasValue ? path.Value! : "/";
        return $"{baseUrl.TrimEnd('/')}/{normalizedPath.TrimStart('/')}";
    }
}
