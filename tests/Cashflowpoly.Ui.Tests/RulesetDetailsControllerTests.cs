// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui RulesetDetailsControllerTests.
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class RulesetDetailsControllerTests
{
    [Fact]
    public void RulesetDetailsController_ShouldMakeReadonlyFromApiMetadata()
    {
        var repoRoot = ResolveRepositoryRoot();
        var controllerPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Controllers", "RulesetsController.cs");
        var controllerContent = File.ReadAllText(controllerPath);

        Assert.Contains("data.IsDefault", controllerContent, StringComparison.Ordinal);
        Assert.Contains("data.IsLockedBySession", controllerContent, StringComparison.Ordinal);
        Assert.Contains("fromDefaultCatalog || data.IsDefault || data.IsLockedBySession", controllerContent, StringComparison.Ordinal);
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
