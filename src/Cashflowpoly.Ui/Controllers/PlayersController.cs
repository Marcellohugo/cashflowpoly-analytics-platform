// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk PlayersController.
// Mengimpor namespace `System.Net.Http.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Json;
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;
// Mengimpor namespace `Cashflowpoly.Ui.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Contracts;
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Controllers` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Controllers;

// menetapkan pola rute (”sessions/{sessionId:guid}/players”) untuk pencocokan URL permintaan.
[Route("sessions/{sessionId:guid}/players")]
// Mendefinisikan tipe class `PlayersController` yang mewarisi atau menerapkan `Controller`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayersController : Controller
// Membuka scope tipe PlayersController; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `IHttpClientFactory`: `_clientFactory` menyimpan nilai client factory. readonly membatasi penggantian
    // referensi/nilai field pada deklarasi atau konstruktor.
    private readonly IHttpClientFactory _clientFactory;

    // Mendefinisikan konstruktor PlayersController yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `clientFactory` bertipe `IHttpClientFactory` membawa nilai client factory.
    public PlayersController(IHttpClientFactory clientFactory)
    // Membuka scope konstruktor PlayersController; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam PlayersController.
    {
        // Memperbarui `_clientFactory` menggunakan `clientFactory` (nilai client factory) dalam PlayersController.
        _clientFactory = clientFactory;
    // Menutup scope konstruktor PlayersController; bagian berikut berada di luar batas blok tersebut dalam PlayersController.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”{playerId:guid}”).
    [HttpGet("{playerId:guid}")]
    // Mendefinisikan metode `Details` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani rincian. async memungkinkan metode menunggu
    // operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi
    // permainan yang menjadi batas data operasi ini; Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> Details(Guid sessionId, Guid playerId, CancellationToken ct)
    // Membuka scope metode Details; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Details.
    {
        // Memeriksa kebalikan kondisi `HttpContext.IsInstructor()`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Details.
        if (!HttpContext.IsInstructor())
        // Membuka scope cabang if untuk kondisi `!HttpContext.IsInstructor()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Details.
        {
            // Menyiapkan variabel lokal `currentUserIdRaw` untuk nilai saat ini pengguna identitas raw dengan
            // `User.FindFirst(ClaimTypes.NameIdentifier)?.Value`; akses setelah ?. hanya dilakukan bila penerimanya tidak null. Tipe variabel disimpulkan dari
            // ekspresi nilai awal.
            var currentUserIdRaw = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!Guid.TryParse(currentUserIdRaw, out var currentUserId)` dan
            // `currentUserId != playerId`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // Details.
            if (!Guid.TryParse(currentUserIdRaw, out var currentUserId) || currentUserId != playerId)
            // Membuka scope cabang if untuk kondisi `!Guid.TryParse(currentUserIdRaw, out var currentUserId) || currentUserId != playerId`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Details.
            {
                // Mengembalikan membentuk hasil HTTP dengan kode dan isi `StatusCodes.Status403Forbidden` kepada pemanggil dalam Details; eksekusi jalur ini
                // selesai setelah nilai hasil ditentukan.
                return StatusCode(StatusCodes.Status403Forbidden);
            // Menutup scope cabang if untuk kondisi `!Guid.TryParse(currentUserIdRaw, out var currentUserId) || currentUserId != playerId`; bagian berikut
            // berada di luar batas blok tersebut dalam Details.
            }
        // Menutup scope cabang if untuk kondisi `!HttpContext.IsInstructor()`; bagian berikut berada di luar batas blok tersebut dalam Details.
        }

        // Menyiapkan variabel lokal `client` untuk nilai client dengan memanggil `_clientFactory.CreateClient` dengan `”Api”`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var client = _clientFactory.CreateClient("Api");
        // Menyiapkan variabel lokal `playerDisplayName` untuk nilai pemain display nama dengan hasil operasi asinkron memanggil
        // `ResolvePlayerDisplayNameAsync` dengan `client`, `playerId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var playerDisplayName = await ResolvePlayerDisplayNameAsync(client, playerId, ct);
        // Menyiapkan variabel lokal `analyticsResponse` untuk nilai analytics respons dengan hasil operasi asinkron memanggil `client.GetAsync` dengan
        // `$”api/v1/analytics/sessions/{sessionId}”`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var analyticsResponse = await client.GetAsync($"api/v1/analytics/sessions/{sessionId}", ct);
        // Menyiapkan variabel lokal `unauthorized` untuk nilai unauthorized dengan memanggil `this.HandleUnauthorizedApiResponse` dengan
        // `analyticsResponse`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var unauthorized = this.HandleUnauthorizedApiResponse(analyticsResponse);
        // Memeriksa hasil pencocokan `unauthorized` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Details.
        if (unauthorized is not null)
        // Membuka scope cabang if untuk kondisi `unauthorized is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Details.
        {
            // Mengembalikan `unauthorized` (nilai unauthorized) kepada pemanggil dalam Details; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return unauthorized;
        // Menutup scope cabang if untuk kondisi `unauthorized is not null`; bagian berikut berada di luar batas blok tersebut dalam Details.
        }

        // Memeriksa kebalikan kondisi `analyticsResponse.IsSuccessStatusCode`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Details.
        if (!analyticsResponse.IsSuccessStatusCode)
        // Membuka scope cabang if untuk kondisi `!analyticsResponse.IsSuccessStatusCode`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Details.
        {
            // Menyiapkan variabel lokal `error` untuk informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil dengan hasil operasi
            // asinkron memanggil `analyticsResponse.Content.TryReadFromJsonAsync<ErrorResponse>` dengan `ct`; await menunggu hasil tanpa memblokir thread
            // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var error = await analyticsResponse.Content.TryReadFromJsonAsync<ErrorResponse>(cancellationToken: ct);
            // Mengembalikan menyiapkan tampilan Razor dengan `new PlayerDetailViewModel { SessionId = sessionId, PlayerId = playerId, PlayerDisplayName =
            // playerDisplayName, ErrorMessage = error?.Message ?? HttpContext .T(”players.error.l...` sebagai nama tampilan atau modelnya kepada pemanggil
            // dalam Details; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return View(new PlayerDetailViewModel
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Details.
            {
                // Memperbarui `SessionId` menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam Details.
                SessionId = sessionId,
                // Memperbarui `PlayerId` menggunakan `playerId` (nilai pemain identitas) dalam Details.
                PlayerId = playerId,
                // Memperbarui `PlayerDisplayName` menggunakan `playerDisplayName` (nilai pemain display nama) dalam Details.
                PlayerDisplayName = playerDisplayName,
                // Memperbarui `ErrorMessage` menggunakan `error?.Message` bila tidak null; jika null gunakan `HttpContext
                // .T(”players.error.load_session_analytics_failed”) .Replace(”{status}”, ((int)analyticsResponse.StatusCode).ToString())` sebagai nilai pengganti
                // dalam Details.
                ErrorMessage = error?.Message ?? HttpContext
                    // Meneruskan nilai literal `”players.error.load_session_analytics_failed”` sebagai argumen ke `HttpContext .T`.
                    .T("players.error.load_session_analytics_failed")
                    // Meneruskan nilai literal `”{status}”` sebagai argumen ke `HttpContext .T(”players.error.load_session_analytics_failed”) .Replace`; Meneruskan
                    // mengubah `((int)analyticsResponse.StatusCode)` menjadi teks sebagai argumen ke `HttpContext .T(”players.error.load_session_analytics_failed”)
                    // .Replace`.
                    .Replace("{status}", ((int)analyticsResponse.StatusCode).ToString())
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Details.
            });
        // Menutup scope cabang if untuk kondisi `!analyticsResponse.IsSuccessStatusCode`; bagian berikut berada di luar batas blok tersebut dalam Details.
        }

        // Menyiapkan variabel lokal `analytics` untuk nilai analytics dengan hasil operasi asinkron memanggil
        // `analyticsResponse.Content.TryReadFromJsonAsync<AnalyticsSessionResponse>` dengan `ct`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var analytics = await analyticsResponse.Content.TryReadFromJsonAsync<AnalyticsSessionResponse>(cancellationToken: ct);
        // Menyiapkan variabel lokal `summary` untuk nilai summary dengan `analytics?.ByPlayer.FirstOrDefault(p => p.UserId == playerId)`; akses setelah ?.
        // hanya dilakukan bila penerimanya tidak null. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var summary = analytics?.ByPlayer.FirstOrDefault(p => p.UserId == playerId);

        // Menyiapkan variabel lokal `txResponse` untuk nilai tx respons dengan hasil operasi asinkron memanggil `client.GetAsync` dengan
        // `$”api/v1/analytics/sessions/{sessionId}/transactions?userId={playerId}&limit=100”`, `ct`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var txResponse = await client.GetAsync($"api/v1/analytics/sessions/{sessionId}/transactions?userId={playerId}&limit=100", ct);
        // Memperbarui `unauthorized` menggunakan memanggil `this.HandleUnauthorizedApiResponse` dengan `txResponse` dalam Details.
        unauthorized = this.HandleUnauthorizedApiResponse(txResponse);
        // Memeriksa hasil pencocokan `unauthorized` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Details.
        if (unauthorized is not null)
        // Membuka scope cabang if untuk kondisi `unauthorized is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Details.
        {
            // Mengembalikan `unauthorized` (nilai unauthorized) kepada pemanggil dalam Details; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return unauthorized;
        // Menutup scope cabang if untuk kondisi `unauthorized is not null`; bagian berikut berada di luar batas blok tersebut dalam Details.
        }

        // Memeriksa kebalikan kondisi `txResponse.IsSuccessStatusCode`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Details.
        if (!txResponse.IsSuccessStatusCode)
        // Membuka scope cabang if untuk kondisi `!txResponse.IsSuccessStatusCode`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Details.
        {
            // Menyiapkan variabel lokal `error` untuk informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil dengan hasil operasi
            // asinkron memanggil `txResponse.Content.TryReadFromJsonAsync<ErrorResponse>` dengan `ct`; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var error = await txResponse.Content.TryReadFromJsonAsync<ErrorResponse>(cancellationToken: ct);
            // Mengembalikan menyiapkan tampilan Razor dengan `new PlayerDetailViewModel { SessionId = sessionId, PlayerId = playerId, PlayerDisplayName =
            // playerDisplayName, Summary = summary, ErrorMessage = error?.Message ?? HttpContext ...` sebagai nama tampilan atau modelnya kepada pemanggil
            // dalam Details; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return View(new PlayerDetailViewModel
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Details.
            {
                // Memperbarui `SessionId` menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam Details.
                SessionId = sessionId,
                // Memperbarui `PlayerId` menggunakan `playerId` (nilai pemain identitas) dalam Details.
                PlayerId = playerId,
                // Memperbarui `PlayerDisplayName` menggunakan `playerDisplayName` (nilai pemain display nama) dalam Details.
                PlayerDisplayName = playerDisplayName,
                // Memperbarui `Summary` menggunakan `summary` (nilai summary) dalam Details.
                Summary = summary,
                // Memperbarui `ErrorMessage` menggunakan `error?.Message` bila tidak null; jika null gunakan `HttpContext
                // .T(”players.error.load_transactions_failed”) .Replace(”{status}”, ((int)txResponse.StatusCode).ToString())` sebagai nilai pengganti dalam
                // Details.
                ErrorMessage = error?.Message ?? HttpContext
                    // Meneruskan nilai literal `”players.error.load_transactions_failed”` sebagai argumen ke `HttpContext .T`.
                    .T("players.error.load_transactions_failed")
                    // Meneruskan nilai literal `”{status}”` sebagai argumen ke `HttpContext .T(”players.error.load_transactions_failed”) .Replace`; Meneruskan mengubah
                    // `((int)txResponse.StatusCode)` menjadi teks sebagai argumen ke `HttpContext .T(”players.error.load_transactions_failed”) .Replace`.
                    .Replace("{status}", ((int)txResponse.StatusCode).ToString())
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Details.
            });
        // Menutup scope cabang if untuk kondisi `!txResponse.IsSuccessStatusCode`; bagian berikut berada di luar batas blok tersebut dalam Details.
        }

        // Menyiapkan variabel lokal `tx` untuk transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan dengan hasil operasi asinkron
        // memanggil `txResponse.Content.TryReadFromJsonAsync<TransactionHistoryResponse>` dengan `ct`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var tx = await txResponse.Content.TryReadFromJsonAsync<TransactionHistoryResponse>(cancellationToken: ct);
        // Menyiapkan variabel lokal `transactions` untuk nilai transactions dengan `tx?.Items` bila tidak null; jika null gunakan `new
        // List<TransactionHistoryItem>()` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var transactions = tx?.Items ?? new List<TransactionHistoryItem>();
        // Mengulangi blok selama gabungan syarat AND: kedua kondisi wajib benar antara `tx?.HasMore == true` dan
        // `!string.IsNullOrWhiteSpace(tx.NextCursor)`; sisi kanan diperiksa hanya jika sisi kiri benar; kondisi diperiksa lagi sebelum setiap iterasi dalam
        // Details.
        while (tx?.HasMore == true && !string.IsNullOrWhiteSpace(tx.NextCursor))
        // Membuka scope loop selama `tx?.HasMore == true && !string.IsNullOrWhiteSpace(tx.NextCursor)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam Details.
        {
            // Memperbarui `txResponse` menggunakan hasil operasi asinkron memanggil `client.GetAsync` dengan
            // `$”api/v1/analytics/sessions/{sessionId}/transactions?userId={playerId}&limit=100&cursor={Uri.EscapeDataString(tx.NextCursor)}”`, `ct`; await
            // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam Details.
            txResponse = await client.GetAsync(
                // Meneruskan teks interpolasi `$”api/v1/analytics/sessions/{sessionId}/transactions?userId={playerId}&limit=100&cursor={Uri.EscapeDataString(tx.Nex
                // tCursor)}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `client.GetAsync`; Meneruskan
                // `tx.NextCursor` (nilai next cursor) sebagai argumen ke `Uri.EscapeDataString`.
                $"api/v1/analytics/sessions/{sessionId}/transactions?userId={playerId}&limit=100&cursor={Uri.EscapeDataString(tx.NextCursor)}",
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `client.GetAsync`.
                ct);
            // Memeriksa kebalikan kondisi `txResponse.IsSuccessStatusCode`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Details.
            if (!txResponse.IsSuccessStatusCode)
            // Membuka scope cabang if untuk kondisi `!txResponse.IsSuccessStatusCode`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // Details.
            {
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam Details.
                break;
            // Menutup scope cabang if untuk kondisi `!txResponse.IsSuccessStatusCode`; bagian berikut berada di luar batas blok tersebut dalam Details.
            }

            // Memperbarui `tx` menggunakan hasil operasi asinkron memanggil `txResponse.Content.TryReadFromJsonAsync<TransactionHistoryResponse>` dengan `ct`;
            // await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam Details.
            tx = await txResponse.Content.TryReadFromJsonAsync<TransactionHistoryResponse>(cancellationToken: ct);
            // Memeriksa hasil pencocokan `tx` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Details.
            if (tx is not null)
            // Membuka scope cabang if untuk kondisi `tx is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Details.
            {
                // Menjalankan menambahkan seluruh elemen `tx.Items` ke `transactions` dalam Details.
                transactions.AddRange(tx.Items);
            // Menutup scope cabang if untuk kondisi `tx is not null`; bagian berikut berada di luar batas blok tersebut dalam Details.
            }
        // Menutup scope loop selama `tx?.HasMore == true && !string.IsNullOrWhiteSpace(tx.NextCursor)`; bagian berikut berada di luar batas blok tersebut
        // dalam Details.
        }
        // Menyiapkan variabel lokal `gameplayError` untuk nilai gameplay kesalahan dengan null, yaitu penanda tidak ada nilai. Tipe yang dipakai adalah
        // `string?`.
        string? gameplayError = null;
        // Menyiapkan variabel lokal `gameplay` untuk nilai gameplay dengan null, yaitu penanda tidak ada nilai. Tipe yang dipakai adalah
        // `GameplayMetricsResponse?`.
        GameplayMetricsResponse? gameplay = null;
        // Menyiapkan variabel lokal `gameplayResponse` untuk nilai gameplay respons dengan hasil operasi asinkron memanggil `client.GetAsync` dengan
        // `$”api/v1/analytics/sessions/{sessionId}/players/{playerId}/gameplay”`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var gameplayResponse = await client.GetAsync($"api/v1/analytics/sessions/{sessionId}/players/{playerId}/gameplay", ct);
        // Memperbarui `unauthorized` menggunakan memanggil `this.HandleUnauthorizedApiResponse` dengan `gameplayResponse` dalam Details.
        unauthorized = this.HandleUnauthorizedApiResponse(gameplayResponse);
        // Memeriksa hasil pencocokan `unauthorized` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Details.
        if (unauthorized is not null)
        // Membuka scope cabang if untuk kondisi `unauthorized is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Details.
        {
            // Mengembalikan `unauthorized` (nilai unauthorized) kepada pemanggil dalam Details; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return unauthorized;
        // Menutup scope cabang if untuk kondisi `unauthorized is not null`; bagian berikut berada di luar batas blok tersebut dalam Details.
        }

        // Memeriksa `gameplayResponse.IsSuccessStatusCode` (nilai berstatus success status kode); blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam Details.
        if (gameplayResponse.IsSuccessStatusCode)
        // Membuka scope cabang if untuk kondisi `gameplayResponse.IsSuccessStatusCode`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Details.
        {
            // Memperbarui `gameplay` menggunakan hasil operasi asinkron memanggil `gameplayResponse.Content.TryReadFromJsonAsync<GameplayMetricsResponse>`
            // dengan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam Details.
            gameplay = await gameplayResponse.Content.TryReadFromJsonAsync<GameplayMetricsResponse>(cancellationToken: ct);
        // Menutup scope cabang if untuk kondisi `gameplayResponse.IsSuccessStatusCode`; bagian berikut berada di luar batas blok tersebut dalam Details.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam Details.
        else
        // Membuka scope cabang else; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Details.
        {
            // Menyiapkan variabel lokal `error` untuk informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil dengan hasil operasi
            // asinkron memanggil `gameplayResponse.Content.TryReadFromJsonAsync<ErrorResponse>` dengan `ct`; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var error = await gameplayResponse.Content.TryReadFromJsonAsync<ErrorResponse>(cancellationToken: ct);
            // Memperbarui `gameplayError` menggunakan `error?.Message` bila tidak null; jika null gunakan `HttpContext .T(”players.error.load_gameplay_failed”)
            // .Replace(”{status}”, ((int)gameplayResponse.StatusCode).ToString())` sebagai nilai pengganti dalam Details.
            gameplayError = error?.Message ?? HttpContext
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .T(”players.error.load_gameplay_failed”) dalam Details; token pada baris ini
                // menyambungkan bagian kode sebelum dan sesudahnya.
                .T("players.error.load_gameplay_failed")
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Replace(”{status}”, ((int)gameplayResponse.StatusCode).ToString()); dalam
                // Details; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Replace("{status}", ((int)gameplayResponse.StatusCode).ToString());
        // Menutup scope cabang else; bagian berikut berada di luar batas blok tersebut dalam Details.
        }

        // Menyiapkan variabel lokal `fallbackStartingCash` untuk nilai fallback starting uang tunai dengan memanggil `InferDefaultStartingCash` dengan
        // `analytics?.RulesetName`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var fallbackStartingCash = InferDefaultStartingCash(analytics?.RulesetName);
        // Menyiapkan variabel lokal `startingCash` untuk nilai starting uang tunai dengan `gameplay?.Economy.StartingCash` bila tidak null; jika null
        // gunakan `fallbackStartingCash` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var startingCash = gameplay?.Economy.StartingCash ?? fallbackStartingCash;
        // Menyiapkan variabel lokal `cashflowJourney` untuk nilai arus kas journey dengan memanggil `BuildCashflowJourneyStats` dengan `transactions`,
        // `startingCash`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var cashflowJourney = BuildCashflowJourneyStats(transactions, startingCash);
        // Menyiapkan variabel lokal `statSummary` untuk nilai stat summary dengan memanggil `PlayerStatSummaryBuilder.Build` dengan `gameplay`, `summary`,
        // `cashflowJourney`, `HttpContext.T`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var statSummary = PlayerStatSummaryBuilder.Build(gameplay, summary, cashflowJourney, HttpContext.T);

        // Mengembalikan menyiapkan tampilan Razor dengan `new PlayerDetailViewModel { SessionId = sessionId, PlayerId = playerId, PlayerDisplayName =
        // playerDisplayName, Summary = summary, StatSummary = statSummary, CashflowJourney = ...` sebagai nama tampilan atau modelnya kepada pemanggil
        // dalam Details; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return View(new PlayerDetailViewModel
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Details.
        {
            // Memperbarui `SessionId` menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam Details.
            SessionId = sessionId,
            // Memperbarui `PlayerId` menggunakan `playerId` (nilai pemain identitas) dalam Details.
            PlayerId = playerId,
            // Memperbarui `PlayerDisplayName` menggunakan `playerDisplayName` (nilai pemain display nama) dalam Details.
            PlayerDisplayName = playerDisplayName,
            // Memperbarui `Summary` menggunakan `summary` (nilai summary) dalam Details.
            Summary = summary,
            // Memperbarui `StatSummary` menggunakan `statSummary` (nilai stat summary) dalam Details.
            StatSummary = statSummary,
            // Memperbarui `CashflowJourney` menggunakan `cashflowJourney` (nilai arus kas journey) dalam Details.
            CashflowJourney = cashflowJourney,
            // Memperbarui `GameplayRaw` menggunakan `gameplay?.RawJson`; akses setelah ?. hanya dilakukan bila penerimanya tidak null dalam Details.
            GameplayRaw = gameplay?.RawJson,
            // Memperbarui `GameplayDerived` menggunakan `gameplay?.DerivedJson`; akses setelah ?. hanya dilakukan bila penerimanya tidak null dalam Details.
            GameplayDerived = gameplay?.DerivedJson,
            // Memperbarui `GameplayComputedAt` menggunakan `gameplay?.ComputedAt`; akses setelah ?. hanya dilakukan bila penerimanya tidak null dalam Details.
            GameplayComputedAt = gameplay?.ComputedAt,
            // Memperbarui `GameplayErrorMessage` menggunakan `gameplayError` (nilai gameplay kesalahan) dalam Details.
            GameplayErrorMessage = gameplayError
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Details.
        });
    // Menutup scope metode Details; bagian berikut berada di luar batas blok tersebut dalam Details.
    }

    // Mendefinisikan metode `ResolvePlayerDisplayNameAsync` dengan hasil bertipe `Task<string?>`; operasi ini menangani resolve pemain display nama
    // asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `client`
    // bertipe `HttpClient` membawa nilai client; Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task<string?> ResolvePlayerDisplayNameAsync(HttpClient client, Guid playerId, CancellationToken ct)
    // Membuka scope metode ResolvePlayerDisplayNameAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ResolvePlayerDisplayNameAsync.
    {
        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
        // `client.GetAsync` dengan `”api/v1/players”`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var response = await client.GetAsync("api/v1/players", ct);
        // Memeriksa kebalikan kondisi `response.IsSuccessStatusCode`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolvePlayerDisplayNameAsync.
        if (!response.IsSuccessStatusCode)
        // Membuka scope cabang if untuk kondisi `!response.IsSuccessStatusCode`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolvePlayerDisplayNameAsync.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam ResolvePlayerDisplayNameAsync; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `!response.IsSuccessStatusCode`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolvePlayerDisplayNameAsync.
        }

        // Menyiapkan variabel lokal `players` untuk nilai pemain dengan hasil operasi asinkron memanggil
        // `response.Content.TryReadFromJsonAsync<PlayerListResponse>` dengan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var players = await response.Content.TryReadFromJsonAsync<PlayerListResponse>(cancellationToken: ct);
        // Mengembalikan `players?.Items.FirstOrDefault(item => item.UserId == playerId)?.DisplayName`; akses setelah ?. hanya dilakukan bila penerimanya
        // tidak null kepada pemanggil dalam ResolvePlayerDisplayNameAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return players?.Items.FirstOrDefault(item => item.UserId == playerId)?.DisplayName;
    // Menutup scope metode ResolvePlayerDisplayNameAsync; bagian berikut berada di luar batas blok tersebut dalam ResolvePlayerDisplayNameAsync.
    }

    // Mendefinisikan metode `BuildCashflowJourneyStats` dengan hasil bertipe `PlayerCashflowJourneyStatsViewModel`; operasi ini menangani build arus
    // kas journey stats. Masukan: Parameter `transactions` bertipe `List<TransactionHistoryItem>` membawa nilai transactions; Parameter `startingCash`
    // bertipe `double` membawa nilai starting uang tunai.
    private static PlayerCashflowJourneyStatsViewModel BuildCashflowJourneyStats(List<TransactionHistoryItem> transactions, double startingCash)
    // Membuka scope metode BuildCashflowJourneyStats; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildCashflowJourneyStats.
    {
        // Menyiapkan variabel lokal `orderedTransactions` untuk nilai ordered transactions dengan mematerialisasi urutan `transactions .OrderBy(item =>
        // item.Timestamp)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var orderedTransactions = transactions
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(item => item.Timestamp) dalam BuildCashflowJourneyStats; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderBy(item => item.Timestamp)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam BuildCashflowJourneyStats; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .ToList();

        // Menyiapkan variabel lokal `labels` untuk nilai labels dengan objek baru bertipe `List<string>` dengan argumen (orderedTransactions.Count + 1).
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var labels = new List<string>(orderedTransactions.Count + 1) { "START" };
        // Menyiapkan variabel lokal `runningBalanceSeries` untuk nilai running saldo series dengan objek baru bertipe `List<double>` dengan argumen
        // (orderedTransactions.Count + 1). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var runningBalanceSeries = new List<double>(orderedTransactions.Count + 1) { startingCash };
        // Menyiapkan variabel lokal `transactionDetails` untuk nilai transaction rincian dengan objek baru bertipe `List<string>` dengan argumen
        // (orderedTransactions.Count + 1). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var transactionDetails = new List<string>(orderedTransactions.Count + 1) { $"START - OPENING_CASH ({startingCash:N0})" };

        // Menyiapkan variabel lokal `totalCashIn` untuk nilai total uang tunai in dengan nilai literal `0d`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var totalCashIn = 0d;
        // Menyiapkan variabel lokal `totalCashOut` untuk nilai total uang tunai out dengan nilai literal `0d`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var totalCashOut = 0d;
        // Menyiapkan variabel lokal `cashInCount` untuk nilai uang tunai in jumlah dengan nilai literal `0`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var cashInCount = 0;
        // Menyiapkan variabel lokal `cashOutCount` untuk nilai uang tunai out jumlah dengan nilai literal `0`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var cashOutCount = 0;
        // Menyiapkan variabel lokal `runningBalance` untuk nilai running saldo dengan `startingCash` (nilai starting uang tunai). Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var runningBalance = startingCash;
        // Menyiapkan variabel lokal `peakRunningBalance` untuk nilai peak running saldo dengan `startingCash` (nilai starting uang tunai). Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var peakRunningBalance = startingCash;
        // Menyiapkan variabel lokal `lowestRunningBalance` untuk nilai lowest running saldo dengan `startingCash` (nilai starting uang tunai). Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var lowestRunningBalance = startingCash;

        // Mengulangi setiap elemen `orderedTransactions`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
        // BuildCashflowJourneyStats.
        foreach (var item in orderedTransactions)
        // Membuka scope loop setiap item dari `orderedTransactions`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildCashflowJourneyStats.
        {
            // Menyiapkan variabel lokal `direction` untuk nilai direction dengan `item.Direction?.Trim().ToUpperInvariant()` bila tidak null; jika null gunakan
            // `string.Empty` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var direction = item.Direction?.Trim().ToUpperInvariant() ?? string.Empty;
            // Menyiapkan variabel lokal `category` untuk nilai category dengan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(item.Category)`
            // benar gunakan `”TRANSACTION”`, jika tidak gunakan `item.Category.Trim()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var category = string.IsNullOrWhiteSpace(item.Category) ? "TRANSACTION" : item.Category.Trim();

            // Memeriksa membandingkan kesamaan `string` dengan `item.Direction`, `”IN”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti
            // overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildCashflowJourneyStats.
            if (string.Equals(item.Direction, "IN", StringComparison.OrdinalIgnoreCase))
            // Membuka scope cabang if untuk kondisi `string.Equals(item.Direction, ”IN”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut
            // berada di dalam batas blok ini dalam BuildCashflowJourneyStats.
            {
                // Memperbarui `totalCashIn` dengan menambahkan `item.Amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam
                // BuildCashflowJourneyStats.
                totalCashIn += item.Amount;
                // Memperbarui `cashInCount` dengan menambahkan nilai literal `1` dalam BuildCashflowJourneyStats.
                cashInCount += 1;
                // Memperbarui `runningBalance` dengan menambahkan `item.Amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam
                // BuildCashflowJourneyStats.
                runningBalance += item.Amount;
            // Menutup scope cabang if untuk kondisi `string.Equals(item.Direction, ”IN”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar
            // batas blok tersebut dalam BuildCashflowJourneyStats.
            }
            // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam BuildCashflowJourneyStats.
            else if (string.Equals(item.Direction, "OUT", StringComparison.OrdinalIgnoreCase))
            // Membuka scope cabang if untuk kondisi `string.Equals(item.Direction, ”OUT”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut
            // berada di dalam batas blok ini dalam BuildCashflowJourneyStats.
            {
                // Memperbarui `totalCashOut` dengan menambahkan `item.Amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam
                // BuildCashflowJourneyStats.
                totalCashOut += item.Amount;
                // Memperbarui `cashOutCount` dengan menambahkan nilai literal `1` dalam BuildCashflowJourneyStats.
                cashOutCount += 1;
                // Memperbarui `runningBalance` dengan mengurangi `item.Amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam
                // BuildCashflowJourneyStats.
                runningBalance -= item.Amount;
            // Menutup scope cabang if untuk kondisi `string.Equals(item.Direction, ”OUT”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar
            // batas blok tersebut dalam BuildCashflowJourneyStats.
            }

            // Menjalankan menambahkan `item.Timestamp.ToString(”dd/MM HH:mm”)` ke `labels` dalam BuildCashflowJourneyStats.
            labels.Add(item.Timestamp.ToString("dd/MM HH:mm"));
            // Menjalankan menambahkan `runningBalance` ke `runningBalanceSeries` dalam BuildCashflowJourneyStats.
            runningBalanceSeries.Add(runningBalance);
            // Memperbarui `peakRunningBalance` menggunakan menentukan nilai terbesar dari `peakRunningBalance`, `runningBalance` dalam
            // BuildCashflowJourneyStats.
            peakRunningBalance = Math.Max(peakRunningBalance, runningBalance);
            // Memperbarui `lowestRunningBalance` menggunakan menentukan nilai terkecil dari `lowestRunningBalance`, `runningBalance` dalam
            // BuildCashflowJourneyStats.
            lowestRunningBalance = Math.Min(lowestRunningBalance, runningBalance);
            // Menjalankan menambahkan `$”{direction} - {category} ({item.Amount:N0})”` ke `transactionDetails` dalam BuildCashflowJourneyStats.
            transactionDetails.Add($"{direction} - {category} ({item.Amount:N0})");
        // Menutup scope loop setiap item dari `orderedTransactions`; bagian berikut berada di luar batas blok tersebut dalam BuildCashflowJourneyStats.
        }

        // Mengembalikan objek baru bertipe `PlayerCashflowJourneyStatsViewModel` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam
        // BuildCashflowJourneyStats; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new PlayerCashflowJourneyStatsViewModel
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildCashflowJourneyStats.
        {
            // Memperbarui `StartingCash` menggunakan `startingCash` (nilai starting uang tunai) dalam BuildCashflowJourneyStats.
            StartingCash = startingCash,
            // Memperbarui `EndingCash` menggunakan `runningBalance` (nilai running saldo) dalam BuildCashflowJourneyStats.
            EndingCash = runningBalance,
            // Memperbarui `TransactionCount` menggunakan `orderedTransactions.Count`, yaitu jumlah elemen atau panjang data dalam BuildCashflowJourneyStats.
            TransactionCount = orderedTransactions.Count,
            // Memperbarui `CashInCount` menggunakan `cashInCount` (nilai uang tunai in jumlah) dalam BuildCashflowJourneyStats.
            CashInCount = cashInCount,
            // Memperbarui `CashOutCount` menggunakan `cashOutCount` (nilai uang tunai out jumlah) dalam BuildCashflowJourneyStats.
            CashOutCount = cashOutCount,
            // Memperbarui `TotalCashIn` menggunakan `totalCashIn` (nilai total uang tunai in) dalam BuildCashflowJourneyStats.
            TotalCashIn = totalCashIn,
            // Memperbarui `TotalCashOut` menggunakan `totalCashOut` (nilai total uang tunai out) dalam BuildCashflowJourneyStats.
            TotalCashOut = totalCashOut,
            // Memperbarui `NetCashflow` menggunakan selisih antara `totalCashIn` dan `totalCashOut` dalam BuildCashflowJourneyStats.
            NetCashflow = totalCashIn - totalCashOut,
            // Memperbarui `PeakRunningNet` menggunakan `peakRunningBalance` (nilai peak running saldo) dalam BuildCashflowJourneyStats.
            PeakRunningNet = peakRunningBalance,
            // Memperbarui `LowestRunningNet` menggunakan `lowestRunningBalance` (nilai lowest running saldo) dalam BuildCashflowJourneyStats.
            LowestRunningNet = lowestRunningBalance,
            // Memperbarui `FirstTransactionAt` menggunakan hasil pemilihan bersyarat: ketika `orderedTransactions.Count > 0` benar gunakan
            // `orderedTransactions.First().Timestamp`, jika tidak gunakan `null` dalam BuildCashflowJourneyStats.
            FirstTransactionAt = orderedTransactions.Count > 0 ? orderedTransactions.First().Timestamp : null,
            // Memperbarui `LastTransactionAt` menggunakan hasil pemilihan bersyarat: ketika `orderedTransactions.Count > 0` benar gunakan
            // `orderedTransactions.Last().Timestamp`, jika tidak gunakan `null` dalam BuildCashflowJourneyStats.
            LastTransactionAt = orderedTransactions.Count > 0 ? orderedTransactions.Last().Timestamp : null,
            // Memperbarui `TimelineLabels` menggunakan `labels` (nilai labels) dalam BuildCashflowJourneyStats.
            TimelineLabels = labels,
            // Memperbarui `RunningNetSeries` menggunakan `runningBalanceSeries` (nilai running saldo series) dalam BuildCashflowJourneyStats.
            RunningNetSeries = runningBalanceSeries,
            // Memperbarui `TransactionDetails` menggunakan `transactionDetails` (nilai transaction rincian) dalam BuildCashflowJourneyStats.
            TransactionDetails = transactionDetails
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildCashflowJourneyStats.
        };
    // Menutup scope metode BuildCashflowJourneyStats; bagian berikut berada di luar batas blok tersebut dalam BuildCashflowJourneyStats.
    }

    // Mendefinisikan metode `InferDefaultStartingCash` dengan hasil bertipe `double`; operasi ini menangani infer bawaan starting uang tunai. Masukan:
    // Parameter `rulesetName` bertipe `string?` membawa nilai aturan nama; nilai null diizinkan ketika data opsional belum tersedia.
    private static double InferDefaultStartingCash(string? rulesetName)
    // Membuka scope metode InferDefaultStartingCash; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam InferDefaultStartingCash.
    {
        // Memeriksa memeriksa apakah `rulesetName` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam InferDefaultStartingCash.
        if (string.IsNullOrWhiteSpace(rulesetName))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(rulesetName)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // InferDefaultStartingCash.
        {
            // Mengembalikan nilai literal `20d` kepada pemanggil dalam InferDefaultStartingCash; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return 20d;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(rulesetName)`; bagian berikut berada di luar batas blok tersebut dalam
        // InferDefaultStartingCash.
        }

        // Menyiapkan variabel lokal `normalized` untuk nilai normalized dengan menormalisasi `rulesetName.Trim()` menjadi huruf kecil dengan aturan kultur
        // invariant. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var normalized = rulesetName.Trim().ToLowerInvariant();
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `normalized.Contains(”mahir”, StringComparison.Ordinal)` dan
        // `normalized.Contains(”advanced”, StringComparison.Ordinal)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam InferDefaultStartingCash.
        if (normalized.Contains("mahir", StringComparison.Ordinal) ||
            // Melanjutkan pengolahan dengan memeriksa apakah `normalized` memuat `”advanced”`, `StringComparison.Ordinal` dalam InferDefaultStartingCash.
            normalized.Contains("advanced", StringComparison.Ordinal))
        // Membuka scope cabang if untuk kondisi `normalized.Contains(”mahir”, StringComparison.Ordinal) || normalized.Contains(”advanced”,
        // StringComparison.Ordinal)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam InferDefaultStartingCash.
        {
            // Mengembalikan nilai literal `10d` kepada pemanggil dalam InferDefaultStartingCash; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return 10d;
        // Menutup scope cabang if untuk kondisi `normalized.Contains(”mahir”, StringComparison.Ordinal) || normalized.Contains(”advanced”,
        // StringComparison.Ordinal)`; bagian berikut berada di luar batas blok tersebut dalam InferDefaultStartingCash.
        }

        // Mengembalikan nilai literal `20d` kepada pemanggil dalam InferDefaultStartingCash; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return 20d;
    // Menutup scope metode InferDefaultStartingCash; bagian berikut berada di luar batas blok tersebut dalam InferDefaultStartingCash.
    }
// Menutup scope tipe PlayersController; bagian berikut berada di luar batas blok tersebut.
}
