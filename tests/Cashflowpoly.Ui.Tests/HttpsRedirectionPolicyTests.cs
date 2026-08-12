// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui HttpsRedirectionPolicyTests.
using Cashflowpoly.Ui.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class HttpsRedirectionPolicyTests
{
    [Fact]
    public void ShouldUseHttpsRedirection_ReturnsFalse_ForHttpOnlyUrls()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ASPNETCORE_URLS"] = "http://+:5203"
            })
            .Build();

        var result = HttpsRedirectionPolicy.ShouldUseHttpsRedirection(configuration);

        Assert.False(result);
    }

    [Fact]
    public void ShouldUseHttpsRedirection_ReturnsTrue_WhenHttpsPortConfigured()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ASPNETCORE_HTTPS_PORT"] = "443"
            })
            .Build();

        var result = HttpsRedirectionPolicy.ShouldUseHttpsRedirection(configuration);

        Assert.True(result);
    }

    [Fact]
    public void ShouldUseHttpsRedirection_ReturnsTrue_WhenUrlsIncludeHttps()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["URLS"] = "http://+:5203;https://+:5443"
            })
            .Build();

        var result = HttpsRedirectionPolicy.ShouldUseHttpsRedirection(configuration);

        Assert.True(result);
    }

    [Fact]
    public void ResolveCookieSecurePolicy_ReturnsSameAsRequest_ForProductionHttpOnlyLocalCompose()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ASPNETCORE_ENVIRONMENT"] = "Production",
                ["ASPNETCORE_URLS"] = "http://+:5203"
            })
            .Build();

        var result = HttpsRedirectionPolicy.ResolveCookieSecurePolicy(configuration);

        Assert.Equal(CookieSecurePolicy.SameAsRequest, result);
    }

    [Fact]
    public void ResolveCookieSecurePolicy_ReturnsAlways_WhenHttpsIsConfigured()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ASPNETCORE_ENVIRONMENT"] = "Production",
                ["ASPNETCORE_URLS"] = "http://+:5203;https://+:5443"
            })
            .Build();

        var result = HttpsRedirectionPolicy.ResolveCookieSecurePolicy(configuration);

        Assert.Equal(CookieSecurePolicy.Always, result);
    }

    [Fact]
    public void RequireHttpsConfiguration_EnablesRedirectionAndSecureCookiesBehindTlsProxy()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ASPNETCORE_URLS"] = "http://+:5203",
                ["Security:RequireHttps"] = "true"
            })
            .Build();

        Assert.True(HttpsRedirectionPolicy.ShouldUseHttpsRedirection(configuration));
        Assert.Equal(CookieSecurePolicy.Always, HttpsRedirectionPolicy.ResolveCookieSecurePolicy(configuration));
    }
}
