// Fungsi file: Mengonfigurasi dependency, middleware, route, dan startup UI MVC.
// Mengimpor namespace `Microsoft.AspNetCore.Authentication.Cookies` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama
// lengkapnya.
using Microsoft.AspNetCore.Authentication.Cookies;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;
// Mengimpor namespace `Microsoft.AspNetCore.Diagnostics.HealthChecks` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama
// lengkapnya.
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
// Mengimpor namespace `Microsoft.Extensions.Diagnostics.HealthChecks` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama
// lengkapnya.
using Microsoft.Extensions.Diagnostics.HealthChecks;
// Mengimpor namespace `System.Net` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net;

// Menyiapkan variabel lokal `builder` untuk nilai pembentuk dengan memanggil `WebApplication.CreateBuilder` dengan `args`. Tipe variabel
// disimpulkan dari ekspresi nilai awal.
var builder = WebApplication.CreateBuilder(args);
// Menyiapkan variabel lokal `useHttpsRedirection` untuk nilai use https redirection dengan memanggil
// `Cashflowpoly.Ui.Infrastructure.HttpsRedirectionPolicy.ShouldUseHttpsRedirection` dengan `builder.Configuration`. Tipe variabel disimpulkan dari
// ekspresi nilai awal.
var useHttpsRedirection = Cashflowpoly.Ui.Infrastructure.HttpsRedirectionPolicy.ShouldUseHttpsRedirection(builder.Configuration);

// Menjalankan mengatur komponen `builder.Services.Configure<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>` melalui `options => {
// options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
// Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto...`.
builder.Services.Configure<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>(options =>
// Membuka scope fungsi lambda yang dipasok ke `builder.Services.Configure<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>`;
// pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Memperbarui `options.ForwardedHeaders` menggunakan penggabungan flag atau operasi OR bit antara
    // `Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor` dan `Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto`.
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    // Memperbarui `options.ForwardLimit` menggunakan nilai literal `1`.
    options.ForwardLimit = 1;
    // Menjalankan mengosongkan seluruh elemen `options.KnownProxies`.
    options.KnownProxies.Clear();
    // Menjalankan mengosongkan seluruh elemen `options.KnownIPNetworks`.
    options.KnownIPNetworks.Clear();

    // Mengulangi setiap elemen `builder.Configuration.GetSection(”Networking:TrustedProxies”).Get<string[]>() ?? []`; elemen saat ini disimpan sebagai
    // `proxy` bertipe `var` untuk diproses oleh badan loop.
    foreach (var proxy in builder.Configuration.GetSection("Networking:TrustedProxies").Get<string[]>() ?? [])
    // Membuka scope loop setiap proxy dari `builder.Configuration.GetSection(”Networking:TrustedProxies”).Get<string[]>() ?? []`; pernyataan/deklarasi
    // berikut berada di dalam batas blok ini.
    {
        // Memeriksa mencoba mengonversi `proxy`, `var parsedProxy` melalui `IPAddress.TryParse`; keberhasilan dilaporkan sebagai boolean dan hasil
        // ditempatkan pada argumen out; blok if hanya dijalankan ketika kondisi ini bernilai benar.
        if (IPAddress.TryParse(proxy, out var parsedProxy))
        // Membuka scope cabang if untuk kondisi `IPAddress.TryParse(proxy, out var parsedProxy)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini.
        {
            // Menjalankan menambahkan `parsedProxy` ke `options.KnownProxies`.
            options.KnownProxies.Add(parsedProxy);
        // Menutup scope cabang if untuk kondisi `IPAddress.TryParse(proxy, out var parsedProxy)`; bagian berikut berada di luar batas blok tersebut.
        }
    // Menutup scope loop setiap proxy dari `builder.Configuration.GetSection(”Networking:TrustedProxies”).Get<string[]>() ?? []`; bagian berikut berada
    // di luar batas blok tersebut.
    }

    // Mengulangi setiap elemen `builder.Configuration.GetSection(”Networking:TrustedNetworks”).Get<string[]>() ?? []`; elemen saat ini disimpan sebagai
    // `network` bertipe `var` untuk diproses oleh badan loop.
    foreach (var network in builder.Configuration.GetSection("Networking:TrustedNetworks").Get<string[]>() ?? [])
    // Membuka scope loop setiap network dari `builder.Configuration.GetSection(”Networking:TrustedNetworks”).Get<string[]>() ?? []`;
    // pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Memeriksa mencoba mengonversi `network`, `var parsedNetwork` melalui `System.Net.IPNetwork.TryParse`; keberhasilan dilaporkan sebagai boolean dan
        // hasil ditempatkan pada argumen out; blok if hanya dijalankan ketika kondisi ini bernilai benar.
        if (System.Net.IPNetwork.TryParse(network, out var parsedNetwork))
        // Membuka scope cabang if untuk kondisi `System.Net.IPNetwork.TryParse(network, out var parsedNetwork)`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini.
        {
            // Menjalankan menambahkan `parsedNetwork` ke `options.KnownIPNetworks`.
            options.KnownIPNetworks.Add(parsedNetwork);
        // Menutup scope cabang if untuk kondisi `System.Net.IPNetwork.TryParse(network, out var parsedNetwork)`; bagian berikut berada di luar batas blok
        // tersebut.
        }
    // Menutup scope loop setiap network dari `builder.Configuration.GetSection(”Networking:TrustedNetworks”).Get<string[]>() ?? []`; bagian berikut
    // berada di luar batas blok tersebut.
    }
