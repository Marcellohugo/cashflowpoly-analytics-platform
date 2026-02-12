using System.Security.Claims;
using System.Net;
using Cashflowpoly.Api.Security;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public class RateLimitPolicyHelperTests
{
    [Theory]
    [InlineData("/api/v1/events", 120)]
    [InlineData("/api/v1/events/batch", 120)]
    [InlineData("/api/events", 120)]
    [InlineData("/api/v1/sessions", 60)]
    [InlineData("/api/v1/analytics/sessions/abc", 60)]
    public void ResolvePermitLimit_returns_expected_limit(string path, int expectedLimit)
    {
        var actual = RateLimitPolicyHelper.ResolvePermitLimit(path);
        Assert.Equal(expectedLimit, actual);
    }

    [Fact]
    public void BuildPartitionKey_uses_user_claim_when_authenticated()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/v1/events";
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, "user-123") },
            "test-auth"));

        var partitionKey = RateLimitPolicyHelper.BuildPartitionKey(context);
        Assert.Equal("ingest:user:user-123", partitionKey);
    }

    [Fact]
    public void BuildPartitionKey_uses_remote_ip_when_user_not_authenticated()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/v1/sessions";
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.7");

        var partitionKey = RateLimitPolicyHelper.BuildPartitionKey(context);
        Assert.Equal("default:ip:10.0.0.7", partitionKey);
    }

    [Fact]
    public void BuildPartitionKey_ignores_spoofed_forwarded_header()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/v1/sessions";
        context.Request.Headers["X-Forwarded-For"] = "203.0.113.10";
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.7");

        var partitionKey = RateLimitPolicyHelper.BuildPartitionKey(context);
        Assert.Equal("default:ip:10.0.0.7", partitionKey);
    }
}
