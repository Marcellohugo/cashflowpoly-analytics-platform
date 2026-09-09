// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk PlayerDirectoryController.
// Mengimpor namespace `System.Net.Http.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Json;
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

// menetapkan pola rute (”players”) untuk pencocokan URL permintaan.
[Route("players")]
// Mendefinisikan tipe class `PlayerDirectoryController` yang mewarisi atau menerapkan `Controller`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerDirectoryController : Controller
// Membuka scope tipe PlayerDirectoryController; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `IHttpClientFactory`: `_clientFactory` menyimpan nilai client factory. readonly membatasi penggantian
    // referensi/nilai field pada deklarasi atau konstruktor.
    private readonly IHttpClientFactory _clientFactory;

    // Mendefinisikan konstruktor PlayerDirectoryController yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter:
    // Parameter `clientFactory` bertipe `IHttpClientFactory` membawa nilai client factory.
    public PlayerDirectoryController(IHttpClientFactory clientFactory)
    // Membuka scope konstruktor PlayerDirectoryController; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam PlayerDirectoryController.
    {
        // Memperbarui `_clientFactory` menggunakan `clientFactory` (nilai client factory) dalam PlayerDirectoryController.
        _clientFactory = clientFactory;
    // Menutup scope konstruktor PlayerDirectoryController; bagian berikut berada di luar batas blok tersebut dalam PlayerDirectoryController.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (””).
    [HttpGet("")]
    // Mendefinisikan metode `Index` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani index. async memungkinkan metode menunggu operasi
    // I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar
    // operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> Index(CancellationToken ct)
    // Membuka scope metode Index; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
    {
        // Menyiapkan variabel lokal `client` untuk nilai client dengan memanggil `_clientFactory.CreateClient` dengan `”Api”`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var client = _clientFactory.CreateClient("Api");
        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
        // `client.GetAsync` dengan `”api/v1/players”`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var response = await client.GetAsync(HttpContext.IsInstructor()
            ? "api/v1/players?inMySessions=true"
            : "api/v1/players", ct);
        // Menyiapkan variabel lokal `unauthorized` untuk nilai unauthorized dengan memanggil `this.HandleUnauthorizedApiResponse` dengan `response`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var unauthorized = this.HandleUnauthorizedApiResponse(response);
        // Memeriksa hasil pencocokan `unauthorized` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Index.
        if (unauthorized is not null)
        // Membuka scope cabang if untuk kondisi `unauthorized is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
        {
            // Mengembalikan `unauthorized` (nilai unauthorized) kepada pemanggil dalam Index; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return unauthorized;
        // Menutup scope cabang if untuk kondisi `unauthorized is not null`; bagian berikut berada di luar batas blok tersebut dalam Index.
        }

        // Memeriksa kebalikan kondisi `response.IsSuccessStatusCode`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Index.
        if (!response.IsSuccessStatusCode)
        // Membuka scope cabang if untuk kondisi `!response.IsSuccessStatusCode`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
        {
            // Mengembalikan menyiapkan tampilan Razor dengan `”~/Views/Players/Index.cshtml”`, `new PlayerDirectoryViewModel { ErrorMessage = HttpContext
            // .T(”players.error.load_player_directory_failed”) .Replace(”{status}”, ((int)response.StatusCode).ToString()) }` sebagai nama tampilan atau
            // modelnya kepada pemanggil dalam Index; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return View("~/Views/Players/Index.cshtml", new PlayerDirectoryViewModel
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
            {
                // Memperbarui `ErrorMessage` menggunakan memanggil `HttpContext .T(”players.error.load_player_directory_failed”) .Replace` dengan `”{status}”`,
                // `((int)response.StatusCode).ToString()` dalam Index.
                ErrorMessage = HttpContext
                    // Meneruskan nilai literal `”players.error.load_player_directory_failed”` sebagai argumen ke `HttpContext .T`.
                    .T("players.error.load_player_directory_failed")
                    // Meneruskan nilai literal `”{status}”` sebagai argumen ke `HttpContext .T(”players.error.load_player_directory_failed”) .Replace`; Meneruskan
                    // mengubah `((int)response.StatusCode)` menjadi teks sebagai argumen ke `HttpContext .T(”players.error.load_player_directory_failed”) .Replace`.
                    .Replace("{status}", ((int)response.StatusCode).ToString())
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Index.
            });
        // Menutup scope cabang if untuk kondisi `!response.IsSuccessStatusCode`; bagian berikut berada di luar batas blok tersebut dalam Index.
        }

        // Menyiapkan variabel lokal `playerData` untuk nilai pemain data dengan hasil operasi asinkron memanggil
        // `response.Content.TryReadFromJsonAsync<PlayerListResponse>` dengan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerData = await response.Content.TryReadFromJsonAsync<PlayerListResponse>(cancellationToken: ct);
        // Menyiapkan variabel lokal `players` untuk nilai pemain dengan `playerData?.Items` bila tidak null; jika null gunakan `new List<PlayerResponse>()`
        // sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var players = playerData?.Items ?? new List<PlayerResponse>();

        // Menyiapkan variabel lokal `sessionsResponse` untuk nilai sessions respons dengan hasil operasi asinkron memanggil `client.GetAsync` dengan
        // `”api/v1/sessions”`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var sessionsResponse = await client.GetAsync("api/v1/sessions", ct);
        // Memperbarui `unauthorized` menggunakan memanggil `this.HandleUnauthorizedApiResponse` dengan `sessionsResponse` dalam Index.
        unauthorized = this.HandleUnauthorizedApiResponse(sessionsResponse);
        // Memeriksa hasil pencocokan `unauthorized` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Index.
        if (unauthorized is not null)
        // Membuka scope cabang if untuk kondisi `unauthorized is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
        {
            // Mengembalikan `unauthorized` (nilai unauthorized) kepada pemanggil dalam Index; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return unauthorized;
        // Menutup scope cabang if untuk kondisi `unauthorized is not null`; bagian berikut berada di luar batas blok tersebut dalam Index.
        }

        // Menyiapkan variabel lokal `groups` untuk nilai groups dengan objek baru bertipe `List<PlayerSessionGroupViewModel>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var groups = new List<PlayerSessionGroupViewModel>();
        // Menyiapkan variabel lokal `groupError` untuk nilai group kesalahan dengan null, yaitu penanda tidak ada nilai. Tipe yang dipakai adalah
        // `string?`.
        string? groupError = null;

        // Memeriksa `sessionsResponse.IsSuccessStatusCode` (nilai berstatus success status kode); blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam Index.
        if (sessionsResponse.IsSuccessStatusCode)
        // Membuka scope cabang if untuk kondisi `sessionsResponse.IsSuccessStatusCode`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Index.
        {
            // Menyiapkan variabel lokal `sessionsData` untuk nilai sessions data dengan hasil operasi asinkron memanggil
            // `sessionsResponse.Content.TryReadFromJsonAsync<SessionListResponse>` dengan `ct`; await menunggu hasil tanpa memblokir thread selama operasi
            // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var sessionsData = await sessionsResponse.Content.TryReadFromJsonAsync<SessionListResponse>(cancellationToken: ct);
            // Menyiapkan variabel lokal `sessions` untuk nilai sessions dengan `sessionsData?.Items` bila tidak null; jika null gunakan `new
            // List<SessionListItem>()` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var sessions = sessionsData?.Items ?? new List<SessionListItem>();
            // Menyiapkan variabel lokal `playerNames` untuk nilai pemain nama dengan membangun kamus dari `players` dengan pemilihan kunci/nilai `player =>
            // player.UserId`, `player => player.DisplayName`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var playerNames = players.ToDictionary(player => player.UserId, player => player.DisplayName);
            // Menyiapkan variabel lokal `requestGate` untuk nilai permintaan gate dengan objek baru bertipe `SemaphoreSlim` dengan argumen (8). Tipe variabel
            // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var requestGate = new SemaphoreSlim(8);
            // Menyiapkan variabel lokal `sessionTasks` untuk nilai sesi tasks dengan memetakan setiap elemen `sessions` melalui `async session => { await
            // requestGate.WaitAsync(ct); try { if (!string.Equals(session.Status, ”ENDED”, StringComparison.OrdinalIgnoreCase)) { var
            // activeParticipantsResponse = a...` menjadi bentuk hasil yang dibutuhkan. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var sessionTasks = sessions.Select(async session =>
            // Membuka scope fungsi lambda yang dipasok ke `sessions.Select`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
            {
                // Menjalankan hasil operasi asinkron memanggil `requestGate.WaitAsync` dengan `ct`; await menunggu hasil tanpa memblokir thread selama operasi
                // belum selesai dalam Index.
                await requestGate.WaitAsync(ct);
                // Memulai blok try dalam Index; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat keluar.
                try
                // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
                {
                    // Memeriksa kebalikan kondisi `string.Equals(session.Status, ”ENDED”, StringComparison.OrdinalIgnoreCase)`; blok if hanya dijalankan ketika kondisi
                    // ini bernilai benar dalam Index.
                    if (!string.Equals(session.Status, "ENDED", StringComparison.OrdinalIgnoreCase))
                    // Membuka scope cabang if untuk kondisi `!string.Equals(session.Status, ”ENDED”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut
                    // berada di dalam batas blok ini dalam Index.
                    {
                        // Menyiapkan variabel lokal `activeParticipantsResponse` untuk nilai aktif participants respons dengan hasil operasi asinkron memanggil
                        // `client.GetAsync` dengan `$”api/v1/sessions/{session.SessionId}/players”`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum
                        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
                        var activeParticipantsResponse = await client.GetAsync($"api/v1/sessions/{session.SessionId}/players", ct);
                        // Memeriksa kebalikan kondisi `activeParticipantsResponse.IsSuccessStatusCode`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                        // Index.
                        if (!activeParticipantsResponse.IsSuccessStatusCode)
                        // Membuka scope cabang if untuk kondisi `!activeParticipantsResponse.IsSuccessStatusCode`; pernyataan/deklarasi berikut berada di dalam batas blok
                        // ini dalam Index.
                        {
                            // Mengembalikan tuple yang membawa bagian 1: session; bagian 2: (SessionPlayerListResponse?)null; bagian 3: (AnalyticsSessionResponse?)null kepada
                            // pemanggil dalam Index; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                            return (session, participants: (SessionPlayerListResponse?)null, analytics: (AnalyticsSessionResponse?)null);
                        // Menutup scope cabang if untuk kondisi `!activeParticipantsResponse.IsSuccessStatusCode`; bagian berikut berada di luar batas blok tersebut dalam
                        // Index.
                        }

                        // Menyiapkan variabel lokal `activeParticipants` untuk nilai aktif participants dengan hasil operasi asinkron memanggil
                        // `activeParticipantsResponse.Content.TryReadFromJsonAsync<SessionPlayerListResponse>` dengan `ct`; await menunggu hasil tanpa memblokir thread
                        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
                        var activeParticipants = await activeParticipantsResponse.Content.TryReadFromJsonAsync<SessionPlayerListResponse>(cancellationToken: ct);
                        // Mengembalikan tuple yang membawa bagian 1: session; bagian 2: activeParticipants; bagian 3: (AnalyticsSessionResponse?)null kepada pemanggil
                        // dalam Index; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                        return (session, participants: activeParticipants, analytics: (AnalyticsSessionResponse?)null);
                    // Menutup scope cabang if untuk kondisi `!string.Equals(session.Status, ”ENDED”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di
                    // luar batas blok tersebut dalam Index.
                    }

                    // Menyiapkan variabel lokal `analyticsResponse` untuk nilai analytics respons dengan hasil operasi asinkron memanggil `client.GetAsync` dengan
                    // `$”api/v1/analytics/sessions/{session.SessionId}”`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
                    // disimpulkan dari ekspresi nilai awal.
                    var analyticsResponse = await client.GetAsync($"api/v1/analytics/sessions/{session.SessionId}", ct);
                    // Memeriksa `analyticsResponse.IsSuccessStatusCode` (nilai berstatus success status kode); blok if hanya dijalankan ketika kondisi ini bernilai
                    // benar dalam Index.
                    if (analyticsResponse.IsSuccessStatusCode)
                    // Membuka scope cabang if untuk kondisi `analyticsResponse.IsSuccessStatusCode`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // Index.
                    {
                        // Menyiapkan variabel lokal `analytics` untuk nilai analytics dengan hasil operasi asinkron memanggil
                        // `analyticsResponse.Content.TryReadFromJsonAsync<AnalyticsSessionResponse>` dengan `ct`; await menunggu hasil tanpa memblokir thread selama
                        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
                        var analytics = await analyticsResponse.Content.TryReadFromJsonAsync<AnalyticsSessionResponse>(cancellationToken: ct);
                        // Memeriksa hasil pencocokan `analytics` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Index.
                        if (analytics is not null)
                        // Membuka scope cabang if untuk kondisi `analytics is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
                        {
                            // Menyiapkan variabel lokal `analyticsParticipants` untuk nilai analytics participants dengan objek baru bertipe `SessionPlayerListResponse` dengan
                            // argumen (analytics.ByPlayer .Select(player => new SessionPlayerResponse( player.UserId, playerNames.GetValueOrDefault(player.UserId,
                            // string.Empty), player.PlayerOrder)).... Tipe variabel disimpulkan dari ekspresi nilai awal.
                            var analyticsParticipants = new SessionPlayerListResponse(analytics.ByPlayer
                                // Meneruskan fungsi lambda `player => new SessionPlayerResponse( player.UserId, playerNames.GetValueOrDefault(player.UserId, string.Empty),
                                // player.PlayerOrder)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `analytics.ByPlayer .Select`.
                                .Select(player => new SessionPlayerResponse(
                                    // Meneruskan `player.UserId` (identitas akun pengguna yang datanya sedang diproses) sebagai argumen ke konstruktor `SessionPlayerResponse`.
                                    player.UserId,
                                    // Meneruskan membaca `playerNames` memakai `player.UserId`, `string.Empty`; nilai bawaan digunakan ketika nilai atau kunci tidak tersedia sebagai
                                    // argumen ke konstruktor `SessionPlayerResponse`; Meneruskan `player.UserId` (identitas akun pengguna yang datanya sedang diproses) sebagai argumen
                                    // ke `playerNames.GetValueOrDefault`; Meneruskan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai argumen ke
                                    // `playerNames.GetValueOrDefault`.
                                    playerNames.GetValueOrDefault(player.UserId, string.Empty),
                                    // Meneruskan `player.PlayerOrder` (nomor urut pemain untuk menentukan urutan tindakan) sebagai argumen ke konstruktor `SessionPlayerResponse`.
                                    player.PlayerOrder))
                                // Meneruskan mematerialisasi urutan `analytics.ByPlayer .Select(player => new SessionPlayerResponse( player.UserId,
                                // playerNames.GetValueOrDefault(player.UserId, string.Empty), player.PlayerOrder))` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam
                                // memori sebagai argumen ke konstruktor `SessionPlayerListResponse`.
                                .ToList());
                            // Mengembalikan tuple yang membawa bagian 1: session; bagian 2: analyticsParticipants; bagian 3: analytics kepada pemanggil dalam Index; eksekusi
                            // jalur ini selesai setelah nilai hasil ditentukan.
                            return (session, participants: analyticsParticipants, analytics);
                        // Menutup scope cabang if untuk kondisi `analytics is not null`; bagian berikut berada di luar batas blok tersebut dalam Index.
                        }
                    // Menutup scope cabang if untuk kondisi `analyticsResponse.IsSuccessStatusCode`; bagian berikut berada di luar batas blok tersebut dalam Index.
                    }

                    // Menyiapkan variabel lokal `participantsResponse` untuk nilai participants respons dengan hasil operasi asinkron memanggil `client.GetAsync`
                    // dengan `$”api/v1/sessions/{session.SessionId}/players”`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
                    // variabel disimpulkan dari ekspresi nilai awal.
                    var participantsResponse = await client.GetAsync($"api/v1/sessions/{session.SessionId}/players", ct);
                    // Memeriksa kebalikan kondisi `participantsResponse.IsSuccessStatusCode`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Index.
                    if (!participantsResponse.IsSuccessStatusCode)
                    // Membuka scope cabang if untuk kondisi `!participantsResponse.IsSuccessStatusCode`; pernyataan/deklarasi berikut berada di dalam batas blok ini
                    // dalam Index.
                    {
                        // Mengembalikan tuple yang membawa bagian 1: session; bagian 2: (SessionPlayerListResponse?)null; bagian 3: (AnalyticsSessionResponse?)null kepada
                        // pemanggil dalam Index; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                        return (session, participants: (SessionPlayerListResponse?)null, analytics: (AnalyticsSessionResponse?)null);
                    // Menutup scope cabang if untuk kondisi `!participantsResponse.IsSuccessStatusCode`; bagian berikut berada di luar batas blok tersebut dalam Index.
                    }

                    // Menyiapkan variabel lokal `participants` untuk nilai participants dengan hasil operasi asinkron memanggil
                    // `participantsResponse.Content.TryReadFromJsonAsync<SessionPlayerListResponse>` dengan `ct`; await menunggu hasil tanpa memblokir thread selama
                    // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
                    var participants = await participantsResponse.Content.TryReadFromJsonAsync<SessionPlayerListResponse>(cancellationToken: ct);
                    // Mengembalikan tuple yang membawa bagian 1: session; bagian 2: participants; bagian 3: (AnalyticsSessionResponse?)null kepada pemanggil dalam
                    // Index; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                    return (session, participants, analytics: (AnalyticsSessionResponse?)null);
                // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam Index.
                }
                // Menangani exception `HttpRequestException` melalui variabel dalam Index.
                catch (HttpRequestException)
                // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
                {
                    // Mengembalikan tuple yang membawa bagian 1: session; bagian 2: (SessionPlayerListResponse?)null; bagian 3: (AnalyticsSessionResponse?)null kepada
                    // pemanggil dalam Index; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                    return (session, participants: (SessionPlayerListResponse?)null, analytics: (AnalyticsSessionResponse?)null);
                // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam Index.
                }
                // Menangani exception `TaskCanceledException` melalui variabel hanya jika filter `!ct.IsCancellationRequested` terpenuhi dalam Index.
                catch (TaskCanceledException) when (!ct.IsCancellationRequested)
                // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
                {
                    // Mengembalikan tuple yang membawa bagian 1: session; bagian 2: (SessionPlayerListResponse?)null; bagian 3: (AnalyticsSessionResponse?)null kepada
                    // pemanggil dalam Index; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                    return (session, participants: (SessionPlayerListResponse?)null, analytics: (AnalyticsSessionResponse?)null);
                // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam Index.
                }
                // Menjalankan blok finally ketika alur meninggalkan try/catch, termasuk saat terjadi exception dalam Index; bagian ini dipakai untuk pekerjaan
                // penutup yang harus tetap dilakukan.
                finally
                // Membuka scope pekerjaan penutup finally; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
                {
                    // Menjalankan memanggil `requestGate.Release` dengan tanpa argumen dalam Index.
                    requestGate.Release();
                // Menutup scope pekerjaan penutup finally; bagian berikut berada di luar batas blok tersebut dalam Index.
                }
            // Menutup scope fungsi lambda yang dipasok ke `sessions.Select`; bagian berikut berada di luar batas blok tersebut dalam Index.
            });

            // Menyiapkan variabel lokal `sessionResults` untuk nilai sesi results dengan hasil operasi asinkron memanggil `Task.WhenAll` dengan `sessionTasks`;
            // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var sessionResults = await Task.WhenAll(sessionTasks);
            // Memeriksa memeriksa apakah `sessionResults` memiliki setidaknya satu elemen yang memenuhi `result => result.participants is null ||
            // (string.Equals(result.session.Status, ”ENDED”, StringComparison.OrdinalIgnoreCase) && result.analytics is null)`; blok if hanya dijalankan ketika
            // kondisi ini bernilai benar dalam Index.
            if (sessionResults.Any(result =>
                    // Meneruskan fungsi lambda `result => result.participants is null || (string.Equals(result.session.Status, ”ENDED”,
                    // StringComparison.OrdinalIgnoreCase) && result.analytics is null)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                    // argumen ke `sessionResults.Any`.
                    result.participants is null ||
                    // Meneruskan `result.session.Status` (nilai status) sebagai argumen ke `string.Equals`; Meneruskan nilai literal `”ENDED”` sebagai argumen ke
                    // `string.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `string.Equals`.
                    (string.Equals(result.session.Status, "ENDED", StringComparison.OrdinalIgnoreCase) && result.analytics is null)))
            // Membuka scope cabang if untuk kondisi `sessionResults.Any(result => result.participants is null || (string.Equals(result.session.Status, ”ENDED”,
            // StringComparison.OrdinalIgnoreCase) && result.analytics is null))`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
            {
                // Memperbarui `groupError` menggunakan memanggil `HttpContext.T` dengan `”players.error.load_session_details_partial”` dalam Index.
                groupError = HttpContext.T("players.error.load_session_details_partial");
            // Menutup scope cabang if untuk kondisi `sessionResults.Any(result => result.participants is null || (string.Equals(result.session.Status, ”ENDED”,
            // StringComparison.OrdinalIgnoreCase) && result.analytics is null))`; bagian berikut berada di luar batas blok tersebut dalam Index.
            }

            // Memperbarui `groups` menggunakan mematerialisasi urutan `sessionResults .Where(x => x.participants is not null && x.participants.Items.Count > 0)
            // .Select(x => new PlayerSessionGroupViewModel { SessionId = x.session.SessionId, Sessio...` menjadi List; enumerasi dijalankan dan hasilnya
            // disimpan dalam memori dalam Index.
            groups = sessionResults
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(x => x.participants is not null && x.participants.Items.Count > 0) dalam
                // Index; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Where(x => x.participants is not null && x.participants.Items.Count > 0)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(x => new PlayerSessionGroupViewModel dalam Index; token pada baris ini
                // menyambungkan bagian kode sebelum dan sesudahnya.
                .Select(x => new PlayerSessionGroupViewModel
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
                {
                    // Memperbarui `SessionId` menggunakan `x.session.SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam Index.
                    SessionId = x.session.SessionId,
                    // Memperbarui `SessionName` menggunakan `x.session.SessionName` (nilai sesi nama) dalam Index.
                    SessionName = x.session.SessionName,
                    // Memperbarui `Status` menggunakan `x.session.Status` (nilai status) dalam Index.
                    Status = x.session.Status,
                    // Memperbarui `StartedAt` menggunakan `x.session.StartedAt` (nilai started at) dalam Index.
                    StartedAt = x.session.StartedAt,
                    // Memperbarui `EndedAt` menggunakan `x.session.EndedAt` (nilai ended at) dalam Index.
                    EndedAt = x.session.EndedAt,
                    // Memperbarui `Players` menggunakan mematerialisasi urutan `x.participants!.Items .OrderBy(p => p.PlayerOrder > 0 ? p.PlayerOrder : int.MaxValue)
                    // .ThenBy(p => p.UserId) .Select((p, index) => { var analytics = x.analytics?.ByPlayer.Firs...` menjadi List; enumerasi dijalankan dan hasilnya
                    // disimpan dalam memori dalam Index.
                    Players = x.participants!.Items
                        // Meneruskan fungsi lambda `p => p.PlayerOrder > 0 ? p.PlayerOrder : int.MaxValue` yang dijalankan oleh operasi pemanggil untuk memproses setiap
                        // masukan sebagai argumen ke `x.participants!.Items .OrderBy`.
                        .OrderBy(p => p.PlayerOrder > 0 ? p.PlayerOrder : int.MaxValue)
                        // Meneruskan fungsi lambda `p => p.UserId` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                        // `x.participants!.Items .OrderBy(p => p.PlayerOrder > 0 ? p.PlayerOrder : int.MaxValue) .ThenBy`.
                        .ThenBy(p => p.UserId)
                        // Meneruskan fungsi lambda `(p, index) => { var analytics = x.analytics?.ByPlayer.FirstOrDefault(item => item.UserId == p.UserId); var leaderboard
                        // = x.analytics?.Leaderboard?.FirstOrDefault(item => item....` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                        // argumen ke `x.participants!.Items .OrderBy(p => p.PlayerOrder > 0 ? p.PlayerOrder : int.MaxValue) .ThenBy(p => p.UserId) .Select`.
                        .Select((p, index) =>
                        // Membuka scope fungsi lambda yang dipasok ke `x.participants!.Items .OrderBy(p => p.PlayerOrder > 0 ? p.PlayerOrder : int.MaxValue) .ThenBy(p =>
                        // p.UserId) .Select`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
                        {
                            // Menyiapkan variabel lokal `analytics` untuk nilai analytics dengan `x.analytics?.ByPlayer.FirstOrDefault(item => item.UserId == p.UserId)`; akses
                            // setelah ?. hanya dilakukan bila penerimanya tidak null. Tipe variabel disimpulkan dari ekspresi nilai awal.
                            var analytics = x.analytics?.ByPlayer.FirstOrDefault(item => item.UserId == p.UserId);
                            // Menyiapkan variabel lokal `leaderboard` untuk nilai leaderboard dengan `x.analytics?.Leaderboard?.FirstOrDefault(item => item.UserId ==
                            // p.UserId)`; akses setelah ?. hanya dilakukan bila penerimanya tidak null. Tipe variabel disimpulkan dari ekspresi nilai awal.
                            var leaderboard = x.analytics?.Leaderboard?.FirstOrDefault(item => item.UserId == p.UserId);
                            // Mengembalikan objek baru bertipe `PlayerSessionEntryViewModel` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam Index; eksekusi
                            // jalur ini selesai setelah nilai hasil ditentukan.
                            return new PlayerSessionEntryViewModel
                            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
                            {
                                // Memperbarui `PlayerId` menggunakan `p.UserId` (identitas akun pengguna yang datanya sedang diproses) dalam Index.
                                PlayerId = p.UserId,
                                // Memperbarui `PlayerOrder` menggunakan hasil pemilihan bersyarat: ketika `p.PlayerOrder > 0` benar gunakan `p.PlayerOrder`, jika tidak gunakan
                                // `index + 1` dalam Index.
                                PlayerOrder = p.PlayerOrder > 0 ? p.PlayerOrder : index + 1,
                                // Memperbarui `FinalRank` menggunakan `leaderboard?.Rank` bila tidak null; jika null gunakan `0` sebagai nilai pengganti dalam Index.
                                FinalRank = leaderboard?.Rank ?? 0,
                                // Memperbarui `DisplayName` menggunakan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(p.DisplayName)` benar gunakan
                                // `$”{HttpContext.T(”common.player”)} {(p.PlayerOrder > 0 ? p.PlayerOrder : index + 1)}”`, jika tidak gunakan `p.DisplayName` dalam Index.
                                DisplayName = string.IsNullOrWhiteSpace(p.DisplayName)
                                    // Meneruskan nilai literal `”common.player”` sebagai argumen ke `HttpContext.T`.
                                    ? $"{HttpContext.T("common.player")} {(p.PlayerOrder > 0 ? p.PlayerOrder : index + 1)}"
                                    // Meneruskan fungsi lambda `(p, index) => { var analytics = x.analytics?.ByPlayer.FirstOrDefault(item => item.UserId == p.UserId); var leaderboard
                                    // = x.analytics?.Leaderboard?.FirstOrDefault(item => item....` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                                    // argumen ke `x.participants!.Items .OrderBy(p => p.PlayerOrder > 0 ? p.PlayerOrder : int.MaxValue) .ThenBy(p => p.UserId) .Select`.
                                    : p.DisplayName,
                                // Memperbarui `CashInTotal` menggunakan `analytics?.CashInTotal` bila tidak null; jika null gunakan `0` sebagai nilai pengganti dalam Index.
                                CashInTotal = analytics?.CashInTotal ?? 0,
                                // Memperbarui `CashOutTotal` menggunakan `analytics?.CashOutTotal` bila tidak null; jika null gunakan `0` sebagai nilai pengganti dalam Index.
                                CashOutTotal = analytics?.CashOutTotal ?? 0,
                                // Memperbarui `DonationTotal` menggunakan `analytics?.DonationTotal` bila tidak null; jika null gunakan `0` sebagai nilai pengganti dalam Index.
                                DonationTotal = analytics?.DonationTotal ?? 0,
                                // Memperbarui `DonationPointsTotal` menggunakan `analytics?.DonationPointsTotal` bila tidak null; jika null gunakan `0` sebagai nilai pengganti
                                // dalam Index.
                                DonationPointsTotal = analytics?.DonationPointsTotal ?? 0,
                                // Memperbarui `PensionPointsTotal` menggunakan `analytics?.PensionPointsTotal` bila tidak null; jika null gunakan `0` sebagai nilai pengganti dalam
                                // Index.
                                PensionPointsTotal = analytics?.PensionPointsTotal ?? 0,
                                // Memperbarui `GoldQty` menggunakan `analytics?.GoldQty` bila tidak null; jika null gunakan `0` sebagai nilai pengganti dalam Index.
                                GoldQty = analytics?.GoldQty ?? 0,
                                // Memperbarui `HappinessPointsTotal` menggunakan `leaderboard?.HappinessPointsTotal` bila tidak null; jika null gunakan
                                // `analytics?.HappinessPointsTotal ?? 0` sebagai nilai pengganti dalam Index.
                                HappinessPointsTotal = leaderboard?.HappinessPointsTotal ?? analytics?.HappinessPointsTotal ?? 0
                            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Index.
                            };
                        // Menutup scope fungsi lambda yang dipasok ke `x.participants!.Items .OrderBy(p => p.PlayerOrder > 0 ? p.PlayerOrder : int.MaxValue) .ThenBy(p =>
                        // p.UserId) .Select`; bagian berikut berada di luar batas blok tersebut dalam Index.
                        })
                        // Meneruskan fungsi lambda `x => new PlayerSessionGroupViewModel { SessionId = x.session.SessionId, SessionName = x.session.SessionName, Status =
                        // x.session.Status, StartedAt = x.session.StartedAt, EndedA...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                        // argumen ke `sessionResults .Where(x => x.participants is not null && x.participants.Items.Count > 0) .Select`.
                        .ToList()
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Index.
                })
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderByDescending(x => x.StartedAt ?? x.EndedAt ?? DateTimeOffset.MinValue)
                // dalam Index; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .OrderByDescending(x => x.StartedAt ?? x.EndedAt ?? DateTimeOffset.MinValue)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenBy(x => x.SessionName) dalam Index; token pada baris ini menyambungkan
                // bagian kode sebelum dan sesudahnya.
                .ThenBy(x => x.SessionName)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam Index; token pada baris ini menyambungkan bagian kode sebelum
                // dan sesudahnya.
                .ToList();
        // Menutup scope cabang if untuk kondisi `sessionsResponse.IsSuccessStatusCode`; bagian berikut berada di luar batas blok tersebut dalam Index.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam Index.
        else
        // Membuka scope cabang else; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
        {
            // Memperbarui `groupError` menggunakan memanggil `HttpContext .T(”players.error.load_sessions_grouping_failed”) .Replace` dengan `”{status}”`,
            // `((int)sessionsResponse.StatusCode).ToString()` dalam Index.
            groupError = HttpContext
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .T(”players.error.load_sessions_grouping_failed”) dalam Index; token pada baris
                // ini menyambungkan bagian kode sebelum dan sesudahnya.
                .T("players.error.load_sessions_grouping_failed")
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Replace(”{status}”, ((int)sessionsResponse.StatusCode).ToString()); dalam
                // Index; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Replace("{status}", ((int)sessionsResponse.StatusCode).ToString());
        // Menutup scope cabang else; bagian berikut berada di luar batas blok tersebut dalam Index.
        }

        // Mengembalikan menyiapkan tampilan Razor dengan `”~/Views/Players/Index.cshtml”`, `new PlayerDirectoryViewModel { Players = players, SessionGroups
        // = groups, ErrorMessage = groupError }` sebagai nama tampilan atau modelnya kepada pemanggil dalam Index; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return View("~/Views/Players/Index.cshtml", new PlayerDirectoryViewModel
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Index.
        {
            // Memperbarui `Players` menggunakan `players` (nilai pemain) dalam Index.
            Players = players,
            // Memperbarui `SessionGroups` menggunakan `groups` (nilai groups) dalam Index.
            SessionGroups = groups,
            // Memperbarui `ErrorMessage` menggunakan `groupError` (nilai group kesalahan) dalam Index.
            ErrorMessage = groupError
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Index.
        });
    // Menutup scope metode Index; bagian berikut berada di luar batas blok tersebut dalam Index.
    }
// Menutup scope tipe PlayerDirectoryController; bagian berikut berada di luar batas blok tersebut.
}