// Menutup scope fungsi lambda yang dipasok ke `builder.Services.Configure<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>`; bagian berikut
// berada di luar batas blok tersebut.
});

// Menjalankan memanggil `builder.Services.AddControllersWithViews` dengan `options => { options.Filters.Add(new
// AutoValidateAntiforgeryTokenAttribute()); }`.
builder.Services.AddControllersWithViews(options =>
// Membuka scope fungsi lambda yang dipasok ke `builder.Services.AddControllersWithViews`; pernyataan/deklarasi berikut berada di dalam batas blok
// ini.
{
    // Menjalankan menambahkan `new AutoValidateAntiforgeryTokenAttribute()` ke `options.Filters`.
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
// Menutup scope fungsi lambda yang dipasok ke `builder.Services.AddControllersWithViews`; bagian berikut berada di luar batas blok tersebut.
});
// Menjalankan memanggil `builder.Services.AddHttpContextAccessor` dengan tanpa argumen.
builder.Services.AddHttpContextAccessor();
// Menjalankan memanggil `builder.Services.AddMemoryCache` dengan tanpa argumen.
builder.Services.AddMemoryCache();
// Menjalankan memanggil `builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) .AddCookie` dengan `options => {
// options.Cookie.Name = Cashflowpoly.Ui.Models.AuthConstants.AuthenticationCookieName; options.Cookie.HttpOnly = true; options.Cookie.IsEssential =
// true; options.Co...`.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .AddCookie(options =>; token pada baris ini menyambungkan bagian kode sebelum
    // dan sesudahnya.
    .AddCookie(options =>
    // Membuka scope fungsi lambda yang dipasok ke `builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) .AddCookie`;
    // pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Memperbarui `options.Cookie.Name` menggunakan `Cashflowpoly.Ui.Models.AuthConstants.AuthenticationCookieName` (nilai authentication cookie nama).
        options.Cookie.Name = Cashflowpoly.Ui.Models.AuthConstants.AuthenticationCookieName;
        // Memperbarui `options.Cookie.HttpOnly` menggunakan true, yaitu kondisi aktif/terpenuhi.
        options.Cookie.HttpOnly = true;
        // Memperbarui `options.Cookie.IsEssential` menggunakan true, yaitu kondisi aktif/terpenuhi.
        options.Cookie.IsEssential = true;
        // Memperbarui `options.Cookie.SameSite` menggunakan `SameSiteMode.Lax` (nilai lax).
        options.Cookie.SameSite = SameSiteMode.Lax;
        // Memperbarui `options.Cookie.SecurePolicy` menggunakan memanggil `Cashflowpoly.Ui.Infrastructure.HttpsRedirectionPolicy.ResolveCookieSecurePolicy`
        // dengan `builder.Configuration`.
        options.Cookie.SecurePolicy = Cashflowpoly.Ui.Infrastructure.HttpsRedirectionPolicy.ResolveCookieSecurePolicy(builder.Configuration);
        // Memperbarui `options.LoginPath` menggunakan nilai literal `”/auth/login”`.
        options.LoginPath = "/auth/login";
        // Memperbarui `options.ExpireTimeSpan` menggunakan memanggil `TimeSpan.FromHours` dengan `8`.
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        // Memperbarui `options.SlidingExpiration` menggunakan false, yaitu kondisi nonaktif/tidak terpenuhi.
        options.SlidingExpiration = false;
    // Menutup scope fungsi lambda yang dipasok ke `builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) .AddCookie`;
    // bagian berikut berada di luar batas blok tersebut.
    });
