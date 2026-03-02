// Fungsi file: Menguji bahwa LegacyApiCompatibilityHelper merewrite path API lama ke format /api/v1/ dengan benar.
using Cashflowpoly.Api.Security;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Cashflowpoly.Api.Tests;

/// <summary>
/// Kelas pengujian unit untuk memvalidasi bahwa LegacyApiCompatibilityHelper
/// mengidentifikasi dan menulis ulang path API legacy /api/* ke /api/v1/* dengan tepat.
/// </summary>
public sealed class LegacyApiCompatibilityHelperTests
{
    [Theory]
    [InlineData("/api/sessions", true, "/api/v1/sessions")]
    [InlineData("/api/auth/login", true, "/api/v1/auth/login")]
    [InlineData("/api/v1/sessions", false, "/api/v1/sessions")]
    [InlineData("/api", false, "/api")]
    [InlineData("/api/", false, "/api/")]
    [InlineData("/swagger", false, "/swagger")]
    /// <summary>
    /// Memvalidasi bahwa TryRewritePath mengembalikan hasil rewrite yang benar
    /// untuk berbagai kombinasi path legacy, versioned, dan non-API.
    /// </summary>
    public void TryRewritePath_ReturnsExpectedResult(string rawPath, bool expectedRewrite, string expectedPath)
    {
        var rewritten = LegacyApiCompatibilityHelper.TryRewritePath(new PathString(rawPath), out var rewrittenPath);

        Assert.Equal(expectedRewrite, rewritten);
        Assert.Equal(expectedPath, rewrittenPath.Value);
    }
}
