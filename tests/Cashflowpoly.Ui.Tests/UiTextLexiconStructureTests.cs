// Fungsi file: Menguji struktur leksikon UI agar kamus terjemahan tidak kembali menjadi satu file besar.
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class UiTextLexiconStructureTests
{
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