// Menjalankan memanggil `builder.Services.AddSession` dengan `options => { options.Cookie.Name = ”.Cashflowpoly.Ui.Session”;
// options.Cookie.HttpOnly = true; options.Cookie.IsEssential = true; options.Cookie.SameSite = SameSiteMode.Lax; o...`.
builder.Services.AddSession(options =>
// Membuka scope fungsi lambda yang dipasok ke `builder.Services.AddSession`; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Memperbarui `options.Cookie.Name` menggunakan nilai literal `”.Cashflowpoly.Ui.Session”`.
    options.Cookie.Name = ".Cashflowpoly.Ui.Session";
    // Memperbarui `options.Cookie.HttpOnly` menggunakan true, yaitu kondisi aktif/terpenuhi.
    options.Cookie.HttpOnly = true;
    // Memperbarui `options.Cookie.IsEssential` menggunakan true, yaitu kondisi aktif/terpenuhi.
    options.Cookie.IsEssential = true;
    // Memperbarui `options.Cookie.SameSite` menggunakan `SameSiteMode.Lax` (nilai lax).
    options.Cookie.SameSite = SameSiteMode.Lax;
    // Memperbarui `options.Cookie.SecurePolicy` menggunakan memanggil `Cashflowpoly.Ui.Infrastructure.HttpsRedirectionPolicy.ResolveCookieSecurePolicy`
    // dengan `builder.Configuration`.
    options.Cookie.SecurePolicy = Cashflowpoly.Ui.Infrastructure.HttpsRedirectionPolicy.ResolveCookieSecurePolicy(builder.Configuration);
    // Memperbarui `options.IdleTimeout` menggunakan memanggil `TimeSpan.FromHours` dengan `8`.
    options.IdleTimeout = TimeSpan.FromHours(8);
// Menutup scope fungsi lambda yang dipasok ke `builder.Services.AddSession`; bagian berikut berada di luar batas blok tersebut.
});
// Menjalankan mendaftarkan layanan `builder.Services.AddTransient<Cashflowpoly.Ui.Infrastructure.BearerTokenHandler>` agar instance baru dibuat
// pada setiap resolusi.
builder.Services.AddTransient<Cashflowpoly.Ui.Infrastructure.BearerTokenHandler>();
// Menjalankan memanggil `builder.Services.AddHealthChecks() .AddCheck(”self”, () => HealthCheckResult.Healthy(), tags: [”live”, ”ready”])
// .AddCheck<Cashflowpoly.Ui.Infrastructure.ApiHealthCheck>` dengan `”api”`, `HealthStatus.Unhealthy`, `[”ready”]`, `TimeSpan.FromSeconds(5)`.
builder.Services.AddHealthChecks()
    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .AddCheck(”self”, () => HealthCheckResult.Healthy(), tags: [”live”, ”ready”]);
    // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live", "ready"])
    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .AddCheck<Cashflowpoly.Ui.Infrastructure.ApiHealthCheck>(; token pada baris ini
    // menyambungkan bagian kode sebelum dan sesudahnya.
    .AddCheck<Cashflowpoly.Ui.Infrastructure.ApiHealthCheck>(
        // Meneruskan nilai literal `”api”` sebagai argumen ke `builder.Services.AddHealthChecks() .AddCheck(”self”, () => HealthCheckResult.Healthy(),
        // tags: [”live”, ”ready”]) .AddCheck<Cashflowpoly.Ui.Infrastructure.ApiHealthCheck>`.
        "api",
        // Meneruskan `HealthStatus.Unhealthy` (nilai unhealthy) sebagai argumen bernama `failureStatus`.
        failureStatus: HealthStatus.Unhealthy,
        // Meneruskan koleksi berisi ”ready” sebagai argumen bernama `tags`.
        tags: ["ready"],
        // Meneruskan memanggil `TimeSpan.FromSeconds` dengan `5` sebagai argumen bernama `timeout`; Meneruskan nilai literal `5` sebagai argumen ke
        // `TimeSpan.FromSeconds`.
        timeout: TimeSpan.FromSeconds(5));

// Menyiapkan variabel lokal `apiBaseUrl` untuk nilai api base url dengan `builder.Configuration[”ApiBaseUrl”]`, yaitu elemen koleksi yang dipilih
// melalui indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
// Memeriksa memeriksa apakah `apiBaseUrl` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai
// benar.
if (string.IsNullOrWhiteSpace(apiBaseUrl))
// Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(apiBaseUrl)`; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Memperbarui `apiBaseUrl` menggunakan hasil pemilihan bersyarat: ketika `builder.Environment.IsDevelopment()` benar gunakan
    // `”http://localhost:5041”`, jika tidak gunakan `”http://api:5041”`.
    apiBaseUrl = builder.Environment.IsDevelopment()
        // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”http://localhost:5041”.
        ? "http://localhost:5041"
        // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: ”http://api:5041”;.
        : "http://api:5041";
// Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(apiBaseUrl)`; bagian berikut berada di luar batas blok tersebut.
}

