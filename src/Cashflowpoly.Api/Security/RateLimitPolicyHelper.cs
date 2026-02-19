using System.Security.Claims;

namespace Cashflowpoly.Api.Security;

internal static class RateLimitPolicyHelper
{
    private const int IngestPermitLimit = 120;
    private const int DefaultPermitLimit = 60;

    internal static int ResolvePermitLimit(PathString path)
    {
        return IsIngestPath(path) ? IngestPermitLimit : DefaultPermitLimit;
    }

    internal static string BuildPartitionKey(HttpContext context)
    {
        var scope = IsIngestPath(context.Request.Path) ? "ingest" : "default";
        var client = ResolveClientKey(context);
        return $"{scope}:{client}";
    }

    private static bool IsIngestPath(PathString path)
    {
        return path.StartsWithSegments("/api/v1/events", StringComparison.OrdinalIgnoreCase);
    }

    private static string ResolveClientKey(HttpContext context)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue("sub");

        if (!string.IsNullOrWhiteSpace(userId))
        {
            return $"user:{userId}";
        }

        var remoteIp = context.Connection.RemoteIpAddress?.ToString();
        return $"ip:{(string.IsNullOrWhiteSpace(remoteIp) ? "unknown" : remoteIp)}";
    }
}
