// Fungsi file: Memverifikasi halaman legal serta metadata publik mengikuti konfigurasi DOMAIN.
using Cashflowpoly.Ui.Controllers;
using Cashflowpoly.Ui.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class LegalAndMetadataTests
{
    [Fact]
    public void SiteUrlResolver_ShouldNormalizeConfiguredDomain()
    {
        var configuration = BuildConfiguration("narafin.org");
        var context = new DefaultHttpContext();
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("localhost", 5203);

        var baseUrl = SiteUrlResolver.ResolveBaseUrl(configuration, context.Request);

        Assert.Equal("https://narafin.org", baseUrl);
        Assert.Equal("https://narafin.org/privacy", SiteUrlResolver.BuildAbsoluteUrl(baseUrl, "/privacy"));
    }

    [Fact]
    public void SiteUrlResolver_ShouldPreserveConfiguredLocalSchemeAndFallbackToRequest()
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("localhost", 5203);

        Assert.Equal(
            "http://localhost:5203",
            SiteUrlResolver.ResolveBaseUrl(BuildConfiguration("http://localhost:5203"), context.Request));
        Assert.Equal(
            "http://localhost:5203",
            SiteUrlResolver.ResolveBaseUrl(BuildConfiguration(null), context.Request));
    }

    [Fact]
    public void HomeController_ShouldExposeSeparateLegalPagesAndConfiguredCrawlerFiles()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var controller = new HomeController(new UnusedHttpClientFactory(), cache, BuildConfiguration("narafin.org"))
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var privacy = Assert.IsType<ViewResult>(controller.Privacy());
        Assert.Equal("PrivacyPolicy", privacy.ViewName);
        Assert.IsType<ViewResult>(controller.Terms());

        var robots = Assert.IsType<ContentResult>(controller.Robots());
        Assert.Equal("text/plain", robots.ContentType);
        Assert.Contains("Sitemap: https://narafin.org/sitemap.xml", robots.Content, StringComparison.Ordinal);
        Assert.Contains("Disallow: /auth/", robots.Content, StringComparison.Ordinal);

        var sitemap = Assert.IsType<ContentResult>(controller.Sitemap());
        Assert.Equal("application/xml", sitemap.ContentType);
        Assert.Contains("<loc>https://narafin.org/</loc>", sitemap.Content, StringComparison.Ordinal);
        Assert.Contains("<loc>https://narafin.org/rulebook</loc>", sitemap.Content, StringComparison.Ordinal);
        Assert.DoesNotContain("/login", sitemap.Content, StringComparison.Ordinal);
        Assert.DoesNotContain("/register", sitemap.Content, StringComparison.Ordinal);
        Assert.DoesNotContain("/privacy</loc>", sitemap.Content, StringComparison.Ordinal);
        Assert.DoesNotContain("/terms</loc>", sitemap.Content, StringComparison.Ordinal);
    }

    [Fact]
    public void LayoutAndRoutes_ShouldKeepLegalPagesPublicAndPrivatePagesNoIndex()
    {
        var repoRoot = ResolveRepositoryRoot();
        var layout = File.ReadAllText(Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Shared", "_Layout.cshtml"));
        var program = File.ReadAllText(Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Program.cs"));
        var webRoot = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "wwwroot");

        Assert.Contains("SiteUrlResolver.ResolveBaseUrl", layout, StringComparison.Ordinal);
        Assert.Contains("? \"noindex, follow\"", layout, StringComparison.Ordinal);
        Assert.Contains(": \"noindex, nofollow\"", layout, StringComparison.Ordinal);
        Assert.Contains("href=\"/privacy\"", layout, StringComparison.Ordinal);
        Assert.Contains("href=\"/terms\"", layout, StringComparison.Ordinal);
        Assert.DoesNotContain("https://narafin.org", layout, StringComparison.Ordinal);
        Assert.Contains("isLegalPath", program, StringComparison.Ordinal);
        Assert.Contains("pattern: \"privacy\"", program, StringComparison.Ordinal);
        Assert.Contains("pattern: \"terms\"", program, StringComparison.Ordinal);
        Assert.False(File.Exists(Path.Combine(webRoot, "robots.txt")));
        Assert.False(File.Exists(Path.Combine(webRoot, "sitemap.xml")));
    }

    private static IConfiguration BuildConfiguration(string? domain)
    {
        var values = new Dictionary<string, string?>();
        if (domain is not null)
        {
            values["DOMAIN"] = domain;
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }

    private static string ResolveRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Cashflowpoly.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("Repository root tidak ditemukan.");
    }

    private sealed class UnusedHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => throw new InvalidOperationException("Tidak digunakan oleh pengujian ini.");
    }
}