// Menjalankan memanggil `builder.Services.AddHttpClient(”Api”, client => { client.BaseAddress = new Uri(apiBaseUrl); })
// .AddHttpMessageHandler<Cashflowpoly.Ui.Infrastructure.BearerTokenHandler>` dengan tanpa argumen.
builder.Services.AddHttpClient("Api", client =>
// Membuka scope fungsi lambda yang dipasok ke `builder.Services.AddHttpClient`; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Memperbarui `client.BaseAddress` menggunakan objek baru bertipe `Uri` dengan argumen (apiBaseUrl).
    client.BaseAddress = new Uri(apiBaseUrl);
// Menutup scope fungsi lambda yang dipasok ke `builder.Services.AddHttpClient`; bagian berikut berada di luar batas blok tersebut.
})
    // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .AddHttpMessageHandler<Cashflowpoly.Ui.Infrastructure.BearerTokenHandler>();;
    // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
    .AddHttpMessageHandler<Cashflowpoly.Ui.Infrastructure.BearerTokenHandler>();
// Menjalankan memanggil `builder.Services.AddHttpClient` dengan `”ApiHealth”`, `client => { client.BaseAddress = new Uri(apiBaseUrl); }`.
builder.Services.AddHttpClient("ApiHealth", client =>
// Membuka scope fungsi lambda yang dipasok ke `builder.Services.AddHttpClient`; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Memperbarui `client.BaseAddress` menggunakan objek baru bertipe `Uri` dengan argumen (apiBaseUrl).
    client.BaseAddress = new Uri(apiBaseUrl);
// Menutup scope fungsi lambda yang dipasok ke `builder.Services.AddHttpClient`; bagian berikut berada di luar batas blok tersebut.
});

// Menyiapkan variabel lokal `app` untuk nilai app dengan memanggil `builder.Build` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi
// nilai awal.
var app = builder.Build();

// Menjalankan menerapkan header proxy tepercaya untuk alamat klien dan skema permintaan.
app.UseForwardedHeaders();

// Memeriksa kebalikan kondisi `app.Environment.IsDevelopment()`; blok if hanya dijalankan ketika kondisi ini bernilai benar.
if (!app.Environment.IsDevelopment())
// Membuka scope cabang if untuk kondisi `!app.Environment.IsDevelopment()`; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Menjalankan memanggil `app.UseExceptionHandler` dengan `”/Home/Error”`.
    app.UseExceptionHandler("/Home/Error");
    // Menjalankan memanggil `app.UseHsts` dengan tanpa argumen.
    app.UseHsts();
// Menutup scope cabang if untuk kondisi `!app.Environment.IsDevelopment()`; bagian berikut berada di luar batas blok tersebut.
}
// Memeriksa `useHttpsRedirection` (nilai use https redirection); blok if hanya dijalankan ketika kondisi ini bernilai benar.
if (useHttpsRedirection)
// Membuka scope cabang if untuk kondisi `useHttpsRedirection`; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Menjalankan memanggil `app.UseWhen` dengan `context => !context.Request.Path.StartsWithSegments(”/health”, StringComparison.OrdinalIgnoreCase)`,
    // `branch => branch.UseHttpsRedirection()`.
    app.UseWhen(
        // Parameter `context` bertipe `` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini.
        context => !context.Request.Path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase),
        // Parameter `branch` bertipe `` membawa nilai branch.
        branch => branch.UseHttpsRedirection());
