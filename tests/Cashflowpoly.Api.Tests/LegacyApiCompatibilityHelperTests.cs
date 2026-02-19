using Cashflowpoly.Api.Security;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class LegacyApiCompatibilityHelperTests
{
    [Theory]
    [InlineData("/api/sessions", true, "/api/v1/sessions")]
    [InlineData("/api/auth/login", true, "/api/v1/auth/login")]
    [InlineData("/api/v1/sessions", false, "/api/v1/sessions")]
    [InlineData("/api", false, "/api")]
    [InlineData("/api/", false, "/api/")]
    [InlineData("/swagger", false, "/swagger")]
    public void TryRewritePath_ReturnsExpectedResult(string rawPath, bool expectedRewrite, string expectedPath)
    {
        var rewritten = LegacyApiCompatibilityHelper.TryRewritePath(new PathString(rawPath), out var rewrittenPath);

        Assert.Equal(expectedRewrite, rewritten);
        Assert.Equal(expectedPath, rewrittenPath.Value);
    }
}
