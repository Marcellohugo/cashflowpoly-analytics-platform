// Fungsi file: Menguji bahwa semua key terjemahan UI terdaftar di leksikon dan tidak ada teks hardcoded di view/controller.
using System.Text.RegularExpressions;
using Xunit;

namespace Cashflowpoly.Api.Tests;

/// <summary>
/// Kelas pengujian unit yang memastikan semua key terjemahan yang digunakan di UI
/// terdaftar pada UiText, dan tidak ada pesan error atau teks literal hardcoded.
/// </summary>
public sealed class UiLocalizationGuardTests
{
    /// <summary>
    /// Regex untuk mengekstrak key dari dictionary leksikon UiText (pola ["key"] = ...).
    /// </summary>
    private static readonly Regex LexiconKeyRegex = new(@"\[""(?<key>[^""]+)""\]\s*=", RegexOptions.Compiled);
    /// <summary>
    /// Regex untuk mendeteksi pemanggilan Context.T("key") atau HttpContext.T("key") di file C#/Razor.
    /// </summary>
    private static readonly Regex TranslationCallRegex = new(@"(?:Context|HttpContext)\.T\(""(?<key>[^""]+)""\)", RegexOptions.Compiled);
    /// <summary>
    /// Regex untuk mendeteksi node teks literal di dalam elemen HTML pada Razor view.
    /// </summary>
    private static readonly Regex LiteralTextNodeRegex = new(
        @"<[^!/][^>]*>\s*(?<text>[A-Za-z][A-Za-z0-9\s\.,'""/&()\-:]{0,120})\s*</[^>]+>",
        RegexOptions.Compiled);
    /// <summary>
    /// Regex untuk mendeteksi assignment pesan error hardcoded pada variabel controller UI.
    /// </summary>
    private static readonly Regex HardcodedErrorAssignmentRegex = new(
        @"\b(?:ErrorMessage|SessionLookupErrorMessage|RulesetErrorMessage|groupError|gameplayError|timelineErrorMessage|errorMessage)\s*=\s*\$?""",
        RegexOptions.Compiled);
    /// <summary>
    /// Regex untuk mendeteksi assignment string hardcoded ke TempData pada controller UI.
    /// </summary>
    private static readonly Regex HardcodedTempDataAssignmentRegex = new(
        @"TempData\[[^\]]+\]\s*=\s*\$?""",
        RegexOptions.Compiled);

    /// <summary>
    /// Daftar teks literal yang diizinkan muncul langsung di Razor view tanpa terjemahan.
    /// </summary>
    private static readonly HashSet<string> AllowedLiteralViewTextNodes = new(StringComparer.Ordinal)
    {
        "Cashflowpoly",
        "ID",
        "EN"
    };

    /// <summary>
    /// Path root repositori yang ditemukan dengan menelusuri ke atas dari BaseDirectory.
    /// </summary>
    private static readonly string RepoRoot = ResolveRepositoryRoot();

    [Fact]
    /// <summary>
    /// Memvalidasi bahwa setiap key yang dipanggil via Context.T() di controller dan view
    /// UI terdaftar dalam dictionary leksikon UiText.cs.
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
    /// Memvalidasi bahwa controller UI tidak mengandung assignment pesan error
    /// hardcoded yang seharusnya menggunakan key terjemahan.
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
    /// Memvalidasi bahwa Razor view tidak mengandung node teks literal hardcoded
    /// yang seharusnya menggunakan mekanisme terjemahan, kecuali yang diizinkan.
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
    /// Helper yang mencari semua kecocokan regex dalam konten file dan mengembalikan
    /// daftar pelanggaran beserta lokasi baris relatif terhadap root repositori.
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
    /// Helper yang menelusuri direktori ke atas dari AppContext.BaseDirectory
    /// untuk menemukan root repositori berdasarkan keberadaan file Cashflowpoly.sln.
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
