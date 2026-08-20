// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui MenuComponentConsistencyTests.
using System.Text.RegularExpressions;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class MenuComponentConsistencyTests
{
    [Fact]
    public void QuickstartToggle_ShouldReferenceItsControlledPanel()
    {
        var layoutContent = File.ReadAllText(Path.Combine(ResolveRepositoryRoot(), "src", "Cashflowpoly.Ui", "Views", "Shared", "_Layout.cshtml"));

        Assert.Contains("aria-controls=\"quickstart-guide-body\"", layoutContent);
        Assert.Contains("id=\"quickstart-guide-body\"", layoutContent);
    }

    [Theory]
    [InlineData("Home", "Index.cshtml", 2)]
    [InlineData("Sessions", "Index.cshtml", 2)]
    [InlineData("Sessions", "Details.cshtml", 3)]
    [InlineData("Players", "Index.cshtml", 3)]
    [InlineData("Players", "Details.cshtml", 3)]
    [InlineData("Rulesets", "Index.cshtml", 3)]
    [InlineData("Rulesets", "Create.cshtml", 5)]
    [InlineData("Rulesets", "Details.cshtml", 6)]
    public void PrimaryMenuViews_ShouldUseSharedSectionHeaders(string folder, string fileName, int minimumSectionTitles)
    {
        var viewContent = File.ReadAllText(Path.Combine(ResolveRepositoryRoot(), "src", "Cashflowpoly.Ui", "Views", folder, fileName));
        if (folder == "Rulesets" && fileName == "Details.cshtml")
        {
            viewContent += File.ReadAllText(Path.Combine(ResolveRepositoryRoot(), "src", "Cashflowpoly.Ui", "Views", "Shared", "_RulesetDetailContent.cshtml"));
        }

        var sectionHeadCount = Count(viewContent, "ruleset-section-head");
        var sectionTitleCount = Count(viewContent, "ruleset-section-title");
        var sectionSubtitleCount = Count(viewContent, "ruleset-section-subtitle");

        Assert.True(sectionTitleCount >= minimumSectionTitles, $"{folder}/{fileName} should expose at least {minimumSectionTitles} shared section titles but had {sectionTitleCount}.");
        Assert.True(sectionHeadCount >= minimumSectionTitles, $"{folder}/{fileName} should expose at least {minimumSectionTitles} shared section heads but had {sectionHeadCount}.");
        Assert.True(sectionSubtitleCount >= Math.Min(minimumSectionTitles, sectionTitleCount), $"{folder}/{fileName} should keep shared section subtitles aligned with section titles.");
    }

    [Fact]
    public void SessionJourneyPartial_ShouldUseSharedSectionHeader()
    {
        var viewContent = File.ReadAllText(Path.Combine(ResolveRepositoryRoot(), "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneySection.cshtml"));

        Assert.Contains("ruleset-section-head", viewContent, StringComparison.Ordinal);
        Assert.Contains("ruleset-section-title", viewContent, StringComparison.Ordinal);
        Assert.Contains("ruleset-section-subtitle", viewContent, StringComparison.Ordinal);
    }

    private static int Count(string content, string value)
    {
        return Regex.Matches(content, Regex.Escape(value)).Count;
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

        throw new InvalidOperationException("Unable to locate repository root.");
    }
}
