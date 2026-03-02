// Fungsi file: Menyediakan extension method pada ISession untuk membaca peran dan status autentikasi pengguna.
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Kelas statis berisi extension method untuk ISession yang mempermudah
/// pengecekan peran pengguna (instruktur atau pemain) dari data sesi.
/// </summary>
public static class AuthSessionExtensions
{
    /// <summary>
    /// Mengambil string peran pengguna saat ini dari sesi (misalnya "instructor" atau "player").
    /// </summary>
    /// <param name="session">Instance sesi HTTP aktif.</param>
    /// <returns>Nama peran pengguna, atau null jika belum ditetapkan.</returns>
    public static string? CurrentRole(this ISession session) =>
        session.GetString(AuthConstants.SessionRoleKey);

    /// <summary>
    /// Memeriksa apakah pengguna yang sedang login memiliki peran instruktur.
    /// </summary>
    /// <param name="session">Instance sesi HTTP aktif.</param>
    /// <returns>True jika peran pengguna adalah instruktur.</returns>
    public static bool IsInstructor(this ISession session) =>
        string.Equals(session.CurrentRole(), AuthConstants.InstructorRole, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Memeriksa apakah pengguna yang sedang login memiliki peran pemain.
    /// </summary>
    /// <param name="session">Instance sesi HTTP aktif.</param>
    /// <returns>True jika peran pengguna adalah pemain.</returns>
    public static bool IsPlayer(this ISession session) =>
        string.Equals(session.CurrentRole(), AuthConstants.PlayerRole, StringComparison.OrdinalIgnoreCase);
}
