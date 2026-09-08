// Fungsi file: Menangani request MVC dan penyusunan tampilan untuk AuthController.
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;
// Mengimpor namespace `System.Net.Http.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Json;
// Mengimpor namespace `Cashflowpoly.Ui.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Contracts;
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;
// Mengimpor namespace `Microsoft.AspNetCore.Authentication` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Authentication;
// Mengimpor namespace `Microsoft.AspNetCore.Authentication.Cookies` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama
// lengkapnya.
using Microsoft.AspNetCore.Authentication.Cookies;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Controllers` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Controllers;

// menetapkan pola rute (”auth”) untuk pencocokan URL permintaan.
[Route("auth")]
// Mendefinisikan tipe class `AuthController` yang mewarisi atau menerapkan `Controller`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AuthController : Controller
// Membuka scope tipe AuthController; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `IHttpClientFactory`: `_clientFactory` menyimpan nilai client factory. readonly membatasi penggantian
    // referensi/nilai field pada deklarasi atau konstruktor.
    private readonly IHttpClientFactory _clientFactory;

    // Mendefinisikan konstruktor AuthController yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `clientFactory` bertipe `IHttpClientFactory` membawa nilai client factory.
    public AuthController(IHttpClientFactory clientFactory)
    // Membuka scope konstruktor AuthController; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AuthController.
    {
        // Memperbarui `_clientFactory` menggunakan `clientFactory` (nilai client factory) dalam AuthController.
        _clientFactory = clientFactory;
    // Menutup scope konstruktor AuthController; bagian berikut berada di luar batas blok tersebut dalam AuthController.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”login”).
    [HttpGet("login")]
    // Mendefinisikan metode `Login` dengan hasil bertipe `IActionResult`; operasi ini menangani login. Masukan: Parameter `returnUrl` bertipe `string?`
    // membawa nilai return url; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda
    // tidak ada nilai; mengambil nilai parameter dari query string URL.
    public IActionResult Login([FromQuery] string? returnUrl = null)
    // Membuka scope metode Login; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Login.
    {
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `User.Identity?.IsAuthenticated == true` dan
        // `string.IsNullOrWhiteSpace(returnUrl)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam Login.
        if (User.Identity?.IsAuthenticated == true && string.IsNullOrWhiteSpace(returnUrl))
        // Membuka scope cabang if untuk kondisi `User.Identity?.IsAuthenticated == true && string.IsNullOrWhiteSpace(returnUrl)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam Login.
        {
            // Mengembalikan mengarahkan browser ke action `”Index”`, `”Home”` setelah pemrosesan selesai kepada pemanggil dalam Login; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return RedirectToAction("Index", "Home");
        // Menutup scope cabang if untuk kondisi `User.Identity?.IsAuthenticated == true && string.IsNullOrWhiteSpace(returnUrl)`; bagian berikut berada di
        // luar batas blok tersebut dalam Login.
        }

        // Mengembalikan menyiapkan tampilan Razor dengan `new LoginViewModel { ReturnUrl = returnUrl }` sebagai nama tampilan atau modelnya kepada
        // pemanggil dalam Login; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return View(new LoginViewModel
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Login.
        {
            // Memperbarui `ReturnUrl` menggunakan `returnUrl` (nilai return url) dalam Login.
            ReturnUrl = returnUrl
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Login.
        });
    // Menutup scope metode Login; bagian berikut berada di luar batas blok tersebut dalam Login.
    }

    // mendaftarkan action untuk metode HTTP POST pada rute (”login”).
    [HttpPost("login")]
    // memvalidasi token antiforgery untuk memastikan permintaan formulir membawa token yang sesuai.
    [ValidateAntiForgeryToken]
    // Mendefinisikan metode `Login` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani login. async memungkinkan metode menunggu operasi
    // I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `model` bertipe `LoginViewModel` membawa nilai model.
    public async Task<IActionResult> Login(LoginViewModel model)
    // Membuka scope metode Login; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Login.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `string.IsNullOrWhiteSpace(model.Username)` dan
        // `string.IsNullOrWhiteSpace(model.Password)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam Login.
        if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Login.
        {
            // Memperbarui `model.ErrorMessage` menggunakan memanggil `HttpContext.T` dengan `”auth.error.login_required”` dalam Login.
            model.ErrorMessage = HttpContext.T("auth.error.login_required");
            // Memperbarui `model.Password` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam Login.
            model.Password = string.Empty;
            // Mengembalikan menyiapkan tampilan Razor dengan `model` sebagai nama tampilan atau modelnya kepada pemanggil dalam Login; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return View(model);
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password)`; bagian berikut
        // berada di luar batas blok tersebut dalam Login.
        }

        // Menyiapkan variabel lokal `client` untuk nilai client dengan memanggil `_clientFactory.CreateClient` dengan `”Api”`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var client = _clientFactory.CreateClient("Api");
        // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan objek baru bertipe `LoginRequest` dengan argumen
        // (model.Username.Trim(), model.Password). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var payload = new LoginRequest(model.Username.Trim(), model.Password);
        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil tanpa nilai awal pada deklarasi ini. Tipe
        // yang dipakai adalah `HttpResponseMessage`.
        HttpResponseMessage response;
        // Memulai blok try dalam Login; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Login.
        {
            // Memperbarui `response` menggunakan hasil operasi asinkron memanggil `client.PostAsJsonAsync` dengan `”api/v1/auth/login”`, `payload`; await
            // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam Login.
            response = await client.PostAsJsonAsync("api/v1/auth/login", payload);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam Login.
        }
        // Menangani exception `HttpRequestException` melalui variabel dalam Login.
        catch (HttpRequestException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Login.
        {
            // Memperbarui `model.ErrorMessage` menggunakan memanggil `HttpContext.T` dengan `”auth.error.api_unavailable”` dalam Login.
            model.ErrorMessage = HttpContext.T("auth.error.api_unavailable");
            // Memperbarui `model.Password` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam Login.
            model.Password = string.Empty;
            // Mengembalikan menyiapkan tampilan Razor dengan `”Login”`, `model` sebagai nama tampilan atau modelnya kepada pemanggil dalam Login; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return View("Login", model);
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam Login.
        }
        // Menangani exception `TaskCanceledException` melalui variabel dalam Login.
        catch (TaskCanceledException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Login.
        {
            // Memperbarui `model.ErrorMessage` menggunakan memanggil `HttpContext.T` dengan `”auth.error.api_unavailable”` dalam Login.
            model.ErrorMessage = HttpContext.T("auth.error.api_unavailable");
            // Memperbarui `model.Password` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam Login.
            model.Password = string.Empty;
            // Mengembalikan menyiapkan tampilan Razor dengan `”Login”`, `model` sebagai nama tampilan atau modelnya kepada pemanggil dalam Login; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return View("Login", model);
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam Login.
        }

        // Memeriksa kebalikan kondisi `response.IsSuccessStatusCode`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Login.
        if (!response.IsSuccessStatusCode)
        // Membuka scope cabang if untuk kondisi `!response.IsSuccessStatusCode`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Login.
        {
            // Menyiapkan variabel lokal `error` untuk informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil dengan hasil operasi
            // asinkron memanggil `response.Content.TryReadFromJsonAsync<ErrorResponse>` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread
            // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var error = await response.Content.TryReadFromJsonAsync<ErrorResponse>();
            // Memperbarui `model.ErrorMessage` menggunakan `error?.Message` bila tidak null; jika null gunakan `HttpContext.T(”auth.error.login_failed”)`
            // sebagai nilai pengganti dalam Login.
            model.ErrorMessage = error?.Message ?? HttpContext.T("auth.error.login_failed");
            // Memperbarui `model.Password` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam Login.
            model.Password = string.Empty;
            // Mengembalikan menyiapkan tampilan Razor dengan `”Login”`, `model` sebagai nama tampilan atau modelnya kepada pemanggil dalam Login; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return View("Login", model);
        // Menutup scope cabang if untuk kondisi `!response.IsSuccessStatusCode`; bagian berikut berada di luar batas blok tersebut dalam Login.
        }

        // Menyiapkan variabel lokal `data` untuk nilai data dengan hasil operasi asinkron memanggil `response.Content.TryReadFromJsonAsync<LoginResponse>`
        // dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var data = await response.Content.TryReadFromJsonAsync<LoginResponse>();
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `data is null || !AuthConstants.IsValidRole(data.Role)` dan
        // `string.IsNullOrWhiteSpace(data.AccessToken)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam Login.
        if (data is null ||
            // Menggunakan kebalikan kondisi `AuthConstants.IsValidRole(data.Role)` sebagai bagian ekspresi yang sedang disusun dalam Login.
            !AuthConstants.IsValidRole(data.Role) ||
            // Melanjutkan pengolahan dengan memeriksa apakah `data.AccessToken` null, kosong, atau hanya berisi karakter spasi dalam Login.
            string.IsNullOrWhiteSpace(data.AccessToken))
        // Membuka scope cabang if untuk kondisi `data is null || !AuthConstants.IsValidRole(data.Role) || string.IsNullOrWhiteSpace(data.AccessToken)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Login.
        {
            // Memperbarui `model.ErrorMessage` menggunakan memanggil `HttpContext.T` dengan `”auth.error.login_response_invalid”` dalam Login.
            model.ErrorMessage = HttpContext.T("auth.error.login_response_invalid");
            // Memperbarui `model.Password` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam Login.
            model.Password = string.Empty;
            // Mengembalikan menyiapkan tampilan Razor dengan `”Login”`, `model` sebagai nama tampilan atau modelnya kepada pemanggil dalam Login; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return View("Login", model);
        // Menutup scope cabang if untuk kondisi `data is null || !AuthConstants.IsValidRole(data.Role) || string.IsNullOrWhiteSpace(data.AccessToken)`;
        // bagian berikut berada di luar batas blok tersebut dalam Login.
        }

        // Menjalankan hasil operasi asinkron memanggil `SignInAsync` dengan `data.UserId`, `data.Username`, `data.DisplayName`, `data.Role`,
        // `data.AccessToken`, `data.ExpiresAt`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam Login.
        await SignInAsync(
            // Meneruskan `data.UserId` (identitas akun pengguna yang datanya sedang diproses) sebagai argumen ke `SignInAsync`.
            data.UserId,
            // Meneruskan `data.Username` (nama akun yang dipakai saat autentikasi) sebagai argumen ke `SignInAsync`.
            data.Username,
            // Meneruskan `data.DisplayName` (nilai display nama) sebagai argumen ke `SignInAsync`.
            data.DisplayName,
            // Meneruskan `data.Role` (peran pengguna yang menentukan hak akses) sebagai argumen ke `SignInAsync`.
            data.Role,
            // Meneruskan `data.AccessToken` (nilai akses token) sebagai argumen ke `SignInAsync`.
            data.AccessToken,
            // Meneruskan `data.ExpiresAt` (nilai expires at) sebagai argumen ke `SignInAsync`.
            data.ExpiresAt);

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!string.IsNullOrWhiteSpace(model.ReturnUrl)` dan
        // `Url.IsLocalUrl(model.ReturnUrl)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam Login.
        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam Login.
        {
            // Mengembalikan memanggil `Redirect` dengan `model.ReturnUrl` kepada pemanggil dalam Login; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Redirect(model.ReturnUrl);
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)`; bagian berikut berada di
        // luar batas blok tersebut dalam Login.
        }

        // Mengembalikan mengarahkan browser ke action `”Index”`, `”Home”` setelah pemrosesan selesai kepada pemanggil dalam Login; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return RedirectToAction("Index", "Home");
    // Menutup scope metode Login; bagian berikut berada di luar batas blok tersebut dalam Login.
    }

    // mendaftarkan action untuk metode HTTP GET pada rute (”register”).
    [HttpGet("register")]
    // Mendefinisikan metode `Register` dengan hasil bertipe `IActionResult`; operasi ini menangani register. Masukan: Parameter `returnUrl` bertipe
    // `string?` membawa nilai return url; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu
    // penanda tidak ada nilai; mengambil nilai parameter dari query string URL.
    public IActionResult Register([FromQuery] string? returnUrl = null)
    // Membuka scope metode Register; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Register.
    {
        // Mengembalikan menyiapkan tampilan Razor dengan `new RegisterViewModel { ReturnUrl = returnUrl }` sebagai nama tampilan atau modelnya kepada
        // pemanggil dalam Register; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return View(new RegisterViewModel
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Register.
        {
            // Memperbarui `ReturnUrl` menggunakan `returnUrl` (nilai return url) dalam Register.
            ReturnUrl = returnUrl
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Register.
        });
    // Menutup scope metode Register; bagian berikut berada di luar batas blok tersebut dalam Register.
    }

    // mendaftarkan action untuk metode HTTP POST pada rute (”register”).
    [HttpPost("register")]
    // memvalidasi token antiforgery untuk memastikan permintaan formulir membawa token yang sesuai.
    [ValidateAntiForgeryToken]
    // Mendefinisikan metode `Register` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani register. async memungkinkan metode menunggu
    // operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `model` bertipe `RegisterViewModel` membawa nilai model.
    public async Task<IActionResult> Register(RegisterViewModel model)
    // Membuka scope metode Register; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Register.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `string.IsNullOrWhiteSpace(model.DisplayName) ||
        // string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password)` dan `string.IsNullOrWhiteSpace(model.ConfirmPassword)`;
        // sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Register.
        if (string.IsNullOrWhiteSpace(model.DisplayName) ||
            // Melanjutkan pengolahan dengan memeriksa apakah `model.Username` null, kosong, atau hanya berisi karakter spasi dalam Register.
            string.IsNullOrWhiteSpace(model.Username) ||
            // Melanjutkan pengolahan dengan memeriksa apakah `model.Password` null, kosong, atau hanya berisi karakter spasi dalam Register.
            string.IsNullOrWhiteSpace(model.Password) ||
            // Melanjutkan pengolahan dengan memeriksa apakah `model.ConfirmPassword` null, kosong, atau hanya berisi karakter spasi dalam Register.
            string.IsNullOrWhiteSpace(model.ConfirmPassword))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(model.DisplayName) || string.IsNullOrWhiteSpace(model.Username) ||
        // string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.Confi...`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam Register.
        {
            // Memperbarui `model.ErrorMessage` menggunakan memanggil `HttpContext.T` dengan `”auth.error.register_required”` dalam Register.
            model.ErrorMessage = HttpContext.T("auth.error.register_required");
            // Memperbarui `model.Password` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam Register.
            model.Password = string.Empty;
            // Memperbarui `model.ConfirmPassword` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam Register.
            model.ConfirmPassword = string.Empty;
            // Mengembalikan menyiapkan tampilan Razor dengan `model` sebagai nama tampilan atau modelnya kepada pemanggil dalam Register; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return View(model);
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(model.DisplayName) || string.IsNullOrWhiteSpace(model.Username) ||
        // string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.Confi...`; bagian berikut berada di luar batas blok tersebut dalam
        // Register.
        }

        // Memeriksa kebalikan kondisi `string.Equals(model.Password, model.ConfirmPassword, StringComparison.Ordinal)`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam Register.
        if (!string.Equals(model.Password, model.ConfirmPassword, StringComparison.Ordinal))
        // Membuka scope cabang if untuk kondisi `!string.Equals(model.Password, model.ConfirmPassword, StringComparison.Ordinal)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam Register.
        {
            // Memperbarui `model.ErrorMessage` menggunakan memanggil `HttpContext.T` dengan `”auth.error.confirm_mismatch”` dalam Register.
            model.ErrorMessage = HttpContext.T("auth.error.confirm_mismatch");
            // Memperbarui `model.Password` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam Register.
            model.Password = string.Empty;
            // Memperbarui `model.ConfirmPassword` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam Register.
            model.ConfirmPassword = string.Empty;
            // Mengembalikan menyiapkan tampilan Razor dengan `model` sebagai nama tampilan atau modelnya kepada pemanggil dalam Register; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return View(model);
        // Menutup scope cabang if untuk kondisi `!string.Equals(model.Password, model.ConfirmPassword, StringComparison.Ordinal)`; bagian berikut berada di
        // luar batas blok tersebut dalam Register.
        }

        // Menyiapkan variabel lokal `client` untuk nilai client dengan memanggil `_clientFactory.CreateClient` dengan `”Api”`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var client = _clientFactory.CreateClient("Api");
        // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan objek baru bertipe `RegisterRequest` dengan argumen (
        // model.Username.Trim(), model.Password, AuthConstants.PlayerRole, model.DisplayName.Trim()). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var payload = new RegisterRequest(
            // Meneruskan membersihkan karakter tepi pada `model.Username` memakai tanpa argumen sebagai argumen ke konstruktor `RegisterRequest`.
            model.Username.Trim(),
            // Meneruskan `model.Password` (kata sandi masukan yang diperiksa sesuai kebijakan autentikasi) sebagai argumen ke konstruktor `RegisterRequest`.
            model.Password,
            // Meneruskan `AuthConstants.PlayerRole` (nilai pemain role) sebagai argumen ke konstruktor `RegisterRequest`.
            AuthConstants.PlayerRole,
            // Meneruskan membersihkan karakter tepi pada `model.DisplayName` memakai tanpa argumen sebagai argumen ke konstruktor `RegisterRequest`.
            model.DisplayName.Trim());
        // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil tanpa nilai awal pada deklarasi ini. Tipe
        // yang dipakai adalah `HttpResponseMessage`.
        HttpResponseMessage response;
        // Memulai blok try dalam Register; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Register.
        {
            // Memperbarui `response` menggunakan hasil operasi asinkron memanggil `client.PostAsJsonAsync` dengan `”api/v1/auth/register”`, `payload`; await
            // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam Register.
            response = await client.PostAsJsonAsync("api/v1/auth/register", payload);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam Register.
        }
        // Menangani exception `HttpRequestException` melalui variabel dalam Register.
        catch (HttpRequestException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Register.
        {
            // Memperbarui `model.ErrorMessage` menggunakan memanggil `HttpContext.T` dengan `”auth.error.api_unavailable”` dalam Register.
            model.ErrorMessage = HttpContext.T("auth.error.api_unavailable");
            // Memperbarui `model.Password` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam Register.
            model.Password = string.Empty;
            // Memperbarui `model.ConfirmPassword` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam Register.
            model.ConfirmPassword = string.Empty;
            // Mengembalikan menyiapkan tampilan Razor dengan `model` sebagai nama tampilan atau modelnya kepada pemanggil dalam Register; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return View(model);
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam Register.
        }
        // Menangani exception `TaskCanceledException` melalui variabel dalam Register.
        catch (TaskCanceledException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Register.
        {
            // Memperbarui `model.ErrorMessage` menggunakan memanggil `HttpContext.T` dengan `”auth.error.api_unavailable”` dalam Register.
            model.ErrorMessage = HttpContext.T("auth.error.api_unavailable");
            // Memperbarui `model.Password` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam Register.
            model.Password = string.Empty;
            // Memperbarui `model.ConfirmPassword` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam Register.
            model.ConfirmPassword = string.Empty;
            // Mengembalikan menyiapkan tampilan Razor dengan `model` sebagai nama tampilan atau modelnya kepada pemanggil dalam Register; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return View(model);
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam Register.
        }

        // Memeriksa kebalikan kondisi `response.IsSuccessStatusCode`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Register.
        if (!response.IsSuccessStatusCode)
        // Membuka scope cabang if untuk kondisi `!response.IsSuccessStatusCode`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Register.
        {
            // Menyiapkan variabel lokal `error` untuk informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil dengan hasil operasi
            // asinkron memanggil `response.Content.TryReadFromJsonAsync<ErrorResponse>` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread
            // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var error = await response.Content.TryReadFromJsonAsync<ErrorResponse>();
            // Memperbarui `model.ErrorMessage` menggunakan `error?.Message` bila tidak null; jika null gunakan `HttpContext.T(”auth.error.register_failed”)`
            // sebagai nilai pengganti dalam Register.
            model.ErrorMessage = error?.Message ?? HttpContext.T("auth.error.register_failed");
            // Memperbarui `model.Password` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam Register.
            model.Password = string.Empty;
            // Memperbarui `model.ConfirmPassword` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam Register.
            model.ConfirmPassword = string.Empty;
            // Mengembalikan menyiapkan tampilan Razor dengan `model` sebagai nama tampilan atau modelnya kepada pemanggil dalam Register; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return View(model);
        // Menutup scope cabang if untuk kondisi `!response.IsSuccessStatusCode`; bagian berikut berada di luar batas blok tersebut dalam Register.
        }

        // Menyiapkan variabel lokal `created` untuk nilai created dengan hasil operasi asinkron memanggil
        // `response.Content.TryReadFromJsonAsync<RegisterResponse>` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var created = await response.Content.TryReadFromJsonAsync<RegisterResponse>();
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `created is null` dan `string.IsNullOrWhiteSpace(created.AccessToken)`;
        // sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Register.
        if (created is null || string.IsNullOrWhiteSpace(created.AccessToken))
        // Membuka scope cabang if untuk kondisi `created is null || string.IsNullOrWhiteSpace(created.AccessToken)`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam Register.
        {
            // Memperbarui `model.ErrorMessage` menggunakan memanggil `HttpContext.T` dengan `”auth.error.register_response_invalid”` dalam Register.
            model.ErrorMessage = HttpContext.T("auth.error.register_response_invalid");
            // Memperbarui `model.Password` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam Register.
            model.Password = string.Empty;
            // Memperbarui `model.ConfirmPassword` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam Register.
            model.ConfirmPassword = string.Empty;
            // Mengembalikan menyiapkan tampilan Razor dengan `model` sebagai nama tampilan atau modelnya kepada pemanggil dalam Register; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return View(model);
        // Menutup scope cabang if untuk kondisi `created is null || string.IsNullOrWhiteSpace(created.AccessToken)`; bagian berikut berada di luar batas
        // blok tersebut dalam Register.
        }

        // Menjalankan hasil operasi asinkron memanggil `SignInAsync` dengan `created.UserId`, `created.Username`, `created.DisplayName`, `created.Role`,
        // `created.AccessToken`, `created.ExpiresAt`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam Register.
        await SignInAsync(
            // Meneruskan `created.UserId` (identitas akun pengguna yang datanya sedang diproses) sebagai argumen ke `SignInAsync`.
            created.UserId,
            // Meneruskan `created.Username` (nama akun yang dipakai saat autentikasi) sebagai argumen ke `SignInAsync`.
            created.Username,
            // Meneruskan `created.DisplayName` (nilai display nama) sebagai argumen ke `SignInAsync`.
            created.DisplayName,
            // Meneruskan `created.Role` (peran pengguna yang menentukan hak akses) sebagai argumen ke `SignInAsync`.
            created.Role,
            // Meneruskan `created.AccessToken` (nilai akses token) sebagai argumen ke `SignInAsync`.
            created.AccessToken,
            // Meneruskan `created.ExpiresAt` (nilai expires at) sebagai argumen ke `SignInAsync`.
            created.ExpiresAt);

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!string.IsNullOrWhiteSpace(model.ReturnUrl)` dan
        // `Url.IsLocalUrl(model.ReturnUrl)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam Register.
        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam Register.
        {
            // Mengembalikan memanggil `Redirect` dengan `model.ReturnUrl` kepada pemanggil dalam Register; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Redirect(model.ReturnUrl);
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)`; bagian berikut berada di
        // luar batas blok tersebut dalam Register.
        }

        // Mengembalikan mengarahkan browser ke action `”Index”`, `”Home”` setelah pemrosesan selesai kepada pemanggil dalam Register; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return RedirectToAction("Index", "Home");
    // Menutup scope metode Register; bagian berikut berada di luar batas blok tersebut dalam Register.
    }

    // mendaftarkan action untuk metode HTTP POST pada rute (”logout”).
    [HttpPost("logout")]
    // memvalidasi token antiforgery untuk memastikan permintaan formulir membawa token yang sesuai.
    [ValidateAntiForgeryToken]
    // Mendefinisikan metode `Logout` dengan hasil bertipe `Task<IActionResult>`; operasi ini menangani logout. async memungkinkan metode menunggu
    // operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    public async Task<IActionResult> Logout()
    // Membuka scope metode Logout; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Logout.
    {
        // Menjalankan hasil operasi asinkron memanggil `HttpContext.SignOutAsync` dengan `CookieAuthenticationDefaults.AuthenticationScheme`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam Logout.
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        // Mengembalikan mengarahkan browser ke action `nameof(Login)` setelah pemrosesan selesai kepada pemanggil dalam Logout; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return RedirectToAction(nameof(Login));
    // Menutup scope metode Logout; bagian berikut berada di luar batas blok tersebut dalam Logout.
    }

    // Mendefinisikan metode `SignInAsync` dengan hasil bertipe `Task`; operasi ini menangani sign in asinkron. Masukan: Parameter `userId` bertipe
    // `Guid` membawa identitas akun pengguna yang datanya sedang diproses; Parameter `username` bertipe `string` membawa nama akun yang dipakai saat
    // autentikasi; Parameter `displayName` bertipe `string?` membawa nilai display nama; nilai null diizinkan ketika data opsional belum tersedia;
    // Parameter `role` bertipe `string` membawa peran pengguna yang menentukan hak akses; Parameter `accessToken` bertipe `string` membawa nilai akses
    // token; Parameter `expiresAt` bertipe `DateTimeOffset` membawa nilai expires at.
    private Task SignInAsync(
        // Parameter `userId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses.
        Guid userId,
        // Parameter `username` bertipe `string` membawa nama akun yang dipakai saat autentikasi.
        string username,
        // Parameter `displayName` bertipe `string?` membawa nilai display nama; nilai null diizinkan ketika data opsional belum tersedia.
        string? displayName,
        // Parameter `role` bertipe `string` membawa peran pengguna yang menentukan hak akses.
        string role,
        // Parameter `accessToken` bertipe `string` membawa nilai akses token.
        string accessToken,
        // Parameter `expiresAt` bertipe `DateTimeOffset` membawa nilai expires at.
        DateTimeOffset expiresAt)
    // Membuka scope metode SignInAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SignInAsync.
    {
        // Menyiapkan variabel lokal `claims` untuk nilai claims dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var claims = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SignInAsync.
        {
            // Menggunakan objek baru bertipe `Claim` dengan argumen (ClaimTypes.NameIdentifier, userId.ToString()) sebagai bagian ekspresi yang sedang disusun
            // dalam SignInAsync.
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            // Menggunakan objek baru bertipe `Claim` dengan argumen (ClaimTypes.Name, username) sebagai bagian ekspresi yang sedang disusun dalam SignInAsync.
            new Claim(ClaimTypes.Name, username),
            // Menggunakan objek baru bertipe `Claim` dengan argumen (ClaimTypes.Role, role.ToUpperInvariant()) sebagai bagian ekspresi yang sedang disusun
            // dalam SignInAsync.
            new Claim(ClaimTypes.Role, role.ToUpperInvariant()),
            // Menggunakan objek baru bertipe `Claim` dengan argumen (AuthConstants.DisplayNameClaim, string.IsNullOrWhiteSpace(displayName) ? username :
            // displayName) sebagai bagian ekspresi yang sedang disusun dalam SignInAsync.
            new Claim(AuthConstants.DisplayNameClaim, string.IsNullOrWhiteSpace(displayName) ? username : displayName),
            // Menggunakan objek baru bertipe `Claim` dengan argumen (AuthConstants.AccessTokenClaim, accessToken) sebagai bagian ekspresi yang sedang disusun
            // dalam SignInAsync.
            new Claim(AuthConstants.AccessTokenClaim, accessToken)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam SignInAsync.
        };
        // Menyiapkan variabel lokal `principal` untuk nilai principal dengan objek baru bertipe `ClaimsPrincipal` dengan argumen (new
        // ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        // Mengembalikan memanggil `HttpContext.SignInAsync` dengan `CookieAuthenticationDefaults.AuthenticationScheme`, `principal`, `new
        // AuthenticationProperties { IsPersistent = true, AllowRefresh = false, ExpiresUtc = expiresAt.ToUniversalTime() }` kepada pemanggil dalam
        // SignInAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return HttpContext.SignInAsync(
            // Meneruskan `CookieAuthenticationDefaults.AuthenticationScheme` (nilai authentication scheme) sebagai argumen ke `HttpContext.SignInAsync`.
            CookieAuthenticationDefaults.AuthenticationScheme,
            // Meneruskan `principal` (nilai principal) sebagai argumen ke `HttpContext.SignInAsync`.
            principal,
            // Meneruskan objek baru bertipe `AuthenticationProperties` dengan nilai awal sesuai konstruktornya sebagai argumen ke `HttpContext.SignInAsync`.
            new AuthenticationProperties
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SignInAsync.
            {
                // Memperbarui `IsPersistent` menggunakan true, yaitu kondisi aktif/terpenuhi dalam SignInAsync.
                IsPersistent = true,
                // Memperbarui `AllowRefresh` menggunakan false, yaitu kondisi nonaktif/tidak terpenuhi dalam SignInAsync.
                AllowRefresh = false,
                // Memperbarui `ExpiresUtc` menggunakan memanggil `expiresAt.ToUniversalTime` dengan tanpa argumen dalam SignInAsync.
                ExpiresUtc = expiresAt.ToUniversalTime()
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam SignInAsync.
            });
    // Menutup scope metode SignInAsync; bagian berikut berada di luar batas blok tersebut dalam SignInAsync.
    }

// Menutup scope tipe AuthController; bagian berikut berada di luar batas blok tersebut.
}
