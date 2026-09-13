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
public sealed class SecurityAuditController : ControllerBase
{
    private readonly SecurityAuditRepository _securityAudit;

    public SecurityAuditController(SecurityAuditRepository securityAudit)
    {
        _securityAudit = securityAudit;
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”audit-logs”).
    [HttpGet("audit-logs")]
    // menerapkan metadata `ProducesResponseType(typeof(SecurityAuditLogResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(SecurityAuditLogResponse), StatusCodes.Status200OK)]
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
    {
        var clampedLimit = Math.Clamp(limit, 1, 500);
        var normalizedEventType = string.IsNullOrWhiteSpace(eventType)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: null dalam GetAuditLogs.
            ? null
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: eventType.Trim().ToUpperInvariant(); dalam GetAuditLogs.
            : eventType.Trim().ToUpperInvariant();

        var logs = await _securityAudit.ListRecentAsync(clampedLimit, normalizedEventType, userId, ct);
        var items = logs
            .Select(log => new SecurityAuditLogItem(
                log.SecurityAuditLogId,
                log.OccurredAt,
                log.TraceId,
                log.EventType,
                log.Outcome,
                log.UserId,
                log.Username,
                log.Role,
                log.IpAddress,
                log.UserAgent,
                log.Method,
                log.Path,
                log.StatusCode,
                ParseJsonElement(log.DetailJson)))
            .ToList();

        return Ok(new SecurityAuditLogResponse(items));
    }

    private static JsonElement? ParseJsonElement(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}
