using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Api.Security;

internal static class LegacyApiCompatibilityHelper
{
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
