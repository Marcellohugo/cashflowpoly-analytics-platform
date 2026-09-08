// Fungsi file: Menangani endpoint, validasi akses, dan response HTTP untuk EventsController.
// Mengimpor namespace `Cashflowpoly.Api.Services` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Services;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Authorization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Authorization;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Controllers` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Controllers;

// mengaktifkan perilaku API controller, termasuk inferensi binding dan respons otomatis atas model tidak valid.
[ApiController]
// menetapkan pola rute (”api/v1”) untuk pencocokan URL permintaan.
[Route("api/v1")]
// mewajibkan otorisasi pengguna sesuai kebijakan autentikasi aplikasi.
[Authorize]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
// Mendefinisikan tipe class `EventsController` yang mewarisi atau menerapkan `ControllerBase`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventsController : ControllerBase
// Membuka scope tipe EventsController; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `IEventIngestionService`: `_ingestion` menyimpan nilai ingestion. readonly membatasi penggantian referensi/nilai
    // field pada deklarasi atau konstruktor.
    private readonly IEventIngestionService _ingestion;

    // Mendefinisikan konstruktor EventsController yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `ingestion` bertipe `IEventIngestionService` membawa nilai ingestion.
    public EventsController(IEventIngestionService ingestion) => _ingestion = ingestion;

    // mendaftarkan action untuk metode HTTP POST pada rute (”events”).
    [HttpPost("events")]
    // menerapkan metadata `ProducesResponseType(typeof(EventStoredResponse), StatusCodes.Status201Created)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(EventStoredResponse), StatusCodes.Status201Created)]
    // Mendefinisikan metode `CreateEvent` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani create event. async memungkinkan metode
    // menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe `EventRequest` membawa data
    // masukan permintaan yang akan divalidasi atau diteruskan ke layanan; mengambil nilai parameter dari badan permintaan HTTP; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> CreateEvent([FromBody] EventRequest request, CancellationToken ct)
    // Membuka scope metode CreateEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
    {
        // Memperbarui `var (result, status, error)` menggunakan hasil operasi asinkron memanggil `_ingestion.IngestEventAsync` dengan `request`, `User`,
        // `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam CreateEvent.
        var (result, status, error) = await _ingestion.IngestEventAsync(request, User, ct);
        // Mengembalikan hasil pemilihan bersyarat: ketika `status == StatusCodes.Status201Created` benar gunakan `StatusCode(StatusCodes.Status201Created,
        // result)`, jika tidak gunakan `StatusCode(status, error)` kepada pemanggil dalam CreateEvent; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return status == StatusCodes.Status201Created ? StatusCode(StatusCodes.Status201Created, result) : StatusCode(status, error);
    // Menutup scope metode CreateEvent; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
    }

    // mendaftarkan action untuk metode HTTP POST pada rute (”events/batch”).
    [HttpPost("events/batch")]
    // menerapkan metadata `ProducesResponseType(typeof(EventBatchResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar framework/compiler
    // dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(EventBatchResponse), StatusCodes.Status200OK)]
    // Mendefinisikan metode `CreateEventsBatch` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani create event batch. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventBatchRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; mengambil nilai parameter dari badan
    // permintaan HTTP; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
    // permintaan atau aplikasi berhenti.
    public async Task<IActionResult> CreateEventsBatch([FromBody] EventBatchRequest request, CancellationToken ct)
    // Membuka scope metode CreateEventsBatch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEventsBatch.
    {
        // Memperbarui `var (result, status, error)` menggunakan hasil operasi asinkron memanggil `_ingestion.IngestBatchAsync` dengan `request`, `User`,
        // `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam CreateEventsBatch.
        var (result, status, error) = await _ingestion.IngestBatchAsync(request, User, ct);
        // Mengembalikan hasil pemilihan bersyarat: ketika `status == 200` benar gunakan `Ok(result)`, jika tidak gunakan `StatusCode(status, error)` kepada
        // pemanggil dalam CreateEventsBatch; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return status == 200 ? Ok(result) : StatusCode(status, error);
    // Menutup scope metode CreateEventsBatch; bagian berikut berada di luar batas blok tersebut dalam CreateEventsBatch.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”sessions/{sessionId:guid}/events”).
    [HttpGet("sessions/{sessionId:guid}/events")]
    // menerapkan metadata `ProducesResponseType(typeof(EventsBySessionResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(EventsBySessionResponse), StatusCodes.Status200OK)]
    // Mendefinisikan metode `GetEventsBySession` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani get event berdasarkan sesi. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `cursor` bertipe `string?` membawa penanda halaman untuk
    // melanjutkan pembacaan setelah elemen sebelumnya; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan
    // null, yaitu penanda tidak ada nilai; mengambil nilai parameter dari query string URL; Parameter `limit` bertipe `int` membawa batas jumlah hasil
    // yang diminta pada satu operasi; bila argumen tidak diberikan digunakan nilai literal `50`; mengambil nilai parameter dari query string URL;
    // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
    // aplikasi berhenti; bila argumen tidak diberikan digunakan nilai literal `default`.
    public async Task<IActionResult> GetEventsBySession(Guid sessionId, [FromQuery] string? cursor = null, [FromQuery] int limit = 50, CancellationToken ct = default)
    // Membuka scope metode GetEventsBySession; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetEventsBySession.
    {
        // Memperbarui `var (result, status, error)` menggunakan hasil operasi asinkron memanggil `_ingestion.GetEventsBySessionAsync` dengan `sessionId`,
        // `User`, `cursor`, `limit`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetEventsBySession.
        var (result, status, error) = await _ingestion.GetEventsBySessionAsync(sessionId, User, cursor, limit, ct);
        // Mengembalikan hasil pemilihan bersyarat: ketika `status == 200` benar gunakan `Ok(result)`, jika tidak gunakan `StatusCode(status, error)` kepada
        // pemanggil dalam GetEventsBySession; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return status == 200 ? Ok(result) : StatusCode(status, error);
    // Menutup scope metode GetEventsBySession; bagian berikut berada di luar batas blok tersebut dalam GetEventsBySession.
    }
// Menutup scope tipe EventsController; bagian berikut berada di luar batas blok tersebut.
}
