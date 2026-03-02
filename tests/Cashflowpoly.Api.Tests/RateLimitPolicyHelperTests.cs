// Fungsi file: Menguji bahwa RateLimitPolicyHelper menentukan batas request dan partition key berdasarkan path dan identitas pengguna.
using System.Net;
using System.Security.Claims;
using Cashflowpoly.Api.Security;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Cashflowpoly.Api.Tests;

/// <summary>
/// Kelas pengujian unit untuk memvalidasi bahwa RateLimitPolicyHelper
/// menghitung permit limit per endpoint dan membangun partition key yang tepat.
/// </summary>
public sealed class RateLimitPolicyHelperTests
{
    [Theory]
    [InlineData("/api/v1/events", 120)]
    [InlineData("/api/v1/sessions", 60)]
    [InlineData("/api/events", 60)]
    [InlineData("/api/v1/auth/login", 60)]
    /// <summary>
    /// Memvalidasi bahwa ResolvePermitLimit mengembalikan batas request yang sesuai
    /// untuk setiap path endpoint API (events, sessions, auth, dll).
    /// </summary>
    public void ResolvePermitLimit_ReturnsExpectedValue(string path, int expectedLimit)
    {
        var result = RateLimitPolicyHelper.ResolvePermitLimit(new PathString(path));
        Assert.Equal(expectedLimit, result);
    }

    [Fact]
    /// <summary>
    /// Memvalidasi bahwa BuildPartitionKey menggunakan user ID dari klaim
    /// NameIdentifier ketika pengguna sudah terotentikasi.
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
    /// Memvalidasi bahwa BuildPartitionKey menggunakan klaim "sub" sebagai fallback
    /// ketika klaim NameIdentifier tidak tersedia pada ClaimsPrincipal.
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
    /// Memvalidasi bahwa BuildPartitionKey menggunakan alamat IP remote
    /// sebagai partition key untuk request anonim tanpa autentikasi.
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
    /// Memvalidasi bahwa BuildPartitionKey mengembalikan "unknown" sebagai IP
    /// ketika RemoteIpAddress pada koneksi tidak tersedia (null).
    /// </summary>
    public void BuildPartitionKey_UsesUnknown_WhenRemoteIpMissing()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/v1/sessions";

        var key = RateLimitPolicyHelper.BuildPartitionKey(context);

        Assert.Equal("default:ip:unknown", key);
    }
}
