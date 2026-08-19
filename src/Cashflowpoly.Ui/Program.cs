// Fungsi file: Mengonfigurasi dependency, middleware, route, dan startup UI MVC.
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
var useHttpsRedirection = Cashflowpoly.Ui.Infrastructure.HttpsRedirectionPolicy.ShouldUseHttpsRedirection(builder.Configuration);

builder.Services.Configure<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = 1;
    options.KnownProxies.Clear();
    options.KnownIPNetworks.Clear();

    foreach (var proxy in builder.Configuration.GetSection("Networking:TrustedProxies").Get<string[]>() ?? [])
    {
        if (IPAddress.TryParse(proxy, out var parsedProxy))
        {
            options.KnownProxies.Add(parsedProxy);
        }
    }

    foreach (var network in builder.Configuration.GetSection("Networking:TrustedNetworks").Get<string[]>() ?? [])
    {
        if (System.Net.IPNetwork.TryParse(network, out var parsedNetwork))
        {
            options.KnownIPNetworks.Add(parsedNetwork);
        }
    }
});

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = Cashflowpoly.Ui.Models.AuthConstants.AuthenticationCookieName;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = Cashflowpoly.Ui.Infrastructure.HttpsRedirectionPolicy.ResolveCookieSecurePolicy(builder.Configuration);
        options.LoginPath = "/auth/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = false;
    });
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".Cashflowpoly.Ui.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = Cashflowpoly.Ui.Infrastructure.HttpsRedirectionPolicy.ResolveCookieSecurePolicy(builder.Configuration);
    options.IdleTimeout = TimeSpan.FromHours(8);
});
builder.Services.AddTransient<Cashflowpoly.Ui.Infrastructure.BearerTokenHandler>();
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live", "ready"])
    .AddCheck<Cashflowpoly.Ui.Infrastructure.ApiHealthCheck>(
        "api",
        failureStatus: HealthStatus.Unhealthy,
        tags: ["ready"],
        timeout: TimeSpan.FromSeconds(5));

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    apiBaseUrl = builder.Environment.IsDevelopment()
        ? "http://localhost:5041"
        : "http://api:5041";
}

builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
    .AddHttpMessageHandler<Cashflowpoly.Ui.Infrastructure.BearerTokenHandler>();
builder.Services.AddHttpClient("ApiHealth", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

var app = builder.Build();

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
if (useHttpsRedirection)
{
    app.UseWhen(
        context => !context.Request.Path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase),
        branch => branch.UseHttpsRedirection());
}

app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();

app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    var isLoginPath = path.StartsWithSegments("/auth/login", StringComparison.OrdinalIgnoreCase);
    var isRegisterPath = path.StartsWithSegments("/auth/register", StringComparison.OrdinalIgnoreCase);
    var isLanguagePath = path.StartsWithSegments("/language", StringComparison.OrdinalIgnoreCase);
    var isHealthPath = path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase);
    var isRulebookPath = path.StartsWithSegments("/rulebook", StringComparison.OrdinalIgnoreCase);
    var isStaticAssetPath = path.StartsWithSegments("/css", StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments("/js", StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments("/images", StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments("/lib", StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments("/favicon.ico", StringComparison.OrdinalIgnoreCase)
        || path.Equals("/robots.txt", StringComparison.OrdinalIgnoreCase)
        || path.Equals("/sitemap.xml", StringComparison.OrdinalIgnoreCase);
    var isAuthenticated = context.User.Identity?.IsAuthenticated == true;
    var hasRole = !string.IsNullOrWhiteSpace(context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value);
    var hasAccessToken = !string.IsNullOrWhiteSpace(context.User.FindFirst(Cashflowpoly.Ui.Models.AuthConstants.AccessTokenClaim)?.Value);
    var hasLanguage = !string.IsNullOrWhiteSpace(
        context.Session.GetString(Cashflowpoly.Ui.Models.AuthConstants.SessionLanguageKey));

    if (!hasLanguage)
    {
        context.Session.SetString(
            Cashflowpoly.Ui.Models.AuthConstants.SessionLanguageKey,
            Cashflowpoly.Ui.Models.AuthConstants.LanguageId);
    }

    if (!isLoginPath &&
        !isRegisterPath &&
        !isLanguagePath &&
        !isHealthPath &&
        !isStaticAssetPath &&
        !isRulebookPath &&
        (!isAuthenticated || !hasRole || !hasAccessToken))
    {
        var returnUrl = $"{context.Request.Path}{context.Request.QueryString}";
        context.Response.Redirect($"/auth/login?returnUrl={Uri.EscapeDataString(returnUrl)}");
        return;
    }

    await next();
});

app.UseAuthorization();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapControllerRoute(
    name: "rulebook",
    pattern: "rulebook",
    defaults: new { controller = "Home", action = "Rulebook" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
