var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".Cashflowpoly.Ui.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(8);
});
builder.Services.AddTransient<Cashflowpoly.Ui.Infrastructure.RoleHeaderHandler>();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5041";

builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
    .AddHttpMessageHandler<Cashflowpoly.Ui.Infrastructure.RoleHeaderHandler>();

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
    var hasRole = !string.IsNullOrWhiteSpace(
        context.Session.GetString(Cashflowpoly.Ui.Models.AuthConstants.SessionRoleKey));
    var hasLanguage = !string.IsNullOrWhiteSpace(
        context.Session.GetString(Cashflowpoly.Ui.Models.AuthConstants.SessionLanguageKey));

    if (!hasLanguage)
    {
        context.Session.SetString(
            Cashflowpoly.Ui.Models.AuthConstants.SessionLanguageKey,
            Cashflowpoly.Ui.Models.AuthConstants.LanguageId);
    }

    if (!isLoginPath && !isRegisterPath && !isLanguagePath && !hasRole)
    {
        var returnUrl = $"{context.Request.Path}{context.Request.QueryString}";
        context.Response.Redirect($"/auth/login?returnUrl={Uri.EscapeDataString(returnUrl)}");
        return;
    }

    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
