// Fungsi file: Menguji perilaku dan kontrak komponen pada domain UiLocalizationGuardTests.
using System.Text.RegularExpressions;
using Xunit;

namespace Cashflowpoly.Api.Tests;

/// <summary>
/// Menyatakan peran utama tipe UiLocalizationGuardTests pada modul ini.
/// </summary>
public sealed class UiLocalizationGuardTests
{
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    private static readonly Regex LexiconKeyRegex = new(@"\[""(?<key>[^""]+)""\]\s*=", RegexOptions.Compiled);
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    private static readonly Regex TranslationCallRegex = new(@"(?:Context|HttpContext)\.T\(""(?<key>[^""]+)""\)", RegexOptions.Compiled);
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    private static readonly Regex LiteralTextNodeRegex = new(
        @"<[^!/][^>]*>\s*(?<text>[A-Za-z][A-Za-z0-9\s\.,'""/&()\-:]{0,120})\s*</[^>]+>",
        RegexOptions.Compiled);
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    private static readonly Regex HardcodedErrorAssignmentRegex = new(
        @"\b(?:ErrorMessage|SessionLookupErrorMessage|RulesetErrorMessage|groupError|gameplayError|timelineErrorMessage|errorMessage)\s*=\s*\$?""",
        RegexOptions.Compiled);
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    private static readonly Regex HardcodedTempDataAssignmentRegex = new(
        @"TempData\[[^\]]+\]\s*=\s*\$?""",
        RegexOptions.Compiled);

    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    private static readonly HashSet<string> AllowedLiteralViewTextNodes = new(StringComparer.Ordinal)
    {
        "Cashflowpoly",
        "ID",
        "EN"
    };

    /// <summary>
    /// Menjalankan fungsi ResolveRepositoryRoot sebagai bagian dari alur file ini.
    /// </summary>
    private static readonly string RepoRoot = ResolveRepositoryRoot();

    [Fact]
    /// <summary>
    /// Menjalankan fungsi TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon sebagai bagian dari alur file ini.
    /// </summary>
    public void TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon()
    {
        var uiRoot = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui");
        var uiTextPath = Path.Combine(uiRoot, "Infrastructure", "UiText.cs");
        var lexiconContent = File.ReadAllText(uiTextPath);
        var lexiconKeys = LexiconKeyRegex
            .Matches(lexiconContent)
            .Select(match => match.Groups["key"].Value)
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var uiFiles = Directory
            .EnumerateFiles(uiRoot, "*.*", SearchOption.AllDirectories)
            .Where(path => path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase));

        var missingKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var filePath in uiFiles)
        {
            var content = File.ReadAllText(filePath);
            foreach (Match match in TranslationCallRegex.Matches(content))
            {
                var key = match.Groups["key"].Value;
                if (!lexiconKeys.Contains(key))
                {
                    missingKeys.Add(key);
                }
            }
        }

        Assert.True(
            missingKeys.Count == 0,
            $"Ditemukan key terjemahan yang tidak ada di UiText: {string.Join(", ", missingKeys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase))}");
    }

    [Fact]
    /// <summary>
    /// Menjalankan fungsi UiControllers_ShouldNotUseHardcodedUserFacingErrorMessages sebagai bagian dari alur file ini.
    /// </summary>
    public void UiControllers_ShouldNotUseHardcodedUserFacingErrorMessages()
    {
        var controllerRoot = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Controllers");
        var controllerFiles = Directory.EnumerateFiles(controllerRoot, "*.cs", SearchOption.TopDirectoryOnly);
        var violations = new List<string>();

        foreach (var filePath in controllerFiles)
        {
            var content = File.ReadAllText(filePath);
            violations.AddRange(FindViolations(filePath, content, HardcodedErrorAssignmentRegex));
            violations.AddRange(FindViolations(filePath, content, HardcodedTempDataAssignmentRegex));
        }

        Assert.True(
            violations.Count == 0,
            $"Ditemukan pesan error hardcoded pada controller UI:{Environment.NewLine}{string.Join(Environment.NewLine, violations)}");
    }

    [Fact]
    /// <summary>
    /// Menjalankan fungsi UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes sebagai bagian dari alur file ini.
    /// </summary>
    public void UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes()
    {
        var viewsRoot = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Views");
        var viewFiles = Directory.EnumerateFiles(viewsRoot, "*.cshtml", SearchOption.AllDirectories);
        var violations = new List<string>();

        foreach (var filePath in viewFiles)
        {
            var content = File.ReadAllText(filePath);
            foreach (Match match in LiteralTextNodeRegex.Matches(content))
            {
                var text = match.Groups["text"].Value.Trim();
                if (string.IsNullOrWhiteSpace(text))
                {
                    continue;
                }

                if (AllowedLiteralViewTextNodes.Contains(text))
                {
                    continue;
                }

                var line = content[..match.Index].Count(ch => ch == '\n') + 1;
                var relativePath = Path.GetRelativePath(RepoRoot, filePath).Replace('\\', '/');
                violations.Add($"{relativePath}:{line} -> \"{text}\"");
            }
        }

        Assert.True(
            violations.Count == 0,
            $"Ditemukan literal text node hardcoded pada Razor view:{Environment.NewLine}{string.Join(Environment.NewLine, violations)}");
    }

    /// <summary>
    /// Menjalankan fungsi FindViolations sebagai bagian dari alur file ini.
    /// </summary>
    private static IEnumerable<string> FindViolations(string filePath, string content, Regex pattern)
    {
        foreach (Match match in pattern.Matches(content))
        {
            var line = content[..match.Index].Count(ch => ch == '\n') + 1;
            yield return $"{Path.GetRelativePath(RepoRoot, filePath).Replace('\\', '/')}:{line}";
        }
    }

    /// <summary>
    /// Menjalankan fungsi ResolveRepositoryRoot sebagai bagian dari alur file ini.
    /// </summary>
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

        throw new DirectoryNotFoundException("Tidak dapat menemukan root repositori (Cashflowpoly.sln).");
    }
}