// Menutup scope cabang if untuk kondisi `useHttpsRedirection`; bagian berikut berada di luar batas blok tersebut.
}

// Menjalankan mengaktifkan penyajian berkas statis yang tersedia pada web root aplikasi.
app.UseStaticFiles();
// Menjalankan memasang pencocokan rute HTTP sebelum kebijakan dan endpoint dijalankan.
app.UseRouting();

// Menjalankan memanggil `app.UseSession` dengan tanpa argumen.
app.UseSession();
// Menjalankan memasang middleware autentikasi agar token/identitas pengguna diperiksa sebelum endpoint diproses.
app.UseAuthentication();

// Menjalankan memanggil `app.Use` dengan `async (context, next) => { var path = context.Request.Path; var isLoginPath =
// path.StartsWithSegments(”/auth/login”, StringComparison.OrdinalIgnoreCase); var isRegisterPath = ...`.
app.Use(async (context, next) =>
// Membuka scope fungsi lambda yang dipasok ke `app.Use`; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Menyiapkan variabel lokal `path` untuk nilai path dengan `context.Request.Path` (nilai path). Tipe variabel disimpulkan dari ekspresi nilai awal.
    var path = context.Request.Path;
    // Menyiapkan variabel lokal `isLoginPath` untuk nilai berstatus login path dengan memanggil `path.StartsWithSegments` dengan `”/auth/login”`,
    // `StringComparison.OrdinalIgnoreCase`. Tipe variabel disimpulkan dari ekspresi nilai awal.
    var isLoginPath = path.StartsWithSegments("/auth/login", StringComparison.OrdinalIgnoreCase);
    // Menyiapkan variabel lokal `isRegisterPath` untuk nilai berstatus register path dengan memanggil `path.StartsWithSegments` dengan
    // `”/auth/register”`, `StringComparison.OrdinalIgnoreCase`. Tipe variabel disimpulkan dari ekspresi nilai awal.
    var isRegisterPath = path.StartsWithSegments("/auth/register", StringComparison.OrdinalIgnoreCase);
    // Menyiapkan variabel lokal `isLanguagePath` untuk nilai berstatus language path dengan memanggil `path.StartsWithSegments` dengan `”/language”`,
    // `StringComparison.OrdinalIgnoreCase`. Tipe variabel disimpulkan dari ekspresi nilai awal.
    var isLanguagePath = path.StartsWithSegments("/language", StringComparison.OrdinalIgnoreCase);
    // Menyiapkan variabel lokal `isHealthPath` untuk nilai berstatus health path dengan memanggil `path.StartsWithSegments` dengan `”/health”`,
    // `StringComparison.OrdinalIgnoreCase`. Tipe variabel disimpulkan dari ekspresi nilai awal.
    var isHealthPath = path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase);
    // Menyiapkan variabel lokal `isRulebookPath` untuk nilai berstatus rulebook path dengan memanggil `path.StartsWithSegments` dengan `”/rulebook”`,
    // `StringComparison.OrdinalIgnoreCase`. Tipe variabel disimpulkan dari ekspresi nilai awal.
    var isRulebookPath = path.StartsWithSegments("/rulebook", StringComparison.OrdinalIgnoreCase);
    // Menyiapkan variabel lokal `isLegalPath` untuk nilai berstatus legal path dengan gabungan syarat OR: setidaknya satu kondisi wajib benar antara
    // `path.StartsWithSegments(”/privacy”, StringComparison.OrdinalIgnoreCase)` dan `path.StartsWithSegments(”/terms”,
    // StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri salah. Tipe variabel disimpulkan dari ekspresi nilai awal.
    var isLegalPath = path.StartsWithSegments("/privacy", StringComparison.OrdinalIgnoreCase)
        // Meneruskan nilai literal `”/terms”` sebagai argumen ke `path.StartsWithSegments`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal
        // ignore case) sebagai argumen ke `path.StartsWithSegments`.
        || path.StartsWithSegments("/terms", StringComparison.OrdinalIgnoreCase);
    // Menyiapkan variabel lokal `isStaticAssetPath` untuk nilai berstatus static aset path dengan gabungan syarat OR: setidaknya satu kondisi wajib
    // benar antara `path.StartsWithSegments(”/css”, StringComparison.OrdinalIgnoreCase) || path.StartsWithSegments(”/js”,
    // StringComparison.OrdinalIgnoreCase) || path.StartsWithSegments(”/images”,...` dan `path.Equals(”/sitemap.xml”,
    // StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri salah. Tipe variabel disimpulkan dari ekspresi nilai awal.
    var isStaticAssetPath = path.StartsWithSegments("/css", StringComparison.OrdinalIgnoreCase)
        // Meneruskan nilai literal `”/js”` sebagai argumen ke `path.StartsWithSegments`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal
        // ignore case) sebagai argumen ke `path.StartsWithSegments`.
        || path.StartsWithSegments("/js", StringComparison.OrdinalIgnoreCase)
        // Meneruskan nilai literal `”/images”` sebagai argumen ke `path.StartsWithSegments`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal
        // ignore case) sebagai argumen ke `path.StartsWithSegments`.
        || path.StartsWithSegments("/images", StringComparison.OrdinalIgnoreCase)
        // Meneruskan nilai literal `”/lib”` sebagai argumen ke `path.StartsWithSegments`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal
        // ignore case) sebagai argumen ke `path.StartsWithSegments`.
        || path.StartsWithSegments("/lib", StringComparison.OrdinalIgnoreCase)
        // Meneruskan nilai literal `”/swagger”` sebagai argumen ke `path.StartsWithSegments`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai
        // ordinal ignore case) sebagai argumen ke `path.StartsWithSegments`.
        || path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase)
        // Meneruskan nilai literal `”/favicon.ico”` sebagai argumen ke `path.StartsWithSegments`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai
        // ordinal ignore case) sebagai argumen ke `path.StartsWithSegments`.
        || path.StartsWithSegments("/favicon.ico", StringComparison.OrdinalIgnoreCase)
        // Meneruskan nilai literal `”/robots.txt”` sebagai argumen ke `path.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore
        // case) sebagai argumen ke `path.Equals`.
        || path.Equals("/robots.txt", StringComparison.OrdinalIgnoreCase)
        // Meneruskan nilai literal `”/sitemap.xml”` sebagai argumen ke `path.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore
        // case) sebagai argumen ke `path.Equals`.
        || path.Equals("/sitemap.xml", StringComparison.OrdinalIgnoreCase);
    // Menyiapkan variabel lokal `isAuthenticated` untuk nilai berstatus authenticated dengan perbandingan kesamaan antara
    // `context.User.Identity?.IsAuthenticated` dan `true`. Tipe variabel disimpulkan dari ekspresi nilai awal.
    var isAuthenticated = context.User.Identity?.IsAuthenticated == true;
    // Menyiapkan variabel lokal `hasRole` untuk nilai memiliki role dengan kebalikan kondisi
    // `string.IsNullOrWhiteSpace(context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value)`. Tipe variabel disimpulkan dari ekspresi nilai
    // awal.
    var hasRole = !string.IsNullOrWhiteSpace(context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value);
    // Menyiapkan variabel lokal `hasAccessToken` untuk nilai memiliki akses token dengan kebalikan kondisi
    // `string.IsNullOrWhiteSpace(context.User.FindFirst(Cashflowpoly.Ui.Models.AuthConstants.AccessTokenClaim)?.Value)`. Tipe variabel disimpulkan dari
    // ekspresi nilai awal.
    var hasAccessToken = !string.IsNullOrWhiteSpace(context.User.FindFirst(Cashflowpoly.Ui.Models.AuthConstants.AccessTokenClaim)?.Value);
    // Menyiapkan variabel lokal `hasLanguage` untuk nilai memiliki language dengan kebalikan kondisi `string.IsNullOrWhiteSpace(
    // context.Session.GetString(Cashflowpoly.Ui.Models.AuthConstants.SessionLanguageKey))`. Tipe variabel disimpulkan dari ekspresi nilai awal.
    var hasLanguage = !string.IsNullOrWhiteSpace(
        // Meneruskan membaca nilai string dari `context.Session` sesuai tipe JSON atau sumber data yang digunakan sebagai argumen ke
        // `string.IsNullOrWhiteSpace`; Meneruskan `Cashflowpoly.Ui.Models.AuthConstants.SessionLanguageKey` (nilai sesi language kunci) sebagai argumen ke
        // `context.Session.GetString`.
        context.Session.GetString(Cashflowpoly.Ui.Models.AuthConstants.SessionLanguageKey));

    // Memeriksa kebalikan kondisi `hasLanguage`; blok if hanya dijalankan ketika kondisi ini bernilai benar.
    if (!hasLanguage)
    // Membuka scope cabang if untuk kondisi `!hasLanguage`; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Menjalankan memanggil `context.Session.SetString` dengan `Cashflowpoly.Ui.Models.AuthConstants.SessionLanguageKey`,
        // `Cashflowpoly.Ui.Models.AuthConstants.LanguageId`.
        context.Session.SetString(
            // Meneruskan `Cashflowpoly.Ui.Models.AuthConstants.SessionLanguageKey` (nilai sesi language kunci) sebagai argumen ke `context.Session.SetString`.
            Cashflowpoly.Ui.Models.AuthConstants.SessionLanguageKey,
            // Meneruskan `Cashflowpoly.Ui.Models.AuthConstants.LanguageId` (nilai language identitas) sebagai argumen ke `context.Session.SetString`.
            Cashflowpoly.Ui.Models.AuthConstants.LanguageId);
    // Menutup scope cabang if untuk kondisi `!hasLanguage`; bagian berikut berada di luar batas blok tersebut.
    }

    // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!isLoginPath && !isRegisterPath && !isLanguagePath && !isHealthPath &&
    // !isStaticAssetPath && !isRulebookPath && !isLegalPath` dan `(!isAuthenticated || !hasRole || !hasAccessToken)`; sisi kanan diperiksa hanya jika
    // sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar.
    if (!isLoginPath &&
        // Meneruskan fungsi lambda `async (context, next) => { var path = context.Request.Path; var isLoginPath = path.StartsWithSegments(”/auth/login”,
        // StringComparison.OrdinalIgnoreCase); var isRegisterPath = ...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
        // argumen ke `app.Use`.
        !isRegisterPath &&
        // Meneruskan fungsi lambda `async (context, next) => { var path = context.Request.Path; var isLoginPath = path.StartsWithSegments(”/auth/login”,
        // StringComparison.OrdinalIgnoreCase); var isRegisterPath = ...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
        // argumen ke `app.Use`.
        !isLanguagePath &&
        // Meneruskan fungsi lambda `async (context, next) => { var path = context.Request.Path; var isLoginPath = path.StartsWithSegments(”/auth/login”,
        // StringComparison.OrdinalIgnoreCase); var isRegisterPath = ...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
        // argumen ke `app.Use`.
        !isHealthPath &&
        // Meneruskan fungsi lambda `async (context, next) => { var path = context.Request.Path; var isLoginPath = path.StartsWithSegments(”/auth/login”,
        // StringComparison.OrdinalIgnoreCase); var isRegisterPath = ...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
        // argumen ke `app.Use`.
        !isStaticAssetPath &&
        // Meneruskan fungsi lambda `async (context, next) => { var path = context.Request.Path; var isLoginPath = path.StartsWithSegments(”/auth/login”,
        // StringComparison.OrdinalIgnoreCase); var isRegisterPath = ...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
        // argumen ke `app.Use`.
        !isRulebookPath &&
        // Meneruskan fungsi lambda `async (context, next) => { var path = context.Request.Path; var isLoginPath = path.StartsWithSegments(”/auth/login”,
        // StringComparison.OrdinalIgnoreCase); var isRegisterPath = ...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
        // argumen ke `app.Use`.
        !isLegalPath &&
        // Meneruskan fungsi lambda `async (context, next) => { var path = context.Request.Path; var isLoginPath = path.StartsWithSegments(”/auth/login”,
        // StringComparison.OrdinalIgnoreCase); var isRegisterPath = ...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
        // argumen ke `app.Use`.
        (!isAuthenticated || !hasRole || !hasAccessToken))
    // Membuka scope cabang if untuk kondisi `!isLoginPath && !isRegisterPath && !isLanguagePath && !isHealthPath && !isStaticAssetPath &&
    // !isRulebookPath && !isLegalPath && (!isAuthenticated || !hasRole || !hasAccessToke...`; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini.
    {
        // Menyiapkan variabel lokal `returnUrl` untuk nilai return url dengan teks interpolasi `$”{context.Request.Path}{context.Request.QueryString}”`;
        // nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var returnUrl = $"{context.Request.Path}{context.Request.QueryString}";
        // Menjalankan memanggil `context.Response.Redirect` dengan `$”/auth/login?returnUrl={Uri.EscapeDataString(returnUrl)}”`.
        context.Response.Redirect($"/auth/login?returnUrl={Uri.EscapeDataString(returnUrl)}");
        // Mengakhiri eksekusi lebih awal tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
        return;
    // Menutup scope cabang if untuk kondisi `!isLoginPath && !isRegisterPath && !isLanguagePath && !isHealthPath && !isStaticAssetPath &&
    // !isRulebookPath && !isLegalPath && (!isAuthenticated || !hasRole || !hasAccessToke...`; bagian berikut berada di luar batas blok tersebut.
    }

    // Menjalankan hasil operasi asinkron memanggil `next` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama operasi belum
    // selesai.
    await next();
