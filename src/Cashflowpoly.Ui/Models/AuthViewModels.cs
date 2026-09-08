// Fungsi file: Mendefinisikan model tampilan dan state UI untuk AuthViewModels.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Models` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// Kelas statis yang menyimpan konstanta cookie/claim autentikasi, definisi peran,
/// dan kunci bahasa UI.
/// </summary>
// Mendefinisikan tipe class `AuthConstants`.
public static class AuthConstants
// Membuka scope tipe AuthConstants; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `AuthenticationCookieName` menyimpan nilai authentication cookie nama dengan nilai awal nilai literal
    // `”.Cashflowpoly.Ui.Auth”`.
    public const string AuthenticationCookieName = ".Cashflowpoly.Ui.Auth";
    // Mendeklarasikan field bertipe `string`: `DisplayNameClaim` menyimpan nilai display nama claim dengan nilai awal nilai literal
    // `”cashflowpoly:display_name”`.
    public const string DisplayNameClaim = "cashflowpoly:display_name";
    // Mendeklarasikan field bertipe `string`: `AccessTokenClaim` menyimpan nilai akses token claim dengan nilai awal nilai literal
    // `”cashflowpoly:access_token”`.
    public const string AccessTokenClaim = "cashflowpoly:access_token";
    // Mendeklarasikan field bertipe `string`: `SessionLanguageKey` menyimpan nilai sesi language kunci dengan nilai awal nilai literal
    // `”current_language”`.
    public const string SessionLanguageKey = "current_language";
    // Mendeklarasikan field bertipe `string`: `InstructorRole` menyimpan nilai instruktur role dengan nilai awal nilai literal `”INSTRUCTOR”`.
    public const string InstructorRole = "INSTRUCTOR";
    // Mendeklarasikan field bertipe `string`: `PlayerRole` menyimpan nilai pemain role dengan nilai awal nilai literal `”PLAYER”`.
    public const string PlayerRole = "PLAYER";
    // Mendeklarasikan field bertipe `string`: `LanguageId` menyimpan nilai language identitas dengan nilai awal nilai literal `”id”`.
    public const string LanguageId = "id";
    // Mendeklarasikan field bertipe `string`: `LanguageEn` menyimpan nilai language en dengan nilai awal nilai literal `”en”`.
    public const string LanguageEn = "en";

    /// <summary>
    /// Memeriksa apakah string peran yang diberikan merupakan peran valid (INSTRUCTOR atau PLAYER), tanpa memperhatikan huruf besar/kecil.
    /// </summary>
    // Mendefinisikan metode `IsValidRole` dengan hasil bertipe `bool`. Memeriksa apakah string peran yang diberikan merupakan peran valid (INSTRUCTOR
    // atau PLAYER), tanpa memperhatikan huruf besar/kecil. Masukan: Parameter `role` bertipe `string?` membawa peran pengguna yang menentukan hak
    // akses; nilai null diizinkan ketika data opsional belum tersedia. Nilai hasil langsung berasal dari gabungan syarat OR: setidaknya satu kondisi
    // wajib benar antara `string.Equals(role, InstructorRole, StringComparison.OrdinalIgnoreCase)` dan `string.Equals(role, PlayerRole,
    // StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri salah.
    public static bool IsValidRole(string? role) =>
        // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `role`, `InstructorRole`, `StringComparison.OrdinalIgnoreCase`; aturan
        // perbandingan mengikuti overload dan comparer yang diberikan dalam IsValidRole.
        string.Equals(role, InstructorRole, StringComparison.OrdinalIgnoreCase) ||
        // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `role`, `PlayerRole`, `StringComparison.OrdinalIgnoreCase`; aturan
        // perbandingan mengikuti overload dan comparer yang diberikan dalam IsValidRole.
        string.Equals(role, PlayerRole, StringComparison.OrdinalIgnoreCase);
// Menutup scope tipe AuthConstants; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// ViewModel formulir login yang menampung nama pengguna, kata sandi, URL kembali, dan pesan error.
/// </summary>
// Mendefinisikan tipe class `LoginViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class LoginViewModel
// Membuka scope tipe LoginViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `Username` bertipe `string` untuk nama akun yang dipakai saat autentikasi; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Username { get; set; } = string.Empty;
    // Mendefinisikan properti `Password` bertipe `string` untuk kata sandi masukan yang diperiksa sesuai kebijakan autentikasi; get menyediakan
    // pembacaan nilai, set mengizinkan penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Password { get; set; } = string.Empty;
    // Mendefinisikan properti `ReturnUrl` bertipe `string?` untuk nilai return url; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai;
    // tanda ? mengizinkan nilai null.
    public string? ReturnUrl { get; set; }
    // Mendefinisikan properti `ErrorMessage` bertipe `string?` untuk nilai kesalahan pesan; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public string? ErrorMessage { get; set; }
// Menutup scope tipe LoginViewModel; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// ViewModel formulir registrasi yang menampung nama tampilan, nama pengguna, kata sandi,
/// konfirmasi kata sandi, URL kembali, dan pesan error.
/// </summary>
// Mendefinisikan tipe class `RegisterViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RegisterViewModel
// Membuka scope tipe RegisterViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `DisplayName` bertipe `string` untuk nilai display nama; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string DisplayName { get; set; } = string.Empty;
    // Mendefinisikan properti `Username` bertipe `string` untuk nama akun yang dipakai saat autentikasi; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Username { get; set; } = string.Empty;
    // Mendefinisikan properti `Password` bertipe `string` untuk kata sandi masukan yang diperiksa sesuai kebijakan autentikasi; get menyediakan
    // pembacaan nilai, set mengizinkan penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Password { get; set; } = string.Empty;
    // Mendefinisikan properti `ConfirmPassword` bertipe `string` untuk nilai confirm password; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string ConfirmPassword { get; set; } = string.Empty;
    // Mendefinisikan properti `ReturnUrl` bertipe `string?` untuk nilai return url; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai;
    // tanda ? mengizinkan nilai null.
    public string? ReturnUrl { get; set; }
    // Mendefinisikan properti `ErrorMessage` bertipe `string?` untuk nilai kesalahan pesan; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public string? ErrorMessage { get; set; }
// Menutup scope tipe RegisterViewModel; bagian berikut berada di luar batas blok tersebut.
}
