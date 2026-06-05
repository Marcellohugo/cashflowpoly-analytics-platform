using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class ComponentMenuSeparationTests
{
    private static readonly string RepoRoot = ResolveRepositoryRoot();
    private static readonly string UiRoot = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui");

    [Fact]
    public void Layout_ShouldRenderComponentsAsSeparatePrimaryMenu()
    {
        var layout = File.ReadAllText(Path.Combine(UiRoot, "Views", "Shared", "_Layout.cshtml"));

        Assert.Contains("asp-controller=\"Components\"", layout, StringComparison.Ordinal);
        Assert.Contains("data-nav=\"components\"", layout, StringComparison.Ordinal);
        Assert.Contains("nav.components", layout, StringComparison.Ordinal);
    }

    [Fact]
    public void RulesetIndex_ShouldNotRenderDefaultComponentsCatalog()
    {
        var rulesetIndex = File.ReadAllText(Path.Combine(UiRoot, "Views", "Rulesets", "Index.cshtml"));

        Assert.DoesNotContain("rulesets.default_components.title", rulesetIndex, StringComparison.Ordinal);
        Assert.DoesNotContain("DefaultComponentItems", rulesetIndex, StringComparison.Ordinal);
        Assert.DoesNotContain("DefaultComponentDetails", rulesetIndex, StringComparison.Ordinal);
    }

    [Fact]
    public void ComponentsIndex_ShouldRenderDefaultComponentsCatalog()
    {
        var componentsViewPath = Path.Combine(UiRoot, "Views", "Components", "Index.cshtml");
        var componentsControllerPath = Path.Combine(UiRoot, "Controllers", "ComponentsController.cs");

        Assert.True(File.Exists(componentsViewPath), "Views/Components/Index.cshtml harus tersedia.");
        Assert.True(File.Exists(componentsControllerPath), "ComponentsController harus tersedia.");

        var componentsView = File.ReadAllText(componentsViewPath);
        var componentsController = File.ReadAllText(componentsControllerPath);

        Assert.Contains("rulesets.default_components.title", componentsView, StringComparison.Ordinal);
        Assert.Contains("api/v1/rulesets/components/defaults", componentsController, StringComparison.Ordinal);
        Assert.Contains("ComponentCatalogListViewModel", componentsView, StringComparison.Ordinal);
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
