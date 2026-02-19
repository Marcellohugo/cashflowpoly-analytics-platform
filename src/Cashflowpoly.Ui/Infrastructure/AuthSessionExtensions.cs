// Fungsi file: Menyediakan utilitas infrastruktur UI untuk kebutuhan AuthSessionExtensions.
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Menyatakan peran utama tipe AuthSessionExtensions pada modul ini.
/// </summary>
public static class AuthSessionExtensions
{
    /// <summary>
    /// Menjalankan fungsi CurrentRole sebagai bagian dari alur file ini.
    /// </summary>
    public static string? CurrentRole(this ISession session) =>
        session.GetString(AuthConstants.SessionRoleKey);

    /// <summary>
    /// Menjalankan fungsi IsInstructor sebagai bagian dari alur file ini.
    /// </summary>
    public static bool IsInstructor(this ISession session) =>
        string.Equals(session.CurrentRole(), AuthConstants.InstructorRole, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Menjalankan fungsi IsPlayer sebagai bagian dari alur file ini.
    /// </summary>
    public static bool IsPlayer(this ISession session) =>
        string.Equals(session.CurrentRole(), AuthConstants.PlayerRole, StringComparison.OrdinalIgnoreCase);
}
