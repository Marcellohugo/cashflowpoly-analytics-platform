// Fungsi file: Menangani endpoint, validasi akses, dan response HTTP untuk AuthController.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Infrastructure;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Cashflowpoly.Api.Security` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Security;
// Mengimpor namespace `Microsoft.AspNetCore.Authorization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Authorization;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;
// Mengimpor namespace `Microsoft.Extensions.Options` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.Extensions.Options;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Controllers` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Controllers;

// mengaktifkan perilaku API controller, termasuk inferensi binding dan respons otomatis atas model tidak valid.
[ApiController]
// menetapkan pola rute (”api/v1/auth”) untuk pencocokan URL permintaan.
[Route("api/v1/auth")]
// mengizinkan endpoint diakses tanpa identitas pengguna yang telah diautentikasi.
[AllowAnonymous]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)` pada deklarasi berikut agar framework/compiler
// dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
// menerapkan metadata `ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)` pada deklarasi berikut agar
// framework/compiler dapat mengenali pengaturannya.
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
// Mendefinisikan tipe class `AuthController` yang mewarisi atau menerapkan `ControllerBase`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AuthController : ControllerBase
// Membuka scope tipe AuthController; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `JwtTokenService`: `_tokens` menyimpan nilai tokens. readonly membatasi penggantian referensi/nilai field pada
    // deklarasi atau konstruktor.
    private readonly JwtTokenService _tokens;
    // Mendeklarasikan field bertipe `UserRepository`: `_users` menyimpan nilai pengguna. readonly membatasi penggantian referensi/nilai field pada
    // deklarasi atau konstruktor.
    private readonly UserRepository _users;
    // Mendeklarasikan field bertipe `SecurityAuditService`: `_securityAudit` menyimpan nilai security audit. readonly membatasi penggantian
    // referensi/nilai field pada deklarasi atau konstruktor.
    private readonly SecurityAuditService _securityAudit;
    // Mendeklarasikan field bertipe `AuthRegistrationOptions`: `_registrationOptions` menyimpan nilai registration options. readonly membatasi
    // penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly AuthRegistrationOptions _registrationOptions;

    // Mendefinisikan konstruktor AuthController yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter `users`
    // bertipe `UserRepository` membawa nilai pengguna; Parameter `tokens` bertipe `JwtTokenService` membawa nilai tokens; Parameter `securityAudit`
    // bertipe `SecurityAuditService` membawa nilai security audit; Parameter `registrationOptions` bertipe `IOptions<AuthRegistrationOptions>` membawa
    // nilai registration options.
    public AuthController(
        // Parameter `users` bertipe `UserRepository` membawa nilai pengguna.
        UserRepository users,
        // Parameter `tokens` bertipe `JwtTokenService` membawa nilai tokens.
        JwtTokenService tokens,
        // Parameter `securityAudit` bertipe `SecurityAuditService` membawa nilai security audit.
        SecurityAuditService securityAudit,
        // Parameter `registrationOptions` bertipe `IOptions<AuthRegistrationOptions>` membawa nilai registration options.
        IOptions<AuthRegistrationOptions> registrationOptions)
    // Membuka scope konstruktor AuthController; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AuthController.
    {
        // Memperbarui `_users` menggunakan `users` (nilai pengguna) dalam AuthController.
        _users = users;
        // Memperbarui `_tokens` menggunakan `tokens` (nilai tokens) dalam AuthController.
        _tokens = tokens;
        // Memperbarui `_securityAudit` menggunakan `securityAudit` (nilai security audit) dalam AuthController.
        _securityAudit = securityAudit;
        // Memperbarui `_registrationOptions` menggunakan `registrationOptions.Value`, yaitu nilai yang dibungkus objek/nullable dalam AuthController.
        _registrationOptions = registrationOptions.Value;
    // Menutup scope konstruktor AuthController; bagian berikut berada di luar batas blok tersebut dalam AuthController.
    }

    // mendaftarkan action untuk metode HTTP POST pada rute (”login”).
    [HttpPost("login")]
    // menerapkan metadata `ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)` pada deklarasi berikut agar framework/compiler dapat
    // mengenali pengaturannya.
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    // Mendefinisikan metode `Login` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani login. async memungkinkan metode menunggu operasi
    // I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe `LoginRequest` membawa data masukan permintaan
    // yang akan divalidasi atau diteruskan ke layanan; mengambil nilai parameter dari badan permintaan HTTP; Parameter `ct` bertipe `CancellationToken`
    // membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    // Membuka scope metode Login; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Login.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `string.IsNullOrWhiteSpace(request.Username)` dan
        // `string.IsNullOrWhiteSpace(request.Password)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam Login.
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Login.
        {
            // Menjalankan hasil operasi asinkron memanggil `_securityAudit.LogAsync` dengan `HttpContext`, `SecurityAuditEventTypes.LoginFailed`,
            // `SecurityAuditOutcomes.Failure`, `StatusCodes.Status400BadRequest`, `new { reason = ”VALIDATION_ERROR”, issue = ”username_or_password_required”
            // }`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam Login.
            await _securityAudit.LogAsync(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `_securityAudit.LogAsync`.
                HttpContext,
                // Meneruskan `SecurityAuditEventTypes.LoginFailed` (nilai login failed) sebagai argumen ke `_securityAudit.LogAsync`.
                SecurityAuditEventTypes.LoginFailed,
                // Meneruskan `SecurityAuditOutcomes.Failure` (nilai failure) sebagai argumen ke `_securityAudit.LogAsync`.
                SecurityAuditOutcomes.Failure,
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `_securityAudit.LogAsync`.
                StatusCodes.Status400BadRequest,
                // Meneruskan objek anonim yang mengelompokkan reason, issue sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Login.
                {
                    // Meneruskan objek anonim yang mengelompokkan reason, issue sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                    reason = "VALIDATION_ERROR",
                    // Meneruskan objek anonim yang mengelompokkan reason, issue sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                    issue = "username_or_password_required"
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam Login.
                },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `_securityAudit.LogAsync`.
                ct);
            // Mengembalikan membentuk respons HTTP 400 dengan `ApiErrorHelper.BuildError(HttpContext, ”VALIDATION_ERROR”, ”Username dan password wajib diisi”)`
            // karena permintaan tidak memenuhi kontrak kepada pemanggil dalam Login; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Username dan password wajib diisi"));
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password)`; bagian
        // berikut berada di luar batas blok tersebut dalam Login.
        }

        // Menyiapkan variabel lokal `username` untuk nama akun yang dipakai saat autentikasi dengan membersihkan karakter tepi pada `request.Username`
        // memakai tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var username = request.Username.Trim();
        // Memeriksa kebalikan kondisi `PasswordPolicy.IsWithinBcryptLimit(request.Password)`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam Login.
        if (!PasswordPolicy.IsWithinBcryptLimit(request.Password))
        // Membuka scope cabang if untuk kondisi `!PasswordPolicy.IsWithinBcryptLimit(request.Password)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam Login.
        {
            // Menjalankan hasil operasi asinkron memanggil `_securityAudit.LogAsync` dengan `HttpContext`, `SecurityAuditEventTypes.LoginFailed`,
            // `SecurityAuditOutcomes.Failure`, `StatusCodes.Status400BadRequest`, `new { reason = ”PASSWORD_TOO_LONG”, username }`, `ct`; await menunggu hasil
            // tanpa memblokir thread selama operasi belum selesai dalam Login.
            await _securityAudit.LogAsync(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `_securityAudit.LogAsync`.
                HttpContext,
                // Meneruskan `SecurityAuditEventTypes.LoginFailed` (nilai login failed) sebagai argumen ke `_securityAudit.LogAsync`.
                SecurityAuditEventTypes.LoginFailed,
                // Meneruskan `SecurityAuditOutcomes.Failure` (nilai failure) sebagai argumen ke `_securityAudit.LogAsync`.
                SecurityAuditOutcomes.Failure,
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `_securityAudit.LogAsync`.
                StatusCodes.Status400BadRequest,
                // Meneruskan objek anonim yang mengelompokkan reason, username sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                new { reason = "PASSWORD_TOO_LONG", username },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `_securityAudit.LogAsync`.
                ct);
            // Mengembalikan membentuk respons HTTP 400 dengan `ApiErrorHelper.BuildError( HttpContext, ”VALIDATION_ERROR”, $”Password maksimal
            // {PasswordPolicy.MaxPasswordUtf8Bytes} byte UTF-8”)` karena permintaan tidak memenuhi kontrak kepada pemanggil dalam Login; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return BadRequest(ApiErrorHelper.BuildError(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`.
                HttpContext,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                "VALIDATION_ERROR",
                // Meneruskan teks interpolasi `$”Password maksimal {PasswordPolicy.MaxPasswordUtf8Bytes} byte UTF-8”`; nilai ekspresi di dalam kurung kurawal
                // disisipkan saat program berjalan sebagai argumen ke `ApiErrorHelper.BuildError`.
                $"Password maksimal {PasswordPolicy.MaxPasswordUtf8Bytes} byte UTF-8"));
        // Menutup scope cabang if untuk kondisi `!PasswordPolicy.IsWithinBcryptLimit(request.Password)`; bagian berikut berada di luar batas blok tersebut
        // dalam Login.
        }

        // Menyiapkan variabel lokal `user` untuk pengguna yang sedang diproses beserta identitas atau klaim akses yang dimilikinya dengan hasil operasi
        // asinkron memanggil `_users.AuthenticateAsync` dengan `username`, `request.Password`, `ct`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var user = await _users.AuthenticateAsync(username, request.Password, ct);
        // Memeriksa hasil pencocokan `user` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Login.
        if (user is null)
        // Membuka scope cabang if untuk kondisi `user is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Login.
        {
            // Menjalankan hasil operasi asinkron memanggil `_securityAudit.LogAsync` dengan `HttpContext`, `SecurityAuditEventTypes.LoginFailed`,
            // `SecurityAuditOutcomes.Failure`, `StatusCodes.Status401Unauthorized`, `new { reason = ”INVALID_CREDENTIALS”, username }`, `ct`; await menunggu
            // hasil tanpa memblokir thread selama operasi belum selesai dalam Login.
            await _securityAudit.LogAsync(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `_securityAudit.LogAsync`.
                HttpContext,
                // Meneruskan `SecurityAuditEventTypes.LoginFailed` (nilai login failed) sebagai argumen ke `_securityAudit.LogAsync`.
                SecurityAuditEventTypes.LoginFailed,
                // Meneruskan `SecurityAuditOutcomes.Failure` (nilai failure) sebagai argumen ke `_securityAudit.LogAsync`.
                SecurityAuditOutcomes.Failure,
                // Meneruskan `StatusCodes.Status401Unauthorized` (nilai status 401 unauthorized) sebagai argumen ke `_securityAudit.LogAsync`.
                StatusCodes.Status401Unauthorized,
                // Meneruskan objek anonim yang mengelompokkan reason, username sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Login.
                {
                    // Meneruskan objek anonim yang mengelompokkan reason, username sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                    reason = "INVALID_CREDENTIALS",
                    // Meneruskan objek anonim yang mengelompokkan reason, username sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                    username
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam Login.
                },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `_securityAudit.LogAsync`.
                ct);
            // Mengembalikan membentuk respons HTTP 401 dengan `ApiErrorHelper.BuildError(HttpContext, ”INVALID_CREDENTIALS”, ”Username atau password salah”)`
            // karena autentikasi tidak terpenuhi kepada pemanggil dalam Login; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Unauthorized(ApiErrorHelper.BuildError(HttpContext, "INVALID_CREDENTIALS", "Username atau password salah"));
        // Menutup scope cabang if untuk kondisi `user is null`; bagian berikut berada di luar batas blok tersebut dalam Login.
        }

        // Menyiapkan variabel lokal `displayName` untuk nilai display nama dengan `user.DisplayName` (nilai display nama). Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var displayName = user.DisplayName;
        // Menyiapkan variabel lokal `issued` untuk nilai issued dengan memanggil `_tokens.IssueToken` dengan `user`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var issued = _tokens.IssueToken(user);
        // Menjalankan hasil operasi asinkron memanggil `_securityAudit.LogAsync` dengan `HttpContext`, `SecurityAuditEventTypes.LoginSuccess`,
        // `SecurityAuditOutcomes.Success`, `StatusCodes.Status200OK`, `new { user_id = user.UserId, username = user.Username, role = user.Role, is_demo =
        // user.IsDemo }`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam Login.
        await _securityAudit.LogAsync(
            // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `_securityAudit.LogAsync`.
            HttpContext,
            // Meneruskan `SecurityAuditEventTypes.LoginSuccess` (nilai login success) sebagai argumen ke `_securityAudit.LogAsync`.
            SecurityAuditEventTypes.LoginSuccess,
            // Meneruskan `SecurityAuditOutcomes.Success` (nilai success) sebagai argumen ke `_securityAudit.LogAsync`.
            SecurityAuditOutcomes.Success,
            // Meneruskan `StatusCodes.Status200OK` (nilai status 200 ok) sebagai argumen ke `_securityAudit.LogAsync`.
            StatusCodes.Status200OK,
            // Meneruskan objek anonim yang mengelompokkan user_id, username, role, is_demo sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Login.
            {
                // Meneruskan objek anonim yang mengelompokkan user_id, username, role, is_demo sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                user_id = user.UserId,
                // Meneruskan objek anonim yang mengelompokkan user_id, username, role, is_demo sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                username = user.Username,
                // Meneruskan objek anonim yang mengelompokkan user_id, username, role, is_demo sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                role = user.Role,
                // Meneruskan objek anonim yang mengelompokkan user_id, username, role, is_demo sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                is_demo = user.IsDemo
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam Login.
            },
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // ke `_securityAudit.LogAsync`.
            ct);
        // Mengembalikan membentuk respons HTTP 200 dengan `new LoginResponse(user.UserId, user.Username, user.Role, displayName, issued.AccessToken,
        // issued.ExpiresAt)` sebagai hasil berhasil kepada pemanggil dalam Login; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return Ok(new LoginResponse(user.UserId, user.Username, user.Role, displayName, issued.AccessToken, issued.ExpiresAt));
    // Menutup scope metode Login; bagian berikut berada di luar batas blok tersebut dalam Login.
    }

    // mendaftarkan action untuk metode HTTP POST pada rute (”register”).
    [HttpPost("register")]
    // menerapkan metadata `ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)` pada deklarasi berikut agar framework/compiler
    // dapat mengenali pengaturannya.
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    // Mendefinisikan metode `Register` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani register. async memungkinkan metode menunggu
    // operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe `RegisterRequest` membawa data masukan
    // permintaan yang akan divalidasi atau diteruskan ke layanan; mengambil nilai parameter dari badan permintaan HTTP; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    // Membuka scope metode Register; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Register.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `string.IsNullOrWhiteSpace(request.Username)` dan
        // `string.IsNullOrWhiteSpace(request.Password)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam Register.
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Register.
        {
            // Menjalankan hasil operasi asinkron memanggil `_securityAudit.LogAsync` dengan `HttpContext`, `SecurityAuditEventTypes.RegisterDenied`,
            // `SecurityAuditOutcomes.Denied`, `StatusCodes.Status400BadRequest`, `new { reason = ”VALIDATION_ERROR”, issue = ”username_or_password_required”
            // }`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam Register.
            await _securityAudit.LogAsync(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `_securityAudit.LogAsync`.
                HttpContext,
                // Meneruskan `SecurityAuditEventTypes.RegisterDenied` (nilai register denied) sebagai argumen ke `_securityAudit.LogAsync`.
                SecurityAuditEventTypes.RegisterDenied,
                // Meneruskan `SecurityAuditOutcomes.Denied` (nilai denied) sebagai argumen ke `_securityAudit.LogAsync`.
                SecurityAuditOutcomes.Denied,
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `_securityAudit.LogAsync`.
                StatusCodes.Status400BadRequest,
                // Meneruskan objek anonim yang mengelompokkan reason, issue sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Register.
                {
                    // Meneruskan objek anonim yang mengelompokkan reason, issue sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                    reason = "VALIDATION_ERROR",
                    // Meneruskan objek anonim yang mengelompokkan reason, issue sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                    issue = "username_or_password_required"
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam Register.
                },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `_securityAudit.LogAsync`.
                ct);
            // Mengembalikan membentuk respons HTTP 400 dengan `ApiErrorHelper.BuildError(HttpContext, ”VALIDATION_ERROR”, ”Username dan password wajib diisi”)`
            // karena permintaan tidak memenuhi kontrak kepada pemanggil dalam Register; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Username dan password wajib diisi"));
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password)`; bagian
        // berikut berada di luar batas blok tersebut dalam Register.
        }

        // Menyiapkan variabel lokal `username` untuk nama akun yang dipakai saat autentikasi dengan membersihkan karakter tepi pada `request.Username`
        // memakai tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var username = request.Username.Trim();
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `username.Length < 3` dan `username.Length > 80`; sisi kanan diperiksa
        // hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Register.
        if (username.Length < 3 || username.Length > 80)
        // Membuka scope cabang if untuk kondisi `username.Length < 3 || username.Length > 80`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam Register.
        {
            // Mengembalikan membentuk respons HTTP 400 dengan `ApiErrorHelper.BuildError(HttpContext, ”VALIDATION_ERROR”, ”Username harus 3-80 karakter”)`
            // karena permintaan tidak memenuhi kontrak kepada pemanggil dalam Register; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Username harus 3-80 karakter"));
        // Menutup scope cabang if untuk kondisi `username.Length < 3 || username.Length > 80`; bagian berikut berada di luar batas blok tersebut dalam
        // Register.
        }

        // Memeriksa pemeriksaan lebih kecil antara `request.Password.Length` dan `PasswordPolicy.MinPasswordLength`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam Register.
        if (request.Password.Length < PasswordPolicy.MinPasswordLength)
        // Membuka scope cabang if untuk kondisi `request.Password.Length < PasswordPolicy.MinPasswordLength`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam Register.
        {
            // Mengembalikan membentuk respons HTTP 400 dengan `ApiErrorHelper.BuildError( HttpContext, ”VALIDATION_ERROR”, $”Password minimal
            // {PasswordPolicy.MinPasswordLength} karakter”)` karena permintaan tidak memenuhi kontrak kepada pemanggil dalam Register; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return BadRequest(ApiErrorHelper.BuildError(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`.
                HttpContext,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                "VALIDATION_ERROR",
                // Meneruskan teks interpolasi `$”Password minimal {PasswordPolicy.MinPasswordLength} karakter”`; nilai ekspresi di dalam kurung kurawal disisipkan
                // saat program berjalan sebagai argumen ke `ApiErrorHelper.BuildError`.
                $"Password minimal {PasswordPolicy.MinPasswordLength} karakter"));
        // Menutup scope cabang if untuk kondisi `request.Password.Length < PasswordPolicy.MinPasswordLength`; bagian berikut berada di luar batas blok
        // tersebut dalam Register.
        }

        // Memeriksa kebalikan kondisi `PasswordPolicy.IsWithinBcryptLimit(request.Password)`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam Register.
        if (!PasswordPolicy.IsWithinBcryptLimit(request.Password))
        // Membuka scope cabang if untuk kondisi `!PasswordPolicy.IsWithinBcryptLimit(request.Password)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam Register.
        {
            // Menjalankan hasil operasi asinkron memanggil `_securityAudit.LogAsync` dengan `HttpContext`, `SecurityAuditEventTypes.RegisterDenied`,
            // `SecurityAuditOutcomes.Denied`, `StatusCodes.Status400BadRequest`, `new { reason = ”PASSWORD_TOO_LONG”, username }`, `ct`; await menunggu hasil
            // tanpa memblokir thread selama operasi belum selesai dalam Register.
            await _securityAudit.LogAsync(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `_securityAudit.LogAsync`.
                HttpContext,
                // Meneruskan `SecurityAuditEventTypes.RegisterDenied` (nilai register denied) sebagai argumen ke `_securityAudit.LogAsync`.
                SecurityAuditEventTypes.RegisterDenied,
                // Meneruskan `SecurityAuditOutcomes.Denied` (nilai denied) sebagai argumen ke `_securityAudit.LogAsync`.
                SecurityAuditOutcomes.Denied,
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `_securityAudit.LogAsync`.
                StatusCodes.Status400BadRequest,
                // Meneruskan objek anonim yang mengelompokkan reason, username sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                new { reason = "PASSWORD_TOO_LONG", username },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `_securityAudit.LogAsync`.
                ct);
            // Mengembalikan membentuk respons HTTP 400 dengan `ApiErrorHelper.BuildError( HttpContext, ”VALIDATION_ERROR”, $”Password maksimal
            // {PasswordPolicy.MaxPasswordUtf8Bytes} byte UTF-8”)` karena permintaan tidak memenuhi kontrak kepada pemanggil dalam Register; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return BadRequest(ApiErrorHelper.BuildError(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `ApiErrorHelper.BuildError`.
                HttpContext,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                "VALIDATION_ERROR",
                // Meneruskan teks interpolasi `$”Password maksimal {PasswordPolicy.MaxPasswordUtf8Bytes} byte UTF-8”`; nilai ekspresi di dalam kurung kurawal
                // disisipkan saat program berjalan sebagai argumen ke `ApiErrorHelper.BuildError`.
                $"Password maksimal {PasswordPolicy.MaxPasswordUtf8Bytes} byte UTF-8"));
        // Menutup scope cabang if untuk kondisi `!PasswordPolicy.IsWithinBcryptLimit(request.Password)`; bagian berikut berada di luar batas blok tersebut
        // dalam Register.
        }

        // Memeriksa memeriksa apakah `request.Role` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam Register.
        if (string.IsNullOrWhiteSpace(request.Role))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(request.Role)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam Register.
        {
            // Mengembalikan membentuk respons HTTP 400 dengan `ApiErrorHelper.BuildError(HttpContext, ”VALIDATION_ERROR”, ”Role wajib diisi”)` karena
            // permintaan tidak memenuhi kontrak kepada pemanggil dalam Register; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Role wajib diisi"));
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(request.Role)`; bagian berikut berada di luar batas blok tersebut dalam
        // Register.
        }

        // Menyiapkan variabel lokal `normalizedRole` untuk nilai normalized role dengan menormalisasi `request.Role` menjadi huruf besar dengan aturan
        // kultur invariant. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var normalizedRole = request.Role.ToUpperInvariant();
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!string.Equals(normalizedRole, ”INSTRUCTOR”,
        // StringComparison.OrdinalIgnoreCase)` dan `!string.Equals(normalizedRole, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa
        // hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Register.
        if (!string.Equals(normalizedRole, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase) &&
            // Menggunakan kebalikan kondisi `string.Equals(normalizedRole, ”PLAYER”, StringComparison.OrdinalIgnoreCase)` sebagai bagian ekspresi yang sedang
            // disusun dalam Register.
            !string.Equals(normalizedRole, "PLAYER", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `!string.Equals(normalizedRole, ”INSTRUCTOR”, StringComparison.OrdinalIgnoreCase) &&
        // !string.Equals(normalizedRole, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Register.
        {
            // Mengembalikan membentuk respons HTTP 400 dengan `ApiErrorHelper.BuildError(HttpContext, ”VALIDATION_ERROR”, ”Role tidak valid”)` karena
            // permintaan tidak memenuhi kontrak kepada pemanggil dalam Register; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Role tidak valid"));
        // Menutup scope cabang if untuk kondisi `!string.Equals(normalizedRole, ”INSTRUCTOR”, StringComparison.OrdinalIgnoreCase) &&
        // !string.Equals(normalizedRole, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok tersebut dalam Register.
        }

        // Memeriksa kebalikan kondisi `_registrationOptions.CanRegisterPublicly(normalizedRole)`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam Register.
        if (!_registrationOptions.CanRegisterPublicly(normalizedRole))
        // Membuka scope cabang if untuk kondisi `!_registrationOptions.CanRegisterPublicly(normalizedRole)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam Register.
        {
            // Menjalankan hasil operasi asinkron memanggil `_securityAudit.LogAsync` dengan `HttpContext`, `SecurityAuditEventTypes.RegisterDenied`,
            // `SecurityAuditOutcomes.Denied`, `StatusCodes.Status403Forbidden`, `new { reason = ”PUBLIC_INSTRUCTOR_REGISTRATION_DISABLED”, username }`, `ct`;
            // await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam Register.
            await _securityAudit.LogAsync(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `_securityAudit.LogAsync`.
                HttpContext,
                // Meneruskan `SecurityAuditEventTypes.RegisterDenied` (nilai register denied) sebagai argumen ke `_securityAudit.LogAsync`.
                SecurityAuditEventTypes.RegisterDenied,
                // Meneruskan `SecurityAuditOutcomes.Denied` (nilai denied) sebagai argumen ke `_securityAudit.LogAsync`.
                SecurityAuditOutcomes.Denied,
                // Meneruskan `StatusCodes.Status403Forbidden` (nilai status 403 forbidden) sebagai argumen ke `_securityAudit.LogAsync`.
                StatusCodes.Status403Forbidden,
                // Meneruskan objek anonim yang mengelompokkan reason, username sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                new { reason = "PUBLIC_INSTRUCTOR_REGISTRATION_DISABLED", username },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `_securityAudit.LogAsync`.
                ct);
            // Mengembalikan membentuk hasil HTTP dengan kode dan isi `StatusCodes.Status403Forbidden`, `ApiErrorHelper.BuildError(HttpContext, ”FORBIDDEN”,
            // ”Akun instruktur hanya dapat dibuat oleh administrator”)` kepada pemanggil dalam Register; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return StatusCode(
                // Meneruskan `StatusCodes.Status403Forbidden` (nilai status 403 forbidden) sebagai argumen ke `StatusCode`.
                StatusCodes.Status403Forbidden,
                // Meneruskan memanggil `ApiErrorHelper.BuildError` dengan `HttpContext`, `”FORBIDDEN”`, `”Akun instruktur hanya dapat dibuat oleh administrator”`
                // sebagai argumen ke `StatusCode`; Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen
                // ke `ApiErrorHelper.BuildError`; Meneruskan nilai literal `”FORBIDDEN”` sebagai argumen ke `ApiErrorHelper.BuildError`; Meneruskan nilai literal
                // `”Akun instruktur hanya dapat dibuat oleh administrator”` sebagai argumen ke `ApiErrorHelper.BuildError`.
                ApiErrorHelper.BuildError(HttpContext, "FORBIDDEN", "Akun instruktur hanya dapat dibuat oleh administrator"));
        // Menutup scope cabang if untuk kondisi `!_registrationOptions.CanRegisterPublicly(normalizedRole)`; bagian berikut berada di luar batas blok
        // tersebut dalam Register.
        }

        // Menyiapkan variabel lokal `displayName` untuk nilai display nama dengan hasil pemilihan bersyarat: ketika
        // `string.IsNullOrWhiteSpace(request.DisplayName)` benar gunakan `username`, jika tidak gunakan `request.DisplayName.Trim()`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var displayName = string.IsNullOrWhiteSpace(request.DisplayName) ? username : request.DisplayName.Trim();
        // Memeriksa pemeriksaan lebih besar antara `displayName.Length` dan `80`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // Register.
        if (displayName.Length > 80)
        // Membuka scope cabang if untuk kondisi `displayName.Length > 80`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Register.
        {
            // Mengembalikan membentuk respons HTTP 400 dengan `ApiErrorHelper.BuildError(HttpContext, ”VALIDATION_ERROR”, ”Display name maksimal 80 karakter”)`
            // karena permintaan tidak memenuhi kontrak kepada pemanggil dalam Register; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BadRequest(ApiErrorHelper.BuildError(HttpContext, "VALIDATION_ERROR", "Display name maksimal 80 karakter"));
        // Menutup scope cabang if untuk kondisi `displayName.Length > 80`; bagian berikut berada di luar batas blok tersebut dalam Register.
        }

        // Menyiapkan variabel lokal `created` untuk nilai created tanpa nilai awal pada deklarasi ini. Tipe yang dipakai adalah `AuthenticatedUserDb`.
        AuthenticatedUserDb created;
        // Memulai blok try dalam Register; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Register.
        {
            // Memperbarui `created` menggunakan hasil operasi asinkron memanggil `_users.CreateUserAsync` dengan `username`, `request.Password`,
            // `normalizedRole`, `displayName`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam Register.
            created = await _users.CreateUserAsync(username, request.Password, normalizedRole, displayName, ct);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam Register.
        }
        // Menangani exception `PostgresException` melalui variabel ex hanya jika filter `ex.SqlState == ”23505”` terpenuhi dalam Register.
        catch (PostgresException ex) when (ex.SqlState == "23505")
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Register.
        {
            // Menjalankan hasil operasi asinkron memanggil `_securityAudit.LogAsync` dengan `HttpContext`, `SecurityAuditEventTypes.RegisterDenied`,
            // `SecurityAuditOutcomes.Denied`, `StatusCodes.Status409Conflict`, `new { reason = ”DUPLICATE_USERNAME”, username }`, `ct`; await menunggu hasil
            // tanpa memblokir thread selama operasi belum selesai dalam Register.
            await _securityAudit.LogAsync(
                // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `_securityAudit.LogAsync`.
                HttpContext,
                // Meneruskan `SecurityAuditEventTypes.RegisterDenied` (nilai register denied) sebagai argumen ke `_securityAudit.LogAsync`.
                SecurityAuditEventTypes.RegisterDenied,
                // Meneruskan `SecurityAuditOutcomes.Denied` (nilai denied) sebagai argumen ke `_securityAudit.LogAsync`.
                SecurityAuditOutcomes.Denied,
                // Meneruskan `StatusCodes.Status409Conflict` (nilai status 409 conflict) sebagai argumen ke `_securityAudit.LogAsync`.
                StatusCodes.Status409Conflict,
                // Meneruskan objek anonim yang mengelompokkan reason, username sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Register.
                {
                    // Meneruskan objek anonim yang mengelompokkan reason, username sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                    reason = "DUPLICATE_USERNAME",
                    // Meneruskan objek anonim yang mengelompokkan reason, username sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                    username
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam Register.
                },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `_securityAudit.LogAsync`.
                ct);
            // Mengembalikan memanggil `Conflict` dengan `ApiErrorHelper.BuildError(HttpContext, ”DUPLICATE”, ”Username sudah digunakan”)` kepada pemanggil
            // dalam Register; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Conflict(ApiErrorHelper.BuildError(HttpContext, "DUPLICATE", "Username sudah digunakan"));
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam Register.
        }

        // Menyiapkan variabel lokal `issued` untuk nilai issued dengan memanggil `_tokens.IssueToken` dengan `created`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var issued = _tokens.IssueToken(created);
        // Menjalankan hasil operasi asinkron memanggil `_securityAudit.LogAsync` dengan `HttpContext`, `SecurityAuditEventTypes.RegisterSuccess`,
        // `SecurityAuditOutcomes.Success`, `StatusCodes.Status201Created`, `new { user_id = created.UserId, username = created.Username, role =
        // created.Role }`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam Register.
        await _securityAudit.LogAsync(
            // Meneruskan `HttpContext` (konteks operasi yang menyediakan data lingkungan pemrosesan saat ini) sebagai argumen ke `_securityAudit.LogAsync`.
            HttpContext,
            // Meneruskan `SecurityAuditEventTypes.RegisterSuccess` (nilai register success) sebagai argumen ke `_securityAudit.LogAsync`.
            SecurityAuditEventTypes.RegisterSuccess,
            // Meneruskan `SecurityAuditOutcomes.Success` (nilai success) sebagai argumen ke `_securityAudit.LogAsync`.
            SecurityAuditOutcomes.Success,
            // Meneruskan `StatusCodes.Status201Created` (nilai status 201 created) sebagai argumen ke `_securityAudit.LogAsync`.
            StatusCodes.Status201Created,
            // Meneruskan objek anonim yang mengelompokkan user_id, username, role sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Register.
            {
                // Meneruskan objek anonim yang mengelompokkan user_id, username, role sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                user_id = created.UserId,
                // Meneruskan objek anonim yang mengelompokkan user_id, username, role sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                username = created.Username,
                // Meneruskan objek anonim yang mengelompokkan user_id, username, role sebagai satu nilai sebagai argumen ke `_securityAudit.LogAsync`.
                role = created.Role
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam Register.
            },
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // ke `_securityAudit.LogAsync`.
            ct);
        // Mengembalikan membentuk hasil HTTP dengan kode dan isi `StatusCodes.Status201Created`, `new RegisterResponse(created.UserId, created.Username,
        // created.Role, displayName, issued.AccessToken, issued.ExpiresAt)` kepada pemanggil dalam Register; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return StatusCode(
            // Meneruskan `StatusCodes.Status201Created` (nilai status 201 created) sebagai argumen ke `StatusCode`.
            StatusCodes.Status201Created,
            // Meneruskan objek baru bertipe `RegisterResponse` dengan argumen (created.UserId, created.Username, created.Role, displayName, issued.AccessToken,
            // issued.ExpiresAt) sebagai argumen ke `StatusCode`; Meneruskan `created.UserId` (identitas akun pengguna yang datanya sedang diproses) sebagai
            // argumen ke konstruktor `RegisterResponse`; Meneruskan `created.Username` (nama akun yang dipakai saat autentikasi) sebagai argumen ke konstruktor
            // `RegisterResponse`; Meneruskan `created.Role` (peran pengguna yang menentukan hak akses) sebagai argumen ke konstruktor `RegisterResponse`;
            // Meneruskan `displayName` (nilai display nama) sebagai argumen ke konstruktor `RegisterResponse`; Meneruskan `issued.AccessToken` (nilai akses
            // token) sebagai argumen ke konstruktor `RegisterResponse`; Meneruskan `issued.ExpiresAt` (nilai expires at) sebagai argumen ke konstruktor
            // `RegisterResponse`.
            new RegisterResponse(created.UserId, created.Username, created.Role, displayName, issued.AccessToken, issued.ExpiresAt));
    // Menutup scope metode Register; bagian berikut berada di luar batas blok tersebut dalam Register.
    }
// Menutup scope tipe AuthController; bagian berikut berada di luar batas blok tersebut.
}
