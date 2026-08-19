// Fungsi file: Memverifikasi pengambilan data realtime frontend tetap memakai status dan autentikasi yang benar.
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class FrontendDataFlowTests
{
    private static readonly string RepoRoot = ResolveRepositoryRoot();
    private static readonly string UiRoot = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui");

    [Fact]
    public void HomeRealtimeStats_ShouldCountOnlyStartedSessionsAsActive()
    {
        var controller = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "HomeController.cs"));

        Assert.Contains("string.Equals(s.Status, \"STARTED\"", controller, StringComparison.Ordinal);
        Assert.DoesNotContain("ActiveSessions = sessions.Count(s => string.Equals(s.Status, \"ENDED\"", controller, StringComparison.Ordinal);
        Assert.Contains("TotalRulesets = rulesets.Count(r => string.Equals(r.Status, \"ACTIVE\"", controller, StringComparison.Ordinal);
    }

    [Fact]
    public void Authentication_ShouldUseEncryptedCookieClaimsInsteadOfServerMemorySession()
    {
        var program = File.ReadAllText(Path.Combine(UiRoot, "Program.cs"));
        var bearerHandler = File.ReadAllText(Path.Combine(UiRoot, "Infrastructure", "BearerTokenHandler.cs"));

        Assert.Contains("AddCookie", program, StringComparison.Ordinal);
        Assert.Contains("UseAuthentication", program, StringComparison.Ordinal);
        Assert.Contains("AccessTokenClaim", bearerHandler, StringComparison.Ordinal);
        Assert.DoesNotContain("SessionAccessTokenKey", bearerHandler, StringComparison.Ordinal);
    }

    [Fact]
    public void PlayerDirectory_ShouldBoundRequestsAndReuseEndedSessionAnalytics()
    {
        var controller = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "PlayerDirectoryController.cs"));

        Assert.Contains("new SemaphoreSlim(8)", controller, StringComparison.Ordinal);
        Assert.Contains("analyticsParticipants", controller, StringComparison.Ordinal);
        Assert.Contains("players.error.load_session_details_partial", controller, StringComparison.Ordinal);
    }

    [Fact]
    public void HomeRealtimePolling_ShouldRedirectWhenAuthenticationExpires()
    {
        var view = File.ReadAllText(Path.Combine(UiRoot, "Views", "Home", "Index.cshtml"));

        Assert.Contains("response.redirected && response.url.includes(\"/auth/login\")", view, StringComparison.Ordinal);
        Assert.Contains("window.location.assign(response.url)", view, StringComparison.Ordinal);
    }

    private static string ResolveRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Cashflowpoly.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("Repository root tidak ditemukan.");
    }
}
