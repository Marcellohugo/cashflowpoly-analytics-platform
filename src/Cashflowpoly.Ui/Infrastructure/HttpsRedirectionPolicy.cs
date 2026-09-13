// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui HttpsRedirectionPolicy.
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;
// Mengimpor namespace `Microsoft.Extensions.Configuration` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.Extensions.Configuration;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

public static class HttpsRedirectionPolicy
{
    public static CookieSecurePolicy ResolveCookieSecurePolicy(IConfiguration configuration)
    {
        return ShouldUseHttpsRedirection(configuration)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: CookieSecurePolicy.Always dalam ResolveCookieSecurePolicy.
            ? CookieSecurePolicy.Always
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: CookieSecurePolicy.SameAsRequest; dalam ResolveCookieSecurePolicy.
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
