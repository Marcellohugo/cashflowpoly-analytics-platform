// Fungsi file: Menangani endpoint, validasi akses, dan response HTTP untuk RulesetsController.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Cashflowpoly.Api.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Infrastructure;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;
// Mengimpor namespace `Microsoft.AspNetCore.Authorization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Authorization;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Controllers` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Controllers;

// mengaktifkan perilaku API controller, termasuk inferensi binding dan respons otomatis atas model tidak valid.
[ApiController]
// menetapkan pola rute (”api/v1/rulesets”) untuk pencocokan URL permintaan.
[Route("api/v1/rulesets")]
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
// Mendefinisikan tipe class `RulesetsController` yang mewarisi atau menerapkan `ControllerBase`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetsController : ControllerBase
// Membuka scope tipe RulesetsController; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `RulesetRepository`: `_rulesets` menyimpan nilai aturan. readonly membatasi penggantian referensi/nilai field pada
    // deklarasi atau konstruktor.
    private readonly RulesetRepository _rulesets;
    // Mendeklarasikan field bertipe `SessionStateRepository`: `_state` menyimpan keadaan permainan yang menjadi sumber atau hasil pembaruan. readonly
    // membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly SessionStateRepository _state;
    // Mendeklarasikan field bertipe `UserRepository`: `_users` menyimpan nilai pengguna. readonly membatasi penggantian referensi/nilai field pada
    // deklarasi atau konstruktor.
    private readonly UserRepository _users;

    // Mendefinisikan konstruktor RulesetsController yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `rulesets` bertipe `RulesetRepository` membawa nilai aturan; Parameter `state` bertipe `SessionStateRepository` membawa keadaan permainan yang
    // menjadi sumber atau hasil pembaruan; Parameter `users` bertipe `UserRepository` membawa nilai pengguna.
    public RulesetsController(RulesetRepository rulesets, SessionStateRepository state, UserRepository users)
    // Membuka scope konstruktor RulesetsController; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RulesetsController.
    {
        // Memperbarui `_rulesets` menggunakan `rulesets` (nilai aturan) dalam RulesetsController.
        _rulesets = rulesets;
        // Memperbarui `_state` menggunakan `state` (keadaan permainan yang menjadi sumber atau hasil pembaruan) dalam RulesetsController.
        _state = state;
        // Memperbarui `_users` menggunakan `users` (nilai pengguna) dalam RulesetsController.
        _users = users;
    // Menutup scope konstruktor RulesetsController; bagian berikut berada di luar batas blok tersebut dalam RulesetsController.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”sections”).
    [HttpGet("sections")]
    // menerapkan metadata `ProducesResponseType(typeof(RulesetSectionsResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(RulesetSectionsResponse), StatusCodes.Status200OK)]
    // Mendefinisikan metode `GetRulesetSections` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani get aturan sections. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `mode` bertipe `string?`
    // membawa mode permainan yang menentukan kelompok aturan yang digunakan; nilai null diizinkan ketika data opsional belum tersedia; mengambil nilai
    // parameter dari query string URL; Parameter `rulesetId` bertipe `Guid?` membawa identitas kumpulan aturan permainan; nilai null diizinkan ketika
    // data opsional belum tersedia; mengambil nilai parameter dari query string URL; Parameter `ct` bertipe `CancellationToken` membawa sinyal
    // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> GetRulesetSections([FromQuery] string? mode, [FromQuery] Guid? rulesetId, CancellationToken ct)
    // Membuka scope metode GetRulesetSections; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetSections.
    {
        // Memeriksa kebalikan kondisi `TryGetCurrentUserId(out var userId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetRulesetSections.
        if (!TryGetCurrentUserId(out var userId))
        // Membuka scope cabang if untuk kondisi `!TryGetCurrentUserId(out var userId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetRulesetSections.
        {
            // Mengembalikan membentuk respons HTTP 401 dengan `ApiErrorHelper.BuildError(HttpContext, ”UNAUTHORIZED”, ”Token user tidak valid”)` karena
            // autentikasi tidak terpenuhi kepada pemanggil dalam GetRulesetSections; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        // Menutup scope cabang if untuk kondisi `!TryGetCurrentUserId(out var userId)`; bagian berikut berada di luar batas blok tersebut dalam
        // GetRulesetSections.
        }

        // Memeriksa kebalikan kondisi `TryNormalizeMode(mode, out var normalizedMode, out var modeError)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam GetRulesetSections.
        if (!TryNormalizeMode(mode, out var normalizedMode, out var modeError))
        // Membuka scope cabang if untuk kondisi `!TryNormalizeMode(mode, out var normalizedMode, out var modeError)`; pernyataan/deklarasi berikut berada
        // di dalam batas blok ini dalam GetRulesetSections.
        {
            // Mengembalikan `modeError` dengan penegasan non-null untuk analisis compiler; operator ! tidak menambah pemeriksaan saat runtime kepada pemanggil
            // dalam GetRulesetSections; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return modeError!;
        // Menutup scope cabang if untuk kondisi `!TryNormalizeMode(mode, out var normalizedMode, out var modeError)`; bagian berikut berada di luar batas
        // blok tersebut dalam GetRulesetSections.
        }

        // Menyiapkan variabel lokal `role` untuk peran pengguna yang menentukan hak akses dengan memanggil `User.FindFirstValue` dengan `ClaimTypes.Role`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var role = User.FindFirstValue(ClaimTypes.Role);
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!string.Equals(role, ”INSTRUCTOR”, StringComparison.OrdinalIgnoreCase)` dan
        // `!string.Equals(role, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan
        // ketika kondisi ini bernilai benar dalam GetRulesetSections.
        if (!string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase) &&
            // Menggunakan kebalikan kondisi `string.Equals(role, ”PLAYER”, StringComparison.OrdinalIgnoreCase)` sebagai bagian ekspresi yang sedang disusun
            // dalam GetRulesetSections.
            !string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `!string.Equals(role, ”INSTRUCTOR”, StringComparison.OrdinalIgnoreCase) && !string.Equals(role, ”PLAYER”,
        // StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetSections.
        {
            // Mengembalikan membentuk hasil HTTP dengan kode dan isi `StatusCodes.Status403Forbidden`, `ApiErrorHelper.BuildError(HttpContext, ”FORBIDDEN”,
            // ”Role tidak diizinkan”)` kepada pemanggil dalam GetRulesetSections; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return StatusCode(StatusCodes.Status403Forbidden,
                // Meneruskan memanggil `ApiErrorHelper.BuildError` dengan `HttpContext`, `”FORBIDDEN”`, `”Role tidak diizinkan”` sebagai argumen ke `StatusCode`;
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`;
                // Meneruskan nilai literal `”FORBIDDEN”` sebagai argumen ke `ApiErrorHelper.BuildError`; Meneruskan nilai literal `”Role tidak diizinkan”` sebagai
                // argumen ke `ApiErrorHelper.BuildError`.
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak diizinkan"));
        // Menutup scope cabang if untuk kondisi `!string.Equals(role, ”INSTRUCTOR”, StringComparison.OrdinalIgnoreCase) && !string.Equals(role, ”PLAYER”,
        // StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetSections.
        }

        // Menyiapkan variabel lokal `instructorUserId` untuk identitas instruktur pemilik sesi atau aturan dengan hasil pemilihan bersyarat: ketika
        // `string.Equals(role, ”INSTRUCTOR”, StringComparison.OrdinalIgnoreCase)` benar gunakan `userId`, jika tidak gunakan `(Guid?)null`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var instructorUserId = string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: userId dalam GetRulesetSections.
            ? userId
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (Guid?)null; dalam GetRulesetSections.
            : (Guid?)null;
        // Menyiapkan variabel lokal `ruleset` untuk nilai aturan dengan hasil operasi asinkron memanggil `_state.GetRulesetSectionAsync` dengan
        // `normalizedMode`, `rulesetId`, `instructorUserId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var ruleset = await _state.GetRulesetSectionAsync(normalizedMode, rulesetId, instructorUserId, ct);
        // Memeriksa hasil pencocokan `ruleset` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam GetRulesetSections.
        if (ruleset is null)
        // Membuka scope cabang if untuk kondisi `ruleset is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetSections.
        {
            // Mengembalikan membentuk respons HTTP 404 dengan `ApiErrorHelper.BuildError(HttpContext, ”NOT_FOUND”, ”Ruleset tidak ditemukan”)` karena sumber
            // daya tidak ditemukan kepada pemanggil dalam GetRulesetSections; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan"));
        // Menutup scope cabang if untuk kondisi `ruleset is null`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetSections.
        }

        // Memeriksa hasil pencocokan `ruleset.Definition` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetRulesetSections.
        if (ruleset.Definition is null)
        // Membuka scope cabang if untuk kondisi `ruleset.Definition is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetRulesetSections.
        {
            // Mengembalikan membentuk respons HTTP 404 dengan `ApiErrorHelper.BuildError(HttpContext, ”NOT_FOUND”, ”Ruleset tidak memiliki definisi relasional
            // yang lengkap”)` karena sumber daya tidak ditemukan kepada pemanggil dalam GetRulesetSections; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak memiliki definisi relasional yang lengkap"));
        // Menutup scope cabang if untuk kondisi `ruleset.Definition is null`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetSections.
        }

        // Menyiapkan variabel lokal `catalog` untuk nilai catalog dengan memanggil `RulesetSectionCatalog.FromDefinition` dengan `ruleset.Definition`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var catalog = RulesetSectionCatalog.FromDefinition(ruleset.Definition);
        // Mengembalikan membentuk respons HTTP 200 dengan `new RulesetSectionsResponse( ruleset.RulesetId, ruleset.RulesetVersionId, ruleset.Mode,
        // catalog.GameConfig, catalog.Bahan, catalog.Resep, catalog.Kebutuhan, catalog.TargetKebu...` sebagai hasil berhasil kepada pemanggil dalam
        // GetRulesetSections; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return Ok(new RulesetSectionsResponse(
            // Meneruskan `ruleset.RulesetId` (identitas kumpulan aturan permainan) sebagai argumen ke konstruktor `RulesetSectionsResponse`.
            ruleset.RulesetId,
            // Meneruskan `ruleset.RulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
            // konstruktor `RulesetSectionsResponse`.
            ruleset.RulesetVersionId,
            // Meneruskan `ruleset.Mode` (mode permainan yang menentukan kelompok aturan yang digunakan) sebagai argumen ke konstruktor
            // `RulesetSectionsResponse`.
            ruleset.Mode,
            // Meneruskan `catalog.GameConfig` (nilai game konfigurasi) sebagai argumen ke konstruktor `RulesetSectionsResponse`.
            catalog.GameConfig,
            // Meneruskan `catalog.Bahan` (nilai bahan) sebagai argumen ke konstruktor `RulesetSectionsResponse`.
            catalog.Bahan,
            // Meneruskan `catalog.Resep` (nilai resep) sebagai argumen ke konstruktor `RulesetSectionsResponse`.
            catalog.Resep,
            // Meneruskan `catalog.Kebutuhan` (nilai kebutuhan) sebagai argumen ke konstruktor `RulesetSectionsResponse`.
            catalog.Kebutuhan,
            // Meneruskan `catalog.TargetKebutuhan` (nilai target kebutuhan) sebagai argumen ke konstruktor `RulesetSectionsResponse`.
            catalog.TargetKebutuhan,
            // Meneruskan `catalog.TujuanFinansial` (nilai tujuan finansial) sebagai argumen ke konstruktor `RulesetSectionsResponse`.
            catalog.TujuanFinansial,
            // Meneruskan `catalog.Narasi` (nilai narasi) sebagai argumen ke konstruktor `RulesetSectionsResponse`.
            catalog.Narasi));
    // Menutup scope metode GetRulesetSections; bagian berikut berada di luar batas blok tersebut dalam GetRulesetSections.
    }


    // mendaftarkan action untuk metode HTTP POST pada rute controller saat ini.
    [HttpPost]
    // mewajibkan otorisasi pengguna dengan ketentuan (Roles = ”INSTRUCTOR”).
    [Authorize(Roles = "INSTRUCTOR")]
    // menerapkan metadata `ProducesResponseType(typeof(CreateRulesetResponse), StatusCodes.Status201Created)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(CreateRulesetResponse), StatusCodes.Status201Created)]
    // Mendefinisikan metode `CreateRuleset` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani create aturan. async memungkinkan metode
    // menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe `CreateRulesetRequest`
    // membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; mengambil nilai parameter dari badan permintaan HTTP; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    public async Task<IActionResult> CreateRuleset([FromBody] CreateRulesetRequest request, CancellationToken ct)
    // Membuka scope metode CreateRuleset; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateRuleset.
    {
        // Memeriksa kebalikan kondisi `TryGetCurrentUserId(out var instructorUserId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // CreateRuleset.
        if (!TryGetCurrentUserId(out var instructorUserId))
        // Membuka scope cabang if untuk kondisi `!TryGetCurrentUserId(out var instructorUserId)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam CreateRuleset.
        {
            // Mengembalikan membentuk respons HTTP 401 dengan `ApiErrorHelper.BuildError(HttpContext, ”UNAUTHORIZED”, ”Token user tidak valid”)` karena
            // autentikasi tidak terpenuhi kepada pemanggil dalam CreateRuleset; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        // Menutup scope cabang if untuk kondisi `!TryGetCurrentUserId(out var instructorUserId)`; bagian berikut berada di luar batas blok tersebut dalam
        // CreateRuleset.
        }

        // Memeriksa memeriksa apakah `request.Name` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam CreateRuleset.
        if (string.IsNullOrWhiteSpace(request.Name))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(request.Name)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam CreateRuleset.
        {
            // Mengembalikan membentuk respons HTTP 400 dengan `ApiErrorHelper.BuildError(HttpContext, ”VALIDATION_ERROR”, ”Field wajib tidak lengkap”, new
            // ErrorDetail(”name”, ”REQUIRED”))` karena permintaan tidak memenuhi kontrak kepada pemanggil dalam CreateRuleset; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Field wajib tidak lengkap",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”name”, ”REQUIRED”) sebagai argumen ke `ApiErrorHelper.BuildError`; Meneruskan nilai
                // literal `”name”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen ke konstruktor
                // `ErrorDetail`.
                new ErrorDetail("name", "REQUIRED")));
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(request.Name)`; bagian berikut berada di luar batas blok tersebut dalam
        // CreateRuleset.
        }

        // Menyiapkan variabel lokal `prepared` untuk nilai prepared dengan hasil operasi asinkron memanggil `PrepareDefinitionForWriteAsync` dengan
        // `request.Definition`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var prepared = await PrepareDefinitionForWriteAsync(request.Definition, ct);
        // Memeriksa hasil pencocokan `prepared.Error` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // CreateRuleset.
        if (prepared.Error is not null)
        // Membuka scope cabang if untuk kondisi `prepared.Error is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CreateRuleset.
        {
            // Mengembalikan `prepared.Error` (informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil) kepada pemanggil dalam
            // CreateRuleset; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return prepared.Error;
        // Menutup scope cabang if untuk kondisi `prepared.Error is not null`; bagian berikut berada di luar batas blok tersebut dalam CreateRuleset.
        }

        // Menyiapkan variabel lokal `created` untuk nilai created dengan hasil operasi asinkron memanggil `_rulesets.CreateRulesetAsync` dengan
        // `request.Name`, `request.Description`, `instructorUserId`, `prepared.Definition!`, `instructorUserId`, `ct`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var created = await _rulesets.CreateRulesetAsync(
            // Meneruskan `request.Name` (nilai nama) sebagai argumen ke `_rulesets.CreateRulesetAsync`.
            request.Name,
            // Meneruskan `request.Description` (nilai description) sebagai argumen ke `_rulesets.CreateRulesetAsync`.
            request.Description,
            // Meneruskan `instructorUserId` (identitas instruktur pemilik sesi atau aturan) sebagai argumen ke `_rulesets.CreateRulesetAsync`.
            instructorUserId,
            // Meneruskan `prepared.Definition` dengan penegasan non-null untuk analisis compiler; operator ! tidak menambah pemeriksaan saat runtime sebagai
            // argumen ke `_rulesets.CreateRulesetAsync`.
            prepared.Definition!,
            // Meneruskan `instructorUserId` (identitas instruktur pemilik sesi atau aturan) sebagai argumen ke `_rulesets.CreateRulesetAsync`.
            instructorUserId,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // ke `_rulesets.CreateRulesetAsync`.
            ct);

        // Mengembalikan memanggil `Created` dengan `$”/api/v1/rulesets/{created.RulesetId}”`, `new CreateRulesetResponse(created.RulesetId,
        // created.RulesetVersionId, created.Version)` kepada pemanggil dalam CreateRuleset; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return Created(
            // Meneruskan teks interpolasi `$”/api/v1/rulesets/{created.RulesetId}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan
            // sebagai argumen ke `Created`.
            $"/api/v1/rulesets/{created.RulesetId}",
            // Meneruskan objek baru bertipe `CreateRulesetResponse` dengan argumen (created.RulesetId, created.RulesetVersionId, created.Version) sebagai
            // argumen ke `Created`; Meneruskan `created.RulesetId` (identitas kumpulan aturan permainan) sebagai argumen ke konstruktor
            // `CreateRulesetResponse`; Meneruskan `created.RulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang
            // tepat) sebagai argumen ke konstruktor `CreateRulesetResponse`; Meneruskan `created.Version` (nomor versi yang dipakai untuk konsistensi data atau
            // konfigurasi) sebagai argumen ke konstruktor `CreateRulesetResponse`.
            new CreateRulesetResponse(created.RulesetId, created.RulesetVersionId, created.Version));
    // Menutup scope metode CreateRuleset; bagian berikut berada di luar batas blok tersebut dalam CreateRuleset.
    }

    // mendaftarkan action untuk metode HTTP PUT pada rute (”{rulesetId:guid}”).
    [HttpPut("{rulesetId:guid}")]
    // mewajibkan otorisasi pengguna dengan ketentuan (Roles = ”INSTRUCTOR”).
    [Authorize(Roles = "INSTRUCTOR")]
    // menerapkan metadata `ProducesResponseType(typeof(CreateRulesetResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar framework/compiler
    // dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(CreateRulesetResponse), StatusCodes.Status200OK)]
    // Mendefinisikan metode `UpdateRuleset` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani update aturan. async memungkinkan metode
    // menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `rulesetId` bertipe `Guid` membawa identitas
    // kumpulan aturan permainan; Parameter `request` bertipe `UpdateRulesetRequest` membawa data masukan permintaan yang akan divalidasi atau
    // diteruskan ke layanan; mengambil nilai parameter dari badan permintaan HTTP; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan
    // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> UpdateRuleset(Guid rulesetId, [FromBody] UpdateRulesetRequest request, CancellationToken ct)
    // Membuka scope metode UpdateRuleset; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam UpdateRuleset.
    {
        // Memeriksa kebalikan kondisi `TryGetCurrentUserId(out var instructorUserId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // UpdateRuleset.
        if (!TryGetCurrentUserId(out var instructorUserId))
        // Membuka scope cabang if untuk kondisi `!TryGetCurrentUserId(out var instructorUserId)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam UpdateRuleset.
        {
            // Mengembalikan membentuk respons HTTP 401 dengan `ApiErrorHelper.BuildError(HttpContext, ”UNAUTHORIZED”, ”Token user tidak valid”)` karena
            // autentikasi tidak terpenuhi kepada pemanggil dalam UpdateRuleset; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        // Menutup scope cabang if untuk kondisi `!TryGetCurrentUserId(out var instructorUserId)`; bagian berikut berada di luar batas blok tersebut dalam
        // UpdateRuleset.
        }

        // Menyiapkan variabel lokal `mutableRuleset` untuk nilai mutable aturan dengan hasil operasi asinkron memanggil `GetMutableInstructorRulesetAsync`
        // dengan `rulesetId`, `instructorUserId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var mutableRuleset = await GetMutableInstructorRulesetAsync(rulesetId, instructorUserId, ct);
        // Memeriksa hasil pencocokan `mutableRuleset.Error` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // UpdateRuleset.
        if (mutableRuleset.Error is not null)
        // Membuka scope cabang if untuk kondisi `mutableRuleset.Error is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // UpdateRuleset.
        {
            // Mengembalikan `mutableRuleset.Error` (informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil) kepada pemanggil dalam
            // UpdateRuleset; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return mutableRuleset.Error;
        // Menutup scope cabang if untuk kondisi `mutableRuleset.Error is not null`; bagian berikut berada di luar batas blok tersebut dalam UpdateRuleset.
        }

        // Menyiapkan variabel lokal `prepared` untuk nilai prepared dengan hasil operasi asinkron memanggil `PrepareDefinitionForWriteAsync` dengan
        // `request.Definition`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var prepared = await PrepareDefinitionForWriteAsync(request.Definition, ct);
        // Memeriksa hasil pencocokan `prepared.Error` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // UpdateRuleset.
        if (prepared.Error is not null)
        // Membuka scope cabang if untuk kondisi `prepared.Error is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // UpdateRuleset.
        {
            // Mengembalikan `prepared.Error` (informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil) kepada pemanggil dalam
            // UpdateRuleset; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return prepared.Error;
        // Menutup scope cabang if untuk kondisi `prepared.Error is not null`; bagian berikut berada di luar batas blok tersebut dalam UpdateRuleset.
        }

        // Menyiapkan variabel lokal `createdVersion` untuk nilai created versi tanpa nilai awal pada deklarasi ini. Tipe yang dipakai adalah `(Guid
        // RulesetVersionId, int Version)`.
        (Guid RulesetVersionId, int Version) createdVersion;
        // Memulai blok try dalam UpdateRuleset; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam UpdateRuleset.
        {
            // Memperbarui `createdVersion` menggunakan hasil operasi asinkron memanggil `_rulesets.CreateRulesetVersionAsync` dengan `rulesetId`,
            // `request.Name`, `request.Description`, `prepared.Definition!`, `instructorUserId`, `ct`; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai dalam UpdateRuleset.
            createdVersion = await _rulesets.CreateRulesetVersionAsync(
                // Meneruskan `rulesetId` (identitas kumpulan aturan permainan) sebagai argumen ke `_rulesets.CreateRulesetVersionAsync`.
                rulesetId,
                // Meneruskan `request.Name` (nilai nama) sebagai argumen ke `_rulesets.CreateRulesetVersionAsync`.
                request.Name,
                // Meneruskan `request.Description` (nilai description) sebagai argumen ke `_rulesets.CreateRulesetVersionAsync`.
                request.Description,
                // Meneruskan `prepared.Definition` dengan penegasan non-null untuk analisis compiler; operator ! tidak menambah pemeriksaan saat runtime sebagai
                // argumen ke `_rulesets.CreateRulesetVersionAsync`.
                prepared.Definition!,
                // Meneruskan `instructorUserId` (identitas instruktur pemilik sesi atau aturan) sebagai argumen ke `_rulesets.CreateRulesetVersionAsync`.
                instructorUserId,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `_rulesets.CreateRulesetVersionAsync`.
                ct);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam UpdateRuleset.
        }
        // Menangani exception `PostgresException` melalui variabel ex hanya jika filter `ex.SqlState == PostgresErrorCodes.UniqueViolation &&
        // IsRulesetConfigHashUniqueViolation(ex)` terpenuhi dalam UpdateRuleset.
        catch (PostgresException ex) when (
            // Melanjutkan ekspresi dengan perbandingan kesamaan antara `ex.SqlState` dan `PostgresErrorCodes.UniqueViolation` dalam UpdateRuleset.
            ex.SqlState == PostgresErrorCodes.UniqueViolation &&
            // Melanjutkan pengolahan dengan memanggil `IsRulesetConfigHashUniqueViolation` dengan `ex` dalam UpdateRuleset.
            IsRulesetConfigHashUniqueViolation(ex))
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam UpdateRuleset.
        {
            // Mengembalikan memanggil `Conflict` dengan `ApiErrorHelper.BuildError( HttpContext, ”DUPLICATE”, ”Konfigurasi ruleset tersebut sudah pernah dibuat
            // sebagai versi ruleset ini”)` kepada pemanggil dalam UpdateRuleset; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Conflict(ApiErrorHelper.BuildError(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`.
                HttpContext,
                // Meneruskan nilai literal `”DUPLICATE”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                "DUPLICATE",
                // Meneruskan nilai literal `”Konfigurasi ruleset tersebut sudah pernah dibuat sebagai versi ruleset ini”` sebagai argumen ke
                // `ApiErrorHelper.BuildError`.
                "Konfigurasi ruleset tersebut sudah pernah dibuat sebagai versi ruleset ini"));
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam UpdateRuleset.
        }

        // Mengembalikan membentuk respons HTTP 200 dengan `new CreateRulesetResponse(rulesetId, createdVersion.RulesetVersionId, createdVersion.Version)`
        // sebagai hasil berhasil kepada pemanggil dalam UpdateRuleset; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return Ok(new CreateRulesetResponse(rulesetId, createdVersion.RulesetVersionId, createdVersion.Version));
    // Menutup scope metode UpdateRuleset; bagian berikut berada di luar batas blok tersebut dalam UpdateRuleset.
    }

    // mendaftarkan action untuk metode HTTP POST pada rute (”{rulesetId:guid}/versions/{version:int}/activate”).
    [HttpPost("{rulesetId:guid}/versions/{version:int}/activate")]
    // mewajibkan otorisasi pengguna dengan ketentuan (Roles = ”INSTRUCTOR”).
    [Authorize(Roles = "INSTRUCTOR")]
    // menerapkan metadata `ProducesResponseType(typeof(CreateRulesetResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar framework/compiler
    // dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(CreateRulesetResponse), StatusCodes.Status200OK)]
    // Mendefinisikan metode `ActivateRulesetVersion` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani activate aturan versi. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `rulesetId` bertipe `Guid`
    // membawa identitas kumpulan aturan permainan; Parameter `version` bertipe `int` membawa nomor versi yang dipakai untuk konsistensi data atau
    // konfigurasi; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
    // permintaan atau aplikasi berhenti.
    public async Task<IActionResult> ActivateRulesetVersion(Guid rulesetId, int version, CancellationToken ct)
    // Membuka scope metode ActivateRulesetVersion; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ActivateRulesetVersion.
    {
        // Memeriksa kebalikan kondisi `TryGetCurrentUserId(out var instructorUserId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ActivateRulesetVersion.
        if (!TryGetCurrentUserId(out var instructorUserId))
        // Membuka scope cabang if untuk kondisi `!TryGetCurrentUserId(out var instructorUserId)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam ActivateRulesetVersion.
        {
            // Mengembalikan membentuk respons HTTP 401 dengan `ApiErrorHelper.BuildError(HttpContext, ”UNAUTHORIZED”, ”Token user tidak valid”)` karena
            // autentikasi tidak terpenuhi kepada pemanggil dalam ActivateRulesetVersion; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        // Menutup scope cabang if untuk kondisi `!TryGetCurrentUserId(out var instructorUserId)`; bagian berikut berada di luar batas blok tersebut dalam
        // ActivateRulesetVersion.
        }

        // Menyiapkan variabel lokal `mutableRuleset` untuk nilai mutable aturan dengan hasil operasi asinkron memanggil `GetMutableInstructorRulesetAsync`
        // dengan `rulesetId`, `instructorUserId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var mutableRuleset = await GetMutableInstructorRulesetAsync(rulesetId, instructorUserId, ct);
        // Memeriksa hasil pencocokan `mutableRuleset.Error` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ActivateRulesetVersion.
        if (mutableRuleset.Error is not null)
        // Membuka scope cabang if untuk kondisi `mutableRuleset.Error is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ActivateRulesetVersion.
        {
            // Mengembalikan `mutableRuleset.Error` (informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil) kepada pemanggil dalam
            // ActivateRulesetVersion; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return mutableRuleset.Error;
        // Menutup scope cabang if untuk kondisi `mutableRuleset.Error is not null`; bagian berikut berada di luar batas blok tersebut dalam
        // ActivateRulesetVersion.
        }

        // Menyiapkan variabel lokal `selectedVersion` untuk nilai selected versi dengan hasil operasi asinkron memanggil `_rulesets.GetRulesetVersionAsync`
        // dengan `rulesetId`, `version`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var selectedVersion = await _rulesets.GetRulesetVersionAsync(rulesetId, version, ct);
        // Memeriksa hasil pencocokan `selectedVersion` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ActivateRulesetVersion.
        if (selectedVersion is null)
        // Membuka scope cabang if untuk kondisi `selectedVersion is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ActivateRulesetVersion.
        {
            // Mengembalikan membentuk respons HTTP 404 dengan `ApiErrorHelper.BuildError(HttpContext, ”NOT_FOUND”, ”Ruleset version tidak ditemukan”)` karena
            // sumber daya tidak ditemukan kepada pemanggil dalam ActivateRulesetVersion; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset version tidak ditemukan"));
        // Menutup scope cabang if untuk kondisi `selectedVersion is null`; bagian berikut berada di luar batas blok tersebut dalam ActivateRulesetVersion.
        }

        // Menyiapkan variabel lokal `configErrors` untuk nilai konfigurasi kesalahan tanpa nilai awal pada deklarasi ini. Tipe yang dipakai adalah
        // `List<ErrorDetail>`.
        List<ErrorDetail> configErrors;
        // Memeriksa hasil pencocokan `selectedVersion.Definition` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ActivateRulesetVersion.
        if (selectedVersion.Definition is null)
        // Membuka scope cabang if untuk kondisi `selectedVersion.Definition is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ActivateRulesetVersion.
        {
            // Memperbarui `configErrors` menggunakan koleksi berisi new ErrorDetail(”definition”, ”REQUIRED”) dalam ActivateRulesetVersion.
            configErrors = [new ErrorDetail("definition", "REQUIRED")];
        // Menutup scope cabang if untuk kondisi `selectedVersion.Definition is null`; bagian berikut berada di luar batas blok tersebut dalam
        // ActivateRulesetVersion.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam ActivateRulesetVersion.
        else if (!RulesetRuntimeMapper.TryBuildConfig(selectedVersion.Definition, out _, out configErrors))
        // Membuka scope cabang if untuk kondisi `!RulesetRuntimeMapper.TryBuildConfig(selectedVersion.Definition, out _, out configErrors)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ActivateRulesetVersion.
        {
            // Mengembalikan membentuk respons HTTP 400 dengan `ApiErrorHelper.BuildError( HttpContext, ”VALIDATION_ERROR”, ”Definition ruleset tidak valid”,
            // configErrors.ToArray())` karena permintaan tidak memenuhi kontrak kepada pemanggil dalam ActivateRulesetVersion; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return BadRequest(ApiErrorHelper.BuildError(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`.
                HttpContext,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Definition ruleset tidak valid”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                "Definition ruleset tidak valid",
                // Meneruskan mematerialisasi urutan `configErrors` menjadi array dengan elemen hasil saat ini sebagai argumen ke `ApiErrorHelper.BuildError`.
                configErrors.ToArray()));
        // Menutup scope cabang if untuk kondisi `!RulesetRuntimeMapper.TryBuildConfig(selectedVersion.Definition, out _, out configErrors)`; bagian berikut
        // berada di luar batas blok tersebut dalam ActivateRulesetVersion.
        }

        // Menjalankan hasil operasi asinkron memanggil `_rulesets.ActivateRulesetVersionAsync` dengan `rulesetId`, `version`, `ct`; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai dalam ActivateRulesetVersion.
        await _rulesets.ActivateRulesetVersionAsync(rulesetId, version, ct);
        // Mengembalikan membentuk respons HTTP 200 dengan `new CreateRulesetResponse(rulesetId, selectedVersion.RulesetVersionId, version)` sebagai hasil
        // berhasil kepada pemanggil dalam ActivateRulesetVersion; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return Ok(new CreateRulesetResponse(rulesetId, selectedVersion.RulesetVersionId, version));
    // Menutup scope metode ActivateRulesetVersion; bagian berikut berada di luar batas blok tersebut dalam ActivateRulesetVersion.
    }

    // mendaftarkan action untuk metode HTTP DELETE pada rute (”{rulesetId:guid}/versions/{version:int}”).
    [HttpDelete("{rulesetId:guid}/versions/{version:int}")]
    // mewajibkan otorisasi pengguna dengan ketentuan (Roles = ”INSTRUCTOR”).
    [Authorize(Roles = "INSTRUCTOR")]
    // menerapkan metadata `ProducesResponseType(StatusCodes.Status204NoContent)` pada deklarasi berikut agar framework/compiler dapat mengenali
    // pengaturannya.
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    // Mendefinisikan metode `DeleteRulesetVersion` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani delete aturan versi. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `rulesetId` bertipe `Guid`
    // membawa identitas kumpulan aturan permainan; Parameter `version` bertipe `int` membawa nomor versi yang dipakai untuk konsistensi data atau
    // konfigurasi; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
    // permintaan atau aplikasi berhenti.
    public async Task<IActionResult> DeleteRulesetVersion(Guid rulesetId, int version, CancellationToken ct)
    // Membuka scope metode DeleteRulesetVersion; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DeleteRulesetVersion.
    {
        // Memeriksa kebalikan kondisi `TryGetCurrentUserId(out var instructorUserId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DeleteRulesetVersion.
        if (!TryGetCurrentUserId(out var instructorUserId))
        // Membuka scope cabang if untuk kondisi `!TryGetCurrentUserId(out var instructorUserId)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam DeleteRulesetVersion.
        {
            // Mengembalikan membentuk respons HTTP 401 dengan `ApiErrorHelper.BuildError(HttpContext, ”UNAUTHORIZED”, ”Token user tidak valid”)` karena
            // autentikasi tidak terpenuhi kepada pemanggil dalam DeleteRulesetVersion; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        // Menutup scope cabang if untuk kondisi `!TryGetCurrentUserId(out var instructorUserId)`; bagian berikut berada di luar batas blok tersebut dalam
        // DeleteRulesetVersion.
        }

        // Memeriksa pemeriksaan lebih kecil antara `version` dan `1`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DeleteRulesetVersion.
        if (version < 1)
        // Membuka scope cabang if untuk kondisi `version < 1`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DeleteRulesetVersion.
        {
            // Mengembalikan membentuk respons HTTP 400 dengan `ApiErrorHelper.BuildError( HttpContext, ”VALIDATION_ERROR”, ”Path version tidak valid”, new
            // ErrorDetail(”version”, ”OUT_OF_RANGE”))` karena permintaan tidak memenuhi kontrak kepada pemanggil dalam DeleteRulesetVersion; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return BadRequest(ApiErrorHelper.BuildError(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`.
                HttpContext,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Path version tidak valid”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                "Path version tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”version”, ”OUT_OF_RANGE”) sebagai argumen ke `ApiErrorHelper.BuildError`; Meneruskan
                // nilai literal `”version”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”OUT_OF_RANGE”` sebagai argumen ke konstruktor
                // `ErrorDetail`.
                new ErrorDetail("version", "OUT_OF_RANGE")));
        // Menutup scope cabang if untuk kondisi `version < 1`; bagian berikut berada di luar batas blok tersebut dalam DeleteRulesetVersion.
        }

        // Menyiapkan variabel lokal `mutableRuleset` untuk nilai mutable aturan dengan hasil operasi asinkron memanggil `GetMutableInstructorRulesetAsync`
        // dengan `rulesetId`, `instructorUserId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var mutableRuleset = await GetMutableInstructorRulesetAsync(rulesetId, instructorUserId, ct);
        // Memeriksa hasil pencocokan `mutableRuleset.Error` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DeleteRulesetVersion.
        if (mutableRuleset.Error is not null)
        // Membuka scope cabang if untuk kondisi `mutableRuleset.Error is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // DeleteRulesetVersion.
        {
            // Mengembalikan `mutableRuleset.Error` (informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil) kepada pemanggil dalam
            // DeleteRulesetVersion; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return mutableRuleset.Error;
        // Menutup scope cabang if untuk kondisi `mutableRuleset.Error is not null`; bagian berikut berada di luar batas blok tersebut dalam
        // DeleteRulesetVersion.
        }

        // Menyiapkan variabel lokal `selectedVersion` untuk nilai selected versi dengan hasil operasi asinkron memanggil `_rulesets.GetRulesetVersionAsync`
        // dengan `rulesetId`, `version`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var selectedVersion = await _rulesets.GetRulesetVersionAsync(rulesetId, version, ct);
        // Memeriksa hasil pencocokan `selectedVersion` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DeleteRulesetVersion.
        if (selectedVersion is null)
        // Membuka scope cabang if untuk kondisi `selectedVersion is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // DeleteRulesetVersion.
        {
            // Mengembalikan membentuk respons HTTP 404 dengan `ApiErrorHelper.BuildError(HttpContext, ”NOT_FOUND”, ”Ruleset version tidak ditemukan”)` karena
            // sumber daya tidak ditemukan kepada pemanggil dalam DeleteRulesetVersion; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset version tidak ditemukan"));
        // Menutup scope cabang if untuk kondisi `selectedVersion is null`; bagian berikut berada di luar batas blok tersebut dalam DeleteRulesetVersion.
        }

        // Menyiapkan variabel lokal `totalVersions` untuk nilai total versions dengan hasil operasi asinkron memanggil
        // `_rulesets.CountRulesetVersionsAsync` dengan `rulesetId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var totalVersions = await _rulesets.CountRulesetVersionsAsync(rulesetId, ct);
        // Memeriksa pemeriksaan lebih kecil atau sama antara `totalVersions` dan `1`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DeleteRulesetVersion.
        if (totalVersions <= 1)
        // Membuka scope cabang if untuk kondisi `totalVersions <= 1`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // DeleteRulesetVersion.
        {
            // Mengembalikan memanggil `UnprocessableEntity` dengan `ApiErrorHelper.BuildError( HttpContext, ”DOMAIN_RULE_VIOLATION”, ”Versi terakhir tidak
            // dapat dihapus. Hapus ruleset jika tidak lagi diperlukan.”)` kepada pemanggil dalam DeleteRulesetVersion; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`.
                HttpContext,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Versi terakhir tidak dapat dihapus. Hapus ruleset jika tidak lagi diperlukan.”` sebagai argumen ke
                // `ApiErrorHelper.BuildError`.
                "Versi terakhir tidak dapat dihapus. Hapus ruleset jika tidak lagi diperlukan."));
        // Menutup scope cabang if untuk kondisi `totalVersions <= 1`; bagian berikut berada di luar batas blok tersebut dalam DeleteRulesetVersion.
        }

        // Memeriksa membandingkan kesamaan `string` dengan `selectedVersion.Status`, `”ACTIVE”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan
        // mengikuti overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam DeleteRulesetVersion.
        if (string.Equals(selectedVersion.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(selectedVersion.Status, ”ACTIVE”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam DeleteRulesetVersion.
        {
            // Mengembalikan memanggil `UnprocessableEntity` dengan `ApiErrorHelper.BuildError( HttpContext, ”DOMAIN_RULE_VIOLATION”, ”Versi aktif tidak dapat
            // dihapus. Aktifkan versi lain terlebih dahulu.”)` kepada pemanggil dalam DeleteRulesetVersion; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`.
                HttpContext,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Versi aktif tidak dapat dihapus. Aktifkan versi lain terlebih dahulu.”` sebagai argumen ke
                // `ApiErrorHelper.BuildError`.
                "Versi aktif tidak dapat dihapus. Aktifkan versi lain terlebih dahulu."));
        // Menutup scope cabang if untuk kondisi `string.Equals(selectedVersion.Status, ”ACTIVE”, StringComparison.OrdinalIgnoreCase)`; bagian berikut
        // berada di luar batas blok tersebut dalam DeleteRulesetVersion.
        }

        // Menyiapkan variabel lokal `isUsed` untuk nilai berstatus used dengan hasil operasi asinkron memanggil `_rulesets.IsRulesetVersionUsedAsync`
        // dengan `selectedVersion.RulesetVersionId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var isUsed = await _rulesets.IsRulesetVersionUsedAsync(selectedVersion.RulesetVersionId, ct);
        // Memeriksa `isUsed` (nilai berstatus used); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam DeleteRulesetVersion.
        if (isUsed)
        // Membuka scope cabang if untuk kondisi `isUsed`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DeleteRulesetVersion.
        {
            // Mengembalikan memanggil `UnprocessableEntity` dengan `ApiErrorHelper.BuildError( HttpContext, ”DOMAIN_RULE_VIOLATION”, ”Versi ruleset sudah
            // dipakai pada sesi/event sehingga tidak dapat dihapus.”)` kepada pemanggil dalam DeleteRulesetVersion; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return UnprocessableEntity(ApiErrorHelper.BuildError(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`.
                HttpContext,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Versi ruleset sudah dipakai pada sesi/event sehingga tidak dapat dihapus.”` sebagai argumen ke
                // `ApiErrorHelper.BuildError`.
                "Versi ruleset sudah dipakai pada sesi/event sehingga tidak dapat dihapus."));
        // Menutup scope cabang if untuk kondisi `isUsed`; bagian berikut berada di luar batas blok tersebut dalam DeleteRulesetVersion.
        }

        // Menyiapkan variabel lokal `deleted` untuk nilai deleted dengan hasil operasi asinkron memanggil `_rulesets.DeleteRulesetVersionAsync` dengan
        // `rulesetId`, `version`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var deleted = await _rulesets.DeleteRulesetVersionAsync(rulesetId, version, ct);
        // Memeriksa kebalikan kondisi `deleted`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam DeleteRulesetVersion.
        if (!deleted)
        // Membuka scope cabang if untuk kondisi `!deleted`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DeleteRulesetVersion.
        {
            // Mengembalikan membentuk respons HTTP 404 dengan `ApiErrorHelper.BuildError(HttpContext, ”NOT_FOUND”, ”Ruleset version tidak ditemukan”)` karena
            // sumber daya tidak ditemukan kepada pemanggil dalam DeleteRulesetVersion; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset version tidak ditemukan"));
        // Menutup scope cabang if untuk kondisi `!deleted`; bagian berikut berada di luar batas blok tersebut dalam DeleteRulesetVersion.
        }

        // Mengembalikan memanggil `NoContent` dengan tanpa argumen kepada pemanggil dalam DeleteRulesetVersion; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return NoContent();
    // Menutup scope metode DeleteRulesetVersion; bagian berikut berada di luar batas blok tersebut dalam DeleteRulesetVersion.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute controller saat ini.
    [HttpGet]
    // menerapkan metadata `ProducesResponseType(typeof(RulesetListResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar framework/compiler
    // dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(RulesetListResponse), StatusCodes.Status200OK)]
    // Mendefinisikan metode `ListRulesets` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani daftar aturan. async memungkinkan metode
    // menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `ct` bertipe `CancellationToken` membawa sinyal
    // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> ListRulesets(CancellationToken ct)
    // Membuka scope metode ListRulesets; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListRulesets.
    {
        // Memeriksa kebalikan kondisi `TryGetCurrentUserId(out var userId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ListRulesets.
        if (!TryGetCurrentUserId(out var userId))
        // Membuka scope cabang if untuk kondisi `!TryGetCurrentUserId(out var userId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ListRulesets.
        {
            // Mengembalikan membentuk respons HTTP 401 dengan `ApiErrorHelper.BuildError(HttpContext, ”UNAUTHORIZED”, ”Token user tidak valid”)` karena
            // autentikasi tidak terpenuhi kepada pemanggil dalam ListRulesets; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        // Menutup scope cabang if untuk kondisi `!TryGetCurrentUserId(out var userId)`; bagian berikut berada di luar batas blok tersebut dalam
        // ListRulesets.
        }

        // Menyiapkan variabel lokal `role` untuk peran pengguna yang menentukan hak akses dengan memanggil `User.FindFirstValue` dengan `ClaimTypes.Role`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var role = User.FindFirstValue(ClaimTypes.Role);
        // Menyiapkan variabel lokal `items` untuk nilai elemen tanpa nilai awal pada deklarasi ini. Tipe yang dipakai adalah `List<RulesetListItem>`.
        List<RulesetListItem> items;

        // Memeriksa membandingkan kesamaan `string` dengan `role`, `”INSTRUCTOR”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti
        // overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ListRulesets.
        if (string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(role, ”INSTRUCTOR”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam ListRulesets.
        {
            // Memperbarui `items` menggunakan hasil operasi asinkron memanggil `_rulesets.ListRulesetsByInstructorAsync` dengan `userId`, `ct`; await menunggu
            // hasil tanpa memblokir thread selama operasi belum selesai dalam ListRulesets.
            items = await _rulesets.ListRulesetsByInstructorAsync(userId, ct);
        // Menutup scope cabang if untuk kondisi `string.Equals(role, ”INSTRUCTOR”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar
        // batas blok tersebut dalam ListRulesets.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam ListRulesets.
        else if (string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(role, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam ListRulesets.
        {
            // Menyiapkan variabel lokal `playerUserId` untuk nilai pemain pengguna identitas dengan hasil operasi asinkron memanggil
            // `_users.GetPlayerUserIdAsync` dengan `userId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
            // disimpulkan dari ekspresi nilai awal.
            var playerUserId = await _users.GetPlayerUserIdAsync(userId, ct);
            // Memeriksa kebalikan kondisi `playerUserId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ListRulesets.
            if (!playerUserId.HasValue)
            // Membuka scope cabang if untuk kondisi `!playerUserId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListRulesets.
            {
                // Mengembalikan membentuk hasil HTTP dengan kode dan isi `StatusCodes.Status403Forbidden`, `ApiErrorHelper.BuildError(HttpContext, ”FORBIDDEN”,
                // ”Akun PLAYER belum terhubung ke profil pemain”)` kepada pemanggil dalam ListRulesets; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return StatusCode(StatusCodes.Status403Forbidden,
                    // Meneruskan memanggil `ApiErrorHelper.BuildError` dengan `HttpContext`, `”FORBIDDEN”`, `”Akun PLAYER belum terhubung ke profil pemain”` sebagai
                    // argumen ke `StatusCode`; Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke
                    // `ApiErrorHelper.BuildError`; Meneruskan nilai literal `”FORBIDDEN”` sebagai argumen ke `ApiErrorHelper.BuildError`; Meneruskan nilai literal
                    // `”Akun PLAYER belum terhubung ke profil pemain”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                    ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Akun PLAYER belum terhubung ke profil pemain"));
            // Menutup scope cabang if untuk kondisi `!playerUserId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam ListRulesets.
            }

            // Memperbarui `items` menggunakan hasil operasi asinkron memanggil `_rulesets.ListRulesetsByPlayerAsync` dengan `playerUserId.Value`, `ct`; await
            // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ListRulesets.
            items = await _rulesets.ListRulesetsByPlayerAsync(playerUserId.Value, ct);
        // Menutup scope cabang if untuk kondisi `string.Equals(role, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas
        // blok tersebut dalam ListRulesets.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam ListRulesets.
        else
        // Membuka scope cabang else; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListRulesets.
        {
            // Mengembalikan membentuk hasil HTTP dengan kode dan isi `StatusCodes.Status403Forbidden`, `ApiErrorHelper.BuildError(HttpContext, ”FORBIDDEN”,
            // ”Role tidak diizinkan”)` kepada pemanggil dalam ListRulesets; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return StatusCode(StatusCodes.Status403Forbidden,
                // Meneruskan memanggil `ApiErrorHelper.BuildError` dengan `HttpContext`, `”FORBIDDEN”`, `”Role tidak diizinkan”` sebagai argumen ke `StatusCode`;
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`;
                // Meneruskan nilai literal `”FORBIDDEN”` sebagai argumen ke `ApiErrorHelper.BuildError`; Meneruskan nilai literal `”Role tidak diizinkan”` sebagai
                // argumen ke `ApiErrorHelper.BuildError`.
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak diizinkan"));
        // Menutup scope cabang else; bagian berikut berada di luar batas blok tersebut dalam ListRulesets.
        }

        // Menyiapkan variabel lokal `defaults` untuk nilai defaults dengan hasil operasi asinkron memanggil `_rulesets.ListDefaultRulesetsAsync` dengan
        // `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var defaults = await _rulesets.ListDefaultRulesetsAsync(ct);
        // Memperbarui `items` menggunakan mematerialisasi urutan `defaults .Concat(items) .GroupBy(item => item.RulesetId) .Select(group => group.First())`
        // menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori dalam ListRulesets.
        items = defaults
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Concat(items) dalam ListRulesets; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .Concat(items)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(item => item.RulesetId) dalam ListRulesets; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .GroupBy(item => item.RulesetId)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(group => group.First()) dalam ListRulesets; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(group => group.First())
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam ListRulesets; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .ToList();

        // Mengembalikan membentuk respons HTTP 200 dengan `new RulesetListResponse(items)` sebagai hasil berhasil kepada pemanggil dalam ListRulesets;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return Ok(new RulesetListResponse(items));
    // Menutup scope metode ListRulesets; bagian berikut berada di luar batas blok tersebut dalam ListRulesets.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”components/defaults”).
    [HttpGet("components/defaults")]
    // mendaftarkan action untuk metode HTTP GET pada rute (”/api/v1/game-components”).
    [HttpGet("/api/v1/game-components")]
    // menerapkan metadata `ProducesResponseType(typeof(DefaultRulesetComponentsResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(DefaultRulesetComponentsResponse), StatusCodes.Status200OK)]
    // Mendefinisikan metode `ListDefaultRulesetComponents` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani daftar bawaan aturan
    // komponen. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `mode`
    // bertipe `string?` membawa mode permainan yang menentukan kelompok aturan yang digunakan; nilai null diizinkan ketika data opsional belum
    // tersedia; mengambil nilai parameter dari query string URL; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi
    // dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> ListDefaultRulesetComponents([FromQuery] string? mode, CancellationToken ct)
    // Membuka scope metode ListDefaultRulesetComponents; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ListDefaultRulesetComponents.
    {
        // Memeriksa kebalikan kondisi `TryGetCurrentUserId(out _)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ListDefaultRulesetComponents.
        if (!TryGetCurrentUserId(out _))
        // Membuka scope cabang if untuk kondisi `!TryGetCurrentUserId(out _)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ListDefaultRulesetComponents.
        {
            // Mengembalikan membentuk respons HTTP 401 dengan `ApiErrorHelper.BuildError(HttpContext, ”UNAUTHORIZED”, ”Token user tidak valid”)` karena
            // autentikasi tidak terpenuhi kepada pemanggil dalam ListDefaultRulesetComponents; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        // Menutup scope cabang if untuk kondisi `!TryGetCurrentUserId(out _)`; bagian berikut berada di luar batas blok tersebut dalam
        // ListDefaultRulesetComponents.
        }

        // Menyiapkan variabel lokal `role` untuk peran pengguna yang menentukan hak akses dengan memanggil `User.FindFirstValue` dengan `ClaimTypes.Role`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var role = User.FindFirstValue(ClaimTypes.Role);
        // Menyiapkan variabel lokal `isInstructor` untuk nilai berstatus instruktur dengan membandingkan kesamaan `string` dengan `role`, `”INSTRUCTOR”`,
        // `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan comparer yang diberikan. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var isInstructor = string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `isPlayer` untuk nilai berstatus pemain dengan membandingkan kesamaan `string` dengan `role`, `”PLAYER”`,
        // `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan comparer yang diberikan. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var isPlayer = string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase);
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!isInstructor` dan `!isPlayer`; sisi kanan diperiksa hanya jika sisi kiri benar;
        // blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ListDefaultRulesetComponents.
        if (!isInstructor && !isPlayer)
        // Membuka scope cabang if untuk kondisi `!isInstructor && !isPlayer`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ListDefaultRulesetComponents.
        {
            // Mengembalikan membentuk hasil HTTP dengan kode dan isi `StatusCodes.Status403Forbidden`, `ApiErrorHelper.BuildError(HttpContext, ”FORBIDDEN”,
            // ”Role tidak diizinkan”)` kepada pemanggil dalam ListDefaultRulesetComponents; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return StatusCode(StatusCodes.Status403Forbidden,
                // Meneruskan memanggil `ApiErrorHelper.BuildError` dengan `HttpContext`, `”FORBIDDEN”`, `”Role tidak diizinkan”` sebagai argumen ke `StatusCode`;
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`;
                // Meneruskan nilai literal `”FORBIDDEN”` sebagai argumen ke `ApiErrorHelper.BuildError`; Meneruskan nilai literal `”Role tidak diizinkan”` sebagai
                // argumen ke `ApiErrorHelper.BuildError`.
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak diizinkan"));
        // Menutup scope cabang if untuk kondisi `!isInstructor && !isPlayer`; bagian berikut berada di luar batas blok tersebut dalam
        // ListDefaultRulesetComponents.
        }

        // Menyiapkan variabel lokal `modeFilter` untuk nilai mode filter dengan null, yaitu penanda tidak ada nilai. Tipe yang dipakai adalah `string?`.
        string? modeFilter = null;
        // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(mode)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ListDefaultRulesetComponents.
        if (!string.IsNullOrWhiteSpace(mode))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(mode)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ListDefaultRulesetComponents.
        {
            // Memperbarui `modeFilter` menggunakan menormalisasi `mode.Trim()` menjadi huruf besar dengan aturan kultur invariant dalam
            // ListDefaultRulesetComponents.
            modeFilter = mode.Trim().ToUpperInvariant();
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!string.Equals(modeFilter, ”PEMULA”, StringComparison.Ordinal)` dan
            // `!string.Equals(modeFilter, ”MAHIR”, StringComparison.Ordinal)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika
            // kondisi ini bernilai benar dalam ListDefaultRulesetComponents.
            if (!string.Equals(modeFilter, "PEMULA", StringComparison.Ordinal) &&
                // Menggunakan kebalikan kondisi `string.Equals(modeFilter, ”MAHIR”, StringComparison.Ordinal)` sebagai bagian ekspresi yang sedang disusun dalam
                // ListDefaultRulesetComponents.
                !string.Equals(modeFilter, "MAHIR", StringComparison.Ordinal))
            // Membuka scope cabang if untuk kondisi `!string.Equals(modeFilter, ”PEMULA”, StringComparison.Ordinal) && !string.Equals(modeFilter, ”MAHIR”,
            // StringComparison.Ordinal)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListDefaultRulesetComponents.
            {
                // Mengembalikan membentuk respons HTTP 400 dengan `ApiErrorHelper.BuildError( HttpContext, ”VALIDATION_ERROR”, ”Query mode tidak valid”, new
                // ErrorDetail(”mode”, ”INVALID_VALUE”))` karena permintaan tidak memenuhi kontrak kepada pemanggil dalam ListDefaultRulesetComponents; eksekusi
                // jalur ini selesai setelah nilai hasil ditentukan.
                return BadRequest(ApiErrorHelper.BuildError(
                    // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`.
                    HttpContext,
                    // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                    "VALIDATION_ERROR",
                    // Meneruskan nilai literal `”Query mode tidak valid”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                    "Query mode tidak valid",
                    // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”mode”, ”INVALID_VALUE”) sebagai argumen ke `ApiErrorHelper.BuildError`; Meneruskan
                    // nilai literal `”mode”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”INVALID_VALUE”` sebagai argumen ke konstruktor
                    // `ErrorDetail`.
                    new ErrorDetail("mode", "INVALID_VALUE")));
            // Menutup scope cabang if untuk kondisi `!string.Equals(modeFilter, ”PEMULA”, StringComparison.Ordinal) && !string.Equals(modeFilter, ”MAHIR”,
            // StringComparison.Ordinal)`; bagian berikut berada di luar batas blok tersebut dalam ListDefaultRulesetComponents.
            }
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(mode)`; bagian berikut berada di luar batas blok tersebut dalam
        // ListDefaultRulesetComponents.
        }

        // Menyiapkan variabel lokal `defaults` untuk nilai defaults dengan hasil operasi asinkron memanggil `_rulesets.ListDefaultRulesetComponentsAsync`
        // dengan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var defaults = await _rulesets.ListDefaultRulesetComponentsAsync(ct);
        // Menyiapkan variabel lokal `items` untuk nilai elemen dengan objek baru bertipe `List<DefaultRulesetComponentItem>` dengan argumen
        // (defaults.Count). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var items = new List<DefaultRulesetComponentItem>(defaults.Count);

        // Mengulangi setiap elemen `defaults`; elemen saat ini disimpan sebagai `row` bertipe `var` untuk diproses oleh badan loop dalam
        // ListDefaultRulesetComponents.
        foreach (var row in defaults)
        // Membuka scope loop setiap row dari `defaults`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListDefaultRulesetComponents.
        {
            // Memeriksa hasil pencocokan `row.Definition` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ListDefaultRulesetComponents.
            if (row.Definition is null)
            // Membuka scope cabang if untuk kondisi `row.Definition is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ListDefaultRulesetComponents.
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam ListDefaultRulesetComponents.
                continue;
            // Menutup scope cabang if untuk kondisi `row.Definition is null`; bagian berikut berada di luar batas blok tersebut dalam
            // ListDefaultRulesetComponents.
            }

            // Menyiapkan variabel lokal `itemMode` untuk nilai elemen mode dengan hasil pemilihan bersyarat: ketika
            // `string.IsNullOrWhiteSpace(row.Definition.Mode)` benar gunakan `row.Mode`, jika tidak gunakan `row.Definition.Mode`. Tipe variabel disimpulkan
            // dari ekspresi nilai awal.
            var itemMode = string.IsNullOrWhiteSpace(row.Definition.Mode)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: row.Mode dalam ListDefaultRulesetComponents.
                ? row.Mode
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: row.Definition.Mode; dalam ListDefaultRulesetComponents.
                : row.Definition.Mode;

            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!string.IsNullOrWhiteSpace(modeFilter)` dan `!string.Equals(itemMode,
            // modeFilter, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam ListDefaultRulesetComponents.
            if (!string.IsNullOrWhiteSpace(modeFilter) &&
                // Menggunakan kebalikan kondisi `string.Equals(itemMode, modeFilter, StringComparison.OrdinalIgnoreCase)` sebagai bagian ekspresi yang sedang
                // disusun dalam ListDefaultRulesetComponents.
                !string.Equals(itemMode, modeFilter, StringComparison.OrdinalIgnoreCase))
            // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(modeFilter) && !string.Equals(itemMode, modeFilter,
            // StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ListDefaultRulesetComponents.
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam ListDefaultRulesetComponents.
                continue;
            // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(modeFilter) && !string.Equals(itemMode, modeFilter,
            // StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok tersebut dalam ListDefaultRulesetComponents.
            }

            // Menjalankan menambahkan `new DefaultRulesetComponentItem( row.RulesetId, row.Name, row.Description, row.RulesetVersionId, row.Version, itemMode,
            // CloneDefinition( row.Definition, row.Definition.Actions...` ke `items` dalam ListDefaultRulesetComponents.
            items.Add(new DefaultRulesetComponentItem(
                // Meneruskan `row.RulesetId` (identitas kumpulan aturan permainan) sebagai argumen ke konstruktor `DefaultRulesetComponentItem`.
                row.RulesetId,
                // Meneruskan `row.Name` (nilai nama) sebagai argumen ke konstruktor `DefaultRulesetComponentItem`.
                row.Name,
                // Meneruskan `row.Description` (nilai description) sebagai argumen ke konstruktor `DefaultRulesetComponentItem`.
                row.Description,
                // Meneruskan `row.RulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
                // konstruktor `DefaultRulesetComponentItem`.
                row.RulesetVersionId,
                // Meneruskan `row.Version` (nomor versi yang dipakai untuk konsistensi data atau konfigurasi) sebagai argumen ke konstruktor
                // `DefaultRulesetComponentItem`.
                row.Version,
                // Meneruskan `itemMode` (nilai elemen mode) sebagai argumen ke konstruktor `DefaultRulesetComponentItem`.
                itemMode,
                // Meneruskan memanggil `CloneDefinition` dengan `row.Definition`, `row.Definition.Actions` sebagai argumen ke konstruktor
                // `DefaultRulesetComponentItem`.
                CloneDefinition(
                    // Meneruskan `row.Definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai argumen ke `CloneDefinition`.
                    row.Definition,
                    // Meneruskan `row.Definition.Actions` (nilai aksi) sebagai argumen ke `CloneDefinition`.
                    row.Definition.Actions)));
        // Menutup scope loop setiap row dari `defaults`; bagian berikut berada di luar batas blok tersebut dalam ListDefaultRulesetComponents.
        }

        // Mengembalikan membentuk respons HTTP 200 dengan `new DefaultRulesetComponentsResponse(items)` sebagai hasil berhasil kepada pemanggil dalam
        // ListDefaultRulesetComponents; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return Ok(new DefaultRulesetComponentsResponse(items));
    // Menutup scope metode ListDefaultRulesetComponents; bagian berikut berada di luar batas blok tersebut dalam ListDefaultRulesetComponents.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”{rulesetId:guid}”).
    [HttpGet("{rulesetId:guid}")]
    // menerapkan metadata `ProducesResponseType(typeof(RulesetDetailResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar framework/compiler
    // dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(RulesetDetailResponse), StatusCodes.Status200OK)]
    // Mendefinisikan metode `GetRulesetDetail` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani get aturan detail. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `rulesetId` bertipe `Guid` membawa
    // identitas kumpulan aturan permainan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika
    // pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> GetRulesetDetail(Guid rulesetId, CancellationToken ct)
    // Membuka scope metode GetRulesetDetail; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetDetail.
    {
        // Memeriksa kebalikan kondisi `TryGetCurrentUserId(out var userId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetRulesetDetail.
        if (!TryGetCurrentUserId(out var userId))
        // Membuka scope cabang if untuk kondisi `!TryGetCurrentUserId(out var userId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetRulesetDetail.
        {
            // Mengembalikan membentuk respons HTTP 401 dengan `ApiErrorHelper.BuildError(HttpContext, ”UNAUTHORIZED”, ”Token user tidak valid”)` karena
            // autentikasi tidak terpenuhi kepada pemanggil dalam GetRulesetDetail; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        // Menutup scope cabang if untuk kondisi `!TryGetCurrentUserId(out var userId)`; bagian berikut berada di luar batas blok tersebut dalam
        // GetRulesetDetail.
        }

        // Menyiapkan variabel lokal `role` untuk peran pengguna yang menentukan hak akses dengan memanggil `User.FindFirstValue` dengan `ClaimTypes.Role`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var role = User.FindFirstValue(ClaimTypes.Role);
        // Menyiapkan variabel lokal `ruleset` untuk nilai aturan tanpa nilai awal pada deklarasi ini. Tipe yang dipakai adalah `RulesetDb?`.
        RulesetDb? ruleset;
        // Memeriksa membandingkan kesamaan `string` dengan `role`, `”INSTRUCTOR”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti
        // overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam GetRulesetDetail.
        if (string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(role, ”INSTRUCTOR”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam GetRulesetDetail.
        {
            // Memperbarui `ruleset` menggunakan hasil operasi asinkron memanggil `_rulesets.GetRulesetForInstructorAsync` dengan `rulesetId`, `userId`, `ct`;
            // await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetRulesetDetail.
            ruleset = await _rulesets.GetRulesetForInstructorAsync(rulesetId, userId, ct);
            // Memeriksa hasil pencocokan `ruleset` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam GetRulesetDetail.
            if (ruleset is null)
            // Membuka scope cabang if untuk kondisi `ruleset is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetDetail.
            {
                // Memperbarui `ruleset` menggunakan hasil operasi asinkron memanggil `_rulesets.GetDefaultSeedRulesetAsync` dengan `rulesetId`, `ct`; await
                // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetRulesetDetail.
                ruleset = await _rulesets.GetDefaultSeedRulesetAsync(rulesetId, ct);
            // Menutup scope cabang if untuk kondisi `ruleset is null`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetDetail.
            }
        // Menutup scope cabang if untuk kondisi `string.Equals(role, ”INSTRUCTOR”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar
        // batas blok tersebut dalam GetRulesetDetail.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam GetRulesetDetail.
        else if (string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(role, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam GetRulesetDetail.
        {
            // Menyiapkan variabel lokal `playerUserId` untuk nilai pemain pengguna identitas dengan hasil operasi asinkron memanggil
            // `_users.GetPlayerUserIdAsync` dengan `userId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
            // disimpulkan dari ekspresi nilai awal.
            var playerUserId = await _users.GetPlayerUserIdAsync(userId, ct);
            // Memeriksa kebalikan kondisi `playerUserId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam GetRulesetDetail.
            if (!playerUserId.HasValue)
            // Membuka scope cabang if untuk kondisi `!playerUserId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // GetRulesetDetail.
            {
                // Mengembalikan membentuk hasil HTTP dengan kode dan isi `StatusCodes.Status403Forbidden`, `ApiErrorHelper.BuildError(HttpContext, ”FORBIDDEN”,
                // ”Akun PLAYER belum terhubung ke profil pemain”)` kepada pemanggil dalam GetRulesetDetail; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return StatusCode(StatusCodes.Status403Forbidden,
                    // Meneruskan memanggil `ApiErrorHelper.BuildError` dengan `HttpContext`, `”FORBIDDEN”`, `”Akun PLAYER belum terhubung ke profil pemain”` sebagai
                    // argumen ke `StatusCode`; Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke
                    // `ApiErrorHelper.BuildError`; Meneruskan nilai literal `”FORBIDDEN”` sebagai argumen ke `ApiErrorHelper.BuildError`; Meneruskan nilai literal
                    // `”Akun PLAYER belum terhubung ke profil pemain”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                    ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Akun PLAYER belum terhubung ke profil pemain"));
            // Menutup scope cabang if untuk kondisi `!playerUserId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetDetail.
            }

            // Memperbarui `ruleset` menggunakan hasil operasi asinkron memanggil `_rulesets.GetRulesetForPlayerAsync` dengan `rulesetId`, `playerUserId.Value`,
            // `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetRulesetDetail.
            ruleset = await _rulesets.GetRulesetForPlayerAsync(rulesetId, playerUserId.Value, ct);
            // Memeriksa hasil pencocokan `ruleset` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam GetRulesetDetail.
            if (ruleset is null)
            // Membuka scope cabang if untuk kondisi `ruleset is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetDetail.
            {
                // Memperbarui `ruleset` menggunakan hasil operasi asinkron memanggil `_rulesets.GetDefaultSeedRulesetAsync` dengan `rulesetId`, `ct`; await
                // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetRulesetDetail.
                ruleset = await _rulesets.GetDefaultSeedRulesetAsync(rulesetId, ct);
            // Menutup scope cabang if untuk kondisi `ruleset is null`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetDetail.
            }
        // Menutup scope cabang if untuk kondisi `string.Equals(role, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas
        // blok tersebut dalam GetRulesetDetail.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam GetRulesetDetail.
        else
        // Membuka scope cabang else; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetDetail.
        {
            // Mengembalikan membentuk hasil HTTP dengan kode dan isi `StatusCodes.Status403Forbidden`, `ApiErrorHelper.BuildError(HttpContext, ”FORBIDDEN”,
            // ”Role tidak diizinkan”)` kepada pemanggil dalam GetRulesetDetail; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return StatusCode(StatusCodes.Status403Forbidden,
                // Meneruskan memanggil `ApiErrorHelper.BuildError` dengan `HttpContext`, `”FORBIDDEN”`, `”Role tidak diizinkan”` sebagai argumen ke `StatusCode`;
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`;
                // Meneruskan nilai literal `”FORBIDDEN”` sebagai argumen ke `ApiErrorHelper.BuildError`; Meneruskan nilai literal `”Role tidak diizinkan”` sebagai
                // argumen ke `ApiErrorHelper.BuildError`.
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak diizinkan"));
        // Menutup scope cabang else; bagian berikut berada di luar batas blok tersebut dalam GetRulesetDetail.
        }

        // Memeriksa hasil pencocokan `ruleset` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam GetRulesetDetail.
        if (ruleset is null)
        // Membuka scope cabang if untuk kondisi `ruleset is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetDetail.
        {
            // Mengembalikan membentuk respons HTTP 404 dengan `ApiErrorHelper.BuildError(HttpContext, ”NOT_FOUND”, ”Ruleset tidak ditemukan”)` karena sumber
            // daya tidak ditemukan kepada pemanggil dalam GetRulesetDetail; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan"));
        // Menutup scope cabang if untuk kondisi `ruleset is null`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetDetail.
        }

        // Menyiapkan variabel lokal `versions` untuk nilai versions dengan hasil operasi asinkron memanggil `_rulesets.ListRulesetVersionsAsync` dengan
        // `rulesetId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var versions = await _rulesets.ListRulesetVersionsAsync(rulesetId, ct);
        // Menyiapkan variabel lokal `versionItems` untuk nilai versi elemen dengan mematerialisasi urutan `versions.Select(v => new RulesetVersionItem(
        // v.RulesetVersionId, v.Version, v.Status, v.CreatedAt))` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var versionItems = versions.Select(v => new RulesetVersionItem(
            // Meneruskan `v.RulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
            // konstruktor `RulesetVersionItem`.
            v.RulesetVersionId,
            // Meneruskan `v.Version` (nomor versi yang dipakai untuk konsistensi data atau konfigurasi) sebagai argumen ke konstruktor `RulesetVersionItem`.
            v.Version,
            // Meneruskan `v.Status` (nilai status) sebagai argumen ke konstruktor `RulesetVersionItem`.
            v.Status,
            // Meneruskan `v.CreatedAt` (nilai created at) sebagai argumen ke konstruktor `RulesetVersionItem`.
            v.CreatedAt)).ToList();

        // Menyiapkan variabel lokal `selectedRulesetVersionId` untuk nilai selected aturan versi identitas dengan null, yaitu penanda tidak ada nilai. Tipe
        // yang dipakai adalah `Guid?`.
        Guid? selectedRulesetVersionId = null;
        // Menyiapkan variabel lokal `selectedVersionNumber` untuk nilai selected versi number dengan null, yaitu penanda tidak ada nilai. Tipe yang dipakai
        // adalah `int?`.
        int? selectedVersionNumber = null;
        // Menyiapkan variabel lokal `selectedMode` untuk nilai selected mode dengan null, yaitu penanda tidak ada nilai. Tipe yang dipakai adalah
        // `string?`.
        string? selectedMode = null;
        // Menyiapkan variabel lokal `selectedDefinition` untuk nilai selected definisi dengan null, yaitu penanda tidak ada nilai. Tipe yang dipakai adalah
        // `RulesetDefinitionDto?`.
        RulesetDefinitionDto? selectedDefinition = null;
        // Menyiapkan variabel lokal `latest` untuk nilai latest dengan mengambil elemen pertama `versions`; jika tidak ada, gunakan nilai default tipe
        // hasil. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var latest = versions.FirstOrDefault();
        // Memeriksa hasil pencocokan `latest?.Definition` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetRulesetDetail.
        if (latest?.Definition is not null)
        // Membuka scope cabang if untuk kondisi `latest?.Definition is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetRulesetDetail.
        {
            // Memperbarui `selectedRulesetVersionId` menggunakan `latest.RulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi
            // aturan yang tepat) dalam GetRulesetDetail.
            selectedRulesetVersionId = latest.RulesetVersionId;
            // Memperbarui `selectedVersionNumber` menggunakan `latest.Version` (nomor versi yang dipakai untuk konsistensi data atau konfigurasi) dalam
            // GetRulesetDetail.
            selectedVersionNumber = latest.Version;
            // Memperbarui `selectedMode` menggunakan `latest.Mode` (mode permainan yang menentukan kelompok aturan yang digunakan) dalam GetRulesetDetail.
            selectedMode = latest.Mode;
            // Memperbarui `selectedDefinition` menggunakan memanggil `CloneDefinition` dengan `latest.Definition`, `latest.Definition.Actions` dalam
            // GetRulesetDetail.
            selectedDefinition = CloneDefinition(
                // Meneruskan `latest.Definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai argumen ke `CloneDefinition`.
                latest.Definition,
                // Meneruskan `latest.Definition.Actions` (nilai aksi) sebagai argumen ke `CloneDefinition`.
                latest.Definition.Actions);
        // Menutup scope cabang if untuk kondisi `latest?.Definition is not null`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetDetail.
        }

        // Menyiapkan variabel lokal `isDefault` untuk nilai berstatus bawaan dengan hasil pencocokan `ruleset.InstructorUserId` dengan pola `null`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var isDefault = ruleset.InstructorUserId is null;
        // Menyiapkan variabel lokal `isLockedBySession` untuk nilai berstatus locked berdasarkan sesi dengan hasil operasi asinkron memanggil
        // `_rulesets.IsRulesetLockedBySessionAsync` dengan `rulesetId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var isLockedBySession = await _rulesets.IsRulesetLockedBySessionAsync(rulesetId, ct);
        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan objek baru bertipe
        // `RulesetDetailResponse` dengan argumen ( ruleset.RulesetId, ruleset.Name, ruleset.Description, versionItems, selectedRulesetVersionId,
        // selectedVersionNumber, selectedMode, selectedDefinition, isDefau.... Tipe variabel disimpulkan dari ekspresi nilai awal.
        var response = new RulesetDetailResponse(
            // Meneruskan `ruleset.RulesetId` (identitas kumpulan aturan permainan) sebagai argumen ke konstruktor `RulesetDetailResponse`.
            ruleset.RulesetId,
            // Meneruskan `ruleset.Name` (nilai nama) sebagai argumen ke konstruktor `RulesetDetailResponse`.
            ruleset.Name,
            // Meneruskan `ruleset.Description` (nilai description) sebagai argumen ke konstruktor `RulesetDetailResponse`.
            ruleset.Description,
            // Meneruskan `versionItems` (nilai versi elemen) sebagai argumen ke konstruktor `RulesetDetailResponse`.
            versionItems,
            // Meneruskan `selectedRulesetVersionId` (nilai selected aturan versi identitas) sebagai argumen ke konstruktor `RulesetDetailResponse`.
            selectedRulesetVersionId,
            // Meneruskan `selectedVersionNumber` (nilai selected versi number) sebagai argumen ke konstruktor `RulesetDetailResponse`.
            selectedVersionNumber,
            // Meneruskan `selectedMode` (nilai selected mode) sebagai argumen ke konstruktor `RulesetDetailResponse`.
            selectedMode,
            // Meneruskan `selectedDefinition` (nilai selected definisi) sebagai argumen ke konstruktor `RulesetDetailResponse`.
            selectedDefinition,
            // Meneruskan `isDefault` (nilai berstatus bawaan) sebagai argumen ke konstruktor `RulesetDetailResponse`.
            isDefault,
            // Meneruskan `isLockedBySession` (nilai berstatus locked berdasarkan sesi) sebagai argumen ke konstruktor `RulesetDetailResponse`.
            isLockedBySession);

        // Mengembalikan membentuk respons HTTP 200 dengan `response` sebagai hasil berhasil kepada pemanggil dalam GetRulesetDetail; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return Ok(response);
    // Menutup scope metode GetRulesetDetail; bagian berikut berada di luar batas blok tersebut dalam GetRulesetDetail.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”{rulesetId:guid}/components”).
    [HttpGet("{rulesetId:guid}/components")]
    // menerapkan metadata `ProducesResponseType(typeof(RulesetComponentsResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar
    // framework/compiler dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(RulesetComponentsResponse), StatusCodes.Status200OK)]
    // Mendefinisikan metode `GetRulesetComponents` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani get aturan komponen. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `rulesetId` bertipe `Guid`
    // membawa identitas kumpulan aturan permainan; Parameter `version` bertipe `int?` membawa nomor versi yang dipakai untuk konsistensi data atau
    // konfigurasi; nilai null diizinkan ketika data opsional belum tersedia; mengambil nilai parameter dari query string URL; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> GetRulesetComponents(Guid rulesetId, [FromQuery] int? version, CancellationToken ct)
    // Membuka scope metode GetRulesetComponents; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetComponents.
    {
        // Memeriksa kebalikan kondisi `TryGetCurrentUserId(out var userId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetRulesetComponents.
        if (!TryGetCurrentUserId(out var userId))
        // Membuka scope cabang if untuk kondisi `!TryGetCurrentUserId(out var userId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetRulesetComponents.
        {
            // Mengembalikan membentuk respons HTTP 401 dengan `ApiErrorHelper.BuildError(HttpContext, ”UNAUTHORIZED”, ”Token user tidak valid”)` karena
            // autentikasi tidak terpenuhi kepada pemanggil dalam GetRulesetComponents; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        // Menutup scope cabang if untuk kondisi `!TryGetCurrentUserId(out var userId)`; bagian berikut berada di luar batas blok tersebut dalam
        // GetRulesetComponents.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `version.HasValue` dan `version.Value < 1`; sisi kanan diperiksa hanya jika sisi
        // kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam GetRulesetComponents.
        if (version.HasValue && version.Value < 1)
        // Membuka scope cabang if untuk kondisi `version.HasValue && version.Value < 1`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetRulesetComponents.
        {
            // Mengembalikan membentuk respons HTTP 400 dengan `ApiErrorHelper.BuildError( HttpContext, ”VALIDATION_ERROR”, ”Query version tidak valid”, new
            // ErrorDetail(”version”, ”OUT_OF_RANGE”))` karena permintaan tidak memenuhi kontrak kepada pemanggil dalam GetRulesetComponents; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return BadRequest(ApiErrorHelper.BuildError(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`.
                HttpContext,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Query version tidak valid”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                "Query version tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”version”, ”OUT_OF_RANGE”) sebagai argumen ke `ApiErrorHelper.BuildError`; Meneruskan
                // nilai literal `”version”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”OUT_OF_RANGE”` sebagai argumen ke konstruktor
                // `ErrorDetail`.
                new ErrorDetail("version", "OUT_OF_RANGE")));
        // Menutup scope cabang if untuk kondisi `version.HasValue && version.Value < 1`; bagian berikut berada di luar batas blok tersebut dalam
        // GetRulesetComponents.
        }

        // Menyiapkan variabel lokal `role` untuk peran pengguna yang menentukan hak akses dengan memanggil `User.FindFirstValue` dengan `ClaimTypes.Role`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var role = User.FindFirstValue(ClaimTypes.Role);
        // Menyiapkan variabel lokal `ruleset` untuk nilai aturan tanpa nilai awal pada deklarasi ini. Tipe yang dipakai adalah `RulesetDb?`.
        RulesetDb? ruleset;
        // Memeriksa membandingkan kesamaan `string` dengan `role`, `”INSTRUCTOR”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti
        // overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam GetRulesetComponents.
        if (string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(role, ”INSTRUCTOR”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam GetRulesetComponents.
        {
            // Memperbarui `ruleset` menggunakan hasil operasi asinkron memanggil `_rulesets.GetRulesetForInstructorAsync` dengan `rulesetId`, `userId`, `ct`;
            // await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetRulesetComponents.
            ruleset = await _rulesets.GetRulesetForInstructorAsync(rulesetId, userId, ct);
            // Memeriksa hasil pencocokan `ruleset` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam GetRulesetComponents.
            if (ruleset is null)
            // Membuka scope cabang if untuk kondisi `ruleset is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetComponents.
            {
                // Memperbarui `ruleset` menggunakan hasil operasi asinkron memanggil `_rulesets.GetDefaultSeedRulesetAsync` dengan `rulesetId`, `ct`; await
                // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetRulesetComponents.
                ruleset = await _rulesets.GetDefaultSeedRulesetAsync(rulesetId, ct);
            // Menutup scope cabang if untuk kondisi `ruleset is null`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetComponents.
            }
        // Menutup scope cabang if untuk kondisi `string.Equals(role, ”INSTRUCTOR”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar
        // batas blok tersebut dalam GetRulesetComponents.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam GetRulesetComponents.
        else if (string.Equals(role, "PLAYER", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(role, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam GetRulesetComponents.
        {
            // Menyiapkan variabel lokal `playerUserId` untuk nilai pemain pengguna identitas dengan hasil operasi asinkron memanggil
            // `_users.GetPlayerUserIdAsync` dengan `userId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
            // disimpulkan dari ekspresi nilai awal.
            var playerUserId = await _users.GetPlayerUserIdAsync(userId, ct);
            // Memeriksa kebalikan kondisi `playerUserId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam GetRulesetComponents.
            if (!playerUserId.HasValue)
            // Membuka scope cabang if untuk kondisi `!playerUserId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // GetRulesetComponents.
            {
                // Mengembalikan membentuk hasil HTTP dengan kode dan isi `StatusCodes.Status403Forbidden`, `ApiErrorHelper.BuildError(HttpContext, ”FORBIDDEN”,
                // ”Akun PLAYER belum terhubung ke profil pemain”)` kepada pemanggil dalam GetRulesetComponents; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return StatusCode(StatusCodes.Status403Forbidden,
                    // Meneruskan memanggil `ApiErrorHelper.BuildError` dengan `HttpContext`, `”FORBIDDEN”`, `”Akun PLAYER belum terhubung ke profil pemain”` sebagai
                    // argumen ke `StatusCode`; Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke
                    // `ApiErrorHelper.BuildError`; Meneruskan nilai literal `”FORBIDDEN”` sebagai argumen ke `ApiErrorHelper.BuildError`; Meneruskan nilai literal
                    // `”Akun PLAYER belum terhubung ke profil pemain”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                    ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Akun PLAYER belum terhubung ke profil pemain"));
            // Menutup scope cabang if untuk kondisi `!playerUserId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetComponents.
            }

            // Memperbarui `ruleset` menggunakan hasil operasi asinkron memanggil `_rulesets.GetRulesetForPlayerAsync` dengan `rulesetId`, `playerUserId.Value`,
            // `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetRulesetComponents.
            ruleset = await _rulesets.GetRulesetForPlayerAsync(rulesetId, playerUserId.Value, ct);
            // Memeriksa hasil pencocokan `ruleset` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam GetRulesetComponents.
            if (ruleset is null)
            // Membuka scope cabang if untuk kondisi `ruleset is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetComponents.
            {
                // Memperbarui `ruleset` menggunakan hasil operasi asinkron memanggil `_rulesets.GetDefaultSeedRulesetAsync` dengan `rulesetId`, `ct`; await
                // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetRulesetComponents.
                ruleset = await _rulesets.GetDefaultSeedRulesetAsync(rulesetId, ct);
            // Menutup scope cabang if untuk kondisi `ruleset is null`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetComponents.
            }
        // Menutup scope cabang if untuk kondisi `string.Equals(role, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas
        // blok tersebut dalam GetRulesetComponents.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam GetRulesetComponents.
        else
        // Membuka scope cabang else; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetComponents.
        {
            // Mengembalikan membentuk hasil HTTP dengan kode dan isi `StatusCodes.Status403Forbidden`, `ApiErrorHelper.BuildError(HttpContext, ”FORBIDDEN”,
            // ”Role tidak diizinkan”)` kepada pemanggil dalam GetRulesetComponents; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return StatusCode(StatusCodes.Status403Forbidden,
                // Meneruskan memanggil `ApiErrorHelper.BuildError` dengan `HttpContext`, `”FORBIDDEN”`, `”Role tidak diizinkan”` sebagai argumen ke `StatusCode`;
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`;
                // Meneruskan nilai literal `”FORBIDDEN”` sebagai argumen ke `ApiErrorHelper.BuildError`; Meneruskan nilai literal `”Role tidak diizinkan”` sebagai
                // argumen ke `ApiErrorHelper.BuildError`.
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Role tidak diizinkan"));
        // Menutup scope cabang else; bagian berikut berada di luar batas blok tersebut dalam GetRulesetComponents.
        }

        // Memeriksa hasil pencocokan `ruleset` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam GetRulesetComponents.
        if (ruleset is null)
        // Membuka scope cabang if untuk kondisi `ruleset is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetComponents.
        {
            // Mengembalikan membentuk respons HTTP 404 dengan `ApiErrorHelper.BuildError(HttpContext, ”NOT_FOUND”, ”Ruleset tidak ditemukan”)` karena sumber
            // daya tidak ditemukan kepada pemanggil dalam GetRulesetComponents; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan"));
        // Menutup scope cabang if untuk kondisi `ruleset is null`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetComponents.
        }

        // Menyiapkan variabel lokal `selectedVersion` untuk nilai selected versi tanpa nilai awal pada deklarasi ini. Tipe yang dipakai adalah
        // `RulesetVersionDb?`.
        RulesetVersionDb? selectedVersion;
        // Memeriksa `version.HasValue`, yaitu penanda bahwa nilai nullable tidak kosong; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetRulesetComponents.
        if (version.HasValue)
        // Membuka scope cabang if untuk kondisi `version.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetComponents.
        {
            // Memperbarui `selectedVersion` menggunakan hasil operasi asinkron memanggil `_rulesets.GetRulesetVersionAsync` dengan `rulesetId`,
            // `version.Value`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetRulesetComponents.
            selectedVersion = await _rulesets.GetRulesetVersionAsync(rulesetId, version.Value, ct);
        // Menutup scope cabang if untuk kondisi `version.HasValue`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetComponents.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam GetRulesetComponents.
        else
        // Membuka scope cabang else; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetComponents.
        {
            // Memperbarui `selectedVersion` menggunakan `await _rulesets.GetLatestActiveVersionAsync(rulesetId, ct)` bila tidak null; jika null gunakan `await
            // _rulesets.GetLatestVersionAsync(rulesetId, ct)` sebagai nilai pengganti dalam GetRulesetComponents.
            selectedVersion = await _rulesets.GetLatestActiveVersionAsync(rulesetId, ct)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: await _rulesets.GetLatestVersionAsync(rulesetId, ct); dalam
                // GetRulesetComponents.
                ?? await _rulesets.GetLatestVersionAsync(rulesetId, ct);
        // Menutup scope cabang else; bagian berikut berada di luar batas blok tersebut dalam GetRulesetComponents.
        }

        // Memeriksa hasil pencocokan `selectedVersion` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetRulesetComponents.
        if (selectedVersion is null)
        // Membuka scope cabang if untuk kondisi `selectedVersion is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetRulesetComponents.
        {
            // Mengembalikan membentuk respons HTTP 404 dengan `ApiErrorHelper.BuildError(HttpContext, ”NOT_FOUND”, ”Ruleset version tidak ditemukan”)` karena
            // sumber daya tidak ditemukan kepada pemanggil dalam GetRulesetComponents; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset version tidak ditemukan"));
        // Menutup scope cabang if untuk kondisi `selectedVersion is null`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetComponents.
        }

        // Memeriksa hasil pencocokan `selectedVersion.Definition` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetRulesetComponents.
        if (selectedVersion.Definition is null)
        // Membuka scope cabang if untuk kondisi `selectedVersion.Definition is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetRulesetComponents.
        {
            // Mengembalikan membentuk respons HTTP 404 dengan `ApiErrorHelper.BuildError(HttpContext, ”NOT_FOUND”, ”Definition ruleset tidak ditemukan”)`
            // karena sumber daya tidak ditemukan kepada pemanggil dalam GetRulesetComponents; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Definition ruleset tidak ditemukan"));
        // Menutup scope cabang if untuk kondisi `selectedVersion.Definition is null`; bagian berikut berada di luar batas blok tersebut dalam
        // GetRulesetComponents.
        }

        // Menyiapkan variabel lokal `definition` untuk definisi terstruktur komponen serta parameter aturan permainan dengan memanggil `CloneDefinition`
        // dengan `selectedVersion.Definition`, `selectedVersion.Definition.Actions`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var definition = CloneDefinition(
            // Meneruskan `selectedVersion.Definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai argumen ke `CloneDefinition`.
            selectedVersion.Definition,
            // Meneruskan `selectedVersion.Definition.Actions` (nilai aksi) sebagai argumen ke `CloneDefinition`.
            selectedVersion.Definition.Actions);

        // Mengembalikan membentuk respons HTTP 200 dengan `new RulesetComponentsResponse( rulesetId, selectedVersion.RulesetVersionId,
        // selectedVersion.Version, selectedVersion.Mode, definition)` sebagai hasil berhasil kepada pemanggil dalam GetRulesetComponents; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return Ok(new RulesetComponentsResponse(
            // Meneruskan `rulesetId` (identitas kumpulan aturan permainan) sebagai argumen ke konstruktor `RulesetComponentsResponse`.
            rulesetId,
            // Meneruskan `selectedVersion.RulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen
            // ke konstruktor `RulesetComponentsResponse`.
            selectedVersion.RulesetVersionId,
            // Meneruskan `selectedVersion.Version` (nomor versi yang dipakai untuk konsistensi data atau konfigurasi) sebagai argumen ke konstruktor
            // `RulesetComponentsResponse`.
            selectedVersion.Version,
            // Meneruskan `selectedVersion.Mode` (mode permainan yang menentukan kelompok aturan yang digunakan) sebagai argumen ke konstruktor
            // `RulesetComponentsResponse`.
            selectedVersion.Mode,
            // Meneruskan `definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai argumen ke konstruktor
            // `RulesetComponentsResponse`.
            definition));
    // Menutup scope metode GetRulesetComponents; bagian berikut berada di luar batas blok tersebut dalam GetRulesetComponents.
    }

    // Mendefinisikan metode `IsRulesetConfigHashUniqueViolation` dengan hasil bertipe `bool`; operasi ini menangani berstatus aturan konfigurasi hash
    // unique violation. Masukan: Parameter `ex` bertipe `PostgresException` membawa nilai ex.
    private static bool IsRulesetConfigHashUniqueViolation(PostgresException ex)
    // Membuka scope metode IsRulesetConfigHashUniqueViolation; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // IsRulesetConfigHashUniqueViolation.
    {
        // Mengembalikan gabungan syarat OR: setidaknya satu kondisi wajib benar antara `string.Equals(ex.ConstraintName,
        // ”uq_ruleset_versions_ruleset_config_hash”, StringComparison.Ordinal)` dan `string.Equals(ex.ConstraintName,
        // ”ruleset_versions_ruleset_id_config_hash_key”, StringComparison.Ordinal)`; sisi kanan diperiksa hanya jika sisi kiri salah kepada pemanggil dalam
        // IsRulesetConfigHashUniqueViolation; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return string.Equals(ex.ConstraintName, "uq_ruleset_versions_ruleset_config_hash", StringComparison.Ordinal) ||
               // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `ex.ConstraintName`, `”ruleset_versions_ruleset_id_config_hash_key”`,
               // `StringComparison.Ordinal`; aturan perbandingan mengikuti overload dan comparer yang diberikan dalam IsRulesetConfigHashUniqueViolation.
               string.Equals(ex.ConstraintName, "ruleset_versions_ruleset_id_config_hash_key", StringComparison.Ordinal);
    // Menutup scope metode IsRulesetConfigHashUniqueViolation; bagian berikut berada di luar batas blok tersebut dalam
    // IsRulesetConfigHashUniqueViolation.
    }

    // mendaftarkan action untuk metode HTTP DELETE pada rute (”{rulesetId:guid}”).
    [HttpDelete("{rulesetId:guid}")]
    // mewajibkan otorisasi pengguna dengan ketentuan (Roles = ”INSTRUCTOR”).
    [Authorize(Roles = "INSTRUCTOR")]
    // menerapkan metadata `ProducesResponseType(StatusCodes.Status204NoContent)` pada deklarasi berikut agar framework/compiler dapat mengenali
    // pengaturannya.
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    // Mendefinisikan metode `DeleteRuleset` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani delete aturan. async memungkinkan metode
    // menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `rulesetId` bertipe `Guid` membawa identitas
    // kumpulan aturan permainan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> DeleteRuleset(Guid rulesetId, CancellationToken ct)
    // Membuka scope metode DeleteRuleset; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DeleteRuleset.
    {
        // Memeriksa kebalikan kondisi `TryGetCurrentUserId(out var instructorUserId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DeleteRuleset.
        if (!TryGetCurrentUserId(out var instructorUserId))
        // Membuka scope cabang if untuk kondisi `!TryGetCurrentUserId(out var instructorUserId)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam DeleteRuleset.
        {
            // Mengembalikan membentuk respons HTTP 401 dengan `ApiErrorHelper.BuildError(HttpContext, ”UNAUTHORIZED”, ”Token user tidak valid”)` karena
            // autentikasi tidak terpenuhi kepada pemanggil dalam DeleteRuleset; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "UNAUTHORIZED", "Token user tidak valid"));
        // Menutup scope cabang if untuk kondisi `!TryGetCurrentUserId(out var instructorUserId)`; bagian berikut berada di luar batas blok tersebut dalam
        // DeleteRuleset.
        }

        // Menyiapkan variabel lokal `mutableRuleset` untuk nilai mutable aturan dengan hasil operasi asinkron memanggil `GetMutableInstructorRulesetAsync`
        // dengan `rulesetId`, `instructorUserId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var mutableRuleset = await GetMutableInstructorRulesetAsync(rulesetId, instructorUserId, ct);
        // Memeriksa hasil pencocokan `mutableRuleset.Error` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DeleteRuleset.
        if (mutableRuleset.Error is not null)
        // Membuka scope cabang if untuk kondisi `mutableRuleset.Error is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // DeleteRuleset.
        {
            // Mengembalikan `mutableRuleset.Error` (informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil) kepada pemanggil dalam
            // DeleteRuleset; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return mutableRuleset.Error;
        // Menutup scope cabang if untuk kondisi `mutableRuleset.Error is not null`; bagian berikut berada di luar batas blok tersebut dalam DeleteRuleset.
        }

        // Menjalankan hasil operasi asinkron memanggil `_rulesets.DeleteRulesetAsync` dengan `rulesetId`, `ct`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai dalam DeleteRuleset.
        await _rulesets.DeleteRulesetAsync(rulesetId, ct);
        // Mengembalikan memanggil `NoContent` dengan tanpa argumen kepada pemanggil dalam DeleteRuleset; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return NoContent();
    // Menutup scope metode DeleteRuleset; bagian berikut berada di luar batas blok tersebut dalam DeleteRuleset.
    }

    // Mendefinisikan metode `GetMutableInstructorRulesetAsync` dengan hasil bertipe `Task<(RulesetDb? Ruleset, IActionResult? Error)>`; operasi ini
    // menangani get mutable instruktur aturan asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian
    // melalui Task. Masukan: Parameter `rulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan; Parameter `instructorUserId` bertipe
    // `Guid` membawa identitas instruktur pemilik sesi atau aturan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi
    // dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private async Task<(RulesetDb? Ruleset, IActionResult? Error)> GetMutableInstructorRulesetAsync(
        // Parameter `rulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan.
        Guid rulesetId,
        // Parameter `instructorUserId` bertipe `Guid` membawa identitas instruktur pemilik sesi atau aturan.
        Guid instructorUserId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode GetMutableInstructorRulesetAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // GetMutableInstructorRulesetAsync.
    {
        // Menyiapkan variabel lokal `ruleset` untuk nilai aturan dengan hasil operasi asinkron memanggil `_rulesets.GetRulesetAsync` dengan `rulesetId`,
        // `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ruleset = await _rulesets.GetRulesetAsync(rulesetId, ct);
        // Memeriksa hasil pencocokan `ruleset` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetMutableInstructorRulesetAsync.
        if (ruleset is null)
        // Membuka scope cabang if untuk kondisi `ruleset is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetMutableInstructorRulesetAsync.
        {
            // Mengembalikan tuple yang membawa bagian 1: null; bagian 2: NotFound(ApiErrorHelper.BuildError(HttpContext, ”NOT_FOUND”, ”Ruleset tidak dite...
            // kepada pemanggil dalam GetMutableInstructorRulesetAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return (null, NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan")));
        // Menutup scope cabang if untuk kondisi `ruleset is null`; bagian berikut berada di luar batas blok tersebut dalam
        // GetMutableInstructorRulesetAsync.
        }

        // Memeriksa hasil pencocokan `ruleset.InstructorUserId` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetMutableInstructorRulesetAsync.
        if (ruleset.InstructorUserId is null)
        // Membuka scope cabang if untuk kondisi `ruleset.InstructorUserId is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetMutableInstructorRulesetAsync.
        {
            // Mengembalikan tuple yang membawa bagian 1: null; bagian 2: UnprocessableEntity(ApiErrorHelper.BuildError( HttpContext, ”DOMAIN_RULE_VIOLATI...
            // kepada pemanggil dalam GetMutableInstructorRulesetAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return (null, UnprocessableEntity(ApiErrorHelper.BuildError(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`.
                HttpContext,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Ruleset default sistem hanya dapat dilihat atau dijadikan dasar membuat ruleset baru.”` sebagai argumen ke
                // `ApiErrorHelper.BuildError`.
                "Ruleset default sistem hanya dapat dilihat atau dijadikan dasar membuat ruleset baru.")));
        // Menutup scope cabang if untuk kondisi `ruleset.InstructorUserId is null`; bagian berikut berada di luar batas blok tersebut dalam
        // GetMutableInstructorRulesetAsync.
        }

        // Memeriksa perbandingan ketidaksamaan antara `ruleset.InstructorUserId.Value` dan `instructorUserId`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam GetMutableInstructorRulesetAsync.
        if (ruleset.InstructorUserId.Value != instructorUserId)
        // Membuka scope cabang if untuk kondisi `ruleset.InstructorUserId.Value != instructorUserId`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam GetMutableInstructorRulesetAsync.
        {
            // Mengembalikan tuple yang membawa bagian 1: null; bagian 2: NotFound(ApiErrorHelper.BuildError(HttpContext, ”NOT_FOUND”, ”Ruleset tidak dite...
            // kepada pemanggil dalam GetMutableInstructorRulesetAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return (null, NotFound(ApiErrorHelper.BuildError(HttpContext, "NOT_FOUND", "Ruleset tidak ditemukan")));
        // Menutup scope cabang if untuk kondisi `ruleset.InstructorUserId.Value != instructorUserId`; bagian berikut berada di luar batas blok tersebut
        // dalam GetMutableInstructorRulesetAsync.
        }

        // Menyiapkan variabel lokal `lockedBySession` untuk nilai locked berdasarkan sesi dengan hasil operasi asinkron memanggil
        // `_rulesets.IsRulesetLockedBySessionAsync` dengan `rulesetId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var lockedBySession = await _rulesets.IsRulesetLockedBySessionAsync(rulesetId, ct);
        // Memeriksa `lockedBySession` (nilai locked berdasarkan sesi); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetMutableInstructorRulesetAsync.
        if (lockedBySession)
        // Membuka scope cabang if untuk kondisi `lockedBySession`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetMutableInstructorRulesetAsync.
        {
            // Mengembalikan tuple yang membawa bagian 1: null; bagian 2: UnprocessableEntity(ApiErrorHelper.BuildError( HttpContext, ”DOMAIN_RULE_VIOLATI...
            // kepada pemanggil dalam GetMutableInstructorRulesetAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return (null, UnprocessableEntity(ApiErrorHelper.BuildError(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`.
                HttpContext,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Ruleset sudah dipakai pada sesi yang berjalan atau selesai sehingga hanya dapat dilihat.”` sebagai argumen ke
                // `ApiErrorHelper.BuildError`.
                "Ruleset sudah dipakai pada sesi yang berjalan atau selesai sehingga hanya dapat dilihat.")));
        // Menutup scope cabang if untuk kondisi `lockedBySession`; bagian berikut berada di luar batas blok tersebut dalam
        // GetMutableInstructorRulesetAsync.
        }

        // Mengembalikan tuple yang membawa bagian 1: ruleset; bagian 2: null kepada pemanggil dalam GetMutableInstructorRulesetAsync; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return (ruleset, null);
    // Menutup scope metode GetMutableInstructorRulesetAsync; bagian berikut berada di luar batas blok tersebut dalam GetMutableInstructorRulesetAsync.
    }

    // Mendefinisikan metode `GetActorName` dengan hasil bertipe `string?`; operasi ini menangani get actor nama.
    private string? GetActorName()
    // Membuka scope metode GetActorName; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetActorName.
    {
        // Mengembalikan `User.FindFirstValue(ClaimTypes.Name)` bila tidak null; jika null gunakan `User.FindFirstValue(ClaimTypes.NameIdentifier)` sebagai
        // nilai pengganti kepada pemanggil dalam GetActorName; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return User.FindFirstValue(ClaimTypes.Name) ??
               // Melanjutkan pengolahan dengan memanggil `User.FindFirstValue` dengan `ClaimTypes.NameIdentifier` dalam GetActorName.
               User.FindFirstValue(ClaimTypes.NameIdentifier);
    // Menutup scope metode GetActorName; bagian berikut berada di luar batas blok tersebut dalam GetActorName.
    }

    // Mendefinisikan metode `TryGetCurrentUserId` dengan hasil bertipe `bool`; operasi ini menangani try get saat ini pengguna identitas. Masukan:
    // Parameter `userId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; out mengembalikan nilai melalui parameter dan
    // harus diisi oleh metode.
    private bool TryGetCurrentUserId(out Guid userId)
    // Membuka scope metode TryGetCurrentUserId; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetCurrentUserId.
    {
        // Menyiapkan variabel lokal `userIdRaw` untuk nilai pengguna identitas raw dengan memanggil `User.FindFirstValue` dengan
        // `ClaimTypes.NameIdentifier`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // Mengembalikan mencoba mengonversi `userIdRaw`, `userId` melalui `Guid.TryParse`; keberhasilan dilaporkan sebagai boolean dan hasil ditempatkan
        // pada argumen out kepada pemanggil dalam TryGetCurrentUserId; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return Guid.TryParse(userIdRaw, out userId);
    // Menutup scope metode TryGetCurrentUserId; bagian berikut berada di luar batas blok tersebut dalam TryGetCurrentUserId.
    }

    // Mendefinisikan metode `PrepareDefinitionForWriteAsync` dengan hasil bertipe `Task<(RulesetDefinitionDto? Definition, IActionResult? Error)>`;
    // operasi ini menangani prepare definisi untuk write asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan
    // penyelesaian melalui Task. Masukan: Parameter `explicitDefinition` bertipe `RulesetDefinitionDto?` membawa nilai explicit definisi; nilai null
    // diizinkan ketika data opsional belum tersedia; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan
    // ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private async Task<(RulesetDefinitionDto? Definition, IActionResult? Error)> PrepareDefinitionForWriteAsync(
        // Parameter `explicitDefinition` bertipe `RulesetDefinitionDto?` membawa nilai explicit definisi; nilai null diizinkan ketika data opsional belum
        // tersedia.
        RulesetDefinitionDto? explicitDefinition,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode PrepareDefinitionForWriteAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // PrepareDefinitionForWriteAsync.
    {
        // Memeriksa hasil pencocokan `explicitDefinition` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // PrepareDefinitionForWriteAsync.
        if (explicitDefinition is null)
        // Membuka scope cabang if untuk kondisi `explicitDefinition is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // PrepareDefinitionForWriteAsync.
        {
            // Mengembalikan tuple yang membawa bagian 1: null; bagian 2: BadRequest(ApiErrorHelper.BuildError( HttpContext, ”VALIDATION_ERROR”, ”Definiti...
            // kepada pemanggil dalam PrepareDefinitionForWriteAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return (
                // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `PrepareDefinitionForWriteAsync`.
                null,
                // Meneruskan membentuk respons HTTP 400 dengan `ApiErrorHelper.BuildError( HttpContext, ”VALIDATION_ERROR”, ”Definition ruleset wajib ada”, new
                // ErrorDetail(”definition”, ”REQUIRED”))` karena permintaan tidak memenuhi kontrak sebagai argumen ke `PrepareDefinitionForWriteAsync`; Meneruskan
                // memanggil `ApiErrorHelper.BuildError` dengan `HttpContext`, `”VALIDATION_ERROR”`, `”Definition ruleset wajib ada”`, `new
                // ErrorDetail(”definition”, ”REQUIRED”)` sebagai argumen ke `BadRequest`.
                BadRequest(ApiErrorHelper.BuildError(
                    // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`.
                    HttpContext,
                    // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                    "VALIDATION_ERROR",
                    // Meneruskan nilai literal `”Definition ruleset wajib ada”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                    "Definition ruleset wajib ada",
                    // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”definition”, ”REQUIRED”) sebagai argumen ke `ApiErrorHelper.BuildError`; Meneruskan
                    // nilai literal `”definition”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen ke konstruktor
                    // `ErrorDetail`.
                    new ErrorDetail("definition", "REQUIRED"))));
        // Menutup scope cabang if untuk kondisi `explicitDefinition is null`; bagian berikut berada di luar batas blok tersebut dalam
        // PrepareDefinitionForWriteAsync.
        }

        // Menyiapkan variabel lokal `definition` untuk definisi terstruktur komponen serta parameter aturan permainan dengan `explicitDefinition` (nilai
        // explicit definisi). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var definition = explicitDefinition;

        // Memeriksa kebalikan kondisi `RulesetRuntimeMapper.TryBuildConfig(definition, out _, out var configErrors)`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam PrepareDefinitionForWriteAsync.
        if (!RulesetRuntimeMapper.TryBuildConfig(definition, out _, out var configErrors))
        // Membuka scope cabang if untuk kondisi `!RulesetRuntimeMapper.TryBuildConfig(definition, out _, out var configErrors)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam PrepareDefinitionForWriteAsync.
        {
            // Mengembalikan tuple yang membawa bagian 1: null; bagian 2: BadRequest(ApiErrorHelper.BuildError( HttpContext, ”VALIDATION_ERROR”, ”Definiti...
            // kepada pemanggil dalam PrepareDefinitionForWriteAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return (
                // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `PrepareDefinitionForWriteAsync`.
                null,
                // Meneruskan membentuk respons HTTP 400 dengan `ApiErrorHelper.BuildError( HttpContext, ”VALIDATION_ERROR”, ”Definition ruleset tidak valid”,
                // configErrors.ToArray())` karena permintaan tidak memenuhi kontrak sebagai argumen ke `PrepareDefinitionForWriteAsync`; Meneruskan memanggil
                // `ApiErrorHelper.BuildError` dengan `HttpContext`, `”VALIDATION_ERROR”`, `”Definition ruleset tidak valid”`, `configErrors.ToArray()` sebagai
                // argumen ke `BadRequest`.
                BadRequest(ApiErrorHelper.BuildError(
                    // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`.
                    HttpContext,
                    // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                    "VALIDATION_ERROR",
                    // Meneruskan nilai literal `”Definition ruleset tidak valid”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                    "Definition ruleset tidak valid",
                    // Meneruskan mematerialisasi urutan `configErrors` menjadi array dengan elemen hasil saat ini sebagai argumen ke `ApiErrorHelper.BuildError`.
                    configErrors.ToArray())));
        // Menutup scope cabang if untuk kondisi `!RulesetRuntimeMapper.TryBuildConfig(definition, out _, out var configErrors)`; bagian berikut berada di
        // luar batas blok tersebut dalam PrepareDefinitionForWriteAsync.
        }

        // Mengembalikan tuple yang membawa bagian 1: definition; bagian 2: null kepada pemanggil dalam PrepareDefinitionForWriteAsync; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return (definition, null);
    // Menutup scope metode PrepareDefinitionForWriteAsync; bagian berikut berada di luar batas blok tersebut dalam PrepareDefinitionForWriteAsync.
    }

    // Mendefinisikan metode `CloneDefinition` dengan hasil bertipe `RulesetDefinitionDto`; operasi ini menangani clone definisi. Masukan: Parameter
    // `source` bertipe `RulesetDefinitionDto` membawa nilai source; Parameter `actions` bertipe `IReadOnlyCollection<RulesetActionDto>` membawa nilai
    // aksi.
    private static RulesetDefinitionDto CloneDefinition(
        // Parameter `source` bertipe `RulesetDefinitionDto` membawa nilai source.
        RulesetDefinitionDto source,
        // Parameter `actions` bertipe `IReadOnlyCollection<RulesetActionDto>` membawa nilai aksi.
        IReadOnlyCollection<RulesetActionDto> actions)
    // Membuka scope metode CloneDefinition; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CloneDefinition.
    {
        // Mengembalikan objek baru bertipe `RulesetDefinitionDto` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam CloneDefinition; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return new RulesetDefinitionDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CloneDefinition.
        {
            // Memperbarui `Mode` menggunakan `source.Mode` (mode permainan yang menentukan kelompok aturan yang digunakan) dalam CloneDefinition.
            Mode = source.Mode,
            // Memperbarui `Settings` menggunakan `source.Settings` (nilai settings) dalam CloneDefinition.
            Settings = source.Settings,
            // Memperbarui `PlayerOrdering` menggunakan `source.PlayerOrdering` (nilai pemain ordering) dalam CloneDefinition.
            PlayerOrdering = source.PlayerOrdering,
            // Memperbarui `Actions` menggunakan mematerialisasi urutan `actions` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori dalam
            // CloneDefinition.
            Actions = actions.ToList(),
            // Memperbarui `Ingredients` menggunakan `source.Ingredients` (nilai bahan) dalam CloneDefinition.
            Ingredients = source.Ingredients,
            // Memperbarui `Orders` menggunakan `source.Orders` (nilai pesanan) dalam CloneDefinition.
            Orders = source.Orders,
            // Memperbarui `Needs` menggunakan `source.Needs` (nilai kebutuhan) dalam CloneDefinition.
            Needs = source.Needs,
            // Memperbarui `NeedSetBonuses` menggunakan `source.NeedSetBonuses` (nilai kebutuhan set bonuses) dalam CloneDefinition.
            NeedSetBonuses = source.NeedSetBonuses,
            // Memperbarui `CollectionMissions` menggunakan `source.CollectionMissions` (nilai collection misi) dalam CloneDefinition.
            CollectionMissions = source.CollectionMissions,
            // Memperbarui `FinancialGoals` menggunakan `source.FinancialGoals` (nilai keuangan target) dalam CloneDefinition.
            FinancialGoals = source.FinancialGoals,
            // Memperbarui `Narratives` menggunakan `source.Narratives` (nilai narratives) dalam CloneDefinition.
            Narratives = source.Narratives,
            // Memperbarui `DonationRankPoints` menggunakan `source.DonationRankPoints` (nilai donasi rank poin) dalam CloneDefinition.
            DonationRankPoints = source.DonationRankPoints,
            // Memperbarui `GoldPointsByQty` menggunakan `source.GoldPointsByQty` (nilai emas poin berdasarkan qty) dalam CloneDefinition.
            GoldPointsByQty = source.GoldPointsByQty,
            // Memperbarui `GoldPrices` menggunakan `source.GoldPrices` (nilai emas prices) dalam CloneDefinition.
            GoldPrices = source.GoldPrices,
            // Memperbarui `PensionRankPoints` menggunakan `source.PensionRankPoints` (nilai pension rank poin) dalam CloneDefinition.
            PensionRankPoints = source.PensionRankPoints,
            // Memperbarui `TieBreakers` menggunakan `source.TieBreakers` (nilai tie breakers) dalam CloneDefinition.
            TieBreakers = source.TieBreakers,
            // Memperbarui `ShariaLoans` menggunakan `source.ShariaLoans` (nilai sharia pinjaman) dalam CloneDefinition.
            ShariaLoans = source.ShariaLoans,
            // Memperbarui `InsuranceProducts` menggunakan `source.InsuranceProducts` (nilai asuransi products) dalam CloneDefinition.
            InsuranceProducts = source.InsuranceProducts,
            // Memperbarui `LifeRisks` menggunakan `source.LifeRisks` (nilai life risks) dalam CloneDefinition.
            LifeRisks = source.LifeRisks
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CloneDefinition.
        };
    // Menutup scope metode CloneDefinition; bagian berikut berada di luar batas blok tersebut dalam CloneDefinition.
    }

    // Mendefinisikan metode `BadRequestError` dengan hasil bertipe `IActionResult`; operasi ini menangani bad permintaan kesalahan. Masukan: Parameter
    // `field` bertipe `string` membawa nilai field; Parameter `issue` bertipe `string` membawa nilai issue; Parameter `message` bertipe `string`
    // membawa nilai pesan.
    private IActionResult BadRequestError(string field, string issue, string message)
    // Membuka scope metode BadRequestError; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BadRequestError.
    {
        // Mengembalikan membentuk respons HTTP 400 dengan `ApiErrorHelper.BuildError(HttpContext, ”VALIDATION_ERROR”, message, new ErrorDetail(field,
        // issue))` karena permintaan tidak memenuhi kontrak kepada pemanggil dalam BadRequestError; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", message, new ErrorDetail(field, issue)));
    // Menutup scope metode BadRequestError; bagian berikut berada di luar batas blok tersebut dalam BadRequestError.
    }

    // Mendefinisikan metode `TryNormalizeMode` dengan hasil bertipe `bool`; operasi ini menangani try normalize mode. Masukan: Parameter `rawMode`
    // bertipe `string?` membawa nilai raw mode; nilai null diizinkan ketika data opsional belum tersedia; Parameter `mode` bertipe `string` membawa
    // mode permainan yang menentukan kelompok aturan yang digunakan; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `error` bertipe `IActionResult?` membawa informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil; nilai null diizinkan
    // ketika data opsional belum tersedia; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    private bool TryNormalizeMode(string? rawMode, out string mode, out IActionResult? error)
    // Membuka scope metode TryNormalizeMode; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryNormalizeMode.
    {
        // Memperbarui `mode` menggunakan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(rawMode)` benar gunakan `”MAHIR”`, jika tidak gunakan
        // `rawMode.Trim().ToUpperInvariant()` dalam TryNormalizeMode.
        mode = string.IsNullOrWhiteSpace(rawMode) ? "MAHIR" : rawMode.Trim().ToUpperInvariant();
        // Memperbarui `error` menggunakan null, yaitu penanda tidak ada nilai dalam TryNormalizeMode.
        error = null;
        // Memeriksa hasil pencocokan `mode` dengan pola `”PEMULA” or ”MAHIR”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryNormalizeMode.
        if (mode is "PEMULA" or "MAHIR")
        // Membuka scope cabang if untuk kondisi `mode is ”PEMULA” or ”MAHIR”`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryNormalizeMode.
        {
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryNormalizeMode; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `mode is ”PEMULA” or ”MAHIR”`; bagian berikut berada di luar batas blok tersebut dalam TryNormalizeMode.
        }

        // Memperbarui `error` menggunakan memanggil `BadRequestError` dengan `”mode”`, `”INVALID_ENUM”`, `”Mode tidak valid”` dalam TryNormalizeMode.
        error = BadRequestError("mode", "INVALID_ENUM", "Mode tidak valid");
        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryNormalizeMode; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return false;
    // Menutup scope metode TryNormalizeMode; bagian berikut berada di luar batas blok tersebut dalam TryNormalizeMode.
    }

// Menutup scope tipe RulesetsController; bagian berikut berada di luar batas blok tersebut.
}