// Menutup scope fungsi lambda yang dipasok ke `app.Use`; bagian berikut berada di luar batas blok tersebut.
});

// Menjalankan memasang middleware otorisasi agar kebijakan peran dan akses endpoint diterapkan.
app.UseAuthorization();

// Menjalankan memanggil `app.MapHealthChecks` dengan `”/health/live”`, `new HealthCheckOptions { Predicate = check => check.Tags.Contains(”live”)
// }`.
app.MapHealthChecks("/health/live", new HealthCheckOptions
// Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Memperbarui `Predicate` menggunakan fungsi lambda `check => check.Tags.Contains(”live”)` yang dijalankan oleh operasi pemanggil untuk memproses
    // setiap masukan.
    Predicate = check => check.Tags.Contains("live")
// Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut.
});
// Menjalankan memanggil `app.MapHealthChecks` dengan `”/health/ready”`, `new HealthCheckOptions { Predicate = check => check.Tags.Contains(”ready”)
// }`.
app.MapHealthChecks("/health/ready", new HealthCheckOptions
// Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Memperbarui `Predicate` menggunakan fungsi lambda `check => check.Tags.Contains(”ready”)` yang dijalankan oleh operasi pemanggil untuk memproses
    // setiap masukan.
    Predicate = check => check.Tags.Contains("ready")
// Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut.
});
// Menjalankan memanggil `app.MapControllerRoute` dengan `”rulebook”`, `”rulebook”`, `new { controller = ”Home”, action = ”Rulebook” }`.
app.MapControllerRoute(
    // Meneruskan nilai literal `”rulebook”` sebagai argumen bernama `name`.
    name: "rulebook",
    // Meneruskan nilai literal `”rulebook”` sebagai argumen bernama `pattern`.
    pattern: "rulebook",
    // Meneruskan objek anonim yang mengelompokkan controller, action sebagai satu nilai sebagai argumen bernama `defaults`.
    defaults: new { controller = "Home", action = "Rulebook" });
