using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Ui.Infrastructure;

public static class AuthSessionExtensions
{
    public static string? CurrentRole(this ISession session) =>
        session.GetString(AuthConstants.SessionRoleKey);

    public static bool IsInstructor(this ISession session) =>
        string.Equals(session.CurrentRole(), AuthConstants.InstructorRole, StringComparison.OrdinalIgnoreCase);

    public static bool IsPlayer(this ISession session) =>
        string.Equals(session.CurrentRole(), AuthConstants.PlayerRole, StringComparison.OrdinalIgnoreCase);
}
