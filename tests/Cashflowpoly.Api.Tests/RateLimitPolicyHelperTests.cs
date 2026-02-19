// Fungsi file: Menguji perilaku dan kontrak komponen pada domain RateLimitPolicyHelperTests.
using System.Net;
using System.Security.Claims;
using Cashflowpoly.Api.Security;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Cashflowpoly.Api.Tests;

/// <summary>
/// Menyatakan peran utama tipe RateLimitPolicyHelperTests pada modul ini.
/// </summary>
public sealed class RateLimitPolicyHelperTests
{
    [Theory]
    [InlineData("/api/v1/events", 120)]
    [InlineData("/api/v1/sessions", 60)]
    [InlineData("/api/events", 60)]
    [InlineData("/api/v1/auth/login", 60)]
    /// <summary>
    /// Menjalankan fungsi ResolvePermitLimit_ReturnsExpectedValue sebagai bagian dari alur file ini.
    /// </summary>
    public void ResolvePermitLimit_ReturnsExpectedValue(string path, int expectedLimit)
    {
        var result = RateLimitPolicyHelper.ResolvePermitLimit(new PathString(path));
        Assert.Equal(expectedLimit, result);
    }

    [Fact]
    /// <summary>
    /// Menjalankan fungsi BuildPartitionKey_UsesUserId_WhenAuthenticated sebagai bagian dari alur file ini.
    /// </summary>
    public void BuildPartitionKey_UsesUserId_WhenAuthenticated()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/v1/events";
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, "user-123")],
            authenticationType: "test"));

        var key = RateLimitPolicyHelper.BuildPartitionKey(context);

        Assert.Equal("ingest:user:user-123", key);
    }

    [Fact]
    /// <summary>
    /// Menjalankan fungsi BuildPartitionKey_FallsBackToSubClaim_WhenNameIdentifierMissing sebagai bagian dari alur file ini.
    /// </summary>
    public void BuildPartitionKey_FallsBackToSubClaim_WhenNameIdentifierMissing()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/v1/sessions";
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim("sub", "subject-456")],
            authenticationType: "test"));

        var key = RateLimitPolicyHelper.BuildPartitionKey(context);

        Assert.Equal("default:user:subject-456", key);
    }

    [Fact]
    /// <summary>
    /// Menjalankan fungsi BuildPartitionKey_UsesRemoteIp_ForAnonymousRequest sebagai bagian dari alur file ini.
    /// </summary>
    public void BuildPartitionKey_UsesRemoteIp_ForAnonymousRequest()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/v1/sessions";
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.10.10.1");

        var key = RateLimitPolicyHelper.BuildPartitionKey(context);

        Assert.Equal("default:ip:10.10.10.1", key);
    }

    [Fact]
    /// <summary>
    /// Menjalankan fungsi BuildPartitionKey_UsesUnknown_WhenRemoteIpMissing sebagai bagian dari alur file ini.
    /// </summary>
    public void BuildPartitionKey_UsesUnknown_WhenRemoteIpMissing()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/v1/sessions";

        var key = RateLimitPolicyHelper.BuildPartitionKey(context);

        Assert.Equal("default:ip:unknown", key);
    }
}
