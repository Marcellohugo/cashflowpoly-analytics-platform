// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui ComponentMenuSeparationTests.
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class ComponentMenuSeparationTests
{
    private static readonly string RepoRoot = ResolveRepositoryRoot();
    private static readonly string UiRoot = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui");

    [Fact]
    public void Layout_ShouldNotRenderComponentsAsSeparatePrimaryMenu()
    {
        var layout = File.ReadAllText(Path.Combine(UiRoot, "Views", "Shared", "_Layout.cshtml"));

        Assert.DoesNotContain("asp-controller=\"Components\"", layout, StringComparison.Ordinal);
        Assert.DoesNotContain("data-nav=\"components\"", layout, StringComparison.Ordinal);
        Assert.DoesNotContain("nav.components", layout, StringComparison.Ordinal);
    }

    [Fact]
    public void RulesetIndex_ShouldRenderDefaultRulesetsInMainTableOnly()
    {
        var rulesetIndex = File.ReadAllText(Path.Combine(UiRoot, "Views", "Rulesets", "Index.cshtml"));
        var rulesetsController = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "RulesetsController.cs"));

        Assert.DoesNotContain("rulesets.default_components.title", rulesetIndex, StringComparison.Ordinal);
        Assert.DoesNotContain("DefaultComponentItems", rulesetIndex, StringComparison.Ordinal);
        Assert.Contains("item.IsDefault", rulesetIndex, StringComparison.Ordinal);
        Assert.Contains("DefaultCatalogSource", rulesetsController, StringComparison.Ordinal);
        Assert.DoesNotContain("DefaultComponentItems =", rulesetsController, StringComparison.Ordinal);
    }

    [Fact]
    public void ComponentsPage_ShouldBeRemoved()
    {
        var componentsViewPath = Path.Combine(UiRoot, "Views", "Components", "Index.cshtml");
        var componentsControllerPath = Path.Combine(UiRoot, "Controllers", "ComponentsController.cs");

        Assert.False(File.Exists(componentsViewPath), "Views/Components/Index.cshtml harus dihapus.");
        Assert.False(File.Exists(componentsControllerPath), "ComponentsController harus dihapus.");
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
