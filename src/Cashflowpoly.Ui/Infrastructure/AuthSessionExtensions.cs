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
        string.Equals(
            context.User.FindFirst(ClaimTypes.Role)?.Value,
            AuthConstants.InstructorRole,
            StringComparison.OrdinalIgnoreCase);
}
