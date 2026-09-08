// Fungsi file: Menerapkan kontrol keamanan aplikasi melalui AuthOptions.
// Mengimpor namespace `System.Text` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Security` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Security;

/// <summary>
/// Opsi untuk men-seed akun instruktur dan pemain default saat startup.
/// </summary>
// Mendefinisikan tipe class `AuthBootstrapOptions`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AuthBootstrapOptions
// Membuka scope tipe AuthBootstrapOptions; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `SeedDefaultUsers` bertipe `bool` untuk nilai seed bawaan pengguna; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public bool SeedDefaultUsers { get; set; }
    // Mendefinisikan properti `InstructorUsername` bertipe `string?` untuk nilai instruktur username; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public string? InstructorUsername { get; set; }
    // Mendefinisikan properti `InstructorPassword` bertipe `string?` untuk nilai instruktur password; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public string? InstructorPassword { get; set; }
    // Mendefinisikan properti `PlayerUsername` bertipe `string?` untuk nilai pemain username; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public string? PlayerUsername { get; set; }
    // Mendefinisikan properti `PlayerPassword` bertipe `string?` untuk nilai pemain password; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public string? PlayerPassword { get; set; }
// Menutup scope tipe AuthBootstrapOptions; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `AuthRegistrationOptions`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AuthRegistrationOptions
// Membuka scope tipe AuthRegistrationOptions; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `AllowPublicInstructorRegistration` bertipe `bool` untuk nilai allow public instruktur registration; get menyediakan
    // pembacaan nilai, set mengizinkan penggantian nilai.
    public bool AllowPublicInstructorRegistration { get; set; }

    // Mendefinisikan metode `CanRegisterPublicly` dengan hasil bertipe `bool`; operasi ini menangani can register publicly. Masukan: Parameter `role`
    // bertipe `string` membawa peran pengguna yang menentukan hak akses. Nilai hasil langsung berasal dari gabungan syarat OR: setidaknya satu kondisi
    // wajib benar antara `AllowPublicInstructorRegistration` dan `!string.Equals(role, ”INSTRUCTOR”, StringComparison.OrdinalIgnoreCase)`; sisi kanan
    // diperiksa hanya jika sisi kiri salah.
    public bool CanRegisterPublicly(string role) =>
        // Melanjutkan ekspresi dengan gabungan syarat OR: setidaknya satu kondisi wajib benar antara `AllowPublicInstructorRegistration` dan
        // `!string.Equals(role, ”INSTRUCTOR”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri salah dalam
        // CanRegisterPublicly.
        AllowPublicInstructorRegistration ||
        // Menggunakan kebalikan kondisi `string.Equals(role, ”INSTRUCTOR”, StringComparison.OrdinalIgnoreCase)` sebagai bagian ekspresi yang sedang disusun
        // dalam CanRegisterPublicly.
        !string.Equals(role, "INSTRUCTOR", StringComparison.OrdinalIgnoreCase);
// Menutup scope tipe AuthRegistrationOptions; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// Kebijakan panjang minimum password yang berlaku pada registrasi dan reset.
/// </summary>
// Mendefinisikan tipe class `PasswordPolicy`.
public static class PasswordPolicy
// Membuka scope tipe PasswordPolicy; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `int`: `MinPasswordLength` menyimpan nilai minimum password length dengan nilai awal nilai literal `12`.
    public const int MinPasswordLength = 12;
    // Mendeklarasikan field bertipe `int`: `MaxPasswordUtf8Bytes` menyimpan nilai maksimum password utf 8 bytes dengan nilai awal nilai literal `72`.
    public const int MaxPasswordUtf8Bytes = 72;

    // Mendefinisikan metode `IsWithinBcryptLimit` dengan hasil bertipe `bool`; operasi ini menangani berstatus within bcrypt limit. Masukan: Parameter
    // `password` bertipe `string` membawa kata sandi masukan yang diperiksa sesuai kebijakan autentikasi. Nilai hasil langsung berasal dari pemeriksaan
    // lebih kecil atau sama antara `Encoding.UTF8.GetByteCount(password)` dan `MaxPasswordUtf8Bytes`.
    public static bool IsWithinBcryptLimit(string password) =>
        // Melanjutkan pengolahan dengan memanggil `Encoding.UTF8.GetByteCount` dengan `password` dalam IsWithinBcryptLimit.
        Encoding.UTF8.GetByteCount(password) <= MaxPasswordUtf8Bytes;
// Menutup scope tipe PasswordPolicy; bagian berikut berada di luar batas blok tersebut.
}