// Menjalankan memanggil `app.MapControllerRoute` dengan `”privacy”`, `”privacy”`, `new { controller = ”Home”, action = ”Privacy” }`.
app.MapControllerRoute(
    // Meneruskan nilai literal `”privacy”` sebagai argumen bernama `name`.
    name: "privacy",
    // Meneruskan nilai literal `”privacy”` sebagai argumen bernama `pattern`.
    pattern: "privacy",
    // Meneruskan objek anonim yang mengelompokkan controller, action sebagai satu nilai sebagai argumen bernama `defaults`.
    defaults: new { controller = "Home", action = "Privacy" });
// Menjalankan memanggil `app.MapControllerRoute` dengan `”terms”`, `”terms”`, `new { controller = ”Home”, action = ”Terms” }`.
app.MapControllerRoute(
    // Meneruskan nilai literal `”terms”` sebagai argumen bernama `name`.
    name: "terms",
    // Meneruskan nilai literal `”terms”` sebagai argumen bernama `pattern`.
    pattern: "terms",
    // Meneruskan objek anonim yang mengelompokkan controller, action sebagai satu nilai sebagai argumen bernama `defaults`.
    defaults: new { controller = "Home", action = "Terms" });
// Menjalankan memanggil `app.MapControllerRoute` dengan `”default”`, `”{controller=Home}/{action=Index}/{id?}”`.
app.MapControllerRoute(
    // Meneruskan nilai literal `”default”` sebagai argumen bernama `name`.
    name: "default",
    // Meneruskan nilai literal `”{controller=Home}/{action=Index}/{id?}”` sebagai argumen bernama `pattern`.
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Menjalankan memanggil `app.Run` dengan tanpa argumen.
app.Run();
