using System.Net;
using System.Security.Claims;
using Cashflowpoly.Api.Security;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class RateLimitPolicyHelperTests
{
    [Theory]
    [InlineData("/api/v1/events", 120)]
    [InlineData("/api/v1/sessions", 60)]
    [InlineData("/api/events", 60)]
    [InlineData("/api/v1/auth/login", 60)]
    public void ResolvePermitLimit_ReturnsExpectedValue(string path, int expectedLimit)
    {
        var result = RateLimitPolicyHelper.ResolvePermitLimit(new PathString(path));
        Assert.Equal(expectedLimit, result);
    }

    [Fact]
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
    public void BuildPartitionKey_UsesRemoteIp_ForAnonymousRequest()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/v1/sessions";
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.10.10.1");

        var key = RateLimitPolicyHelper.BuildPartitionKey(context);

        Assert.Equal("default:ip:10.10.10.1", key);
    }

    [Fact]
    public void BuildPartitionKey_UsesUnknown_WhenRemoteIpMissing()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/v1/sessions";

        var key = RateLimitPolicyHelper.BuildPartitionKey(context);

        Assert.Equal("default:ip:unknown", key);
    }
}
