// Fungsi file: Menyediakan pemeriksaan role dari principal autentikasi UI.
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Kelas statis berisi extension method untuk membaca identitas pengguna
/// dari principal cookie yang terenkripsi.
/// </summary>
// Mendefinisikan tipe class `AuthContextExtensions`.
public static class AuthContextExtensions
// Membuka scope tipe AuthContextExtensions; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Memeriksa apakah pengguna yang sedang login memiliki peran instruktur.
    /// </summary>
    /// <param name="context">Konteks HTTP aktif.</param>
    /// <returns>True jika peran pengguna adalah instruktur.</returns>
    // Mendefinisikan metode `IsInstructor` dengan hasil bertipe `bool`. Memeriksa apakah pengguna yang sedang login memiliki peran instruktur. Masukan:
    // Parameter `context` bertipe `HttpContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini. Nilai hasil langsung
    // berasal dari membandingkan kesamaan `string` dengan `context.User.FindFirst(ClaimTypes.Role)?.Value`, `AuthConstants.InstructorRole`,
    // `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan comparer yang diberikan.
    public static bool IsInstructor(this HttpContext context) =>
        // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `context.User.FindFirst(ClaimTypes.Role)?.Value`,
        // `AuthConstants.InstructorRole`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan comparer yang diberikan dalam
        // IsInstructor.
        string.Equals(
            // Meneruskan `context.User.FindFirst(ClaimTypes.Role)?.Value`; akses setelah ?. hanya dilakukan bila penerimanya tidak null sebagai argumen ke
            // `string.Equals`; Meneruskan `ClaimTypes.Role` (peran pengguna yang menentukan hak akses) sebagai argumen ke `context.User.FindFirst`.
            context.User.FindFirst(ClaimTypes.Role)?.Value,
            // Meneruskan `AuthConstants.InstructorRole` (nilai instruktur role) sebagai argumen ke `string.Equals`.
            AuthConstants.InstructorRole,
            // Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `string.Equals`.
            StringComparison.OrdinalIgnoreCase);
// Menutup scope tipe AuthContextExtensions; bagian berikut berada di luar batas blok tersebut.
}
