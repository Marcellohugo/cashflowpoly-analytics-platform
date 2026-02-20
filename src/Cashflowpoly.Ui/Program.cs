// Fungsi file: Mengonfigurasi bootstrap service, session, middleware, dan routing untuk aplikasi UI MVC.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".Cashflowpoly.Ui.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(8);
});
builder.Services.AddTransient<Cashflowpoly.Ui.Infrastructure.BearerTokenHandler>();
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live", "ready"]);

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

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRouting();

app.UseStaticFiles();
app.UseSession();

app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    var isLoginPath = path.StartsWithSegments("/auth/login", StringComparison.OrdinalIgnoreCase);
    var isRegisterPath = path.StartsWithSegments("/auth/register", StringComparison.OrdinalIgnoreCase);
    var isLanguagePath = path.StartsWithSegments("/language", StringComparison.OrdinalIgnoreCase);
    var isHealthPath = path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase);
    var hasRole = !string.IsNullOrWhiteSpace(
        context.Session.GetString(Cashflowpoly.Ui.Models.AuthConstants.SessionRoleKey));
    var hasAccessToken = !string.IsNullOrWhiteSpace(
        context.Session.GetString(Cashflowpoly.Ui.Models.AuthConstants.SessionAccessTokenKey));
    var hasLanguage = !string.IsNullOrWhiteSpace(
        context.Session.GetString(Cashflowpoly.Ui.Models.AuthConstants.SessionLanguageKey));
    var tokenExpiresAtRaw = context.Session.GetString(Cashflowpoly.Ui.Models.AuthConstants.SessionTokenExpiresAtKey);

    if (!hasLanguage)
    {
        context.Session.SetString(
            Cashflowpoly.Ui.Models.AuthConstants.SessionLanguageKey,
            Cashflowpoly.Ui.Models.AuthConstants.LanguageId);
    }

    if (DateTimeOffset.TryParse(tokenExpiresAtRaw, out var tokenExpiresAt) &&
        tokenExpiresAt <= DateTimeOffset.UtcNow)
    {
        context.Session.Remove(Cashflowpoly.Ui.Models.AuthConstants.SessionRoleKey);
        context.Session.Remove(Cashflowpoly.Ui.Models.AuthConstants.SessionUsernameKey);
        context.Session.Remove(Cashflowpoly.Ui.Models.AuthConstants.SessionAccessTokenKey);
        context.Session.Remove(Cashflowpoly.Ui.Models.AuthConstants.SessionTokenExpiresAtKey);
        hasRole = false;
        hasAccessToken = false;
    }

    if (!isLoginPath && !isRegisterPath && !isLanguagePath && !isHealthPath && (!hasRole || !hasAccessToken))
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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
