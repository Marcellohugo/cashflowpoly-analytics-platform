// Fungsi file: Menyediakan pemeriksaan role dari principal autentikasi UI.
using System.Security.Claims;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Kelas statis berisi extension method untuk membaca identitas pengguna
/// dari principal cookie yang terenkripsi.
/// </summary>
public static class AuthContextExtensions
{
    /// <summary>
    /// Memeriksa apakah pengguna yang sedang login memiliki peran instruktur.
    /// </summary>
    /// <param name="context">Konteks HTTP aktif.</param>
    /// <returns>True jika peran pengguna adalah instruktur.</returns>
    public static bool IsInstructor(this HttpContext context) =>
        string.Equals(
            context.User.FindFirst(ClaimTypes.Role)?.Value,
            AuthConstants.InstructorRole,
            StringComparison.OrdinalIgnoreCase);
}
