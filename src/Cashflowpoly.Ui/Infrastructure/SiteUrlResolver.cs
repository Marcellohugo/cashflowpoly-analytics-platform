// Fungsi file: Menyusun URL publik aplikasi dari konfigurasi DOMAIN atau request aktif.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

public static class SiteUrlResolver
{
    public static string ResolveBaseUrl(IConfiguration configuration, HttpRequest request)
    {
        var configuredDomain = configuration["DOMAIN"]?.Trim();
        if (!string.IsNullOrWhiteSpace(configuredDomain))
        {
            var absoluteDomain = configuredDomain.Contains("://", StringComparison.Ordinal)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: configuredDomain dalam ResolveBaseUrl.
                ? configuredDomain
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: $”https://{configuredDomain}”; dalam ResolveBaseUrl.
                : $"https://{configuredDomain}";

            if (Uri.TryCreate(absoluteDomain, UriKind.Absolute, out var configuredUri))
            {
                return configuredUri.GetLeftPart(UriPartial.Authority).TrimEnd('/');
            }

            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Konfigurasi DOMAIN harus berupa nama host atau
            // URL absolut yang valid.”) dalam ResolveBaseUrl; pemanggil atau middleware penanganan error menerima kegagalan ini.
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
