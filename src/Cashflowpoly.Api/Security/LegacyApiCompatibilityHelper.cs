// Fungsi file: Middleware helper untuk menulis ulang path API lama (/api/...) ke format berversi (/api/v1/...).
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Api.Security;

/// <summary>
/// Helper kompatibilitas mundur agar klien lama tanpa prefix /v1/ tetap dapat mengakses API.
/// </summary>
internal static class LegacyApiCompatibilityHelper
{
    /// <summary>
    /// Mendeteksi path /api/ tanpa /v1/ dan menulis ulang menjadi /api/v1/...; mengembalikan true jika berhasil.
    /// </summary>
    internal static bool TryRewritePath(PathString path, out PathString rewrittenPath)
    {
        rewrittenPath = path;

        var value = path.Value;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (value.Length <= "/api/".Length)
        {
            return false;
        }

        if (!value.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (value.StartsWith("/api/v1/", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        rewrittenPath = new PathString($"/api/v1{value["/api".Length..]}");
        return true;
    }
}
