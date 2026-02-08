using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Menyisipkan role login user ke header API agar endpoint bisa membedakan INSTRUCTOR/PLAYER.
/// </summary>
public sealed class RoleHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RoleHeaderHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var role = _httpContextAccessor.HttpContext?.Session.GetString(AuthConstants.SessionRoleKey);
        var normalizedRole = string.IsNullOrWhiteSpace(role)
            ? AuthConstants.PlayerRole
            : role.ToUpperInvariant();

        request.Headers.Remove("X-Actor-Role");
        request.Headers.TryAddWithoutValidation("X-Actor-Role", normalizedRole);

        return base.SendAsync(request, cancellationToken);
    }
}
