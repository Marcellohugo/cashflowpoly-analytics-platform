// Fungsi file: Menangani endpoint, validasi akses, dan response HTTP untuk SecurityAuditController.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
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
// menetapkan pola rute (”api/v1/security”) untuk pencocokan URL permintaan.
[Route("api/v1/security")]
// mewajibkan otorisasi pengguna dengan ketentuan (Roles = ”INSTRUCTOR”).
[Authorize(Roles = "INSTRUCTOR")]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
// Mendefinisikan tipe class `SecurityAuditController` yang mewarisi atau menerapkan `ControllerBase`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SecurityAuditController : ControllerBase
// Membuka scope tipe SecurityAuditController; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `SecurityAuditRepository`: `_securityAudit` menyimpan nilai security audit. readonly membatasi penggantian
    // referensi/nilai field pada deklarasi atau konstruktor.
    private readonly SecurityAuditRepository _securityAudit;

    // Mendefinisikan konstruktor SecurityAuditController yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `securityAudit` bertipe `SecurityAuditRepository` membawa nilai security audit.
    public SecurityAuditController(SecurityAuditRepository securityAudit)
    // Membuka scope konstruktor SecurityAuditController; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SecurityAuditController.
    {
        // Memperbarui `_securityAudit` menggunakan `securityAudit` (nilai security audit) dalam SecurityAuditController.
        _securityAudit = securityAudit;
    // Menutup scope konstruktor SecurityAuditController; bagian berikut berada di luar batas blok tersebut dalam SecurityAuditController.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”audit-logs”).
    [HttpGet("audit-logs")]
    // menerapkan metadata `ProducesResponseType(typeof(SecurityAuditLogResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(SecurityAuditLogResponse), StatusCodes.Status200OK)]
    // Mendefinisikan metode `GetAuditLogs` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani get audit logs. async memungkinkan metode
    // menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `limit` bertipe `int` membawa batas jumlah
    // hasil yang diminta pada satu operasi; bila argumen tidak diberikan digunakan nilai literal `100`; mengambil nilai parameter dari query string
    // URL; Parameter `eventType` bertipe `string?` membawa jenis aktivitas yang menentukan aturan validasi dan proyeksi event; nilai null diizinkan
    // ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; mengambil nilai parameter dari
    // query string URL; Parameter `userId` bertipe `Guid?` membawa identitas akun pengguna yang datanya sedang diproses; nilai null diizinkan ketika
    // data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; mengambil nilai parameter dari query
    // string URL; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
    // permintaan atau aplikasi berhenti; bila argumen tidak diberikan digunakan nilai literal `default`.
    public async Task<IActionResult> GetAuditLogs(
        // Parameter `limit` bertipe `int` membawa batas jumlah hasil yang diminta pada satu operasi; bila argumen tidak diberikan digunakan nilai literal
        // `100`; mengambil nilai parameter dari query string URL.
        [FromQuery] int limit = 100,
        // Parameter `eventType` bertipe `string?` membawa jenis aktivitas yang menentukan aturan validasi dan proyeksi event; nilai null diizinkan ketika
        // data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; mengambil nilai parameter dari query
        // string URL.
        [FromQuery] string? eventType = null,
        // Parameter `userId` bertipe `Guid?` membawa identitas akun pengguna yang datanya sedang diproses; nilai null diizinkan ketika data opsional belum
        // tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; mengambil nilai parameter dari query string URL.
        [FromQuery] Guid? userId = null,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti; bila argumen tidak diberikan digunakan nilai literal `default`.
        CancellationToken ct = default)
    // Membuka scope metode GetAuditLogs; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetAuditLogs.
    {
        // Menyiapkan variabel lokal `clampedLimit` untuk nilai clamped limit dengan membatasi `limit` agar tidak lebih kecil dari `1` dan tidak lebih besar
        // dari `500`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var clampedLimit = Math.Clamp(limit, 1, 500);
        // Menyiapkan variabel lokal `normalizedEventType` untuk nilai normalized event jenis dengan hasil pemilihan bersyarat: ketika
        // `string.IsNullOrWhiteSpace(eventType)` benar gunakan `null`, jika tidak gunakan `eventType.Trim().ToUpperInvariant()`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var normalizedEventType = string.IsNullOrWhiteSpace(eventType)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: null dalam GetAuditLogs.
            ? null
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: eventType.Trim().ToUpperInvariant(); dalam GetAuditLogs.
            : eventType.Trim().ToUpperInvariant();

        // Menyiapkan variabel lokal `logs` untuk nilai logs dengan hasil operasi asinkron memanggil `_securityAudit.ListRecentAsync` dengan `clampedLimit`,
        // `normalizedEventType`, `userId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var logs = await _securityAudit.ListRecentAsync(clampedLimit, normalizedEventType, userId, ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan mematerialisasi urutan `logs .Select(log => new SecurityAuditLogItem(
        // log.SecurityAuditLogId, log.OccurredAt, log.TraceId, log.EventType, log.Outcome, log.UserId, log.Username, log.Role, log.IpAddres...` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = logs
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(log => new SecurityAuditLogItem( dalam GetAuditLogs; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(log => new SecurityAuditLogItem(
                // Meneruskan `log.SecurityAuditLogId` (nilai security audit log identitas) sebagai argumen ke konstruktor `SecurityAuditLogItem`.
                log.SecurityAuditLogId,
                // Meneruskan `log.OccurredAt` (nilai occurred at) sebagai argumen ke konstruktor `SecurityAuditLogItem`.
                log.OccurredAt,
                // Meneruskan `log.TraceId` (identitas penelusuran yang menghubungkan respons, log, dan permintaan yang sama) sebagai argumen ke konstruktor
                // `SecurityAuditLogItem`.
                log.TraceId,
                // Meneruskan `log.EventType` (jenis aktivitas yang menentukan aturan validasi dan proyeksi event) sebagai argumen ke konstruktor
                // `SecurityAuditLogItem`.
                log.EventType,
                // Meneruskan `log.Outcome` (nilai hasil) sebagai argumen ke konstruktor `SecurityAuditLogItem`.
                log.Outcome,
                // Meneruskan `log.UserId` (identitas akun pengguna yang datanya sedang diproses) sebagai argumen ke konstruktor `SecurityAuditLogItem`.
                log.UserId,
                // Meneruskan `log.Username` (nama akun yang dipakai saat autentikasi) sebagai argumen ke konstruktor `SecurityAuditLogItem`.
                log.Username,
                // Meneruskan `log.Role` (peran pengguna yang menentukan hak akses) sebagai argumen ke konstruktor `SecurityAuditLogItem`.
                log.Role,
                // Meneruskan `log.IpAddress` (nilai ip address) sebagai argumen ke konstruktor `SecurityAuditLogItem`.
                log.IpAddress,
                // Meneruskan `log.UserAgent` (nilai pengguna agent) sebagai argumen ke konstruktor `SecurityAuditLogItem`.
                log.UserAgent,
                // Meneruskan `log.Method` (nilai method) sebagai argumen ke konstruktor `SecurityAuditLogItem`.
                log.Method,
                // Meneruskan `log.Path` (nilai path) sebagai argumen ke konstruktor `SecurityAuditLogItem`.
                log.Path,
                // Meneruskan `log.StatusCode` (kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan) sebagai argumen ke konstruktor
                // `SecurityAuditLogItem`.
                log.StatusCode,
                // Meneruskan memanggil `ParseJsonElement` dengan `log.DetailJson` sebagai argumen ke konstruktor `SecurityAuditLogItem`; Meneruskan
                // `log.DetailJson` (nilai detail JSON) sebagai argumen ke `ParseJsonElement`.
                ParseJsonElement(log.DetailJson)))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam GetAuditLogs; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .ToList();

        // Mengembalikan membentuk respons HTTP 200 dengan `new SecurityAuditLogResponse(items)` sebagai hasil berhasil kepada pemanggil dalam GetAuditLogs;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return Ok(new SecurityAuditLogResponse(items));
    // Menutup scope metode GetAuditLogs; bagian berikut berada di luar batas blok tersebut dalam GetAuditLogs.
    }

    // Mendefinisikan metode `ParseJsonElement` dengan hasil bertipe `JsonElement?`; operasi ini menangani parse JSON element. Masukan: Parameter `json`
    // bertipe `string?` membawa nilai JSON; nilai null diizinkan ketika data opsional belum tersedia.
    private static JsonElement? ParseJsonElement(string? json)
    // Membuka scope metode ParseJsonElement; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ParseJsonElement.
    {
        // Memeriksa memeriksa apakah `json` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ParseJsonElement.
        if (string.IsNullOrWhiteSpace(json))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(json)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ParseJsonElement.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam ParseJsonElement; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(json)`; bagian berikut berada di luar batas blok tersebut dalam
        // ParseJsonElement.
        }

        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `json`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(json);
        // Mengembalikan membuat salinan `document.RootElement` agar hasil dapat digunakan terpisah dari objek sumber kepada pemanggil dalam
        // ParseJsonElement; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return document.RootElement.Clone();
    // Menutup scope metode ParseJsonElement; bagian berikut berada di luar batas blok tersebut dalam ParseJsonElement.
    }
// Menutup scope tipe SecurityAuditController; bagian berikut berada di luar batas blok tersebut.
}
