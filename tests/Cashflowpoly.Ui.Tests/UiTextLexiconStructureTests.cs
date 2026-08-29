// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui UiTextLexiconStructureTests.
using System.Text.RegularExpressions;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class UiTextLexiconStructureTests
{
    private static readonly Regex LexiconEntryRegex = new(
        """terms\[\"(?<key>[^\"]+)\"\]\s*=\s*\(\"(?<id>(?:\\.|[^\"])*)\",\s*\"(?<en>(?:\\.|[^\"])*)\"\);""",
        RegexOptions.Compiled);

    private static readonly Regex PlaceholderRegex = new(@"\{[^{}]+\}", RegexOptions.Compiled);

    [Fact]
    public void UiTextLexicon_ShouldBeSplitAcrossDomainFiles()
    {
        var repoRoot = ResolveRepositoryRoot();
        var infrastructureRoot = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Infrastructure");
        var uiTextPath = Path.Combine(infrastructureRoot, "UiText.cs");
        var uiTextLineCount = File.ReadLines(uiTextPath).Count();
        var lexiconFiles = Directory
            .EnumerateFiles(infrastructureRoot, "UiTextLexicon*.cs", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.True(uiTextLineCount < 250, $"UiText.cs masih terlalu besar: {uiTextLineCount} baris.");
        Assert.Contains("UiTextLexicon.Core.cs", lexiconFiles);
        Assert.Contains("UiTextLexicon.Sessions.cs", lexiconFiles);
        Assert.Contains("UiTextLexicon.Players.cs", lexiconFiles);
        Assert.Contains("UiTextLexicon.Rulesets.cs", lexiconFiles);
        Assert.Contains("UiTextLexicon.Rulebook.cs", lexiconFiles);
    }

    [Fact]
    public void BackActions_ShouldRenderArrowPrefix()
    {
        var repoRoot = ResolveRepositoryRoot();
        var viewsRoot = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views");
        var backActionLines = Directory
            .EnumerateFiles(viewsRoot, "*.cshtml", SearchOption.AllDirectories)
            .SelectMany(File.ReadLines)
            .Where(line => line.Contains("back_to", StringComparison.Ordinal) || line.Contains("nav_back_", StringComparison.Ordinal))
            .ToList();

        Assert.Equal(10, backActionLines.Count);
        Assert.All(backActionLines, line =>
        {
            var expectedPrefix = line.Contains("privacy.back_to_top", StringComparison.Ordinal)
                ? "@(\"↑ \" + Context.T("
                : "@(\"<- \" + Context.T(";
            Assert.Contains(expectedPrefix, line, StringComparison.Ordinal);
        });
    }

    [Fact]
    public void UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible()
    {
        var repoRoot = ResolveRepositoryRoot();
        var infrastructureRoot = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Infrastructure");
        var entries = Directory
            .EnumerateFiles(infrastructureRoot, "UiTextLexicon*.cs", SearchOption.TopDirectoryOnly)
            .SelectMany(path => LexiconEntryRegex.Matches(File.ReadAllText(path)))
            .Select(match => new
            {
                Key = match.Groups["key"].Value,
                Id = match.Groups["id"].Value,
                En = match.Groups["en"].Value
            })
            .ToList();

        var duplicateKeys = entries
            .GroupBy(entry => entry.Key, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .OrderBy(key => key, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var incompleteKeys = entries
            .Where(entry => string.IsNullOrWhiteSpace(entry.Id) || string.IsNullOrWhiteSpace(entry.En))
            .Select(entry => entry.Key)
            .ToList();
        var placeholderMismatches = entries
            .Where(entry => !PlaceholderRegex.Matches(entry.Id).Select(match => match.Value)
                .OrderBy(value => value, StringComparer.Ordinal)
                .SequenceEqual(
                    PlaceholderRegex.Matches(entry.En).Select(match => match.Value)
                        .OrderBy(value => value, StringComparer.Ordinal),
                    StringComparer.Ordinal))
            .Select(entry => entry.Key)
            .ToList();

        Assert.True(duplicateKeys.Count == 0, $"Key terjemahan ganda: {string.Join(", ", duplicateKeys)}");
        Assert.True(incompleteKeys.Count == 0, $"Terjemahan kosong: {string.Join(", ", incompleteKeys)}");
        Assert.True(placeholderMismatches.Count == 0, $"Placeholder ID/EN tidak sama: {string.Join(", ", placeholderMismatches)}");
    }

    [Fact]
    public void ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName()
    {
        var infrastructureRoot = Path.Combine(ResolveRepositoryRoot(), "src", "Cashflowpoly.Ui", "Infrastructure");
        var entries = Directory
            .EnumerateFiles(infrastructureRoot, "UiTextLexicon*.cs", SearchOption.TopDirectoryOnly)
            .SelectMany(path => LexiconEntryRegex.Matches(File.ReadAllText(path)));

        foreach (var entry in entries)
        {
            var id = entry.Groups["id"].Value;
            var en = entry.Groups["en"].Value;
            Assert.DoesNotMatch(@"(?i)\bpoin\b(?!\s+kebahagiaan\b)", id);
            Assert.DoesNotMatch(@"(?i)\bpoin\s+kebahagiaan\s+kebahagiaan\b", id);
            Assert.DoesNotMatch(@"(?i)\bhappiness\s+happiness\s+points?\b", en);
            if (id.Contains("poin", StringComparison.OrdinalIgnoreCase))
            {
                Assert.DoesNotMatch(@"(?i)(?<!happiness )\bpoints?\b", en);
            }
        }
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
