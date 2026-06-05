using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class RulesetManagementActionTests
{
    private static readonly string RepoRoot = ResolveRepositoryRoot();
    private static readonly string UiRoot = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui");

    [Fact]
    public void RulesetViews_ShouldRenderInstructorMutationActions()
    {
        var indexView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Rulesets", "Index.cshtml"));
        var detailView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Rulesets", "Details.cshtml"));
        var sessionDetailView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Sessions", "Details.cshtml"));

        Assert.Contains("asp-action=\"Create\"", indexView, StringComparison.Ordinal);
        Assert.Contains("asp-action=\"BulkDelete\"", indexView, StringComparison.Ordinal);
        Assert.Contains("asp-action=\"Edit\"", indexView, StringComparison.Ordinal);
        Assert.Contains("asp-action=\"Delete\"", indexView, StringComparison.Ordinal);
        Assert.Contains("asp-action=\"ActivateVersion\"", detailView, StringComparison.Ordinal);
        Assert.Contains("asp-action=\"DeleteVersion\"", detailView, StringComparison.Ordinal);
        Assert.Contains("asp-action=\"Ruleset\"", sessionDetailView, StringComparison.Ordinal);
    }

    [Fact]
    public void RulesetControllers_ShouldCallMutationApiEndpoints()
    {
        var rulesetsController = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "RulesetsController.cs"));
        var sessionsController = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "SessionsController.cs"));

        Assert.Contains("PostAsJsonAsync(\"api/v1/rulesets\"", rulesetsController, StringComparison.Ordinal);
        Assert.Contains("PutAsJsonAsync($\"api/v1/rulesets/{rulesetId}\"", rulesetsController, StringComparison.Ordinal);
        Assert.Contains("PostAsync($\"api/v1/rulesets/{rulesetId}/versions/{version}/activate\"", rulesetsController, StringComparison.Ordinal);
        Assert.Contains("DeleteAsync($\"api/v1/rulesets/{rulesetId}/versions/{version}\"", rulesetsController, StringComparison.Ordinal);
        Assert.Contains("DeleteAsync($\"api/v1/rulesets/{rulesetId}\"", rulesetsController, StringComparison.Ordinal);
        Assert.Contains("PostAsJsonAsync($\"api/v1/sessions/{sessionId}/ruleset/activate\"", sessionsController, StringComparison.Ordinal);
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
