// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk HomeController.
// Mengimpor namespace `System.Diagnostics` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Diagnostics;
// Mengimpor namespace `System.Net.Http.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Json;
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;
// Mengimpor namespace `System.Security` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security;
// Mengimpor namespace `Cashflowpoly.Ui.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Contracts;
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;
// Mengimpor namespace `Microsoft.Extensions.Caching.Memory` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.Extensions.Caching.Memory;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Controllers` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Controllers;

// Mendefinisikan tipe class `HomeController` yang mewarisi atau menerapkan `Controller`.
public class HomeController : Controller
// Membuka scope tipe HomeController; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `TimeSpan`: `RealtimeStatsCacheDuration` menyimpan nilai realtime stats cache duration dengan nilai awal memanggil
    // `TimeSpan.FromSeconds` dengan `20`. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field
    // menjadi milik tipe dan dibagikan antar instance.
    private static readonly TimeSpan RealtimeStatsCacheDuration = TimeSpan.FromSeconds(20);

    // Mendeklarasikan field bertipe `IHttpClientFactory`: `_clientFactory` menyimpan nilai client factory. readonly membatasi penggantian
    // referensi/nilai field pada deklarasi atau konstruktor.
    private readonly IHttpClientFactory _clientFactory;
    // Mendeklarasikan field bertipe `IMemoryCache`: `_memoryCache` menyimpan nilai memory cache. readonly membatasi penggantian referensi/nilai field
    // pada deklarasi atau konstruktor.
    private readonly IMemoryCache _memoryCache;
    // Mendeklarasikan field bertipe `IConfiguration`: `_configuration` menyimpan konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber
    // terdaftar. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly IConfiguration _configuration;

    // Mendefinisikan konstruktor HomeController yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `clientFactory` bertipe `IHttpClientFactory` membawa nilai client factory; Parameter `memoryCache` bertipe `IMemoryCache` membawa nilai memory
    // cache; Parameter `configuration` bertipe `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar.
    public HomeController(
        // Parameter `clientFactory` bertipe `IHttpClientFactory` membawa nilai client factory.
        IHttpClientFactory clientFactory,
        // Parameter `memoryCache` bertipe `IMemoryCache` membawa nilai memory cache.
        IMemoryCache memoryCache,
        // Parameter `configuration` bertipe `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar.
        IConfiguration configuration)
    // Membuka scope konstruktor HomeController; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam HomeController.
    {
        // Memperbarui `_clientFactory` menggunakan `clientFactory` (nilai client factory) dalam HomeController.
        _clientFactory = clientFactory;
        // Memperbarui `_memoryCache` menggunakan `memoryCache` (nilai memory cache) dalam HomeController.
        _memoryCache = memoryCache;
        // Memperbarui `_configuration` menggunakan `configuration` (konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar) dalam
        // HomeController.
        _configuration = configuration;
    // Menutup scope konstruktor HomeController; bagian berikut berada di luar batas blok tersebut dalam HomeController.
    }

    // Mendefinisikan metode `Index` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani index. async memungkinkan metode menunggu operasi
    // I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar
    // operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> Index(CancellationToken ct)
    // Membuka scope metode Index; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
    {
        // Menyiapkan variabel lokal `statsResult` untuk nilai stats hasil dengan hasil operasi asinkron memanggil `GetRealtimeStatsInternal` dengan `ct`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var statsResult = await GetRealtimeStatsInternal(ct);
        // Memeriksa hasil pencocokan `statsResult.UnauthorizedResult` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam Index.
        if (statsResult.UnauthorizedResult is not null)
        // Membuka scope cabang if untuk kondisi `statsResult.UnauthorizedResult is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam Index.
        {
            // Mengembalikan `statsResult.UnauthorizedResult` (nilai unauthorized hasil) kepada pemanggil dalam Index; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return statsResult.UnauthorizedResult;
        // Menutup scope cabang if untuk kondisi `statsResult.UnauthorizedResult is not null`; bagian berikut berada di luar batas blok tersebut dalam
        // Index.
        }

        // Menyiapkan variabel lokal `model` untuk nilai model dengan `statsResult.Model` (nilai model). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var model = statsResult.Model;

        // Mengembalikan menyiapkan tampilan Razor dengan `model` sebagai nama tampilan atau modelnya kepada pemanggil dalam Index; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return View(model);
    // Menutup scope metode Index; bagian berikut berada di luar batas blok tersebut dalam Index.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute controller saat ini.
    [HttpGet]
    // Mendefinisikan metode `RealtimeStats` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani realtime stats. async memungkinkan metode
    // menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `ct` bertipe `CancellationToken` membawa sinyal
    // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> RealtimeStats(CancellationToken ct)
    // Membuka scope metode RealtimeStats; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RealtimeStats.
    {
        // Menyiapkan variabel lokal `statsResult` untuk nilai stats hasil dengan hasil operasi asinkron memanggil `GetRealtimeStatsInternal` dengan `ct`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var statsResult = await GetRealtimeStatsInternal(ct);
        // Memeriksa hasil pencocokan `statsResult.UnauthorizedResult` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam RealtimeStats.
        if (statsResult.UnauthorizedResult is not null)
        // Membuka scope cabang if untuk kondisi `statsResult.UnauthorizedResult is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam RealtimeStats.
        {
            // Mengembalikan `statsResult.UnauthorizedResult` (nilai unauthorized hasil) kepada pemanggil dalam RealtimeStats; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return statsResult.UnauthorizedResult;
        // Menutup scope cabang if untuk kondisi `statsResult.UnauthorizedResult is not null`; bagian berikut berada di luar batas blok tersebut dalam
        // RealtimeStats.
        }

        // Menyiapkan variabel lokal `model` untuk nilai model dengan `statsResult.Model` (nilai model). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var model = statsResult.Model;
        // Mengembalikan memanggil `Json` dengan `new { activeSessions = model.ActiveSessions, totalSessions = model.TotalSessions, totalPlayers =
        // model.TotalPlayers, totalRulesets = model.TotalRulesets, lastSyncedAt = model....` kepada pemanggil dalam RealtimeStats; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return Json(new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RealtimeStats.
        {
            // Meneruskan objek anonim yang mengelompokkan activeSessions, totalSessions, totalPlayers, totalRulesets, lastSyncedAt, errorMessage sebagai satu
            // nilai sebagai argumen ke `Json`.
            activeSessions = model.ActiveSessions,
            // Meneruskan objek anonim yang mengelompokkan activeSessions, totalSessions, totalPlayers, totalRulesets, lastSyncedAt, errorMessage sebagai satu
            // nilai sebagai argumen ke `Json`.
            totalSessions = model.TotalSessions,
            // Meneruskan objek anonim yang mengelompokkan activeSessions, totalSessions, totalPlayers, totalRulesets, lastSyncedAt, errorMessage sebagai satu
            // nilai sebagai argumen ke `Json`.
            totalPlayers = model.TotalPlayers,
            // Meneruskan objek anonim yang mengelompokkan activeSessions, totalSessions, totalPlayers, totalRulesets, lastSyncedAt, errorMessage sebagai satu
            // nilai sebagai argumen ke `Json`.
            totalRulesets = model.TotalRulesets,
            // Meneruskan objek anonim yang mengelompokkan activeSessions, totalSessions, totalPlayers, totalRulesets, lastSyncedAt, errorMessage sebagai satu
            // nilai sebagai argumen ke `Json`.
            lastSyncedAt = model.LastSyncedAt,
            // Meneruskan objek anonim yang mengelompokkan activeSessions, totalSessions, totalPlayers, totalRulesets, lastSyncedAt, errorMessage sebagai satu
            // nilai sebagai argumen ke `Json`.
            errorMessage = model.ErrorMessage
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam RealtimeStats.
        });
    // Menutup scope metode RealtimeStats; bagian berikut berada di luar batas blok tersebut dalam RealtimeStats.
    }

    // Mendefinisikan metode `Rulebook` dengan hasil bertipe `IActionResult`; operasi ini menangani rulebook.
    public IActionResult Rulebook()
    // Membuka scope metode Rulebook; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Rulebook.
    {
        // Menyiapkan variabel lokal `requestPath` untuk nilai permintaan path dengan `HttpContext.Request.Path.Value?.TrimEnd('/')`; akses setelah ?. hanya
        // dilakukan bila penerimanya tidak null. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var requestPath = HttpContext.Request.Path.Value?.TrimEnd('/');
        // Memeriksa membandingkan kesamaan `string` dengan `requestPath`, `”/Home/Rulebook”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan
        // mengikuti overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Rulebook.
        if (string.Equals(requestPath, "/Home/Rulebook", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(requestPath, ”/Home/Rulebook”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam Rulebook.
        {
            // Mengembalikan memanggil `Redirect` dengan `”/rulebook”` kepada pemanggil dalam Rulebook; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Redirect("/rulebook");
        // Menutup scope cabang if untuk kondisi `string.Equals(requestPath, ”/Home/Rulebook”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada
        // di luar batas blok tersebut dalam Rulebook.
        }

        // Menyiapkan variabel lokal `language` untuk nilai language dengan memanggil `UiText.NormalizeLanguage` dengan
        // `HttpContext.Session.GetString(AuthConstants.SessionLanguageKey)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var language = UiText.NormalizeLanguage(HttpContext.Session.GetString(AuthConstants.SessionLanguageKey));
        // Mengembalikan menyiapkan tampilan Razor dengan `”Privacy”`, `RulebookContent.Build(language)` sebagai nama tampilan atau modelnya kepada
        // pemanggil dalam Rulebook; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return View("Privacy", model: RulebookContent.Build(language));
    // Menutup scope metode Rulebook; bagian berikut berada di luar batas blok tersebut dalam Rulebook.
    }

    // Mendefinisikan metode `Privacy` dengan hasil bertipe `IActionResult`; operasi ini menangani privacy.
    public IActionResult Privacy()
    // Membuka scope metode Privacy; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Privacy.
    {
        // Mengembalikan menyiapkan tampilan Razor dengan `”PrivacyPolicy”` sebagai nama tampilan atau modelnya kepada pemanggil dalam Privacy; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return View("PrivacyPolicy");
    // Menutup scope metode Privacy; bagian berikut berada di luar batas blok tersebut dalam Privacy.
    }

    // Mendefinisikan metode `Terms` dengan hasil bertipe `IActionResult`; operasi ini menangani terms.
    public IActionResult Terms()
    // Membuka scope metode Terms; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Terms.
    {
        // Mengembalikan menyiapkan tampilan Razor dengan tanpa argumen sebagai nama tampilan atau modelnya kepada pemanggil dalam Terms; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return View();
    // Menutup scope metode Terms; bagian berikut berada di luar batas blok tersebut dalam Terms.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”/robots.txt”).
    [HttpGet("/robots.txt")]
    // Mendefinisikan metode `Robots` dengan hasil bertipe `IActionResult`; operasi ini menangani robots.
    public IActionResult Robots()
    // Membuka scope metode Robots; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Robots.
    {
        // Menyiapkan variabel lokal `baseUrl` untuk nilai base url dengan memanggil `SiteUrlResolver.ResolveBaseUrl` dengan `_configuration`, `Request`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var baseUrl = SiteUrlResolver.ResolveBaseUrl(_configuration, Request);
        // Menyiapkan variabel lokal `content` untuk nilai content dengan teks interpolasi `$”User-agent: *\nAllow: /rulebook\nAllow: /privacy\nAllow:
        // /terms\nDisallow: /auth/\nDisallow: /sessions/\nDisallow: /players/\nDisallow: /rulesets/\nSitemap: {baseUrl}/sitema...`; nilai ekspresi di dalam
        // kurung kurawal disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var content = $"User-agent: *\nAllow: /rulebook\nAllow: /privacy\nAllow: /terms\nDisallow: /auth/\nDisallow: /sessions/\nDisallow: /players/\nDisallow: /rulesets/\nSitemap: {baseUrl}/sitemap.xml\n";
        // Mengembalikan memanggil `Content` dengan `content`, `”text/plain”` kepada pemanggil dalam Robots; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return Content(content, "text/plain");
    // Menutup scope metode Robots; bagian berikut berada di luar batas blok tersebut dalam Robots.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”/sitemap.xml”).
    [HttpGet("/sitemap.xml")]
    // Mendefinisikan metode `Sitemap` dengan hasil bertipe `IActionResult`; operasi ini menangani sitemap.
    public IActionResult Sitemap()
    // Membuka scope metode Sitemap; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Sitemap.
    {
        // Menyiapkan variabel lokal `baseUrl` untuk nilai base url dengan memanggil `SecurityElement.Escape` dengan
        // `SiteUrlResolver.ResolveBaseUrl(_configuration, Request)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var baseUrl = SecurityElement.Escape(SiteUrlResolver.ResolveBaseUrl(_configuration, Request));
        // Menyiapkan variabel lokal `content` untuk nilai content dengan penjumlahan/penggabungan antara `$”<?xml version=\”1.0\” encoding=\”UTF-8\”?>\n” +
        // $”<urlset xmlns=\”http://www.sitemaps.org/schemas/sitemap/0.9\”>\n” + $” <url><loc>{baseUrl}/</loc></url>\n” + $” <url><loc>{...` dan
        // `$”</urlset>\n”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var content = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
            // Menggunakan teks interpolasi `$”<urlset xmlns=\”http://www.sitemaps.org/schemas/sitemap/0.9\”>\n”`; nilai ekspresi di dalam kurung kurawal
            // disisipkan saat program berjalan sebagai bagian ekspresi yang sedang disusun dalam Sitemap.
            $"<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">\n" +
            // Menggunakan teks interpolasi `$” <url><loc>{baseUrl}/</loc></url>\n”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan
            // sebagai bagian ekspresi yang sedang disusun dalam Sitemap.
            $"  <url><loc>{baseUrl}/</loc></url>\n" +
            // Menggunakan teks interpolasi `$” <url><loc>{baseUrl}/rulebook</loc></url>\n”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai bagian ekspresi yang sedang disusun dalam Sitemap.
            $"  <url><loc>{baseUrl}/rulebook</loc></url>\n" +
            // Menggunakan teks interpolasi `$”</urlset>\n”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai bagian ekspresi
            // yang sedang disusun dalam Sitemap.
            $"</urlset>\n";
        // Mengembalikan memanggil `Content` dengan `content`, `”application/xml”` kepada pemanggil dalam Sitemap; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return Content(content, "application/xml");
    // Menutup scope metode Sitemap; bagian berikut berada di luar batas blok tersebut dalam Sitemap.
    }

    // menerapkan metadata `ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // Mendefinisikan metode `Error` dengan hasil bertipe `IActionResult`; operasi ini menangani kesalahan.
    public IActionResult Error()
    // Membuka scope metode Error; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Error.
    {
        // Mengembalikan menyiapkan tampilan Razor dengan `new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }` sebagai
        // nama tampilan atau modelnya kepada pemanggil dalam Error; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // Menutup scope metode Error; bagian berikut berada di luar batas blok tersebut dalam Error.
    }

    // Mendefinisikan metode `GetRealtimeStatsInternal` dengan hasil bertipe `Task<(HomeIndexViewModel Model, IActionResult? UnauthorizedResult)>`;
    // operasi ini menangani get realtime stats internal. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian
    // melalui Task. Masukan: Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    private async Task<(HomeIndexViewModel Model, IActionResult? UnauthorizedResult)> GetRealtimeStatsInternal(CancellationToken ct)
    // Membuka scope metode GetRealtimeStatsInternal; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRealtimeStatsInternal.
    {
        // Menyiapkan variabel lokal `cacheKey` untuk nilai cache kunci dengan memanggil `BuildRealtimeStatsCacheKey` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var cacheKey = BuildRealtimeStatsCacheKey();
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `_memoryCache.TryGetValue(cacheKey, out HomeIndexViewModel? cachedModel)` dan
        // `cachedModel is not null`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetRealtimeStatsInternal.
        if (_memoryCache.TryGetValue(cacheKey, out HomeIndexViewModel? cachedModel) && cachedModel is not null)
        // Membuka scope cabang if untuk kondisi `_memoryCache.TryGetValue(cacheKey, out HomeIndexViewModel? cachedModel) && cachedModel is not null`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRealtimeStatsInternal.
        {
            // Mengembalikan tuple yang membawa bagian 1: cachedModel; bagian 2: null kepada pemanggil dalam GetRealtimeStatsInternal; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return (cachedModel, null);
        // Menutup scope cabang if untuk kondisi `_memoryCache.TryGetValue(cacheKey, out HomeIndexViewModel? cachedModel) && cachedModel is not null`;
        // bagian berikut berada di luar batas blok tersebut dalam GetRealtimeStatsInternal.
        }

        // Menyiapkan variabel lokal `client` untuk nilai client dengan memanggil `_clientFactory.CreateClient` dengan `”Api”`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var client = _clientFactory.CreateClient("Api");
        var errors = new System.Collections.Concurrent.ConcurrentBag<string>();
        var expired = false;
        async Task<T?> LoadAsync<T>(string path) where T : class
        {
            try
            {
                using var response = await client.GetAsync(path, ct);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) expired = true;
                var value = response.IsSuccessStatusCode
                    ? await response.Content.TryReadFromJsonAsync<T>(cancellationToken: ct) : null;
                if (value is null) errors.Add(path.Split('?')[0].Split('/').Last());
                return value;
            }
            catch (HttpRequestException) { errors.Add(path.Split('?')[0].Split('/').Last()); return null; }
            catch (TaskCanceledException) when (!ct.IsCancellationRequested)
            { errors.Add(path.Split('?')[0].Split('/').Last()); return null; }
        }

        var sessionsTask = LoadAsync<SessionListResponse>("api/v1/sessions");
        var playersTask = LoadAsync<PlayerListResponse>(HttpContext.IsInstructor()
            ? "api/v1/players?inMySessions=true" : "api/v1/players");
        var rulesetsTask = LoadAsync<RulesetListResponse>("api/v1/rulesets");
        await Task.WhenAll(sessionsTask, playersTask, rulesetsTask);
        if (expired)
        {
            using var response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            return (new HomeIndexViewModel(), this.HandleUnauthorizedApiResponse(response));
        }

        var sessions = sessionsTask.Result?.Items;
        var players = playersTask.Result?.Items;
        var rulesets = rulesetsTask.Result?.Items;
        if (sessions is null && sessionsTask.Result is not null) errors.Add("sessions");
        if (players is null && playersTask.Result is not null) errors.Add("players");
        if (rulesets is null && rulesetsTask.Result is not null) errors.Add("rulesets");
        var model = new HomeIndexViewModel
        {
            TotalSessions = sessions?.Count,
            ActiveSessions = sessions?.Count(s => string.Equals(s.Status, "STARTED", StringComparison.OrdinalIgnoreCase)),
            // The player-list endpoint includes the account itself even before it joins a session.
            TotalPlayers = !HttpContext.IsInstructor() && sessions is null ? null
                : sessions?.Count == 0 ? 0 : players?.Select(p => p.UserId).Distinct().Count(),
            TotalRulesets = rulesets?.Count(r => string.Equals(r.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase)),
            LastSyncedAt = DateTimeOffset.UtcNow,
            ErrorMessage = errors.IsEmpty ? null : HttpContext.T("home.error.partial_realtime_failed")
                .Replace("{details}", string.Join(", ", errors.Distinct().Order()))
        };
        if (errors.IsEmpty) _memoryCache.Set(cacheKey, model, RealtimeStatsCacheDuration);
        return (model, null);
    }

    private string BuildRealtimeStatsCacheKey()
    // Membuka scope metode BuildRealtimeStatsCacheKey; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildRealtimeStatsCacheKey.
    {
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan `HttpContext.Session.Id`
        // (nilai identitas). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = HttpContext.Session.Id;
        // Menyiapkan variabel lokal `userId` untuk identitas akun pengguna yang datanya sedang diproses dengan
        // `User.FindFirst(ClaimTypes.NameIdentifier)?.Value`; akses setelah ?. hanya dilakukan bila penerimanya tidak null. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // Menyiapkan variabel lokal `role` untuk peran pengguna yang menentukan hak akses dengan `User.FindFirst(ClaimTypes.Role)?.Value`; akses setelah ?.
        // hanya dilakukan bila penerimanya tidak null. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        // Menyiapkan variabel lokal `sessionScope` untuk nilai sesi cakupan dengan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(sessionId)`
        // benar gunakan `”anonymous-session”`, jika tidak gunakan `sessionId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionScope = string.IsNullOrWhiteSpace(sessionId) ? "anonymous-session" : sessionId;
        // Menyiapkan variabel lokal `userScope` untuk nilai pengguna cakupan dengan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(userId)`
        // benar gunakan `”anonymous-user”`, jika tidak gunakan `userId.Trim()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var userScope = string.IsNullOrWhiteSpace(userId) ? "anonymous-user" : userId.Trim();
        // Menyiapkan variabel lokal `roleScope` untuk nilai role cakupan dengan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(role)` benar
        // gunakan `”unknown-role”`, jika tidak gunakan `role.Trim().ToUpperInvariant()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var roleScope = string.IsNullOrWhiteSpace(role) ? "unknown-role" : role.Trim().ToUpperInvariant();
        // Mengembalikan teks interpolasi `$”home:realtime:{sessionScope}:{userScope}:{roleScope}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
        // program berjalan kepada pemanggil dalam BuildRealtimeStatsCacheKey; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return $"home:realtime:{sessionScope}:{userScope}:{roleScope}";
    // Menutup scope metode BuildRealtimeStatsCacheKey; bagian berikut berada di luar batas blok tersebut dalam BuildRealtimeStatsCacheKey.
    }
// Menutup scope tipe HomeController; bagian berikut berada di luar batas blok tersebut.
}
